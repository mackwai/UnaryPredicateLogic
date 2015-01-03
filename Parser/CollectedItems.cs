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
  /// collections of matrices, predicates and variables created while parsing a statement
  /// </summary>
  internal class CollectedItems
  {
    private Dictionary<char, UnaryPredicate> mUnaryPredicates;
    private Dictionary<char, NullPredicate> mNullPredicates;
    private Dictionary<Matrix, Matrix> mPredications;
    private Dictionary<Matrix, Matrix> mIdentifications;
    private Dictionary<char,Variable> mUnboundVariables;

    public CollectedItems()
    {
      mUnaryPredicates = new Dictionary<char, UnaryPredicate>();
      mNullPredicates = new Dictionary<char, NullPredicate>();
      mPredications = new Dictionary<Matrix, Matrix>();
      mIdentifications = new Dictionary<Matrix, Matrix>();
      mUnboundVariables = new Dictionary<char,Variable>();
    }   

    public Matrix AddIdentification( Variable aLeftVariable, Variable aRightVariable )
    {
      Matrix lIdentification = Factory.Identification( aLeftVariable, aRightVariable );
      if ( mIdentifications.ContainsKey( lIdentification ) )
      {
        return mIdentifications[ lIdentification ];
      }
      else
      {
        mIdentifications.Add( lIdentification, lIdentification );

        return lIdentification;
      }
    }
    
    public NullPredicate AddNullPredicate( char aSymbol )
    {
      if ( mNullPredicates.ContainsKey( aSymbol ) )
      {
        return mNullPredicates[ aSymbol ];
      }
      else
      {
        NullPredicate aPredicate = Factory.NullPredicate( aSymbol.ToString() );

        mNullPredicates.Add( aSymbol, aPredicate );

        return aPredicate;
      }
    }

    public UnaryPredicate AddUnaryPredicate( char aSymbol )
    {
      if ( mUnaryPredicates.ContainsKey( aSymbol ) )
      {
        return mUnaryPredicates[ aSymbol ];
      }
      else
      {
        UnaryPredicate aPredicate = Factory.UnaryPredicate( aSymbol );

        mUnaryPredicates.Add( aSymbol, aPredicate );

        return aPredicate;
      }
    }

    public Matrix AddUnaryPredication( UnaryPredicate aPredicate, Variable aVariable )
    {
      Matrix lPredication = Factory.Predication( aPredicate, aVariable );
      if ( mPredications.ContainsKey( lPredication ) )
      {
        return mPredications[ lPredication ];
      }
      else
      {
        mPredications.Add( lPredication, lPredication );

        return lPredication;
      }
    }

    public Variable AddUnboundVariable( char aSymbol )
    {
      if ( mUnboundVariables.ContainsKey( aSymbol ) )
      {
        return mUnboundVariables[ aSymbol ];
      }
      else
      {
        Variable aVariable = Factory.Variable( aSymbol );

        mUnboundVariables.Add( aSymbol, aVariable );

        return aVariable;
      }
    }

    public IEnumerable<Variable> UnboundVariables
    {
      get
      {
        return mUnboundVariables.Values;
      }
    }
  }
}
