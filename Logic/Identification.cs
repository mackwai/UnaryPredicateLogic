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
using System.Text;

namespace Logic
{
  internal class Identification : Matrix
  {
    public Variable Left { get; private set; }
    public Variable Right { get; private set; }

    public Identification( Variable aLeft, Variable aRight )
    {
      Left = aLeft;
      Right = aRight;
    }

    public override string ToString()
    {
      return string.Format( "{0}={1}", Left, Right );
    }

    public override bool Equals( object obj )
    {
      Identification that = obj as Identification;

      if ( that == null )
        return false;
        //throw new EngineException( "Identification compared to an object that is not an Identification." );

      return ( this.Left.Equals( that.Left ) && this.Right.Equals( that.Right ) )
        || ( this.Left.Equals( that.Right ) && this.Right.Equals( that.Left ) );
    }

    internal override IEnumerable<NullPredicate> NullPredicates
    {
      get { yield break; }
    }

    internal override IEnumerable<Variable> FreeVariables
    {
      get { return IdentifiedVariables; }
    }

    internal override IEnumerable<Necessity> FreeModalities
    {
      get { return ModalitiesInIdentifications; }
    }

    internal override IEnumerable<Variable> IdentifiedVariables
    {
      get
      {
        yield return Left;

        if ( !Left.Equals( Right ) )
          yield return Right;
      }
    }

    internal override IEnumerable<Necessity> ModalitiesInIdentifications
    {
      get
      {
        yield return Left.Modality;

        if ( Left.Modality != Right.Modality )
          yield return Right.Modality;
      }
    }

    internal override IEnumerable<UnaryPredicate> UnaryPredicates
    {
      get { yield break; }
    }

    internal override bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicateDictionary )
    {
      return Left.InstantiatedKindOfObject == Right.InstantiatedKindOfObject
          && Left.InstantiatedKindOfWorld == Right.InstantiatedKindOfWorld;
    }

    public override int GetHashCode()
    {
      return Left.GetHashCode() ^ Right.GetHashCode();
    }

    internal override IEnumerable<Matrix> NonNullPredications
    {
      get { yield return this; }
    }

    internal override int MaxmimumNumberOfDistinguishableObjects
    {
      get { return IdentifiedVariables.Count(); }
    }

    internal override int MaxmimumNumberOfModalitiesInIdentifications
    {
      get { return ModalitiesInIdentifications.Count(); }
    }

    internal override string DOTLabel
    {
      get
      {
        return String.Format(
          "<Identification<BR/><B><FONT FACE=\"MONOSPACE\">{0}</FONT></B>>",
          this.ToString() );
      }
    }

    internal override Matrix Substitute( Variable aVariable, Variable aReplacement )
    {
      return new Identification( Left.Substitute( aVariable, aReplacement ), Right.Substitute( aVariable, aReplacement ) );
    }

    internal override string Prover9InputHelper( Dictionary<char, string> aTranslatedVariableNames )
    {
      return string.Format(
        "{0}={1}",
        aTranslatedVariableNames[ Left.ToString()[ 0 ] ],
        aTranslatedVariableNames[ Right.ToString()[ 0 ] ] );
    }
  }
}
