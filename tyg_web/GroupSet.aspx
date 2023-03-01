<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GroupSet.aspx.cs" Inherits="GroupMember" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI - 權限組別成員</title>
<link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
<script src="js/bootstrap.min.js" type="text/javascript"></script>
<script src="js/tyg.js" type="text/javascript"></script>
<!--通用-->
<link href="css/general.css" rel="stylesheet" type="text/css" />
<link href="css/luo_default.css" rel="stylesheet" type="text/css" />
<!--私用-->
<link href="css/authority_m_add_member.css" rel="stylesheet" type="text/css" />
<script>
	function soap(){
		tyg.setHead("<% Response.Write(Session["user"]); %>", true);
        $("#groupNo").val("<% Response.Write(Request.QueryString["no"]); %>");
	}
	
	function addMember(){
		var groupNo = $("#groupNo").val();
        var username = $("#username").val();
        var founder = "<% Response.Write(Session["user"]); %>";

        group.addPermission(groupNo, username, founder);
	}
	
	function goBack(){
		var groupNo = $("#groupNo").val();
		group.gotoMember(groupNo);
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
					<h1 class="clearfix ma_10_0"><span class="fn_20 flo_l fn_black fn_bold">權限管理</span></h1>
					<div class="hr bc_blue3 ma_btm_20"></div>
					<form class="loginform pa_20" action="###" method="post">
						<div class="">
							<div class="input-group wid100">
								<input type="hidden" id="groupNo" name="groupNo" />
								<input type="text" id="username" class="form-control" placeholder="請輸入會員名稱查詢" />
							</div>
						</div>
						<div class="clear"></div>
						<div class="row ">
							<div class="col-sm-6 col-xs-12 ma_btm_20">
								<input Type="button" value="取消" onClick="goBack()" class="btn input_style2">
							</div>
							<div class="col-sm-6 col-xs-12 ma_btm_20">
								<input type="button" value="確定加入" onClick="addMember()" class="btn input_style1">
							</div>
						</div>
						<div class="clear"></div>
					</form>
					<div class="clr"></div>
				</div>
			</div>
		</div>
	</div>
	<!--foot-->
	<div id="foot" class="txt_center txt_spac_3 ma_10_0">© Copyright 2017 東陽事業集團 All Rights Reserved</div>
</body>
</html>
