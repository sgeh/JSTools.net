namespace("JSTools.Web");


/// <class>
/// Loads a file script at html document design-time. Represents the using directive.
/// </class>
JSTools.Web.ScriptLoaderEngine = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Web.ScriptLoaderEngine");

	var _this = this;


	/// <property type="String">
	/// Sepcifies the script type (javascript / vbscript)
	/// </property>
	this.ScriptLanguage = "JavaScript";


	/// <property type="String">
	/// Web location of the script files. The param given by the LoadFile() method will
	/// be replaced with the {0} mark.
	/// </property>
	this.ScriptFileLocation = String.Empty;


	/// <property type="Boolean">
	/// Gets/sets if the requested file location replacement should be encoded. This
	/// may be useful, if the replacement mark {0} is placed in a query string request.
	/// </property>
	this.EncodeFileLocation = false;


	/// <property type="Number">
	/// Represents the script version, which should be used to import the script.
	/// </property>
	this.ScriptVersion = 1.3;


	/// <property type="Number">
	/// Represents the script version, which should be used to import the script (e.g. ".js").
	/// </property>
	this.ScriptExtension = ".js";


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Initializes the string and creates a new java script tag which will
	/// be written into the document.
	/// </method>
	/// <param name="strFileLocation" type="String">The script file source (e.g. "/JSTools/Web.js").</param>
	this.LoadFile = function(strFileLocation)
	{
		if (typeof(strFileLocation) != 'string' || strFileLocation == String.Empty)
			return;

		if (!strFileLocation.EndsWith(_this.ScriptExtension))
		{
			strFileLocation += _this.ScriptExtension;
		}

		if (_this.EncodeFileLocation)
		{
			strFileLocation = escape(strFileLocation);
		}

		var filePattern = String(_this.ScriptFileLocation);
		
		if (filePattern.indexOf("{0}") != -1)
		{
			WriteScriptTag(String.Format(filePattern, strFileLocation));
		}
		else
		{
			WriteScriptTag(strFileLocation);
		}
	}
	
	
	/// <method>
	/// Writes a new &lt;script&gt; tag with the specified location.
	/// </method>
	/// <param name="strLocation" type="String">Location to write.</param>
	function WriteScriptTag(strLocation)
	{
		var scriptVersion = (_this.ScriptVersion) ? _this.ScriptVersion : String.Empty;

		window.document.write('<SCRIPT LANGUAGE="'
			+ _this.ScriptLanguage + scriptVersion
			+ '" TYPE="text/' + String(_this.ScriptLanguage).toLowerCase()
			+ '" SRC="' + strLocation
			+ '"><\/SCRIPT>');
	}
}


/// <property type="JSTools.Web.ScriptLoaderEngine">
/// Default ScriptLoader instance.
/// </property>
JSTools.ScriptLoader = new JSTools.Web.ScriptLoaderEngine();
