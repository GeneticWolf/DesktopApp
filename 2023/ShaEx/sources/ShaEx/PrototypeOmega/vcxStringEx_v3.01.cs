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

//v1.01 - 2019-12-25:	Initial release;
//						BREAKING CODE - Mid() function was working by Index (C#) Instead of Position (Vb6);
//						Fix order comparaison in Left() and Right() to evaluate IsNullOrEmpty() first;
//v1.02 - 2019-12-29:	Add ExtractXmlValue();
//v1.03 - 2020-01-02:	add NewGuidEx();
//v1.04	- 2020-05-13:	Add internal modifier;
//v1.05 - 2020-05-14:	Add ToEx internal class;
//v1.06 - 2020-05-17:	Add ToBytes();
//v1.07 - 2020-05-17:	.ToInt32 now support HEX string;
//v1.08 - 2020-08-28:	Fix a bug in ToDateTime() when getting Time position;
//						ren ToDateStr() to ToSqlString() for less confusion;
//						ren ToDateTime() to SqlStringToDateTime() for less confusion;
//						2019-12-25 change to MID function, broke the SqlStringToDateTime() function using Mid() - FIXED;
//v1.09 - 2020-08-29:	add new LEFT/RIGHT function to make Length parameter optional;
//v1.10 - 2021-02-28:	add new HexEncode(), HexDecode(); update var name in Base64Encode(), Base64Decode();
//v1.11 - 2021-03-05:	cosmetic: fix some unnecessary declaration;
//v1.12 - 2021-03-27:	Fix an issue with InstrRev() startposition where -1 were not taking into account;
//v1.13 - 2021-05-09:	Use of PrototypeOmega namespace; simplify NewGuidEx(); add ExtractPath();
//v1.14 - 2021-09-10:	Use of inline declaration;
//v1.15 - 2021-09-22:	Add PascalCase() function;
//v1.16 - 2021-09-26:	remove secondary namespace [vcxStringEx];
//v1.17 - 2022-01-03:	Adapt to FrameNetCore6;
//v2.00 - 2022-01-03:	Adapt to FrameNetCore6;
//v2.01 - 2022-02-19:	Add ToEx.ToPhone;
//v2.02 - 2023-06-18:	Compatible with Vs2019;
//v2.03 - 2023-08-26:	Move Base64 function to another module;
//v2.04 - 2023-09-24:	fix Mid function (was returning value for index 1 when asked for index 0)
//v2.05 - 2023-10-01:	add RemoveAccents(); change Copyright style header;
//v3.00 - 2023-10-15:	change public to internal; Add AttemptSetTextClipBoard(); Add InternationalId;
//						AppEx dependant;
//v3.01 - 2023-11-03:	removed AttemptSetTextClipBoard() because Clipboard() is already handled in vcxAppEx_v3.01;

//Variable declaration
using System;
using System.Text;
using System.Globalization;

namespace PrototypeOmega {
	internal static class StringEx {
		internal static string RemoveAccents(this string text) {
			StringBuilder sbReturn = new StringBuilder();
			var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
			foreach (char letter in arrayText) {
				if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
					sbReturn.Append(letter);
			}
			return sbReturn.ToString();
		}

		internal static String Left(String pstrString, Int32 plngLength) {
			String strRetValue = "";

			if (!String.IsNullOrEmpty(pstrString)) {
				if (plngLength > 0) {
					if (pstrString.Length <= plngLength) {
						strRetValue = pstrString;
					} else {
						strRetValue = pstrString.Substring(0, plngLength);
					}
				}
			}

			return strRetValue;
		} //Left

		internal static Boolean Left(String pstrString, String pstrLeft, Boolean pblnCaseInsensitive = false) {
			Boolean blnRetValue = false;

			if (!String.IsNullOrEmpty(pstrString)) {
				String strLeft = pstrLeft;
				Int32 lngLength = strLeft.Length;
				if (lngLength > 0) {
					if (pstrString.Length >= lngLength) {

						String strExtract = pstrString.Substring(0, lngLength);
						if (pblnCaseInsensitive == true) {
							strLeft = strLeft.ToLower();
							strExtract = strExtract.ToLower();
						}

						if (strExtract == strLeft) {
							blnRetValue = true;
						}
					}
				}
			}

			return blnRetValue;
		} //Left

