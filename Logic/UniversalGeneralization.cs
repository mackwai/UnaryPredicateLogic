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

namespace Logic
{
	/// <summary>
	/// a universal quantification
	/// </summary>
	internal class UniversalGeneralization : UnaryOperator
	{
		private readonly Variable mVariable;
		
		internal UniversalGeneralization( Variable aVariable,  Matrix aInnerProposition )
		  : base( aInnerProposition )
		{
			mVariable = aVariable;
		}
		
		internal Variable Variable
		{
			get { return mVariable; }
		}

    internal override IEnumerable<Variable> FreeVariables
    {
      get { return mInnerMatrix.FreeVariables.Where( fVariable => !fVariable.Equals( mVariable ) ); }
    }

    internal override IEnumerable<Tuple<UniversalGeneralization, Matrix>> ClosedPredications
    {
      get
      {
        return NonNullPredications.Where( fPredication => fPredication.FreeVariables.Any( fVariable => fVariable == this.Variable ) )
          .Select( fPredication => Tuple.Create( this, fPredication ) ).Concat( mInnerMatrix.ClosedPredications );
      }
    }

    internal override int MaxmimumNumberOfDistinguishableObjects
    {
      get
      {
        return Math.Max(
          mInnerMatrix.MaxmimumNumberOfDistinguishableObjects,
          mInnerMatrix.FreeVariables.Intersect( mInnerMatrix.IdentifiedVariables ).Count() );
      }
    }
		
		internal override bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicates )
		{
      foreach ( string lKindOfObject in aPredicates.GetKindsOfObjectsIn( aKindOfWorld ) )
      {
        Variable.Instantiate( lKindOfObject );

        if ( !mInnerMatrix.TrueIn( aInterpretation, aKindOfWorld, aPredicates ) )
          return false;
      }

      return true;

      //return aPredicates
      //  .GetKindsOfObjectsIn( aKindOfWorld )
      //  .All( fPredicateCombination => TrueInInstance( fPredicateCombination, aInterpretation, aKindOfWorld, aPredicates ) );
		}
		
    //private bool TrueInInstance( string aKindOfObject, uint aInterpretation, uint aKindOfWorld, Predicates aPredicates )
    //{
    //  Variable.Instantiate( aKindOfObject );

    //  return mInnerMatrix.TrueIn( aInterpretation, aKindOfWorld, aPredicates );
    //}
		
		public override string ToString()
		{
			return string.Format( "({0},{1})", mVariable, mInnerMatrix );
		}

    internal override string DOTLabel
    {
      get
      {
        return String.Format(
          "<Universal Generalization<BR/><B><FONT FACE=\"MONOSPACE\">{0},</FONT></B>>",
          this.mVariable );
      }
    }
	}
}
