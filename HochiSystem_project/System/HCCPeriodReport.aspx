<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HCCPeriodReport.aspx.cs" Inherits="HCCPeriodReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        .label {
            padding: 5px;
        }



        .table-bordered, .table-bordered td, .table-bordered th, .table.table-bordered thead th {
            border: 1px solid #7a7a7a !important;
        }

        .table.table-bordered thead th {
            vertical-align: middle;
            background-color: #dbe9ed;
        }

        .table tbody tr td, .table tbody th td {
            word-break: break-word;
            white-space: normal;
        }

        .table th, .table td {
            padding: 0.2rem;
        }

        .table th {
            font-size: 0.9rem;
        }

        .table tbody td span {
            font-size: 13px !important;
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
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>信用卡授權書交易總表</h2>
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="HWelcome.aspx"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item active">信用卡授權書交易總表</li>
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
                                        <div class="font-weight-bold">搜尋功能：</div>
                                        <div class="box form-group row m-b-0">
                                            <div class="col-md-3 p-r-0 p-l-0 mr-2">
                                                <asp:TextBox ID="TB_SHCourseName" runat="server" class="form-control p-l-1" placeholder="請輸入捐款項目" AutoComplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3 p-r-0 p-l-0 mr-2" style="max-width: 22%; flex: 0 0 22%;">
                                                <asp:TextBox ID="TB_Search" runat="server" class="form-control p-l-1" placeholder="授權訂單代碼/綠界廠商訂單編號/學員姓名" AutoComplete="off"></asp:TextBox>
                                            </div>

                                            <div class="col-md-3 p-r-0 p-l-0 mr-2" style="max-width: 14%;">
                                                <asp:TextBox ID="TB_SHPaymentDate" runat="server" class="form-control daterange" placeholder="付款日期區間" AutoComplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-md-2 col-sm-2 p-r-0 p-l-0 mr-2">
                                                <asp:DropDownList ID="DDL_SHRtnCode" runat="server" class="form-control">
                                                    <asp:ListItem Value="0">選擇授權結果</asp:ListItem>
                                                    <asp:ListItem Value="1">成功</asp:ListItem>
                                                    <asp:ListItem Value="2">失敗</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>

                                            <div class="col-md-2 excel_outer" style="max-width: 13%; display: flex; align-items: center;">
                                                <asp:LinkButton ID="LBtn_Search" runat="server" OnClick="LBtn_Search_Click" class="btn btn-outline-secondary"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                                <asp:LinkButton ID="LBtn_SearchCancel" runat="server" OnClick="LBtn_SearchCancel_Click" class="btn btn-outline-secondary m-l-10"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                            </div>

                                        </div>
                                    </div>

                                </div>





                                <asp:Panel ID="Panel_OrderList" runat="server" Visible="false">
                                    <div class="table-responsive">
                                        <table class="table m-b-0 table-hover table-bordered m-t-20">
                                            <thead>
                                                <tr class="font-weight-bold">
                                                    <th style="width: 8%">授權書單號</th>
                                                    <th style="width: 20%">捐款項目</th>
                                                    <th style="width: 10%">捐款人姓名</th>
                                                    <%-- <th style="width: 10%">祝福人數</th>--%>
                                                    <th class="text-center" style="width: 6%">捐款總金額</th>
                                                    <th class="text-center" style="width: 4%">總期數</th>
                                                    <th class="text-center" style="width: 6%">每期扣款金額</th>
                                                    <th class="text-center" style="width: 5%">扣款期數</th>
                                                    <%--<th class="text-center" style="width: 5%">累計扣款期數</th>--%>
                                                    <th class="text-center" style="width: 4%">剩餘期數</th>
                                                    <th class="text-center" style="width: 8%">授權訂單代碼</th>
                                                    <%--<th class="text-center" style="width: 10%">綠界廠商訂單編號</th>--%>
                                                    <th class="text-center" style="width: 6%">授權日期</th>
                                                    <th class="text-right" style="width: 4%">授權金額</th>
                                                    <th class="text-center" style="width: 4%">授權結果</th>

                                                    <th class="text-center" style="width: 6%">其他付款日期</th>
                                                    <th class="text-center" style="width: 6%">其他付款金額</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:SqlDataSource ID="SDS_HCCPOTRecord" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_HCCPOTRecord" runat="server" OnItemDataBound="Rpt_HCCPOTRecord_ItemDataBound">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="LB_HCCPeriodCode" runat="server" Text='<%# Eval("HCCPeriodCode") %>'></asp:Label>
                                                            </td>

                                                            <td>
                                                                <asp:Label ID="LB_HCourseName" runat="server" Text='<%# Eval("HCourseName") %>'></asp:Label>
                                                            </td>

                                                            <td>
                                                                <asp:Label ID="LB_MemberName" runat="server" Text='<%# Eval("UserName") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HDTotal" runat="server" Text='<%# Eval("HDTotal") %>'></asp:Label>
                                                            </td>

                                                            <td class="text-center">
                                                                <asp:Label ID="LB_HDCCPTimes" runat="server" Text='<%# Eval("HDCCPTimes") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HDCCPAmount" runat="server" Text='<%# Eval("HDCCPAmount") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_DeductRound" runat="server" Text='<%# Eval("DeductRound") %>'></asp:Label>
                                                            </td>
                                                            <%--<td class="text-center">
                                                                <asp:Label ID="LB_Num" runat="server" Text='<%# Eval("num") %>'></asp:Label>
                                                            </td>--%>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_RemainTimes" runat="server" Text='<%# Eval("RemainTimes") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_HCCPOrderCode" runat="server" Text='<%# Eval("HCCPOrderCode") %>'></asp:Label>
                                                            </td>
                                                            <%--<td class="text-center">
                                                                <asp:Label ID="LB_HMerchantTradeNo" runat="server" Text='<%# Eval("HMerchantTradeNo") %>'></asp:Label>
                                                            </td>--%>
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
                                                                <asp:Label ID="LB_HPaymentDate" runat="server" Text='<%# Eval("HPaymentDate") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-right">
                                                                <asp:Label ID="LB_HPayAmount" runat="server" Text='<%# Eval("HPayAmount") %>'></asp:Label>
                                                            </td>


                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                </asp:Panel>


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
            //}, function (start, end, label) {
            //    console.log("A new date selection was made: " + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD'));
            //});
        });
    </script>




</asp:Content>

