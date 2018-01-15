using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Logic
{
  /// <summary>
  /// Summary description for FactoryTesting
  /// </summary>
  [TestClass]
  public class FactoryTesting
  {
    [TestMethod]
    public void Test_Subjunction1()
    {
      Matrix[] lBackground = new Matrix[] {
        Parser.Parse( new string[] { "A" } ),
        Parser.Parse( new string[] { "B" }  ),
      };
      //Console.WriteLine( Factory.Subjunction( Factory.Not( lBackground[ 1 ] ), lBackground ) );
      Assert.Inconclusive();
    }

    [TestMethod]
    public void Test_Subjunction2()
    {
      Matrix[] lBackground = new Matrix[] {
        Parser.Parse( new string[] { "A" } ),
        Parser.Parse( new string[] { "B" } ),
        Parser.Parse( new string[] { "C" } )
      };
      //Console.WriteLine( Factory.Subjunction( Factory.Not( lBackground[ 1 ] ), lBackground ) );
      Assert.Inconclusive();
    }
  }
}
