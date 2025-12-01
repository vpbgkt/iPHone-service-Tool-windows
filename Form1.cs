using iMobileDevice;
using iMobileDevice.iDevice;
using iMobileDevice.Lockdown;
using iMobileDevice.Restore;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace iPhoneTool
{
    public partial class Form1 : Form
    {
        private string currentDeviceUdid = string.Empty;
        private System.Windows.Forms.Timer? monitorTimer = null;
        private int monitorSeconds = 0;
        private const int MaxLogLines = 5000; // Limit log buffer to prevent memory issues
        
        public Form1()
        {
            InitializeComponent();
            // Redirect console output to the TextBox
            var writer = new TextBoxStreamWriter(infoTextBox, MaxLogLines);
            Console.SetOut(writer);
            Console.SetError(writer);

            // Initialize the API when the form loads.
            this.Load += (s, e) => {
                AppleMobileDeviceAPI.Initialize();
                UpdateDeviceStatus("No device detected", "");
                UpdateProgressLabel("Ready");
            };
            // Ensure API is shut down cleanly when the form closes.
            this.FormClosing += (s, e) => AppleMobileDeviceAPI.Shutdown();
        }

        private void getInfoButton_Click(object sender, EventArgs e)
        {
            SetButtonsEnabled(false);
            infoTextBox.Text = "Searching for devices..." + Environment.NewLine;
            UpdateProgressLabel("Detecting devices...");

            var iDevice = LibiMobileDevice.Instance.iDevice;
            var lockdown = LibiMobileDevice.Instance.Lockdown;

            // First, check for normal mode devices
            int count = 0;
            iDevice.idevice_get_device_list(out var devices, ref count);

            if (count > 0)
            {
                infoTextBox.Text += $"Found {count} device(s) in normal mode." + Environment.NewLine;

                foreach (var udid in devices)
                {
                    // Create device handle from UDID
                    iDevice.idevice_new(out var deviceHandle, udid);
                    
                    if (deviceHandle.IsInvalid)
                    {
                        infoTextBox.Text += $"Could not create handle for device {udid}." + Environment.NewLine;
                        continue;
                    }

                    lockdown.lockdownd_client_new_with_handshake(deviceHandle, out var client, "iPhoneTool");

                    if (client.IsInvalid)
                    {
                        infoTextBox.Text += $"Could not connect to device {udid}." + Environment.NewLine;
                        deviceHandle.Dispose();
                        continue;
                    }

                    // Helper function to get string values from the device
                    string? GetStringValue(LockdownClientHandle client, string key, string? domain = null)
                    {
                        var lockdown = LibiMobileDevice.Instance.Lockdown;
                        var result = lockdown.lockdownd_get_value(client, domain, key, out var node);
                        if (result == LockdownError.Success && node != null && !node.IsInvalid)
                        {
                            node.Api.Plist.plist_get_string_val(node, out var value);
                            node.Dispose();
                            return value;
                        }
                        return null;
                    }

                    infoTextBox.Text += $"Device Name: {GetStringValue(client, "DeviceName") ?? "N/A"}" + Environment.NewLine;
                    infoTextBox.Text += $"iOS Version: {GetStringValue(client, "ProductVersion") ?? "N/A"}" + Environment.NewLine;
                    infoTextBox.Text += $"Build Version: {GetStringValue(client, "BuildVersion") ?? "N/A"}" + Environment.NewLine;
                    infoTextBox.Text += $"Product Type: {GetStringValue(client, "ProductType") ?? "N/A"}" + Environment.NewLine;
                    infoTextBox.Text += $"Model Number: {GetStringValue(client, "ModelNumber") ?? "N/A"}" + Environment.NewLine;
                    infoTextBox.Text += $"Serial Number: {GetStringValue(client, "SerialNumber") ?? "N/A"}" + Environment.NewLine;
                    infoTextBox.Text += $"UDID: {GetStringValue(client, "UniqueDeviceID") ?? "N/A"}" + Environment.NewLine;
                    infoTextBox.Text += $"IMEI: {GetStringValue(client, "InternationalMobileEquipmentIdentity") ?? "N/A"}" + Environment.NewLine;
                    string? imei2 = GetStringValue(client, "InternationalMobileEquipmentIdentity2");
                    if (!string.IsNullOrEmpty(imei2))
                    {
                        infoTextBox.Text += $"IMEI 2: {imei2}" + Environment.NewLine;
                    }
                    infoTextBox.Text += $"Baseband Version: {GetStringValue(client, "BasebandVersion") ?? "N/A"}" + Environment.NewLine;
                    infoTextBox.Text += $"MAC Address: {GetStringValue(client, "EthernetAddress") ?? "N/A"}" + Environment.NewLine;
                    infoTextBox.Text += $"Activation State: {GetStringValue(client, "ActivationState") ?? "N/A"}" + Environment.NewLine;

                    currentDeviceUdid = GetStringValue(client, "UniqueDeviceID") ?? string.Empty;
                    
                    // Update status bar with device info
                    string deviceName = GetStringValue(client, "DeviceName") ?? "Unknown";
                    UpdateDeviceStatus($"✅ Connected: {deviceName}", $"UDID: {currentDeviceUdid}");
                    
                    infoTextBox.Text += "--------------------------------" + Environment.NewLine;

                    client.Dispose();
                    deviceHandle.Dispose();
                }
            }
            else
            {
                UpdateDeviceStatus("No device detected", "");
            }
            
            // Check for recovery mode devices
            infoTextBox.Text += Environment.NewLine + "Checking for recovery mode devices..." + Environment.NewLine;
            var recoveryDevices = RecoveryModeDetector.GetRecoveryDevices();
            
            if (recoveryDevices.Count > 0)
            {
                infoTextBox.Text += $"Found {recoveryDevices.Count} device(s) in recovery/DFU mode!" + Environment.NewLine;
                infoTextBox.Text += Environment.NewLine;
                
                foreach (var device in recoveryDevices)
                {
                    infoTextBox.Text += $"Device: {device.Name}" + Environment.NewLine;
                    infoTextBox.Text += $"Mode: {device.Mode}" + Environment.NewLine;
                    infoTextBox.Text += $"USB Info: VID:{device.VendorId} PID:{device.ProductId}" + Environment.NewLine;
                    infoTextBox.Text += $"Device ID: {device.DeviceId}" + Environment.NewLine;
                    infoTextBox.Text += "--------------------------------" + Environment.NewLine;
                }
                
                infoTextBox.Text += Environment.NewLine;
                infoTextBox.Text += "ℹ️ Recovery mode devices detected!" + Environment.NewLine;
                infoTextBox.Text += "You can now use 'Restore from IPSW' or 'Exit Recovery Mode'." + Environment.NewLine;
                
                // Update status for recovery mode
                UpdateDeviceStatus($"⚠️ Recovery Mode: {recoveryDevices[0].Name}", $"Mode: {recoveryDevices[0].Mode}");
            }
            else if (count == 0)
            {
                infoTextBox.Text += "No devices found in any mode." + Environment.NewLine;
                UpdateDeviceStatus("No device detected", "");
                infoTextBox.Text += Environment.NewLine;
                infoTextBox.Text += "Please ensure:" + Environment.NewLine;
                infoTextBox.Text += "1. Device is connected via USB" + Environment.NewLine;
                infoTextBox.Text += "2. iTunes or Apple Mobile Device Support is installed" + Environment.NewLine;
                infoTextBox.Text += "3. You've trusted this computer on the device" + Environment.NewLine;
            }
            
            SetButtonsEnabled(true);
            UpdateProgressLabel("Ready");
        }
        
        private async void restoreButton_Click(object sender, EventArgs e)
        {
            // Open file dialog to select IPSW file
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "IPSW files (*.ipsw)|*.ipsw|All files (*.*)|*.*";
                openFileDialog.Title = "Select IPSW File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string ipswPath = openFileDialog.FileName;
                    infoTextBox.Text = "";
                    
                    // Ask user if they want to erase all data
                    var result = MessageBox.Show(
                        "Do you want to ERASE all data on the device?\n\n" +
                        "• YES: Complete erase and restore (like new device)\n" +
                        "• NO: Update iOS (attempts to preserve data)",
                        "Restore Options",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                    {
                        return;
                    }

                    bool eraseData = (result == DialogResult.Yes);

                    // Disable all buttons during operation
                    SetButtonsEnabled(false);
                    UpdateProgressLabel("Starting restore...");

                    try
                    {
                        var restoreManager = new RestoreManager(
                            UpdateTextBox,
                            (value) => {
                                UpdateProgress(value);
                                UpdateProgressLabel($"Restoring... {value}%");
                            }
                        );

                        await restoreManager.RestoreDeviceAsync(ipswPath, eraseData);
                        UpdateProgressLabel("Restore complete!");
                    }
                    catch (Exception ex)
                    {
                        UpdateProgressLabel("Restore failed");
                        MessageBox.Show($"Error during restore: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        SetButtonsEnabled(true);
                        UpdateProgress(0);
                        UpdateProgressLabel("Ready");
                    }
                }
            }
        }



        /// <summary>
        /// Event handler for "Enter Recovery" button click
        /// Puts a device in normal mode into recovery mode using lockdownd API
        /// </summary>
        private void enterRecoveryButton_Click(object sender, EventArgs e)
        {
            SetButtonsEnabled(false);
            infoTextBox.Text = "Entering recovery mode..." + Environment.NewLine;
            UpdateProgressLabel("Entering recovery mode...");
            Task.Run(() => {
                EnterRecoveryMode();
                Invoke(new Action(() => {
                    SetButtonsEnabled(true);
                    UpdateProgressLabel("Ready");
                }));
            });
        }

        /// <summary>
        /// Puts connected iOS device into recovery mode
        /// Uses libimobiledevice's lockdownd_enter_recovery function
        /// </summary>
        private void EnterRecoveryMode()
        {
            try
            {
                // Get libimobiledevice instances for device communication
                var iDevice = LibiMobileDevice.Instance.iDevice;
                var lockdown = LibiMobileDevice.Instance.Lockdown;
                
                // Get list of connected iOS devices
                int count = 0;
                iDevice.idevice_get_device_list(out var devices, ref count);

                if (count == 0)
                {
                    UpdateTextBox("No devices found!");
                    return;
                }

                // Use the first device found
                string udid = devices[0];
                iDevice.idevice_new(out var deviceHandle, udid);
                
                if (deviceHandle.IsInvalid)
                {
                    UpdateTextBox("Failed to create device handle!");
                    return;
                }

                // Create lockdownd client to communicate with device
                lockdown.lockdownd_client_new_with_handshake(deviceHandle, out var client, "iPhoneTool");
                
                if (client.IsInvalid)
                {
                    UpdateTextBox("Failed to connect to device!");
                    deviceHandle.Dispose();
                    return;
                }

                UpdateTextBox($"Device: {udid}");
                UpdateTextBox("Requesting recovery mode...");
                
                // Send command to device to enter recovery mode
                // This will reboot the device into recovery mode (shows iTunes/Computer icon)
                var result = lockdown.lockdownd_enter_recovery(client);
                
                if (result == LockdownError.Success)
                {
                    UpdateTextBox("✓ Device entering recovery mode!");
                    UpdateTextBox("⏳ Please wait for device to reboot...");
                    UpdateTextBox("");
                    UpdateTextBox("Recovery Mode Info:");
                    UpdateTextBox("  • Device will show iTunes/Computer icon");
                    UpdateTextBox("  • Use 'Exit Recovery' button to exit back to normal");
                }
                else
                {
                    UpdateTextBox($"✗ Failed to enter recovery mode (Error: {result})");
                }
                
                // Clean up resources
                client.Dispose();
                deviceHandle.Dispose();
            }
            catch (Exception ex)
            {
                UpdateTextBox($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Event handler for "Exit Recovery" button click
        /// Starts the recovery mode exit process in a background task
        /// </summary>
        private async void exitRecoveryButton_Click(object sender, EventArgs e)
        {
            // Disable all buttons to prevent multiple clicks
            SetButtonsEnabled(false);
            infoTextBox.Text = "Starting exit recovery process...\r\n";
            UpdateProgressLabel("Exiting recovery mode...");
            
            try
            {
                // Show start message
                UpdateTextBox($"=== Exit Recovery Started - {DateTime.Now} ===\r\n\r\n");
                
                // Create log file
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "recovery_log.txt");
                using (var fileWriter = new StreamWriter(logPath, false) { AutoFlush = true })
                {
                    // Redirect console temporarily
                    var originalOut = Console.Out;
                    var writer = new TextBoxStreamWriter(infoTextBox);
                    var multiWriter = new MultiTextWriter(writer, fileWriter);
                    Console.SetOut(multiWriter);
                    
                    fileWriter.WriteLine($"=== Exit Recovery Log - {DateTime.Now} ===");
                    fileWriter.WriteLine($"Log file: {logPath}");
                    fileWriter.WriteLine("");
                    
                    // Run recovery exit in background
                    await Task.Run(() =>
                    {
                        try
                        {
                            ExitRecoveryMode();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("");
                            Console.WriteLine($"✗ Error: {ex.Message}");
                            Console.WriteLine($"Stack: {ex.StackTrace}");
                        }
                    });
                    
                    // Restore console immediately after task completes
                    Console.SetOut(originalOut);
                }
                
                // Update UI - this runs on UI thread
                UpdateTextBox("\r\n=== Operation Complete ===\r\n");
                
                // Show completion dialog
                MessageBox.Show(
                    this,
                    "The exit recovery command has been sent.\n\n" +
                    "Your device should now be rebooting into normal mode.",
                    "Operation Finished",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                UpdateTextBox($"\r\n✗ Error: {ex.Message}\r\n");
                MessageBox.Show(
                    this,
                    $"An error occurred:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable buttons
                SetButtonsEnabled(true);
                UpdateProgressLabel("Ready");
            }
        }

        /// <summary>
        /// Main method to exit recovery mode
        /// Detects device in recovery, calls helper to exit, then monitors the device state
        /// </summary>
        private void ExitRecoveryMode()
        {
            try
            {
                Console.WriteLine("Detecting recovery mode devices...");
                Console.WriteLine("");
                
                // Use WMI to detect if any Apple devices are in recovery mode
                var recoveryDevices = RecoveryModeDetector.GetRecoveryDevices();
                
                if (recoveryDevices.Count == 0)
                {
                    // No recovery mode device found - show helpful message
                    Console.WriteLine("✗ No devices found in recovery mode!");
                    Console.WriteLine("");
                    Console.WriteLine("The device must be in recovery mode to exit it.");
                    Console.WriteLine("");
                    Console.WriteLine("Current state: Device is either:");
                    Console.WriteLine("  • In normal mode (already exited)");
                    Console.WriteLine("  • Not connected");
                    Console.WriteLine("  • In DFU mode (different from recovery)");
                    Console.WriteLine("");
                    Console.WriteLine("If your device is stuck, try:");
                    Console.WriteLine("  1. Force restart:");
                    Console.WriteLine("     iPhone 8 or later: Press and quickly release Volume Up,");
                    Console.WriteLine("     then Volume Down, then hold Side button until Apple logo");
                    Console.WriteLine("  2. Reconnect the USB cable");
                    return;
                }
                
                // Found at least one device in recovery mode
                var device = recoveryDevices[0];
                Console.WriteLine($"✓ Found device: {device.Name}");
                Console.WriteLine("");
                
                // Call RecoveryModeHelper which tries multiple methods to exit recovery
                // This includes Apple API, irecovery, and USB communication methods
                bool success = RecoveryModeHelper.ExitRecoveryMode(device);
                
                if (success)
                {
                    Console.WriteLine("");
                    Console.WriteLine("✓ Device should be exiting recovery mode!");
                    Console.WriteLine("");
                    Console.WriteLine("🎉 SUCCESS! Your device should:");
                    Console.WriteLine("  1. Show Apple logo");
                    Console.WriteLine("  2. Boot to iOS home screen (15-30 seconds)");
                    Console.WriteLine("  3. Appear in iTunes/Finder when fully booted");
                    Console.WriteLine("");
                    Console.WriteLine("Note: The device takes 15-30 seconds to fully boot.");
                }
                else
                {
                    // RecoveryModeHelper failed to send exit command
                    Console.WriteLine("");
                    Console.WriteLine("⚠ Could not send exit recovery command.");
                    Console.WriteLine("   See error messages above for details.");
                }
                
                Console.WriteLine("");
                Console.WriteLine("=== Process Complete ===");
            }
            catch (Exception ex)
            {
                // Catch any errors during the process
                Console.WriteLine("");
                Console.WriteLine($"✗ Error during exit recovery: {ex.Message}");
                Console.WriteLine("");
                Console.WriteLine("Please try:");
                Console.WriteLine("  1. Disconnect and reconnect the device");
                Console.WriteLine("  2. Restart this application");
                Console.WriteLine("  3. Try the manual force restart method");
            }
        }
        
        /// <summary>
        /// Event handler for Clear Log button
        /// </summary>
        private void clearLogButton_Click(object sender, EventArgs e)
        {
            infoTextBox.Clear();
            infoTextBox.Text = "Log cleared.\r\n";
        }

        /// <summary>
        /// Event handler for Save Log button
        /// </summary>
        private void saveLogButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log|All files (*.*)|*.*";
                    saveFileDialog.Title = "Save Log File";
                    saveFileDialog.FileName = $"iphone_tool_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog.FileName, infoTextBox.Text);
                        MessageBox.Show($"Log saved successfully to:\n{saveFileDialog.FileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving log: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler for Exit menu item
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Event handler for About menu item
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "iPhone Service Tool v1.0\n\n" +
                "A comprehensive tool for iOS device management:\n" +
                "• Device information retrieval\n" +
                "• IPSW firmware restore\n" +
                "• Recovery mode entry/exit\n\n" +
                "Built with libimobiledevice\n" +
                "© 2025",
                "About iPhone Service Tool",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// Enable or disable all action buttons
        /// </summary>
        private void SetButtonsEnabled(bool enabled)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => SetButtonsEnabled(enabled)));
                    return;
                }
                
                getInfoButton.Enabled = enabled;
                restoreButton.Enabled = enabled;
                enterRecoveryButton.Enabled = enabled;
                exitRecoveryButton.Enabled = enabled;
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        /// <summary>
        /// Updates the device status in the status bar
        /// </summary>
        private void UpdateDeviceStatus(string status, string udidInfo)
        {
            try
            {
                if (statusStrip.InvokeRequired)
                {
                    statusStrip.Invoke(new Action(() => {
                        deviceStatusLabel.Text = status;
                        udidStatusLabel.Text = udidInfo;
                    }));
                }
                else
                {
                    deviceStatusLabel.Text = status;
                    udidStatusLabel.Text = udidInfo;
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        /// <summary>
        /// Updates the progress label text
        /// </summary>
        private void UpdateProgressLabel(string text)
        {
            try
            {
                if (progressLabel.InvokeRequired)
                {
                    progressLabel.Invoke(new Action(() => {
                        progressLabel.Text = text;
                    }));
                }
                else
                {
                    progressLabel.Text = text;
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        /// <summary>
        /// Helper class to redirect Console.WriteLine output to a TextBox control.
        /// Handles cross-thread calls safely and limits buffer size.
        /// </summary>
        public class TextBoxStreamWriter : TextWriter
        {
            private TextBox _output;
            private StringBuilder _buffer = new StringBuilder();
            private int _maxLines;

            public TextBoxStreamWriter(TextBox output, int maxLines = 5000)
            {
                _output = output;
                _maxLines = maxLines;
            }

            public override void Write(char value)
            {
                base.Write(value);
                _buffer.Append(value);
                if (value == '\n')
                {
                    Flush();
                }
            }

            public override void Flush()
            {
                if (_output.InvokeRequired)
                {
                    _output.BeginInvoke(new Action(() =>
                    {
                        _output.AppendText(_buffer.ToString());
                        TrimLogIfNeeded();
                        _output.ScrollToCaret();
                    }));
                }
                else
                {
                    _output.AppendText(_buffer.ToString());
                    TrimLogIfNeeded();
                    _output.ScrollToCaret();
                }
                _buffer.Clear();
            }

            private void TrimLogIfNeeded()
            {
                // Trim log to prevent memory issues
                var lines = _output.Lines;
                if (lines.Length > _maxLines)
                {
                    var keepLines = lines.Skip(lines.Length - _maxLines).ToArray();
                    _output.Lines = keepLines;
                }
            }

            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        /// <summary>
        /// Updates the progress bar value safely from any thread
        /// </summary>
        private void UpdateProgress(int value)
        {
            try
            {
                if (progressBar.InvokeRequired)
                {
                    progressBar.Invoke(new Action(() => {
                        progressBar.Value = value;
                    }));
                }
                else
                {
                    progressBar.Value = value;
                }
            }
            catch (ObjectDisposedException)
            {
                // Form was closed, ignore
            }
            catch (InvalidOperationException)
            {
                // Form is being disposed, ignore
            }
        }

        /// <summary>
        /// Updates the info textbox safely from any thread
        /// </summary>
        private void UpdateTextBox(string message)
        {
            try
            {
                if (infoTextBox.InvokeRequired)
                {
                    infoTextBox.Invoke(new Action(() => {
                        infoTextBox.AppendText(message);
                    }));
                }
                else
                {
                    infoTextBox.AppendText(message);
                }
            }
            catch (ObjectDisposedException)
            {
                // Form was closed, ignore
            }
            catch (InvalidOperationException)
            {
                // Form is being disposed, ignore
            }
        }
        
        /// <summary>
        /// Helper class to write to multiple TextWriters at once
        /// Used to write to both UI and log file simultaneously
        /// </summary>
        private class MultiTextWriter : System.IO.TextWriter
        {
            private readonly TextWriter[] writers;
            
            public MultiTextWriter(params TextWriter[] writers)
            {
                this.writers = writers;
            }
            
            public override void Write(char value)
            {
                foreach (var writer in writers)
                {
                    try { writer.Write(value); } catch { }
                }
            }
            
            public override void Write(string? value)
            {
                foreach (var writer in writers)
                {
                    try { writer.Write(value); } catch { }
                }
            }
            
            public override void WriteLine(string? value)
            {
                foreach (var writer in writers)
                {
                    try { writer.WriteLine(value); } catch { }
                }
            }
            
            public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;
        }
    }
}
