<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HCCRegular.aspx.cs" Inherits="System_HCCRegular" MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="Page" TagName="Paging" Src="~/System/Paging_backend.ascx" %>
<%--分頁--%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .card .body {
            padding: 15px 10px;
        }

        thead th {
            font-size: 0.87rem;
        }

        #AddCode thead th {
            font-size: 0.9rem;
        }

        .datepicker {
            padding: 3px;
        }

        input::placeholder {
            color: #484848 !important;
        }

        .badge span {
            font-size: 14px !important;
            padding: 3px;
            margin-left: 0;
            margin-right: 0;
        }

        @media (max-width:992px) {
            .col-sm-3 {
                flex: 0 0 16%;
                max-width: 16%;
            }

            .col-sm-10 {
                flex: 0 0 84%;
                max-width: 84%;
            }
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

            <asp:Panel ID="Panel_List" runat="server">
                <!-- ============================================================== -->
                <div class="block-header">
                    <div class="row">
                        <div class="col-lg-3 col-md-12 col-sm-4">
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>定期定額專區</h2>
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item active">定期定額專區</li>
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
                                <asp:UpdatePanel ID="UPanel_HCourseMaterial" runat="server" Visible="true" Style="width: 100%;">
                                    <ContentTemplate>

                                        <div class="text-right">
                                            <a class="btn btn-info" href="javascript:void(0);" data-toggle="modal" data-target="#info">操作說明</a>
                                        </div>

                                        <asp:SqlDataSource ID="SDS_HDPurpose" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HDPurpose FROM HDPurpose WHERE HStatus='1'"></asp:SqlDataSource>
                                        <asp:SqlDataSource ID="SDS_HDonationItem" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HDPurposeID, HDItem FROM HDonationItem WHERE HStatus='1'"></asp:SqlDataSource>


                                        <div class="row">
                                            <div class="col-md-10 col-lg-12 col-xlg-12">
                                                <div class="box form-group row m-b-0">
                                                    <div class="col-md-2">

                                                        <asp:DropDownList ID="DDL_SHDPurpose" runat="server" class="form-control js-example-basic-single" Style="width: 100%" placeholder="選擇捐款用途" DataSourceID="SDS_HDPurpose" DataTextField="HDPurpose" DataValueField="HID" AppendDataBoundItems="true" OnSelectedIndexChanged="DDL_SHDPurpose_SelectedIndexChanged" AutoPostBack="true">
                                                            <asp:ListItem Value="0">選擇捐款用途</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-2">
                                                        <asp:DropDownList ID="DDL_SHDItem" runat="server" class="form-control js-example-basic-single" Style="width: 100%" placeholder="選擇捐款項目" DataSourceID="SDS_HDonationItem" DataTextField="HDItem" DataValueField="HID" AppendDataBoundItems="true">
                                                            <asp:ListItem Value="0">選擇捐款項目</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-2">
                                                        <asp:LinkButton ID="LBtn_Search" runat="server" class="btn btn-outline-secondary" OnClick="LBtn_Search_Click"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                                        <asp:LinkButton ID="LBtn_SearchCancel" runat="server" class="btn btn-outline-secondary m-l-10" OnClick="LBtn_SearchCancel_Click"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <%-- <p class="mb-0 mt-3 text-info">*單號開頭編碼規則： <br />
                                            1. 共7碼，帳戶(1碼)+捐款用途代碼(2碼)+紙本/電子(1碼)+八十四輪年度(2碼)+八十四輪續報(1碼)； <br />
                                            2. 前四碼系統會依設定自訂編碼；八十四輪年度(2碼)+八十四輪續報(1碼)會依是否開放紙本授權作判別。</p>--%>

                                        <div class="table-responsive">
                                            <table class="table table-hover m-t-10">
                                                <thead>
                                                    <tr>
                                                        <th class="text-center" style="width: 2%">序</th>
                                                        <th class="text-center" style="width: 6%">執行</th>
                                                        <th class="text-left" style="width: 10%"><span class="text-danger">*</span>捐款用途</th>
                                                        <th class="text-left" style="width: 9%"><span class="text-danger">*</span>捐款項目</th>
                                                        <th class="text-center" style="width: 8%"><span class="text-danger">*</span>開放紙本授權</th>
                                                        <th class="text-left" style="width: 7%"><span class="text-danger">*</span>單號開頭</th>
                                                        <th class="text-right" style="width: 7%"><span class="text-danger">*</span>最低總金額</th>
                                                        <%--有金額限制時必填--%>
                                                        <th class="text-center d-none" style="width: 7%">是否限制<br />
                                                            最低總金額</th>
                                                        <th class="text-center" style="width: 7%"><span class="text-danger">*</span>最低扣款<br />
                                                            期數</th>
                                                        <th class="text-center" style="width: 7%"><span class="text-danger">*</span>最高扣款<br />
                                                            期數</th>
                                                        <th class="text-center" style="width: 7%"><span class="text-danger">*</span>開放祝福<br />
                                                            對象</th>

                                                        <th class="text-center" style="width: 8%"><span class="text-danger" id="Star" runat="server" visible="false">*</span>紙本扣款起始日</th>
                                                        <th class="text-center" style="width: 8%"><span class="text-danger">*</span>開放填寫日期</th>
                                                        <th class="text-center" style="width: 8%"><span class="text-danger">*</span>截止填寫日期</th>


                                                        <th class="text-center" style="width: 5%">狀態</th>
                                                    </tr>
                                                </thead>
                                                <tbody>

                                                    <asp:SqlDataSource ID="SDS_HDPurpose1" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HDPurpose FROM HDPurpose WHERE HStatus='1'"></asp:SqlDataSource>
                                                    <asp:SqlDataSource ID="SDS_HDonationItem1" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HDPurposeID, HDItem FROM HDonationItem WHERE HStatus='1'"></asp:SqlDataSource>
                                                    <tr>
                                                        <td class="text-center"></td>
                                                        <td class="text-center">
                                                            <asp:LinkButton ID="LBtn_Add" runat="server" class="btn btn-sm btn-outline-primary" ToolTip="新增" OnClick="LBtn_Add_Click"><i class="ti-plus text-primary"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:DropDownList ID="DDL_HDPurpose" runat="server" class="form-control js-example-basic-single" name="state" Style="width: 100%" DataSourceID="SDS_HDPurpose" DataTextField="HDPurpose" DataValueField="HID" AppendDataBoundItems="true" Visible="true" OnSelectedIndexChanged="DDL_HDPurpose_SelectedIndexChanged" AutoPostBack="true">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td class="text-left">
                                                            <asp:DropDownList ID="DDL_HDItem" runat="server" class="form-control js-example-basic-single" name="state" Style="width: 100%" DataSourceID="SDS_HDonationItem" DataTextField="HDItem" DataValueField="HID" AppendDataBoundItems="true" Visible="true" OnSelectedIndexChanged="DDL_HDItem_SelectedIndexChanged" AutoPostBack="true">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:DropDownList ID="DDL_HOpenPaper" runat="server" class="form-control js-example-basic-single text-center" name="state" Style="width: 100%" Visible="true" OnSelectedIndexChanged="DDL_HOpenPaper_SelectedIndexChanged" AutoPostBack="true">
                                                                <asp:ListItem Value="0">否</asp:ListItem>
                                                                <asp:ListItem Value="1">是</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td class="text-center">

                                                            <asp:Label ID="LB_HCCPCodeHead" runat="server" Text=""></asp:Label>

                                                            <asp:LinkButton ID="LBtn_AddCode" runat="server" OnClick="LBtn_AddCode_Click"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                            <%--<a data-toggle="modal" data-target="#AddCode"><i class="fa fa-pencil"></i></a>--%>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:TextBox ID="TB_HMinTotal" runat="server" class="form-control text-right" AutoComplete="off" MaxLength="5" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>
                                                        </td>

                                                        <td class="d-none">
                                                            <asp:DropDownList ID="DDL_HLimitTotal" runat="server" class="form-control js-example-basic-single" name="state" Style="width: 100%" Visible="true">
                                                                <asp:ListItem Value="0">不限制</asp:ListItem>
                                                                <asp:ListItem Value="1">限制</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TB_HMinTimes" runat="server" class="form-control text-center" AutoComplete="off" MaxLength="2" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TB_HMaxTimes" runat="server" class="form-control  text-center" AutoComplete="off" MaxLength="2" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:DropDownList ID="DDL_HOpenBlessing" runat="server" class="form-control js-example-basic-single text-center" name="state" Style="width: 100%" Visible="true">
                                                                <asp:ListItem Value="0">否</asp:ListItem>
                                                                <asp:ListItem Value="1">是</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>

                                                        <td>
                                                            <asp:TextBox ID="TB_HCCPSDate" runat="server" class="form-control datepicker text-center" AutoComplete="off" PlaceHolder="yyyy/MM/dd" Enabled="false"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TB_HDOpenDate" runat="server" class="form-control datepicker text-center" AutoComplete="off" PlaceHolder="yyyy/MM/dd"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TB_HDExpDate" runat="server" class="form-control datepicker text-center" AutoComplete="off" PlaceHolder="yyyy/MM/dd"></asp:TextBox>
                                                        </td>

                                                        <td></td>
                                                    </tr>
                                                    <asp:SqlDataSource ID="SDS_HCCRegular" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HCCPCodeHead, HDPurposeID, HDPurpose, HDItemID, HDItem, HLimitTotal, HMinTotal, HMinTimes, HMaxTimes, HCCPSDate,  HDOpenDate, HDExpDate, HOpenBlessing, HOpenPaper, HStatus FROM HCCRegular"></asp:SqlDataSource>
                                                    <asp:Repeater ID="Rpt_HCCRegular" runat="server" OnItemDataBound="Rpt_HCCRegular_ItemDataBound">
                                                        <ItemTemplate>
                                                            <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>

                                                            <tr>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:LinkButton ID="LBtn_Save" runat="server" CssClass="btn btn-sm btn-outline-info" ToolTip="儲存" Visible="false" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Save_Click"><i class="ti-check"></i></asp:LinkButton>
                                                                    <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success" ToolTip="編輯" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Edit_Click"><i class="icon-pencil"></i></asp:LinkButton>
                                                                    <asp:LinkButton ID="LBtn_Del" runat="server" class="btn btn-sm btn-outline-danger js-sweetalert" ToolTip="刪除" Btmessage="確定要刪除嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Del_Click"><i class="ti-trash"></i></asp:LinkButton>
                                                                    <asp:LinkButton ID="LBtn_Stop" runat="server" class="btn btn-sm btn-outline-danger js-sweetalert" ToolTip="停用" Btmessage="確定要停用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' CommandArgument='<%# Eval("HID") %>' Visible="false" OnClick="LBtn_Stop_Click"><i class="ti-na"></i></asp:LinkButton>
                                                                    <asp:LinkButton ID="LBtn_Upload" runat="server" class="btn btn-sm btn-outline-info js-sweetalert" ToolTip="啟用" CommandArgument='<%# Eval("HID") %>' Btmessage="確定要啟用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" OnClick="LBtn_Upload_Click"><i class="ti-upload"></i></asp:LinkButton>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="LB_HDPurpose" runat="server" Text='<%# Eval("HDPurpose") %>'></asp:Label>
                                                                    <asp:Label ID="LB_HDPurposeID" runat="server" Text='<%# Eval("HDPurposeID") %>' Visible="false"></asp:Label>
                                                                    <asp:DropDownList ID="DDL_HDPurpose1" runat="server" class="form-control js-example-basic-single" name="state" Style="width: 100%" DataSourceID="SDS_HDPurpose1" DataTextField="HDPurpose" DataValueField="HID" AppendDataBoundItems="true" Visible="false" OnSelectedIndexChanged="DDL_HDPurpose1_SelectedIndexChanged" AutoPostBack="true">
                                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="LB_HDItem" runat="server" Text='<%# Eval("HDItem") %>'></asp:Label>
                                                                    <asp:Label ID="LB_HDItemID" runat="server" Text='<%# Eval("HDItemID") %>' Visible="false"></asp:Label>
                                                                    <asp:DropDownList ID="DDL_HDItem" runat="server" class="form-control js-example-basic-single" name="state" Style="width: 100%" DataSourceID="SDS_HDonationItem1" DataTextField="HDItem" DataValueField="HID" AppendDataBoundItems="true" Visible="false">
                                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HOpenPaper" runat="server" Text='<%# Eval("HOpenPaper") %>'></asp:Label>
                                                                    <asp:DropDownList ID="DDL_HOpenPaper" runat="server" class="form-control js-example-basic-single text-center" name="state" Style="width: 100%" Visible="false" Text='<%# Eval("HOpenPaper") %>' OnSelectedIndexChanged="DDL_HOpenPaper_SelectedIndexChanged1" AutoPostBack="true">
                                                                        <asp:ListItem Value="0">否</asp:ListItem>
                                                                        <asp:ListItem Value="1">是</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HCCPCodeHead" runat="server" Text='<%# Eval("HCCPCodeHead") %>'></asp:Label>
                                                                    <asp:TextBox ID="TB_HCCPCodeHead" runat="server" class="form-control text-right" AutoComplete="off" MaxLength="7" Visible="false"></asp:TextBox>
                                                                    <asp:LinkButton ID="LBtn_EditCode" runat="server" OnClick="LBtn_EditCode_Click" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' Visible="false"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                </td>
                                                                <td class="text-right">
                                                                    <asp:Label ID="LB_HMinTotal" runat="server" Text='<%# Eval("HMinTotal") %>'></asp:Label>
                                                                    <asp:TextBox ID="TB_HMinTotal" runat="server" class="form-control text-right" AutoComplete="off" Text='<%# Eval("HMinTotal") %>' Visible="false" MaxLength="5" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>
                                                                </td>
                                                                <td class="d-none">
                                                                    <asp:Label ID="LB_HLimitTotal" runat="server" Text='<%# Eval("HLimitTotal") %>'></asp:Label>
                                                                    <asp:DropDownList ID="DDL_HLimitTotal" runat="server" class="form-control js-example-basic-single text-center" name="state" Style="width: 100%" Visible="false">
                                                                        <asp:ListItem Value="0">不限制</asp:ListItem>
                                                                        <asp:ListItem Value="1">限制</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HMinTimes" runat="server" Text='<%# Eval("HMinTimes") %>'></asp:Label>
                                                                    <asp:TextBox ID="TB_HMinTimes" runat="server" class="form-control text-center" AutoComplete="off" Text='<%# Eval("HMinTimes") %>' Visible="false" MaxLength="2" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HMaxTimes" runat="server" Text='<%# Eval("HMaxTimes") %>'></asp:Label>
                                                                    <asp:TextBox ID="TB_HMaxTimes" runat="server" class="form-control text-center" AutoComplete="off" Text='<%# Eval("HMaxTimes") %>' Visible="false" MaxLength="2" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HOpenBlessing" runat="server" Text='<%# Eval("HOpenBlessing") %>'></asp:Label>
                                                                    <asp:DropDownList ID="DDL_HOpenBlessing" runat="server" class="form-control js-example-basic-single text-center" name="state" Style="width: 100%" Visible="false" Text='<%# Eval("HOpenBlessing") %>'>
                                                                        <asp:ListItem Value="0">否</asp:ListItem>
                                                                        <asp:ListItem Value="1">是</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>

                                                                <td>
                                                                    <asp:Label ID="LB_HCCPSDate" runat="server" Text='<%# Eval("HCCPSDate") %>'></asp:Label>
                                                                    <asp:TextBox ID="TB_HCCPSDate" runat="server" class="form-control datepicker text-center" AutoComplete="off" PlaceHolder="yyyy/MM/dd" Visible="false" Text='<%# Eval("HCCPSDate") %>'></asp:TextBox>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HDOpenDate" runat="server" Text='<%# Eval("HDOpenDate") %>'></asp:Label>
                                                                    <asp:TextBox ID="TB_HDOpenDate" runat="server" class="form-control datepicker text-center" AutoComplete="off" Text='<%# Eval("HDOpenDate") %>' Visible="false"></asp:TextBox>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HDExpDate" runat="server" Text='<%# Eval("HDExpDate") %>'></asp:Label>
                                                                    <asp:TextBox ID="TB_HDExpDate" runat="server" class="form-control datepicker text-center" AutoComplete="off" Text='<%# Eval("HDExpDate") %>' Visible="false"></asp:TextBox>
                                                                </td>

                                                                <td class="text-center">
                                                                    <div class="" id="Status" runat="server">
                                                                        <asp:Label ID="LB_Status" runat="server" Text='<%# Eval("HStatus") %>' Style="font-size: 0.9rem;"></asp:Label>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            </tbody>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                            </table>
                                            <!--分頁-->
                                            <!------------------分頁功能開始------------------>
                                            <div class="box text-right">
                                                <Page:Paging runat="server" ID="Pg_Paging" />
                                            </div>
                                            <!------------------分頁功能結束------------------>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="LBtn_Search" EventName="Click" />
                                        <asp:PostBackTrigger ControlID="LBtn_AddCode" />
                                        <asp:AsyncPostBackTrigger ControlID="LBtn_SearchCancel" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="DDL_SHDPurpose" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>

                </div>
            </asp:Panel>



        </div>
    </div>


    <!-- Modal 編輯單號編碼規則 START-->
    <asp:Label ID="LB_SubmitType" runat="server" Text="" Visible="false"></asp:Label><!--1=Add，2=Edit-->
    <asp:Label ID="LB_RptNum" runat="server" Text="" Visible="false"></asp:Label>
    <div class="modal fade" id="AddCode" tabindex="-1" role="dialog" aria-labelledby="AddCode" aria-hidden="true">
        <div class="modal-dialog" role="document" style="max-width: 60%;">
            <div class="modal-content">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title" id="exampleModalLongTitle">設定單號開頭編碼</h5>
                </div>
                <div class="modal-body">
                    <table class="table table-bordered table-hover">
                        <thead>
                            <tr style="background-color: #dadada">
                                <th style="width: 10%">帳戶 (1碼)</th>
                                <th>捐款用途代碼 (2碼)</th>
                                <th style="width: 13%">紙本/電子 (1碼)</th>
                                <th>八十四輪年度 (2碼)</th>
                                <th>八十四輪續報 (1碼)</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>
                                    <asp:TextBox ID="TB_HPayAccount" runat="server" Enabled="false" Text="F" class="form-control text-left p-1" Placeholder="F(基金會帳戶)"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="TB_HDTypeCode" runat="server" Enabled="false" Text="" class="form-control text-left p-1" Placeholder="依選捐款用途設定自動帶入"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="TB_Type" runat="server" Enabled="false" Text="" class="form-control text-left p-1" Placeholder="P/E"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="TB_HCCPCodeHeadYear" runat="server" class="form-control text-left p-1" AutoComplete="off" Enabled="false" MaxLength="2" Placeholder="請輸入西元年末兩碼(24、25...)"></asp:TextBox>

                                </td>
                                <td>
                                    <asp:TextBox ID="TB_HCCPCodeHeadSerial" runat="server" class="form-control text-left p-1" AutoComplete="off" Enabled="false" MaxLength="1" Placeholder="請輸入續報次數(0、1、2...)"></asp:TextBox>

                                </td>
                            </tr>
                        </tbody>
                    </table>

                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="LBtn_ECodeSubmit" runat="server" class="btn btn-success" OnClick="LBtn_ECodeSubmit_Click">儲存</asp:LinkButton>
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">關閉</button>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal 編輯單號編碼規則 END-->

    <!-- Modal 操作說明 START-->
    <div class="modal fade" id="info" tabindex="-1" role="dialog" aria-labelledby="info" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title">操作說明</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body ">

                    <video id="video_block" class="embed-responsive-item" style="width: 100%;" controls>
                        <source src="/images/video/HFParameter.mp4" type="video/mp4">
                        您的瀏覽器不支援 video 標籤。
                    </video>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">關閉</button>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal 操作說明 END-->

    <!-- ============================================================== -->
    <!-- All Jquery -->
    <!-- ============================================================== -->
    <script src="assets/node_modules/jquery/jquery-3.2.1.min.js"></script>
    <!-- Bootstrap tether Core JavaScript -->
    <script src="assets/node_modules/popper/popper.min.js"></script>
    <script src="assets/node_modules/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- slimscrollbar scrollbar JavaScript -->
    <script src="dist/js/perfect-scrollbar.jquery.min.js"></script>
    <!--Wave Effects -->
    <script src="dist/js/waves.js"></script>
    <!--Menu sidebar -->
    <script src="dist/js/sidebarmenu.js"></script>
    <!--stickey kit -->
    <script src="assets/node_modules/sticky-kit-master/dist/sticky-kit.min.js"></script>
    <script src="assets/node_modules/sparkline/jquery.sparkline.min.js"></script>
    <!--Custom JavaScript -->
    <script src="dist/js/custom.min.js"></script>
    <!--datepicker-->
    <script src="js/bootstrap-datepicker.js"></script>
    <!--Select2-->
    <script src="js/select2.min.js"></script>

    <script>
        $(function () {
            //單選
            $('.js-example-basic-single').select2();
        });
    </script>
</asp:Content>
