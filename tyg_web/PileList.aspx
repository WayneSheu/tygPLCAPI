<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PileList.aspx.cs" Inherits="PileList" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>PMI - 料號查詢</title>
<link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
<script src="js/bootstrap.min.js" type="text/javascript"></script>
<script src="js/tyg.js" type="text/javascript"></script>
<!--通用-->
<link href="css/general.css" rel="stylesheet" type="text/css" />
<link href="css/luo_default.css" rel="stylesheet" type="text/css" />
<link href="css/all.css" rel="stylesheet" type="text/css">
<!--私用-->
<link href="css/mat_station_m.css" rel="stylesheet" type="text/css" />

<script type="text/javascript">
	function soap(){
        tyg.setHead("<% Response.Write(Session["user"]); %>", true);
		pile.getList();
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
					<h1 class="clearfix ma_10_0"><span class="fn_20 flo_l fn_black fn_bold main-title">料號查詢</span>
                        <a href="PileSet.aspx" class="btn btn-default btn-lg flo_r">回上一頁</a>
					</h1>
					<div class="hr bc_blue3 ma_btm_20"></div>
					
					<!--表格-->
					<div class="table-responsive bor_0">
						<table id="my_table" class="my_table table table-bordered bc_gray3">
							<thead>
								<tr>
									<th>供料台</th>
									<th>料堆</th>
									<th width="20%">料種1</th>
									<th width="20%">料種2</th>
									<th width="20%">料種3</th>
                                    <th>混料比例</th>
								</tr>
							</thead>
							<tbody>
							</tbody>
						</table>
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
