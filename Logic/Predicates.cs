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

namespace Logic
{
  /// <summary>
  /// data collected regarding predicates in a matrix, used to determine what possible worlds the matrix can distinguish from
  /// other possible worlds and whether a given predication is true or false in a given possible world
  /// </summary>
  internal class Predicates
  {
    const int BitsAvailable = sizeof( uint ) * 8;

    private readonly UnaryPredicate[] mUnaryPredicates;
    private readonly NullPredicate[] mNullPredicates;
    private readonly int mBitsNeededToDistinguishObjects;
    private readonly int mBitsNeededToDistinguishWorlds;
    private readonly ulong mBitsNeeded;
    private readonly bool mModalitiesPresent;
    
    public Predicates(
      IEnumerable<NullPredicate> aNullPredicates,
      IEnumerable<UnaryPredicate> aUnaryPredicates,
      int aMaximumNumberOfDistinguishableObjects,
      bool aModalitiesPresent,
      int aMaximumNumberOfModalitiesInvolvedInIdentifications )
    {
      mUnaryPredicates = aUnaryPredicates.ToArray();
      mNullPredicates = aNullPredicates.ToArray();
      mBitsNeededToDistinguishObjects = BitsNeededToEnumerate( aMaximumNumberOfDistinguishableObjects + 1 );
      mBitsNeededToDistinguishWorlds = mBitsNeededToDistinguishObjects * NumberOfCombinationsOfUnaryPredicates + NumberOfNullPredicates;
      if ( aMaximumNumberOfModalitiesInvolvedInIdentifications > 1 )
        mBitsNeededToDistinguishWorlds *= BitsNeededToEnumerate( aMaximumNumberOfModalitiesInvolvedInIdentifications );
      mBitsNeeded = aModalitiesPresent
        ? 1UL << mBitsNeededToDistinguishWorlds
        : (ulong) mBitsNeededToDistinguishWorlds;
      mModalitiesPresent = aModalitiesPresent;

      if ( mBitsNeeded > BitsAvailable || mBitsNeededToDistinguishWorlds > BitsAvailable )
        throw new EngineException( "Too many predicates!" );
    }

    /// <summary>
    /// The first bits in the encoding represent null predicates; if all bits above
    /// these bits are zero, then the encoding represents a kind of world with
    /// nothing in it.
    /// </summary>
    public uint FirstNonemptyWorld
    {
      get { return mBitsNeededToDistinguishObjects == 0 ? 0 : 1U << NumberOfNullPredicates; }
    }

    //public IEnumerable<uint> Interpretations
    //{
    //  get
    //  {
    //    if ( mModalitiesPresent )
    //    {
    //      for ( uint i = LastInterpretation; i > 0; i-- )
    //      {
    //        yield return i;
    //      }
    //    }
    //    else
    //    {
    //      yield return LastInterpretation;
    //    }
    //  }
    //}

    public IEnumerable<string> GetKindsOfObjectsIn( uint aKindOfWorld )
    {
      if ( NumberOfUnaryPredicates == 0 && mBitsNeededToDistinguishObjects == 0 )
      {
        // This is needed in order to cause the body of the loop in UnviversalGeneralization.TrueIn to execute
        // once when the proposition being decided contains no unary predicates.
        yield return "";
      }
      else
      {
        int lNumberOfCombinationsOfUnaryPredicates = NumberOfCombinationsOfUnaryPredicates;

        for ( int lPredicateCombination = 0; lPredicateCombination < lNumberOfCombinationsOfUnaryPredicates; lPredicateCombination++ )
        {
          uint lDistinguishableInstancesOfThisPredicateCombination =
            DistinguishableInstancesOfThisPredicateCombination( aKindOfWorld, lPredicateCombination );
          for ( uint i = 0; i < lDistinguishableInstancesOfThisPredicateCombination; i++ )
            yield return BuildUnaryPredicateCombination( lPredicateCombination, i );
        }
      }
    }
    
