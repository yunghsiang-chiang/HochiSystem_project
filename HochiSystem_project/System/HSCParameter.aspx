<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HSCParameter.aspx.cs" Inherits="System_HSCParameter" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  
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
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" Visible="true" Style="width: 100%;">
                <ContentTemplate>

                    <%--<asp:Panel ID="Panel_List" runat="server">--%>
                    <div class="block-header">
                        <div class="row">
                            <div class="col-lg-6 col-md-12 col-sm-12">
                                <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>專欄相關參數設定</h2>
                                <ul class="breadcrumb">
                                    <li class="breadcrumb-item"><a href="HSCParameter.aspx"><i class="icon-home"></i></a></li>
                                    <li class="breadcrumb-item">專欄相關參數設定</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <!-- ============================================================== -->
                    <!-- Start Page Content -->
                    <!-- ============================================================== -->

                    <div class="row">
                        <div class="col-12">
                            <div class="card">
                                <div class="card-body">

                                    <asp:Label ID="LB_NavTab" runat="server" Text="1" Visible="false"></asp:Label><!--目前在哪個tab-->
                                    <ul class="nav nav-tabs" id="ParaTab" role="tablist">

                                        <asp:SqlDataSource ID="SDS_Tag" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HName_TW, HStatus FROM HSCParmTab WHERE HStatus = 1"></asp:SqlDataSource>
                                        <asp:Repeater ID="RPT_Tag" runat="server" DataSourceID="SDS_Tag">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="HF_HID" runat="server" Value='<%# Eval("HID") %>' />
                                                <li class="nav-item">
                                                    <asp:LinkButton ID="LBtn_Tag" runat="server" class="nav-link font-weight-bold" TabIndex='<%#Convert.ToInt32(Eval("HID")) %>' OnClick="LBtn_Tag_Click">
					                              <span class="hidden-sm-up"></span><span class="hidden-xs-down"><%# Eval("HName_TW") %></span>
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="LBtn_Del" runat="server" class="nav-link font-weight-bold d-none" Visible="false"></asp:LinkButton>
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>

                                    <div class="tab-content p-t-20 p-l-0 p-r-0">

                                        <!--------------------==================專欄分類 START========================--------------->
                                        <div class="tab-pane fade" id="Div_HSCClass" runat="server" role="tabpanel">
                                            <div class="table-responsive">
                                                <table class="table table-hover" style="width: 100%">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 3%" class="text-center">No</th>
                                                            <th class="text-center" style="width: 8%">執行</th>
                                                            <th style="width: 25%">專欄分類</th>
                                                            <th style="width: 25%">狀態</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr runat="server" id="Tr_HSCClass_Add">
                                                            <td class="text-center"></td>
                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_HSCClass_Add" runat="server" class="btn btn-sm btn-outline-primary" ToolTip="新增" OnClick="LBtn_Add_Click"><i class="ti-plus text-primary"></i></asp:LinkButton>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TB_HSCClassName" runat="server" class="form-control" AutoComplete="off"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <asp:SqlDataSource ID="SDS_HSCClass" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCClassName, HStatus FROM HSCClass"></asp:SqlDataSource>
                                                        <asp:Repeater ID="Rpt_HSCClass" runat="server" DataSourceID="SDS_HSCClass" OnItemDataBound="Rpt_ItemDataBound">
                                                            <ItemTemplate>
                                                                <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                                <tr>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:LinkButton ID="LBtn_Save" runat="server" CssClass="btn btn-sm btn-outline-primary" ToolTip="儲存" Visible="false" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Save_Click"><i class="ti-check"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success" ToolTip="編輯" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Edit_Click"><i class="icon-pencil"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Del" runat="server" class="btn btn-sm btn-outline-danger js-sweetalert" ToolTip="停用" Btmessage="確定要停用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Del_Click"><i class="ti-na"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Upload" runat="server" class="btn btn-sm btn-outline-info js-sweetalert" ToolTip="啟用" Btmessage="確定要啟用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Upload_Click"><i class="ti-upload"></i></asp:LinkButton>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HSCClassName" runat="server" CssClass="p-l-10" Text='<%# Eval("HSCClassName") %>'></asp:Label>
                                                                        <asp:TextBox ID="TB_HSCClassName" runat="server" Text='<%# Eval("HSCClassName") %>' class="form-control" Visible="false" BorderColor="#00c292" AutoComplete="off"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <div class="badge badge-default" id="Status" runat="server">
                                                                            <asp:Label ID="LB_Status" runat="server" Text='<%# Eval("HStatus") %>'></asp:Label><!--狀態-->
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>

                                            </div>
                                        </div>
                                        <!--------------------==================專欄分類 END========================--------------->

                                        <!--------------------==================紀錄類型 START========================--------------->
                                        <div class="tab-pane fade" id="Div_HSCRecordType" runat="server" role="tabpanel">
                                            <div class="table-responsive">
                                                <table class="table table-hover" style="width: 100%">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 3%" class="text-center">No</th>
                                                            <th class="text-center" style="width: 8%">執行</th>
                                                            <th style="width: 25%">紀錄類型</th>
                                                            <th style="width: 25%">狀態</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr runat="server" id="Tr_HSCRecordType_Add">
                                                            <td class="text-center"></td>
                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_HSCRType_Add" runat="server" class="btn btn-sm btn-outline-primary" ToolTip="新增" OnClick="LBtn_Add_Click"><i class="ti-plus text-primary"></i></asp:LinkButton>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TB_HSCRTName" runat="server" class="form-control" AutoComplete="off"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <asp:SqlDataSource ID="SDS_HSCRecordType" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCRTName, HStatus FROM HSCRecordType"></asp:SqlDataSource>
                                                        <asp:Repeater ID="Rpt_HSCRecordType" runat="server" DataSourceID="SDS_HSCRecordType" OnItemDataBound="Rpt_ItemDataBound">
                                                            <ItemTemplate>
                                                                <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                                <tr>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:LinkButton ID="LBtn_Save" runat="server" CssClass="btn btn-sm btn-outline-primary" ToolTip="儲存" Visible="false" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Save_Click"><i class="ti-check"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success" ToolTip="編輯" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Edit_Click"><i class="icon-pencil"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Del" runat="server" class="btn btn-sm btn-outline-danger js-sweetalert" ToolTip="停用" Btmessage="確定要停用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Del_Click"><i class="ti-na"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Upload" runat="server" class="btn btn-sm btn-outline-info js-sweetalert" ToolTip="啟用" Btmessage="確定要啟用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Upload_Click"><i class="ti-upload"></i></asp:LinkButton>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HSCRTName" runat="server" CssClass="p-l-10" Text='<%# Eval("HSCRTName") %>'></asp:Label>
                                                                        <asp:TextBox ID="TB_HSCRTName" runat="server" Text='<%# Eval("HSCRTName") %>' class="form-control" Visible="false" BorderColor="#00c292" AutoComplete="off"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <div class="badge badge-default" id="Status" runat="server">
                                                                            <asp:Label ID="LB_Status" runat="server" Text='<%# Eval("HStatus") %>'></asp:Label><!--狀態-->
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>

                                            </div>
                                        </div>
                                        <!--------------------==================紀錄類型 END========================--------------->


                                














                                        <!--------------------==================討論區主類別 START========================--------------->
                                        <div class="tab-pane fade" id="Div_HSCForumClassA" runat="server" role="tabpanel">
                                            <div class="table-responsive">

                                                <table class="table table-hover" style="width: 100%">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 3%" class="text-center">No</th>
                                                            <th class="text-center" style="width: 8%">執行</th>
                                                            <th style="width: 25%">討論區主類別</th>
                                                            <th style="width: 25%">狀態</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr runat="server" id="Tr_HSCForumClassA_Add">
                                                            <td class="text-center"></td>
                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_HSCForumClassA_Add" runat="server" class="btn btn-sm btn-outline-primary" ToolTip="新增" OnClick="LBtn_Add_Click"><i class="ti-plus text-primary"></i></asp:LinkButton>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TB_HSCFCNameA" runat="server" class="form-control" AutoComplete="off"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <asp:SqlDataSource ID="SDS_HSCForumClassA" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCFCName, HStatus FROM HSCForumClass WHERE HSCFCLevel='10'"></asp:SqlDataSource>
                                                        <asp:Repeater ID="Rpt_HSCForumClassA" runat="server" DataSourceID="SDS_HSCForumClassA" OnItemDataBound="Rpt_ItemDataBound">
                                                            <ItemTemplate>
                                                                <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                                <tr>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:LinkButton ID="LBtn_Save" runat="server" CssClass="btn btn-sm btn-outline-primary" ToolTip="儲存" Visible="false" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Save_Click"><i class="ti-check"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success" ToolTip="編輯" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Edit_Click"><i class="icon-pencil"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Del" runat="server" class="btn btn-sm btn-outline-danger js-sweetalert" ToolTip="停用" Btmessage="確定要停用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Del_Click"><i class="ti-na"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Upload" runat="server" class="btn btn-sm btn-outline-info js-sweetalert" ToolTip="啟用" Btmessage="確定要啟用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Upload_Click"><i class="ti-upload"></i></asp:LinkButton>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HSCFCNameA" runat="server" CssClass="p-l-10" Text='<%# Eval("HSCFCName") %>'></asp:Label>
                                                                        <asp:TextBox ID="TB_HSCFCNameA" runat="server" Text='<%# Eval("HSCFCName") %>' class="form-control" Visible="false" BorderColor="#00c292" AutoComplete="off"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <div class="badge badge-default" id="Status" runat="server">
                                                                            <asp:Label ID="LB_Status" runat="server" Text='<%# Eval("HStatus") %>'></asp:Label><!--狀態-->
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>

                                            </div>
                                        </div>
                                        <!--------------------==================討論區主類別 END========================--------------->


                                        <!--------------------==================討論區次類別 START========================--------------->
                                        <div class="tab-pane fade" id="Div_HSCForumClassB" runat="server" role="tabpanel">
                                            <div class="table-responsive">
                                                <asp:SqlDataSource ID="SDS_HSCFCMasterA" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCFCName, HStatus FROM HSCForumClass WHERE HSCFCLevel='10' AND HStatus='1'"></asp:SqlDataSource>
                                                <table class="table table-hover" style="width: 100%">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 3%" class="text-center">No</th>
                                                            <th class="text-center" style="width: 8%">執行</th>
                                                            <th style="width: 10%">討論區主類別</th>
                                                            <th style="width: 25%">討論區次類別</th>
                                                            <th style="width: 25%">狀態</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr runat="server" id="Tr_HSCForumClassB_Add">
                                                            <td class="text-center"></td>
                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_HSCForumClassB_Add" runat="server" class="btn btn-sm btn-outline-primary" ToolTip="新增" OnClick="LBtn_Add_Click"><i class="ti-plus text-primary"></i></asp:LinkButton>
                                                            </td>
                                                            <td>

                                                                <asp:DropDownList ID="DDL_HSCFCMasterA" runat="server" CssClass="form-control  js-example-basic-single" DataSourceID="SDS_HSCFCMasterA" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true">
                                                                    <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TB_HSCFCNameB" runat="server" class="form-control" AutoComplete="off"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <asp:SqlDataSource ID="SDS_HSCForumClassB" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT A.HID, A.HSCFCMaster, B.HSCFCName AS HSCFCNameA, A.HSCFCName AS HSCFCNameB, A.HStatus FROM HSCForumClass A INNER JOIN HSCForumClass B ON A.HSCFCMaster=B.HID WHERE A.HSCFCLevel='20'"></asp:SqlDataSource>
                                                        <asp:Repeater ID="Rpt_HSCForumClassB" runat="server" DataSourceID="SDS_HSCForumClassB" OnItemDataBound="Rpt_ItemDataBound">
                                                            <ItemTemplate>
                                                                <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                                <tr>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:LinkButton ID="LBtn_Save" runat="server" CssClass="btn btn-sm btn-outline-primary" ToolTip="儲存" Visible="false" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Save_Click"><i class="ti-check"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success" ToolTip="編輯" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Edit_Click"><i class="icon-pencil"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Del" runat="server" class="btn btn-sm btn-outline-danger js-sweetalert" ToolTip="停用" Btmessage="確定要停用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Del_Click"><i class="ti-na"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Upload" runat="server" class="btn btn-sm btn-outline-info js-sweetalert" ToolTip="啟用" Btmessage="確定要啟用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Upload_Click"><i class="ti-upload"></i></asp:LinkButton>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HSCFCNameA" runat="server" CssClass="p-l-10" Text='<%# Eval("HSCFCNameA") %>'></asp:Label>
                                                                        <asp:Label ID="LB_HSCFCMasterA" runat="server" Text='<%# Eval("HSCFCMaster") %>' Visible="false"></asp:Label>
                                                                        <asp:DropDownList ID="DDL_HSCFCMasterA" runat="server" class="form-control js-example-basic-single" name="state" Style="width: 100%" DataSourceID="SDS_HSCFCMasterA" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true" Visible="false">
                                                                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HSCFCNameB" runat="server" CssClass="p-l-10" Text='<%# Eval("HSCFCNameB") %>'></asp:Label>
                                                                        <asp:TextBox ID="TB_HSCFCNameB" runat="server" Text='<%# Eval("HSCFCNameB") %>' class="form-control" Visible="false" BorderColor="#00c292" AutoComplete="off"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <div class="badge badge-default" id="Status" runat="server">
                                                                            <asp:Label ID="LB_Status" runat="server" Text='<%# Eval("HStatus") %>'></asp:Label><!--狀態-->
                                                                        </div>
                                                                    </td>
                                                                </tr>

                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>

                                            </div>
                                        </div>
                                        <!--------------------==================討論區次類別 END========================--------------->


                                        <!--------------------==================討論區名稱 START========================--------------->
                                        <div class="tab-pane fade" id="Div_HSCForumClassC" runat="server" role="tabpanel">
                                            <div class="table-responsive">
                                                <asp:SqlDataSource ID="SDS_HSCFCMasterB" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCFCName, HStatus FROM HSCForumClass WHERE HSCFCLevel='20' AND HStatus='1'"></asp:SqlDataSource>
                                                <table class="table table-hover" style="width: 100%">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 3%" class="text-center">No</th>
                                                            <th class="text-center" style="width: 8%">執行</th>
                                                            <th style="width: 10%">討論區次類別</th>
                                                            <th style="width: 25%">討論區名稱</th>
                                                            <th style="width: 25%">狀態</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>

                                                        <tr runat="server" id="Tr_HSCForumClassC_Add">
                                                            <td class="text-center"></td>
                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_HSCForumClassC_Add" runat="server" class="btn btn-sm btn-outline-primary" ToolTip="新增" OnClick="LBtn_Add_Click"><i class="ti-plus text-primary"></i></asp:LinkButton>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="DDL_HSCFCMasterB" runat="server" CssClass="form-control  js-example-basic-single" DataSourceID="SDS_HSCFCMasterB" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true">
                                                                    <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TB_HSCFCNameC" runat="server" class="form-control" AutoComplete="off"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <asp:SqlDataSource ID="SDS_HSCForumClassC" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT A.HID, A.HSCFCMaster AS HSCFCMasterB, B.HSCFCName AS HSCFCNameB, A.HSCFCName AS HSCFCNameC, A.HStatus FROM HSCForumClass A INNER JOIN HSCForumClass B ON A.HSCFCMaster=B.HID WHERE A.HSCFCLevel='30'"></asp:SqlDataSource>
                                                        <asp:Repeater ID="Rpt_HSCForumClassC" runat="server" DataSourceID="SDS_HSCForumClassC" OnItemDataBound="Rpt_ItemDataBound">
                                                            <ItemTemplate>
                                                                <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                                <tr>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:LinkButton ID="LBtn_Save" runat="server" CssClass="btn btn-sm btn-outline-primary" ToolTip="儲存" Visible="false" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Save_Click"><i class="ti-check"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success" ToolTip="編輯" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Edit_Click"><i class="icon-pencil"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Del" runat="server" class="btn btn-sm btn-outline-danger js-sweetalert" ToolTip="停用" Btmessage="確定要停用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Del_Click"><i class="ti-na"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Upload" runat="server" class="btn btn-sm btn-outline-info js-sweetalert" ToolTip="啟用" Btmessage="確定要啟用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Upload_Click"><i class="ti-upload"></i></asp:LinkButton>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HSCFCNameB" runat="server" CssClass="p-l-10" Text='<%# Eval("HSCFCNameB") %>'></asp:Label>
                                                                        <asp:Label ID="LB_HSCFCMasterB" runat="server" CssClass="p-l-10" Text='<%# Eval("HSCFCMasterB") %>' Visible="false"></asp:Label>
                                                                        <asp:DropDownList ID="DDL_HSCFCMasterB" runat="server" class="form-control js-example-basic-single" name="state" Style="width: 100%" DataSourceID="SDS_HSCFCMasterB" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true" Visible="false">
                                                                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HSCFCNameC" runat="server" CssClass="p-l-10" Text='<%# Eval("HSCFCNameC") %>'></asp:Label>
                                                                        <asp:TextBox ID="TB_HSCFCNameC" runat="server" Text='<%# Eval("HSCFCNameC") %>' class="form-control" Visible="false" BorderColor="#00c292" AutoComplete="off"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <div class="badge badge-default" id="Status" runat="server">
                                                                            <asp:Label ID="LB_Status" runat="server" Text='<%# Eval("HStatus") %>'></asp:Label><!--狀態-->
                                                                        </div>
                                                                    </td>
                                                                </tr>

                                                            </ItemTemplate>
                                                        </asp:Repeater>

                                                    </tbody>
                                                </table>


                                            </div>
                                        </div>
                                        <!--------------------==================討論區名稱 END========================--------------->

                                        <!--------------------==================熱門標籤 START========================--------------->
                                        <div class="tab-pane fade" id="Div_HSCHotHashTag" runat="server" role="tabpanel">
                                            <div class="table-responsive">

                                                <table class="table table-hover" style="width: 100%">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 3%" class="text-center">No</th>
                                                            <th class="text-center" style="width: 8%">執行</th>
                                                            <th style="width: 25%">熱門標籤</th>
                                                            <th style="width: 25%">狀態</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr runat="server" id="Tr_HSCHotHashTag_Add">
                                                            <td class="text-center"></td>
                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_HSCHHashTag_Add" runat="server" class="btn btn-sm btn-outline-primary" ToolTip="新增" OnClick="LBtn_Add_Click"><i class="ti-plus text-primary"></i></asp:LinkButton>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TB_HSCHHashTag" runat="server" class="form-control" AutoComplete="off"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <asp:SqlDataSource ID="SDS_HSCHotHashTag" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCHHashTag, HStatus FROM HSCHotHashTag"></asp:SqlDataSource>
                                                        <asp:Repeater ID="Rpt_HSCHotHashTag" runat="server" DataSourceID="SDS_HSCHotHashTag" OnItemDataBound="Rpt_ItemDataBound">
                                                            <ItemTemplate>
                                                                <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                                <tr>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:LinkButton ID="LBtn_Save" runat="server" CssClass="btn btn-sm btn-outline-primary" ToolTip="儲存" Visible="false" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Save_Click"><i class="ti-check"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success" ToolTip="編輯" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Edit_Click"><i class="icon-pencil"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Del" runat="server" class="btn btn-sm btn-outline-danger js-sweetalert" ToolTip="停用" Btmessage="確定要停用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Del_Click"><i class="ti-na"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Upload" runat="server" class="btn btn-sm btn-outline-info js-sweetalert" ToolTip="啟用" Btmessage="確定要啟用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Upload_Click"><i class="ti-upload"></i></asp:LinkButton>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HSCHHashTag" runat="server" CssClass="p-l-10" Text='<%# Eval("HSCHHashTag") %>'></asp:Label>
                                                                        <asp:TextBox ID="TB_HSCHHashTag" runat="server" Text='<%# Eval("HSCHHashTag") %>' class="form-control" Visible="false" BorderColor="#00c292" AutoComplete="off"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <div class="badge badge-default" id="Status" runat="server">
                                                                            <asp:Label ID="LB_Status" runat="server" Text='<%# Eval("HStatus") %>'></asp:Label><!--狀態-->
                                                                        </div>
                                                                    </td>
                                                                </tr>

                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>

                                            </div>
                                        </div>
                                        <!--------------------==================熱門標籤 END========================--------------->

                                    </div>


                                </div>
                            </div>
                        </div>
                    </div>

                    <%--</asp:Panel>--%>
                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>




    <script src="js/jquery-3.3.1.min.js"></script>
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
    <!--select2-->
    <script src="js/select2.min.js"></script>
    <!--Custom JavaScript -->
    <script src="js/_custom.js"></script>
    <!--sumoselect-->
    <script src="js/jquery.sumoselect.min.js"></script>
    <!--Nestable js -->
    <script src="assets/node_modules/nestable/jquery.nestable.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            // Nestable
            var updateOutput = function (e) {
                var list = e.length ? e : $(e.target),
                    output = list.data('output');
                if (window.JSON) {
                    output.val(window.JSON.stringify(list.nestable('serialize'))); //, null, 2));
                } else {
                    output.val('JSON browser support required for this demo.');
                }
            };


            $('#nestable').nestable({
                group: 1
            }).on('change', updateOutput);

            updateOutput($('#nestable').data('output', $('#nestable-output')));

            $('#nestable-menu').on('click', function (e) {
                var target = $(e.target),
                    action = target.data('action');
                if (action === 'expand-all') {
                    $('.dd').nestable('expandAll');
                }
                if (action === 'collapse-all') {
                    $('.dd').nestable('collapseAll');
                }
            });

            $('#nestable-menu').nestable();
        });
    </script>
    <script>
        $('.js-example-basic-single').select2({ closeOnSelect: true, });
    </script>
</asp:Content>

