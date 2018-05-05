using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
  /// <summary>
  /// Summary description for ArgumentTesting
  /// </summary>
  [TestClass]
  public class ArgumentTesting
  {
    private static void TestArgumentIs( Logic.Quality aQuality, string aStatement )
    {
      Assert.AreEqual(
        aQuality,
        UnitTestUtility.GetQuality( aStatement ),
        aStatement );
    }
   

    [TestMethod]
    public void Test_ArgumentValid()
    {
      TestArgumentIs(
        Logic.Quality.Valid,
        @"P
P->Q
.'.
Q" );
    }

    [TestMethod]
    public void Test_ArgumentInvalid()
    {
      TestArgumentIs(
        Logic.Quality.Invalid,
        @"P
P->Q
.'.
J" );
    }

    [TestMethod]
    public void Test_ArgumentInconsistentPremises()
    {
      TestArgumentIs(
        Logic.Quality.InconsistentPremises,
        @"P&~Q
P->Q
.'.
~P" );
    }

    [TestMethod]
    public void Test_ArgumentTautologicalConclusion()
    {
      TestArgumentIs(
        Logic.Quality.TautologicalConclusion,
        @"P
P->Q
.'.
~P|P" );
    }

    [TestMethod]
    public void Test_ArgumentInconsistentPremisesAndTautologicalConclusion()
    {
      TestArgumentIs(
        Logic.Quality.InconsistentPremisesAndTautologicalConclusion,
        @"P&~Q
P->Q
.'.
~P|P" );
    }
  }
}
