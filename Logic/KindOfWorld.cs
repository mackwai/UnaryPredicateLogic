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
  public class KindOfWorld : Counterexample
  {
    public KindOfObject[] KindsOfObjects { get; private set; }
    
    private Dictionary<NullPredicate, bool> mPredicates;
    public IEnumerable<NullPredicate> Predicates
    {
      get { return mPredicates.Keys.OrderBy( fPredicate => fPredicate.ToString() ); }
    }

    public bool Affirms( NullPredicate aPredicate )
    {
      return mPredicates.ContainsKey( aPredicate ) ? mPredicates[ aPredicate ] : false;
    }

    public bool Denies( NullPredicate aPredicate )
    {
      return mPredicates.ContainsKey( aPredicate ) ? !mPredicates[ aPredicate ] : false;
    }

    public KindOfWorld( IEnumerable<KindOfObject> aKindsOfObjects, IEnumerable<NullPredicate> aAffirmedPredicates, IEnumerable<NullPredicate> aDeniedPredicates )
    {
      KindsOfObjects = aKindsOfObjects.Distinct().ToArray();

      mPredicates = new Dictionary<NullPredicate, bool>();
      foreach ( NullPredicate lPredicate in aAffirmedPredicates )
      {
        mPredicates[ lPredicate ] = true;
      }
      foreach ( NullPredicate lPredicate in aDeniedPredicates )
      {
        mPredicates[ lPredicate ] = false;
      }

      if ( aAffirmedPredicates.Intersects( aDeniedPredicates ) )
        throw new EngineException( "Attempted to construct an invalid KindOfWorld: some predicates are both affirmed and denied by the object." );
    }
  }
}
