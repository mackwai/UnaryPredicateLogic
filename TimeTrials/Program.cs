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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Logic;

namespace TimeTrials
{
  class Program
  {
    private const string LocationOfTestFiles = @"..\..\..\VerificationTesting";
    private const string LocationOfSourceFiles = @"..\..\..\Logic";
    private const int DefaultNumberOfTrials = 1000;

    private static readonly string[] TestFileNames = new string[]
    {
      "Socrates.txt",
      "Socrates.txt",
      "SimpleContradiction.txt",
      "RulesOfPropositionalLogic.txt",
      "BigNullPredicates.txt",
      "NullPredicates.txt",
      "MixedNullUnaryPredicates1.txt",
      "DiSpezio135.txt",
      "XorOnUnboundVariables.txt",
      "Hurley213.txt",
      "Hurley454.txt",
      "Hurley457.txt",
      "ModalTest1.txt",
      //"ModalTest2.txt",
      "ModalTest3.txt",
      "SocratesOverload.txt",
      "AxiomsOfModalLogic.txt",
      "EmptyWorldTest.txt",
      "ToBeOrNotToBe.txt",
      "LastWorldTested.txt",
      "AxiomsOfIdentity.txt"
    };

    private static Matrix ParseFile( string aPathToFile )
    {
      return Logic.Parser.Parse( File.ReadAllText( aPathToFile ).Split( '\n' ) );
    }

    private static StreamWriter CreateOutputStream()
    {
      return new StreamWriter( File.OpenWrite( String.Format( "Time_Trial_{0}.txt", DateTime.Now.ToString( "yyyyMMddHHmmss" ) ) ) );
    }

    private static string LogicSourceCode
    {
      get
      {
        StringBuilder lCode = new StringBuilder();
        foreach ( string lFilePath in Directory.EnumerateFiles( LocationOfSourceFiles, "*.cs", SearchOption.AllDirectories ) )
        {
          lCode.AppendLine();
          lCode.AddLine( "// Source File {0}:", lFilePath );
          lCode.AppendLine( File.ReadAllText( lFilePath ) );
        }
        return lCode.ToString();
      }
    }

    private static long ObserveDecisionTime( Matrix aMatrix, int aTrials )
    {
      Stopwatch.Start();
      for ( int i = 0; i < aTrials; i++ )
      {
        aMatrix.Decide();
      }
      return Stopwatch.ElapsedTime.Ticks / aTrials;
    }

    static void Main( string[] args )
    {
      int lTrials;

      if ( args.Length > 0 ) 
      {
        try 
        {
          lTrials = Convert.ToInt32( args[0] );
        }
        catch ( Exception )
        {
          lTrials = DefaultNumberOfTrials;
        }
      }
      else
      {
        lTrials = DefaultNumberOfTrials;
      }

      StringBuilder lContentsOfTestFiles = new StringBuilder();
      StringBuilder lTimingResults = new StringBuilder();
      List<long> lTimes = new List<long>();

      foreach ( string lTestFileName in TestFileNames )
      {
        Console.Write( "{0}... ", lTestFileName );
        try
        {
          string[] lFileText = File.ReadAllLines( Path.Combine( LocationOfTestFiles, lTestFileName ) );
          long lDecisionTime = ObserveDecisionTime( Parser.Parse( lFileText ), lTrials );
          Console.WriteLine( "{0:X16} - {1} seconds", lDecisionTime, TimeSpan.FromTicks( lDecisionTime ).TotalSeconds );
          lContentsOfTestFiles.AppendLine();
          lContentsOfTestFiles.AddLine( "// Test File {0}:", lTestFileName );
          lContentsOfTestFiles.Append( string.Join( Environment.NewLine, lFileText ) );
          lTimingResults.AddLine( "{0:X16}\t{1}", lDecisionTime, lTestFileName );
          lTimes.Add( lDecisionTime );
        }
        catch ( Exception lException )
        {
          lTimingResults.AddLine( @"Failed to test {0}: {1}", lTestFileName, lException );
        }
      }

      StreamWriter lOutput = CreateOutputStream();
      lOutput.WriteLine( "Test run, " + DateTime.Now.ToString() );
      lOutput.Write( lTimingResults );
      lOutput.WriteLine( "Mean\tMedian" );
      lTimes.Sort();
      lOutput.WriteLine( "{0}\t{1}", lTimes.Average(), lTimes.ElementAt( lTimes.Count() / 2 ) );
      lOutput.WriteLine();
      lOutput.WriteLine( LogicSourceCode );
      lOutput.WriteLine( lContentsOfTestFiles );
      lOutput.Close();

#if DEBUG
      Console.WriteLine( "Done." );
      Console.ReadKey();
#endif
    }
  }
}