		internal static Boolean Right(String pstrString, String pstrRight, Boolean pblnCaseInsensitive = false) {
			Boolean blnRetValue = false;

			if (!String.IsNullOrEmpty(pstrString)) {
				String strRight = pstrRight;
				Int32 lngLength = strRight.Length;
				if (lngLength > 0) {
					if (pstrString.Length >= lngLength) {
						String strExtract = pstrString.Substring(pstrString.Length - lngLength, lngLength);
						if (pblnCaseInsensitive == true) {
							strRight = strRight.ToLower();
							strExtract = strExtract.ToLower();
						}

						if (strExtract == strRight) {
							blnRetValue = true;
						}
					}
				}
			}

			return blnRetValue;
		} //Right

		internal static String Right(String pstrString, Int32 plngLength) {
			String strRetValue = "";

			if (!String.IsNullOrEmpty(pstrString)) {
				if (plngLength > 0) {
					if (pstrString.Length <= plngLength) {
						strRetValue = pstrString;
					} else {
						strRetValue = pstrString.Substring(pstrString.Length - plngLength, plngLength);
					}
				}
			}

			return strRetValue;
		} //Right

		internal static String Mid(String pstrString, Int32 plngStartPos, Int32 plngLength = -1) {
			String strRetValue = "";

			if (!String.IsNullOrEmpty(pstrString)) {
				if (plngStartPos <= pstrString.Length) {
					if (plngStartPos > 0) {
						//from here, work with index instead of position (C# convention)
						Int32 lngStartIndex = (plngStartPos - 1);

						if (lngStartIndex < pstrString.Length) {
							Int32 lngMaxLength = LengthFromIndex(pstrString, lngStartIndex);
							Int32 lngLength;

							if (plngLength < 0) {
								//return all available caracter
								lngLength = lngMaxLength;
							} else {
								//return available caracter up to lngMaxLength
								lngLength = IntNotHigher(plngLength, lngMaxLength);
							}

							if (lngLength > 0) {
								strRetValue = pstrString.Substring(lngStartIndex, lngLength);
							}
						}
					}
				}
			}

			return strRetValue;
		} //Mid

		internal static Int32 Instr(String pstrString, String pstrChar, Int32 plngStart = 1) {
			Int32 lngRetValue = -1;

			if (pstrChar.Length > 0) {
				if (plngStart < 1) {
					plngStart = 1;
				}

				if (plngStart <= pstrString.Length) {
					lngRetValue = pstrString.IndexOf(pstrChar, plngStart - 1);
					if (lngRetValue != -1) {
						lngRetValue = (lngRetValue + 1);
					}
				}
			}

			return lngRetValue;
		} //Instr

		internal static Int32 InstrRev(String pstrString, String pstrChar, Int32 plngStart = -1) {
			Int32 lngRetValue = -1;

			if (pstrChar.Length > 0) {
				if ((plngStart > pstrString.Length) || (plngStart == -1)) {
					plngStart = pstrString.Length;
				}

				if (plngStart >= 1) {
					lngRetValue = pstrString.LastIndexOf(pstrChar, plngStart - 1);
					if (lngRetValue != -1) {
						lngRetValue = (lngRetValue + 1);
					}
				}
			}

			return lngRetValue;
		} //InstrRev

		internal static String PascalCase(String pstrData) {
			String strRetValue = StringEx.Left(pstrData, 1).ToUpper() + StringEx.Mid(pstrData, 2).ToLower();

			return strRetValue;
		}

