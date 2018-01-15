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
#if SALTARELLE
using System.Runtime.CompilerServices;
#endif
using System.Text;

namespace Logic
{
  /// <summary>
  /// A logical structure which is either true or false and may or may not contain free variables.  The term "matrix" is borrowed
  /// from Quine's book, <i>Mathematical Logic</i>, though he defines it as a string of symbols that signifies some proposition.
  /// In this program, a matrix is a data structure.  
  /// </summary>
	public abstract class Matrix : NamedObject
	{
    public int Complexity
    {
      get { return (int) CollectPredicates().BitsNeeded; }
    }

    private Alethicity DecideForFreeVariables()
    {
      if ( Factory.Bind( FreeVariables, this, Factory.ForAll ).Decide() == Alethicity.Necessary )
        return Alethicity.Necessary;
      else if ( Factory.Bind( FreeVariables, this, Factory.ThereExists ).Decide() == Alethicity.Impossible )
        return Alethicity.Impossible;
      else
        return Alethicity.Contingent;
    }

    private Predicates CollectPredicates()
    {
      return new Predicates(
        NullPredicates,
        UnaryPredicates,
        MaxmimumNumberOfDistinguishableObjectsOfAKind,
        ContainsModalities,
        MaxmimumNumberOfModalitiesInIdentifications );
    }

    private bool HasCounterexample( uint aInterpretation, Predicates aPredicates )
    {
      foreach ( uint lKindOfWorld in aPredicates.KindsOfWorlds( aInterpretation ) )
      {
        if ( !this.TrueIn( aInterpretation, lKindOfWorld, aPredicates ) )
          return true;
      }

      return false;
    }

    public Counterexample FindExample()
    {
      if ( this.FreeVariables.Any() && ContainsModalities )
        throw new EngineException( "This proposition can't be evaluated; it contains both constants and modal operators." );

      Matrix lNegation = Factory.Not( this );

      if ( lNegation.FreeVariables.Any() )
        return Factory.Bind( FreeVariables, lNegation, Factory.ForAll ).FindCounterexample();
      
      Predicates lPredicates = lNegation.CollectPredicates();
      if ( lNegation.ContainsModalities )
      {
#if COMPLEX_COUNTEREXAMPLE
        uint lLastInterpretation = lPredicates.LastInterpretation;
        for ( uint lInterpretation = lPredicates.FirstInterpretation; lInterpretation <= lLastInterpretation; lInterpretation++ )
        
#else
        uint lFirstInterpretation = lPredicates.FirstInterpretation;
        for ( uint lInterpretation = lPredicates.LastInterpretation; lInterpretation >= lFirstInterpretation; lInterpretation-- )
#endif
        {
          if ( lNegation.HasCounterexample( lInterpretation, lPredicates ) )
          {
            List<KindOfWorld> lCounterexamples = new List<KindOfWorld>();
            List<KindOfWorld> lNonCounterexamples = new List<KindOfWorld>();
            foreach ( uint lKindOfWorld in lPredicates.KindsOfWorlds( lInterpretation ) )
            {
              if ( lNegation.TrueIn( lInterpretation, lKindOfWorld, lPredicates ) )
                lNonCounterexamples.Add( lPredicates.DecodeKindOfWorldNumber( lKindOfWorld ) );
              else
                lCounterexamples.Add( lPredicates.DecodeKindOfWorldNumber( lKindOfWorld ) );   
            }
            return new ModalCounterexample( lCounterexamples, lNonCounterexamples );
          }
        }
      }
      else
      {
        uint lInterpretation = lPredicates.FirstInterpretation;
#if COMPLEX_COUNTEREXAMPLE
        uint lLastKindOfWorld = lPredicates.LastKindOfWorld;
        for ( uint lKindOfWorld = lPredicates.FirstNonemptyWorld; lKindOfWorld <= lLastKindOfWorld; lKindOfWorld++ )
#else
        uint lFirstKindOfWorld = lPredicates.FirstNonemptyWorld;
        for ( uint lKindOfWorld = lPredicates.LastKindOfWorld; lKindOfWorld >= lFirstKindOfWorld; lKindOfWorld-- )
#endif   
        {
          if ( !lNegation.TrueIn( lInterpretation, lKindOfWorld, lPredicates ) )
            return lPredicates.DecodeKindOfWorldNumber( lKindOfWorld );
        }
      }

      // No counterexample exists; the proposition is necessarily true.
      return null;
    }

