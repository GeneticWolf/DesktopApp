//*****************************************************************************************//
// Copyright 2023, Patrice Charbonneau                                                     //
//                 a.k.a. Genetic Wolf                                                     //
//                 Iid: [80a11b77a52aa9eeed80c9d37dcb7878519289d40beeddb40bb23a60d2711963] //
//                 All rights reserved.                                                    //
//                                                                                         //
// This source code is licensed under the [BSD 3-Clause "New" or "Revised" License]        //
// found in the LICENSE file in the root directory of this source tree.                    //
//*****************************************************************************************//

using PrototypeOmega;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace ExtractCountryName {
	public partial class frmMain : Form {
		//https://www.worldometers.info/geography/alphabetical-list-of-countries/
		private const string cUrlCountryList = "https://www.worldometers.info/geography/alphabetical-list-of-countries/";

		public frmMain() {
			InitializeComponent();

			//Export a list of Country in a C# class
			cmdGo.Click += CmdGo_Click;
		}

		private void CmdGo_Click(object sender, EventArgs e) {
			this.cmdGo.Enabled = false;

			HtmlAgilityPack.HtmlWeb objHtmlWeb = new HtmlAgilityPack.HtmlWeb();
			objHtmlWeb.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36";
			HtmlAgilityPack.HtmlDocument objHtmlDocument = objHtmlWeb.Load(cUrlCountryList);

			//Save all <td> element
			List<string> lstTd = new List<string>();
			HtmlAgilityPack.HtmlNodeCollection objTds = objHtmlDocument.DocumentNode.SelectNodes("//td");
			if (objTds != null) {
				foreach (HtmlAgilityPack.HtmlNode objTd in objTds) {
					lstTd.Add(objTd.InnerText);
				}

				//did we found our array ?
				if (lstTd.Count > 0) {
					List<string> lstCountryName = new List<string>();

					//We only want the CountryName, wich are element 2 every 5 items
					for (int i = 0; i < lstTd.Count; i += 5) {
						lstCountryName.Add(lstTd[i + 1]);
					}
					ExportCountryName(lstCountryName);
				}
			}

			//Exit program
			this.Close();
		}

		private void WriteLine(StreamWriter pobjStreamWriter, int plngIndent, string pstrText) {
			const string sep = "\t";
			string strOutput = pstrText;

			for (int i = 0; i < plngIndent; i++) {
				strOutput = sep + strOutput;
			}
			pobjStreamWriter.WriteLine(strOutput);
		}

		private void ExportCountryName(List<string> plstCountryName) {
			DateTime datNow = DateTime.UtcNow;
			string strYear = datNow.Year.ToString("0000") + "-" + datNow.Month.ToString("00") + "-" + datNow.Day.ToString("00");
			string strPath = AppEx.Path + "vcxCountryName_v" + strYear + ".cs";

			using (StreamWriter objStreamWriter = new StreamWriter(strPath)) {
				int lngIndent = 0;
				this.WriteLine(objStreamWriter, lngIndent, "using System.ComponentModel;\r\n");
				this.WriteLine(objStreamWriter, lngIndent, "namespace PrototypeOmega {");
				lngIndent++;

				this.WriteLine(objStreamWriter, lngIndent, "//Country Name [" + strYear + "]");
				this.WriteLine(objStreamWriter, lngIndent, "//Extracted from [" + cUrlCountryList + "]");
				this.WriteLine(objStreamWriter, lngIndent, "public enum eCountryName {");
				lngIndent++;

				//Write all country name
				string strKey;
				string strKeyEx;
				int lngCount = 0;
				foreach (string strCountryName in plstCountryName) {
					strKey = strCountryName;
					strKeyEx = strKey;
					lngCount++;

					//Process [strKey]
					//replace forbiden character
					strKey = strKey.Replace(" ", "_");
					strKey = strKey.Replace("'", "");
					strKey = strKey.Replace("\"", "");
					strKey = strKey.Replace("(", "");
					strKey = strKey.Replace(")", "");
					strKey = strKey.Replace("-", "");
					strKey = strKey.Replace(".", "");

					//Replace html entities like [&ocirc;] with [ô]
					strKey = WebUtility.HtmlDecode(strKey);

					//Remove accent
					strKey = strKey.RemoveAccents();

					//Process [strKeyEx]
					//[Description("country name")]
					strKeyEx = strKeyEx.Replace("\"", "");
					//Replace html entities like [&ocirc;] with [ô]
					strKeyEx = WebUtility.HtmlDecode(strKeyEx);

					//Export current enum [Description]
					if (strKeyEx != strKey) {
						strKeyEx = "[Description(\"" + strKeyEx + "\")]";
						this.WriteLine(objStreamWriter, lngIndent, strKeyEx);
					}

					//Export current enum
					if (lngCount != plstCountryName.Count) {
						strKey = strKey + ",\r\n";
					}
					this.WriteLine(objStreamWriter, lngIndent, strKey);
				}
				lngIndent--;
				this.WriteLine(objStreamWriter, lngIndent, "}");

				lngIndent--;
				this.WriteLine(objStreamWriter, lngIndent, "}");
			}
		}
	}
}
