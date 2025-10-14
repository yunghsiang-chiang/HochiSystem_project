<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HCourseAttendRec.aspx.cs" Inherits="HCourseAttendRec" %>

<%@ Register Assembly="MyWebControls" Namespace="MyWebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .table thead th, .table thead td, .table th, .table td {
            vertical-align: middle;
            border-color: #000;
            white-space: nowrap;
        }

        .select2-container--default .select2-selection--multiple {
            max-height: 38px;
            overflow: auto;
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
                        <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>課程出席紀錄表</h2>
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                            <li class="breadcrumb-item"><a href="HCourseAttendRec.aspx">課程出席紀錄表</a></li>
                            <li class="breadcrumb-item active">課程出席紀錄表</li>
                        </ul>
                    </div>
                </div>
            </div>
            <!-- ============================================================== -->
            <!-- Start Page Content -->
            <!-- ============================================================== -->

            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="row clearfix">
                        <div class="col-lg-12 col-md-12">
                            <div class="card">
                                <div class="body">

                                    <asp:LinkButton ID="LBtn_Back" runat="server" OnClick="LBtn_Back_Click" class="btn btn-outline-secondary d-none"><span class="btn-label"><i class="fa fa-arrow-left mr-2"></i></span>回上一頁</asp:LinkButton>
                                    <%--<div class="excel_outer text-right">
								<cc1:WordExcelButton ID="WordExcelButton2" runat="server" GridView="Panel_Report" ViewStateMode="Enabled" class="NoPrint" Style="display: inline;" />
							</div>--%>
                                    <div class="row">
                                        <div class="col-md-12 col-lg-12 col-xlg-12">
                                            <div class="box form-group row m-b-0">
                                                <div class="col-md-3">
                                                    <%--<asp:ListBox ID="LBox_HCourseName" runat="server" class="form-control ListB_Multi ListB_CourseName" name="state" SelectionMode="Single" Style="width: 100%;" Enabled="true" OnSelectedIndexChanged="LBox_HCourseName_SelectedIndexChanged" AutoPostBack="true"></asp:ListBox>--%>
                                                    <asp:DropDownList ID="DDL_HCourseName" runat="server" class="form-control js-example-basic-single" Style="width: 100%; height: 38px;" OnSelectedIndexChanged="DDL_HCourseName_SelectedIndexChanged" AutoPostBack="true">
                                                        <asp:ListItem Value="0">請選擇課程名稱</asp:ListItem>
                                                    </asp:DropDownList>


                                                </div>
                                                <div class="col-md-3">
                                                    <%--<asp:ListBox ID="LBox_HDateRange" runat="server" class="form-control ListB_Multi ListB_HDateRange" name="state" SelectionMode="Single" Style="width: 100%;" Enabled="true"></asp:ListBox>--%>
                                                    <asp:DropDownList ID="DDL_HDateRange" runat="server" class="form-control js-example-basic-single" Style="width: 100%; height: 38px;">
                                                        <asp:ListItem Value="0">請選擇日期</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <%--(為必填)--%>
                                                <div class="col-md-4" style="max-width: 30%">

                                                    <asp:ListBox ID="LBox_HOCPlace" runat="server" class="form-control  select2-multiple" name="state" SelectionMode="Multiple" Width="100%" OnSelectedIndexChanged="LBox_HOCPlace_SelectedIndexChanged" AutoPostBack="true"></asp:ListBox>

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


                                    <asp:Panel ID="Panel_Report" runat="server" Visible="true">

                                        <h3 class="text-center">
                                            <asp:Label ID="LB_HCourseName" runat="server" Text="" Font-Size="Large" Font-Bold="true"></asp:Label>
                                        </h3>


                                        <p class="text-danger mb-2">*欄位空白表示<b>未報名</b>該天課程；數字為<b>累計</b>的的出席次數</p>
                                        <div class="table-responsive">
                                            <asp:Table ID="TBL_HCAttendRec" runat="server" CssClass="table table-hover"></asp:Table>
                                        </div>

                                    </asp:Panel>

                                    <div class="text-center m-t-20 d-none">
                                        <asp:Button ID="Btn_Cancel" runat="server" Text="回上一頁" class="btn btn-inverse" OnClick="Btn_Cancel_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="LBtn_Search" />
                    <asp:PostBackTrigger ControlID="LBtn_SearchCancel" />
                    <asp:PostBackTrigger ControlID="IBtn_ToExcel" />
                    <asp:AsyncPostBackTrigger ControlID="DDL_HCourseName" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="LBox_HOCPlace" EventName="SelectedIndexChanged" />
                </Triggers>

            </asp:UpdatePanel>





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
        $(function () {
            $(".js-example-basic-single").select2();
            $('.select2-multiple').select2();
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
    <script>
        $(function () {
            //單選
            //$('.js-example-basic-single').select2();

            //上傳圖片 --dropify js
            // $('.dropify').dropify();

            ////多選--sumoselect js
            //$('.ListB_Multi').SumoSelect({
            //    search: true,
            //    //placeholder: '請選擇',
            //    csvDispCount: 5,
            //});

            $('.ListB_CourseName').SumoSelect({
                search: true,
                placeholder: '請選擇課程名稱',
                csvDispCount: 5,
            });

            $('.ListB_HDateRange').SumoSelect({
                search: true,
                placeholder: '請選擇上課日期',
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

