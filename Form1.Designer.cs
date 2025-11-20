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
        this.getInfoButton = new System.Windows.Forms.Button();
        this.restoreButton = new System.Windows.Forms.Button();
        this.enterRecoveryButton = new System.Windows.Forms.Button();
        this.exitRecoveryButton = new System.Windows.Forms.Button();
        this.progressBar = new System.Windows.Forms.ProgressBar();
        this.infoTextBox = new System.Windows.Forms.TextBox();
        this.SuspendLayout();
        // 
        // getInfoButton
        // 
        this.getInfoButton.Location = new System.Drawing.Point(12, 12);
        this.getInfoButton.Name = "getInfoButton";
        this.getInfoButton.Size = new System.Drawing.Size(120, 23);
        this.getInfoButton.TabIndex = 0;
        this.getInfoButton.Text = "Get Device Info";
        this.getInfoButton.UseVisualStyleBackColor = true;
        this.getInfoButton.Click += new System.EventHandler(this.getInfoButton_Click);
        // 
        // restoreButton
        // 
        this.restoreButton.Location = new System.Drawing.Point(138, 12);
        this.restoreButton.Name = "restoreButton";
        this.restoreButton.Size = new System.Drawing.Size(120, 23);
        this.restoreButton.TabIndex = 2;
        this.restoreButton.Text = "Restore IPSW";
        this.restoreButton.UseVisualStyleBackColor = true;
        this.restoreButton.Click += new System.EventHandler(this.restoreButton_Click);
        // 
        // enterRecoveryButton
        // 
        this.enterRecoveryButton.Location = new System.Drawing.Point(264, 12);
        this.enterRecoveryButton.Name = "enterRecoveryButton";
        this.enterRecoveryButton.Size = new System.Drawing.Size(120, 23);
        this.enterRecoveryButton.TabIndex = 3;
        this.enterRecoveryButton.Text = "Enter Recovery";
        this.enterRecoveryButton.UseVisualStyleBackColor = true;
        this.enterRecoveryButton.Click += new System.EventHandler(this.enterRecoveryButton_Click);
        // 
        // exitRecoveryButton
        // 
        this.exitRecoveryButton.Location = new System.Drawing.Point(390, 12);
        this.exitRecoveryButton.Name = "exitRecoveryButton";
        this.exitRecoveryButton.Size = new System.Drawing.Size(120, 23);
        this.exitRecoveryButton.TabIndex = 4;
        this.exitRecoveryButton.Text = "Exit Recovery";
        this.exitRecoveryButton.UseVisualStyleBackColor = true;
        this.exitRecoveryButton.Click += new System.EventHandler(this.exitRecoveryButton_Click);
        // 
        // progressBar
        // 
        this.progressBar.Location = new System.Drawing.Point(516, 12);
        this.progressBar.Name = "progressBar";
        this.progressBar.Size = new System.Drawing.Size(272, 23);
        this.progressBar.TabIndex = 5;
        // 
        // infoTextBox
        // 
        this.infoTextBox.Location = new System.Drawing.Point(12, 41);
        this.infoTextBox.Multiline = true;
        this.infoTextBox.Name = "infoTextBox";
        this.infoTextBox.ReadOnly = true;
        this.infoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.infoTextBox.Size = new System.Drawing.Size(776, 397);
        this.infoTextBox.TabIndex = 1;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.progressBar);
        this.Controls.Add(this.exitRecoveryButton);
        this.Controls.Add(this.enterRecoveryButton);
        this.Controls.Add(this.infoTextBox);
        this.Controls.Add(this.restoreButton);
        this.Controls.Add(this.getInfoButton);
        this.Name = "Form1";
        this.Text = "iPhone Tool";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Button getInfoButton;
    private System.Windows.Forms.Button restoreButton;
    private System.Windows.Forms.Button enterRecoveryButton;
    private System.Windows.Forms.Button exitRecoveryButton;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.TextBox infoTextBox;
}
