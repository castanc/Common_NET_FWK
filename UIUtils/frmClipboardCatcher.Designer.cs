namespace UIUtils
{
    partial class frmClipboardCatcher
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txMsg = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txMsg
            // 
            this.txMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txMsg.Enabled = false;
            this.txMsg.Location = new System.Drawing.Point(0, 0);
            this.txMsg.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txMsg.Multiline = true;
            this.txMsg.Name = "txMsg";
            this.txMsg.ReadOnly = true;
            this.txMsg.Size = new System.Drawing.Size(338, 243);
            this.txMsg.TabIndex = 0;
            this.txMsg.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // frmClipboardCatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 243);
            this.Controls.Add(this.txMsg);
            this.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "frmClipboardCatcher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Clipboard Catcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmClipboardCatcher_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txMsg;
    }
}