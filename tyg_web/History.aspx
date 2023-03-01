<%@ Page Language="C#" AutoEventWireup="true" CodeFile="History.aspx.cs" Inherits="History" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI - 歷史資料</title>
<link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
<link href="css/jquery-ui-1.12.1.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-ui-1.12.1.js" type="text/javascript" ></script>
<script src="js/bootstrap.min.js" type="text/javascript"></script>
<script src="js/tyg.js" type="text/javascript"></script>
<!--通用-->
<link href="css/general.css" rel="stylesheet" type="text/css" />
<link href="css/luo_default.css" rel="stylesheet" type="text/css" />
<link href="css/all.css" rel="stylesheet" type="text/css" />
<!--私用-->
<link href="css/mat_station_m_edit.css" rel="stylesheet" type="text/css" />
<link href="css/history.css" rel="stylesheet" type="text/css" />

<script type="text/javascript">
	function soap(){
        tyg.setHead("<% Response.Write(Session["user"]); %>", true);
        search.getSearchElement();
    };

    function getAreaOption(factoryID) {
        search.getAreaOption(factoryID);
    }

    function getPlatformOption(areaID) {
        var factoryID = $("#factoryID").val();
        search.getPlatformOption(factoryID, areaID);
    }

    function getMachineOption(platformID) {
        var factoryID = $("#factoryID").val();
        var areaID = $("#areaID").val();
        search.getMachineOption(factoryID, areaID, platformID);
    }
</script>

<script>
    $(document).ready(function () {
        var opt = {
            dayNames: ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"],
            dayNamesMin: ["日", "一", "二", "三", "四", "五", "六"],
            monthNames: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
            monthNamesShort: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
            prevText: "上月",
            nextText: "次月",
            weekHeader: "週",
            showMonthAfterYear: true,
            dateFormat: "yy-mm-dd",

            //changeMonth : true,
            changeYear: true,

            //限制顯示年份
            //yearRange: "+0:+5",

            //minDate: new Date(),

        };

        $("#startDate , #endDate").datepicker(opt);

    });
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
					<h1 class="clearfix ma_10_0"><span class="fn_20 flo_l fn_black fn_bold">歷史資料</span></h1>
					<div class="hr bc_blue3 ma_btm_20"></div>
					<div class="container view_content">
                        <form id="search-form" method="post" action="HistoryInfo.aspx">
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12"></div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<span class="fn_red">*依選取廠區，列出區域、供料站、成型機</span>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>廠區</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<select id="factoryID" name="factoryID" onchange="getAreaOption(this.value)"></select>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>區域</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<select id="areaID" name="areaID" onchange="getPlatformOption(this.value)"></select>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>供料台</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<select id="platformID" name="platformID" onchange="getMachineOption(this.value)"></select>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1"><span class="fn_red">*</span>成型機</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<select id="machineID" name="machineID"></select>
								</div>
							</div>
                            <div class="row ma_btm_20 date">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1">工單起訖日期</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<input type="text" id="startDate" name="startDate" readonly="readonly" value="">
									<span class="wave">～</span>
									<input type="text" id="endDate" name="endDate" readonly="readonly" value="">
									<br>
									<span class="fn_red">若為空白代表全部</span>
								</div>
							</div>
							<div class="row ma_btm_20">
								<div class="col-md-3 col-sm-3 col-xs-12">
									<p class="title_lvl1">工單號碼</p>
								</div>
								<div class="col-md-9 col-sm-9 col-xs-12">
									<input type="text" id="orderID" name="orderID" value="">
                                    <input type="hidden" id="pg" name="pg" value="1">
									<span class="fn_red">若為空白代表全部</span>
								</div>
							</div>
							<div class="row ">
								<div class="col-md-6 col-sm-6 col-xs-12 ma_btm_20">
									<input type="reset" value="清空" class="btn input_style2">
								</div>
								<div class="col-md-6 col-sm-6 col-xs-12 ma_btm_20">
									<input Type="submit" value="送出" class="btn input_style1">
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
