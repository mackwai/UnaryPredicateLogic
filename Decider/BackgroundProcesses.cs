// somerby.net/mack/logic
// Copyright (C) 2016 MacKenzie Cumings
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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Logic
{
  internal static class BackgroundProcesses
  {
    public static Process SetBackgroundProcessRunning( ShellParameters aParameters )
    {
      Process lProcess = new System.Diagnostics.Process();
      lProcess.StartInfo.WorkingDirectory = Path.GetTempPath();
      lProcess.StartInfo.UseShellExecute = false;
      lProcess.StartInfo.FileName = aParameters.Executable;
      lProcess.StartInfo.RedirectStandardInput = true;
      lProcess.StartInfo.RedirectStandardOutput = true;
      lProcess.StartInfo.CreateNoWindow = true;
      if ( aParameters.Arguments != null && aParameters.Arguments.Length > 0 )
        lProcess.StartInfo.Arguments = aParameters.Arguments;

      lProcess.Start();
      lProcess.StandardInput.WriteLine( aParameters.StandardInput );
      lProcess.StandardInput.Close();

      return lProcess;
    }

    public static void AttemptToKill( Process aProcess )
    {
      try
      {
        if ( !aProcess.HasExited )
          aProcess.Kill();
      }
      catch ( Exception lException )
      {
        Console.WriteLine( "killing: " + lException.ToString() );
      }
      aProcess.Close();
    }

    public static Action ExcuteUntilExitOrCancellation(
      CancellationToken aCancellationToken,
      ShellParameters aShellParameters,
      Action<string> aActOnProcessOutput )
    {
      return () =>
      {
        using ( Process lBackgroundProcess = BackgroundProcesses.SetBackgroundProcessRunning( aShellParameters ) )
        {
          // Start a task to read all of STDOUT from the background process.  The process will exit
          // if and when the process closes STDOUT and this task reads it to the end.
          Task<string> lReadingStdout = lBackgroundProcess.StandardOutput.ReadToEndAsync();

          // Wait on the task.  If the task is cancelled, an OperationCanceledException will be raised.
          // If the task is cancelled, the background process probably hasn't exited, so try to kill it,
          // else it won't exit.
          try
          {
            lReadingStdout.Wait( aCancellationToken );
          }
          catch ( System.OperationCanceledException )
          {
            BackgroundProcesses.AttemptToKill( lBackgroundProcess );
          }

          aActOnProcessOutput( lReadingStdout.Result );
        }
      };
    }
  }
}
