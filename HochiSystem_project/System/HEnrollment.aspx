<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HEnrollment.aspx.cs" Inherits="System_HEnrollment" %>

<%@ Register Assembly="MyWebControls" Namespace="MyWebControls" TagPrefix="cc1" %>

<%--分頁--%>
<%@ Register TagPrefix="Page" TagName="Paging" Src="~/System/Paging_backend.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- chartist CSS -->
    <link href="css/morris.css" rel="stylesheet" />
    <link href="dist/css/pages/dashboard1.css" rel="stylesheet" />
    <style>
        .table th, .table td {
            padding: 0.3rem 3px;
        }

            .table td span {
                word-break: unset !important;
                overflow-wrap: unset;
                white-space: nowrap;
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
            <!-- ============================================================== -->

            <div class="block-header">
                <div class="row">
                    <div class="col-lg-3 col-md-12 col-sm-4">
                        <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>單一課程報名總名單</h2>
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                            <li class="breadcrumb-item active">單一課程報名總名單</li>
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

                            <asp:Panel ID="Panel_Search" runat="server">
                                <div class="row">
                                    <div class="col-md-12 col-lg-12 col-xlg-12">
                                        <div class="box form-group row m-b-0">
                                            <div class="col-md-3">
                                                <asp:TextBox ID="TB_Search" runat="server" class="form-control" placeholder="請輸入課程名稱" AutoComplete="off"></asp:TextBox>
                                            </div>

                                            <div class="col-md-2">
                                                <asp:DropDownList ID="DDL_HOCPlace" runat="server" class="form-control js-example-basic-single" Style="width: 100%; height: 38px;" placeholder="請選擇上課地點">
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-md-2">
                                                <asp:TextBox ID="TB_SearchDate" runat="server" class="form-control daterange" placeholder="選擇課程日期區間" AutoComplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-md-2 d-none">
                                                <asp:DropDownList ID="DDL_Type" runat="server" class="form-control js-example-basic-single" Style="width: 100%; height: 38px;" placeholder="請選擇身分別">
                                                    <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                    <asp:ListItem Value="1">參班(一般)</asp:ListItem>
                                                    <asp:ListItem Value="2">參班(學青)</asp:ListItem>
                                                    <asp:ListItem Value="3">參班(經濟困難)</asp:ListItem>
                                                    <asp:ListItem Value="4">參班(清修人士)</asp:ListItem>
                                                    <asp:ListItem Value="5">不參班(純護持)</asp:ListItem>
                                                    <asp:ListItem Value="6">一對一護持者</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-md-3 excel_outer" style="max-width: 20%; display: flex; align-items: center;">
                                                <asp:LinkButton ID="LBtn_Search" runat="server" OnClick="LBtn_Search_Click" class="btn btn-outline-secondary"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                                <asp:LinkButton ID="LBtn_SearchCancel" runat="server" OnClick="LBtn_SearchCancel_Click" class="btn btn-outline-secondary m-l-10"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>


                            <asp:Panel ID="Panel_CourseList" runat="server" Visible="true">
                                <div class="mt-3">
                                    <h3 class="text-center mb-1"></h3>
                                    <div class="table-responsive">
                                        <table id="demo-foo-accordion" class="table table-bordered m-b-0 table-hover table-stripped m-t-20" data-page-size="30" data-sorting="false">
                                            <thead>
                                                <tr class="font-weight-bold">
                                                    <th class="text-left font-weight-bold" style="width: 20%">課程名稱</th>
                                                    <th class="text-left font-weight-bold" style="width: 10%">講師名稱</th>
                                                    <th class="text-left font-weight-bold" style="width: 12%">上課地點</th>
                                                    <th class="text-left font-weight-bold" style="width: 20%">課程日期</th>
                                                    <th class="text-center font-weight-bold" style="width: 8%">報名人數</th>
                                                    <th class="text-center font-weight-bold" style="width: 10%" data-sort-ignore="true">依區屬</th>
                                                    <th class="text-center font-weight-bold" style="width: 10%" data-sort-ignore="true">依身分別</th>
                                                    <th class="text-center font-weight-bold" style="width: 10%" data-sort-ignore="true">依組別</th>
                                                </tr>
                                            </thead>
                                            <tbody>

                                                <asp:SqlDataSource ID="SDS_HC" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_HC" runat="server" OnItemDataBound="Rpt_HC_ItemDataBound">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="LB_HCourseName" runat="server" Text='<%# Eval("HCourseName") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HTeacherName" runat="server" Text='<%# Eval("HTeacherName") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-left">
                                                                <asp:Label ID="LB_HPlaceName" runat="server" Text='<%# Eval("HPlaceName") %>'></asp:Label>

                                                            </td>
                                                            <td class="text-left">
                                                                <asp:Label ID="LB_HDateRange" runat="server" Text='<%# Eval("HDateRange") %>' Style="word-break: break-word;"></asp:Label>

                                                            </td>

                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HApplyNum" runat="server"></asp:Label>

                                                            </td>
                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_TotalListArea" runat="server" CssClass="btn btn-sm btn-outline-success mr-2" OnClick="LBtn_TotalListArea_Click" CommandArgument='<%# Eval("HID") %>'><i class="icon-eye"></i></asp:LinkButton>

                                                                   <asp:LinkButton ID="LBtn_BBDate" runat="server" CssClass="btn btn-sm btn-outline-success mr-2" OnClick="LBtn_BBDate_Click" CommandArgument='<%# Eval("HID") +"^"+Eval("HPlaceName") +"^"+Eval("HCourseName") +"^"+ Eval("HDateRange")%>'><i class="icon-eye"></i></asp:LinkButton>

                                                                <asp:Button ID="Btn_BBDate" runat="server" Text="查看" CssClass="btn btn-sm btn-outline-success mr-2 d-none" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Eval("HPlaceName") +"^"+Eval("HCourseName") +"^"+ Eval("HDateRange")%>' OnClick="Btn_BBDate_Click" />

                                                            </td>

                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_Identity" runat="server" CssClass="btn btn-sm btn-outline-success mr-2" OnClick="LBtn_Identity_Click" CommandArgument='<%# Eval("HID") %>'><i class="icon-eye"></i></asp:LinkButton>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_TotalList" runat="server" CssClass="btn btn-sm btn-outline-success mr-2" OnClick="LBtn_TotalList_Click" CommandArgument='<%# Eval("HID") %>'><i class="icon-eye"></i></asp:LinkButton>
                                                            </td>
                                                        </tr>


                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>

                                        <!------------------分頁功能開始------------------>
                                        <nav class="list-pagination">
                                            <Page:Paging runat="server" ID="Pg_Paging" />
                                        </nav>
                                        <!------------------分頁功能結束------------------>


                                    </div>
                                </div>
                            </asp:Panel>




                            <!--//傳統總名單格式//--->
                            <asp:Panel ID="Panel_TotalList" runat="server" Visible="false">
                                <div class="text-right pr-2 pt-2">
                                    <cc1:WordExcelButton ID="WordExcelButton3" runat="server" GridView="ToExcelList" ViewStateMode="Enabled" class="NoPrint excel_outer" Style="display: inline;" />

                                </div>

                                <div id="ToExcelList" runat="server">
                                    <h3 class="text-center mt-20">
                                        <asp:Label ID="LB_HCourseNameTLTitle" runat="server" Text="" Font-Size="Large" Font-Bold="true"></asp:Label>
                                    </h3>
                                    <div class="text-right">
                                           <asp:Label ID="LB_PlaceTitle" runat="server" Text="上課地點："></asp:Label>&nbsp;&nbsp;<asp:Label ID="LB_BBDatePlaceName" runat="server" Text=""></asp:Label>;
                                        <asp:Label ID="LB_HTeacherNameTLTitle" runat="server" Text=""></asp:Label>&nbsp;;&nbsp;<asp:Label ID="LB_HDateRangeTLTitle" runat="server" Text=""></asp:Label>
                                    </div>



                                    <asp:Panel ID="Panel_GeneralCourseView" runat="server">

                                        <div class="">
                                            <a class="btn btn-outline-info text-info" data-toggle="modal" data-target="#Div_Light">21道光人數統計</a>
                                        </div>

                                        <div class="table-responsive">
                                            <table id="demo-foo-accordion1" class="table m-b-0 table-bordered table-hover m-t-20" data-sorting="false">
                                                <thead>
                                                    <tr>
                                                        <th class="text-center font-weight-bold" style="width: 5%">區屬</th>
                                                        <th class="text-center font-weight-bold" style="width: 6%">光團</th>
                                                        <th class="text-center font-weight-bold" style="width: 4%">體系</th>
                                                        <th class="text-center font-weight-bold" style="width: 4%">期別</th>
                                                        <th class="text-center font-weight-bold" style="width: 5%">姓名</th>
                                                        <th class="text-center font-weight-bold" style="width: 5%">三載體光</th>
                                                        <th class="text-center font-weight-bold" style="width: 4%">七彩光</th>
                                                        <th class="text-center font-weight-bold" style="width: 6%">21道光</th>
                                                        <th class="text-center font-weight-bold" style="width: 3%">光使</th>
                                                        <th class="text-center font-weight-bold" style="width: 8%">信箱</th>
                                                        <th class="text-center font-weight-bold" style="width: 6%">連絡電話</th>
                                                        <th class="text-center font-weight-bold" style="width: 3%">性別</th>
                                                        <th class="text-center font-weight-bold d-none" style="width: 4%">身分別</th>
                                                        <th class="text-center font-weight-bold d-none" style="width: 3%">組別</th>
                                                        <th class="text-center font-weight-bold d-none" style="width: 7%">工作項目</th>
                                                        <th class="text-center font-weight-bold" style="width: 5%">備註</th>
                                                    </tr>
                                                </thead>
                                                <tbody>




                                                    <asp:SqlDataSource ID="SDS_TotalList" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                    <asp:Repeater ID="Rpt_TotalList" runat="server" DataSourceID="SDS_TotalList" OnItemDataBound="Rpt_TotalList_ItemDataBound">
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HArea" runat="server" Text='<%# Eval("HArea") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HTeamID" runat="server" Text='<%# Eval("HTeamID") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HSystemName" runat="server" Text='<%# Eval("HSystemName") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-center">&nbsp;
																<asp:Label ID="LB_HPeriod" runat="server">&nbsp;<%# Eval("Period") %></asp:Label>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HUserName" runat="server" Text='<%# Eval("HUserName") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HCarrier" runat="server" Text='<%# Eval("HCarrier") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-center" style="word-break: break-word">
                                                                    <asp:Label ID="LB_HRainbow" runat="server" Text='<%# Eval("HRainbow") %>'></asp:Label>
                                                                </td>

                                                                <td class="text-center">

                                                                    <asp:Label ID="LB_HLightName" runat="server" Text=""></asp:Label>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HLightEnvoy" runat="server" Text='<%# Eval("HLightEnvoy") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-center">&nbsp;
																<asp:Label ID="LB_HAccount" runat="server" Text='<%# Eval("HAccount") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-center">&nbsp;
																<asp:Label ID="LB_HPhone" runat="server" Text='<%# Eval("HPhone") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-center">
                                                                    <asp:Label ID="LB_HSex" runat="server" Text='<%# Eval("HSex") %>'></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>


                                                </tbody>
                                            </table>
                                        </div>
                                    </asp:Panel>


                                    <!--七天班會-->
                                    <asp:Panel ID="Panel_BBDateBookingList" runat="server" Visible="false">
                                        <div class="text-right font-weight-bold">

                                            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>&nbsp;&nbsp;報名人數：<asp:Label ID="LB_BBDateTotalNum" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div>
                                            <p class="font-weight-bold mb-1">【注意事項】</p>
                                            <p class="mb-2">上課日期中的數字代表意思：<span class="font-weight-bold text-danger">1-參班、2-純護持、3-參班兼護持，空格表示未報名</span></p>
                                        </div>





                                        <div class="table-responsive">
                                            <asp:Table ID="TBL_HBBDateBookingList" runat="server" CssClass="table table-bordered table-hover"></asp:Table>
                                        </div>


                                    </asp:Panel>




                                </div>








                                <div class="text-center mt-2">
                                    <a class="btn btn-secondary" onclick="window.history.go(-1);">回上一頁</a>
                                </div>


                            </asp:Panel>


                            <!--//依身分別搜尋//--->
                            <asp:Panel ID="Panel_Identity" runat="server" Visible="false">




                                <h3 class="text-center mt-20">
                                    <asp:Label ID="LB_HCourseNameITitle" runat="server" Text="" Font-Size="Large" Font-Bold="true"></asp:Label>
                                </h3>
                                <div class="text-right">
                                    <asp:Label ID="LB_HTeacherNameITitle" runat="server" Text=""></asp:Label>&nbsp;;&nbsp;<asp:Label ID="LB_HDateRangeITitle" runat="server" Text=""></asp:Label>
                                </div>




                                <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4 p-t-10 text-center" style="margin: 0 auto">
                                    <div id="morris-donut-chart" class="ecomm-donute" style="height: 317px;"></div>
                                    <ul class="list-inline m-t-30 text-center">
                                        <li class="p-r-20">
                                            <h5 class="text-muted"><i class="ti-control-record" style="color: #99d683;"></i>參班</h5>
                                            <h4 class="m-b-0">
                                                <asp:Label ID="LB_Attend" runat="server" Text="0" Visible="true"></asp:Label>
                                                人
                                                 (<asp:Label ID="LB_AttendPCT" runat="server" Text="0" Visible="true"></asp:Label>%)
                                            </h4>
                                        </li>
                                        <li class="p-r-20">
                                            <h5 class="text-muted"><i class="ti-control-record" style="color: #13dafe;"></i>參班兼護持</h5>
                                            <h4 class="m-b-0">
                                                <asp:Label ID="LB_ProGuide" runat="server" Text="0" Visible="true"></asp:Label>
                                                人
                                                 (<asp:Label ID="LB_ProGuidePCT" runat="server" Text="0" Visible="true"></asp:Label>%)
                                            </h4>
                                        </li>
                                        <li>
                                            <h5 class="text-muted"><i class="ti-control-record" style="color: #6164c1;"></i>純護持(非班員)<!--護持體系專業者--></h5>
                                            <h4 class="m-b-0">
                                                <asp:Label ID="LB_Guide" runat="server" Text="0" Visible="true"></asp:Label>
                                                人
                                                 (<asp:Label ID="LB_GuidePCT" runat="server" Text="0" Visible="true"></asp:Label>%)
                                            </h4>
                                        </li>
                                    </ul>
                                </div>

                                <div class="text-center mt-2">
                                    <a class="btn btn-secondary" onclick="window.history.go(-1);">回上一頁</a>
                                </div>

                            </asp:Panel>

                            <!--//依區屬搜尋//--->
                            <asp:Panel ID="Panel_TotalListArea" runat="server" Visible="false">

                                <div class="text-right pt-2 pr-2">
                                    <cc1:WordExcelButton ID="WordExcelButton1" runat="server" GridView="TotalListArea_Excel" ViewStateMode="Enabled" class="NoPrint excel_outer" Style="display: inline;" />
                                
                                </div>

                                <div id="TotalListArea_Excel" runat="server">
                                    <h3 class="text-center mt-20">
                                        <asp:Label ID="LB_HCourseNameTLATitle" runat="server" Text="" Font-Size="Large" Font-Bold="true"></asp:Label>
                                    </h3>
                                    <div class="row mt-1">
                                        <div class="text-left col-md-6">
                                            <asp:Label ID="LB_HTeacherNameTLATitle" runat="server" Text=""></asp:Label>&nbsp;;&nbsp;課程日期：<asp:Label ID="LB_HDateRangeTLATitle" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div class="text-right col-md-6">
                                            <asp:Label ID="LB_ApplyNumTLATitle" runat="server" Text=""></asp:Label>&nbsp;&nbsp;報名人數：<asp:Label ID="LB_ApplyNumTLA" runat="server" Text=""></asp:Label>
                                        </div>
                                    </div>

                                    <div class="">
                                        <a class="btn btn-outline-info text-info" data-toggle="modal" data-target="#Div_Light">21道光人數統計</a>
                                    </div>


                                    <div class="table-responsive">
                                        <table class="table m-b-0 table-hover m-t-20">
                                            <thead>
                                                <tr>
                                                    <th class="text-center font-weight-bold" style="width: 5%">區屬</th>
                                                    <th class="text-center font-weight-bold" style="width: 6%">光團</th>
                                                    <th class="text-center font-weight-bold" style="width: 4%">期別</th>
                                                    <th class="text-center font-weight-bold" style="width: 5%">姓名</th>
                                                    <th class="text-center font-weight-bold" style="width: 4%">體系</th>
                                                    <th class="text-center font-weight-bold" style="width: 5%">三載體光</th>
                                                    <th class="text-center font-weight-bold" style="width: 4%">七彩光</th>
                                                    <th class="text-center font-weight-bold" style="width: 6%">21道光</th>
                                                    <th class="text-center font-weight-bold" style="width: 3%">光使</th>
                                                    <th class="text-center font-weight-bold" style="width: 8%">信箱</th>
                                                    <th class="text-center font-weight-bold" style="width: 6%">連絡電話</th>
                                                    <th class="text-center font-weight-bold" style="width: 3%">性別</th>
                                                    <th class="text-center font-weight-bold" style="width: 4%">身分別</th>
                                                    <th class="text-center font-weight-bold" style="width: 3%">組別</th>
                                                    <th class="text-center font-weight-bold" style="width: 8%">工作項目</th>
                                                    <th class="text-center font-weight-bold" style="width: 5%">備註</th>
                                                </tr>
                                            </thead>
                                            <tbody>


                                                <asp:SqlDataSource ID="SDS_TotalListArea" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_TotalListArea" runat="server" DataSourceID="SDS_TotalListArea" OnItemDataBound="Rpt_TotalListArea_ItemDataBound">
                                                    <ItemTemplate>


                                                        <tr style="background: #e0e0e0">
                                                            <td colspan="16">
                                                                <h4 class="text-center mb-0">
                                                                    <asp:Label ID="LB_HArea" runat="server" Text='<%# Eval("HArea") %>' CssClass="d-inline"></asp:Label>
                                                                    <asp:Label ID="LB_Count" runat="server" Text="" CssClass="d-inline"></asp:Label>
                                                                </h4>
                                                            </td>
                                                        </tr>


                                                        <asp:SqlDataSource ID="SDS_TotalListAreaChild" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:Repeater ID="Rpt_TotalListAreaChild" runat="server" OnItemDataBound="Rpt_TotalListAreaChild_ItemDataBound" DataSourceID="SDS_TotalListAreaChild">
                                                            <ItemTemplate>

                                                                <tr>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_HArea" runat="server" Text='<%# Eval("HArea") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_HTeamID" runat="server" Text='<%# Eval("HTeamID") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_HPeriod" runat="server">&nbsp;<%# Eval("HPeriod") %></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_HUserName" runat="server" Text='<%# Eval("HUserName") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_HSystemName" runat="server" Text='<%# Eval("HSystemName") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_HCarrier" runat="server" Text='<%# Eval("HCarrier") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center" style="word-break: break-word">
                                                                        <asp:Label ID="LB_HRainbow" runat="server" Text='<%# Eval("HRainbow") %>'></asp:Label>
                                                                    </td>

                                                                    <td class="text-center">

                                                                        <asp:Label ID="LB_HLightName" runat="server" Text=""></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_HLightEnvoy" runat="server" Text='<%# Eval("HLightEnvoy") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">&nbsp;
				<asp:Label ID="LB_HAccount" runat="server" Text='<%# Eval("HAccount") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">&nbsp;
				<asp:Label ID="LB_HPhone" runat="server" Text='<%# Eval("HPhone") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_HSex" runat="server" Text='<%# Eval("HSex") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_HAttend" runat="server" Text='<%# Eval("HAttend") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_HMemberGroup" runat="server" Text='<%# Eval("HMemberGroup") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_HTask" runat="server" Text='<%# Eval("HTask") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_HRemark" runat="server" Text='<%# Eval("HRemark") %>'></asp:Label>
                                                                    </td>
                                                                </tr>

                                                            </ItemTemplate>
                                                        </asp:Repeater>


                                                    </ItemTemplate>
                                                </asp:Repeater>


                                            </tbody>
                                        </table>







                                    </div>
                                </div>
                                <div class="text-center mt-2">
                                    <a class="btn btn-secondary" onclick="window.history.go(-1);">回上一頁</a>
                                </div>


                            </asp:Panel>





                        </div>
                    </div>
                </div>
            </div>


            <!-- ============================================================== -->
        </div>
    </div>


    <!-- Modal START-->
    <div class="modal fade" id="Div_Light" tabindex="-1" role="dialog" aria-labelledby="info" aria-hidden="true">
        <div class="modal-dialog" role="document" style="max-width: 700px">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="modal-title text-center" id="exampleModalLongTitle">21道光人數統計</h3>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <asp:Literal ID="Literal1" runat="server" />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">關閉</button>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal   END-->

    <!-- Mainly scripts -->
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

    <script src="js/moment.min.js"></script>
    <!--datepicker-->
    <script src="js/bootstrap-datepicker.js"></script>
    <!--daterangepicker-->
    <script src="js/daterangepicker.js"></script>
    <!--select2-->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.7/js/select2.min.js"></script>
    <!--morris JavaScript -->
    <script src="js/raphael-min.js"></script>
    <script src="js/morris.min.js"></script>
    <!-- Footable -->
    <script src="assets/node_modules/footable/js/footable.all.min.js"></script>
    <!--FooTable init-->
    <script src="assets/node_modules/footable/footable-init.js"></script>

    <script>
        $(function () {
            $(".js-example-basic-single").select2();
        });
    </script>
    <script>
        // Date Picker
        $(function () {
            $(".daterange").daterangepicker({
                opens: 'right',
                //autoApply: true,
                autoUpdateInput: false,
                locale: {
                    cancelLabel: 'Clear',
                    format: 'YYYY/MM/DD'
                }
            });

            $(".daterange").on('apply.daterangepicker', function (ev, picker) {
                $(this).val(picker.startDate.format('YYYY/MM/DD') + ' - ' + picker.endDate.format('YYYY/MM/DD'));
            });

            $(".daterange").on('cancel.daterangepicker', function (ev, picker) {
                $(this).val('');
            });
        });
    </script>

    <script>
        //Morris Donut chart
        Morris.Donut({
            element: 'morris-donut-chart',
            data: [{
                label: "參班",
                value: document.getElementById('<%= LB_Attend.ClientID %>').innerText
            }, {
                    label: "參班兼護持",
                    value: document.getElementById('<%= LB_ProGuide.ClientID %>').innerText
                }, {
                    label: "純護持(非班員)", //護持體系專業者
                    value: document.getElementById('<%= LB_Guide.ClientID %>').innerText
                }],
            resize: true,
            colors: ['#99d683', '#13dafe', '#6164c1']
        });
    </script>


</asp:Content>

