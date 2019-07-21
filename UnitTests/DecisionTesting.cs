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

    private static void TestPropositionFileWithProver9( string aPathToFile, int aTimeout )
    {
      string lText = File.ReadAllText( aPathToFile );
      Assert.AreEqual(
        UnitTestUtility.ParseExpectedResult( lText ).ToString(),
        UnitTestUtility.GetProver9sDecision( lText, aTimeout ).ToString() );
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

    private static void TestEngineExceptionThrown( string aStatement )
    {
      UnitTestUtility.ConfirmThrows<Logic.EngineException>( () => UnitTestUtility.GetDecision( aStatement ) );
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
      TestPropositionIs( Logic.Alethicity.Necessary, @"p<=>p" );
      TestPropositionIs( Logic.Alethicity.Contingent, @"p<=>P" );
      TestPropositionIs( Logic.Alethicity.Necessary, @"(P^p)->(P|p)" );
      TestPropositionIs( Logic.Alethicity.Contingent, @"p&q" );
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
    public void Test_XorOnUnboundVariables()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\XorOnUnboundVariables.txt" );
    }

    [TestMethod]
    public void Test_UnboundVariables1()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\UnboundVariables1.txt" );
    }

    [TestMethod]
    public void Test_UnboundVariables2()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\UnboundVariables2.txt" );
    }

    [TestMethod]
    public void Test_UnboundVariables3()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\UnboundVariables3.txt" );
    }

    [TestMethod]
    public void Test_UnboundVariables4()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\UnboundVariables4.txt" );
    }

    [TestMethod]
    public void Test_UnboundVariables5()
    {
      TestEngineExceptionThrown( File.ReadAllText( @"..\..\..\VerificationTesting\UnboundVariables5.txt" ) );
    }

    [TestMethod]
    public void Test_ProblemCaseTrivialExistentialQuantification()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ProblemCaseTrivialExistentialQuantification.txt" );
    }

    [TestMethod]
    public void Test_ProblemCaseTrivialUniversalGeneralization()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ProblemCaseTrivialUniversalGeneralization.txt" );
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
      TestEngineExceptionThrown(  File.ReadAllText( @"..\..\..\VerificationTesting\ModalTest2.txt" ) );
    }

    [TestMethod]
    public void Test_ModalTest3()
    {
      TestEngineExceptionThrown( File.ReadAllText( @"..\..\..\VerificationTesting\ModalTest3.txt" ) );
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
      Assert.Inconclusive( "This test passed on 2016-06-01 after running 19 hours." );
      //TestPropositionFile( @"..\..\..\VerificationTesting\BigLastWorldTested.txt" );
    }

    [TestMethod]
    public void Test_AxiomsOfIdentity()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\AxiomsOfIdentity.txt" );
    }

    [TestMethod]
    public void Test_TransworldIdentity1()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\TransworldIdentity1.txt" );
    }

    [TestMethod]
    public void Test_TransworldIdentity2()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\TransworldIdentity2.txt" );
    }

    [TestMethod]
    public void Test_TransworldIdentity3()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\TransworldIdentity3.txt" );
    }

    [TestMethod]
    public void Test_TransworldIdentity4()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\TransworldIdentity4.txt" );
    }

    [TestMethod]
    public void Test_TransworldIdentity5()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\TransworldIdentity5.txt" );
    }

    [TestMethod]
    public void Test_TransworldIdentity6()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\TransworldIdentity6.txt" );
    }

    [TestMethod]
    public void Test_TransworldIdentity7()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\TransworldIdentity7.txt" );
    }

    [TestMethod]
    public void Test_Identification2()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\Identification2.txt" );
    }

    [TestMethod]
    public void Test_Identification3()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\Identification3.txt" );
    }

    [TestMethod]
    public void Test_KindaSlowEvaluation()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\KindaSlowEvaluation.txt" );
    }

    [TestMethod]
    public void Test_ATerm()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ATerm.txt" );
    }

    [TestMethod]
    public void Test_ITerm()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ITerm.txt" );
    }

    [TestMethod]
    public void Test_ETerm()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ETerm.txt" );
    }

    [TestMethod]
    public void Test_OTerm()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\OTerm.txt" );
    }

    [TestMethod]
    public void Test_ModernATerm()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ModernATerm.txt" );
    }

    [TestMethod]
    public void Test_ModernITerm()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ModernITerm.txt" );
    }

    [TestMethod]
    public void Test_ModernETerm()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ModernETerm.txt" );
    }

    [TestMethod]
    public void Test_ModernOTerm()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ModernOTerm.txt" );
    }

    [TestMethod]
    public void Test_SquareOfOpposition()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\SquareOfOpposition.txt" );
    }

    [TestMethod]
    public void Test_SquareOfOppositionWithSomeNegations()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\SquareOfOppositionWithSomeNegations.txt" );
    }

    [TestMethod]
    public void Test_HexagonOfOpposition()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\HexagonOfOpposition.txt" );
    }

    [TestMethod]
    public void Test_ProblemCaseTermA()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ProblemCaseTermA.txt" );
    }

    [TestMethod]
    public void Test_ProblemCaseExistence()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\ProblemCaseExistence.txt" );
    }

    [TestMethod]
    public void Test_UniversalNecessaryExistence()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\UniversalNecessaryExistence.txt" );
    }

    [TestMethod]
    public void Test_FreeVariablesExistence()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\FreeVariablesExistence.txt" );
    }

    [TestMethod]
    public void Test_FreeVariables()
    {
      TestPropositionIs( Logic.Alethicity.Contingent, @"x,x=y" );
      //TestPropositionIs( Logic.Alethicity.Necessary, @"3x,x=y" );
      //TestPropositionIs( Logic.Alethicity.Impossible, @"x,~x=y" );
      //TestPropositionIs( Logic.Alethicity.Contingent, @"3x,~x=y" );
      //TestPropositionIs( Logic.Alethicity.Contingent, @"x,x=y&A" );
      //TestPropositionIs( Logic.Alethicity.Necessary, @"(Ax|A)|(Ax|~A)" );
      //TestPropositionIs( Logic.Alethicity.Contingent, @"<>Ax" );
      //TestPropositionIs( Logic.Alethicity.Necessary, @"(Ax&~Ax)->(~3y,y=x)" );
      //TestPropositionIs( Logic.Alethicity.Necessary, @"(Ax&~Ax)->([]~3y,y=x)" );
      //TestPropositionIs( Logic.Alethicity.Contingent, @"Ax&By" );
      //TestPropositionIs( Logic.Alethicity.Contingent, @"Ax|By" );
      //TestPropositionIs( Logic.Alethicity.Contingent, @"Ax->By" );
      //TestPropositionIs( Logic.Alethicity.Necessary, @"(x=y&Ax)->Ay" );
      //TestPropositionIs( Logic.Alethicity.Necessary, @"Ax|~Ax" );
      //TestPropositionIs( Logic.Alethicity.Impossible, @"~Ax&Ax" );
      //TestPropositionIs( Logic.Alethicity.Contingent, @"[]Ax" );
    }

    [TestMethod]
    public void Test_OrderOfOperations()
    {
      TestPropositionIs( Logic.Alethicity.Necessary, @"(A|B&C)<=>(A|(B&C))" );
      TestPropositionIs( Logic.Alethicity.Necessary, @"(A->B&C)<=>(A->(B&C))" );
      TestPropositionIs( Logic.Alethicity.Necessary, @"(A<=>B->C)<=>(A<=>(B->C))" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"(A|B!C)<=>(A|(B!C))" );
      TestPropositionIs( Logic.Alethicity.Necessary, @"(A-<B|C)<=>(A-<(B|C))" );
      TestPropositionIs( Logic.Alethicity.Necessary, @"(A^B-<C)<=>(A^(B-<C))" );

      TestPropositionIs( Logic.Alethicity.Contingent, @"(A!B&C)<=>(A!(B&C))" );
      TestPropositionIs( Logic.Alethicity.Contingent, @"(A-<B->C)<=>(A-<(B->C))" );

      TestPropositionIs( Logic.Alethicity.Necessary, @"(A!B&C)<=>((A!B)&C)" );
      TestPropositionIs( Logic.Alethicity.Necessary, @"(A->B-<C)<=>((A->B)-<C)" );
    }

    [TestMethod]
    public void Test_NumberedPropositions()
    {
      TestPropositionIs( Logic.Alethicity.Necessary, @"0AB	<=>	(~A&~B)" );
      TestPropositionIs( Logic.Alethicity.Necessary, @"1AB	<=>	((~A&B)|(A&~B))" );
      TestPropositionIs( Logic.Alethicity.Necessary, @"2AB	<=>	(A&B)" );
      TestPropositionIs( Logic.Alethicity.Necessary, @"0ABC	<=>	((~A&~B)&~C)" );
      TestPropositionIs( Logic.Alethicity.Necessary, @"1ABC	<=>	((((~A&~B)&C)|((~A&B)&~C))|((A&~B)&~C))" );
      TestPropositionIs( Logic.Alethicity.Necessary, @"2ABC	<=>	((((~A&B)&C)|((A&~B)&C))|((A&B)&~C))" );
      TestPropositionIs( Logic.Alethicity.Necessary, @"3ABC	<=>	((A&B)&C)" );
    }

    [TestMethod]
    public void Test_LowercasePredicatesInProver9()
    {
      TestPropositionIs( Logic.Alethicity.Contingent, @"p" );
      TestPropositionIs( Logic.Alethicity.Contingent, @"q" );
      TestPropositionIs( Logic.Alethicity.Contingent, @"a" );
      TestPropositionIs( Logic.Alethicity.Contingent, @"x" );
    }

  [TestMethod]
    public void Test_Number1()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\Number1.txt" );
    }

    [TestMethod]
    public void Test_Number2()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\Number2.txt" );
    }

    [TestMethod]
    public void Test_Number3()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\Number3.txt" );
    }

    [TestMethod]
    public void Test_Number4()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\Number4.txt" );
    }

    [TestMethod]
    public void Test_Number5()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\Number5.txt" );
    }

    [TestMethod]
    public void Test_Number6()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\Number6.txt" );
    }

    [TestMethod]
    public void Test_Number7()
    {
      TestPropositionFile( @"..\..\..\VerificationTesting\Number7.txt" );
    }

    [TestMethod]
    public void Test_Prover9ProblemCase01()
    {
      TestPropositionFileWithProver9( @"..\..\..\VerificationTesting\Prover9ProblemCase01.txt", 10 );
    }

    [TestMethod]
    public void Test_ExceptionThrownBecauseOfBinaryPredication()
    {
      TestEngineExceptionThrown( @"xRy" );
    }
  }
}
