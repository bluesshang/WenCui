
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


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
<title>Flexigrid</title>
<link rel="stylesheet" type="text/css" href="css_grid/flexigrid.css" />
<script type="text/javascript" src="js/jquery-1.2.3.pack.js"></script>
<script type="text/javascript" src="js/flexigrid.js"></script>
<script type="text/javascript">
$(document).ready(function(){

	$("#flex1").flexigrid
			(
			{
			url: 'test.aspx',
			dataType: 'json',
			colModel : [
				{display: 'ID', name : 'id', width : 40, sortable : true, align: 'center'},
				{display: 'ISO', name : 'iso', width : 40, sortable : true, align: 'center'},
				{display: 'Name', name : 'name', width : 180, sortable : true, align: 'left'},
				{display: 'Printable Name', name : 'printable_name', width : 120, sortable : true, align: 'left'},
				{display: 'ISO3', name : 'iso3', width : 130, sortable : true, align: 'left', hide: true},
				{display: 'Number Code', name : 'numcode', width : 80, sortable : true, align: 'right'}
				],
			buttons : [
				{name: 'Add', bclass: 'add', onpress : test},
				{name: 'Delete', bclass: 'delete', onpress : test},
				{separator: true},
				{name: 'A', onpress: sortAlpha},
                {name: 'B', onpress: sortAlpha},
				{name: 'C', onpress: sortAlpha},
				{name: 'D', onpress: sortAlpha},
				{name: 'E', onpress: sortAlpha},
				{name: 'F', onpress: sortAlpha},
				{name: 'G', onpress: sortAlpha},
				{name: 'H', onpress: sortAlpha},
				{name: 'I', onpress: sortAlpha},
				{name: 'J', onpress: sortAlpha},
				{name: 'K', onpress: sortAlpha},
				{name: 'L', onpress: sortAlpha},
				{name: 'M', onpress: sortAlpha},
				{name: 'N', onpress: sortAlpha},
				{name: 'O', onpress: sortAlpha},
				{name: 'P', onpress: sortAlpha},
				{name: 'Q', onpress: sortAlpha},
				{name: 'R', onpress: sortAlpha},
				{name: 'S', onpress: sortAlpha},
				{name: 'T', onpress: sortAlpha},
				{name: 'U', onpress: sortAlpha},
				{name: 'V', onpress: sortAlpha},
				{name: 'W', onpress: sortAlpha},
				{name: 'X', onpress: sortAlpha},
				{name: 'Y', onpress: sortAlpha},
				{name: 'Z', onpress: sortAlpha},
				{name: '#', onpress: sortAlpha}

				],
			searchitems : [
				{display: 'ISO', name : 'iso'},
				{display: 'Name', name : 'name', isdefault: true}
				],
			sortname: "id",
			sortorder: "asc",
			usepager: true,
			title: 'Countries',
			useRp: true,
			rp: 100,
			showTableToggleBtn: true,
			width: 700,
			height: 255
			}
			);

	$("#btk_ok").bind('click', function () {
	    //alert($("#form1").serialize());
	    $.ajax({  
	        type:'post',      
            url:'data_parse.aspx',  
            data: $("#form1").serialize(),
            cache:false,  
            dataType:'json',  
            success: function (data) {
                //alert('return data len' + data.len);
                $("#proc_info").html('return data is ' + data.len + ':<br><br>' + data.text);
            },
	        error: function(err) {
	            alert(err);
	        }
	    });  
	});
	
});
function sortAlpha(com)
			{ 
			jQuery('#flex1').flexOptions({newp:1, params:[{name:'letter_pressed', value: com},{name:'qtype',value:$('select[name=qtype]').val()}]});
			jQuery("#flex1").flexReload(); 
			}

function test(com,grid)
{
    if (com=='Delete')
        {
           if($('.trSelected',grid).length>0){
		   if(confirm('Delete ' + $('.trSelected',grid).length + ' items?')){
            var items = $('.trSelected',grid);
            var itemlist ='';
        	for(i=0;i<items.length;i++){
				itemlist+= items[i].id.substr(3)+",";
			}
			$.ajax({
			   type: "POST",
			   dataType: "json",
			   url: "delete.aspx",
			   data: "items="+itemlist,
			   success: function(data){
			       alert("Query: " + data.query + " - Total affected rows is: " + data.total_del);
			   $("#flex1").flexReload();
			   }
			 });
			}
			} else {
				return false;
			} 
        }
    else if (com=='Add')
        {
            alert('Add New Item Action');
           
        }            
} 
</script>
</head>

<body>
<h1>Flexigrid Example Page</h1>
<a href="http://webplicity.net/flexigrid/">Flexigrid home</a> 
<a href="http://codeigniter.com/forums/viewthread/75326">Flexigrid discussion on CodeIgniter forum</a><br /><br />
<b>Demonstrating the flexgrid with search and delete functionality</b><br /><br />
<table id="flex1" style="display:none"></table>
<br /><br />
    <script runat="server">
        string a = "this is c# string中hello world!";
    </script>
<% = a %>
<%
    Response.Write("<br/> aaaaaa <br/>");    

    string[] b = a.Split(new char[]{'中'});
    foreach (string i in b)
    {
        Response.Write("<br>" + i);
    }
%>
<br />
<a href="#">Download this example</a>

    <form id="form1">
        <textarea id="usr_data" name="usr_data"></textarea><br />
        <input type="button" value="ok" id="btk_ok" />
        <div id="proc_info"></div>
    </form>
</body>
</html>