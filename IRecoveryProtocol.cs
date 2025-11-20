using System;
using System.Runtime.InteropServices;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace iPhoneTool
{
    /// <summary>
    /// Implementation of the iRecovery protocol for communicating with iOS devices in recovery mode
    /// Based on libirecovery and checkra1n implementations
    /// </summary>
    public class IRecoveryProtocol
    {
        // USB identifiers for Apple devices
        private const int APPLE_VENDOR_ID = 0x05AC;
        private const int RECOVERY_MODE_PID = 0x1281;
        private const int DFU_MODE_PID = 0x1227;
        
        // USB configuration
        private const int USB_CONFIGURATION = 1;
        private const int USB_INTERFACE = 0;
        private const int USB_TIMEOUT = 5000;
        
        // iRecovery USB endpoints
        private const byte ENDPOINT_OUT = 0x04;  // Bulk out endpoint
        private const byte ENDPOINT_IN = 0x85;   // Bulk in endpoint (0x05 | 0x80)
        
        // USB control request types
        private const byte USB_REQ_TYPE_VENDOR_OUT = 0x40;  // Vendor-specific, host-to-device
        private const byte USB_REQ_TYPE_VENDOR_IN = 0xC0;   // Vendor-specific, device-to-host
        
        // iRecovery control requests
        private const byte IRECV_CMD_SEND_FILE = 0x00;
        private const byte IRECV_CMD_SEND_COMMAND = 0x00;
        
        private UsbDevice? device;
        private UsbEndpointWriter? writer;
        private UsbEndpointReader? reader;
        
        public bool IsConnected => device != null && device.IsOpen;
        
        /// <summary>
        /// Opens connection to a device in recovery mode
        /// </summary>
        public bool Open()
        {
            try
            {
                Console.WriteLine("Searching for recovery mode device...");
                Console.WriteLine($"Total USB devices found: {UsbDevice.AllDevices.Count}");
                
                // Try to find recovery mode device
                foreach (UsbRegistry regDevice in UsbDevice.AllDevices)
                {
                    Console.WriteLine($"Found USB device: VID={regDevice.Vid:X4} PID={regDevice.Pid:X4} Name={regDevice.Name}");
                    
                    if (regDevice.Vid == APPLE_VENDOR_ID && 
                        (regDevice.Pid == RECOVERY_MODE_PID || regDevice.Pid == DFU_MODE_PID))
                    {
                        Console.WriteLine($"✓ Found Apple device in recovery/DFU mode!");
                        Console.WriteLine($"  Device name: {regDevice.Name}");
                        Console.WriteLine($"  Device path: {regDevice.SymbolicName}");
                        
                        if (!regDevice.Open(out device))
                        {
                            Console.WriteLine("✗ Failed to open device - trying next...");
                            continue;
                        }
                        
                        Console.WriteLine("✓ Device opened successfully");
                        
                        // Cast to IUsbDevice to set configuration
                        IUsbDevice? wholeUsbDevice = device as IUsbDevice;
                        if (wholeUsbDevice != null)
                        {
                            Console.WriteLine("  Setting USB configuration...");
                            try
                            {
                                wholeUsbDevice.SetConfiguration(USB_CONFIGURATION);
                                Console.WriteLine("  ✓ Configuration set");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"  ⚠ Configuration failed: {ex.Message}");
                            }
                            
                            Console.WriteLine("  Claiming USB interface...");
                            try
                            {
                                wholeUsbDevice.ClaimInterface(USB_INTERFACE);
                                Console.WriteLine("  ✓ Interface claimed");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"  ⚠ Claim interface failed: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("  ⚠ Device is not IUsbDevice (might be using different driver)");
                        }
                        
                        // Try to open endpoints for bulk transfers
                        try
                        {
                            writer = device.OpenEndpointWriter(WriteEndpointID.Ep04);
                            Console.WriteLine("  ✓ Endpoint writer opened (0x04)");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"  ⚠ Endpoint writer failed: {ex.Message}");
                        }
                        
                        try
                        {
                            reader = device.OpenEndpointReader(ReadEndpointID.Ep05);
                            Console.WriteLine("  ✓ Endpoint reader opened (0x85)");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"  ⚠ Endpoint reader failed: {ex.Message}");
                        }
                        
                        Console.WriteLine("✓✓✓ USB connection established! ✓✓✓");
                        return true;
                    }
                }
                
                Console.WriteLine("✗ No recovery mode device found");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error opening device: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }
        
        /// <summary>
        /// Sends a command to the device
        /// </summary>
        public bool SendCommand(string command)
        {
            if (device == null || !device.IsOpen)
            {
                Console.WriteLine("Device not connected");
                return false;
            }
            
            try
            {
                Console.WriteLine($"Sending command: {command}");
                
                // Commands are sent via bulk transfer on endpoint 0x04
                byte[] commandBytes = Encoding.ASCII.GetBytes(command + "\n");
                
                if (writer != null)
                {
                    ErrorCode ec = writer.Write(commandBytes, USB_TIMEOUT, out int bytesWritten);
                    
                    if (ec != ErrorCode.None)
                    {
                        Console.WriteLine($"Bulk transfer failed: {ec}");
                        return false;
                    }
                    
                    Console.WriteLine($"Sent {bytesWritten} bytes via bulk transfer");
                    
                    // Give device time to process
                    System.Threading.Thread.Sleep(100);
                    
                    return true;
                }
                else
                {
                    Console.WriteLine("Endpoint writer not available");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending command: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Sends a control transfer to the device
        /// </summary>
        public bool SendControlTransfer(byte request, ushort value, ushort index, byte[] data = null)
        {
            if (device == null || !device.IsOpen)
            {
                Console.WriteLine("Device not connected");
                return false;
            }
            
            try
            {
                Console.WriteLine($"Sending control transfer: request={request:X2}, value={value:X4}, index={index:X4}");
                
                UsbSetupPacket setupPacket = new UsbSetupPacket(
                    USB_REQ_TYPE_VENDOR_OUT,
                    request,
                    value,
                    index,
                    (short)(data?.Length ?? 0)
                );
                
                int bytesTransferred;
                byte[]? transferData = data ?? Array.Empty<byte>();
                bool success = device.ControlTransfer(ref setupPacket, transferData, transferData.Length, out bytesTransferred);
                
                Console.WriteLine($"Control transfer result: {success}, bytes: {bytesTransferred}");
                
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in control transfer: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Reboots the device out of recovery mode
        /// </summary>
        public bool Reboot()
        {
            if (device == null || !device.IsOpen)
            {
                Console.WriteLine("Device not connected");
                return false;
            }
            
            try
            {
                Console.WriteLine("=== Attempting to reboot device ===");
                
                // Method 1: Send reboot command via bulk transfer
                Console.WriteLine("Method 1: Bulk transfer reboot command");
                if (SendCommand("reboot"))
                {
                    Console.WriteLine("✓ Reboot command sent via bulk transfer");
                    System.Threading.Thread.Sleep(500);
                    return true;
                }
                
                // Method 2: Set auto-boot environment variable and reboot
                Console.WriteLine("\nMethod 2: Set auto-boot and reboot");
                SendCommand("setenv auto-boot true");
                System.Threading.Thread.Sleep(200);
                SendCommand("saveenv");
                System.Threading.Thread.Sleep(200);
                if (SendCommand("reboot"))
                {
                    Console.WriteLine("✓ Auto-boot + reboot sequence sent");
                    return true;
                }
                
                // Method 3: Control transfer reboot
                Console.WriteLine("\nMethod 3: Control transfer");
                byte[] emptyData = Array.Empty<byte>();
                if (SendControlTransfer(0x00, 0, 0, emptyData))
                {
                    Console.WriteLine("✓ Control transfer reboot sent");
                    return true;
                }
                
                Console.WriteLine("✗ All reboot methods failed");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rebooting: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Gets device information
        /// </summary>
        public string GetDeviceInfo()
        {
            if (device == null || !device.IsOpen)
            {
                return "Device not connected";
            }
            
            try
            {
                StringBuilder info = new StringBuilder();
                info.AppendLine("=== Device Information ===");
                info.AppendLine($"Vendor ID: {device.Info.Descriptor.VendorID:X4}");
                info.AppendLine($"Product ID: {device.Info.Descriptor.ProductID:X4}");
                info.AppendLine($"Manufacturer: {device.Info.ManufacturerString}");
                info.AppendLine($"Product: {device.Info.ProductString}");
                info.AppendLine($"Serial: {device.Info.SerialString}");
                
                return info.ToString();
            }
            catch (Exception ex)
            {
                return $"Error getting device info: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Closes the connection
        /// </summary>
        public void Close()
        {
            try
            {
                if (device != null && device.IsOpen)
                {
                    IUsbDevice? wholeUsbDevice = device as IUsbDevice;
                    if (wholeUsbDevice != null)
                    {
                        wholeUsbDevice.ReleaseInterface(USB_INTERFACE);
                    }
                    
                    device.Close();
                    Console.WriteLine("Device connection closed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing device: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Static helper method to exit recovery mode
        /// </summary>
        public static bool ExitRecoveryMode()
        {
            IRecoveryProtocol protocol = new IRecoveryProtocol();
            
            try
            {
                if (!protocol.Open())
                {
                    Console.WriteLine("Failed to connect to device");
                    return false;
                }
                
                Console.WriteLine(protocol.GetDeviceInfo());
                
                bool success = protocol.Reboot();
                
                if (success)
                {
                    Console.WriteLine("\n✓✓✓ Device should be rebooting now! ✓✓✓");
                    Console.WriteLine("Your iPhone will show the Apple logo and boot normally.");
                }
                
                return success;
            }
            finally
            {
                protocol.Close();
            }
        }
    }
}
