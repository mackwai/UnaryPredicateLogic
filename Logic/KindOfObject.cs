// somerby.net/mack/logic
// Copyright (C) 2016 MacKenzie Cumings
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
  public class KindOfObject
  {
    public uint NumberOfDistinguishableInstances { get; private set; }
    private Dictionary<UnaryPredicate,bool> mPredicates;
    public IEnumerable<UnaryPredicate> Predicates
    {
      get { return mPredicates.Keys.OrderBy( fPredicate => fPredicate.Letter ); }
    }

    public bool Affirms( UnaryPredicate aPredicate )
    {
      return mPredicates.ContainsKey( aPredicate ) ? mPredicates[ aPredicate ] : false;
    }

    public bool Denies( UnaryPredicate aPredicate )
    {
      return mPredicates.ContainsKey( aPredicate ) ? !mPredicates[ aPredicate ] : false;
    }

    public KindOfObject( uint aNumberOfDistinguishableInstances, IEnumerable<UnaryPredicate> aAffirmedPredicates, IEnumerable<UnaryPredicate> aDeniedPredicates )
    {
      NumberOfDistinguishableInstances = aNumberOfDistinguishableInstances;
      mPredicates = new Dictionary<UnaryPredicate,bool>();
      foreach ( UnaryPredicate lPredicate in aAffirmedPredicates )
      {
        mPredicates[ lPredicate ] = true;
      }
      foreach ( UnaryPredicate lPredicate in aDeniedPredicates )
      {
        mPredicates[ lPredicate ] = false;
      }

      if ( aAffirmedPredicates.Intersects( aDeniedPredicates ) )
        throw new EngineException( "Attempted to construct an invalid KindOfObject: some predicates are both affirmed and denied by the world." );
    }

    public override bool Equals( object obj )
    {
      KindOfObject that = obj as KindOfObject;
      
      if ( that == null )
        return false;

      if ( this.NumberOfDistinguishableInstances != that.NumberOfDistinguishableInstances )
        return false;

      if ( !Enumerable.SequenceEqual( this.Predicates, that.Predicates ) )
        return false;

      foreach ( UnaryPredicate lPredicate in Predicates )
      {
        if ( this.Affirms( lPredicate ) != that.Affirms( lPredicate ) )
          return false;
      }
      
      return true;
    }

    public override int GetHashCode()
    {
      return (int) ( this.NumberOfDistinguishableInstances << 6 )
        + this.Predicates
          .Select( fPredicate => (int) ( mPredicates[ fPredicate ] ? fPredicate.Letter : 4 ) )
          .Aggregate( 0, ( fOne, fTwo ) => ( fOne << 8 ) + fTwo );
      throw new EngineException( "No hash code defined for KindOfObject." );
    }
  }
}
