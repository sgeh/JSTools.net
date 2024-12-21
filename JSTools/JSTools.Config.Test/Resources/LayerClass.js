<!--
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
// Ab Netscape 4.0, InternetExplorer 4.0 und Netscape 6.0
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
// Die Model Klasse verwaltet das Ansprechen von Layern, je
// nach Browser-Objekt.
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//
//
//
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
// Benuetigte Includes:
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
// javascript.easytools.ch/browserClass.js
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//
//
//
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//	Instanzierung:
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//	var thisModel = new ObjectModelById(strLayerId);
//
//	Bei der Instanzierung dieses Objektes muss die ID des <div>
// oder <span> Tags schonbereits angegeben werden. Falls der Layer
// noch nicht existiert, wird dieser mit JavaScript geschrieben.
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
//			id						String						String, der beim Instanzieren durch strLayerId uebergeben wurde.
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//
//
//
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//			Methode:					Rueckgabe:					Parameter:				Beschreibung:
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//			getStyles()				Variant						strStyle					Gibt den Style-Wert zurueck, der durch strStyle angefordert wurde.
//			setStyles()				Variant						strStyle,varValues			Schreibt den Style, der durch strStyle angegeben wurde und gibt den geschriebenen Wert zurueck.
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//
// by Silvan G.
//-->


//---Begin Tools---

// hat der Parameter den Wert undefined, gibt die Funktion den Wert true zurueck
function isEmpty(undefinedExpression) {
	if(String(undefinedExpression).toLowerCase() == 'undefined') {
		return true;
	}
	return false;
}

// hat der Parameter den Wert '', gibt die Funktion den Wert true zurueck
function isVoid(strExpression) {
	if(String(strExpression) == '') {
		return true;
	}
	return false;
}

// hat der Parameter den Wert null, gibt die Funktion den Wert true zurueck
function isNull(nullExpression) {
	if(String(nullExpression).toLowerCase() == 'null') {
		return true;
	}
	return false;
}
//---End Tools---



//---Begin Class Object Model---

var thisBrowser = new BrowserInfo();
var myClipNames = new Array("top","right","bottom","left");
var timeOutSequ = 200;

// liest Layer-ObjektModel aus
function ObjectModelById(strLayerId) {
	this.getLayerStyles = false;
	this.setLayerStyles = false;
	this.id = strLayerId;
	var ReturnElement = false;

	if(thisBrowser.generation >= 4) {
		if(thisBrowser.dom) {
			ReturnElement = document.getElementById(strLayerId);
		}
		else {
			if(thisBrowser.typ.isIE) {
				ReturnElement = document.all[strLayerId];
			}
			else if(thisBrowser.typ.isNC) {
				ReturnElement = document.layers[strLayerId];
			}
		}
	}
	this.ObjectModel = ReturnElement;

	if(!isEmpty(ReturnElement) && String(ReturnElement) != 'false' && !isNull(ReturnElement)) {
		this.getLayerStyles = getStyles;
		this.setLayerStyles = setStyles;
	}
	else {
		this.id = false;
	}
}


