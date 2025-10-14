<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HSCModeratorSetting.aspx.cs" Inherits="System_HSCModeratorSetting" %>

<%--分頁--%>
<%@ Register TagPrefix="Page" TagName="Paging" Src="~/System/Paging_backend.ascx" %>

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

            <asp:Panel ID="Panel_List" runat="server">
                <!-- ============================================================== -->
                <div class="block-header">
                    <div class="row">
                        <div class="col-lg-3 col-md-12 col-sm-4">
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>討論區管理員設定</h2>
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item active">討論區管理員設定</li>
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
                                    <div class="col-md-12 col-lg-2 col-xlg-2 d-none">
                                        <div class="box text-left">
                                            <button id="Btn_Add" runat="server" type="button" class="btn btn-outline-info" onclick="window.location.href='HSCModeratorSetting_Add.aspx';"><i class="fa fa-plus"></i>新增討論區管理員</button>
                                        </div>
                                    </div>
                                </div>


                                <div class="row ">
                                    <div class="col-md-10 col-lg-12 col-xlg-12  excel_outer">
                                        <div class="box form-group row m-b-0">
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TB_Search" runat="server" class="form-control" placeholder="請輸入討論區名稱" AutoComplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-md-2">
                                                <asp:LinkButton ID="LBtn_Search" runat="server" class="btn btn-outline-secondary" OnClick="LBtn_Search_Click"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                                <asp:LinkButton ID="LBtn_SearchCancel" runat="server" class="btn btn-outline-secondary m-l-10" OnClick="LBtn_SearchCancel_Click"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>

                                            </div>

                                        </div>
                                    </div>
                                </div>

                                <div class="table-responsive">
                                    <table class="table table-hover m-t-20">
                                        <thead>
                                            <tr>
                                                <th class="text-center" style="width: 10%">執行</th>
                                                <th class="text-center" style="width: 5%">No</th>
                                                <th style="width: 45%">討論區名稱</th>
                                                <th class="text-center" style="width: 30%">管理員人數</th>
                                                <th class="text-center" style="width: 10%">討論區狀態</th>
                                                <%-- <th style="width: 45%">管理員姓名</th>--%>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:SqlDataSource ID="SDS_HSCModeratorSetting" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT  b.HID, b.HSCFCName, Count(c.HUserName) AS HTotalPeople, b.HStatus FROM HSCModeratorSetting AS a Cross Apply SPLIT(a.HMemberID, ',') AS d RIGHT JOIN HSCForumClass as b on b.HID=a.HSCForumClassID LEFT JOIN MemberList as c on d.value=c.HID WHERE  b.HSCFCLevel='30' GROUP BY  b.HID, b.HSCFCName, b.HStatus ORDER BY b.HID ASC"></asp:SqlDataSource>
                                            <%--b.HStatus=1 AND--%>
                                            <asp:Repeater ID="Rpt_HSCModeratorSetting" runat="server" OnItemDataBound="Rpt_HSCModeratorSetting_ItemDataBound">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="text-center">
                                                            <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success mr-2" ToolTip="編輯" OnClick="LBtn_Edit_Click" CommandArgument='<%# Eval("HID") %>'><i class="icon-pencil"></i></asp:LinkButton>
                                                        </td>
                                                        <td style="text-align: center">
                                                            <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex + 1%>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HSCFCName" runat="server" Text='<%# Eval("HSCFCName") %>'></asp:Label>
                                                        </td>
                                                        <td style="text-align: center">
                                                            <asp:Label ID="LB_HTotalPeople" runat="server" Text='<%# Eval("HTotalPeople") %>'></asp:Label>
                                                        </td>
                                                        <td style="text-align: center">
                                                            <div class="badge badge-default" id="Status" runat="server">
                                                                <asp:Label ID="LB_Status" runat="server" Text='<%# Eval("HStatus") %>'></asp:Label><!--狀態-->
                                                            </div>
                                                        </td>
                                                    </tr>

                                                </ItemTemplate>
                                            </asp:Repeater>

                                        </tbody>
                                    </table>
                                    <!------------------分頁功能開始------------------>
                                    <nav class="box text-right">
                                        <Page:Paging runat="server" ID="Pg_Paging" />
                                    </nav>
                                    <!------------------分頁功能結束------------------>

                                </div>

                            </div>
                        </div>
                    </div>

                </div>
            </asp:Panel>





            <asp:Panel ID="Panel_Edit" runat="server" Visible="false">

                <asp:Label ID="LB_HUsualTask" runat="server" Text="" Visible="false"></asp:Label>

                <div class="block-header">
                    <div class="row">
                        <div class="col-lg-3 col-md-12 col-sm-4">
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>討論區管理員設定</h2>
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item"><a href="HSCModeratorSetting.aspx">討論區管理員設定</a></li>
                                <li class="breadcrumb-item active">編輯討論區管理員設定</li>
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


                                <asp:Label ID="LB_HID" runat="server" Text="" Visible="false"></asp:Label>
                                <div class="row clearfix">

                                    <div class="col-md-6 col-sm-12">
                                        <label>討論區名稱</label>
                                        <div class="form-group">
                                            <asp:TextBox ID="TB_HSCFCName" runat="server" class="form-control" placeholder="" AutoComplete="off" Text="" Enabled="false"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <asp:Panel ID="Panel_MList" runat="server" Visible="false">
                                    <!--選人-->
                                    <label>管理員名單</label>


                                    <div class="row d-none">
                                        <div class="col-md-10 col-lg-12 col-xlg-12">
                                            <div class="box form-group row m-b-0">
                                                <div class="col-md-2">
                                                    <asp:TextBox ID="TB_JoinSearch" runat="server" class="form-control" placeholder="請輸入姓名" AutoComplete="off"></asp:TextBox>
                                                </div>
                                                <div class="col-md-4">
                                                    <asp:LinkButton ID="LBtn_JoinSearch" runat="server" class="btn btn-outline-secondary"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                                    <asp:LinkButton ID="LBtn_JoinSearchCancel" runat="server" class="btn btn-outline-secondary m-l-10"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="table-responsive col-6">
                                        <table class="table table-hover">
                                            <thead>
                                                <tr>
                                                    <th class="text-center" style="width: 10%">執行</th>
                                                    <th class="text-center" style="width: 25%">No</th>
                                                    <th style="width: 50%">姓名</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td style="text-align: center;">
                                                        <asp:LinkButton ID="LBtn_HMemberAdd" runat="server" OnClick="LBtn_HMemberAdd_Click"><i class="ti-plus text-info"></i></asp:LinkButton>
                                                    </td>
                                                    <td class="text-center"></td>
                                                    <td>
                                                        <asp:SqlDataSource ID="SDS_HMemberID" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT a.HID, (b.HArea+'/'+a.HPeriod+' '+a.HUserName) AS HUserName, a.HStatus FROM HMember AS a LEFT JOIN HArea AS b ON a.HAreaID=b.HID WHERE a.HStatus='1'"></asp:SqlDataSource>
                                                        <asp:DropDownList ID="DDL_HMemberID" runat="server" class="form-control js-example-basic-single" DataSourceID="SDS_HMemberID" DataTextField="HUserName" DataValueField="HID" AppendDataBoundItems="true">
                                                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>

                                                <asp:Label ID="LB_AllHMemberID" runat="server" Text="" Visible="false"></asp:Label>

                                                <asp:SqlDataSource ID="SDS_HMemberList" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_HMemberList" runat="server" DataSourceID="SDS_HMemberList">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td style="text-align: center;">
                                                                <asp:LinkButton ID="LBtn_HMemberDel" runat="server" CommandArgument='<%# Eval("HMemberID") %>' OnClick="LBtn_HMemberDel_Click" Btmessage="確定要刪除嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))'><i class="ti-close text-danger"></i></asp:LinkButton>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HMemberID" runat="server" Text='<%# Eval("HMemberID") %>' Visible="false"></asp:Label>
                                                                <asp:Label ID="LB_HUserName" runat="server" Text='<%# Eval("HUserName") %>'></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>


                                            </tbody>
                                        </table>
                                        <!------------------分頁功能開始------------------>
                                        <div class="box text-center">
                                            <%--<Page:Paging runat="server" ID="Pg_MList" />--%>
                                        </div>
                                        <!------------------分頁功能結束------------------>
                                    </div>



                                    <div class="text-center">
                                        <asp:LinkButton ID="LBtn_Submit" runat="server" Text="儲存" CssClass="btn btn-success" Visible="false" />
                                        <asp:LinkButton ID="LBtn_Return" runat="server" Text="回上一頁" CssClass="btn btn-secondary" OnClick="LBtn_Return_Click" />

                                    </div>
                                </asp:Panel>





                            </div>
                        </div>
                    </div>
                </div>


            </asp:Panel>

        </div>
        <!-- ============================================================== -->
        <!-- End Container  -->
        <!-- ============================================================== -->
    </div>
    <!-- ============================================================== -->
    <!-- End Page wrapper  -->
    <!-- ============================================================== -->



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
    <!--select2-->
    <script src="js/select2.min.js"></script>
    <!--Custom JavaScript -->
    <script src="dist/js/custom.min.js"></script>

    <script>
        $(function () {
            //單選
            $('.js-example-basic-single').select2();
        });
    </script>



</asp:Content>

