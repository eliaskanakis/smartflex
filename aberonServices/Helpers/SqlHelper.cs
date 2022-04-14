using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace aberonServices.Helpers
{
    public class SqlHelper
    {
        public static DataTable GetQueryFields(string queryName, params OracleParameter[] parameters)
        {
            try {
                string sql = ResourseHelper.GetEmbeddedFileContent("Queries." + queryName + ".sql");
                using (OracleConnection cn = new OracleConnection(Settings.ConnectionString)) {
                    using (OracleCommand cmd = new OracleCommand(sql, cn)) {
                        cmd.BindByName = true;
                        cmd.Parameters.AddRange(parameters);
                        cn.Open();
                        using (OracleDataReader rd = cmd.ExecuteReader()) {
                            return rd.GetSchemaTable();
                        }
                    }
                }
            }catch(Exception ex) {
                while (ex.InnerException != null) ex = ex.InnerException;
                throw new Exception("Error getting query field for query <" + queryName + ">." + ex.Message);
            }
        }


        public static string GetQueryString(string resourceName)
        {
            if (!string.IsNullOrEmpty(Settings.SqlFolder) &&
                File.Exists(Path.Combine(Settings.SqlFolder, resourceName))){

                using (StreamReader sr = new StreamReader(Path.Combine(Settings.SqlFolder, resourceName))) {
                    return sr.ReadToEnd();
                }
            } else {
                return ResourseHelper.GetEmbeddedFileContent(resourceName);
            }
        }

        public static List<Dictionary<string,object>> GetData(string queryName,params OracleParameter[] parameters)
        {
            try {
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                string sql = GetQueryString("Queries." + queryName + ".sql");
                using (OracleConnection cn = new OracleConnection(Settings.ConnectionString)) {
                    using (OracleCommand cmd = new OracleCommand(sql, cn)) {
                        cmd.Parameters.AddRange(parameters);
                        cmd.BindByName = true;
                        cn.Open();
                        using (OracleDataReader rd = cmd.ExecuteReader()) {                            
                            while (rd.Read()) {
                                Dictionary<string, object> row = new Dictionary<string, object>();
                                for (int i = 0; i < rd.FieldCount; i++) {
                                    if (rd[i] is string) {
                                        row.Add(rd.GetName(i), rd[i].ToString().TrimEnd());
                                    } else {
                                        row.Add(rd.GetName(i), rd[i]);
                                    }
                                }
                                rows.Add(row);
                            }
                            return rows;
                        }
                    }
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) ex = ex.InnerException;
                throw new Exception("Error getting data for query <" + queryName + ">." + ex.Message);
            }
        }

        public static Dictionary<string, object> GetRow(string queryName, params OracleParameter[] parameters)
        {
            List<Dictionary<string, object>> rows = GetData(queryName, parameters);
            if (rows.Count == 0) return null;
            return rows[0];
        }

        public static void ExecuteCommand(string queryName, params OracleParameter[] parameters)
        {
            string sql = ResourseHelper.GetEmbeddedFileContent("Queries." + queryName + ".sql");
            ExecuteCommandStr(sql, parameters);
        }

        public static void ExecuteCommandStr(string queryStr, params OracleParameter[] parameters)
        {
            try
            {
                string sql = queryStr;
                using (OracleConnection cn = new OracleConnection(Settings.ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(sql, cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.AddRange(parameters);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;
                throw new Exception("Error:"+ex.Message+". running query <" + queryStr + ">.");
            }
        }
    }
}
