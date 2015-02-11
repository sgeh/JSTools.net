var undefined;

// returns a valid number
function ToNumber(objExpression)
{
	var newNumber = Number(objExpression);
	return newNumber.valueOf();
}


// returns a valid string
function ToString(objExpression)
{
	var newString = String(objExpression);
	return newString.valueOf();
}


// returns a valid boolean
function ToBoolean(objExpression)
{
	var newBool = Boolean(objExpression);
	return newBool.valueOf();
}



// returns true, if the varExpression is like ''
function IsVoid(objExpression)
{
	return (ToString(objExpression) == '')
}


// returns true, if the varExpression is undefined
function IsUndefined(objExpression)
{
	return (typeof(objExpression) == 'undefined');
}


// returns true, if the varExpression is null
function IsNull(objExpression)
{
	return (objExpression == null);
}


// returns true, if the varExpression is a number
function IsNumeric(objExpression)
{
	return !isNaN(parseInt(objExpression));
}


// returns true, if the varException is an array
function IsArray(objExpression)
{
	return (!IsUndefined(objExpression) && !IsUndefined(objExpression.length) && !IsUndefined(objExpression.IsArray));
}


// returns true, if the varException is a function
function IsFunction(objExpression)
{
	return (typeof(objExpression) == 'function');
}