		//Extract a path from full filename
		internal static string ExtractPath(String pstrPath) {
			string strRetValue = pstrPath;

			int lngPos = InstrRev(pstrPath, @"\");
			if (lngPos > 0) {
				strRetValue = Left(pstrPath, lngPos - 1);
			}

			return strRetValue;
		}

		//Extract the String content of the first node provided
		internal static String ExtractXmlValue(String pxmlData, String pstrNodeName) {
			String strRetValue = "";
			Int32 lngInstr;

			lngInstr = Instr(pxmlData, "<" + pstrNodeName + ">");
			if (lngInstr == 0) {
				lngInstr = Instr(pxmlData, "<" + pstrNodeName + " ");
			}

			if (lngInstr != 0) {
				Int32 lngInstr2;
				lngInstr2 = Instr(pxmlData, "</" + pstrNodeName + ">");
				if (lngInstr2 != 0) {
					//Ok Find Begin of String
					lngInstr = Instr(pxmlData, ">", lngInstr + 1) + 1;
					strRetValue = Mid(pxmlData, lngInstr, lngInstr2 - lngInstr);
				}
			}

			//using System.Xml.Linq;
			//XDocument xmlDoc = XDocument.Parse(typPostResult.mstrRetData);
			////XElement xmlDoc = XElement.Parse(typPostResult.mstrRetData);
			//XElement xmlSubmission = xmlDoc.Element("mt-submission-response");
			//XElement xmlMessages = xmlSubmission.Element("messages");
			//XElement xmlMessage = xmlMessages.Element("message");
			//XElement xmlStatus = xmlMessage.Element("status");
			return strRetValue;
		}

		/* *************************************************************************************************** */
		private static Int32 LengthFromIndex(String pstrString, Int32 plngIndex) {
			Int32 lngRetValue = 0;

			if (pstrString != null) {
				if (pstrString.Length > 0) {
					Int32 lngIndex = plngIndex;
					if (lngIndex < 0) {
						lngIndex = 0;
					}

					Int32 lngLength = pstrString.Length;
					if (lngIndex < lngLength) {
						lngRetValue = (lngLength - lngIndex);
					}
				}
			}

			return lngRetValue;
		} //LengthFromIndex

		////Return value not under plngMinValue
		//private static Int32 IntNotBelow(Int32 plngValue, Int32 plngMinValue) {
		//	Int32 lngRetValue = plngValue;
		//	if (lngRetValue < plngMinValue) {
		//		lngRetValue = plngMinValue;
		//	}
		//	return lngRetValue;
		//} //intNotBelow

		//Return value not over plngMaxValue
		private static Int32 IntNotHigher(Int32 plngValue, Int32 plngMaxValue) {
			Int32 lngRetValue = plngValue;

			if (lngRetValue > plngMaxValue) {
				lngRetValue = plngMaxValue;
			}

			return lngRetValue;
		} //intNotHigher
		/* *************************************************************************************************** */

		internal static String NewGuidEx() {
			String strRetValue;
			Guid objGuid = Guid.NewGuid();
			//String strGuid = objGuid.ToString();
			//strRetValue = StringEx.Left(strGuid, 8);
			//strRetValue += StringEx.Mid(strGuid, 10, 4);
			//strRetValue += StringEx.Mid(strGuid, 15, 4);
			//strRetValue += StringEx.Mid(strGuid, 20, 4);
			//strRetValue += StringEx.Mid(strGuid, 25);

			strRetValue = objGuid.ToString("N");

			return strRetValue;
		}

		internal static string HexEncode(String pstrPlainText) {
			String strRetValue;
			StringBuilder sb = new StringBuilder();

			byte[] bytes = Encoding.Unicode.GetBytes(pstrPlainText);
			foreach (byte t in bytes) {
				sb.Append(t.ToString("X2"));
			}
			strRetValue = sb.ToString();

			return strRetValue; // returns: "48656C6C6F20776F726C64" for "Hello world"
		}

		internal static string HexDecode(String pstrHexData) {
			String strRetValue;
			byte[] bytes = new byte[pstrHexData.Length / 2];
			for (int i = 0; i < bytes.Length; i++) {
				bytes[i] = Convert.ToByte(pstrHexData.Substring(i * 2, 2), 16);
			}
			strRetValue = Encoding.Unicode.GetString(bytes);

			return strRetValue;
		}
	} //StringEx

	internal static class ToEx {
		internal static Byte[] ToBytes(this String pstrData) {
			Byte[] bytBytes;

			//byte[] bytes = Encoding.ASCII.GetBytes(pstrData);
			//string strData = Encoding.ASCII.GetString(bytes);
			bytBytes = Encoding.ASCII.GetBytes(pstrData);

			return bytBytes;
		}

		//Convert String to Int32 or null if fail
		internal static Int32 ToInt32(this String pstrInt32) {
			Int32 lngRetValue = -1;

			try {
				lngRetValue = Int32.Parse(pstrInt32);
			} catch {
			}

			return lngRetValue;
		}

		internal static Int32 HexToInt32(this String pstrInt32) {
			Int32 lngRetValue = -1;
			try {
				lngRetValue = Int32.Parse(pstrInt32, System.Globalization.NumberStyles.HexNumber);
			} catch {
			}

			return lngRetValue;
		}

		//String To Boolean
		internal static Boolean ToBoolean(this String pstrValue) {
			if (Boolean.TryParse(pstrValue, out Boolean blnRetValue) == false) {
				blnRetValue = false;
			}
			return blnRetValue;
		}

