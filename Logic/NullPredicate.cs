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
  public class NullPredicate : Matrix
  {
    private string mDescription;

    internal NullPredicate( string aDescription )
    {
      mDescription = aDescription;
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

    internal override string Prover9InputHelper( Dictionary<char, string> aTranslatedVariableNames )
    {
      return string.Format( "{0}0", this );
    }

    internal override IEnumerable<Variable> FreeVariables
    {
      get { yield break; }
    }

    internal override IEnumerable<Necessity> FreeModalities
    {
      get { yield break; }
    }

    internal override IEnumerable<Variable> IdentifiedVariables
    {
      get { yield break; }
    }

    public override bool IsPropositional
    {
      get { return true; }
    }

    internal override int MaxmimumNumberOfDistinguishableObjectsOfAKind
    {
      get { return 0; }
    }

    internal override int MaxmimumNumberOfModalitiesInIdentifications
    {
      get { return 0; }
    }

    internal override IEnumerable<NullPredicate> NullPredicates
    {
      get { yield return this; }
    }

    internal override IEnumerable<UnaryPredicate> UnaryPredicates
    {
      get { yield break; }
    }

    internal override bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicates )
    {
      return aPredicates.TrueIn( this, aKindOfWorld );
    }

    internal override Matrix Substitute( Variable aVariable, Variable aReplacement )
    {
      return this;
    }

    public override string ToString()
    {
      return mDescription;
    }

    public override string TreeProofGeneratorInput
    {
      get { return mDescription.ToLower(); }
    }
  }
}
