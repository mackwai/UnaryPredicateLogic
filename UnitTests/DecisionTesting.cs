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
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

namespace UnitTests
{
  [TestClass]
  public class DecisionTesting
  {
    private static void TestPropositionFile( string aPathToFile )
    {
      string lText = File.ReadAllText( aPathToFile );
      Assert.AreEqual( UnitTestUtility.ParseExpectedResult( lText ), UnitTestUtility.GetDecision( lText ) );
    }

    private static void TestPropositionIs( Logic.Alethicity aAlethicity, string aStatement )
    {
      Assert.AreEqual(
        aAlethicity,
        UnitTestUtility.GetDecision( aStatement ),
        aStatement );
    }

    private static void TestPropositionIsNot( Logic.Alethicity aAlethicity, string aStatement )
    {
      Assert.AreNotEqual(
        aAlethicity,
        UnitTestUtility.GetDecision( aStatement ),
        aStatement );
    }

    [TestMethod]
    public void Test_ModalPropositionsFromEncyclopediaOfPhilosophy()
    {
      TestPropositionIs( Logic.Alethicity.Necessary, @"[]P <=> ~<>~P" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"[]~P <=> ~<>P" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"~[]P <=> <>~P" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"~[]~P <=> <>P" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"[]~P -> ~P" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"[]P -> ~[]~P" );

      TestPropositionIsNot( Logic.Alethicity.Necessary, @"~P -> []~P" );

      TestPropositionIsNot( Logic.Alethicity.Necessary, @"~[]~P -> []P" );

      TestPropositionIsNot( Logic.Alethicity.Necessary, @"~[]~P -> []P" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"[]P -> P" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"[]~P -> ~P" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"~P -> ~[]P" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"P -> ~[]~P" );

      TestPropositionIsNot( Logic.Alethicity.Necessary, @"(P->[]Q) <=> [](P->Q)" );

      TestPropositionIsNot( Logic.Alethicity.Necessary, @"P->[]P" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"[](P->P)" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"([](P->Q)&[]P) -> []Q" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"[](P->Q) -> ([]P -> []Q)" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"[](P->Q) -> (<>P -> <>Q)" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"([](P->Q) & ~<>Q) -> ~<>P" );

      TestPropositionIsNot( Logic.Alethicity.Necessary, @"[](P|Q) <=> ([]P | []Q)" );

      TestPropositionIsNot( Logic.Alethicity.Necessary, @"(<>P & <>Q) <=> <>(P&Q)" );

      TestPropositionIsNot( Logic.Alethicity.Impossible, @"<>P & <>~P" );

      TestPropositionIs( Logic.Alethicity.Impossible, @"<>(P & ~P)" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"(P & ~Q) -> ~(P -> Q)" );

      TestPropositionIsNot( Logic.Alethicity.Impossible, @"(P->Q)&P&Q" );

      TestPropositionIsNot( Logic.Alethicity.Impossible, @"(P->Q)&~P&Q" );

      TestPropositionIsNot( Logic.Alethicity.Impossible, @"(P->Q)&~P&~Q" );

      //TestPropositionIs( Logic.Alethicity.Necessary, @"(~<>P & ~<>Q) -> ([](P->Q) & [](Q->P))" );

      //TestPropositionIs( Logic.Alethicity.Necessary, @"([]P & []Q) -> ([](P->Q) & [](Q->P))" );

      //TestPropositionIs( Logic.Alethicity.Necessary, @"([]([](P->Q)->[]([](Q->R)->[](P->R))))&([]((P&R)->((P|R)&R)))&([](((P|Q)&~P)->Q))" );
    }

    [TestMethod]
    public void Test_Socrates()
    {
       TestPropositionFile( @"..\..\..\VerificationTesting\Socrates.txt" );
    }

    [TestMethod]
    public void Test_SimpleContradiction()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\SimpleContradiction.txt" );
    }

    [TestMethod]
    public void Test_RulesOfPropositionalLogic()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\RulesOfPropositionalLogic.txt" );
    }

    [TestMethod]
    public void Test_BigNullPredicates()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\BigNullPredicates.txt" );
    }

    [TestMethod]
    public void Test_NullPredicates()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\NullPredicates.txt" );
    }

    [TestMethod]
    public void Test_MixedNullUnaryPredicates1()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\MixedNullUnaryPredicates1.txt" );
    }

    [TestMethod]
    public void Test_DiSpezio135()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\DiSpezio135.txt" );
    }

    [TestMethod]
    public void Test_Hurley213()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\Hurley213.txt" );
    }

    [TestMethod]
    public void Test_Hurley454()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\Hurley454.txt" );
    }

    [TestMethod]
    public void Test_Hurley457()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\Hurley457.txt" );
    }

    [TestMethod]
    public void Test_ModalTest1()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ModalTest1.txt" );
    }

    [TestMethod]
    public void Test_ModalTest2()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ModalTest2.txt" );
    }

    [TestMethod]
    public void Test_ModalTest3()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ModalTest3.txt" );
    }

    [TestMethod]
    public void Test_SocratesOverload()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\SocratesOverload.txt" );
    }

    [TestMethod]
    public void Test_AxiomsOfModalLogic()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\AxiomsOfModalLogic.txt" );
    }

    [TestMethod]
    public void Test_EmptyWorldTest()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\EmptyWorldTest.txt" );
    }

    [TestMethod]
    public void Test_ToBeOrNotToBe()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ToBeOrNotToBe.txt" );
    }

    [TestMethod]
    public void Test_LastWorldTested()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\LastWorldTested.txt" );
    }

    [TestMethod]
    public void Test_BigLastWorldTested()
    {
      Assert.Inconclusive( "This one takes too long." );
      //TestPropositionFile( @"..\..\..\VerificationTesting\BigLastWorldTested.txt" );
    }

    [TestMethod]
    public void Test_AxiomsOfIdentity()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\AxiomsOfIdentity.txt" );
    }
  }
}
