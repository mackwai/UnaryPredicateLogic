// somerby.net/mack/logic
// Copyright (C) 2016 MacKenzie Cumings
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
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Logic
{
  public static class Prover9Mace4
  {
    public enum Result { Necessary, Possible, Contingent, Unnecessary, Impossible, NoDecision };

    public static string ToString( Result aResult )
    {
      switch ( aResult )
      {
        case Result.Necessary:
          return "necessary";
        case Result.Possible:
          return "possible";
        case Result.Contingent:
          return "contingent";
        case Result.Unnecessary:
          return "unncessary";
        case Result.Impossible:
          return "impossible";
        case Result.NoDecision:
          return "no decision";
        default:
          throw new EngineException( "Unhanded enum {0}.", aResult );
      }
    }

    private const string prover9 = @"prover9";
    private const string mace4 = @"mace4";
    private static Regex TheoremProved = new Regex( @"Exiting with \d+ proof" );
    private static Regex CounterexampleFound = new Regex( @"Exiting with \d+ model" );

    public static Result Decide( Matrix aProposition, int aSecondsTimeout )
    {
      string lPositiveInput;
      string lNegativeInput;
      try
      {
        lPositiveInput = aProposition.Prover9Input;
        lNegativeInput = Factory.Not( aProposition ).Prover9Input;
      }
      catch ( EngineException lException )
      {
        if ( lException.Message.Contains( "modal" ) )
          return Result.NoDecision;
        else
          throw lException;
      }

      bool lPositiveCounterexampleFound = false;
      bool lNegativeCounterexampleFound = false;
      bool lFoundThatPropositionIsNecessary = false;
      bool lFoundThatPropositionIsImpossible = false;

      // Time out value for the cancellation tokens. -1 signifies no time out.
      int aMillisecondsTimeout = aSecondsTimeout >= 0
        ? aSecondsTimeout * 1000
        : -1;

      // Time out argument for prover9 and mace4.  Leave blank if the time out is negative.
      string aTimeoutArgument = aSecondsTimeout >= 0
        ? string.Format( "-t {0}", aSecondsTimeout )
        : "";

      CancellationTokenSource lPositiveCaseTokenSource = new System.Threading.CancellationTokenSource( aMillisecondsTimeout );
      CancellationTokenSource lNegativeCaseTokenSource = new System.Threading.CancellationTokenSource( aMillisecondsTimeout );

      ParallelOptions lParallelOptions = new System.Threading.Tasks.ParallelOptions();
      lParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;

      Parallel.Invoke( 
        lParallelOptions,
        // Try to prove that the proposition is necessary.
        BackgroundProcesses.ExcuteUntilExitOrCancellation(
          lPositiveCaseTokenSource.Token,
          new ShellParameters( prover9, aTimeoutArgument, lPositiveInput ),
          // If the proposition is proven, set a flag and cancel all other operations.
          ( fOutput ) => {
            if ( TheoremProved.IsMatch( fOutput ) )
            {
              lFoundThatPropositionIsNecessary = true;
              lPositiveCaseTokenSource.Cancel();
              lNegativeCaseTokenSource.Cancel();
            }
          } ),
        // Try to prove that the proposition is impossible.
        BackgroundProcesses.ExcuteUntilExitOrCancellation(
          lNegativeCaseTokenSource.Token,
          new ShellParameters( prover9, aTimeoutArgument, lNegativeInput ),
          // If the negation of the proposition is proven, set a flag and cancel all other operations.
          ( fOutput ) => {
            if ( TheoremProved.IsMatch( fOutput ) )
            {
              lFoundThatPropositionIsImpossible = true;
              lPositiveCaseTokenSource.Cancel();
              lNegativeCaseTokenSource.Cancel();
            }
          } ),
        // Try to find a counterexample for the proposition.
        BackgroundProcesses.ExcuteUntilExitOrCancellation(
          lPositiveCaseTokenSource.Token,
          new ShellParameters( mace4, aTimeoutArgument, lPositiveInput ),
          // If a counterexample is found for the positive case,
          // set a flag and cancel the attempt to prove that the proposition is necessary.
          ( fOutput ) => {
            if ( CounterexampleFound.IsMatch( fOutput ) )
            {
              lPositiveCounterexampleFound = true;
              lPositiveCaseTokenSource.Cancel();
            }
          } ),
        // Try to find a counterexample to the negative case.
        BackgroundProcesses.ExcuteUntilExitOrCancellation(
          lNegativeCaseTokenSource.Token,
          new ShellParameters( mace4, aTimeoutArgument, lNegativeInput ),
          // If a counterexample is found for the negative case,
          // set a flag and cancel the attempt to prove that the proposition is impossible.
          ( fOutput ) => {
            if ( CounterexampleFound.IsMatch( fOutput ) )
            {
              lNegativeCounterexampleFound = true;
              lNegativeCaseTokenSource.Cancel();
            }
          } )
        );

      if ( lPositiveCounterexampleFound && lFoundThatPropositionIsNecessary )
        throw new EngineException( "Inconsistent result!" );

      if ( lNegativeCounterexampleFound && lFoundThatPropositionIsImpossible )
        throw new EngineException( "Inconsistent result!" );

      if ( lFoundThatPropositionIsNecessary && lFoundThatPropositionIsImpossible )
        throw new EngineException( "Inconsistent result!" );

      if ( lFoundThatPropositionIsNecessary )
        return Result.Necessary;

      if  ( lFoundThatPropositionIsImpossible )
        return Result.Impossible;
     
      if ( lPositiveCounterexampleFound && lNegativeCounterexampleFound )
        return Result.Contingent;

      if ( lPositiveCounterexampleFound )
        return Result.Unnecessary;

      if ( lNegativeCounterexampleFound )
        return Result.Possible;

      return Result.NoDecision;
    }
  }
}
