<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GroupMember.aspx.cs" Inherits="GroupSet" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI - 權限組別成員</title>
<link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
<script src="js/bootstrap.min.js" type="text/javascript"></script>
<script src="js/tyg.js" type="text/javascript"></script>
<!--通用-->
<link href="css/general.css" rel="stylesheet" type="text/css" />
<link href="css/luo_default.css" rel="stylesheet" type="text/css" />
<!--私用-->
<link href="css/authority_m_add.css" rel="stylesheet" type="text/css" />
<link href="css/mat_station_m.css" rel="stylesheet" type="text/css" />

<script>
	function soap(){
        tyg.setHead("<% Response.Write(Session["user"]); %>", true);
        group.getPermission("<% Response.Write(Request.QueryString["no"]); %>");
    };

    function gotoSet() {
        var groupNo = "<% Response.Write(Request.QueryString["no"]); %>";
        group.gotoSet(groupNo);
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
					<h1 class="clearfix ma_10_0"><span class="fn_20 flo_l fn_black fn_bold">權限管理</span>
                         <a href="javascript:group.gotoList()" class="btn btn-default btn-lg flo_r">回上一頁</a>
                        <!--<button type="button" class="btn btn-default" onclick="group.gotoList()">返回組別列表</button>-->
					</h1>
					<div class="hr bc_blue3 ma_btm_20"></div>					
					<div class="tab-content">
						<div role="tabpanel" class="tab-pane active" id="member_set">
							<div class="table_ctrl">
								<div class="flo_l">
                                    
									<button type="button" class="btn btn-default" onclick="gotoSet()">新增成員</button>
								</div>
							</div>
							<!--表格-->
							<div class="table-responsive bor_0">
								<table id="my_table" class="my_table table table-bordered bc_gray3">
									<thead>
										<tr>
											<th>加入時間</th>
											<th width="60%">帳號</th>
											<th>操作處理</th>
										</tr>
									</thead>
									<tbody>
									</tbody>
								</table>
							</div>							
							<div class="clr"></div>
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
