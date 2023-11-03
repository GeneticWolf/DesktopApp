namespace ShaEx {
	partial class ShaEx {
		/// <summary>
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Nettoyage des ressources utilisées.
		/// </summary>
		/// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Code généré par le Concepteur Windows Form

		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShaEx));
			this.picSha = new System.Windows.Forms.PictureBox();
			this.cmbFilename = new System.Windows.Forms.ComboBox();
			this.txtHash = new System.Windows.Forms.TextBox();
			this.chkWrite = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.picSha)).BeginInit();
			this.SuspendLayout();
			// 
			// picSha
			// 
			this.picSha.BackColor = System.Drawing.Color.Turquoise;
			this.picSha.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.picSha.Image = ((System.Drawing.Image)(resources.GetObject("picSha.Image")));
			this.picSha.Location = new System.Drawing.Point(12, 12);
			this.picSha.Name = "picSha";
			this.picSha.Size = new System.Drawing.Size(84, 84);
			this.picSha.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.picSha.TabIndex = 0;
			this.picSha.TabStop = false;
			// 
			// cmbFilename
			// 
			this.cmbFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cmbFilename.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbFilename.FormattingEnabled = true;
			this.cmbFilename.Location = new System.Drawing.Point(102, 12);
			this.cmbFilename.Name = "cmbFilename";
			this.cmbFilename.Size = new System.Drawing.Size(527, 28);
			this.cmbFilename.Sorted = true;
			this.cmbFilename.TabIndex = 1;
			// 
			// txtHash
			// 
			this.txtHash.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtHash.Location = new System.Drawing.Point(102, 46);
			this.txtHash.Name = "txtHash";
			this.txtHash.ReadOnly = true;
			this.txtHash.Size = new System.Drawing.Size(527, 26);
			this.txtHash.TabIndex = 2;
			// 
			// chkWrite
			// 
			this.chkWrite.AutoSize = true;
			this.chkWrite.Checked = true;
			this.chkWrite.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkWrite.Location = new System.Drawing.Point(102, 78);
			this.chkWrite.Name = "chkWrite";
			this.chkWrite.Size = new System.Drawing.Size(140, 24);
			this.chkWrite.TabIndex = 3;
			this.chkWrite.Text = "Write Sha to file";
			this.chkWrite.UseVisualStyleBackColor = true;
			// 
			// ShaEx
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(641, 108);
			this.Controls.Add(this.chkWrite);
			this.Controls.Add(this.txtHash);
			this.Controls.Add(this.cmbFilename);
			this.Controls.Add(this.picSha);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "ShaEx";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ShaEx 512/224 v1.0.0";
			((System.ComponentModel.ISupportInitialize)(this.picSha)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox picSha;
		private System.Windows.Forms.ComboBox cmbFilename;
		private System.Windows.Forms.TextBox txtHash;
		private System.Windows.Forms.CheckBox chkWrite;
	}
}

