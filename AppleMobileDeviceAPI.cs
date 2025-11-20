using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace iPhoneTool
{
    public static class AppleMobileDeviceAPI
    {
        // DllImports and delegate definitions
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool SetDllDirectory(string lpPathName);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DeviceNotificationCallback(IntPtr device);

        [DllImport("MobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int AMRestoreRegisterForDeviceNotifications(
            DeviceNotificationCallback? dfu_connect,
            DeviceNotificationCallback? recovery_connect,
            DeviceNotificationCallback? dfu_disconnect,
            DeviceNotificationCallback? recovery_disconnect,
            int unknown,
            IntPtr user_info);

        // Corrected signature: Unregister likely takes one callback at a time.
        [DllImport("MobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int AMRestoreUnregisterForDeviceNotifications(DeviceNotificationCallback callback);


        [DllImport("MobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int AMRecoveryModeDeviceSetAutoBoot(IntPtr device, bool autoBoot);

        [DllImport("MobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int AMRecoveryModeDeviceReboot(IntPtr device);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        private static bool isInitialized = false;
        private static IntPtr recoveryDevice = IntPtr.Zero;
        
        // Keep static references to the delegates to prevent them from being garbage collected
        private static DeviceNotificationCallback? recoveryConnectCallback;
        private static DeviceNotificationCallback? recoveryDisconnectCallback;

        private static IntPtr mobileDeviceLibraryHandle = IntPtr.Zero;

        public static void Initialize()
        {
            if (isInitialized) return;

            Console.WriteLine("Initializing Apple Mobile Device API...");
            if (LoadAppleLibraries())
            {
                // Assign delegates to static fields
                recoveryConnectCallback = new DeviceNotificationCallback(OnRecoveryDeviceConnected);
                recoveryDisconnectCallback = new DeviceNotificationCallback(OnRecoveryDeviceDisconnected);

                // Register only for the recovery mode notifications we need.
                int result = AMRestoreRegisterForDeviceNotifications(
                    null, 
                    recoveryConnectCallback, 
                    null, 
                    recoveryDisconnectCallback, 
                    0, 
                    IntPtr.Zero);

                if (result == 0)
                {
                    Console.WriteLine("✓ Successfully registered for device notifications.");
                    isInitialized = true;
                }
                else
                {
                    Console.WriteLine($"✗ Failed to register for notifications: {result}");
                    FreeAllLibraries();
                }
            }
        }

        public static void Shutdown()
        {
            if (!isInitialized) return;

            Console.WriteLine("Shutting down Apple Mobile Device API...");
            // Unregister callbacks individually.
            if (recoveryConnectCallback != null)
            {
                AMRestoreUnregisterForDeviceNotifications(recoveryConnectCallback);
            }
            if (recoveryDisconnectCallback != null)
            {
                AMRestoreUnregisterForDeviceNotifications(recoveryDisconnectCallback);
            }
            Console.WriteLine("✓ Unregistered device notifications.");

            FreeAllLibraries();
            isInitialized = false;
            Console.WriteLine("✓ API Shutdown complete.");
        }

        private static void OnRecoveryDeviceConnected(IntPtr device)
        {
            recoveryDevice = device;
            Console.WriteLine("✓ Recovery mode device connected.");
        }

        private static void OnRecoveryDeviceDisconnected(IntPtr device)
        {
            if (device == recoveryDevice)
            {
                recoveryDevice = IntPtr.Zero;
                Console.WriteLine("✓ Recovery mode device disconnected.");
            }
        }

        public static bool ExitRecoveryMode()
        {
            if (!isInitialized)
            {
                Console.WriteLine("✗ API not initialized. Please restart the application.");
                return false;
            }
            
            if (recoveryDevice == IntPtr.Zero)
            {
                Console.WriteLine("✗ No device in recovery mode detected.");
                Console.WriteLine("  Please connect a device in recovery mode.");
                return false;
            }

            try
            {
                Console.WriteLine("✓ Device detected. Proceeding to exit recovery mode.");

                int result = AMRecoveryModeDeviceSetAutoBoot(recoveryDevice, true);
                if (result != 0)
                {
                    Console.WriteLine($"✗ AMRecoveryModeDeviceSetAutoBoot failed with error: {result}");
                    return false;
                }
                Console.WriteLine("✓ Successfully set AutoBoot flag.");

                result = AMRecoveryModeDeviceReboot(recoveryDevice);
                if (result != 0)
                {
                    Console.WriteLine($"✗ AMRecoveryModeDeviceReboot failed with error: {result}");
                    return false;
                }

                Console.WriteLine("✓ Reboot command sent. The device should be exiting recovery mode.");
                // The device will disconnect, and the OnRecoveryDeviceDisconnected callback will clear the recoveryDevice handle.
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return false;
            }
        }

        private static void FreeAllLibraries()
        {
            if (mobileDeviceLibraryHandle != IntPtr.Zero)
            {
                if (FreeLibrary(mobileDeviceLibraryHandle))
                {
                    Console.WriteLine("✓ MobileDevice.dll freed.");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to free MobileDevice.dll. Error: {Marshal.GetLastWin32Error()}");
                }
                mobileDeviceLibraryHandle = IntPtr.Zero;
            }
        }

        private static bool LoadAppleLibraries()
        {
            string? dllPath = null;
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string commonFiles = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);

            // Use Path.Combine for robust path construction
            string[] possiblePaths = new[]
            {
                Path.Combine(commonFiles, @"Apple\Mobile Device Support\MobileDevice.dll"),
                Path.Combine(programFiles, @"Common Files\Apple\Mobile Device Support\MobileDevice.dll"),
                // For 32-bit processes on 64-bit Windows
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Common Files\Apple\Mobile Device Support\MobileDevice.dll")
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    dllPath = path;
                    break;
                }
            }

            if (dllPath == null)
            {
                Console.WriteLine("✗ MobileDevice.dll not found in standard locations.");
                return false;
            }

            string dllDirectory = Path.GetDirectoryName(dllPath)!;
            if (!SetDllDirectory(dllDirectory))
            {
                Console.WriteLine("✗ Failed to set DLL directory.");
                return false;
            }
            Console.WriteLine($"✓ Set DLL directory to: {dllDirectory}");

            mobileDeviceLibraryHandle = LoadLibrary(dllPath);
            if (mobileDeviceLibraryHandle == IntPtr.Zero)
            {
                Console.WriteLine($"✗ Failed to load MobileDevice.dll: Error {Marshal.GetLastWin32Error()}");
                return false;
            }
            
            Console.WriteLine("✓ MobileDevice.dll loaded successfully.");
            return true;
        }
    }
}
