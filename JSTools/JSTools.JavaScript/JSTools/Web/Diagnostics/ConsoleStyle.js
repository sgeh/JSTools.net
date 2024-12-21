function ConsoleStyle()
{
	// contains style definitions for the debug windows (e.g. Reflection and Console);
	this.TableColor = "#CCCCCC";
	this.TableFont = "td { padding:0,3,0,3; font: 8pt verdana, arial; color: #000000; margin:0,0,0,0; };";
	this.TableFont += "h1 { font: 12pt verdana, arial; color: #000000; font-weight: bold; margin:25,0,0,0; };";
	this.TableFont += "h2 { font: 10pt verdana, arial; color: #000000; font-weight: bold; margin:10,0,2,0; };";
	this.TableFont += "a { font: 8pt verdana, arial; color: #000000; font-weight: bold; font-style: none; text-decoration: none; margin:0,0,0,0; };";
	this.WindowColor = "#FFFFFF";
	this.WindowTopMargin = 10;
	this.WindowLeftMargin = 10;
	this.WindowOptions = new WindowOptions(WindowOptions.Options.Scrollbars);
	this.WindowTitle = "Window";

	// returns the head tag and the begin of the body tag with the specified style properties
	this.Render = function(strBody)
	{
		return	"<html><head><title>" + this.WindowTitle + "</title><style>" + this.TableFont + "</style></head>"
				+ "<body bgcolor='" + this.WindowColor + "' marginwidth='" + this.WindowTopMargin + "' marginheight='" + this.WindowLeftMargin + "' leftmargin='" + this.WindowLeftMargin + "' topmargin='" + this.WindowTopMargin + "'>"
				+ strBody + "</body></html>";
	}
}
ConsoleStyle.prototype.toString = function()
{
	return "[object ConsoleStyle]";
}