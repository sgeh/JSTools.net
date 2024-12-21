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
using System.Globalization;
using System.Text.RegularExpressions;

namespace JSTools.ScriptTypes
{
	/// <summary>
	/// Represents the javascript number type. A number is mapped with the
	/// following .NET datatypes:
	///  double, float, long, int, short, byte
	/// </summary>
	public class Number : AScriptType
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private static readonly Regex SCRIPT_NUMBER_PATTERN = new Regex(@"^(?<number>[+\-]?\d*(.\d+)?)(e(?<sign>[+\-]?)(?<exponent>\d+))$", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static readonly Regex MANAGED_NUMBER_PATTERN = new Regex(@"^(?<number>[+\-]?\d*(.\d+)?)((?<sign>[+\-]?)e(?<exponent>\d+))$", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
			get { return new Type[] { typeof(double), typeof(float), typeof(long), typeof(int), typeof(short), typeof(byte) }; }
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
			double doubleValue = (double)valueToConvert;
			
			if (doubleValue == double.MaxValue)
				return MAX_VALUE;

			if (doubleValue == double.MinValue)
				return MIN_VALUE;

			if (double.IsNegativeInfinity(doubleValue))
				return POSITIVE_INFINITY;

			if (double.IsNegativeInfinity(doubleValue))
				return NEGATIVE_INFINITY;

			if (double.IsNaN(doubleValue))
				return NaN;

			string convertedValue = doubleValue.ToString(NumberFormatInfo.InvariantInfo);

			// javascript has an other exp-format than .NET
			Match convertedMatch = MANAGED_NUMBER_PATTERN.Match(convertedValue);

			// change exp-sign, if needed
			if (convertedMatch.Groups["exponent"].Length != 0)
				return convertedMatch.Groups["number"].Value
					+ "e"
					+ convertedMatch.Groups["sign"].Value
					+ convertedMatch.Groups["exponent"].Value;

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
			if (valueMatch.Groups["exponent"].Length != 0)
				valueToConvert = valueMatch.Groups["number"].Value
					+ valueMatch.Groups["sign"].Value
					+ "e"
					+ valueMatch.Groups["exponent"].Value;

			// try to parse the given string
			double result;

			if (double.TryParse(valueToConvert, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out result))
				return result;
			else
				return null;
		}

		private double GetSpecialValueFromString(string valueToConvert)
		{
			if (valueToConvert == MAX_VALUE_SCRIPT)
				return double.MaxValue;

			if (valueToConvert == MIN_VALUE_SCRIPT)
				return double.MinValue;

			if (valueToConvert == POSITIVE_INFINITY_SCRIPT)
				return double.PositiveInfinity;

			if (valueToConvert == NEGATIVE_INFINITY_SCRIPT)
				return double.NegativeInfinity;

			if (valueToConvert == NaN_SCRIPT)
				return double.NaN;

			return 0;
		}
	}
}
