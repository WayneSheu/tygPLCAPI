<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PileSet.aspx.cs" Inherits="PileSet" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI - 更換料種</title>
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
<link href="css/mat_station_edit.css" rel="stylesheet" type="text/css" />
<link href="css/authority_m_add.css" rel="stylesheet" type="text/css" />
<link href="css/combo.select.css" rel="stylesheet" />
<script type="text/javascript">
    function soap() {
        tyg.setHead("<% Response.Write(Session["user"]); %>", true);
        pile.getFactoryOpion();
    }

    function getAreaOption(factoryID) {
        pile.getAreaOption(factoryID);
    }

    function getPlatformOption(areaID) {
        var factoryID = $("#factoryID").val();
        pile.getPlatformOption(factoryID, areaID);
    }

    function getMPileSortList(platformID) {
        pile.getMPileSortList(platformID);
    }

    function modMPile() {
        var setMode = false;
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
					pile.modBatchMPile(setMode, factoryID, areaID, platformID, MPileSort, MPileSerial1, MPileSerial2, MPileSerial3, MRatio, updater);
				}else{
					alert("設定兩個料堆以上時，請選擇混料比例。");
				}
			}else{
				if(MRatio == ""){
					pile.modBatchMPile(setMode, factoryID, areaID, platformID, MPileSort, MPileSerial1, MPileSerial2, MPileSerial3, MRatio, updater);
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
					<h1 class="clearfix ma_10_0"><span class="fn_20 flo_l fn_black fn_bold">供料台</span></h1>
					<div class="hr bc_blue3 ma_btm_20"></div>
					<div id="form-bg" class="pd1 pl1 pr1">
						<form id="mat_station_edit" name="mat_station_edit" method="post" enctype="multipart/form-data" action="###">
							<div class="row"><div class="col-md-12 ta-c"><h3>替換料號</h3></div></div>
							<div class="row">
								<div class="col-sm-2 ta-r item-title">廠區</div>
								<div class="col-sm-4">
									<select class="w100" id="factoryID" name="factoryID" onchange="getAreaOption(this.value)"></select>
								</div>
								<div class="col-sm-4">
									<select class="w100" id="areaID" name="areaID" onchange="getPlatformOption(this.value)"></select>
								</div>
								<div class="col-sm-2 fn_red">依廠挑區</div>
							</div>
							<div class="row">
								<div class="col-sm-2 ta-r item-title">供料台</div>
								<div class="col-sm-8">
									<select class="w100" id="platformID" name="platformID" onchange="getMPileSortList(this.value)"></select>									
								</div>
                                <div class="col-sm-2">
                                </div>
							</div>
							<div class="row">
								<div class="col-sm-2 ta-r red-star item-title red-star">目前料號</div>
								<div class="col-sm-8">
									<select class="w100" id="MPileSort" name="MPileSort"></select>
								</div>
                                <div class="col-sm-2">
                                </div>
							</div>
							<div class="row">
								<div class="col-sm-2 ta-r red-star item-title red-star">替換料號</div>
								<div class="col-sm-8">
									<select class="w100" id="serial1" name="serial1">
                                        <option value="">輸入5碼後啟動篩選</option>
									</select>
								</div>
                                <div class="col-sm-2">
                                    <button type="button" class="btn btn-default reset-btn" onclick="pile.resetMPileOption('serial1', 'cs1', false)">重設</button>
                                </div>
							</div>
                            <div class="row">
								<div class="col-sm-2 ta-r item-title"></div>
								<div class="col-sm-8">
									<select class="w100" id="serial2" name="serial2">
                                        <option value="">輸入5碼後啟動篩選</option>
									</select>
								</div>  
                                <div class="col-sm-2">
                                    <button type="button" class="btn btn-default reset-btn" onclick="pile.resetMPileOption('serial2', 'cs2', true)">重設</button>
                                </div>
							</div>
                            <div class="row">
								<div class="col-sm-2 ta-r item-title"></div>
								<div class="col-sm-8">
									<select class="w100" id="serial3" name="serial3">
                                        <option value="">輸入5碼後啟動篩選</option>
									</select>
								</div>
                                <div class="col-sm-2">
                                    <button type="button" class="btn btn-default reset-btn" onclick="pile.resetMPileOption('serial2', 'cs2', true)">重設</button>
                                </div>
							</div>
							<div class="row">
								<div class="col-sm-2 ta-r item-title"></div>
								<div class="col-sm-8">
									<select class="w100" id="ratio" name="ratio"></select>
								</div>
							</div>
							<div class="row ">
								<div class="col-md-4 col-sm-4 col-xs-12 ma_btm_20">
									<input Type="button" value="料號查詢" onClick="pile.gotoList()" class="btn input_style3">
								</div>
								<div class="col-md-4 col-sm-4 col-xs-12 ma_btm_20">
									<input Type="button" value="取消" onClick="pile.gotoState()" class="btn input_style2">
								</div>
								<div class="col-md-4 col-sm-4 col-xs-12 ma_btm_20">
									<input type="button" value="儲存" onClick="modMPile()" class="btn input_style1">
								</div>
							</div>
						</form>
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
