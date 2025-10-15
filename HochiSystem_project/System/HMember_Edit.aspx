<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HMember_Edit.aspx.cs" Inherits="HMember_Edit" MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="Page" TagName="Paging" Src="~/System/Paging_backend.ascx" %>
<%--分頁--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="d-none">
        <asp:Label ID="LB_HArea" runat="server" Text=""></asp:Label><!--登入者所屬區屬-->
        <asp:Label ID="LB_HOPosition" runat="server" Text=""></asp:Label>
        <asp:Label ID="LB_HOPositionID" runat="server" Text="AAA"></asp:Label><!--天命法位原HID-->
        <!--天命法位-->
        <asp:Label ID="LB_HOArea" runat="server" Text=""></asp:Label>
        <!--區屬-->
        <asp:Label ID="LB_HOTeam" runat="server" Text=""></asp:Label>
        <!--光團-->
    </div>
    <!-- ============================================================== -->
    <!-- Page wrapper  -->
    <!-- ============================================================== -->
    <div class="page-wrapper">
        <!-- ============================================================== -->
        <!-- Container fluid  -->
        <!-- ============================================================== -->
        <div class="container">

            <asp:Panel ID="Panel_List" runat="server">
                <!-- ============================================================== -->
                <div class="block-header">
                    <div class="row">
                        <div class="col-lg-5 col-md-12 col-sm-4">
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>學員管理</h2>
                            <!--(Member Management)-->
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item active">學員管理</li>
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
                                    <!-- Column -->
                                    <div class="col-md-12 col-lg-2 col-xlg-2">
                                        <div class="box text-left">
                                            <button id="Btn_Add" runat="server" type="button" class="btn btn-outline-info" onclick="window.location.href='HMember_add.aspx';"><i class="fa fa-plus"></i>新增學員</button>
                                            <!--(Add Member)-->
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12 col-lg-12 col-xlg-12">
                                        <div class="box form-group row m-b-0">
                                            <div class="col-md-4">
                                                <asp:TextBox ID="TB_Search" runat="server" class="form-control" AutoComplete="off" placeholder="請輸入期別、姓名、電話、帳號的關鍵字"></asp:TextBox>
                                                <!--<input type="text" class="form-control" placeholder="請輸入期別、姓名、電話、Email的關鍵字">-->
                                                <!--(Keywords include period, name, mobile, email)-->
                                            </div>
                                            <div class="col-md-2">
                                                <asp:SqlDataSource ID="SDS_SHArea" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HArea FROM HArea"></asp:SqlDataSource>
                                                <asp:DropDownList ID="DDL_SHArea" CssClass="form-control  js-example-basic-single" runat="server" Style="width: 100%" DataValueField="HID" DataTextField="HArea" DataSourceID="SDS_SHArea" AppendDataBoundItems="true">
                                                    <asp:ListItem Value="0">請選擇區屬</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-md-2">
                                                <asp:DropDownList ID="DDL_Status" CssClass="form-control" runat="server" Style="width: 100%">
                                                    <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                    <asp:ListItem Value="1">啟用</asp:ListItem>
                                                    <asp:ListItem Value="2">停用</asp:ListItem>
                                                </asp:DropDownList>
                                                <!--(Valid)-->
                                                <!--(Invalid)-->
                                            </div>

                                            <div class="col-md-4">
                                                <asp:LinkButton ID="LBtn_Search" runat="server" OnClick="LBtn_Search_Click" class="btn btn-outline-secondary"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                                <!-- (Search)-->
                                                <asp:LinkButton ID="LBtn_SearchCancel" runat="server" OnClick="LBtn_SearchCancel_Click" class="btn btn-outline-secondary m-l-10"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="table-responsive">
                                    <table class="table table-hover m-t-20">
                                        <thead>
                                            <tr>
                                                <th class="text-center" style="width: 8%">執行</th>
                                                <th class="text-center" style="width: 5%">No</th>
                                                <th style="width: 5%">體系</th>
                                                <!--(System)-->
                                                <th style="width: 8%">區屬</th>
                                                <!--(Area)-->
                                                <th style="width: 13%">光團</th>
                                                <th style="width: 10%">期別</th>
                                                <!--(Period)-->
                                                <th style="width: 12%">姓名</th>
                                                <th style="width: 10%">手機號碼</th>
                                                <!--(Mobile)-->
                                                <th style="width: 18%">帳號</th>
                                                <!--(Email)-->
                                                <th class="text-center" style="width: 8%">學員類別</th>
                                                <!--(參班與否)-->
                                                <th style="width: 10%">參班</th>
                                                <!--(Member Type)-->
                                                <th class="text-center" style="width: 5%">狀態</th>
                                                <!--(Status)-->
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:SqlDataSource ID="SDS_Member" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="" OnSelecting="SDS_Member_Selecting"></asp:SqlDataSource>
                                            <asp:Repeater ID="Rpt_Member" runat="server" OnItemDataBound="Rpt_Member_ItemDataBound">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="text-center">
                                                            <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success mr-2" ToolTip="編輯" OnClick="LBtn_Edit_Click" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "HID") %>'><i class="icon-pencil"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="LBtn_Off" runat="server" class="btn btn-sm btn-outline-danger mr-2" ToolTip="停用" OnClick="LBtn_Off_Click" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "HID") %>' Btmessage="確定要停用此帳號嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))'><i class="icon-ban"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="LBtn_On" runat="server" CssClass="btn btn-sm btn-outline-primary mr-2" ToolTip="啟用" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "HID") %>' Btmessage="確定要啟用此帳號嗎？" CommandName="upload" OnClientClick='return confirm(this.getAttribute("btmessage"))' OnClick="LBtn_On_Click"><i class="ti-check"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="LBtn_Del" runat="server" Visible="false" Style="display: none"></asp:LinkButton>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HNo" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HSystemID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HSystemName") %>'></asp:Label>
                                                        </td>

                                                        <td>
                                                            <asp:Label ID="LB_HAreaID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HArea") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HTeamID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HTeamID") %>' Visible="false"></asp:Label>
                                                            <asp:Label ID="LB_HTeamName" runat="server" Text=""></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HPeriod" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HPeriod") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HUserName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HUserName") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HPhone" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HPhone") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HEmail" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HAccount") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HType" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HMType") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_IsCourseBooking" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "參班") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div class="badge badge-default" id="Activation" runat="server">
                                                                <asp:Label ID="LB_HStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HStatus") %>'></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                    <!--分頁-->
                                    <!------------------分頁功能開始------------------>
                                    <div class="box text-right">
                                        <Page:Paging runat="server" ID="Pg_Paging" />
                                    </div>
                                    <!------------------分頁功能結束------------------>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </asp:Panel>

            <asp:Panel ID="Panel_Edit" runat="server" Visible="false">
                <div class="block-header">
                    <div class="row">
                        <div class="col-lg-3 col-md-12 col-sm-4">
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>學員管理</h2>
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item"><a href="HMember_Edit.aspx">學員管理</a></li>
                                <li class="breadcrumb-item active">編輯學員</li>
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
                                <p class="text-danger">*表示必填</p>
                                <asp:Label ID="LB_HID" runat="server" Text="" Visible="false"></asp:Label>

                                <div class="form-group">
                                    <asp:LinkButton ID="LBtn_Info" runat="server" class="col-12 btn btn-secondary text-left moreinfo">
										<i class="ti-angle-down"></i>
										<label class="m-b-0 font-weight-bold">基本資料主檔</label>
                                    </asp:LinkButton>

                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <div class="hideinfo col-md-12 mb-4">
                                                <div class="row clearfix">
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>法脈序號(由系統產生)</label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HSeries" runat="server" class="form-control" Enabled="false"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label><span class="text-danger">*</span>帳號</label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HEmail" runat="server" class="form-control" placeholder="" OnTextChanged="FormCheck" AutoPostBack="true" AutoComplete="off" Enabled="true" Style="width: 100%"></asp:TextBox>
                                                            <asp:Label ID="LB_NoticeEmail" runat="server" Text="請輸入正確格式的信箱" ForeColor="Red" Visible="false"></asp:Label>
                                                            <asp:Label ID="LB_NoticeEmail2" runat="server" Text="帳號(信箱)已經存在" ForeColor="Red" Visible="false"></asp:Label>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label><span class="text-danger">*</span>密碼<span class="text-danger">(不顯示密碼，如需變更請輸入新密碼)</span></label>
                                                        <div class="form-group">
                                                            <asp:Label ID="LB_OriPassword" runat="server" Text="" Visible="false"></asp:Label>
                                                            <asp:TextBox ID="TB_HPassword" runat="server" class="form-control" placeholder="至少8位數(包含有英文大小寫及數字)" AutoComplete="off" AutoPostBack="true" type="password" MaxLength="20" OnTextChanged="FormCheck"></asp:TextBox><%----%>
                                                            <asp:Label ID="LB_NoticePassword" runat="server" Text="請輸入至少8位數(包含有英文大小寫及數字)" ForeColor="Red" Visible="false"></asp:Label><%--（The password must be five-twenty characters and include numbers, letters or characters.）--%>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>帳號啟用</label>
                                                        <!-- (Account Activation)-->
                                                        <div class="form-group">
                                                            <asp:RadioButtonList ID="RBL_HStatus" runat="server" CssClass="d-block" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                                <asp:ListItem Value="0" Style="margin-right: 10px">停用</asp:ListItem>
                                                                <asp:ListItem Value="1">啟用</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row clearfix">
                                                    <div class="col-md-3 col-sm-12">
                                                        <label><span class="text-danger">*</span>期別</label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HPeriod" runat="server" class="form-control" placeholder="2000" MaxLength="4"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label><span class="text-danger">*</span>區屬</label>
                                                        <div class="form-group">
                                                            <div class="row">
                                                                <div class="col-md-5 pr-2">
                                                                    <asp:DropDownList ID="DDL_AreaType" runat="server" class="form-control" required="required" Style="width: 100%">
                                                                        <asp:ListItem Value="1">本人</asp:ListItem>
                                                                        <asp:ListItem Value="2">介紹人</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <!--(Self)-->
                                                                    <!--(Introducer)-->
                                                                </div>
                                                                <div class="col-md-7 pl-2">
                                                                    <asp:SqlDataSource ID="SDS_Area" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HLAreaID, HArea, HARange, HStatus FROM HArea"></asp:SqlDataSource>
                                                                    <asp:DropDownList ID="DDL_HAreaID" runat="server" DataSourceID="SDS_Area" class="form-control js-example-basic-single" DataTextField="HArea" DataValueField="HID" AppendDataBoundItems="true" Style="width: 100%">
                                                                        <%-- DataSourceID="SDS_Area" DataTextField="HSystemName" DataValueField="HID" AppendDataBoundItems="true"--%>
                                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>光團</label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_Team" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HMTeam,(convert(nvarchar(50),HMTeam.HID)+','+ HType)as ID FROM HMTeam UNION SELECT HCTeam,(convert(nvarchar(50),HCTeam.HID)+','+ HType)as ID FROM HCTeam"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HTeamID" runat="server" class="form-control js-example-basic-single" DataSourceID="SDS_Team" DataTextField="HMTeam" DataValueField="ID" AppendDataBoundItems="true" Style="width: 100%">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>體系</label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_System" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID,HSystemName,HStatus FROM HSystem WHERE HStatus = 1"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HSystemID" runat="server" class="form-control js-example-basic-single" DataSourceID="SDS_System" DataTextField="HSystemName" DataValueField="HID" AppendDataBoundItems="true" Style="width: 100%">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>
                                                            天命法位
                                                        </label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HRole" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HRNum, HUTaskClass, HUTSystem, HRName, HMemberID, HRAccess, HRType, HStatus FROM HRole WHERE HStatus = 1 AND HRType=2"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HRole" runat="server" class="form-control js-example-basic-single" DataSourceID="SDS_HRole" DataTextField="HRName" DataValueField="HID" AppendDataBoundItems="true" Style="width: 100%">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-3 col-sm-12">
                                                        <label><span class="text-danger">*</span>姓名</label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HUserName" runat="server" class="form-control" placeholder="王大明"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>性別(Gender)</label>
                                                        <div class="form-group">
                                                            <asp:DropDownList ID="DDL_HSex" runat="server" class="form-control" Style="width: 100%">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                <asp:ListItem Value="1">男</asp:ListItem>
                                                                <asp:ListItem Value="2">女</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>血型</label>
                                                        <!-- (Blood Type)-->
                                                        <div class="form-group">
                                                            <asp:DropDownList ID="DDL_HBlood" runat="server" class="form-control" Style="width: 100%">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                <asp:ListItem Value="1">A</asp:ListItem>
                                                                <asp:ListItem Value="2">B</asp:ListItem>
                                                                <asp:ListItem Value="3">O</asp:ListItem>
                                                                <asp:ListItem Value="4">AB</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>三載體光</label>
                                                        <div class="form-group">
                                                            <asp:DropDownList ID="DDL_HCarrier" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="DDL_HCarrier_SelectedIndexChanged">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                <asp:ListItem Value="1">金光</asp:ListItem>
                                                                <asp:ListItem Value="2">銀光</asp:ListItem>
                                                                <asp:ListItem Value="3">純光</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>七彩光</label>
                                                        <div class="form-group">
                                                            <asp:DropDownList ID="DDL_HRainbow" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="DDL_HRainbow_SelectedIndexChanged">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                <asp:ListItem Value="1">紅光</asp:ListItem>
                                                                <asp:ListItem Value="2">橙光</asp:ListItem>
                                                                <asp:ListItem Value="3">黃光</asp:ListItem>
                                                                <asp:ListItem Value="4">綠光</asp:ListItem>
                                                                <asp:ListItem Value="5">藍光</asp:ListItem>
                                                                <asp:ListItem Value="6">靛光</asp:ListItem>
                                                                <asp:ListItem Value="7">紫光</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>二十一道光</label>
                                                        <div class="form-group">
                                                            <asp:Label ID="LB_HLightName" runat="server" Text="未定光系"></asp:Label>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>是否為光使</label>
                                                        <div class="form-group">
                                                            <asp:RadioButtonList ID="RBL_HLightEnvoy" runat="server" CssClass="d-block" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                                <asp:ListItem Value="0" Style="margin-right: 10px">否</asp:ListItem>
                                                                <asp:ListItem Value="1">是</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>護照英文姓名</label>
                                                        <!--(Passport Name)-->
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HPPName" runat="server" class="form-control" placeholder="護照英文姓名"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label><span class="text-danger">*</span>國家</label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_Country" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, CountryEN, CountryTW, (CountryEN+'-'+CountryTW) as CountryName FROM HCountry"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HCountryID" runat="server" class="form-control js-example-basic-single" DataSourceID="SDS_Country" DataTextField="CountryName" DataValueField="HID" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="DDL_HCountryID_SelectedIndexChanged" Style="width: 100%">
                                                                <asp:ListItem Value="0">請選擇國家</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label><span class="text-danger" id="personIDmust" runat="server" visible="false">*</span>身分證字號(台灣)</label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HPersonID" runat="server" class="form-control" placeholder="" AutoComplete="off" MaxLength="10" AutoPostBack="true" OnTextChanged="TB_HPersonID_TextChanged" Style="width: 100%; padding: 4px;"></asp:TextBox>
                                                            <asp:Label ID="LB_NoticePersonID" runat="server" Text="請輸入正確格式的身分證字號" ForeColor="Red" Visible="false"></asp:Label>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label><span class="text-danger" id="Span1" runat="server" visible="false">*</span>是否上傳國稅局</label>
                                                        <div class="form-group">
                                                            <asp:DropDownList ID="DDL_HUploadIRS" runat="server" class="form-control js-example-basic-single" Style="width: 100%" Enabled="false">
                                                                <asp:ListItem Value="99">請選擇</asp:ListItem>
                                                                <asp:ListItem Value="0">否</asp:ListItem>
                                                                <asp:ListItem Value="1">是</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12 d-none">
                                                        <label>海外編號</label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HForeignID" runat="server" class="form-control" placeholder="" AutoComplete="off" Style="width: 100%; padding: 4px;"></asp:TextBox>

                                                        </div>
                                                    </div>

                                                    <div class="col-md-3 col-sm-12">
                                                        <label>生日</label>
                                                        <asp:Label ID="LB_BirthNotice" runat="server" Visible="false" CssClass="text-danger font-weight-bold" Text="*生日格式有誤! (請輸入yyyy/MM/dd)"></asp:Label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HBirth" runat="server" class="form-control" placeholder="yyyy/MM/dd" AutoComplete="off"></asp:TextBox>
                                                            <asp:RegularExpressionValidator
                                                                ID="REV_HBirth"
                                                                runat="server"
                                                                ControlToValidate="TB_HBirth"
                                                                ValidationExpression="^(19|20)\d{2}/(0[1-9]|1[0-2])/(0[1-9]|[12]\d|3[01])$"
                                                                ErrorMessage="請輸入正確格式：yyyy/MM/dd"
                                                                Display="Dynamic"
                                                                CssClass="validator-error" />
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>年齡</label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HAge" runat="server" class="form-control" placeholder="由生日自動計算年齡"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>學員類別</label>
                                                        <!--(Member Type)-->
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HMType" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HMType FROM HMType  WHERE HStatus != '2' ORDER BY HID ASC"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HType" runat="server" class="form-control js-example-basic-single" Enabled="true" DataSourceID="SDS_HMType" DataTextField="HMType" DataValueField="HID" AppendDataBoundItems="true" Style="width: 100%">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-sm-12">
                                                        <label>學員軸線類別</label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HAxisType" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HAxisType FROM HAxisType  WHERE HStatus ='1' ORDER BY HID ASC"></asp:SqlDataSource>
                                                            <asp:Label ID="LB_HAxisType" runat="server" Visible="false"></asp:Label>
                                                            <asp:ListBox ID="LBox_HAxisType" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" DataSourceID="SDS_HAxisType" DataTextField="HAxisType" DataValueField="HID" Enabled="true"></asp:ListBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-3 col-sm-12">
                                                        <label>生命導師</label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HLifeLeader" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT a.HID, IIF(a.HStatus='0',(b.HArea+'/'+ a.HPeriod+' '+ a.HUserName+'(停用)'),(b.HArea+'/'+ a.HPeriod+' '+ a.HUserName)) as UserName from HMember AS a Left Join HArea  AS b On a.HAreaID =b.HID WHERE  a.HType in(7,8,9,10,11,12) AND a.HID <>'9390'  order by a.HType desc, a.HStatus desc"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HLifeLeaderID" runat="server" class="form-control js-example-basic-single" Enabled="true" DataSourceID="SDS_HLifeLeader" DataTextField="UserName" DataValueField="HID" AppendDataBoundItems="true" Style="width: 100%">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>

                                                    <div class="col-md-6 col-sm-12">
                                                        <label>常態任務</label>
                                                        <div class="form-group">
                                                            <asp:Label ID="LB_HUsualTask" runat="server" Text=""></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row clearfix">
                                                    <div class="col-md-3 ">
                                                        <label>認識和氣大愛的管道</label>
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HWay" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HWayName, HStatus FROM HWay WHERE HStatus=1"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HKWay" runat="server" class="form-control js-example-basic-single" DataSourceID="SDS_HWay" DataTextField="HWayName" DataValueField="HID" AppendDataBoundItems="true" Style="width: 100%">
                                                                <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                <%--<asp:ListItem Value="1">網站</asp:ListItem>
														<asp:ListItem Value="2">親友介紹</asp:ListItem>--%>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <!--/span-->
                                                    <div class="col-md-3 ">
                                                        <label>吸引確定加入和氣大愛的管道</label>
                                                        <div class="form-group">

                                                            <asp:DropDownList ID="DDL_HSWay" runat="server" class="form-control js-example-basic-single" DataSourceID="SDS_HWay" DataTextField="HWayName" DataValueField="HID" AppendDataBoundItems="true" Style="width: 100%">
                                                                <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 ">
                                                        <label>從得知和氣大愛到實際加入間隔時間</label>
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HJDuration" runat="server" class="form-control" placeholder=""></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row clearfix">
                                                    <div class="col-md-6 ">
                                                        <label>圖片上傳<span class="text-primary">建議上傳尺寸: W=700px, H=560px</span></label>
                                                        <div class="col-md-3">
                                                            <asp:FileUpload ID="FU_HImg" runat="server" CssClass="dropify" onchange="BrowsePic()" Width="100%" />
                                                            <asp:Label ID="LB_Pic" runat="server" Visible="false" />
                                                            <asp:Label ID="LB_OldPic" runat="server" Visible="false" />
                                                            <p id="NewUpload" class="text-danger"></p>
                                                        </div>
                                                        <div class="col-md-2">
                                                            <asp:Image ID="IMG_Pic" runat="server" Height="100px"></asp:Image>
                                                        </div>
                                                        <div class="col-md-2">
                                                            <asp:Button ID="Btn_Del" runat="server" Text="移除圖片" CssClass="btn btn-secondary" OnClick="Btn_Del_Click" Btmessage="確定要移除已經上傳的圖片嗎?" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="TB_HEmail" EventName="TextChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="TB_HPassword" EventName="TextChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="DDL_HCarrier" EventName="SelectedIndexChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="DDL_HRainbow" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>

                                <div class="form-group">
                                    <asp:LinkButton ID="LBtn_Contact" runat="server" class="col-12 btn btn-secondary text-left moreinfo" OnClick="LBtn_Contact_Click">
												<i class="ti-angle-down"></i>
												<label class="m-b-0 font-weight-bold">聯絡資訊</label>
                                        <!--(Contact Info.)-->
                                    </asp:LinkButton>

                                    <asp:Panel ID="Panel_Contact" runat="server" Visible="false">
                                        <div class="hideinfo col-md-12 mb-4">
                                            <div class="row clearfix">
                                                <div class="col-md-3 col-sm-12">
                                                    <label>電話(住家)</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HOTel" runat="server" class="form-control" placeholder="+886-04-233XXXXX"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>電話(公司)</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HCTel" runat="server" class="form-control" placeholder="+886-04-233XXXXX"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>手機號碼</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HPhone" runat="server" class="form-control" placeholder="0955-225-XXX"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>和氣大愛公務E-mail</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HOEmail" runat="server" class="form-control" placeholder="" AutoComplete="off" OnTextChanged="FormCheck" AutoPostBack="true" MaxLength="200"></asp:TextBox>
                                                        <asp:Label ID="LB_NoticeOEmail" runat="server" Text="請輸入正確格式的信箱" ForeColor="Red" Visible="false"></asp:Label>
                                                        <!--(Please check mail format.)-->
                                                        <asp:Label ID="LB_NoticeOEmail2" runat="server" Text="信箱已經存在" ForeColor="Red" Visible="false"></asp:Label>
                                                        <!-- (The mail is existed.)-->
                                                    </div>
                                                </div>

                                                <div class="col-md-3 col-sm-12">
                                                    <label>緊急聯絡人姓名</label>
                                                    <!--  (Emergency Contact Name)-->
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HEmerName" runat="server" class="form-control" placeholder=""></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>緊急聯絡人關係</label>
                                                    <!-- (Relationship with Emergency Contact)-->
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HEmerRelated" runat="server" class="form-control" placeholder=""></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>緊急聯絡人電話</label>
                                                    <!--(Emergency Contact Phone) -->
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HEmerPhone" runat="server" class="form-control" placeholder=""></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-12 col-sm-12">
                                                    <label>戶籍地址</label>
                                                    <!--(Permanent Address)-->
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HRPostal" runat="server" class="form-control" Style="width: 15%" placeholder="郵遞區號"></asp:TextBox>
                                                        <asp:TextBox ID="TB_HRAddress" runat="server" class="form-control" Style="width: 84%" placeholder="請輸入地址"></asp:TextBox>

                                                    </div>
                                                </div>
                                                <div class="col-md-12 col-sm-12">
                                                    <label>通訊地址</label>
                                                    <!--(Current Address)-->
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HCPostal" runat="server" class="form-control" Style="width: 15%" placeholder="郵遞區號"></asp:TextBox>
                                                        <asp:TextBox ID="TB_HCAddress" runat="server" class="form-control" Style="width: 84%" placeholder="請輸入地址"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>line</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HLineID" runat="server" class="form-control"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>Facebook</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HFB" runat="server" class="form-control"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>Wechat</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HWechat" runat="server" class="form-control"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>Whatsapp</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HWhatsapp" runat="server" class="form-control"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>接引人</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HIntroName" runat="server" class="form-control"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>接引人關係</label>
                                                    <!--(Introducer Relationship)-->
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HIntroRelated" runat="server" class="form-control"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>

                                <div class="form-group" style="display: none;">

                                    <asp:LinkButton ID="LBtn_Family" runat="server" class="col-12 btn btn-secondary text-left moreinfo" OnClick="LBtn_Family_Click">
												<i class="ti-angle-down"></i>
												<label class="m-b-0 font-weight-bold">血脈資訊</label>
                                        <!--(Blood Lineage)-->
                                    </asp:LinkButton>
                                    <asp:Panel ID="Panel_Family" runat="server" Visible="false">
                                        <div class="hideinfo col-md-12 mb-4">
                                            <div class="row clearfix">
                                                <div class="col-md-9 col-sm-12">
                                                    <label>本人重大疾病</label>
                                                    <!--(Self Major Illness)-->
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HDisease" runat="server" class="form-control" placeholder="EX:癌症、心臟病(ex:cancer, heart disease)"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>家族病史</label>
                                                    <!--(Family History)-->
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HFDisease" runat="server" class="form-control" placeholder="輸入家族病史"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-md-12 col-sm-12">
                                                    <label>父親姓名</label>
                                                    <!--(Father's Name)-->
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HDad" runat="server" class="form-control" placeholder="" Style="width: 25%;"></asp:TextBox>
                                                        同修：
												<asp:RadioButtonList ID="RBL_HDFellow" runat="server" class="check mr-2" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                    <asp:ListItem Value="0" Selected="True">否</asp:ListItem>
                                                    <asp:ListItem Value="1">是</asp:ListItem>
                                                </asp:RadioButtonList>

                                                        存歿：
												<asp:RadioButtonList ID="RBL_HDLife" runat="server" class="check mr-2" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                    <asp:ListItem Value="0">歿</asp:ListItem>
                                                    <asp:ListItem Value="1" Selected="True">存</asp:ListItem>
                                                </asp:RadioButtonList>

                                                        同住：
													<asp:RadioButtonList ID="RBL_HDLTogether" runat="server" class="check" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                        <asp:ListItem Value="0" Selected="True">否</asp:ListItem>
                                                        <asp:ListItem Value="1">是</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                                <div class="col-md-12 col-sm-12">
                                                    <label>母親姓名</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HMom" runat="server" class="form-control" placeholder="" Style="width: 25%;"></asp:TextBox>

                                                        同修：
													<asp:RadioButtonList ID="RBL_HMFellow" runat="server" class="check mr-2" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                        <asp:ListItem Value="0" Selected="True">否</asp:ListItem>
                                                        <asp:ListItem Value="1">是</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                        存歿：
												<asp:RadioButtonList ID="RBL_HMLife" runat="server" class="check mr-2" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                    <asp:ListItem Value="0">歿</asp:ListItem>
                                                    <asp:ListItem Value="1" Selected="True">存</asp:ListItem>
                                                </asp:RadioButtonList>
                                                        同住：
													<asp:RadioButtonList ID="RBL_HMLTogether" runat="server" class="check" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                        <asp:ListItem Value="0" Selected="True">否</asp:ListItem>
                                                        <asp:ListItem Value="1">是</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>婚姻狀況(Marriage)</label>
                                                    <div class="form-group">
                                                        <asp:DropDownList ID="DDL_HMarriage" runat="server" CssClass="form-control" Style="width: 100%">
                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                            <asp:ListItem Value="1">未婚(Single)</asp:ListItem>
                                                            <asp:ListItem Value="2">已婚(Marriaged)</asp:ListItem>
                                                            <asp:ListItem Value="3">離婚(Divorced)</asp:ListItem>
                                                            <asp:ListItem Value="4">喪偶(Widowed)</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-md-9 col-sm-12">
                                                    <label>配偶姓名(Spouse's Name)</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HSpouse" runat="server" class="form-control" placeholder="請輸入姓名" Style="width: 32%;"></asp:TextBox>
                                                        同修：
												<asp:RadioButtonList ID="RBL_HSFellow" runat="server" class="check mr-2" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                    <asp:ListItem Value="0" Selected="True">否</asp:ListItem>
                                                    <asp:ListItem Value="1">是</asp:ListItem>
                                                </asp:RadioButtonList>
                                                        同住：
													<asp:RadioButtonList ID="RBL_HSLTogether" runat="server" class="check" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                        <asp:ListItem Value="0" Selected="True">否</asp:ListItem>
                                                        <asp:ListItem Value="1">是</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>兄弟姊妹人數</label>
                                                    <!-- (Number of Siblings)-->
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HSiblingNum" runat="server" class="form-control" placeholder="請輸入人數"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>本人家中排行</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HRank" runat="server" class="form-control" placeholder="請輸入數字"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-12 col-sm-12">
                                                    <label>子女</label>
                                                    <div class="form-group">
                                                        <table class="table table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center" style="width: 10%">執行</th>
                                                                    <th class="text-center" style="width: 5%">No</th>
                                                                    <th style="width: 20%">姓名</th>
                                                                    <th style="width: 20%">性別</th>
                                                                    <th style="width: 15%">同修</th>
                                                                    <th style="width: 15%">存歿</th>
                                                                    <th style="width: 15%">同住</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr>
                                                                    <td style="text-align: center;">
                                                                        <asp:LinkButton ID="LBtn_HChildAdd" runat="server" title="新增" class="p-10" OnClick="LBtn_HChildAdd_Click"><i class="ti-plus text-info"></i></asp:LinkButton>
                                                                    </td>
                                                                    <td class="text-center"></td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HCName_Add" runat="server" class="form-control"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="DDL_HCSex_Add" runat="server" class="form-control" Style="width: 100%">
                                                                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                            <asp:ListItem Value="1">男</asp:ListItem>
                                                                            <asp:ListItem Value="2">女</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td>
                                                                        <asp:RadioButtonList ID="RBL_HCFellow_Add" runat="server" class="check" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                                            <asp:ListItem Value="0" Selected="True">否</asp:ListItem>
                                                                            <asp:ListItem Value="1">是</asp:ListItem>
                                                                        </asp:RadioButtonList>
                                                                    </td>
                                                                    <td>
                                                                        <asp:RadioButtonList ID="RBL_HCLife_Add" runat="server" class="check" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                                            <asp:ListItem Value="0">歿</asp:ListItem>
                                                                            <asp:ListItem Value="1" Selected="True">存</asp:ListItem>
                                                                        </asp:RadioButtonList>
                                                                    </td>
                                                                    <td>
                                                                        <asp:RadioButtonList ID="RBL_HCLTogether_Add" runat="server" class="check" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                                            <asp:ListItem Value="0" Selected="True">否</asp:ListItem>
                                                                            <asp:ListItem Value="1">是</asp:ListItem>
                                                                        </asp:RadioButtonList>
                                                                    </td>
                                                                </tr>

                                                                <asp:Repeater ID="Rpt_HChildren" runat="server">
                                                                    <ItemTemplate>
                                                                        <tr>
                                                                            <td style="text-align: center;">
                                                                                <asp:LinkButton ID="LBtn_HChildDelete" runat="server" title="刪除" class="p-10" OnClick="LBtn_HChildDelete_Click" CommandArgument='<%# Container.ItemIndex%>'><i class="ti-trash text-danger"></i></asp:LinkButton>
                                                                            </td>
                                                                            <td class="text-center">
                                                                                <asp:Label ID="LB_HNo" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_HCName_Edit" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CName") %>' class="form-control" AutoComplete="off"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:DropDownList ID="DDL_HCSex_Edit" runat="server" class="form-control" Style="width: 100%" Text='<%# DataBinder.Eval(Container.DataItem, "CSex") %>'>
                                                                                    <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                                    <asp:ListItem Value="1">男</asp:ListItem>
                                                                                    <asp:ListItem Value="2">女</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td>
                                                                                <asp:RadioButtonList ID="RBL_HCFellow_Edit" runat="server" class="check" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow" Text='<%# DataBinder.Eval(Container.DataItem, "CFellow") %>'>
                                                                                    <asp:ListItem Value="0" Selected="True">否</asp:ListItem>
                                                                                    <asp:ListItem Value="1">是</asp:ListItem>
                                                                                </asp:RadioButtonList>
                                                                            </td>
                                                                            <td>
                                                                                <asp:RadioButtonList ID="RBL_HCLife_Edit" runat="server" class="check" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow" Text='<%# DataBinder.Eval(Container.DataItem, "CLife") %>'>
                                                                                    <asp:ListItem Value="0">歿</asp:ListItem>
                                                                                    <asp:ListItem Value="1" Selected="True">存</asp:ListItem>
                                                                                </asp:RadioButtonList>
                                                                            </td>
                                                                            <td>
                                                                                <asp:RadioButtonList ID="RBL_HCLTogether_Edit" runat="server" class="check" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow" Text='<%# DataBinder.Eval(Container.DataItem, "CLTogether") %>'>
                                                                                    <asp:ListItem Value="0" Selected="True">否</asp:ListItem>
                                                                                    <asp:ListItem Value="1">是</asp:ListItem>
                                                                                </asp:RadioButtonList>
                                                                            </td>
                                                                        </tr>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>

                                <div class="form-group">
                                    <asp:LinkButton ID="LBtn_Education" runat="server" class="col-12 btn btn-secondary text-left moreinfo" OnClick="LBtn_Education_Click">
												<i class="ti-angle-down"></i>
												<label class="m-b-0 font-weight-bold">學經歷資料</label>
                                        <!--(Education and Experience Info.)-->
                                    </asp:LinkButton>
                                    <asp:Panel ID="Panel_Education" runat="server" Visible="false">
                                        <div class="hideinfo col-md-12 mb-4">
                                            <div class="row clearfix">
                                                <div class="col-md-3 col-sm-12">
                                                    <label>最高學歷</label>
                                                    <!--(Highest Education)-->
                                                    <div class="form-group">
                                                        <asp:DropDownList ID="DDL_HEducation" runat="server" CssClass="form-control  js-example-basic-single" Style="width: 100%">
                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                            <asp:ListItem Value="1">幼教</asp:ListItem>
                                                            <asp:ListItem Value="2">國小</asp:ListItem>
                                                            <asp:ListItem Value="3">國中</asp:ListItem>
                                                            <asp:ListItem Value="4">高職</asp:ListItem>
                                                            <asp:ListItem Value="5">高中</asp:ListItem>
                                                            <asp:ListItem Value="6">專科</asp:ListItem>
                                                            <asp:ListItem Value="7">大學</asp:ListItem>
                                                            <asp:ListItem Value="8">研究所</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>畢業學校/科系名稱</label>
                                                    <!-- (Graduated School/ Department)-->
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HGraduate" runat="server" class="form-control" placeholder="輸入40中文字" MaxLength="40"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>職業主類別</label>
                                                    <!-- (Occupation Main Category)-->
                                                    <div class="form-group">
                                                        <asp:SqlDataSource ID="SDS_WType" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HWType FROM HWType ORDER BY HID ASC"></asp:SqlDataSource>
                                                        <asp:DropDownList ID="DDL_HWorkType" runat="server" CssClass="form-control  js-example-basic-single" DataSourceID="SDS_WType" DataTextField="HWType" DataValueField="HID" AutoPostBack="true" OnSelectedIndexChanged="DDL_HWorkType_SelectedIndexChanged" AppendDataBoundItems="true" Style="width: 100%">
                                                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>職業次類別</label>
                                                    <!--(Occupation Subcategory)-->
                                                    <div class="form-group">
                                                        <asp:SqlDataSource ID="SDS_WTItem" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HWType, HWTItemName FROM HWTItem ORDER BY HID ASC"></asp:SqlDataSource>
                                                        <asp:DropDownList ID="DDL_HWTItem" runat="server" CssClass="form-control  js-example-basic-single" DataSourceID="SDS_WTItem" DataTextField="HWTItemName" DataValueField="HID" AppendDataBoundItems="true" Style="width: 100%">
                                                            <asp:ListItem Value="0">請先選擇職業主類別</asp:ListItem>

                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="TB_HWTOthers" runat="server" class="form-control" placeholder="請輸入其他職業類別名稱" MaxLength="40" Visible="false"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>服務機構全銜</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HServiceTitle" runat="server" class="form-control" placeholder="請輸入公司名稱"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>職務名稱</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HWorkName" runat="server" class="form-control" placeholder="請輸入職務名稱"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>學校層級</label>
                                                    <div class="form-group">
                                                        <asp:DropDownList ID="DDL_HSchoolLevel" runat="server" CssClass="form-control  js-example-basic-single" Style="width: 100%">
                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                            <asp:ListItem Value="1">幼教</asp:ListItem>
                                                            <asp:ListItem Value="2">國小</asp:ListItem>
                                                            <asp:ListItem Value="3">國中</asp:ListItem>
                                                            <asp:ListItem Value="4">高職)</asp:ListItem>
                                                            <asp:ListItem Value="5">高中</asp:ListItem>
                                                            <asp:ListItem Value="6">專科</asp:ListItem>
                                                            <asp:ListItem Value="7">大學</asp:ListItem>
                                                            <asp:ListItem Value="8">研究所</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-md-12 col-sm-12">
                                                    <label>職涯歷程</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HCareer" runat="server" class="form-control" placeholder="" TextMode="MultiLine" Rows="2"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row clearfix">
                                                <div class="col-md-3 col-sm-12 d-none">
                                                    <label>專長主分類</label>
                                                    <div class="form-group">
                                                        <asp:SqlDataSource ID="SDS_EPType" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HEPType FROM HEPType ORDER BY HID ASC"></asp:SqlDataSource>
                                                        <asp:DropDownList ID="DDL_EPType" runat="server" CssClass="form-control  js-example-basic-single" DataSourceID="SDS_EPType" DataTextField="HEPType" DataValueField="HID" AppendDataBoundItems="true" Style="width: 100%">
                                                            <%--AutoPostBack="true" OnSelectedIndexChanged="DDL_EPType_SelectedIndexChanged"--%>
                                                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                        </asp:DropDownList>

                                                    </div>
                                                </div>
                                                <div class="col-md-12 col-sm-12">
                                                    <label>專長</label>
                                                    <div class="form-group">
                                                        <asp:SqlDataSource ID="SDS_HEPItem" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HEPType, HEPItem FROM HEPItem ORDER BY HID ASC"></asp:SqlDataSource>
                                                        <asp:ListBox ID="LBox_HEPItem" runat="server" class="form-control LBox_Expertise" name="state" SelectionMode="Multiple" DataSourceID="SDS_HEPItem" DataTextField="HEPItem" DataValueField="HID" placeholder="請選擇" Style="width: 100%"></asp:ListBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </asp:Panel>
                                </div>

                                <div class="form-group">
                                    <asp:LinkButton ID="LBtn_Others" runat="server" class="col-12 btn btn-secondary text-left moreinfo" OnClick="LBtn_Others_Click">
												<i class="ti-angle-down"></i>
												<label class="m-b-0 font-weight-bold">其他資料</label>
                                    </asp:LinkButton>

                                    <asp:Panel ID="Panel_Others" runat="server" Visible="false">
                                        <div class="hideinfo col-md-12 mb-4">
                                            <div class="row clearfix">

                                                <div class="col-md-3 col-sm-12">
                                                    <label>其他任職單位全銜(除主職業以外受薪單位)</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HUnitName" runat="server" class="form-control" placeholder="請輸入其他任職單位名稱"></asp:TextBox>

                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>簽屬肖像及文稿授權書</label>
                                                    <div class="form-group">
                                                        <asp:RadioButtonList ID="RBL_HAuthor" runat="server" class="check" name="flat-radio" Checked data-radio="iradio_flat-blue" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                            <asp:ListItem Value="0">不同意</asp:ListItem>
                                                            <asp:ListItem Value="1">同意</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>

                                                <div class="col-md-3 col-sm-12">
                                                    <label>宗教信仰</label>
                                                    <div class="form-group">
                                                        <asp:ListBox ID="LBox_HReligion" runat="server" CssClass="form-control ListB_Multi" name="state" SelectionMode="Multiple" placeholder="請選擇" Style="width: 100%">
                                                            <asp:ListItem Value="1">佛教</asp:ListItem>
                                                            <asp:ListItem Value="2">道教</asp:ListItem>
                                                            <asp:ListItem Value="3">一貫道</asp:ListItem>
                                                            <asp:ListItem Value="4">基督教</asp:ListItem>
                                                            <asp:ListItem Value="5">天主教</asp:ListItem>
                                                            <asp:ListItem Value="6">伊斯蘭教</asp:ListItem>
                                                            <asp:ListItem Value="7">其他(others)</asp:ListItem>
                                                            <asp:ListItem Value="8">無(none)</asp:ListItem>
                                                        </asp:ListBox>
                                                    </div>
                                                </div>



                                                <div class="col-md-12 col-sm-12">
                                                    <label>志願服務</label>
                                                    <div class="form-group">
                                                        <table class="table table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center" style="width: 10%">執行</th>
                                                                    <th class="text-center" style="width: 10%">No</th>
                                                                    <th style="width: 40%">志願服務名稱</th>
                                                                    <th style="width: 40%">服務單位</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr>
                                                                    <td style="text-align: center;">
                                                                        <asp:LinkButton ID="LBtn_HVolunteer_Add" runat="server" title="新增" class="p-10" OnClick="LBtn_HVolunteer_Add_Click"><i class="ti-plus text-info"></i></asp:LinkButton>
                                                                    </td>
                                                                    <td class="text-center"></td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_SName_Add" runat="server" class="form-control"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_SUnit_Add" runat="server" class="form-control"></asp:TextBox>
                                                                    </td>

                                                                </tr>

                                                                <asp:Repeater ID="Rpt_HVolunteer" runat="server">
                                                                    <ItemTemplate>
                                                                        <tr>
                                                                            <td style="text-align: center;">
                                                                                <asp:LinkButton ID="LBtn_HVolunteerDel" runat="server" title="刪除" class="p-10" OnClick="LBtn_HVolunteerDel_Click" CommandArgument='<%# Container.ItemIndex%>'><i class="ti-trash text-danger"></i></asp:LinkButton>
                                                                            </td>
                                                                            <td class="text-center">
                                                                                <asp:Label ID="LB_HNo" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_SName_Edit" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "SName") %>' class="form-control" AutoComplete="off"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_SUnit_Edit" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "SUnit") %>' class="form-control" AutoComplete="off"></asp:TextBox>
                                                                            </td>

                                                                        </tr>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>


                                                <div class="col-md-12 col-sm-12">
                                                    <label>法門</label>
                                                    <div class="form-group">
                                                        <table class="table table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center" style="width: 10%">執行</th>
                                                                    <th class="text-center" style="width: 10%">No</th>
                                                                    <th style="width: 80%">法門(半年以上)</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr>
                                                                    <td style="text-align: center;">
                                                                        <asp:LinkButton ID="LBtn_HFamen_Add" runat="server" title="新增" class="p-10" OnClick="LBtn_HFamen_Add_Click"><i class="ti-plus text-info"></i></asp:LinkButton>
                                                                    </td>
                                                                    <td class="text-center"></td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_HFamen_Add" runat="server" class="form-control"></asp:TextBox>
                                                                    </td>
                                                                </tr>

                                                                <asp:Repeater ID="Rpt_HFamen" runat="server">
                                                                    <ItemTemplate>
                                                                        <tr>
                                                                            <td style="text-align: center;">
                                                                                <asp:LinkButton ID="LBtn_HFamenDel" runat="server" title="刪除" class="p-10" OnClick="LBtn_HFamenDel_Click" CommandArgument='<%# Container.ItemIndex%>'><i class="ti-trash text-danger"></i></asp:LinkButton>
                                                                            </td>
                                                                            <td class="text-center">
                                                                                <asp:Label ID="LB_HNo" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_HFamen_Edit" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Famen") %>' class="form-control" AutoComplete="off"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>


                                                <div class="col-md-12 col-sm-12">
                                                    <label>
                                                        參加(過)團體
														

                                                    </label>
                                                    <div class="form-group">
                                                        <table class="table table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center" style="width: 8%">執行</th>
                                                                    <th class="text-center" style="width: 5%">No</th>
                                                                    <th style="width: 15%">團體類別<a class="text-info font-weight-bold" data-toggle="modal" data-target="#info" style="cursor: pointer;"><i class="fa ti-help-alt ml-1"></i>&nbsp;</a></th>
                                                                    <th style="width: 25%">團體名稱</th>
                                                                    <th style="width: 18%">參加期間</th>
                                                                    <th style="width: 15%">擔任職務</th>
                                                                    <th style="width: 14%">職務層級</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr>
                                                                    <td style="text-align: center;">
                                                                        <asp:LinkButton ID="LBtn_HGroup_Add" runat="server" title="新增" class="p-10" OnClick="LBtn_HGroup_Add_Click"><i class="ti-plus text-info"></i></asp:LinkButton>
                                                                    </td>
                                                                    <td class="text-center"></td>
                                                                    <td>
                                                                        <asp:DropDownList ID="DDL_GType_Add" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DDL_GType_Add_SelectedIndexChanged" Style="width: 100%">
                                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                            <asp:ListItem Value="1">宗教團體</asp:ListItem>
                                                                            <asp:ListItem Value="2">身心靈成長團體</asp:ListItem>
                                                                            <asp:ListItem Value="3">服務團體</asp:ListItem>
                                                                            <asp:ListItem Value="4">民間社團</asp:ListItem>
                                                                            <asp:ListItem Value="5">其他</asp:ListItem>
                                                                        </asp:DropDownList><br />
                                                                        <asp:TextBox ID="TB_HGOthers_Add" runat="server" placeholder="請輸入其他團體別名稱" class="form-control" Style="width: 100%; margin-top: 5px;" Visible="false"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_GName_Add" runat="server" class="form-control"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_Duration_Add" runat="server" class="form-control daterange"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TB_Job_Add" runat="server" class="form-control"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="DDL_JobLevel_Add" runat="server" class="form-control" Style="width: 100%">
                                                                            <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                            <asp:ListItem Value="1">領導人</asp:ListItem>
                                                                            <asp:ListItem Value="2">幹部</asp:ListItem>
                                                                            <asp:ListItem Value="3">參與者</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>

                                                                <asp:Repeater ID="Rpt_HGroup" runat="server" OnItemDataBound="Rpt_HGroup_ItemDataBound">
                                                                    <ItemTemplate>
                                                                        <tr>
                                                                            <td style="text-align: center;">
                                                                                <asp:LinkButton ID="LBtn_HGroupDel" runat="server" title="刪除" class="p-10" OnClick="LBtn_HGroupDel_Click" CommandArgument='<%# Container.ItemIndex%>'><i class="ti-trash text-danger"></i></asp:LinkButton>
                                                                            </td>
                                                                            <td class="text-center">
                                                                                <asp:Label ID="LB_HNo" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:DropDownList ID="DDL_GType_Edit" runat="server" class="form-control" Text='<%# DataBinder.Eval(Container.DataItem, "GType") %>' Style="width: 100%">
                                                                                    <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                                    <asp:ListItem Value="1">宗教團體</asp:ListItem>
                                                                                    <asp:ListItem Value="2">身心靈成長團體</asp:ListItem>
                                                                                    <asp:ListItem Value="3">服務團體</asp:ListItem>
                                                                                    <asp:ListItem Value="4">民間社團</asp:ListItem>
                                                                                    <asp:ListItem Value="5">其他</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                                <br />
                                                                                <asp:TextBox ID="TB_HGOthers_Edit" runat="server" placeholder="請輸入其他團體別名稱" class="form-control" Style="width: 100%; margin-top: 5px;" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "GOtherType") %>'></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_GName_Edit" runat="server" class="form-control" Text='<%# DataBinder.Eval(Container.DataItem, "GName") %>'></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_Duration_Edit" runat="server" class="form-control" Text='<%# DataBinder.Eval(Container.DataItem, "Duration") %>'></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_Job_Edit" runat="server" class="form-control" Text='<%# DataBinder.Eval(Container.DataItem, "Job") %>'></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:DropDownList ID="DDL_JobLevel_Edit" runat="server" class="form-control" Text='<%# DataBinder.Eval(Container.DataItem, "JobLevel") %>' Style="width: 100%">
                                                                                    <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                                    <asp:ListItem Value="1">領導人</asp:ListItem>
                                                                                    <asp:ListItem Value="2">幹部</asp:ListItem>
                                                                                    <asp:ListItem Value="3">參與者</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>

                                                                        </tr>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>


                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>

                                <div class="form-group">
                                    <asp:LinkButton ID="LBtn_Manage" runat="server" class="col-12 btn btn-secondary text-left moreinfo" OnClick="LBtn_Manage_Click">
												<i class="ti-angle-down"></i>
												<label class="m-b-0 font-weight-bold">管理日期</label>
                                    </asp:LinkButton>

                                    <asp:Panel ID="Panel_Manage" runat="server" Visible="false">
                                        <div class="hideinfo col-md-12 mb-4">
                                            <div class="row clearfix">
                                                <div class="col-md-3 col-sm-12">
                                                    <label>暫停活動日期</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HSADate" runat="server" class="form-control datepicker" placeholder="1990/01/01"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>暫停活動原因</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HSAReason" runat="server" class="form-control" placeholder=""></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>退出日期</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HLeaveDT" runat="server" class="form-control datepicker" placeholder="1990/01/01"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>退出原因</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HQReason" runat="server" class="form-control" placeholder=""></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3 col-sm-12">
                                                    <label>回歸大愛光日期</label>
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TB_HDeadDT" runat="server" class="form-control datepicker" placeholder="1990/01/01"></asp:TextBox>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>


                                <div class="text-center">
                                    <asp:Button ID="Btn_Submit" runat="server" Text="儲存" CssClass="btn btn-info m-r-10" OnClick="Btn_Submit_Click" /><%--Btmessage="確定要儲存嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' --%>
                                    <asp:Button ID="Btn_Cancel" runat="server" Text="取消" class="btn btn-inverse  m-r-10" OnClick="Btn_Cancel_Click" Btmessage="確定要取消嗎？沒有儲存的内容將不會變更" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>

    <!-- Modal 參加(過)團體說明 START-->
    <div class="modal fade" id="info" tabindex="-1" role="dialog" aria-labelledby="info" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLongTitle">團體類別範例說明</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    ●　宗教團體：天主教、道教…<br />
                    ●　身心靈成長團體：靈氣、道場禪修、梅門、水晶治療、塔羅命數…<br />
                    ●　服務團體：家扶社工、環保志工…<br />
                    ●　民間社團：青商會、婦女會、獅子會…<br />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal 參加(過)團體說明  END-->


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
    <!--stickey kit -->
    <script src="assets/node_modules/sticky-kit-master/dist/sticky-kit.min.js"></script>
    <script src="assets/node_modules/sparkline/jquery.sparkline.min.js"></script>
    <!--Custom JavaScript -->
    <script src="dist/js/custom.min.js"></script>

    <!-- icheck -->
    <script src="assets/node_modules/icheck/icheck.min.js"></script>
    <script src="assets/node_modules/icheck/icheck.init.js"></script>
    <!--datepicker-->
    <script src="js/moment.min.js"></script>
    <script src="js/bootstrap-datepicker.js"></script>
    <!--daterangepicker-->
    <script src="js/daterangepicker.js"></script>
    <!--select2-->
    <script src="js/select2.min.js"></script>
    <!--sumoselect-->
    <script src="js/jquery.sumoselect.min.js"></script>
    <!--Custom JavaScript -->
    <script src="js/_custom.js"></script>


    <script>
        $(function () {
            //單選
            $('.js-example-basic-single').select2();



            //多選--sumoselect js
            $('.ListB_Multi').SumoSelect({
                search: true,
                placeholder: '請選擇',
                csvDispCount: 5,
            });

            $('.LBox_Expertise').SumoSelect({
                search: true,
                placeholder: '請選擇',
                csvDispCount: 10,
            });


            $('#<%= TB_HBirth.ClientID %>').datepicker({
                multidate: false,
                format: "yyyy/mm/dd",
                language: "zh-TW",
                autoclose: true,
                todayHighlight: true,
                orientation: 'bottom auto',
                //如果年份只輸入兩碼會擋掉
                startDate: "1900/01/01",
                endDate: new Date(),
            }).on('changeDate change input', function () {
                var val = $(this).val().trim();
                var notice = document.getElementById('<%= LB_BirthNotice.ClientID %>');

                // 簡單格式驗證 yyyy/mm/dd
                var dateRegex = /^\d{4}\/\d{2}\/\d{2}$/;
                var isValid = dateRegex.test(val) && !isNaN(Date.parse(val));

                if (!isValid) {
                    // 清空輸入
                    $(this).val('');

                    // 顯示提醒
                    if (notice) notice.style.display = 'inline';
                } else {
                    // 隱藏提醒
                    if (notice) notice.style.display = 'none';
                }

            });

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

