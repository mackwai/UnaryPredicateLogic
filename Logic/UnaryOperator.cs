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

using System;
using System.Collections.Generic;

namespace Logic
{
  /// <summary>
  /// an abstract class for unary logical operators
  /// </summary>
  internal abstract class UnaryOperator : Matrix
  {
    protected readonly Matrix mInnerMatrix;
    
    public UnaryOperator( Matrix aInnerMatrix )
    {
      mInnerMatrix = aInnerMatrix;
    }
    
    internal override IEnumerable<UnaryPredicate> UnaryPredicates()
    {
      return mInnerMatrix.UnaryPredicates();
    }

    internal override IEnumerable<NullPredicate> NullPredicates()
    {
      return mInnerMatrix.NullPredicates();
    }

    internal override System.Collections.Generic.IEnumerable<Variable> FreeVariables
    {
      get { return mInnerMatrix.FreeVariables; }
    }

    internal override System.Collections.Generic.IEnumerable<Variable> IdentifiedVariables
    {
      get { return mInnerMatrix.IdentifiedVariables; }
    }

    internal override bool ContainsModalities
    {
      get { return mInnerMatrix.ContainsModalities; }
    }

    internal override IEnumerable<Matrix> NonNullPredications
    {
      get { return mInnerMatrix.NonNullPredications; }
    }

    internal override IEnumerable<Tuple<UniversalGeneralization, Matrix>> ClosedPredications
    {
      get { return mInnerMatrix.ClosedPredications; }
    }

    internal override IEnumerable<Tuple<Matrix, Matrix>> DirectDependencies
    {
      get
      {
        yield return Tuple.Create( this as Matrix, mInnerMatrix );
        foreach ( Tuple<Matrix, Matrix> lPair in mInnerMatrix.DirectDependencies )
        {
          yield return lPair;
        }
      }
    }

    internal override IEnumerable<Matrix> Matrices
    {
      get
      {
        yield return this;
        foreach ( Matrix lMatrix in mInnerMatrix.Matrices )
        {
          yield return lMatrix;
        }
      }
    }

    internal override int MaxmimumNumberOfDistinguishableObjects
    {
      get { return mInnerMatrix.MaxmimumNumberOfDistinguishableObjects; }
    }
  }
}
