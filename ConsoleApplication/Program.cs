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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Logic
{
  class Program
  {
    static Program()
    {
      EmbeddedResourceLoader.InstallAssemblyResolution();
    }

    static string[] ReadAllOfStdin()
    {
      List<string> lLines = new List<string>();
      string lLine;

      while ( ( lLine = System.Console.In.ReadLine() ) != null )
      {
        lLines.Add( lLine );
      }

      return lLines.ToArray();
    }

    static string[] ReadStdinUntilQuestionMark()
    {
      List<string> lLines = new List<string>();
      string lLine = Console.In.ReadLine();

      while ( lLine != null )
      {
        //if ( Regex.IsMatch( lLine, "((?<!.*//.*)\\?" ) )
        if ( lLine.Trim() ==  "?" )
          break;

        lLines.Add( lLine );

        lLine = Console.In.ReadLine();
      }

      return lLine == null ? null : lLines.ToArray();
    }

    private enum Mode
    {
      Basic,
      Query,
      Silent,
      Graph,
      Usage
    };

    private static Mode ChooseMode( string[] aCommandLineArguments )
    {
      if ( aCommandLineArguments.Intersects( Utility.MakeArray( "-q", "--query" ) ) )
        return Mode.Query;
      else if ( aCommandLineArguments.Intersects( Utility.MakeArray( "-s", "--silent" ) ) )
        return Mode.Silent;
      else if ( aCommandLineArguments.Intersects( Utility.MakeArray( "-g", "--graph" ) ) )
        return Mode.Graph;
      else if ( aCommandLineArguments.Intersects( Utility.MakeArray( "-h", "--help" ) ) )
        return Mode.Usage;
      else
        return Mode.Basic;
    }

    private static void PrintUsageAndQuit()
    {
      PrintUsage();
      Environment.Exit( -1 );
    }

    private static string[] GetInputText( string[] aCommandLineArguments )
    {
      string[] lPossibleFileArguments = aCommandLineArguments.Where( fArgument => !fArgument.StartsWith( @"-" ) ).ToArray();

      switch ( lPossibleFileArguments.Length )
      {
        case 0:
          return ReadAllOfStdin();
        case 1:
          string lPathToFile = lPossibleFileArguments[0];
          if ( File.Exists( lPathToFile ) )
            return File.ReadLines( lPathToFile ).ToArray();
          else
            throw new Exception( string.Format( "File \"{0}\" not found.", lPathToFile ) );
        default:
          PrintUsageAndQuit();
          return null;
      }
    }

    private static void AnswerQueries()
    {
      string[] lLines = ReadStdinUntilQuestionMark();
      while ( lLines != null )
      {
        try
        {
          Console.WriteLine( Parser.Parse( lLines ).Decide().ToString() );
        }
        catch ( Exception lException )
        {
          Console.Error.WriteLine( lException.Message );
          Console.WriteLine( "Error" );
        }
        lLines = ReadStdinUntilQuestionMark();
      }
    }

    /// <summary>
    /// Each command-line argument is interpreted as a path to a file containing a single proposition or argument.  Each file
    /// is parsed and its contents are tested for validity, contingency and inconsistency.
    /// </summary>
    /// <param name="aCommandLineArguments">an array of paths to files containing propositions or arguments</param>
    public static int Main( string[] aCommandLineArguments )
    {
      try
      {
        string[] lOtherArguments = aCommandLineArguments.Where( fArgument => !fArgument.StartsWith( @"-" ) ).ToArray();

        switch ( ChooseMode( aCommandLineArguments ) )
        {
          case Mode.Basic:
            Alethicity lResult;
            try
            {
              lResult = Parser.Parse( GetInputText( aCommandLineArguments ) ).Decide();              
            }
            catch ( Exception lException )
            {
              Console.WriteLine( "Error" );
              throw lException;
            }
            Console.WriteLine( lResult.ToString() );
            return (int) lResult;
          case Mode.Graph:
            Console.WriteLine( Parser.Parse( GetInputText( aCommandLineArguments ) ).GraphvizDOT );
            break;
          case Mode.Silent:
            return (int) Parser.Parse( GetInputText( aCommandLineArguments ) ).Decide();
          case Mode.Usage:
            PrintUsageAndQuit();
            return -1;
          case Mode.Query:
            AnswerQueries();
            break;
        }
      }
      catch ( Exception lException )
      {
        Console.Error.WriteLine( "Error: {0}", lException.Message );
        return -1; 
      }
      
      return 0;   
    }

    private static int MakeGraphForPropositionInFile( string aPath )
    {
      if ( !File.Exists( aPath ) )
      {
        Console.WriteLine( "File not found: {0}", aPath );
        return -1;
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
        return -1;
      }

      return 0;
    }

    private static void PrintUsage()
    {
      Console.WriteLine( "Usage: upl [option] [file]" );
      Console.WriteLine();
      Console.WriteLine( "Decide the contents of a file.  If no file is given, read stdin.  Print the" );
      Console.WriteLine( "result and return a corresponding exit code." );
      Console.WriteLine();
      Console.WriteLine( "Exit Codes:" );
      Console.WriteLine();
      Console.WriteLine( "     0\tNecessary" );
      Console.WriteLine( "     1\tContingent" );
      Console.WriteLine( "     2\tImpossible" );
      Console.WriteLine( "    -1\tError" );
      Console.WriteLine();
      Console.WriteLine( "Options:" );
      Console.WriteLine();
      Console.WriteLine( "  -q, --query\tQuery mode.  Enter a statement, then enter a line containing" );
      Console.WriteLine( "\t\tonly a question mark ('?').  upl will decide the statement," );
      Console.WriteLine( "\t\tprint its decision, then accept more input.  Enter ctrl-Z to" );
      Console.WriteLine( "\t\tquit." );
      Console.WriteLine();
      Console.WriteLine( "  -g, --graph\tGenerate a graph of the statement in GraphViz DOT code instead" );
      Console.WriteLine( "\t\tof deciding the statement.  Return an exit code of 0." );
      Console.WriteLine();
      Console.WriteLine( "  -s, --silent\tDon't print anything to stdout, but do return the result as an" );
      Console.WriteLine( "\t\texit code." );
      Console.WriteLine();
      Console.WriteLine( "  -h, --help\tDisplay this information." );
    }
  }
}
