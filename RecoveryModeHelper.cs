using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace iPhoneTool;

/// <summary>
/// Provides methods to exit iOS devices from recovery mode
/// Tries multiple approaches: Apple API, irecovery commands, and USB communication
/// </summary>
public class RecoveryModeHelper
{
    // Apple USB Vendor ID (same for all Apple devices)
    private const int APPLE_VENDOR_ID = 0x05AC;
    
    // Recovery mode Product IDs
    private const int RECOVERY_MODE_PID = 0x1281;  // Standard recovery mode
    private const int DFU_MODE_PID = 0x1227;       // Device Firmware Update mode
    
    // USB Control transfer parameters for recovery mode
    private const byte USB_REQUEST_TYPE_VENDOR = 0x40;
    private const byte RECOVERY_CMD_SETENV = 0x00;
    private const byte RECOVERY_CMD_REBOOT = 0x00;
    
    /// <summary>
    /// Attempts to exit recovery mode using multiple methods
    /// Tries in order: Apple API, irecovery, USB bulk transfer, USB control transfer
    /// Returns true if any method succeeds
    /// </summary>
    public static bool ExitRecoveryMode(RecoveryDevice device)
    {
        try
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         ATTEMPTING TO EXIT RECOVERY MODE                  ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"Device: {device.Name}");
            Console.WriteLine($"Device ID: {device.DeviceId}");
            Console.WriteLine($"USB: VID:{device.VendorId:X4} PID:{device.ProductId:X4}");
            Console.WriteLine("");
            
