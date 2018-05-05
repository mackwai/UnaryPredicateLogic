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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
  /// <summary>
  /// a factory which controls the creation of Variables, Predicates and Matrices.
  /// </summary>
	public class Factory
	{
	  /// <summary>
	  /// Create a variable with a letter assigned to it, so a string representation can be created for it.
	  /// </summary>
    /// <param name="aLetter">a lowercase letter that stands for the variable</param>
	  public static Variable Variable( char aLetter )
	  {
	    return new Variable( aLetter );
	  }

    /// <summary>
    /// Create a Binary predicate.
    /// </summary>
    /// <param name="aLetter">A capital letter that stands for the predicate</param>
    public static BinaryPredicate BinaryPredicate( char aLetter )
    {
      return new BinaryPredicate( aLetter );
    }
	  
	  /// <summary>
	  /// Create a unary predicate.
	  /// </summary>
    /// <param name="aLetter">A capital letter that stands for the predicate</param>
	  public static UnaryPredicate UnaryPredicate( char aLetter )
	  {
	    return new UnaryPredicate( aLetter );
	  }

    /// <summary>
    /// Create a null predicate.
    /// </summary>
    /// <param name="aLetter">A capital letter that stands for the predicate</param>
    public static NullPredicate NullPredicate( string aLetter )
    {
      return new NullPredicate( aLetter );
    }
	  
	  /// <summary>
	  /// Create a predication on a single variable.
	  /// </summary>
	  /// <param name="aPredicate">a predicate</param>
	  /// <param name="aVariable">a variable</param>
	  /// <returns>a new predication</returns>
	  public static Matrix Predication( UnaryPredicate aPredicate, Variable aVariable )
	  {
	    return new UnaryPredication( aPredicate, aVariable );
	  }

    /// <summary>
    /// Create a predication on two variables.
    /// </summary>
    /// <param name="aPredicate">a predicate</param>
    /// <param name="aVariable">a variable</param>
    /// <param name="aVariable">another variable</param>
    /// <returns>a new predication</returns>
    public static Matrix Predication( BinaryPredicate aPredicate, Variable aVariable1, Variable aVariable2 )
    {
      return new BinaryPredication( aPredicate, aVariable1, aVariable2 );
    }

    /// <summary>
    /// Create an identification of two variables.
    /// </summary>
    /// <param name="aLeft">a variable</param>
    /// <param name="aRight">another variable</param>
    /// <returns>a new identification</returns>
    public static Matrix TheSame( Variable aLeft, Variable aRight )
    {
      return new Identification( aLeft, aRight );
    }

    /// <summary>
    /// Create a joint denial; all truth-functional relationships can be defined in terms of NOR.
    /// </summary>
    /// <param name="aLeft">the left side of the joint denial</param>
    /// <param name="aRight">the right side of the joint denial</param>
    public static Matrix Nor( Matrix aLeft, Matrix aRight )
    {
      return new JointDenial( aLeft, aRight );
    }

    /// <summary>
    /// Create an exclusive disjunction.
    /// </summary>
    /// <param name="aLeft">the left side of the exclusive disjunction</param>
    /// <param name="aRight">the right side of the exclusive disjunction</param>
    public static Matrix Xor( Matrix aLeft, Matrix aRight )
    {
      return Not( IfAndOnlyIf( aLeft, aRight ) );
    }

    /// <summary>
    /// Create a universal quantification.
    /// </summary>
    /// <param name="aVariable">the variable representing each object in the world</param>
    /// <param name="aInnerMatrix">that which is being asserted for each object in the world</param>
    public static Matrix ForAll( Variable aVariable, Matrix aInnerMatrix )
    {
      return new UniversalGeneralization( aVariable, aInnerMatrix );
    }

    /// <summary>
    /// D1
    /// </summary>
    /// <param name="aInnerMatrix">that which is being negated</param>
    public static Matrix Not( Matrix aInnerMatrix )
    {
      //return Nor( aInnerMatrix, aInnerMatrix );
      return new Negation( aInnerMatrix );
    }

    /// <summary>
    /// D2 and D6
    /// </summary>
    /// <param name="aLeft">the left side of the conjunction</param>
    /// <param name="aRight">the right side of the conjunction</param>
    /// <param name="aOthers">more matrices to conjoin with aLeft and aRight</param>
    public static Matrix And( Matrix aLeft, Matrix aRight, params Matrix[] aOthers )
    {
      //	    if ( aOthers.Length == 0 )
      //	      return Nor( Not( aLeft ), Not( aRight ) );
      if ( aOthers.Length == 0 )
        return new Conjunction( aLeft, aRight );
      else
        return And( And( aLeft, aRight ), aOthers.Head(), aOthers.Tail() );
    }

    /// <summary>
    /// D2 and D6
    /// </summary>
    /// <param name="aConjuncts">matrices to be conjoined</param>
    public static Matrix And( IEnumerable<Matrix> aConjuncts )
    {
      try
      {
        return aConjuncts.Aggregate( ( fConjunction, fNext ) => And( fConjunction, fNext ) );
      }
      catch ( InvalidOperationException )
      {
        throw new EngineException( "Can't conjoin an empty list of matrices." );
      }
    }

    /// <summary>
    /// D3 and D7
    /// </summary>
    public static Matrix Or( Matrix aLeft, Matrix aRight, params Matrix[] aOthers )
    {
      //	    if ( others.Length == 0 )
      //	      return Not( Nor( aLeft, aRight ) );
      if ( aOthers.Length == 0 )
        return new Disjunction( aLeft, aRight );
      else
        return Or( Or( aLeft, aRight ), aOthers.Head(), aOthers.Tail() );
    }

    /// <summary>
    /// D3 and D7
    /// </summary>
    /// <param name="aConjuncts">matrices to be disjoined</param>
    public static Matrix Or( IEnumerable<Matrix> aDisjuncts )
    {
      try
      {
        return aDisjuncts.Aggregate( ( fDisjunction, fNext ) => Or( fDisjunction, fNext ) );
      }
      catch ( InvalidOperationException )
      {
        throw new EngineException( "Can't disjoin an empty list of matrices." );
      }
    }

    /// <summary>
    /// D4
    /// </summary>
    /// <param name="aAntecedent">the antecedent of the conditional</param>
    /// <param name="aConsequent">the consequent of the conditional</param>
    public static Matrix OnlyIf( Matrix aAntecedent, Matrix aConsequent )
    {
      //return Or( Not( antecedent ), consequent );
      return new MaterialConditional( aAntecedent, aConsequent );
    }

    /// <summary>
    /// D5
    /// </summary>
    /// <param name="aLeft">the left operand</param>
    /// <param name="aRight">the right operand</param>
    public static Matrix IfAndOnlyIf( Matrix aLeft, Matrix aRight )
    {
      //return And( Entails( left, right ), Entails( right, left ) );
      return new Equivalence( aLeft, aRight );
    }

    /// <summary>
    /// D8 
    /// </summary>
    /// <param name="aVariable">the variable representing the object or objects for which the matrix is being asserted</param>
    /// <param name="aInnerMatrix">the matrix that is being asserted</param>
    /// <returns>an existential quantifier</returns>
    public static Matrix ThereExists( Variable aVariable, Matrix aInnerMatrix )
    {
      return Not( ForAll( aVariable, Not( aInnerMatrix ) ) );
    }

    /// <summary>
    /// Definite Description
    /// </summary>
    /// <param name="aVariable">the variable representing the one object for which the matrix is being asserted</param>
    /// <param name="aInnerMatrix">the matrix that is being asserted</param>
    /// <returns>an definite description</returns>
    public static Matrix The( Variable aVariable, Matrix aInnerMatrix )
    {
      char lSymbol = aVariable.ToString()[0];
      char lNext = lSymbol;
      lNext++;
      Variable lGamma = Variable( (lSymbol == 'z') ? 'a' : lNext );
      return ThereExists( lGamma, ForAll( aVariable, IfAndOnlyIf( TheSame( aVariable, lGamma ), aInnerMatrix ) ) );
    }

    /// <summary>
    /// Create an assertion of logical necessity.
    /// </summary>
    /// <param name="aInnerMatrix">the matrix being asserted for all nonempty worlds</param>
    /// <returns>a logical necessity</returns>
    public static Matrix Necessarily( Matrix aInnerMatrix )
    {
      return new Necessity( aInnerMatrix );
    }

    /// <summary>
    /// Create an assertion of possibility.
    /// </summary>
    /// <param name="aInnerMatrix">the matrix being asserted for one or more nonempty worlds</param>
    /// <returns>an assertion of possibility</returns>
    public static Matrix Possibly( Matrix aInnerMatrix )
    {
      return Not( Impossibly( aInnerMatrix ) );
    }

    /// <summary>
    /// Create an assertion of impossibility.
    /// </summary>
    /// <param name="aInnerMatrix">the matrix being denied for all nonempty worlds</param>
    /// <returns>an assertion of impossibility</returns>
    public static Matrix Impossibly( Matrix aInnerMatrix )
    {
      return Necessarily( Not( aInnerMatrix ) );
    }

    /// <summary>
    /// Create a term.
    /// </summary>
    /// <param name="aPredicate">a unary predicate</param>
    /// <param name="aIsNegative">true if the term contains all and only objects which verify the predicate,
    /// false if the term contains all and only objects which falsify the predicate</param>
    /// <returns>a term</returns>
    public static Term AllAndOnly( UnaryPredicate aPredicate, bool aIsNegative )
    {
      return new Term( aPredicate, aIsNegative );
    }

    /// <summary>
    /// Create a two-term proposition of form A.
    /// </summary>
    /// <param name="aSubject">the subject term</param>
    /// <param name="aPredicate">the predicate term</param>
    /// <param name="aExistentialImport">true if the proposition has existential import, false if not.  This paramter defaults to
    /// true, per Aristotle's interpretation</param>
    /// <returns>a two-term proposition of form A</returns>
    public static Matrix FormA( Term aSubject, Term aPredicate, bool aExistentialImport = true )
    {
      Variable lVariable1 = Variable( 'x' );

      Matrix lProposition = ForAll( lVariable1, Factory.OnlyIf( aSubject.Apply( lVariable1 ), aPredicate.Apply( lVariable1 ) ) );

      if ( aExistentialImport )
      {
        Variable lVariable2 = Variable( 'x' );
        lProposition = And( lProposition, ThereExists( lVariable2, aSubject.Apply( lVariable2 ) ) );
      }

      return lProposition;
    }

    /// <summary>
    /// Create a two-term proposition of form I.
    /// </summary>
    /// <param name="aSubject">the subject term</param>
    /// <param name="aPredicate">the predicate term</param>
    /// <returns>a two-term proposition of form I</returns>
    public static Matrix FormI( Term aSubject, Term aPredicate )
    {
      Variable lVariable = Variable( 'x' );

      return ThereExists( lVariable, And( aSubject.Apply( lVariable ), aPredicate.Apply( lVariable ) ) );
    }

    /// <summary>
    /// Create a two-term proposition of form E.
    /// </summary>
    /// <param name="aSubject">the subject term</param>
    /// <param name="aPredicate">the predicate term</param>
    /// <returns>a two-term proposition of form E</returns>
    public static Matrix FormE( Term aSubject, Term aPredicate )
    {
      Variable lVariable = Variable( 'x' );

      return ForAll( lVariable, Factory.OnlyIf( aSubject.Apply( lVariable ), Not( aPredicate.Apply( lVariable ) ) ) );
    }

    /// <summary>
    /// Create a two-term proposition of form O.
    /// </summary>
    /// <param name="aSubject">the subject term</param>
    /// <param name="aPredicate">the predicate term</param>
    /// <param name="aExistentialImport">true if the proposition has existential import, false if not.  This paramter defaults to
    /// false, in order to complete the square of opposition under Aristotle's interpretation of form A</param>
    /// <returns>a two-term proposition of form O</returns>
    public static Matrix FormO( Term aSubject, Term aPredicate, bool aExistentialImport = false )
    {
      Variable lVariable1 = Variable( 'x' );

      //Matrix lProposition = ThereExists( lVariable1, Factory.And( aSubject.Apply( lVariable1 ), Not( aPredicate.Apply( lVariable1 ) ) ) );
      Matrix lProposition = Not( ForAll( lVariable1, Factory.OnlyIf( aSubject.Apply( lVariable1 ), aPredicate.Apply( lVariable1 ) ) ) );

      if ( !aExistentialImport )
      {
        Variable lVariable2 = Variable( 'x' );
        lProposition = Or( lProposition, ForAll( lVariable2, Not( aSubject.Apply( lVariable2 ) ) ) );
      }

      return lProposition;
    }

    /// <summary>
    /// Create a two-term proposition of form U.
    /// </summary>
    /// <param name="aSubject">the subject term</param>
    /// <param name="aPredicate">the predicate term</param>
    /// <returns>a two-term proposition of form U</returns>
    public static Matrix FormU( Term aSubject, Term aPredicate, bool aExistentialImport = false )
    {
      Variable lVariable = Variable( 'x' );

      return Or(
        ForAll( lVariable, Factory.OnlyIf( aSubject.Apply( lVariable ), aPredicate.Apply( lVariable ) ) ),
        ForAll( lVariable, Factory.OnlyIf( aSubject.Apply( lVariable ), Not( aPredicate.Apply( lVariable ) ) ) ) );
    }

    /// <summary>
    /// Create a two-term proposition of form Y.
    /// </summary>
    /// <param name="aSubject">the subject term</param>
    /// <param name="aPredicate">the predicate term</param>
    /// <returns>a two-term proposition of form Y</returns>
    public static Matrix FormY( Term aSubject, Term aPredicate, bool aExistentialImport = false )
    {
      Variable lVariable = Variable( 'x' );

      return And(
        ThereExists( lVariable, And( aSubject.Apply( lVariable ), aPredicate.Apply( lVariable ) ) ),
        ThereExists( lVariable, And( aSubject.Apply( lVariable ), Not( aPredicate.Apply( lVariable ) ) ) ) );
    }

    /// <summary>
    /// Strict Implication
    /// </summary>
    /// <param name="aAntecedent">the antecedent of the conditional</param>
    /// <param name="aConsequent">the consequent of the conditional</param>
    public static Matrix NecessarilyOnlyIf( Matrix aAntecedent, Matrix aConsequent )
    {
      return Necessarily( OnlyIf( aAntecedent, aConsequent ) );
    }

    /// <summary>
    /// Bind a set of variables in a matrix with a quantifier.
    /// </summary>
    /// <param name="aVariables">a set of variables</param>
    /// <param name="aMatrix">a matrix</param>
    /// <param name="aQuantify">a function that binds a variable with a quantifier and returns the resulting Matrix</param>
    /// <returns>a matrix</returns>
    public static Matrix Bind( IEnumerable<Variable> aVariables, Matrix aMatrix, Func<Variable, Matrix, Matrix> aQuantify )
    {
      Matrix lResult = aMatrix;

      foreach ( Variable lVariable in aVariables )
      {
        lResult = aQuantify( lVariable, lResult );
      }

      return lResult;
    }

    private static Variable[] MakeNVariables( uint aNumber )
    {
      char lChar = 'a';
      List<Variable> lVariables = new List<Variable>();
      for ( int i = 0; i < aNumber; i++ )
      {
        lVariables.Add( Variable( lChar ) );
        if ( lChar == 'z' )
          lChar = 'a';
        else
          lChar++;
      }
      return lVariables.ToArray();
    }

    /// <summary>
    /// D11
    /// </summary>
    /// <param name="aLeft">a variable</param>
    /// <param name="aRight">another variable</param>
    /// <returns>a matrix asserting that the two variables are instantiated with distinct objects</returns>
    public static Matrix NotTheSame( Variable aLeft, Variable aRight )
    {
      return Not( TheSame( aLeft, aRight ) );
    }

    private static Matrix AreDistinct( IEnumerable<Variable> aVariables )
    {
      Tuple<Variable,Variable>[] lPairs = Utility.Pairs( aVariables ).ToArray();
      Matrix lMatrix = NotTheSame( lPairs.First().Item1, lPairs.First().Item2 );
      foreach ( Tuple<Variable,Variable> lPair in lPairs.Skip(1) )
      {
        lMatrix = And( lMatrix, NotTheSame( lPair.Item1, lPair.Item2 ) );
      }
      return lMatrix;
    }

    private static Matrix AllAre( IEnumerable<Variable> aVariables, UnaryPredicate aPredicate )
    {
      Matrix lMatrix = Predication( aPredicate, aVariables.First() );
      foreach ( Variable lVariable in aVariables.Skip( 1 ) )
      {
        lMatrix = And( lMatrix, Predication( aPredicate, lVariable ) );
      }
      return lMatrix;
    }

    private static Matrix Disjoin( IEnumerable<Matrix> aMatrices )
    {
      Matrix lResult = aMatrices.First();
      foreach ( Matrix lMatrix in aMatrices.Skip(1) )
      {
        lResult = Or( lResult, lMatrix );
      }
      return lResult;
    }

    public static Argument Therefore( IEnumerable<Matrix> aPremises, Matrix aConclusion )
    {
      return new Argument( aPremises, aConclusion );
    }

    public static Matrix ThereAreThisManyOfThese( uint aNumber, UnaryPredicate aPredicate )
    {
      Variable[] lVariables = MakeNVariables( Math.Max( aNumber, 1 ) );
      switch ( aNumber )
      {
        case 0:
          return ForAll( lVariables[ 0 ], Not( Predication( aPredicate, lVariables[ 0 ] ) ) );
        case 1:
          return The( lVariables[ 0 ], Predication( aPredicate, lVariables[ 0 ] ) );
        default:
          Variable lVariable = Variable( 'x' );
          return Bind(
            lVariables,
            And(
              AllAre( lVariables, aPredicate ),
              AreDistinct( lVariables ),
              ForAll( lVariable, OnlyIf( Predication( aPredicate, lVariable ), Disjoin( lVariables.Select( fVariable => TheSame( lVariable, fVariable ) ) ) ) ) ),
            ThereExists );
      }
    }

    /// <summary>
    /// Select all subsets of indices of a given size from the indices in an array.
    /// </summary>
    /// <param name="aNumber">the size the subsets</param>
    /// <param name="aTotal">the number of items in the array</param>
    /// <returns></returns>
    private static IEnumerable<IEnumerable<int>> Choose( uint aNumber, int aTotal )
    {
      //int aSizeOfPowerSet = 2 << aTotal;
      //for ( int i = 0; i < aSizeOfPowerSet; i++ )
      //{
      //  List<int> lIndices = new List<int>();

      //}
      if ( aNumber == 0 )
      {
        return new IEnumerable<int>[] { new int[] { } };
      }
      else if ( aNumber >= aTotal )
      {
        return new IEnumerable<int>[] { Enumerable.Range( 0, aTotal ) };
      }
      else
      {
        return Choose( aNumber, aTotal - 1 ).Concat( Choose( aNumber - 1, aTotal - 1 ).Select( fChoice => fChoice.Concat( new int[] { aTotal - 1 } ) ) );
      }
    }

    private static Matrix Combine( IEnumerable<int> aIndicesOfNegatedMatrices, Matrix[] aMatrices )
    {
      Matrix[] lMatrices = aMatrices.Clone() as Matrix[];
      foreach ( int i in aIndicesOfNegatedMatrices )
      {
        lMatrices[ i ] = Not( lMatrices[ i ] );
      }
      return And( lMatrices );
    }

    /// <summary>
    /// Creates a Matrix equivalent to the proposition that a specified number of matrices in a
    /// list of matrices are true and the rest are false.
    /// </summary>
    /// <param name="aNumber">the number of null predictates</param>
    /// <param name="aMatrices">the list of null predicates</param>
    /// <returns>a Matrix equivalent to the proposition that a specified number of null predicates in a
    /// list of null predicates are true and the rest are false</returns>
    public static Matrix ThisManyOfTheseAreTrue( uint aNumber, IEnumerable<Matrix> aMatrices )
    {
      Matrix[] lMatrices = aMatrices.ToArray();

      if ( lMatrices.Length < aNumber )
      {
        throw new EngineException(
          @"{0} of the predicate(s) {1} can't be true because there are only {2} of them.",
          aNumber,
          string.Join( ", ", (object[]) lMatrices ),
          lMatrices.Length );
      }

      return Or( Choose( (uint) lMatrices.Length - aNumber, lMatrices.Length ).Select(
        fIndicesOfNegatedPredicates => Combine( fIndicesOfNegatedPredicates, lMatrices ) ) );
    }
  }
}
