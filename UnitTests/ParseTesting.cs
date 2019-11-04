// somerby.net/mack/logic
// Copyright (C) 2015, 2017 MacKenzie Cumings
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
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

namespace UnitTests
{
  [TestClass]
  public class ParseTesting
  {
    [TestMethod]
    public void Test_ParseProblems()
    {
      Type lType = typeof( Logic.NamedObject );
      FieldInfo lField = lType.GetField( "NextObjectNumber", BindingFlags.Static | BindingFlags.NonPublic );
      lField.SetValue( null, 1 );
      string lGraph1 = UnitTestUtility.GetGraph( @"([]Gx) -> (3y,Gy&y=x)" );      
      lField.SetValue( null, 1 );
      string lGraph2 = UnitTestUtility.GetGraph( @"([]Gx) -> (3y,Gy&y=x)" );

      Assert.AreEqual(
        lGraph1,
        lGraph2 );
    }

    private static void ConfirmParseErrorOnParsing( params string[] aStatement )
    {
      UnitTestUtility.ConfirmThrows<Logic.ParseError>( () => Logic.Parser.Parse( aStatement ) );
    }

    [TestMethod]
    public void Test_ParseExceptionMessages()
    {
      ConfirmParseErrorOnParsing( @"<>A -> []<>" );
      ConfirmParseErrorOnParsing( @"<>A -> []x," );
    }

    [TestMethod]
    public void Test_ParseBinaryPredications()
    {
      Logic.Parser.Parse( new string[] { @"aRb" } );
      Logic.Parser.Parse( new string[] { @"x,y,Px&xZy" } );
      Logic.Parser.Parse( new string[] { @"x,y,DaO|xZy" } );
      Logic.Parser.Parse( new string[] { @"x,y,xZy|DaO" } );
      Logic.Parser.Parse( new string[] { "x,y,xRy->yRx", "aRb", "->", "bRa" } );
      ConfirmParseErrorOnParsing( @"x,y,xZyDaO" );
      ConfirmParseErrorOnParsing( @"x,y,xZyDyO" );
    }

    [TestMethod]
    public void Test_Xor()
    {
      Logic.Parser.Parse( new string[]{"P^Q"} );
    }

    [TestMethod]
    public void Test_Terms()
    {
      Logic.Parser.Parse( new string[] { "PoQ" } );
      Logic.Parser.Parse( new string[] { "PaQ" } );
      Logic.Parser.Parse( new string[] { "PiQ" } );
      Logic.Parser.Parse( new string[] { "PeQ" } );
      Logic.Parser.Parse( new string[] { "PyQ" } );
      Logic.Parser.Parse( new string[] { "PuQ" } );
      Logic.Parser.Parse( new string[] { "~PoQ" } );
      Logic.Parser.Parse( new string[] { "~PaQ" } );
      Logic.Parser.Parse( new string[] { "~PiQ" } );
      Logic.Parser.Parse( new string[] { "~PeQ" } );
      Logic.Parser.Parse( new string[] { "~Po~Q" } );
      Logic.Parser.Parse( new string[] { "~Pa~Q" } );
      Logic.Parser.Parse( new string[] { "~Pi~Q" } );
      Logic.Parser.Parse( new string[] { "~Pe~Q" } );
      Logic.Parser.Parse( new string[] { "Po~Q" } );
      Logic.Parser.Parse( new string[] { "Pa~Q" } );
      Logic.Parser.Parse( new string[] { "Pi~Q" } );
      Logic.Parser.Parse( new string[] { "Pe~Q" } );
      Logic.Parser.Parse( new string[] { "x,~Po~Q&Rx" } );
      Logic.Parser.Parse( new string[] { "<>~Pa~Q" } );
      Logic.Parser.Parse( new string[] { "<>(~Pi~Q|3a,PaR)" } );
      Logic.Parser.Parse( new string[] { "<>(~Pi~Q|3a,PaR)&P&Pa~P" } );
    }

