<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HCPCountingReport.aspx.cs" Inherits="System_HCPCountingReport" %>

<%--分頁--%>
<%@ Register TagPrefix="Page" TagName="Paging" Src="~/System/Paging_backend.ascx" %>

<%@ Register Assembly="MyWebControls" Namespace="MyWebControls" TagPrefix="cc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        #demo-foo-accordion th {
            font-size: 13px;
            vertical-align: central;
            padding: 5px 2px;
        }

        #demo-foo-accordion tbody td, #demo-foo-accordion tbody td span {
            font-size: 0.85rem !important;
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
                        <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>開課明細與人數統計表</h2>
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                            <li class="breadcrumb-item active">開課明細與人數統計表</li>
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
                                                <asp:DropDownList ID="DDL_HPMethod" runat="server" class="form-control js-example-basic-single" Style="width: 100%; height: 38px;" placeholder="請選擇繳費帳戶">
                                                    <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                    <asp:ListItem Value="1">基金會</asp:ListItem>
                                                    <asp:ListItem Value="2">文化事業</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                         
                                            <div class="col-md-2">
                                                <asp:TextBox ID="TB_SearchDate" runat="server" class="form-control daterange" placeholder="選擇課程日期區間" AutoComplete="off"></asp:TextBox>
                                            </div>
                                        
                                            <div class="col-md-3 excel_outer" style="max-width: 20%; display: flex; align-items: center;">
                                                <asp:LinkButton ID="LBtn_Search" runat="server" OnClick="LBtn_Search_Click" class="btn btn-outline-secondary"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                                <asp:LinkButton ID="LBtn_SearchCancel" runat="server" OnClick="LBtn_SearchCancel_Click" class="btn btn-outline-secondary m-l-10"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                               <%-- <a class="btn btn-success" title="匯出Excel" style="color: #fff"><span class="btn-label"><i class="ti-export"></i></span></a>--%>
                                              <div class="text-right excel_outer">
				<cc1:WordExcelButton ID="WordExcelButton2" runat="server" GridView="ToWordExcel" ViewStateMode="Enabled" class="NoPrint" Style="display: inline;" />
</div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>


                            <asp:Panel ID="Panel_CourseList" runat="server" Visible="true">
                                <div class="mt-3">
                                    <h3 class="text-center mb-1"></h3>
                                    <%--<label>到班: 1</label>/ <label>未到班:0 </label>--%>
                                    <div class="table-responsive"  id="ToWordExcel" runat="server">
                                        <table class="table table-bordered m-b-0 table-hover table-stripped m-t-20" >
                                            <thead>

                                                <tr class="font-weight-bold">
                                                    <th class="text-center font-weight-bold" rowspan="2" style="width: 5%">預算類別</th>
                                                    <th class="text-center font-weight-bold" rowspan="2" style="width: 5%">繳費帳戶</th>
                                                    <th class="text-center font-weight-bold" rowspan="2" style="width: 5%">課程類別</th>
                                                    <th class="text-left font-weight-bold" rowspan="2" style="width: 8%">課程範本名稱</th>
                                                    <th class="text-left font-weight-bold" rowspan="2" style="width: 10%">課程名稱</th>
                                                    <th class="text-left font-weight-bold" rowspan="2" style="width: 10%">課程日期</th>
                                                    <th class="text-left font-weight-bold" rowspan="2" style="width: 7%">開課實際日期</th>
                                                    <th class="text-center font-weight-bold" rowspan="2" style="width: 4%">實際<br />
                                                        上課<br />
                                                        天數</th>
                                                
                                                    <th class="text-left font-weight-bold" rowspan="2" style="width: 6%">課程講師</th>

                                                    <th class="text-center font-weight-bold" rowspan="2" style="width: 5%">上課方式</th>
                                                    <th class="text-center font-weight-bold" colspan="6" style="width: 5%">人數統計</th>

                                                </tr>
                                                <tr>
                                                    <th class="text-center font-weight-bold" style="width: 5%; font-size: 12px;">總報名
                                                    </th>
                                                    <th class="text-center font-weight-bold" style="width: 4%; font-size: 12px;">男生</th>
                                                    <th class="text-center font-weight-bold" style="width: 4%; font-size: 12px;">女生</th>
                                                    <th class="text-center font-weight-bold" style="width: 4%; font-size: 12px;">參班</th>
                                                    <th class="text-center font-weight-bold" style="width: 6%; font-size: 12px;">參班兼護持</th>
                                                    <th class="text-center font-weight-bold" style="width: 5%; font-size: 12px;">純護持</th>
                                                </tr>
                                            </thead>
                                            <tbody>

                                                <asp:SqlDataSource ID="SDS_HC" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_HC" runat="server" DataSourceID="SDS_HC" OnItemDataBound="Rpt_HC_ItemDataBound">
                                                    <ItemTemplate>

                                                        <tr>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_HBudgetType" runat="server" Text='<%# Eval("HBudgetType") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_PMethod" runat="server" Text='<%# Eval("PMethod") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_CourseType" runat="server" Text='<%# Eval("CourseType") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HTemplateName" runat="server" Text='<%# Eval("HTemplateName") %>'></asp:Label>
                                                            </td>

                                                            <td>
                                                                <asp:Label ID="LB_HCourseName" runat="server" Text='<%# Eval("HCourseName") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HDateRange" runat="server" Text='<%# Eval("HDateRange") %>' Style="word-break: break-word;"></asp:Label>

                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_StartDate" runat="server" Text='<%# Eval("StartDate") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_Days" runat="server" Text="0"></asp:Label><!--上課天數-->
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HTeacherName" runat="server" Text='<%# Eval("TeacherName") %>'></asp:Label>
                                                                <%--<asp:Label ID="LB_HTeacher" runat="server" Text='<%# Eval("HTeacherName") %>' ></asp:Label>--%>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_HCPlace" runat="server" Text="0"></asp:Label><!--實體/線上-->
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HApplyNum" runat="server" Text="0"></asp:Label><!--報名總人數-->
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_MaleNum" runat="server" Text="0"></asp:Label><!--男生人數-->
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_FemaleNum" runat="server" Text="0"></asp:Label><!--女生人數-->
                                                            </td>

                                                            <td class="text-right">
                                                                <asp:Label ID="LB_AttendNum" runat="server" Text="0"></asp:Label><!--參班-->
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_ProGuideNum" runat="server" Text="0"></asp:Label><!--參班兼護持人數-->
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_GuideNum" runat="server" Text="0"></asp:Label><!--純護持人數-->
                                                            </td>

                                                        </tr>


                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>

                                        <!------------------分頁功能開始------------------>
                                        <%--<nav class="list-pagination">
                                            <Page:Paging runat="server" ID="Pg_Paging" />
                                        </nav>--%>
                                        <!------------------分頁功能結束------------------>


                                    </div>
                                </div>
                            </asp:Panel>


                        </div>
                    </div>
                </div>
            </div>


            <!-- ============================================================== -->
        </div>
    </div>


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
            //}, function (start, end, label) {
            //    console.log("A new date selection was made: " + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD'));
            //});
        });
    </script>


</asp:Content>

