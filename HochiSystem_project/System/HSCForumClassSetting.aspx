<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HSCForumClassSetting.aspx.cs" Inherits="System_HSCForumClassSetting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <link href="css/customsortable.css" rel="stylesheet" />
    <script src="js/Sortable.min.js"></script>


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



            <div class="block-header">
                <div class="row">
                    <div class="col-lg-6 col-md-12 col-sm-12">
                        <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>專欄MENU設定</h2>
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="HSCParameter.aspx"><i class="icon-home"></i></a></li>
                            <li class="breadcrumb-item">專欄MENU設定</li>
                        </ul>
                    </div>
                </div>





            </div>





            <div class="row">
                <div class="col-12">
                    <div class="card">
                        <div class="card-body">


                            <div class="row ">
                                <div class="col-12 d-flex align-items-center justify-content-between">
                                    <a class="btn btn-info text-white" data-toggle="modal" data-target="#Div_SCFourmClassAdd">
                                        <span class="ti-plus"></span>新增討論區
                                    </a>

                                    <div>
                                        <asp:Button ID="Btn_UpdateOrder" runat="server" CssClass="btn btn-success" Text="更新排序" OnClientClick="saveOrderToHiddenField();" OnClick="Btn_UpdateOrder_Click" />
                                        <asp:Button ID="Btn_Cancel" runat="server" CssClass="btn btn-gray text-white" Text="取消" OnClick="Btn_Cancel_Click" />

                                    </div>
                                </div>
                            </div>

                            <hr />

                            <!--------------------==================討論區分類層級( START========================--------------->



                            <asp:HiddenField ID="hfOrder" runat="server" />
                            <!-- 讓我們把新的階層結構(JSON)存到這個 hidden field，後端即可讀取 -->

                            <div id="nestable">
                                <ol id="topList" class="sortable-list">
                                    <!-- 第一層項目 -->

                                    <asp:SqlDataSource ID="SDS_HSCForumClassFirst" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCFCMaster, HSCFCName AS HSCForumClassA, HSCFCLevel, HStatus, HSort FROM HSCForumClass WHERE HSCFCLevel='10' AND HSCFCMaster='0' ORDER BY HSort ASC, HStatus DESC"></asp:SqlDataSource>
                                    <asp:Repeater ID="Rpt_HSCForumClassFirst" runat="server" DataSourceID="SDS_HSCForumClassFirst" OnItemDataBound="Rpt_HSCForumClassFirst_ItemDataBound">
                                        <ItemTemplate>

                                            <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>

                                            <li class="sortable-item" data-id='<%# Eval("HID") %>' data-sort='<%# Eval("HSort") %>' data-master='<%# Eval("HSCFCMaster") %>' data-level='<%# Eval("HSCFCLevel") %>'>
                                                <!-- item-header: 放拖曳 + content(含 toggle + 標題 + 啟用/停用) -->
                                                <div class="item-header">
                                                    <div class="drag-handle">☰</div>
                                                    <div class="content">
                                                        <div class="content-left">
                                                            <div class="toggle-btn" id="Btn_Toggle" runat="server">+</div>
                                                            <div>
                                                                <%# Eval("HSCForumClassA") %>
                                                                <asp:LinkButton ID="LBtn_EditSCForumClassA" runat="server" OnClick="LBtn_EditSCForumClassA_Click" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Eval("HSCForumClassA") %>'>  
      <span class="ti-pencil text-purple"></span>
                                                                </asp:LinkButton>
                                                            </div>
                                                        </div>
                                                        <div>
                                                            <asp:Label ID="LB_HStatus" runat="server" Text='<%# Eval("HStatus") %>'></asp:Label>
                                                            <asp:Button ID="Btn_SCForumClassA_Disabled" runat="server" class="btn btn-sm btn-danger ml-3" Text="停用" CommandArgument='<%# Eval("HID") %>' OnClick="Btn_SCForumClassA_Disabled_Click" />
                                                            <asp:Button ID="Btn_SCForumClassA_Enabled" runat="server" Text="啟用" class="btn btn-sm btn-success ml-3" OnClick="Btn_SCForumClassA_Enabled_Click" CommandArgument='<%# Eval("HID") %>' Visible="false" />
                                                        </div>
                                                    </div>
                                                </div>



                                                <ol class="sortable-list collapsible">
                                                    <asp:SqlDataSource ID="SDS_HSCForumClassSecond" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                    <asp:Repeater ID="Rpt_HSCForumClassSecond" runat="server" DataSourceID="SDS_HSCForumClassSecond" OnItemDataBound="Rpt_HSCForumClassSecond_ItemDataBound">
                                                        <ItemTemplate>
                                                            <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                            <!-- 子清單(預設收合) -->
                                                            <!-- 第二層項目 -->
                                                            <li class="sortable-item" data-id='<%# Eval("HID") %>' data-sort='<%# Eval("HSort") %>' data-master='<%# Eval("HSCFCMaster") %>' data-level='<%# Eval("HSCFCLevel") %>'>
                                                                <div class="item-header">
                                                                    <div class="drag-handle">☰</div>
                                                                    <div class="content">
                                                                        <div class="content-left">
                                                                            <div class="toggle-btn" id="Btn_Toggle" runat="server">+</div>
                                                                            <div>
                                                                                <%# Eval("HSCForumClassB") %>

                                                                                <asp:LinkButton ID="LBtn_EditSCForumClassB" runat="server" OnClick="LBtn_EditSCForumClassB_Click" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Eval("HSCForumClassB") %>'>  
<span class="ti-pencil text-purple"></span>
                                                                                </asp:LinkButton>
                                                                            </div>
                                                                        </div>
                                                                        <div>
                                                                            <asp:Label ID="LB_HStatus" runat="server" Text='<%# Eval("HStatus") %>'></asp:Label>
                                                                            <%--<span class="status text-success">公開</span>--%>
                                                                            <asp:Button ID="Btn_SCForumClassB_Disabled" runat="server" class="btn btn-sm btn-danger ml-3" Text="停用" CommandArgument='<%# Eval("HID") %>' OnClick="Btn_SCForumClassB_Disabled_Click" />
                                                                            <asp:Button ID="Btn_SCForumClassB_Enabled" runat="server" Text="啟用" class="btn btn-sm btn-success ml-3" OnClick="Btn_SCForumClassB_Enabled_Click" CommandArgument='<%# Eval("HID") %>' Visible="false" />
                                                                        </div>
                                                                    </div>
                                                                </div>


                                                                <!-- 第三層 -->
                                                                <ol class="sortable-list collapsible">
                                                                    <asp:SqlDataSource ID="SDS_HSCForumClassThird" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                                    <asp:Repeater ID="Rpt_HSCForumClassThird" runat="server" DataSourceID="SDS_HSCForumClassThird" OnItemDataBound="Rpt_HSCForumClassThird_ItemDataBound">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                                            <!-- 第三層項目 1 -->
                                                                            <li class="sortable-item" data-id='<%# Eval("HID") %>' data-sort='<%# Eval("HSort") %>' data-master='<%# Eval("HSCFCMaster") %>' data-level='<%# Eval("HSCFCLevel") %>'>
                                                                                <div class="item-header">
                                                                                    <div class="drag-handle">☰</div>
                                                                                    <div class="content">
                                                                                        <div class="content-left">
                                                                                            <!-- 如果此層沒有子清單，可選擇不放 toggle-btn -->
                                                                                            <div>
                                                                                                <%# Eval("HSCForumClassC") %>
                                                                                                <asp:LinkButton ID="LBtn_EditSCForumClassC" runat="server" OnClick="LBtn_EditSCForumClassC_Click" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Eval("HSCForumClassC") %>'>  
<span class="ti-pencil text-purple"></span>
                                                                                                </asp:LinkButton>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div>
                                                                                            <asp:Label ID="LB_HPublic" runat="server" Style="margin-right: 20px" Text='<%# Eval("HPublic") %>'></asp:Label>
                                                                                            <asp:Label ID="LB_HStatus" runat="server" Text='<%# Eval("HStatus") %>'></asp:Label>
                                                                                            <%--<span class="status text-success">公開</span>--%>
                                                                                            <asp:Button ID="Btn_SCForumClassC_Disabled" runat="server" class="btn btn-sm btn-danger ml-3" Text="停用" CommandArgument='<%# Eval("HID") %>' OnClick="Btn_SCForumClassC_Disabled_Click" />
                                                                                            <asp:Button ID="Btn_SCForumClassC_Enabled" runat="server" Text="啟用" class="btn btn-sm btn-success ml-3" OnClick="Btn_SCForumClassC_Enabled_Click" CommandArgument='<%# Eval("HID") %>' Visible="false" />
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </li>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                                </ol>
                                                            </li>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </ol>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ol>
                            </div>


















                            <!--------------------==================討論區分類層級( END========================--------------->




                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>








    <!--==================新增討論區的畫面==================--->
    <div id="Div_SCFourmClassAdd" class="modal fade hochi-modal grecord-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 60%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0" id="exampleModalLongTitle">新增討論區</h5>

                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" onclick="window.location.reload();">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="modal-body" style="width: 100%; padding: 10px">

                    <div class="p-0">
                        <div class="row clearfix pl-2 pr-2">
                            <div class="col-lg-12 col-md-12">

                                <div class="form-group">

                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <div class="col-md-12 mb-2">
                                                <div class="row clearfix">
                                                    <div class="col-md-12 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            <span class="text-danger">*</span>討論區主類別</label>
                                                        <div class="form-group input-group">
                                                            <asp:SqlDataSource ID="SDS_HSCForumClassA" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCFCName AS HSCForumClassA, HStatus FROM HSCForumClass WHERE HSCFCLevel='10' AND HSCFCMaster='0' AND HStatus=1 ORDER BY HSort ASC"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HSCForumClassA" runat="server" CssClass="form-control  js-example-basic-single" DataSourceID="SDS_HSCForumClassA" DataTextField="HSCForumClassA" DataValueField="HID" OnSelectedIndexChanged="DDL_HSCForumClassA_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true" Style="width: 80%">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <div class="input-group-append">
                                                                <asp:LinkButton ID="LBtn_AddSCForumClassA" runat="server" CssClass="input-group-text" OnClick="LBtn_AddSCForumClassA_Click">
                                                                     <span class="ti-plus"></span>
                                                                </asp:LinkButton>
                                                                <%-- data-toggle="modal" data-target="#Div_AddSCForumClassA"--%>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>


                                                <div class="row clearfix">
                                                    <div class="col-md-12 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            <span class="text-danger">*</span>討論區次類別</label>
                                                        <div class="form-group input-group">
                                                            <asp:SqlDataSource ID="SDS_HSCForumClassB" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HSCForumClassB" runat="server" CssClass="form-control js-example-basic-single" DataSourceID="SDS_HSCForumClassB" DataTextField="HSCForumClassB" DataValueField="HID" AppendDataBoundItems="true" Style="width: 80%">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <div class="input-group-append">

                                                                <asp:LinkButton ID="LBtn_AddSCForumClassB" runat="server" CssClass="input-group-text" OnClick="LBtn_AddSCForumClassB_Click"><%--data-toggle="modal" data-target="#Div_AddSCForumClassB"--%>
       <span class="ti-plus"></span>
                                                                </asp:LinkButton>


                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>

                                                <div class="row clearfix">
                                                    <div class="col-md-12 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            <span class="text-danger">*</span>討論區名稱</label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HSCForumClassC" runat="server" CssClass="form-control" AutoComplete="off" Style="width: 80%"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>


                                                <div class="row clearfix">
                                                    <div class="col-md-12 col-sm-12">
                                                        <label class="font-weight-bold">
                                                            <span class="text-danger">*</span>公開設定</label>
                                                        <p class="mb-1">【說明】預設為公開(與課程相關的討論區)，若是內部、與課程無關的討論區請選擇不公開</p>
                                                        <div class="form-group">
                                                            <asp:RadioButtonList ID="RBL_HPublic" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                                <asp:ListItem Value="1" Selected Style="margin-right: 10px;">公開</asp:ListItem>
                                                                <asp:ListItem Value="0">不公開 </asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>

                                                </div>

                                            </div>


                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="DDL_HSCForumClassA" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>

                                </div>
                            </div>
                        </div>
                    </div>


                    <div class="modal-footer justify-content-center text-center">
                        <asp:Button ID="Btn_Submit" runat="server" class="btn btn-success" Text="儲存" UseSubmitBehavior="false" OnClick="Btn_Submit_Click" />
                        <asp:Button ID="Btn_CloseModal" runat="server" class="btn btn-gray" OnClick="Btn_CloseModal_Click" Text="取消" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal  END-->


    <!--==================修改名稱==================--->
    <div id="Div_SCForumClassName" class="modal fade hochi-modal grecord-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 40%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0">修改專欄Menu名稱</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" onclick="window.location.reload();">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="modal-body" style="width: 100%; padding: 10px">

                    <div class="p-0">
                        <div class="row clearfix pl-2 pr-2">
                            <div class="col-lg-12 col-md-12">
                                <div class="form-group">
                                    <div class="col-md-12 mb-2">
                                        <div class="row clearfix">
                                            <div class="col-md-12 col-sm-12">
                                                <div class="form-group">
                                                    <asp:Label ID="LB_HID" runat="server" Text="" Visible="false"></asp:Label>
                                                    <asp:TextBox ID="TB_HSCFourmClassName" runat="server" CssClass="form-control" AutoComplete="off" Style="width: 100%"></asp:TextBox>
                                                </div>
                                            </div>

                                        </div>

                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>


                    <div class="modal-footer justify-content-center text-center">
                        <asp:Button ID="Btn_SCForumClassName_Save" runat="server" class="btn btn-success" Text="儲存" UseSubmitBehavior="false" OnClick="Btn_SCForumClassName_Save_Click" />
                        <asp:Button ID="Btn_SCForumClassName_Cancel" runat="server" class="btn btn-gray" Text="取消" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal  END-->

    <!--==================新增討論區主類別==================--->
    <div id="Div_AddSCForumClassA" class="modal fade hochi-modal grecord-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 40%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0">新增討論區主類別</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" onclick="window.location.reload();">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="modal-body" style="width: 100%; padding: 10px">

                    <div class="p-0">
                        <div class="row clearfix pl-2 pr-2">
                            <div class="col-lg-12 col-md-12">
                                <div class="form-group">
                                    <div class="col-md-12 mb-2">
                                        <div class="row clearfix">
                                            <div class="col-md-12 col-sm-12">
                                                <div class="form-group">
                                                    <asp:TextBox ID="TB_HSCFCNameA" runat="server" class="form-control" Width="100%" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>

                                        </div>

                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>


                    <div class="modal-footer justify-content-center text-center">
                        <asp:Button ID="Btn_SCFClassASubmit" runat="server" class="btn btn-success" Text="儲存" UseSubmitBehavior="false" OnClick="Btn_SCFClassASubmit_Click" />
                        <asp:Button ID="Button2" runat="server" class="btn btn-gray" Text="取消" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal  END-->


    <!--==================新增討論區次類別==================--->
    <div id="Div_AddSCForumClassB" class="modal fade hochi-modal grecord-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 40%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0">新增討論區次類別</h5>

                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" onclick="window.location.reload();">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="modal-body" style="width: 100%; padding: 10px">

                    <div class="p-0">
                        <div class="row clearfix pl-2 pr-2">

                            <div class="col-lg-12 col-md-12">
                                <div class="form-group">
                                    <%--<asp:SqlDataSource ID="SDS_HSCFCMasterA" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCFCName, HStatus FROM HSCForumClass WHERE HSCFCLevel='10' AND HStatus='1'"></asp:SqlDataSource>--%>
                                    <label class="font-weight-bold">
                                        <span class="text-danger">*</span>討論區主類別</label>
                                    <asp:DropDownList ID="DDL_HSCFCMasterA" runat="server" CssClass="form-control  js-example-basic-single" DataSourceID="SDS_HSCForumClassA" DataTextField="HSCForumClassA" DataValueField="HID" AppendDataBoundItems="true" Style="width: 100%">
                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>



                            <div class="col-lg-12 col-md-12">
                                <div class="form-group">
                                    <label class="font-weight-bold">
                                        <span class="text-danger">*</span>討論區次類別名稱</label>
                                    <asp:TextBox ID="TB_HSCFCNameB" runat="server" class="form-control" Width="100%" AutoComplete="off"></asp:TextBox>
                                </div>
                            </div>


                        </div>
                    </div>


                    <div class="modal-footer justify-content-center text-center">
                        <asp:Button ID="Btn_SCFClassBSubmit" runat="server" class="btn btn-success" Text="儲存" UseSubmitBehavior="false" OnClick="Btn_SCFClassBSubmit_Click" />
                        <asp:Button ID="Button3" runat="server" class="btn btn-gray" Text="取消" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal  END-->



    <script src="js/jquery-3.3.1.min.js"></script>
    <!-- Bootstrap tether Core JavaScript -->
    <script src="assets/node_modules/popper/popper.min.js"></script>
    <script src="assets/node_modules/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- slimscrollbar scrollbar JavaScript -->
    <script src="dist/js/perfect-scrollbar.jquery.min.js"></script>
    <!-- datepicker -->
    <script src="js/bootstrap-datepicker.js"></script>
    <!--Wave Effects -->
    <script src="dist/js/waves.js"></script>
    <!--Menu sidebar -->
    <script src="dist/js/sidebarmenu.js"></script>
    <!--stickey kit -->
    <script src="assets/node_modules/sticky-kit-master/dist/sticky-kit.min.js"></script>
    <script src="assets/node_modules/sparkline/jquery.sparkline.min.js"></script>
    <!--Custom JavaScript -->
    <script src="dist/js/custom.min.js"></script>
    <!--select2-->
    <script src="js/select2.min.js"></script>
    <!--Custom JavaScript -->
    <script src="js/_custom.js"></script>





    <script>
        // === 初始化 Sortable (主清單) ===
        const nestable = document.getElementById('nestable');
        Sortable.create(nestable, {
            group: 'nested',
            animation: 150,
            fallbackOnBody: true,
            swapThreshold: 0.65,
            handle: '.drag-handle'
        });

        // === 初始化 Sortable (內層所有清單) ===
        document.querySelectorAll('.sortable-list').forEach(list => {
            Sortable.create(list, {
                group: 'nested',
                animation: 150,
                fallbackOnBody: true,
                swapThreshold: 0.65,
                handle: '.drag-handle'
            });
        });

        // === 收合 / 展開 (垂直) ===
        document.querySelectorAll('.toggle-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                // 找到所在 .sortable-item
                const listItem = btn.closest('.sortable-item');
                // 該 .sortable-item 下方是否有 <ol> 子清單
                const sublist = listItem.querySelector(':scope > .sortable-list');
                // :scope > .sortable-list 可確保只抓到「當前層級的子清單」

                if (sublist) {
                    const isCollapsed = sublist.classList.contains('collapsible');
                    sublist.classList.toggle('collapsible', !isCollapsed);
                    sublist.classList.toggle('expanded', isCollapsed);
                    btn.textContent = isCollapsed ? '-' : '+';
                }
            });
        });
