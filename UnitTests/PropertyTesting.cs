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

    private static bool IsCompatibleWithTreeProofGenerator( string aStatement )
    {
      return Parser.Parse( aStatement.Split( '\n' ) ).IsCompatibleWithTreeProofGenerator;
    }

    private static int DepthOfLoopNesting( string aStatement )
    {
      return Parser.Parse( aStatement.Split( '\n' ) ).DepthOfLoopNesting;
    }

    [TestMethod]
    public void Test_DepthOfLoopNesting01()
    {
      Assert.AreEqual( 0, DepthOfLoopNesting( @"P" ) );
    }

    [TestMethod]
    public void Test_DepthOfLoopNesting02()
    {
      Assert.AreEqual( 0, DepthOfLoopNesting( @"Px" ) );
    }

    [TestMethod]
    public void Test_DepthOfLoopNesting03()
    {
      Assert.AreEqual( 0, DepthOfLoopNesting( @"x=y" ) );
    }

    [TestMethod]
    public void Test_DepthOfLoopNesting04()
    {
      Assert.AreEqual( 1, DepthOfLoopNesting( @"x,Y" ) );
    }

    [TestMethod]
    public void Test_DepthOfLoopNesting05()
    {
      Assert.AreEqual( 1, DepthOfLoopNesting( @"3x,Yx" ) );
    }

    [TestMethod]
    public void Test_DepthOfLoopNesting06()
    {
      Assert.AreEqual( 1, DepthOfLoopNesting( @"~3x,Yx" ) );
    }

    [TestMethod]
    public void Test_DepthOfLoopNesting07()
    {
      Assert.AreEqual( 1, DepthOfLoopNesting( @"(~3x,Yx)|j,Kj" ) );
    }

    [TestMethod]
    public void Test_DepthOfLoopNesting08()
    {
      Assert.AreEqual( 2, DepthOfLoopNesting( @"~3x,Yx|j,Kj" ) );
    }

    [TestMethod]
    public void Test_DepthOfLoopNesting09()
    {
      Assert.AreEqual( 2, DepthOfLoopNesting( @"(t,P)&(~3x,Yx|j,Kj)" ) );
    }

    [TestMethod]
    public void Test_DepthOfLoopNesting10()
    {
      Assert.AreEqual( 3, DepthOfLoopNesting( @"(t,P)&(~3x,Yx|[]j,Kj)" ) );
    }

    [TestMethod]
    public void Test_DepthOfLoopNesting11()
    {
      Assert.AreEqual( 4, DepthOfLoopNesting( @"<>((t,P)&(~3x,Yx|[]j,Kj))" ) );
    }

    [TestMethod]
    public void Test_DepthOfLoopNesting12()
    {
      Assert.AreEqual( 1, DepthOfLoopNesting( @"<>Q->[]Q" ) );
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

    [TestMethod]
    public void Test_IsCompatibleWithTreeProofGenerator01()
    {
      Assert.AreEqual( true, IsCompatibleWithTreeProofGenerator( @"P&Q" ) );
    }

    [TestMethod]
    public void Test_IsCompatibleWithTreeProofGenerator02()
    {
      Assert.AreEqual( false, IsCompatibleWithTreeProofGenerator( @"P&[]Q" ) );
    }

    [TestMethod]
    public void Test_IsCompatibleWithTreeProofGenerator03()
    {
      Assert.AreEqual( true, IsCompatibleWithTreeProofGenerator( @"x,xRy" ) );
    }

    [TestMethod]
    public void Test_IsCompatibleWithTreeProofGenerator04()
    {
      Assert.AreEqual( false, IsCompatibleWithTreeProofGenerator( @"3y,x,xRy|x=y" ) );
    }

    [TestMethod]
    public void Test_IsCompatibleWithTreeProofGenerator05()
    {
      Assert.AreEqual( true, IsCompatibleWithTreeProofGenerator( @"((Px->Rx)&(J->xRx))" ) );
    }

    [TestMethod]
    public void Test_IsCompatibleWithTreeProofGenerator06()
    {
      Assert.AreEqual( false, IsCompatibleWithTreeProofGenerator( @"(((x=x<=>Px)->Rx)&(J->xRx))" ) );
    }

    [TestMethod]
    public void Test_IsCompatibleWithTreeProofGenerator07()
    {
      Assert.AreEqual( false, IsCompatibleWithTreeProofGenerator( @"((Px->Rx)&(J->[]xRx))" ) );
    }

    [TestMethod]
    public void Test_IsCompatibleWithTreeProofGenerator08()
    {
      Assert.AreEqual( false, IsCompatibleWithTreeProofGenerator( @"[]((Px->Rx)&(J->xRx))" ) );
    }
  }
}
