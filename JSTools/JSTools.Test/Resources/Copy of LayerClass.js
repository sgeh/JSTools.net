<!--
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
// Ab Netscape 4.01, InternetExplorer 4.07 SP2 und Netscape 6.0
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
// Diese Klasse dient zum Ansprechen der Layer unter den ver-
// schiedenen Browsern. Genaeuere Zusammenhaenge koennen im
// Klassendiagramm (layerObject.vsd) nachgelesen werden.
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//
//
//
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
// Benoetigte Includes:
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
// javascript.easytools.ch/browserClass.js
// javascript.easytools.ch/modelClass.js
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//
//
//
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
// Instanzierung:
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
// document.createLayer(strDivID);
//
// Damit wird der Layer instanziert. Mit dem Parameter
// strDivID wird die Id im <div> oder <span> Tag angegeben.
// Das instanzierte Objekt wird anschliessend unter
// document.layer[strDivID] abgelegt. Falls noch kein Layer
// vorhanden war, wird dieser mit JavaScript geschrieben.
//
// Dieses Tool wurde mit den Layer-Styles "position:absolute"
// und "visibility:[hiddden|visible]" getestet. Alle anderen
// Attribute z.B. "position:[fixed|relative|static]" oder
// "visibility:[show|hide]" koennten Fehler oder andere unge-
// wollte Effekte verursachen.
//
//
// Dieses Beispiel soll helfen, Fehler zu vermeiden:
//
// <!-- Benoetigte Includes -->
// <script language="javascript" src="http://javascript.easytools.ch/browserClass.js"></script>
// <script language="javascript" src="http://javascript.easytools.ch/modelClass.js"></script>
// <script language="javascript" src="http://javascript.easytools.ch/layerClass.js"></script>
//
// <!-- Layer Definition -->
// <div id="helloLayer" style="position:absolute; visibility:visible; top:45; left:90; height:15; width:15;">
// 	<font>Hello World Layer</font>
// </div>
//
// <!-- Layer Instanzierung und Verarbeitung -->
// <script language="JavaScript">
// 	document.createLayer("helloLayer");
// 	document.layer.helloLayer.hide();
//
// 	with(document.layer.helloLayer) {
// 		show();
// 		zIndex = 56;
// 		var layerWidth = width;
// 	}
// </script>
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//
//
//
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//	Members:
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//			Eigenschaft:			Rueckgabe:					Beschreibung:
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//			id						String						String, der beim Instanzieren durch strDivID uebergeben wurde.
//			height					Integer						Liest die Hoehe des Layers aus.
//			width					Integer						Liest die Breite des Layers aus.
//			top						Integer						Liest den Abstand zum oberen Rand Fensters aus.
//			left					Integer						Liest den Abstand zum linken Rand Fensters aus.
//			visibility				String						Liest die Sichtbarkeit des Layers aus. (visible/hidden)
//			innerHTML				String						Liest den HTML-Inhalt des Layers aus. Enthaelt nach dem Anwenden der Funktion setInnerHTML() einen gueltigen Inhalt.
//			images[]				Objekt						Liest die Bilder des Layers in einem Array aus.
//			clip[]					Objekt						Liest die Clip-Werte aus des Layers aus. Wird benoetigt, wenn die Clip-Werte geaendert werden sollen.
//				.top				Integer						Liest den ersten der vier Clip-Werte aus. Gemessen wird der Wert von oben an der oberen Elementgrenze.
//				.right				Integer						Liest den zweiten der vier Clip-Werte aus. Gemessen wird der Wert von links an der rechten Elementgrenze.
//				.bottom				Integer						Liest den dritten der vier Clip-Werte aus. Gemessen wird der Wert von oben an der unteren Elementgrenze.
//				.left				Integer						Liest den letzten der vier Clip-Werte aus. Gemessen wird der Wert von links an der linken Elementgrenze.
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//
//
//
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//			Methode:					Rueckgabe:					Parameter:				Beschreibung:
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//			setHeight()				void							intHeight				Schreibt die Hoehe des Layers.
//			setWidth()				void							intWidth				Schreibt die Breite des Layers.
//			hide()					void							void					Versteckt den Layer.
//			show()					void							void					Laesst den Layer erscheinen.
//			setZIndex()				void							intZIndex				Schreibt den z-Index des Layers.
//			setClip()				void							[object Clip]			Schreibt den Clip des Layers. Erwartet ein Clip-Objekt als Parameter.
//			setInnerHTML()			void							strContent				Schreibt den Inhalt des Layers.
//			setTopMargin()			void							intTopPixel				Schreibt den Abstand des Layers zum oberen Rand des Fensters.
//			setLeftMargin()			void							intLeftPixel			Schreibt den Abstand des Layers zum linken Rand des Fensters.
//			moveTo()				void							intTop,intLeft			Verschiebt den Layer zu den angegebenen Positionen (intTop/intLeft).
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//
// by Silvan G.
//-->

