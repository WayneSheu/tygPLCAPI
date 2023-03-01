<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StatusInfo.aspx.cs" Inherits="StatusInfo" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI - 狀態統計</title>
<link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
<script src="js/bootstrap.min.js" type="text/javascript"></script>
<script src="js/tyg.js" type="text/javascript"></script>
<!--通用-->
<link href="css/general.css" rel="stylesheet" type="text/css" />
<link href="css/luo_default.css" rel="stylesheet" type="text/css" />
<link href="css/all.css"rel="stylesheet" type="text/css" />
<!--私用-->
<link href="css/mat_station_m.css" rel="stylesheet" type="text/css" />
<link href="css/news.css"rel="stylesheet" type="text/css" />
<script type="text/javascript">
    var factoryID = "<% Response.Write(Request.Form["factoryID"].ToString()); %>";
    var areaID = "<% Response.Write(Request.Form["areaID"].ToString()); %>";
    var platformID = "<% Response.Write(Request.Form["platformID"].ToString()); %>";
    var machineID = "<% Response.Write(Request.Form["machineID"].ToString()); %>";
    var startDate = "<% Response.Write(Request.Form["startDate"].ToString()); %>";
    var endDate = "<% Response.Write(Request.Form["endDate"].ToString()); %>";
    var orderID = "<% Response.Write(Request.Form["orderID"].ToString()); %>";    
    var pg = "<% Response.Write(Request.Form["pg"].ToString()); %>";

	function soap(){
        tyg.setHead("<% Response.Write(Session["user"]); %>", true);
        mstatus.getList(factoryID, areaID, platformID, machineID, startDate, endDate, orderID, pg);
    };

    function setStatusPage(target) {
        mstatus.getList(factoryID, areaID, platformID, machineID, startDate, endDate, orderID, target);
    }
</script>
</head>
<body onload="soap()">
	<!--head-->
	<div id="head"></div>
	<!--content-->
	<div id="content">
		<div class="container">
			<div class="v1">
				<div class="ma_0_10">
					<h1 class="clearfix ma_10_0"><span class="fn_20 flo_l fn_black fn_bold main-title">狀態統計</span>
                        <a href="Status.aspx" class="btn btn-default btn-lg flo_r">回上一頁</a>
					</h1>
					<div class="hr bc_blue3 ma_btm_20"></div>
					<!--表格-->
					<div class="table-responsive bor_0">
						<table id="my_table" class="my_table table table-bordered bc_gray3">
							<thead>
								<tr>
									<th>廠區</th>
									<th>區域</th>
									<th>成型機</th>
									<th>工單類型</th>
									<th>工單號碼</th>
									<th>停機次數</th>
									<th>全自動次數</th>
									<th>半自動次數</th>
									<th>手動次數</th>
									<th>換模次數</th>
								</tr>
							</thead>
							<tbody>
							</tbody>
						</table>
					</div>
					<!--導覽列-->
					<nav aria-label="Page navigation" class="txt_center">

                        <div class='container w100'>
                            <div class='row'>
                                <div class='col-sm-6 col-xs-12'>  
                                    <p class="fn_black flo_l" id="tipbar" style="line-height:50px"></p> 
                                </div>
                                <div class='col-sm-6 col-xs-12'>
                                    <ul class="pagination flo_r" id="pagination">							
						            </ul>
                                </div>
                            </div>
                        </div>

					</nav>
					<div class="clr"></div>
				</div>
			</div>
		</div>
	</div>
	<!--foot-->
	<div id="foot" class="txt_center txt_spac_3 ma_10_0">© Copyright 2017 東陽事業集團 All Rights Reserved</div>
</body>
</html>
