/*
 * JSTools.Test.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
using System.Collections;
using System.IO;

using JSTools;
using JSTools.Util.Serialization;
using JSTools.Test.Resources;

using NUnit.Framework;

namespace JSTools.Test.Util.Serialization
{
	/// <summary>
	/// Summary description for Cruncher.
	/// </summary>
	[TestFixture]
	public class Serialization
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new test instance.
		/// </summary>
		public Serialization()
		{
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Initialize this test instance.
		/// </summary>
		[SetUp()]
		public void SetUp()
		{
		}

		/// <summary>
		/// Clear up this test instance.
		/// </summary>
		[TearDown()]
		public void TearDown()
		{
		}

		[Test()]
		public void Serialize()
		{
			SerializationTest testObject = GetObjectToSerialize();
			SimpleObjectSerializer serializer = new SimpleObjectSerializer();
			string serialized = serializer.Serialize(testObject, false);
			string serializedEnc = serializer.Serialize(testObject, true);

			Assert.IsNotNull(serialized);
			Assert.IsTrue(serialized.Length > 0);
			System.Console.WriteLine(serialized);

			Assert.IsNotNull(serializedEnc);
			Assert.IsTrue(serializedEnc.Length > 0);
			System.Console.WriteLine(serializedEnc);
		}

		[Test()]
		public void DeserializeJSObject()
		{
			SerializationTest testObject = GetObjectToSerialize();
			SimpleObjectSerializer serializer = new SimpleObjectSerializer();
			string serialized = serializer.Serialize(testObject, false);
			CheckObject(serializer.Deserialize(serialized, true));
		}

		[Test()]
		public void DeserializeJSObjectWithEncoding()
		{
			SerializationTest testObject = GetObjectToSerialize();
			SimpleObjectSerializer serializer = new SimpleObjectSerializer();
			string serializedEnc = serializer.Serialize(testObject, true);
			CheckObject(serializer.Deserialize(serializedEnc, true));
		}

		[Test()]
		public void DeserializeCustomObject()
		{
			SerializationTest testObject = GetObjectToSerialize();
			SimpleObjectSerializer serializer = new SimpleObjectSerializer();

			SerializationTest deSerializedTestObj = new SerializationTest();
			deSerializedTestObj.Matrix.Add(null);
			deSerializedTestObj.Matrix.Add(null);
			deSerializedTestObj.Matrix.Add(new SerializationTest());

			string serialized = serializer.Serialize(testObject, false);
			serializer.Deserialize(serialized, deSerializedTestObj, true);

			CheckObject(deSerializedTestObj);
		}

		[Test()]
		public void DeserializeCustomObjectWithEncoding()
		{
			SerializationTest testObject = GetObjectToSerialize();
			SimpleObjectSerializer serializer = new SimpleObjectSerializer();

			SerializationTest deSerializedTestObjEnc = new SerializationTest();
			deSerializedTestObjEnc.Matrix.Add(null);
			deSerializedTestObjEnc.Matrix.Add(null);
			deSerializedTestObjEnc.Matrix.Add(new SerializationTest());

			string serializedEnc = serializer.Serialize(testObject, true);
			serializer.Deserialize(serializedEnc, deSerializedTestObjEnc, true);

			CheckObject(deSerializedTestObjEnc);
		}

		private void CheckObject(object deserializedObj)
		{
			Assert.IsTrue(deserializedObj is JSScriptObject);
			Assert.IsTrue(((JSScriptObject)deserializedObj)["Matrix"] is JSScriptArray);
			Assert.AreEqual(Convert.ToDouble(Decimal.MaxValue), ((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[0]);
			Assert.AreEqual(Convert.ToDouble(Decimal.MinValue), ((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[1]);
			Assert.IsNotNull(((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[2]);
			Assert.AreEqual(Double.MaxValue, ((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[3]);
			Assert.AreEqual(Double.MinValue, ((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[4]);
			Assert.AreEqual(Double.NaN, ((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[5]);
			Assert.AreEqual(Double.NegativeInfinity, ((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[6]);
			Assert.AreEqual(Double.PositiveInfinity, ((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[7]);
			Assert.IsNull(((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[8]);
			Assert.AreEqual(43.3, ((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[9]);
			Assert.IsNull(((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[10]);
			Assert.AreEqual("-", ((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[11]);
			Assert.AreEqual("+.", ((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[12]);
			Assert.IsNull(((JSScriptArray)((JSScriptObject)deserializedObj)["Matrix"])[13]);
			Assert.AreEqual("\\s\r\n \" \tes\\t \\", ((JSScriptObject)deserializedObj)["Name"]);
			Assert.AreEqual(Double.MaxValue - Double.MinValue, ((JSScriptObject)deserializedObj)["Number"]);
			Assert.IsTrue(((JSScriptObject)deserializedObj)["NestedObject"] is JSScriptObject);
			Assert.AreEqual(string.Empty, ((JSScriptObject)(((JSScriptObject)deserializedObj)["NestedObject"]))["Name"]);
			Assert.AreEqual(-1, ((JSScriptObject)(((JSScriptObject)deserializedObj)["NestedObject"]))["Number"]);
		}

		private void CheckObject(SerializationTest deSerializedTestObj)
		{
			Assert.AreEqual(Convert.ToDouble(Decimal.MaxValue), deSerializedTestObj.Matrix[0]);
			Assert.AreEqual(Convert.ToDouble(Decimal.MinValue), deSerializedTestObj.Matrix[1]);
			Assert.IsNotNull(deSerializedTestObj.Matrix[2]);
			Assert.AreEqual(Double.MaxValue, deSerializedTestObj.Matrix[3]);
			Assert.AreEqual(Double.MinValue, deSerializedTestObj.Matrix[4]);
			Assert.AreEqual(Double.NaN, deSerializedTestObj.Matrix[5]);
			Assert.AreEqual(Double.NegativeInfinity, deSerializedTestObj.Matrix[6]);
			Assert.AreEqual(Double.PositiveInfinity, deSerializedTestObj.Matrix[7]);
			Assert.IsNull(deSerializedTestObj.Matrix[8]);
			Assert.AreEqual(43.3, deSerializedTestObj.Matrix[9]);
			Assert.IsNull(deSerializedTestObj.Matrix[10]);
			Assert.AreEqual("-", deSerializedTestObj.Matrix[11]);
			Assert.AreEqual("+.", deSerializedTestObj.Matrix[12]);
			Assert.IsNull(deSerializedTestObj.Matrix[13]);
			Assert.AreEqual("\\s\r\n \" \tes\\t \\", deSerializedTestObj.Name);
			Assert.AreEqual(Double.MaxValue - Double.MinValue, deSerializedTestObj.Number);
			Assert.AreEqual(string.Empty, deSerializedTestObj.NestedObject.Name);
			Assert.AreEqual(-1, deSerializedTestObj.NestedObject.Number);
		}

		private SerializationTest GetObjectToSerialize()
		{
			SerializationTest testObject = new SerializationTest();
			
			SerializationTest nestedTest = new SerializationTest();
			nestedTest.Name = "nestedTest";
			nestedTest.Number = 27.34;

			testObject.Matrix.Add(Decimal.MaxValue);
			testObject.Matrix.Add(Decimal.MinValue);
			testObject.Matrix.Add(nestedTest);
			testObject.Matrix.Add(Double.MaxValue);
			testObject.Matrix.Add(Double.MinValue);
			testObject.Matrix.Add(Double.NaN);
			testObject.Matrix.Add(Double.NegativeInfinity);
			testObject.Matrix.Add(Double.PositiveInfinity);
			testObject.Matrix.Add(null);
			testObject.Matrix.Add(43.3F);
			testObject.Matrix.Add(null);
			testObject.Matrix.Add("-");
			testObject.Matrix.Add("+.");
			testObject.Matrix.Add(null);
			testObject.Name = "\\s\r\n \" \tes\\t \\";
			testObject.Number = (Double.MaxValue - Double.MinValue);
			return testObject;
		}

		/// <summary>
		/// Represents the serialization test class.
		/// </summary>
		public class SerializationTest
		{
			//--------------------------------------------------------------------
			// Declarations
			//--------------------------------------------------------------------

			private string _name = string.Empty;
			private double _number = -1;
			private ArrayList _matrix = new ArrayList();

			//--------------------------------------------------------------------
			// Properties
			//--------------------------------------------------------------------

			[ScriptValueType()]
			public string Name
			{
				get { return _name; }
				set { _name = (value != null) ? value : string.Empty; }
			}

			[ScriptValueType()]
			public double Number
			{
				get { return _number; }
				set { _number = value; }
			}

			[ScriptValueType()]
			public object NullObject
			{
				get { return null; }
			}

			[ScriptValueType()]
			public object EmptyObject
			{
				get { return new object(); }
			}

			[ScriptValueType()]
			public IList Matrix
			{
				get { return _matrix; }
			}

			[ScriptValueType()]
			public InnerSerializationTest NestedObject
			{
				get { return new InnerSerializationTest(); }
			}

			[ScriptValueType()]
			public SerializationTest InvalidRecursion
			{
				get { return this; }
			}

			public string DoNotSerialize
			{
				get { throw new InvalidOperationException("Do not serialize this property."); }
				set { throw new InvalidOperationException("Do not deserialize this property."); }
			}

			//--------------------------------------------------------------------
			// Constructors / Destructor
			//--------------------------------------------------------------------
		}


		/// <summary>
		/// Represents the inner serialization test class.
		/// </summary>
		public class InnerSerializationTest
		{
			//--------------------------------------------------------------------
			// Declarations
			//--------------------------------------------------------------------

			private string _name = string.Empty;
			private double _number = -1;

			//--------------------------------------------------------------------
			// Properties
			//--------------------------------------------------------------------

			[ScriptValueType()]
			public string Name
			{
				get { return _name; }
				set { _name = (value != null) ? value : string.Empty; }
			}

			[ScriptValueType()]
			public double Number
			{
				get { return _number; }
				set { _number = value; }
			}

			[ScriptValueType()]
			public object NullObject
			{
				get { return null; }
			}

			//--------------------------------------------------------------------
			// Constructors / Destructor
			//--------------------------------------------------------------------
		}
	}
}
