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
  internal class VariableDictionary
  {
    internal Dictionary<char, Variable> mVariables;

    public VariableDictionary()
    {
      mVariables = new Dictionary<char, Variable>();
    }

    protected void Add( char aSymbol, Variable aVariable )
    {
      mVariables.Add( aSymbol, aVariable );
    }

    public VariableDictionary CreateNewSetThatRebinds( char aSymbol, Variable aVariable )
    {
      VariableDictionary lUpdatedDictionary = new VariableDictionary();

      foreach ( char lSymbol in mVariables.Keys )
      {
        if ( lSymbol != aSymbol )
          lUpdatedDictionary.Add( lSymbol, mVariables[ lSymbol ] );
      }

      lUpdatedDictionary.Add( aSymbol, aVariable );

      return lUpdatedDictionary;
    }

    public bool ContainsVariableForSymbol( char aSymbol )
    {
      return mVariables.ContainsKey( aSymbol );
    }

    public Variable Retrieve( char aSymbol )
    {
      try
      {
        return mVariables[ aSymbol ];
      }
      catch ( KeyNotFoundException )
      {
        throw new FreeVariableNotFoundException( aSymbol );
      }
    }
  }
}
