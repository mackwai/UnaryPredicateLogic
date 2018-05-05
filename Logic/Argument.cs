using System.Collections.Generic;

namespace Logic
{
  public enum Quality
  {
    Invalid,
    Valid,
    InconsistentPremises,
    TautologicalConclusion,
    InconsistentPremisesAndTautologicalConclusion
  }

  public class Argument : MaterialConditional
  {
    private Quality? mQuality;

    internal Argument( IEnumerable<Matrix> aPremises, Matrix aConclusion )
      : base( Factory.And( aPremises ), aConclusion )
    {
    }

    protected override string Connector
    {
      get { return ".'."; }
    }

    internal override string DOTLabel
    {
      get { return "<Argument<BR/><B><FONT FACE=\"MONOSPACE\">.'.</FONT></B>>"; }
    }

    public Quality Quality
    {
      get
      {
        if ( !mQuality.HasValue )
        {
          Alethicity lArgument = Decide();

          if ( lArgument != Alethicity.Necessary )
          {
            mQuality = Quality.Invalid;
          }
          else
          {
            Alethicity lPremises = Left.Decide();
            Alethicity lConclusion = Right.Decide();

            if ( lPremises == Alethicity.Impossible && lConclusion == Alethicity.Necessary )
              mQuality = Quality.InconsistentPremisesAndTautologicalConclusion;
            else if ( lPremises == Alethicity.Impossible )
              mQuality = Quality.InconsistentPremises;
            else if ( lConclusion == Alethicity.Necessary )
              mQuality = Quality.TautologicalConclusion;
            else
              mQuality = Quality.Valid;

          }
        }

        return mQuality.Value;
      }
    }
  }
}
