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

using System.Linq;
using System.Collections.Generic;

namespace Logic
{
  /// <summary>
  /// a logical necessity; true if the matrix is true in all possible worlds, false otherwise
  /// </summary>
  internal class Necessity : UnaryOperator
  {
    private sbyte[] mTruthValues = null;

    internal Necessity( Matrix aMatrix )
      : base( aMatrix )
    {
      if ( Memoizible )
        mTruthValues = Utility.CreateSByteArray( 1 << 16 );
    }

    internal override bool ContainsModalities
    {
      get { return true; }
    }

    private bool Memoizible
    {
      get { return !mInnerMatrix.ContainsModalities && !this.FreeVariables.Any(); }
    }
    
    internal override bool TrueIn( uint aInterpetation, uint aKindOfWorld, Predicates aPredicates )
    {
      if ( Memoizible )
      {
        foreach ( uint lKindOfWorld in aPredicates.KindsOfWorlds( aInterpetation ) )
        {
          if ( mTruthValues[ lKindOfWorld ] == 0 )
            mTruthValues[ lKindOfWorld ] = mInnerMatrix.TrueIn( aInterpetation, lKindOfWorld, aPredicates ) ? (sbyte) 1 : (sbyte) -1;

          if ( mTruthValues[ lKindOfWorld ] == (sbyte) -1 )
            return false;
        }

        return true;
      }
      else
      {
        return aPredicates.KindsOfWorlds( aInterpetation )
          .All( fWorld => mInnerMatrix.TrueIn( aInterpetation, fWorld, aPredicates ) );
      }
    }
	  
	  public override string ToString()
	  {
	    return string.Format( "[]{0}", mInnerMatrix );
	  }

    internal override string DOTLabel
    {
      get { return "<Necessity<BR/><B><FONT FACE=\"MONOSPACE\">[]</FONT></B>>"; }
    }
  }
}
