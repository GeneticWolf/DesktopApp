using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ShaEx {
	internal static class Program {
		public static ShaEx _frmMain;
		//public static String[] _lstFullFileName = new string[0];
		public static List<String> _lstFullFileName = new List<String>();

		/// <summary>
		/// Point d'entrée principal de l'application.
		/// </summary>
		[STAThread]
		static void Main(String[] args) {
			//Get app GUID
			Assembly assembly = typeof(Program).Assembly;
			GuidAttribute attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
			String appGuid = attribute.Value;

			//Prevent this program to be loaded twice
			using (Mutex mutex = new Mutex(false, appGuid)) {
				//if no instance are running
				if (mutex.WaitOne(0, false)) {
					//GC.Collect();
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);

					if (args.Length != 0) {
						//retrieve the file list
						foreach(String _args in args) {
							if (_args.Trim().Length != 0) {
								_lstFullFileName.Add(_args);
							}
						}
					}

					_frmMain = new ShaEx();
					Application.Run(_frmMain);
				}
			}
		}
	}
}
