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
        string json = "{\"@odata.context\":\"http://services.odata.org/V4/Northwind/Northwind.svc/$metadata\",\"value\":[{\"name\":\"Categories\",\"kind\":\"EntitySet\",\"url\":\"Categories\"},{\"name\":\"CustomerDemographics\",\"kind\":\"EntitySet\",\"url\":\"CustomerDemographics\"},{\"name\":\"Customers\",\"kind\":\"EntitySet\",\"url\":\"Customers\"},{\"name\":\"Employees\",\"kind\":\"EntitySet\",\"url\":\"Employees\"},{\"name\":\"Order_Details\",\"kind\":\"EntitySet\",\"url\":\"Order_Details\"},{\"name\":\"Orders\",\"kind\":\"EntitySet\",\"url\":\"Orders\"},{\"name\":\"Products\",\"kind\":\"EntitySet\",\"url\":\"Products\"},{\"name\":\"Regions\",\"kind\":\"EntitySet\",\"url\":\"Regions\"},{\"name\":\"Shippers\",\"kind\":\"EntitySet\",\"url\":\"Shippers\"},{\"name\":\"Suppliers\",\"kind\":\"EntitySet\",\"url\":\"Suppliers\"},{\"name\":\"Territories\",\"kind\":\"EntitySet\",\"url\":\"Territories\"},{\"name\":\"Alphabetical_list_of_products\",\"kind\":\"EntitySet\",\"url\":\"Alphabetical_list_of_products\"},{\"name\":\"Category_Sales_for_1997\",\"kind\":\"EntitySet\",\"url\":\"Category_Sales_for_1997\"},{\"name\":\"Current_Product_Lists\",\"kind\":\"EntitySet\",\"url\":\"Current_Product_Lists\"},{\"name\":\"Customer_and_Suppliers_by_Cities\",\"kind\":\"EntitySet\",\"url\":\"Customer_and_Suppliers_by_Cities\"},{\"name\":\"Invoices\",\"kind\":\"EntitySet\",\"url\":\"Invoices\"},{\"name\":\"Order_Details_Extendeds\",\"kind\":\"EntitySet\",\"url\":\"Order_Details_Extendeds\"},{\"name\":\"Order_Subtotals\",\"kind\":\"EntitySet\",\"url\":\"Order_Subtotals\"},{\"name\":\"Orders_Qries\",\"kind\":\"EntitySet\",\"url\":\"Orders_Qries\"},{\"name\":\"Product_Sales_for_1997\",\"kind\":\"EntitySet\",\"url\":\"Product_Sales_for_1997\"},{\"name\":\"Products_Above_Average_Prices\",\"kind\":\"EntitySet\",\"url\":\"Products_Above_Average_Prices\"},{\"name\":\"Products_by_Categories\",\"kind\":\"EntitySet\",\"url\":\"Products_by_Categories\"},{\"name\":\"Sales_by_Categories\",\"kind\":\"EntitySet\",\"url\":\"Sales_by_Categories\"},{\"name\":\"Sales_Totals_by_Amounts\",\"kind\":\"EntitySet\",\"url\":\"Sales_Totals_by_Amounts\"},{\"name\":\"Summary_of_Sales_by_Quarters\",\"kind\":\"EntitySet\",\"url\":\"Summary_of_Sales_by_Quarters\"},{\"name\":\"Summary_of_Sales_by_Years\",\"kind\":\"EntitySet\",\"url\":\"Summary_of_Sales_by_Years\"}]}";
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

