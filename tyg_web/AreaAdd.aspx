<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AreaAdd.aspx.cs" Inherits="AreaList" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI - 區域成型機新增</title>
<link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
<script src="js/bootstrap.min.js" type="text/javascript"></script>
<script src="js/tyg.js" type="text/javascript"></script>
<!--通用-->
<link href="css/general.css" rel="stylesheet" type="text/css" />
<link href="css/all.css" rel="stylesheet" type="text/css" />
<link href="css/luo_default.css" rel="stylesheet" type="text/css" />
<!--私用-->
<link href="css/mat_station_m_edit.css" rel="stylesheet" type="text/css" />
<link href="css/area_device_m_edit.css" rel="stylesheet" type="text/css" />
<script type="text/javascript">
	function soap(){
		tyg.setHead("<% Response.Write(Session["user"]); %>", true);
		area.getFactoryList();
	};
	
	function addArea(){
		var factoryID = $("#factoryID").val();
		var areaID = $("#areaID").val();
		var areaName = $("#areaName").val();
		var areaIP = $("#areaIP").val();
		var areaPort = $("#areaPort").val();
		var testMode = $("#testMode").val();
		var note = $("#note").val();
		var founder = "<% Response.Write(Session["user"]); %>";
		
		if(areaID != "" && areaName != "" && areaIP != "" && areaPort != ""){
			area.addArea(factoryID, areaID, areaName, areaIP, areaPort, testMode, note, founder);
		}else{
			alert("請填入所有必填欄位!!!");
		}
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
					<h1 class="clearfix ma_10_0"><span class="fn_20 flo_l fn_black fn_bold">區域成型機新增</span></h1>
					<div class="hr bc_blue3 ma_btm_20"></div>
					<div class="container view_content">
						<form id="area-add" name="area-add" method="post" action="###">
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>廠區</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<select id="factoryID" name="factoryID"></select>
									<span class="">若找不到廠區，請先至廠區管理新增廠區</span>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>區域ID</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<input type="text" id="areaID" name="areaID" />
									<span>必填，限定英數組合、長度3，不可重復</span>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>區域名稱</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<input type="text" id="areaName" name="areaName" />
									<span>必填，限定長度10</span>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>主PLC IP</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<input type="text" id="areaIP" name="areaIP" />
									<span>必填，限定數字、長度15</span>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>主PLC Port</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<input type="text" id="areaPort" name="areaPort" />
									<span>必填，限定數字、長度4</span>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>測試模式</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<select id="testMode" name="testMode">
										<option value="1">是</option>
										<option value="0">否</option>
									</select>
									<span>必填</span>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="row ma_btm_20">
									<div class="col-md-3 col-sm-3 col-xs-12">
										<p class="title_lvl1">備註</p>
									</div>
									<div class="col-md-9 col-sm-9 col-xs-12">
										<textarea id="note" name="note" style="resize: none;"></textarea>										
									</div>
								</div>
								<div class="row ">
									<div class="col-md-6 col-sm-6 col-xs-12 ma_btm_20"><input Type="button" value="取消" onClick="area.gotoList()" class="btn input_style2"></div>
									<div class="col-md-6 col-sm-6 col-xs-12 ma_btm_20"><input type="button" value="儲存" onClick="addArea()" class="btn input_style1"></div>
								</div>
							</div>
							<div class="clr"></div>
						</form>
					</div>
				</div>
			</div>
		</div>
	</div>
	<!--foot-->
	<div id="foot" class="txt_center txt_spac_3 ma_10_0">© Copyright 2017 東陽事業集團 All Rights Reserved</div>
</body>
</html>
