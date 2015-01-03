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
using System.Linq;
using System.Collections.Generic; 
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logic
{
  [TestClass]
  public class BitsNeeded
  {
    [TestMethod]
    public void Test_Bits_Needed_Non_Modal()
    {
      NullPredicate[] lNullPredicates = new NullPredicate[ 34 ];
      UnaryPredicate[] lUnaryPredicates = new UnaryPredicate[ 7 ];
      bool[,,] lMap = new bool[ lNullPredicates.Length + 2, lUnaryPredicates.Length + 2, 19 ];

      for ( int n = lNullPredicates.Length; n >= 0; n-- )
      {
        for ( int u = lUnaryPredicates.Length; u >= 0; u-- )
        {
          for ( int i = 17; i >= 1 || ( i == 0 && u == 0 ); i-- )
          {
            try
            {
              new Predicates( lNullPredicates.Take( n ), lUnaryPredicates.Take( u ), i, false, 0 );
              lMap[ n, u, i ] = true;
            }
            catch ( EngineException )
            {

            }
          }
        }
      }

      for ( int n = lNullPredicates.Length; n >= 0; n-- )
      {
        for ( int u = lUnaryPredicates.Length; u >= 0; u-- )
        {
          for ( int i = 17; i >= 1 || ( i == 0 && u == 0 ); i-- )
          {
            if ( lMap[n,u,i] && !lMap[n+1,u,i] && !lMap[n,u+1,i] && !lMap[n,u,i+1] )
            {
              //Console.WriteLine( "n = {0}, u = {1}, i = {2}", n, u, i );
              Console.WriteLine( "  <tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", n, u, i );
            }
          }
        }
      }

    }

    [TestMethod]
    public void Test_Bits_Needed_Modal()
    {
      NullPredicate[] lNullPredicates = new NullPredicate[ 34 ];
      UnaryPredicate[] lUnaryPredicates = new UnaryPredicate[ 7 ];
      bool[ , , , ] lMap = new bool[ lNullPredicates.Length + 2, lUnaryPredicates.Length + 2, 19, 19 ];

      for ( int n = lNullPredicates.Length; n >= 0; n-- )
      {
        for ( int u = lUnaryPredicates.Length; u >= 0; u-- )
        {
          for ( int i = 17; i >= 1 || (i == 0 && u == 0); i-- )
          {
            for ( int t = i; t >= 0; t-- )
            {
              try
              {
                new Predicates( lNullPredicates.Take( n ), lUnaryPredicates.Take( u ), i, true, t );
                lMap[ n, u, i, t ] = true;
              }
              catch ( EngineException )
              {

              }
            }
          }
        }
      }

      for ( int n = lNullPredicates.Length; n >= 0; n-- )
      {
        for ( int u = lUnaryPredicates.Length; u >= 0; u-- )
        {
          for ( int i = 17; i >= 1 || ( i == 0 && u == 0 ); i-- )
          {
            for ( int t = i; t >= 0; t-- )
            {
              if ( lMap[ n, u, i, t ] && !lMap[ n + 1, u, i, t ] && !lMap[ n, u + 1, i, t ] && !lMap[ n, u, i + 1, t ] && !lMap[ n, u, i, t + 1 ] )
              {
                //Console.WriteLine( "n = {0}, u = {1}, i = {2}, t = {3}", n, u, i, t );
                Console.WriteLine( "  <tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", n, u, i, t );
              }
            }
          }
        }
      }

      new Predicates( lNullPredicates.Take( 32 ), lUnaryPredicates.Take( 0 ), 0, false, 0 );
    }
  }
}
