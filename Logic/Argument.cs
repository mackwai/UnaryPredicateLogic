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

    //protected override string Connector
    //{
    //  get { return ".'."; }
    //}

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
