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

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Logic;

namespace UnitTests
{
  [TestClass]
  public class WebPageInputTesting
  {
    public static void TestEngineExceptionThrown( string aStatement )
    {
      try
      {
        Parser.Parse( aStatement.Split( '\n' ) ).TreeProofGeneratorInput.ToString();
        Assert.Fail( "Exception not thrown when converting \"{0}\" to input for Tree Proof Generator.", aStatement );
      }
      catch ( Logic.EngineException )
      {
      }
    }

    private static void LaunchTreeProofGeneratorPage( string aStatement )
    {
      System.Diagnostics.Process.Start(
        string.Format( "http://www.umsu.de/logik/trees/?f={0}",
        Parser.Parse( aStatement.Split( '\n' ) ).TreeProofGeneratorInput ) );
    }

    private static void ConfirmExceptionThrown( string aStatement )
    {
      System.Diagnostics.Process.Start(
        string.Format( "http://www.umsu.de/logik/trees/?f={0}",
        Parser.Parse( aStatement.Split( '\n' ) ).TreeProofGeneratorInput ) );
    }

    [TestMethod]
    public void Test_LaunchHurley454()
    {
      LaunchTreeProofGeneratorPage( File.ReadAllText( @"..\..\..\VerificationTesting\Hurley454.txt" ) );
    }

    [TestMethod]
    public void Test_LaunchETerm()
    {
      LaunchTreeProofGeneratorPage( File.ReadAllText( @"..\..\..\VerificationTesting\ETerm.txt" ) );
    }

    [TestMethod]
    public void Test_LaunchExesInTexas()
    {
      LaunchTreeProofGeneratorPage( File.ReadAllText( @"..\..\..\VerificationTesting\ExesInTexas.txt" ) );
    }

    [TestMethod]
    public void Test_LaunchMixedNullUnaryPredicates1()
    {
      LaunchTreeProofGeneratorPage( File.ReadAllText( @"..\..\..\VerificationTesting\MixedNullUnaryPredicates1.txt" ) );
    }

    [TestMethod]
    public void Test_LaunchIdentity()
    {
      TestEngineExceptionThrown( "x,y,x=y" );
    }

    [TestMethod]
    public void Test_LaunchNecessity()
    {
      TestEngineExceptionThrown( "[]Y" );
    }
  }
}