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
      get { return this.Decide() == Alethicity.Necessary; }
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
        ContainsModalities,
        MaxmimumNumberOfModalitiesInIdentifications );

      bool lNotImpossible = false;
      bool lNotNecessary = false;
      uint lLastInterpretation = lPredicates.LastInterpretation;
      uint lLastKindOfWorld = lPredicates.LastKindOfWorld;

#if PARALLELIZE
      //System.Windows.Forms.MessageBox.Show( string.Format( "Maximum number of distinguishable objects: {0}", MaxmimumNumberOfDistinguishableObjects ) );
      
      System.Threading.Tasks.ParallelOptions lParallelOptions = new System.Threading.Tasks.ParallelOptions();
      System.Threading.CancellationTokenSource lCancellationTokenSource = new System.Threading.CancellationTokenSource();
      lParallelOptions.CancellationToken = lCancellationTokenSource.Token;
      lParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;

      try
      {
#endif
        if ( ContainsModalities )
        {
#if PARALLELIZE
          System.Threading.Tasks.Parallel.For(
            Convert.ToInt64( lPredicates.FirstInterpretation ),
            Convert.ToInt64( lLastInterpretation ) + 1,
            lParallelOptions,
            ( fInterpretation ) =>
          {
            uint lInterpretation = Convert.ToUInt32( fInterpretation );

#else
          for ( uint lInterpretation = lPredicates.FirstInterpretation; lInterpretation <= lLastInterpretation; lInterpretation++ )
          {
#endif
            for ( uint lKindOfWorld = lPredicates.FirstNonemptyWorld; lKindOfWorld <= lLastKindOfWorld; lKindOfWorld++ )
            {
              if ( ( ( 1U << (int) lKindOfWorld ) & lInterpretation ) == 0 )
                continue;

              if ( this.TrueIn( lInterpretation, lKindOfWorld, lPredicates ) )
                lNotImpossible = true;
              else
                lNotNecessary = true;
              if ( lNotImpossible && lNotNecessary )
#if PARALLELIZE
                lCancellationTokenSource.Cancel();
            }
          } );
#else
                return Alethicity.Contingent;
            }
          }
#endif
        }
        else
        {
          uint lInterpretation = lPredicates.FirstInterpretation;
#if PARALLELIZE
          System.Threading.Tasks.Parallel.For(
            Convert.ToInt64( lPredicates.FirstNonemptyWorld ),
            Convert.ToInt64( lLastKindOfWorld ) + 1,
            lParallelOptions,
            ( fKindOfWorld ) =>
          {
            uint lKindOfWorld = Convert.ToUInt32( fKindOfWorld );
#else
          for ( uint lKindOfWorld = lPredicates.FirstNonemptyWorld; lKindOfWorld <= lLastKindOfWorld; lKindOfWorld++ )
          {
#endif         
            if ( this.TrueIn( lInterpretation, lKindOfWorld, lPredicates ) )
              lNotImpossible = true;
            else
              lNotNecessary = true;
            if ( lNotImpossible && lNotNecessary )
#if PARALLELIZE
              lCancellationTokenSource.Cancel();
          } );
        }
      }
      catch ( OperationCanceledException )
      {
      }

      if ( lNotImpossible && lNotNecessary )
        return Alethicity.Contingent;
#else
              return Alethicity.Contingent;
          }
        }
#endif

      return lNotNecessary ? Alethicity.Impossible :  Alethicity.Necessary;
    }
		
		internal abstract IEnumerable<UnaryPredicate> UnaryPredicates();
    internal abstract IEnumerable<NullPredicate> NullPredicates();

    internal abstract IEnumerable<Variable> FreeVariables
    {
      get;
    }

    internal abstract IEnumerable<Necessity> FreeModalities
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

    internal abstract int MaxmimumNumberOfModalitiesInIdentifications
    {
      get;
    }

    internal virtual bool ContainsModalities
    {
      get { return false; }
    }
		
		internal abstract bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicateDictionary );

    public virtual bool Propositional
    {
      get { return false; }
    }

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

    internal virtual void AssignModality( Necessity aNecessity ) {}

    internal virtual IEnumerable<Necessity> ModalitiesInIdentifications
    {
      get { yield break; }
    }
	}
}
