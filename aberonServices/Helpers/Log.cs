using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace aberonServices.Utils
{
    public class LogEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
    class Log
    {
        static public EventHandler<LogEventArgs> LogAppended;
        private static DateTime NextDeleteLogDate = DateTime.Now;
        private static string lockHelper="";
        static public void Write(string message)
        {
            Write(message, null);
        }

        static public void Write(string message,Exception ex)
        {
            string finalMsg = message;
            Exception innerEx = ex;            
            if (ex != null) {
                while (innerEx.InnerException != null) innerEx = innerEx.InnerException;
                finalMsg += " " + innerEx.Message + " " + innerEx.StackTrace;
            }
            WriteLogFile(FormatMessage(finalMsg));
            LogAppended?.Invoke(null, new LogEventArgs() { Message = finalMsg });
        }

        public static string FormatMessage(string message)
        {
            return DateTime.Now.ToShortDateString() + "\t" +
                DateTime.Now.ToShortTimeString() + "\t" +
                message;
        }


        private static void WriteLogFile(string sLog)
        {
            lock (lockHelper) {
                if (LogFolder == null) {
                    return;
                }
                PrepareDiskLogFile();
                string fileName = string.Format("{0}.{1}.{2}.log.txt", DateTime.Now.Year.ToString(),
                    DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString().PadLeft(2, '0'));
                using (StreamWriter sw = new StreamWriter(Path.Combine(LogFolder, fileName), true)) {
                    sw.WriteLine(sLog);
                }
            };
        }

        private static void PrepareDiskLogFile()
        {
            if (LogFolder == null) {
                return;
            }
            if (NextDeleteLogDate > DateTime.Now.Date) return;
            DateTime minDate = DateTime.Now.AddMonths(-1);
            string earliestFilePrefix = string.Format("{0}.{1}.{2}.", minDate.Year.ToString(),
                minDate.Month.ToString().PadLeft(2, '0'), minDate.Day.ToString().PadLeft(2, '0'));
            string[] files = Directory.GetFiles(LogFolder, "*.log.txt");
            for (int i = 0; i < files.Length; i++) {
                if (Path.GetFileName(files[i]).Substring(0, 11).CompareTo(earliestFilePrefix) < 0) {
                    File.Delete(files[i]);
                }
            }
            NextDeleteLogDate = DateTime.Now.Date.AddDays(1);
        }

        private static string LogFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["XmlFolder"];
            }
        }


        private static string DayFolder(string logFolder)
        {
            string folderName = DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day;
            folderName = Path.Combine(logFolder, folderName);
            if (!Directory.Exists(folderName)) {
                Directory.CreateDirectory(folderName);
            }
            return folderName;
         
        }
        public static string WriteFile(string folder,string prefix,string data)
        {
            string fileName = prefix + "-" + DateTime.Now.Ticks;
            using (StreamWriter sw = new StreamWriter(Path.Combine(DayFolder(LogFolder+ folder), fileName), true)) {
                sw.WriteLine(data);
            }
            return fileName;
        }
        public static string ConvertToXml(string prefix,object obj)
        {
            string xml;
            StringBuilder xmlStringBuilder = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(xmlStringBuilder, new XmlWriterSettings() {
                Encoding = Encoding.UTF8,
                Indent = true
            })) {
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("","");
                xmlSerializer.Serialize(xmlWriter, obj,ns);                
                xml = xmlStringBuilder.ToString().Replace("utf-16", "utf-8");
                string fileName = prefix + "-" + DateTime.Now.Ticks + ".xml";
                if (!Directory.Exists(LogFolder + "/Ortec")) Directory.CreateDirectory(LogFolder + "/Ortec");
                using (StreamWriter w = new StreamWriter(Path.Combine(DayFolder(LogFolder + "/Ortec"), fileName))) {
                    w.Write("<![CDATA[");//Line
                    w.Write(xml);//Line
                    w.Write("]]>");
                }
            }
            return xml;
        }
        public static string WriteXmlFile(string prefix, object xmlBody)
        {
            string fileName = prefix + "-" + DateTime.Now.Ticks+".xml";
            if (!Directory.Exists(LogFolder +"/Ortec")) Directory.CreateDirectory(LogFolder + "/Ortec");
            using (StreamWriter sw = new StreamWriter(Path.Combine(DayFolder(LogFolder+"/Ortec"), fileName), true)) {
                XmlSerializer xSerializer = new XmlSerializer(xmlBody.GetType());
                xSerializer.Serialize(sw, xmlBody);
            }
            return fileName;
        }

    }
}
