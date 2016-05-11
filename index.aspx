
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


<!--jQuery References-->
<script src="http://code.jquery.com/jquery-1.9.1.min.js" type="text/javascript"></script>
<script src="http://code.jquery.com/ui/1.10.1/jquery-ui.min.js" type="text/javascript"></script>

<!--Theme-->
<link href="http://cdn.wijmo.com/themes/aristo/jquery-wijmo.css" rel="stylesheet" type="text/css" />

<!--Wijmo Widgets CSS-->
<link href="http://cdn.wijmo.com/jquery.wijmo-pro.all.3.20132.15.min.css" rel="stylesheet" type="text/css" />

    <!-- Material Lite -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons" />
    <link rel="stylesheet" href="https://code.getmdl.io/1.1.1/material.indigo-red.min.css" />
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:regular,bold,italic,thin,light,bolditalic,black,medium&amp;lang=en" />
    <script defer src="https://code.getmdl.io/1.1.1/material.min.js"></script>

    <!-- Syntax Highlighter -->
    <!-- <link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/highlight.js/9.1.0/styles/default.min.css"> -->
    <link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/highlight.js/9.1.0/styles/github.min.css">
    <script src="//cdnjs.cloudflare.com/ajax/libs/highlight.js/9.1.0/highlight.min.js"></script>
    <!-- Wijmo -->
    <link rel="stylesheet" href="http://demos.wijmo.com/5/purejs/OlapIntro/OlapIntro/styles/vendor/wijmo.min.css" />
    <link href="http://demos.wijmo.com/5/purejs/OlapIntro/OlapIntro/styles/vendor/wijmo.theme.material.min.css" rel="stylesheet" />
    <script src="http://demos.wijmo.com/5/purejs/OlapIntro/OlapIntro/scripts/vendor/wijmo.min.js"></script>
    <script src="http://demos.wijmo.com/5/purejs/OlapIntro/OlapIntro/scripts/vendor/wijmo.input.min.js"></script>
    <script src="http://demos.wijmo.com/5/purejs/OlapIntro/OlapIntro/scripts/vendor/wijmo.grid.min.js"></script>
    <script src="http://demos.wijmo.com/5/purejs/OlapIntro/OlapIntro/scripts/vendor/wijmo.grid.filter.min.js"></script>
    <script src="http://demos.wijmo.com/5/purejs/OlapIntro/OlapIntro/scripts/vendor/wijmo.chart.min.js"></script>
    <script src="http://demos.wijmo.com/5/purejs/OlapIntro/OlapIntro/scripts/vendor/wijmo.xlsx.min.js"></script>
    <script src="http://demos.wijmo.com/5/purejs/OlapIntro/OlapIntro/scripts/vendor/wijmo.grid.xlsx.min.js"></script>
    <script src="http://demos.wijmo.com/5/purejs/OlapIntro/OlapIntro/scripts/vendor/wijmo.odata.min.js"></script>
    <script src="http://demos.wijmo.com/5/purejs/OlapIntro/OlapIntro/scripts/vendor/wijmo.olap.min.js"></script>
    <script src="http://cdnjs.cloudflare.com/ajax/libs/jszip/2.5.0/jszip.min.js"></script>
<!--Wijmo Widgets JavaScript-->
<script src="http://cdn.wijmo.com/jquery.wijmo-open.all.3.20132.15.min.js" type="text/javascript"></script>
<script src="http://cdn.wijmo.com/jquery.wijmo-pro.all.3.20132.15.min.js" type="text/javascript"></script>

