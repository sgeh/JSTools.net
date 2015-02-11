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
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;

using JSTools.Config.ExceptionHandling;
using JSTools.Config.ScriptFileManagement;

namespace JSTools.Config.Session
{
	/// <summary>
	/// Specifies the initialization status of the current configuration instance.
	/// </summary>
	public enum Status
	{
		NothingDone,
		Initializing,
		Initilized
	}


	/// <summary>
	/// This class is used for storing the instances in a session. The session storage is different
	/// in windows and web applications. The implemented architecture follows the copy-on-write pattern.
	/// </summary>
	public abstract class AJSToolsSessionHandler : IJSToolsConfiguration
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns a default JSTools configuration instance. If a JSTools.net section is specified
		/// in the web.config, this property will initialize the values of the web.config configuration
		/// section. The default instance of the JSToolsConfiguration is immutable. When you write a
		/// value, a new copy of the current instance will be created and stored in the session cache.
		/// Thus changes on the default instance are noticeable in the actual session only.
		/// </summary>
		public	static readonly	IJSToolsConfiguration	Instance				= CreateDefaultEnvInstance();
		public	const			string					DEFAULT_SESSION_KEY		= "JSTools_Session_Config";

		private					string					_sessionName			= null;
		private					Status					_initStatus				= Status.NothingDone;
		private					bool					_isDefault				= false;


		/// <summary>
		/// Returns the writeable configuration. If it does not exist, this method will create a new
		/// writeable configuration and store it in the session cache.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		/// <exception cref="InvalidOperationException">Could not create a new writeable instance, the immutable instance is not initialized yet.</exception>
		/// <exception cref="JSToolsEventException">An error has occured during event bubbling.</exception>
		IJSToolsConfiguration IJSToolsConfiguration.WriteableInstance
		{
			get { return WriteableInstance; }
		}


		/// <summary>
		/// Will be fired if the user writes a value into a immutable configuration instance. Before this
		/// event, a new writeable IJSToolsConfiguration instance was created.
		/// </summary>
		public event JSToolsWriteableCreateEvent OnWriteableCreate;


		/// <summary>
		/// Returns true, if a HttpContext is active, which has a valid page handler.
		/// </summary>
		public static bool IsWebEnvironment
		{
			get { return HttpContext.Current != null && (HttpContext.Current.Handler as Page) != null; }
		}


		/// <summary>
		/// Returns true, if the current instance is contained in the static instance property.
		/// </summary>
		public bool IsDefaultInstance
		{
			get { return _isDefault; }
		}


		/// <summary>
		/// Returns the initialization status of this instance.
		/// </summary>
		public Status InitStatus
		{
			get { return _initStatus; }
		}


		/// <summary>
		/// Gets/sets the session name.
		/// </summary>
		public string SessionName
		{
			get { return _sessionName; }
		}


		/// <summary>
		/// Returns the application configuration.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		/// <exception cref="InvalidOperationException">Could not refer to the immutable instance, it is not fully created yet.</exception>
		public IJSToolsConfiguration ImmutableInstance
		{
			get { return CheckImmutableInstanceStatus(); }
		}


		/// <summary>
		/// Returns true, if a XmlDocument is initialized.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		public bool IsXmlDocumentInitialized
		{
			get { return GetInstance().IsXmlDocumentInitialized; }
		}


		/// <summary>
		/// Gets/sets the type of client exception handling.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		public AJSExceptionHandler ErrorHandling
		{
			get { return GetInstance().ErrorHandling; }
		}


		/// <summary>
		/// Gets the script file handler, which handles using of the debug and release scripts.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		public AJSScriptFileHandler ScriptFileHandler
		{
			get { return GetInstance().ScriptFileHandler; }
		}


		/// <summary>
		/// True to write the instance into the session storage.
		/// </summary>
		public bool StoreInSession
		{
			get { return (_sessionName != null && _sessionName != ""); }
		}


