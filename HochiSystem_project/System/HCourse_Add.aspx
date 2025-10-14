<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeFile="HCourse_Add.aspx.cs" Inherits="HCourse_Add" ValidateRequest="false" %>

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

        /*-遮罩--*/
        #loadingOverlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(255,255,255,0.85); /* 白色半透明背景 */
            z-index: 9999;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .loadingImage {
            width: 150px; /* 可自行調整大小 */
            height: 150px;
            display: block;
        }

        .loadingText {
            color: #8d63db;
            font-size: 1.5em;
            font-weight: bold;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">


    <!-- 遮罩 -->
    <div id="loadingOverlay">
        <img src="images/loading.gif" alt="載入中..." class="loadingImage" />
        <div class="loadingText">資料讀取中，請稍後~</div>
    </div>


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
                        <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>新增課程</h2>
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                            <li class="breadcrumb-item"><a href="HCourse_Edit.aspx">開課管理</a></li>
                            <li class="breadcrumb-item active">新增課程</li>
                        </ul>
                    </div>
                </div>
            </div>
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
                                    <asp:Button ID="Btn_Submit" runat="server" Text="儲存" class="btn btn-info m-r-10" OnClick="Btn_Submit_Click" />
                                    <asp:Button ID="Btn_Verify" runat="server" Text="送審" class="btn btn-danger m-r-10" OnClick="Btn_Verify_Click" />
                                    <asp:Button ID="Btn_Cancel" runat="server" Text="取消" class="btn btn-inverse" OnClick="Btn_Cancel_Click" Btmessage="確定要取消嗎？沒有儲存的内容將不會變更" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                                </div>
                            </div>

                            <div class="row mt-2  d-none">
                                <div class="col-12">
                                    <div>
                                        <label class="text-info">*請勿輸入以下危險字元:&nbsp;&nbsp;-&nbsp;、&nbsp;,&nbsp;、&nbsp;;&nbsp;、&nbsp;/&nbsp;、&nbsp;|&nbsp;、&nbsp;}&nbsp;、&nbsp;{&nbsp;、&nbsp;%&nbsp;、&nbsp;@&nbsp;、&nbsp;*&nbsp;、&nbsp;!&nbsp;、&nbsp;'&nbsp;&nbsp;</label>
                                    </div>
                                </div>
                            </div>

                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" Visible="true" Style="width: 100%;">
                                <ContentTemplate>

                                    <div class="mt-3">
                                        <asp:Label ID="LB_NavTab" runat="server" Text="" Visible="false"></asp:Label><!--目前在哪個tab-->
                                        <ul class="nav nav-tabs" role="tablist">
                                            <li class="nav-item">
                                                <asp:LinkButton ID="LBtn_Template" runat="server" class="nav-link" OnClick="LBtn_NavTab_Click" TabIndex="1">
											 <span class="hidden-sm-up"></span><span class="hidden-xs-down">課程範本資訊</span>
                                                </asp:LinkButton>
                                            </li>
                                            <li class="nav-item">
                                                <asp:LinkButton ID="LBtn_Course" runat="server" class="nav-link" OnClick="LBtn_NavTab_Click" TabIndex="2">
											 <span class="hidden-sm-up"></span><span class="hidden-xs-down">開課資訊</span>
                                                </asp:LinkButton>
                                            </li>
                                            <li class="nav-item">
                                                <asp:LinkButton ID="LBtn_Introduction" runat="server" class="nav-link" OnClick="LBtn_NavTab_Click" TabIndex="3">
											 <span class="hidden-sm-up"></span><span class="hidden-xs-down">內文</span>
                                                </asp:LinkButton>
                                            </li>
                                            <li class="nav-item">
                                                <asp:LinkButton ID="LBtn_Material" runat="server" class="nav-link" OnClick="LBtn_NavTab_Click" TabIndex="4">
											 <span class="hidden-sm-up"></span><span class="hidden-xs-down">學員課程教材</span>
                                                </asp:LinkButton>
                                            </li>
                                            <li class="nav-item">
                                                <asp:LinkButton ID="LBtn_LeadCourse" runat="server" class="nav-link" OnClick="LBtn_NavTab_Click" TabIndex="5">
											 <span class="hidden-sm-up"></span><span class="hidden-xs-down">前導課程</span>
                                                </asp:LinkButton>
                                            </li>
                                            <li class="nav-item">
                                                <asp:LinkButton ID="LBtn_SupportJob" runat="server" class="nav-link" OnClick="LBtn_NavTab_Click" TabIndex="6">
											 <span class="hidden-sm-up"></span><span class="hidden-xs-down">體系護持工作項目</span>
                                                </asp:LinkButton>
                                            </li>
                                            <li class="nav-item">
                                                <asp:LinkButton ID="LBtn_Notice" runat="server" class="nav-link" OnClick="LBtn_NavTab_Click" TabIndex="7">
											 <span class="hidden-sm-up"></span><span class="hidden-xs-down">報名須知/條款內容</span>
                                                </asp:LinkButton>
                                            </li>
                                            <li class="nav-item">
                                                <asp:LinkButton ID="LBtn_Homework" runat="server" class="nav-link" OnClick="LBtn_NavTab_Click" TabIndex="8">
											 <span class="hidden-sm-up"></span><span class="hidden-xs-down">作業</span>
                                                </asp:LinkButton>
                                            </li>
                                            <li class="nav-item">
                                                <asp:LinkButton ID="LBtn_Related" runat="server" class="nav-link" OnClick="LBtn_NavTab_Click" TabIndex="9">
											 <span class="hidden-sm-up"></span><span class="hidden-xs-down">相關文件</span>
                                                </asp:LinkButton>
                                            </li>
                                            <li class="nav-item d-none">
                                                <asp:LinkButton ID="LBtn_Evaluation" runat="server" class="nav-link" OnClick="LBtn_NavTab_Click" TabIndex="10">
		                                   	 <span class="hidden-sm-up"></span><span class="hidden-xs-down">檢覈評比項目</span>
                                                </asp:LinkButton>
                                            </li>
                                            <li class="nav-item">
                                                <asp:LinkButton ID="LBtn_HTMaterial" runat="server" class="nav-link" OnClick="LBtn_NavTab_Click" TabIndex="11">
		                                   	 <span class="hidden-sm-up"></span><span class="hidden-xs-down">講師教材</span>
                                                </asp:LinkButton>
                                            </li>
                                        </ul>
                                    </div>

                                    <asp:Label ID="LB_HID" runat="server" Text="" Visible="false"></asp:Label>
                                    <asp:Label ID="LB_HCBatchNum" runat="server" Text="" Visible="false"></asp:Label>

                                    <div class="tab-content">
                                        <!--課程範本基本資訊 START-->
                                        <asp:Panel ID="Panel_Template" runat="server" class="tab-pane p-0  fade show active" role="tabpanel">
                                            <asp:Panel ID="Panel_EditFalse" runat="server">
                                                <div class="form-horizontal m-t-10 row">

                                                    <div class="form-group col-md-9">
                                                        <label class="font-weight-bold">
                                                            課程範本名稱<span class="text-danger">*</span>
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HCourseTemplate" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HCourseTemplate" runat="server" class="form-control js-example-basic-single" Style="width: 100%" OnSelectedIndexChanged="DDL_CourseTemplate_SelectedIndexChanged" AutoPostBack="true" DataSourceID="SDS_HCourseTemplate" DataTextField="HTNumName" DataValueField="HID" AppendDataBoundItems="true">
                                                                <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold" for="example-text">
                                                            課程類別
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HType" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HType" runat="server" class="form-control js-example-basic-single" Style="width: 100%; height: 38px" Enabled="false" DataSourceID="SDS_HType" DataTextField="HCourseName" DataValueField="HID" AppendDataBoundItems="true">
                                                                <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold">
                                                            是否為檢覈課程
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:RadioButtonList ID="RBL_TestCourse" runat="server" RepeatDirection="Horizontal" Enabled="false" RepeatLayout="Flow">
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
                                                            <asp:RadioButtonList ID="RBL_HSerial" runat="server" RepeatDirection="Horizontal" Enabled="false">
                                                                <asp:ListItem Value="0" Selected="True" Style="margin-right: 10px;">否(N)</asp:ListItem>
                                                                <asp:ListItem Value="1">是(Y)</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>

                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold">
                                                            需要前導課程
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:RadioButtonList ID="RBL_HNLCourse" runat="server" RepeatDirection="Horizontal" Enabled="false" RepeatLayout="Flow">
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
                                                            <asp:RadioButtonList ID="RBL_HNGuide" runat="server" RepeatDirection="Horizontal" Enabled="false" RepeatLayout="Flow">
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
                                                            <asp:RadioButtonList ID="RBL_HNFull" runat="server" RepeatDirection="Horizontal" Enabled="false" RepeatLayout="Flow">
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
                                                            <asp:RadioButtonList ID="RBL_HBudget" runat="server" RepeatDirection="Horizontal" Enabled="false" RepeatLayout="Flow">
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
                                                            <asp:RadioButtonList ID="RBL_HAxisYN" runat="server" RepeatDirection="Horizontal" Enabled="false">
                                                                <asp:ListItem Value="0" Selected="True" Style="margin-right: 10px;">否</asp:ListItem>
                                                                <asp:ListItem Value="1">是</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>

                                                    <div class="form-group col-md-3 d-none">
                                                        <label class="font-weight-bold">
                                                            軸線類別
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:DropDownList ID="DDL_HAxisClass" runat="server" class="form-control js-example-basic-single" Style="width: 100%" Enabled="false">
                                                                <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>


                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold">
                                                            是否須提供住宿
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:RadioButtonList ID="RBL_HLodging" runat="server" RepeatDirection="Horizontal" Enabled="false" RepeatLayout="Flow">
                                                                <asp:ListItem Value="0" Selected="True" Style="margin-right: 10px;">否</asp:ListItem>
                                                                <asp:ListItem Value="1">是</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>

                                                    <!--AA20250611-->
                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold">
                                                            是否開放單天報名
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:RadioButtonList ID="RBL_HBookByDateYN" runat="server" RepeatDirection="Horizontal" Enabled="false">
                                                                <asp:ListItem Value="0" Style="margin-right: 10px;">否</asp:ListItem>
                                                                <asp:ListItem Value="1">是</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>

                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold">
                                                            奉獻專區是否需導向信用卡授權書填寫
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:RadioButtonList ID="RBL_HCCPeriodYN" runat="server" RepeatDirection="Horizontal" Enabled="false">
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
                                                        <asp:TextBox ID="TB_HCDeadlineDay" runat="server" class="form-control text-right col-md-12 text-center" placeholder="僅限輸入數字" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Text="7" Width="30%" Enabled="false"></asp:TextBox>&nbsp;天
                                                        </div>
                                                    </div>


                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold">
                                                            換課/地點/身分的期限
                                                        </label>
                                                        <div class="form-group">
                                                            上課起始日前 &nbsp;<asp:TextBox ID="TB_HCDeadline" runat="server" class="form-control text-center col-md-12" placeholder="" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false" Width="35%"></asp:TextBox>
                                                            &nbsp;天
                                                        </div>
                                                    </div>



                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold" for="example-text">
                                                            可開課之體系
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HSystem" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HOSystem" runat="server" class="form-control js-example-basic-single" Style="width: 100%; height: 38px" Enabled="false" DataSourceID="SDS_HSystem" DataTextField="HSystemName" DataValueField="HID">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold">
                                                            可報名之體系
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:Label ID="LB_HRSystem" runat="server" Visible="false"></asp:Label>
                                                            <asp:ListBox ID="LBox_HRSystem" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" DataSourceID="SDS_HSystem" DataTextField="HSystemName" DataValueField="HID" Enabled="false"></asp:ListBox>
                                                        </div>
                                                    </div>


                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold">
                                                            受傳過之法條件
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HDharma" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:ListBox ID="LBox_HNRequirement" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" DataSourceID="SDS_HDharma" DataTextField="HDharmaName" DataValueField="HID" Enabled="false" AppendDataBoundItems="true"></asp:ListBox>
                                                            <div class="d-none">
                                                                <asp:Label ID="LB_HNRequirement" runat="server" Text=""></asp:Label>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold">
                                                            學員類別限制
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HMType" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:ListBox ID="LBox_HIRestriction" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" Enabled="false" DataSourceID="SDS_HMType" DataTextField="HMType" DataValueField="HID"></asp:ListBox>
                                                            <div class="d-none">
                                                                <asp:Label ID="LB_HIRestriction" runat="server" Text=""></asp:Label>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold">
                                                            繳費帳戶
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:DropDownList ID="DDL_HPMethod" runat="server" class="form-control js-example-basic-single" Style="width: 100%" Enabled="false">
                                                                <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                <asp:ListItem Value="1">基金會</asp:ListItem>
                                                                <asp:ListItem Value="2">文化事業</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>


                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold" for="example-text">
                                                            預算類別
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HBudgetType" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HBudgetType" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HBudgetType" DataTextField="HContent" DataValueField="HID" AppendDataBoundItems="True" Enabled="false">
                                                                <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>


                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold">
                                                            參與課程基本金額(元)
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HBCPoint" runat="server" class="form-control text-right col-md-12" placeholder="僅限輸入數字" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>
                                                        </div>
                                                    </div>


                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold">
                                                            問卷
                                                        </label>
                                                        <div class="form-group input-group">
                                                            <asp:SqlDataSource ID="SDS_HQuestionID" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:ListBox ID="LBox_HQuestionID" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" Style="width: 100%;" Enabled="false" DataSourceID="SDS_HQuestionID" DataTextField="HTitle" DataValueField="HID"></asp:ListBox>

                                                            <div class="input-group-append d-none">
                                                                <a data-toggle="modal" data-target="#SearchRoles" class="input-group-text"><i class="fa fa-search"></i></a>
                                                                <a data-toggle="modal" data-target="#AddRoles" class="input-group-text"><i class="fa fa-plus"></i></a>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="form-group col-md-6 d-none">
                                                        <label class="font-weight-bold">
                                                            講師教材
                                                        </label>
                                                        <div class="form-group input-group">
                                                            <%--<asp:SqlDataSource ID="SDS_HTMaterialID" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>--%>
                                                            <asp:ListBox ID="LBox_HTMaterialID" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" Style="width: 100%;" Enabled="false" AppendDataBoundItems="True">
                                                                <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                            </asp:ListBox>
                                                            <%--DataSourceID="SDS_HTMaterialID" DataTextField="HTMName" DataValueField="HID"--%>
                                                            <div class="input-group-append">
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="form-group col-md-3 d-none">
                                                        <label class="font-weight-bold">
                                                            體系護持人員名單
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:RadioButtonList ID="RBL_HSGList" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Value="0">不須審核</asp:ListItem>
                                                                <asp:ListItem Value="1">須審核</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>




                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold" for="example-text">
                                                            檢覈內容名稱
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HExamContentName" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HExamContentID" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HExamContentName" DataTextField="HExamContentName" DataValueField="HID" AppendDataBoundItems="true" Enabled="false">
                                                                <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="form-group col-md-3 d-none" runat="server" id="Div_HExamSubject" visible="true">
                                                        <label class="font-weight-bold" for="example-text">
                                                            檢覈科目名稱
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HExamSubject" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HExamSubject" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HExamSubject" DataTextField="HExamSubject" DataValueField="HID" AppendDataBoundItems="true" Enabled="false">
                                                                <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="form-group col-md-3">
                                                        <label class="font-weight-bold">
                                                            報名人數上限
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HParticipantLimit" runat="server" class="form-control col-md-12" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="false"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="form-group col-md-12">
                                                        <label class="font-weight-bold">
                                                            備註
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HRemark" runat="server" class="form-control col-md-12" TextMode="MultiLine" Rows="3" AutoComplete="off"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>

                                            </asp:Panel>
                                        </asp:Panel>
                                        <!--課程範本基本資訊 END-->

                                        <!--課程開課資訊 START-->
                                        <asp:Panel ID="Panel_Course" runat="server" class="tab-pane p-0 fade" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">

                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        課程代碼
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HCBatchNum" runat="server" class="form-control form-control-line" placeholder="由系統產生" Enabled="false" Style="width: 100%; height: 38px"></asp:TextBox>
                                                        <asp:TextBox ID="TB_HCourseNum" runat="server" class="form-control form-control-line" placeholder="由系統產生" Enabled="false" Style="width: 100%; height: 38px" Visible="false"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-9">
                                                    <label class="font-weight-bold">
                                                        課程名稱<span class="text-danger">*</span>
                                                        <a class="text-info font-weight-bold" data-toggle="modal" data-target="#info" style="cursor: pointer; font-size: large"><i class="fa ti-help-alt"></i>&nbsp;</a>
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HCourseName" runat="server" class="form-control form-control-line" placeholder="" AutoComplete="off"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-12">
                                                    <label class="font-weight-bold">
                                                        課程講師
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:ListBox ID="LBox_HTeacherName" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple"></asp:ListBox>

                                                    </div>
                                                </div>

                                                <div class="form-group col-md-12">
                                                    <label class="font-weight-bold">
                                                        主班團隊
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:SqlDataSource ID="SDS_HTeam" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:ListBox ID="LBox_HTeam" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" Width="100%" DataSourceID="SDS_HTeam" DataTextField="UserName" DataValueField="HID" AppendDataBoundItems="true"></asp:ListBox>
                                                    </div>
                                                </div>
                                                <!--暫註解-->
                                                <div class="form-group col-md-12 d-none">
                                                    <label class="font-weight-bold">
                                                        督導
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:SqlDataSource ID="SDS_HSupervise" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:ListBox ID="LBox_HSupervise" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" Width="100%" DataSourceID="SDS_HSupervise" DataTextField="UserName" DataValueField="HID" AppendDataBoundItems="true"></asp:ListBox>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        上課地點<span class="text-danger">*</span>
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:SqlDataSource ID="SDS_HOCPlace" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:ListBox ID="LBox_HOCPlace" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" Width="100%" DataSourceID="SDS_HOCPlace" DataTextField="HPlaceName" DataValueField="HID" AppendDataBoundItems="true"></asp:ListBox>

                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        課程日期連續與否
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:RadioButtonList ID="RBL_Continuous" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="RBL_Continuous_SelectedIndexChanged">
                                                            <asp:ListItem Value="0" Style="margin-right: 10px;">否</asp:ListItem>
                                                            <asp:ListItem Value="1" Selected="True">是</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-6">
                                                    <label class="font-weight-bold">
                                                        課程日期<span class="text-danger">*</span>
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HDateRange" runat="server" class="form-control daterange disabledate" AutoComplete="off" placeholder="選擇課程日期" ClientIDMode="Static" oninput="SortDate()"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        上課時間<span class="text-danger">*</span>
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HSTime" runat="server" class="form-control pl-1 timepicker text-center" AutoComplete="off" placeholder="" Style="width: 40%"></asp:TextBox>
                                                        到
												<asp:TextBox ID="TB_HETime" runat="server" class="form-control pl-1 timepicker text-center" AutoComplete="off" placeholder="" Style="width: 40%"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3 d-none">
                                                    <label class="font-weight-bold">
                                                        <span class="text-danger">*</span>課程類型
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:DropDownList ID="DDL_HCourseType" runat="server" class="form-control js-example-basic-single">
                                                            <asp:ListItem Value="0" Enabled="false">-請選擇-</asp:ListItem>
                                                            <asp:ListItem Value="1">必修</asp:ListItem>
                                                            <asp:ListItem Value="2">選修</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>




                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        捐款項目名稱(僅範本勾選需導向信用卡授權書用)
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:SqlDataSource ID="SDS_HDonationItem" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:DropDownList ID="DDL_HCCPeriodDItem" runat="server" class="form-control js-example-basic-single" AppendDataBoundItems="true" >
                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3  d-none">
                                                    <label class="font-weight-bold">
                                                        成績計算方式
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:DropDownList ID="DDL_HGradeCalculation" runat="server" class="form-control js-example-basic-single">
                                                            <asp:ListItem Value="0" Enabled="false">-請選擇-</asp:ListItem>
                                                            <asp:ListItem Value="1">加權平均</asp:ListItem>
                                                            <asp:ListItem Value="2">單一成績</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        考卷
                                                    </label>
                                                    <div class="form-group input-group">
                                                        <asp:SqlDataSource ID="SDS_HExamPaperID" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:ListBox ID="LBox_HExamPaperID" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" Style="width: 100%;" Enabled="true" DataSourceID="SDS_HExamPaperID" DataTextField="HTitle" DataValueField="HID"></asp:ListBox>
                                                    </div>
                                                </div>


                                                <div class="form-group col-md-3">
                                                    <label class="font-weight-bold">
                                                        出席率標準(僅輸入數字)
                                                    </label>
                                                    <div class="form-group input-group">
                                                        <asp:TextBox ID="TB_HAttRateStandard" runat="server" class="form-control text-center col-md-12" placeholder="" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="true" Width="35%"></asp:TextBox>%
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3 d-none">
                                                    <label class="font-weight-bold">
                                                        檢覈成績通過標準(僅輸入數字)
                                                    </label>
                                                    <div class="form-group input-group">
                                                        <asp:TextBox ID="TB_HExamPassStandard" runat="server" class="form-control text-center col-md-12" placeholder="" AutoComplete="off" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Enabled="true" Width="35%"></asp:TextBox>%
                                                    </div>
                                                </div>


                                                <div class="form-group col-md-3 d-none">
                                                    <label class="font-weight-bold" for="example-text">
                                                        通過後成為的講師類別
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:DropDownList ID="DDL_HTeacherClass" runat="server" class="form-control js-example-basic-single" name="state" Style="width: 100%" AppendDataBoundItems="true">
                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-3 d-none">
                                                    <label class="font-weight-bold" for="example-text">
                                                        通過後成為的講師層級分類
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:DropDownList ID="DDL_HTearcherLV" runat="server" class="form-control js-example-basic-single" name="state" Style="width: 100%" AppendDataBoundItems="true">
                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>


                                            </div>
                                            <div class="row">
                                                <div class="form-group col-md-5">
                                                    <label>圖片上傳<span class="text-primary">建議上傳尺寸: W=900px, H=600px</span></label>
                                                    <div class="form-group">
                                                        <asp:FileUpload ID="FU_HImg" runat="server" class="dropify" onchange="BrowsePic()" />
                                                        <asp:Label ID="LB_PicName" runat="server" Visible="false" />
                                                        <p id="NewUpload" class="text-danger"></p>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <!--課程開課資訊 END-->

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

                                                <div class="col-md-12 mb-4">
                                                    <div class="table-responsive">
                                                        <table class="table table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center" style="width: 5%">執行</th>
                                                                    <th class="text-center" style="width: 5%">No</th>
                                                                    <th style="width: 25%">教材名稱</th>
                                                                    <th style="width: 25%">上傳教材 <span class="text-danger">(限PDF、mp3檔；上限200Mb)</span></th>
                                                                    <th style="width: 30%">影片/其他教材連結</th>
                                                                    <th style="width: 10%">排序</th>
                                                                </tr>
                                                            </thead>


                                                            <tbody>
                                                                <tr>
                                                                    <td style="text-align: center;">
                                                                        <asp:LinkButton ID="LBtn_HCourseMaterial_add" runat="server" OnClick="LBtn_HCourseMaterial_add_Click" CausesValidation="false" Text="" class="ti-plus text-info"></asp:LinkButton>

                                                                    </td>
                                                                    <td class="text-center"></td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HCMName" runat="server" class="form-control" placeholder="請輸入教材名稱"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:FileUpload ID="FU_HCMaterial" runat="server" class="dropify" onchange="BrowseFile()" />
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HCMLink" runat="server" class="form-control" placeholder="請輸入影片/其他教材連結"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HSort" runat="server" Text="" class="form-control" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Style="text-align: center"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </tbody>


                                                            <asp:SqlDataSource ID="SDS_HCourseMaterial" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:Repeater ID="Rpt_HCourseMaterial" runat="server" DataSourceID="SDS_HCourseMaterial" OnItemDataBound="Rpt_HCourseMaterial_ItemDataBound">
                                                                <ItemTemplate>
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="text-align: center;"></td>
                                                                            <td class="text-center">
                                                                                <asp:Label ID="LB_HNo" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_HCMName" runat="server" class="form-control" placeholder="請輸入教材名稱" Text='<%# Eval("HCMName") %>'></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:FileUpload ID="FU_HCMaterial" runat="server" class="dropify" onchange="BrowseFile()" />
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

                                            </div>
                                        </asp:Panel>
                                        <!--學員課程教材 END-->

                                        <!--前導課程 START-->
                                        <asp:Panel ID="Panel_LeadingCourse" runat="server" class="tab-pane p-0 fade" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">
                                                <asp:Label ID="LB_Note" runat="server" CssClass="text-danger font-weight-bold d-block" Text="系統將幫您開啟另一個視窗設定前導課程頁面，或點以下連結查看前導課程管理："></asp:Label>
                                                <asp:HyperLink ID="HL_HLCourseEdit" runat="server" NavigateUrl="~/System/HLCourseRelated_Edit.aspx" Target="_blank">開啟前導課程管理</asp:HyperLink>
                                            </div>
                                        </asp:Panel>
                                        <!--前導課程 END-->

                                        <!--體系護持工作項目 START-->
                                        <asp:Panel ID="Panel_Jobs" runat="server" class="tab-pane p-0 fade" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">

                                                <div class="col-md-12 mb-4">
                                                    <div class="table-responsive">

                                                        <table class="table table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center" style="width: 10%">執行</th>
                                                                    <th class="text-center" style="width: 5%">No</th>
                                                                    <th style="width: 10%">所屬組別</th>
                                                                    <th style="width: 15%">任務職稱</th>
                                                                    <th style="width: 10%; display: none">人數</th>
                                                                    <th style="width: 30%">體系護持工作項目</th>
                                                                    <th style="width: 10%">負責組長</th>
                                                                    <%--<th style="width: 10%;display:none">試務人員名單</th>--%>
                                                                </tr>
                                                            </thead>



                                                            <tbody>
                                                                <tr>
                                                                    <td style="text-align: center;">
                                                                        <asp:LinkButton ID="LBtn_HTodoList_add" runat="server" OnClick="LBtn_HTodoList_add_Click" CausesValidation="false" Text="" class="ti-plus text-info"></asp:LinkButton>
                                                                    </td>
                                                                    <td class="text-center"></td>
                                                                    <td>
                                                                        <asp:SqlDataSource ID="SDS_HGroupName" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                                        <asp:DropDownList ID="DDL_HGroupName" runat="server" class="form-control js-example-basic-single" Style="width: 100%" AutoPostBack="true" OnSelectedIndexChanged="DDL_HGroupName_SelectedIndexChanged" DataSourceID="SDS_HGroupName" DataTextField="HGroupName" DataValueField="HID" AppendDataBoundItems="true">
                                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="DDL_HTask" runat="server" class="form-control js-example-basic-single" Style="width: 100%" AutoPostBack="true" OnSelectedIndexChanged="DDL_HTask_SelectedIndexChanged" Enabled="false">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td class="text-right d-none">
                                                                        <asp:TextBox ID="TB_HTaskNum" runat="server" class="form-control" placeholder="" Text="" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" MaxLength="3" Enabled="true"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HTaskContent" runat="server" class="form-control" placeholder=""></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:SqlDataSource ID="SDS_HGroupLeader" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand=""></asp:SqlDataSource>
                                                                        <asp:DropDownList ID="DDL_HGroupLeader" runat="server" class="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HGroupLeader" DataTextField="UserName" DataValueField="HID" AppendDataBoundItems="true">
                                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td class="d-none">
                                                                        <asp:SqlDataSource ID="SDS_HExamStaff" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                                        <asp:ListBox ID="LBox_HExamStaff" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" DataSourceID="SDS_HExamStaff" DataTextField="UserName" DataValueField="HID" Enabled="true" AppendDataBoundItems="true"></asp:ListBox>
                                                                    </td>
                                                                </tr>
                                                            </tbody>



                                                            <asp:SqlDataSource ID="SDS_HTodoList" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:Repeater ID="Rpt_HTodoList" runat="server" DataSourceID="SDS_HTodoList" OnItemDataBound="Rpt_HTodoList_ItemDataBound">
                                                                <ItemTemplate>
                                                                    <tbody>
                                                                        <asp:Label ID="LB_HSave" runat="server" class="form-control" Text='<%# Eval("HSave") %>' Visible="false"></asp:Label>
                                                                        <tr>
                                                                            <td style="text-align: center;"></td>
                                                                            <td class="text-center">
                                                                                <asp:Label ID="LB_HNo" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                                            </td>
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
                                                                                <asp:TextBox ID="TB_HTaskNum" runat="server" class="form-control" placeholder="" Text='<%# Eval("HTaskNum") %>' onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" MaxLength="3" Enabled="false"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_HTaskContent" runat="server" class="form-control" placeholder="" Text='<%# Eval("HTaskContent") %>'></asp:TextBox>
                                                                            </td>
                                                                            <td>

                                                                                <asp:DropDownList ID="DDL_HGroupLeader" runat="server" class="form-control js-example-basic-single" Text='<%# Eval("HGroupLeaderID") %>' Style="width: 100%" DataSourceID="SDS_HGroupLeader" DataTextField="UserName" DataValueField="HID" AppendDataBoundItems="true">
                                                                                    <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td class="d-none">
                                                                                <%--<asp:SqlDataSource ID="SDS_HExamStaff" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>--%>
                                                                                <asp:ListBox ID="LBox_HExamStaff" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" DataSourceID="SDS_HExamStaff" DataTextField="UserName" DataValueField="HID" Enabled="true" AppendDataBoundItems="true"></asp:ListBox>
                                                                                <asp:Label ID="LB_HExamStaff" runat="server" Text='<%# Eval("HExamStaff") %>' Visible="false"></asp:Label>
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
                                        <!--體系護持工作項目 END-->

                                        <!--報名須知 START-->
                                        <asp:Panel ID="Panel_Notice" runat="server" class="tab-pane p-0 fade" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">
                                                <div class="form-group col-md-12">
                                                    <label class="font-weight-bold">
                                                        報名須知/條款內容設定
                                                    </label>
                                                    <div class="form-group">
                                                        <CKEditor:CKEditorControl ID="CKE_HRNContent" runat="server" class="form-control" Text=""></CKEditor:CKEditorControl>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <!--報名須知 END-->

                                        <!--作業 START-->
                                        <asp:Panel ID="Panel_Homework" runat="server" class="tab-pane p-0 fade" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">
                                                <div class="col-md-12 mb-4">
                                                    <div class="table-responsive">
                                                        <table class="table table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center" style="width: 5%">執行</th>
                                                                    <th class="text-center" style="width: 5%">No</th>
                                                                    <th style="width: 20%">作業類型</th>
                                                                    <th style="width: 20%">問卷</th>
                                                                    <th style="width: 30%">作業內容描述</th>
                                                                    <th style="width: 10%">作業篇數</th>
                                                                    <th style="width: 10%">繳交期限</th>
                                                                </tr>
                                                            </thead>

                                                            <tbody>
                                                                <tr>
                                                                    <td style="text-align: center;">
                                                                        <asp:LinkButton ID="LBtn_HW_Add" runat="server" OnClick="LBtn_HW_Add_Click" CausesValidation="false" Text="" class="ti-plus text-info"></asp:LinkButton>
                                                                    </td>
                                                                    <td class="text-center"></td>
                                                                    <td>
                                                                        <asp:DropDownList ID="DDL_HHWType" runat="server" CssClass="form-control" Style="width: 100%" AutoPostBack="true" OnSelectedIndexChanged="DDL_HHWType_SelectedIndexChanged">
                                                                            <asp:ListItem Value="1">回應(可上傳圖片/PDF/Excel)</asp:ListItem>
                                                                            <asp:ListItem Value="2">問卷</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="DDL_HQuestion" runat="server" CssClass="form-control  js-example-basic-single" Style="width: 100%" Visible="false">
                                                                            <asp:ListItem Value="0">請選擇</asp:ListItem>

                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HDescription" runat="server" class="form-control" placeholder="請輸入作業內容描述"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HNumbers" runat="server" class="form-control" placeholder="請輸入作業篇數" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HDeadLine" runat="server" class="form-control datesinglepicker" placeholder="yyyy/MM/dd"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </tbody>


                                                            <asp:SqlDataSource ID="SDS_HCourseHWSetting" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:Repeater ID="RPT_HCourseHWSetting" runat="server" DataSourceID="SDS_HCourseHWSetting" OnItemDataBound="RPT_HCourseHWSetting_ItemDataBound">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LB_HCBatchNum" runat="server" Text='<%# Eval("HCBatchNum") %>' Visible="false"></asp:Label>
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="text-align: center;">
                                                                                <asp:LinkButton ID="LBtn_HW_Del" runat="server" OnClick="LBtn_HW_Del_Click" class="ti-close text-danger"></asp:LinkButton>
                                                                            </td>
                                                                            <td class="text-center">
                                                                                <asp:Label ID="LB_HNo" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="LB_HHWType" runat="server" Text='<%# Eval("HHWType") %>'></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="LB_HTitle" runat="server" Text='<%# Eval("HTitle") %>'></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="LB_HDescription" runat="server" Text='<%# Eval("HDescription") %>'></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="LB_HNumbers" runat="server" Text='<%# Eval("HNumbers") %>'></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="LB_HDeadLine" runat="server" Text='<%# Eval("HDeadLine") %>'></asp:Label>
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
                                        <!--作業 END-->

                                        <!--相關文件 START-->
                                        <asp:Panel ID="Panel_Related" runat="server" class="tab-pane p-0 fade" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">
                                                <div class="form-group col-md-3" runat="server" id="HBudgetTable" visible="false">
                                                    <label class="font-weight-bold">
                                                        經費預算表
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:FileUpload ID="FU_HBudgetTable" runat="server" class="dropify" onchange="BrowseFile()" />
                                                        <asp:Label ID="LB_HBudgetTable" runat="server" Visible="false" />
                                                    </div>
                                                </div>

                                                <div class="col-md-6">
                                                    <label class="font-weight-bold">
                                                        課前課程表
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:FileUpload ID="FU_BCSchedule" runat="server" class="dropify" onchange="BrowseFile()" />
                                                        <asp:Label ID="LB_BCSchedule" runat="server" Visible="false" />
                                                    </div>
                                                </div>

                                                <div class="form-group col-md-6">
                                                    <label class="font-weight-bold">
                                                        課前簡易課程表(會顯示於前台)
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:FileUpload ID="FU_BECSchedule" runat="server" class="dropify" onchange="BrowseFile()" />
                                                        <asp:Label ID="LB_BECSchedule" runat="server" Visible="false" />
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <!--相關文件 END-->

                                        <!--檢覈評比項目 START-->
                                        <asp:Panel ID="Panel_HCourseEvaluation" runat="server" class="tab-pane p-0 fade" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">
                                                <div class="col-md-12 mb-4">
                                                    <div class="table-responsive">
                                                        <table class="table table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center" style="width: 5%">執行</th>
                                                                    <th class="text-center" style="width: 5%">No</th>
                                                                    <th style="width: 20%">評比類型(影片/檔案)</th>
                                                                    <th style="width: 30%">評比內容描述</th>
                                                                    <th style="width: 10%">繳交數量</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr>
                                                                    <td style="text-align: center;">
                                                                        <asp:LinkButton ID="LBtn_HCE_Add" runat="server" OnClick="LBtn_HCE_Add_Click" CausesValidation="false" Text="" class="ti-plus text-info"></asp:LinkButton>
                                                                    </td>
                                                                    <td class="text-center"></td>
                                                                    <td>
                                                                        <asp:DropDownList ID="DDL_HCEType" runat="server" CssClass="form-control" Style="width: 100%" AutoPostBack="true">
                                                                            <asp:ListItem Value="1">影片</asp:ListItem>
                                                                            <asp:ListItem Value="2">檔案(圖片/PDF/Excel)</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HCEContent" runat="server" class="form-control" placeholder="請輸入評比內容描述"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HCENum" runat="server" class="form-control" placeholder="請輸入繳交數量" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </tbody>

                                                            <asp:SqlDataSource ID="SDS_HCourseEvaluation" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:Repeater ID="Rpt_HCourseEvaluation" runat="server" DataSourceID="SDS_HCourseEvaluation" OnItemDataBound="Rpt_HCourseEvaluation_ItemDataBound">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LB_HCBatchNum" runat="server" Text='<%# Eval("HCBatchNum") %>' Visible="false"></asp:Label>
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="text-align: center;">
                                                                                <asp:LinkButton ID="LBtn_HCE_Del" runat="server" OnClick="LBtn_HCE_Del_Click" class="ti-close text-danger"></asp:LinkButton>
                                                                            </td>
                                                                            <td class="text-center">
                                                                                <asp:Label ID="LB_HNo" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="LB_HCETypeName" runat="server" Text=""></asp:Label>
                                                                                <asp:Label ID="LB_HCEType" runat="server" Text='<%# Eval("HCEType") %>' Visible="false"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="LB_HCEContent" runat="server" Text='<%# Eval("HCEContent") %>'></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="LB_HCENum" runat="server" Text='<%# Eval("HCENum") %>'></asp:Label>
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
                                        <!--檢覈評比項目 END-->

                                        <!--講師教材 START-->
                                        <asp:Panel ID="Panel_HTMaterial" runat="server" class="tab-pane p-0 fade" role="tabpanel">
                                            <div class="form-horizontal m-t-10 row">
                                                <div class="col-md-12 mb-4">
                                                    <div class="table-responsive">
                                                        <table class="table table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th style="width: 30%">講師教材名稱</th>
                                                                    <th class="text-center" style="width: 10%">排序</th>
                                                                </tr>
                                                            </thead>
                                                            <asp:SqlDataSource ID="SDS_HTMaterial" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:SqlDataSource ID="SDS_HTMaterialDetail" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                            <asp:Repeater ID="Rpt_HTMaterialDetail" runat="server" DataSourceID="SDS_HTMaterialDetail" OnItemDataBound="Rpt_HTMaterialDetail_ItemDataBound">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                                    <asp:Label ID="LB_HSave" runat="server" class="form-control" Text='<%# Eval("HSave") %>' Visible="false"></asp:Label>
                                                                    <tbody>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="LB_HTMaterialID" runat="server" CssClass="p-l-10" Text='<%# Eval("HTMaterialID") %>' Visible="false"></asp:Label>
                                                                                <asp:DropDownList ID="DDL_HTMaterial" runat="server" class="form-control js-example-basic-single" Style="width: 100%" Enabled="false" DataSourceID="SDS_HTMaterial" DataTextField="HTMName" DataValueField="HID" AppendDataBoundItems="true">
                                                                                    <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td class="text-center">
                                                                                <asp:TextBox ID="TB_HTMaterialSort" runat="server" class="form-control" placeholder="" Text='<%# Eval("HSort") %>' onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Style="text-align: center" Enabled="false"></asp:TextBox>
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
                                    <asp:PostBackTrigger ControlID="LBtn_HCourseMaterial_add" />
                                    <asp:PostBackTrigger ControlID="LBtn_HTodoList_add" />
                                    <asp:PostBackTrigger ControlID="LBtn_HW_Add" />
                                    <asp:PostBackTrigger ControlID="LBtn_HCE_Add" />
                                    <asp:AsyncPostBackTrigger ControlID="DDL_HCourseTemplate" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="RBL_Continuous" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="DDL_HGroupName" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="DDL_HTask" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="DDL_HHWType" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>

                    </div>
                </div>

            </div>
            <!-- ============================================================== -->
            <!-- End Container fluid  -->
            <!-- ============================================================== -->

        </div>
    </div>



    <!-- Modal 課程名稱命名規則 START-->
    <div class="modal fade" id="info" tabindex="-1" role="dialog" aria-labelledby="info" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title" id="exampleModalLongTitle">課程名稱命名規則</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body name_body">
                    <p>1.請勿加入上課地點資訊</p>
                    <p>2.月份請以<span class="text-danger">底線</span>做為區隔，勿使用其他符號</p>
                    <p>3.月份請以<span class="text-danger">數字</span>呈現</p>
                    <hr />

                    <p class="font-weight-bold">【常用課程名稱】<span class="text-info">*可直接複製課程名稱，修改月份即可</span></p>
                    <table class="table table-bordered table-striped table-hover">
                        <tr>
                            <td>晨光上_七體玉成（台灣）_7月</td>
                            <td>晨光上_七體玉成（美西）_7月</td>
                        </tr>
                        <tr>
                            <td>晨光上_大愛光時代青年_7月</td>
                            <td>晨光上_幸福印記_7月</td>
                        </tr>
                        <tr>
                            <td>晨光上_大愛光入門_7月</td>
                            <td>晨光上_身強體壯功_7月</td>
                        </tr>
                        <tr>
                            <td>晨光上_春風化雨_7月</td>
                            <td>晨光下_（台灣）_7月</td>
                        </tr>
                        <tr>
                            <td>長青圓滿班_7月</td>
                            <td>和氣幸福班_7月</td>
                        </tr>
                        <tr>
                            <td>和氣幸福班_7月（日間）</td>
                            <td>和氣幸福班_7月（全球）</td>
                        </tr>
                    </table>

                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">關閉</button>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal 客戶等級說明 END-->



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
    <!--Custom JavaScript -->
    <script src="dist/js/custom.js"></script>
    <!--Select2-->
    <script src="js/select2.min.js"></script>
    <!--sumoselect-->
    <script src="js/jquery.sumoselect.min.js"></script>

    <!-- icheck -->
    <script src="assets/node_modules/icheck/icheck.min.js"></script>
    <script src="assets/node_modules/icheck/icheck.init.js"></script>

    <!--datepicker-->
    <script src="js/moment.min.js"></script>
    <link href="css/datepicker.min.css" rel="stylesheet" />
    <script src="js/datepicker.min.js"></script>
    <script src="dist/js/i18n/datepicker.en.js"></script>
    <!--daterangepicker-->
    <script src="js/daterangepicker.js"></script>
    <!--timepicker-->
    <script src="js/jquery.timepicker.min.js"></script>


    <!--dropify js-->
    <script src="js/dropify.min.js"></script>



    <script>
        $(window).on('load', function () {
            $('#loadingOverlay').fadeOut(300); // 淡出更流暢
        });

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_beginRequest(function () {
            document.getElementById("loadingOverlay").style.display = "flex";
        });
        prm.add_endRequest(function () {
            document.getElementById("loadingOverlay").style.display = "none";
        });


        $(function () {
            //單選
            $('.js-example-basic-single').select2();

            //多選--sumoselect js
            $('.ListB_Multi').SumoSelect({
                search: true,
                placeholder: '-請選擇-',
                csvDispCount: 5,
            });

            $('.datesinglepicker').datepicker({
                language: 'en',
                dateFormat: 'yyyy/mm/dd',
                multipleDates: false,
                minDate: new Date(),
                todayHighlight: true,
                orientation: 'bottom auto',
                //startDate: new Date(),
            });

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

            $('.timepicker').timepicker({
                timeFormat: 'HH:mm',
                interval: 30,
                minTime: '4',
                maxTime: '23:00',
                //defaultTime: '4',
                //startTime: '04:00am',
                dynamic: false,
                dropdown: true,
                scrollbar: true
            });
        });

        function BrowseFile() {
            document.getElementById("NewUpload").innerText = "選擇了新檔案，如要取消請按[REMOVE]";
        }
    </script>

</asp:Content>

