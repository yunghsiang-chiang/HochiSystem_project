<%@ Page Title="" Language="C#" MasterPageFile="~/HochiSCMaster.master" AutoEventWireup="true" CodeFile="HSCGRecord.aspx.cs" Inherits="HSCGRecord" ValidateRequest="False" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="css/HSCGRecord.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.ckeditor.com/ckeditor5/46.1.1/ckeditor5.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="HF_EIPUid" runat="server" />
    <div class="middlepart growthrecord">

        <p class="mb-2">首頁 > 成長紀錄</p>
        <div class="d-flex justify-content-between align-items-end flex-wrap">
            <h4 class="font-weight-bold">成長紀錄</h4>

            <div>
                <asp:LinkButton ID="LBtn_AddSCGRecord" runat="server" CssClass="btn btn-purple" OnClick="LBtn_AddSCGRecord_Click" data-toggle="modal" data-target="#ContentModal" href="javascript:void(0)">新增成長紀錄</asp:LinkButton>
                <asp:LinkButton ID="LBtn_SearchOldData" runat="server" CssClass="btn btn-info" OnClick="LBtn_SearchOldData_Click">查詢過往成長紀錄</asp:LinkButton><%--data-toggle="modal" data-target="#SearchModal"OnClick="LBtn_SearchOldData_Click"--%>

                <asp:LinkButton ID="LBtn_ExportSCGRecord" runat="server" CssClass="btn btn-success" data-toggle="modal" data-target="#Export" href="javascript:void(0)">匯出我的成長紀錄</asp:LinkButton>
            </div>
        </div>

        <hr />
        <!--搜尋列 start-->
        <div class="col-md-12 search-bar growthsearch">
            <div class="row">
                <div class="skeyword col-md-3 col-sm-6 pl-1 pr-1 d-none">
                    <div class="form-group">
                        <asp:TextBox ID="TB_CourseSearch" runat="server" CssClass="form-control" AutoComplete="off" Placeholder="請輸入關鍵字(主題/內容)"></asp:TextBox>
                        <%--OnTextChanged="TB_CourseSearch_TextChanged" AutoPostBack="true"--%>
                    </div>
                </div>
                <div class="sname col-md-2 col-sm-6 pl-1 pr-1">
                    <div class="form-group">
                        <asp:TextBox ID="TB_HUsernameSearch" runat="server" CssClass="form-control" AutoComplete="off" Placeholder="請輸入同修姓名"></asp:TextBox>
                    </div>
                </div>
                <div class="sdate col-md-3 pl-1 pr-1">
                    <div class="form-group">
                        <asp:TextBox ID="TB_DateSearch" runat="server" CssClass="form-control datepicker" AutoComplete="off" Placeholder="請輸入紀錄日期"></asp:TextBox>
                    </div>
                </div>
                <div class="sarea col-md-2 pl-1 pr-1">
                    <div class="form-group">
                        <asp:SqlDataSource ID="SDS_SHArea" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HArea FROM HArea"></asp:SqlDataSource>
                        <asp:DropDownList ID="DDL_HAreaSearch" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataValueField="HID" DataTextField="HArea" DataSourceID="SDS_SHArea" AppendDataBoundItems="true">
                            <asp:ListItem Value="0">選擇區屬</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="sarea col-md-2 pl-1 pr-1">
                    <div class="form-group">
                        <asp:SqlDataSource ID="SDS_System" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSystemName FROM HSystem"></asp:SqlDataSource>
                        <asp:DropDownList ID="DDL_HSystemSearch" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataValueField="HID" DataTextField="HSystemName" DataSourceID="SDS_System" AppendDataBoundItems="true">
                            <asp:ListItem Value="0">選擇體系</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="sarea col-md-2 pl-1 pr-1">
                    <div class="form-group">
                        <asp:SqlDataSource ID="SDS_HAxisType" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HAxisType FROM HAxisType"></asp:SqlDataSource>
                        <asp:DropDownList ID="DDL_HAxisTypeSearch" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataValueField="HID" DataTextField="HAxisType" DataSourceID="SDS_HAxisType" AppendDataBoundItems="true">
                            <asp:ListItem Value="0">選擇軸線</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="sbtn col-md-2 pl-1 pr-1 sm-text-right">
                    <div class="form-group">
                        <asp:LinkButton ID="LBtn_Search" runat="server" class="btn btn-outline-secondary ml-1 mr-1" OnClick="LBtn_Search_Click"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                        <asp:LinkButton ID="LBtn_SearchCancel" runat="server" class="btn btn-outline-secondary" OnClick="LBtn_SearchCancel_Click"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                    </div>
                </div>

            </div>
        </div>

        <!--搜尋列  end-->



        <div id="legendContainer" class="d-none d-md-block">
            <div class="col-md-12 sm-area iconexample ">
                <ul class="d-flex list-unstyled align-items-center justify-content-start ul_keyword">
                    <li class="gicon">圖例： </li>

                    <asp:SqlDataSource ID="SDS_HSCCLassIcon" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCClassName, LEFT(HSCClassName,1) AS SCClassFirstWord, HStatus FROM HSCClass WHERE HStatus=1 ORDER BY HID ASC"></asp:SqlDataSource>
                    <asp:Repeater ID="Rpt_HSCCLassIcon" runat="server" DataSourceID="SDS_HSCCLassIcon" OnItemDataBound="Rpt_HSCCLassIcon_ItemDataBound">
                        <ItemTemplate>
                            <li class="mr-2 d-flex justify-content-start align-items-center">
                                <div class="icon" id="Div_Icon" runat="server">
                                    <asp:Label ID="LB_SCClassFirstWord" runat="server" Text='<%# Eval("SCClassFirstWord") %>'></asp:Label>
                                </div>
                                <div class="ml-1 icon_name">
                                    <asp:Label ID="LB_HSCClassName" runat="server" Text='<%# Eval("HSCClassName") %>'></asp:Label>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>


                    <li class="mr-2 d-flex justify-content-start align-items-center scforum">
                        <div style="display: inline-block; width: 40px; background: #f5b247; border-radius: 5px; color: #fff; padding-left: 5px;">
                            專
                        </div>
                        <div class="ml-1" style="width: 77%;">專欄分享</div>
                    </li>
                    <li class="mr-2 d-flex justify-content-start align-items-center scforum">
                        <div style="display: inline-block; width: 40px; background: #f5b247; border-radius: 5px; color: #fff; padding-left: 5px;">
                            問
                        </div>
                        <div class="ml-1" style="width: 77%;">專欄提問</div>
                    </li>
                    <li class="mr-2 d-flex justify-content-start align-items-center scforum">
                        <div style="display: inline-block; width: 40px; background: #f5b247; border-radius: 5px; color: #fff; padding-left: 5px;">
                            回
                        </div>
                        <div class="ml-1" style="width: 77%;">專欄回應</div>
                    </li>
                </ul>
            </div>

            <div class="row align-items-center pl-4" style="font-size: 13px">
                回應：<span class="mr-2"><i class="fa fa-reply"></i>同修</span><span class="mr-2"> <i class="far fa-star"></i>光團母團</span><span class="mr-2"> <i class="fas fa-star-half-alt"></i>區母團</span> <span class="mr-2"><i class="fas fa-star"></i>大區母團</span><span class="mr-2"><i class="fab fa-yelp"> </i>總監/總導護 </span><span class="mr-2"><i class="far fa-sun"></i>大愛光老師 </span>
            </div>
        </div>



        <hr class="mt-1 mb-2" />
        <!--行事曆 start-->

        <!--週數-->
        <div class="d-none">
            <asp:Label ID="LB_WhichWeek" runat="server" Text="0"></asp:Label>
        </div>




        <div class="col-md-12">
            <div class="d-flex align-items-center justify-content-between flex-wrap ul_week">
                <!-- 光團 / 連線 切換（與上一週/下一週同一行） -->
                <asp:UpdatePanel ID="upMode" runat="server">
                    <ContentTemplate>
                        <div class="mb-2">
                            <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" TextAlign="Right"
                                CssClass="rbl-inline mb-0" OnSelectedIndexChanged="RadioButtonList1_SelectedIndexChanged">
                                <asp:ListItem Text="光團" Value="team" Selected="True" />
                                <asp:ListItem Text="連線" Value="network" />
                            </asp:RadioButtonList>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <div class="d-flex list-unstyled align-items-center  ul_week">
                    <!-- 手機版才顯示 -->
                    <div class="d-block d-md-none mb-2">
                        <button class="btn btn-outline-secondary btn-sm" type="button" id="toggleLegendBtn">顯示圖例說明</button>
                    </div>
                    <div>
                        <div class="d-flex justify-content-around align-items-center">
                            <asp:LinkButton ID="LBtn_LastWeek" runat="server" CssClass="btn btn-outline-secondary mr-2">上一週</asp:LinkButton>
                            <asp:LinkButton ID="LBtn_NextWeek" runat="server" CssClass="btn btn-outline-secondary">下一週</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="table-responsive">
            <asp:Table ID="TBL_HSCGRecord" runat="server" CssClass="table table-hover table-striped table-bordered table-scgrecord"></asp:Table>

            <asp:Literal ID="ltITable" runat="server" Visible="false" />
        </div>

        <asp:GridView ID="GridView1" runat="server" Visible="true"></asp:GridView>

        <asp:GridView ID="GridView2" runat="server" Visible="true"></asp:GridView>


    </div>




    <!--==================新增成長紀錄彈跳出的畫面==================--->
    <div id="ContentModal" class="modal fade hochi-modal grecord-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 75%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0" id="exampleModalLongTitle">新增成長紀錄</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="modal-body" style="width: 100%; padding: 10px">
                    <asp:UpdatePanel ID="UP_ContentModal" runat="server">
                        <ContentTemplate>
                            <div class="p-0">
                                <div class="row clearfix pl-2 pr-2">
                                    <div class="col-lg-12 col-md-12">

                                        <div class="form-group">

                                            <div class="col-md-12 mb-2">
                                                <div class="row clearfix">
                                                    <div class="col-md-12 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            <span class="text-danger">*</span> 主題名稱</label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HTopicName" runat="server" class="form-control" placeholder="主題名稱" AutoComplete="off"></asp:TextBox>
                                                        </div>
                                                    </div>


                                                    <div class="col-md-3 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            <span class="text-danger">*</span>專欄分類</label>
                                                        <div class="form-group vertical-align-top">
                                                            <asp:SqlDataSource ID="SDS_HSCClass" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCClassName  FROM HSCClass WHERE HStatus=1"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HSCClass" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCClass" DataTextField="HSCClassName" DataValueField="HID" AppendDataBoundItems="true">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-3 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            紀錄類型</label>
                                                        <div class="form-group vertical-align-top">
                                                            <asp:SqlDataSource ID="SDS_HSCRecordType" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCRTName FROM HSCRecordType WHERE HStatus=1"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HSCRecordType" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCRecordType" DataTextField="HSCRTName" DataValueField="HID" AppendDataBoundItems="true">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-6 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            課程名稱</label>
                                                        <div class="form-group vertical-align-top">
                                                            <asp:SqlDataSource ID="SDS_HCourseName" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <%--SELECT MAX(HID) AS HID, HCourseName , MAX(HDateRange) AS HDateRange FROM HCourse_Merge Cross Apply SPLIT(Replace(HDateRange,' - ',','), ',') b WHERE HStatus=1 AND HVerifyStatus=2 GROUP BY HCourseName , HCBatchNum ORDER BY IIF(DATEPART(YYYY,LEFT(TRY_CONVERT(nvarchar, getdate(), 111),10))=DATEPART(YYYY,TRY_CONVERT(nvarchar, MIN(b.Value),111)), '5', '6') ASC, IIF(DATEPART(Month,LEFT(TRY_CONVERT(nvarchar, getdate(), 111),10))=DATEPART(Month,TRY_CONVERT(nvarchar, MIN(b.Value),111)),'1' ,'2')  ASC--%>
                                                            <asp:DropDownList ID="DDL_HCourseName" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HCourseName" DataTextField="HCourseName" DataValueField="HID" AppendDataBoundItems="true" OnSelectedIndexChanged="DDL_HCourseName_SelectedIndexChanged" AutoPostBack="true">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>



                                                    <div class="col-md-6 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            九宮格類型</label>
                                                        <div class="form-group vertical-align-top">
                                                            <asp:RadioButtonList ID="RBL_HSCJiugonggeTypeID" runat="server" CssClass="radiolist" Style="vertical-align: top" RepeatLayout="Flow" RepeatDirection="Horizontal">
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-6 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            成長進度</label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HSCGProgress" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCGProgressName FROM HSCGProgress WHERE HStatus=1"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HGProgress" runat="server" CssClass="form-control js-example-basic-single" Width="100%" DataValueField="HID" DataTextField="HSCGProgressName" DataSourceID="SDS_HSCGProgress" AppendDataBoundItems="true">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-6 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            其他成長進度</label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HOGProgress" runat="server" class="form-control" placeholder="其他" AutoComplete="off"></asp:TextBox>
                                                        </div>
                                                    </div>


                                                    <div class="col-md-12 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            內容</label>
                                                        <div class="form-group">
                                                            <CKEditor:CKEditorControl ID="CKE_HContent" runat="server" class="form-control"></CKEditor:CKEditorControl>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-12 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            上傳檔案
                                                <span class="text-danger font-weight-normal font-13">*格式僅限jpg、jpeg、heic、heif、png、gif、mp3、pdf、docx、doc、xlsx、xls</span>
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:FileUpload ID="FU_HFile1" runat="server" class="mb-1" /><asp:LinkButton ID="LBtn_HFile1" runat="server" CssClass="btn btn-outline-gray" OnClick="LBtn_HFile1_Click">上傳</asp:LinkButton>
                                                            <asp:Label ID="LB_HFileMsg1" runat="server" Text="" CssClass="text-danger"></asp:Label><!--提示訊息-->
                                                            <asp:LinkButton ID="LBtn_HFile1_Del" runat="server" ToolTip="移除已經上傳的檔案" Visible="false" OnClick="LBtn_HFile1_Del_Click"><i class="fa fa-times-circle" style="color:red" >刪除</i></asp:LinkButton>
                                                            <div class="d-none">
                                                                <asp:Label ID="LB_HFile1" runat="server" Text="" Visible="true" CssClass="text-muted"></asp:Label><!--上傳檔案名稱-->
                                                            </div>
                                                            <br />
                                                            <asp:FileUpload ID="FU_HFile2" runat="server" class="mb-1" /><asp:LinkButton ID="LBtn_HFile2" runat="server" CssClass="btn btn-outline-gray" OnClick="LBtn_HFile2_Click">上傳</asp:LinkButton>
                                                            <asp:Label ID="LB_HFileMsg2" runat="server" Text="" CssClass="text-danger"></asp:Label><!--提示訊息-->
                                                            <asp:LinkButton ID="LBtn_HFile2_Del" runat="server" ToolTip="移除已經上傳的檔案" Visible="false" OnClick="LBtn_HFile2_Del_Click"><i class="fa fa-times-circle" style="color:red" >刪除</i></asp:LinkButton>
                                                            <asp:Label ID="LB_HFile2" runat="server" Text="" Visible="false" CssClass="text-muted"></asp:Label><!--上傳檔案名稱-->
                                                            <br />
                                                            <asp:FileUpload ID="FU_HFile3" runat="server" class="mb-1" /><asp:LinkButton ID="LBtn_HFile3" runat="server" CssClass="btn btn-outline-gray" OnClick="LBtn_HFile3_Click">上傳</asp:LinkButton>
                                                            <asp:Label ID="LB_HFileMsg3" runat="server" Text="" CssClass="text-danger"></asp:Label><!--提示訊息-->
                                                            <asp:LinkButton ID="LBtn_HFile3_Del" runat="server" ToolTip="移除已經上傳的檔案" Visible="false" OnClick="LBtn_HFile3_Del_Click"><i class="fa fa-times-circle" style="color:red" >刪除</i></asp:LinkButton>
                                                            <asp:Label ID="LB_HFile3" runat="server" Text="" Visible="false" CssClass="text-muted"></asp:Label><!--上傳檔案名稱-->
                                                        </div>




                                                    </div>

                                                    <div class="col-md-12 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            影片嵌入連結
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HVideoLink" runat="server" class="form-control" placeholder=" 影片連結" AutoComplete="off"></asp:TextBox>
                                                        </div>
                                                    </div>


                                                    <div class="col-md-12 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            同步發布於大愛光老師專欄
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HSCTopic" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HTopicName FROM HSCTopic WHERE HStatus=1 AND HCreateDT >= DATEADD(DAY, 1 - DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE)) AND HCreateDT < DATEADD(DAY, 1, CAST(GETDATE() AS DATE)) ORDER BY HCreateDT DESC"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HSCTopic" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataValueField="HID" DataTextField="HTopicName" DataSourceID="SDS_HSCTopic" AppendDataBoundItems="true">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>



                                                    <div class="col-md-12 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            HashTag標籤
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HHashTag" runat="server" class="form-control" placeholder="HashTag標籤(請以逗號隔開)" AutoComplete="off"></asp:TextBox>
                                                        </div>
                                                    </div>



                                                    <div class="col-md-12 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            開放對象</label>
                                                        <div class="form-group vertical-align-top">
                                                            <asp:RadioButtonList ID="RBL_HOpenObject" runat="server" Style="vertical-align: top;" RepeatLayout="Flow" RepeatDirection="Horizontal">
                                                                <asp:ListItem Value="1" class="item_margin" Selected>&nbsp;全體同修&nbsp;</asp:ListItem>
                                                                <asp:ListItem Value="2" class="item_margin" Enabled="false">&nbsp;僅班會成員&nbsp;</asp:ListItem>
                                                                <asp:ListItem Value="3" class="item_margin">&nbsp;僅導師及帶領導師&nbsp;</asp:ListItem>
                                                                <asp:ListItem Value="4" class="item_margin">&nbsp;僅自己&nbsp;</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-12 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            是否通知導師及帶領導師</label>
                                                        <div class="form-group vertical-align-top">
                                                            <asp:CheckBox ID="CB_HNotifyMentor" runat="server" Text="是" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row clearfix">
                                                </div>
                                            </div>

                                        </div>

                                    </div>


                                </div>

                            </div>


                            <div class="modal-footer text-center">
                                <asp:Button ID="Btn_Submit" runat="server" class="button button-green" Text="儲存" UseSubmitBehavior="false" OnClick="Btn_Submit_Click" />
                                <button class="button button-gray text-white" data-dismiss="modal">關閉</button>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="DDL_HCourseName" EventName="SelectedIndexChanged" />
                            <asp:PostBackTrigger ControlID="LBtn_HFile1" />
                            <asp:PostBackTrigger ControlID="LBtn_HFile1_Del" />
                            <asp:PostBackTrigger ControlID="LBtn_HFile2" />
                            <asp:PostBackTrigger ControlID="LBtn_HFile2_Del" />
                            <asp:PostBackTrigger ControlID="LBtn_HFile3" />
                            <asp:PostBackTrigger ControlID="LBtn_HFile3_Del" />
                            <asp:PostBackTrigger ControlID="Btn_Submit" />
                            <asp:AsyncPostBackTrigger ControlID="RadioButtonList1" EventName="SelectedIndexChanged" />
                            <%--<asp:PostBackTrigger ControlID="Btn_CloseModal" />--%>
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal  END-->


    <!--==================查詢成長紀錄彈跳出的畫面==================--->
    <div id="SearchModal" class="modal fade hochi-modal grecord-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 40%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0">查詢過往成長紀錄</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="modal-body" style="width: 100%; padding: 10px">

                    <div class="p-0">
                        <div class="row clearfix pl-2 pr-2">
                            <div class="col-lg-12 col-md-12">

                                <div class="form-group">

                                    <div class="col-md-12 mb-2">
                                        <div class="row clearfix">

                                            <div class="col-md-6 col-sm-12">
                                                <label class="font-weight-bold">
                                                    紀錄日期</label>
                                                <div class="form-group">
                                                    <asp:TextBox ID="TB_RDate" runat="server" class="form-control datepicker" placeholder="請選擇紀錄日期" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>


                                        </div>
                                        <div class="row clearfix">
                                            <div class="col-md-6 col-sm-12">
                                                <label class="font-weight-bold">
                                                    姓名</label>
                                                <div class="form-group">
                                                    <asp:SqlDataSource ID="SDS_HUsername" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT (HArea+'/'+HPeriod+' '+HUserName) AS Name, HEIPUid FROM MemberList WHERE HStatus=1"></asp:SqlDataSource>
                                                    <asp:DropDownList ID="DDL_HUsername" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataValueField="HEIPUid" DataTextField="Name" DataSourceID="SDS_HUsername" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">選擇姓名</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                </div>

                            </div>


                        </div>

                    </div>


                    <div class="modal-footer text-center">
                        <asp:Button ID="Btn_SearchOldRecord" runat="server" class="button button-green" Text="搜尋" OnClick="Btn_SearchOldRecord_Click" />
                        <a class="button button-gray text-white" data-dismiss="modal">關閉</a>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal  END-->




    <!--==================匯出成長紀錄彈跳出的畫面==================--->
    <div id="Export" class="modal fade hochi-modal grecord-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 40%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0">匯出我的成長紀錄</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="modal-body" style="width: 100%; padding: 10px">

                    <div class="p-0">
                        <div class="row clearfix pl-2 pr-2">
                            <div class="col-lg-12 col-md-12">
                                <p class="text-danger mb-1">* 僅可搜尋半年前，一次最長1個月的紀錄</p>
                                <div class="form-group">

                                    <div class="col-md-12 mb-2">
                                        <div class="row clearfix">

                                            <div class="col-md-6 col-sm-12">
                                                <label class="font-weight-bold">
                                                    紀錄日期區間</label>
                                                <div class="form-group">
                                                    <asp:TextBox ID="TB_DateRange" runat="server" class="form-control daterange" placeholder="請選擇紀錄日期區間" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>


                                        </div>
                                        <div class="row clearfix">
                                        </div>
                                    </div>

                                </div>

                            </div>


                        </div>

                    </div>


                    <div class="modal-footer text-center">
                        <asp:Button ID="Btn_Export" runat="server" class="button button-green" Text="匯出" OnClick="Btn_Export_Click" />
                        <button class="button button-gray text-white" data-dismiss="modal">關閉</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal  END-->



    <!--==================顯示紀錄內容==================--->
    <div id="Div_SCRecord" class="modal fade hochi-modal recordcontent" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 80%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                </div>

                <div class="modal-body" style="width: 100%; padding: 0 20px">
                </div>

                <div class="modal-footer text-center border-top-0">
                    <button class="button button-gray" data-dismiss="modal">關閉</button>
                    <%--<asp:Button ID="Btn_SCRecord_Close" runat="server" class="button button-gray" Text="關閉" />--%>
                </div>
            </div>
        </div>
    </div>




    <!--顯示報名資訊與出席資訊-->
    <div id="OrderInfo" class="modal fade hochi-modal grecord-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 60%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0"><span id="studentInfo" class="text-white"></span>&nbsp;&nbsp;課程報名與出席紀錄</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="modal-body" id="modal-body-content" style="width: 100%; padding: 10px">

                    <div class="p-0">
                        <div class="justify-content-center table-responsive">
                            <table class="table table-bordered table-striped booking-table">
                                <thead>
                                    <tr>
                                        <th>課程名稱</th>
                                        <th class="text-center">應出席次數</th>
                                        <th class="text-center">實際出席次數</th>
                                        <th class="text-center">請假次數</th>
                                        <th class="text-center">遲到次數</th>
                                        <th class="text-center">缺席次數</th>
                                    </tr>
                                </thead>
                                <tbody>

                                    <!--從後端產生-->

                                </tbody>
                            </table>
                        </div>

                    </div>


                    <div class="modal-footer text-center p-1">
                        <button type="button" class="button button-gray" data-dismiss="modal" aria-label="Close">
                            關閉
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>




    <!-- 顯示EIP成長記錄 -->
    <div class="modal inmodal" id="myModal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content animated bounceInRight">
                <div class="modal-header">
                </div>
                <div class="modal-body" style="background-color: #FFFFFF;">
                </div>

            </div>
        </div>
    </div>





    <script src="js/jquery-3.4.1.min.js"></script>
    <script src="js/moment.min.js"></script>




    <!-- 摺疊圖標 -->
    <script>
        $(function () {

            $('.js-example-basic-single').select2({
            });

            $(".panel-heading").click(function (e) {
                $(this).find("span.fa-chevron-down").toggleClass("fa-chevron-up");
                $(this).find("span.fa-chevron-up").toggleClass("fa-chevron-down");
            });

            $(".solutionList li").click(function () {
                $(".solutionList .linkactive").removeClass('linkactive');
                $(this).addClass('linkactive');
            });


            $(function () {
                $(".daterange").daterangepicker({
                    opens: 'right',
                    minDate: moment().subtract(6, 'months').startOf('day'), // 最近半年
                    maxDate: moment().endOf('day'),                         // 今天
                    autoUpdateInput: false,
                    locale: {
                        cancelLabel: 'Clear',
                        format: 'YYYY/MM/DD'
                    }
                }, function (start, end, label) {
                    // 檢查選取的區間是否超過 1 個月（31 天）
                    var maxRangeDays = 31;
                    if (end.diff(start, 'days') > maxRangeDays) {
                        alert("搜尋區間最多只能選擇 1 個月（最多 31 天）");
                        $(this.element).val(''); // 清空選擇
                    } else {
                        // 若通過檢查，才套用日期文字
                        $(this.element).val(start.format('YYYY/MM/DD') + ' - ' + end.format('YYYY/MM/DD'));
                    }
                });

                $(".daterange").on('apply.daterangepicker', function (ev, picker) {
                    $(this).val(picker.startDate.format('YYYY/MM/DD') + ' - ' + picker.endDate.format('YYYY/MM/DD'));
                });

                $(".daterange").on('cancel.daterangepicker', function (ev, picker) {
                    $(this).val('');
                });
            });

        });


        //AA20240826_呼叫資料顯示modal內容
        var strRecTitle = "";
        var buttop, butleft;
        var strCacheType, strCacheDate, strCacheTitle, strCacheHID;

        //新版專欄
        function ShowModalContent(strType, strDate, strMemberID, strTitle) {
            var infoModal = $('#Div_SCRecord');

            $.ajax({
                url: 'HSCGRecordData.aspx',
                cache: false,
                datatype: 'html',
                method: 'post',
                data: {
                    memberid: strMemberID,
                    date: strDate,
                    type: strType
                },
                success: function (data) {
                    strCacheDate = strDate;
                    strCacheTitle = strTitle;
                    infoModal.find('.modal-header').html("<button type='button' class='close' data-dismiss='modal'><span aria-hidden='true'>&times;</span><span class='sr-only'>Close</span></button>" + strTitle + "<h6>" + strDate.substring(0, 4) + "." + strDate.substring(5, 7) + "." + strDate.substring(8, 10) + " 記錄</h6><hr/>");

                    infoModal.find('.modal-body').html(data);

                    infoModal.modal('show');
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("無法顯示資料！");
                }
            });

        };

        //EIP資料
        function ShowModalEIPContent(strType, strDate, strId, strTitle) {
            var infoModal = $('#Div_SCRecord');

            $.ajax({
                url: 'HEIPRecordData.aspx',
                cache: false,
                dataType: 'html',
                method: 'post',
                data: {
                    uid: strId,
                    date: strDate,
                    type: strType
                },
                success: function (data) {

                    strCacheDate = strDate;
                    strCacheTitle = strTitle;
                    if (strTitle != "") {
                        infoModal.find('.modal-header').html("<button type='button' class='close' data-dismiss='modal'><span aria-hidden='true'>&times;</span><span class='sr-only'>Close</span></button>" + strTitle + "<p style='font-size:1rem'>" + strDate + "記錄</p>");
                        strRecTitle = strTitle;

                        infoModal.find('.modal-body').html(data);
                        infoModal.modal('show');
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert("無法顯示資料！");
                    // xhr 物件包含 HTTP 狀態與回應文字
                    // ajaxOptions 通常是傳送時的選項字串
                    // thrownError 是瀏覽器丟出的錯誤訊息（例如 "Not Found"）

                    alert("狀態碼：" + xhr.status +
                        "\n狀態文字：" + xhr.statusText +
                        "\n錯誤訊息：" + thrownError +
                        "\n伺服器回應：" + xhr.responseText);
                }

            });

        };


        //出席紀錄
        function ShowOrderModalByID(hid, username) {

            var whichWeek = $("#<%= LB_WhichWeek.ClientID %>").text();  // 取得當前顯示的週數

            //取得點擊的學員姓名
            $('#studentInfo').text(username);


            // 顯示 loading（你可以加個 spinner）
            $("#modal-body-content").html("載入中，請稍候~! ");

            // 呼叫後端取得 HTML
            $.ajax({
                type: "POST",
                url: "HBookingInfo.ashx",
                data: {
                    hid: hid,  //學員id
                    whichWeek: whichWeek  //週數
                },
                success: function (result) {
                    $("#modal-body-content").html(result);
                    $("#OrderInfo").modal("show");
                },
                error: function () {
                    $("#modal-body-content").html("讀取失敗，請稍後再試");
                }
            });
        }


        $(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });

        //回應成長紀錄&儲存

        var strCacheCategory, strCacheId;
        //回應
        function func_reply(strCategory, strId) {
            var infoModal = $('#myModal');
            strCacheCategory = strCategory;
            strCacheId = strId;
            $.ajax({
                url: 'HEIPRecordReply.aspx',
                cache: false,
                dataType: 'html',
                method: 'post',
                data: {
                    category: strCategory,
                    id: strId,  //EIP成長紀錄的篇數
                    type: 'replyrecord'
                },
                success: function (data) {
                    infoModal.find('.modal-header').html(strCacheTitle + "<hr/>");
                    infoModal.find('.modal-body').html(data);
                    $('#Div_SCRecord').modal('hide');
                    var config = {};
                    config.toolbar = [
                        { name: 'clipboard', groups: ['clipboard', 'undo'], items: ['-'] },
                        { name: 'basicstyles', groups: ['basicstyles', 'cleanup'], items: ['Bold', 'Italic', 'Underline'] },
                        { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'], items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'TextColor', 'BGColor'] },
                        { name: 'links', items: ['Link', 'Unlink'] },
                        { name: 'styles', items: ['Font', 'FontSize'] }
                    ];
                    config.font_names = "標楷體;新細明體;微軟正黑體;Arial;Times New Roman;Verdana;";
                    config.fontSize_sizes = "14/14px;16/16px;18/18px;20/20px;22/22px;24/24px;26/26px;28/28px;";
                    editor = CKEDITOR.appendTo('editor', config, data);
                    editor.setData($('#content').html());
                    infoModal.modal("show");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("無法顯示資料！");
                }
            });



        }

        //// 建議：關閉 modal 時順手銷毀 editor
        //$('#myModal').on('hidden.bs.modal', function () {
        //    if (CKEDITOR.instances.editor) CKEDITOR.instances.editor.destroy(true);
        //});
        function CKupdate() {
            for (instance in CKEDITOR.instances)
                CKEDITOR.instances[instance].updateElement();
        }


        //儲存
        function func_savereply() {
            CKupdate();
            var currentInstance;
            for (instance in CKEDITOR.instances)
                currentInstance = instance;

            var content = encodeURIComponent(CKEDITOR.instances[currentInstance].getData());
            strCacheHID = strCacheId;
            $.ajax({
                url: 'HEIPRecordReply.aspx',
                datatype: 'html',
                method: 'post',
                data: {
                    category: strCacheCategory,
                    id: strCacheId,  //EIP成長紀錄的第幾篇
                    type: 'savereply',
                    reply: content
                },
                success: function (data) {
                    //console.log("data=" + data);
                    //console.log("data=" + strCacheHID);
                    //console.log("data=" + strCacheDate);
                    //console.log("data=" + strCacheTitle);
                    alert("儲存成功!");
                    $('#myModal').modal('hide');

                    //ShowModalEIPContent("4", strCacheDate, strCacheHID, strCacheTitle);

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("儲存失敗！");
                }
            });
        }



        /*---------------------------------------------*/
        var strCacheCategory, strCacheId;
        //回應
        function func_EDUReply(strCategory, strId) {
            var infoModal = $('#myModal');
            strCacheCategory = strCategory;
            strCacheId = strId;
            $.ajax({
                url: 'HEDURecordReply.aspx',
                cache: false,
                dataType: 'html',
                method: 'post',
                data: {
                    category: strCategory,
                    id: strId,  //EIP成長紀錄的篇數
                    type: 'replyrecord'
                },
                success: function (data) {
                    infoModal.find('.modal-header').html(strCacheTitle + "<hr/>");
                    infoModal.find('.modal-body').html(data);
                    $('#Div_SCRecord').modal('hide');
                    var config = {};
                    config.toolbar = [
                        { name: 'clipboard', groups: ['clipboard', 'undo'], items: ['-'] },
                        { name: 'basicstyles', groups: ['basicstyles', 'cleanup'], items: ['Bold', 'Italic', 'Underline'] },
                        { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'], items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'TextColor', 'BGColor'] },
                        { name: 'links', items: ['Link', 'Unlink'] },
                        { name: 'styles', items: ['Font', 'FontSize'] }
                    ];
                    config.font_names = "標楷體;新細明體;微軟正黑體;Arial;Times New Roman;Verdana;";
                    config.fontSize_sizes = "14/14px;16/16px;18/18px;20/20px;22/22px;24/24px;26/26px;28/28px;";
                    editor = CKEDITOR.appendTo('editor', config, data);
                    editor.setData($('#content').html());
                    infoModal.modal("show");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("無法顯示資料！");
                }
            });



        }

        //// 建議：關閉 modal 時順手銷毀 editor
        //$('#myModal').on('hidden.bs.modal', function () {
        //    if (CKEDITOR.instances.editor) CKEDITOR.instances.editor.destroy(true);
        //});
        function CKupdate() {
            for (instance in CKEDITOR.instances)
                CKEDITOR.instances[instance].updateElement();
        }


        //儲存
        function func_EDUsavereply() {
            CKupdate();
            var currentInstance;
            for (instance in CKEDITOR.instances)
                currentInstance = instance;

            var content = encodeURIComponent(CKEDITOR.instances[currentInstance].getData());
            strCacheHID = strCacheId;
            console.log("strCacheId=" + strCacheId + "//strCacheHID=" + strCacheHID);
            $.ajax({
                url: 'HEDURecordReply.aspx',
                datatype: 'html',
                method: 'post',
                data: {
                    category: strCacheCategory,
                    id: strCacheId,  //EDU成長紀錄的第幾篇
                    type: 'savereply',
                    reply: content
                },
                success: function (data) {
                    //console.log("data=" + data);
                    //console.log("data=" + strCacheHID);
                    //console.log("data=" + strCacheDate);
                    //console.log("data=" + strCacheTitle);
                    alert("儲存成功!");
                    $('#myModal').modal('hide');

                    //ShowModalEIPContent("4", strCacheDate, strCacheHID, strCacheTitle);

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("儲存失敗！");
                }
            });
        }

    </script>


    <link rel="stylesheet" href="css/datepicker.css" />
    <script src="js/bootstrap-datepicker.js"></script>
    <script>

        $('#toggleLegendBtn').click(function () {
            const $legend = $('#legendContainer');
            $legend.toggleClass('d-none');

            if ($legend.hasClass('d-none')) {
                $(this).text('顯示圖例說明');
            } else {
                $(this).text('隱藏圖例說明');
            }

            //$('#legendContainer').toggleClass('d-none');
        });




        $('.datepicker').datepicker({
            format: 'yyyy/mm/dd',
            autoclose: true,
            toggleActive: false,
            todayHighlight: true,
            orientation: 'bottom auto',
            container: 'body',
            beforeShow: function (input, inst) {
                // 強制放在 input 下方
                setTimeout(function () {
                    $(inst.dpDiv).css({
                        top: $(input).offset().top + $(input).outerHeight(),
                        left: $(input).offset().left
                    });
                }, 0);
            }
        });



    </script>


    <script>
        //查詢過去成長紀錄
        function searchOldData() {
            var uid = document.getElementById('<%= HF_EIPUid.ClientID %>').value; //Uid
            var range = document.getElementById('<%= TB_DateRange.ClientID %>').value; // 日期

            // PageMethods.方法名(參數..., 成功callback, 失敗callback)
            PageMethods.GetOldGrowthRecords(uid, range,
                function (result) {
                    $('#SearchModal').modal('show');
                },
                function (err) {
                    alert('查詢失敗：' + (err.get_message ? err.get_message() : err));
                }
            );

            return false; // **阻止原本的 PostBack**
        }
    </script>



    <script src="https://cdn.jsdelivr.net/npm/heic2any/dist/heic2any.min.js"></script>
    <script>
        document.getElementById('<%= LBtn_HFile1.ClientID %>').addEventListener("click", async function (e) {
            e.preventDefault(); // 阻止 LinkButton 的 postback
            const input = document.getElementById('<%= FU_HFile1.ClientID %>');
            if (!input.files.length) {
                alert("請先選擇檔案");
                return;
            }

            let file = input.files[0];
            let uploadFile = file;
            let uploadName = file.name;

            // 判斷是否為 HEIC/HEIF
            if (/\.(heic|heif)$/i.test(file.name) || file.type === "image/heic" || file.type === "image/heif") {
                try {
                    const jpgBlob = await heic2any({
                        blob: file,
                        toType: "image/jpeg",
                        quality: 0.9
                    });

                    const baseName = file.name.replace(/\.[^.]+$/, "");
                    uploadName = baseName + ".jpg";
                    uploadFile = new File([jpgBlob], uploadName, { type: "image/jpeg" });

                    console.log("轉檔完成，準備上傳:", uploadName);
                } catch (err) {
                    console.error("轉檔失敗", err);
                    //alert("HEIC 轉檔失敗");
                    return;
                }
            }

            // 用 AJAX 上傳
            const formData = new FormData();
            formData.append("file", uploadFile, uploadName);

            fetch("HEICHandler.ashx", {
                method: "POST",
                body: formData
            })
                .then(r => r.json())
                .then(res => {
                    if (res.success) {
                        // 顯示在頁面 Label（LB_File 是伺服器控制項）
                        document.getElementById('<%= LB_HFile1.ClientID %>').innerText = res.fileName;
                        console.log("A=" + document.getElementById('<%= LB_HFile1.ClientID %>').innerText);
                       // alert("上傳成功，檔名: " + document.getElementById('<%= LB_HFile1.ClientID %>').innerText);
                    } else {
                        //alert("上傳失敗: " + res.message);
                    }
                })
                .catch(err => {
                    console.error(err);
                    //alert("上傳錯誤");
                });
        });


    </script>





</asp:Content>

