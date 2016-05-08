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

    protected void Page_Load(object sender, EventArgs args)
    {
        string bizData = Request["usr_data"];

        DataParagrapher paragraph = new DataParagrapher();

        int count = paragraph.DoParagraph(bizData);
        
        DataRecordItem dri = new DataRecordItem();

        string json = "{\n"
            + "  \"count\": \"" + count + "\",\n"
            + "  \"records\": [\n";

        for (int i = 0; i < count; i++)
        {
            DataParagraph para = paragraph.paragraphs[i];
            
            ParseError status;
            string message;
            
            try
            {
                char[] trimChars = { '\r', ' ', '.', '¡£', ';', '£»' };
                string data = para.text
                    .Trim(trimChars)
                    .Replace("\r\n", "")
                    .Replace("\n", "");
                
                dri.Reset();
                
                DataParser dp = DataParser.GetParser(data);

                dp.Parse(data, ref dri);

                status = ParseError.Okay;
                message = "OK";
            }
            catch (DataParseException e)
            {
                status = e.code;
                message = "½âÎö³öÏÖ´íÎó:" + e.Message.Replace("\r\n", "<br>");
            }

            json += "{"
                + "\"accused\":\"" + dri.accused + "\""
                + ", \"accuser\":\"" + dri.accuser + "\""
                + ", \"court\":\"" + dri.court + "\""
                + ", \"courtroom\":\"" + dri.court_room + "\""
                + ", \"telephone\":\"" + dri.telephone + "\""
                + ", \"title\":\"" + dri.case_title + "\""
                + ", \"status\":\"" + (int)status + "\""
                + ", \"message\":\"" + message.Replace("\"", "\\\"") + "\""
                + ", \"para\":{"
                    + "\"begin\":\"" + para.begin + "\""
                    + ", \"end\":\"" + para.end + "\""
                    + ", \"text\":\"" + para.text
                        .Replace("\r\n", "<br>")
                        .Replace("\n", "<br>") + "\""
                    + "}"
                + "},\n";
        }
        
        json += "]}";
        
        Response.Write(json);
     }
</script>