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
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logic
{
  [TestClass]
  public class ConsoleApplicationTesting
  {

//  Process out = new Process("program1.exe", "-some -options");
//Process in = new Process("program2.exe", "-some -options");

//out.UseShellExecute = false;

//out.RedirectStandardOutput = true;
//in.RedirectStandardInput = true;

//using(StreamReader sr = new StreamReader(out.StandardOutput))
//using(StreamWriter sw = new StreamWriter(in.StandardInput))
//{
//  string line;
//  while((line = sr.ReadLine()) != null)
//  {
//    sw.WriteLine(line);
//  }
//}
    private static string ExecuteAndReturnOutput( string aFileName, string aArguments, string aStandardInput = null )
    {
      using ( Process lProcess = new Process() )
      {
        lProcess.StartInfo = new ProcessStartInfo( aFileName, aArguments );
        lProcess.StartInfo.RedirectStandardInput = true;
        lProcess.StartInfo.RedirectStandardOutput = true;
        lProcess.StartInfo.UseShellExecute = false;
        lProcess.Start();
        if ( aStandardInput != null )
        {
          using ( StreamWriter lInput = lProcess.StandardInput )
          {
            lInput.Write( aStandardInput );
            lInput.Close();            
          }
        }

        using ( StreamReader lOutput = lProcess.StandardOutput )
        {
          string lString = lOutput.ReadToEnd();

          lProcess.WaitForExit();
          return lString;
        }
      }
    }

    private static int ExecuteAndReturnExitCode( string aFileName, string aArguments, string aStandardInput = null )
    {
      using ( Process lProcess = new Process() )
      {
        lProcess.StartInfo = new ProcessStartInfo( aFileName, aArguments );
        lProcess.StartInfo.RedirectStandardInput = true;
        lProcess.StartInfo.UseShellExecute = false;
        lProcess.Start();
        if ( aStandardInput != null )
        {
          using ( StreamWriter lInput = lProcess.StandardInput )
          {
            lInput.Write( aStandardInput );
            lInput.Close();
          }
        }

        lProcess.WaitForExit();

        return lProcess.ExitCode;
      }
    }

    private const string PathToConsoleApplication = @"..\..\..\upl.exe";

    [TestMethod]
    public void Test_Help()
    {
      StringAssert.StartsWith( ExecuteAndReturnOutput( PathToConsoleApplication, "-h" ).ToLower(), "usage" );
      StringAssert.StartsWith( ExecuteAndReturnOutput( PathToConsoleApplication, "--help" ).ToLower(), "usage" );
    }

    [TestMethod]
    public void Test_Silent()
    {
      foreach ( string lArgument in new string[2] { @"-s", @"--silent" } )
      {
        Assert.IsTrue( ExecuteAndReturnExitCode( PathToConsoleApplication, lArgument, ".." ) < 0 );
        Assert.AreEqual( 0, ExecuteAndReturnExitCode( PathToConsoleApplication, lArgument, "P|~P" ) );
        Assert.AreEqual( 1, ExecuteAndReturnExitCode( PathToConsoleApplication, lArgument, "P" ) );
        Assert.AreEqual( 2, ExecuteAndReturnExitCode( PathToConsoleApplication, lArgument, "P&~P" ) );
      }
    }

    [TestMethod]
    public void Test_Basic()
    {
      Assert.AreEqual( "Error\r\n", ExecuteAndReturnOutput( PathToConsoleApplication, null, ".." ) );
      Assert.AreEqual( "Necessary\r\n", ExecuteAndReturnOutput( PathToConsoleApplication, null, "P|~P" ) );
      Assert.AreEqual( "Contingent\r\n", ExecuteAndReturnOutput( PathToConsoleApplication, null, "P" ) );
      Assert.AreEqual( "Impossible\r\n", ExecuteAndReturnOutput( PathToConsoleApplication, null, "P&~P" ) );
    }

    [TestMethod]
    public void Test_Graph()
    {
      foreach ( string lArgument in new string[2] { @"-g", @"--graph" } )
      {
        StringAssert.StartsWith(
          ExecuteAndReturnOutput( PathToConsoleApplication, lArgument, "P" ),
          "digraph " );
      }
    }

    [TestMethod]
    public void Test_Query()
    {
      foreach ( string lArgument in new string[2] { @"-q", @"--query" } )
      {
        Assert.AreEqual(
          "Necessary\r\nContingent\r\nImpossible\r\n",
          ExecuteAndReturnOutput( PathToConsoleApplication, lArgument, "P|~P\n?\nP\n?\nP&~P\n?\n" ) );
      }
    }
  }
}
