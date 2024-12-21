namespace("JSTools.Web");


/// <class>
/// Loads a file script at html document design-time. Represents the using directive.
/// </class>
JSTools.Web.ScriptLoaderEngine = function()
{
	//------------------------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------------------------

	var _this = this;

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
	this.ScriptVersion = 1.3


	//------------------------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------------------------


	//------------------------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------------------------

	/// <method>
	/// Initilizes the string and creates a new java script tag which will
	/// be written into the document.
	/// </method>
	/// <param name="strFileLocation" type="String">The script file source.</param>
	this.LoadFile = function(strFileLocation)
	{
		var scriptFileLocation = String(_this.ScriptFileLocation);

		if (!scriptFileLocation || scriptFileLocation.indexOf("{0}") == -1)
			return;

		var scriptLocation = String.Format(scriptFileLocation, strFileLocation);
		var scriptVersion = (_this.ScriptVersion) ? _this.ScriptVersion : String.Empty;

		window.document.write('<script ' +
			'language="JavaScript' + scriptVersion + '" ' +
			'type="text/javascript" ' +
			'src="' + (_this.EncodeFileLocation ? escape(scriptLocation) : scriptLocation) + '"></script>"');
	}
}


/// <property type="JSTools.Web.ScriptLoaderEngine">
/// Default ScriptLoader instance.
/// </property>
JSTools.ScriptLoader = new JSTools.ScriptLoader();