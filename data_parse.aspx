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

    protected void Page_Load(object sender, EventArgs e)
    {
        string items = Request["usr_data"];

        DataParser dp = DataParser.GetParser(items);
        DataRecordItem dri = new DataRecordItem();
        dp.Parse(items, ref dri);
        
        
        if (items != null)
        {
            
            string json = "";

            json = "{\"text\":\"accused:" + dri.accused + ", accuser:" + dri.accuser
                + ", court:" + dri.court + ", court-room:" + dri.court_room 
                + ", telephone:" + dri.telephone + ", case-title:" + dri.case_title + "\", len:0}";
                        
            
            /*json += "{\n";
            json += "\"text\": \"" + items.Replace("\n", "<li>").ToUpper() + "\",\n";
            json += "len: " + items.Length + ",\n";
            json += "}\n";*/
            Response.Write(json);
        }
        else
        {
            Response.Write("{query:'none',total:0,}");
        }
     }
</script>