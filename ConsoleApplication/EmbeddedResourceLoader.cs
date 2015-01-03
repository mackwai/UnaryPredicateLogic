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
using System.Reflection;

namespace Logic
{
  internal static class EmbeddedResourceLoader
  {
    private static bool mResolutionInstalled = false;

    /// <summary>
    /// If it isn't already installed, install an event handler that will guide the runtime to DLLs embedded in this assembly.
    /// </summary>
    public static void InstallAssemblyResolution()
    {
      if ( mResolutionInstalled )
        return;

      AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( GetEmbeddedAssembly );
      mResolutionInstalled = true;
    }

    private static Assembly GetEmbeddedAssembly( object aSender, ResolveEventArgs aArgs )
    {
      string lResourceName = string.Format(
        "Logic.{0}.dll",
        ( new AssemblyName( aArgs.Name ) ).Name );

      using ( var lStream = Assembly.GetAssembly(typeof(Program)).GetManifestResourceStream( lResourceName ) )
      {
        if ( lStream == null )
          return null;

        Byte[] lAssemblyData = new Byte[ lStream.Length ];

        lStream.Read( lAssemblyData, 0, lAssemblyData.Length );

        return Assembly.Load( lAssemblyData );
      }
    }
  }
}
