//*****************************************************************************************//
// Copyright 2023, Patrice Charbonneau                                                     //
//                 a.k.a. Genetic Wolf                                                     //
//                 Iid: [80a11b77a52aa9eeed80c9d37dcb7878519289d40beeddb40bb23a60d2711963] //
//                 All rights reserved.                                                    //
//                                                                                         //
// This source code is licensed under the [BSD 3-Clause "New" or "Revised" License]        //
// found in the LICENSE file in the root directory of this source tree.                    //
//*****************************************************************************************//
//**      If you modify this file, you MUST rename it to exclude the version number.     **//
//*****************************************************************************************//

//v1.01 - 2017-12-09:	add MaximizeForm()
//v1.02 - 2017-12-10:	name normalistion, directory prefix [@] used
//v1.03 - 2017-12-22:	syntax upgrade
//v1.04 - 2019-04-24:	Form Extention removed and added to formExtender (vcxForm.cs)
//v1.05 - 2019-05-04:	Move Debug Extender to vcxExtension
//v1.06 - 2019-05-05:	Add blnError (for Main() check)
//v2.00 - 2019-12-08:	BREAKING CHANGE - it's a new monster wich can handle string and FileExist method.  Static class [AppEx]
//v2.01 - 2019-12-08:	Adding Instr() and InstrRev() method, Add Debug class, Add ExeName, Add DbPath
//v2.02 - 2019-12-25:	String function have been migrated to vcxStringEx_v1.01.cs to ensure compatibility
//						vcxAppEx is dependency free of any StringEx function
//						removing unrequired @ on DirectoryEx() parameter
//v2.03 - 2020-04-06:	Fixing FileExist64() to return true on directory root
//v2.04 - 2020-07-11:	add CreatePath(); add DesktopPath(); fix missing check in Set Path(); fix missing check in Set Ressources(); add Constructor
//v2.05 - 2021-03-26:	Fixing an annoying readonly warning on mstrDesktopPath;
//v2.06 - 2021-05-09:	Add support for Settings; Move Debug outside AppEx for direct Access; add AppDataPath();
//v2.07 - 2021-05-13:	Move Settings to vcxSettings_v1.00
//v2.08 - 2021-09-10:	Correct a fork from v2.06/v2.06b where v2.06 was more recent. We're integrating in v2.08 the modification from v2.06b
//						Add ApplicationData Folder function AppDataPath() (doesn't mean it exist); Fix CreatePath() so it return true on success;
//v2.09 - 2021-09-19:	Remove support for Settings as it would require DictionaryEx to be a dependency
//v2.10 - 2021-10-03:	Remove double namespace
//v2.11 - 2022-02-05:	change Debug.Print to Debug.WriteLineEx; Add AppEx.SmallDate();
//v2.12 - 2023-09-24:	change AppEx to [partial class];
//v2.13 - 2023-10-01:	change Copyright style header
//v3.00 - 2023-10-15:	changing [public] to [internal]; Add GetLoginName(); Add InternationalId in (C) notice
//						Add [ClipboardEx] class; Add MessageBoxEx();
//v3.01 - 2023-10-16:	Change Debug for DebugEx

