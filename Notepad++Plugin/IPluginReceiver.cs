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

namespace Logic
{
  /// <summary>
  /// An interface for an object which is used to install this plugin in an
  /// application.
  /// </summary>
  public interface IPluginReceiver
  {
    /// <summary>
    /// A method which should install callbacks which the application
    /// can use to cause the plugin to execute its functions.  The plugin's
    /// MainForm object is constructed, it will call Install() to provide
    /// these callbacks to the application.
    /// </summary>
    /// <param name="aDecideContentsOfActiveDocument">an action that the application should execute when the user commands the
    /// application to decide the contents of the application's active document</param>
    /// <param name="aDecideSelectedText">an action that the application should execute when the user commands the
    /// application to decide the selected text in the application's active document</param>
    /// <param name="aDepictContentsOfActiveDocument">an action that the application should execute when the user commands the
    /// application to depict the contents of the application's active document as graph</param>
    /// <param name="aDepictSelectedText">an action that the application should execute when the user commands the
    /// application to depict the selected text in the application's active document as a graph</param>
    /// <param name="aExecuteProver9OnActiveDocument">an action that the application should execute when the user commands the
    /// application to try to prove the contents of the active buffer with Prover9.</param>
    void Install(
      Action aDecideContentsOfActiveDocument,
      Action aDecideSelectedText,
      Action aDepictContentsOfActiveDocument,
      Action aDepictSelectedText,
      Action aExecuteProver9OnActiveDocument );

    /// <summary>
    /// The name of the application's active document (usually the name of a file opened by the application).
    /// </summary>
    string NameOfActiveDocument { get; }

    /// <summary>
    /// The text of the application's active document.
    /// </summary>
    string ContentsOfActiveDocument { get; }

    /// <summary>
    /// The text the in application's active document that is currently selected by the cursor.
    /// </summary>
    string SelectedText { get; }
  }
}
