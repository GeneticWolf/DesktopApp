using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Security.Authentication;
using PrototypeOmega;

namespace ShaEx {
	public partial class ShaEx : Form {
		private Int32 glngJobState = 0;  //not working
		private string gstrFormCaption = "ShaEx 512/224 v1.0.0";
		
		public ShaEx() {
			InitializeComponent();

			//set original caption
			this.Text = gstrFormCaption;
			this.Autotest();

			#region converted to ShaEx
			// Enable drag-and-drop operations and 
			// add handlers for DragLeave, DragEnter and DragDrop
			this.picSha.AllowDrop = true;
			//this.picSha.DragEnter += new DragEventHandler(this.PicSha_DragEnter);
			//this.picSha.DragLeave += new EventHandler(this.PicSha_DragLeave);
			//this.picSha.DragDrop += new DragEventHandler(this.PicSha_DragDrop);
			this.picSha.DragEnter += PicSha_DragEnter;
			this.picSha.DragLeave += PicSha_DragLeave;
			this.picSha.DragDrop += PicSha_DragDrop;
			#endregion

			// Add Mandatory handling
			this.cmbFilename.SelectedIndexChanged += CmbFilename_SelectedIndexChanged;
			this.picSha.DoubleClick += PicSha_DoubleClick;
			this.FormClosing += ShaEx_FormClosing;
			this.Shown += ShaEx_Shown;
			this.txtHash.Click += TxtHash_Click;
		}

		private void Autotest() {
			String strTest = "";

			strTest = Sha512Ex.Sha512("Hello", Sha512Ex.eSha512.L224);
			if (strTest != "0d075258abfd1f8b81fc0a5207a1aa5cc82eb287720b1f849b862235") {
				throw new Exception("512/224: " + strTest);
			}

			strTest = Sha512Ex.Sha512("Hello", Sha512Ex.eSha512.L256);
			if (strTest != "7e75b18b88d2cb8be95b05ec611e54e2460408a2dcf858f945686446c9d07aac") {
				throw new Exception("512/256: " + strTest);
			}

			strTest = Sha512Ex.Sha512("Hello", Sha512Ex.eSha512.L384);
			if (strTest != "3519fe5ad2c596efe3e276a6f351b8fc0b03db861782490d45f7598ebd0ab5fd5520ed102f38c4a5ec834e98668035fc") {
				throw new Exception("512/384: " + strTest);
			}

			strTest = Sha512Ex.Sha512("Hello", Sha512Ex.eSha512.L512);
			if (strTest != "3615f80c9d293ed7402687f94b22d58e529b8cc7916f8fac7fddf7fbd5af4cf777d3d795a7a00a16bf7e7f3fb9561ee9baae480da9fe7a18769e71886b03f315") {
				throw new Exception("512/512: " + strTest);
			}

			//DebugEx.WriteLineEx("Ok");
		}

		private void TxtHash_Click(object sender, EventArgs e) {
			this.txtHash.SelectionStart = 0;
			this.txtHash.SelectionLength = this.txtHash.Text.Length;
			ClipboardEx.SetText(this.txtHash.Text);
		}

		private void ShaEx_FormClosing(object sender, FormClosingEventArgs e) {
			//Check if currently computing Sha
			if (this.glngJobState != 0) {
				//ask to cancel current job and unload
				this.glngJobState = 2;

				//prevent form closing until file process is cancelled
				e.Cancel = true;
			}
		}

		#region converted to ShaEx
		private void PicSha_DragEnter(object sender, DragEventArgs e) {
			this.picSha.BackColor = Color.FromArgb(255, 0, 0, 255);
			//Check if it's a file drop
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				//allow drop
				e.Effect = DragDropEffects.All;
			} else {
				//deny drop
				e.Effect = DragDropEffects.None;
			}
		}

		private void PicSha_DragLeave(object sender, EventArgs e) {
			this.picSha.BackColor = Color.FromArgb(255, 64, 224, 208);
		}
		#endregion



		private void PicSha_DragDrop(object sender, DragEventArgs e) {
			this.cmbFilename.Items.Clear();

			//retrieve the dropped file list
			String[] args = (String[])e.Data.GetData(DataFormats.FileDrop, false);
			Program._lstFullFileName = new List<String>();
			foreach (String _args in args) {
				if (_args.Trim().Length != 0) {
					Program._lstFullFileName.Add(_args);
				}
			}
			ProcessFileListA();
		}

