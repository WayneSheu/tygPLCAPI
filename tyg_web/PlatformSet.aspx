<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PlatformSet.aspx.cs" Inherits="PlatformSet" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI - 供料台管理</title>
<link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
<script src="js/bootstrap.min.js" type="text/javascript"></script>
<script src="js/jquery.combo.select.js"></script>
<script src="js/tyg.js" type="text/javascript"></script>
<!--通用-->
<link href="css/general.css" rel="stylesheet" type="text/css" />
<link href="css/luo_default.css" rel="stylesheet" type="text/css" />
<link href="css/all.css" rel="stylesheet" type="text/css" />
<!--私用-->
<link href="css/mat_station_m_edit.css" rel="stylesheet" type="text/css" />
<link href="css/combo.select.css" rel="stylesheet" />
<script type="text/javascript">	
	function soap(){
		tyg.setHead("<% Response.Write(Session["user"]); %>", true);
        pile.getMPileList("<% Response.Write(Request.QueryString["pid"]); %>", "<% Response.Write(Request.QueryString["sort"]); %>", true);
	};
	
	function gotoPile(){
		var platformID = $("#platformID").val();
		platform.gotoPile(platformID);
	}
	
    function modMPile() {
        var setMode = true;
		var factoryID = $("#factoryID").val();
		var areaID = $("#areaID").val();
		var platformID = $("#platformID").val();
		var MPileSort = $("#MPileSort").val();
		var MPileSerial1 = $("#serial1").val();
		var MPileSerial2 = $("#serial2").val();
		var MPileSerial3 = $("#serial3").val();
		var MRatio = $("#ratio").val();
		var updater = "<% Response.Write(Session["user"]); %>";		
		
        if (MPileSerial1 != "") {
			if(MPileSerial2 != "" ||MPileSerial3 != ""){
				if(MRatio != ""){
					pile.modMPile(setMode, factoryID, areaID, platformID, MPileSort, MPileSerial1, MPileSerial2, MPileSerial3, MRatio, updater);
				}else{
					alert("設定兩個料堆以上時，請選擇混料比例。");
				}
			}else{
				if(MRatio == ""){
					pile.modMPile(setMode, factoryID, areaID, platformID, MPileSort, MPileSerial1, MPileSerial2, MPileSerial3, MRatio, updater);
				}else{
					alert("僅設定一個料堆時，請勿選擇混料比例。");
				}
			}            
        } else {
            alert("請至少設定料堆1");
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
					<h1 class="clearfix ma_10_0"><span class="fn_20 flo_l fn_black fn_bold">料堆設定</span></h1>
					<div class="hr bc_blue3 ma_btm_20"></div>
					<div class="container view_content">
						<div class="row ma_btm_20">
							<div class="col-sm-3 col-xs-12">
								<p class="title_lvl1"><span class="fn_red">*</span>料種1</p>
							</div>
							<div class="col-sm-7 col-xs-12">
								<input type="hidden" id="factoryID" name="factoryID" />
								<input type="hidden" id="areaID" name="areaID" />
								<input type="hidden" id="platformID" name="platformID" />
								<input type="hidden" id="MPileSort" name="MPileSort" />
								<select id="serial1" name="serial1"></select>
							</div>
                            <div class=" col-sm-2 col-xs-12">
                                 <button type="button" class="btn btn-default reset-btn" onclick="pile.resetMPileOption('serial1', 'cs1', false)">重設</button>
                            </div>
						</div>
						<div class="row ma_btm_20">
							<div class="col-md-3 col-sm-3 col-xs-12">
								<p class="title_lvl1"><span class="fn_red">*</span>料種2</p>
							</div>
							<div class="col-sm-7 col-xs-12">
								<select id="serial2" name="serial2"></select>
							</div>
                            <div class=" col-sm-2 col-xs-12">
                                <button type="button" class="btn btn-default reset-btn" onclick="pile.resetMPileOption('serial2', 'cs2', false)">重設</button>
                            </div>
						</div>
						<div class="row ma_btm_20">
							<div class="col-md-3 col-sm-3 col-xs-12">
								<p class="title_lvl1"><span class="fn_red">*</span>料種3</p>
							</div>
							<div class="col-sm-7 col-xs-12">
								<select id="serial3" name="serial3"></select>
							</div>
                            <div class=" col-sm-2 col-xs-12">
                                <button type="button" class="btn btn-default reset-btn" onclick="pile.resetMPileOption('serial3', 'cs3', false)">重設</button>
                            </div>
						</div>
                        <div class="row ma_btm_20">
							<div class="col-md-3 col-sm-3 col-xs-12">
								<p class="title_lvl1"><span class="fn_red">*</span>混料比例</p>
							</div>
							<div class="col-sm-7 col-xs-12">
								<select id="ratio" name="ratio" ></select>
							</div>
						</div>
						<div class="row ">
							<div class="col-md-6 col-sm-6 col-xs-12 ma_btm_20"><input Type="button" value="取消" onClick="gotoPile()" class="btn input_style2"></div>
							<div class="col-md-6 col-sm-6 col-xs-12 ma_btm_20"><input type="button" value="儲存" onClick="modMPile()" class="btn input_style1"></div>
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
