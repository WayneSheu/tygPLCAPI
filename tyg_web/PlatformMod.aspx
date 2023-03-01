<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PlatformMod.aspx.cs" Inherits="PlatformList" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI - 供料台修改</title>
<link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
<script src="js/bootstrap.min.js" type="text/javascript"></script>
<script src="js/tyg.js" type="text/javascript"></script>
<!--通用-->
<link href="css/general.css" rel="stylesheet" type="text/css" />
<link href="css/luo_default.css" rel="stylesheet" type="text/css" />
<link href="css/all.css" rel="stylesheet" type="text/css" />
<!--私用-->
<link href="css/mat_station_m_edit.css" rel="stylesheet" type="text/css" />
<script type="text/javascript">	
	function soap(){
		tyg.setHead("<% Response.Write(Session["user"]); %>", true);				
        platform.getPlatform("<% Response.Write(Request.QueryString["id"]); %>");
	};
	
	function modPlatform(){		
		var factoryID = $("#factoryID").val();
		var areaID = $("#areaID").val();
		var platformID = $("#platformID").val();
		var platformName = $("#platformName").val();
		var platformIP = $("#platformIP").val();
		var platformPort = $("#platformPort").val();
		var oldMNum = parseInt($("#oldMNum").val());
		var mNum = $("#mNum").val();
		var dNum = $("#dNum").val();
		var note = $("#note").val();
		var updater = "<% Response.Write(Session["user"]); %>";
		
		if(platformID != "" && platformName != "" && platformIP != "" && platformPort != ""){
			platform.modPlatform(factoryID, areaID, platformID, platformName, platformIP, platformPort, mNum, dNum, note, updater);
			
			if(oldMNum > mNum){
				for(i=oldMNum;i>mNum;i--){
					var MPileSort = i;
					pile.delMPileByPlatformSort(platformID, MPileSort);
				}
			}else if(oldMNum < mNum){
				for(i=(oldMNum+1);i<=mNum;i++){
					var MPileSort = i;
					var MPileSerial1 = "";
					var MPileSerial2 = "";
					var MPileSerial3 = "";
					
					pile.addMPile(platformID, MPileSort, "", "", "", updater);
				}
			}
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
					<h1 class="clearfix ma_10_0"><span class="fn_20 flo_l fn_black fn_bold">供料台修改</span></h1>
					<div class="hr bc_blue3 ma_btm_20"></div>
					<div class="container view_content">
						<div class="row ma_btm_20">
							<div class="col-md-3 col-sm-3 col-xs-12">
								<p class="title_lvl1"><span class="fn_red">*</span>廠區</p>
							</div>
							<div class="col-md-9 col-sm-9 col-xs-12">
								<select id="factoryID" name="factoryID" onchange="platform.getAreaList(this.value)"></select>
								<select id="areaID" name="areaID"></select>
								<span class="fn_red">依廠挑區</span>
							</div>
						</div>
						<div class="row ma_btm_20">
							<div class="col-md-3 col-sm-3 col-xs-12">
								<p class="title_lvl1"><span class="fn_red">*</span>供料台ID</p>
							</div>
							<div class="col-md-9 col-sm-9 col-xs-12">
								<input type="text" id="platformID" name="platformID" readonly="readonly" />
								<span>禁止修改</span>
							</div>
						</div>
						<div class="row ma_btm_20">
							<div class="col-md-3 col-sm-3 col-xs-12">
								<p class="title_lvl1"><span class="fn_red">*</span>供料台名稱</p>
							</div>
							<div class="col-md-9 col-sm-9 col-xs-12">
								<input type="text" id="platformName" name="platformName" />
								<span>必填，限定長度10</span>
							</div>
						</div>
						<div class="row ma_btm_20">
							<div class="col-md-3 col-sm-3 col-xs-12">
								<p class="title_lvl1"><span class="fn_red">*</span>供料台IP</p>
							</div>
							<div class="col-md-9 col-sm-9 col-xs-12">
								<input type="text" id="platformIP" name="platformIP" />
								必填，限定數字、長度15
							</div>
						</div>
						<div class="row ma_btm_20">
							<div class="col-md-3 col-sm-3 col-xs-12">
								<p class="title_lvl1"><span class="fn_red">*</span>供料台Port</p>
							</div>
							<div class="col-md-9 col-sm-9 col-xs-12">
								<input type="text" id="platformPort" name="platformPort" />
								必填，限定數字、長度4
							</div>
						</div>
						<div class="row ma_btm_20">
							<div class="col-md-3 col-sm-3 col-xs-12">
								<p class="title_lvl1"><span class="fn_red">*</span>料堆數量</p>
							</div>
							<div class="col-md-9 col-sm-9 col-xs-12">
								<input type="hidden" id="oldMNum" name="oldMNum" />
								<select id="mNum" name="mNum">
									<option value="1">1</option>
									<option value="2">2</option>
									<option value="3">3</option>
									<option value="4">4</option>
									<option value="5">5</option>
									<option value="6">6</option>
								</select>
								<span class="fn_red ma_left_15">最多6</span>								
							</div>
						</div>
						<div class="row ma_btm_20">
							<div class="col-md-3 col-sm-3 col-xs-12">
								<p class="title_lvl1"><span class="fn_red">*</span>行(垂直)料孔數</p>
							</div>
							<div class="col-md-9 col-sm-9 col-xs-12">
								<select id="dNum" name="dNum">
									<option value="1">1</option>
									<option value="2">2</option>
									<option value="3">3</option>
									<option value="4">4</option>
									<option value="5">5</option>
								</select> <span class="fn_red ma_left_15">最多5</span>
							</div>
						</div>
						<div class="row ma_btm_20">
							<div class="col-md-3 col-sm-3 col-xs-12">
								<p class="title_lvl1">備註</p>
							</div>
							<div class="col-md-9 col-sm-9 col-xs-12">
								<textarea id="note" name="note" style="resize: none;"></textarea>
							</div>
						</div>
						<div class="row ">
							<div class="col-md-6 col-sm-6 col-xs-12 ma_btm_20"><input Type="button" value="取消" onClick="platform.gotoList()" class="btn input_style2"></div>
							<div class="col-md-6 col-sm-6 col-xs-12 ma_btm_20"><input type="button" value="儲存" onClick="modPlatform()" class="btn input_style1"></div>
						</div>
					</div>
					<div class="clr"></div>
				</div>
			</div>
		</div>
	</div>
	<!--foot-->
	<div id="foot" class="txt_center txt_spac_3 ma_10_0">© Copyright 2017 東陽事業集團 All Rights Reserved</div>	
</body>
</html>
