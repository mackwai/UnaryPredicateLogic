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

namespace Logic
{
  public class BinaryPredicate
  {
    internal BinaryPredicate( char aLetter )
    {
      Letter = aLetter;
    }

    public char Letter { get; private set; }

    public override bool Equals( object obj )
    {
      if ( obj is UnaryPredicate )
      {
        UnaryPredicate that = obj as UnaryPredicate;

        return this.Letter == that.Letter;
      }

      throw new EngineException( "BinaryPredicate compared to non-BinaryPredicate." );
    }

    public override int GetHashCode()
    {
      return Letter.GetHashCode();
    }

    public override string ToString()
    {
      return Letter.ToString();
    }
  }
}
