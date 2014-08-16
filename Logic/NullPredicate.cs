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

using System.Collections.Generic;
using System.Linq;

namespace Logic
{
  public class NullPredicate : Matrix
  {
    private string mDescription;

    internal NullPredicate( string aDescription )
    {
      mDescription = aDescription;
    }

    internal override bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicates )
    {
      return aPredicates.TrueIn( this, aKindOfWorld );
    }

    public override bool Propositional
    {
      get { return true; }
    }

    public override string ToString()
    {
      return mDescription;
    }

    internal override IEnumerable<UnaryPredicate> UnaryPredicates()
    {
      yield break;
    }

    internal override IEnumerable<NullPredicate> NullPredicates()
    {
      yield return this;
    }

    internal override IEnumerable<Variable> FreeVariables
    {
      get { yield break; }
    }

    internal override IEnumerable<Variable> IdentifiedVariables
    {
      get { yield break; }
    }

    internal override string DOTLabel
    {
      get
      {
        return string.Format(
          "<Null Predicate<BR/><B><FONT FACE=\"MONOSPACE\">{0}</FONT></B>>",
          this );
      }
    }

    internal override int MaxmimumNumberOfDistinguishableObjects
    {
      get { return 0; }
    }
  }
}
