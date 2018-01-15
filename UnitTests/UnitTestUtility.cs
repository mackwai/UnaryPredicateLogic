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


using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Logic;

namespace UnitTests
{
  internal static class UnitTestUtility
  {
    private const int Prover9Mace4Timeout = 30;

    public static Alethicity ParseExpectedResult( string aText )
    {
      if ( aText.Contains( "Expected Result: Necessary" ) )
        return Alethicity.Necessary;
      else if ( aText.Contains( "Expected Result: Contingent" ) )
        return Alethicity.Contingent;
      else if ( aText.Contains( "Expected Result: Impossible" ) )
        return Alethicity.Impossible;
      else
        throw new Exception( "No expected result specified in file." );
    }

    private static bool DecisionsAreConsistent( Alethicity aAlethicity, Prover9Mace4.Result aResult )
    {
      if ( aResult == Prover9Mace4.Result.NoDecision )
        return true;

      if ( aAlethicity == Alethicity.Necessary
        && aResult != Prover9Mace4.Result.Necessary
        && aResult != Prover9Mace4.Result.Possible )
        return false;

      if ( aAlethicity == Alethicity.Impossible
        && aResult != Prover9Mace4.Result.Impossible
        && aResult != Prover9Mace4.Result.Unnecessary )
        return false;

      if ( aAlethicity == Alethicity.Contingent
        && ( aResult == Prover9Mace4.Result.Necessary || aResult == Prover9Mace4.Result.Impossible ) )
        return false;

      return true;
    }

    public static Alethicity GetDecision( string aText )
    {
      Matrix lProposition = Parser.Parse( aText.Split( '\n' ) );
      Alethicity lDecision = lProposition.Decide();
      Prover9Mace4.Result lProver9sDecision = Prover9Mace4.Decide( lProposition, Prover9Mace4Timeout );
      Console.WriteLine( "Prover9/Mace4 decided {0}.", lProver9sDecision );
      Assert.IsTrue(
        DecisionsAreConsistent( lDecision, lProver9sDecision ),
        string.Format( "Inconsistent decisions: {0}, {1}", lDecision, lProver9sDecision ) );
      return Parser.Parse( aText.Split( '\n' ) ).Decide();
    }

    public static Prover9Mace4.Result GetProver9sDecision( string aText, int aTimeout )
    {
      Matrix lProposition = Parser.Parse( aText.Split( '\n' ) );
      return Prover9Mace4.Decide( lProposition, aTimeout );
    }

    public static string GetGraph( string aText )
    {
      return Parser.Parse( aText.Split( '\n' ) ).GraphvizDOT;
    }

    public static bool TestDecision( string aText )
    {
      return ParseExpectedResult( aText ) == GetDecision( aText );
    }
  }
}
