<%@ import namespace="System" %>
<%@ import namespace="System.Data" %>
<%@ import namespace="System.Configuration" %>
<%@ import namespace="System.Collections" %>
<%@ import namespace="System.Configuration" %>
<%@ import namespace="System.Web" %>
<%@ import namespace="System.Web.Security" %>
<%@ import namespace="System.Web.UI" %>
<%@ import namespace="System.Web" %>
<%@ import namespace="System.Web.UI.WebControls" %>
<%@ import namespace="System.Web.UI.WebControls.WebParts" %>
<%@ import namespace="System.Web.UI.HtmlControls" %>
<%@ import namespace="System.Data.OleDb" %>
<%@ Page Language="C#" %>

<script runat="server">

    public void exeSql(string StrSql)
    {
        DataTable dt;
        OleDbConnection MyConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Server.MapPath(g_StrConn));
        OleDbCommand MyCmd = new OleDbCommand(StrSql, MyConn);
        MyCmd.Connection.Open();
        //OleDbDataReader Dr = MyCmd.ExecuteReader();
        //dt = GetDataTableFromDataReader(Dr);
        //MyConn.ex
        MyCmd.ExecuteNonQuery();
        MyCmd.Connection.Close();
        //return dt;
    }

    //Webconfig文件中获取数据库路径
    public string g_StrConn = ConfigurationManager.AppSettings["Conn_Access"];
    protected void Page_Load(object sender, EventArgs e)
    {
        string items = Request["items"];
        if (items != null)
        {
            items = items.Substring(0, items.Length - 1);
            string sql = "DELETE FROM country WHERE id IN (" + items + ")";
            string[] sp = items.Split(',');
            int total = sp.Length;
            DBUtility.SQLHelper sHelper = new DBUtility.SQLHelper();
            //sHelper.RunSQL(sql);
            exeSql(sql);
            string json = "";
            json += "{\n";
            json += "query: '" + sql + "',\n";
            json += "total_del: " + total + ",\n";
            json += "}\n";
            Response.Write(json);
        }
        else
        {
            Response.Write("{query:'none',total:0,}");
        }
     }
</script>