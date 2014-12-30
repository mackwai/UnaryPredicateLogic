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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Logic
{
  /// <summary>
  /// The plugin consists of a single instance of this form.  Code which installs this plugin
  /// must create an instance of an object that implements the IPluginReceiver interface, then
  /// pass that object to this form's constructor.
  /// </summary>
  public partial class MainForm : Form
  {
    private const string dot = "dot.exe";
    private const string ImageFileNameExtension = "png";
    private const int TimeLimitForDepiction = 30000;

    private static List<string> gFilesToDelete = new List<string>();

    private IPluginReceiver mApplication;

    /// <summary>
    /// Install a cleanup subroutine to be run when the application exits.
    /// </summary>
    static MainForm()
    {
      AppDomain.CurrentDomain.ProcessExit += Cleanup;
    }

    /// <summary>
    /// Construct the plugin's form.  Attach the plugin to the application.
    /// </summary>
    /// <param name="aApplication">an interface to the application that will receive this plugin</param>
    public MainForm( IPluginReceiver aApplication )
    {
      InitializeComponent();
      mApplication = aApplication;
      bool lDepictionAvailable = ExistsInPath( dot );
      mApplication.Install(
        DecideActiveBuffer,
        DecideSelectedText,
        lDepictionAvailable ? DepictActiveBuffer : (Action) null,
        lDepictionAvailable ? DepictSelectedText : (Action) null );
    }

    private void DepictActiveBuffer()
    {
      Depict( mApplication.NameOfActiveDocument, mApplication.ContentsOfActiveDocument );     
    }

    private void DepictSelectedText()
    {
      Depict(
        string.Format( "{0} (selected text)", mApplication.NameOfActiveDocument ),
        mApplication.SelectedText );
    }

    private void DecideActiveBuffer()
    {
      ActOnDialog( () =>
      {
        string lActiveFileName = mApplication.NameOfActiveDocument;
        this.Log( "Deciding {0}...", lActiveFileName );
        this.Log(
          "{0} is {1}",
          lActiveFileName,
          Logic.Parser.Parse( mApplication.ContentsOfActiveDocument.Split( '\n' ) ).Decide().ToString() );
      } );
    }

    private void DecideSelectedText()
    {
      ActOnDialog( () =>
      {
        string lSelectedText = mApplication.SelectedText;
        this.Log( "Deciding \"{0}\" ...", lSelectedText );
        this.Log( Logic.Parser.Parse( lSelectedText.Split( '\n' ) ).Decide().ToString() );
      } );
    }

    private void ActOnDialog( Action aAction )
    {
      if ( !this.Visible )
        this.Show();
      this.BringToFront();
      //Cursor.Current = Cursors.WaitCursor;
      try
      {
        aAction();
      }
      catch ( Exception lException )
      {
        this.Log( "Error: {0}", lException.Message );
      }
      finally
      {
        //Cursor.Current = Cursors.Default;
      }
    }

    private void button_close_Click( object sender, EventArgs e )
    {
      this.Hide();
      txtResult.Text = "";
    }

    private void Log( string aFormatString, params object[] aParameters )
    {
      txtResult.AppendText( string.Format(
        "{0}: {1}\r\n",
        DateTime.Now.TimeOfDay,
        string.Format( aFormatString, aParameters ) ) );
      Application.DoEvents();
    }

    public static void Depict( string aFileName, string aText )
    {
      try
      {
        string lOutputFileName = Path.ChangeExtension( aFileName, ImageFileNameExtension );
        string lOutputFilePath = Path.Combine( Path.GetTempPath(), lOutputFileName );

        Process lProcess = new System.Diagnostics.Process();
        lProcess.StartInfo.WorkingDirectory = Path.GetTempPath();
        lProcess.StartInfo.UseShellExecute = false;
        lProcess.StartInfo.FileName = dot;
        lProcess.StartInfo.RedirectStandardInput = true;
        lProcess.StartInfo.CreateNoWindow = true;
        lProcess.StartInfo.Arguments = string.Format( "-T{0} -o\"{1}\"", ImageFileNameExtension, lOutputFileName );
      
        lProcess.Start();
        lProcess.StandardInput.Write( Logic.Parser.Parse( aText.Split( '\n' ) ).GraphvizDOT );
        lProcess.StandardInput.Close();
        lProcess.WaitForExit( TimeLimitForDepiction );

        if ( lProcess.ExitCode == 0 && File.Exists( lOutputFilePath ) )
        {
          Process.Start( lOutputFilePath );
          if ( !gFilesToDelete.Contains( lOutputFilePath ) )
            gFilesToDelete.Add( lOutputFilePath );
        }
        else
          MessageBox.Show( "Depiction failed for some reason." );
      }
      catch ( Exception lException )
      {
        MessageBox.Show( lException.Message );
      }
    }

    private static bool ExistsInPath( string aFileName )
    {
      aFileName = Environment.ExpandEnvironmentVariables( aFileName );

      if ( File.Exists( aFileName ) )
        return true;

      if ( Path.GetDirectoryName( aFileName ) != String.Empty )
        return false;

      return ( Environment.GetEnvironmentVariable( "PATH" ) ?? "" ).Split( ';' ).Any( fPath => File.Exists( Path.Combine( fPath, aFileName ) ) );
    }

    /// <summary>
    /// Delete the image files that have been created by this plugin since the application started.
    /// </summary>
    /// <param name="sender">the event's sender</param>
    /// <param name="e">event arguments</param>
    static void Cleanup( object sender, EventArgs e )
    {
      foreach ( string lFile in gFilesToDelete )
      {
        try
        {
          File.Delete( lFile );
        }
        catch ( Exception )
        {
        }
      }
    }
  }
}
