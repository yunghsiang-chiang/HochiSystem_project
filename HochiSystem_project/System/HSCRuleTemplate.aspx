<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HSCRuleTemplate.aspx.cs" Inherits="System_HSCRuleTemplate" %>

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
            <div class="block-header">
                <div class="row">
                    <div class="col-lg-3 col-md-12 col-sm-4">
                        <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>版規範本</h2>
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="HMember_Edit.aspx"><i class="icon-home"></i></a></li>
                            <li class="breadcrumb-item">版規範本</li>
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
                                <div class="col-md-12">
                                    <div class="form-group row">
                                        <label class="control-label text-right col-md-2 text-middle">版規內容</label>
                                        <div class="col-md-8">
                                            <CKEditor:CKEditorControl ID="CKE_HSCRule" runat="server" class="form-control" Text=""></CKEditor:CKEditorControl>
                                        </div>
                                    </div>
                                </div>
                                <!--/span-->
                            </div>
                        
                            <div class="text-center">
                                <asp:Button ID="Btn_Submit" runat="server" Text="儲存" CssClass="btn btn-success" OnClick="Btn_Submit_Click"/>
                                <asp:Button ID="Btn_Cancel" runat="server" Text="取消" CssClass="btn btn-inverse" OnClick="Btn_Cancel_Click" Btmessage="確定要取消嗎？沒有儲存的内容將不會變更" OnClientClick='return confirm(this.getAttribute("btmessage"))'/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        

        <!-- ============================================================== -->
        <!-- End Page wrapper  -->
        <!-- ============================================================== -->
    </div>
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
    <!--Custom JavaScript -->
    <script src="dist/js/custom.min.js"></script>

    <script src="../js/dropify/js/dropify.min.js"></script>

    <script>
        $(document).ready(function () {
            // Basic
            $('.dropify').dropify();
        });
    </script>
    <script type="text/javascript">
        function BrowsePic() {
            document.getElementById("NewUpload").innerText = "选择了新档案，如要取消请按[REMOVE]";
        }
    </script>



</asp:Content>

