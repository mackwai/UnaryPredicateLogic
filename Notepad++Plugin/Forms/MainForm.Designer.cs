// somerby.net/mack/logic
// Copyright (C) 2014 MacKenzie Cumings
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

namespace Logic
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.btnClose = new System.Windows.Forms.Button();
      this.txtResult = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // btnClose
      // 
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnClose.Location = new System.Drawing.Point(445, 446);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "&Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.button_close_Click);
      // 
      // txtResult
      // 
      this.txtResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.txtResult.Location = new System.Drawing.Point(3, 3);
      this.txtResult.Multiline = true;
      this.txtResult.Name = "txtResult";
      this.txtResult.ReadOnly = true;
      this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txtResult.Size = new System.Drawing.Size(569, 355);
      this.txtResult.TabIndex = 3;
      // 
      // ResultsDialog
      // 
      this.AcceptButton = this.btnClose;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = global::Logic.Properties.Resources.Leibniz_machine;
      this.CancelButton = this.btnClose;
      this.ClientSize = new System.Drawing.Size(575, 498);
      this.ControlBox = false;
      this.Controls.Add(this.txtResult);
      this.Controls.Add(this.btnClose);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "ResultsDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Symbolic Logic";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtResult;
    }
}