// Instanziert eine neue Layer - Klasse
function m_createLayer(strDivId) {
	if(!document.errorAlert) {
		var thisLayerId;

		if(thisBrowser.generation >= 4) {
			thisLayerId = strDivId;
		}

		if(isNaN(thisLayerId) && !isEmpty(thisLayerId)) {
			strDivId = String(strDivId);
			document.layer[strDivId] = new CrossLayer(strDivId);
			document.layer[document.layer.length] = document.layer[strDivId];
		}
		else {
			alert("Sorry, your Browser is too old or the identification is invalid. Please download a new version of " + thisBrowser.name +".");
			document.errorAlert = true;
		}
	}
}

document.errorAlert		= false;
document.createLayer	= m_createLayer;
document.layer			= new Array();

document.getImageNames			= new Array();
document.getImageNames.count	= 0;
document.getImageNames.loaded	= false;


/**
 * Kreiert einen neuen Layer nach dem OnLoad event.
 */
/*void*/ document.createRunTimeLayer = function(strLayerId, strLayerValue)
{
	if (thisBrowser.typ.isIE && !thisBrowser.dom)
	{
		document.body.insertAdjacentHTML("BeforeEnd", '<div id="' + strLayerId + '" style="position:absolute; visibility:hidden; top:0px; left:0px;">' + strLayerValue + '</div>');
	}
	if (thisBrowser.typ.isNC && document.layers)
	{
		var layer = new Layer();
		layer.name = strLayerId;
		layer.id = strLayerId;
		layer.visibility = "hide";
		layer.top = 0;
		layer.left = 0;

		with (layer.document)
		{
			open();
			writeln(strLayerValue);
			close();
		}
		document.layers[strLayerId] = layer;
	}
	if (thisBrowser.dom)
	{
		var element = document.createElement("div");
		element.id = strLayerId;
		element.innerHTML = strLayerValue;

		with (element.style)
		{
			position = "absolute";
			visibility = "hidden";
			top = "0px";
			left = "0px";
		}
		document.body.appendChild(element);
	}

	document.createLayer(strLayerId);
}


// gibt Bilder mit ID oder NAME zurueck
function getDocumentImageNames() {
	var thisImageId		= "";
	var thisImageCount	= 0;

	for(myImages=0; myImages<document.images.length; myImages++) {
		thisImageId = "";

		if(document.images[myImages].name != "") {
			thisImageId = document.images[myImages].name;
		}
		else if(document.images[myImages].id != "") {
			thisImageId = document.images[myImages].id;
		}

		if(thisImageId != "") {
			document.getImageNames[thisImageCount] = thisImageId;
			thisImageCount++;
		}
	}
	document.getImageNames.count = document.images.length;
	document.getImageNames.loaded = true;
}


//---Begin Class Layer---

