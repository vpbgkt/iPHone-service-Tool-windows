using System.Diagnostics;
using System.IO;

namespace iPhoneTool;

/// <summary>
/// Helper to use command-line tools for recovery mode operations
/// </summary>
public class CommandLineHelper
{
    public static bool TryExitRecoveryUsingTools(string udid = "")
    {
        // Try multiple tools in order of preference
        
        // 1. Try ideviceenterrecovery with "exit" mode (if installed)
        if (TryTool("ideviceenterrecovery", udid, "--exit"))
            return true;
            
        // 2. Try irecovery (if available in libs folder)
        if (TryIRecovery())
            return true;
            
        return false;
    }

    private static bool TryTool(string toolName, string udid, string args)
    {
        try
        {
            // Check in PATH
            if (ExecuteCommand(toolName, $"{args} {udid}"))
                return true;
                
            // Check in libs folder
            string libPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libs", $"{toolName}.exe");
            if (File.Exists(libPath))
            {
                return ExecuteCommand(libPath, $"{args} {udid}");
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryIRecovery()
    {
        try
        {
            // Check for irecovery in libs
            string irecoveryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libs", "irecovery.exe");
            
            if (!File.Exists(irecoveryPath))
            {
                Console.WriteLine($"irecovery.exe not found at: {irecoveryPath}");
                return false;
            }
            
            Console.WriteLine($"Found irecovery at: {irecoveryPath}");
            
            // Method 1: Simple reboot command
            if (ExecuteCommand(irecoveryPath, "-c \"reboot\""))
            {
                Console.WriteLine("Reboot command sent successfully");
                return true;
            }
            
            // Method 2: Set auto-boot and reboot
            ExecuteCommand(irecoveryPath, "-c \"setenv auto-boot true\"");
            System.Threading.Thread.Sleep(500);
            ExecuteCommand(irecoveryPath, "-c \"saveenv\"");
            System.Threading.Thread.Sleep(500);
            bool result = ExecuteCommand(irecoveryPath, "-c \"reboot\"");
            
            if (result)
            {
                Console.WriteLine("Full sequence completed successfully");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in TryIRecovery: {ex.Message}");
            return false;
        }
    }

    private static bool ExecuteCommand(string command, string arguments)
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (Process? process = Process.Start(psi))
            {
                if (process == null)
                {
                    Console.WriteLine($"Failed to start process: {command}");
                    return false;
                }

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                
                process.WaitForExit(10000); // 10 second timeout
                
                Console.WriteLine($"Command: {command} {arguments}");
                Console.WriteLine($"Exit code: {process.ExitCode}");
                Console.WriteLine($"Output: {output}");
                if (!string.IsNullOrEmpty(error))
                    Console.WriteLine($"Error: {error}");
                
                return process.ExitCode == 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception executing command: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Check if any recovery tools are available
    /// </summary>
    public static string GetAvailableTools()
    {
        List<string> available = new List<string>();
        
        string libsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libs");
        
        string[] tools = { "irecovery.exe", "ideviceenterrecovery.exe", "idevicerestore.exe" };
        
        foreach (var tool in tools)
        {
            string fullPath = Path.Combine(libsPath, tool);
            if (File.Exists(fullPath))
            {
                available.Add(Path.GetFileNameWithoutExtension(tool));
            }
        }
        
        if (available.Count > 0)
            return "Available: " + string.Join(", ", available);
        else
            return "No command-line tools found in libs folder";
    }
}
