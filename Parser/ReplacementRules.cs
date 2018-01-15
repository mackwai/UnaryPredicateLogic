using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Logic
{
  internal class ReplacementRules
  {
    private static Regex ReplacementDirectivePattern = new Regex( @"^\s*#\s*replace\s+(\S+)\s+(\S+)\s*$" );
    private static Regex StartsWithWord = new Regex( @"^\w" );
    private static Regex EndsWithWord = new Regex( @"^\w" );

    public static string RegexReplace( string aString, Regex aPattern, string aReplacement )
    {
#if SALTARELLE
      string lChangedString = aString;
      do
      {
        aString = lChangedString;
        lChangedString = aString.Replace( aPattern, aReplacement );
      } while ( lChangedString != aString );
      return aString;
#else
      return aPattern.Replace( aString, aReplacement );
#endif

    }

    public static bool IsReplacementDirective( string aString )
    {
      return ReplacementDirectivePattern.Exec( aString ) != null;
    }

    private List<Tuple<Regex, string>> mReplacements;

    private static Tuple<Regex,string> MakeReplacement( string aWordOrSymbol, string aReplacement )
    {
      return new Tuple<Regex,string>(
        new Regex( string.Format( "{0}{1}{2}",
          StartsWithWord.Exec( aWordOrSymbol ) != null ? @"(\W|^)" : @"([\w\s]|^)",
          Regex.Escape( aWordOrSymbol ),
          EndsWithWord.Exec( aWordOrSymbol ) != null ? @"(\W|$)" : @"([\w\s]|$)" ) ),
        string.Format ("$1{0}$2", aReplacement ) );
    }

    public ReplacementRules()
    {
      mReplacements = new List<Tuple<Regex, string>>();
    }

    public void Add( string aReplacementDirective )
    {
      RegexMatch lMatch = ReplacementDirectivePattern.Exec( aReplacementDirective );
      mReplacements.Add( MakeReplacement( lMatch[ 1 ], lMatch[ 2 ] ) );
    }

    public string Apply( string aString )
    {
      return mReplacements.Aggregate( aString, ( fString, fPair ) => RegexReplace( fString, fPair.Item1, fPair.Item2 ) );
    }
  }
}
