// Following classes are implement at startup:
//
// - JSTools.Web.Browser.BrowserInfo
//
// They must not use the using procedure.


// init default namespace
namespace("JSTools");


/// <procedure>
/// Loads the given NameSpace. The given NameSpace will be separated
/// by the "." character. This characters will be replaced with a
/// slash "/" and a request will be send to the server. If you specify
/// a NameSpace like "JSTools.Web.Data", the file "/JSTools/Web/Data.js"
/// will be used.
///
/// The given NameSpace can contain browser identification patterns.
/// They will be replaced as follows:
///
/// {I} Browser Identification
///  IE - Internet Explorer 4.0
///  NS - Netscape Navigator 4.x
///  DOM - Document Object Model Browsers
///   Internet Explorer 5.x +
///   Netscape Navigator 6.x +
///  OP
///   Opera 5.x, 6.x
///  NO - Unknown Browser
///
/// {G} Browser Generation
///  4 - 4. Browser Generation
///   Netscape 4.x
///   Internet Explorer 4.0
///   Opera 5.x / 6.x
///  5 - 5. Browser Generation
///   Netscape 6.x +
///   Internet Explorer 5.x +
///   Opera 7.x
///  0 - Unknowkn Browser
/// </procedure>
function using(strNameSpaceToLoad)
{
	if (NameSpaceManager.Instance.IsNameSpaceLoaded(strNameSpaceToLoad))
		return;
	
	var generation = (document.getElementById && document.createElement) ? 5 : 4;
	var browser = "";

// TODO: add supported browsers here.....	

	JSTools.ScriptLoader.LoadFile(strNameSpaceToLoad);
}


/// <property type="Array">
/// Contains elements, which are used to describe the browser
/// 
/// </property>
using.SupportedBrowsers = [
	// Netscape 4.x
	{
		Name: "NS",
		Generation: 4,
		Enabled: Boolean(document.layers)
	},
	// Internex Explorer 4.+
	{
		Name: "IE",
		Generation: 4,
		Enabled: Boolean(document.all)
	},
	// DOM Browsers
	{
		Name: "DOM",
		Generation: 5,
		Enabled: Boolean(document.documentElement)
	},
	// Opera Browsers (not DOM compatible)
	{
		Name: "OP",
		Generation: 4,
		Enabled: (navigator.userAgent.toLowerCase().indexOf("opera") != -1)
	}
];


/// <procedure>
/// Creates the given NameSpace object.
/// </procedure>
/// <param name="strNameSpace" type="String">NameSpace to create, separated by '.'.</param>
function namespace(strNameSpace)
{
	if (NameSpaceManager.Instance.IsNameSpaceLoaded(strNameSpace))
		return;

	NameSpaceManager.Instance.Add(strNameSpace);
}