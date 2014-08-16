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

namespace Logic
{
	/// <summary>
	/// A class that encapsulates simple functionality for measuring elapsed time within the code.
	/// </summary>
	public class Stopwatch
	{
    /// <summary>
    /// the time at which the stopwatch was last started
    /// </summary>
    private static DateTime oStartTime;

    /// <summary>
    /// Start the stopwatch counting.
    /// </summary>
    public static void Start()
    {
      oStartTime = DateTime.Now;
    }

    /// <summary>
    /// Get the time elapsed since the stopwatch was last started.
    /// </summary>
    /// <returns>the time elapsed since the stopwatch was last started</returns>
    public static TimeSpan ElapsedTime
    {
      get
      {
        return DateTime.Now - oStartTime;
      }
    }
	}
}