    public Counterexample FindCounterexample()
    {
      if ( this.FreeVariables.Any() && ContainsModalities )
        throw new EngineException( "This proposition can't be evaluated; it contains both constants and modal operators." );

      if ( this.FreeVariables.Any() )
        return Factory.Bind( FreeVariables, this, Factory.ForAll ).FindCounterexample();

      Predicates lPredicates = CollectPredicates();
      if ( this.ContainsModalities )
      {
#if COMPLEX_COUNTEREXAMPLE
        uint lFirstInterpretation = lPredicates.FirstInterpretation;
        for ( uint lInterpretation = lPredicates.LastInterpretation; lInterpretation >= lFirstInterpretation; lInterpretation-- )
#else
        uint lLastInterpretation = lPredicates.LastInterpretation;
        for ( uint lInterpretation = lPredicates.FirstInterpretation; lInterpretation <= lLastInterpretation; lInterpretation++ )
#endif
        {
          if ( HasCounterexample( lInterpretation, lPredicates ) )
          {
            List<KindOfWorld> lCounterexamples = new List<KindOfWorld>();
            List<KindOfWorld> lNonCounterexamples = new List<KindOfWorld>();
            foreach ( uint lKindOfWorld in lPredicates.KindsOfWorlds( lInterpretation ) )
            {
              if ( this.TrueIn( lInterpretation, lKindOfWorld, lPredicates ) )
                lNonCounterexamples.Add( lPredicates.DecodeKindOfWorldNumber( lKindOfWorld ) );
              else
                lCounterexamples.Add( lPredicates.DecodeKindOfWorldNumber( lKindOfWorld ) );   
            }
            return new ModalCounterexample( lCounterexamples, lNonCounterexamples );
          }
        }
      }
      else
      {
        uint lInterpretation = lPredicates.FirstInterpretation;
#if COMPLEX_COUNTEREXAMPLE
        uint lFirstKindOfWorld = lPredicates.FirstNonemptyWorld;
        for ( uint lKindOfWorld = lPredicates.LastKindOfWorld; lKindOfWorld >= lFirstKindOfWorld; lKindOfWorld-- )
#else
        uint lLastKindOfWorld = lPredicates.LastKindOfWorld;
        for ( uint lKindOfWorld = lPredicates.FirstNonemptyWorld; lKindOfWorld <= lLastKindOfWorld; lKindOfWorld++ )
#endif   
        {
          if ( !this.TrueIn( lInterpretation, lKindOfWorld, lPredicates ) )
            return lPredicates.DecodeKindOfWorldNumber( lKindOfWorld );
        }
      }

      // No counterexample exists; the proposition is necessarily true.
      return null;
    }

#if SALTARELLE
    private int StatusInterval
    {
      get
      {
        const int BaseInterval = 2500000;

        int lDepthOfLoopNesting = DepthOfLoopNesting;
        int lInterval = BaseInterval;

        for ( int i = 0; i < lDepthOfLoopNesting; i++ )
          lInterval /= 10;

        return Math.Max( 1, lInterval );
      }
    }
#endif

