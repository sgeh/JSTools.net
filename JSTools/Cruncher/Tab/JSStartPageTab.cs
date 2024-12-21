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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace JSCompiler.Tab
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für JSStartPageTab.
	/// </summary>
	public class JSStartPageTab : JSBaseTab
	{
		private	Label	_welcomeLabel;
		private	Label	_descriptionLabel;


		protected override string ElementName
		{
			get { return "start"; }
		}


		public JSStartPageTab(JSCompiler baseClass)
		{
			_baseClass					= baseClass;
			_elementPage				= _baseClass.JSDesignDefinition.CreateStyledTabPage(JSCompiler.START_PAGE_NAME, TabName, true, true);

			_welcomeLabel				= _baseClass.JSDesignDefinition.CreateStyledLabel("welcomeLabel", GetConfiguration("start", "title"), true, true, true);
			_welcomeLabel.Location		= new Point(20, 20);
			_welcomeLabel.Size			= new Size(200, 30);
			_elementPage.Controls.Add(_welcomeLabel);

			_descriptionLabel			= _baseClass.JSDesignDefinition.CreateStyledLabel("descriptionLabel", GetStartUpLabelContent(), true, true, false);
			_descriptionLabel.Location	= new Point(30, 50);
			_descriptionLabel.Size		= new Size(400, 160);
			_elementPage.Controls.Add(_descriptionLabel);
		}


		/// <summary>
		/// gets the start up label content
		/// </summary>
		/// <returns>returns the start up label content</returns>
		private string GetStartUpLabelContent()
		{
			if (_baseClass.JSConfigurationLoaded)
			{
				XmlNodeList descriptionNodes = JSConfig.Instance.GetNodeList("//start/add");

				if (descriptionNodes != null)
				{
					return GetStartUpContentFromXmlDoc(descriptionNodes);
				}
			}
			return "[xml file error]";
		}

		/// <summary>
		/// reads the start up label content from the xml document
		/// </summary>
		/// <param name="descriptionNodes">xml node list to read the attributes</param>
		/// <returns>returns the generated content string</returns>
		private string GetStartUpContentFromXmlDoc(XmlNodeList descriptionNodes)
		{
			StringBuilder labelText = new StringBuilder();

			for (int i = 0; i < descriptionNodes.Count; ++i)
			{
				if (descriptionNodes[i].Attributes["title"] != null)
				{
					if (i != 0)
					{
						labelText.Append(" - " + descriptionNodes[i].Attributes["title"].Value + "\n");
					}
					labelText.Append(descriptionNodes[i].InnerText + "\n\n");
				}
			}
			return labelText.ToString();
		}
	}
}
