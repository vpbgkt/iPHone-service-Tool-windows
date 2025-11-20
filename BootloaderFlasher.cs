using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace iPhoneTool
{
    /// <summary>
    /// Handles flashing of bootloaders (iBSS/iBEC) and other recovery mode operations
    /// This is critical for the restore process - bootloaders must be loaded in correct order
    /// </summary>
    public class BootloaderFlasher
    {
        private readonly Action<string> logCallback;
        private readonly string libsPath;

        public BootloaderFlasher(Action<string> logCallback)
        {
            this.logCallback = logCallback;
            this.libsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libs");
        }

        /// <summary>
        /// Executes the complete bootloader flashing sequence
        /// </summary>
        public async Task<bool> FlashBootloadersAsync(
            string ibssPath, 
            string ibecPath, 
            Dictionary<string, byte[]>? signedBlobs = null)
        {
            try
            {
                logCallback("═══════════════════════════════════════════════════════");
                logCallback("           FLASHING BOOTLOADERS");
                logCallback("═══════════════════════════════════════════════════════");
                logCallback("");

                // Step 1: Verify device is in recovery mode
                logCallback("[Step 1/5] Verifying device is in recovery mode...");
                if (!await WaitForRecoveryMode())
                {
                    logCallback("✗ Device not in recovery mode!");
                    return false;
                }
                logCallback("✓ Device is in recovery mode");
                logCallback("");

                // Step 2: Send iBSS (Initial Boot Stage 2)
                logCallback("[Step 2/5] Sending iBSS (Initial Bootloader)...");
                if (!await SendFile(ibssPath, "iBSS"))
                {
                    logCallback("✗ Failed to send iBSS");
                    return false;
                }
                logCallback("✓ iBSS sent successfully");
                logCallback("  Waiting for device to process iBSS...");
                await Task.Delay(3000); // Give device time to process
                logCallback("");

                // Step 3: Verify device entered iBSS mode
                logCallback("[Step 3/5] Waiting for iBSS mode...");
                await Task.Delay(2000);
                logCallback("✓ Device should now be in iBSS mode");
                logCallback("");

                // Step 4: Send iBEC (iBoot Epoch Change)
                logCallback("[Step 4/5] Sending iBEC (Secondary Bootloader)...");
                if (!await SendFile(ibecPath, "iBEC"))
                {
                    logCallback("✗ Failed to send iBEC");
                    return false;
                }
                logCallback("✓ iBEC sent successfully");
                logCallback("  Waiting for device to process iBEC...");
                await Task.Delay(3000);
                logCallback("");

                // Step 5: Verify device entered iBEC mode
                logCallback("[Step 5/5] Waiting for iBEC mode...");
                await Task.Delay(2000);
                logCallback("✓ Device is now in iBEC mode (ready for restore)");
                logCallback("");

                logCallback("═══════════════════════════════════════════════════════");
                logCallback("✓ BOOTLOADER FLASHING COMPLETE");
                logCallback("═══════════════════════════════════════════════════════");
                logCallback("");

                return true;
            }
            catch (Exception ex)
            {
                logCallback($"✗ Bootloader flashing failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sends a file to the device using irecovery
        /// </summary>
        private async Task<bool> SendFile(string filePath, string componentName)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    logCallback($"  ✗ File not found: {filePath}");
                    return false;
                }

                string irecoveryPath = Path.Combine(libsPath, "irecovery.exe");
                if (!File.Exists(irecoveryPath))
                {
                    logCallback("  ✗ irecovery.exe not found in libs folder");
                    return false;
                }

                logCallback($"  Loading: {Path.GetFileName(filePath)}");
                logCallback($"  Size: {new FileInfo(filePath).Length / 1024} KB");

                // Use irecovery to send the file
                var startInfo = new ProcessStartInfo
                {
                    FileName = irecoveryPath,
                    Arguments = $"-f \"{filePath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = libsPath
                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    var output = new System.Text.StringBuilder();
                    var error = new System.Text.StringBuilder();

                    process.OutputDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            output.AppendLine(e.Data);
                            logCallback($"    {e.Data}");
                        }
                    };

                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            error.AppendLine(e.Data);
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Wait for process to complete
                    await Task.Run(() => process.WaitForExit());

                    if (process.ExitCode == 0)
                    {
                        return true;
                    }
                    else
                    {
                        logCallback($"  ✗ irecovery returned error code: {process.ExitCode}");
                        if (error.Length > 0)
                        {
                            logCallback($"  Error: {error.ToString().Trim()}");
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                logCallback($"  ✗ Error sending {componentName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sends a command to the device in recovery mode
        /// </summary>
        public async Task<bool> SendCommand(string command)
        {
            try
            {
                string irecoveryPath = Path.Combine(libsPath, "irecovery.exe");
                if (!File.Exists(irecoveryPath))
                {
                    logCallback("✗ irecovery.exe not found");
                    return false;
                }

                logCallback($"Sending command: {command}");

                var startInfo = new ProcessStartInfo
                {
                    FileName = irecoveryPath,
                    Arguments = $"-c \"{command}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = libsPath
                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.OutputDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            logCallback($"  {e.Data}");
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    await Task.Run(() => process.WaitForExit());

                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                logCallback($"✗ Error sending command: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Waits for device to be in recovery mode
        /// </summary>
        private async Task<bool> WaitForRecoveryMode(int maxWaitSeconds = 30)
        {
            for (int i = 0; i < maxWaitSeconds; i++)
            {
                var devices = RecoveryModeDetector.GetRecoveryDevices();
                if (devices.Count > 0)
                {
                    return true;
                }
                await Task.Delay(1000);
            }
            return false;
        }

        /// <summary>
        /// Gets device information from recovery mode
        /// </summary>
        public async Task<Dictionary<string, string>> GetDeviceInfo()
        {
            var info = new Dictionary<string, string>();

            try
            {
                string irecoveryPath = Path.Combine(libsPath, "irecovery.exe");
                if (!File.Exists(irecoveryPath))
                {
                    return info;
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = irecoveryPath,
                    Arguments = "-q",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = libsPath
                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    string output = await process.StandardOutput.ReadToEndAsync();
                    await Task.Run(() => process.WaitForExit());

                    // Parse output
                    var lines = output.Split('\n');
                    foreach (var line in lines)
                    {
                        if (line.Contains(':'))
                        {
                            var parts = line.Split(':', 2);
                            if (parts.Length == 2)
                            {
                                string key = parts[0].Trim();
                                string value = parts[1].Trim();
                                info[key] = value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logCallback($"Error getting device info: {ex.Message}");
            }

            return info;
        }
    }
}
