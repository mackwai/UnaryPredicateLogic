using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Translate
{
  class Program
  {
    private static void Translate( ref StringBuilder aText, Tuple<string,string> aTranslation )
    {
      aText.Replace( ">" + aTranslation.Item1 + "<", ">" + aTranslation.Item2 + "<" );
      aText.Replace( "\"" + aTranslation.Item1 + "\"", "\"" + aTranslation.Item2 + "\"" );
      aText.Replace( "'" + aTranslation.Item1 + "'", "'" + aTranslation.Item2 + "'" );
    }

    private static IEnumerable<Tuple<string,string>> ParseTranslations( string aPathToFile )
    {
      string lOriginal = null;
      string lTranslation = null;
      foreach ( string lLine in File.ReadLines( aPathToFile ) )
      {
        if ( lOriginal == null && !String.IsNullOrWhiteSpace( lLine ) )
          lOriginal = lLine;
        else if ( lOriginal != null && lTranslation == null )
          lTranslation = lLine;
        else
        {
          yield return new Tuple<string,string>( lOriginal, lTranslation );
          lOriginal = null;
          lTranslation = null;
        }
      }

      if ( lOriginal != null && lTranslation != null )
        yield return new Tuple<string,string>( lOriginal, lTranslation );
    }
    static void Main( string[] args )
    {
      try
      {
        StringBuilder lText = new StringBuilder( File.ReadAllText( args[ 0 ] ) );
        foreach ( Tuple<string,string> lTranslation in ParseTranslations( args[1] ) )
        {
          Translate( ref lText, lTranslation );
        }
        File.WriteAllText( args[2], lText.ToString() );
      }
      catch ( Exception lException )
      {
        Console.Error.WriteLine( lException.Message );
        Environment.Exit( 1 );
      }
    }
  }
}
