using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace aberonServices.Helpers
{
    class ResourseHelper
    {
        public static string GetEmbeddedFileContent(string resourceName)
        {
            Assembly asm = typeof(SqlHelper).Assembly;
            Stream s = asm.GetManifestResourceStream("aberonServices." + resourceName);
            if (s == null) {
                throw new Exception("aberonServices." + resourceName + " embedded resource not found.");
            }
            StreamReader sr=new StreamReader(s);
            return sr.ReadToEnd();
        }
    }
}