		private void ProcessFileListA(Boolean pblnRunFromExplorer = false) {
			if (Program._lstFullFileName.Count > 0) {
				this.picSha.BackColor = Color.FromArgb(255, 64, 224, 208);

				String strTotalFiles = Program._lstFullFileName.Count.ToString();
				foreach (String strFullFileName in Program._lstFullFileName) {
					ShaExFileInfo objComboItem = new ShaExFileInfo();
					objComboItem.FullFileName = strFullFileName;
					if (objComboItem.FullFileName != "") {
						this.cmbFilename.Items.Add(objComboItem);
					}
				}

				Int32 lngCount = 0;
				foreach (ShaExFileInfo objComboItem in this.cmbFilename.Items) {
					lngCount++;
					this.Text = gstrFormCaption + " - " + lngCount.ToString() + "/" + strTotalFiles;
					this.ProcessShaEx(objComboItem);

					//check if an unload request was issued
					if (this.glngJobState == 2) {
						//cancel remaining job
						break;
					}
				}

				//return original caption
				this.Text = gstrFormCaption;

				if (pblnRunFromExplorer == true) {
					this.glngJobState = 2;
				}

				//check if a cancel request was issued
				this.CheckUnload();
			}
		}

		private void ShaEx_Shown(object sender, EventArgs e) {
			ProcessFileListA(true);
		}

		private void CmbFilename_SelectedIndexChanged(object sender, EventArgs e) {
			if (this.cmbFilename.SelectedIndex > -1) {
				ShaExFileInfo objFileInfo = (ShaExFileInfo)(this.cmbFilename.Items[this.cmbFilename.SelectedIndex]);

				//Retrieve Hash
				this.txtHash.Text = objFileInfo.FileHash;
			}
		}

		private void PicSha_DoubleClick(object sender, EventArgs e) {
			//Clear current data
			this.cmbFilename.Items.Clear();
			this.txtHash.Text = "";
		}

		private void CheckUnload() {
			//check if an unload request was issued
			if (this.glngJobState == 2) {
				this.glngJobState = 0;
				Program._frmMain.Close();
			}
		}

		private void WriteShaFile(String pstrFilePath, String pstrFileName, String pstrFileHash) {
			String strShaFile = pstrFilePath + pstrFileName + ".sha";

			//Write a new file
			try {
				//Pass the filepath and filename to the StreamWriter Constructor
				using (StreamWriter objWriter = new StreamWriter(strShaFile)) {
					//Write a line of text
					objWriter.WriteLine("; ShaEx by Patrice Charbonneau (c) 2023");
					objWriter.WriteLine(pstrFileHash + " *" + pstrFileName);

					//Close the file
					objWriter.Close();
				}
			} catch (Exception e) {
				MessageBox.Show("Error:  [" + e.Message + "] trying to create .sha file");
			}
		}

		private void UpdateShaFile(String pstrFilePath, String pstrFileName, String pstrFileHash) {
			String strFullFilePath = pstrFilePath + pstrFileName;

			//Check if we need to verify old content and update
			if (AppEx.FileExist64(strFullFilePath + ".sha") == true) {
				String strOldShaValue = "";

				//Pass the file path and file name to the StreamReader constructor
				using (StreamReader objReader = new StreamReader(strFullFilePath + ".sha")) {
					while (objReader.EndOfStream != true) {
						String strLineIn = objReader.ReadLine();
						strLineIn = strLineIn.Trim();
						if (StringEx.Left(strLineIn, 1) != ";") {
							//we assume it is our Sha line
							Int32 lngInstr = StringEx.Instr(strLineIn, "*");
							if (lngInstr > 0) {
								strOldShaValue = StringEx.Left(strLineIn, (lngInstr - 1)).Trim();
							}
							break;
						}
					}
					//close the file
					objReader.Close();
				}

				//check if invalid ShaEx in the .sha file
				if (strOldShaValue != pstrFileHash) {
					//Check if Sha256 (old Format)
					if (strOldShaValue.Length == 64) {
						string strSha256Value = this.ShaCheckSum(strFullFilePath, pstrFileName, HashAlgorithmType.Sha256);
						if (strOldShaValue == strSha256Value) {
							//Upgrade automatiquely to Sha512/224
							WriteShaFile(pstrFilePath, pstrFileName, pstrFileHash);
							MessageBox.Show("Sha validated and updated to Sha512/224");
						} else {
							MessageBox.Show("An invalid signature was detected in the .sha file, plz erase the file to update it");
						}
					//Check if Sha384 (old Format)
					} else if (strOldShaValue.Length == 96) {
						string strSha384Value = this.ShaCheckSum(strFullFilePath, pstrFileName, HashAlgorithmType.Sha384);
						if (strOldShaValue == strSha384Value) {
							//Upgrade automatiquely to Sha512/224
							WriteShaFile(pstrFilePath, pstrFileName, pstrFileHash);
							MessageBox.Show("Sha validated and updated to Sha512/224");
						} else {
							MessageBox.Show("An invalid signature was detected in the .sha file, plz erase the file to update it");
						}
					} else {
						MessageBox.Show("An invalid signature was detected in the .sha file, plz erase the file to update it");
					}
				}
			} else {
				WriteShaFile(pstrFilePath, pstrFileName, pstrFileHash);
			}
		}

