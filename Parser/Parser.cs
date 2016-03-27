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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Logic
{
  /// <summary>
  /// Contains methods for parsing symbolic logic expressions.
  /// </summary>
  public static class Parser
  {
    /// <summary>
    /// Parse a file containing a list of statements or a logical argument.  Ignore comments and whitespace.
    /// Interpret each line as a statement; conjoin the statements into one statement.  If one line contains
    /// a binary operator, conjoin all statements before it and make them the left side of the operation and
    /// conjoin all statements after it and make them the right side of the operation.  Throw a ParseError
    /// if the text can't be parsed.
    /// </summary>
    /// <param name="aFileContents">lines of text, as from a text file</param>
    /// <returns>a proposition</returns>
    public static Matrix Parse( string[] aFileContents )
    {
      List<string> lAntecedents = new List<string>();
      List<string> lConsequents = new List<string>();
      string lConnective = null;
      foreach ( string line in aFileContents )
      {
        // Get rid of comments.
        string lAdjustedLine = Utility.RegexReplace( line, @"//.*$", "" );

        if ( ( lAdjustedLine.Contains( "(" ) || lAdjustedLine.Contains( ")" ) )
          && !HasMatchingParentheses( lAdjustedLine ) )
        {
          throw new ParseError( "Found unmatched parenthesis in \"{0}\"", lAdjustedLine );
        }

        // Get rid of whitespace.
        lAdjustedLine = Utility.RegexReplace( lAdjustedLine, @"\s+", "" );

        // Skip empty lines.
        if ( lAdjustedLine.Length == 0 )
          continue;

        if ( lAdjustedLine.IsBinaryOperator() )
        {
          lConnective = lAdjustedLine;
          continue;
        }

        if ( lConnective == null )
          lAntecedents.Add( lAdjustedLine );
        else
          lConsequents.Add( lAdjustedLine );
      }

      if ( lConnective != null )
      {
        if ( lAntecedents.Count == 0 )
          throw new ParseError( "No antecedents found in argument" );
        else if ( lConsequents.Count == 0 )
          throw new ParseError( "No consequents found in argument" );
        else
          return Parse( Parenthesize( Conjoin( lAntecedents ) ) + lConnective + Parenthesize( Conjoin( lConsequents ) ) );
      }
      else
      {
        if ( lAntecedents.Count == 0 )
          throw new ParseError( "No content found." );
        else
          return Parse( Conjoin( lAntecedents ) );
      }
    }

    private static Regex The = ExactMatch( @"1[a-z]," );
    private static Regex ThereExists = ExactMatch( @"3[a-z]," );
    private static Regex ThereAreThisManyOfThese = ExactMatch( @"\d+[A-Z]" );
    private static Regex ThisManyOfTheseAreTrue = ExactMatch( @"(\d+)([A-Z]{2,})" );
    private static Regex ForAll = ExactMatch( @"[a-z]," );
    private static Regex And = ExactMatch( @"&" );
    private static Regex Or = ExactMatch( @"\|" );
    private static Regex OnlyIf = ExactMatch( @"->" );
    private static Regex IfAndOnlyIf = ExactMatch( @"<=>" );
    private static Regex Not = ExactMatch( @"~" );
    private static Regex Nor = ExactMatch( @"!" );
    private static Regex Xor = ExactMatch( @"\^" );
    private static Regex NecessarilyOnlyIf = ExactMatch( @"-<" );
    private static Regex Is = ExactMatch( @"[A-Z][a-z]" );
    private static Regex IsTheSameAs = ExactMatch( @"[a-z]=[a-z]" );
    private static Regex Are = ExactMatch( @"[a-z][A-Z][a-z]" );
    private static Regex TrueThat = ExactMatch( @"[A-Z]" );
    private static Regex Possibly = ExactMatch( @"<>" );
    private static Regex Necessarily = ExactMatch( @"\[\]" );
    private static Regex ParenthesizedExpression = ExactMatch( @"\(.*\)" );
    private static Regex TwoTermProposition = ExactMatch( @"(~?)([A-Z])([aeiouy])(~?)([A-Z])('?)" );
    private static Regex StringPrefixedWithTwoTermProposition = new Regex( @"^~?[A-Z][aeiouy]~?[A-Z]'?" );
    private static Regex StringPrefixedWithThisMany = new Regex( @"^\d+[A-Z]+" );

    private static bool AreUnariesFollowedByAQuantifier( IEnumerable<string> aExpressions )
    {
      if ( !IsUnaryOperator( aExpressions.First() ) )
        return false;

      IEnumerable<string> lExpressionsFollowingUnaryOperators = aExpressions.SkipWhile( fToken => IsUnaryOperator( fToken ) );

      if ( lExpressionsFollowingUnaryOperators.Count() == 0 )
        return false;

      return lExpressionsFollowingUnaryOperators.First().IsQuantifier();
    }

    private static string Conjoin( IEnumerable<string> aExpressions )
    {
      return string.Join( "&", aExpressions.Select( fExpression => Parenthesize( fExpression ) ).ToArray() );
    }

    private static Regex ExactMatch( string aPattern )
    {
      return new Regex( string.Format( "^{0}$", aPattern ) );
    }

    private static string[] GetSubexpressions( this string aString )
    {
      List<string> lSubexpressions = new List<string>();
      int lStartingPoint = 0;

      for ( int i = 1; i < aString.Length; i++ )
      {
        RegexMatch lMatchStringPrefixedWithTwoTermProposition =
          StringPrefixedWithTwoTermProposition.Exec( aString.Substring( lStartingPoint ) );

        RegexMatch lMatchThisMany =
          StringPrefixedWithThisMany.Exec( aString.Substring( lStartingPoint ) );

        if ( lMatchStringPrefixedWithTwoTermProposition != null )
        {
          lSubexpressions.Add( lMatchStringPrefixedWithTwoTermProposition[ 0 ] );
          lStartingPoint += lMatchStringPrefixedWithTwoTermProposition[ 0 ].Length;
          i = lStartingPoint;
        }
        else if ( lMatchThisMany != null )
        {
          lSubexpressions.Add( lMatchThisMany[ 0 ] );
          lStartingPoint += lMatchThisMany[ 0 ].Length;
          i = lStartingPoint;
        }
        // Always interpret a capital letter followed by a lowercase letter as a predication over one variable, unless it is part of
        // a two-term proposition.
        else if ( aString.Substring( lStartingPoint, 2 ).Matches( Is ) )
        {
          lSubexpressions.Add( aString.Substring( lStartingPoint, 2 ) );
          lStartingPoint += 2;
          i = lStartingPoint;
        }
        else if ( aString.Substring( lStartingPoint, i - lStartingPoint ).IsToken() )
        {
          lSubexpressions.Add( aString.Substring( lStartingPoint, i - lStartingPoint ) );
          lStartingPoint = i;
        }
      }

      if ( lStartingPoint < aString.Length )
        lSubexpressions.Add( aString.Substring( lStartingPoint, aString.Length - lStartingPoint ) );

      return lSubexpressions.ToArray();
    }

    private static Variable FindVariable(
      char aSymbol,
      VariableDictionary aBoundVariables,
      CollectedItems aCollectedItems )
    {
      if ( aBoundVariables.ContainsVariableForSymbol( aSymbol ) )
        return aBoundVariables.Retrieve( aSymbol );
      else
        return aCollectedItems.AddUnboundVariable( aSymbol );
    }

    private static Matrix FormTwoTermProposition( Term aSubject, Term aPredicate, char aForm, bool lModernInterpretation )
    {
      switch ( aForm )
      {
        case 'a':
          return Factory.FormA( aSubject, aPredicate, !lModernInterpretation );
        case 'e':
          return Factory.FormE( aSubject, aPredicate );
        case 'i':
          return Factory.FormI( aSubject, aPredicate );
        case 'o':
          return Factory.FormO( aSubject, aPredicate, lModernInterpretation );
        case 'u':
          return Factory.FormU( aSubject, aPredicate );
        case 'y':
          return Factory.FormY( aSubject, aPredicate );
        default:
          throw new System.Exception( string.Format( "Unhandled form of two-term proposition: {0}", aForm.ToString() ) );
      }
    }

    private static char GetSymbolForVariable( string aQuantifier )
    {
      if ( aQuantifier.Matches( ForAll ) )
        return aQuantifier[ 0 ];
      else if ( aQuantifier.Matches( ThereExists ) || aQuantifier.Matches( The ) )
        return aQuantifier[ 1 ];
      else
        // The program should never reach this point.
        throw new Exception();
    }

    private static bool HasMatchingParentheses( this string aString )
    {
      int lCount = 0;
      bool lHasParentheses = false;
      for ( int i = 0; i < aString.Length; i++ )
      {
        char lChar = aString[ i ];

        switch ( lChar )
        {
          case '(':
            lCount++;
            lHasParentheses = true;
            break;
          case ')':
            lCount--;
            break;
        }

        if ( lCount < 0 )
          return false;

      }
      return lHasParentheses && lCount == 0;
    }

    private static bool IsAnyOf( this string aString, params Regex[] aRegularExpressions )
    {
      return aRegularExpressions.Any( fRegex => aString.Matches( fRegex ) );
    }

    private static int PrecedenceOfBinaryOperator( this string aString )
    {
      if ( aString.IsAnyOf( And, Nor ) )
        return 0;
      else if ( aString.IsAnyOf( Or ) )
        return 1;
      else if ( aString.IsAnyOf( OnlyIf, NecessarilyOnlyIf ) )
        return 2;
      else if ( aString.IsAnyOf( IfAndOnlyIf, Xor ) )
        return 3;
      else
        throw new Exception();
    }

    private static bool IsBinaryOperator( this string aString )
    {
      return aString.IsAnyOf( And, Or, IfAndOnlyIf, OnlyIf, Nor, Xor, NecessarilyOnlyIf );
    }

    private static bool IsQuantifier( this string aString )
    {
      return aString.IsAnyOf( ForAll, ThereExists, The );
    }

    private static bool IsToken( this string aString )
    {
      return aString.IsParenthesizedGroup() || aString.IsAnyOf(
        And,
        Or,
        IfAndOnlyIf,
        ThereExists,
        ThisManyOfTheseAreTrue,
        ThereAreThisManyOfThese,
        OnlyIf,
        Not,
        Nor,
        Xor,
        Is,
        IsTheSameAs,
        Are,
        TrueThat,
        ForAll,
        Possibly,
        Necessarily,
        NecessarilyOnlyIf,
        TwoTermProposition,
        The );
    }

    private static bool IsParenthesizedGroup( this string aString )
    {
      return aString.StartsWith( "(" ) && aString.HasMatchingParentheses();
    }

    private static bool IsUnaryOperator( string aExpression )
    {
      return aExpression.IsAnyOf( Not, Possibly, Necessarily );
    }

    private static bool Matches( this string aString, Regex aRegularExpression )
    {
      return aRegularExpression.Exec( aString ) != null;
    }

    private static string Parenthesize( string aExpression )
    {
      return string.Format( "({0})", aExpression );
    }

    private static Matrix Parse( string aString )
    {
      return Parse( aString.GetSubexpressions(), new CollectedItems(), new VariableDictionary() );
    }

    private static Matrix Parse(
      string[] aExpressions,
      CollectedItems aCollectedItems,
      VariableDictionary aBoundVariables )
    {
      string lFirst = aExpressions[ 0 ];

      if ( aExpressions.Length == 1 )
        return ParsePropositionalToken( lFirst, aCollectedItems, aBoundVariables );

      string[] lTail = aExpressions.Tail<string>();

      if ( AreUnariesFollowedByAQuantifier( aExpressions ) )
        return ParseUnaryOperatorOnTail( lFirst, lTail, aCollectedItems, aBoundVariables );

      if ( lFirst.IsQuantifier() )
        return ParseQuantification( lFirst, lTail, aCollectedItems, aBoundVariables );

      if ( aExpressions.TakeWhile( fExpression => !fExpression.IsQuantifier() ).Any( fToken => fToken.IsBinaryOperator() ) )
        return ParseBinaryOperator( aExpressions, aCollectedItems, aBoundVariables );

      if ( IsUnaryOperator( lFirst ) )
        return ParseUnaryOperatorOnTail( lFirst, lTail, aCollectedItems, aBoundVariables );

      throw new ParseError( "Could not parse \"{0}\"", string.Join( "", aExpressions ) );
    }

    private static Matrix ParseBinaryOperator(
      string[] aTokens,
      CollectedItems aCollectedItems,
      VariableDictionary aBoundVariables )
    {
      int lNumberOfTokensBeforeBinaryOperator = -1;
      int lHighestPrecedenceFound = -1;
      for ( int i = 0; i < aTokens.Length; i++ )
      {
        if ( aTokens[ i ].IsBinaryOperator() )
        {
          int lPrecedence = aTokens[ i ].PrecedenceOfBinaryOperator();
          if ( lPrecedence >= lHighestPrecedenceFound )
          {
            lHighestPrecedenceFound = lPrecedence;
            lNumberOfTokensBeforeBinaryOperator = i;
          }
        }

        if ( aTokens[ i ].IsQuantifier() )
          break;
      }

      string[] lTokensBeforeLastBinaryOperator = aTokens.Subarray( 0, lNumberOfTokensBeforeBinaryOperator );
      string[] lTokensAfterLastBinaryOperator = aTokens.Subarray(
        lNumberOfTokensBeforeBinaryOperator + 1,
        aTokens.Length - lNumberOfTokensBeforeBinaryOperator - 1 );
      string lLastBinaryOperator = aTokens[ lNumberOfTokensBeforeBinaryOperator ];

      if ( lTokensBeforeLastBinaryOperator.Length == 0 || lTokensAfterLastBinaryOperator.Length == 0 )
        throw new ParseError( "Could not parse \"{0}\"", string.Join( "", aTokens ) );

      Matrix lLeft = Parse( lTokensBeforeLastBinaryOperator, aCollectedItems, aBoundVariables );
      Matrix lRight = Parse( lTokensAfterLastBinaryOperator, aCollectedItems, aBoundVariables );

      if ( lLastBinaryOperator.Matches( And ) )
        return Factory.And( lLeft, lRight );

      if ( lLastBinaryOperator.Matches( Or ) )
        return Factory.Or( lLeft, lRight );

      if ( lLastBinaryOperator.Matches( OnlyIf ) )
        return Factory.OnlyIf( lLeft, lRight );

      if ( lLastBinaryOperator.Matches( IfAndOnlyIf ) )
        return Factory.IfAndOnlyIf( lLeft, lRight );

      if ( lLastBinaryOperator.Matches( Nor ) )
        return Factory.Nor( lLeft, lRight );

      if ( lLastBinaryOperator.Matches( Xor ) )
        return Factory.Xor( lLeft, lRight );

      if ( lLastBinaryOperator.Matches( NecessarilyOnlyIf ) )
        return Factory.NecessarilyOnlyIf( lLeft, lRight );

      // The program should never reach this point.
      throw new Exception();
    }

    private static Matrix ParseQuantification(
      string aQuantifier,
      string[] aBody,
      CollectedItems aCollectedItems,
      VariableDictionary aBoundVariables )
    {
      char lSymbolForVariable = GetSymbolForVariable( aQuantifier );
      Variable lVariable = Factory.Variable( lSymbolForVariable );
      Matrix lBody = Parse(
        aBody,
        aCollectedItems,
        aBoundVariables.CreateNewSetThatRebinds( lSymbolForVariable, lVariable ) );

      if ( aQuantifier.Matches( ForAll ) )
        return Factory.ForAll( lVariable, lBody );

      if ( aQuantifier.Matches( ThereExists ) )
        return Factory.ThereExists( lVariable, lBody );

      if ( aQuantifier.Matches( The ) )
        return Factory.The( lVariable, lBody );

      // The program should never reach this point.
      throw new Exception();
    }

    private static Matrix ParsePropositionalToken(
      string aExpression,
      CollectedItems aCollectedItems,
      VariableDictionary aBoundVariables )
    {
      if ( aExpression.Matches( TwoTermProposition ) )
        return ParseTwoTermProposition( aExpression, aCollectedItems );

      if ( aExpression.Matches( Is ) )
      {
        return aCollectedItems.AddUnaryPredication(
          aCollectedItems.AddUnaryPredicate( aExpression[ 0 ] ),
          FindVariable( aExpression[ 1 ], aBoundVariables, aCollectedItems ) );
      }

      if ( aExpression.Matches( Are ) )
      {
        return aCollectedItems.AddBinaryPredication(
          aCollectedItems.AddBinaryPredicate( aExpression[ 1 ] ),
          FindVariable( aExpression[ 0 ], aBoundVariables, aCollectedItems ),
          FindVariable( aExpression[ 2 ], aBoundVariables, aCollectedItems ) );
      }

      if ( aExpression.Matches( TrueThat ) )
        return aCollectedItems.AddNullPredicate( aExpression[ 0 ] );

      if ( aExpression.Matches( IsTheSameAs ) )
      {
        return aCollectedItems.AddIdentification(
          FindVariable( aExpression[ 0 ], aBoundVariables, aCollectedItems ),
          FindVariable( aExpression[ 2 ], aBoundVariables, aCollectedItems ) );
      }

      if ( aExpression.Matches( ThereAreThisManyOfThese ) )
      {
        return Factory.ThereAreThisManyOfThese(
          UInt32.Parse( aExpression.Substring( 0, aExpression.Length - 1 ) ),
          aCollectedItems.AddUnaryPredicate( aExpression[ aExpression.Length - 1 ] ) );
      }

      if ( aExpression.Matches( ThisManyOfTheseAreTrue ) )
      {
        List<Matrix> lPredicates = new List<Matrix>();
        RegexMatch lMatch = ThisManyOfTheseAreTrue.Exec( aExpression );
        for ( int i = 0; i < lMatch[ 2 ].Length; i++ )
        {
          lPredicates.Add( aCollectedItems.AddNullPredicate( lMatch[ 2 ][ i ] ) );
        }
        return Factory.ThisManyOfTheseAreTrue( UInt32.Parse( lMatch[ 1 ] ), lPredicates );
      }

      if ( aExpression.Matches( ParenthesizedExpression ) )
      {
        return Parse(
          aExpression.Substring( 1, aExpression.Length - 2 ).GetSubexpressions(),
          aCollectedItems,
          aBoundVariables );
      }

      throw new ParseError( "Expected predication or parenthesized expression, found \"{0}\"", aExpression );
    }

    private static Matrix ParseTwoTermProposition( string aExpression, CollectedItems aCollectedItems )
    {
      RegexMatch lMatch = TwoTermProposition.Exec( aExpression );

      Term lSubject = Factory.AllAndOnly(
        aCollectedItems.AddUnaryPredicate( lMatch[ 2 ][ 0 ] ),
        lMatch[ 1 ].Length > 0 );
      Term lPredicate = Factory.AllAndOnly(
        aCollectedItems.AddUnaryPredicate( lMatch[ 5 ][ 0 ] ),
        lMatch[ 4 ].Length > 0 );
      bool lModernInterpretation = lMatch[ 6 ].Length > 0;

      return FormTwoTermProposition( lSubject, lPredicate, lMatch[ 3 ][ 0 ], lModernInterpretation );
    }
    
    private static Matrix ParseUnaryOperatorOnTail(
      string aUnaryOperator,
      string[] aTail,
      CollectedItems aCollectedItems,
      VariableDictionary aBoundVariables )
    {
      Matrix lTail = Parse( aTail, aCollectedItems, aBoundVariables );
      
      if ( aUnaryOperator.Matches( Not ) )
        return Factory.Not( lTail );
        
      if ( aUnaryOperator.Matches( Possibly ) )
        return Factory.Possibly( lTail );
        
      if ( aUnaryOperator.Matches( Necessarily ) )
        return Factory.Necessarily( lTail );
      
      // The program should never reach this point.
      throw new Exception();
    }
  }
}
