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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.RegularExpressions
{
  internal static class RegexExtensions
  {
    public static RegexMatch Exec( this Regex aRegex, string aString )
    {
      Match lMatch = aRegex.Match( aString );
      return lMatch.Success
        ? new RegexMatch( aRegex.Match( aString ), aString )
        : null;
    }
  }

  internal class RegexMatch
  {
    private Match mMatch;
    private string mInput;
    private string[] mResults;

    internal RegexMatch( Match aMatch, string aInput )
    {
      mMatch = aMatch;
      mInput = aInput;
      mResults = aMatch.Groups.OfType<Group>().Select( fGroup => fGroup.Value ).ToArray();
    }

    public int Index
    {
      get { return mMatch.Index; }
    }

    public string Input
    {
      get { return mInput; }
    }

    public int Length
    {
      get { return mResults.Length; }
    }

    public string this[ int aIndex ]
    {
      get { return mResults[ aIndex ]; }
    }
  }
}