		private void ProcessShaEx(ShaExFileInfo objComboItem) {
			//ensure we're not processing a .sha file
			if (StringEx.Right(objComboItem.FullFileName, 4).ToLower() != ".sha") {
				this.glngJobState = 1;  //Setting Work State
				this.cmbFilename.Text = objComboItem.FileName;

				String strFileHash = this.ShaCheckSum(objComboItem.FullFileName, objComboItem.FileName, HashAlgorithmType.Sha512);
				if (this.glngJobState == 1) {
					objComboItem.FileHash = strFileHash;
					//this.cmbFilename.Items.Add(objComboItem);

					//Check to update .sha file
					this.UpdateShaFile(objComboItem.FilePath, objComboItem.FileName, strFileHash);
				}

				if (this.glngJobState == 1) {
					//Force Refreshing sha info
					Int32 lngSelectedIndex = this.cmbFilename.SelectedIndex;
					this.cmbFilename.SelectedIndex = -1;
					this.cmbFilename.SelectedIndex = lngSelectedIndex;

					//Set back to idle state
					this.glngJobState = 0;
				}
			}
		}

		//https://docs.microsoft.com/en-us/dotnet/api/system.io.filestream?view=netframework-4.8
		//http://www.alexandre-gomes.com/?p=144
		public String ShaCheckSum(String pstrFullFilePath, String pstrFileName, HashAlgorithmType penmShaType) {
			const Int32 BUFFER_MAX_SIZE = (4 * 1024 * 1000);  //4Mo
			String strRetValue = "";
			String strBaseText = this.Text;
			Int64 dblProgression1;
			Int64 dblProgression2 = 0;

			this.Text = strBaseText + " [0%]";
			//this.cmbFilename.Text = strBaseText;

			//Should check if file exist first
			if (AppEx.FileExist64(pstrFullFilePath) == true) {
				//using (SHA384 objSHA = SHA384.Create())
				//HashAlgorithm
				HashAlgorithm objSHA;

				switch (penmShaType) {
					case HashAlgorithmType.Sha256:
						objSHA = SHA256.Create();
						break;

					case HashAlgorithmType.Sha384:
						objSHA = SHA384.Create();
						break;

					case HashAlgorithmType.Sha512:
						objSHA = SHA512.Create();
						break;

					default:  //512
						objSHA = SHA512.Create();
						break;
				}

				using (FileStream objFileStream = new FileStream(pstrFullFilePath, FileMode.Open, FileAccess.Read)) {
					Int32 _bufferSize;
					Byte[] readAheadBuffer;
					Int32 readAheadBytesRead;
					Int64 lngBytesRemaining = objFileStream.Length;
					Double dblTotalBytes = lngBytesRemaining;

					while ((lngBytesRemaining > 0) && (this.glngJobState != 2)) {
						if (lngBytesRemaining > BUFFER_MAX_SIZE) {
							_bufferSize = BUFFER_MAX_SIZE;
						} else {
							_bufferSize = (Int32)lngBytesRemaining;
						}

						readAheadBuffer = new Byte[_bufferSize];
						readAheadBytesRead = objFileStream.Read(readAheadBuffer, 0, _bufferSize);

						lngBytesRemaining = (lngBytesRemaining - _bufferSize);
						if (lngBytesRemaining != 0) {
							objSHA.TransformBlock(readAheadBuffer, 0, readAheadBytesRead, readAheadBuffer, 0);
						} else {
							objSHA.TransformFinalBlock(readAheadBuffer, 0, readAheadBytesRead);
							strRetValue = BitConverter.ToString(objSHA.Hash).Replace("-", "").ToLower();
						}

						dblProgression1 = (Int64)(((dblTotalBytes - lngBytesRemaining) / dblTotalBytes) * 100);
						if (dblProgression1 != dblProgression2) {
							dblProgression2 = dblProgression1;
							this.Text = strBaseText + " [" + dblProgression2.ToString() + "%]";
							Application.DoEvents();
						}
					}
				}
				objSHA.Dispose();
			}
			this.Text = strBaseText + " [100%]";

			return strRetValue;
		}
	}
}
