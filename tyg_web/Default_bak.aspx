<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI</title>
<link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
<script src="js/bootstrap.min.js" type="text/javascript"></script>
<!--通用-->
<link href="css/luo_default.css" rel="stylesheet" type="text/css" />
<link href="css/general.css" rel="stylesheet" type="text/css" />
<!--私用-->
<link href="css/index.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <!--head-->
	<div id="head">
		<div class="container">
			<div class="row ma_10_0">
				<div class="col-sm-3 col-xs-3"></div>
				<div class="col-sm-6 col-xs-6 txt_center">
					<div class="ma_top_10">
						<span class="fn_20">現場自動化資訊系統</span>
					</div>
				</div>
				<div class="col-sm-3 col-xs-3"></div>
			</div>
		</div>
	</div>
	<!--content-->
	<div id="content">
		<div class="loginpage-box">
			<!-- 登入區塊開始 -->
			<!--<div class="l-loginbox">-->
			<div class="l-member-title txt_center">用戶登入</div>
			<form class="loginform pa_20" method="post" action="Login.aspx">
				<div class="ma_btm_15 pa_top_10">
					<label class="sr-only" for="loginid"></label>
					<div class="input-group">
						<div class="input-group-addon">
							<i class="glyphicon glyphicon-user" aria-hidden="true"></i>
						</div>
						<input type="text" class="form-control" id="username" name="username" placeholder="請輸入您的帳號">
					</div>
				</div>
				<div class="ma_btm_15 pa_top_15">
					<label class="sr-only" for="loginpaddword"></label>
					<div class="input-group">
						<div class="input-group-addon">
							<i class="glyphicon glyphicon-lock" aria-hidden="true"></i>
						</div>
						<input type="password" class="form-control" id="password" name="password" placeholder="請輸入密碼">
					</div>
				</div>
				<div class="clear"></div>
				<input type="submit" value="立即登入" class="btn l-btn-login">
				<div class="clear"></div>
			</form>
			<!--</div>-->
		</div>
	</div>
	<!--foot-->
	<div id="foot" class="txt_center txt_spac_3 ma_10_0">© Copyright 2017 東陽事業集團 All Rights Reserved</div>
</body>
</html>
