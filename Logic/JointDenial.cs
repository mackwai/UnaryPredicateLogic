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

namespace Logic
{
  /// <summary>
  /// a joint denial
  /// </summary>
  internal class JointDenial : BinaryOperator
  {
    internal JointDenial( Matrix aLeft, Matrix aRight )
      : base( aLeft, aRight )
    {
    }

    protected override string Connector
    {
      get { return "!"; }
    }

    internal override string DOTLabel
    {
      get { return "<Joint Denial<BR/><B><FONT FACE=\"MONOSPACE\">!</FONT></B>>"; }
    }
  
    internal override bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicates )
    {
      return !Left.TrueIn( aInterpretation, aKindOfWorld, aPredicates ) && !Right.TrueIn( aInterpretation, aKindOfWorld, aPredicates );
    }

    internal override string Prover9InputHelper( Dictionary<char, string> aTranslatedVariableNames )
    {
      return string.Format(
        @"(-{0} & -{1})",
        Left.Prover9InputHelper( aTranslatedVariableNames ),
        Right.Prover9InputHelper( aTranslatedVariableNames ) );
    }

    public override string TreeProofGeneratorInput
    {
      get { return String.Format( @"(%C2%AC({0}%E2%88%A8{1})))", Left.TreeProofGeneratorInput, Right.TreeProofGeneratorInput ); }
    }

    protected override string TreeProofGeneratorConnector
    {
      get { throw new NotImplementedException( "JointDenial.TreeProofGeneratorConnector should be unreachable." ); }
    }

    internal override Matrix Substitute( Variable aVariable, Variable aReplacement )
    {
      return new JointDenial( Left.Substitute( aVariable, aReplacement ), Right.Substitute( aVariable, aReplacement ) );
    }

    internal override CoefficientVector CoefficientVectorForOperator
    {
      get { return Logic.CoefficientVector.NOR; }
    }
  }
}
