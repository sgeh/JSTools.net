document.write("<h1>TEST SIMPLEOBJECTSERIALZER</h1>");


var helperRefObj = { 
	aField: 3
}

var objectToSerialize =
{
	__dummy: true,
	a3: 3,
	b6: /ds/gi,
	c0: new Object(),
	d5: new RegExp("[a-z]?", "g"),
	d3i: " \" ||tes t",
	e0$: ' \' \t testy',
	_d3e: false,
	ar: [ objectToSerialize, 38, 0x38, 'string', [ 0, 2, 4 ], new Object(), helperRefObj ],
	ef: helperRefObj
};

// objectToSerialize will not be serialized, it represents a recursion
var serializer = new JSTools.Util.SimpleObjectSerializer();
var serializedObject = serializer.SerializeObject(objectToSerialize);

document.write("<p>serialization output: <br>" + serializedObject + "</p>");

var deserializedObject = serializer.Deserialize(serializedObject);

if (objectToSerialize.a3 != deserializedObject.a3)
	document.write("<p>Deserialization/Serialization failed -> a3 != a3 ('" + objectToSerialize.a3 + "' != '" + deserializedObject.a3 + "')</p>");

if (objectToSerialize.d3i != deserializedObject.d3i)
	document.write("<p>Deserialization/Serialization failed -> d3i != d3i ('" + objectToSerialize.d3i + "' != '" + deserializedObject.d3i + "')</p>");

if (objectToSerialize.e0$ != deserializedObject.e0$)
	document.write("<p>Deserialization/Serialization failed -> e0$ != e0$ ('" + objectToSerialize.e0$ + "' != '" + deserializedObject.e0$ + "')</p>");

if (objectToSerialize.ar[2] != deserializedObject.ar[1])
	document.write("<p>Deserialization/Serialization failed -> ar[2] != ar[1] ('" + objectToSerialize.ar[2] + "' != '" + deserializedObject.ar[1] + "')</p>");

if (objectToSerialize.ar[4][0] != deserializedObject.ar[3][0])
	document.write("<p>Deserialization/Serialization failed -> ar[4][0] != ar[3][0] ('" + objectToSerialize.ar[4][0] + "' != '" + deserializedObject.ar[3][0] + "')</p>");

if (!objectToSerialize.ef)
	document.write("<p>Deserialization/Serialization failed -> ef must contain an instance ('" + ef + "')</p>");

if (deserializedObject.__dummy)
	document.write("<p>Deserialization/Serialization failed -> __dummy must be empty (it's a hidden public field)</p>");

document.write("<p>test done</p>");