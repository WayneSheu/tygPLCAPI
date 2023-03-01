<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Menu.aspx.cs" Inherits="Menu" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI - 功能列表</title>
<link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
<script src="js/bootstrap.min.js" type="text/javascript"></script>
<script src="js/tyg.js" type="text/javascript"></script>
<!--通用-->
<link href="css/luo_default.css" rel="stylesheet" type="text/css" />
<link href="css/general.css" rel="stylesheet" type="text/css" />
<!--私用-->
<link href="css/menu.css" rel="stylesheet" type="text/css" />
<script type="text/javascript">
	function soap(){
		tyg.setHead("<% Response.Write(Session["user"]); %>", false);
	};
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
					<h1 class="clearfix ma_top_10 ma_btm_20">
						<span class="fn_22 flo_l fn_black">功能列表</span>
						<span class="fn_22 flo_r fn_black"></span>
					</h1>
					<!--<div class="hr bc_blue3 ma_btm_20"></div>-->
					<ul class="nav nav-tabs" role="tablist" id="myTab">
                        <% if (DetailAuth != 0 || InfoAuth != 0 || NewsAuth != 0){ %>
						<li role="presentation" class="active"><a href="#member_set" aria-controls="member_set" role="tab" data-toggle="tab">即時資料</a></li>
                        <% } %>
                        <% if (HistoryAuth != 0 || StatusAuth != 0){ %>
                        <li role="presentation"><a href="#history_set" aria-controls="history_set" role="tab" data-toggle="tab">歷史資訊</a></li>
                        <% } %>
                        <% if (FactoryAuth != 0 || AreaAuth != 0 || PlatformAuth != 0 || GroupAuth != 0){ %>
						<li role="presentation"><a href="#authority_set" aria-controls="authority_set" role="tab" data-toggle="tab">基本設定</a></li>
                        <% } %>
					</ul>
					<div class="tab-content">
                        <% if (DetailAuth != 0 || InfoAuth != 0 || NewsAuth != 0){ %>
						<div role="tabpanel" class="tab-pane active" id="member_set">
                            <% if (DetailAuth != 0){ %>
							<!--成型機狀態-->
							<div class="col-md-2 col-sm-3 col-xs-4">
								<a href="StatusDetail.aspx"><img class="wid100" src="image/mbtn.png" alt="" /></a>
							</div>
                            <% } %>
                            <% if (InfoAuth != 0){ %>
							<!--供料台-->
							<div class="col-md-2 col-sm-3 col-xs-4">
								<a href="PlatformState.aspx"><img class="wid100" src="image/cbtn.png" alt="" /></a>
							</div>
                            <% } %>
                            <% if (NewsAuth != 0){ %>
							<!--即時資訊-->
							<div class="col-md-2 col-sm-3 col-xs-4">
								<a href="News.aspx"><img class="wid100" src="image/ibtn.png" alt="" /></a>
							</div>
                            <% } %>
						</div>
                        <% } %>
                        <% if (HistoryAuth != 0 || StatusAuth != 0){ %>
                        <div role="tabpanel" class="tab-pane" id="history_set">
                            <% if (HistoryAuth != 0){ %>
							<!--歷史資料-->
							<div class="col-md-2 col-sm-3 col-xs-4">
								<a href="History.aspx"><img class="wid100" src="image/sbtn.png" alt="" /></a>
							</div>
                            <% } %>
                            <% if (StatusAuth != 0){ %>
							<!--狀態統計-->
							<div class="col-md-2 col-sm-3 col-xs-4">
								<a href="Status.aspx"> <img class="wid100" src="image/statistic-btn.png" alt="" /></a>
							</div>
                            <% } %>
						</div>
                        <% } %>
                        <% if (FactoryAuth != 0 || AreaAuth != 0 || PlatformAuth != 0 || GroupAuth != 0){ %>
						<div role="tabpanel" class="tab-pane" id="authority_set">
							<% if (FactoryAuth != 0){ %>
                            <!--廠區管理-->
							<div class="col-md-2 col-sm-3 col-xs-4">
								<a href="FactoryList.aspx"><img class="wid100" src="image/fabtn.png" alt="" /></a>
							</div>
                            <% } %>
                            <% if (AreaAuth != 0){ %>
							<!--區域成型機管理-->
							<div class="col-md-2 col-sm-3 col-xs-4">
								<a href="AreaList.aspx"><img class="wid100" src="image/area-btn.png" alt="" /></a>
							</div>
                            <% } %>
                            <% if (PlatformAuth != 0){ %>
							<!--供料台管理-->
							<div class="col-md-2 col-sm-3 col-xs-4">
								<a href="PlatformList.aspx"><img class="wid100" src="image/fbtn.png" alt="" /></a>
							</div>
                            <% } %>
                            <% if (GroupAuth != 0){ %>
                            <!--權限管理-->
                            <div class="col-md-2 col-sm-3 col-xs-4">
								<a href="GroupList.aspx"><img class="wid100" src="image/abtn.png" alt="" /></a>
							</div>
                            <% } %>
						</div>
                        <% } %>
					</div>
				</div>
			</div>
		</div>
	</div>
	<!--foot-->
	<div id="foot" class="txt_center txt_spac_3 ma_10_0">© Copyright 2017 東陽事業集團 All Rights Reserved</div>
</body>
</html>
