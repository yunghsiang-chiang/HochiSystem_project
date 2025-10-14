<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HCCPeriodVerify.aspx.cs" Inherits="System_HCCPeriodVerify" %>

<%--匯出Excel--%>
<%@ Register Assembly="MyWebControls" Namespace="MyWebControls" TagPrefix="cc1" %>
<%@ Register TagPrefix="Page" TagName="Paging" Src="~/System/Paging_backend.ascx" %>
<%--分頁--%>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .form-control:disabled, .form-control[readonly] {
            opacity: 1;
        }

        .table th, .table td {
            padding: 0.2rem;
        }

        .table tr th, .table tr td span {
            font-size: 0.85rem !important;
        }


        .table tr td span {
            display: inline !important;
        }

            .table tr td span.badge {
                font-size: 0.85rem !important;
                padding: 3px 2px;
            }

        .btn-purple {
            padding: 0.3rem 0.4rem;
        }

        .form-group {
            margin-bottom: 6px;
        }

        .form-control:disabled, .form-control[readonly] {
            opacity: 1;
            background-color: #eee;
        }


        .list-style li {
            font-size: 0.8rem !important;
        }

        .form-control {
            min-height: 34px;
        }

        /*定期定額樣式--START*/
        .moreinfo {
            cursor: auto !important;
            background-color: #d5d5d5 !important;
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

        .multiline-ellipsis, .table tr td span.multiline-ellipsis {
            display: -webkit-box !important;
            -webkit-box-orient: vertical;
            -webkit-line-clamp: 3;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        div label {
            font-weight: bold !important
        }

        div.form-group label {
            font-weight: normal;
        }

        input:disabled:checked {
            background-color: #9080ad !important;
        }

        .label-black {
            color: #464646;
        }
        /*定期定額樣式--END*/
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

            <asp:Panel ID="Panel_List" runat="server">
                <!-- ============================================================== -->
                <div class="block-header">
                    <div class="row">
                        <div class="col-lg-5 col-md-12 col-sm-4">
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>信用卡授權審核管理<%--信用卡定期定額授權審核管理--%></h2>
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item active">信用卡授權審核管理<%--信用卡定期定額授權審核管理--%>
                                </li>
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

                                <div class="row">
                                    <!-- Column -->
                                    <div class="col-md-12 col-lg-2 col-xlg-2">
                                        <div class="box text-left">
                                            <button id="Btn_Add" runat="server" type="button" class="btn btn-outline-info" onclick="window.location.href='HCCPeriod_Add.aspx';"><i class="fa fa-plus"></i>新增授權書</button>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12 col-lg-12 col-xlg-12">
                                        <div class="box form-group row m-b-0">
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TB_SKeyword" runat="server" class="form-control" AutoComplete="off" placeholder="請輸入授權申請單號或捐款人姓名或祝福對象或捐款項目"></asp:TextBox>
                                            </div>
                                            <div class="col-md-2">
                                                <asp:DropDownList ID="DDL_SHStatus" CssClass="form-control" runat="server" Style="width: 100%">
                                                    <asp:ListItem Value="99">請選擇授權書狀態</asp:ListItem>
                                                    <asp:ListItem Value="0">停用/中止</asp:ListItem>
                                                    <asp:ListItem Value="1">有效</asp:ListItem>
                                                    <asp:ListItem Value="2">作廢</asp:ListItem>
                                                    <asp:ListItem Value="3">新增申請中</asp:ListItem>
                                                    <%--新增申請中--%>
                                                    <asp:ListItem Value="4">變更申請中</asp:ListItem>
                                                    <%--變更申請中--%>
                                                    <%--<asp:ListItem Value="5">提前付清</asp:ListItem>--%>
                                                    <asp:ListItem Value="6">付訖終止 </asp:ListItem>
                                                    <%--<asp:ListItem Value="7">換單</asp:ListItem>--%>


                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-md-2">
                                                <asp:DropDownList ID="DDL_SHVerifyStatus" CssClass="form-control" runat="server" Style="width: 100%">
                                                    <asp:ListItem Value="0">請選擇審核狀態</asp:ListItem>
                                                    <asp:ListItem Value="1">未送審</asp:ListItem>
                                                    <asp:ListItem Value="2">送審中</asp:ListItem>
                                                    <asp:ListItem Value="3">審核通過</asp:ListItem>
                                                    <asp:ListItem Value="4">審核不通過</asp:ListItem>

                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-md-2">
                                                <asp:DropDownList ID="DDL_SHOrderStatus" CssClass="form-control" runat="server" Style="width: 100%">
                                                    <asp:ListItem Value="0">請選擇授權訂單狀態</asp:ListItem>
                                                    <asp:ListItem Value="1">未成立</asp:ListItem>
                                                    <asp:ListItem Value="2">已成立</asp:ListItem>

                                                </asp:DropDownList>
                                            </div>



                                            <div class="col-md-2">
                                                <asp:LinkButton ID="LBtn_Search" runat="server" class="btn btn-outline-secondary" OnClick="LBtn_Search_Click"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                                <asp:LinkButton ID="LBtn_SearchCancel" runat="server" class="btn btn-outline-secondary m-l-10" OnClick="LBtn_SearchCancel_Click"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                            </div>

                                            <div class="text-right excel_outer d-none">
                                                <cc1:WordExcelButton ID="WordExcelButton2" runat="server" GridView="GridView1" ViewStateMode="Enabled" class="NoPrint" Style="display: inline;" />
                                            </div>



                                        </div>
                                    </div>
                                </div>

                                <div class="row justify-content-start align-items-center mt-3 mb-2 pl-4 pr-3">
                                    <asp:LinkButton ID="LBtn_TransferCCPOrder" runat="server" class="btn btn-success mr-2" OnClick="LBtn_TransferCCPOrder_Click"><span class="btn-label"><i class="fa fa-share"></i></span>&nbsp;轉成信用卡授權訂單
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="LBtn_BatchAllow" runat="server" class="btn btn-info" OnClick="LBtn_BatchAllow_Click"><span class="btn-label"><i class="fa fa-check"></i></span>&nbsp;批次審核通過
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="LBtn_ToExcel" runat="server" class="btn btn-outline-success ml-2" Visible="false" OnClick="LBtn_ToExcel_Click"><span class="btn-label"><i class="ti-export m-r-5"></i>匯出Excel</span></asp:LinkButton>

                                </div>
                                <span class="text-danger font-weight-bold">【提醒】授權書申請單經審核通過後才可勾選轉成信用卡授權訂單。</span>
                                <div class="table-responsive" id="ToWordExcel" runat="server">
                                    <table class="table table-hover">
                                        <thead>
                                            <tr>
                                                <th class="text-center" style="width: 3%; font-size: 0.85rem">勾選</th>
                                                <th class="text-center" style="width: 2%">No</th>
                                                <th style="width: 8%">授權申請單號</th>
                                                <th style="width: 5%">捐款人姓名</th>
                                                <th style="width: 7%">捐款人手機</th>
                                                <th style="width: 6%">祝福對象</th>
                                                <th class="text-center" style="width: 4%">卡號<br />
                                                    末四碼</th>
                                                <th style="width: 8%">捐款項目</th>
                                                <th class="text-right" style="width: 6%">捐款總金額</th>
                                                <th class="text-right d-none" style="width: 4%">扣款<br />
                                                    期數</th>
                                                <th class="text-right" style="width: 6%">每期<br />
                                                    扣款金額</th>
                                                <th class="text-center" style="width: 12%">扣款期間</th>
                                                <th class="text-center" style="width: 8%">期數<br />
                                                    <span style="font-size: 0.8rem;">(已扣/總期數)</span></th>
                                                <th class="text-center" style="width: 6%">授權書<br />
                                                    狀態</th>
                                                <th class="text-center" style="width: 4%">審核<br />
                                                    狀態</th>
                                                <th class="text-center" style="width: 5%">授權訂單<br />
                                                    狀態</th>
                                                <th class="text-center" style="width: 5%">申請資訊</th>
                                                <th class="text-center" style="width: 5%">執行<%--啟用/停用狀態--%><br />
                                                </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:SqlDataSource ID="SDS_HCCPeriod" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                            <asp:Repeater ID="Rpt_HCCPeriod" runat="server" OnItemDataBound="Rpt_HCCPeriod_ItemDataBound">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="text-center">
                                                            <asp:CheckBox ID="CB_Select" runat="server" />
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HIndex" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HCCPeriodCode" runat="server" Text='<%# Eval("HCCPeriodCode") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HDUserName" runat="server" Text='<%# Eval("HDUserName") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HDPhone" runat="server" Text='<%# Eval("HDPhone") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HDonor" runat="server" Text='<%# Eval("HDonor") %>' CssClass="multiline-ellipsis"></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HCardNum" runat="server" Text='<%# Eval("HCardNum") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HCourseName" runat="server" Text='<%# Eval("HCourseName") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HDTotal" runat="server" Text='<%# Eval("HDTotal") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-right d-none">
                                                            <asp:Label ID="LB_HDCCPTimes" runat="server" Text='<%# Eval("HDCCPTimes") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HDCCPAmount" runat="server" Text='<%# Eval("HDCCPAmount") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HDCCPDateRange" runat="server" Style="font-size: 0.95rem" Text='<%# Eval("HDCCPDateRange") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_PaidPeriod" runat="server" Text="0"></asp:Label>/<asp:Label ID="LB_TotalPeriod" runat="server" Text='<%# Eval("HDCCPTimes") %>'></asp:Label>
                                                        </td>

                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HStatus" runat="server" CssClass="badge badge-danger border-0" Text='<%# Eval("HStatus") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HVerifyStatus" runat="server" CssClass="badge badge-danger border-0" Text='<%# Eval("HVerifyStatus") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HOrderStatus" runat="server" CssClass="badge badge-danger border-0" Text='<%# Eval("HOrderStatus") %>'></asp:Label>
                                                        </td>

                                                        <td class="text-center">
                                                            <asp:LinkButton ID="LBtn_View" runat="server" CssClass="btn btn-purple text-white" OnClick="LBtn_View_Click">審核/查看</asp:LinkButton>
                                                        </td>

                                                        <td class="text-center">
                                                            <asp:LinkButton ID="LBtn_StartUsing" runat="server" CssClass="btn btn-success text-white" OnClick="LBtn_StartUsing_Click">啟用</asp:LinkButton>
                                                            <asp:LinkButton ID="LBtn_Deactivate" runat="server" CssClass="btn btn-danger text-white" OnClick="LBtn_Deactivate_Click">停用</asp:LinkButton>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                    <!--分頁-->
                                    <!------------------分頁功能開始------------------>
                                    <div class="box text-right">
                                        <Page:Paging runat="server" ID="Pg_Paging" />
                                    </div>
                                    <!------------------分頁功能結束------------------>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>

    <asp:GridView ID="GridView1" runat="server" Visible="false"></asp:GridView>


    <!-- Modal 授權書明細 START-->
    <div class="modal fade" id="Div_HCCPeriod" tabindex="-1" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" role="document" style="max-width: 90%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h3 class="modal-title font-weight-bold mb-0" id="exampleModalLongTitle"><%--【--%><asp:Label ID="LB_HTitle" runat="server" Text="" Style="font-size: 24px;"></asp:Label><%--】--%>定期定額授權書申請資訊
                    </h3>

                    <asp:Label ID="LB_HCourseID" runat="server" Text="0" Visible="false"></asp:Label>
                    <asp:Label ID="LB_HCTemplateID" runat="server" Text="0" Visible="false"></asp:Label>
                    <asp:Label ID="LB_HDateRange" runat="server" Text="0" Visible="false"></asp:Label>

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
                                            <asp:Label ID="LB_Changed" runat="server" CssClass="text-danger" Text="(變更後)" Visible="false"></asp:Label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_MHCCPeriodCode" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-3 col-sm-12">
                                            <label>
                                                授權書狀態</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_OriHStatus" runat="server" Text="Label" Visible="false"></asp:Label>
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
                                                <asp:Label ID="LB_HMemberID" runat="server" Text="" Visible="false"></asp:Label>
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

                                <asp:LinkButton ID="LinkButton1" runat="server" class="col-12 btn btn-secondary text-left moreinfo">
<label class="font-weight-bold mb-0">備註
</label>
                                </asp:LinkButton>

                                <div class="row clearfix p-2">
                                    <div class="col-md-12 col-sm-12">
                                        <div class="form-group">
                                            <asp:TextBox ID="TB_HRemark" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" PlaceHolder="請輸入備註資訊"></asp:TextBox>
                                        </div>
                                    </div>


                                </div>

                                <!----開啟前台變更申請按鈕與設定扣款起始日---->
                                <asp:LinkButton ID="LinkButton2" runat="server" class="col-12 btn btn-secondary text-left moreinfo">
<label class="font-weight-bold mb-0">開啟前台變更申請按鈕與設定扣款起始日
</label>
                                </asp:LinkButton>


                                <div class="row p-2">
                                    <div class="col-md-3 col-sm-12">
                                        <label class="mb-1">開啟前台變更申請按鈕</label>
                                        <div class="form-group d-flex">
                                            <asp:RadioButtonList ID="RBL_HOpenEdit" runat="server" Style="vertical-align: top;" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                <asp:ListItem Value="0" Selected="True" Style="margin-right: 5px;">關閉</asp:ListItem>
                                                <asp:ListItem Value="1" Style="margin-right: 5px;">開啟</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>

                                    <div class="col-md-3 col-sm-12">
                                        <label class="mb-1">新的扣款起始日</label>
                                        <div class="form-group d-flex">
                                            <asp:TextBox ID="TB_HNewDCCPSDate" runat="server" CssClass="form-control datepicker"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <!----變更內容---->



                                <!----變更內容---->
                                <asp:Panel ID="Panel_Change" runat="server" Visible="false">

                                    <asp:Label ID="LB_Change" runat="server" Text="0" Visible="false"></asp:Label>
                                    <asp:LinkButton ID="LinkButton3" runat="server" class="col-12 btn btn-secondary text-left moreinfo" Style="background-color: #9080ad !important; color: #fff">
<label class="font-weight-bold mb-0">變更申請內容
</label>
                                    </asp:LinkButton>




                                    <div class="col-md-6 col-sm-12" id="Div_ChangedCode" runat="server">
                                        <label class="mb-1">變更後授權書單號</label>
                                        <div class="form-group d-flex">
                                            <asp:Label ID="LB_HNewCCPeriodCode" runat="server" Text="0" Visible="true"></asp:Label>

                                        </div>
                                    </div>

                                    <div class="row p-2">

                                        <div class="col-md-3 col-sm-12">
                                            <label class="mb-1">選擇是否提前付清或部分付款</label>
                                            <div class="form-group d-flex">
                                                <asp:RadioButtonList ID="RBL_PayOption" runat="server" Style="vertical-align: top;" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                    <asp:ListItem Value="0" Enabled="false" Selected="True" Style="margin-right: 5px;">不選擇</asp:ListItem>
                                                    <asp:ListItem Value="1" Enabled="false" Style="margin-right: 5px;">部份付款</asp:ListItem>
                                                    <asp:ListItem Value="2" Enabled="false">提前付清</asp:ListItem>
                                                </asp:RadioButtonList>


                                            </div>
                                        </div>

                                        <div id="Div_Original" runat="server" visible="false" class="text-danger font-weight-bold" style="font-size: 1.2rem">
                                            (原授權書單號：<asp:Label ID="LB_HOriCCPeriodCode" runat="server" Text="0" Style="font-size: 1.2rem" Visible="true"></asp:Label>，
      <asp:Label ID="LB_HModifyInfo" runat="server" Text="0" Style="font-size: 1.2rem"></asp:Label>)
                                        </div>
                                        <asp:Label ID="LB_HPartialPayTimes" runat="server" Text="0" Visible="false"></asp:Label>


                                        <div class="col-md-9 col-sm-12">
                                            <div class="row clearfix">
                                                <div class="col-md-3 col-sm-12">
                                                    <label>部分付款金額</label>
                                                    <div class="form-group d-flex">
                                                        <asp:Label ID="LB_HPartialPayAmount" runat="server"></asp:Label>
                                                    </div>
                                                </div>

                                                <div class="col-md-3 col-sm-12">
                                                    <label>訂單代碼</label>
                                                    <div class="form-group d-flex">
                                                        <asp:Label ID="LB_HOrderGroup" runat="server"></asp:Label>
                                                    </div>
                                                </div>

                                                <div class="col-md-3 col-sm-12">
                                                    <label>付款狀態</label>
                                                    <div class="form-group d-flex">

                                                        <asp:Label ID="LB_HItemStatus" runat="server"></asp:Label>
                                                    </div>
                                                </div>


                                            </div>
                                        </div>

                                    </div>
                                </asp:Panel>

                                <div class="d-none">
                                    <h4>部分付款/補登作業</h4>
                                    <div class="row p-2">

                                        <div class="col-md-3 col-sm-12">
                                            <label class="mb-1">選擇是否提前付清或部分付款</label>
                                            <div class="form-group d-flex">
                                                <asp:RadioButtonList ID="RadioButtonList1" runat="server" Style="vertical-align: top;" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                    <asp:ListItem Value="0" Enabled="false" Selected="True" Style="margin-right: 5px;">不選擇</asp:ListItem>
                                                    <asp:ListItem Value="1" Enabled="false" Style="margin-right: 5px;">部份付款</asp:ListItem>
                                                    <asp:ListItem Value="2" Enabled="false">提前付清</asp:ListItem>
                                                </asp:RadioButtonList>


                                            </div>
                                        </div>

                                        <div id="Div1" runat="server" visible="false" class="text-danger font-weight-bold" style="font-size: 1.2rem">
                                            (原授權書單號：<asp:Label ID="Label1" runat="server" Text="0" Style="font-size: 1.2rem" Visible="true"></asp:Label>，
                                            <asp:Label ID="Label2" runat="server" Text="0" Style="font-size: 1.2rem"></asp:Label>)
                                        </div>
                                        <asp:Label ID="Label3" runat="server" Text="0" Visible="false"></asp:Label>


                                        <div class="col-md-9 col-sm-12">
                                            <div class="row clearfix">
                                                <div class="col-md-3 col-sm-12">
                                                    <label>部分付款金額</label>
                                                    <div class="form-group d-flex">
                                                        <asp:Label ID="Label4" runat="server"></asp:Label>
                                                    </div>
                                                </div>

                                                <div class="col-md-3 col-sm-12">
                                                    <label>訂單代碼</label>
                                                    <div class="form-group d-flex">
                                                        <asp:Label ID="Label5" runat="server"></asp:Label>
                                                    </div>
                                                </div>

                                                <div class="col-md-3 col-sm-12">
                                                    <label>付款狀態</label>
                                                    <div class="form-group d-flex">

                                                        <asp:Label ID="Label6" runat="server"></asp:Label>
                                                    </div>
                                                </div>


                                            </div>
                                        </div>

                                    </div>


                                    <!----結清補登---->
                                    <div class="container">

                                        <table class="table table-bordered table-hover" style="width: 70%">
                                            <thead>
                                                <tr>
                                                    <th>NO</th>
                                                    <th>付款時間</th>
                                                    <th>付款金額</th>
                                                    <th>付款方式</th>
                                                    <th>備註</th>
                                                    <th>執行</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <input class="form-control datepicker" />
                                                    </td>
                                                    <td>
                                                        <input class="form-control" />
                                                    </td>
                                                    <td>
                                                        <select class="form-control">
                                                            <option value="0">請選擇</option>
                                                            <option value="EIP">EIP付款</option>
                                                            <option value="ATM">ATM匯款</option>
                                                            <option value="現金">現金</option>
                                                        </select>
                                                    </td>
                                                    <td>
                                                        <input class="form-control" />
                                                    </td>
                                                    <td>
                                                        <button class="btn btn-success">新增</button>

                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>1</td>
                                                    <td>2024/11/7</td>
                                                    <td class="text-right">15,000</td>
                                                    <td>
                                                        <select class="form-control">
                                                            <option value="EIP">EIP付款</option>
                                                            <option value="ATM">ATM匯款</option>
                                                            <option value="現金">現金</option>
                                                        </select>
                                                    </td>
                                                    <td>已結清</td>
                                                    <td>
                                                        <button class="btn btn-danger">刪除</button>
                                                    </td>
                                                </tr>

                                            </tbody>
                                        </table>

                                    </div>

                                </div>
                                <div class="modal-footer justify-content-center">
                                    <asp:Button ID="Btn_Allow" runat="server" class="btn btn-success" OnClick="Btn_Allow_Click" Text="通過"></asp:Button>
                                    <asp:Button ID="Btn_Deny" runat="server" class="btn btn-info" OnClick="Btn_Deny_Click" Text="不通過"></asp:Button>
                                    <asp:Button ID="Btn_Valid" runat="server" class="btn btn-success" OnClick="Btn_Valid_Click" Visible="false" Text="啟用"></asp:Button>
                                    <asp:Button ID="Btn_Invalid" runat="server" class="btn btn-danger" OnClick="Btn_Invalid_Click" Text="作廢"></asp:Button>
                                    <asp:Button ID="Btn_Save" runat="server" class="btn btn-success" OnClick="Btn_Save_Click" Visible="false" Text="儲存"></asp:Button>
                                    <asp:Button ID="Btn_Download" runat="server" Text="下載pdf" CssClass="btn btn-purple text-white" OnClick="Btn_Download_Click" />
                                    <asp:LinkButton ID="LBtn_Cancel" runat="server" class="btn btn-secondary" data-miss="modal">取消</asp:LinkButton>
                                    <%--<asp:Button ID="Btn_Cancel" runat="server" class="btn btn-secondary" OnClick="Btn_Cancel_Click" Text="取消"></asp:Button>--%>
                                </div>


                            </div>

                        </div>


                    </div>




                </div>
            </div>
        </div>
    </div>
    <!-- Modal 訂單明細  END-->




    <!-- ============================================================== -->
    <!-- All Jquery -->
    <!-- ============================================================== -->
    <script src="assets/node_modules/jquery/jquery-3.2.1.min.js"></script>
    <!-- Bootstrap tether Core JavaScript -->
    <script src="assets/node_modules/popper/popper.min.js"></script>
    <script src="assets/node_modules/bootstrap/dist/js/bootstrap.min.js"></script>
    <!--Menu sidebar -->
    <script src="dist/js/sidebarmenu.js"></script>
    <!--Custom JavaScript -->
    <script src="dist/js/custom.min.js"></script>

    <!-- icheck -->
    <script src="assets/node_modules/icheck/icheck.min.js"></script>
    <script src="assets/node_modules/icheck/icheck.init.js"></script>
    <!--select2-->
    <script src="js/select2.min.js"></script>
    <!--Custom JavaScript -->
    <script src="js/_custom.js"></script>
    <!--datepicker -->
    <script src="js/moment.min.js"></script>
    <script src="js/bootstrap-datepicker.js"></script>

    <script>
        $('.datepicker').datepicker(
            {
                format: 'yyyy/mm/dd',
                todayHighlight: true,
                orientation: 'top auto',
                toggleActive: true, autoclose: true,
            });
    </script>

</asp:Content>

