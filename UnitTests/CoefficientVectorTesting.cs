// somerby.net/mack/logic
// Copyright (C) 2019 MacKenzie Cumings
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
using Logic;

namespace UnitTests
{
  [TestClass]
  public class CoefficientVectorTesting
  {
    private static CoefficientVector GetVector( string aStatement )
    {
      return Parser.Parse( aStatement.Split( '\n' ) ).CoefficientVector;
    }

    private static string GetFormula( string aStatement )
    {
      return Parser.Parse( aStatement.Split( '\n' ) ).CoefficientVector.Formula;
    }

    private static T[] Array<T>( params T[] aItems )
    {
      return aItems;
    }

    [TestMethod]
    public void Test_Vectors()
    {
      Assert.AreEqual( GetVector( "p&q" ).ToString(), new CoefficientVector( 0, 0, 0, 1 ).ToString() );
    }

    [TestMethod]
    public void Test_Formulas()
    {
      System.Console.WriteLine("formula...");
      Assert.AreEqual( GetFormula( "p&q" ), "pq" );
      System.Console.WriteLine("formula...");
      Assert.AreEqual( GetFormula( "(p<=>q)|(r<=>s)" ), "1 - pr - qr + 2pqr - ps - qs + 2pqs + 2prs + 2qrs - 4pqrs" );
      System.Console.WriteLine("formula...");
      Assert.AreEqual( GetFormula( "(p<=>q)|(r<=>s)|(r&p)" ), "1 - qr + pqr - ps - qs + 2pqs + prs + 2qrs - 3pqrs" );
      System.Console.WriteLine( GetFormula( "(p&~p)|~q" ) );
    }
  }
}
