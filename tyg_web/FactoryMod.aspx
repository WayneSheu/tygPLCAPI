<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FactoryMod.aspx.cs" Inherits="FactoryList" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI - 廠區修改</title>
<link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
<script src="js/bootstrap.min.js" type="text/javascript"></script>
<script src="js/tyg.js" type="text/javascript"></script>
<!--通用-->
<link href="css/general.css" rel="stylesheet" type="text/css" />
<link href="css/all.css" rel="stylesheet" type="text/css" />
<link href="css/luo_default.css" rel="stylesheet" type="text/css" />
<!--私用-->
<link href="css/factory_m_edit.css" rel="stylesheet" type="text/css" />
<script type="text/javascript">
	function soap(){
        tyg.setHead("<% Response.Write(Session["user"]); %>", true);
		factory.getFactory("<% Response.Write(Request.QueryString["id"]); %>");
	};
	
	function modFactory(){
		var factoryID = $("#factoryID").val();
		var factoryName = $("#factoryName").val();
		var bgImageStr = $("#bgImageStr").val();
		var note = $("#note").val();
		var updater = "<% Response.Write(Session["user"]); %>";
		
		if(factoryName != ""){
			factory.modFactory(factoryID, factoryName, bgImageStr, note, updater);
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
					<h1 class="clearfix ma_10_0"><span class="fn_20 flo_l fn_black fn_bold">廠區修改</span></h1>
					<div class="hr bc_blue3 ma_btm_20"></div>
					<div class="container view_content">
						<form id="factory-edit" name="factory-edit" method="post" enctype="multipart/form-data" action="###">
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>廠區ID</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<input type="text" id="factoryID" name="factoryID" readonly="readonly" />
									<span class="">禁止修改</span>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>廠區名稱</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<input type="text" id="factoryName" name="factoryName" />
									<span>必填，限定長度10</span>
								</div>
							</div>
							<div class="row ma_btm_20 plan">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1">平面圖</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<input type="text" id="bgImageText" name="bgImageText" readonly="readonly" />
									<input type="file" id="bgImage" name="bgImage" style="display: none;" onchange="factory.setBase64()" />
									<input type="hidden" id="oribgImageStr" name="oribgImageStr" />									
									<input type="hidden" id="bgImageStr" name="bgImageStr" />
									<button type="button" id="upload" name="upload" class="btn btn-primary w30" onclick="factory.uploadPic()">上傳</button><br>
									<span>建議上傳平面圖寬 964 高723</span>
									<div class="row m-t1">
										<div class="col-md-6 col-sm-6 col-sm-12">
											<img id="factoryPic" name="factoryPic" class="upload-img img-100" src="" />
										</div>
									</div>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1">備註</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<textarea id="note" name="note" style="resize: none;"></textarea>
									<span id="modDate"></span>
								</div>
							</div>
							<div class="row ">
								<div class="col-md-6 col-sm-6 col-xs-12 ma_btm_20"><input type="button" value="取消" onClick="factory.gotoList()" class="btn input_style2"></div>
								<div class="col-md-6 col-sm-6 col-xs-12 ma_btm_20"><input type="button" value="儲存" onClick="modFactory()" class="btn input_style1"></div>
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
