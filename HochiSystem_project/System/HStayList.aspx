<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HStayList.aspx.cs" Inherits="HStayList" %>
<%@ Register Assembly="MyWebControls" Namespace="MyWebControls" TagPrefix="cc1" %>

<%--分頁--%>
<%@ Register TagPrefix="Page" TagName="Paging" Src="~/System/Paging_backend.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
	<style>
		.table {
			white-space: nowrap !important;
			word-break: keep-all !important;
		}

		/*	.table th, .table td{
			border-top: 1px solid #222222;
		}*/

		.table {
			border-top: 1.1px solid #222222;
		}

			.table tbody {
				border-top: 1.1px solid #000;
			}

		.staylist_cell {
			border-width: 0px;
			text-align: left;
			padding: 0;
			background: unset;
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
			<%--p-t-20--%>
			<!-- ============================================================== -->

			<div class="block-header">
				<div class="row">
					<div class="col-lg-3 col-md-12 col-sm-4">
						<h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>住宿登記表</h2>
						<ul class="breadcrumb">
							<li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
							<li class="breadcrumb-item">報表分析</li>
							<li class="breadcrumb-item active">住宿登記表</li>
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
						<div class="body p-2">

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
													<asp:ListItem Value="5">非參班(純護持)</asp:ListItem>
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
													<th class="text-left font-weight-bold" style="width: 25%">課程名稱</th>
													<th class="text-left font-weight-bold" style="width: 10%">講師名稱</th>
													<th class="text-left font-weight-bold" style="width: 12%">上課地點</th>
													<th class="text-left font-weight-bold" style="width: 23%">課程日期</th>
													<th class="text-center font-weight-bold" style="width: 10%" data-sort-ignore="true">查看住宿登記表</th>
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
															<td class="text-center">
																<asp:LinkButton ID="LBtn_StayList" runat="server" CssClass="btn btn-sm btn-outline-success mr-2" OnClick="LBtn_StayList_Click" CommandArgument='<%# Eval("HID") %>'><i class="icon-eye"></i></asp:LinkButton>
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









							<asp:Panel ID="Panel_StayList" runat="server" Visible="false">
								<asp:Label ID="LB_HID" runat="server" Text="" Visible="false"></asp:Label>

								<div class="text-right pr-2 pt-2">
									<asp:ImageButton ID="IBtn_ToExcel" runat="server" ImageUrl="~/images/icons/excel.png" CssClass="excel_img" OnClick="IBtn_ToExcel_Click" />
								</div>
								<asp:Panel ID="Panel1" runat="server" Visible="true">
									<div class="table-responsive">


										<table border="0" style="width: 100%">
											<tr>
												<td align="center">
													<asp:Label ID="LB_HCourseName" runat="server" Text="" Font-Size="Large" Font-Bold="true"></asp:Label>
												</td>
											</tr>
											<tr>
												<td align="right">
													<asp:Label ID="LB_HTeacherName" runat="server" Text=""></asp:Label>&nbsp;;&nbsp;<asp:Label ID="LB_HDateRange" runat="server" Text=""></asp:Label>
												</td>
											</tr>
											<tr>
												<td>
													<asp:Table ID="TBL_StayList" runat="server"></asp:Table>
												</td>
											</tr>
										</table>





									</div>

								</asp:Panel>

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

