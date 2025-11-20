using System.Management;
using System.Text.RegularExpressions;

namespace iPhoneTool;

/// <summary>
/// Detects iOS devices in recovery/DFU mode using Windows Management Instrumentation (WMI)
/// Searches for USB devices with Apple's Vendor ID and recovery mode Product IDs
/// </summary>
public class RecoveryModeDetector
{
    // Apple's USB Vendor ID (same for all Apple devices)
    private const string APPLE_VENDOR_ID = "05AC";
    
    // Product IDs for different recovery/restore modes
    // Each mode represents a different device state:
    private static readonly string[] RECOVERY_MODE_PIDS = new[]
    {
        "1281", // Recovery Mode - Device shows iTunes/Computer icon
        "1227", // DFU Mode - Device screen is black, can restore firmware
        "1222", // WTF Mode - "What The Flash" - Low-level restore mode
    };

    /// <summary>
    /// Scans system for iOS devices in recovery/DFU mode
    /// Returns list of all recovery mode devices found
    /// </summary>
    public static List<RecoveryDevice> GetRecoveryDevices()
    {
        var devices = new List<RecoveryDevice>();
        
        try
        {
            // Query all USB devices from Windows Management Instrumentation (WMI)
            // Win32_PnPEntity contains info about all Plug and Play devices
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE '%USB%'"))
            {
                using (var collection = searcher.Get())
                {
                    foreach (ManagementObject device in collection)
                    {
                        try
                        {
                            // Get device ID and name from WMI object
                            string? deviceId = device["DeviceID"]?.ToString();
                            string? name = device["Name"]?.ToString();
                            
                            if (deviceId != null && name != null)
                            {
                                // Check if device has Apple's Vendor ID (05AC)
                                // DeviceID format: USB\VID_05AC&PID_1281\...
                                if (deviceId.Contains($"VID_{APPLE_VENDOR_ID}", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Check if Product ID matches any recovery mode PIDs
                                    foreach (var pid in RECOVERY_MODE_PIDS)
                                    {
                                        if (deviceId.Contains($"PID_{pid}", StringComparison.OrdinalIgnoreCase))
                                        {
                                            // Found a device in recovery mode!
                                            devices.Add(new RecoveryDevice
                                            {
                                                Name = name,
                                                DeviceId = deviceId,
                                                Mode = GetDeviceMode(pid),
                                                VendorId = APPLE_VENDOR_ID,
                                                ProductId = pid
                                            });
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception deviceEx)
                        {
                            // Skip this device if there's an error reading it
                            Console.WriteLine($"[DEBUG] Error reading device: {deviceEx.Message}");
                        }
                        finally
                        {
                            device?.Dispose();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error detecting recovery devices: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        
        return devices;
    }

    /// <summary>
    /// Converts Product ID to human-readable device mode name
    /// </summary>
    private static string GetDeviceMode(string productId)
    {
        return productId switch
        {
            "1281" => "Recovery Mode",    // Normal recovery - iTunes icon on screen
            "1227" => "DFU Mode",          // Device Firmware Update - black screen
            "1222" => "WTF Mode",          // Low-level flash mode
            _ => "Unknown Mode"
        };
    }

    /// <summary>
    /// Quick check if any recovery mode device is connected
    /// </summary>
    public static bool IsRecoveryModeAvailable()
    {
        return GetRecoveryDevices().Count > 0;
    }
}

/// <summary>
/// Represents an iOS device in recovery/DFU mode
/// Contains USB identifiers and device information
/// </summary>
public class RecoveryDevice
{
    /// <summary>Device name as shown in Device Manager</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Full Windows Device ID (contains VID, PID, and instance path)</summary>
    public string DeviceId { get; set; } = string.Empty;
    
    /// <summary>Human-readable mode name (Recovery Mode, DFU Mode, etc.)</summary>
    public string Mode { get; set; } = string.Empty;
    
    /// <summary>USB Vendor ID (05AC for Apple)</summary>
    public string VendorId { get; set; } = string.Empty;
    
    /// <summary>USB Product ID (1281 for Recovery, 1227 for DFU)</summary>
    public string ProductId { get; set; } = string.Empty;
}
