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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
  public enum Alethicity { Impossible, Contingent, Necessary };
  /// <summary>
  /// A logical expression which may or may not contain free variables.  The term "matrix" is borrowed from Quine's book,
  /// <i>Mathematical Logic</i>, though he defines it as a string of symbols that signifies some proposition and inside
  /// of this program a matrix is a data structure that signifies the same proposition.  
  /// </summary>
	public abstract class Matrix : NamedObject
	{
		internal Matrix()
		{
		}

    public bool Valid
    {
      get
      {
        if ( this.FreeVariables.Any() )
          throw new EngineException( "This proposition can't be decided; it contains free variables." );

        Predicates lPredicates = new Predicates(
          NullPredicates(),
          UnaryPredicates(),
          MaxmimumNumberOfDistinguishableObjects,
          this.ContainsModalities );

        foreach ( uint lInterpretation in lPredicates.Interpretations )
        {
          foreach ( uint lKindOfWorld in lPredicates.KindsOfWorlds( lInterpretation ) )
          {
            if ( !this.TrueIn( lInterpretation, lKindOfWorld, lPredicates ) )
              return false;
          }
        }

        return true;
      }
    }

    /// <summary>
    /// Find the set of kinds of worlds that this matrix, with free variables instantiated, excludes from
    /// possibility.
    /// </summary>
    /// <param name="aPredicates">the predicates present in the proposition this matrix belongs to.</param>
    /// <returns>the kinds of worlds in which this matrix is false</returns>
    private uint Exclusions( Predicates aPredicates )
    {
      uint lExclusions = 0;
      uint lLastKindOfWorld = aPredicates.LastKindOfWorld;
      uint lMostInclusiveInterpretation = aPredicates.LastInterpretation;

      for ( uint lKindOfWorld = aPredicates.FirstNonemptyWorld; lKindOfWorld <= lLastKindOfWorld; lKindOfWorld++ )
      {
        if ( !this.TrueIn( lMostInclusiveInterpretation, lKindOfWorld, aPredicates ) )
          lExclusions |= 1U << (int) lKindOfWorld;
      }

      return lExclusions;
    }

    public Alethicity Decide()
    {
      if ( this.FreeVariables.Any() )
        throw new EngineException( "This proposition can't be decided; it contains free variables." );

      Predicates lPredicates = new Predicates(
        NullPredicates(),
        UnaryPredicates(),
        MaxmimumNumberOfDistinguishableObjects,
        ContainsModalities );

      bool lNotImpossible = false;
      bool lNotNecessary = false;

      uint lLastInterpretation = lPredicates.LastInterpretation;
      uint lLastKindOfWorld = lPredicates.LastKindOfWorld;
      if ( ContainsModalities )
      {      
        for ( uint lInterpretation = lPredicates.FirstInterpretation; lInterpretation <= lLastInterpretation; lInterpretation++ )
        {
          //foreach ( uint lKindOfWorld in lPredicates.KindsOfWorlds( lInterpretation ) )

          for ( uint lKindOfWorld = lPredicates.FirstNonemptyWorld; lKindOfWorld <= lLastKindOfWorld; lKindOfWorld++ )
          {
            if ( ( ( 1U << (int) lKindOfWorld ) & lInterpretation ) == 0 )
              continue;

            if ( this.TrueIn( lInterpretation, lKindOfWorld, lPredicates ) )
              lNotImpossible = true;
            else
              lNotNecessary = true;
            if ( lNotImpossible && lNotNecessary )
              return Alethicity.Contingent;
          }
        }
      }
      else
      {
        for ( uint lInterpretation = lPredicates.FirstInterpretation; lInterpretation <= lLastInterpretation; lInterpretation++ )
        {
          //foreach ( uint lKindOfWorld in lPredicates.KindsOfWorlds( lInterpretation ) )
          for ( uint lKindOfWorld = lPredicates.FirstNonemptyWorld; lKindOfWorld <= lLastKindOfWorld; lKindOfWorld++ )
          {
            if ( this.TrueIn( lInterpretation, lKindOfWorld, lPredicates ) )
              lNotImpossible = true;
            else
              lNotNecessary = true;
            if ( lNotImpossible && lNotNecessary )
              return Alethicity.Contingent;
          }
        }
      }

      if ( !lNotNecessary )
        return Alethicity.Necessary;
      else
        return Alethicity.Impossible;
    }
		
		internal abstract IEnumerable<UnaryPredicate> UnaryPredicates();
    internal abstract IEnumerable<NullPredicate> NullPredicates();

    internal abstract IEnumerable<Variable> FreeVariables
    {
      get;
    }

    internal abstract IEnumerable<Variable> IdentifiedVariables
    {
      get;
    }

    internal virtual IEnumerable<Matrix> NonNullPredications
    {
      get { yield break; }
    }

    internal virtual IEnumerable<Tuple<UniversalGeneralization, Matrix>> ClosedPredications
    {
      get { yield break; }
    }

    internal virtual IEnumerable<Tuple<Matrix, Matrix>> DirectDependencies
    {
      get { yield break; }
    }

    internal virtual IEnumerable<Matrix> Matrices
    {
      get { yield return this; }
    }

    internal abstract int MaxmimumNumberOfDistinguishableObjects
    {
      get;
    }

    internal virtual bool ContainsModalities
    {
      get { return false; }
    }
		
		internal abstract bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicateDictionary );

    public virtual string GraphvizDOT
    {
      get
      {
        StringBuilder lDOT = new StringBuilder();
        lDOT.AddLine( "digraph Proposition_{0} {{", this.Name );
        lDOT.AddLine( "ordering=out;bgcolor=\"#FFFFFF80\"" );
        foreach ( Matrix lMatrix in this.Matrices.Distinct() )
        {
          lDOT.AddLine(
            "{0} [label={1},shape=rectangle,fontsize=10,style=filled,fillcolor=\"white\",margin=\"0.11,0.00\"];",
            lMatrix.Name,
            lMatrix.DOTLabel );
        }
        foreach ( Tuple<Matrix, Matrix> lPair in DirectDependencies )
        {
          lDOT.AddLine(
            "{0}->{1}",
            lPair.Item1.Name,
            lPair.Item2.Name );
        }
        foreach ( Tuple<UniversalGeneralization, Matrix> lPair in ClosedPredications )
        {
          lDOT.AddLine(
            "{0}->{1} [style=dashed]",
            lPair.Item1.Name,
            lPair.Item2.Name );
        }

        lDOT.AddLine( "}}" );

        return lDOT.ToString();
        //return string.Format( "digraph Proposition{0}" );
        //return "digraph Proposition {1 [label=<Universal Generalization<BR/><B><FONT FACE=\"MONOSPACE\">x,</FONT></B>>];2 [label=<Predication<BR/><B><FONT FACE=\"MONOSPACE\">Px</FONT></B>>];1->2;1->2 [style=dashed]}";
      }
    }

    internal abstract string DOTLabel { get; }
	}
}
