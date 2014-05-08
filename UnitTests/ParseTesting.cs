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
      string lGraph1 = UnitTestUtility.GetGraph( @"([]Gx) -> (3y,Gy&y=x)" );
      Type lType = typeof( Logic.NamedObject );
      FieldInfo lField = lType.GetField( "NextObjectNumber", BindingFlags.Static | BindingFlags.NonPublic );
      lField.SetValue( null, 1 );
      string lGraph2 = UnitTestUtility.GetGraph( @"([]Gx) -> (3y,Gy&y=x)" );

      Assert.AreEqual(
        lGraph1,
        lGraph2 );
    }

    [TestMethod]
    public void Test_Xor()
    {
      Logic.Parser.Parse( new string[]{"P^Q"} );
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
    }
  }
}