    /// <summary>
    /// Decide this matrix as a proposition.  Throw an exception if it contains both free variables and modal operators.
    /// Use up to System.Environment.ProcessorCount threads to make the decision.
    /// </summary>
    /// <returns>whether this proposition is necessary, contingent or impossible</returns>
    public Alethicity Decide()
    {
      if ( this.FreeVariables.Any() )
      {
        if ( ContainsModalities )
          throw new EngineException( "This proposition can't be decided; it contains both constants and modal operators." );
        else
          return DecideForFreeVariables();
      }

      Predicates lPredicates = CollectPredicates();

      bool lNotImpossible = false;
      bool lNotNecessary = false;
      uint lLastInterpretation = lPredicates.LastInterpretation;
      uint lLastKindOfWorld = lPredicates.LastKindOfWorld;
      uint lFirstNonemptyWorld = lPredicates.FirstNonemptyWorld;
      uint lFirstInterpretation = lPredicates.FirstInterpretation;
#if SALTARELLE
      int lStatusInterval = StatusInterval;
#endif

#if PARALLELIZE
      //System.Windows.Forms.MessageBox.Show( string.Format( "Maximum number of distinguishable objects: {0}", MaxmimumNumberOfDistinguishableObjects ) );
      
      System.Threading.Tasks.ParallelOptions lParallelOptions = new System.Threading.Tasks.ParallelOptions();
      System.Threading.CancellationTokenSource lCancellationTokenSource = new System.Threading.CancellationTokenSource();
      lParallelOptions.CancellationToken = lCancellationTokenSource.Token;
      lParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;

      try
      {
#endif
      if ( ContainsModalities  )
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
          for ( uint lInterpretation = lFirstInterpretation; lInterpretation <= lLastInterpretation; lInterpretation++ )
          {
#endif
#if SALTARELLE
            if ( ( lInterpretation - lFirstInterpretation ) % lStatusInterval == 0 )
              Utility.Status( String.Format(
                "Deciding... {0:n0} of {1:n0} interpretations of predicates tested.",
                lInterpretation - lFirstInterpretation,
                lLastInterpretation - lFirstInterpretation + 1 ) );
#endif

            foreach ( uint lKindOfWorld in lPredicates.KindsOfWorlds( lInterpretation ) )
            {
              if ( this.TrueIn( lInterpretation, lKindOfWorld, lPredicates ) )
                lNotImpossible = true;
              else
                lNotNecessary = true;

              // End the decision once it has been determined that the proposition is neither necessary nor impossible.
              // Further evaluation will not change the outcome.
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
          for ( uint lKindOfWorld = lFirstNonemptyWorld; lKindOfWorld <= lLastKindOfWorld; lKindOfWorld++ )
          {
#endif
#if SALTARELLE
            if ( ( lKindOfWorld - lFirstNonemptyWorld ) % lStatusInterval == 0 )
              Utility.Status( String.Format(
                "Deciding... {0:n0} of {1:n0} kinds of worlds tested.",
                lKindOfWorld - lFirstNonemptyWorld,
                lLastKindOfWorld - lFirstNonemptyWorld + 1 ) );
#endif

            if ( this.TrueIn( lInterpretation, lKindOfWorld, lPredicates ) )
                lNotImpossible = true;
            else
              lNotNecessary = true;

          // End the decision once it has been determined that the proposition is neither necessary nor impossible.
          // Further evaluation will not change the outcome.
            if ( lNotImpossible && lNotNecessary )
#if PARALLELIZE
              lCancellationTokenSource.Cancel();
          } );
        }
      }
      catch ( OperationCanceledException )
      {
        // An OperationCanceledException will occur iff lCancellationTokenSource.Cancel() is called; nothing needs to be done for it.
      }
      catch ( AggregateException lException )
      {
        // Combine all exception messages from the tasks into one message.
        throw new EngineException( String.Join(
          Environment.NewLine,
          lException.Flatten().InnerExceptions.Select( fException => fException.Message ).Distinct() ) );
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

    internal virtual IEnumerable<Tuple<UniversalGeneralization, Matrix>> ClosedPredications
    {
      get { yield break; }
    }

    internal virtual bool ContainsModalities
    {
      get { return false; }
    }

    internal virtual bool ContainsIdentifications
    {
      get { return false; }
    }

    internal virtual IEnumerable<Tuple<Matrix, Matrix>> DirectDependencies
    {
      get { yield break; }
    }

    internal abstract string DOTLabel { get; }

    /// <summary>
    /// DOT code for a graph diagram of this matrix
    /// </summary>
    public string GraphvizDOT
    {
      get
      {
        StringBuilder lDOT = new StringBuilder();
        lDOT.AddLine( "digraph Proposition_{0} {{", this.Name );
        lDOT.AddLine( "ordering=out;bgcolor=\"#FFFFFF80\"" );
        foreach ( Matrix lMatrix in this.Matrices.Distinct( new ReferenceComparer() ) )
        {
          if ( !( lMatrix is NullPredicate ) )
          {
            lDOT.AddLine(
              "{0} [label={1},shape=rectangle,fontsize=10,style=filled,fillcolor=\"white\",margin=\"0.11,0.00\"];",
              lMatrix.Name,
              lMatrix.DOTLabel );
          }
        }
        Tuple < Matrix, Matrix >[] lDirectDependencies = DirectDependencies.ToArray();
        for ( int i = 0; i < lDirectDependencies.Length; i++ )
        {
          Tuple<Matrix, Matrix> lPair = lDirectDependencies[ i ];
          if ( lPair.Item2 is NullPredicate )
          {
            lDOT.AddLine(
              "-{0} [label={1},shape=rectangle,fontsize=10,style=filled,fillcolor=\"white\",margin=\"0.11,0.00\"];",
              i,
              lPair.Item2.DOTLabel );
          }
        }
        for ( int i = 0; i < lDirectDependencies.Length; i++ )
        {
          Tuple<Matrix, Matrix> lPair = lDirectDependencies[ i ];
          if ( lPair.Item2 is NullPredicate )
          {
            lDOT.AddLine(
              "{0}-> -{1}",
              lPair.Item1.Name,
              i );
          }
          else
          {
            lDOT.AddLine(
              "{0}->{1}",
              lPair.Item1.Name,
              lPair.Item2.Name );
          }
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

    internal string MakeProver9Formulas( Dictionary<char, string> aTranslatedVariableNames )
    {
      return string.Join( "", Conjuncts.Select( fConjunct => fConjunct.Prover9InputHelper( aTranslatedVariableNames ) + ".\n" ) );
    }

    internal virtual string Prover9InputHelper1( Dictionary<char, string> aTranslatedVariableNames )
    {
      return string.Format(
        "formulas(goals).\n{0}.\nend_of_list.\n",
        Prover9InputHelper( aTranslatedVariableNames ) );
    }

    public string Prover9Input
    {
      get
      {
        Dictionary<char,string> lTranslatedVariableNames = new Dictionary<char,string>();
        foreach ( Variable lVariable in FreeVariables )
        {
          char lFirstCharacter = lVariable.ToString()[0];
          lTranslatedVariableNames[ lFirstCharacter ] = lFirstCharacter < 'u' || lFirstCharacter > 'z'
            ? lVariable.ToString()
            : "c" + lVariable.ToString();
        }
        return Prover9InputHelper1( lTranslatedVariableNames );
      }
    }

    internal abstract string Prover9InputHelper( Dictionary<char,string> aTranslatedVariableNames );

    public virtual string TreeProofGeneratorInput
    {
      get { throw new EngineException( "Can't generate input for Tree Proof Generator from this proposition." ); }
    }

    /// <summary>
    /// True if this matrix contains only nullary predicates and logical operators, false otherwise.  If it does, then
    /// it is a proposition in Propositional Logic, and a truth table can be generated for it.
    /// </summary>
    public virtual bool IsPropositional
    {
      get { return false; }
    }

    /// <summary>
    /// True if this matrix does not contain modal operators or identifications, false otherwise.  If it doesn't then
    /// it is compatible with the Tree Proof Generator.
    /// </summary>
    public virtual bool IsCompatibleWithTreeProofGenerator
    {
      get { return !( ContainsIdentifications || ContainsModalities ); }
    }

    internal abstract IEnumerable<Variable> FreeVariables { get; }

    internal abstract IEnumerable<Necessity> FreeModalities { get; }

    internal abstract IEnumerable<Variable> IdentifiedVariables { get; }    

    internal virtual IEnumerable<Matrix> Matrices
    {
      get { yield return this; }
    }

    internal abstract int MaxmimumNumberOfDistinguishableObjectsOfAKind { get; }

    internal abstract int MaxmimumNumberOfModalitiesInIdentifications { get; }

    internal virtual IEnumerable<Necessity> ModalitiesInIdentifications
    {
      get { yield break; }
    }

    internal virtual int DepthOfLoopNesting
    {
      get { return 0; }
    }

    internal virtual IEnumerable<Matrix> NonNullPredications
    {
      get { yield break; }
    }

    internal abstract IEnumerable<NullPredicate> NullPredicates { get; }	

    internal abstract IEnumerable<UnaryPredicate> UnaryPredicates { get; }

    internal virtual void AssignModality( Necessity aNecessity ) {}

    /// <summary>
    /// Find the set of kinds of worlds that this matrix, with free variables instantiated, excludes from
    /// possibility.
    /// </summary>
    /// <param name="aPredicates">the predicates present in the proposition that this matrix belongs to.</param>
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

    internal abstract bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicates );
    
    internal virtual IEnumerable<Matrix> Conjuncts
    {
      get { yield return this; }
    }

    internal abstract Matrix Substitute( Variable aVariable, Variable aReplacement );
	}

  /// <summary>
  /// possible alethic modalities
  /// </summary>
  public enum Alethicity { Necessary, Contingent, Impossible };
}
