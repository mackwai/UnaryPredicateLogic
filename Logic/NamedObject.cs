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

namespace Logic
{
  /// <summary>
  /// A base class for objects that should automatically be assigned a unique name on construction.
  /// The name will be unique only for the current execution.
  /// </summary>
  public abstract class NamedObject
  {
#if SALTARELLE
    private static int NextObjectNumber = 1;
#else
    private static volatile int NextObjectNumber = 1;
#endif

    /// <summary>
    /// The object's name.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Assign a unique name to the object.
    /// </summary>
    protected NamedObject()
    {
#if SALTARELLE
      Name = NextObjectNumber.ToString();
#else
      Name = NextObjectNumber.ToString( "000000" );
#endif
      NextObjectNumber++;
    }
  }
}
