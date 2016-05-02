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
    //Webconfig文件中获取数据库路径
    public string g_StrConn = ConfigurationManager.AppSettings["Conn_Access"];
    protected void Page_Load(object sender, EventArgs e)
    {
        string page = Request["page"];
        string rp = Request["rp"];
        string sortname = Request["sortname"];
        string sortorder = Request["sortorder"];
        string where;
        string sort;
        int start;
        int total;
        
        if (!string.IsNullOrEmpty(sortname)) sortname = "name";
           if (sortorder=="") sortorder ="desc";
		if(!string.IsNullOrEmpty(Request["query"])){
			where = "WHERE "+ Request ["qtype"]+" LIKE '%"+Request ["query"]+"%' ";
		} else {
			where =" where (1=1) ";
		}
		if(!string.IsNullOrEmpty(Request["letter_pressed"])){
			where = "WHERE "+Request["qtype"]+" LIKE '"+Request["letter_pressed"]+"%' ";	
		}
		if(Request["letter_pressed"]=="#"){
			where = "WHERE "+Request["qtype"]+" REGEXP '[[:digit:]]' ";
		}
        sort = " ORDER BY "+sortname +" " +sortorder;
        
        if (page=="") page = "1";
         if (rp=="") rp = "10";
         start = ((int.Parse(page.ToString()) - 1) * int.Parse(rp.ToString()));
        //SqlServer 用法
         string sql_mssql = "select   IDENTITY(int,   1,1)   AS   ID_Num,*   into   #temp   from country " + where + sort;
         sql_mssql += " SELECT id,iso,name,printable_name,iso3,numcode FROM #temp where ID_Num   between " + (start + 1) + " and " + (start + int.Parse(rp));
         string sql_mdb;
        if (start==0)
            sql_mdb = "select top " + rp + " id,iso,name,printable_name,iso3,numcode from country " + where  + sort;
        else
        {
            sql_mdb = " select top " + start + " id from country " + where + sort;
            sql_mdb = "select top " + rp + " id,iso,name,printable_name,iso3,numcode from country " + where + " and id not in ( " + sql_mdb + ")" + sort;
        }
         
        //total = countRec("iso","country",where);
        total = countRec("iso", "country", where, 1);
        string json;
        json = "";
        json += "{\n";
        json += "page: " + page + ",\n";
        json += "total: " + total + ",\n";
        json += "rows: [";
        bool rc = false;
          //dataTable dt = runSql(sql_mdb);
        DataTable dt = runSql(sql_mdb, 1);
        foreach (DataRow row in dt.Rows)
        {
            if (rc) json += ",";
            json += "\n{";
            json += "id:'" + row["id"] + "',";
            json += "cell:['" + row["id"] + "','" + row["iso"] + "'";
            json += ",'" + row["name"] + "'";
            json += ",'" + row["printable_name"] + "'";
            json += ",'" + row["iso3"] + "'";
            json += ",'" + row["numcode"] + "']";
            json += "}";
            rc = true;
        }   
       
        json += "]\n";
        json += "}";
        Response.Write(json);
    }



    #region "int countRec(string fname,string tname,string where)"

    /// <summary>
    /// count the all record with tname and where
    /// </summary>
    /// <param name="fname">string</param>
    /// <param name="tname">string</param>
    /// <param name="where">string</param>
    /// <returns>int</returns>
    public int countRec(string fname,string tname,string where)
    {
        DataTable dt;
        string StrSql = "SELECT count(" + fname + ") as total FROM " + tname + " " + where;
        OleDbConnection MyConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Server.MapPath(g_StrConn));
        OleDbCommand MyCmd = new OleDbCommand(StrSql, MyConn);
        MyCmd.Connection.Open();
        OleDbDataReader Dr = MyCmd.ExecuteReader();
        dt = GetDataTableFromDataReader(Dr);
        MyCmd.Connection.Close();
        if (dt.Rows.Count > 0)
            return int.Parse (dt.Rows[0]["total"].ToString());
        
        else
            return 0;
    }
    
    #endregion

    public int countRec(string fname, string tname, string where, int type)
    {
        DBUtility.SQLHelper sHelper =new DBUtility.SQLHelper() ;
        string StrSql = "SELECT count(" + fname + ") as total FROM " + tname + " " + where;
        DataSet  ds=new DataSet()  ;
        DataTable dt;
        //sHelper.RunSQL(StrSql,ref ds);
        //dt = ds.Tables[0]; 
        dt = runSql(StrSql);
        if (dt.Rows.Count > 0)
            return int.Parse(dt.Rows[0]["total"].ToString());

        else
            return 0;
    }
    
    #region "DataTable  runSql(string StrSql)"
    
    /// <summary>
    /// Run the Sql string user gived
    /// </summary>
    /// <param name="StrSql">string</param>
    /// <returns>DataTable</returns>
    public DataTable  runSql(string StrSql)
    {
        DataTable dt;
        OleDbConnection MyConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Server.MapPath(g_StrConn));
        OleDbCommand MyCmd = new OleDbCommand(StrSql, MyConn);
        MyCmd.Connection.Open();
        OleDbDataReader Dr = MyCmd.ExecuteReader();
        dt=GetDataTableFromDataReader(Dr);
        MyCmd.Connection.Close();
        return dt;
    }
    
    #endregion

    public DataTable runSql(string StrSql, int type)
    {
        DBUtility.SQLHelper sHelper = new DBUtility.SQLHelper();
        
        DataSet ds=new DataSet() ;
        DataTable dt;
        //sHelper.RunSQL(StrSql,ref ds);
        //dt = ds.Tables[0];
        dt = runSql(StrSql);
        return dt;
    }
    
    #region "DataTable GetDataTableFromDataReader(OleDbDataReader reader) "
    /// <summary> 
    /// Return a DataTable From OleDbDataReader 
    /// </summary> 
    /// <param name="reader">OleDbDataReader</param> 
    /// <returns>DataTable</returns> 
    public DataTable GetDataTableFromDataReader(OleDbDataReader reader) 
    {
        DataTable schema = reader.GetSchemaTable(); 
        DataColumn[] columns = new DataColumn[schema.Rows.Count]; 
        DataColumn column; 
        //Build the schema 
        for(int i = 0; i < schema.Rows.Count; i++) 
        { 
            column = new DataColumn(); 
            column.AllowDBNull = (bool)schema.Rows[i]["AllowDBNull"]; 
            column.AutoIncrement = (bool)schema.Rows[i]["IsAutoIncrement"]; 
            column.ColumnName = (string)schema.Rows[i]["ColumnName"]; 
            column.DataType = Type.GetType(schema.Rows[i]["DataType"].ToString()); 
            if(column.DataType == Type.GetType("System.String")) 
            { 
                column.MaxLength = (int)schema.Rows[i]["ColumnSize"]; 
            } 
            column.ReadOnly = (bool)schema.Rows[i]["IsReadOnly"]; 
            column.Unique = (bool)schema.Rows[i]["IsUnique"]; 
            columns[i] = column; 
        } 
        DataTable data = new DataTable(); 
        data.Columns.AddRange(columns); 
        //Get the data itself. 
        while (reader.Read()) 
        {
            DataRow row = data.NewRow(); 
            for(int i = 0 ; i < schema.Rows.Count;i++) 
            { 
                row[i] = reader[i]; } data.Rows.Add(row);
        } return data;
    }

    #endregion
    
  
</script>

