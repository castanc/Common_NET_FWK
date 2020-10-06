using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIUtils
{
    public partial class frmUserBase : Form
    {
        private frmClipboardCatcher frmCC = null;

        public string WarningON { set; get; }
        public string WarningOff { set; get; }
        public string Warning
        {
            set { btnOpenMonitor.Text = value; }
        }

        public bool MonitorActive
        {
            get
            {
                return frmCC != null;
            }
        }

        public frmUserBase()
        {
            WarningOff = "CLIPBOARD MONITOR IS OFF";
            WarningON = "CLIPBOARD MONITOR IS ON";
            InitializeComponent();
            Warning = "WARNING. CLIPBOARD MONITOR IS ACTIVE";
            refreshMonitorButtons();
        }

        public void HideMonitorForm()
        {
            if (frmCC != null)
            {
                frmCC.Hide();
                frmCC.SendToBack();
            }
        }


        private void refreshMonitorButtons()
        {
            if ( MonitorActive )
            {
                btnOpenMonitor.BackColor = Color.Red;
                btnOpenMonitor.ForeColor = Color.White;
                Warning = WarningON;
            }
            else
            {
                btnOpenMonitor.BackColor = Color.LightGray;
                btnOpenMonitor.ForeColor = Color.Black;
                Warning = WarningOff;
            }
        }

        public void ShowMonitorForm()
        {
            if (frmCC != null)
            {
                frmCC.Show();
                frmCC.BringToFront();
            }

        }

        public virtual void OpenMonitorForm()
        {
            Clipboard.Clear();
            if (frmCC == null)
            {
                frmCC = new frmClipboardCatcher();
                frmCC.OnClipboardCapture += ClipboardCapture;
                frmCC.OnClipboardCaptureImage += ClipboardCaptureImage;
                frmCC.OnFormClosed += FrmCC_OnFormClosed;
            }
            frmCC.Show();
            frmCC.BringToFront();
            refreshMonitorButtons();
        }

        protected virtual void ClipboardCaptureImage(string fName)
        {
            
        }

        protected virtual void FrmCC_OnFormClosed()
        {
            frmCC.OnClipboardCapture -= ClipboardCapture;
            frmCC.OnFormClosed -= FrmCC_OnFormClosed;
            frmCC.Dispose();
            frmCC = null;
            refreshMonitorButtons();
        }

        public virtual void ClipboardCapture(string text)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (frmCC != null)
            {
                frmCC.OnClipboardCapture -= ClipboardCapture;
                frmCC.OnClipboardCaptureImage -= ClipboardCaptureImage;
                frmCC.OnFormClosed -= FrmCC_OnFormClosed;
            }
        }

        //shwo hide other applications
        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/9bde4870-1599-4958-9ab4-902fa98ba53a/how-do-i-maximizeminimize-applications-programmatically-in-c?forum=csharpgeneral
        private void BtnOpenMonitor_Click(object sender, EventArgs e)
        {
            OpenMonitorForm();
            //TODO have to find window by win api and show it
        }

    }
}
