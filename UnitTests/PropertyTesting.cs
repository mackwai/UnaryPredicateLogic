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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logic
{
  [TestClass]
  public class PropertyTesting
  {
    private static bool IsPropositional( string aStatement )
    {
      return Parser.Parse( aStatement.Split( '\n' ) ).Propositional;
    }

    [TestMethod]
    public void TestPropositional01()
    {
      Assert.AreEqual( true, IsPropositional( @"P" ) );
    }

    [TestMethod]
    public void TestPropositional02()
    {
      Assert.AreEqual( true, IsPropositional( @"P&Q" ) );
    }

    [TestMethod]
    public void TestPropositional03()
    {
      Assert.AreEqual( true, IsPropositional( @"P->Q" ) );
    }

    [TestMethod]
    public void TestPropositional04()
    {
      Assert.AreEqual( true, IsPropositional( @"P->Q->R" ) );
    }

    [TestMethod]
    public void TestPropositional05()
    {
     Assert.AreEqual( true, IsPropositional( @"~P->Q->R" ) );
    }

    [TestMethod]
    public void TestPropositional06()
    {
      Assert.AreEqual( false, IsPropositional( @"P->(x,Qx)->R" ) );
    }

    [TestMethod]
    public void TestPropositional07()
    {
      Assert.AreEqual( false, IsPropositional( @"P->(x,Qx)->(3x,R)" ) );
    }

    [TestMethod]
    public void TestPropositional08()
    {
      Assert.AreEqual( false, IsPropositional( @"[]P&<>Q" ) );
    }

    [TestMethod]
    public void TestPropositional09()
    {
      Assert.AreEqual( false, IsPropositional( @"[]P" ) );
    }

    [TestMethod]
    public void TestPropositional10()
    {
      Assert.AreEqual( false, IsPropositional( @"<>Q" ) );
    }

    [TestMethod]
    public void TestPropositional11()
    {
      Assert.AreEqual( false, IsPropositional( @"x,x=y" ) );
    }

    [TestMethod]
    public void TestPropositional12()
    {
      Assert.AreEqual( false, IsPropositional( @"3x,P" ) );
    }
  }
}
