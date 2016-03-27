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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Logic;

namespace UnitTests
{
  [TestClass]
  public class HTMLTesting
  {
    [TestMethod]
    public void TestMethod1()
    {
      File.WriteAllText(
        "test.html",
        "<!DOCTYPE html>\n<html><head><meta charset=\"UTF-8\" /><link rel=\"stylesheet\" type=\"text/css\" href=\"../../../WebApplication/style.css\" /></head><body>"
        + HTMLMaker.MakeHTML( Parser.Parse( new string[] { "~(P&Q&<>~(P|Q)&<>(P&~Q)&<>x,Gx&<>~x,Gx)" } ).FindCounterexample() )
        + "</body></html>" );
      System.Diagnostics.Process.Start( "test.html" );
    }
  }
}
