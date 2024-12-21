/*
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

/// <file>
///     <copyright see="prj:///doc/copyright.txt"/>
///     <license see="prj:///doc/license.txt"/>
///     <owner name="Silvan Gehrig" email="silvan.gehrig@mcdark.ch"/>
///     <version value="$version"/>
///     <since>JSTools.dll 0.1.0</since>
/// </file>

using System;
using System.Configuration;
using System.Text;
using System.Xml;

using JSTools.Config.ScriptFileManagement;
using JSTools.Config.Session;
using JSTools.Xml;

namespace JSTools.Config.ExceptionHandling
{
	/// <summary>
	/// Describes the type of error handling on the client side.
	/// </summary>
	[Flags]
	public enum ErrorHandling : byte
	{
		None		= 0x00,
		LogError	= 0x01,
		AlertError	= 0x02,
		CatchError	= 0x04
	}


	/// <summary>
	/// Represents an instance of the &lt;exception&gt; configuration section in the JSTools.net configuration.
	/// </summary>
	public abstract class AJSExceptionHandler : AJSToolsEventHandler, IWriteable
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		public	readonly	string							SECTION_NAME;

		protected			ErrorHandling					_errorHandling		= ErrorHandling.None;
		protected			string							_requiredModule		= "";

		private	const		string							TYPE_ATTRIB			= "type";
		private	const		string							LOG_ATTIRB			= "log";
		private	const		string							ALERT_ATTIRB		= "alert";
		private	const		string							CATCH_ATTIRB		= "catch";
		private	const		string							REQUIRES_ATTIRB		= "requires";
		
		private				AJSToolsConfiguration			_ownerConfig		= null;


		/// <summary>
		/// Returns the writeable instance of the current instance.
		/// </summary>
		AJSToolsEventHandler IWriteable.WriteableInstance
		{
			get { return WriteableInstance; }
		}


		/// <summary>
		/// Returns the name of the representing element.
		/// </summary>
		public string SectionName
		{
			get { return SECTION_NAME; }
		}


		/// <summary>
		/// Represents the owner configuration.
		/// </summary>
		public override IJSToolsConfiguration OwnerConfiguration
		{
			get { return _ownerConfig; }
		}


		/// <summary>
		/// Represents the owner configuration.
		/// </summary>
		public override AJSToolsEventHandler OwnerSection
		{
			get { return null; }
		}


		/// <summary>
		/// Gets/sets the type of error handling on the client side.
		/// </summary>
		public abstract ErrorHandling Handling
		{
			get;
			set;
		}


		/// <summary>
		/// Gets/sets the name of the required module.
		/// </summary>
		public abstract string RequiredModule
		{
			get;
			set;
		}


		/// <summary>
		/// Returns a writeable instance of this object.
		/// </summary>
		public AJSExceptionHandler WriteableInstance
		{
			get { return _ownerConfig.WriteableInstance.ErrorHandling; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new exception handling factory instance.
		/// </summary>
		/// <param name="exceptionNode">XmlNode, which contains the configuration data.</param>
		/// <param name="nodeName">Contains the name of the representing node.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		public AJSExceptionHandler(XmlNode exceptionNode, string nodeName)
		{
			if (exceptionNode == null)
				throw new ArgumentNullException("exceptionNode", "The given xml section contains a null reference!");

			if (nodeName == null)
				throw new ArgumentNullException("nodeName", "The given node name contains a null reference!");

			OnInit += new JSToolsInitEvent(OnParentInit);

			SECTION_NAME	= nodeName;
			_requiredModule	= JSToolsXmlFunctions.GetAttributeFromNode(exceptionNode, REQUIRES_ATTIRB);

			if (exceptionNode != null)
			{
				if (JSToolsXmlFunctions.GetBoolFromNodeValue(exceptionNode.Attributes[LOG_ATTIRB]))
				{
					_errorHandling |= ErrorHandling.LogError;
				}
				if (JSToolsXmlFunctions.GetBoolFromNodeValue(exceptionNode.Attributes[ALERT_ATTIRB]))
				{
					_errorHandling |= ErrorHandling.AlertError;
				}
				if (JSToolsXmlFunctions.GetBoolFromNodeValue(exceptionNode.Attributes[CATCH_ATTIRB]))
				{
					_errorHandling |= ErrorHandling.CatchError;
				}
			}
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------


		/// <summary>
		/// Renders the configuration of the "exception" xml section.
		/// </summary>
		/// <param name="renderContext">A StringBuilder object which content will be rendered to the client.</param>
		/// <exception cref="ArgumentNullException">The specified RenderProcessTicket contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The required module with the given name is not registered.</exception>
		protected override void RenderScriptConfiguration(RenderProcessTicket renderContext)
		{
			if (renderContext == null)
				throw new ArgumentNullException("renderContext", "The specified RenderProcessTicket contains a null reference!");

			renderContext.Write(_ownerConfig.ScriptFileHandler.GetScriptBeginTag());
			WriteHandlingScript(renderContext);
			renderContext.Write("\n");
			renderContext.Write(_ownerConfig.ScriptFileHandler.GetScriptEndTag());
			renderContext.Write("\n");

			// call base function to enable event bubbling
			base.RenderScriptConfiguration(renderContext);
		}


		/// <summary>
		/// This method will be called, if the parent element has fired the OnSerialize event. Renders the configuration
		/// settings of the current section into the given XmlNode instance. The current instance will not fire an event,
		/// if the deep flag is set to false.
		/// </summary>
		/// <param name="parentNode">Parent xml node instance. You have to create a new xml node instance and append it
		/// to the parent node.</param>
		/// <param name="deep">True to copy all sub elements, otherwise only the settings of the current node will be
		///  copied.</param>
		protected override void SerializeXmlConfiguration(XmlNode parentNode, bool deep)
		{
			// get type name
			Type factoryType = typeof(JSExceptionSectionHandlerFactory);
			string assemblyName = factoryType.Assembly.FullName.Substring(0, factoryType.Assembly.FullName.IndexOf(","));
			string typeDefinition = factoryType.FullName + ", " + assemblyName;

			// create new exception node, fill in type name
			XmlNode sectionNode = parentNode.OwnerDocument.CreateElement(SectionName);
			JSToolsXmlFunctions.AppendAttributeToNode(sectionNode, TYPE_ATTRIB, typeDefinition);

			// fill in values
			JSToolsXmlFunctions.AppendAttributeToNode(sectionNode, LOG_ATTIRB, GetBoolFromErrorHandling(ErrorHandling.LogError).ToLower());
			JSToolsXmlFunctions.AppendAttributeToNode(sectionNode, ALERT_ATTIRB, GetBoolFromErrorHandling(ErrorHandling.AlertError).ToLower());
			JSToolsXmlFunctions.AppendAttributeToNode(sectionNode, CATCH_ATTIRB, GetBoolFromErrorHandling(ErrorHandling.CatchError).ToLower());
			JSToolsXmlFunctions.AppendAttributeToNode(sectionNode, REQUIRES_ATTIRB, RequiredModule);

			// append exception node to parent node
			parentNode.AppendChild(sectionNode);

			if (deep)
			{
				// call base function to enable event bubbling
				base.SerializeXmlConfiguration(sectionNode, deep);
			}
		}


		/// <summary>
		/// This method will be called, if the parent element has fired the OnPreRender event. Event bubbling
		/// functionality must be implemented by this method. This event will be fired before calling the OnRender
		/// event, to prepare render operations.
		/// </summary>
		/// <param name="processTicket">A StringBuilder object which content will be rendered to the client.</param>
		protected override void PreRenderScriptConfiguration(RenderProcessTicket processTicket)
		{
			processTicket.AddRequiredModule(_requiredModule);

			// call base function to enable event bubbling
			base.PreRenderScriptConfiguration(processTicket);
		}


		/// <summary>
		/// Returns a true string, if the specified ErrorHandling flag is contained by the
		/// _errorHandling variable.
		/// </summary>
		/// <param name="handling">ErrorHandling flag to check.</param>
		/// <returns>Returns a true (bool.TrueString) or a false (bool.false) string.</returns>
		private string GetBoolFromErrorHandling(ErrorHandling handling)
		{
			return (((_errorHandling & ErrorHandling.LogError) != 0) ? bool.TrueString : bool.FalseString);
		}


		/// <summary>
		/// Renders the value of the Handling property.
		/// </summary>
		/// <param name="renderContext">StringBuilder in which the rendered content will be stored.</param>
		private void WriteHandlingScript(RenderProcessTicket renderContext)
		{
			string[] enumEntries = Enum.GetNames(typeof(ErrorHandling));

			for (int i = 0; i < enumEntries.Length; ++i)
			{
				if ((_errorHandling & (ErrorHandling)Enum.Parse(typeof(ErrorHandling), (string)enumEntries.GetValue(i))) != 0)
				{
					renderContext.Write("\nException.DebugMode |= Exception.Handling.");
					renderContext.Write((string)enumEntries.GetValue(i));
					renderContext.Write(";");
				}
			}
		}


		/// <summary>
		/// Initializes the parent instance.
		/// </summary>
		/// <param name="sender">This objcect.</param>
		/// <param name="newParent">New parent object.</param>
		/// <exception cref="ArgumentException">The given parent is not a valid AJSToolsConfiguration instance.</exception>
		private void OnParentInit(AJSToolsEventHandler sender, AJSToolsEventHandler newParent)
		{
			if (sender != this)
				throw new InvalidOperationException("Invalid sender specified!");

			if ((newParent as AJSToolsConfiguration) == null)
				throw new ArgumentException("The given parent is not a valid AJSToolsConfiguration instance!", "sender");

			_ownerConfig = (newParent as AJSToolsConfiguration);
		}
	}
}
