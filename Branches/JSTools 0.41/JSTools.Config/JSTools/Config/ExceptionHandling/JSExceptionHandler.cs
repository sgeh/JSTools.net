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

using System;
using System.Configuration;
using System.Text;

using JSTools.Config.ExceptionHandling.Serialization;
using JSTools.Config.ScriptFileManagement;

namespace JSTools.Config.ExceptionHandling
{
	/// <summary>
	/// Used to determine the error handling mode on client side.
	/// </summary>
	public enum ErrorHandling : byte
	{
		/// <summary>
		/// Native client side error handling is disabled. This is usefull in
		/// conjunction with client side debuggers. Script internal exceptions
		/// will be thrown.
		/// </summary>
		None = 0x00,

		/// <summary>
		/// All client side error messages are suppressed. This mode may be used
		/// in conjuction with the release mode.
		/// </summary>
		Catch = 0x01,

		/// <summary>
		/// Client side error messages are displayed and error handling is enabled.
		/// </summary>
		Throw = 0x02,
	}

	/// <summary>
	/// Represents a bit mask used to filter client side events.
	/// </summary>
	[Flags]
	public enum ErrorEvent
	{
		/// <summary>
		/// The error is stored on client side but no event will be fired.
		/// </summary>
		None = 0x00,

		/// <summary>
		/// The error is stored on client side and all events will be fired.
		/// </summary>
		All = 0x01,

		/// <summary>
		/// The log message is stored on client side and an OnLog event will be fired.
		/// </summary>
		Log = 0x02,

		/// <summary>
		/// The error message is stored on client side and an OnError event will be fired.
		/// </summary>
		Error = 0x04,

		/// <summary>
		/// The warning message is stored on client side and an OnWarn event will be fired.
		/// </summary>
		Warn = 0x08
	}

	/// <summary>
	/// Represents an &lt;exception&gt; configuration node instance.
	/// </summary>
	public class JSExceptionHandler : AJSToolsSection
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private ErrorHandling _errorHandling = ErrorHandling.None;
		private ErrorEvent _errorEvent = ErrorEvent.None;
		private string _requiredModule = string.Empty;
		private string _errorProvider = string.Empty;
		private string _sectionName = string.Empty;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the error provider on client side. This is normally window.onerror.
		/// </summary>
		public string ErrorProvider
		{
			get { return _errorProvider; }
		}

		/// <summary>
		/// Returns the name of the representing element.
		/// </summary>
		public string SectionName
		{
			get { return _sectionName; }
		}

		/// <summary>
		/// Gets the type of error handling on the client side.
		/// </summary>
		public ErrorHandling ErrorHandling
		{
			get { return _errorHandling; }
		}

		/// <summary>
		/// Gets the error event handling on the client side.
		/// </summary>
		public ErrorEvent ErrorEvent
		{
			get { return _errorEvent; }
		}

		/// <summary>
		/// Gets the name of the required module.
		/// </summary>
		public string RequiredModule
		{
			get { return _requiredModule; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Initializes a new JSExceptionHandler instance.
		/// </summary>
		/// <param name="exceptionData">Exception data.</param>
		/// <param name="ownerConfig">Owner (parent) configuration instance.</param>
		/// <param name="nodeName">Contains the name of the representing node.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		public JSExceptionHandler(
			JSTools.Config.ExceptionHandling.Serialization.ExceptionHandling exceptionData,
			IJSToolsConfiguration ownerConfig,
			string nodeName) : base(ownerConfig)
		{
			if (exceptionData == null)
				throw new ArgumentNullException("exceptionNode", "The given xml section contains a null reference.");

			if (nodeName == null)
				throw new ArgumentNullException("nodeName", "The given node name contains a null reference.");

			_sectionName = nodeName;
			_errorHandling = exceptionData.ErrorHandling;
			_requiredModule = exceptionData.Requires;
			_errorProvider = exceptionData.ErrorProvider;

			InitErrorEventEnum(exceptionData.Event);
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Checks the relations between the configuration sections. The script section is initilialized
		/// before calling this method.
		/// </summary>
		/// <exception cref="InvalidOperationException">Could not find a module with the required module.</exception>
		/// <exception cref="ConfigurationException">The script file handling section was not initialized.</exception>
		public override void CheckRelations()
		{
			if (OwnerConfiguration.ScriptFileHandler == null)
				throw new ConfigurationException("The script file handling section was not initialized.");

			if (OwnerConfiguration.ScriptFileHandler.GetModuleByName(_requiredModule) == null)
				throw new InvalidOperationException("Could not find a module with the name '" + _requiredModule + "'.");
		}

		private void InitErrorEventEnum(Event data)
		{
			if (data.Log && data.Error && data.Warn)
			{
				_errorEvent = ErrorEvent.All;
			}
			else
			{
				if (data.Log)
					_errorEvent |= ErrorEvent.Log;

				if (data.Error)
					_errorEvent |= ErrorEvent.Error;

				if (data.Warn)
					_errorEvent |= ErrorEvent.Warn;
			}
		}
	}
}
