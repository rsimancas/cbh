namespace Utilidades
{
    using System;
    using System.IO;
    using System.Web;

    public static class LogManager
    {
        //AppDomain.CurrentDomain.BaseDirectory

        public static string _path;
        public static string _filename;
        public static DateTime _datelog;

        static LogManager()
        {
            _path = Path.Combine(HttpContext.Current.Request.MapPath("~/App_Data/Logs/"));
            _filename = Path.Combine(_path, "CBH.LOG" + DateTime.Now.ToString("yyyyMMdd HH") + ".txt");
            _datelog = DateTime.Now;
        }

        public static void Write(params object[] args)
        {
            if (_datelog.ToString("yyyyMMdd HH") != DateTime.Now.ToString("yyyyMMdd HH"))
            {
                _filename = Path.Combine(_path, "CBH.LOG" + DateTime.Now.ToString("yyyyMMdd HH") + ".txt");
            }

            try
            {
                while (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }

                while (!File.Exists(_filename))
                {
                    StreamWriter ws = new StreamWriter(_filename);
                    ws.WriteLine("INICIO: " + DateTime.Now.ToString());
                    ws.Close();
                }
            }
            catch
            {
                // throw
            }

            string text = "";
            int i = 0;
            foreach (var arg in args)
            {
                text += String.Format(" {0}", arg);
                i++;
            }

            if (!String.IsNullOrEmpty(text))
            {
                text = text.Substring(1);
                text = String.Format(text, args);
            }

            //ReplicacionService.WriteToLog(text);
            StreamWriter SW = File.AppendText(_filename);
            SW.WriteLine(DateTime.Now.ToLongTimeString() + ": " + text);
            SW.Close();
        }

        public static void WriteText(string text)
        {
            if (_datelog.ToString("yyyyMMdd HH") != DateTime.Now.ToString("yyyyMMdd HH"))
            {
                _filename = Path.Combine(_path, "CBH.LOG" + DateTime.Now.ToString("yyyyMMdd HH") + ".txt");
            }

            try
            {
                while (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }

                while (!File.Exists(_filename))
                {
                    StreamWriter ws = new StreamWriter(_filename);
                    ws.WriteLine("INICIO: " + DateTime.Now.ToString());
                    ws.Close();
                }
            }
            catch
            {
                // throw
            }

            //ReplicacionService.WriteToLog(text);
            StreamWriter SW = File.AppendText(_filename);
            SW.WriteLine(DateTime.Now.ToLongTimeString() + ": " + text);
            SW.Close();
        }
    }
}

