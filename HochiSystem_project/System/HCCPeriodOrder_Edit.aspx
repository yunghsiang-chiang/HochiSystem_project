<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HCCPeriodOrder_Edit.aspx.cs" Inherits="System_HCCPeriodOrder_Edit" %>

<%@ Register TagPrefix="Page" TagName="Paging" Src="~/System/Paging_backend.ascx" %>
<%--分頁--%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .table tr th {
            vertical-align: middle;
        }

        .table_CCP th, .table_CCP td {
            padding: 0.5rem 0.2rem;
        }

        .table_CCP th {
            font-size: 0.85rem !important;
        }

        .table_CCP tr td, .table_CCP tr td span {
            font-size: 0.9rem !important;
        }

            .table_CCP tr td, .table_CCP tr td span.badge {
                font-size: 0.85rem !important;
            }

        .card-body {
            padding: 1.25rem 0.5rem;
        }

        .btn-purple {
            padding: 0.375rem 0.5rem !important;
        }

        .list-style li {
            font-size: 0.8rem !important;
        }

        /*定期定額樣式--START*/
        .moreinfo {
            cursor: auto !important;
            background-color: #ebebeb !important;
        }

            .moreinfo label {
                font-size: 1.1rem;
            }

        .form-group label {
            margin-bottom: 0.2rem;
        }

        .list-style {
            list-style: none !important;
            padding-left: 8px !important;
        }

            .list-style li::marker {
                display: none !important;
            }

        .cb_outer label {
            margin-left: 5px;
        }

        .cb_outer input[type=checkbox] {
            margin-top: 4px;
        }

        .rb_outer input[type=radio] {
            margin-top: 4px;
        }

        input[type=checkbox], input[type=radio] {
            vertical-align: top;
        }

        .item_margin input[type=radio] {
            margin-top: 5px !important;
        }

        /*定期定額樣式--END*/

        .btn {
            padding: 0.2rem 0.5rem !important;
        }

        .step-table td {
            padding: 0.3rem 0.35rem;
        }

        .form-control {
            min-height: 30px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!-- ============================================================== -->
    <!-- Page wrapper  -->
    <!-- ============================================================== -->
    <div class="page-wrapper">
        <!-- ============================================================== -->
        <!-- Container fluid  -->
        <!-- ============================================================== -->
        <div class="container">

            <asp:Panel ID="Panel_List" runat="server" Visible="true">
                <!-- ============================================================== -->
                <div class="block-header">
                    <div class="row">
                        <div class="col-lg-3 col-md-12 col-sm-4">
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>信用卡授權訂單</h2>
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item active">信用卡授權訂單</li>
                            </ul>
                        </div>
                    </div>

                </div>
                <!-- ============================================================== -->
                <!-- Start Page Content -->
                <!-- ============================================================== -->
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12">
                        <div class="card">
                            <div class="body">
                                <%--  <asp:UpdatePanel ID="UP_HTMaterial" runat="server" Visible="true" Style="width: 100%;">
                                    <ContentTemplate>--%>
                                <!--暫時隱藏-->
                                <div class="row d-none">
                                    <div class="col-md-12 col-lg-2 col-xlg-2 p-l-0">
                                        <div class="box text-left">
                                            <button id="Btn_Add" runat="server" type="button" class="btn btn-outline-info" onclick="window.location.href='HCCPeriodOrder_Add.aspx';"><i class="fa fa-plus"></i>新增信用卡授權訂單</button>
                                        </div>
                                    </div>
                                </div>
                                <div class="row m-t-10">
                                    <div class="col-md-10 col-lg-12 col-xlg-12">
                                        <div class="box form-group row m-b-0">
                                            <div class="col-md-4 pl-0">
                                                <asp:TextBox ID="TB_HCCPOrderCodeS" runat="server" class="form-control" placeholder="請輸入授權訂單代碼或授權書單號" AutoComplete="off"></asp:TextBox>
                                            </div>
                                            
                                            <div class="col-md-2 pl-0">
                                                <asp:TextBox ID="TB_HTMNameS" runat="server" class="form-control" placeholder="請輸入匯入編號" AutoComplete="off"></asp:TextBox>
                                            </div>

                                            <div class="col-md-2">
                                                <asp:LinkButton ID="LBtn_Search" runat="server" class="btn btn-outline-secondary" OnClick="LBtn_Search_Click"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                                <asp:LinkButton ID="LBtn_SearchCancel" runat="server" class="btn btn-outline-secondary m-l-10" OnClick="LBtn_SearchCancel_Click"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="table-responsive">
                                    <table class="table table-hover m-t-20" style="width: 100%">
                                        <thead>
                                            <tr>
                                                <th class="text-center font-weight-bold" style="width: 5%">執行</th>
                                                <th class="text-center font-weight-bold" style="width: 8%">No</th>
                                                <th class="font-weight-bold" style="width: 22%">授權訂單代碼</th>
                                                <th class="font-weight-bold" style="width: 22%">匯入編號</th>
                                                <th class="text-center font-weight-bold" style="width: 16%;">上傳筆數</th>
                                                <th class="text-center font-weight-bold" style="width: 9%;">成功筆數</th>
                                                <th class="text-center font-weight-bold" style="width: 9%;">失敗筆數</th>
                                                <th class="text-center font-weight-bold" style="width: 9%;">處理中筆數</th>
                                                <th class="text-center font-weight-bold d-none" style="width: 15%">匯出Excel (上傳至綠界用)</th>
                                                <%--<th class="text-center font-weight-bold" style="width: 8%; text-align: center">匯入Excel</th>--%>
                                            </tr>
                                        </thead>

                                        <tbody>


                                            <asp:SqlDataSource ID="SDS_HCCPOrder" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>

                                            <asp:Repeater ID="Rpt_HCCPOrder" runat="server" OnItemDataBound="Rpt_HCCPOrder_ItemDataBound">
                                                <ItemTemplate>
                                                    <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                    <tr>
                                                        <td class="text-center">
                                                            <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success mr-2" ToolTip="編輯" Visible="true" Text="編輯" OnClick="LBtn_Edit_Click"><i class="icon-pencil"></i></asp:LinkButton>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HIndex" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HCCPOrderCode" runat="server" Text='<%# Eval("HCCPOrderCode") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HCCPImportNum" runat="server" Text='<%# Eval("HCCPImportNum") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_TotalNum" runat="server" Text=""></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_Successed" runat="server" Text=""></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_Failed" runat="server" Text=""></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_Processed" runat="server" Text=""></asp:Label>
                                                        </td>
                                                        <td class="text-center d-none">
                                                            <asp:LinkButton ID="LBtn_ExportExcel" runat="server" CssClass="btn btn-success"><span class="ti-export mr-2"></span>匯出Excel</asp:LinkButton>
                                                        </td>

                                                    </tr>

                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>


                                <!--分頁-->
                                <!------------------分頁功能開始------------------>
                                <div class="box text-right">
                                    <Page:Paging runat="server" ID="Pg_Paging" />
                                </div>
                                <!------------------分頁功能結束------------------>
                            </div>
                            <%--      </ContentTemplate>
                                    <Triggers>
                                        <asp:PostBackTrigger ControlID="LBtn_UP_Add" />
                                        <asp:PostBackTrigger ControlID="LBtn_HTMaterial" />
                                    </Triggers>
                                </asp:UpdatePanel>--%>
                        </div>
                    </div>
                </div>
            </asp:Panel>





            <asp:Panel ID="Panel_Edit" runat="server" Visible="false">
                <asp:Label ID="LB_HID" runat="server" Text="" Visible="false"></asp:Label>

                <div class="block-header">
                    <div class="row">
                        <div class="col-lg-3 col-md-12 col-sm-4">
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>信用卡授權訂單</h2>
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item"><a href="HCCPeriodOrder_Edit.aspx">信用卡授權訂單</a></li>
                                <li class="breadcrumb-item active">編輯信用卡授權訂單</li>
                            </ul>
                        </div>
                    </div>
                </div>
                <!-- ============================================================== -->
                <!-- Start Page Content -->
                <!-- ============================================================== -->
                <!-- Row -->
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12">
                        <div class="card">
                            <div class="card-body pt-2">
                                <%--<h4 class="mb-0 mt-2">● 步驟</h4>--%>


                                <p class="font-weight-bold mt-2 mb-1">STEP 1. </p>
                                <p class="font-weight-bold mt-2 mb-1 pl-5">
                                    <asp:LinkButton ID="LBtn_Export" runat="server" CssClass="btn btn-purple  text-white mr-2" OnClick="LBtn_Export_Click"><span class="ti-export mr-2"></span>匯出Excel (.csv檔)</asp:LinkButton>
                                    <%--<span class="font-weight-normal">(綠界/信用卡收單/建立信用卡授權訂單)&nbsp;&nbsp;<a class="font-weight-normal text-info" style="text-decoration: underline" href="https://vendor-stage.ecpay.com.tw/CreditUpload/CreditFile" target="_blank">連結</a></span>--%><!--測試-->
                                    <span class="font-weight-normal">(綠界/信用卡收單/建立信用卡授權訂單)&nbsp;&nbsp;<a class="font-weight-normal text-info" style="text-decoration: underline" href="https://vendor.ecpay.com.tw/CreditUpload/CreditFile" target="_blank">連結</a></span><!--正式-->
                                </p>
                                <p class="mb-1" style="padding-left: 5.2rem;">
                                </p>
                                <p class="font-weight-bold  mt-2 mb-1"></p>
                                <p class="font-weight-bold mt-2 mb-1">
                                    STEP 2.
                                </p>
                                <p class="font-weight-bold mt-2 mb-1 pl-5">
                                    填寫匯入編號 &nbsp;
									<%--<span class="font-weight-normal">(綠界/信用卡收單/建立信用卡授權訂單查詢)&nbsp;&nbsp;<a class="font-weight-normal text-info" style="text-decoration: underline" href="https://vendor-stage.ecpay.com.tw/CreditUpload/ListUploadedCredit" target="_blank">連結</a></span>--%><!--測試-->
                                    <span class="font-weight-normal">(綠界/信用卡收單/建立信用卡授權訂單查詢)&nbsp;&nbsp;<a class="font-weight-normal text-info" style="text-decoration: underline" href="https://vendor.ecpay.com.tw/CreditUpload/ListUploadedCredit" target="_blank">連結</a></span><!--正式-->

                                    <div class="row col-md-12 clearfix mb-1 d-flex d-inline" style="padding-left: 3.6rem;">
                                        <div class="col-md-3 pl-0">
                                            <asp:TextBox ID="TB_HCCPImportNum" runat="server" class="form-control" Style="min-height: 29px!important; padding: 0 0.5rem;" AutoComplete="off" Placeholder="請輸入綠界信用卡授權訂單的匯入編號"></asp:TextBox>
                                        </div>
                                        <div class="col-md-4 pl-0">
                                            <asp:Button ID="Btn_Submit" runat="server" Text="儲存" CssClass="btn btn-success mr-1" OnClick="Btn_Submit_Click" />
                                            <asp:Button ID="Btn_Cancel" runat="server" Text="取消" CssClass="btn btn-inverse" Btmessage="確定要取消嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' OnClick="Btn_Cancel_Click" />

                                        </div>
                                    </div>

                                </p>


                                <p class="font-weight-bold mt-2 mb-1">
                                    STEP 3. 
                                </p>
                                <p class="font-weight-bold mt-2 mb-1 pl-5">
                                    <asp:LinkButton ID="LBtn_Import" runat="server" CssClass="btn btn-info text-white" OnClick="LBtn_Import_Click"><span class="ti-import mr-2"></span>匯入Excel (.csv檔)</asp:LinkButton>&nbsp;&nbsp;<span class="font-weight-normal">(綠界/信用卡收單/定期定額查詢)&nbsp;&nbsp;
                                        <%--<a class="font-weight-normal text-info" style="text-decoration: underline" href="https://vendor-stage.ecpay.com.tw/TradeCreditPeriod/TradeCreditPeriodQuery" target="_blank">連結</a></span>--%><!--測試-->
                                        <a class="font-weight-normal text-info" style="text-decoration: underline" href="https://vendor.ecpay.com.tw/TradeCreditPeriod/TradeCreditPeriodQuery" target="_blank">連結</a></span><!--正式-->
                                </p>

                                <%--		</div>--%>


                                <hr />
                                <h4 class="mb-1 mt-3">● 信用卡授權主檔</h4>
                                <%--<p class="text-info">
										【提醒】匯入編號的資訊於綠界後台登入後，信用卡收單 > 建立信用卡授權訂單查詢
									</p>--%>
                                <div class="row clearfix pl-4">

                                    <div class="col-md-2 col-sm-12">
                                        <label class="control-label text-left col-md-12 text-middle pl-0">授權訂單代碼<span class="text-danger">(由系統產生)</span></label>
                                        <div class="form-group">
                                            <asp:TextBox ID="TB_HCCPOrderCode" runat="server" class="form-control" AutoComplete="off" Enabled="false" Text="P20230804001"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-3 col-sm-12">
                                        <label class="control-label text-left col-md-12 text-middle pl-0">匯入編號</label>
                                        <div class="form-group">
                                            <asp:TextBox ID="TB_FHCCPImportNum" runat="server" class="form-control" AutoComplete="off" Enabled="false" Placeholder="請輸入綠界信用卡授權訂單的匯入編號"></asp:TextBox>
                                        </div>
                                    </div>


                                </div>

                                <!--信用卡授權明細 Start-->

                                <h4 class="mb-0 mt-2">● 信用卡授權明細</h4>

                                <div class="row align-items-end justify-content-between pl-4 pr-4">
                                    <div class="mb-0 text-danger font-weight-bold pl-3 mt-2" style="font-size: 0.89rem;">*代表必填</div>
                                    <div>


                                        <%--Btmessage="功能尚未完成"  OnClientClick='return confirm(this.getAttribute("btmessage"))'--%>
                                    </div>
                                </div>


                                <asp:GridView ID="GridView1" runat="server"></asp:GridView>


                                <div class="row col-md-12 mb-1 mr-0 pl-3">

                                    <div class="table-responsive mt-2">

                                        <table class="table table-hover table-bordered table_CCP" style="width: 100%">
                                            <thead>
                                                <tr>
                                                    <th class="text-center" style="width: 2%;">No</th>
                                                    <th style="width: 6%">捐款人
														姓名</th>
                                                    <th style="width: 8%">捐款項目</th>
                                                    <th style="width: 9%"><span class="text-danger">* </span>刷卡項目</th>
                                                    <th style="width: 12%">綠界廠商訂單編號</th>
                                                    <th class="text-center" style="width: 4%"><span class="text-danger">* </span>分期<br />
                                                        期數</th>
                                                    <th class="text-right" style="width: 5%"><span class="text-danger">* </span>刷卡金額</th>
                                                    <th class="text-left" style="width: 6%">持卡人姓名</th>
                                                    <th class="text-left" style="width: 9%"><span class="text-danger">* </span>信用卡卡號</th>
                                                    <th class="text-center" style="width: 5%"><span class="text-danger">* </span>有效月年</th>
                                                    <th class="text-left" style="width: 5%">信用卡<br />
                                                        背面末3碼
                                                    </th>
                                                    <th class="text-left" style="width: 6%; font-size: 0.85rem"><span class="text-danger">* </span>定期定額<br />
                                                        月扣款頻率</th>
                                                    <th class="text-left" style="width: 5%; font-size: 0.85rem"><span class="text-danger">* </span>定期定額<br />
                                                        扣款期數</th>
                                                    <th class="text-left" style="width: 6%">手機號碼</th>
                                                    <th class="text-center" style="width: 4%">授權狀態</th>
                                                    <th class="text-center" style="width: 3%">授權書</th>
                                                    <th class="text-center" style="width: 3%">部分付款/補登</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:SqlDataSource ID="SDS_HCCPOrderDetail" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_HCCPOrderDetail" runat="server" DataSourceID="SDS_HCCPOrderDetail" OnItemDataBound="Rpt_HCCPOrderDetail_ItemDataBound">
                                                    <ItemTemplate>
                                                        <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                        <tr>
                                                            <td style="text-align: center">
                                                                <asp:Label ID="LB_HIndex" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HDUserName" runat="server" Text='<%# Eval("HDUserName") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HCourseName" runat="server" Text='<%# Eval("HCourseName") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HCCPeriodCode" runat="server" Text='<%# Eval("HCCPeriodCode") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HMerchantTradeNo" runat="server" Text='<%# Eval("HMerchantTradeNo") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_PayInInstallments" runat="server" Text="0"></asp:Label><!--預設帶0-->
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HDCCPAmount" runat="server" Text='<%# Eval("HDCCPAmount") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HCardHolder" runat="server" Text='<%# Eval("HCardHolder") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HCardNum" runat="server" Text='<%# Eval("HCardNum") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_HCardValidDate" runat="server" Text='<%# Eval("HCardValidDate") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_HCVCCode" runat="server" Text='<%# Eval("HCVCCode") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_HMFrequency" runat="server" Text="1"></asp:Label><!--預設帶1-->
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_HDCCPTimes" runat="server" Text='<%# Eval("HDCCPTimes") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HCHPhone" runat="server" Text='<%# Eval("HCHPhone") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_HRtnCode" runat="server" Text='<%# Eval("HRtnCode") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_Review" runat="server" CssClass="btn btn-purple text-white" OnClick="LBtn_Review_Click">查看</asp:LinkButton>
                                                                <!--Btmessage="功能尚未完成" OnClientClick='return confirm(this.getAttribute("btmessage"))'-->
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_Partial" runat="server" CssClass="btn btn-success text-white" OnClick="LBtn_Partial_Click">補登</asp:LinkButton>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>

                                        </table>
                                    </div>
                                </div>
                                <!--信用卡授權明細 End-->

                                <div class="mr-0 text-center">
                                    <asp:Button ID="Btn_Back" runat="server" Text="回上一頁" class="btn btn-secondary" OnClick="Btn_Back_Click" />
                                </div>

                            </div>
                        </div>
                    </div>

                </div>
                <!-- Row -->
                <!-- ============================================================== -->
                <!-- End Page Content -->
                <!-- ============================================================== -->



            </asp:Panel>





        </div>
    </div>


    <!-- Modal 上傳檔案 START-->
    <div class="modal fade" id="Div_UploadFile" tabindex="-1" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" role="document" style="max-width: 50%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h3 class="modal-title font-weight-bold mb-0" id="exampleModalLongTitle">上傳檔案
                    </h3>

                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" style="margin-top: -9px;">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="modal-body order_info pt-2" style="width: 100%; color: #000 !important;">


                    <div class="row clearfix">
                        <div class="col-lg-12 col-md-12">

                            <div class="form-group">
                                <p class="text-danger  font-weight-bold d-none">【提醒】匯入的檔案路徑：登入綠界後台 > 信用卡收單 > 定期定額查詢 > 匯出csv檔</p>
                                <p class="mt-2 mb-1 font-weight-normal">請選擇要匯入的檔案(.csv檔)</p>
                                <!--(請選擇要匯入的檔案)-->
                                <div class="mt-3">
                                    <asp:FileUpload ID="FU_CCPeriodOrder" runat="server" />

                                </div>

                                <%--<asp:GridView ID="GridView1" runat="server"></asp:GridView>--%>

                                <div class="mt-2 modal-footer justify-content-center">
                                    <asp:Button ID="Btn_Import" runat="server" CssClass="btn btn-success" Text="確認匯入" OnClick="Btn_Import_Click" /><!--匯入-->
                                    <a class="btn btn-secondary" href="javascript:void(0);" data-dismiss="modal">取消</a>
                                    <%--<asp:Button ID="Btn_ImportCancel" runat="server" class="btn btn-secondary" OnClick="Btn_ImportCancel_Click" Text="取消"></asp:Button>--%>
                                </div>


                            </div>

                        </div>


                    </div>




                </div>
            </div>
        </div>
    </div>
    <!-- Modal 上傳檔案  END-->


    <!-- Modal 授權書明細 START-->
    <div class="modal fade" id="Div_HCCPeriod" tabindex="-1" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" role="document" style="max-width: 90%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h3 class="modal-title font-weight-bold mb-0" id="exampleModalLongTitle"><%--【--%><asp:Label ID="LB_HTitle" runat="server" Text="" Style="font-size: 24px;"></asp:Label><%--】--%>定期定額授權書申請資訊
                    </h3>

                    <button type="button" class="close d-none" data-dismiss="modal" aria-label="Close" style="margin-top: -9px;">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="modal-body order_info pt-2" style="width: 100%; color: #000 !important;">
                    <div class="row clearfix">
                        <div class="col-lg-12 col-md-12">

                            <div class="form-group">

                                <!----主檔資料---->
                                <div class="col-12 btn btn-secondary text-left moreinfo">
                                    <!--<i class="ti-angle-down"></i>-->
                                    <label class="font-weight-bold mb-0">授權書主檔</label>
                                </div>

                                <div class="col-md-12 mt-2 mb-2">
                                    <div class="row clearfix">
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                授權申請單號</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_MHCCPeriodCode" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                授權書狀態</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_MHStatus" runat="server" Text="Label"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                審核狀態</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_MHVerifyStatus" runat="server" Text="Label"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                授權訂單狀態</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_MHOrderStatus" runat="server" Text="Label"></asp:Label>
                                            </div>
                                        </div>


                                    </div>

                                </div>

                                <!----捐款人基本資料---->
                                <div class="col-12 btn btn-secondary text-left moreinfo">
                                    <!--<i class="ti-angle-down"></i>-->
                                    <label class="font-weight-bold mb-0">捐款人基本資料</label>
                                </div>

                                <div class="col-md-12 mt-2 mb-2">
                                    <div class="row clearfix">
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                <span class="text-danger">*</span>
                                                姓名(收據抬頭)</label>
                                            <div class="form-group">
                                                <asp:TextBox ID="TB_HDUserName" runat="server" class="form-control" placeholder="請填寫" AutoComplete="off" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                <span class="text-danger">*</span>
                                                Email</label>
                                            <div class="form-group">
                                                <asp:TextBox ID="TB_HDEmail" runat="server" class="form-control" placeholder="請填寫" AutoComplete="off" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                <span class="text-danger">*</span>
                                                聯絡電話</label>
                                            <div class="form-group">
                                                <asp:TextBox ID="TB_HDTel" runat="server" class="form-control" placeholder="請填寫" AutoComplete="off" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                <span class="text-danger">*</span>
                                                行動電話</label>
                                            <div class="form-group">
                                                <asp:TextBox ID="TB_HDPhone" runat="server" class="form-control" placeholder="請填寫" AutoComplete="off" MaxLength="10" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                <span class="text-danger">*</span>
                                                出生年月日</label>
                                            <div class="form-group">
                                                <asp:TextBox ID="TB_HDBirth" runat="server" class="form-control datepicker" placeholder="西元年月日" AutoComplete="off" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                <span class="text-danger" runat="server" id="Span_HDPersonID" visible="false">*</span>
                                                身分證字號</label>
                                            <asp:Label ID="LB_NoticePersonID" runat="server" Text="" CssClass="text-danger" Style="font-size: 14px" Visible="false"></asp:Label>
                                            <div class="form-group">
                                                <asp:TextBox ID="TB_HDPersonID" runat="server" class="form-control" placeholder="請填寫" AutoComplete="off" MaxLength="10" Style="width: 100%; padding: 4px;" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%--</div>
														<div class="row clearfix">--%>
                                        <div class="col-md-6 col-sm-12">
                                            <label>
                                                <span class="text-danger">*</span>通訊地址</label>
                                            <div class="form-group d-flex">
                                                <asp:TextBox ID="TB_HDPostal" runat="server" class="form-control mr-2" Style="width: 20%" placeholder="郵遞區號" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>
                                                <asp:TextBox ID="TB_HDAddress" runat="server" class="form-control" Style="width: 80%" placeholder="請填寫通訊地址" AutoComplete="off" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row clearfix d-none">
                                        <div class="col-md-12 col-sm-12">
                                            <label>
                                                <span class="text-danger">*</span>收據寄送</label>
                                            <div class="form-group vertical-align-top">
                                                <asp:RadioButtonList ID="RBL_HDReceiptSType" runat="server" Style="vertical-align: top;" RepeatLayout="Flow" Enabled="false">
                                                    <asp:ListItem class="item_margin" Selected="True" Value="1">&nbsp;&nbsp;年度捐款收據(隔年二月起陸續Email寄發)</asp:ListItem>
                                                    <asp:ListItem class="item_margin" Value="2">&nbsp;&nbsp;捐款紀錄上傳國稅局免寄收據(收據抬頭人之身分證字號為必填)
																			<div style="font-size:0.9rem;color:#464646">
																				<ul class="list-style" style="font-size:0.8rem !important;">
																					<li>1. 請填寫身分證字號，本會將主動上傳捐款紀錄至國稅局，可免附捐款憑證申報「個人綜所稅」。</li>
																						<li>2. 若授權多人以上者，請多作考量後，再行個別單獨填寫此授權書。</li>
																						<li>3. 不適用於企業/團體。</li>
																				</ul>
																			</div>
                                                    </asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>


                                    </div>
                                </div>


                                <!--對象-->

                                <div class="col-12 btn btn-secondary text-left moreinfo">
                                    <!--<i class="ti-angle-down"></i>-->
                                    <label class="font-weight-bold mb-0">祝福對象</label>
                                </div>
                                <div class="col-md-12 mt-2 mb-2">
                                    <div class="row clearfix">
                                        <div class="col-md-12 col-sm-12">
                                            <label>
                                                <span class="text-danger">*</span>
                                                祝福對象(不限人數)</label>
                                            <div class="form-group">
                                                <asp:TextBox ID="TB_HDonor" runat="server" class="form-control" placeholder="請填寫關係:完整姓名" AutoComplete="off" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <!----信用卡捐款資料---->
                                <div class="col-12 btn btn-secondary text-left moreinfo">
                                    <!--<i class="ti-angle-down"></i>-->
                                    <label class="font-weight-bold mb-0">信用卡捐款資料</label>
                                </div>
                                <div class="col-md-12 mt-2 mb-2">
                                    <div class="row clearfix">
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                <span class="text-danger">*</span>
                                                持卡人姓名</label>
                                            <div class="form-group">
                                                <asp:TextBox ID="TB_HCardHolder" runat="server" class="form-control" placeholder="請填寫" AutoComplete="off" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                <%--<span class="text-danger">*</span>--%>持卡人身分證字號</label>
                                            <asp:Label ID="LB_NoticeCHPersonID" runat="server" Text="" CssClass="text-danger" Style="font-size: 14px" Visible="false"></asp:Label>
                                            <div class="form-group">
                                                <asp:TextBox ID="TB_HCHPersonID" runat="server" class="form-control" placeholder="請填寫" AutoComplete="off" MaxLength="10" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12" runat="server" id="Div3" visible="true">
                                            <label><span class="text-danger">*</span>持卡人手機號碼</label>
                                            <div class="form-group d-flex">
                                                <asp:TextBox ID="TB_HCHPhone" runat="server" class="form-control" placeholder="請填寫" AutoComplete="off" MaxLength="10" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                <span class="text-danger">*</span>
                                                發卡銀行</label>
                                            <div class="form-group">
                                                <asp:TextBox ID="TB_HCardBank" runat="server" class="form-control" placeholder="請填寫" AutoComplete="off" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-6 col-sm-12">
                                            <label>
                                                <span class="text-danger">*</span>
                                                信用卡卡號</label>
                                            <div class="form-group d-flex justify-content-around align-items-center ">
                                                <asp:TextBox ID="TB_HCardNum1" runat="server" class="form-control text-center" placeholder="" AutoComplete="off" MaxLength="4" Style="width: 23%" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>-
																	<asp:TextBox ID="TB_HCardNum2" runat="server" class="form-control text-center" placeholder="" AutoComplete="off" MaxLength="4" Style="width: 23%" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>-
																	<asp:TextBox ID="TB_HCardNum3" runat="server" class="form-control text-center" placeholder="" AutoComplete="off" MaxLength="4" Style="width: 23%" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>-
																	<asp:TextBox ID="TB_HCardNum4" runat="server" class="form-control text-center" placeholder="" AutoComplete="off" MaxLength="4" Style="width: 23%" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-6 col-sm-12">
                                            <label><span class="text-danger">*</span>信用卡卡別</label>
                                            <div class="form-group d-none">
                                                <img alt="VISA" src="images/icons/icon_creditcard.jpg" style="width: 30%">
                                            </div>
                                            <div class="form-group">
                                                <asp:RadioButtonList ID="RBL_HCardType" runat="server" CssClass="rb_outer" RepeatDirection="Horizontal" RepeatLayout="Flow" Style="vertical-align: middle" Enabled="false">
                                                    <asp:ListItem Value="1">&nbsp;&nbsp;VISA&nbsp;&nbsp;</asp:ListItem>
                                                    <asp:ListItem Value="2">&nbsp;&nbsp;MASTER&nbsp;&nbsp;</asp:ListItem>
                                                    <asp:ListItem Value="3">&nbsp;&nbsp;JCB&nbsp;&nbsp;</asp:ListItem>
                                                    <asp:ListItem Enabled="false">&nbsp;&nbsp;聯合信用卡&nbsp;&nbsp;</asp:ListItem>
                                                    <asp:ListItem Enabled="false">&nbsp;&nbsp;美國運通卡&nbsp;&nbsp;</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                <span class="text-danger">*</span>信用卡背面末三碼</label>
                                            <div class="form-group">
                                                <asp:TextBox ID="TB_HCVCCode" runat="server" class="form-control autotab" placeholder="請填寫" AutoComplete="off" MaxLength="3" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                <span class="text-danger">*</span>信用卡有效期限</label>
                                            <div class="form-group d-flex justify-content-start">
                                                <asp:TextBox ID="TB_HCardValidDateM" runat="server" class="form-control mr-2 text-center" placeholder="月" AutoComplete="off" MaxLength="2" Style="width: 48%" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>
                                                <span class="mr-2">/</span>
                                                <asp:TextBox ID="TB_HCardValidDateY" runat="server" class="form-control text-center" placeholder="年" AutoComplete="off" MaxLength="2" Style="width: 48%" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row clearfix">
                                        <div class="col-md-3 col-sm-12">
                                            <label><span class="text-danger">*</span>捐款總金額<span style="font-size: 0.85rem">(基本金額：NT 84,000)</span></label>
                                            <div class="form-group d-flex">
                                                <asp:TextBox ID="TB_HDTotal" runat="server" class="form-control text-right" placeholder="" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-md-3 col-sm-12">
                                            <label><span class="text-danger">*</span>定期定額扣款期數</label>
                                            <div class="form-group d-flex">
                                                <asp:TextBox ID="TB_HDCCPTimes" runat="server" class="form-control text-center" placeholder="" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" MaxLength="2" Style="text-align: center" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-md-3 col-sm-12">
                                            <label>每期扣款金額<span style="font-size: 0.85rem">（捐款總金額/扣款期數）</span></label>
                                            <div class="form-group d-flex">
                                                <asp:TextBox ID="TB_HDCCPAmount" runat="server" class="form-control text-righ" Style="text-align: right" placeholder="" AutoComplete="off" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-md-3 col-sm-12">
                                            <label>扣款期間</label>
                                            <div class="form-group d-flex align-items-center">
                                                <asp:TextBox ID="TB_HDCCPSDate" runat="server" class="form-control text-center" placeholder="" AutoComplete="off" Enabled="false"></asp:TextBox>至
                                                 <asp:TextBox ID="TB_HDCCPEDate" runat="server" class="form-control text-center" placeholder="" AutoComplete="off" Enabled="false"></asp:TextBox>止
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row clearfix d-none">
                                        <div class="col-md-12 col-sm-12">
                                            <label class="mb-0">捐贈不公開</label>
                                            <div class="form-group cb_outer">
                                                <asp:CheckBox ID="CB_HDAPublic" runat="server" Text="我不同意將全名公開於捐款芳名錄" Enabled="false" />
                                                <p class="mb-1" style="font-size: 0.9rem;">※本會依財團法人法第25條規定，應公開捐款人姓名及金額，如您不同意公開，請勾選上述選項。</p>
                                            </div>
                                        </div>


                                    </div>
                                </div>

                                <div class="modal-footer justify-content-center">
                                    <a class="btn btn-secondary" href="javascript:void(0);" data-dismiss="modal">關閉</a>
                                </div>


                            </div>

                        </div>


                    </div>




                </div>
            </div>
        </div>
    </div>
    <!-- Modal 訂單明細  END-->


    <!-- Modal 補登作業 START-->
    <asp:Label ID="LB_HCCPOPaidRecordID" runat="server" Text="Label" Visible="false"></asp:Label>
    <div class="modal fade" id="Div_HCCOPaidRecord" tabindex="-1" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" role="document" style="max-width: 90%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h3 class="modal-title font-weight-bold mb-0">補登作業
                    </h3>

                    <button type="button" class="close d-none" data-dismiss="modal" aria-label="Close" style="margin-top: -9px;">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="mt-2 pl-2">
                    <asp:Label ID="LB_RTitle" runat="server" Text="" CssClass="font-weight-bold" Style="font-size: 18px;"></asp:Label>

                </div>

                <div class="modal-body order_info pt-2" style="width: 100%; color: #000 !important;">

                    <!----主檔資料---->
                    <div class="col-12 btn btn-secondary text-left moreinfo">
                        <!--<i class="ti-angle-down"></i>-->
                        <label class="font-weight-bold mb-0">授權書主檔</label>
                    </div>

                    <div class="col-md-12 mt-2 mb-2">
                        <div class="row clearfix">
                            <div class="col-md-3 col-sm-12">
                                <label>
                                    授權申請單號</label>
                                <div class="form-group">
                                    <asp:Label ID="LB_RHCCPeriodCode" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-3 col-sm-12">
                                <label>
                                    授權書狀態</label>
                                <div class="form-group">
                                    <asp:Label ID="LB_RHStatus" runat="server" Text="Label"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-3 col-sm-12">
                                <label>
                                    審核狀態</label>
                                <div class="form-group">
                                    <asp:Label ID="LB_RHVerifyStatus" runat="server" Text="Label"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-3 col-sm-12">
                                <label>
                                    授權訂單狀態</label>
                                <div class="form-group">
                                    <asp:Label ID="LB_RHOrderStatus" runat="server" Text="Label"></asp:Label>
                                </div>
                            </div>


                        </div>

                    </div>


                    <!----捐款人基本資料---->

                    <div class="col-md-12 mt-2 mb-2">
                        <div class="row clearfix">
                            <div class="col-md-3 col-sm-12">
                                <label>
                                    <span class="text-danger">*</span>
                                    姓名(收據抬頭)</label>
                                <div class="form-group">
                                    <asp:TextBox ID="TB_RDUserName" runat="server" class="form-control" placeholder="請填寫" AutoComplete="off" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-3 col-sm-12">
                                <label>
                                    <span class="text-danger">*</span>
                                    Email</label>
                                <div class="form-group">
                                    <asp:TextBox ID="TB_RDEmail" runat="server" class="form-control" placeholder="請填寫" AutoComplete="off" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-3 col-sm-12">
                                <label>
                                    <span class="text-danger">*</span>
                                    聯絡電話</label>
                                <div class="form-group">
                                    <asp:TextBox ID="TB_RDTel" runat="server" class="form-control" placeholder="請填寫" AutoComplete="off" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-3 col-sm-12">
                                <label>
                                    <span class="text-danger">*</span>
                                    行動電話</label>
                                <div class="form-group">
                                    <asp:TextBox ID="TB_RDPhone" runat="server" class="form-control" placeholder="請填寫" AutoComplete="off" MaxLength="10" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>
                                </div>
                            </div>

                        </div>


                    </div>


                    <!----信用卡捐款資料---->
                    <div class="col-md-12 mt-2 mb-2">

                        <div class="row clearfix">
                            <div class="col-md-3 col-sm-12">
                                <label><span class="text-danger">*</span>捐款總金額<span style="font-size: 0.85rem">(基本金額：NT 84,000)</span></label>
                                <div class="form-group d-flex">
                                    <asp:TextBox ID="TB_RDTotal" runat="server" class="form-control text-right" placeholder="" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-3 col-sm-12">
                                <label><span class="text-danger">*</span>定期定額扣款期數</label>
                                <div class="form-group d-flex">
                                    <asp:TextBox ID="TB_RDCCPTimes" runat="server" class="form-control text-center" placeholder="" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" MaxLength="2" Style="text-align: center" Enabled="false"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-3 col-sm-12">
                                <label>每期扣款金額<span style="font-size: 0.85rem">（捐款總金額/扣款期數）</span></label>
                                <div class="form-group d-flex">
                                    <asp:TextBox ID="TB_RDCCPAmount" runat="server" class="form-control text-righ" Style="text-align: right" placeholder="" AutoComplete="off" Enabled="false"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-3 col-sm-12">
                                <label>扣款期間</label>
                                <div class="form-group d-flex align-items-center">
                                    <asp:TextBox ID="TB_RDCCPSDate" runat="server" class="form-control text-center" placeholder="" AutoComplete="off" Enabled="false"></asp:TextBox>至
                 <asp:TextBox ID="TB_RDCCPEDate" runat="server" class="form-control text-center" placeholder="" AutoComplete="off" Enabled="false"></asp:TextBox>止
                                </div>
                            </div>
                        </div>


                    </div>

                    <!----部分付款作業---->
                    <asp:Panel ID="Panel1" runat="server" Visible="false">
                        <div class="col-12 btn btn-secondary text-left moreinfo">
                            <label class="font-weight-bold mb-0">部分付款作業</label>
                        </div>

                        <div class="row p-2">
                            <div class="col-md-4 col-sm-12">
                                <label class="mb-1">選擇是否提前付清或部分付款</label>
                                <div class="form-group d-flex">
                                    <asp:RadioButtonList ID="RadioButtonList1" runat="server" Style="vertical-align: top;" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Value="0" Enabled="false" Selected="True" Style="margin-right: 5px;">不選擇</asp:ListItem>
                                        <asp:ListItem Value="1" Style="margin-right: 5px;">部份付款</asp:ListItem>
                                        <asp:ListItem Value="2">提前付清</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </div>

                            <div id="Div1" runat="server" visible="false" class="text-danger font-weight-bold" style="font-size: 1.2rem">
                                (原授權書單號：<asp:Label ID="Label2" runat="server" Text="0" Style="font-size: 1.2rem" Visible="true"></asp:Label>，
        <asp:Label ID="Label3" runat="server" Text="0" Style="font-size: 1.2rem"></asp:Label>)
                            </div>
                            <asp:Label ID="Label4" runat="server" Text="0" Visible="false"></asp:Label>


                            <div class="col-md-9 col-sm-12">
                                <div class="row clearfix">
                                    <div class="col-md-3 col-sm-12">
                                        <label>部分付款金額</label>
                                        <div class="form-group d-flex">
                                            <asp:Label ID="Label5" runat="server"></asp:Label>
                                        </div>
                                    </div>



                                </div>
                            </div>

                        </div>
                    </asp:Panel>


                    <!--定期定額授權扣款紀錄-->
                    <div class="col-12 btn btn-secondary text-left moreinfo">
                        <label class="font-weight-bold mb-0">定期定額授權扣款紀錄</label>
                    </div>
                    <div class="container mt-2">
                        <h5>已扣款金額總計：<asp:Label ID="LB_HPaidTotal" runat="server" CssClass="text-success" Text="0"></asp:Label>
                            元；
                            尚未扣款金額總計：<asp:Label ID="LB_HUnPaidTotal" CssClass="text-danger" runat="server" Text="0"></asp:Label>
                            元
                        </h5>
                        <div class="table-responsive">
                            <table class="table table-bordered table-striped" style="width: 100%">
                                <thead>
                                    <tr>
                                        <th class="text-center font-weight-bold" style="width: 4%">序</th>
                                        <th class="font-weight-bold text-center" style="width: 20%">執行日期</th>
                                        <th class="font-weight-bold text-right" style="width: 20%">已授權金額</th>
                                        <th class="font-weight-bold text-center" style="width: 15%">授權狀態</th>
                                        <th class="font-weight-bold text-center" style="width: 15%">授權失敗原因</th>
                                    </tr>
                                </thead>
                                <tbody>

                                    <asp:SqlDataSource ID="SDS_HCCPOTRecord" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                    <asp:Repeater ID="Rpt_HCCPOTRecord" runat="server" OnItemDataBound="Rpt_HCCPOTRecord_ItemDataBound">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="text-center">
                                                    <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Label ID="LB_HProcessDate" runat="server" Text='<%# Eval("HProcessDate") %>'></asp:Label>
                                                </td>
                                                <td class="text-right">
                                                    <asp:Label ID="LB_HAmount" runat="server" Text='<%# Eval("HAmount") %>'></asp:Label>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Label ID="LB_HRtnMsg" runat="server" Text='<%# Eval("HRtnMsg") %>'></asp:Label>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Label ID="LB_HRtnCode" runat="server" Text='<%# Eval("HRtnCode") %>'></asp:Label>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </div>



                    <!----補登紀錄---->
                    <div class="col-12 btn btn-secondary text-left moreinfo">
                        <label class="font-weight-bold mb-0">補登作業&nbsp;&nbsp;<span class="text-danger">(已完成付款才填寫補登作業)</span></label>
                    </div>
                    <div class="container mt-2">
                        <h5>補登金額總計：<asp:Label ID="LB_RecognizeTotal" runat="server" Text="0" CssClass="text-info"></asp:Label>
                            元</h5>
                        <table class="table table-bordered table-hover" style="width: 100%">
                            <thead>
                                <tr>
                                    <th class="text-center" style="width: 5%">序</th>
                                    <th class="text-center" style="width: 15%">付款日期</th>
                                    <th class="text-right" style="width: 15%">付款金額</th>
                                    <th class="text-center" style="width: 15%">付款方式</th>
                                    <th>備註</th>
                                    <th class="text-center" style="width: 15%">執行</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td class="text-center"></td>
                                    <td class="text-center">
                                        <asp:TextBox ID="TB_HPaymentDate" runat="server" CssClass="form-control datepickertop"></asp:TextBox>
                                    </td>
                                    <td class="text-center">
                                        <asp:TextBox ID="TB_HPayAmount" runat="server" CssClass="form-control text-right" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>
                                    </td>
                                    <td class="text-center">
                                        <asp:DropDownList ID="DDL_HPayMethod" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                                            <asp:ListItem Value="1">EIP</asp:ListItem>
                                            <asp:ListItem Value="2">匯款</asp:ListItem>
                                            <asp:ListItem Value="3">現金</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TB_HRemark" runat="server" CssClass="form-control"></asp:TextBox>
                                    </td>
                                    <td class="text-center">
                                        <asp:Button ID="Btn_PaidRecordAdd" runat="server" Text="新增" CssClass="btn btn-success" OnClick="Btn_PaidRecordAdd_Click" />
                                    </td>
                                </tr>
                                <asp:SqlDataSource ID="SDS_HCCPOPaidRecord" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>

                                <asp:Repeater ID="Rpt_HCCPOPaidRecord" runat="server" DataSourceID="SDS_HCCPOPaidRecord" OnItemDataBound="Rpt_HCCPOPaidRecord_ItemDataBound">
                                    <ItemTemplate>
                                        <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                        <tr>
                                            <td class="text-center">
                                                <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label>
                                            </td>
                                            <td class="text-center">
                                                <asp:Label ID="LB_HPaymentDate" runat="server" Text='<%# Eval("HPaymentDate") %>'></asp:Label>
                                            </td>
                                            <td class="text-right">
                                                <asp:Label ID="LB_HPayAmount" runat="server" Text='<%# Eval("HPayAmount") %>'></asp:Label>
                                            </td>
                                            <td class="text-center">
                                                <asp:DropDownList ID="DDL_HPayMethod" runat="server" CssClass="form-control" Text='<%# Eval("HPayMethod") %>'>
                                                    <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                    <asp:ListItem Value="1">EIP</asp:ListItem>
                                                    <asp:ListItem Value="2">匯款</asp:ListItem>
                                                    <asp:ListItem Value="3">現金</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:Label ID="LB_HRemark" runat="server" Text='<%# Eval("HRemark") %>'></asp:Label>
                                            </td>
                                            <td class="text-center">
                                                <asp:Button ID="Btn_Del" runat="server" Text="刪除" CssClass="btn btn-danger" OnClick="Btn_Del_Click" />
                                            </td>

                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                    </div>
                    <div class="modal-footer justify-content-center">
                        <a class="btn btn-secondary" href="javascript:void(0);" data-dismiss="modal">關閉</a>
                    </div>

                </div>
            </div>
        </div>
    </div>
    <!-- Modal 部分付款/補登  END-->


    <!-- ============================================================== -->
    <!-- All Jquery -->
    <!-- ============================================================== -->
    <script src="assets/node_modules/jquery/jquery-3.2.1.min.js"></script>
    <!-- Bootstrap tether Core JavaScript -->
    <script src="assets/node_modules/popper/popper.min.js"></script>
    <script src="assets/node_modules/bootstrap/dist/js/bootstrap.min.js"></script>
    <script src="dist/js/sidebarmenu.js"></script>
    <!--Custom JavaScript -->
    <script src="dist/js/custom.min.js"></script>
    <!--Select2-->
    <script src="js/select2.min.js"></script>
    <!--datepicker-->
    <script src="js/moment.min.js"></script>
    <script src="js/bootstrap-datepicker.js"></script>


    <script>
        $(function () {
            $('.js-example-basic-single').select2({
            });
        });

        $(function () {
            $(".datepicker").datepicker({
                format: 'yyyy/mm/dd',
                autoclose: true,
                toggleActive: false,
                todayHighlight: true,
                orientation: 'bottom auto',
            });
        });

        $(function () {
            $(".datepickertop").datepicker({
                format: 'yyyy/mm/dd',
                autoclose: true,
                toggleActive: false,
                todayHighlight: true,
                orientation: 'top auto',
            });
        });
    </script>


</asp:Content>

