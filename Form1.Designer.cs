namespace iPhoneTool;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
        this.statusStrip = new System.Windows.Forms.StatusStrip();
        this.deviceStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
        this.udidStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
        this.toolTip = new System.Windows.Forms.ToolTip(this.components);
        this.menuStrip = new System.Windows.Forms.MenuStrip();
        this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.saveLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
        this.getInfoButton = new System.Windows.Forms.Button();
        this.restoreButton = new System.Windows.Forms.Button();
        this.enterRecoveryButton = new System.Windows.Forms.Button();
        this.exitRecoveryButton = new System.Windows.Forms.Button();
        this.progressPanel = new System.Windows.Forms.Panel();
        this.progressLabel = new System.Windows.Forms.Label();
        this.progressBar = new System.Windows.Forms.ProgressBar();
        this.logPanel = new System.Windows.Forms.Panel();
        this.infoTextBox = new System.Windows.Forms.TextBox();
        this.logButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
        this.clearLogButton = new System.Windows.Forms.Button();
        this.saveLogButton = new System.Windows.Forms.Button();
        this.mainTableLayout.SuspendLayout();
        this.statusStrip.SuspendLayout();
        this.buttonPanel.SuspendLayout();
        this.progressPanel.SuspendLayout();
        this.logPanel.SuspendLayout();
        this.logButtonPanel.SuspendLayout();
        this.menuStrip.SuspendLayout();
        this.SuspendLayout();
        // 
        // menuStrip
        // 
        this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
        this.menuStrip.Location = new System.Drawing.Point(0, 0);
        this.menuStrip.Name = "menuStrip";
        this.menuStrip.Size = new System.Drawing.Size(1000, 24);
        this.menuStrip.TabIndex = 1;
        this.menuStrip.Text = "menuStrip";
        // 
        // fileToolStripMenuItem
        // 
        this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveLogToolStripMenuItem,
            this.exitToolStripMenuItem});
        this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
        this.fileToolStripMenuItem.Text = "&File";
        // 
        // saveLogToolStripMenuItem
        // 
        this.saveLogToolStripMenuItem.Name = "saveLogToolStripMenuItem";
        this.saveLogToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
        this.saveLogToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
        this.saveLogToolStripMenuItem.Text = "&Save Log...";
        this.saveLogToolStripMenuItem.Click += new System.EventHandler(this.saveLogButton_Click);
        // 
        // exitToolStripMenuItem
        // 
        this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
        this.exitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
        this.exitToolStripMenuItem.Text = "E&xit";
        this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
        // 
        // helpToolStripMenuItem
        // 
        this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
        this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
        this.helpToolStripMenuItem.Text = "&Help";
        // 
        // aboutToolStripMenuItem
        // 
        this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        this.aboutToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
        this.aboutToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
        this.aboutToolStripMenuItem.Text = "&About";
        this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
        // 
        // mainTableLayout
        // 
        this.mainTableLayout.ColumnCount = 1;
        this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.mainTableLayout.Controls.Add(this.buttonPanel, 0, 0);
        this.mainTableLayout.Controls.Add(this.progressPanel, 0, 1);
        this.mainTableLayout.Controls.Add(this.logPanel, 0, 2);
        this.mainTableLayout.Controls.Add(this.statusStrip, 0, 3);
        this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mainTableLayout.Location = new System.Drawing.Point(0, 0);
        this.mainTableLayout.Name = "mainTableLayout";
        this.mainTableLayout.Padding = new System.Windows.Forms.Padding(8);
        this.mainTableLayout.RowCount = 4;
        this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
        this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
        this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
        this.mainTableLayout.Size = new System.Drawing.Size(1000, 600);
        this.mainTableLayout.TabIndex = 0;
        // 
        // statusStrip
        // 
        this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deviceStatusLabel,
            this.udidStatusLabel});
        this.statusStrip.Location = new System.Drawing.Point(8, 574);
        this.statusStrip.Name = "statusStrip";
        this.statusStrip.Size = new System.Drawing.Size(984, 26);
        this.statusStrip.TabIndex = 0;
        this.statusStrip.Text = "statusStrip";
        // 
        // deviceStatusLabel
        // 
        this.deviceStatusLabel.Name = "deviceStatusLabel";
        this.deviceStatusLabel.Size = new System.Drawing.Size(130, 21);
        this.deviceStatusLabel.Text = "🔌 No device detected";
        this.deviceStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // udidStatusLabel
        // 
        this.udidStatusLabel.Name = "udidStatusLabel";
        this.udidStatusLabel.Size = new System.Drawing.Size(839, 21);
        this.udidStatusLabel.Spring = true;
        this.udidStatusLabel.Text = "";
        this.udidStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // buttonPanel
        // 
        this.buttonPanel.Controls.Add(this.getInfoButton);
        this.buttonPanel.Controls.Add(this.restoreButton);
        this.buttonPanel.Controls.Add(this.enterRecoveryButton);
        this.buttonPanel.Controls.Add(this.exitRecoveryButton);
        this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.buttonPanel.Location = new System.Drawing.Point(11, 11);
        this.buttonPanel.Name = "buttonPanel";
        this.buttonPanel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
        this.buttonPanel.Size = new System.Drawing.Size(978, 50);
        this.buttonPanel.TabIndex = 1;
        // 
        // getInfoButton
        // 
        this.getInfoButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.getInfoButton.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
        this.getInfoButton.Cursor = System.Windows.Forms.Cursors.Hand;
        this.getInfoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.getInfoButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.getInfoButton.ForeColor = System.Drawing.Color.White;
        this.getInfoButton.Location = new System.Drawing.Point(3, 11);
        this.getInfoButton.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
        this.getInfoButton.Name = "getInfoButton";
        this.getInfoButton.Size = new System.Drawing.Size(140, 36);
        this.getInfoButton.TabIndex = 0;
        this.getInfoButton.Text = "📱 &Get Device Info";
        this.toolTip.SetToolTip(this.getInfoButton, "Detect connected devices and display information (Alt+G)");
        this.getInfoButton.UseVisualStyleBackColor = false;
        this.getInfoButton.Click += new System.EventHandler(this.getInfoButton_Click);
        // 
        // restoreButton
        // 
        this.restoreButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.restoreButton.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
        this.restoreButton.Cursor = System.Windows.Forms.Cursors.Hand;
        this.restoreButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.restoreButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.restoreButton.ForeColor = System.Drawing.Color.White;
        this.restoreButton.Location = new System.Drawing.Point(154, 11);
        this.restoreButton.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
        this.restoreButton.Name = "restoreButton";
        this.restoreButton.Size = new System.Drawing.Size(140, 36);
        this.restoreButton.TabIndex = 1;
        this.restoreButton.Text = "💾 &Restore IPSW";
        this.toolTip.SetToolTip(this.restoreButton, "Restore or update device from IPSW firmware file (Alt+R)");
        this.restoreButton.UseVisualStyleBackColor = false;
        this.restoreButton.Click += new System.EventHandler(this.restoreButton_Click);
        // 
        // enterRecoveryButton
        // 
        this.enterRecoveryButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.enterRecoveryButton.BackColor = System.Drawing.Color.FromArgb(255, 140, 0);
        this.enterRecoveryButton.Cursor = System.Windows.Forms.Cursors.Hand;
        this.enterRecoveryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.enterRecoveryButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.enterRecoveryButton.ForeColor = System.Drawing.Color.White;
        this.enterRecoveryButton.Location = new System.Drawing.Point(305, 11);
        this.enterRecoveryButton.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
        this.enterRecoveryButton.Name = "enterRecoveryButton";
        this.enterRecoveryButton.Size = new System.Drawing.Size(140, 36);
        this.enterRecoveryButton.TabIndex = 2;
        this.enterRecoveryButton.Text = "🔄 &Enter Recovery";
        this.toolTip.SetToolTip(this.enterRecoveryButton, "Put device into recovery mode (Alt+E)");
        this.enterRecoveryButton.UseVisualStyleBackColor = false;
        this.enterRecoveryButton.Click += new System.EventHandler(this.enterRecoveryButton_Click);
        // 
        // exitRecoveryButton
        // 
        this.exitRecoveryButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.exitRecoveryButton.BackColor = System.Drawing.Color.FromArgb(16, 124, 16);
        this.exitRecoveryButton.Cursor = System.Windows.Forms.Cursors.Hand;
        this.exitRecoveryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.exitRecoveryButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.exitRecoveryButton.ForeColor = System.Drawing.Color.White;
        this.exitRecoveryButton.Location = new System.Drawing.Point(456, 11);
        this.exitRecoveryButton.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
        this.exitRecoveryButton.Name = "exitRecoveryButton";
        this.exitRecoveryButton.Size = new System.Drawing.Size(140, 36);
        this.exitRecoveryButton.TabIndex = 3;
        this.exitRecoveryButton.Text = "✅ E&xit Recovery";
        this.toolTip.SetToolTip(this.exitRecoveryButton, "Exit recovery mode and boot normally (Alt+X)");
        this.exitRecoveryButton.UseVisualStyleBackColor = false;
        this.exitRecoveryButton.Click += new System.EventHandler(this.exitRecoveryButton_Click);
        // 
        // progressPanel
        // 
        this.progressPanel.Controls.Add(this.progressLabel);
        this.progressPanel.Controls.Add(this.progressBar);
        this.progressPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.progressPanel.Location = new System.Drawing.Point(11, 64);
        this.progressPanel.Name = "progressPanel";
        this.progressPanel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 8);
        this.progressPanel.Size = new System.Drawing.Size(978, 60);
        this.progressPanel.TabIndex = 2;
        // 
        // progressLabel
        // 
        this.progressLabel.AutoSize = true;
        this.progressLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.progressLabel.Location = new System.Drawing.Point(3, 11);
        this.progressLabel.Name = "progressLabel";
        this.progressLabel.Size = new System.Drawing.Size(52, 15);
        this.progressLabel.TabIndex = 1;
        this.progressLabel.Text = "Ready";
        // 
        // progressBar
        // 
        this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.progressBar.Location = new System.Drawing.Point(0, 33);
        this.progressBar.Name = "progressBar";
        this.progressBar.Size = new System.Drawing.Size(978, 19);
        this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
        this.progressBar.TabIndex = 0;
        // 
        // logPanel
        // 
        this.logPanel.Controls.Add(this.infoTextBox);
        this.logPanel.Controls.Add(this.logButtonPanel);
        this.logPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.logPanel.Location = new System.Drawing.Point(11, 127);
        this.logPanel.Name = "logPanel";
        this.logPanel.Size = new System.Drawing.Size(978, 441);
        this.logPanel.TabIndex = 3;
        // 
        // infoTextBox
        // 
        this.infoTextBox.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
        this.infoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.infoTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
        this.infoTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.infoTextBox.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
        this.infoTextBox.Location = new System.Drawing.Point(0, 0);
        this.infoTextBox.Multiline = true;
        this.infoTextBox.Name = "infoTextBox";
        this.infoTextBox.ReadOnly = true;
        this.infoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.infoTextBox.ShortcutsEnabled = true;
        this.infoTextBox.Size = new System.Drawing.Size(978, 405);
        this.infoTextBox.TabIndex = 0;
        this.infoTextBox.Text = "Welcome to iPhone Service Tool\r\nConnect your device and click \'Get Device Info\' to start.\r\n";
        this.toolTip.SetToolTip(this.infoTextBox, "Operation log - Press Ctrl+A to select all, Ctrl+C to copy");
        // 
        // logButtonPanel
        // 
        this.logButtonPanel.Controls.Add(this.clearLogButton);
        this.logButtonPanel.Controls.Add(this.saveLogButton);
        this.logButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.logButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
        this.logButtonPanel.Location = new System.Drawing.Point(0, 405);
        this.logButtonPanel.Name = "logButtonPanel";
        this.logButtonPanel.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
        this.logButtonPanel.Size = new System.Drawing.Size(978, 36);
        this.logButtonPanel.TabIndex = 1;
        // 
        // clearLogButton
        // 
        this.clearLogButton.Cursor = System.Windows.Forms.Cursors.Hand;
        this.clearLogButton.Location = new System.Drawing.Point(891, 7);
        this.clearLogButton.Name = "clearLogButton";
        this.clearLogButton.Size = new System.Drawing.Size(84, 24);
        this.clearLogButton.TabIndex = 0;
        this.clearLogButton.Text = "🗑️ &Clear Log";
        this.toolTip.SetToolTip(this.clearLogButton, "Clear all log messages (Alt+C)");
        this.clearLogButton.UseVisualStyleBackColor = true;
        this.clearLogButton.Click += new System.EventHandler(this.clearLogButton_Click);
        // 
        // saveLogButton
        // 
        this.saveLogButton.Cursor = System.Windows.Forms.Cursors.Hand;
        this.saveLogButton.Location = new System.Drawing.Point(801, 7);
        this.saveLogButton.Name = "saveLogButton";
        this.saveLogButton.Size = new System.Drawing.Size(84, 24);
        this.saveLogButton.TabIndex = 1;
        this.saveLogButton.Text = "💾 &Save Log";
        this.toolTip.SetToolTip(this.saveLogButton, "Export log to text file (Alt+S)");
        this.saveLogButton.UseVisualStyleBackColor = true;
        this.saveLogButton.Click += new System.EventHandler(this.saveLogButton_Click);
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
        this.ClientSize = new System.Drawing.Size(1000, 600);
        this.Controls.Add(this.mainTableLayout);
        this.Controls.Add(this.menuStrip);
        this.MainMenuStrip = this.menuStrip;
        this.MinimumSize = new System.Drawing.Size(800, 500);
        this.Name = "Form1";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "iPhone Service Tool - v1.0";
        this.mainTableLayout.ResumeLayout(false);
        this.mainTableLayout.PerformLayout();
        this.statusStrip.ResumeLayout(false);
        this.statusStrip.PerformLayout();
        this.buttonPanel.ResumeLayout(false);
        this.progressPanel.ResumeLayout(false);
        this.progressPanel.PerformLayout();
        this.logPanel.ResumeLayout(false);
        this.logPanel.PerformLayout();
        this.logButtonPanel.ResumeLayout(false);
        this.menuStrip.ResumeLayout(false);
        this.menuStrip.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel mainTableLayout;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripStatusLabel deviceStatusLabel;
    private System.Windows.Forms.ToolStripStatusLabel udidStatusLabel;
    private System.Windows.Forms.FlowLayoutPanel buttonPanel;
    private System.Windows.Forms.Button getInfoButton;
    private System.Windows.Forms.Button restoreButton;
    private System.Windows.Forms.Button enterRecoveryButton;
    private System.Windows.Forms.Button exitRecoveryButton;
    private System.Windows.Forms.Panel progressPanel;
    private System.Windows.Forms.Label progressLabel;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.Panel logPanel;
    private System.Windows.Forms.TextBox infoTextBox;
    private System.Windows.Forms.FlowLayoutPanel logButtonPanel;
    private System.Windows.Forms.Button clearLogButton;
    private System.Windows.Forms.Button saveLogButton;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem saveLogToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
}
