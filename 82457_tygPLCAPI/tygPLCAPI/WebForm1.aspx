<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="tygPLCAPI.WebForm1" %>

<!DOCTYPE html>
<html lang="zh">
    
    <head>
        <meta charset="UTF-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        
        <title>PLC-權限管理</title>
        
        <link href="css/bootstrap.min.css" rel="stylesheet" type="text/css"/>
        <script src="js/jquery-1.12.0.min.js" type="text/javascript"></script>
        <script src="js/bootstrap.min.js" type="text/javascript"></script>
        
        <!--通用-->
        <link href="css/general.css" rel="stylesheet" type="text/css"/>
        <!--私用-->
        <link href="css/authority_m.css" rel="stylesheet" type="text/css"/>
        
        <link href="css/luo_default.css" rel="stylesheet" type="text/css"/>


	<script>
	    function soap() {
	        var data = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"+
                            "<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">\r\n  "+
                                "<soap12:Body>\r\n    "+
                                "<GetAllMachine xmlns=\"http://tempuri.org/\" />\r\n  " +
                                "</soap12:Body>\r\n"+
                            "</soap12:Envelope>";

	        var xhr = new XMLHttpRequest();
	        xhr.withCredentials = false;
	        xhr.addEventListener("readystatechange", function () {
	            if (this.readyState === 4) {
	                alert(this.responseText);
	                var parser = new DOMParser();
	                var xmlDoc = parser.parseFromString(this.responseText, "text/xml");
	                var json = JSON.parse(xmlDoc.getElementsByTagName("GetGroupResult")[0].innerHTML);
	                for (i = 0; i < json.Data.length; i++) {
	                    document.getElementById("my_table").getElementsByTagName("tbody")[0].getElementsByTagName("tr")[i].getElementsByTagName("td")[1].innerHTML = json.Data[i].GroupName;
	                    document.getElementById("my_table").getElementsByTagName("tbody")[0].getElementsByTagName("tr")[i].getElementsByTagName("td")[0].innerHTML = json.Data[i].Date;
	                }
	            }
	        });
	        
	        xhr.open("POST", "http://60.248.242.15:9999/Service2.asmx", true);
	        xhr.setRequestHeader("content-type", "text/xml");
	        xhr.send(data);

	    };

</script>
    </head>
    
    <body onload = "soap()">
        
        <!--head-->
        <div id="head">
            <div class="container">
                <div class="row ma_10_0">
                    
                    <div class="col-sm-3 col-xs-3">
                        <a class="flo_l" href="menu.html" title=""><img src="image/menubtn.png" alt=""></a>
                    </div>
                    
                    <div class="col-sm-6 col-xs-6 txt_center">
                        <div class="ma_top_10 sys_name"><span class="fn_20">現場自動化資訊系統</span></div>
                        <p class="login-user">系統管理者 蔡珮琪</p>                      
                    </div>
                    
                    <div class="col-sm-3 col-xs-3">
                        <a class="flo_r" href="index.html" title=""><img src="image/outbtn.png" alt=""></a>
                    </div>
                    <div class="col-sm-12 col-xs-12">
                        
                        <div class="hr bc_white ma_10_0"> </div>
                        
                        <h1 class="clearfix"><span class="fn_16 fn_white" >PLC狀態顏色說明：</span></h1>
                        
                        <div class="status_list">
    
                                <span class="bc_red fn_white">停&emsp;&emsp;機</span> 
                                <span class="bc_green fn_black">全自動</span> 
                                <span class="bc_blue fn_black">半自動</span> 
                                <span class="bc_tan fn_black">手&emsp;&emsp;動</span> 
                                <span class="bc_white fn_black">換模中</span> 
                                <span class="bc_gray2 fn_white">未連線</span> 
                                
   
                        </div>
                        
                    </div> 
                 </div>
            </div>
        </div>
        
        <!--content-->
        <div id="content">
            

            
            <div class="container">
                
                <div class="v1">
                    <div class="ma_0_10">

                        <h1 class="clearfix ma_10_0">
                            <span class="fn_20 flo_l fn_black fn_bold" >權限管理</span>
                            <!--<span class="fn_20 fn_yellow flo_r" >Menu</span>-->
                        </h1>
                        
                        <div class="hr bc_blue3 ma_btm_20"> </div>
                        
                        <div class="table_ctrl">
                            
                            <div class="flo_l">
                                <button type="button" class="btn btn-default" onclick="location.href='authority_m_add.html'">新增組別</button>
                            </div>
                            
                        </div>
                        
                        
                        <!--表格-->
                        <div  class="table-responsive bor_0" >
                            
                            <table id="my_table" class="my_table table table-bordered bc_gray3">
                                <thead>
                                    <tr>
                                        <th>建立時間</th>
                                        <th>組別名稱</th>
                                        <th>操作管理</th>
                                    </tr>
                                </thead>
                                
                                <tbody>

                                    <tr>
                                        <td class="td_datatime">2017/03/09 13:00</td>
                                        <td>第一組</td>
                                        <td><button type="button" class="btn btn-primary" onclick="location.href='authority_m_add.html'">設定</button></td>
                                    </tr>
                                    <tr>
                                        <td class="td_datatime">2017/03/09 13:00</td>
                                        <td>第二組</td>
                                        <td><button type="button" class="btn btn-primary" onclick="location.href='authority_m_add.html'">設定</button></td>
                                    </tr> 
                                    <tr>
                                        <td class="td_datatime">2017/03/09 13:00</td>
                                        <td>第三組</td>
                                        <td><button type="button" class="btn btn-primary" onclick="location.href='authority_m_add.html'">設定</button></td>
                                    </tr>
                                    
                                </tbody>
                               
                            </table>
                            
                        </div>
                        <!--導覽列-->
                        <nav aria-label="Page navigation" class="txt_center">
                            <ul class="pagination">
                                <li>
                                    <a href="#" aria-label="Previous">
                                        <span aria-hidden="true">&laquo;</span>
                                    </a>
                                </li>
                                <li><a href="#">1</a></li>
                                <li><a href="#">2</a></li>
                                <li><a href="#">3</a></li>
                                <li><a href="#">4</a></li>
                                <li><a href="#">5</a></li>
                                <li>
                                    <a href="#" aria-label="Next">
                                        <span aria-hidden="true">&raquo;</span>
                                    </a>
                                </li>
                            </ul>
                            <p class="fn_black">｜第1頁/共10頁｜顯示第1-15筆資料/共147筆資料｜</p>
                        </nav>
                        
                        <div class="clr"></div>
                        
                    </div>

                </div>

            </div>
                       
        </div>
     
        <!--foot-->
        <div id="foot" class="txt_center txt_spac_3 ma_10_0">
            © Copyright 2017 東陽事業集團 All Rights Reserved
        </div>


                 
    </body>
    
</html>




