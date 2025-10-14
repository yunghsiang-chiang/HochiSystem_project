<%@ Page Title="" Language="C#" MasterPageFile="~/HochiSCMaster.master" AutoEventWireup="true" CodeFile="HSCModerator.aspx.cs" Inherits="HSCModerator" %>

<%@ Register Src="~/Paging.ascx" TagPrefix="Page" TagName="Paging" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .table {
            font-size: 1rem !important;
        }

        .btn-danger, .btn-success {
            padding: 0.1rem 0.7rem 0.2rem;
        }

        .list-pagination .page-link.active {
            background: #896ee2;
            color: #fff;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <main class="container-fluid" style="min-height: 50vh; margin-top: 0;">
        <div>
            <%--class="container"--%>

            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>

                    <div class="area">
                        <div class="sq">
                            <div id="twopart" style="padding-top: 0">
                                <div class="leftpart">
                                    <div class="sidearea">
                                        <h1 class="headtitle hottag all mt-0 font-weight-bold"><span class="ti-settings mr-1"></span>管理員功能</h1>

                                        <div class="defaultLeftMenu">
                                            <div class="defaultLeftList">
                                                <div class="defaultLeftContent">

                                                    <div class="panel-heading">
                                                        <asp:LinkButton ID="LBtn_EditHSCMRule" runat="server" Style="color: #896ee2" OnClick="LBtn_EditHSCMRule_Click">編輯版規</asp:LinkButton>
                                                    </div>
                                                    <div class="panel-heading">
                                                        <asp:LinkButton ID="LBtn_MemManage" runat="server" Style="color: #000" OnClick="LBtn_MemManage_Click">成員管理</asp:LinkButton>
                                                    </div>
                                                    <div class="panel-heading">
                                                        <asp:LinkButton ID="LBtn_PublicSetting" runat="server" Style="color: #000" OnClick="LBtn_PublicSetting_Click">公開設定</asp:LinkButton>
                                                    </div>
                                                    <div class="panel-heading">
                                                        <asp:LinkButton ID="LBtn_ReturnForum" runat="server" Style="color: #000" OnClick="LBtn_ReturnForum_Click">回討論區</asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                <div class="middlepart">

                                    <h3 class="font-weight-bold"><span class="ti-book mr-1"></span>
                                        <asp:Label ID="LB_HSCFCName" runat="server" Text=""></asp:Label>討論區管理
                                    </h3>
                                    <hr />

                                    <asp:Panel ID="Panel_Rule" runat="server">
                                        <div class="d-flex justify-content-between align-items-center">
                                            <h4 class="font-weight-bold">編輯版規</h4>

                                        </div>
                                        <div style="border-radius: 15px; background-color: rgba(255,255,255,0.8); padding: 10px">


                                            <CKEditor:CKEditorControl ID="CKE_HSCMRule" runat="server">
                                       注意事項：
                                    <ul>
                                            <li>1. 請勿發表不雅言論</li>
                                              <li>2. 未報名此課程的同修請先報名</li>
                                       </ul>
                                            </CKEditor:CKEditorControl>


                                            <div class="row text-center mt-3 ">
                                                <div class="col-md-12">
                                                    <asp:LinkButton ID="LBtn_HSCMRuleSubmit" runat="server" CssClass="btn btn-purple" OnClick="LBtn_HSCMRuleSubmit_Click">儲存</asp:LinkButton>
                                                    <asp:LinkButton ID="LBtn_HSCMRuleCancel" runat="server" CssClass="btn btn-secondary ml-2" OnClick="LBtn_HSCMRuleCancel_Click" Btmessage="確定要取消嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))'>取消</asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>


                                    <asp:Panel ID="Panel_SCTopic" runat="server" Visible="false">
                                        <div class="d-flex justify-content-between align-items-center">
                                            <h4 class="font-weight-bold">成員管理</h4>

                                        </div>
                                        <div style="border-radius: 15px; background-color: rgba(255,255,255,0.8); padding: 10px">

                                            <!--搜尋列 start-->
                                            <div class="col-md-12 search-bar">
                                                <div class="row">
                                                    <div class="col-md-5 pl-1 pr-1">
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_HSCTopicSearch" runat="server" CssClass="form-control" AutoComplete="off" Placeholder="請輸入主題名稱"></asp:TextBox>
                                                        </div>
                                                    </div>


                                                    <div class="col-md-2 pl-1 pr-1 sm-text-right">
                                                        <div class="form-group">
                                                            <asp:LinkButton ID="LBtn_HSCTopicSearch" runat="server" class="btn btn-outline-secondary ml-1 mr-1" OnClick="LBtn_HSCTopicSearch_Click">搜尋</asp:LinkButton>
                                                            <asp:LinkButton ID="LBtn_HSCTopicCancel" runat="server" class="btn btn-outline-secondary" OnClick="LBtn_HSCTopicCancel_Click">取消</asp:LinkButton>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>

                                            <!--搜尋列  end-->

                                            <!--此討論區的主題列表 start-->
                                            <div class="table-responsive">
                                                <table class="table table-striped table-bordered">
                                                    <thead>
                                                        <tr>
                                                            <th class="text-center" style="width: 5%">No</th>
                                                            <th style="width: 67%">主題名稱</th>
                                                            <th class="text-center" style="width: 13%">總人數</th>
                                                            <th class="text-center" style="width: 15%">執行</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:SqlDataSource ID="SDS_HSCTopic" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:Repeater ID="Rpt_HSCTopic" runat="server" DataSourceID="SDS_HSCTopic">
                                                            <ItemTemplate>
                                                                <asp:Label ID="LB_HCourseID" runat="server" Text='<%# Eval("HCourseID") %>' Visible="false"></asp:Label>
                                                                <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                                                <tr>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex + 1%>'></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HTopicName" runat="server" Text='<%# Eval("HTopicName") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_MemberNum" runat="server" Text="0"></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:LinkButton ID="LBtn_View" runat="server" class="btn btn-purple" OnClick="LBtn_View_Click" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Eval("HTopicName") %>'>查看</asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <div class="list-pagination text-center mt-30 md-mt-0 mb-5">
                                                <ul class="pagination justify-content-center sm-pl-5 sm-pr-5">
                                                    <li class="page-item">
                                                        <Page:Paging runat="server" ID="Pg_Paging" />
                                                        <%--<asp:LinkButton ID="LinkButton4" runat="server" Text="1" class="page-link active">--%><%--class="page-link"--%>
                                                        <%--</asp:LinkButton>--%>
                                                    </li>
                                                </ul>
                                            </div>
                                            <!--成員列表 end-->

                                        </div>
                                    </asp:Panel>





                                    <!--變成主題裡有多少成員-->
                                    <asp:Panel ID="Panel_Member" runat="server" Visible="false">
                                        <div class="d-flex justify-content-between align-items-center mb-2">
                                            <div class="d-flex justify-content-left align-items-center">
                                                <asp:LinkButton ID="LBtn_Back" runat="server" CssClass="btn btn-gray mr-1 pt-1 pb-1" ToolTip="返回" OnClick="LBtn_Back_Click">
                                                     <span class="ti-arrow-left"></span>
                                                </asp:LinkButton>
                                                <h4 class="font-weight-bold text-left">成員管理 / 
                                                <asp:Label ID="LB_HTopicName" runat="server" Text=""></asp:Label></h4>
                                            </div>
                                            <p class="mb-0 d-inline">
                                                <asp:Label ID="LB_HMemberNum" runat="server" Text="0"></asp:Label>位成員
                                            </p>
                                        </div>
                                        <div style="border-radius: 15px; background-color: rgba(255,255,255,0.8); padding: 10px">

                                            <asp:Label ID="LB_HSCTopicID" runat="server" Text="" Visible="false"></asp:Label>
                                            <asp:Label ID="LB_HCourseID" runat="server" Text="" Visible="false"></asp:Label>

                                            <!--搜尋列 start-->
                                            <div class="col-md-12 search-bar">
                                                <div class="row">
                                                    <div class="col-md-2 pl-1 pr-1">
                                                        <div class="form-group">
                                                            <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" AutoComplete="off" Placeholder="請輸入同修姓名"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-2 pl-1 pr-1">
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_HArea" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HArea from HArea order by HID ASC "></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HArea" runat="server" class="form-control  js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HArea" DataTextField="HArea" DataValueField="HID" AppendDataBoundItems="true">
                                                                <asp:ListItem Value="0">選擇區屬</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-2 pl-1 pr-1">
                                                        <div class="form-group">
                                                            <asp:SqlDataSource ID="SDS_System" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID,HSystemName FROM HSystem"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HSystem" runat="server" class="form-control  js-example-basic-single" Style="width: 100%" DataSourceID="SDS_System" DataTextField="HSystemName" DataValueField="HID" AppendDataBoundItems="true">
                                                                <asp:ListItem Value="0">選擇體系</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-2 pl-1 pr-1">
                                                        <div class="form-group">
                                                            <asp:DropDownList ID="DDL_HStatus" runat="server" class="form-control" Style="width: 100%">
                                                                <asp:ListItem Value="0">請選擇狀態</asp:ListItem>
                                                                <asp:ListItem Value="1">啟用</asp:ListItem>
                                                                <asp:ListItem Value="2">停用</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-2 pl-1 pr-1 sm-text-right">
                                                        <div class="form-group">
                                                            <asp:LinkButton ID="LBtn_Search" runat="server" class="btn btn-outline-secondary ml-1 mr-1" OnClick="LBtn_Search_Click">搜尋</asp:LinkButton>
                                                            <asp:LinkButton ID="LBtn_SearchCancel" runat="server" class="btn btn-outline-secondary" OnClick="LBtn_SearchCancel_Click">取消</asp:LinkButton>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                            <!--搜尋列  end-->


                                            <!--新增成員 start-->
                                            <!--新增成員-->
                                            <table class="mb-2" style="width:60%;">
                                                <tbody>
                                                    <tr runat="server" id="Tr_MemberAdd">
                                                        <td>新增成員：</td>
                                                        <td colspan="5">
                                                            <asp:SqlDataSource ID="SDS_HMemberID" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT a.HID, (b.HArea+'/'+a.HPeriod+' '+a.HUserName) AS HUserName, a.HStatus FROM HMember AS a LEFT JOIN HArea AS b ON a.HAreaID=b.HID WHERE a.HStatus='1'"></asp:SqlDataSource>
                                                            <asp:DropDownList ID="DDL_HMemberID" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HMemberID" DataTextField="HUserName" DataValueField="HID" AppendDataBoundItems="true">
                                                                <asp:ListItem Value="0">請選擇學員(區屬/期別 姓名)</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>


                                                        <td class="text-center">
                                                            <asp:LinkButton ID="LBtn_MemberAdd" runat="server" class="btn btn-info" OnClick="LBtn_MemberAdd_Click">新增</asp:LinkButton>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                            <!--新增成員 end-->

                                            <!--成員列表 start-->
                                            <div class="table-responsive">
                                                <table class="table table-striped table-bordered">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 10%">區屬</th>
                                                            <th style="width: 20%">光團</th>
                                                            <th style="width: 15%">體系</th>
                                                            <th style="width: 10%">期別</th>
                                                            <th style="width: 15%">姓名</th>
                                                            <th class="text-center" style="width: 15%">狀態</th>
                                                            <th class="text-center" style="width: 15%">執行</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>


                                                        <asp:SqlDataSource ID="SDS_HMember" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:Repeater ID="Rpt_HMember" runat="server" DataSourceID="SDS_HMember" OnItemDataBound="Rpt_HMember_ItemDataBound">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="LB_HArea" runat="server" Text='<%# Eval("HArea") %>'></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HTeamID" runat="server" Text='<%# Eval("HTeamID") %>'></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HSystem" runat="server" Text='<%# Eval("HSystemName") %>'></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HPeriod" runat="server" Text='<%# Eval("HPeriod") %>'></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LB_HUsername" runat="server" Text='<%# Eval("HUsername") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:Label ID="LB_HStatus" runat="server" Text='<%# Eval("HStatus") %>' CssClass="text-success"></asp:Label><!--狀態-->
                                                                    </td>
                                                                    <td class="text-center">
                                                                        <asp:LinkButton ID="LBtn_Stop" runat="server" class="btn btn-danger" ToolTip="停用" Btmessage="確定要停用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Stop_Click" Text="停用"></asp:LinkButton>
                                                                        <asp:LinkButton ID="LBtn_Upload" runat="server" class="btn btn-success" ToolTip="啟用" Btmessage="確定要啟用嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' Visible="false" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_Upload_Click" Text="啟用"></asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <div class="list-pagination text-center mt-30 md-mt-0 mb-5">
                                                <ul class="pagination justify-content-center sm-pl-5 sm-pr-5">
                                                    <li class="page-item">
                                                        <asp:LinkButton ID="lbPaging" runat="server" Text="1" class="page-link active"><%--class="page-link"--%>
                                                        </asp:LinkButton>
                                                    </li>
                                                </ul>
                                            </div>
                                            <!--成員列表 end-->


                                        </div>
                                    </asp:Panel>

                                    <asp:Panel ID="Panel_Setting" runat="server" Visible="false">
                                        <div class="d-flex justify-content-between align-items-center">
                                            <h4 class="font-weight-bold">公開設定</h4>

                                        </div>
                                        <div style="border-radius: 15px; background-color: rgba(255,255,255,0.8); padding: 10px">


                                            <asp:RadioButtonList ID="RBL_HPublic" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                <asp:ListItem Value="1" Style="margin-right: 10px;">公開</asp:ListItem>
                                                <asp:ListItem Value="0">不公開</asp:ListItem>
                                            </asp:RadioButtonList>


                                            <div class="row text-center mt-3 ">
                                                <div class="col-md-12">
                                                    <asp:LinkButton ID="LBtn_HPSSubmit" runat="server" CssClass="btn btn-purple" OnClick="LBtn_HPSSubmit_Click">儲存</asp:LinkButton>
                                                    <asp:LinkButton ID="LBtn_HPSCancel" runat="server" CssClass="btn btn-secondary ml-2" OnClick="LBtn_HPSCancel_Click" Btmessage="確定要取消嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))'>取消</asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>



                                </div>


                            </div>
                        </div>
                    </div>

                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>
            <!---------------------------------------->



        </div>
    </main>

</asp:Content>