    [TestMethod]
    public void Test_TermsWithModernInterpretation()
    {
      Logic.Parser.Parse( new string[] { "PoQ'" } );
      Logic.Parser.Parse( new string[] { "PaQ'" } );
      Logic.Parser.Parse( new string[] { "PiQ'" } );
      Logic.Parser.Parse( new string[] { "PeQ'" } );
      Logic.Parser.Parse( new string[] { "PyQ'" } );
      Logic.Parser.Parse( new string[] { "PuQ'" } );
      Logic.Parser.Parse( new string[] { "~PoQ'" } );
      Logic.Parser.Parse( new string[] { "~PaQ'" } );
      Logic.Parser.Parse( new string[] { "~PiQ'" } );
      Logic.Parser.Parse( new string[] { "~PeQ'" } );
      Logic.Parser.Parse( new string[] { "~Po~Q'" } );
      Logic.Parser.Parse( new string[] { "~Pa~Q'" } );
      Logic.Parser.Parse( new string[] { "~Pi~Q'" } );
      Logic.Parser.Parse( new string[] { "~Pe~Q'" } );
      Logic.Parser.Parse( new string[] { "Po~Q'" } );
      Logic.Parser.Parse( new string[] { "Pa~Q'" } );
      Logic.Parser.Parse( new string[] { "Pi~Q'" } );
      Logic.Parser.Parse( new string[] { "Pe~Q'" } );
      Logic.Parser.Parse( new string[] { "x,~Po~Q'&Rx" } );
      Logic.Parser.Parse( new string[] { "<>~Pa~Q'" } );
      Logic.Parser.Parse( new string[] { "<>(~Pi~Q'|3a,PaR')" } );
      Logic.Parser.Parse( new string[] { "<>(~Pi~Q'|3a,PaR')&P&Pa~P'" } );
    }

    [TestMethod]
    public void Test_NumberedPredication()
    {
      Logic.Matrix lMatrix = Logic.Parser.Parse( new string[] { "1A" } );
      lMatrix = Logic.Parser.Parse( new string[] { "2A" } );
      lMatrix = Logic.Parser.Parse( new string[] { "3A" } );
      lMatrix = Logic.Parser.Parse( new string[] { "0A" } );
    }

    [TestMethod]
    public void Test_Propositions()
    {
      Logic.Matrix lMatrix = Logic.Parser.Parse( new string[] { "A" } );
      lMatrix = Logic.Parser.Parse( new string[] { "a" } );
      lMatrix = Logic.Parser.Parse( new string[] { "a<=>A" } );
      lMatrix = Logic.Parser.Parse( new string[] { "a&b" } );
    }

    [TestMethod]
    public void Test_NumberedPropositions()
    {
      foreach ( string lStatement in new string[] { "0AB", "1AB", "2AB", "0ABC", "1ABC", "2ABC", "3ABC" } )
      {
        Console.WriteLine( "{0}\t<=>\t{1}", lStatement, Logic.Parser.Parse( new string[] { lStatement } ) );
      }

      foreach ( string lStatement in new string[] { "0AB <=> C", "C <=> 1AB", "A & 3x,2BC & D" } )
      {
        //Logic.Parser.Parse( new string[] { lStatement } );
        Console.WriteLine( "{0}\t<=>\t{1}", lStatement, Logic.Parser.Parse( new string[] { lStatement } ) );
      }

      foreach ( string lStatement in new string[] { "3AB", "4ABC", "5ABC", "6ABC" } )
      {
        UnitTestUtility.ConfirmThrows<Logic.EngineException>( () => Logic.Parser.Parse( new string[] {  lStatement }  ) );
      }
    }

    private static void ParseFile( string aPathToFile )
    {
      string lText = File.ReadAllText( aPathToFile );
      try
      {
        Logic.Parser.Parse( lText.Split( '\n' ) );
      }
      catch ( Logic.ParseError )
      {
      }
    }

    [TestMethod]
    public void Test_ParseErrors()
    {
      ParseFile( @"..\..\..\VerificationTesting\UnhelpfulParseErrorMessage.txt" );

      try
      {
        Logic.Parser.Parse( new string[] { "()" } );
      }
      catch ( Exception lException )
      {
        Assert.IsTrue( lException is Logic.ParseError, "Exception is not a ParseError." );
      }
    }

