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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;


namespace Logic
{
#if SALTARELLE
  delegate bool Predicate<T>(T aArgument);
#endif
  /// <summary>
  /// Utility methods
  /// </summary>
  public static class Utility
  {
    public static bool IsOneOf<T>( this T aItem, params T[] aItems )
    {
      return aItems.Any( fItem => fItem.Equals( aItem ) );
    }
    
    public static T[] MakeArray<T>( params T[] aItems )
    {
      return aItems;
    }
    
    public static bool Intersects<T>( this IEnumerable<T> aSet1, IEnumerable<T> aSet2 )
    {
      return aSet1.Intersect( aSet2 ).Count() > 0;
    }

    public static T[] Subarray<T>( this T[] aArray, int aStartingIndex, int aLength )
    {
      T[] lNewArray = new T[ aLength ];
#if SALTARELLE
      for ( int i = 0; i < aLength; i++ )
      {
        lNewArray[ i ] = aArray[ i + aStartingIndex ];
      }
#else 
      Array.Copy( aArray, aStartingIndex, lNewArray, 0, aLength );
#endif
      return lNewArray;
    }
    
    public static T[] Tail<T>( this T[] aArray )
    {
      return aArray.Subarray( 1, aArray.Length - 1 );
    }
    
    public static T Head<T>( this IEnumerable<T> aEnumerable )
    {
      return aEnumerable.First();
    }

    public static void AddLine( this StringBuilder aBuilder, string aFormat, params object[] aParams )
    {
      aBuilder.AppendLine( string.Format( aFormat, aParams ) );
    }

    public static string TextForDecision( Alethicity lDecision )
    {
      switch ( lDecision )
      {
        case Alethicity.Necessary:
          return "necessarily true";
        case Alethicity.Contingent:
          return "contingent";
        case Alethicity.Impossible:
          return "self-contradictory";
        default:
          throw new NotImplementedException(
            String.Format( "The value Alethicity.{0} is not supported by this application.", lDecision ) );
      }
    }


    public static string RegexReplace( string aString, string aPattern, string aReplacement )
    {
#if SALTARELLE
      string lChangedString = aString;
      Regex lPattern = new Regex( aPattern );
      do
      {
        aString = lChangedString;
        lChangedString = aString.Replace( lPattern, aReplacement );
      } while ( lChangedString != aString );
      return aString;
#else
      return Regex.Replace( aString, aPattern, aReplacement );
#endif

    }

#if SALTARELLE
    [InlineCode( "new Int8Array({aNumberOfElements})" )]
#endif
    public static sbyte[] CreateSByteArray( int aNumberOfElements )
    {
      return new sbyte[ aNumberOfElements ];
    }
  }
}
