<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HFResponseReport.aspx.cs" Inherits="HFResponseReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

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
                          <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>體悟分享護持者回應統計表</h2>
                          <ul class="breadcrumb">
                              <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                              <li class="breadcrumb-item active">體悟分享護持者回應統計表</li>
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
                                

                                          <div class="col-md-2 p-r-0 p-l-0 mr-2">
                                              <asp:TextBox ID="TB_HMemberUserName" runat="server" class="form-control" placeholder="同修/新朋友姓名" AutoComplete="off"></asp:TextBox>
                                          </div>

                                          <div class="col-md-2 p-r-0 p-l-0 mr-2">
                                              <asp:TextBox ID="TB_MentorUsername" runat="server" class="form-control p-l-1" placeholder="護持者姓名" AutoComplete="off"></asp:TextBox>
                                          </div>

                                          <div class="col-md-2 p-r-0 p-l-0 mr-2">
                                              <asp:DropDownList ID="DDL_HArea" runat="server" class="form-control js-example-basic-single">
                                              </asp:DropDownList>
                                          </div>

                                      
                                          <div class="col-md-2 p-r-0 p-l-0 excel_outer" style="max-width: 10%; display: flex; align-items: center;">
                                              <asp:LinkButton ID="LBtn_Search" runat="server" OnClick="LBtn_Search_Click" class="btn btn-outline-secondary"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                              <asp:LinkButton ID="LBtn_SearchCancel" runat="server" OnClick="LBtn_SearchCancel_Click" class="btn btn-outline-secondary m-l-10"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                          </div>

                                      </div>

                                  </div>
                              </div>

                            

                          </div>





                              <div class="body">
                                  <div class="table-responsive">
                                      <table class="table m-b-0 table-hover table-bordered">
                                          <thead>
                                              <tr class="font-weight-bold">
                                                  <th style="width: 15%">學員姓名</th>
                                                  <th class="text-center" style="width: 12%">學員分享篇數</th>
                                                  <th style="width: 15%">護持者姓名</th>
                                                  <th class="text-center" style="width: 15%">護持者回應篇數</th>
                                                  <th class="text-center" style="width: 15%">護持者回應比例</th>
                                              </tr>
                                          </thead>
                                          <tbody>
                                              <asp:SqlDataSource ID="SDS_HFeelingsPost" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                              <asp:Repeater ID="Rpt_HFeelingsPost" runat="server" DataSourceID="SDS_HFeelingsPost" OnItemDataBound="Rpt_HFeelingsPost_ItemDataBound">
                                                  <ItemTemplate>
                                                      <tr id="Tr_Row" runat="server">
                                                          <td class="d-none">
                                                              <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                          </td>
                                                          <td>
                                                              <asp:Label ID="LB_MemberName" runat="server" Text='<%# Eval("MemberMemberList") %>'></asp:Label>
                                                              <asp:HiddenField ID="HF_MemberArea" runat="server" Value='<%# Eval("MemberArea") %>' />
                                                          </td>
                                                          <td class="text-center">
                                                              <asp:Label ID="LB_TotalPosts" runat="server" Text='<%# Eval("TotalPosts") %>'></asp:Label>
                                                          </td>
                                                          <td>
                                                              <asp:Label ID="LB_MentorName" runat="server" Text='<%# Eval("MentorMemberList") %>'></asp:Label>
                                                              <asp:HiddenField ID="HF_MentorArea" runat="server" Value='<%# Eval("MentorArea") %>' />
                                                          </td>
                                                          <td class="text-center">
                                                              <asp:Label ID="LB_MentorRepliedPosts" runat="server" Text='<%# Eval("MentorRepliedPosts") %>'></asp:Label>
                                                          </td>
                                                           <td class="text-center">
                                                               <asp:Label ID="LB_MentorReplyRatePct" runat="server" Text='<%# Eval("MentorReplyRatePct") %>'></asp:Label>
                                                          </td>
                                                         

                                                      </tr>
                                                  </ItemTemplate>
                                              </asp:Repeater>
                                          </tbody>
                                      </table>

                                      <!------------------分頁功能開始------------------>
<%--                                      <nav class="list-pagination">
                                          <Page:Paging runat="server" ID="Pg_Paging" />
                                      </nav>--%>
                                      <!------------------分頁功能結束------------------>







                                  </div>
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


      });
  </script>



</asp:Content>

