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
  internal class BinaryPredication : Matrix
  {
    private Variable mVariable1;
    private Variable mVariable2;
    private BinaryPredicate mPredicate;

    internal BinaryPredication( BinaryPredicate aPredicate, Variable aVariable1, Variable aVariable2 )
    {
      mPredicate = aPredicate;
      mVariable1 = aVariable1;
      mVariable2 = aVariable2;
    }

    internal override string DOTLabel
    {
      get
      {
        return string.Format(
          "<Binary Predication<BR/><B><FONT FACE=\"MONOSPACE\">{0}</FONT></B>>",
          this );
      }
    }

    internal override IEnumerable<Necessity> FreeModalities
    {
      get
      {
        yield return mVariable1.Modality;

        if ( (mVariable1.Modality == null) != (mVariable2.Modality == null) )
          yield return mVariable2.Modality;
        else if ( mVariable1.Modality != null && mVariable2.Modality != null && !mVariable1.Modality.Equals( mVariable2.Modality ) )
          yield return mVariable2.Modality;
      }
    }

    internal override IEnumerable<Variable> FreeVariables
    {
      get
      {
        yield return mVariable1;

        if ( !mVariable2.Equals( mVariable1 ) )
          yield return mVariable2;
      }
    }

    internal override IEnumerable<Variable> IdentifiedVariables
    {
      get { yield break; }
    }

    internal override int MaxmimumNumberOfDistinguishableObjects
    {
      get { return 2; }
    }

    internal override int MaxmimumNumberOfModalitiesInIdentifications
    {
      get { return 0; }
    }

    internal override IEnumerable<Matrix> NonNullPredications
    {
      get { yield return this; }
    }

    internal override IEnumerable<NullPredicate> NullPredicates
    {
      get { yield break; }
    }

    internal override IEnumerable<UnaryPredicate> UnaryPredicates
    {
      get { yield break; }
    }

    internal override bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicates )
    {
      throw new EngineException( "Can't decide a proposition that contains binary predicates." );
    }

    internal override Matrix Substitute( Variable aVariable, Variable aReplacement )
    {
      return new BinaryPredication(
        mPredicate, mVariable1.Substitute( aVariable, aReplacement ),
        mVariable2.Substitute( aVariable, aReplacement ) );
    }

    public override bool Equals( object obj )
    {
      BinaryPredication that = obj as BinaryPredication;

      if ( that == null )
        return false;

      return this.mPredicate.Equals( that.mPredicate )
        && this.mVariable1.Equals( that.mVariable1 )
        && this.mVariable2.Equals( that.mVariable2 );
    }

    public override int GetHashCode()
    {
      return mPredicate.GetHashCode() ^ mVariable1.GetHashCode() ^ mVariable2.GetHashCode();
    }

    internal override string Prover9InputHelper( Dictionary<char, string> aTranslatedVariableNames )
    {
      return string.Format(
        "{1}2({0},{2})",
        aTranslatedVariableNames[ mVariable1.ToString()[ 0 ] ],
        mPredicate,
        aTranslatedVariableNames[ mVariable2.ToString()[ 0 ] ] );
    }

    public override string ToString()
    {
      return string.Format( "{0}{1}{2}", mVariable1, mPredicate, mVariable2 );
    }
  }
}
