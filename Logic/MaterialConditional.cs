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
  /// a material conditional
  /// </summary>
  internal class MaterialConditional : BinaryOperator
  {
    internal MaterialConditional( Matrix aLeft, Matrix aRight )
      : base( aLeft, aRight )
    {
    }

    protected override string Connector
    {
      get { return "->"; }
    }

    protected override string TreeProofGeneratorConnector
    {
      get { return @"\to"; }
    }

    internal override string DOTLabel
    {
      get { return "<Material Conditional<BR/><B><FONT FACE=\"MONOSPACE\">-&gt;</FONT></B>>"; }
    }
    
    internal override bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicates )
    {
      return !Left.TrueIn( aInterpretation, aKindOfWorld, aPredicates ) || Right.TrueIn( aInterpretation, aKindOfWorld, aPredicates );
    }

    internal override Matrix Substitute( Variable aVariable, Variable aReplacement )
    {
      return new MaterialConditional( Left.Substitute( aVariable, aReplacement ), Right.Substitute( aVariable, aReplacement ) );
    }

    internal override string Prover9InputHelper1( Dictionary<char, string> aTranslatedVariableNames )
    {
      return string.Format(
        "formulas(assumptions).\n{0}end_of_list.\n\nformulas(goals).\n{1}.\nend_of_list.\n",
        this.Left.MakeProver9Formulas( aTranslatedVariableNames ),
        this.Right.Prover9InputHelper( aTranslatedVariableNames ) );
    }
  }
}
