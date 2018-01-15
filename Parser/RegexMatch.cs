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
using System.Threading.Tasks;

namespace System.Text.RegularExpressions
{
  /// <summary>
  /// This provides a facade for .NET regular expressions that matches
  /// the regular expression classes available in Saltarelle, which are somewhat simpler and more limited.  It exists so the
  /// the normal Windows version of the code can do regular expression processing the same way as the Saltarelle
  /// version of the code.
  /// </summary>
  internal static class RegexExtensions
  {
    public static RegexMatch Exec( this Regex aRegex, string aString )
    {
      return aRegex.IsMatch( aString )
        ? new RegexMatch( aRegex.Match( aString ), aString )
        : null;
    }

    public static bool IsMatch( this Regex aRegex, string aString )
    {
      return aRegex.Match( aString ).Success;
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