//Variable declaration
using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace PrototypeOmega {
	internal static partial class AppEx {
		//https://stackoverflow.com/questions/6041332/best-way-to-get-application-folder-path
		private static String mstrPath;
		private static String mstrRessources;
		private static String mstrExeName;
		private static String mstrDbPath;
		private readonly static string mstrDesktopPath;
		private readonly static string mstrAppDataPath;

		//Constructor
		static AppEx() {
			mstrPath = AppDomain.CurrentDomain.BaseDirectory; //this got EndBackSlash;
			mstrRessources = (mstrPath + @"ressources\");
			mstrExeName = Process.GetCurrentProcess().ProcessName;  //ProcessName is the AssemblyName in VS and ExeName in OS
			mstrDbPath = (mstrRessources + mstrExeName + ".accdb");
			mstrDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\";
			mstrAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\" + mstrExeName + @"\";
		}

		internal static DialogResult MessageBoxEx(string pstrMessage, string pstrCaption = "Message", MessageBoxButtons penmMessageBoxButtons = MessageBoxButtons.OK, MessageBoxIcon pobjMessageBoxIcon = MessageBoxIcon.Information, MessageBoxDefaultButton pcntDefaultButton = MessageBoxDefaultButton.Button1) {
			DialogResult ReturnValue = MessageBox.Show(pstrMessage, pstrCaption, penmMessageBoxButtons, pobjMessageBoxIcon, pcntDefaultButton);

			return ReturnValue;
		}

		internal static void Sleep(int plngLength) {
			Thread.Sleep(plngLength);
			Application.DoEvents();
		}

		internal static Boolean CreatePath(String pstrPath) {
			Boolean blnRetValue = FileExist64(pstrPath, 16); ;

			if (blnRetValue == false) {
				try {
					DirectoryInfo _ = Directory.CreateDirectory(pstrPath);
					blnRetValue = FileExist64(pstrPath, 16);
				} catch {
					blnRetValue = false;
				}
			}

			return blnRetValue;
		}

		internal static String DesktopPath() {
			return mstrDesktopPath;
		}

		internal static String AppDataPath() {
			return mstrAppDataPath;
		}

		internal static String Path {
			get { return mstrPath; }

			set {
				//check if new path value exist and if EndBackSlash is provided
				String strNewPath = DirectoryEx(value);
				Boolean IsValid = FileExist64(strNewPath, 16);

				if (IsValid == true) {
					mstrPath = strNewPath + @"\";
				}
			}
		} //Path

		internal static String Ressources {
			get { return mstrRessources; }

			set {
				//check if new path value exist and if EndBackSlash is provided
				String strNewPath = DirectoryEx(value);
				Boolean IsValid = FileExist64(strNewPath, 16);

				if (IsValid == true) {
					mstrRessources = strNewPath + @"\";
				}
			}
		}   //Ressources

		internal static String ExeName {
			get {
				return mstrExeName;
			}

			set {
				mstrExeName = value;
			}
		}   //ExeName

		internal static String DbPath {
			get {
				return mstrDbPath;
			}

			set {
				mstrDbPath = value;
			}
		}   //DbPath

		internal static Boolean FileExist64(String pstrFilePath, Int32 plngFileAttrib = 0) {
			Boolean blnRetValue = false;
			String strDirectory = DirectoryEx(pstrFilePath);

			if (strDirectory.Length > 2) {
				if ((plngFileAttrib & 16) == 16) {
					blnRetValue = Directory.Exists(strDirectory);
				} else {
					blnRetValue = File.Exists(pstrFilePath);
				}
			} else {
				if ((plngFileAttrib & 16) == 16) {
					blnRetValue = Directory.Exists(strDirectory + "\\");
				}
			}

			return blnRetValue;
		}   //FileExist64

		//Return directory without ending BackSlash
		internal static String DirectoryEx(String pstrString) {
			String strRetValue = "";

			if (!String.IsNullOrEmpty(pstrString)) {
				String strLastChar = pstrString.Substring(pstrString.Length - 1, 1);

				if (strLastChar == @"\") {
					if (pstrString.Length > 1) {
						//remove last char
						strRetValue = pstrString.Substring(0, (pstrString.Length - 1));
					}
				} else {
					strRetValue = pstrString;
				}
			}

			return strRetValue;
		}  //DirectoryEx

		internal static string GetLoginName(bool pblnIncludeDomain = false) {
			string strLoginName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

			if (pblnIncludeDomain == false) {
				int lngInstr = StringEx.Instr(strLoginName, @"\");
				if (lngInstr > 0) {
					strLoginName = StringEx.Mid(strLoginName, lngInstr + 1);
				}
			}

			return strLoginName;
		}

		//Extention ************************************************************************
		internal static DateTime SmallDate(DateTime pdatDateTime, Boolean pblnDateOnly = true) {
			DateTime datRetValue;
			Int32 lngYear, lngMonth, lngDay;

			lngYear = pdatDateTime.Year;
			lngMonth = pdatDateTime.Month;
			lngDay = pdatDateTime.Day;

			if (pblnDateOnly == true) {
				datRetValue = new DateTime(lngYear, lngMonth, lngDay);
			} else {
				Int32 lngHour, lngMinute, lngSecond;
				lngHour = pdatDateTime.Hour;
				lngMinute = pdatDateTime.Minute;
				lngSecond = pdatDateTime.Second;
				datRetValue = new DateTime(lngYear, lngMonth, lngDay, lngHour, lngMinute, lngSecond);
			}

			return datRetValue;
		}
	}   //AppEx

	//ClipboardEx
	internal static partial class ClipboardEx {
		internal static bool Clear() {
			bool blnRetValue = false;

			Clipboard.Clear();
			Application.DoEvents();

			string strTest = Clipboard.GetText();
			if (strTest.Length == 0) { blnRetValue = true; }

			return blnRetValue;
		}

		internal static bool SetText(string pstrText, TextDataFormat ptextDataFormat = TextDataFormat.UnicodeText) {
			bool blnRetValue = false;

			Clipboard.SetText(pstrText, ptextDataFormat);
			Application.DoEvents();
			string strTest = Clipboard.GetText();
			if (strTest == pstrText) { blnRetValue = true; }

			return blnRetValue;
		}

		internal static string GetText(TextDataFormat ptextDataFormat = TextDataFormat.UnicodeText) {
			string strRetValue = Clipboard.GetText(ptextDataFormat);
			return strRetValue;
		}
	}   //ClipboardEx

	//Debug
	internal static class DebugEx {
		private static Int32 mintError = 0;

		internal static void WriteLineEx(String pstrString, String pstrLeft = "", String pstrRight = "") {
			System.Diagnostics.Debug.WriteLine(pstrLeft + pstrString + pstrRight);
		}   //Debug

		internal static void Exception(Exception pexception, String pstrLabel = "") {
			String strError = ("******************************************\n" + pstrLabel + "\n");
			strError = (strError + "[" + pexception.HResult.ToString() + "]\n");
			strError = (strError + "[" + pexception.Message + "]\n******************************************\n");
			System.Diagnostics.Debug.WriteLine(strError);
		}   //Debug

		internal static Int32 Error {
			get {
				return mintError;
			}
			set {
				mintError = value;
			}
		} //Error
	}  //Debug
}
