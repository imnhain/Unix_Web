using System;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace Unix_Web.Providers
{
    public class UnixConn
    {
        static string ConnectionString = @"Driver={IBM INFORMIX 3.82 32 BIT}; Host=192.1.1.1; Server=ids12; Service=3002; " +
                                        "Protocol=onsoctcp; Database=erp; Uid=kendaweb; Pwd=kenda; NEWLOCALE=zh_cn,zh_tw; " +
                                        "NEWCODESET=big5,57352,utf8; DB_LOCALE=zh_tw.57352; CLIENT_LOCALE=zh_tw.big5";

        //static string ConnectionString = @"Driver={IBM INFORMIX ODBC DRIVER}; Host=192.1.1.1; Server=ids12; Service=on7tcp; " +
        //                                 "Protocol=onsoctcp; Database=erp; Uid=kendaweb; Pwd=kenda; " +
        //                                 "NEWLOCALE=zh_cn,zh_tw; NEWCODESET=big5,57352,utf8; " +
        //                                 "DB_LOCALE=zh_tw.57352; CLIENT_LOCALE=zh_tw.big5";

        public static DataTable ExecuteQuery(
            string Query,
            object[] parameter = null)
        {
            using (var conn = new OdbcConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    OdbcCommand cmd = new OdbcCommand(Query, conn);
                    if (parameter != null)
                    {
                        string[] listPara = Query.Split(' ');
                        int i = 0;
                        foreach (string item in listPara)
                        {
                            if (item.Contains('?'))
                            {
                                cmd.Parameters.AddWithValue(item, parameter[i]);
                                i++;
                            }
                        }
                    }
                    OdbcDataAdapter adapter = new OdbcDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    return new DataTable();
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();
                }
            }
        }

        public static bool ExecuteNonQuery(string query,
           object[] parameter = null)
        {
            using (var conn = new OdbcConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    OdbcCommand cmd = new OdbcCommand(query, conn);

                    if (parameter != null)
                    {
                        string[] listPara = query.Split(' ');
                        int i = 0;
                        foreach (string item in listPara)
                        {
                            if (item.Contains('?'))
                            {
                                cmd.Parameters.AddWithValue(item, parameter[i]);
                                i++;
                            }
                        }
                    }
                    int effectedRow = cmd.ExecuteNonQuery();
                    return effectedRow > 0;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();
                }
            }
        }
    }
}