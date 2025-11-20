using System;
using System.Diagnostics;
using System.IO;

namespace iPhoneTool
{
    /// <summary>
    /// Helper class for using libimobiledevice command-line tools
    /// </summary>
    public static class LibimobiledeviceHelper
    {
        private static readonly string LibsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libs");
        
        /// <summary>
        /// Attempts to exit recovery mode using ideviceenterrecovery with special parameter
        /// This uses libimobiledevice's built-in recovery exit functionality
        /// </summary>
        public static bool ExitRecoveryMode()
        {
            try
            {
                Console.WriteLine("=== Using libimobiledevice ideviceenterrecovery ===");
                Console.WriteLine("");
                
                // First, check if device is visible in recovery
                string irecoveryPath = Path.Combine(LibsPath, "irecovery.exe");
                if (!File.Exists(irecoveryPath))
                {
                    Console.WriteLine($"✗ irecovery.exe not found at: {irecoveryPath}");
                    return false;
                }
                
                Console.WriteLine("Checking device in recovery mode...");
                
                var checkProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = irecoveryPath,
                        Arguments = "-m",  // Get mode
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = LibsPath
                    }
                };
                
                checkProcess.Start();
                string checkOutput = checkProcess.StandardOutput.ReadToEnd();
                string checkError = checkProcess.StandardError.ReadToEnd();
                checkProcess.WaitForExit();
                
                if (checkProcess.ExitCode != 0)
                {
                    Console.WriteLine("✗ Device not found in recovery mode");
                    if (!string.IsNullOrEmpty(checkError))
                    {
                        Console.WriteLine($"   Error: {checkError.Trim()}");
                    }
                    return false;
                }
                
                Console.WriteLine($"✓ Device found: {checkOutput.Trim()}");
                Console.WriteLine("");
                Console.WriteLine("Sending 'setenv auto-boot true' command...");
                
                // Set auto-boot to true
                var setenvProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = irecoveryPath,
                        Arguments = "-c \"setenv auto-boot true\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = LibsPath
                    }
                };
                
                setenvProcess.Start();
                string setenvOutput = setenvProcess.StandardOutput.ReadToEnd();
                string setenvError = setenvProcess.StandardError.ReadToEnd();
                setenvProcess.WaitForExit();
                
                if (!string.IsNullOrEmpty(setenvOutput))
                {
                    Console.WriteLine($"   Output: {setenvOutput.Trim()}");
                }
                
                Console.WriteLine("Saving environment...");
                
                // Save environment
                var saveenvProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = irecoveryPath,
                        Arguments = "-c \"saveenv\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = LibsPath
                    }
                };
                
                saveenvProcess.Start();
                saveenvProcess.StandardOutput.ReadToEnd();
                saveenvProcess.StandardError.ReadToEnd();
                saveenvProcess.WaitForExit();
                
                Console.WriteLine("Sending reboot command...");
                
                // Reboot device
                var rebootProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = irecoveryPath,
                        Arguments = "-c \"reboot\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = LibsPath
                    }
                };
                
                rebootProcess.Start();
                string rebootOutput = rebootProcess.StandardOutput.ReadToEnd();
                string rebootError = rebootProcess.StandardError.ReadToEnd();
                rebootProcess.WaitForExit();
                
                if (rebootProcess.ExitCode == 0)
                {
                    Console.WriteLine("✓✓✓ Commands sent successfully! ✓✓✓");
                    Console.WriteLine("Device should be exiting recovery mode...");
                    return true;
                }
                else
                {
                    Console.WriteLine($"✗ Reboot failed with exit code: {rebootProcess.ExitCode}");
                    if (!string.IsNullOrEmpty(rebootError))
                    {
                        Console.WriteLine($"   Error: {rebootError.Trim()}");
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Exception: {ex.Message}");
                return false;
            }
        }
    }
}
