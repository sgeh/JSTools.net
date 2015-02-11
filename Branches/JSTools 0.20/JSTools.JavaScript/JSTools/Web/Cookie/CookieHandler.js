function CookieHandler()
{
	// returns the value of a cookie with a specified name. returns null if no cookie with the specified name exists
	this.GetCookie = function(strName)
	{
		strName = ToString(strName).ToEnumerable();
		return (IsUndefined(_cookieValues[strName])) ? null : _cookieValues[strName].value;
	}


	// sets a new cookie with a specified name and returns true, if the cookie value was set
	this.SetCookie = function(strCookieName, objCookieValue, datCookieExpires, strCookieDomain, strCookiePath, blnCookieSecure)
	{
		if(this.CookiesEnabled())
		{
			_cookieValues.Add(strCookieName, objCookieValue);
			WriteCookie(strCookieName, objCookieValue, datCookieExpires, strCookieDomain, strCookiePath, blnCookieSecure);
			return true;
		}
		return false;
	}


	// removes the cookie with a specified name
	this.RemoveCookie = function(strName)
	{
		var expireDate = new Date();
		expireDate.setMonth(-1);

		WriteCookie(strName, "", expireDate);
		return _cookieValues.Remove(strName);
	}


	// returns true, if cookies are enabled
	this.CookiesEnabled = function()
	{
		var cookiesEnabled = window.navigator.cookieEnabled;

		if(IsUndefined(cookiesEnabled))
		{
			document.cookie	= "cookiesEnabled=true";
			cookiesEnabled	= ToBoolean(document.cookie);
		}
		return cookiesEnabled;
	}


	//private statements

	// writes the new cookie
	function WriteCookie(strName, objValue, datExpires, strDomain, strPath, blnSecure)
	{
		var cookieSetter	=	escape(strName) + "=" + escape(objValue) + ";";
		cookieSetter		+=	((datExpires)	? "expires=" + datExpires.toGMTString() + ";" : "");
		cookieSetter		+=	((strDomain)	? "domain=" + strDomain + ";" : "");
		cookieSetter		+=	((strPath)		? "path=" + datExpires + ";" : "");
		cookieSetter		+=	((blnSecure)	? "secure" : "");
		document.cookie = cookieSetter;
	}


	// returns a multi value object, if the objMultiValue has multi values
	function GetMultiValueObject(objMultiValue)
	{
		if(IsMultiValueCookie(objMultiValue) && IsUndefined(objMultiValue.IsMultiValue))
		{
			objMultiValue = new CookieMultiValue(objMultiValue);
		}
		return objMultiValue;
	}


	// returns true, if strMulitValue has multi values
	function IsMultiValueCookie(strMulitValue)
	{
		var multiValueCookie = /^([^=&]+=[^=&]*&?)+$/;
		return multiValueCookie.test(strMulitValue);
	}


	// CookieContainer Class
	function CookieContainer(strCookie)
	{
		// returns the object name
		this.toString = function()
		{
			return "[object CookieContainer]";
		}


		// adds a new cookie to the current cookie container
		this.Add = function(strCookieName, objCookieValue)
		{
			AddCookieToArray([strCookieName, objCookieValue], this);
		}


		// removes a new cookie from the current cookie container and returns the value of it
		this.Remove = function(strCookieName)
		{
			var cookieValue = null;

			for(var i = 0; i < _cookieContainer.length; ++i)
			{
				if(_cookieContainer[i].name == strCookieName)
				{
					var relationName = ToString(strCookieName).ToEnumerable();
					cookieValue = _cookieContainer[i].value;
					_cookieContainer.Remove(i);

					if(!IsUndefined(CookieContainer.prototype[relationName]))
					{
						delete CookieContainer.prototype[relationName];
					}
					else
					{
						delete this[relationName];
					}

					break;
				}
			}
			return cookieValue;
		}


		// reads all from window.document.cookie
		function ReadCookies()
		{
			var cookieArray = _cookieString.split(";");

			for(var i = 0; i < cookieArray.length; ++i)
			{
				AddCookieToArray(cookieArray[i].split("="), CookieContainer.prototype);
			}
		}


		// creates a new container item with the specified relation
		function AddCookieToArray(arrValues, objToAdd)
		{
			_cookieContainer.Add( { name: arrValues[0], value: GetMultiValueObject(unescape(arrValues[1])) } );
			objToAdd[ToString(arrValues[0]).ToEnumerable()] = _cookieContainer[_cookieContainer.length - 1];
		}

		var _cookieString		= strCookie;
		var _cookieContainer	= new Array();
		ReadCookies();
	}



	var _cookieValues = new CookieContainer(window.document.cookie);
}
CookieHandler.prototype.toString = function()
{
    return "[object CookieHandler]";
}

window.document.Cookie = new CookieHandler();