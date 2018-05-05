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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic;


namespace UnitTests
{
  [TestClass]
  public class CounterexampleTesting
  {
    [TestMethod]
    public void Test_CounterexampleProblemCase1()
    {
      Matrix lMatrix = Logic.Parser.Parse( new string[] { "P & x,Gx" } );
      Counterexample lCounterexample = lMatrix.FindNextCounterexample();
    }

    [TestMethod]
    public void Test_Counterexample()
    {
      Matrix lMatrix = Logic.Parser.Parse( new string[] { "P<=>Q" } );
      Counterexample lCounterexample = lMatrix.FindNextCounterexample();
      KindOfWorld lWorld = lCounterexample as KindOfWorld;
      Assert.IsTrue( lWorld.Affirms( lWorld.Predicates.ElementAt( 0 ) ) );
      Assert.IsTrue( lWorld.Denies( lWorld.Predicates.ElementAt( 1 ) ) );

      lCounterexample = lMatrix.FindNextCounterexample();
      lWorld = lCounterexample as KindOfWorld;
      Assert.IsTrue( lWorld.Affirms( lWorld.Predicates.ElementAt( 1 ) ) );
      Assert.IsTrue( lWorld.Denies( lWorld.Predicates.ElementAt( 0 ) ) );

      lCounterexample = lMatrix.FindNextCounterexample();
      lWorld = lCounterexample as KindOfWorld;
      Assert.IsTrue( lWorld.Affirms( lWorld.Predicates.ElementAt( 0 ) ) );
      Assert.IsTrue( lWorld.Denies( lWorld.Predicates.ElementAt( 1 ) ) );
    }

    [TestMethod]
    public void Test_Example()
    {
      Matrix lMatrix = Logic.Parser.Parse( new string[] { "P<=>Q" } );

      Counterexample lCounterexample = lMatrix.FindNextExample();
      KindOfWorld lWorld = lCounterexample as KindOfWorld;
      Assert.IsTrue( lWorld.Denies( lWorld.Predicates.ElementAt( 0 ) ) );
      Assert.IsTrue( lWorld.Denies( lWorld.Predicates.ElementAt( 1 ) ) );

      lCounterexample = lMatrix.FindNextExample();
      lWorld = lCounterexample as KindOfWorld;
      Assert.IsTrue( lWorld.Affirms( lWorld.Predicates.ElementAt( 0 ) ) );
      Assert.IsTrue( lWorld.Affirms( lWorld.Predicates.ElementAt( 1 ) ) );

      lCounterexample = lMatrix.FindNextExample();
      lWorld = lCounterexample as KindOfWorld;
      Assert.IsTrue( lWorld.Denies( lWorld.Predicates.ElementAt( 0 ) ) );
      Assert.IsTrue( lWorld.Denies( lWorld.Predicates.ElementAt( 1 ) ) );
    }

    [TestMethod]
    public void Test_ProblemCase1()
    {
      Matrix lMatrix = Logic.Parser.Parse( new string[] { "P|Q" } );
      for ( int i = 1; i < 10; i++ )
      {
        Assert.IsNotNull( lMatrix.FindNextCounterexample(), String.Format( "Counterexemple not found for P|Q on iteration {0}", i ) );
      }
    }
  }
}
