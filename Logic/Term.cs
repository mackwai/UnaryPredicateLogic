namespace Logic
{
  /// <summary>
  /// a term of Term Logic, interpreted as a unary predicate or the negation of a unary predicate
  /// </summary>
  public class Term
  {
    private bool mIsNegative;
    private UnaryPredicate mPredicate;

    internal Term( UnaryPredicate aPredicate, bool aIsNegative )
    {
      mIsNegative = aIsNegative;
      mPredicate = aPredicate;
    }

    /// <summary>
    /// Apply the term to a variable, yielding a Matrix representing whether or not the term contains an object.
    /// </summary>
    /// <param name="aVariable">a variable</param>
    /// <returns>a Matrix representing whether or not the term contains an object</returns>
    public Matrix Apply( Variable aVariable )
    {
      return mIsNegative
        ? Factory.Not( Factory.Predication( mPredicate, aVariable ) )
        : Factory.Predication( mPredicate, aVariable );
    }
  }
}