    [TestMethod]
    public void Test_MutliLineStatements()
    {
      Assert.AreEqual(
        @"((a&b)&c)",
        Logic.Parser.Parse( new string[] {
          "( a",
          " & b",
          " & c // Comment!",
          ")" } ).ToString() );

      ConfirmParseErrorOnParsing(
          "( a",
          "  b",
          " & c // Comment!",
          ")" );

      ConfirmParseErrorOnParsing(
          "( a",
          "  b",
          " & c // )",
          ")" );

      ConfirmParseErrorOnParsing(
          "( a",
          " & b",
          " & c )",
          ")" );

      ConfirmParseErrorOnParsing(
          "( a",
          "  b",
          " & c // )" );

      Assert.AreEqual(
        @"((a|b)|c)",
        Logic.Parser.Parse( new string[] {
          "a",
          " | b |",
          "c",
          "" } ).ToString() );

      Assert.AreEqual(
        @"((a|b)|c)",
        Logic.Parser.Parse( new string[] {
          "a",
          " | b |",
          "c" } ).ToString() );

      Assert.AreEqual(
        @"(a->((a|b)|c))",
        Logic.Parser.Parse( new string[] {
          "a",
          "->",
          "a",
          " | b |",
          "c" } ).ToString() );

      Assert.AreEqual(
        @"((a&b)->((a|b)|c))",
        Logic.Parser.Parse( new string[] {
          "a",
          "b",
          "->",
          "( a",
          " | b |",
          "c )" } ).ToString() );

      Assert.AreEqual(
        @"((a&(b&c))->d)",
        Logic.Parser.Parse( new string[] {
          "a",
          "b",
          "&c",
          "->",
          "d" } ).ToString() );

      Assert.AreEqual(
        @"(((a&b)&c)->d)",
        Logic.Parser.Parse( new string[] {
          "a",
          "&b",
          "&c",
          "->",
          "d" } ).ToString() );

      Assert.AreEqual(
        @"(((p->~q)&(~p->~q))->~q)",
        Logic.Parser.Parse( new string[] {
         "(p->~q)",
         "(~p->~q)",
         ".'.",
         "~q" } ).ToString() );

      Assert.AreEqual(
        @"(A<=>A)",
        Logic.Parser.Parse( new string[] {
         "( A",
         "<=>",
         "A )" } ).ToString() );

      Assert.AreEqual(
        @"(A<=>A)",
        Logic.Parser.Parse( new string[] {
         "( A",
         "<=>",
         "A ",
         ")" } ).ToString() );
    }

    [TestMethod]
    public void Test_Replacement()
    {
      Assert.AreEqual(
        @"(B<=>C)",
        Logic.Parser.Parse( new string[] { "#replace == <=>", "B==C" } ).ToString() );
      Assert.AreEqual(
        @"(G<=>C)",
        Logic.Parser.Parse( new string[] { "#replace B G", "B<=>C" } ).ToString() );
      Assert.AreEqual(
        @"(G<=>C)",
        Logic.Parser.Parse( new string[] { "#replace B G", "#replace == <=>", "B==C" } ).ToString() );
      Assert.AreEqual(
        @"(G<=>C)",
        Logic.Parser.Parse( new string[] { "#replace mervyn G", "mervyn <=>C" } ).ToString() );
      Assert.AreEqual(
        @"(G<=>C)",
        Logic.Parser.Parse( new string[] { "#replace mervyn G", "mervyn<=>C" } ).ToString() );
      Assert.AreEqual(
        @"(G<=>C)",
        Logic.Parser.Parse( new string[] { "#replace mervyn C", "G<=>mervyn" } ).ToString() );
      Assert.AreEqual(
        @"(G<=>C)",
        Logic.Parser.Parse( new string[] { "#replace mervyn =", "G<mervyn>C" } ).ToString() );
      Assert.AreEqual(
        @"(G<=>(C->D))",
        Logic.Parser.Parse( new string[] { "#replace => ->", "G<=>C=>D" } ).ToString() );

      Assert.AreEqual(
        @"(((x,(Hx->Mx))&Hs)->Ms)",
        Logic.Parser.Parse( new string[] {
          "#replace => ->",
          "#replace man H",
          "#replace mortal M",
          "#replace socrates s",
          "#replace therefore ->",
          "",
          "x, man x => mortal x",
          "man socrates",
          "therefore",
          "mortal socrates" } ).ToString() );

      Assert.AreEqual(
        Logic.Alethicity.Necessary,
        Logic.Parser.Parse( new string[] {
          "#replace => ->",
          "#replace man H",
          "#replace mortal M",
          "#replace socrates s",
          "#replace therefore ->",
          "",
          "x, man x=> mortal x",
          "man socrates",
          "therefore",
          "mortal socrates" } ).Decide() );
    }
  }
}