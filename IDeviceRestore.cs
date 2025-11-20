using System;
using System.Runtime.InteropServices;

namespace iPhoneTool
{
    /// <summary>
    /// P/Invoke wrapper for libidevicerestore.dll (from 3uTools)
    /// This provides direct access to recovery mode operations
    /// </summary>
    public static class IDeviceRestore
    {
        private const string DllName = "libs\\libidevicerestore.dll";
        
        // Recovery client structure
        [StructLayout(LayoutKind.Sequential)]
        public struct irecv_client_t
        {
            public IntPtr handle;
        }
        
        // Error codes
        public enum irecv_error_t
        {
            IRECV_E_SUCCESS = 0,
            IRECV_E_NO_DEVICE = -1,
            IRECV_E_OUT_OF_MEMORY = -2,
            IRECV_E_UNABLE_TO_CONNECT = -3,
            IRECV_E_INVALID_INPUT = -4,
            IRECV_E_FILE_NOT_FOUND = -5,
            IRECV_E_USB_UPLOAD = -6,
            IRECV_E_USB_STATUS = -7,
            IRECV_E_USB_INTERFACE = -8,
            IRECV_E_USB_CONFIGURATION = -9,
            IRECV_E_PIPE = -10,
            IRECV_E_TIMEOUT = -11,
            IRECV_E_UNSUPPORTED = -254,
            IRECV_E_UNKNOWN_ERROR = -255
        }
        
        // Try to import functions from libidevicerestore.dll
        // These are common function names used in irecovery/libidevicerestore
        
        /// <summary>
        /// Opens a connection to a device in recovery mode
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern irecv_error_t irecv_open_with_ecid(out IntPtr client, ulong ecid);
        
        /// <summary>
        /// Opens a connection to any recovery mode device
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern irecv_error_t irecv_open(out IntPtr client);
        
        /// <summary>
        /// Closes the recovery client connection
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern irecv_error_t irecv_close(IntPtr client);
        
        /// <summary>
        /// Sends a command to the device
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern irecv_error_t irecv_send_command(IntPtr client, string command);
        
        /// <summary>
        /// Sets an environment variable on the device
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern irecv_error_t irecv_setenv(IntPtr client, string variable, string value);
        
        /// <summary>
        /// Saves environment variables
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern irecv_error_t irecv_saveenv(IntPtr client);
        
        /// <summary>
        /// Reboots the device from recovery mode
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern irecv_error_t irecv_reboot(IntPtr client);
        
        /// <summary>
        /// High-level function to exit recovery mode
        /// </summary>
        public static bool ExitRecoveryMode()
        {
            IntPtr client = IntPtr.Zero;
            
            try
            {
                Console.WriteLine("Attempting to connect to recovery mode device...");
                
                // Try to open connection to recovery device
                irecv_error_t result = irecv_open(out client);
                
                if (result != irecv_error_t.IRECV_E_SUCCESS)
                {
                    Console.WriteLine($"Failed to connect: {result}");
                    return false;
                }
                
                Console.WriteLine("Connected successfully!");
                
                // Method 1: Try simple reboot
                Console.WriteLine("Sending reboot command...");
                result = irecv_reboot(client);
                
                if (result == irecv_error_t.IRECV_E_SUCCESS)
                {
                    Console.WriteLine("Reboot command sent successfully!");
                    return true;
                }
                
                // Method 2: Set auto-boot and reboot
                Console.WriteLine("Trying auto-boot method...");
                result = irecv_setenv(client, "auto-boot", "true");
                
                if (result == irecv_error_t.IRECV_E_SUCCESS)
                {
                    result = irecv_saveenv(client);
                    if (result == irecv_error_t.IRECV_E_SUCCESS)
                    {
                        result = irecv_reboot(client);
                        if (result == irecv_error_t.IRECV_E_SUCCESS)
                        {
                            Console.WriteLine("Device should be rebooting now!");
                            return true;
                        }
                    }
                }
                
                // Method 3: Send command strings directly
                Console.WriteLine("Trying command strings...");
                result = irecv_send_command(client, "setenv auto-boot true");
                System.Threading.Thread.Sleep(100);
                result = irecv_send_command(client, "saveenv");
                System.Threading.Thread.Sleep(100);
                result = irecv_send_command(client, "reboot");
                
                Console.WriteLine($"Command result: {result}");
                return result == irecv_error_t.IRECV_E_SUCCESS;
            }
            catch (DllNotFoundException ex)
            {
                Console.WriteLine($"DLL not found: {ex.Message}");
                Console.WriteLine("Make sure libidevicerestore.dll and its dependencies are in the libs folder");
                return false;
            }
            catch (EntryPointNotFoundException ex)
            {
                Console.WriteLine($"Function not found in DLL: {ex.Message}");
                Console.WriteLine("The DLL may not have the expected functions");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
            finally
            {
                // Always close the connection
                if (client != IntPtr.Zero)
                {
                    try
                    {
                        irecv_close(client);
                        Console.WriteLine("Connection closed");
                    }
                    catch
                    {
                        // Ignore errors on close
                    }
                }
            }
        }
    }
}
