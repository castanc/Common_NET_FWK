using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using CCY.Common.WindowsAPI;


namespace UIUtils
{
    public delegate void delegateClipboardCapturedText(string text);
    public delegate void delegateFormClosed();
    public partial class frmClipboardCatcher : Form
    {

        public string Message
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    txMsg.Text = value;
            }
            get { return txMsg.Text; }
        }
        #region CONSTRUCTOR
        public frmClipboardCatcher()
        {
            InitializeComponent();
            Message = @"Clipboard text is being monitored while this window is open.  To disable it, close this window.";
        }
        #endregion



        public string ClipboardText = "";
        public event delegateClipboardCapturedText OnClipboardCapture = null;
        public event delegateFormClosed OnFormClosed = null;

        IntPtr _ClipboardViewerNext;
        Queue _hyperlink = new Queue();
        string lastData = "";

        private void RegisterClipboardViewer()
        {
            _ClipboardViewerNext = WAPI.SetClipboardViewer(this.Handle);
        }

        /// <summary>
        /// Remove this form from the Clipboard Viewer list
        /// </summary>
        private void UnregisterClipboardViewer()
        {
            WAPI.ChangeClipboardChain(this.Handle, _ClipboardViewerNext);
        }


        private bool ClipboardSearch(IDataObject iData)
        {
            bool FoundNewLinks = false;
            //
            // If it is not text then quit
            // cannot search bitmap etc
            //
            if (iData.GetDataPresent(DataFormats.Bitmap))
            {
                OnClipboardCapture?.Invoke("GET_IMAGE");
                return true;
            }

            if (!iData.GetDataPresent(DataFormats.Text))
            {
                return false;
            }

            try
            {
                ClipboardText = (string)iData.GetData(DataFormats.Text);
                OnClipboardCapture?.Invoke(ClipboardText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            return FoundNewLinks;
        }

        private void GetClipboardData()
        {
            //
            // Data on the clipboard uses the 
            // IDataObject interface
            //
            IDataObject iData = new DataObject();
            string strText = "clipmon";

            try
            {
                iData = Clipboard.GetDataObject();
            }
            catch (System.Runtime.InteropServices.ExternalException externEx)
            {
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            // 
            // Get Text if it is present
            //
            if (iData.GetDataPresent(DataFormats.Text))
            {
                //ctlClipboardText.Text = (string)iData.GetData(DataFormats.Text);
                strText = "Text";
                Debug.WriteLine((string)iData.GetData(DataFormats.Text));
            }

            if (ClipboardSearch(iData))
            {
                //
                // Found some new links
                //
                System.Text.StringBuilder strBalloon = new System.Text.StringBuilder(100);

                foreach (string objLink in _hyperlink)
                {
                    strBalloon.Append(objLink.ToString() + "\n");
                }

                //FormatMenuBuild(iData);
                //SupportedMenuBuild(iData);
                //ContextMenuBuild();

                if (_hyperlink.Count > 0)
                {
                    //notAreaIcon.BalloonDisplay(NotificationAreaIcon.NOTIFYICONdwInfoFlags.NIIF_INFO, "Links", strBalloon.ToString());
                }
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterClipboardViewer();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            RegisterClipboardViewer();
        }
        //https://web.archive.org/web/20131104125500/http://www.radsoftware.com.au/articles/clipboardmonitor.aspx
        protected override void WndProc(ref Message m)
        {
            switch ((Msgs)m.Msg)
            {
                //
                // The WM_DRAWCLIPBOARD message is sent to the first window 
                // in the clipboard viewer chain when the content of the 
                // clipboard changes. This enables a clipboard viewer 
                // window to display the new content of the clipboard. 
                //
                case Msgs.WM_DRAWCLIPBOARD:

                    //Debug.WriteLine("WindowProc DRAWCLIPBOARD: " + m.Msg, "WndProc");

                    GetClipboardData();

                    //
                    // Each window that receives the WM_DRAWCLIPBOARD message 
                    // must call the SendMessage function to pass the message 
                    // on to the next window in the clipboard viewer chain.
                    //
                    WAPI.SendMessage(_ClipboardViewerNext, m.Msg, m.WParam, m.LParam);
                    break;


                //
                // The WM_CHANGECBCHAIN message is sent to the first window 
                // in the clipboard viewer chain when a window is being 
                // removed from the chain. 
                //
                case Msgs.WM_CHANGECBCHAIN:
                    Debug.WriteLine("WM_CHANGECBCHAIN: lParam: " + m.LParam, "WndProc");

                    // When a clipboard viewer window receives the WM_CHANGECBCHAIN message, 
                    // it should call the SendMessage function to pass the message to the 
                    // next window in the chain, unless the next window is the window 
                    // being removed. In this case, the clipboard viewer should save 
                    // the handle specified by the lParam parameter as the next window in the chain. 

                    //
                    // wParam is the Handle to the window being removed from 
                    // the clipboard viewer chain 
                    // lParam is the Handle to the next window in the chain 
                    // following the window being removed. 
                    if (m.WParam == _ClipboardViewerNext)
                    {
                        //
                        // If wParam is the next clipboard viewer then it
                        // is being removed so update pointer to the next
                        // window in the clipboard chain
                        //
                        _ClipboardViewerNext = m.LParam;
                    }
                    else
                    {
                        WAPI.SendMessage(_ClipboardViewerNext, m.Msg, m.WParam, m.LParam);
                    }
                    break;

                default:
                    //
                    // Let the form process the messages that we are
                    // not interested in
                    //
                    base.WndProc(ref m);
                    break;

            }

        }

        private void FrmClipboardCatcher_FormClosed(object sender, FormClosedEventArgs e)
        {
            OnFormClosed?.Invoke();
        }
    }
}
