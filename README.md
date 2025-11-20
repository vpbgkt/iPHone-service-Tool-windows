# iPhone Tool - Professional iOS Device Management

A comprehensive Windows application for iOS device management, restore operations, and recovery mode handling. Built with C# and .NET 8, this tool provides professional-grade functionality similar to 3uTools, iTunes, and other commercial iOS management software.

## 🌟 Features

### Device Information
- **Comprehensive Device Info**: Get detailed information about connected iOS devices
- **Hardware Details**: IMEI, IMEI 2 (dual SIM), Serial Number, UDID
- **Software Info**: iOS version, build version, product type, model number
- **Network Info**: MAC address, baseband version, activation state
- **Recovery Mode Detection**: Automatically detects devices in recovery/DFU mode

### Recovery Mode Operations
- **Enter Recovery Mode**: Put device into recovery mode from normal mode
- **Exit Recovery Mode**: Boot device back to normal iOS from recovery mode
- **Stable Operation**: No crashes, clean lifecycle management
- **Multiple Methods**: Uses Apple Mobile Device Support API, irecovery, and direct USB communication

### Professional IPSW Restore (NEW!)
- **✓ TSS Server Communication**: Requests SHSH blobs from Apple's signing server
- **✓ Firmware Component Extraction**: Extracts all required components from IPSW
  - iBSS (Initial Boot Stage 2)
  - iBEC (iBoot Epoch Change)
  - Restore Ramdisk
  - Kernelcache
  - Device Tree
  - Root Filesystem
- **✓ Bootloader Flashing**: Loads iBSS and iBEC bootloaders to device
- **✓ Ramdisk Loading**: Loads restore ramdisk and boots into restore mode
- **✓ Complete Workflow**: Professional 8-step restore process

### Restore Process Steps
1. **IPSW Validation** - Verifies firmware file integrity
2. **Device Detection** - Finds devices in normal or recovery mode
3. **Recovery Mode Entry** - Automatically enters recovery mode
4. **Firmware Extraction** - Extracts bootloaders, ramdisk, kernel from IPSW
5. **TSS Communication** - Requests signed blobs from Apple
6. **Bootloader Flashing** - Sends iBSS and iBEC to device
7. **Ramdisk & Kernel** - Loads restore ramdisk and kernel
8. **Restore Operation** - Prepares device for filesystem restore

## 🚀 Usage

### Get Device Information
1. Connect your iPhone via USB
2. Trust the computer on your device
3. Click **"Get Device Info"**
4. View comprehensive device details including IMEI, IMEI 2, serial number, iOS version, etc.

### Exit Recovery Mode
1. Connect device in recovery mode (showing iTunes/Computer icon)
2. Click **"Exit Recovery Mode"**
3. Device will automatically reboot to normal iOS

### Restore from IPSW
1. Download the desired IPSW file for your device
2. Connect your device (normal or recovery mode)
3. Click **"Restore from IPSW"**
4. Select your IPSW file
5. Choose restore type (Erase All Data or Update)
6. The tool will:
   - Extract firmware components
   - Request signing from Apple TSS server
   - Flash bootloaders (iBSS/iBEC)
   - Load ramdisk and kernel
   - Prepare device for restore
7. Complete restore using iTunes/Finder

## 🔧 How to Build

1.  Make sure you have the .NET 8 SDK installed.
2.  Open the project in Visual Studio or VS Code.
3.  Build and run the project:
    ```bash
    dotnet build
    dotnet run
    ```

## 📋 Requirements

- Windows 10/11 (64-bit)
- .NET 8 Desktop Runtime
- Apple Mobile Device Support (iTunes or Apple Devices app)
- USB connection to iOS device

## ⚠️ Important Notes

### About Restore Completion
The tool prepares the device completely for restore by loading all bootloaders and ramdisk. To complete the filesystem restore, use iTunes/Finder (recommended) or idevicerestore command-line tool.

### TSS Server & Signing
Only Apple-signed iOS versions can be restored. Check [IPSW.me](https://ipsw.me) for signing status.

## 📜 License

This project is for educational and personal use.

---

**Version**: 2.0  
**Last Updated**: November 7, 2025

