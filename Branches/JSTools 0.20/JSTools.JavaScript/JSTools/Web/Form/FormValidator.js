function FormValidator(objForm)
{
	var _errorSummary	= false;
	var _showMessageBox	= false;
	var _showError		= true;
	var _errorTitle		= "";
	var _errorLayer		= null;

	var _formIsValid	= false;
	var _formObject		= objForm;
	var _formElements	= new Array();
	var _errorMessage	= "";

	this.SubmitEvent	= new SubmitHandler(objForm, this);
	objForm.onsubmit	= this.SubmitEvent.OrderSubmit;


	// an error message box appears, if blnAlert is true
	this.ShowMessageBox = function(blnAlert)
	{
		_showMessageBox = ToBoolean(blnAlert);
	}


	// if blnError is true, the error messages will appear
	this.ShowErrorMessages = function(blnError)
	{
		_showError = ToBoolean(blnError);
	}


	// if blnAlert is true, only an error summary will appear
	this.ShowErrorSummary = function(blnAlert)
	{
		_errorSummary = ToBoolean(blnAlert);
	}


	// sets the title of the error summary
	this.SetErrorTitle = function(strTitle)
	{
		_errorTitle = ToString(strTitle);
	}


	// sets a layer that will contain the error messages
	this.SetErrorMessageLayer = function(strLayerId)
	{
		if(window.document.Layer)
		{
			_errorLayer = window.document.Layer[ToString(strLayerId)];
		}
	}


	// sets the form return value
	this.SetReturnValue = function(blnReturnValue)
	{
		_formIsValid = ToBoolean(blnReturnValue);
	}


	// is true, if all validators return true
	this.IsValid = function()
	{
		return _formIsValid;
	}


	// adds a selection validator to the container
	this.AddDefaultSelectionValidator = function(strOptionName, blnIsDefault, steErrorMessage)
	{
		if(IsValidSelectField(strOptionName))
		{
			AddElement(this.DefaultSelectionValidator, arguments);
		}
	}


	// returns true, if the select option match the default position
	this.DefaultSelectionValidator = function(objValues, blnValue)
	{
		var returnValue = null;

		if(GetArgumentValues(objValues, objValues, 0))
		{
			var optionField = GetFieldObj(GetArgumentValues(objValues, objValues, 0));

			for(var i = 0; i < optionField.length && !(returnValue = (optionField[i].defaultSelected && optionField[i].selected)); ++i)
			{
				;
			}
		}
		return (returnValue == GetArgumentValues(objValues, blnValue, 1));
	}


	// adds a compare validator to the container
	this.AddCompareValidator = function(strNameFirst, strNameSecond, strErrorMessage)
	{
		if(IsValidTextField(strNameFirst) && IsValidTextField(strNameSecond))
		{
			AddElement(this.CompareValidator, arguments);
		}
	}


	// returns true, if the value of objValues match strSecondValue
	this.CompareValidator = function(objValues, strSecondValue)
	{
		return (GetFieldValue(GetArgumentValues(objValues, objValues, 0)) == GetFieldValue(GetArgumentValues(objValues, strSecondValue, 1)));
	}


	// adds a regexp validator to the container
	this.AddRegExpValidator = function(strFieldName, regValidator, strErrorMessage)
	{
		if(IsValidTextField(strFieldName))
		{
			AddElement(this.RegExpValidator, arguments);
		}
	}


	// returns true, if the value of objValues match objRegExp
	this.RegExpValidator = function(objValues, objRegExp)
	{
		return (GetFieldValue(GetArgumentValues(objValues, objValues, 0)).search(GetArgumentValues(objValues, objRegExp, 1)) != -1);
	}


	// adds a length validator to the container
	this.AddLengthValidator = function(strFieldName, intLength, strErrorMessage)
	{
		if(IsValidTextField(strFieldName) && IsNumeric(intLength))
		{
			AddElement(this.LengthValidator, arguments);
		}
	}


	// returns true, if the length of the value is equal to intSecondValue
	this.LengthValidator = function(objValues, intSecondValue)
	{
		return (GetFieldValue(GetArgumentValues(objValues, objValues, 0)).length == GetArgumentValues(objValues, intSecondValue, 1));
	}


	// adds an alpha numeric validator to the container
	this.AddAlphaNumericValidator = function(strFieldName, strErrorMessage)
	{
		if(IsValidTextField(strFieldName))
		{
			AddElement(this.AlphaNumericValidator, arguments);
		}
	}


	// returns true, if objValues is alpah numeric
	this.AlphaNumericValidator = function(objValues)
	{
		return GetFieldValue(GetArgumentValues(objValues, objValues, 0)).IsAlphaNumeric();
	}


	// adds a numeric validator to the container
	this.AddNumericValidator = function(strFieldName, strErrorMessage)
	{
		if(IsValidTextField(strFieldName))
		{
			AddElement(this.NumericValidator, arguments);
		}
	}


	// returns true, if objValues is numeric
	this.NumericValidator = function(objValues)
	{
		return IsNumeric(GetFieldValue(GetArgumentValues(objValues, objValues, 0)));
	}


	// starts the validation of the controls
	this.ValidateControls = function()
	{
		_formIsValid	= true;
		_errorMessage	= "";

		for(var i = 0; i < _formElements.length && (_formIsValid || _errorSummary); ++i)
		{
			if(!_formElements[i].validator(_formElements[i].args))
			{
				_formIsValid = false;
				SetErrorMessageOption(i);
			}
		}
		ShowError();
	}



	//private statements

	// writes the error message into a layer
	function ShowError()
	{
		if(_formIsValid)
		{
			SetErrorLayerText("");
		}
		else
		{
			CheckErrorOptions();
		}
	}


	// checks, if the error should appear
	function CheckErrorOptions()
	{
		if(_showError)
		{
			if(_showMessageBox)
			{
				alert(_errorMessage);
			}

			SetErrorLayerText(_errorMessage.replace(/\n+/g, "<br>"));
		}
	}


	// writes strText into the errorLayer
	function SetErrorLayerText(strText)
	{
		if(_errorLayer)
		{
			_errorLayer.SetContent(strText);
		}
	}


	// sets the error messages
	function SetErrorMessageOption(intIndex)
	{
		if(_errorSummary)
		{
			if(!_errorMessage)
			{
				_errorMessage = (_errorTitle) ? _errorTitle + "\n\n" : "";
			}
			_errorMessage += "  * " + GetErrorFromElement(intIndex) + "\n";
		}
		else
		{
			_errorMessage = GetErrorFromElement(intIndex);
			GetFieldObj(_formElements[intIndex].args[0]).focus();
		}
	}


	// adds an element to the container
	function AddElement(objValidator, objArguments)
	{
		objArguments.IsArgument = true;
		_formElements.Add( { validator: objValidator, args: objArguments } );
	}


	// returns the container error message
	function GetErrorFromElement(intElementIndex)
	{
		return _formElements[intElementIndex].args[_formElements[intElementIndex].args.length - 1];
	}


	// returns true, if strValueFieldName is a valid text field
	function IsValidTextField(strValueFieldName)
	{
		return (IsValidField(strValueFieldName) ? !(IsUndefined(GetFieldObj(strValueFieldName).value) || IsUndefined(GetFieldObj(strValueFieldName).focus)) : false);
	}


	// returns true, if strFormField is a valid select field
	function IsValidSelectField(strFormField)
	{
		return (IsValidField(strFormField) ? ToBoolean(GetFieldObj(strFormField).options) : false);
	}


	// returns true, if strFieldName is a valid field
	function IsValidField(strFieldName)
	{
		return ToBoolean(_formObject.elements[strFieldName]);
	}


	// returns the value from the objArgument or the params
	function GetArgumentValues(objArgument, objParam, intNumber)
	{
		var returnObject = null;

		if(objArgument.IsArgument)
		{
			returnObject = objArgument[intNumber];
		}
		else if(IsValidTextField(objArgument))
		{
			returnObject = (objArgument == objParam) ? GetFieldObj(objArgument) : objParam;
		}
		return returnObject;
	}


	// returns the a field object
	function GetFieldObj(strElementName)
	{
		return (IsValidField(strElementName) ? _formObject.elements[strElementName] : null);
	}


	// returns the value of a field object
	function GetFieldValue(strFieldId)
	{
		return (IsValidTextField(strFieldId) ? GetFieldObj(strFieldId).value : "undefined");
	}


	// private submit event handler class
	function SubmitHandler(objFormField, objValidator)
	{
		var _formObject			= objFormField;
		var _formValidator		= objValidator;
		var _submitContainer	= new Array();


		// adds an event to the _submitContainer
		this.Add = function(objFunction)
		{
			_submitContainer.Add(objFunction);
		}


		// removes an event from the _submitContainer
		this.Remove = function(objFunction)
		{
			_submitContainer.Remove(_submitContainer.Search(objFunction));
		}


		// runs all registered submit events
		this.OrderSubmit = function()
		{
			for(var i = 0; i < _submitContainer.length; ++i)
			{
				_submitContainer[i]();
			}
			return _formValidator.IsValid();
		}
	}
	this.SubmitEvent.Add(this.ValidateControls);
}
FormValidator.prototype.toString = function()
{
    return "[object FormValidator]";
}