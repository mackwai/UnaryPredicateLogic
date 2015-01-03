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
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logic
{
  [TestClass]
  public class PropertyTesting
  {
    private static bool IsPropositional( string aStatement )
    {
      return Parser.Parse( aStatement.Split( '\n' ) ).IsPropositional;
    }

    [TestMethod]
    public void Test_Propositional01()
    {
      Assert.AreEqual( true, IsPropositional( @"P" ) );
    }

    [TestMethod]
    public void Test_Propositional02()
    {
      Assert.AreEqual( true, IsPropositional( @"P&Q" ) );
    }

    [TestMethod]
    public void Test_Propositional03()
    {
      Assert.AreEqual( true, IsPropositional( @"P->Q" ) );
    }

    [TestMethod]
    public void Test_Propositional04()
    {
      Assert.AreEqual( true, IsPropositional( @"P->Q->R" ) );
    }

    [TestMethod]
    public void Test_Propositional05()
    {
     Assert.AreEqual( true, IsPropositional( @"~P->Q->R" ) );
    }

    [TestMethod]
    public void Test_Propositional06()
    {
      Assert.AreEqual( false, IsPropositional( @"P->(x,Qx)->R" ) );
    }

    [TestMethod]
    public void Test_Propositional07()
    {
      Assert.AreEqual( false, IsPropositional( @"P->(x,Qx)->(3x,R)" ) );
    }

    [TestMethod]
    public void Test_Propositional08()
    {
      Assert.AreEqual( false, IsPropositional( @"[]P&<>Q" ) );
    }

    [TestMethod]
    public void Test_Propositional09()
    {
      Assert.AreEqual( false, IsPropositional( @"[]P" ) );
    }

    [TestMethod]
    public void Test_Propositional10()
    {
      Assert.AreEqual( false, IsPropositional( @"<>Q" ) );
    }

    [TestMethod]
    public void Test_Propositional11()
    {
      Assert.AreEqual( false, IsPropositional( @"x,x=y" ) );
    }

    [TestMethod]
    public void Test_Propositional12()
    {
      Assert.AreEqual( false, IsPropositional( @"3x,P" ) );
    }

    private static int MaxmimumNumberOfModalitiesInIdentifications( string aStatement )
    {
      return Parser.Parse( aStatement.Split( '\n' ) ).MaxmimumNumberOfModalitiesInIdentifications;
    }

    [TestMethod]
    public void Test_MaxmimumNumberOfModalitiesInIdentifications1()
    {
      Assert.AreEqual( 1, MaxmimumNumberOfModalitiesInIdentifications( @"x,y, x=y" ) );
    }

    [TestMethod]
    public void Test_MaxmimumNumberOfModalitiesInIdentifications2()
    {
      Assert.AreEqual( 2, MaxmimumNumberOfModalitiesInIdentifications( @"x,[]y,x=y" ) );
    }

    [TestMethod]
    public void Test_MaxmimumNumberOfModalitiesInIdentifications3()
    {
      Assert.AreEqual( 2, MaxmimumNumberOfModalitiesInIdentifications( @"x,[]y,x=y|z=y" ) );
    }

    [TestMethod]
    public void Test_MaxmimumNumberOfModalitiesInIdentifications4()
    {
      Assert.AreEqual( 2, MaxmimumNumberOfModalitiesInIdentifications( @"x,([]y,x=y|(<>(z=y|x=y)))" ) );
    }

    [TestMethod]
    public void Test_MaxmimumNumberOfModalitiesInIdentifications5()
    {
      Assert.AreEqual( 3, MaxmimumNumberOfModalitiesInIdentifications( @"x,([]y,x=y|(<>3z,(z=y|x=y)))" ) );
    }

    [TestMethod]
    public void Test_ModalitiesInIdentifications()
    {
      Necessity[] lModalities = Parser.Parse(new string[]{@"x,([]y,x=y|(<>(z=y|x=y)))"}).ModalitiesInIdentifications.ToArray();
      Assert.IsTrue(lModalities.Contains(null));
    }
  }
}
