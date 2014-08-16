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
    /// conjoin all statements after it and make them the right side of the operation.  Bind all free
    /// variables in existential quantifiers.
    /// </summary>
    /// <param name="aFileContents"></param>
    /// <returns></returns>
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

    private static Matrix Parse( string aString )
    {
      PredicateDictionary lPredicateDictionary = new PredicateDictionary();
      VariableDictionary lVariableDictionary = new VariableDictionary();
      Matrix lParsedMatrix;
      List<char> lFreeVariableSymbols = new List<char>();

      while ( true )
      {
        try
        {
          lParsedMatrix = Parse( aString.GetSubexpressions(), lPredicateDictionary, lVariableDictionary );
          break;
        }
        catch ( FreeVariableNotFoundException lException )
        {
          lFreeVariableSymbols.Add( lException.VariableSymbol );
          lVariableDictionary = lVariableDictionary.UpdateWith( lException.VariableSymbol, Factory.Variable( lException.VariableSymbol ) );
        }
      }

      foreach ( char lSymbol in lFreeVariableSymbols )
      {
        lParsedMatrix = Factory.ThereExists( lVariableDictionary.Retrieve( lSymbol ), lParsedMatrix );
        //lParsedMatrix = Factory.ForAll( lVariableDictionary.Retrieve( lSymbol ), lParsedMatrix );
      }

      return lParsedMatrix;
    }
   

    private static Regex ExactMatch( string aPattern )
    {
      return new Regex( string.Format( "^{0}$", aPattern ) );
    }
    private static Regex ThereExists = ExactMatch( @"3[a-z]," );
    private static Regex ForAll = ExactMatch( @"[a-z]," );
    private static Regex And = ExactMatch( @"&" );
    private static Regex Or = ExactMatch( @"\|" );
    private static Regex OnlyIf = ExactMatch( @"->" );
    private static Regex IfAndOnlyIf = ExactMatch( @"<=>" );
    private static Regex Not = ExactMatch( @"~" );
    private static Regex Nor = ExactMatch( @"!" );
    private static Regex Xor = ExactMatch( @"\^" );
    private static Regex Is = ExactMatch( @"[A-Z][a-z]" );
    private static Regex Same = ExactMatch( @"[a-z]=[a-z]" );
    private static Regex TrueThat = ExactMatch( @"[A-Z]" );
    private static Regex Possibly = ExactMatch( @"<>" );
    private static Regex Necessarily = ExactMatch( @"\[\]" );
    private static Regex ParenthesizedExpression = ExactMatch( @"\(.*\)" );

    private static char GetSymbolForVariable( string aQuantifier )
    {
      if ( aQuantifier.Matches( ThereExists ) )
        return aQuantifier[ 1 ];
      else if ( aQuantifier.Matches( ForAll ) )
        return aQuantifier[ 0 ];
      else
        throw new Exception();
    }

    private static T[] Array<T>( params T[] aArray )
    {
      return aArray;
    }

    private static bool IsAnyOf( this string aString, params Regex[] aRegularExpressions )
    {
      return aRegularExpressions.Any( fRegex => fRegex.IsMatch( aString ) );
    }

    private static bool IsParenthesizedGroup( this string aString )
    {
      return aString.StartsWith( "(" ) && aString.HasMatchingParentheses();
    }

    private static bool IsToken( this string aString )
    {
      return aString.IsParenthesizedGroup() || aString.IsAnyOf(
        And,
        Or,
        IfAndOnlyIf,
        ThereExists,
        OnlyIf,
        Not,
        Nor,
        Xor,
        Is,
        Same,
        TrueThat,
        ForAll,
        Possibly,
        Necessarily );
    }

    private static string[] GetSubexpressions( this string aString )
    {
      List<string> lSubexpressions = new List<string>();
      int lStartingPoint = 0;

      for ( int i = 1; i < aString.Length; i++ )
      {
        // Always interpret a capital letter followed by a lowercase latter as a predication over one variable.
        if ( aString.Substring( lStartingPoint, 2 ).Matches( Is ) )
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

    private static bool IsBinaryOperator( this string aString )
    {
      return aString.IsAnyOf( And, Or, IfAndOnlyIf, OnlyIf, Nor, Xor );
    }

    private static T[] UpToLast<T>( this T[] aArray, Predicate<T> aPredicate )
    {
      int lIndexOfLast;
      for ( lIndexOfLast = aArray.Length - 1; lIndexOfLast >= 0; lIndexOfLast-- )
      {
        if ( aPredicate( aArray[ lIndexOfLast ] ) )
          break;
      }

      return aArray.Subarray( 0, lIndexOfLast );
    }

    private static T[] AfterLast<T>( this T[] aArray, Predicate<T> aPredicate )
    {
      int lIndexOfLast;
      for ( lIndexOfLast = aArray.Length - 1; lIndexOfLast >= 0; lIndexOfLast-- )
      {
        if ( aPredicate( aArray[ lIndexOfLast ] ) )
          break;
      }

      return aArray.Subarray( lIndexOfLast + 1, aArray.Length - lIndexOfLast - 1 );
    } 

    private static string Conjoin( IEnumerable<string> aExpressions )
    {
      return string.Join( "&", aExpressions.Select( fExpression => Parenthesize( fExpression ) ).ToArray() );
    }

    private static string Parenthesize( string aExpression )
    {
      return string.Format( "({0})", aExpression );
    }

    private static Matrix ParsePredicationIdentificationOrGrouping(
      string aExpression,
      PredicateDictionary aCollectedPredicates,
      VariableDictionary aFreeVariables )
    {
      if ( aExpression.Matches( Is ) )
      {
        return aCollectedPredicates.AddUnaryPredication(
          aCollectedPredicates.AddUnaryPredicate( aExpression[ 0 ] ),
          aFreeVariables.Retrieve( aExpression[ 1 ] ) );
      }

      if ( aExpression.Matches( TrueThat ) )
        return aCollectedPredicates.AddNullPredicate( aExpression[ 0 ] );

      if ( aExpression.Matches( Same ) )
      {
        return aCollectedPredicates.AddIdentification(
          aFreeVariables.Retrieve( aExpression[ 0 ] ),
          aFreeVariables.Retrieve( aExpression[ 2 ] ) );
      }

      if ( aExpression.Matches( ParenthesizedExpression ) )
      {
        return Parse(
          aExpression.Substring( 1, aExpression.Length - 2 ).GetSubexpressions(),
          aCollectedPredicates,
          aFreeVariables );
      }

      throw new ParseError( "Expected predication or parenthesized expression, found \"{0}\"", aExpression );
    }

    private static Matrix ParseQuantification(
      string aQuantifier,
      string[] aBody,
      PredicateDictionary aCollectedPredicates,
      VariableDictionary aFreeVariables )
    {
      char lSymbolForVariable = GetSymbolForVariable( aQuantifier );
      Variable lVariable = Factory.Variable( lSymbolForVariable );
      Matrix lBody = Parse(
        aBody,
        aCollectedPredicates,
        aFreeVariables.UpdateWith( lSymbolForVariable, lVariable ) );

      if ( aQuantifier.Matches( ForAll ) )
        return Factory.ForAll( lVariable, lBody );

      if ( aQuantifier.Matches( ThereExists ) )
        return Factory.ThereExists( lVariable, lBody );

      throw new Exception();
    }

    private static Matrix ParseBinaryOperator(
      string[] aTokens,
      PredicateDictionary aCollectedPredicates,
      VariableDictionary aFreeVariables )
    {
      string[] lTokensBeforeLastBinaryOperator = aTokens.UpToLast( fToken => fToken.IsBinaryOperator() );
      string[] lTokensAfterLastBinaryOperator = aTokens.AfterLast( fToken => fToken.IsBinaryOperator() );
      string lLastBinaryOperator = aTokens.Last( fToken => fToken.IsBinaryOperator() );

      if ( lTokensBeforeLastBinaryOperator.Length == 0 || lTokensAfterLastBinaryOperator.Length == 0 )
        throw new ParseError( "Could not parse \"{0}\"", string.Join( "", aTokens ) );

      Matrix lLeft = Parse( lTokensBeforeLastBinaryOperator, aCollectedPredicates, aFreeVariables );
      Matrix lRight = Parse( lTokensAfterLastBinaryOperator, aCollectedPredicates, aFreeVariables );

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

      throw new Exception();
    }

    private static Matrix Parse(
      string[] aExpressions,
      PredicateDictionary aCollectedPredicates,
      VariableDictionary aFreeVariables )
    {
      string lFirst = aExpressions[ 0 ];

      if ( aExpressions.Length == 1 )
        return ParsePredicationIdentificationOrGrouping( lFirst, aCollectedPredicates, aFreeVariables );

      string[] lTail = aExpressions.Tail<string>();

      if ( AreUnariesFollowedByAQuantifier( aExpressions ) )
        return ParseUnaryOperatorOnTail( lFirst, lTail, aCollectedPredicates, aFreeVariables );

      if ( lFirst.IsAnyOf( ForAll, ThereExists ) )
        return ParseQuantification( lFirst, lTail, aCollectedPredicates, aFreeVariables );

      if ( aExpressions.TakeWhile( fExpression => !fExpression.IsAnyOf( ForAll, ThereExists ) ).Any( fToken => fToken.IsBinaryOperator() ) )
        return ParseBinaryOperator( aExpressions, aCollectedPredicates, aFreeVariables );

      if ( IsUnaryOperator( lFirst ) )
        return ParseUnaryOperatorOnTail( lFirst, lTail, aCollectedPredicates, aFreeVariables );

      throw new ParseError( "Could not parse \"{0}\"", string.Join( "", aExpressions ) );
    }
    
    private static bool IsUnaryOperator( string aExpression )
    {
      return Not.IsMatch( aExpression ) || Possibly.IsMatch( aExpression ) || Necessarily.IsMatch( aExpression );
    }
    
    private static bool AreUnariesFollowedByAQuantifier( IEnumerable<string> aExpressions )
    {
      if ( IsUnaryOperator( aExpressions.First() ) )
        return aExpressions.SkipWhile( fToken => IsUnaryOperator( fToken ) ).First().IsAnyOf( ForAll, ThereExists );
      else
        return false;
    }
    
    private static Matrix ParseUnaryOperatorOnTail(
      string aUnaryOperator,
      string[] aTail,
      PredicateDictionary aCollectedPredicates,
      VariableDictionary aFreeVariables )
    {
      Matrix lTail = Parse( aTail, aCollectedPredicates, aFreeVariables );
      
      if ( aUnaryOperator.Matches( Not ) )
        return Factory.Not( lTail );
        
      if ( aUnaryOperator.Matches( Possibly ) )
        return Factory.Possibly( lTail );
        
      if ( aUnaryOperator.Matches( Necessarily ) )
        return Factory.Necessarily( lTail );
        
      throw new Exception();
    }

    private static bool HasMatchingParentheses( this string aString )
    {
      int lCount = 0;
      bool lHasParentheses = false;
      for ( int i = 0; i < aString.Length; i++ )
      {
        char lChar = aString[i];

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
    
    private static bool Matches( this string aString, Regex aRegularExpression )
    {
      return aRegularExpression.IsMatch( aString );
    }
  }
}
