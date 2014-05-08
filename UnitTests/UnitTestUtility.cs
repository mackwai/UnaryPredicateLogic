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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Logic;

namespace UnitTests
{
#if SALTARELLE
  public static class UnitTestUtility
#else
  internal static class UnitTestUtility
#endif
  {
    public static Alethicity ParseExpectedResult( string aText )
    {
      if ( aText.Contains( "Expected Result: Necessary" ) )
        return Alethicity.Necessary;
      else if ( aText.Contains( "Expected Result: MerelyPossible" ) )
        return Alethicity.Contingent;
      else if ( aText.Contains( "Expected Result: Impossible" ) )
        return Alethicity.Impossible;
      else
        throw new Exception( "No expected result specified in file." );
    }

    public static Alethicity GetDecision( string aText )
    {
      return Parser.Parse( aText.Split( '\n' ) ).Decide();
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
