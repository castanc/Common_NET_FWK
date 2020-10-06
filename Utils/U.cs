using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;


namespace Utils
{
    public class U
    {
        public static bool Result { set; get; }
        public static string StartUpPath { set; get; }

        public static bool FileIsValid(string fileName)
        {
            bool result = false;
            try
            {
                if (File.Exists(fileName))
                {
                    //byte[] b = File.ReadAllBytes(fileName);
                    result = true;
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public static string GetWebFile(string url)
        {
            string text = "";
            try
            {
                WebClient wc = new WebClient();
                text = wc.DownloadString(url);
            }
            catch (WebException we)
            {
                // add some kind of error processing
                Result = false;
            }
            return text;
        }

        public static List<string> ArrayToList(string[] arr)
        {
            return arr.OfType<string>().ToList();
        }

        public static DateTime GetFecha(string fecha, string hora = "0:0", string format = "" )
        {
            DateTime dt = new DateTime(1900,1,1);
            fecha = fecha.Replace("-", "/");
            var p = fecha.Split('/');
            int d, m, y,h,mn,sec = 0;
            int p0, p1, p2 = 0;
            bool h24 = false;

            d = 0;m = 0;y = 0;h = 0;mn = 0;sec = 0;
            if ( p.Length == 3 )
            {
                int.TryParse(p[0], out p0);
                int.TryParse(p[1], out p1);
                int.TryParse(p[2], out p2);

                if (p0 >= 1900)
                {
                    y = p0;
                    m = p1;
                    d = p2;
                }
                else
                {
                    y = p2;
                    if (p0 >= 1 && p0 <= 31 && p1 >= 1 && p1 <= 12) { m = p1; d = p0; }
                    else if (p0 >= 1 && p0 <= 12 && p1 >= 1 && p1 <= 31) { m = p0; d = p1; }
                    else
                    {
                        if (format == "DM")
                        {
                            d = p0;
                            m = p1;
                        }
                        else if (format == "MD")
                        {
                            d = p1;
                            m = 0;
                        }
                        else
                            throw new Exception("GetFercha() invalid or empty format for ambigous date.");
                    }
                }


                hora = hora.ToUpper();
                h24 = hora.Contains("PM");
                hora = hora.Replace("AM", "");
                hora = hora.Replace("PM","");

                p = hora.Split(':');

                if (p.Length > 2) int.TryParse(p[2],out sec);
                if ( p.Length > 1 )
                {
                    int.TryParse(p[0], out h);
                    int.TryParse(p[1], out mn);
                }
                if (h24) h = h + 12;
                try
                {
                    dt = new DateTime(y, m, d, h, mn, sec);
                }
                catch(Exception ex )
                {
                    string s = "";
                }
            }

            return dt;
        }

        public static string[] GetFileLines(string fileName)
        {
            string[] lines = { };
            if (File.Exists(fileName))
                lines = File.ReadAllLines(fileName);
            return lines;
        }

        public static string[] GetFileWords(string fileName, char sep = ' ')
        {
            string[] lines = { };
            if (File.Exists(fileName))
                lines = File.ReadAllText(fileName).Split(sep);
            return lines;

        }

        public static void SaveThumbnails(string sourcePath, string ext, int newWidth, int newHeight, SearchOption so = SearchOption.TopDirectoryOnly)
        {
            string[] files = Directory.GetFiles(sourcePath, ext, so);
            string newPath = Path.GetDirectoryName(sourcePath) + "\\Thumbnails";
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);

            foreach (string f in files)
            {
                FileInfo fi = new FileInfo(f);
                //if ("jpg.jpeg.bmp.png.gif".IndexOf(fi.Extension.ToLower())>=0)
                {
                    using (Bitmap bm = CreateThumbnail(f, newWidth, newHeight))
                    {
                        string newName = newPath + "\\" + Path.GetFileName(f);
                        if (bm != null)
                        {
                            bm.Save(newName);
                            FileInfo fi2 = new FileInfo(newName);
                            fi2.LastWriteTime = fi.LastWriteTime;
                        }
                    }
                }
            }
        }
        public static string SaveThumbnail(string fileName, string newPath, int newWidth, int newHeight = 0 )
        {
            string newName = "";
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);

            FileInfo fi = new FileInfo(fileName);
            using (Bitmap bm = CreateThumbnail(fileName, newWidth, newHeight))
            {
                if (bm != null)
                {
                    newName = newPath + "\\" + Path.GetFileName(fileName);
                    bm.Save(newName);
                    FileInfo fi2 = new FileInfo(newName);
                    fi2.LastWriteTime = fi.LastWriteTime;
                }
            }
            return newName;
        }

        public static void DeleteFiles(string fileName )
        {
            string path = Path.GetDirectoryName(fileName);
            string mask = Path.GetFileNameWithoutExtension(fileName) + "*.*";
            if ( Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, mask, SearchOption.TopDirectoryOnly);
                foreach (string f in files)
                {
                    try
                    {
                        File.Delete(f);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public static void DeleteFolder(string path )
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

                foreach (string f in files)
                {
                    try
                    {
                        File.Delete(f);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                string[] dirs = Directory.GetDirectories(path, "*.*", SearchOption.AllDirectories);
                foreach (string d in dirs)
                {
                    try
                    {
                        DeleteFolder(d);
                        Directory.Delete(d);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                try
                {
                    Directory.Delete(path);
                }
                catch (Exception ex)
                { }
            }
        }

        public static void CopyFolder(string source, string target )
        {
            string[] files = Directory.GetFiles(source, "*.*", SearchOption.TopDirectoryOnly);
            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);
            foreach( string f in files )
            {
                string newFile = $"{target}\\{Path.GetFileName(f)}";
                File.Copy(f, newFile, true);
            }
            string[] dirs = Directory.GetDirectories(source, "*.*", SearchOption.TopDirectoryOnly);
            foreach(string d in dirs )
            {
                string dirName = $"{target}\\{Path.GetFileName(d)}";
                CopyFolder(d, dirName );
            }
        }

        public static void CopyFiles(string path, string mask, string target )
        {
            string[] files = Directory.GetFiles(path, mask, SearchOption.TopDirectoryOnly);
            foreach(string f in files )
                CopyTo(f, target);
        }
        public static void CopyTo(string fileName, string folder)
        {
            if (File.Exists(fileName))
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string newName = folder + "\\" + Path.GetFileName(fileName);
                File.Copy(fileName, newName, true);
            }
        }

        public static Bitmap CreateThumbnail(string lcFilename, int lnWidth, int lnHeight = 0)
        {
            System.Drawing.Bitmap bmpOut = null;
            if (File.Exists(lcFilename))
            {
                try
                {
                    Bitmap loBMP = new Bitmap(lcFilename);
                    ImageFormat loFormat = loBMP.RawFormat;

                    decimal lnRatio;
                    int lnNewWidth = 0;
                    int lnNewHeight = 0;

                    //*** If the image is smaller than a thumbnail just return it
                    if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
                        return loBMP;

                    if (loBMP.Width > loBMP.Height)
                    {
                        lnRatio = (decimal)lnWidth / loBMP.Width;
                        lnNewWidth = lnWidth;
                        decimal lnTemp = loBMP.Height * lnRatio;
                        lnNewHeight = (int)lnTemp;
                    }
                    else
                    {
                        decimal lnTemp = 0;
                        if ( lnHeight == 0)
                        {
                            lnRatio = (decimal)640 / 480;
                            lnTemp = lnWidth * lnRatio;
                            lnHeight = (int)lnTemp;
                        }
                        lnRatio = (decimal)lnHeight / loBMP.Height;
                        lnNewHeight = lnHeight;
                        lnTemp = loBMP.Width * lnRatio;
                        lnNewWidth = (int)lnTemp;
                    }
                    bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
                    Graphics g = Graphics.FromImage(bmpOut);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
                    g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);

                    loBMP.Dispose();
                }
                catch
                {
                    return null;
                }
            }

            return bmpOut;
        }

        public static string DateStringYYMM(DateTime dt)
        {

            string m = dt.Month.ToString();
            m = m.PadLeft(2, '0');

            string dts = $"{dt.Year.ToString()}{m}";
            return dts;
        }
        public static string DateStringCompact(string m, DateTime dt)
        {
            string[] months = m.Split(',');
            return $"{dt.Day.ToString()}-{months[dt.Month - 1]}-{dt.Year.ToString()}-{dt.DayOfWeek.ToString().Substring(0, 3)}";
        }

        public static string DateStringCompact(string[] months, DateTime dt)
        {
            return  $"{dt.Day.ToString()}-{months[dt.Month-1]}-{dt.Year.ToString()}-{dt.DayOfWeek.ToString().Substring(0,3)}";
        }


        public static string DateString(DateTime dt, bool time = false)
        {

            string m = dt.Month.ToString();
            m = m.PadLeft(2, '0');
            string day = dt.Day.ToString();
            day = day.PadLeft(2, '0');

            string dts = string.Format("{0}_{1}_{2}-{3}", dt.Year.ToString(),
                m, day, dt.DayOfWeek.ToString());
            if ( time )
            {
                string h = dt.Hour.ToString().PadLeft(2, '0');
                m = dt.Minute.ToString().PadLeft(2, '0');
                string s = dt.Second.ToString().PadLeft(2, '0');
                dts = dts + $" {h}-{m}-{s}";
            }
            return dts;
        }

        public static string DateString()
        {
            DateTime dt = DateTime.Now;
            string m = dt.Month.ToString();
            m = m.PadLeft(2, '0');
            string day = dt.Day.ToString();
            day = day.PadLeft(2, '0');

            return string.Format("{0}_{1}_{2}-{3}", dt.Year.ToString(),
                m, day, dt.DayOfWeek.ToString());
        }
        public static string GetMD5( string fileName )
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream));
                }
            }
        }

        public static List<List<string>> FindDuplicateFiles(string path, string mask)
        {
            SortedDictionary<String, List<string>> results = new SortedDictionary<string, List<string>>();
            List<List<string>> duplicates = new List<List<string>>();
            SortedDictionary<long, List<string>> sameSizeFiles = new SortedDictionary<long, List<string>>();

            string[] files = Directory.GetFiles(path, mask, SearchOption.AllDirectories);
            foreach (string f in files)
            {
                FileInfo fi = new FileInfo(f);
                if (sameSizeFiles.ContainsKey(fi.Length))
                    sameSizeFiles[fi.Length].Add(f);
                else
                {
                    List<string> list = new List<string>
                    {
                        f
                    };
                    sameSizeFiles.Add(fi.Length, list);
                }
            }

            foreach (var ssf in sameSizeFiles)
            {
                if (ssf.Value.Count > 1)
                {
                    foreach (string f in ssf.Value)
                    {
                        string hash = GetMD5(f);
                        if (results.ContainsKey(hash))
                        {
                            results[hash].Add(f);
                        }
                        else
                        {
                            List<String> list = new List<string>
                            {
                                f
                            };
                            results.Add(hash, list);
                        }
                    }
                }

            }
            foreach (var kvp in results)
            {
                if (kvp.Value.Count > 1)
                    duplicates.Add(kvp.Value);
            }
            return duplicates;
        }

        public static List<string> DeleteDuplicateFiles(List<List<string>> duplicates)
        {
            List<string> notDeleted = new List<string>();

            foreach (var d in duplicates)
            {
                for(int i=1; i< d.Count; i++ )
                {
                    try
                    {
                        File.Delete(d[i]);
                    }
                    catch (Exception ex)
                    {
                        notDeleted.Add(d[i]);
                    }
                }
            }
            return notDeleted;
        }

        

        public static bool IsImage(string fileName)
        {
            return ".jpg.jpeg.bmp.gif.tif.png".IndexOf(Path.GetExtension(fileName).ToLower()) >= 0;
        }

        public static bool IsVideo(string fileName)
        {
            return ".mov.mp4.flv.webm.wmv.rm.mpg.mkv".IndexOf(Path.GetExtension(fileName).ToLower()) >= 0;
        }

        public static string FindAttribute(string text, string attr, string ending)
        {
            string result = "";
            int index = text.IndexOf(attr);
            if (index > 0)
            {
                int index2 = text.IndexOf(ending, index);
                if (index2 > index)
                {
                    result = text.Substring(index + attr.Length, index2 - index - attr.Length).Trim();
                }
            }
            return result;
        }

        public static string GenerateRandomString( string min, string max, int len)
        {
            Random rnd = new Random();
            int imin = (int)min[0];
            int imax = (int)max[0];

            byte[] buffer = new byte[len];

            for(int i=0; i<buffer.Length; i++)
                buffer[i] = (byte) rnd.Next(imin,imax);
            string s = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            return s;

        }

        public static string SerializeJson<T>(T obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string s = serializer.Serialize(obj);
            return s;
        }

        public static void DesearializeJson<T>(string jSonData,ref T obj)
        {
            Result = false;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                obj = serializer.Deserialize<T>(jSonData);
                Result = obj != null;
            }
            catch (Exception ex)
            {
                Result = false;
            }
        }

        public static void SerializeToFile<T>( string fileName, T obj)
        {
            try
            {
                File.WriteAllText(fileName, SerializeJson<T>(obj));
            }
            catch(Exception ex )
            {
                string s = "";
            }
        }
          
        public static void DeSerializeFromFile<T>(string fileName, ref T obj)
        {
            Result = true;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                if (File.Exists(fileName))
                    DesearializeJson<T>(File.ReadAllText(fileName), ref obj);
                else Result = false;
            }
            catch(Exception ex)
            {
                Result = false;
            }

        }

        public static T DeSerializeFromFile<T>(string fileName, T refObject )
        {
            Result = true;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (File.Exists(fileName))
                DesearializeJson<T>(File.ReadAllText(fileName), ref refObject);
            else Result = false;
            return refObject;
        }

        //https://stackoverflow.com/questions/4736831/open-webpage-programmatically-and-retrieve-its-html-contain-as-a-string
        public static string GetUrlContent(string url)
        {
            string strResponse = "";
            try
            {
                WebRequest myWebRequest = WebRequest.Create(url);
                WebResponse myWebResponse = myWebRequest.GetResponse();
                Stream ReceiveStream = myWebResponse.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader readStream = new StreamReader(ReceiveStream, encode);
                strResponse = readStream.ReadToEnd();
                //StreamWriter oSw = new StreamWriter(strFilePath);
                //oSw.WriteLine(strResponse);
                //oSw.Close();
                readStream.Close();
                myWebResponse.Close();
            }
            catch (Exception ex)
            {
            }

            return strResponse;
        }

        public static string GetAccentedChar( int KeyValue )
        {
            if (KeyValue == 65 || KeyValue == 97)
                return "á";
            else if (KeyValue == 69 || KeyValue == 101)
                return "é";
            else if (KeyValue == 73 || KeyValue == 105)
                return "í";
            else if (KeyValue == 79 || KeyValue == 111)
                return "ó";
            else if (KeyValue == 85 || KeyValue == 117)
                return "ú";
            else if (KeyValue == 78 || KeyValue == 110)
                return "ñ";
            return "";
        }

        public static string ExtractPositioned(string text, string start, string end = "", int beforeIndex = 0, bool caseSensitive = true)
        {
            int index = 0;
            int index2 = 0;
            int index0 = 0;
            string result = "";

            if (!caseSensitive)
            {
                text = text.ToLower();
                start = start.ToLower();
                end = end.ToLower();
            }

            index = text.IndexOf(start,beforeIndex);
            if (index >= beforeIndex)
            {
                index0 = index;
                index += start.Length;
                index2 = text.IndexOf(end, index);
                if (index2 > index)
                {
                    result = text.Substring(index, index2 - index);
                    if (string.IsNullOrEmpty(result))
                    {
                        index++;
                        result = text.Substring(index, index2 - index);
                    }
                }
            }
            return result;
        }


        public static string Extract(string text, string start, string end="",  bool caseSensitive = false)
        {
            int index = 0;
            int index2 = 0;
            int index0 = 0;
            string result = "";

            if ( !caseSensitive)
            {
                text = text.ToLower();
                start = start.ToLower();
                end = end.ToLower();
            }

            index = text.IndexOf(start);
            if ( index >= 0 )
            {
                index0 = index;
                index += start.Length;
                index2 = text.IndexOf(end, index);
                if ( index2 > index )
                {
                    result = text.Substring(index, index2 - index);
                    if ( string.IsNullOrEmpty(result))
                    {
                        index++;
                        result = text.Substring(index, index2 - index);
                    }
                }
            }
            return result;
        }
    }
}
