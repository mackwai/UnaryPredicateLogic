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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
  internal class ShellParameters
  {
    public string Executable { get; private set; }
    public string Arguments { get; private set; }
    public string StandardInput { get; private set; }

    public ShellParameters( string aExecutable, string aStandardInput )
    {
      this.Executable = aExecutable;
      this.StandardInput = aStandardInput;
    }

    public ShellParameters( string aExecutable, string aArguments, string aStandardInput )
    {
      this.Executable = aExecutable;
      this.Arguments = aArguments;
      this.StandardInput = aStandardInput;
    }
  }
}
