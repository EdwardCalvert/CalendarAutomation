﻿
namespace GoogleCalender
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.DisplayClock = new System.Windows.Forms.Label();
            this.NextUp = new System.Windows.Forms.Label();
            this.debugMode = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // DisplayClock
            // 
            this.DisplayClock.BackColor = System.Drawing.Color.Transparent;
            this.DisplayClock.Font = new System.Drawing.Font("Open Sans", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DisplayClock.ForeColor = System.Drawing.Color.DimGray;
            this.DisplayClock.Location = new System.Drawing.Point(-5, -5);
            this.DisplayClock.Name = "DisplayClock";
            this.DisplayClock.Size = new System.Drawing.Size(91, 42);
            this.DisplayClock.TabIndex = 2;
            this.DisplayClock.Text = "Loading...";
            // 
            // NextUp
            // 
            this.NextUp.AutoSize = true;
            this.NextUp.Location = new System.Drawing.Point(-1, 34);
            this.NextUp.Name = "NextUp";
            this.NextUp.Size = new System.Drawing.Size(169, 15);
            this.NextUp.TabIndex = 3;
            this.NextUp.Text = "Please wait for synchronisation";
            this.NextUp.Click += new System.EventHandler(this.NextUp_Click);
            // 
            // debugMode
            // 
            this.debugMode.AutoSize = true;
            this.debugMode.Checked = true;
            this.debugMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.debugMode.Location = new System.Drawing.Point(171, 12);
            this.debugMode.Name = "debugMode";
            this.debugMode.Size = new System.Drawing.Size(15, 14);
            this.debugMode.TabIndex = 4;
            this.debugMode.UseVisualStyleBackColor = true;
            this.debugMode.CheckedChanged += new System.EventHandler(this.debugMode_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(189, 52);
            this.Controls.Add(this.debugMode);
            this.Controls.Add(this.NextUp);
            this.Controls.Add(this.DisplayClock);
            this.Cursor = System.Windows.Forms.Cursors.No;
            this.Font = new System.Drawing.Font("Open Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Opacity = 0.5D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Clock";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label DisplayClock;
        private System.Windows.Forms.Label NextUp;
        private System.Windows.Forms.CheckBox debugMode;
    }
}

