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
using System.IO;
using System.Linq;

namespace Logic
{
  class Program
  {
    static Program()
    {
      EmbeddedResourceLoader.InstallAssemblyResolution();
    }

    /// <summary>
    /// Each command-line argument is interpreted as a path to a file containing a single proposition or argument.  Each file
    /// is parsed and its contents are tested for validity, contingency and inconsistency.
    /// </summary>
    /// <param name="aCommandLineArguments">an array of paths to files containing propositions or arguments</param>
    public static void Main( string[] aCommandLineArguments )
    {
      if ( aCommandLineArguments.Length == 0
        || aCommandLineArguments.Intersects( Utility.MakeArray( "-h", "--help", "/?" ) ) )
      {
        PrintUsage();
      }
      else if ( aCommandLineArguments[0].IsOneOf( Utility.MakeArray( "-g", "--graph", "/g" ) ) )
      {
        MakeGraphForPropositionInFile( aCommandLineArguments[ 1 ] );
      } 
      else
      {
        foreach ( string lPath in aCommandLineArguments )
        {
          DecidePropositionInFile( lPath );
        }

#if DEBUG
        Console.ReadLine();
#endif
      }
    }

    private static void MakeGraphForPropositionInFile( string aPath )
    {
      if ( !File.Exists( aPath ) )
      {
        Console.WriteLine( "File not found: {0}", aPath );
        return;
      }

      try
      {
        string[] lFileContents = File.ReadAllLines( aPath );
        Matrix lProposition = Parser.Parse( lFileContents );
        Console.WriteLine( lProposition.GraphvizDOT );
      }
      catch ( Exception lException )
      {
        Console.WriteLine( "invalid: {0}: {1}", lException.GetType(), lException.Message );
      }
    }

    private static void PrintUsage()
    {
      Console.WriteLine( "Usage: mpl.exe [proposition file] [another proposition file] ..." );
      Console.WriteLine();
      Console.WriteLine( "Parse each file in the command line and decide if the proposition in the is " );
      Console.WriteLine( "is necessarily true, contingent, or self-contradictory." );
    }

    private static void DecidePropositionInFile( string aPath )
    {
      if ( !File.Exists( aPath ) )
      {
        Console.WriteLine( "File not found: {0}", aPath );
        return;
      }
      
      try
      {
        string[] lFileContents = File.ReadAllLines( aPath );
        Matrix lProposition = Parser.Parse( lFileContents );
        //Console.WriteLine( lProposition );
        //DateTime lInitialTime = DateTime.Now;
        if ( lFileContents.Any( fLine => fLine.Trim() == "->" ) )
        {
          Console.Write( "The argument in {0} is ", aPath );
          if ( lProposition.Valid )
            Console.WriteLine( "valid." );
          else
            Console.WriteLine( "invalid." );
        }
        else
        {
          Console.WriteLine( "The contents of {0} are {1}.", aPath, Utility.TextForDecision( lProposition.Decide() ) );
        }
        //Console.WriteLine( DateTime.Now - lInitialTime );
      }
      catch ( Exception lException )
      {
        Console.WriteLine( "invalid: {0}: {1}", lException.GetType(), lException.Message );
      }
    }
  }
}
