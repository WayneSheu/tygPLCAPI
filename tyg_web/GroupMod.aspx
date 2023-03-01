<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GroupMod.aspx.cs" Inherits="GroupMod" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI - 權限組別修改</title>
<link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
<script src="js/bootstrap.min.js" type="text/javascript"></script>
<script src="js/tyg.js" type="text/javascript"></script>
<script src="js/chosen.jquery.js" type="text/javascript"></script>
<!--通用-->
<link href="css/general.css" rel="stylesheet" type="text/css" />
<link href="css/luo_default.css" rel="stylesheet" type="text/css" />
<!--私用-->
<link href="css/authority_m_add.css" rel="stylesheet" type="text/css" />
<link href="css/chosen.css" rel="stylesheet" type="text/css" />
<script>
	function soap(){
		tyg.setHead("<% Response.Write(Session["user"]); %>", true);
        group.getGroups("<% Response.Write(Request.QueryString["no"]); %>");
	};

    function showAllow(show) {
        if (show) {
            $("#allowArea").attr("style", "");
            $("#allowAuth").chosen();
        } else {
            $("#allowArea").attr("style", "display: none;");
        }
    }
	
    function modGroups() {
        var groupNo = $("#groupNo").val();
        var groupName = $("#groupName").val();
        var detailAuth = $("input[name='detailAuth']:checked").val();
        var infoAuth = $("input[name='infoAuth']:checked").val();
        var allowAuth = ((infoAuth == "2") ? $("#allowAuth").val() : "");
        var newsAuth = $("input[name='newsAuth']:checked").val();
        var historyAuth = $("input[name='historyAuth']:checked").val();
        var statusAuth = $("input[name='statusAuth']:checked").val();
        var factoryAuth = $("input[name='factoryAuth']:checked").val();
        var areaAuth = $("input[name='areaAuth']:checked").val();
        var platformAuth = $("input[name='platformAuth']:checked").val();
        var groupAuth = $("input[name='groupAuth']:checked").val();
        var updater = "<% Response.Write(Session["user"]); %>";

        if (groupName != "") {
            group.modGroups(groupNo, groupName, detailAuth, infoAuth, allowAuth, newsAuth, historyAuth, statusAuth, factoryAuth, areaAuth, platformAuth, groupAuth, updater);
        } else {
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
					<h1 class="clearfix ma_10_0"><span class="fn_20 flo_l fn_black fn_bold">權限管理</span></h1>
					<div class="hr bc_blue3 ma_btm_20"></div>					
					<div class="tab-content">
						<div role="tabpanel" class="tab-pane active" id="authority_set">
							<div class="container wid100">
								<div class="row ma_btm_20">
									<div class="col-md-3 col-sm-3 col-xs-12">
										<p class="title_lvl1">組別名稱</p>
									</div>
									<div class="col-md-9 col-sm-9 col-xs-12">
										<p>
											<input type="hidden" id="groupNo" name="groupNo">
											<input type="text" id="groupName" name="groupName" class="wid100">
										</p>
									</div>
								</div>
								<div class="row ">
									<div class="col-md-3 col-sm-3 col-xs-12">
										<p class="title_lvl1">即時資訊權限</p>
									</div>
									<div class="col-md-9 col-sm-9 col-xs-12">
										<p></p>
									</div>
								</div>
								<div class="row">
									<div class="pa_0_15">
										<div class="row ma_btm_20">
											<div class="col-sm-3 col-xs-12">
												<p class="title_lvl2 ">成型機狀態</p>
											</div>
											<div class="col-sm-9 col-xs-12">
												<p class="radio-group">
													<label class="radio-inline"><input type="radio" id="detailAuth" name="detailAuth" value="0">不可使用</label>
													<label class="radio-inline"><input type="radio" id="detailAuth" name="detailAuth" value="1">可瀏覽</label>
												</p>
											</div>
										</div>
										<div class="row ma_btm_20">
											<div class="col-sm-3 col-xs-12">
												<p class="title_lvl2 ">供料台</p>
											</div>
											<div class="col-sm-9 col-xs-12">
												<p class="radio-group">
													<label class="radio-inline"><input type="radio" id="infoAuth" name="infoAuth" value="0" onchange="showAllow(false)">不可使用</label>
													<label class="radio-inline"><input type="radio" id="infoAuth" name="infoAuth" value="1" onchange="showAllow(false)">可瀏覽</label>
													<label class="radio-inline"><input type="radio" id="infoAuth" name="infoAuth" value="2" onchange="showAllow(true)">可編輯</label>
												</p>
											</div>
										</div>
                                        <div class="row ma_btm_20" id="allowArea" style="display: none;">
											<div class="col-sm-3 col-xs-12">
												<p class="title_lvl2 ">可操作廠區</p>
											</div>
											<div class="col-sm-9 col-xs-12" style="margin-top: 10px;">
												<select id="allowAuth" name="allowAuth" multiple="multiple" data-placeholder="請選擇可操作廠區..." style="width: 400px;"></select>
											</div>
										</div>
										<div class="row ma_btm_20">
											<div class="col-sm-3 col-xs-12">
												<p class="title_lvl2 ">即時資訊</p>
											</div>
											<div class="col-sm-9 col-xs-12">
												<p class="radio-group">
													<label class="radio-inline"><input type="radio" id="newsAuth" name="newsAuth" value="0">不可使用</label>
													<label class="radio-inline"><input type="radio" id="newsAuth" name="newsAuth" value="1">可瀏覽</label>
												</p>
											</div>
										</div>										
									</div>
								</div>
                                <div class="row ">
									<div class="col-md-3 col-sm-3 col-xs-12">
										<p class="title_lvl1">歷史資訊權限</p>
									</div>
									<div class="col-md-9 col-sm-9 col-xs-12">
										<p></p>
									</div>
								</div>
                                <div class="row">
									<div class="pa_0_15">										
										<div class="row ma_btm_20">
											<div class="col-sm-3 col-xs-12">
												<p class="title_lvl2 ">歷史資料</p>
											</div>
											<div class="col-sm-9 col-xs-12">
												<p class="radio-group">
													<label class="radio-inline"><input type="radio" id="historyAuth" name="historyAuth" value="0" checked="checked">不可使用</label>
													<label class="radio-inline"><input type="radio" id="historyAuth" name="historyAuth" value="1">可瀏覽</label>
												</p>
											</div>
										</div>
										<div class="row ma_btm_20">
											<div class="col-sm-3 col-xs-12">
												<p class="title_lvl2 ">狀態統計</p>
											</div>
											<div class="col-sm-9 col-xs-12">
												<p class="radio-group">
													<label class="radio-inline"><input type="radio" id="statusAuth" name="statusAuth" value="0" checked="checked">不可使用</label>
													<label class="radio-inline"><input type="radio" id="statusAuth" name="statusAuth" value="1">可瀏覽</label>
												</p>
											</div>
										</div>
									</div>
								</div>
								<div class="row ">
									<div class="col-md-3 col-sm-3 col-xs-12">
										<p class="title_lvl1">基本資料權限</p>
									</div>
									<div class="col-md-9 col-sm-9 col-xs-12">
										<p></p>
									</div>
								</div>
								<div class="row">
									<div class="pa_0_15">
										<div class="row ma_btm_20">
											<div class="col-sm-3 col-xs-12">
												<p class="title_lvl2 ">廠區管理</p>
											</div>
											<div class="col-sm-9 col-xs-12">
												<p class="radio-group">
													<label class="radio-inline"><input type="radio" id="factoryAuth" name="factoryAuth" value="0">不可使用</label>
													<label class="radio-inline"><input type="radio" id="factoryAuth" name="factoryAuth" value="1">可瀏覽</label>
													<label class="radio-inline"><input type="radio" id="factoryAuth" name="factoryAuth" value="2">可編輯</label>
												</p>
											</div>
										</div>
										<div class="row ma_btm_20">
											<div class="col-sm-3 col-xs-12">
												<p class="title_lvl2 ">區域成型機管理</p>
											</div>
											<div class="col-sm-9 col-xs-12">
												<p class="radio-group">
													<label class="radio-inline"><input type="radio" id="areaAuth" name="areaAuth" value="0">不可使用</label>
													<label class="radio-inline"><input type="radio" id="areaAuth" name="areaAuth" value="1">可瀏覽</label>
													<label class="radio-inline"><input type="radio" id="areaAuth" name="areaAuth" value="2">可編輯</label>
												</p>
											</div>
										</div>
										<div class="row ma_btm_20">
											<div class="col-sm-3 col-xs-12">
												<p class="title_lvl2 ">供料台管理</p>
											</div>
											<div class="col-sm-9 col-xs-12">
												<p class="radio-group">
													<label class="radio-inline"><input type="radio" id="platformAuth" name="platformAuth" value="0">不可使用</label>
													<label class="radio-inline"><input type="radio" id="platformAuth" name="platformAuth" value="1">可瀏覽</label>
													<label class="radio-inline"><input type="radio" id="platformAuth" name="platformAuth" value="2">可編輯</label>
												</p>
											</div>
										</div>
										<div class="row ma_btm_20">
											<div class="col-sm-3 col-xs-12">
												<p class="title_lvl2 ">權限管理</p>
											</div>
											<div class="col-sm-9 col-xs-12">
												<p class="radio-group">
													<label class="radio-inline"><input type="radio" id="groupAuth" name="groupAuth" value="0">不可使用</label>
													<label class="radio-inline"><input type="radio" id="groupAuth" name="groupAuth" value="1">可瀏覽</label>
													<label class="radio-inline"><input type="radio" id="groupAuth" name="groupAuth" value="2">可編輯</label>
												</p>
												<span id="modDate"></span>
											</div>
										</div>
									</div>									
								</div>
								<div class="row">
									<div class="col-sm-6 col-xs-12 ma_btm_20">
										<input Type="button" value="取消" onClick="group.gotoList()" class="btn input_style2">
									</div>
									<div class="col-sm-6 col-xs-12 ma_btm_20">
										<input type="submit" value="儲存" onClick="modGroups()" class="btn input_style1">
									</div>
								</div>
							</div>
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
