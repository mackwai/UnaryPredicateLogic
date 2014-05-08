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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
    }

    private void quitToolStripMenuItem_Click( object sender, EventArgs e )
    {
      Application.Exit();
    }

    private void openToolStripMenuItem_Click( object sender, EventArgs e )
    {
      if ( DialogResult.OK == OpenFileDialog.ShowDialog(this) )
      {
        TextBox.Lines = File.ReadAllLines( OpenFileDialog.FileName );
      }
    }

    private void DecideButton_Click( object sender, EventArgs e )
    {
      try
      {
        labelResult.Text = Logic.Parser.Parse( TextBox.Lines ).Decide().ToString();
      }
      catch ( Exception lException )
      {
        labelResult.Text = lException.Message;
      }
    }

    private void TextBox_TextChanged( object sender, EventArgs e )
    {
      labelResult.Text = "";
    }
  }
}
