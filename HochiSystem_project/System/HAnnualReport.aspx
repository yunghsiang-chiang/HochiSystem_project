<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HAnnualReport.aspx.cs" Inherits="System_HAnnualReport" %>

<%@ Register Assembly="MyWebControls" Namespace="MyWebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .label {
            padding: 5px;
        }

        .table tbody tr td, .table tbody th td {
            word-break: break-word;
            white-space: normal;
        }

        .table th, .table td {
            padding: 0.2rem;
            font-size: 13px !important;
        }

        .table tbody td span {
            font-size: 12px !important;
        }

        .btn-outline-success {
            color: #00c292 !important;
        }

            .btn-outline-success:hover {
                color: #fff !important;
            }

        .pull-left h3 b {
            font-size: 1.15rem;
        }

        .excel_table thead tr th {
            font-size: 0.9rem !important;
        }

        .excel_table tr td span {
            font-size: 0.9rem !important;
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



            <asp:Panel ID="Panel_List" runat="server">
                <div class="block-header">
                    <div class="row">
                        <div class="col-lg-3 col-md-12 col-sm-4">
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>文化年度各月課程大表</h2>
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item active">文化年度各月課程大表</li>
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
                                    <div class="col-md-12 col-lg-12 col-xlg-12">
                                        <div class="box form-group row m-b-0">
                                            <div class="col-md-5 p-r-0 p-l-0 mr-2 d-none" style="max-width: 20%;">
                                                <asp:TextBox ID="TB_SHCourseName" runat="server" class="form-control p-l-1" placeholder="請輸入課程名稱" AutoComplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3 col-sm-6 p-r-0 p-l-0 mr-2" style="max-width: 12%;">
                                                <asp:DropDownList ID="DDL_SHCDYear" runat="server" class="form-control js-example-basic-single" Style="width: 100%">
                                                    <asp:ListItem Value="99">請選擇課程年份</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-md-3 col-sm-6 p-r-0 p-l-0 mr-2" style="max-width: 12%;">
                                                <asp:DropDownList ID="DDL_SHCDMonth" runat="server" class="form-control js-example-basic-single" Style="width: 100%">
                                                    <asp:ListItem Value="99">請選擇課程月份</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            
                                            <div class="col-md-2 col-sm-2 p-r-0 p-l-0 mr-2 d-none" style="max-width: 8%;">
                                                <asp:DropDownList ID="DDL_SHPMethod" runat="server" class="form-control">
                                                    <asp:ListItem Value="0">繳費帳戶</asp:ListItem>
                                                    <asp:ListItem Value="1">基金會</asp:ListItem>
                                                    <asp:ListItem Value="2" Selected>文化事業</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-md-2 col-sm-2 p-r-0 p-l-0 mr-2 d-none" style="max-width: 15%;">
                                                <asp:TextBox ID="TB_SHPaymentDate" runat="server" class="form-control daterange" placeholder="付款日期區間" AutoComplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-md-2 excel_outer" style="max-width: 13%; display: flex; align-items: center;">
                                                <asp:LinkButton ID="LBtn_Search" runat="server" OnClick="LBtn_Search_Click" class="btn btn-outline-secondary"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                                <asp:LinkButton ID="LBtn_SearchCancel" runat="server" OnClick="LBtn_SearchCancel_Click" class="btn btn-outline-secondary m-l-10"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                                <cc1:WordExcelButton ID="WordExcelButton2" runat="server" GridView="Div_AnnualReport" ViewStateMode="Enabled" class="NoPrint" Style="width:30px;" />

                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="excel_outer mt-2 p-l-10" style="max-width: 100%; display: flex; justify-content: flex-start; align-items: center;">
                                    <asp:LinkButton ID="LBtn_AnnualReport" runat="server" class="btn btn-outline-primary mr-2" OnClick="LBtn_AnnualReport_Click" Visible="false"><span class="btn-label"><i class="ti-export m-r-5"></i>匯出文化報名明細表(依付款日期條件)</span></asp:LinkButton>
                                    <asp:LinkButton ID="LBtn_MCourseReport" runat="server" class="btn btn-outline-success mr-2 d-none" OnClick="LBtn_MCourseReport_Click" Visible="true"><span class="btn-label"><i class="ti-export m-r-5"></i>年度課程報表</span></asp:LinkButton>
                                </div>


                                
                                <asp:Table ID="TBL_HCMCB" runat="server" CssClass="table table-hover"></asp:Table>




                                <asp:Panel ID="Panel_NormalList" runat="server" Visible="false">
                                    <div class="table-responsive" id="Div_AnnualReport" runat="server">
                                        <table class="table m-b-0 table-bordered table-hover m-t-20">
                                            <thead>
                                                <tr class="font-weight-bold" style="background-color: #29c7a0; color: #fff">
                                                    <th style="width: 20%">課程名稱</th>
                                                    <th class="text-right" style="width: 6%">1月</th>
                                                    <th class="text-right" style="width: 6%">2月</th>
                                                    <th class="text-right" style="width: 6%">3月</th>
                                                    <th class="text-right" style="width: 6%">4月</th>
                                                    <th class="text-right" style="width: 6%">5月</th>
                                                    <th class="text-right" style="width: 6%">6月</th>
                                                    <th class="text-right" style="width: 6%">7月</th>
                                                    <th class="text-right" style="width: 6%">8月</th>
                                                    <th class="text-right" style="width: 6%">9月</th>
                                                    <th class="text-right" style="width: 6%">10月</th>
                                                    <th class="text-right" style="width: 6%">11月</th>
                                                    <th class="text-right" style="width: 6%">12月</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <!--常態課程 Start-->
                                                <asp:SqlDataSource ID="SDS_HNormal" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_HNormal" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="LB_HCourseName" runat="server" Text='<%# Eval("CourseGroup") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HJanTotal" runat="server" Text='<%# Eval("Jan") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HFebTotal" runat="server" Text='<%# Eval("Feb") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HMarTotal" runat="server" Text='<%# Eval("Mar") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HAprTotal" runat="server" Text='<%# Eval("Apr") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HMayTotal" runat="server" Text='<%# Eval("May") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HJunTotal" runat="server" Text='<%# Eval("Jun") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HJulTotal" runat="server" Text='<%# Eval("Jul") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HAugTotal" runat="server" Text='<%# Eval("Aug") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HSepTotal" runat="server" Text='<%# Eval("Sep") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HOctTotal" runat="server" Text='<%# Eval("Oct") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HNovTotal" runat="server" Text='<%# Eval("Nov") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HDecTotal" runat="server" Text='<%# Eval("Dec") %>'></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <tr class="font-weight-bold" style="background-color: #48a4c8; color: #fff">
                                                    <td style="text-align: center">
                                                        <asp:Label ID="LB_HNTotalTitle" runat="server" Text="常態課程小計"></asp:Label></td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_HNSubTotal1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_HNSubTotal2" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_HNSubTotal3" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_HNSubTotal4" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_HNSubTotal5" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_HNSubTotal6" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_HNSubTotal7" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_HNSubTotal8" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_HNSubTotal9" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_HNSubTotal10" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_HNSubTotal11" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_HNSubTotal12" runat="server" Text="0"></asp:Label>
                                                    </td>

                                                </tr>
                                                <!--常態課程 End-->

                                                <!--單一課程 Start-->
                                                <asp:Panel ID="Panel_SingleList" runat="server" Visible="false">
                                                    <asp:SqlDataSource ID="SDS_HSingle" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                    <asp:Repeater ID="Rpt_HSingle" runat="server">
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="LB_HCourseName" runat="server" Text='<%# Eval("HCourseName") %>'></asp:Label>
                                                                </td>
                                                                <%-- <td class="text-center">
                                                                <asp:Label ID="Label1" runat="server" Text="單一課程"></asp:Label>
                                                            </td>--%>
                                                                <td class="text-right">
                                                                    <asp:Label ID="LB_HJanTotal" runat="server" Text='<%# Eval("Jan") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-right">
                                                                    <asp:Label ID="LB_HFebTotal" runat="server" Text='<%# Eval("Feb") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-right">
                                                                    <asp:Label ID="LB_HMarTotal" runat="server" Text='<%# Eval("Mar") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-right">
                                                                    <asp:Label ID="LB_HAprTotal" runat="server" Text='<%# Eval("Apr") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-right">
                                                                    <asp:Label ID="LB_HMayTotal" runat="server" Text='<%# Eval("May") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-right">
                                                                    <asp:Label ID="LB_HJunTotal" runat="server" Text='<%# Eval("Jun") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-right">
                                                                    <asp:Label ID="LB_HJulTotal" runat="server" Text='<%# Eval("Jul") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-right">
                                                                    <asp:Label ID="LB_HAugTotal" runat="server" Text='<%# Eval("Aug") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-right">
                                                                    <asp:Label ID="LB_HSepTotal" runat="server" Text='<%# Eval("Sep") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-right">
                                                                    <asp:Label ID="LB_HOctTotal" runat="server" Text='<%# Eval("Oct") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-right">
                                                                    <asp:Label ID="LB_HNovTotal" runat="server" Text='<%# Eval("Nov") %>'></asp:Label>
                                                                </td>
                                                                <td class="text-right">
                                                                    <asp:Label ID="LB_HDecTotal" runat="server" Text='<%# Eval("Dec") %>'></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                    <tr class="font-weight-bold" style="background-color: #48a4c8; color: #fff">

                                                        <td style="text-align: center">
                                                            <asp:Label ID="LB_HSingleTitle" runat="server" Text="單一課程小計"></asp:Label></td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HSSubTotal1" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HSSubTotal2" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HSSubTotal3" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HSSubTotal4" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HSSubTotal5" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HSSubTotal6" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HSSubTotal7" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HSSubTotal8" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HSSubTotal9" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HSSubTotal10" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HSSubTotal11" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HSSubTotal12" runat="server" Text="0"></asp:Label>
                                                        </td>

                                                    </tr>
                                                </asp:Panel>
                                                <!--單一課程 End-->


                                                <!--總計 start-->

                                                <tr class="font-weight-bold" style="background-color: #555555; color: #fff">

                                                    <td style="text-align: center">
                                                        <asp:Label ID="Label1" runat="server" Text="總計"></asp:Label></td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_SumJan" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_SumFeb" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_SumMar" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_SumApr" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_SumMay" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_SumJun" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_SumJul" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_SumAug" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_SumSep" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_SumOct" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_SumNov" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:Label ID="LB_SumDec" runat="server" Text="0"></asp:Label>
                                                    </td>

                                                </tr>

                                            </tbody>
                                        </table>
                                    </div>
                                </asp:Panel>
                                <!--常態課程 End-->




                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>




        </div>
        <!-- ============================================================== -->
        <!-- End Container fluid  -->
        <!-- ============================================================== -->
    </div>
    <!-- ============================================================== -->
    <!-- End Page wrapper  -->
    <!-- ============================================================== -->


    <!-- ============================================================== -->
    <!-- All Jquery -->
    <!-- ============================================================== -->
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

    <!-- icheck -->
    <script src="assets/node_modules/icheck/icheck.min.js"></script>
    <script src="assets/node_modules/icheck/icheck.init.js"></script>
    <script src="js/moment.min.js"></script>
    <!--datepicker-->
    <script src="js/bootstrap-datepicker.js"></script>
    <!--daterangepicker-->
    <script src="js/daterangepicker.js"></script>
    <!--select2-->
    <script src="js/select2.min.js"></script>


    <script>
        // Date Picker
        $(function () {
            //單選
            $('.js-example-basic-single').select2();


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
</asp:Content>