		//SqlString "mm/dd/yyyy Hh:Nn:Ss" to DateTime
		internal static DateTime SqlStringToDateTime(this String pstrSqlDate) {
			DateTime datRetValue = new DateTime();

			String strTextDate = pstrSqlDate + "";
			String strDate;
			String strTime;

			Boolean blnSuccess;
			String strTmp;
			Int32 intYear = 0;
			Int32 intDay = 0;
			Int32 intMinute = 0;
			Int32 intSecond = 0;

			Int32 lngInstr = StringEx.Instr(strTextDate, " ");
			if (lngInstr != -1) {
				strDate = StringEx.Left(strTextDate, lngInstr - 1);
				strTime = StringEx.Mid(strTextDate, lngInstr + 1);
			} else {
				strDate = strTextDate;
				strTime = "00:00:00";
			}

			//Reconstruct Date
			blnSuccess = false;
			strTmp = StringEx.Left(strDate, 2);
			if (Int32.TryParse(strTmp, out Int32 intMonth)) {
				strTmp = StringEx.Mid(strDate, 4, 2);
				if (Int32.TryParse(strTmp, out intDay)) {
					strTmp = StringEx.Right(strDate, 4);
					if (Int32.TryParse(strTmp, out intYear)) {
						blnSuccess = true;
					}
				}
			}

			if (blnSuccess == true) {
				//Reconstruct Time
				blnSuccess = false;
				strTmp = StringEx.Left(strTime, 2);
				if (Int32.TryParse(strTmp, out Int32 intHour)) {
					strTmp = StringEx.Mid(strTime, 4, 2);
					if (Int32.TryParse(strTmp, out intMinute)) {
						strTmp = StringEx.Right(strTime, 2);
						if (Int32.TryParse(strTmp, out intSecond)) {
							blnSuccess = true;
						}
					}
				}

				if (blnSuccess == true) {
					datRetValue = new DateTime(intYear, intMonth, intDay, intHour, intMinute, intSecond);
				} else {
					//Parse only Date
					//AppEx.Debug.Print("ERROR PARSING DATE 2");
					datRetValue = new DateTime(intYear, intMonth, intDay);
				}
			} else {
				//AppEx.Debug.Print("ERROR PARSING DATE 1");
			}

			return datRetValue;
		} //DateFromText

		//DateTime to SqlString "mm/dd/yyyy Hh:Nn:Ss"
		internal static String ToSqlString(this DateTime pdatSql) {
			String strRetValue = "";
			String strTmp;

			Int32 lngYear = pdatSql.Year;
			Int32 lngMonth = pdatSql.Month;
			Int32 lngDay = pdatSql.Day;

			// mm/dd/yyyy Hh:Nn:Ss
			strTmp = "0" + lngMonth.ToString();
			strRetValue = (strRetValue + StringEx.Right(strTmp, 2)) + "/";

			strTmp = "0" + lngDay.ToString();
			strRetValue = (strRetValue + StringEx.Right(strTmp, 2)) + "/";

			strTmp = lngYear.ToString();
			strRetValue = (strRetValue + strTmp) + " ";

			// mm/dd/yyyy Hh:Nn:Ss
			Int32 lngHour = pdatSql.Hour;
			Int32 lngMinute = pdatSql.Minute;
			Int32 lngSecond = pdatSql.Second;

			strTmp = "0" + lngHour.ToString();
			strRetValue = (strRetValue + StringEx.Right(strTmp, 2)) + ":";

			strTmp = "0" + lngMinute.ToString();
			strRetValue = (strRetValue + StringEx.Right(strTmp, 2)) + ":";

			strTmp = "0" + lngSecond.ToString();
			strRetValue = (strRetValue + StringEx.Right(strTmp, 2));

			return strRetValue;
		}

		//return a 10 digit string
		internal static string ToPhone(string strData, bool pblnFormat = false) {
			string strRetValue = "";

			foreach (char c in strData) {
				if (char.IsDigit(c) == true) {
					strRetValue = strRetValue + c.ToString();
				}
			}
			strRetValue = StringEx.Left(strRetValue, 10);

			if (pblnFormat == true) {
				strRetValue = "(" + StringEx.Left(strRetValue, 3) + ") " + StringEx.Mid(strRetValue, 4, 3) + "-" + StringEx.Right(strRetValue, 4);
			}

			return strRetValue;
		}
	}
}
