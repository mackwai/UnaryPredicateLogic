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

namespace Logic
{
  /// <summary>
  /// a predication on one variable
  /// </summary>
  internal class UnaryPredication : Matrix
  {
    private Variable mVariable;
    private UnaryPredicate mPredicate;
    
    internal UnaryPredication( UnaryPredicate aPredicate, Variable aVariable )
    {
      mPredicate = aPredicate;
      mVariable = aVariable;
    }
    
    internal override bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicates )
    {
      return mVariable.InstantiatedKindOfObject.IndexOf( mPredicate.ToString() ) >= 0;
    }
    
    public override string ToString()
    {
      return string.Format( "{0}{1}", mPredicate, mVariable );
    }
    
    internal override IEnumerable<UnaryPredicate> UnaryPredicates()
    {
      yield return mPredicate;
    }

    internal override IEnumerable<NullPredicate> NullPredicates()
    {
      yield break;
    }

    internal override IEnumerable<Variable> FreeVariables
    {
      get { yield return mVariable; }
    }

    internal override IEnumerable<Necessity> FreeModalities
    {
      get { yield return mVariable.Modality; }
    }

    internal override IEnumerable<Variable> IdentifiedVariables
    {
      get { yield break; }
    }

    internal override IEnumerable<Matrix> NonNullPredications
    {
      get { yield return this; }
    }

    internal override int MaxmimumNumberOfDistinguishableObjects
    {
      get { return 1; }
    }

    internal override int MaxmimumNumberOfModalitiesInIdentifications
    {
      get { return 0; }
    }

    internal override string DOTLabel
    {
      get
      {
        return string.Format(
          "<Unary Predication<BR/><B><FONT FACE=\"MONOSPACE\">{0}</FONT></B>>",
          this );
      }
    }

    public override bool Equals( object obj )
    {
      UnaryPredication that = obj as UnaryPredication;

      if ( that == null )
        throw new EngineException( "Object of type {0} compared to a {1}", obj.GetType(), this.GetType() );

      return this.mPredicate.Equals( that.mPredicate ) && this.mVariable.Equals( that.mVariable );
    }

    public override int GetHashCode()
    {
      return mPredicate.GetHashCode() ^ mVariable.GetHashCode();
    }
  }
}
