using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYG.HoleDetect
{
    public sealed class OracleHelper
    {
        private static string ERPServer = System.Configuration.ConfigurationManager.AppSettings["ERPServer"];

        public static string OracleConnection
        {
            get
            {
                return "Data Source=//172.17.3.131:1521/erp92.tyg.com.tw;Persist Security Info=True;User ID=POCAD;Password=TEAAB7121973;Unicode=True";
            }
        }

        public static DataTable ExecuteNonQuery(string selectCommandText)
        {
            return OracleHelper.ExecuteNonQuery(selectCommandText, null);
        }


        public static DataTable ExecuteNonQuery(string selectCommandText, OracleParameter[] parameters)
        {
            DataTable dt = new DataTable();//建立datatable接收query的資料
            OracleCommand selectCommand = new OracleCommand(selectCommandText);
            if (parameters != null)
                    selectCommand.Parameters.AddRange(parameters);

            using (OracleConnection conn = new OracleConnection(OracleHelper.OracleConnection))
            {
                conn.Open();//開始連線
                selectCommand.Connection = conn;
                //建立od物件接收select結果
                OracleDataAdapter OD = new OracleDataAdapter(selectCommand);

                OD.Fill(dt);
                //指定datagridview的datasource

                conn.Close();//結束連線
            }

            return dt;
        }

        public static int UpdateNonQuery(string sql, OracleParameter[] parameters)
        {
            int affectRow = 0;
            if (sql == null || sql.Trim().Equals("")) throw new Exception();

            try
            {
                using (OracleConnection cn = new OracleConnection(OracleHelper.OracleConnection))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = sql;

                        if (parameters != null)
                            foreach (OracleParameter parameter in parameters)
                            {
                                if (parameter.Value == null) parameter.Value = DBNull.Value;
                                cmd.Parameters.Add(parameter);
                            }
                        cn.Open();
                        affectRow = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
            }

            return affectRow;
        }
    }
}
