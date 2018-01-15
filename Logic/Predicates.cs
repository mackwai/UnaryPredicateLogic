// somerby.net/mack/logic
// Copyright (C) 2015, 2018 MacKenzie Cumings
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
    private readonly int mBitsNeededToRepresentObjectsOfAKind;
    private readonly int mBitsNeededToDistinguishWorlds;
    private readonly ulong mBitsNeeded;
    private readonly bool mModalitiesPresent;
    private readonly int mDistinguishableWorldsWithinAKind;
    
    public Predicates(
      IEnumerable<NullPredicate> aNullPredicates,
      IEnumerable<UnaryPredicate> aUnaryPredicates,
      int aMaximumNumberOfDistinguishableObjectsOfAKind,
      bool aModalitiesPresent,
      int aMaximumNumberOfModalitiesInvolvedInIdentifications )
    {
      mUnaryPredicates = aUnaryPredicates.ToArray();
      mNullPredicates = aNullPredicates.ToArray();
      mBitsNeededToRepresentObjectsOfAKind = BitsNeededToEnumerate( aMaximumNumberOfDistinguishableObjectsOfAKind );
      mBitsNeededToDistinguishWorlds =
        mBitsNeededToRepresentObjectsOfAKind * NumberOfCombinationsOfUnaryPredicates + NumberOfNullPredicates;
      mDistinguishableWorldsWithinAKind = Math.Max( aMaximumNumberOfModalitiesInvolvedInIdentifications, 1 );
      mBitsNeeded = aModalitiesPresent
        ? 1UL << mBitsNeededToDistinguishWorlds
        : (ulong) mBitsNeededToDistinguishWorlds;
      mModalitiesPresent = aModalitiesPresent;

      NoDistinguishableObjects = aMaximumNumberOfDistinguishableObjectsOfAKind == 0;

      if ( mBitsNeeded > BitsAvailable || mBitsNeededToDistinguishWorlds > BitsAvailable )
        throw new EngineException( "Too many predicates!" );
    }

    public bool NoDistinguishableObjects
    {
      get; private set;
    }

    /// <summary>
    /// The first bits in the encoding represent null predicates.
    /// </summary>
    public uint FirstNonemptyWorld
    {
      get { return NumberOfUnaryPredicates > 0 ? 1U << NumberOfNullPredicates : 0; }
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

    internal IEnumerable<KindOfWorld> DecodeInterpretationNumber( uint aInterpretation )
    {
      return KindsOfWorlds( aInterpretation ).Select( fNumber => DecodeKindOfWorldNumber( fNumber ) );
    }

    internal KindOfWorld DecodeKindOfWorldNumber( uint aKindOfWorld )
    {
      List<KindOfObject> lKindsOfObjects = new List<KindOfObject>();

      int lNumberOfCombinationsOfUnaryPredicates = NumberOfCombinationsOfUnaryPredicates;

      for ( int lPredicateCombination = 0; lPredicateCombination < lNumberOfCombinationsOfUnaryPredicates; lPredicateCombination++ )
      {
        uint lInstances = DistinguishableInstancesOfThisPredicateCombination( aKindOfWorld, lPredicateCombination );
        if ( lInstances > 0 )
        {
          List<UnaryPredicate> lAffirmedPredicates = new List<UnaryPredicate>();
          List<UnaryPredicate> lDeniedPredicates = new List<UnaryPredicate>();

          int lKindOfObject = lPredicateCombination;
          foreach ( UnaryPredicate lPredicate in mUnaryPredicates )
          {
            if ( ( lKindOfObject & 1 ) == 0 )
              lDeniedPredicates.Add( lPredicate );
            else
              lAffirmedPredicates.Add( lPredicate );

            lKindOfObject >>= 1;
          }
          lKindsOfObjects.Add( new KindOfObject( lInstances, lAffirmedPredicates, lDeniedPredicates ) );
        }
      }

      return new KindOfWorld(
        lKindsOfObjects,
        mNullPredicates.Where( fPredicate => TrueIn( fPredicate, aKindOfWorld ) ),
        mNullPredicates.Where( fPredicate => !TrueIn( fPredicate, aKindOfWorld ) ) );
    }

    public IEnumerable<string> GetKindsOfObjectsIn( uint aKindOfWorld )
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
    
    public IEnumerable<uint> KindsOfWorlds( uint aInterpretation )
    {
      if ( mModalitiesPresent )
      {
        uint lLastKindOfWorld = LastKindOfWorld;

        for ( uint i = FirstNonemptyWorld; i <= lLastKindOfWorld; i++ )
        {
          if ( ( ( 1U << (int) i ) & aInterpretation ) == 0 )
            continue;

          for ( uint j = 0; j < mDistinguishableWorldsWithinAKind; j++ )
          {
            yield return i + ( j << mBitsNeededToDistinguishWorlds );
          }
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
    /// <param name="aNumber">the number of different things that are to be enumerated</param>
    /// <returns>the number of bits needed to enumerate aNumber different things</returns>
    private static int BitsNeededToEnumerate( int aNumber )
    {
      if ( aNumber == 1 )
        return 1;

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

      // Add a character to the string that corresponds to the instance number.  The character
      // must not be a capital letter.
      lPredicateCombination[ lIndex ] = (char) ( aInstanceNumber + 49 );

      return new String( lPredicateCombination, 0, lIndex + 1 );
    }
    
    private uint DistinguishableInstancesOfThisPredicateCombination( uint aKindOfWorld, int aKindOfObject )
		{
#if SALTARELLE
      uint lBitMask = (uint) ( ( 1 << mBitsNeededToRepresentObjectsOfAKind ) - 1 );
#else
      uint lBitMask = Convert.ToUInt32( ( 1 << mBitsNeededToRepresentObjectsOfAKind ) - 1 );
#endif
      
      if ( NumberOfUnaryPredicates == 0 )
        return ( ( aKindOfWorld >> NumberOfNullPredicates ) & lBitMask ) + 1;
      else
        return ( aKindOfWorld >> ( mBitsNeededToRepresentObjectsOfAKind * aKindOfObject + NumberOfNullPredicates ) ) & lBitMask;
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
