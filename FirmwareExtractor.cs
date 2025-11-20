using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace iPhoneTool
{
    /// <summary>
    /// Extracts and manages firmware components from IPSW files
    /// Handles extraction of bootloaders, kernels, ramdisks, and firmware files
    /// </summary>
    public class FirmwareExtractor
    {
        private readonly string ipswPath;
        private readonly Action<string> logCallback;
        private string extractPath = string.Empty;

        public class FirmwareComponents
        {
            public string? iBSS { get; set; }           // Initial Boot Stage 2
            public string? iBEC { get; set; }           // iBoot Epoch Change
            public string? RestoreRamdisk { get; set; } // Restore RAM disk
            public string? Kernelcache { get; set; }    // iOS Kernel
            public string? DeviceTree { get; set; }     // Device tree
            public string? RestoreLogo { get; set; }    // Apple logo for restore
            public string? BuildManifest { get; set; }  // Build manifest
            public string? Restore { get; set; }        // Restore.plist
            public string? SystemImage { get; set; }    // Root filesystem
            
            public Dictionary<string, byte[]> ComponentData { get; set; } = new Dictionary<string, byte[]>();
        }

        public FirmwareExtractor(string ipswPath, Action<string> logCallback)
        {
            this.ipswPath = ipswPath;
            this.logCallback = logCallback;
        }

        /// <summary>
        /// Extracts all firmware components from IPSW
        /// </summary>
        public FirmwareComponents ExtractComponents(string productType)
        {
            var components = new FirmwareComponents();

            try
            {
                logCallback("═══════════════════════════════════════════════════════");
                logCallback("           EXTRACTING FIRMWARE COMPONENTS");
                logCallback("═══════════════════════════════════════════════════════");
                logCallback("");

                // Create temporary extraction directory
                extractPath = Path.Combine(Path.GetTempPath(), "iPhoneTool_Restore_" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(extractPath);
                logCallback($"Extract path: {extractPath}");
                logCallback("");

                using (ZipFile zip = new ZipFile(ipswPath))
                {
                    logCallback($"Opening IPSW: {Path.GetFileName(ipswPath)}");
                    logCallback($"Total entries: {zip.Count}");
                    logCallback("");

                    // Extract BuildManifest.plist first (contains file mappings)
                    logCallback("[1/9] Extracting BuildManifest.plist...");
                    components.BuildManifest = ExtractFile(zip, "BuildManifest.plist");
                    if (components.BuildManifest != null)
                    {
                        logCallback($"✓ BuildManifest: {new FileInfo(components.BuildManifest).Length / 1024} KB");
                        components.ComponentData["BuildManifest"] = File.ReadAllBytes(components.BuildManifest);
                    }

                    // Extract Restore.plist
                    logCallback("[2/9] Extracting Restore.plist...");
                    components.Restore = ExtractFile(zip, "Restore.plist");
                    if (components.Restore != null)
                    {
                        logCallback($"✓ Restore.plist: {new FileInfo(components.Restore).Length / 1024} KB");
                    }

                    // Find and extract iBSS (Initial Boot Stage 2)
                    logCallback("[3/9] Extracting iBSS (Initial Bootloader)...");
                    components.iBSS = FindAndExtractComponent(zip, "iBSS", productType);
                    if (components.iBSS != null)
                    {
                        logCallback($"✓ iBSS: {new FileInfo(components.iBSS).Length / 1024} KB");
                        components.ComponentData["iBSS"] = File.ReadAllBytes(components.iBSS);
                    }

                    // Find and extract iBEC (iBoot Epoch Change)
                    logCallback("[4/9] Extracting iBEC (Secondary Bootloader)...");
                    components.iBEC = FindAndExtractComponent(zip, "iBEC", productType);
                    if (components.iBEC != null)
                    {
                        logCallback($"✓ iBEC: {new FileInfo(components.iBEC).Length / 1024} KB");
                        components.ComponentData["iBEC"] = File.ReadAllBytes(components.iBEC);
                    }

                    // Find and extract DeviceTree
                    logCallback("[5/9] Extracting DeviceTree...");
                    components.DeviceTree = FindAndExtractComponent(zip, "DeviceTree", productType);
                    if (components.DeviceTree != null)
                    {
                        logCallback($"✓ DeviceTree: {new FileInfo(components.DeviceTree).Length / 1024} KB");
                        components.ComponentData["DeviceTree"] = File.ReadAllBytes(components.DeviceTree);
                    }

                    // Find and extract Kernelcache
                    logCallback("[6/9] Extracting Kernelcache...");
                    components.Kernelcache = FindAndExtractComponent(zip, "kernelcache", productType);
                    if (components.Kernelcache != null)
                    {
                        logCallback($"✓ Kernelcache: {new FileInfo(components.Kernelcache).Length / 1024} KB");
                        components.ComponentData["KernelCache"] = File.ReadAllBytes(components.Kernelcache);
                    }

                    // Find and extract Restore Ramdisk
                    logCallback("[7/9] Extracting Restore Ramdisk...");
                    components.RestoreRamdisk = FindAndExtractComponent(zip, "RestoreRamDisk", productType);
                    if (components.RestoreRamdisk == null)
                    {
                        // Try alternative names
                        components.RestoreRamdisk = FindAndExtractComponent(zip, "ramdisk", productType);
                    }
                    if (components.RestoreRamdisk != null)
                    {
                        logCallback($"✓ RestoreRamdisk: {new FileInfo(components.RestoreRamdisk).Length / 1024} KB");
                        components.ComponentData["RestoreRamDisk"] = File.ReadAllBytes(components.RestoreRamdisk);
                    }
                    else
                    {
                        logCallback("  ℹ RestoreRamdisk not required for this restore method");
                    }

                    // Find and extract Restore Logo
                    logCallback("[8/9] Extracting Restore Logo...");
                    components.RestoreLogo = FindAndExtractComponent(zip, "RestoreLogo", productType);
                    if (components.RestoreLogo == null)
                    {
                        // Try alternative names
                        components.RestoreLogo = FindAndExtractComponent(zip, "applelogo", productType);
                    }
                    if (components.RestoreLogo != null)
                    {
                        logCallback($"✓ RestoreLogo: {new FileInfo(components.RestoreLogo).Length / 1024} KB");
                        components.ComponentData["RestoreLogo"] = File.ReadAllBytes(components.RestoreLogo);
                    }
                    else
                    {
                        logCallback("  ℹ RestoreLogo not required for this restore method");
                    }

                    // Find root filesystem (largest .dmg file)
                    logCallback("[9/9] Locating Root Filesystem...");
                    components.SystemImage = FindRootFilesystem(zip);
                    if (components.SystemImage != null)
                    {
                        logCallback($"✓ Root Filesystem: {Path.GetFileName(components.SystemImage)}");
                        // Note: We don't load the entire filesystem into memory (it's several GB)
                    }
                }

                logCallback("");
                logCallback("═══════════════════════════════════════════════════════");
                logCallback($"✓ Extracted {components.ComponentData.Count} components");
                logCallback("═══════════════════════════════════════════════════════");
                logCallback("");

                return components;
            }
            catch (Exception ex)
            {
                logCallback($"✗ Error extracting components: {ex.Message}");
                logCallback($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Finds and extracts a specific firmware component by name pattern
        /// </summary>
        private string? FindAndExtractComponent(ZipFile zip, string componentName, string productType)
        {
            try
            {
                // Search patterns for the component
                var searchPatterns = new List<string>
                {
                    $"{componentName}.{productType}",
                    $"{componentName}.*{productType}",
                    componentName
                };

                foreach (ZipEntry entry in zip)
                {
                    string entryName = entry.Name;
                    
                    // Check if entry matches any search pattern
                    foreach (var pattern in searchPatterns)
                    {
                        if (entryName.Contains(pattern, StringComparison.OrdinalIgnoreCase) ||
                            entryName.Contains(componentName, StringComparison.OrdinalIgnoreCase))
                        {
                            // Extract the file
                            string outputPath = Path.Combine(extractPath, Path.GetFileName(entryName));
                            
                            using (var inputStream = zip.GetInputStream(entry))
                            using (var outputStream = File.Create(outputPath))
                            {
                                inputStream.CopyTo(outputStream);
                            }

                            logCallback($"  Found: {entryName}");
                            return outputPath;
                        }
                    }
                }

                logCallback($"  ⚠ Not found: {componentName}");
                return null;
            }
            catch (Exception ex)
            {
                logCallback($"  ✗ Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Extracts a specific file from IPSW
        /// </summary>
        private string? ExtractFile(ZipFile zip, string fileName)
        {
            try
            {
                var entry = zip.GetEntry(fileName);
                if (entry != null)
                {
                    string outputPath = Path.Combine(extractPath, fileName);
                    
                    using (var inputStream = zip.GetInputStream(entry))
                    using (var outputStream = File.Create(outputPath))
                    {
                        inputStream.CopyTo(outputStream);
                    }

                    return outputPath;
                }

                logCallback($"  ⚠ Not found: {fileName}");
                return null;
            }
            catch (Exception ex)
            {
                logCallback($"  ✗ Error extracting {fileName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Finds the root filesystem DMG (usually the largest .dmg file)
        /// </summary>
        private string? FindRootFilesystem(ZipFile zip)
        {
            try
            {
                ZipEntry? largestDmg = null;
                long largestSize = 0;

                foreach (ZipEntry entry in zip)
                {
                    if (entry.Name.EndsWith(".dmg", StringComparison.OrdinalIgnoreCase) &&
                        !entry.Name.Contains("Ramdisk", StringComparison.OrdinalIgnoreCase) &&
                        !entry.Name.Contains("Update", StringComparison.OrdinalIgnoreCase))
                    {
                        if (entry.Size > largestSize)
                        {
                            largestSize = entry.Size;
                            largestDmg = entry;
                        }
                    }
                }

                if (largestDmg != null)
                {
                    logCallback($"  Found: {largestDmg.Name} ({largestSize / (1024 * 1024)} MB)");
                    return largestDmg.Name;
                }

                return null;
            }
            catch (Exception ex)
            {
                logCallback($"  ✗ Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Cleans up extracted files
        /// </summary>
        public void Cleanup()
        {
            try
            {
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                    logCallback("✓ Cleaned up temporary files");
                }
            }
            catch (Exception ex)
            {
                logCallback($"⚠ Warning: Could not clean up temp files: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the extract path for other operations
        /// </summary>
        public string GetExtractPath()
        {
            return extractPath;
        }
    }
}