            // Check for Apple drivers first
            Console.WriteLine("--- Checking Prerequisites ---");
            bool hasDrivers = WinUSBRecovery.IsAppleMobileDeviceSupportInstalled();
            if (!hasDrivers)
            {
                Console.WriteLine("⚠ WARNING: Apple Mobile Device Support may not be installed");
                Console.WriteLine("  This is required for USB communication with iOS devices");
                Console.WriteLine("  Install iTunes or Apple Devices app from Microsoft Store");
                Console.WriteLine("");
            }
            else
            {
                Console.WriteLine("");
                
                // Method 0: Use Apple's native MobileDevice.dll API (BEST METHOD!)
                Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║  METHOD 0: Apple Mobile Device Support API (NATIVE)       ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
                try
                {
                    if (AppleMobileDeviceAPI.ExitRecoveryMode())
                    {
                        Console.WriteLine("");
                        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
                        Console.WriteLine("║              ✓✓✓ SUCCESS! ✓✓✓                             ║");
                        Console.WriteLine("║     Your iPhone should be rebooting to normal mode!        ║");
                        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Apple API error: {ex.Message}");
                }
                
                Console.WriteLine("");
                
                // Method 0.5: Use irecovery with auto-boot commands
                Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║  METHOD 0.5: irecovery auto-boot (libimobiledevice)      ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
                try
                {
                    if (LibimobiledeviceHelper.ExitRecoveryMode())
                    {
                        Console.WriteLine("");
                        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
                        Console.WriteLine("║              ✓✓✓ SUCCESS! ✓✓✓                             ║");
                        Console.WriteLine("║     Your iPhone should be exiting recovery mode!           ║");
                        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ irecovery error: {ex.Message}");
                }
                
                Console.WriteLine("");
            }
            
            // Method 1: Use our IRecoveryProtocol implementation
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  METHOD 1: Direct USB Communication (IRecoveryProtocol)   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            try
            {
                if (IRecoveryProtocol.ExitRecoveryMode())
                {
                    Console.WriteLine("");
                    Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
                    Console.WriteLine("║              ✓✓✓ SUCCESS! ✓✓✓                             ║");
                    Console.WriteLine("║     Your iPhone should be rebooting to normal mode!        ║");
                    Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ IRecoveryProtocol error: {ex.Message}");
                Console.WriteLine($"  {ex.StackTrace}");
            }
            
            Console.WriteLine("");
            
            // Method 2: Try alternative USB approach
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  METHOD 2: Alternative USB Communication                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            if (TryExitViaUSB())
            {
                Console.WriteLine("✓ Success with alternative USB method!");
                return true;
            }
            
            Console.WriteLine("");
            
            // All methods failed - provide helpful guidance
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  ✗✗✗ ALL METHODS FAILED ✗✗✗               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine("");
            Console.WriteLine("POSSIBLE CAUSES:");
            Console.WriteLine("1. Missing USB drivers");
            Console.WriteLine("   → Install iTunes or Apple Devices app");
            Console.WriteLine("");
            Console.WriteLine("2. USB driver conflict");
            Console.WriteLine("   → The device might be using Apple's driver instead of WinUSB");
            Console.WriteLine("   → LibUsbDotNet requires WinUSB or libusb-win32 driver");
            Console.WriteLine("");
            Console.WriteLine("3. Insufficient permissions");
            Console.WriteLine("   → Try running this application as Administrator");
            Console.WriteLine("");
            Console.WriteLine("MANUAL WORKAROUND:");
            Console.WriteLine("Option 1 - Force Restart (RECOMMENDED):");
            if (device.ProductId.Equals("1281", StringComparison.OrdinalIgnoreCase)) // Recovery mode
            {
                Console.WriteLine("  • iPhone 8 or later:");
                Console.WriteLine("    1. Press Volume Up (quick)");
                Console.WriteLine("    2. Press Volume Down (quick)");
                Console.WriteLine("    3. Hold Power button until Apple logo (10-15 sec)");
                Console.WriteLine("");
                Console.WriteLine("  • iPhone 7:");
                Console.WriteLine("    1. Hold Volume Down + Power");
                Console.WriteLine("    2. Keep holding until Apple logo");
                Console.WriteLine("");
                Console.WriteLine("  • iPhone 6s and earlier:");
                Console.WriteLine("    1. Hold Home + Power");
                Console.WriteLine("    2. Keep holding until Apple logo");
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ FATAL ERROR: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }
    
    private static bool TryRestartDeviceViaDeviceManager(string deviceId)
    {
        try
        {
            // Use WMI to restart the USB device
            using (var searcher = new System.Management.ManagementObjectSearcher(
                $"SELECT * FROM Win32_PnPEntity WHERE DeviceID='{deviceId.Replace("\\", "\\\\")}'"))
            {
                foreach (System.Management.ManagementObject device in searcher.Get())
                {
                    Console.WriteLine($"Found WMI device: {device["Name"]}");
                    
                    // Try to disable and re-enable the device
                    try
                    {
                        // Disable
                        var result = device.InvokeMethod("Disable", null);
                        Console.WriteLine($"Disable result: {result}");
                        System.Threading.Thread.Sleep(500);
                        
                        // Enable
                        result = device.InvokeMethod("Enable", null);
                        Console.WriteLine($"Enable result: {result}");
                        
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"WMI method failed: {ex.Message}");
                        // This often fails due to permissions, try alternative
                    }
                }
            }
            
            // Alternative: Use PnPUtil or DevCon approach via command line
            string pnpCommand = $"pnputil /restart-device \"{deviceId}\"";
            Console.WriteLine($"Trying: {pnpCommand}");
            
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "pnputil",
                Arguments = $"/restart-device \"{deviceId}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Verb = "runas" // Run as administrator
            };
            
            using (var process = System.Diagnostics.Process.Start(psi))
            {
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    
                    Console.WriteLine($"Output: {output}");
                    if (!string.IsNullOrEmpty(error))
                        Console.WriteLine($"Error: {error}");
                    
                    return process.ExitCode == 0;
                }
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Device Manager method error: {ex.Message}");
            return false;
        }
    }
    
    private static bool TryExitViaUSB()
    {
        try
        {
            // Find the Apple device in recovery mode
            UsbDevice? usbDevice = null;
            
            foreach (UsbRegistry regDevice in UsbDevice.AllDevices)
            {
                if (regDevice.Vid == APPLE_VENDOR_ID && 
                    (regDevice.Pid == RECOVERY_MODE_PID || regDevice.Pid == DFU_MODE_PID))
                {
                    if (regDevice.Open(out usbDevice))
                    {
                        break;
                    }
                }
            }
            
            if (usbDevice == null)
            {
                Console.WriteLine("Could not open USB device");
                return false;
            }
            
            try
            {
                // For WinUSB devices, we need to claim the interface
                IUsbDevice? wholeUsbDevice = usbDevice as IUsbDevice;
                if (wholeUsbDevice != null)
                {
                    wholeUsbDevice.SetConfiguration(1);
                    wholeUsbDevice.ClaimInterface(0);
                }
                
                // Send reboot command
                // This sends a vendor-specific control transfer to reboot the device
                UsbSetupPacket setupPacket = new UsbSetupPacket(
                    0x40,  // bmRequestType: Vendor request, Device recipient, Host-to-device
                    0x00,  // bRequest (reboot)
                    0x0000, // wValue
                    0x0000, // wIndex
                    0      // wLength
                );
                
                int bytesTransferred;
                bool success = usbDevice.ControlTransfer(ref setupPacket, null, 0, out bytesTransferred);
                
                if (wholeUsbDevice != null)
                {
                    wholeUsbDevice.ReleaseInterface(0);
                }
                
                return success;
            }
            finally
            {
                if (usbDevice != null)
                {
                    usbDevice.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"USB communication error: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Sends setenv auto-boot command followed by reboot
    /// </summary>
    public static bool SendRebootCommand()
    {
        try
        {
            var recoveryDevices = RecoveryModeDetector.GetRecoveryDevices();
            if (recoveryDevices.Count == 0)
            {
                return false;
            }
            
            return TryExitViaUSB();
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Alternative method: Try to communicate using WinUSB driver
    /// </summary>
    public static bool TryExitWithWinUSB()
    {
        try
        {
            // Find all USB devices
            UsbDeviceFinder finder = new UsbDeviceFinder(APPLE_VENDOR_ID, RECOVERY_MODE_PID);
            UsbDevice device = UsbDevice.OpenUsbDevice(finder);
            
            if (device == null)
            {
                // Try DFU mode
                finder = new UsbDeviceFinder(APPLE_VENDOR_ID, DFU_MODE_PID);
                device = UsbDevice.OpenUsbDevice(finder);
            }
            
            if (device == null)
            {
                return false;
            }
            
            try
            {
                // Send exit recovery command
                UsbSetupPacket packet = new UsbSetupPacket(
                    0x40, // bmRequestType: Vendor, Device, Out
                    0x00, // bRequest: Custom command
                    0x0000, // wValue
                    0x0000, // wIndex
                    0  // wLength
                );
                
                int transferred;
                bool result = device.ControlTransfer(ref packet, null, 0, out transferred);
                
                return result;
            }
            finally
            {
                device.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WinUSB error: {ex.Message}");
            return false;
        }
    }
}
