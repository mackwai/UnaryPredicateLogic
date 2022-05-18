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

namespace Logic
{
  /// <summary>
  /// a logical necessity; true if the matrix is true in all possible worlds, false otherwise
  /// </summary>
  internal class Necessity : UnaryOperator
  {
    private sbyte[] mTruthValues = null;
    private readonly bool Memoizible;

    internal Necessity( Matrix aMatrix )
      : base( aMatrix )
    {
      Memoizible = !mInnerMatrix.ContainsModalities && !this.FreeVariables.Any();

      if ( Memoizible )
        mTruthValues = Utility.CreateSByteArray( 1 << 16 );

      aMatrix.AssignModality( this );
    }

    internal override bool ContainsModalities
    {
      get { return true; }
    }

    internal override string DOTLabel
    {
      get { return "<Necessity<BR/><B><FONT FACE=\"MONOSPACE\">[]</FONT></B>>"; }
    }

    internal override IEnumerable<Necessity> FreeModalities
    {
      get { return mInnerMatrix.FreeModalities.Where( fModality => fModality != this ); }
    }

    internal override int MaxmimumNumberOfModalitiesInIdentifications
    {
      get
      {
        return Math.Max(
          mInnerMatrix.MaxmimumNumberOfModalitiesInIdentifications,
          mInnerMatrix.FreeModalities.Intersect( mInnerMatrix.ModalitiesInIdentifications ).Count() );
      }
    }

    internal override int DepthOfLoopNesting
    {
      get { return mInnerMatrix.DepthOfLoopNesting + 1; }
    }

    internal override bool TrueIn( uint aInterpetation, uint aKindOfWorld, Predicates aPredicates )
    {
      if ( Memoizible )
      {
        foreach ( uint lKindOfWorld in aPredicates.KindsOfWorlds( aInterpetation ) )
        {
          if ( mTruthValues[ lKindOfWorld ] == 0 )
            mTruthValues[ lKindOfWorld ] = mInnerMatrix.TrueIn( aInterpetation, lKindOfWorld, aPredicates ) ? (sbyte) 1 : (sbyte) -1;

          if ( mTruthValues[ lKindOfWorld ] == (sbyte) -1 )
            return false;
        }

        return true;
      }
      else
      {
        return aPredicates.KindsOfWorlds( aInterpetation )
          .All( fWorld => mInnerMatrix.TrueIn( aInterpetation, fWorld, aPredicates ) );
      }
    }

    internal override string Prover9InputHelper( Dictionary<char, string> aTranslatedVariableNames )
    {
      throw new EngineException( "Prover9 does not support modal logic." );
    }
    
    public override string ToString()
    {
      return string.Format( "[]{0}", mInnerMatrix );
    }

    internal override Matrix Substitute( Variable aVariable, Variable aReplacement )
    {
      return new Necessity( mInnerMatrix.Substitute( aVariable, aReplacement ) );
    }

    public override string TreeProofGeneratorInput
    {
      get
      {
        return string.Format( "%E2%96%A1{0}", mInnerMatrix.TreeProofGeneratorInput );
      }
    }
  }
}
