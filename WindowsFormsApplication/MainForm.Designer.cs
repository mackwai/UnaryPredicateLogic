// somerby.net/mack/logic
// Copyright (C) 2015 MacKenzie Cumings
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

namespace WindowsFormsApplication
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
    protected override void Dispose( bool disposing )
    {
      if ( disposing && ( components != null ) )
      {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.TextBox = new System.Windows.Forms.TextBox();
      this.DecideButton = new System.Windows.Forms.Button();
      this.Result = new System.Windows.Forms.Label();
      this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.labelResult = new System.Windows.Forms.Label();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(606, 24);
      this.menuStrip1.TabIndex = 0;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.quitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "&File";
      // 
      // openToolStripMenuItem
      // 
      this.openToolStripMenuItem.Name = "openToolStripMenuItem";
      this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.openToolStripMenuItem.Text = "&Open";
      this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
      // 
      // newToolStripMenuItem
      // 
      this.newToolStripMenuItem.Name = "newToolStripMenuItem";
      this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.newToolStripMenuItem.Text = "&New";
      // 
      // saveToolStripMenuItem
      // 
      this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
      this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.saveToolStripMenuItem.Text = "&Save";
      // 
      // saveAsToolStripMenuItem
      // 
      this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
      this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.saveAsToolStripMenuItem.Text = "Save &As";
      // 
      // quitToolStripMenuItem
      // 
      this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
      this.quitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.quitToolStripMenuItem.Text = "&Quit";
      this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
      // 
      // TextBox
      // 
      this.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.TextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TextBox.Location = new System.Drawing.Point(0, 28);
      this.TextBox.Multiline = true;
      this.TextBox.Name = "TextBox";
      this.TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.TextBox.Size = new System.Drawing.Size(606, 347);
      this.TextBox.TabIndex = 1;
      this.TextBox.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
      // 
      // DecideButton
      // 
      this.DecideButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.DecideButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.DecideButton.Location = new System.Drawing.Point(11, 385);
      this.DecideButton.Name = "DecideButton";
      this.DecideButton.Size = new System.Drawing.Size(75, 23);
      this.DecideButton.TabIndex = 2;
      this.DecideButton.Text = "Decide";
      this.DecideButton.UseVisualStyleBackColor = true;
      this.DecideButton.Click += new System.EventHandler(this.DecideButton_Click);
      // 
      // Result
      // 
      this.Result.AutoSize = true;
      this.Result.Location = new System.Drawing.Point(95, 390);
      this.Result.Name = "Result";
      this.Result.Size = new System.Drawing.Size(0, 13);
      this.Result.TabIndex = 3;
      // 
      // OpenFileDialog
      // 
      this.OpenFileDialog.AddExtension = false;
      this.OpenFileDialog.Title = "Select a Text File...";
      // 
      // labelResult
      // 
      this.labelResult.AutoSize = true;
      this.labelResult.Location = new System.Drawing.Point(101, 385);
      this.labelResult.Name = "labelResult";
      this.labelResult.Size = new System.Drawing.Size(0, 13);
      this.labelResult.TabIndex = 4;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(606, 416);
      this.Controls.Add(this.labelResult);
      this.Controls.Add(this.Result);
      this.Controls.Add(this.DecideButton);
      this.Controls.Add(this.TextBox);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "MainForm";
      this.ShowIcon = false;
      this.Text = "mack/logic";
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
    private System.Windows.Forms.TextBox TextBox;
    private System.Windows.Forms.Button DecideButton;
    private System.Windows.Forms.Label Result;
    private System.Windows.Forms.OpenFileDialog OpenFileDialog;
    private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
    private System.Windows.Forms.Label labelResult;
  }
}

