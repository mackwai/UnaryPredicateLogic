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

    internal override IEnumerable<Tuple<UniversalGeneralization, Matrix>> ClosedPredications
    {
      get
      {
        return NonNullPredications.Where( fPredication => fPredication.FreeVariables.Any( fVariable => fVariable == this.Variable ) )
          .Select( fPredication => Tuple.Create( this, fPredication ) ).Concat( mInnerMatrix.ClosedPredications );
      }
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

    internal override IEnumerable<Variable> FreeVariables
    {
      get { return mInnerMatrix.FreeVariables.Where( fVariable => !fVariable.Equals( mVariable ) ); }
    }

    internal override int MaxmimumNumberOfDistinguishableObjectsOfAKind
    {
      get
      {
        return Math.Max(
          mInnerMatrix.MaxmimumNumberOfDistinguishableObjectsOfAKind,
          mInnerMatrix.FreeVariables.Intersect( mInnerMatrix.IdentifiedVariables ).Count() );
      }
    }

    internal override int DepthOfLoopNesting
    {
      get { return mInnerMatrix.DepthOfLoopNesting + 1; }
    }

    internal Variable Variable
		{
			get { return mVariable; }
		}

    internal override void AssignModality( Necessity aNecessity )
    {
      mVariable.Modality = aNecessity;
      mInnerMatrix.AssignModality( aNecessity );
    }

		internal override bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicates )
		{
      if ( aPredicates.NoDistinguishableObjects )
        return mInnerMatrix.TrueIn( aInterpretation, aKindOfWorld, aPredicates );

      foreach ( string lKindOfObject in aPredicates.GetKindsOfObjectsIn( aKindOfWorld ) )
      {
        Variable.Instantiate( lKindOfObject, aKindOfWorld );

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

    internal override Matrix Substitute( Variable aVariable, Variable aReplacement )
    {
      return new UniversalGeneralization(
        mVariable.Substitute( aVariable, aReplacement ),
        mInnerMatrix.Substitute( aVariable, aReplacement ) );
    }

    internal override string Prover9InputHelper( Dictionary<char, string> aTranslatedVariableNames )
    {
      char lLetter = Variable.ToString()[0];
      aTranslatedVariableNames[ lLetter ] = lLetter < 'u' || lLetter > 'z'
        ? "v" + Variable.ToString()
        : Variable.ToString();

      return string.Format(
        "( all {0} {1} )",
        aTranslatedVariableNames[ lLetter ],
        mInnerMatrix.Prover9InputHelper( aTranslatedVariableNames ) );
    }

    public override string TreeProofGeneratorInput
    {
      get { return String.Format( @"(\forall {0} {1})", mVariable, mInnerMatrix.TreeProofGeneratorInput ); }
    }

    public override string ToString()
    {
      return string.Format( "({0},{1})", mVariable, mInnerMatrix );
    }

    public override bool Equals( object obj )
    {
      UniversalGeneralization that = obj as UniversalGeneralization;

      if ( that == null )
        return false;

      return this.mVariable.Equals( that.mVariable ) && base.Equals( obj );
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
	}
}