		/// <summary>
		/// Returns the writeable configuration. If it does not exist, this method will create a new
		/// writeable configuration and store it in the session cache.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		/// <exception cref="InvalidOperationException">Could not create a new writeable instance, the immutable instance is not initialized yet.</exception>
		/// <exception cref="JSToolsEventException">An error has occured during event bubbling.</exception>
		public AJSToolsConfiguration WriteableInstance
		{
			get
			{
				if (_initStatus != Status.Initilized)
					throw new InvalidOperationException("Could not create a new writeable instance, the immutable instance is not initialized yet!");

				try
				{
					if (GetWriteableInstance() == null)
					{
						// creates writeable configuration instance
						CreateWriteableInstance();
						FireOnWriteableCreateEvent();
					}
				}
				catch (Exception e)
				{
					throw new InvalidSessionHandlerException(e, "Could not create an AJSToolsConfiguration instance, the session handler has returned an error!", JSToolsInstanceType.Writeable);
				}
				return GetWriteableInstance();
			}
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new AJSToolsSessionHandler instance.
		/// </summary>
		/// <param name="sessionName">Name of the session storage.</param>
		/// <param name="isDefault">True to determine the default instance.</param>
		/// <exception cref="ArgumentException">The specified value does not contain a valid session key.</exception>
		internal AJSToolsSessionHandler(string sessionName, bool isDefault)
		{
			if (sessionName == null || sessionName == String.Empty)
				throw new ArgumentException("The specified value does not contain a valid session key!", "SessionName");

			_sessionName	= sessionName;
			_isDefault		= isDefault;
		}


		/// <summary>
		/// Initializes a new AJSToolsSessionHandler instance.
		/// </summary>
		/// <param name="sessionName">Name of the session storage.</param>
		/// <exception cref="ArgumentException">The specified value does not contain a valid session key.</exception>
		internal AJSToolsSessionHandler(string sessionName)
		{
			if (sessionName == null || sessionName == String.Empty)
				throw new ArgumentException("The specified value does not contain a valid session key!", "SessionName");

			_sessionName	= sessionName;
		}


		/// <summary>
		/// Initializes a new AJSToolsSessionHandler instance.
		/// </summary>
		internal AJSToolsSessionHandler()
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new configuration instance. The session handling is appropriated to the
		/// current environment. The wirteable instance will not be stored in the
		/// session chache.
		/// </summary>
		/// <returns>Returns the created instance.</returns>
		/// <exception cref="NotSupportedException">Could not localize a valid session handler for the current environment.</exception>
		public static IJSToolsConfiguration CreateEnvInstance()
		{
			if (IsWebEnvironment)
			{
				return new JSToolsWebSessionHandler();
			}
			else
			{
				return new JSToolsClientSessionHandler();
			}
		}


		/// <summary>
		/// Creates a new configuration instance. The session handling is appropriated to the
		/// current environment.
		/// </summary>
		/// <param name="sessionName">Name of the session storage.</param>
		/// <returns>Returns the created instance.</returns>
		/// <exception cref="NotSupportedException">Could not localize a valid session handler for the current environment.</exception>
		public static IJSToolsConfiguration CreateEnvInstance(string sessionName)
		{
			if (IsWebEnvironment)
			{
				return new JSToolsWebSessionHandler(sessionName);
			}
			else
			{
				return new JSToolsClientSessionHandler(sessionName);
			}
		}


		/// <summary>
		/// Writes the JSTools sections into the given render context.
		/// </summary>
		/// <param name="renderContext">The RenderProcessTicket object.</param>
		/// <exception cref="ConfigurationException">Could not render a configuration section.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument is not specified.</exception>
		/// <exception cref="ArgumentNullException">The given RenderProcessTicket object contains a null reference</exception>
		public void Render(RenderProcessTicket renderContext)
		{
			GetInstance().Render(renderContext);
		}


		/// <summary>
		/// Returns the requested configuration section, specified in the JSTools configuration file.
		/// You will obtain a null reference, if no handler was found.
		/// </summary>
		/// <param name="configNodeName">Name of the section node.</param>
		/// <returns>Returns the requested configuration instance.</returns>
		/// <exception cref="ArgumentNullException">The specified name contains a null reference.</exception>
		public AJSToolsEventHandler GetConfig(string configNodeName)
		{
			return GetInstance().GetConfig(configNodeName);
		}


		/// <summary>
		/// Serializes the configuration into a xml document and saves it to the specified file path.
		/// Default encoding is UTF-8.
		/// </summary>
		/// <param name="filePath">File path where the serialized file should be placed in.</param>
		/// <exception cref="ArgumentNullException">The specified file path contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not write into the specified stream or the specified file path is invalid.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument is not specified.</exception>
		/// <exception cref="IOException">An I/O error occurs.</exception>
		public void SaveConfiguration(string filePath)
		{
			GetInstance().SaveConfiguration(filePath);
		}


		/// <summary>
		/// Serializes the configuration into a xml document and saves it to the specified file path.
		/// </summary>
		/// <param name="filePath">File path where the serialized file should be placed in.</param>
		/// <param name="encoding">Ecoding to write.</param>
		/// <exception cref="ArgumentNullException">The specified file path contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not write into the specified stream or the specified file path is invalid.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument is not specified.</exception>
		/// <exception cref="IOException">An I/O error occurs.</exception>
		/// <exception cref="ArgumentNullException">The given encoding contains a null reference.</exception>
		public void SaveConfiguration(string filePath, Encoding encoding)
		{
			GetInstance().SaveConfiguration(filePath, encoding);
		}


		/// <summary>
		/// Serializes the configuration into a xml document which will be written into the specified stream. Be sure
		/// that you close the stream manually.
		/// </summary>
		/// <param name="toWrite">Specifies the stream.</param>
		/// <param name="encoding">Ecoding to write.</param>
		/// <exception cref="ArgumentNullException">The specified file path contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not write into the given stream.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument is not specified.</exception>
		/// <exception cref="IOException">An I/O error occurs, such as the file being closed.</exception>
		/// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
		/// <exception cref="ArgumentNullException">The given encoding contains a null reference.</exception>
		public void SaveConfiguration(Stream toWrite, Encoding encoding)
		{
			GetInstance().SaveConfiguration(toWrite, encoding);
		}


		/// <summary>
		/// Serializes the configuration into a xml document which will be written into the specified stream. Be sure
		/// that you close the stream manually. Default encoding is UTF-8.
		/// </summary>
		/// <param name="toWrite">Specifies the stream.</param>
		/// <exception cref="ArgumentNullException">The specified file path contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not write into the given stream.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument is not specified.</exception>
		/// <exception cref="IOException">An I/O error occurs, such as the file being closed.</exception>
		/// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public void SaveConfiguration(Stream toWrite)
		{
			GetInstance().SaveConfiguration(toWrite);
		}


		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified xml document.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was not already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		public void LoadXml(XmlDocument configDocument)
		{
			GetInstance().LoadXml(configDocument);
		}

		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified Stream.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was not already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		/// <exception cref="XmlException">There is a load or parse error in the XML.</exception>
		public void LoadXml(Stream configDocument)
		{
			GetInstance().LoadXml(configDocument);
		}


		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified string.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was not already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		/// <exception cref="XmlException">There is a load or parse error in the XML.</exception>
		public void LoadXml(string configDocument)
		{
			GetInstance().LoadXml(configDocument);
		}


		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified XmlReader.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was not already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		/// <exception cref="XmlException">There is a load or parse error in the XML.</exception>
		public void LoadXml(XmlReader configDocument)
		{
			GetInstance().LoadXml(configDocument);
		}


		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified TextReader.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was not already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		/// <exception cref="XmlException">There is a load or parse error in the XML.</exception>
		public void LoadXml(TextReader configDocument)
		{
			GetInstance().LoadXml(configDocument);
		}


		/// <summary>
		/// Returns the writeable created instance. If the instance was not created yet,
		/// this method returns a null reference.
		/// </summary>
		/// <returns>Retruns a AJSToolsConfiguration instance.</returns>
		protected abstract AJSToolsConfiguration GetWriteableInstance();


		/// <summary>
		/// Creates a new writeable session instance. This method must be overwritten for client and 
		/// web applications.
		/// </summary>
		protected abstract void CreateWriteableInstance();


		/// <summary>
		/// Returns the immutable created instance. If the instance was not created yet,
		/// this method returns a null reference.
		/// </summary>
		/// <returns>Retruns a AJSToolsConfiguration instance.</returns>
		protected abstract AJSToolsConfiguration GetImmutableInstance();


		/// <summary>
		/// Creates a new immutable JSToolsConfiguration instance. This method must be overwritten for
		/// client and web applications.
		/// </summary>
		protected abstract void CreateImmutableInstance();


		/// <summary>
		/// Creates and returns the default configuration instance.
		/// </summary>
		/// <returns>Returns the created instance.</returns>
		private static IJSToolsConfiguration CreateDefaultEnvInstance()
		{
			if (IsWebEnvironment)
			{
				return new JSToolsWebSessionHandler(DEFAULT_SESSION_KEY, true);
			}
			else
			{
				return new JSToolsClientSessionHandler(DEFAULT_SESSION_KEY, true);
			}
		}


		/// <summary>
		/// Returns the writeable instance, if it was already initialized. Otherwise
		/// this method will return the immutable instance.
		/// </summary>
		/// <returns>Returns a valid AJSToolsConfiguration instance.</returns>
		private AJSToolsConfiguration GetInstance()
		{
			AJSToolsConfiguration writeableInstance = null;
			
			try
			{
				writeableInstance = GetWriteableInstance();
			}
			catch (Exception e)
			{
				throw new InvalidSessionHandlerException(e, "Could not get the AJSToolsConfiguration instance!", JSToolsInstanceType.Writeable);
			}

			return ((writeableInstance != null) ? writeableInstance : CheckImmutableInstanceStatus());
		}


		/// <summary>
		/// Checks the initialization status.
		/// </summary>
		private AJSToolsConfiguration CheckImmutableInstanceStatus()
		{
			if (_initStatus == Status.Initializing)
				throw new InvalidOperationException("Could not refer to the immutable instance, it is not created yet!");

			try
			{
				return InitImmutableInstance();
			}
			catch (Exception e)
			{
				throw new InvalidSessionHandlerException(e, "Could not access an AJSToolsConfiguration instance!", JSToolsInstanceType.Immutable);
			}
		}


		/// <summary>
		/// Initializes a new immutable instance.
		/// </summary>
		private AJSToolsConfiguration InitImmutableInstance()
		{
			// is immutalbe instance already created
			if (GetImmutableInstance() == null)
			{
				_initStatus = Status.Initializing;

				// create immutable configuration instance
				CreateImmutableInstance();

				_initStatus = Status.Initilized;
			}
			return GetImmutableInstance();
		}


		/// <summary>
		/// Fires the OnWriteableCreate event.
		/// </summary>
		private void FireOnWriteableCreateEvent()
		{
			try
			{
				// fire OnWriteableCreate event
				if (OnWriteableCreate != null)
				{
					OnWriteableCreate(GetWriteableInstance());
				}
			}
			catch (Exception e)
			{
				throw new JSToolsEventException(e, "An error has occured during event bubbling!", "OnWriteableCreate");
			}
		}
	}
}
