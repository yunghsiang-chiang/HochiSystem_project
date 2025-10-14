<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HAMCTeamList.aspx.cs" Inherits="System_HAMCTeamList" %>

<%@ Register Assembly="MyWebControls" Namespace="MyWebControls" TagPrefix="cc1" %>

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
			<!-- ============================================================== -->

			<div class="block-header">
				<div class="row">
					<div class="col-lg-3 col-md-12 col-sm-4">
						<h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>區光團連線名單</h2>
						<ul class="breadcrumb">
							<li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
							<li class="breadcrumb-item active">區光團連線名單</li>
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

											<div class="col-md-2">
												<asp:DropDownList ID="DDL_HArea" runat="server" class="form-control js-example-basic-single" Style="width: 100%; height: 38px;" placeholder="請選擇區屬">
												</asp:DropDownList>
											</div>

											<div class="col-md-3" style="max-width: 20%; display: flex; align-items: center;">
												<asp:LinkButton ID="LBtn_Search" runat="server" OnClick="LBtn_Search_Click" class="btn btn-outline-secondary"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
												<asp:LinkButton ID="LBtn_SearchCancel" runat="server" OnClick="LBtn_SearchCancel_Click" class="btn btn-outline-secondary m-l-10"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
												<div class="text-right excel_outer">
													<cc1:WordExcelButton ID="WordExcelButton2" runat="server" GridView="ToWordExcel" ViewStateMode="Enabled" class="NoPrint" Style="display: inline;" />
												</div>
											</div>
										</div>
									</div>
								</div>
							</asp:Panel>


							<!--//依區屬搜尋//--->
							<asp:Panel ID="Panel_MCTeamList" runat="server" Visible="true">

								<h3 class="text-center mt-20">
									<asp:Label ID="LB_HTitle" runat="server" Text="" Font-Size="Large" Font-Bold="true"></asp:Label>
								</h3>

								<div class="table-responsive" id="ToWordExcel" runat="server">
									<table class="table m-b-0 table-hover m-t-20">
										<thead>
											<tr>
												<th class="text-center font-weight-bold" style="width: 8%">大區</th>
												<th class="text-center font-weight-bold" style="width: 8%">區屬</th>
												<th class="text-center font-weight-bold" style="width: 10%">光團</th>
												<th class="text-center font-weight-bold" style="width: 5%">期別</th>
												<th class="text-center font-weight-bold" style="width: 12%">姓名</th>
												<th class="text-center font-weight-bold" style="width: 8%">學員類別</th>
												<th class="text-center font-weight-bold" style="width: 5%">體系</th>
												<th class="text-center font-weight-bold" style="width: 10%">天命法位</th>
												<th class="text-center font-weight-bold" style="width: 10%">上線團</th>
												<th class="text-center font-weight-bold" style="width: 12%">帶領導師</th>
												<th class="text-center font-weight-bold" style="width: 12%">上線傳導師</th>
											</tr>
										</thead>
										<tbody>



											<asp:SqlDataSource ID="SDS_TotalListArea" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
											<asp:Repeater ID="Rpt_TotalListArea" runat="server" DataSourceID="SDS_TotalListArea" OnItemDataBound="Rpt_TotalListArea_ItemDataBound">
												<ItemTemplate>


													<tr style="background: #e0e0e0">
														<td colspan="13">
															<h4 class="text-center mb-0">
																<asp:Label ID="LB_HArea" runat="server" Text='<%# Eval("HArea") %>'></asp:Label>
																<asp:Label ID="LB_Count" runat="server" Text=""></asp:Label>
															</h4>
														</td>
													</tr>


													<asp:SqlDataSource ID="SDS_TotalListAreaChild" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
													<asp:Repeater ID="Rpt_TotalListAreaChild" runat="server" OnItemDataBound="Rpt_TotalListAreaChild_ItemDataBound" DataSourceID="SDS_TotalListAreaChild">
														<ItemTemplate>

															<tr>
																<td class="text-center">
																	<asp:Label ID="LB_HLArea" runat="server" Text='<%# Eval("HLArea") %>'></asp:Label>
																</td>
																<td class="text-center">
																	<asp:Label ID="LB_HArea" runat="server" Text='<%# Eval("HArea") %>'></asp:Label>
																</td>
																<td class="text-center">
																	<asp:Label ID="LB_HTeamID" runat="server" Text='<%# Eval("HTeamID") %>'></asp:Label>
																</td>
																<td class="text-center">&nbsp;
                                                                    <asp:Label ID="LB_HPeriod" runat="server" Text='<%# Eval("HPeriod") %>'></asp:Label>
																</td>
																<td class="text-center">
																	<asp:Label ID="LB_HUserName" runat="server" Text='<%# Eval("HUserName") %>'></asp:Label>
																</td>
																<td class="text-center">
																	<asp:Label ID="LB_HType" runat="server" Text='<%# Eval("HTypeName") %>'></asp:Label>
																</td>
																<td class="text-center">
																	<asp:Label ID="LB_HSystem" runat="server" Text='<%# Eval("HSystemName") %>'></asp:Label>
																</td>
																<td class="text-center">
																	<asp:Label ID="LB_HPosition" runat="server" Text='<%# Eval("HRName") %>'></asp:Label>
																</td>
																<td class="text-center">
																	<asp:Label ID="LB_MTeam" runat="server" Text=""></asp:Label>
																</td>
																<td class="text-center">
																	<asp:Label ID="LB_CTLeader" runat="server" Text=""></asp:Label>
																</td>
																<td class="text-center">
																	<asp:Label ID="LB_ATLeader" runat="server" Text=""></asp:Label>
																</td>

															</tr>

														</ItemTemplate>
													</asp:Repeater>



												</ItemTemplate>
											</asp:Repeater>


										</tbody>
									</table>







								</div>
								<!------------------分頁功能開始------------------>
								<div class="box text-right">
									<Page:Paging runat="server" ID="Pg_TotalListArea" />
								</div>
								<!------------------分頁功能結束------------------>

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

</asp:Content>

