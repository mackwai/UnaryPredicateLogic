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


using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
  public static class HTMLMaker
  {
    public static string MakeHTML( Counterexample aCounterexample )
    {
      if ( aCounterexample == null )
        return "<p>This proposition is necessarily true; there are no counterexamples.</p>";
      else if ( aCounterexample is KindOfWorld )
        return "<div class=\"counterexample\"><h3>Counterexample</h3><p>This is a kind of world in which the statement is not true.</p>"
         + MakeHTML( aCounterexample as KindOfWorld, @"black" ) + "</div>";
      else if ( aCounterexample is ModalCounterexample )
        return MakeHTML( aCounterexample as ModalCounterexample );
      else
        throw new EngineException( "Unhandled subclass of Counterexample encountered in MakeHTML: {0}", aCounterexample.GetType() );
    }

    public static string MakeHTMLForExample( Counterexample aCounterexample )
    {
      if ( aCounterexample == null )
        return "<p>This proposition is self-contradictory; there are no examples.</p>";
      else if ( aCounterexample is KindOfWorld )
        return "<div class=\"counterexample\"><h3>Example</h3><p>This is a kind of world in which the statement is true.</p>"
         + MakeHTML( aCounterexample as KindOfWorld, @"black" ) + "</div>";
      else if ( aCounterexample is ModalCounterexample )
        return MakeHTMLForExample( aCounterexample as ModalCounterexample );
      else
        throw new EngineException( "Unhandled subclass of Counterexample encountered in MakeHTML: {0}", aCounterexample.GetType() );
    }

    private static string MakeHTML( KindOfWorld aCounterexample, string aColor )
    {
      StringBuilder lText = new StringBuilder();
      lText.AddLine( "<div style=\"color:{0}\" class=\"counterexample\">", aColor );
      lText.AddLine( "<h3>Kind of World</h3>" );
      if ( aCounterexample.Predicates.Any() )
      {
        lText.AddLine( "<h4>Nullary Predicates:</h4>" );
        lText.AddLine( "<ul><li style=\"{0}\">", aColor );
        foreach ( NullPredicate lPredicate in aCounterexample.Predicates )
        {
          if ( aCounterexample.Affirms( lPredicate ) )
            lText.Append( lPredicate.ToString() );
          else
            lText.AppendFormat( "<span style=\"text-decoration:overline;\">{0}</span>", lPredicate );
        }
        lText.AddLine( "</li></ul>" );
      }
      if ( aCounterexample.KindsOfObjects.Any() )
      {
        lText.AddLine( "<h4>Kinds of Objects:</h4>" );
        lText.AddLine( "<ul>" );
        foreach ( KindOfObject lKindOfObject in aCounterexample.KindsOfObjects )
        {
          lText.AppendFormat( "<li>{0} {1}", lKindOfObject.NumberOfDistinguishableInstances, lKindOfObject.Predicates.Count() == 0 ? "distinct" : "&times; "  );
          foreach ( UnaryPredicate lPredicate in lKindOfObject.Predicates )
          {
            if ( lKindOfObject.Affirms( lPredicate ) )
              lText.Append( lPredicate.ToString() );
            if ( lKindOfObject.Denies( lPredicate ) )
              lText.AppendFormat( "<span style=\"text-decoration:overline;\">{0}</span>", lPredicate );
          }
          lText.AddLine ( "</li>" );
        }
        lText.AddLine( "</ul>" );
      }
      lText.AddLine( "</div>" );

      return lText.ToString().Replace( "</span><span style=\"text-decoration:overline;\">", "" );
    }

    private static string MakeHTML( ModalCounterexample aCounterexample )
    {
      StringBuilder lText = new StringBuilder();
      lText.AddLine( "<div class=\"counterexample\">" );
      lText.AddLine( "<h3>Counterexample</h3>" );
      lText.AddLine( "<p>This is an interpretation of predicates in which the statement is not necessarily true."
        + "  <span style=\"color:red\">Red text</span> indicates a kind of world in which the statement is false."
        + "  <span style=\"color:black;font-weight:bold\">Black text</span> indicates a kind of world in which the statement is true.</p>" );
      foreach ( KindOfWorld lKindOfWorld in aCounterexample.Counterexamples )
      {
        lText.AddLine( MakeHTML( lKindOfWorld, "red" ) );
      }
      foreach ( KindOfWorld lKindOfWorld in aCounterexample.NonCounterexamples )
      {
        lText.AddLine( MakeHTML( lKindOfWorld, "black" ) );
      }
      lText.AddLine( "</div>" );
      return lText.ToString();
    }

    private static string MakeHTMLForExample( ModalCounterexample aExample )
    {
      StringBuilder lText = new StringBuilder();
      lText.AddLine( "<div class=\"counterexample\">" );
      lText.AddLine( "<h3>Example</h3>" );
      lText.AddLine( "<p>This is an interpretation of predicates in which the statement is possibly true."
        + "  <span style=\"color:red\">Red text</span> indicates a kind of world in which the statement is true."
        + "  <span style=\"color:black;font-weight:bold\">Black text</span> indicates a kind of world in which the statement is false.</p>" );
      foreach ( KindOfWorld lKindOfWorld in aExample.Counterexamples )
      {
        lText.AddLine( MakeHTML( lKindOfWorld, "red" ) );
      }
      foreach ( KindOfWorld lKindOfWorld in aExample.NonCounterexamples )
      {
        lText.AddLine( MakeHTML( lKindOfWorld, "black" ) );
      }
      lText.AddLine( "</div>" );
      return lText.ToString();
    }
  }
}
