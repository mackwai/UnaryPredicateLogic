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

using System.Collections.Generic;

namespace Logic
{
  /// <summary>
  /// a negation
  /// </summary>
  internal class Negation : UnaryOperator
  {
    internal Negation( Matrix aProposition )
      : base( aProposition )
    {
    }
  
    internal override string DOTLabel
    {
      get { return "<Negation<BR/><B><FONT FACE=\"MONOSPACE\">~</FONT></B>>"; }
    }

    public override bool IsPropositional
    {
      get { return mInnerMatrix.IsPropositional; }
    }

    internal override void AssignModality( Necessity aNecessity )
    {
      mInnerMatrix.AssignModality( aNecessity );
    }

    internal override bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicates )
    {
      return !mInnerMatrix.TrueIn( aInterpretation, aKindOfWorld, aPredicates );
    }

    internal override Matrix Substitute( Variable aVariable, Variable aReplacement )
    {
      return new Negation( mInnerMatrix.Substitute( aVariable, aReplacement ) );
    }

    internal override string Prover9InputHelper( Dictionary<char, string> aTranslatedVariableNames )
    {
      return string.Format( "-{0}", mInnerMatrix.Prover9InputHelper( aTranslatedVariableNames ) );
    }

    public override string ToString()
    {
      return string.Format( "~{0}", mInnerMatrix );
    }

    public override string TreeProofGeneratorInput
    {
      get
      {
        return string.Format( @"(%C2%AC{0})", mInnerMatrix.TreeProofGeneratorInput );
      }
    }

    internal override CoefficientVector CoefficientVectorHelper( NullPredicate[] aNullPredicates )
    {
      return Logic.CoefficientVector.Apply(
        Logic.CoefficientVector.NOT,
        mInnerMatrix.CoefficientVectorHelper( aNullPredicates ) );
    }
  }
}
