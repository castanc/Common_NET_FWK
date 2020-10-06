using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIUtils
{
    public partial class frmUserBase : Form
    {
        private frmClipboardCatcher frmCC = null;

        public frmUserBase()
        {
            InitializeComponent();
        }

        public void HideMonitorForm()
        {
            if (frmCC != null)
                frmCC.Visible = false;
        }

        public void ShowMonitorForm()
        {
            if (frmCC != null)
                frmCC.Visible = true;
        }

        public virtual void OpenMonitorForm()
        {
            if (frmCC == null)
            {
                frmCC = new frmClipboardCatcher();
                frmCC.OnClipboardCapture += ClipboardCapture;
                frmCC.OnFormClosed += FrmCC_OnFormClosed;
            }
            frmCC.Top = this.Top + 100;
            frmCC.Left = this.Left + 100;
            frmCC.Show();
        }

        private void FrmCC_OnFormClosed()
        {
            frmCC.OnClipboardCapture -= ClipboardCapture;
            frmCC.OnFormClosed -= FrmCC_OnFormClosed;
            frmCC.Dispose();
            frmCC = null;
        }

        public virtual void ClipboardCapture(string text)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (frmCC != null)
            {
                frmCC.OnClipboardCapture -= ClipboardCapture;
                frmCC.OnFormClosed -= FrmCC_OnFormClosed;
            }
        }

    }
}
