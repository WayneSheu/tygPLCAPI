<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StatusPlan.aspx.cs" Inherits="StatusPlan" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI - 成型機狀態</title>
<link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
<script src="js/bootstrap.min.js" type="text/javascript"></script>
<script src="js/tyg.js" type="text/javascript"></script>
<!--通用-->
<link href="css/general.css" rel="stylesheet" type="text/css" />
<link href="css/luo_default.css" rel="stylesheet" type="text/css" />
<link href="css/all.css" rel="stylesheet" type="text/css" />
<!--私用-->
<link href="css/device_status_detail.css" rel="stylesheet" type="text/css" />
<link href="css/device_status_plan.css" rel="stylesheet" type="text/css" />
<script src="js/device_status_plan.js"></script>
<script type="text/javascript">
	function soap(){
        tyg.setHead("<% Response.Write(Session["user"]); %>", true);
		
		var factoryID = "<% Response.Write(Request.Form["factoryID"]); %>";		
		
		machine.getFactoryList(factoryID);		
	};

    function changeFactory() {
        form.submit();
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
				<!--按鈕-->
				<div class="row">
					<div class="col-md-2 col-sm-3 col-xs-12">
						<a href="StatusDetail.aspx" class="btn btn-default w100">詳細資料</a>
					</div>
                    <div class="col-md-2 col-sm-3 col-xs-12">
						<a class="btn btn-default disabled  btn-detail w100">平面圖</a>
					</div>
                    <div class="col-md-8 col-sm-6 col-xs-12">
                        <!--查詢-->
				        <form id="form" class="m-b2 search-fac" method="post" action="StatusPlan.aspx">
					        <div class="row">
						        <div class="col-md-11 col-sm-11 col-xs-12">
							        <!-- <span class="fn_black search-txt">廠區</span> -->
							        <select class="w50 fn_black" id="factoryID" name="factoryID" onchange="changeFactory()"></select>
							        <!-- <input type="submit" value="查詢" class="btn btn-primary" /> -->
						        </div>
					        </div>
				        </form>
                    </div>
				</div>				
			</div>
		</div>
		<div class="scroll_rect" style="">
			<div class="device_area">
				<img id="device_bg" src="image/fa_bg.png" alt="" />
				<div id="machineArea"></div>
				<div class="clr"></div>
			</div>
		</div>
	</div>
	<!--foot-->
	<div id="foot" class="txt_center txt_spac_3 ma_10_0">© Copyright 2017 東陽事業集團 All Rights Reserved</div>
</body>
</html>