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
  /// a logical conjunction
  /// </summary>
  internal class Conjunction : BinaryOperator
  {
    internal Conjunction( Matrix aLeft, Matrix aRight ) : base( aLeft, aRight )
    {
    }

    protected override string Connector
    {
      get { return "&"; }
    }

    protected override string TreeProofGeneratorConnector
    {
      get { return @"%E2%88%A7"; }
    }

    internal override string DOTLabel
    {
      get { return "<Conjunction<BR/><B><FONT FACE=\"MONOSPACE\">&amp;</FONT></B>>"; }
    }

    internal override bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicates )
    {
      return Left.TrueIn( aInterpretation, aKindOfWorld, aPredicates ) && Right.TrueIn( aInterpretation, aKindOfWorld, aPredicates );
    }

    internal override IEnumerable<Matrix> Conjuncts
    {
      get { return Left.Conjuncts.Concat( Right.Conjuncts ); }
    }

    internal override Matrix Substitute( Variable aVariable, Variable aReplacement )
    {
      return new Conjunction( Left.Substitute( aVariable, aReplacement ), Right.Substitute( aVariable, aReplacement ) );
    }

    internal override CoefficientVector CoefficientVectorForOperator
    {
      get { return Logic.CoefficientVector.AND; }
    }
  }
}
