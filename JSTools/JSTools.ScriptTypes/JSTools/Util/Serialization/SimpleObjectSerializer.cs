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
using System.Collections;
using System.Reflection;
using System.Text;

using JSTools.ScriptTypes;

namespace JSTools.Util.Serialization
{
	/// <summary>
	/// Represents an object serializer which serializes/deserialized the
	/// property values and the corresponding property names.
	/// 
	/// The serialized string contains a javascript object, which can
	/// be send to the client browser. To serialize an object hirarchy
	/// you should call the Serialize(...) method. All types which are
	/// derived from IList will be serialized as javascript arrays. All
	/// other complex types will be serialized as javascript objects.
	/// 
	/// To deserialize an object string, you should call the Deserialize(...)
	/// method. All javascript arrays are deserialized into types, which
	/// are derived from IList. If the corresponding type is not derived
	/// from IList, the array values will not be deserilalized. All values
	/// of complex types are serialized into the public properties (requires
	/// a property SETTER). There are two ways to deserialize a javascript
	/// object string:
	/// 
	/// - Deserialize(string, object, bool) method.
	/// You can deserialize the javascript string into an object hirachy.
	/// This is usefull if you have the same object hirachy during
	/// serialization and deserialization. All complex types must be
	/// instantiated in the object hirarchy in order to deserialize all values.
	/// Otherwise some values may be lost. Instances of classes which are
	/// derived from IList and the ReadOnly flag is set to "true" will not be
	/// deserialized.
	/// 
	/// - Deserialize(string, bool) method.
	/// You can deserialize the javascript string into an new object hirachy.
	/// Well, you will obtain JSScriptArray/JSScriptObject objects. This
	/// classes correspond to the javascript array/object types. 
	/// </summary>
	/// <remarks>
	/// Properties which contain a recursion will not be serialized.
	/// </remarks>
	public class SimpleObjectSerializer
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new SimpleObjectSerializer instance.
		/// </summary>
		public SimpleObjectSerializer()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Deserializes the specified string and returns the deserialized
		/// object structure.
		/// </summary>
		/// <param name="toDeserialize">String to deserialize.</param>
		/// <returns>Returns the created object structure.</returns>
		public object Deserialize(string toDeserialize)
		{
			return Deserialize(toDeserialize, true);
		}

		/// <summary>
		/// Deserializes the specified string and returns the deserialized
		/// object structure.
		/// </summary>
		/// <param name="toDeserialize">String to deserialize.</param>
		/// <param name="decodeValues">True if the values contain characters (e.g. %EF) which should be decoded.</param>
		/// <returns>Returns the created object structure.</returns>
		/// <exception cref="ArgumentNullException">The given string contains a null reference.</exception>
		public object Deserialize(string toDeserialize, bool decodeValues)
		{
			if (toDeserialize == null)
				throw new ArgumentNullException("toDeserialize");

			return new Deserializer().Deserialize(toDeserialize, decodeValues);
		}

		/// <summary>
		/// Deserializes the specified string and fills the deserialized values
		/// into the given object structure.
		/// </summary>
		/// <param name="toDeserialize">String to deserialize.</param>
		/// <param name="toFill">Object structure to fill.</param>
		/// <returns>Returns the object structure.</returns>
		/// <exception cref="ArgumentNullException">The given string contains a null reference.</exception>
		public object Deserialize(string toDeserialize, object toFill)
		{
			return Deserialize(toDeserialize, toFill, true);
		}

		/// <summary>
		/// Deserializes the specified string and fills the deserialized values
		/// into the given object structure.
		/// </summary>
		/// <param name="toDeserialize">String to deserialize.</param>
		/// <param name="toFill">Object structure to fill.</param>
		/// <param name="decodeValues">True if the values contain characters (e.g. %EF) which should be decoded.</param>
		/// <returns>Returns the object structure.</returns>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		public object Deserialize(string toDeserialize, object toFill, bool decodeValues)
		{
			if (toDeserialize == null)
				throw new ArgumentNullException("toDeserialize");

			if (toFill == null)
				throw new ArgumentNullException("toFill");

			return new Deserializer().Deserialize(toDeserialize, toFill, decodeValues);
		}

		/// <summary>
		/// Serializes the given object.
		/// </summary>
		/// <param name="toSerialize">Object which should be serialized.</param>
		/// <returns>Returns the corresponding javascript object string.</returns>
		/// <exception cref="ArgumentNullException">The given object contains a null reference.</exception>
		public string Serialize(object toSerialize)
		{
			return Serialize(toSerialize, false);
		}

		/// <summary>
		/// Serializes the given object.
		/// </summary>
		/// <param name="toSerialize">Object which should be serialized.</param>
		/// <param name="decodeValues">True if the values contain characters (e.g. %) which should be encoded.</param>
		/// <returns>Returns the corresponding javascript object string.</returns>
		/// <exception cref="ArgumentNullException">The given object contains a null reference.</exception>
		public string Serialize(object toSerialize, bool encodeValues)
		{
			if (toSerialize == null)
				throw new ArgumentNullException("toSerialize");

			return new Serializer().Serialize(toSerialize, encodeValues);
		}
	}
}
