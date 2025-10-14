<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HApplyRecordSame.aspx.cs" Inherits="System_HApplyRecordSame" %>

<%@ Register Assembly="MyWebControls" Namespace="MyWebControls" TagPrefix="cc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .table {
			white-space:nowrap !important;
			word-break:keep-all !important;
		}

        .data_cell {
            border-width: 0px;
            text-align: left;
            padding: 0;
            background: unset;
            min-height: unset;
            height: auto;
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
                        <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>同課程同日期參班紀錄分析</h2>
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                            <li class="breadcrumb-item active">同課程同日期參班紀錄分析</li>
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
                                        <div class="col-md-3">
                                            <asp:ListBox ID="LBox_HCourseName" runat="server" class="form-control ListB_Multi ListB_CourseName" name="state" SelectionMode="Multiple" Style="width: 100%;" Enabled="true"></asp:ListBox>
                                        </div>
                                        <div class="col-md-2">
                                            <asp:TextBox ID="TB_SearchDate" runat="server" class="form-control daterange" placeholder="選擇課程日期區間" AutoComplete="off"></asp:TextBox>
                                        </div><%--(為必填)--%>
                                        <div class="col-md-2">
                                            <asp:ListBox ID="LBox_HOCPlace" runat="server" class="form-control ListB_Multi ListB_HOCPlace" name="state" SelectionMode="Multiple" Width="100%"></asp:ListBox>
                                        </div>
                                        <div class="col-md-2">
                                            <asp:DropDownList ID="DDL_HArea" runat="server" class="form-control js-example-basic-single" Style="width: 100%; height: 38px;" placeholder="請選擇區屬">
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-md-3 excel_outer" style="max-width: 20%; display: flex; align-items: center;">
                                            <asp:LinkButton ID="LBtn_Search" runat="server" OnClick="LBtn_Search_Click" class="btn btn-outline-secondary"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                            <asp:LinkButton ID="LBtn_SearchCancel" runat="server" OnClick="LBtn_SearchCancel_Click" class="btn btn-outline-secondary m-l-10"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                            <div class="text-right pr-2 pt-2">
												<asp:ImageButton ID="IBtn_ToExcel" runat="server" ImageUrl="~/images/icons/excel.png" CssClass="excel_img" OnClick="IBtn_ToExcel_Click" />
											</div>

                                        </div>
                                    </div>
                                </div>
                            </div>



                            <asp:Panel ID="Panel_CourseList" runat="server" Visible="false">
                               
                                <div class="mt-3">
                                    <h3 class="text-center mb-1"></h3>
                                    <div class="table-responsive mt-20">
                                        <asp:Table ID="TBL_ApplyRecord" runat="server" CssClass="table table-hover" Style="border: 1px solid #ccc"></asp:Table>
                                    </div>
                                </div>
                            </asp:Panel>
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

         <!--sumoselect-->
        <script src="js/jquery.sumoselect.min.js"></script>

        
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
                $(function () {
                    //單選
                    $('.js-example-basic-single').select2();

					$('.ListB_CourseName').SumoSelect({
						search: true,
						placeholder: '請選擇課程名稱',
						csvDispCount: 5,
                    });

					$('.ListB_HOCPlace').SumoSelect({
						search: true,
						placeholder: '請選擇上課地點',
						csvDispCount: 5,
					});

                });
			</script>






</asp:Content>

