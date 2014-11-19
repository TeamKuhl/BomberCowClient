namespace BomberCowClient
{
    partial class frmMain
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtdummy = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtdummy
            // 
            this.txtdummy.Enabled = false;
            this.txtdummy.Location = new System.Drawing.Point(-12, -24);
            this.txtdummy.Name = "txtdummy";
            this.txtdummy.Size = new System.Drawing.Size(10, 20);
            this.txtdummy.TabIndex = 0;
            this.txtdummy.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtdummy_KeyDown);
            this.txtdummy.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtdummy_KeyUp);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(697, 322);
            this.Controls.Add(this.txtdummy);
            this.Name = "frmMain";
            this.Text = "BomberCow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtdummy;




    }
}