    public IEnumerable<uint> KindsOfWorlds( uint aInterpretation )
    {
      if ( mModalitiesPresent )
      {
        uint lLastKindOfWorld = LastKindOfWorld;

        for ( uint i = FirstNonemptyWorld; i <= lLastKindOfWorld; i++ )
        {
          //if ( aInterpretation == 1 && i == 32 )
          //  System.Diagnostics.Debugger.Break();
          if ( ( ( 1U << (int) i ) & aInterpretation ) != 0 )
            yield return i;
        }
      }
      else
      {
        uint lLastKindOfWorld = LastKindOfWorld;

        for ( uint i = FirstNonemptyWorld; i <= lLastKindOfWorld; i++ )
        {
          yield return i;
        }
      }
    }

    public ulong BitsNeeded
    {
      get { return mBitsNeeded; }
    }

    public uint LastInterpretation
    {
      get { return mBitsNeeded == BitsAvailable ? 0xFFFFFFFF : ( 1U << (int) mBitsNeeded ) - 1U; }
    }

    public uint LastKindOfWorld
    {
      get { return mBitsNeeded == BitsAvailable ? 0xFFFFFFFF : ( 1U << (int) mBitsNeededToDistinguishWorlds ) - 1U; }
    }

    public uint FirstInterpretation
    {
      get { return mModalitiesPresent ? 1U : LastInterpretation; }
    }

    public bool TrueIn( NullPredicate aPredicate, uint aKindOfWorld )
    {
      for ( int i = 0; i < mNullPredicates.Length; i++ )
      {
        if ( aPredicate.Equals( mNullPredicates[ i ] ) )
          return VerifiesPredicate( aKindOfWorld, i );
      }

      throw new EngineException( "Null predicate {0}. not found in proposition.", aPredicate );
    }

    /// <summary>
    /// Calculate the number of bits needed to enumerate aNumber different things.
    /// </summary>
    /// <param name="aNumber">the number of different that are to be enumerated</param>
    /// <returns>the number of bits needed to enumerate aNumber different things</returns>
    private static int BitsNeededToEnumerate( int aNumber )
    {
      int i;
      aNumber--;

      for ( i = 0; aNumber > 0; aNumber >>= 1, i++ ) ;

      return i;
    }

    private string BuildUnaryPredicateCombination( int aKindOfObject, uint aInstanceNumber )
    {
      char[] lPredicateCombination = new char[ 6 ];
      int lIndex = 0;

      foreach ( UnaryPredicate lPredicate in mUnaryPredicates )
      {
        if ( ( aKindOfObject & 1 ) != 0 )
          lPredicateCombination[ lIndex++ ] = lPredicate.Letter;

        aKindOfObject >>= 1;
      }

      // Add a character that correspondS to the instance number to the string.  The character
      // must not be a capital letter.
      lPredicateCombination[ lIndex ] = (char) ( aInstanceNumber + 32 );

      return new String( lPredicateCombination, 0, lIndex + 1 );
    }
    
    private uint DistinguishableInstancesOfThisPredicateCombination( uint aKindOfWorld, int aKindOfObject )
		{
#if SALTARELLE
      uint lBitMask = (uint) ( ( 1 << mBitsNeededToDistinguishObjects ) - 1 );
#else
      uint lBitMask = Convert.ToUInt32( ( 1 << mBitsNeededToDistinguishObjects ) - 1 );
#endif
      
      return ( aKindOfWorld >> ( mBitsNeededToDistinguishObjects * aKindOfObject + NumberOfNullPredicates ) ) & lBitMask;
		}

    private int NumberOfCombinationsOfUnaryPredicates
    {
      get { return 1 << NumberOfUnaryPredicates; }
    }

    private int NumberOfNullPredicates
    {
      get { return mNullPredicates.Length; }
    }

    private int NumberOfUnaryPredicates
    {
      get { return mUnaryPredicates.Length; }
    }

    private bool VerifiesPredicate( uint aKindOfWorld, int aPredicate )
    {
      return ( ( aKindOfWorld >> aPredicate ) & 1U ) == 1U;
    }
  }
}
