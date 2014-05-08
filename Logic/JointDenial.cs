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

namespace Logic
{
  /// <summary>
  /// a joint denial
  /// </summary>
	internal class JointDenial : BinaryOperator
	{
	  protected override string Connector
    {
      get { return "!"; }
    }
	  
	  internal JointDenial( Matrix aLeft, Matrix aRight ) : base( aLeft, aRight )
	  {
		}
		
		internal override bool TrueIn( uint aInterpretation, uint aKindOfWorld, Predicates aPredicates )
		{
      return !Left.TrueIn( aInterpretation, aKindOfWorld, aPredicates ) && !Right.TrueIn( aInterpretation, aKindOfWorld, aPredicates );
		}

    internal override string DOTLabel
    {
      get { return "<Joint Denial<BR/><B><FONT FACE=\"MONOSPACE\">!</FONT></B>>"; }
    }
	}
}
