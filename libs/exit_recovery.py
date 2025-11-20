#!/usr/bin/env python3
"""
Simple script to exit recovery mode using libimobiledevice
Requires: pip install pymobiledevice3
"""

import sys
import usb.core
import usb.util

# Apple Vendor ID and Recovery Mode Product ID
APPLE_VENDOR_ID = 0x05AC
RECOVERY_MODE_PID = 0x1281
DFU_MODE_PID = 0x1227

def exit_recovery_mode():
    """Exit recovery mode by sending reboot command via USB"""
    try:
        # Find the device
        dev = usb.core.find(idVendor=APPLE_VENDOR_ID, idProduct=RECOVERY_MODE_PID)
        
        if dev is None:
            # Try DFU mode
            dev = usb.core.find(idVendor=APPLE_VENDOR_ID, idProduct=DFU_MODE_PID)
        
        if dev is None:
            print("ERROR: No device found in recovery or DFU mode")
            return False
        
        print(f"Found device: {hex(dev.idVendor)}:{hex(dev.idProduct)}")
        
        # Send reboot command (USB control transfer)
        # bmRequestType=0x40 (vendor, host-to-device)
        # bRequest=0 (reboot)
        try:
            result = dev.ctrl_transfer(0x40, 0, 0, 0, None)
            print(f"Reboot command sent successfully: {result}")
            return True
        except usb.core.USBError as e:
            print(f"USB Error: {e}")
            # Try alternative method
            try:
                result = dev.ctrl_transfer(0x21, 0, 0, 0, None)
                print(f"Alternative reboot sent: {result}")
                return True
            except:
                pass
        
        return False
        
    except Exception as e:
        print(f"Error: {e}")
        return False

if __name__ == "__main__":
    success = exit_recovery_mode()
    sys.exit(0 if success else 1)