// Layer - Klasse
function CrossLayer(strLayerId) {
	this.loaded			= false;
	this.id				= strLayerId;
	this.refresh		= m_refresh;
	this.innerHTML		= "[nothing]";
	this.browserObj	= new ObjectModelById(this.id);

	if(!this.browserObj.id) {
		this.browserObj = m_writeLayer(strLayerId);
	}

	if(this.browserObj.id) {
		this.refresh();
		this.visibility		= this.browserObj.getLayerStyles('visibility');
		this.clip				= this.browserObj.getLayerStyles('clip');
		this.zIndex				= this.browserObj.getLayerStyles('zIndex');

		this.setHeight			= m_setHeight;
		this.setWidth			= m_setWidth;
		this.hide				= m_hide;
		this.show				= m_show;
		this.setZIndex			= m_setZIndex;
		this.setClip			= m_setClip;
		this.setInnerHTML		= m_setInnerHTML;
		this.setLeftMargin		= m_setLeftMargin;
		this.setTopMargin		= m_setTopMargin;
		this.moveTo				= m_moveTo;
	}
	this.loaded	= true;
}

// schreibt Layer, falls nicht vorhanden
function m_writeLayer(strDivId) {
	//document.write("<span id=\"" + strDivId + "\" style=\"position:absolute; visibility:hidden;\"></span>");
	//return new ObjectModelById(strDivId);

	alert("A LayerNotFoundException has occured.\n\nCould not find a layer with the specified ID \"" + strDivId + "\"! The script may corrupt.");
	return null;
}

// refresht die Werte in der Klasse
function m_refresh() {
	this.images				= this.browserObj.getLayerStyles('images');
	this.height				= this.browserObj.getLayerStyles('height');
	this.width				= this.browserObj.getLayerStyles('width');
	this.top					= this.browserObj.getLayerStyles('top');
	this.left				= this.browserObj.getLayerStyles('left');
}

// setzt Hoehe des Layers
function m_setHeight(intHeight) {
	if(this.loaded && !isNaN(intHeight)) {
		this.height = this.browserObj.setLayerStyles('height',intHeight);

		this.clip.bottom = intHeight;
		this.clip = this.browserObj.setLayerStyles('clip',this.clip);
	}
}

// setzt Breite des Layers
function m_setWidth(intWidth) {
	if(this.loaded && !isNaN(intWidth)) {
		this.width = this.browserObj.setLayerStyles('width',intWidth);

		this.clip.right = intWidth;
		this.clip = this.browserObj.setLayerStyles('clip',this.clip);
	}
}

// versteckt den Layer
function m_hide() {
	if(this.loaded) {
		this.visibility = this.browserObj.setLayerStyles('visibility','hidden');
	}
}

// laesst den Layer erscheinen
function m_show() {
	if(this.loaded) {
		this.visibility = this.browserObj.setLayerStyles('visibility','visible');
	}
}

// setzt zIndex des Layers
function m_setZIndex(intZIndex) {
	if(this.loaded && !isNaN(intZIndex)) {
		this.zIndex = this.browserObj.setLayerStyles('zIndex',intZIndex);
	}
}

// setzt Clip des Layers
function m_setClip(objRect) {
	if(this.loaded && objRect == "[object Rect]") {
		this.clip = this.browserObj.setLayerStyles('clip',objRect);
	}
}

// setzt Inhalt des Layers
function m_setInnerHTML(strContent) {
	if(this.loaded) {
		while(strContent.search(/\"/) != -1) {
			strContent = strContent.replace(/\"/,"'");
		}
		this.innerHTML = this.browserObj.setLayerStyles('innerHTML',String(strContent));
		this.refresh();
	}
}

// setzt den Abstand vom oberen Fensterrand zum Layer
function m_setTopMargin(intTopPixel) {
	if(this.loaded && !isNaN(intTopPixel)) {
		this.top = this.browserObj.setLayerStyles('top',intTopPixel);
	}
}

// setzt den Abstand vom linken Fensterrand zum Layer
function m_setLeftMargin(intLeftPixel) {
	if(this.loaded && !isNaN(intLeftPixel)) {
		this.left = this.browserObj.setLayerStyles('left',intLeftPixel);
	}
}

// verschiebt den Layer
function m_moveTo(intTop,intLeft) {
	this.setTopMargin(intTop);
	this.setLeftMargin(intLeft);
}

//---End Class Layer- -->
