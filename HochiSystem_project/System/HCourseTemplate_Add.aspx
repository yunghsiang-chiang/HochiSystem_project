<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeFile="HCourseTemplate_Add.aspx.cs" Inherits="HCourseTemplate_Add" ValidateRequest="false" %>

<%-- ValidateRequest="false" --%>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .form-material .form-group {
            overflow: inherit;
        }

        .nav-tabs .nav-link {
            border: 1px solid #c9c9c9;
            border-radius: 0.25rem;
            margin: 0 5px 5px 0;
        }

            .nav-tabs .nav-link.active, .nav-tabs .nav-item.show .nav-link {
                color: #fff;
                background-color: #8d63db;
                border-color: #dee2e6 #dee2e6 #fff;
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
                    <div class="col-lg-6 col-md-12 col-sm-12">
                        <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>新增課程範本<%--(Add Course Template)--%></h2>
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                            <li class="breadcrumb-item"><a href="HCourseTemplate_Edit.aspx">課程範本管理<%--(Course Template)--%></a></li>
                            <li class="breadcrumb-item active">新增課程範本<%--(Add Course Template)--%></li>
                        </ul>
                    </div>
                </div>
            </div>
            <!-- ============================================================== -->
            <!-- End Bread crumb and right sidebar toggle -->
            <!-- ============================================================== -->
            <!-- ============================================================== -->
            <!-- Start Page Content -->
            <!-- ============================================================== -->
            <div class="row">
                <div class="col-12">
                    <div class="card">
                        <div class="card-body">

                            <div class="col-12 d-flex align-items-center justify-content-between p-10" style="box-shadow: 0 0 3px rgba(0,0,0,.2)">
                                <div>
                                    <span class="mr-10">填寫完各項資料後，再按下【儲存】儲存您的設定。<%--(Remember to click "Save" button to save your changes.)--%></span>
                                </div>
                                <div>
                                    <asp:Button ID="Button1" runat="server" Text="儲存" class="btn btn-info m-r-10" OnClick="Btn_Submit_Click" />
                                    <asp:Button ID="Button2" runat="server" Text="取消" class="btn btn-inverse" OnClick="Btn_Cancel_Click" Btmessage="確定要取消嗎？沒有儲存的内容將不會變更" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                                    <%--(Are you sure to leave this page? The information won't be saved until you plugin save button.)--%>
                                </div>
                            </div>

                            <div class="row mt-2  d-none">
                                <div class="col-12">
                                    <div>
                                        <label class="text-info">*請勿輸入以下危險字元:&nbsp;&nbsp;-&nbsp;、&nbsp;,&nbsp;、&nbsp;;&nbsp;、&nbsp;/&nbsp;、&nbsp;|&nbsp;、&nbsp;}&nbsp;、&nbsp;{&nbsp;、&nbsp;%&nbsp;、&nbsp;@&nbsp;、&nbsp;*&nbsp;、&nbsp;!&nbsp;、&nbsp;'&nbsp;&nbsp;</label>
                                    </div>
                                </div>
                            </div>

                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" Visible="true" Style="width: 100%;">
                                <ContentTemplate>

                                    <div class="mt-2">
                                        <asp:Label ID="LB_NavTab" runat="server" Text="" Visible="false"></asp:Label><!--目前在哪個tab-->

                                        <ul class="nav nav-tabs" id="ParaTab" role="tablist">

                                            <asp:SqlDataSource ID="SDS_Tag" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                            <asp:Repeater ID="RPT_Tag" runat="server">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="HF_HID" runat="server" Value='<%# Eval("HID") %>' />
                                                    <li class="nav-item">
                                                        <asp:LinkButton ID="LBtn_Tag" runat="server" class="nav-link font-weight-bold" TabIndex='<%#Convert.ToInt32(Eval("HID")) %>' OnClick="LBtn_NavTab_Click">
                                                                       <span class="hidden-sm-up"></span><span class="hidden-xs-down"><%# Eval("HName_TW") %></span>
                                                        </asp:LinkButton>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>

                                    </div>


                                    <div class="tab-content">

                                        <!--課程範本基本資訊 START-->
                                        <asp:Panel ID="Panel_Template" runat="server" class="tab-pane p-0  fade show active" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">

                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        <asp:Label ID="LB_HID" runat="server" Text="" Visible="false"></asp:Label>
                                                        課程範本編號 <span class="text-danger">*系統產生</span>
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HTemplateNum" runat="server" class="form-control form-control-line" placeholder="由系統產生" Enabled="false"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold" for="example-text">
                                                        課程類別<span class="text-danger">*</span>
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:DropDownList ID="DDL_HType" runat="server" class="form-control js-example-basic-single" Style="width: 100%" AutoPostBack="true" OnSelectedIndexChanged="DDL_HType_SelectedIndexChanged">
                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="form-group col-md-6">
                                                    <label class="font-weight-bold">
                                                        課程範本名稱<span class="text-danger">*</span>
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HTemplateName" runat="server" class="form-control form-control-line" placeholder="" AutoComplete="off"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        是否為檢覈課程
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:RadioButtonList ID="RBL_TestCourse" runat="server" RepeatDirection="Horizontal" Enabled="false">
                                                            <asp:ListItem Value="0" Selected="True" Style="margin-right: 10px;">否</asp:ListItem>
                                                            <asp:ListItem Value="1">是</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        是否為帶狀課程
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:RadioButtonList ID="RBL_HSerial" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="RBL_HSerial_SelectedIndexChanged" AutoPostBack="true">
                                                            <asp:ListItem Value="0" Selected="True" Style="margin-right: 10px;">否</asp:ListItem>
                                                            <asp:ListItem Value="1">是</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        需要前導課程
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:RadioButtonList ID="RBL_HNLCourse" runat="server" RepeatDirection="Horizontal">
                                                            <asp:ListItem Value="0" Selected="True" Style="margin-right: 10px;">否</asp:ListItem>
                                                            <asp:ListItem Value="1">是</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        需要護持者
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:RadioButtonList ID="RBL_HNGuide" runat="server" RepeatDirection="Horizontal">
                                                            <asp:ListItem Value="0" Selected="True" Style="margin-right: 10px;">否</asp:ListItem>
                                                            <asp:ListItem Value="1">是</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        課程須全到，才能通過課程
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:RadioButtonList ID="RBL_HNFull" runat="server" RepeatDirection="Horizontal">
                                                            <asp:ListItem Value="0" Selected="True" Style="margin-right: 10px;">否</asp:ListItem>
                                                            <asp:ListItem Value="1">是</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        是否須提供經費預算表
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:RadioButtonList ID="RBL_HBudget" runat="server" RepeatDirection="Horizontal">
                                                            <asp:ListItem Value="0" Selected="True" Style="margin-right: 10px;">否</asp:ListItem>
                                                            <asp:ListItem Value="1">是</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3 d-none">
                                                    <label class="font-weight-bold">
                                                        是否為軸線課程
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:RadioButtonList ID="RBL_HAxisYN" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="RBL_HAxisYN_SelectedIndexChanged" AutoPostBack="true">
                                                            <asp:ListItem Value="0" Selected="True" Style="margin-right: 10px;">否</asp:ListItem>
                                                            <asp:ListItem Value="1">是</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3 d-none">
                                                    <label class="font-weight-bold">
                                                        軸線類別<span class="text-danger" id="Span_HAxisClass" runat="server" visible="false">*</span>
                                                    </label>
                                                    <div class="form-group">
                                                        <%--<asp:SqlDataSource ID="SDS_HAxisClass" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HExamSubject FROM HExamSubject WHERE HStatus=1"></asp:SqlDataSource>--%>
                                                        <asp:DropDownList ID="DDL_HAxisClass" runat="server" class="form-control js-example-basic-single" Style="width: 100%" Enabled="false">
                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                            <asp:ListItem Value="1">軸線類別A</asp:ListItem>
                                                            <asp:ListItem Value="2">軸線類別B</asp:ListItem>
                                                            <asp:ListItem Value="3">軸線類別C</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <%--DataSourceID="SDS_HAxisClass" DataTextField="HAxisClass" DataValueField="HID" AppendDataBoundItems="true"--%>
                                                    </div>
                                                </div>


                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        是否須提供住宿
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:RadioButtonList ID="RBL_HLodging" runat="server" RepeatDirection="Horizontal">
                                                            <asp:ListItem Value="0" Selected="True" Style="margin-right: 10px;">否</asp:ListItem>
                                                            <asp:ListItem Value="1">是</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>


                                                <!--AA20250609-->
                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        是否開放單天報名
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:RadioButtonList ID="RBL_HBookByDateYN" runat="server" RepeatDirection="Horizontal">
                                                            <asp:ListItem Value="0" Selected="True" Style="margin-right: 10px;">否</asp:ListItem>
                                                            <asp:ListItem Value="1">是</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        奉獻專區是否需導向信用卡授權書填寫
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:RadioButtonList ID="RBL_HCCPeriodYN" runat="server" RepeatDirection="Horizontal">
                                                            <asp:ListItem Value="0" Selected="true" Style="margin-right: 10px;">否</asp:ListItem>
                                                            <asp:ListItem Value="1">是</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>



                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        課程報名截止日
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:Label ID="LB_HCDeadlineDayTitle" runat="server" Text="課程開始日前"></asp:Label>&nbsp
                                                        <asp:TextBox ID="TB_HCDeadlineDay" runat="server" class="form-control text-right col-md-12 text-center" placeholder="僅限輸入數字" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Text="7" Width="40%"></asp:TextBox>&nbsp;天
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3 d-none">
                                                    <label class="font-weight-bold">
                                                        換課/地點/身分的期限<span class="text-danger"></span>
                                                    </label>
                                                    <div class="form-group input-group">
                                                        上課起始日前 &nbsp<asp:TextBox ID="TB_HCDeadline" runat="server" class="form-control text-left col-md-12" placeholder="請輸入天數(限數字)" AutoComplete="off" Text="3" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>
                                                    </div>
                                                </div>



                                                <asp:SqlDataSource ID="SDS_HSystem" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold" for="example-text">
                                                        可開課之體系
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:DropDownList ID="DDL_HOSystem" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSystem" DataTextField="HSystemName" DataValueField="HID">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        可報名之體系
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:ListBox ID="LBox_HRSystem" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" DataSourceID="SDS_HSystem" DataTextField="HSystemName" DataValueField="HID"></asp:ListBox>
                                                    </div>
                                                </div>
                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        受傳過之法條件
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:SqlDataSource ID="SDS_HDharma" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:ListBox ID="LBox_HNRequirement" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" DataSourceID="SDS_HDharma" DataTextField="HDharmaName" DataValueField="HID"></asp:ListBox>
                                                    </div>
                                                </div>
                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        學員類別限制
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:SqlDataSource ID="SDS_HMType" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:ListBox ID="LBox_HIRestriction" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" DataSourceID="SDS_HMType" DataTextField="HMType" DataValueField="HID"></asp:ListBox>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        繳費帳戶<span class="text-danger">*</span>
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:DropDownList ID="DDL_HPMethod" runat="server" class="form-control js-example-basic-single" Style="width: 100%" AutoPostBack="true" OnSelectedIndexChanged="DDL_HPMethod_SelectedIndexChanged">
                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                            <asp:ListItem Value="1">基金會</asp:ListItem>
                                                            <asp:ListItem Value="2">文化事業</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>

                                                <asp:SqlDataSource ID="SDS_HBudgetType" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold" for="example-text">
                                                        預算類別<span class="text-danger" id="Required" runat="server" visible="false">*</span>
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:DropDownList ID="DDL_HBudgetType" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HBudgetType" DataTextField="HContent" DataValueField="HID" AppendDataBoundItems="True" Enabled="false">
                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        參與課程基本金額(元)<span class="text-danger">*</span>
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HBCPoint" runat="server" class="form-control text-right col-md-12" placeholder="請輸入金額(限數字)" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        問卷
                                                    </label>
                                                    <div class="form-group input-group">
                                                        <asp:ListBox ID="LBox_HQuestionID" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" Style="width: 100%;"></asp:ListBox>

                                                        <div class="input-group-append">
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-6 d-none">
                                                    <label class="font-weight-bold">
                                                        講師教材
                                                    </label>
                                                    <div class="form-group input-group">
                                                        <asp:ListBox ID="LBox_HTMaterialID" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" Style="width: 100%;" DataSourceID="SDS_HTMaterial" DataTextField="HTMName" DataValueField="HID"></asp:ListBox>

                                                        <div class="input-group-append">
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="form-group col-md-3 d-none">
                                                    <label class="font-weight-bold">
                                                        體系護持人員名單
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:RadioButtonList ID="RBL_HSGList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                            <asp:ListItem Value="0" Selected="True">不須審核</asp:ListItem>
                                                            <asp:ListItem Value="1">須審核</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                                <div class="form-group col-md-3 d-none">
                                                    <label class="font-weight-bold">
                                                        主班團隊
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:ListBox ID="LBox_HTeam" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" Width="100%"></asp:ListBox>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold" for="example-text">
                                                        檢覈內容名稱
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:SqlDataSource ID="SDS_HExamContentName" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:DropDownList ID="DDL_HExamContentID" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HExamContentName" DataTextField="HExamContentName" DataValueField="HID" AppendDataBoundItems="true" Enabled="true">
                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3 d-none" runat="server" id="Div_HExamSubject" visible="true">
                                                    <label class="font-weight-bold" for="example-text">
                                                        檢覈科目名稱<span class="text-danger" id="Span_HExamSubject" runat="server" visible="false">*</span>
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:SqlDataSource ID="SDS_HExamSubject" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:DropDownList ID="DDL_HExamSubject" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HExamSubject" DataTextField="HExamSubjectName" DataValueField="HID" AppendDataBoundItems="true" Enabled="false">
                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>


                                                <div class="form-group col-md-3 d-none">
                                                    <label class="font-weight-bold">
                                                        報名人數上限
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HParticipantLimit" runat="server" class="form-control col-md-12" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-6">
                                                    <label class="font-weight-bold">
                                                        備註
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HRemark" runat="server" class="form-control col-md-12" AutoComplete="off"></asp:TextBox>
                                                    </div>
                                                </div>


                                            </div>
                                        </asp:Panel>
                                        <!--課程範本基本資訊 END-->

                                        <!--內文 START-->
                                        <asp:Panel ID="Panel_Content" runat="server" class="tab-pane p-0 fade" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">
                                                <div class="form-group col-md-12">
                                                    <label class="font-weight-bold">標題</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HContentTitle" runat="server" class="form-control"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-12">
                                                    <label class="font-weight-bold">內容</label>
                                                    <div class="form-group">
                                                        <CKEditor:CKEditorControl ID="CKE_HContent" runat="server"></CKEditor:CKEditorControl>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <!--內文 END-->

                                        <!--學員課程教材 START-->
                                        <asp:Panel ID="Panel_Material" runat="server" class="tab-pane p-0 fade" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">
                                                <%--  <asp:UpdatePanel ID="UPanel_HCourseMaterial_T" runat="server" Visible="true" Style="width: 100%;">
                                                    <ContentTemplate>--%>
                                                <%--<asp:LinkButton ID="LBtn_HCourseMaterial_T" runat="server" class="col-12 btn btn-secondary text-left moreinfo" OnClick="LBtn_HCourseMaterial_T_Click" Visible="false">
												<i class="ti-angle-down"></i>
												<label class="m-b-0 font-weight-bold">教材設定</label>
												</asp:LinkButton> --%>
                                                <%--<asp:Panel ID="Panel_HCourseMaterial_T" runat="server" Visible="true">--%>
                                                <div class="col-md-12 mb-4">
                                                    <%--hideinfo --%>
                                                    <div class="table-responsive">
                                                        <table class="table table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center" style="width: 5%">執行<%--<br />(Action)--%></th>
                                                                    <th class="text-center" style="width: 5%">No</th>
                                                                    <th style="width: 25%">教材名稱<%--<br />(Title)--%></th>
                                                                    <th style="width: 25%">上傳教材<%--<br />(Uploads) --%><span class="text-danger">(限PDF、mp3檔；上限200Mb)</span></th>
                                                                    <th style="width: 30%">影片/其他教材連結<%--<br />(Related Links)--%></th>
                                                                    <th style="width: 10%">排序</th>
                                                                </tr>
                                                            </thead>


                                                            <tbody>
                                                                <tr>
                                                                    <td style="text-align: center;">
                                                                        <%--<a class="p-10" title="新增"><i class="ti-plus text-info"></i></a>--%>
                                                                        <asp:LinkButton ID="LBtn_HCourseMaterial_T_add" runat="server" OnClick="LBtn_HCourseMaterial_T_add_Click" CausesValidation="false" Text="" class="ti-plus text-info"></asp:LinkButton><!--教材設定新增-->

                                                                    </td>
                                                                    <td class="text-center"></td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HCMName" runat="server" class="form-control" placeholder="請輸入教材名稱"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:FileUpload ID="FU_HCMaterial" runat="server" /><%-- onchange="BrowseFile()"--%>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HCMLink" runat="server" class="form-control" placeholder="請輸入影片/其他教材連結"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HSort" runat="server" Text="" class="form-control" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Style="text-align: center"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </tbody>


                                                            <asp:SqlDataSource ID="SDS_HCourseMaterial_T" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:Repeater ID="Rpt_HCourseMaterial_T" runat="server" DataSourceID="SDS_HCourseMaterial_T" OnItemDataBound="Rpt_HCourseMaterial_T_ItemDataBound">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="text-align: center;">
                                                                                <asp:LinkButton ID="LBtn_HCourseMaterial_T_Del" runat="server" OnClick="LBtn_HCourseMaterial_T_Del_Click" CommandArgument='<%# Eval("HID") %>'><i class="ti-close text-danger"></i></asp:LinkButton>
                                                                            </td>
                                                                            <td class="text-center">
                                                                                <asp:Label ID="LB_HNo" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_HCMName" runat="server" class="form-control" placeholder="請輸入教材名稱" Text='<%# Eval("HCMName") %>'></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:FileUpload ID="FU_HCMaterial" runat="server" class="dropify" /><%-- onchange="BrowseFile()"--%>
                                                                                <asp:HyperLink ID="HL_HCMaterial" runat="server" Visible="false" Text='<%# Eval("HCMaterial") %>'></asp:HyperLink>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_HCMLink" runat="server" class="form-control" placeholder="請輸入影片/其他教材連結" Text='<%# Eval("HCMLink") %>'></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_HSort" runat="server" Text='<%# Eval("HSort") %>' class="form-control" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Style="text-align: center"></asp:TextBox>
                                                                                <asp:Label ID="LB_HSort" runat="server" Text='<%# Eval("HSort") %>' Visible="false"></asp:Label>
                                                                            </td>

                                                                        </tr>
                                                                    </tbody>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </table>
                                                    </div>
                                                </div>
                                                <%--</asp:Panel>--%>
                                                <%--  </ContentTemplate>
                                                    <Triggers>
                                                        <asp:PostBackTrigger ControlID="LBtn_HCourseMaterial_T_add" />
                                                    </Triggers>
                                                </asp:UpdatePanel>--%>
                                            </div>
                                        </asp:Panel>
                                        <!--學員課程教材 END-->

                                        <!--體系護持工作項目 START-->
                                        <asp:Panel ID="Panel_Jobs" runat="server" class="tab-pane p-0 fade" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">
                                                <%--  <asp:UpdatePanel ID="UPanel_HTodoList_T" runat="server" Visible="true" Style="width: 100%;">
                                                    <ContentTemplate>--%>
                                                <%--<asp:LinkButton ID="LBtn_HTodoList_T" runat="server" class="col-12 btn btn-secondary text-left moreinfo" OnClick="LBtn_HTodoList_T_Click">
												<i class="ti-angle-down"></i>
												<label class="m-b-0 font-weight-bold">體系護持工作項目設定</label>
												</asp:LinkButton>
												<asp:Panel ID="Panel_HTodoList_T" runat="server" Visible="false">--%>
                                                <div class="col-md-12 mb-4">
                                                    <div class="table-responsive">
                                                        <table class="table table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center" style="width: 5%">執行</th>
                                                                    <th class="text-center" style="width: 5%">No</th>
                                                                    <th style="width: 20%">所屬組別</th>
                                                                    <th style="width: 15%">任務職稱</th>
                                                                    <th class="d-none" style="width: 10%">人數</th>
                                                                    <th style="width: 50%">體系護持工作項目</th>
                                                                </tr>
                                                            </thead>

                                                            <tbody>
                                                                <tr>
                                                                    <td style="text-align: center;">
                                                                        <%--<a class="p-10" title="新增"><i class="ti-plus text-info"></i></a>--%>
                                                                        <asp:LinkButton ID="LBtn_HTodoList_T_add" runat="server" OnClick="LBtn_HTodoList_T_add_Click" CausesValidation="false" Text="" class="ti-plus text-info"></asp:LinkButton><!--前導課程設定新增-->
                                                                    </td>
                                                                    <td class="text-center"></td>
                                                                    <td>
                                                                        <asp:DropDownList ID="DDL_HGroupName" runat="server" class="form-control js-example-basic-single" Style="width: 100%" AutoPostBack="true" OnSelectedIndexChanged="DDL_HGroupName_SelectedIndexChanged">
                                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="DDL_HTask" runat="server" class="form-control js-example-basic-single" Style="width: 100%" AutoPostBack="true" OnSelectedIndexChanged="DDL_HTask_SelectedIndexChanged" Enabled="false">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td class="text-right d-none">
                                                                        <asp:TextBox ID="TB_HTaskNum" runat="server" class="form-control" placeholder="" Text="" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" MaxLength="3"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HTaskContent" runat="server" class="form-control" placeholder=""></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </tbody>


                                                            <asp:SqlDataSource ID="SDS_HTodoList_T" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:Repeater ID="Rpt_HTodoList_T" runat="server" DataSourceID="SDS_HTodoList_T" OnItemDataBound="Rpt_HTodoList_T_ItemDataBound">
                                                                <ItemTemplate>
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="text-align: center;">
                                                                                <asp:LinkButton ID="LBtn_HTodoList_T_Del" runat="server" OnClick="LBtn_HTodoList_T_Del_Click" CommandArgument='<%# Eval("HID") %>'><i class="ti-close text-danger"></i></asp:LinkButton>
                                                                            </td>
                                                                            <td class="text-center"></td>
                                                                            <td>
                                                                                <asp:DropDownList ID="DDL_HGroupName" runat="server" class="form-control js-example-basic-single" Style="width: 100%" Enabled="false">
                                                                                    <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td>
                                                                                <asp:DropDownList ID="DDL_HTask" runat="server" class="form-control js-example-basic-single" Style="width: 100%" Enabled="false">
                                                                                    <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td class="text-right d-none">
                                                                                <asp:TextBox ID="TB_HTaskNum" runat="server" class="form-control" placeholder="" Text='<%# Eval("HTaskNum") %>' Enabled="false"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_HTaskContent" runat="server" class="form-control" placeholder="" Text='<%# Eval("HTaskContent") %>'></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                    </tbody>
                                                                </ItemTemplate>
                                                            </asp:Repeater>


                                                        </table>
                                                    </div>
                                                </div>
                                                <%--</asp:Panel>--%>
                                                <%-- </ContentTemplate>
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="LBtn_HTodoList_T_add" EventName="Click" />
                                                        <asp:AsyncPostBackTrigger ControlID="DDL_HGroupName" EventName="SelectedIndexChanged" />
                                                    </Triggers>
                                                </asp:UpdatePanel>--%>
                                            </div>
                                        </asp:Panel>
                                        <!--體系護持工作項目 END-->

                                        <!--作業 START-->
                                        <asp:Panel ID="Panel_Homework" runat="server" class="tab-pane p-0 fade" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">
                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        功課須繳交篇數與天數
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HNCWSheet" runat="server" class="form-control form-control-line text-center" Width="40%" placeholder="" AutoComplete="off"></asp:TextBox>&nbsp;篇
												<asp:TextBox ID="TB_HNCWDay" runat="server" class="form-control form-control-line text-center" Width="40%" placeholder="" AutoComplete="off"></asp:TextBox>&nbsp;天
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <!--作業 END-->

                                        <!--講師教材 START-->
                                        <asp:Panel ID="Panel_HTMaterial" runat="server" class="tab-pane p-0 fade" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">
                                                <div class="col-md-12 mb-4">
                                                    <div class="table-responsive">
                                                        <table class="table table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center" style="width: 5%">執行</th>
                                                                    <th class="text-center" style="width: 5%">No</th>
                                                                    <th style="width: 30%">講師教材名稱</th>
                                                                    <th class="text-center" style="width: 10%">排序</th>
                                                                </tr>
                                                            </thead>

                                                            <tbody>
                                                                <tr>
                                                                    <td style="text-align: center;">
                                                                        <asp:LinkButton ID="LBtn_HTMaterial_add" runat="server" CausesValidation="false" Text="" class="ti-plus text-info" OnClick="LBtn_HTMaterial_add_Click"></asp:LinkButton>
                                                                    </td>
                                                                    <td class="text-center"></td>
                                                                    <td>
                                                                        <asp:SqlDataSource ID="SDS_HTMaterial" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                                        <asp:DropDownList ID="DDL_HTMaterial" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HTMaterial" DataTextField="HTMName" DataValueField="HID" AppendDataBoundItems="true">
                                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:TextBox ID="TB_HTMaterialSort" runat="server" class="form-control" placeholder="" Text="" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Style="text-align: center"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </tbody>


                                                            <asp:SqlDataSource ID="SDS_HTMaterial_T" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:Repeater ID="Rpt_HTMaterial_T" runat="server" DataSourceID="SDS_HTMaterial_T" OnItemDataBound="Rpt_HTMaterial_T_ItemDataBound">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="text-align: center;">
                                                                                <asp:LinkButton ID="LBtn_HTMaterial_Del" runat="server" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_HTMaterial_Del_Click"><i class="ti-close text-danger"></i></asp:LinkButton>
                                                                            </td>
                                                                            <td class="text-center"></td>
                                                                            <td>
                                                                                <asp:Label ID="LB_HTMaterialID" runat="server" CssClass="p-l-10" Text='<%# Eval("HTMaterialID") %>' Visible="false"></asp:Label>
                                                                                <asp:DropDownList ID="DDL_HTMaterial" runat="server" class="form-control js-example-basic-single" Style="width: 100%" Enabled="false" DataSourceID="SDS_HTMaterial" DataTextField="HTMName" DataValueField="HID" AppendDataBoundItems="true">
                                                                                    <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td class="text-center">
                                                                                <asp:Label ID="LB_HTMaterialSort" runat="server" Text='<%# Eval("HSort") %>' Visible="false"></asp:Label>
                                                                                <asp:TextBox ID="TB_HTMaterialSort" runat="server" class="form-control" placeholder="" Text='<%# Eval("HSort") %>' onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Style="text-align: center"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                    </tbody>
                                                                </ItemTemplate>
                                                            </asp:Repeater>


                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <!--講師教材 END-->

                                    </div>

                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="LBtn_HCourseMaterial_T_add" />
                                    <asp:PostBackTrigger ControlID="LBtn_HTodoList_T_add" />
                                    <asp:PostBackTrigger ControlID="LBtn_HTMaterial_add" />
                                    <asp:AsyncPostBackTrigger ControlID="DDL_HGroupName" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="RBL_HSerial" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="RBL_HAxisYN" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>

                            <div class="col-12 align-items-center justify-content-between p-10 d-none" style="box-shadow: 0 0 3px rgba(0,0,0,.2)">
                                <div>
                                    <span class="mr-10">填寫完各項資料後，再按下【儲存】儲存您的設定。</span>
                                </div>
                                <div>
                                    <asp:Button ID="Btn_Submit" runat="server" Text="儲存" class="btn btn-info m-r-10" OnClick="Btn_Submit_Click" />
                                    <asp:Button ID="Btn_Cancel" runat="server" Text="取消" class="btn btn-inverse" OnClick="Btn_Cancel_Click" Btmessage="確定要取消嗎？沒有儲存的内容將不會變更" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
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
    <%--    <!--stickey kit -->
    <script src="assets/node_modules/sticky-kit-master/dist/sticky-kit.min.js"></script>
    <script src="assets/node_modules/sparkline/jquery.sparkline.min.js"></script>--%>
    <!--Custom JavaScript -->
    <script src="dist/js/custom.min.js"></script>
    <!--datepicker-->
    <%--<script src="js/bootstrap-datepicker.js"></script>--%>
    <!--Select2-->
    <script src="js/select2.min.js"></script>
    <!--sumoselect-->
    <script src="js/jquery.sumoselect.min.js"></script>

    <!-- icheck -->
    <script src="assets/node_modules/icheck/icheck.min.js"></script>
    <script src="assets/node_modules/icheck/icheck.init.js"></script>


    <!--ckeditor-->
    <script src="../js/config.js"></script>



    <script>
        $(function () {
            //單選
            $('.js-example-basic-single').select2();
        });
    </script>

    <!--dropify js-->
    <script src="js/dropify.min.js"></script>

    <script>
        $(document).ready(function () {
            $('.dropify').dropify();
        });
    </script>

    <script>
        $(function () {
            $('.ListB_Multi').SumoSelect({
                search: true,
                placeholder: '-請選擇-',
                csvDispCount: 5,
            });


            <%--//可報名體系
            var regsystem = document.getElementById('<%=LB_HRSystem.ClientID %>').innerText;
            var regData = regsystem.split(',');
            for (let i = 0; i < regData.length; i++) {
                $(<%=LBox_HRSystem.ClientID%>)[0].sumo.selectItem(regData[i]);
            }

            //受傳過之法條件
            var regsystem = document.getElementById('<%=LB_HNRequirement.ClientID %>').innerText;
            var regData = regsystem.split(',');
            for (let i = 0; i < regData.length; i++) {
                $(<%=LBox_HNRequirement.ClientID%>)[0].sumo.selectItem(regData[i]);
            }

            //主班團隊
            var team = document.getElementById('<%=LB_HTeam.ClientID %>').innerText;
            var words = team.split(',');
            for (let i = 0; i < words.length; i++) {
                $(<%=LBox_HTeam.ClientID%>)[0].sumo.selectItem(words[i]);
            }

            //身份限制
            var team = document.getElementById('<%=LB_HIRestriction.ClientID %>').innerText;
            var words = team.split(',');
            for (let i = 0; i < words.length; i++) {
                $(<%=LBox_HIRestriction.ClientID%>)[0].sumo.selectItem(words[i]);
            }--%>

        });
    </script>

    <%--  <script>
        $(document).ready(function () {
            $('.datepicker').datepicker({
                format: 'yyyy/mm/dd',
                autoclose: true,
                toggleActive: false,
                todayHighlight: true,
                orientation: 'bottom auto',
                //startDate: new Date(),
            });
        });
    </script>--%>
    <script type="text/javascript">
        function BrowseFile() {
            document.getElementById("NewUpload").innerText = "選擇了新檔案，如要取消請按[REMOVE]";
        }


    </script>


    <%--縮放改為用UpdatePanel %>
<%--    <script>
        $('.moreinfo').click(function (e) {
            e.preventDefault();
            var notthis = $('.active').not(this);
            notthis.find('.ti-angle-down').addClass('ti-angle-up').removeClass('ti-angle-down');
            notthis.toggleClass('active').next('.hideinfo').slideToggle(300);
            $(this).toggleClass('active').next().slideToggle("fast");
            $(this).children('i').toggleClass('ti-angle-up ti-angle-down');
        });
    </script>--%>
</asp:Content>

