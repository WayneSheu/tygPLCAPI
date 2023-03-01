<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MachineAdd.aspx.cs" Inherits="MachineList" %>
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
        machine.getPlatformList("<% Response.Write(Request.QueryString["aid"]); %>");
	};
	
	function addMachine(){
		var factoryID = $("#factoryID").val();
		var areaID = $("#areaID").val();
		var platformID = $("#platformID").val();
		var machineID = $("#machineID").val();
		var slaveNo = $("#slaveNo").val();
		var channel = $("#channel").val();
		var coordinateX = $("#coordinateX").val();
		var coordinateY = $("#coordinateY").val();
		var founder = "<% Response.Write(Session["user"]); %>";
		
		if(machineID != "" && coordinateX != "" && coordinateY != ""){
			machine.addMachine(factoryID, areaID, platformID, machineID, slaveNo, channel, coordinateX, coordinateY, founder);
		}else{
			alert("請填入所有必填欄位!!!");
		}
	}
	
	function gotoSet(){
		var strUrl = location.search;
		var areaID = strUrl.split("?aid=");
		
		area.gotoSet(areaID[1]);
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
									<p class="title_lvl1"><span class="fn_red">*</span>供料台</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<input type="hidden" id="factoryID" name="factoryID" />
									<input type="hidden" id="areaID" name="areaID" />
									<select id="platformID" name="platformID"></select>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>成型機ID</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<input type="text" id="machineID" name="machineID" />
									<span>必填，限定數字、長度4，不可重復</span>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>Slave No</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<select id="slaveNo" name="slaveNo" >
										<option value="1">1</option>
										<option value="2">2</option>
										<option value="3">3</option>
										<option value="4">4</option>
										<option value="5">5</option>
										<option value="6">6</option>
										<option value="7">7</option>
										<option value="8">8</option>
										<option value="9">9</option>
										<option value="10">10</option>
										<option value="11">11</option>
										<option value="12">12</option>
										<option value="13">13</option>
										<option value="14">14</option>
										<option value="15">15</option>
										<option value="16">16</option>
									</select>
									<span>限1~16</span>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>Channel</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<select id="channel" name="channel" >
										<option value="1">1</option>
										<option value="2">2</option>
										<option value="3">3</option>
										<option value="4">4</option>
										<option value="5">5</option>
										<option value="6">6</option>
										<option value="7">7</option>
									</select>
									<span>限1~7</span>
								</div>
							</div>
                            <div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>成型機位置右下座標X</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<input type="text" id="coordinateY" name="coordinateY" />
									<span>必填，限定數字、長度4</span>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>成型機位置右下座標Y</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<input type="text" id="coordinateX" name="coordinateX" />
									<span>必填，限定數字、長度4</span>
								</div>
							</div>							
							<div class="row ma_btm_20">
								<div class="row ">
									<div class="col-md-6 col-sm-6 col-xs-12 ma_btm_20"><input Type="button" value="取消" onClick="gotoSet()" class="btn input_style2"></div>
									<div class="col-md-6 col-sm-6 col-xs-12 ma_btm_20"><input type="button" value="儲存" onClick="addMachine()" class="btn input_style1"></div>
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
