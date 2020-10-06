using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace CCY.Utils.PrintScreen
{
    public class PrintScreen
    {
        public static string BasePath { set; get; }
        public static int Count { set; get; }
        public static string Prefix { set; get; }
        public static string PostFix { set; get; }
        public static int ScreenNum { set; get; }
        public static Exception Ex { set; get; }
        public static string Legend { set; get; }
        public static string FileName { set; get; }
        public static Rectangle ForcedBounds { set; get; }
        public static bool ForceResolution { set; get; }
        public static int TextBarHeight { set; get; }
        public static string Annotation { set; get; }
        public static Rectangle PrintArea;

        public PrintScreen()
        {
            if (string.IsNullOrEmpty(BasePath))
            {
                BasePath = "c:\\temp";
                Count = 0;
                Prefix = "PS_";
                PostFix = string.Empty;
                Ex = null;
                Legend = string.Empty;
                FileName = string.Empty;
                TextBarHeight = 0;
                Annotation = string.Empty;
            }
        }
        public static string SaveImage(Bitmap image)
        {
            string path = "";
            int padLen = 7;

            DateTime dt = DateTime.Now;
            path = BasePath; 
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Count = Directory.GetFiles(path, "*.jpg").Length + 1;
            FileName = path + "\\" + Prefix + Count.ToString().PadLeft(padLen, '0') + "_" +
                 ScreenNum.ToString() +"_" + PostFix +  ".jpg";
            while (File.Exists(FileName))
            {
                Count++;
                FileName = path + Prefix + Count.ToString().PadLeft(padLen, '0') + 
                    "_" + ScreenNum.ToString() + "_" + PostFix + ".jpg";
            }
            try
            {
                image.Save(FileName, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                Ex = ex;
                MessageBox.Show(ex.Message, "Error saving:\n" + FileName);
            }
            return FileName;
        }

        public static string PrintToFile(string path, int screenNum = 0, 
            bool writeText = false)
        {
            BasePath = path;
            Count = 0;
            Prefix = "PS_";
            PostFix = string.Empty;
            Ex = null;
            Legend = string.Empty;
            FileName = string.Empty;
            TextBarHeight = 0;
            Annotation = string.Empty;

            if ((screenNum < 0) || (screenNum >= Screen.AllScreens.Length))
                screenNum = 0;

            if (!Directory.Exists(BasePath))
                Directory.CreateDirectory(BasePath);

            ScreenNum = screenNum;
            Bitmap image = Capture(screenNum);
            FileName = SaveImage(image);
            if (writeText &&  Clipboard.ContainsText())
            {
                File.WriteAllText( FileName + ".txt", Clipboard.GetText());
                Clipboard.Clear();
            }
            return FileName;
        }


        public static string PrintToFile(int screenNum)
        {
            string path = string.Empty;

            if (string.IsNullOrEmpty(BasePath))
            {
                BasePath = "c:\\temp";
                Count = 0;
                Prefix = "PS_";
                PostFix = string.Empty;
                Ex = null;
                Legend = string.Empty;
                FileName = string.Empty;
                TextBarHeight = 0;
                Annotation = string.Empty;
            }
            try
            {
                if (!Directory.Exists(BasePath))
                    Directory.CreateDirectory(BasePath);

                ScreenNum = screenNum;
                Bitmap image = Capture(screenNum);
                FileName = SaveImage(image);
                if (Clipboard.ContainsText())
                {
                    File.WriteAllText(FileName + ".txt", Clipboard.GetText());
                    Clipboard.Clear();
                }
            }
            catch(Exception ex )
            {
                FileName = "";
            }
            return FileName;
        }

        public static Bitmap CaptureWindow(Control window, Rectangle rc)
        {
            Bitmap memoryImage = null;

            // Create new graphics object using handle to window.
            using (Graphics graphics = window.CreateGraphics())
            {
                memoryImage = new Bitmap(rc.Width,
                              rc.Height, graphics);

                using (Graphics memoryGrahics =
                        Graphics.FromImage(memoryImage))
                {
                    memoryGrahics.CopyFromScreen(rc.X, rc.Y,
                       0, 0, rc.Size, CopyPixelOperation.SourceCopy);
                }
            }
            return memoryImage;
        }



        public static Bitmap Capture(int screenNum)
        {
            Rectangle rc;
            // used to capture then screen in memory
            Bitmap memoryImage = null;
            // number of screens to capture,
            // will be updated below if necessary


            var images = new Bitmap[Screen.AllScreens.Length];
            try
            {
                Screen[] screens = Screen.AllScreens;
                if (screenNum >= screens.Length)
                    screenNum = screens.Length - 1;


                if (screenNum < 0)
                        rc = SystemInformation.VirtualScreen;
                else
                        rc = screens[screenNum].Bounds;

                //if (PrintArea != null)
                //    rc = PrintArea;


                if (ForceResolution)
                    if (screenNum < 0)
                        rc = new Rectangle(-2200, -61, 2200 * 2, 1200 + TextBarHeight);
                    else if (screenNum == 0)
                        rc = new Rectangle(-1920, -61, 1920, 1080 + TextBarHeight);
                    else if (screenNum == 1)
                        rc = new Rectangle(0, 0, 1920, 1080 + TextBarHeight);


                // allocate a member for saving the captured image(s)
                memoryImage = new Bitmap(rc.Width, rc.Height,
                              PixelFormat.Format32bppArgb);
                using (Graphics memoryGrahics =
                        Graphics.FromImage(memoryImage))
                {
                    // copy the screen data
                    // to the memory allocated above
                    memoryGrahics.CopyFromScreen(rc.X, rc.Y,
                       0, 0, rc.Size, CopyPixelOperation.SourceCopy);
                }
            }
            catch (Exception ex)
            {
                // handle any erros which occured during capture
                Ex = ex;
            }
            return memoryImage;
        }

        /*
         * write text to image
         * http://stackoverflow.com/questions/6311545/c-sharp-write-text-on-bitmap
         * 
         * Bitmap bmp = new Bitmap("filename.bmp");
            RectangleF rectf = new RectangleF(70, 90, 90, 50);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString("yourText", new Font("Tahoma",8), Brushes.Black, rectf);

            g.Flush();

         */

        /*
         global hotkeys

        http://www.codeproject.com/Articles/442285/Global-Shortcuts-in-WinForms-and-WPF
        
        save outlook mails
        https://msdn.microsoft.com/en-us/library/ms268998.aspx
         */



    }
}