<script type="text/javascript">
$(document).ready(function(){
    /*
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
			);*/

    var count = 10;
    var data = [];

    for (var i = 0; i < count ; i++) {
        data.push({
            序号: "00" + (i + 1).toString(),
            ID号: "21601" + i.toString(),
            英文名: "TingTao Ge",
            中文名: "听涛阁",
            最小楼层: 2,
            最大楼层: 12,
            状态: true,
            日期: new Date(2014, i % 12, i % 28),
        });
    }
    var cv = new wijmo.collections.CollectionView(data);

    my_rawGrid = new wijmo.grid.FlexGrid('#my_rawGrid', {
        showSelectedHeaders: 'All',
        itemsSource: null
            //new wijmo.odata.ODataCollectionView(
            //'http://services.odata.org/V4/Northwind/Northwind.svc/',
            //'Order_Details_Extendeds'),
    });

    my_rawGridFilter = new wijmo.grid.filter.FlexGridFilter(my_rawGrid);

    $('#dialog').wijdialog({
        autoOpen: true,
        modal: true,
        captionButtons: {
            refresh: { visible: false }
        },
        expandingAnimation: { effect: "puff", duration: 300, easing: "easeOutExpo" }
    });

    $("#wijeditor").wijeditor({
        editorMode: "split",
        mode: "simple",
        //simpleModeCommands: ["Bold", "Italic", "Link", "BlockQuote", "StrikeThrough", "InsertDate", "InsertImage", "NumberedList", "BulletedList", "InsertCode"]
        simpleModeCommands: ["Bold", "Italic", "FontName", "FontSize", "InsertImage", "NumberedList", "BulletedList", "Undo"]
    });

    var parse_records = [];
    

	$("#btk_ok").bind('click', function () {
	    //alert($("#form1").serialize());
	    $("#proc_info").html("");
	    $("#proc_status").html("committing data ...");
	    
	    var pb_timer = setInterval(function () {
	        var text = "";
	        $.ajax({
	            type: 'post',
	            url: 'data_parse.aspx',
	            data: "get_status=true",
	            cache: false,
	            dataType: 'json',
	            success: function (data) {
	                text = data.number + " of " + data.total + " processed, " + data.error + " errors";
	                if (data.number > 0 && eval(data.number) + eval(data.error) == eval(data.total)) {
	                    clearInterval(pb_timer);
	                    text += " done!";
	                } else text += ", please wait for a moment ...";
	                $("#proc_status").html(text);
	            },
	            error: function (o, message) {
	                $("#proc_status").html(message);
	            }
	        });
	    }, 600);

	    $.ajax({  
	        type:'post',      
            url:'data_parse.aspx',  
            data: $("#form1").serialize(),
            cache:false,  
            dataType:'json',  
            success: function (data) {
                if (data.type == "status") {
                    $("#proc_info").html(data.message);
                } else {
                    //alert('return data len' + data.len);
                    var text = "<b>Total " + data.records.length + " records.</b><br><table border=1>";
                    //alert(data.count + ", records:" + data.records.length);
                    var count = data.records.length;
                    for (i = 0; i < count; i++) {
                        //alert(data.records[i].accused);
                        var item = data.records[i];
                        var para = item.para;
                        /*
                        text += "<tr>";
                        text += "<td colspan=9>[" + para.begin + "," + para.end + "]" + para.text + "</td>";
                        text += "</tr>";

                        if (data.records[i].status == 0) {
                            color = "#c5ede8";
                            if (data.records[i].accused == ""
                                || data.records[i].accuser == "")
                                color = "#ffff00";
                        } else if (data.records[i].status == 2)
                            color = "#808080";
                        else color = "#ff0000"

                        text += "<tr style=\"background-color:" + color + "\">";
                        text += "<td>" + data.records[i].type + "</td>";
                        text += "<td>" + data.records[i].accused + "</td>";
                        text += "<td>" + data.records[i].accuser + "</td>";
                        text += "<td>" + data.records[i].court + "</td>";
                        text += "<td>" + data.records[i].courtroom + "</td>";
                        text += "<td>" + data.records[i].telephone + "</td>";
                        text += "<td>" + data.records[i].title + "</td>";
                        text += "<td>" + data.records[i].status + "</td>";
                        text += "<td>" + data.records[i].message + "</td>";
                        text += "</tr>";*/
                        parse_records.push({
                            "业务类型": item.type,
                            "accused": item.accused,
                            "accuser": item.accuser,
                            "court": item.court,
                            "courtroom": item.courtroom,
                            "telephone": item.telephone,
                            "title": item.title,
                            "status": item.status,
                            "message": item.message,
                        });
                    }
                    //////text += "</table>"
                    //////$("#proc_info").html(text);

                    my_rawGrid.itemsSource = new wijmo.collections.CollectionView(data.records);
                    my_rawGrid.itemsSource.pageSize = 20;
                    my_rawGrid.itemsSource.moveToNextPage();
                    //my_rawGrid.itemsSource.pageIndex = 3;
                }
            },
	        error: function(o, message) {
	            alert(message);
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
    <div id="dialog" title="Login ...">      
        username:<input type="text" name="username" />
</div>
    <form id="form1">
        <textarea id="usr_data" name="usr_data" style="height: 221px; width: 891px"></textarea><br />
        <input type="button" value="ok" id="btk_ok" />
        <div id="proc_status"></div><br />
        <div id="proc_info"></div>

        
    </form>

    <table border="1" width="80%" align="center">
        <tr>
            <td>
                <div id="my_rawGrid"></div>
            </td>
        </tr>
    </table>

<textarea id="wijeditor" style="width: 756px; height: 475px;">
    <h2>Blippity Fling-Flang</h2>
    <p>Ho ha meepfloo hum nip zongle, yap izzle ho noodle da. Doo twaddle zap? dilznoofus Jackson. 
                Loo cake blungflib. Yip dingle ha? bang Mr. Slav. Flab zap dingely dizzleshrubbery. Quabble ha 
        blop da shnuzzle-slap!! Funk hum zang shnuzzle? Crongle loo twaddling hizzywoogle.</p>
<p><a href="http://bff.orangehairedboy.com/">Blippity Fling-Flang by orangehairedboy</a></p>
</body>
</html>