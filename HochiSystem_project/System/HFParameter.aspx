<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HFParameter.aspx.cs" Inherits="System_HFParameter" %>

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
                                <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>捐款參數設定</h2>
                                <ul class="breadcrumb">
                                    <li class="breadcrumb-item"><a href="HFParameter.aspx"><i class="icon-home"></i></a></li>
                                    <li class="breadcrumb-item">捐款參數設定</li>
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
                                    <div class="text-right">
                                        <a class="btn btn-info" href="javascript:void(0);" data-toggle="modal" data-target="#info">操作說明</a>
                                    </div>
                                    <asp:Label ID="LB_NavTab" runat="server" Text="1" Visible="false"></asp:Label><!--目前在哪個tab-->
                                    <ul class="nav nav-tabs" id="ParaTab" role="tablist">
                                        <asp:SqlDataSource ID="SDS_Tag" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HName_TW, HStatus FROM HFParmTab WHERE HStatus = 1"></asp:SqlDataSource>
                                        <asp:Repeater ID="RPT_Tag" runat="server" DataSourceID="SDS_Tag">
                                            <ItemTemplate>
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

                                        <!--------------------==================捐款用途 START========================--------------->
                                        <div class="tab-pane fade" id="Div_HDPurpose" runat="server" role="tabpanel">
                                            <div class="table-responsive">

                                                <table class="table table-hover" style="width: 100%">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 3%" class="text-center">No</th>
                                                            <th class="text-center" style="width: 8%">執行</th>
                                                            <th style="width: 12%"><span class="text-danger">*</span>捐款用途代碼
                                                                 <span class="" style="font-size: 0.9rem;">(限兩碼數字)</span></th>
                                                            <th style="width: 25%">捐款用途</th>
                                                            <th style="width: 25%">狀態</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr runat="server" id="Tr_HDPurpose_Add">
                                                            <td class="text-center"></td>


                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_HDPurposeAdd" runat="server" class="btn btn-sm btn-outline-primary" ToolTip="新增" OnClick="LBtn_Add_Click"><i class="ti-plus text-primary"></i></asp:LinkButton>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TB_HDTypeCode" runat="server" class="form-control" AutoComplete="off" Placeholder="限輸入兩碼數字"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TB_HDPurpose" runat="server" class="form-control" AutoComplete="off"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <asp:SqlDataSource ID="SDS_HDPurpose" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HDTypeCode, HDPurpose, HStatus FROM HDPurpose ORDER BY HStatus DESC"></asp:SqlDataSource>
                                                        <asp:Repeater ID="Rpt_HDPurpose" runat="server" DataSourceID="SDS_HDPurpose" OnItemDataBound="RPT_ItemDataBound">
                                                            <ItemTemplate>
                                                                <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                                <tr>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:LinkButton ID="LBtn_Save" runat="server" CssClass="btn btn-sm btn-outline-primary" ToolTip="儲存" Visible="false" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Save_Click"><i class="ti-check"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success" ToolTip="編輯" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Edit_Click"><i class="icon-pencil"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Del" runat="server" class="btn btn-sm btn-outline-danger js-sweetalert" ToolTip="停用" Btmessage="確定要停用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' CommandArgument='<%# Eval("HID") %>' Visible="false" OnClick="LBtn_Del_Click"><i class="ti-na"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Upload" runat="server" class="btn btn-sm btn-outline-info js-sweetalert" ToolTip="啟用" CommandArgument='<%# Eval("HID") %>' Btmessage="確定要啟用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" OnClick="LBtn_Upload_Click"><i class="ti-upload"></i></asp:LinkButton>
                                                                    </td>
                                                                    <td class="text-left">
                                                                        <asp:Label ID="LB_HDTypeCode" runat="server" CssClass="p-l-10" Text='<%# Eval("HDTypeCode") %>'></asp:Label>
                                                                        <asp:TextBox ID="TB_HDTypeCode" runat="server" CssClass="form-control" AutoComplete="Off" Visible="false" Text='<%# Eval("HDTypeCode") %>' MaxLength="2" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" PlaceHolder="限輸入兩碼數字"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HDPurpose" runat="server" CssClass="p-l-10" Text='<%# Eval("HDPurpose") %>'></asp:Label>
                                                                        <asp:TextBox ID="TB_HDPurpose" runat="server" Text='<%# Eval("HDPurpose") %>' class="form-control" Visible="false" BorderColor="#00c292" AutoComplete="off"></asp:TextBox>
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
                                        <!--------------------==================捐款用途 END========================--------------->

                                        <!--------------------==================捐款項目 START========================--------------->
                                        <div class="tab-pane fade" id="Div_HDItem" runat="server" role="tabpanel">
                                            <div class="table-responsive">
                                                <asp:SqlDataSource ID="SDS_HDPurpose1" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HDPurpose FROM HDPurpose WHERE HStatus='1'"></asp:SqlDataSource>
                                                <table class="table table-hover" style="width: 100%">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 3%" class="text-center">No</th>
                                                            <th class="text-center" style="width: 8%">執行</th>

                                                            <th style="width: 25%"><span class="text-danger">*</span>捐款用途</th>
                                                            <th style="width: 25%"><span class="text-danger">*</span>捐款項目</th>
                                                            <th style="width: 10%">新增定期定額</th>
                                                            <th style="width: 10%">狀態</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr runat="server" id="Tr_HDItem_Add">
                                                            <td class="text-center"></td>
                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_HDItemAdd" runat="server" class="btn btn-sm btn-outline-primary" ToolTip="新增" OnClick="LBtn_Add_Click"><i class="ti-plus text-primary"></i></asp:LinkButton>
                                                            </td>
                                                            <td class="text-center d-none">
                                                                <asp:TextBox ID="TB_HDItemCode" runat="server" CssClass="form-control" AutoComplete="Off" MaxLength="2" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" PlaceHolder="限輸入數字兩碼"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="DDL_HDPurpose" runat="server" class="form-control js-example-basic-single" name="state" Style="width: 100%" DataSourceID="SDS_HDPurpose1" DataTextField="HDPurpose" DataValueField="HID" AppendDataBoundItems="true" Visible="true">
                                                                    <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TB_HDItem" runat="server" class="form-control" AutoComplete="off"></asp:TextBox>
                                                            </td>
                                                            <td></td>
                                                        </tr>
                                                        <asp:SqlDataSource ID="SDS_HDonationItem" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT A.HID, A.HDPurposeID, B.HDPurpose, A.HDItemCode, A.HDItem, A.HRemark, A.HStatus FROM HDonationItem A INNER JOIN HDPurpose B ON A.HDPurposeID=B.HID ORDER BY A.HStatus DESC"></asp:SqlDataSource>
                                                        <asp:Repeater ID="Rpt_HDonationItem" runat="server" DataSourceID="SDS_HDonationItem" OnItemDataBound="RPT_ItemDataBound">
                                                            <ItemTemplate>
                                                                <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                                <tr>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:LinkButton ID="LBtn_Save" runat="server" CssClass="btn btn-sm btn-outline-primary" ToolTip="儲存" Visible="false" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Save_Click"><i class="ti-check"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success" ToolTip="編輯" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Edit_Click"><i class="icon-pencil"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Del" runat="server" class="btn btn-sm btn-outline-danger js-sweetalert" ToolTip="停用" Btmessage="確定要停用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' CommandArgument='<%# Eval("HID") %>' Visible="false" OnClick="LBtn_Del_Click"><i class="ti-na"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Upload" runat="server" class="btn btn-sm btn-outline-info js-sweetalert" ToolTip="啟用" CommandArgument='<%# Eval("HID") %>' Btmessage="確定要啟用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" OnClick="LBtn_Upload_Click"><i class="ti-upload"></i></asp:LinkButton>
                                                                    </td>

                                                                    <td>
                                                                        <asp:Label ID="LB_HDPurpose" runat="server" CssClass="p-l-10" Text='<%# Eval("HDPurpose") %>'></asp:Label>
                                                                        <asp:Label ID="LB_HDPurposeID" runat="server" CssClass="p-l-10" Text='<%# Eval("HDPurposeID") %>' Visible="false"></asp:Label>
                                                                        <asp:DropDownList ID="DDL_HDPurpose" runat="server" class="form-control js-example-basic-single" name="state" Style="width: 100%" DataSourceID="SDS_HDPurpose1" DataTextField="HDPurpose" DataValueField="HID" AppendDataBoundItems="true" Visible="false">
                                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HDItem" runat="server" CssClass="p-l-10" Text='<%# Eval("HDItem") %>'></asp:Label>
                                                                        <asp:TextBox ID="TB_HDItem" runat="server" Text='<%# Eval("HDItem") %>' class="form-control" Visible="false" BorderColor="#00c292" AutoComplete="off"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:HyperLink ID="HL_HCCRegular" runat="server" Target="_blank">新增定期定額</asp:HyperLink>
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
                                        <!--------------------==================捐款項目 END========================--------------->

                                        <!--------------------==================前台信用卡授權設定 START========================--------------->
                                        <div class="tab-pane fade" id="Div_HCCPeriodYN" runat="server" role="tabpanel" visible="false">
                                            <div class="row col-md-12">
                                                <div class="form-group  col-md-6">
                                                    <label class="control-label">紙本信用卡授權開關</label>
                                                    <div class="d-flex col-md-6 align-items-center">
                                                        <asp:DropDownList ID="DDL_HCCPeriodYN" runat="server" class="form-control js-example-basic-single" name="state" Style="width: 100%" Enabled="true">
                                                            <asp:ListItem Value="0">關閉</asp:ListItem>
                                                            <asp:ListItem Value="1">開啟</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <div class="col-md-6">
                                                            <asp:Button ID="Btn_HCCPYNModify" runat="server" Text="修改" CssClass="btn btn-success" OnClick="Btn_HCCPYNModify_Click" Visible="false" />
                                                            <asp:Button ID="Btn_HCCPYNSubmit" runat="server" Text="儲存" CssClass="btn btn-success" OnClick="Btn_HCCPYNSubmit_Click" />
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                        <!--------------------==================前台信用卡授權設定 END========================--------------->


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

    <!-- Modal 操作說明 START-->
    <div class="modal fade" id="info" tabindex="-1" role="dialog" aria-labelledby="info" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title" id="exampleModalLongTitle">操作說明</h5>
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
    <!--select2-->
    <script src="js/select2.min.js"></script>
    <!--Custom JavaScript -->
    <script src="js/_custom.js"></script>
    <!--sumoselect-->
    <script src="js/jquery.sumoselect.min.js"></script>


</asp:Content>

