/*
 * JSTools.ScriptTypes.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
 * Copyright (C) 2005  Silvan Gehrig
 *
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
 *
 * Author:
 *  Silvan Gehrig
 */

using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JSTools.ScriptTypes
{
	/// <summary>
	/// Represents the javascript number type. A number is mapped with the
	/// following .NET datatypes:
	/// decimal, double, float, long, int, short, byte
	/// </summary>
	public class Number : AScriptType
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private static readonly Regex SCRIPT_NUMBER_PATTERN = new Regex(@"^(?<number>[+\-]?(\d+.?\d*|\d*.\d+))(e(?<sign>[+\-]?)(?<exponent>\d+))?$", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static readonly Regex MANAGED_NUMBER_PATTERN = new Regex(@"^(?<number>[+\-]?(\d+.?\d*|\d*.\d+))((?<sign>[+\-]?)e(?<exponent>\d+))?$", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private const string NUMBER_GROUP = "number";
		private const string SIGN_GROUP = "sign";
		private const string EXPONENT_GROUP = "exponent";

		private const string MAX_VALUE = "Number.MAX_VALUE";
		private const string MIN_VALUE = "Number.MIN_VALUE";
		private const string POSITIVE_INFINITY = "Number.POSITIVE_INFINITY";
		private const string NEGATIVE_INFINITY = "Number.NEGATIVE_INFINITY";
		private const string NaN = "Number.NaN";

		private const string MAX_VALUE_SCRIPT = "1.7976931348623157e+308";
		private const string MIN_VALUE_SCRIPT = "5e-324";
		private const string POSITIVE_INFINITY_SCRIPT = "Infinity";
		private const string NEGATIVE_INFINITY_SCRIPT = "-Infinity";
		private const string NaN_SCRIPT = "NaN";

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="AScriptType.ManagedTypes"/>
		/// </summary>
		internal protected override Type[] ManagedTypes
		{
			get
			{
				return new Type[] {
									  typeof(decimal),
									  typeof(double),
									  typeof(float),
									  typeof(long),
									  typeof(ulong),
									  typeof(int),
									  typeof(uint),
									  typeof(short),
									  typeof(ushort),
									  typeof(byte)
								  };
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new Number instance.
		/// </summary>
		public Number()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="AScriptType" />
		/// </summary>
		/// <param name="toCheck">
		///  <see cref="AScriptType" />
		/// </param>
		/// <returns>
		///  <see cref="AScriptType" />
		/// </returns>
		public override bool IsTypeOf(string toCheck)
		{
			if (toCheck != null && (GetSpecialValueFromString(toCheck) != 0 || SCRIPT_NUMBER_PATTERN.IsMatch(toCheck)))
				return true;

			return false;
		}

		/// <summary>
		///  <see cref="AScriptType" />
		/// </summary>
		/// <param name="valueToConvert">
		///  <see cref="AScriptType" />
		/// </param>
		/// <param name="encodeValue">
		///  <see cref="AScriptType"/>
		/// </param>
		/// <returns>
		///  <see cref="AScriptType" />
		/// </returns>
		protected override string GetStringRepresentation(object valueToConvert, bool encodeValue)
		{
			// convert given value
			double managedValue = 0;
			unchecked { managedValue = Convert.ToDouble(valueToConvert); }

			// try to evaluate the representing client script values
			if (double.IsPositiveInfinity(managedValue))
				return POSITIVE_INFINITY;

			if (double.IsNegativeInfinity(managedValue))
				return NEGATIVE_INFINITY;

			if (managedValue >= double.MaxValue)
				return MAX_VALUE;

			if (managedValue <= double.MinValue)
				return MIN_VALUE;

			if (double.IsNaN(managedValue))
				return NaN;

			string convertedValue = GetStringRepresentation(valueToConvert);

			// javascript has an other exp-format than .NET
			Match convertedMatch = MANAGED_NUMBER_PATTERN.Match(convertedValue);

			// change exp-sign, if needed
			if (convertedMatch.Groups[EXPONENT_GROUP].Length != 0)
				return convertedMatch.Groups[NUMBER_GROUP].Value
					+ "e"
					+ convertedMatch.Groups[SIGN_GROUP].Value
					+ convertedMatch.Groups[EXPONENT_GROUP].Value;

			return convertedValue;
		}

		/// <summary>
		///  <see cref="AScriptType" />
		/// </summary>
		/// <param name="valueToConvert">
		///  <see cref="AScriptType" />
		/// </param>
		/// <param name="decodeValue">
		///  <see cref="AScriptType" />
		/// </param>
		/// <returns>
		///  <see cref="AScriptType" />
		/// </returns>
		protected override object GetValueFromString(string valueToConvert, bool decodeValue)
		{
			// try to parse the given string as special numeric value
			double specialValue = GetSpecialValueFromString(valueToConvert);

			if (specialValue != 0)
				return specialValue;

			// prepare given string for casting
			Match valueMatch = SCRIPT_NUMBER_PATTERN.Match(valueToConvert);

			// javascript has an other exp-format than .NET
			if (valueMatch.Groups[EXPONENT_GROUP].Length != 0)
				valueToConvert = valueMatch.Groups[NUMBER_GROUP].Value
					+ valueMatch.Groups[SIGN_GROUP].Value
					+ "e"
					+ valueMatch.Groups[EXPONENT_GROUP].Value;

			// try to parse the given string
			double result;

			if (double.TryParse(valueToConvert, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out result))
				return result;
			else
				return null;
		}

		private double GetSpecialValueFromString(string valueToConvert)
		{
			if (valueToConvert == MAX_VALUE_SCRIPT || valueToConvert == MAX_VALUE)
				return double.MaxValue;

			if (valueToConvert == MIN_VALUE_SCRIPT || valueToConvert == MIN_VALUE)
				return double.MinValue;

			if (valueToConvert == POSITIVE_INFINITY_SCRIPT || valueToConvert == POSITIVE_INFINITY)
				return double.PositiveInfinity;

			if (valueToConvert == NEGATIVE_INFINITY_SCRIPT || valueToConvert == NEGATIVE_INFINITY)
				return double.NegativeInfinity;

			if (valueToConvert == NaN_SCRIPT || valueToConvert == NaN)
				return double.NaN;

			return 0;
		}

		private string GetStringRepresentation(object valueToConvert)
		{
			MethodInfo toStringMethod = valueToConvert.GetType().GetMethod("ToString", new Type[] { typeof(IFormatProvider) } );

			if (toStringMethod != null)
				return (string)toStringMethod.Invoke(valueToConvert, new object[] { CultureInfo.InvariantCulture} );
			else
				return valueToConvert.ToString();
		}
	}
}
