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

namespace Logic
{
  /// <summary>
  /// a term of Term Logic, interpreted as a unary predicate or the negation of a unary predicate
  /// </summary>
  public class Term
  {
    private bool mIsNegative;
    private UnaryPredicate mPredicate;

    internal Term( UnaryPredicate aPredicate, bool aIsNegative )
    {
      mIsNegative = aIsNegative;
      mPredicate = aPredicate;
    }

    /// <summary>
    /// Apply the term to a variable, yielding a Matrix representing whether or not the term contains an object.
    /// </summary>
    /// <param name="aVariable">a variable</param>
    /// <returns>a Matrix representing whether or not the term contains an object</returns>
    public Matrix Apply( Variable aVariable )
    {
      return mIsNegative
        ? Factory.Not( Factory.Predication( mPredicate, aVariable ) )
        : Factory.Predication( mPredicate, aVariable );
    }
  }
}