</script>

    <script>
        // 取得清單結構
        function getOrder() {
            const topList = document.getElementById("topList");
            return serializeList(topList, 0, 10); // parent=0(表示無父層), level=10(最外層從 level = 10 開始)
        }

        // 遞迴遍歷 <ol> 裡的 <li>
        function serializeList(list, parentId, level) {
            let order = [];
            // 僅抓取當前清單 (list) 直接子項目的 <li>
            const items = list.querySelectorAll(':scope > li.sortable-item');
            items.forEach((li, index) => {
                const nodeId = parseInt(li.getAttribute('data-id'), 10);
                // 本層資訊
                const itemData = {
                    Id: nodeId,
                    Master: parentId,
                    Level: level,
                    Sort: index  // 或 index + 1，看你想從 0 還是 1 開始
                };
                // 檢查是否有子 <ol>
                const subList = li.querySelector(':scope > ol.sortable-list');
                if (subList) {
                    // 如果有子清單，下一層的 Level = level + 10
                    itemData.Children = serializeList(subList, nodeId, level + 10);
                } else {
                    itemData.Children = [];
                }
                order.push(itemData);
            });
            return order;
        }

        // 把結果存在 hidden field，以便後端能讀取
        function saveOrderToHiddenField() {
            const data = getOrder();
            const jsonData = JSON.stringify(data);
            document.getElementById('<%= hfOrder.ClientID %>').value = jsonData;
        }
</script>

</asp:Content>

