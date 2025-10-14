<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HSCCourseSetting.aspx.cs" Inherits="System_HSCCourseSetting" %>

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
            <!-- ============================================================== -->
            <!-- Bread crumb and right sidebar toggle -->
            <!-- ============================================================== -->
            <div class="block-header">
                <div class="row">
                    <div class="col-lg-6 col-md-12 col-sm-12">
                        <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>課程討論區與主題設定</h2>
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                            <li class="breadcrumb-item active">課程討論區與主題設定</li>
                        </ul>
                    </div>
                </div>
            </div>
            <!-- ============================================================== -->
            <!-- End Bread crumb and right sidebar toggle -->
            <!-- ============================================================== -->
            <!-- ============================================================== -->
            <!-- Start Page Content -->
            <!-- ============================================================== -->
            <!-- Row -->

            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-12">
                            <div class="card">
                                <div class="body">
                                    <!-- Row -->
                                    <div class="row m-t-10 p-l-10 mb-2">
                                        <div class="col-md-10 col-lg-12 col-xlg-12">
                                            <div class="form-group row m-b-0">
                                                <div class="col-md-2 p-l-0">
                                                    <asp:DropDownList ID="DDL_SHSCForumClassID" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCForumClassID" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">選擇討論區名稱</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-md-2 p-l-0">
                                                    <asp:DropDownList ID="DDL_SHCTemplateID" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HCTemplateID" DataTextField="HTemplateName" DataValueField="HID" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">選擇課程範本</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-md-4">
                                                    <asp:LinkButton ID="LBtn_Search" runat="server" class="btn btn-outline-secondary" OnClick="LBtn_Search_Click"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                                    <asp:LinkButton ID="LBtn_SearchCancel" runat="server" class="btn btn-outline-secondary m-l-10" OnClick="LBtn_SearchCancel_Click"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="table-responsive" style="clear: both;">
                                        <asp:SqlDataSource ID="SDS_HCTemplateID" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HTemplateName  FROM HCourse_T WHERE HStatus=1"></asp:SqlDataSource>
                                        <asp:SqlDataSource ID="SDS_HSCForumClassID" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCFCName FROM HSCForumClass WHERE HStatus=1 AND HSCFCLevel='30'"></asp:SqlDataSource>
                                        <table class="table table-hover">
                                            <thead>
                                                <tr>
                                                    <th class="text-center font-weight-bold" style="width: 3%">No</th>
                                                    <th class="text-center font-weight-bold" style="width: 10%">執行</th>
                                                    <th class="font-weight-bold" style="width: 20%">討論區名稱</th>
                                                    <th class="font-weight-bold" style="width: 30%">課程範本</th>
                                                    <th class="font-weight-bold" style="width: 20%">主題名稱產生方式</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr runat="server" id="tr_add">
                                                    <td class="text-center"></td>
                                                    <td style="text-align: center;">
                                                        <asp:LinkButton ID="LBtn_Add" runat="server" ToolTip="新增" class="btn btn-sm  btn-outline-success" OnClick="LBtn_Add_Click"><i class="ti-plus text-success"></i></asp:LinkButton>
                                                    </td>
                                                    <td>

                                                        <asp:DropDownList ID="DDL_HSCForumClassID" runat="server" CssClass="form-control  js-example-basic-single" DataSourceID="SDS_HSCForumClassID" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true">
                                                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="DDL_HCTemplateID" runat="server" CssClass="form-control  js-example-basic-single" DataSourceID="SDS_HCTemplateID" DataTextField="HTemplateName" DataValueField="HID" AppendDataBoundItems="true">
                                                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="DDL_HTopicNameType" runat="server" CssClass="form-control  js-example-basic-single">
                                                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            <asp:ListItem Value="1">依課程名稱</asp:ListItem>
                                                            <asp:ListItem Value="2">依上課日期</asp:ListItem>
                                                            <asp:ListItem Value="3" Enabled="false">不依上課地點</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>

                                                </tr>
                                                <asp:SqlDataSource ID="SDS_HSCCourseSetting" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT a.HID, a.HCTemplateID, b.HTemplateName, a.HSCForumClassID, c.HSCFCName, a.HTopicNameType, a.HStatus FROM HSCCTopicSetting AS a INNER JOIN HCourse_T AS b ON a.HCTemplateID=b.HID INNER JOIN HSCForumClass AS c ON a.HSCForumClassID=c.HID WHERE a.HStatus=@HStatus">
                                                    <SelectParameters>
                                                        <asp:Parameter Name="HStatus" Type="Int32" DefaultValue="1" />
                                                    </SelectParameters>
                                                </asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_HSCCourseSetting" runat="server" DataSourceID="SDS_HSCCourseSetting" OnItemDataBound="Rpt_HSCCourseSetting_ItemDataBound">
                                                    <ItemTemplate>
                                                        <tr runat="server" id="tr1">
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                            </td>
                                                            <td style="text-align: center;">
                                                                <asp:LinkButton ID="LBtn_Del" runat="server" ToolTip="刪除" class="btn btn-sm  btn-outline-danger" Btmessage="確定要刪除嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Del_Click"><i class="icon-trash "></i></asp:LinkButton>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HSCForumClassID" runat="server"  Text='<%# Eval("HSCForumClassID") %>' Visible="false"></asp:Label>
                                                                <asp:DropDownList ID="DDL_HSCForumClassID" runat="server" CssClass="form-control  js-example-basic-single" DataSourceID="SDS_HSCForumClassID" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true" Enabled="false">
                                                                    <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HCTemplateID" runat="server" Text='<%# Eval("HCTemplateID") %>' Visible="false"></asp:Label>
                                                                <asp:DropDownList ID="DDL_HCTemplateID" runat="server" CssClass="form-control  js-example-basic-single" DataSourceID="SDS_HCTemplateID" DataTextField="HTemplateName" DataValueField="HID" AppendDataBoundItems="true" Enabled="false">
                                                                    <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HTopicNameType" runat="server"  Text='<%# Eval("HTopicNameType") %>' Visible="false"></asp:Label>
                                                                <asp:DropDownList ID="DDL_HTopicNameType" runat="server" CssClass="form-control  js-example-basic-single" Enabled="false">
                                                                    <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                    <asp:ListItem Value="1">依課程名稱</asp:ListItem>
                                                                    <asp:ListItem Value="2">依上課日期</asp:ListItem>
                                                                    <asp:ListItem Value="3" Enabled="false">不依上課地點</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>


                                    <div class="text-center d-none">
                                        <asp:Button ID="Btn_Submit" runat="server" Text="儲存" class="btn btn-success" />
                                        <asp:Button ID="Btn_Cancel" runat="server" Text="取消" class="btn btn-inverse" OnClientClick='return confirm(this.getAttribute("btmessage"))' btmessage="確定要取消嗎？沒有儲存的内容將不會變更" />

                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>


            <!-- End Row -->
            <!-- ============================================================== -->
            <!-- End PAge Content -->
            <!-- ============================================================== -->
        </div>
        <!-- ============================================================== -->
        <!-- End Container fluid  -->
        <!-- ============================================================== -->
    </div>
    <!-- ============================================================== -->
    <!-- End Page wrapper  -->
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

    <!--select2-->
    <script src="js/select2.min.js"></script>

    <script>
        $('.js-example-basic-single').select2({ closeOnSelect: true, });
    </script>

</asp:Content>

