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
using System.Linq;
#if SALTARELLE
using System.Runtime.CompilerServices;
#endif
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
    public static IEnumerable<T> AtIndices<T>( this IEnumerable<T> aSequence, IEnumerable<int> aIndices )
    {
      return aSequence.Where( ( fItem, fIndex ) => aIndices.Contains( fIndex ) );
    }

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

    public static int Product( this IEnumerable<int> aNumbers )
    {
      return aNumbers.Aggregate( 1, (f1,f2) => f1*f2, fResult => fResult );
    }

    public static int Sum( this IEnumerable<int> aNumbers )
    {
      return aNumbers.Aggregate( 0, (f1,f2) => f1+f2, fResult => fResult );
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

#if SALTARELLE
    public static void AppendFormat( this StringBuilder aBuilder, string aFormat, params object[] aParams )
    {
      aBuilder.Append( string.Format( aFormat, aParams ) );
    }
#endif

    public static string StatementOfResult( Matrix aMatrix )
    {
      if ( aMatrix is Argument )
        return TextResult( ( aMatrix as Argument ).Quality );
      else
        return TextForDecision( aMatrix.Decide() );
    }

    public static string TextResult( Quality aQuality )
    {
      switch ( aQuality )
      {
        case Quality.InconsistentPremises:
          return "The argument is valid but its premises are inconsistent.";
        case Quality.InconsistentPremisesAndTautologicalConclusion:
          return "The argument valid but its premises are inconsistent and its conclusion is tautological.";
        case Quality.Invalid:
          return "The argument is invalid.";
        case Quality.TautologicalConclusion:
          return "The argument is valid because its conclusion is tautological.";
        case Quality.Valid:
          return "The argument is valid.";
        default:
          throw new NotImplementedException(
            String.Format( "The value Quality.{0} is not supported by this application.", aQuality ) );
      }
    }

    public static string TextForDecision( Alethicity aDecision )
    {
      switch ( aDecision )
      {
        case Alethicity.Necessary:
          return "The statement is necessarily true.";
        case Alethicity.Contingent:
          return "The statement is contingent.";
        case Alethicity.Impossible:
          return "The statement is self-contradictory.";
        default:
          throw new NotImplementedException(
            String.Format( "The value Alethicity.{0} is not supported by this application.", aDecision ) );
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

#if SALTARELLE
    [InlineCode( "status({aStatusMessage})" )]
    public static void Status( string aStatusMessage )
    {
    }

    [InlineCode( "console.log({aLogMessage})" )]
    public static void Log( string aLogMessage )
    {
    }
#endif

    public static IEnumerable<Tuple<TType,TType>> Pairs<TType>( IEnumerable<TType> aList )
    {
      TType[] lArray = aList.ToArray();
      for ( int i = 0; i < lArray.Length - 1; i++ )
      {
        for ( int j = i + 1; j < lArray.Length; j++ )
        {
          yield return new Tuple<TType,TType>( lArray[i], lArray[j] );
        }
      }
    }

    public static IEnumerable<T> Concat<T>( this IEnumerable<T> aEnumerable, T aItem )
    {
      return aEnumerable.Concat( new T[] { aItem } );
    }

    public static IEnumerable<T> AllBut<T>( this IEnumerable<T> aList, int aIndex )
    {
      int lIndex = 0;
      foreach ( T lItem in aList )
      {
        if ( lIndex != aIndex )
          yield return lItem;
        lIndex++;
      }
    }
  }
}
