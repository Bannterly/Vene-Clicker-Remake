namespace VeneClicker
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toggleButton = new System.Windows.Forms.Button();
            this.onlyMinecraftCheckBox = new System.Windows.Forms.CheckBox();
            this.cpsDropsCheckBox = new System.Windows.Forms.CheckBox();
            this.cpsRangeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // toggleButton
            // 
            this.toggleButton.Location = new System.Drawing.Point(12, 40);
            this.toggleButton.Name = "toggleButton";
            this.toggleButton.Size = new System.Drawing.Size(200, 30);
            this.toggleButton.TabIndex = 0;
            this.toggleButton.Text = "Set Hotkey";
            this.toggleButton.UseVisualStyleBackColor = true;
            // 
            // onlyMinecraftCheckBox
            // 
            this.onlyMinecraftCheckBox.AutoSize = true;
            this.onlyMinecraftCheckBox.Location = new System.Drawing.Point(12, 80);
            this.onlyMinecraftCheckBox.Name = "onlyMinecraftCheckBox";
            this.onlyMinecraftCheckBox.Size = new System.Drawing.Size(102, 17);
            this.onlyMinecraftCheckBox.TabIndex = 1;
            this.onlyMinecraftCheckBox.Text = "Only in Minecraft";
            this.onlyMinecraftCheckBox.UseVisualStyleBackColor = true;
            this.onlyMinecraftCheckBox.CheckedChanged += new System.EventHandler(this.onlyMinecraftCheckBox_CheckedChanged);
            // 
            // cpsDropsCheckBox
            // 
            this.cpsDropsCheckBox.AutoSize = true;
            this.cpsDropsCheckBox.Location = new System.Drawing.Point(120, 80);
            this.cpsDropsCheckBox.Name = "cpsDropsCheckBox";
            this.cpsDropsCheckBox.Size = new System.Drawing.Size(77, 17);
            this.cpsDropsCheckBox.TabIndex = 2;
            this.cpsDropsCheckBox.Text = "CPS Drops";
            this.cpsDropsCheckBox.UseVisualStyleBackColor = true;
            this.cpsDropsCheckBox.CheckedChanged += new System.EventHandler(this.cpsDropsCheckBox_CheckedChanged);
            // 
            // cpsRangeLabel
            // 
            this.cpsRangeLabel.AutoSize = true;
            this.cpsRangeLabel.Location = new System.Drawing.Point(10, 110);
            this.cpsRangeLabel.Name = "cpsRangeLabel";
            this.cpsRangeLabel.Size = new System.Drawing.Size(85, 13);
            this.cpsRangeLabel.TabIndex = 5;
            this.cpsRangeLabel.Text = "CPS Range: 5-15";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 260);
            this.Controls.Add(this.cpsRangeLabel);
            this.Controls.Add(this.cpsDropsCheckBox);
            this.Controls.Add(this.onlyMinecraftCheckBox);
            this.Controls.Add(this.toggleButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.Text = "VeneClicker";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button toggleButton;
        private System.Windows.Forms.CheckBox onlyMinecraftCheckBox;
        private System.Windows.Forms.CheckBox cpsDropsCheckBox;
        private System.Windows.Forms.Label cpsRangeLabel;
    }
}