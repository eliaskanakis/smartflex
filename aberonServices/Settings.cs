using System;
using System.Configuration;

namespace aberonServices
{
	/// <summary>
	/// Summary description for Options.
	/// </summary>
	public class Settings
	{
        public static string ConnectionString
        {
            get
            {
                //return "Data Source=OPV-AGATHI/AB112;User Id=W_OPAP;Password=W_OPAP;";
                return ConfigurationManager.ConnectionStrings["aberonDb"].ConnectionString;
            }
        }

        public static string SqlFolder
        {
            get {
                return ConfigurationManager.AppSettings["SqlFolder"];
            }
        }

    }
}
