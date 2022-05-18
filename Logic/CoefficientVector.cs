// somerby.net/mack/logic
// Copyright (C) 2019 MacKenzie Cumings
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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
  public class CoefficientVector
  {
    private readonly int[] Coefficients;
    private readonly string[] Variables;

    public CoefficientVector( string aVariable, IEnumerable<string> aVariables )
    {
      Variables = aVariables.OrderBy( fVariable => fVariable ).ToArray();
      Coefficients = new int[1 << Variables.Length];
      for ( int i = 0; i < Variables.Length; i++ )
      {
        if ( Variables[i] == aVariable )
        {
          Coefficients[1 << i] = 1;
          break;
        }
      }
    }

    public CoefficientVector( params int[] aCoefficients )
    {
      Coefficients = aCoefficients;
      Variables = new string[0];
    }

    public CoefficientVector( int[] aCoefficients, IEnumerable<string> aVariables )
    {
      Coefficients = aCoefficients;
      Variables = aVariables.OrderBy( fVariable => fVariable ).ToArray();
    }

    public override string ToString()
    {
      return string.Format( "[{0}]", string.Join( ", ", Coefficients ) );
    }

    public string Formula
    {
      get
      {
        StringBuilder lFormula = new StringBuilder("");
        for ( int i = 0; i < Coefficients.Length; i++ )
        {
          if ( Coefficients[i] != 0 )
          {
            int lAbsoluteCoefficient = System.Math.Abs( Coefficients[i] );
            string[] lVariables = Variables.AtIndices( PowersOfTwo( i ) ).ToArray();

            if ( lFormula.Length > 0 )
            {
              if ( Coefficients[i] > 0 )
                lFormula.Append( " + " );
              else
                lFormula.Append( " - " );
            }
            else
            {
              if ( Coefficients[i] < 0 )
                lFormula.Append( "-" );
            }

            if ( lAbsoluteCoefficient > 1 || lVariables.Length == 0 )
                lFormula.Append( lAbsoluteCoefficient );
              
            foreach ( string lVariableName in lVariables )
            {
              lFormula.Append( lVariableName );
            }
          }
        }
        return lFormula.Length == 0
          ? "0"
          : lFormula.ToString();
      }
    }


    private static CoefficientVector Operator( params int[] aItems )
    {
      return new CoefficientVector( aItems );
    }

    private static int DotProduct( int[] aX, int [] aY )
    {
      if ( aX.Length != aY.Length )
        throw new EngineException( "invalid dot product" );
      return aX.Zip( aY, (f1,f2) => f1*f2 ).Sum();
    }


    private static int[] Expand( params int[] aPropositions )
    {
      int[] lVector = new int[ 1 << aPropositions.Length ];
      for ( int i = 0; i < lVector.Length; i++ )
      {
        lVector[i] = aPropositions.AtIndices( PowersOfTwo( i ) ).Product();
      }
      return lVector;
    }

    private static int[] PowersOfTwo( int aNumber )
    {
      List<int> lPowersOfTwo = new List<int>();

      int lPower = 0;

      while ( aNumber > 0 )
      {
        if ( ( aNumber & 1 ) == 1 )
          lPowersOfTwo.Add( lPower );

        aNumber >>= 1;
        lPower++;
      }

      return lPowersOfTwo.ToArray();
    }

    private static int[] Extend( int[] aVector )
    {
      int[] lExtension = new int[2*aVector.Length];
      for ( int i = 0; i < aVector.Length; i++ )
      {
        lExtension[i] = aVector[i];
      }
      return lExtension;
    }

    private static void ApplyComponent(
      ref int[] aAccumulatedSums,
      int aAccumulatedProduct,
      int aAccumulatedIndex,
      IEnumerable<int[]> aOperandsToBeApplied )
    {
      if ( aOperandsToBeApplied.Count() == 0 )
      {
        aAccumulatedSums[aAccumulatedIndex] += aAccumulatedProduct;
        if ( aAccumulatedIndex == 0 )
          System.Console.WriteLine( string.Format( "{0} added to product", aAccumulatedProduct ) );
      }
      else
      {
        int[] aFirstOperand = aOperandsToBeApplied.First();
        int[][] aOtherOperands = aOperandsToBeApplied.Skip(1).ToArray();
        for ( int i = 0; i < aFirstOperand.Length; i++ )
        {
          ApplyComponent(
            ref aAccumulatedSums,
            aAccumulatedProduct * aFirstOperand[i],
            aAccumulatedIndex | i,
            aOtherOperands );
        }
      }
    }

    internal static CoefficientVector Apply( CoefficientVector aOperator, params CoefficientVector[] aOperands )
    {
      System.Console.WriteLine( string.Format( "Applying {0}", aOperator ) );
      ValidateOperands( aOperands );

      int[] lResult = new int[ aOperands[ 0 ].Coefficients.Length ];
      int[][] lOperands = aOperands.Select( fVector => fVector.Coefficients ).ToArray();
      for ( int i = 0; i < aOperator.Coefficients.Length; i++ )
      {
        ApplyComponent(
          ref lResult,
          aOperator.Coefficients[ i ],
          0,
          lOperands.AtIndices( PowersOfTwo( i ) ) );
      }

      return new CoefficientVector( lResult, aOperands[ 0 ].Variables );
    }

    private static void ValidateOperands( CoefficientVector[] aOperands )
    {
      if ( aOperands.Length < 1 )
        throw new EngineException( "Too few operands." );

      foreach ( CoefficientVector lVector in aOperands.Skip( 1 ) )
      {
        if ( lVector.Coefficients.Length != aOperands[ 0 ].Coefficients.Length )
          throw new EngineException( "Operands' lengths do not match." );
        // TODO: compare variable names in operands
      }
    }

    internal static int TruthValue( int[] aTruthFunction, params int[] aPropositions )
    {
      return DotProduct( aTruthFunction, Expand( aPropositions ) );
    }

    internal static CoefficientVector AND { get; private set; }
    internal static CoefficientVector OR { get; private set; }
    internal static CoefficientVector NOT { get; private set; }
    internal static CoefficientVector NOTP { get; private set; }
    internal static CoefficientVector NOTQ { get; private set; }
    internal static CoefficientVector IMPL { get; private set; }
    internal static CoefficientVector NOR { get; private set; }
    internal static CoefficientVector EQUIV { get; private set; }
    static CoefficientVector()
    {
      CoefficientVector P = Operator( 0, 1, 0, 0 );
      CoefficientVector Q = Operator( 0, 0, 1, 0 );
      AND = Operator( 0, 0, 0, 1 );
      NOT = Operator( 1, -1 );
      CoefficientVector NOTP = Apply(NOT,P);
      CoefficientVector NOTQ = Apply(NOT,Q);
      NOR = Apply( AND, NOTP, NOTQ );
      OR = Apply( NOT, NOR );
      IMPL = Apply( OR, NOTP, Q );
      EQUIV = Apply( AND, IMPL, Apply( IMPL, Q, P ) );
    }
  }
}
