using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace aberonServices.Helpers
{
    public class HttpHelper
    {
        public static async Task<String> GetBodyContentAsStringAsync(HttpRequestMessage request)
        {

            Stream s = await request.Content.ReadAsStreamAsync();
            StreamReader reader = new StreamReader(s);
            reader.BaseStream.Position = 0;
            string requestStr = reader.ReadToEnd();
            return requestStr;
        }
    }
}