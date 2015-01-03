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

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using NppPluginNET;

namespace Logic
{
  class NppPluginReceiver : IPluginReceiver
  {
    #region " Fields "
    internal const string PluginName = "Symbolic Logic";
    const int IndexOfTheCommandThatGetsTheToolBarIcon = 0;

    private static readonly NppPluginReceiver Instance = new NppPluginReceiver();

    private static MainForm ThePlugin;

    private static Action mDecideContentsOfActiveDocument = null;
    private static Action mDecideSelectedText = null;
    private static Action mDepictContentsOfActiveDocument = null;
    private static Action mDepictSelectedText = null;

    #endregion

    #region " StartUp/CleanUp "
    internal static void CommandMenuInit()
    {
      ThePlugin = new MainForm( Instance );
      StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
      Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);

      PluginBase.SetCommand( 0, "Decide", DecideContentsOfActiveDocument, new ShortcutKey( true, false, false, Keys.NumPad0 ) );
      PluginBase.SetCommand( 1, "Decide Selected Text", DecideSelectedText, new ShortcutKey( true, true, false, Keys.NumPad0 ) );
      if ( mDepictContentsOfActiveDocument != null )
        PluginBase.SetCommand( 2, "Depict", DepictContentsOfActiveDocument, new ShortcutKey( false, true, false, Keys.G ) );
      if ( mDepictSelectedText != null )
        PluginBase.SetCommand( 3, "Depict Selected Text", DepictSelectedText, new ShortcutKey( false, true, true, Keys.G ) );
    }

    internal static void SetToolBarIcon()
    {       
      toolbarIcons tbIcons = new toolbarIcons();
      tbIcons.hToolbarBmp = new Bitmap( @"plugins\Config\Decide.bmp" ).GetHbitmap();
      IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
      Marshal.StructureToPtr(tbIcons, pTbIcons, false);
      Win32.SendMessage(
        PluginBase.nppData._nppHandle, NppMsg.NPPM_ADDTOOLBARICON,
        PluginBase._funcItems.Items[ IndexOfTheCommandThatGetsTheToolBarIcon ]._cmdID, pTbIcons );
      Marshal.FreeHGlobal(pTbIcons);   
    }

    internal static void PluginCleanUp()
    {
    }
    #endregion

    private static void DecideContentsOfActiveDocument()
    {
      if ( mDecideContentsOfActiveDocument != null )
        mDecideContentsOfActiveDocument();
    }

    private static void DecideSelectedText()
    {
      if ( mDecideSelectedText != null )
        mDecideSelectedText();
    }

    private static void DepictContentsOfActiveDocument()
    {
      if ( mDepictContentsOfActiveDocument != null )
        mDepictContentsOfActiveDocument();
    }

    private static void DepictSelectedText()
    {
      if ( mDepictSelectedText != null )
        mDepictSelectedText();
    }

    #region " IPluginReceiver interface "
    public void Install(
      Action aDecideContentsOfActiveDocument,
      Action aDecideSelectedText,
      Action aDepictContentsOfActiveDocument,
      Action aDepictSelectedText )
    {
      mDecideContentsOfActiveDocument = aDecideContentsOfActiveDocument;
      mDecideSelectedText = aDecideSelectedText;
      mDepictContentsOfActiveDocument = aDepictContentsOfActiveDocument;
      mDepictSelectedText = aDepictSelectedText;
    }

    public string NameOfActiveDocument
    {
      get
      {
        StringBuilder lActiveFileName = new StringBuilder( Win32.MAX_PATH );
        Win32.SendMessage( PluginBase.nppData._nppHandle, NppMsg.NPPM_GETFILENAME, Win32.MAX_PATH, lActiveFileName );
        return lActiveFileName.ToString();
      }
    }

    public string ContentsOfActiveDocument
    {
      get
      {
        IntPtr lActiveScintilla = PluginBase.GetCurrentScintilla();
        int lBufferLength = 1 + (int) Win32.SendMessage( lActiveScintilla, SciMsg.SCI_GETLENGTH, 0, 0 );

        /*This returns length-1 characters of text from the start of the document plus one terminating 0 character. To collect all the
          * text in a document, use SCI_GETLENGTH to get the number of characters in the document (nLen), allocate a character buffer
          * of length nLen+1 bytes, then call SCI_GETTEXT(nLen+1, char *text). If the text argument is 0 then the length that should be
          * allocated to store the entire document is returned. If you then save the text, you should use SCI_SETSAVEPOINT to mark the
          * text as unmodified.*/

        // Walkaround the crash problem if length is 86016
        try
        {
          IntPtr lBuffer = Marshal.AllocHGlobal( lBufferLength == 86016 ? 86017 : lBufferLength );
          Win32.SendMessage( lActiveScintilla, SciMsg.SCI_GETTEXT, lBufferLength, lBuffer );
          byte[] lBytes = new byte[ lBufferLength ];
          Marshal.Copy( lBuffer, lBytes, 0, lBufferLength );
          Marshal.FreeHGlobal( lBuffer );

          return System.Text.Encoding.Default.GetString( lBytes ).TrimEnd( '\0' );
        }
        catch ( Exception ex )
        {
          throw new ApplicationException( "Buffer Length : " + lBufferLength + "\n" + ex.ToString() );
        }
      }
    }

    public string SelectedText
    {
      get
      {
        IntPtr lActiveScintilla = PluginBase.GetCurrentScintilla();

        int textLen = Math.Abs(
          (int) Win32.SendMessage( lActiveScintilla, SciMsg.SCI_GETSELECTIONEND, 0, 0 )
          - (int) Win32.SendMessage( lActiveScintilla, SciMsg.SCI_GETSELECTIONSTART, 0, 0 ) );

        //if (textLen == 0) return null;

        // Walkaround the crash problem if length is 86016
        try
        {
          IntPtr lBuffer = Marshal.AllocHGlobal( textLen == 86016 ? 86017 : textLen );
          Win32.SendMessage( lActiveScintilla, SciMsg.SCI_GETSELTEXT, 200, lBuffer );
          byte[] lBytes = new byte[ textLen ];
          Marshal.Copy( lBuffer, lBytes, 0, textLen );
          Marshal.FreeHGlobal( lBuffer );

          return System.Text.Encoding.Default.GetString( lBytes );
        }
        catch ( Exception ex )
        {
          throw new ApplicationException( "Buffer Length : " + textLen + "\n" + ex.ToString() );
        }
      }
    }
  }
  #endregion
}