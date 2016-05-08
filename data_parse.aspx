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
        if (Request["get_status"] == "true")
        {
            Response.Write("{\"number\":\"" + Application["_ok_records"]
                + "\",\"error\":\"" + Application["_error_records"]
                + "\",\"total\":\"" + Application["_total_records"] + "\"}");
            return;
        }

        string bizData = Request["usr_data"];

        DataParagrapher paragraph = new DataParagrapher();

        int ok = 0, err = 0;
        int count = paragraph.DoParagraph(bizData);

        //_processed_records = 0;
        //_total_records = count;
        Application["_ok_records"] = 0;
        Application["_error_records"] = 0;
        Application["_total_records"] = count;
        //Response.ContentType = "text/plain";
        //Response.Write("{\"type\":\"status\",\"message\":\"total " + count + "paragraphs will be processed.\"}");
        //Response.Flush();
        //Response.End();
        
        DataRecordItem dri = new DataRecordItem();

        string json = "{\n"
            + "  \"type\": \"data\",\n"
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
                //_processed_records += 1;

                Application["_ok_records"] = ++ok;
            }
            catch (DataParseException e)
            {
                status = e.code;
                message = "½âÎö³öÏÖ´íÎó:" + e.Message.Replace("\r\n", "<br>");
                Application["_error_records"] = ++err;
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