// gibt Layer-Eigenschaften zurueck
function getStyles(strStyle) {
	var ReturnElement = false;

	if(thisBrowser.dom || thisBrowser.typ.isIE) {
		switch(strStyle) {
			case 'clip':
				ReturnElement = new Array("[object Rect]");
				var myClip = String(this.ObjectModel.style[strStyle]).toLowerCase();

				if(!isVoid(myClip) && myClip != "auto" && myClip != "rect()") {
					myClip = myClip.replace(/rect\(/,"");

					while(myClip.search(/[px]|[\)]/) != -1) {
						myClip = myClip.replace(/[px]|[\)]/,"");
					}
					myClip = myClip.split(" ");

					for(clipName=0; clipName<myClipNames.length; clipName++) {
						myClip[clipName] = (myClip[clipName] == 'auto' || isNaN(myClip[clipName]))?0:parseInt(myClip[clipName]);
						ReturnElement[myClipNames[clipName]] = myClip[clipName];
					}
				}
				else {
					ReturnElement.top = 0;
					ReturnElement.left = 0;
					ReturnElement.right = parseInt(this.ObjectModel.offsetWidth);
					ReturnElement.bottom = parseInt(this.ObjectModel.offsetHeight);
				}
				break;

			case 'images':
				if(document.getImageNames.loaded) {
					ReturnElement		= new Array();
					var myLayerContent	= String(this.ObjectModel.innerHTML);

					if(isEmpty(myLayerContent)) {
						myLayerContent = String(this.ObjectModel.text);
					}

					var myLayerCount	= 0;
					var pattern,imgId;

					for(myPic=0; myPic<document.getImageNames.length; myPic++) {
						imgId	= document.getImageNames[myPic];
						pattern = eval("/\<img[^<|^>]{3,}\=.{0,1}" + imgId + "[^<|^>]*\>/i");

						if(myLayerContent.search(pattern) != -1) {
							ReturnElement[myLayerCount] = document.images[imgId];
							ReturnElement[imgId] = document.images[imgId];
							myLayerCount++;
						}
					}
				}
				break;

			case 'width':
				WidthClip = this.getLayerStyles('clip');
				if(!isEmpty(WidthClip.right)) {
					ReturnElement = WidthClip.right - WidthClip.left;
					if(ReturnElement < 0 || isNaN(ReturnElement)) {
						ReturnElement = 0;
					}
					break;
				}

			case 'height':
				HeightClip = this.getLayerStyles('clip');

				if(!isEmpty(HeightClip.bottom)) {
					ReturnElement = HeightClip.bottom - HeightClip.top;
					if(ReturnElement < 0 || isNaN(ReturnElement)) {
						ReturnElement = 0;
					}
					break;
				}

			default:
				if(strStyle == "top" || strStyle == "left" || strStyle == "height" || strStyle == "width") {
					var FirstValue = strStyle.substring(0,1).toUpperCase();
					strStyle = FirstValue + strStyle.substring(1,strStyle.length);
					ReturnElement = this.ObjectModel["offset" + strStyle];
				}
				else {
					ReturnElement = this.ObjectModel.style[strStyle];
				}
		}
	}
	else if(thisBrowser.typ.isNC) {
		switch(strStyle) {
			case 'visibility':
				if(this.ObjectModel[strStyle] == 'show') {
					ReturnElement = 'visible';
				}
				else {
					ReturnElement = 'hidden';
				}
				break;

			case 'left':
				ReturnElement = this.ObjectModel.pageX;
				break;

			case 'top':
				ReturnElement = this.ObjectModel.pageY;
				break;

			case 'images':
				ReturnElement = this.ObjectModel.document.images;
				break;

			default:
				if(strStyle == 'height' || strStyle == 'width') {
					ReturnElement = this.ObjectModel.clip[strStyle];
				}
				else {
					ReturnElement = this.ObjectModel[strStyle];
				}
		}
	}

	var writeElement = (!isEmpty(ReturnElement) && !isVoid(ReturnElement) && String(ReturnElement) != 'false')?ReturnElement:'';
	return ctrlLayerStyles(this.ObjectModel.id,strStyle,writeElement);
}

// schreibt neue Layer-Informationen
function setStyles(strStyle,varValues) {
	if(thisBrowser.dom || thisBrowser.typ.isIE) {
		switch(strStyle) {
			case 'clip':
				var myClip = "rect(";
				for(clip=0; clip<myClipNames.length; clip++) {
					var mySplit = (clip+1 != myClipNames.length)?"px ":")";
					myClip += varValues[myClipNames[clip]] + mySplit;
				}
				this.ObjectModel.style[strStyle] = myClip;
				break;

			case 'innerHTML':
				if(thisBrowser.dom) {
					window.setTimeout("document.getElementById(\"" + this.id + "\").innerHTML = \"" + varValues + "\";",timeOutSequ);
				}
				else {
					window.setTimeout("document.all." + this.id + ".innerHTML = \"" + varValues + "\";",timeOutSequ);
				}
				break;

			default:
				this.ObjectModel.style[strStyle] = varValues;
		}
	}
	else if(thisBrowser.typ.isNC) {
		switch(strStyle) {
			case 'visibility':
				var LayerVisibility = (varValues == 'visible')?'show':'hide';
				this.ObjectModel[strStyle] = LayerVisibility;
				break;

			case 'innerHTML':
				window.setTimeout("with(document.layers." + this.id + ".document) { open(); write(\"" + varValues + "\"); close(); }",timeOutSequ);
				break;

			case 'clip':
				for(clip=0; clip<myClipNames.length; clip++) {
					this.ObjectModel.clip[myClipNames[clip]] = varValues[myClipNames[clip]];
				}
				break;

			default:
				if(strStyle == 'height' || strStyle == 'width') {
					this.ObjectModel.clip[strStyle] = varValues;
				}
				else {
					this.ObjectModel[strStyle] = varValues;
				}
		}
	}
	return ctrlLayerStyles(this.ObjectModel.id,strStyle,varValues);
}

// gibt Styles im gueltigen Format zurueck
function ctrlLayerStyles(strId,strEntry,varWrite) {
	if(strEntry == 'width' || strEntry == 'height' || strEntry == 'zIndex' || strEntry == 'top' || strEntry == 'left') {
		varWrite = parseInt(varWrite);
		if(isNaN(varWrite)) {
			varWrite = 0;
		}
	}
	if(strEntry == 'visibility') {
		varWrite = String(varWrite).toLowerCase();
	}
	return varWrite;
}

//---End Class Object Model---