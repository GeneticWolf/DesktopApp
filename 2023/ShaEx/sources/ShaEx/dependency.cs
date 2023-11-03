using PrototypeOmega;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaEx {
	//http://forums.codeguru.com/showthread.php?402926-What-is-equivalent-of-Combobox-ItemData-property
	//https://exceptionshub.com/combobox-adding-text-and-value-to-an-item-no-binding-source.html
	internal class ShaExFileInfo {
		private String strFullFileName = "";

		public String FullFileName { 
			get {
				return strFullFileName;
			}

			set {
				strFullFileName = "";

				Int32 lngInstr = StringEx.InstrRev(value, "\\");
				if (lngInstr > -1) {
					strFullFileName = value;
					FileName = StringEx.Mid(value, lngInstr + 1);
					FilePath = StringEx.Left(value, lngInstr);
				}
			}
		}

		public String FileName { get; set; } = "";
		public String FilePath { get; set; } = "";
		public String FileHash { get; set; } = "";

		//What will appear as TEXT in ComboBox
		public override String ToString() {
			return this.FileName;
		}
	}
}
