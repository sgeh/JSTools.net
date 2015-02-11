/*
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;

namespace JSTools.Parser.Cruncher
{
	/// <summary>
	/// There are three types of functions that can be defined. The first
	/// is a function statement. This is a function appearing as a top-level
	/// statement (i.e., not nested inside some other statement) in either a
	/// script or a function.
	/// 
	/// The second is a function expression, which is a function appearing in
	/// an expression except for the third type, which is...
	/// 
	/// The third type is a function expression where the expression is the
	/// top-level expression in an expression statement.
	/// 
	/// The three types of functions have different treatment and must be
	/// distinquished.
	/// </summary>
	public enum FunctionType
	{
		FunctionStatement = 1,
		FunctionExpression = 2,
		FunctionExpressionStatement = 3
	}

	internal class FunctionNode : Node 
	{
		protected bool _itsNeedsActivation = false;
		protected bool _itsCheckThis = false;
		protected FunctionType _itsFunctionType = FunctionType.FunctionStatement;
		private string _functionName;

		public FunctionType FunctionType
		{
			get { return _itsFunctionType; }
			set { _itsFunctionType = value; }
		}

		public bool RequiresActivation
		{
			get { return _itsNeedsActivation; }
			set { _itsNeedsActivation = value; }
		}

		public bool CheckThis
		{
			get { return _itsCheckThis; }
			set { _itsCheckThis = value; }
		}

		public string FunctionName
		{
			get { return _functionName; }
		}

		internal FunctionNode(string name, Node left, Node right) : base(TokenType.Function, left, right)
		{
			_functionName = name;
		}
	}
}
