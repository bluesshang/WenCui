using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Web;

namespace DBUtility
{
    public class OleDbHelper
    {
        public OleDbHelper(){ }

    /// <summary>
    /// �������ݿ������ַ���
    /// </summary>
    /// <returns></returns>
    public static String GetSqlConnection()
    {
        System.Web.UI.Page page = new System.Web.UI.Page();
        String conn = ConfigurationSettings.AppSettings["OleDbConnectionString"].ToString() + page.Server.MapPath("~/" + ConfigurationSettings.AppSettings["AccessDbPath"].ToString());
        return conn;
    }

    /// <summary>
    ///  ��ò������� 
    /// </summary>
    /// <param name="paramName">��������</param>
    /// <param name="paramType">��������</param>
    /// <param name="paramSize">����</param>
    /// <param name="ColName">Դ������</param>
    /// <param name="paramValue">����ʵֵ</param>
    /// <returns></returns>
    public static OleDbParameter GetParameter(String paramName, OleDbType paramType, Int32 paramSize, String ColName, Object paramValue)
    {
        OleDbParameter param = new OleDbParameter(paramName, paramType, paramSize, ColName);
        param.Value = paramValue;
        return param;
    }

    /// <summary>
    /// ��ò�������
    /// </summary>
    /// <param name="paramName">��������</param>
    /// <param name="paramType">��������</param>
    /// <param name="paramSize">����</param>
    /// <param name="ColName">Դ������</param>
    /// <returns></returns>
    public static OleDbParameter GetParameter(String paramName, OleDbType paramType, Int32 paramSize, String ColName)
    {
        OleDbParameter param = new OleDbParameter(paramName, paramType, paramSize, ColName);
        return param;
    }

    /// <summary>
    /// ��ò�������
    /// </summary>
    /// <param name="paramName">��������</param>
    /// <param name="paramType">��������</param>
    /// <param name="paramSize">����</param>
    /// <param name="ColName">Դ������</param>
    /// <returns></returns>
    public static OleDbParameter GetParameter(String paramName, OleDbType paramType, Object paramValue)
    {
        OleDbParameter param = new OleDbParameter(paramName,paramType);
        param.Value = paramValue;
        return param;
    }

    /// <summary>
    /// ִ��SQL���
    /// </summary>
    /// <param name="Sqlstr">SQL���</param>
    /// <param name="param">������������</param>
    /// <returns></returns>
    public static int ExecuteSql(String Sqlstr, OleDbParameter[] param)
    {
        String ConnStr = OleDbHelper.GetSqlConnection();
        using (OleDbConnection conn = new OleDbConnection(ConnStr))
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = Sqlstr;
            cmd.Parameters.AddRange(param);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            return 1;
        }
    }

    /// <summary>
    /// ִ��SQL��䲢�������ݱ�
    /// </summary>
    /// <param name="Sqlstr">SQL���</param>
    /// <returns></returns>
    public static DataTable ExecuteDt(String Sqlstr)
    {
        String ConnStr = OleDbHelper.GetSqlConnection();
        using (OleDbConnection conn = new OleDbConnection(ConnStr))
        {
            OleDbDataAdapter da = new OleDbDataAdapter(Sqlstr, conn);
            DataTable dt = new DataTable();
            conn.Open();
            da.Fill(dt);
            conn.Close();
            return dt;
        }
    }

    /// <summary>
    /// ִ��SQL��䲢�������ݱ�
    /// </summary>
    /// <param name="Sqlstr">SQL���</param>
    /// <param name="param">���������б�</param>
    /// <returns></returns>
    public static DataTable ExecuteDt(String Sqlstr, OleDbParameter[] param)
    {
        String ConnStr = OleDbHelper.GetSqlConnection();
        using (OleDbConnection conn = new OleDbConnection(ConnStr))
        {
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter();
            OleDbCommand cmd = new OleDbCommand(Sqlstr,conn);//ִ�����
            cmd.Connection = conn;
            cmd.Parameters.AddRange(param);//��������б�
            da.SelectCommand = cmd;
            conn.Open();
            da.Fill(dt);
            conn.Close();
            return dt;
        }
    }

    /// <summary>
    /// ����ִ��SQL���
    /// </summary>
    /// <param name="Sqlstr">SQL�������</param>
    /// <param name="param">SQL������������</param>
    /// <returns></returns>
    public static Int32 ExecuteSqls(String [] Sqlstr,List<OleDbParameter []> param)
    {
        String ConnStr = OleDbHelper.GetSqlConnection();
        using (OleDbConnection conn = new OleDbConnection(ConnStr))
        {
            
            OleDbCommand cmd = new OleDbCommand();
            OleDbTransaction tran = null;
            cmd.Transaction = tran;
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                cmd.Connection = conn;
                cmd.Transaction = tran;

                Int32 count = Sqlstr.Length;
                for (Int32 i = 0; i < count; i ++ )
                {
                    cmd.CommandText = Sqlstr[i];
                    cmd.Parameters.AddRange(param[i]);
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
                return 1;
            }
            catch
            {
                tran.Rollback();
                return 0;
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }
    }
    }
}
