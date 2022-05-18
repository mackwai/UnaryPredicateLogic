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
    private uint? mLastExampleFound = null;
    private bool mExampleMightBe = true;
    private uint? mLastCounterexampleFound = null;
    private bool mCounterexampleMightBe = true;

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

    private static uint NextPossibility( uint aPossibility, uint aFirstPossiblity, uint aLastPossbility )
    {
      return aPossibility == aLastPossbility
        ? aFirstPossiblity
        : aPossibility + 1;
    }

    private static ModalCounterexample ConstructModalCounterexample( Matrix aProposition, uint aInterpretation, Predicates aPredicates )
    {
      List<KindOfWorld> lCounterexamples = new List<KindOfWorld>();
      List<KindOfWorld> lNonCounterexamples = new List<KindOfWorld>();
      foreach ( uint lKindOfWorld in aPredicates.KindsOfWorlds( aInterpretation ) )
      {
        if ( aProposition.TrueIn( aInterpretation, lKindOfWorld, aPredicates ) )
          lNonCounterexamples.Add( aPredicates.DecodeKindOfWorldNumber( lKindOfWorld ) );
        else
          lCounterexamples.Add( aPredicates.DecodeKindOfWorldNumber( lKindOfWorld ) );   
      }
      return new ModalCounterexample( lCounterexamples, lNonCounterexamples );
    }

    public Counterexample FindNextExample()
    {
      if ( this.FreeVariables.Any() && ContainsModalities )
        throw new EngineException( "This proposition can't be evaluated; it contains both constants and modal operators." );

      Matrix lNegation = Factory.Not( this );
      lNegation = Factory.Bind( FreeVariables, lNegation, Factory.ForAll );
      Counterexample lExample = null;
      
      Predicates lPredicates = lNegation.CollectPredicates();
      if ( lNegation.ContainsModalities )
      {
        uint lLastInterpretation = lPredicates.LastInterpretation;

        uint lPossibility = mLastExampleFound.HasValue
          ? mLastExampleFound.Value
          : lLastInterpretation;

        while ( mExampleMightBe )
        {
          lPossibility = NextPossibility( lPossibility, lPredicates.FirstInterpretation, lLastInterpretation );

          if ( lNegation.HasCounterexample( lPossibility, lPredicates ) )
          {
            mLastExampleFound = lPossibility;
            break;
          }
          else if ( lPossibility == lLastInterpretation && !mLastExampleFound.HasValue )
          {
            mExampleMightBe = false;
            break;
          }
        }

        if ( mLastExampleFound.HasValue )
          lExample = ConstructModalCounterexample( lNegation, mLastExampleFound.Value, lPredicates );
      }
      else
      {
        uint lInterpretation = lPredicates.FirstInterpretation;
        uint lLastKindOfWorld = lPredicates.LastKindOfWorld;

        uint lPossibility = mLastExampleFound.HasValue
          ? mLastExampleFound.Value
          : lLastKindOfWorld;

        while ( mExampleMightBe )
        {
          lPossibility = NextPossibility( lPossibility, lPredicates.FirstNonemptyWorld, lLastKindOfWorld );

          if ( !lNegation.TrueIn( lInterpretation, lPossibility, lPredicates ) )
          {
            mLastExampleFound = lPossibility;
            break;
          }
          else if ( lPossibility == lLastKindOfWorld && !mLastExampleFound.HasValue )
          {
            mExampleMightBe = false;
            break;
          }
        }

        if ( mLastExampleFound.HasValue )
          lExample = lPredicates.DecodeKindOfWorldNumber( mLastExampleFound.Value );
      }
      return lExample;
    }

    public Counterexample FindNextCounterexample()
    {
      if ( this.FreeVariables.Any() && ContainsModalities )
        throw new EngineException( "This proposition can't be evaluated; it contains both constants and modal operators." );

      Matrix lProposition = this;
      if ( lProposition.FreeVariables.Any() )
        lProposition = Factory.Bind( FreeVariables, this, Factory.ForAll );
      Counterexample lCounterexample = null;

      Predicates lPredicates = lProposition.CollectPredicates();
      if ( lProposition.ContainsModalities )
      {
        uint lLastInterpretation = lPredicates.LastInterpretation;

        uint lPossibility = mLastCounterexampleFound.HasValue
          ? mLastCounterexampleFound.Value
          : lLastInterpretation;

        while ( mCounterexampleMightBe )
        {
          lPossibility = NextPossibility( lPossibility, lPredicates.FirstInterpretation, lLastInterpretation );

          if ( lProposition.HasCounterexample( lPossibility, lPredicates ) )
          {
            mLastCounterexampleFound = lPossibility;
            break;
          }
          else if ( lPossibility == lLastInterpretation && !mLastCounterexampleFound.HasValue )
          {
            mCounterexampleMightBe = false;
            break;
          }
        }

        if ( mLastCounterexampleFound.HasValue )
          lCounterexample = ConstructModalCounterexample( lProposition, mLastCounterexampleFound.Value, lPredicates );
      }
      else
      {
        uint lInterpretation = lPredicates.FirstInterpretation;
        uint lLastKindOfWorld = lPredicates.LastKindOfWorld;

        uint lPossibility = mLastCounterexampleFound.HasValue
          ? mLastCounterexampleFound.Value
          : lLastKindOfWorld;

        while ( mExampleMightBe )
        {
          lPossibility = NextPossibility( lPossibility, lPredicates.FirstNonemptyWorld, lLastKindOfWorld );

          if ( !lProposition.TrueIn( lInterpretation, lPossibility, lPredicates ) )
          {
            mLastCounterexampleFound = lPossibility;
            break;
          }
          else if ( lPossibility == lLastKindOfWorld && !mLastCounterexampleFound.HasValue )
          {
            mCounterexampleMightBe = false;
            break;
          }
        }

        if ( mLastCounterexampleFound.HasValue )
          lCounterexample = lPredicates.DecodeKindOfWorldNumber( mLastCounterexampleFound.Value );
      }
      return lCounterexample;
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

    private delegate void ActionWithBreakFlag<T>( T aInput, ref bool aBreak );

#if PARALLELIZE
    private static void Foreach<T>( IEnumerable<T> aEnumerable, ActionWithBreakFlag<T> aAction )
    {
      System.Threading.Tasks.ParallelOptions lParallelOptions = new System.Threading.Tasks.ParallelOptions();
      System.Threading.CancellationTokenSource lCancellationTokenSource = new System.Threading.CancellationTokenSource();
      lParallelOptions.CancellationToken = lCancellationTokenSource.Token;
      lParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;

      bool lBreak = false;

      try
      {
        System.Threading.Tasks.Parallel.ForEach(
          aEnumerable,
          lParallelOptions,
          (fItem) =>
          {
            aAction( fItem, ref lBreak );

            if ( lBreak )
              lCancellationTokenSource.Cancel();
          } );
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
    }
#else
    private static void Foreach<T>( IEnumerable<T> aEnumerable, ActionWithBreakFlag<T> aAction )
    {
      bool lBreak = false;

      foreach ( T aItem in aEnumerable )
      {
        aAction( aItem, ref lBreak );

        if ( lBreak )
          break;
      }
    }
#endif

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

      Foreach(
        lPredicates.Interpretations,
        ( uint fInterpretation, ref bool fNoMoreIterationsNeeded ) =>
        {
          foreach ( uint lKindOfWorld in lPredicates.KindsOfWorlds( fInterpretation ) )
          {
            if ( this.TrueIn( fInterpretation, lKindOfWorld, lPredicates ) )
              lNotImpossible = true;
            else
              lNotNecessary = true;
            
            // End the decision once it has been determined that the proposition is neither necessary nor impossible.
            // Further evaluation will not change the outcome.
            if ( lNotImpossible && lNotNecessary )
            {
              fNoMoreIterationsNeeded = true;
              return;
            }
          }
        }
      );

      if ( lNotImpossible && lNotNecessary )
        return Alethicity.Contingent;
      else if ( lNotNecessary )
        return Alethicity.Impossible;
      else
        return Alethicity.Necessary;
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
        Matrix[] lMatrices = this.Matrices.Distinct( new ReferenceComparer() ).Cast<Matrix>().ToArray();
        if ( lMatrices.Length == 1 )
        {
          lDOT.AddLine(
            "{0} [label={1},shape=rectangle,fontsize=10,style=filled,fillcolor=\"white\",margin=\"0.11,0.00\"];",
            this.Name,
            this.DOTLabel );
        }
        else
        {
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
          Tuple < Matrix, Matrix >[] lDirectDependencies = this.DirectDependencies.ToArray();
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

    internal virtual CoefficientVector CoefficientVectorHelper( NullPredicate[] aNullPredicates )
    {
      throw new NotImplementedException( "Coefficient vectors are only defined for statements that only have nullary predicates and truth-functional operators." );
    }

    public CoefficientVector CoefficientVector
    {
      get
      {
        return CoefficientVectorHelper( NullPredicates.OrderBy( fPredicate => fPredicate.ToString() ).ToArray() );
      }
    }
  }

  /// <summary>
  /// possible alethic modalities
  /// </summary>
  public enum Alethicity { Necessary, Contingent, Impossible };
}
