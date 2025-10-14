<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HCourseVerify.aspx.cs" Inherits="HCourseVerify" ValidateRequest="false" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<%--分頁--%>
<%@ Register TagPrefix="Page" TagName="Paging" Src="~/System/Paging_backend.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        .table td span {
            word-break: break-word;
            white-space: normal;
        }

        @media (max-width:768.98px) {
            .table td span {
                word-break: break-word;
                white-space: nowrap;
            }
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

            <asp:Panel ID="Panel_List" runat="server">
                <div class="block-header">
                    <div class="row">
                        <div class="col-lg-6 col-md-12 col-sm-12">
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>開班審核</h2>
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item active">開班審核</li>
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
                                <div class="row m-t-10">
                                    <div class="col-md-10 col-lg-12 col-xlg-12">
                                        <div class="form-group row m-b-0 p-l-10">

                                            <div class="col-md-4">
                                                <asp:TextBox ID="TB_Search" runat="server" class="form-control" placeholder="請輸入課程名稱" AutoComplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-md-2 col-sm-2 p-l-0">
                                                <asp:DropDownList ID="DDL_HOCPlace" runat="server" class="form-control js-example-basic-single" Style="width: 100%" placeholder="選擇上課地點">
                                                </asp:DropDownList>
                                            </div>

                                            <div class="col-md-4">
                                                <asp:LinkButton ID="LBtn_Search" runat="server" OnClick="LBtn_Search_Click" class="btn btn-outline-secondary"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                                <asp:LinkButton ID="LBtn_SearchCancel" runat="server" OnClick="LBtn_SearchCancel_Click" class="btn btn-outline-secondary m-l-10"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <span class="text-danger m-t-20">*預設先載入3個月的資料，可透過搜尋查舊資料</span>
                                <div class="table-responsive">
                                    <table class="table table-hover">
                                        <thead>
                                            <tr>
                                                <th class="text-center" style="width: 5%">執行</th>
                                                <th class="text-center" style="width: 5%">No</th>
                                                <%--<th style="width: 10%">審核單號</th>--%>
                                                <th style="width: 8%">申請人</th>
                                                <th style="width: 25%">課程名稱</th>
                                                <th style="width: 25%">上課地點</th>
                                                <th style="width: 12%">申請時間</th>
                                                <th style="width: 12%">核准時間</th>
                                                <th class="text-center" style="width: 12%">審核狀態</th>
                                            </tr>
                                        </thead>
                                        <tbody>



                                            <asp:SqlDataSource ID="SDS_HCourse" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                            <asp:Repeater ID="Rpt_HCourse" runat="server" OnItemDataBound="Rpt_HCourse_ItemDataBound">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="text-center">
                                                            <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success" ToolTip="編輯" OnClick="LBtn_Edit_Click" CommandArgument='<%# Eval("HID") %>'><i class="icon-pencil"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="LBtn_Del" runat="server" class="btn btn-sm btn-outline-danger js-sweetalert d-none" OnClick="LBtn_Del_Click" CommandArgument='<%# Eval("HID") %>' Btmessage="確定要刪除嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))'><i class="icon-trash"></i></asp:LinkButton>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HNo" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                        </td>
                                                        <%--<td>
															<asp:Label ID="LB_HVerifyNum" runat="server" Text='<%# Eval("HVerifyNum") %>'></asp:Label>
														</td>--%>
                                                        <td>
                                                            <asp:Label ID="LB_HApplicant" runat="server" Text='<%# Eval("Applicant") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HCName" runat="server" Text='<%# Eval("HCourseName") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HPlaceName" runat="server" Text='<%# Eval("HPlaceName") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HApplyTime" runat="server" Text='<%# Eval("HApplyTime") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HVTime" runat="server" Text='<%# Eval("HVerifyTime") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <div class="label label-default" id="Status" runat="server">
                                                                <asp:Label ID="LB_HVStatus" runat="server" Text='<%# Eval("HVerifyStatus") %>'></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>








                                        </tbody>
                                    </table>
                                    <!------------------分頁功能開始------------------>
                                    <nav class="box text-right">
                                        <Page:Paging runat="server" ID="Pg_Paging" />
                                    </nav>
                                    <!------------------分頁功能結束------------------>

                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>


















            <!-----==========================我是分隔線============================-->
            <asp:Panel ID="Panel_Edit" runat="server" Visible="false">
                <div class="block-header">
                    <div class="row">
                        <div class="col-lg-6 col-md-12 col-sm-12">
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>編輯開班審核</h2>
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item"><a href="HCourseVerify.aspx">開班審核</a></li>
                                <li class="breadcrumb-item active">編輯開班審核</li>
                            </ul>
                        </div>
                    </div>
                </div>
                <asp:Label ID="LB_HID" runat="server" Text="" Visible="false"></asp:Label>
                <div class="row">
                    <div class="col-12">
                        <div class="card">
                            <div class="card-body">
                                <div class="table-responsive">
                                    <table class="table table-bordered">
                                        <tr>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">課程代碼<%--課程編號--%></td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HCBatchNum" runat="server" Text="" /></td>
                                            <%--<asp:Label ID="LB_HCourseNum" runat="server" Text="" /></td>--%>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">課程名稱</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HCourseName" runat="server" Text="" />
                                                <asp:Label ID="LB_HCTemplateID" runat="server" Text="" Visible="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">課程講師</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HTeacherName" runat="server" Text="" />
                                                <asp:Label ID="LB_HTeacherHID" runat="server" Text="" Visible="false" />
                                                <asp:Label ID="LB_HMemberHID" runat="server" Text="" Visible="false" />
                                                <asp:Label ID="LB_HAreaID" runat="server" Text="" Visible="false" />
                                            </td>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">上課地點</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HOCPlace" runat="server" Text="" />

                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">課程日期</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HDateRange" runat="server" Text="" /></td>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">上課時間
                                            </td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LBHTime" runat="server" Text="" Visible="true" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">課程類別</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HType" runat="server" Text="" /></td>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">繳費帳戶</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HPMethod" runat="server" Text="" />
                                                <asp:Label ID="LB_HPMethod_temp" runat="server" Text="" Visible="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">受傳過之法條件</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HNRequirement" runat="server" Text="" />
                                            </td>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">學員類別限制</td>
                                            <td colspan="4">
                                                <asp:Label ID="LB_HIRestriction" runat="server" Text="" /></td>
                                        </tr>
                                        <tr>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">可開課體系</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HOSystem" runat="server" Text="" />
                                            </td>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">可報名體系</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HRSystem" runat="server" Text="" /></td>

                                        </tr>
                                        <tr>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">需要護持</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HNGuide" runat="server" Text="" /></td>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">課程須全到，才能通過課程</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HNFull" runat="server" Text="" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">主班團隊</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HTeam" runat="server" Text="" />
                                                <asp:Label ID="LB_HTeam_temp" runat="server" Text="" Visible="false" />
                                            </td>
                                            <td class="text-center font-weight-bold bg-light d-none" style="width: 15%">功課須繳交篇數與天數</td>
                                            <td style="width: 35%" class=" d-none">
                                                <asp:Label ID="LB_HNCW" runat="server" Text="" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">課程基本費用(台幣)</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HBCPoint" runat="server" Text="" /></td>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">需要前導課程</td>
                                            <td style="width: 35%">
                                                <asp:Label ID="LB_HNLCourse" runat="server" Text="" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="text-center font-weight-bold bg-light" style="width: 15%">備註</td>
                                            <td colspan="5">
                                                <asp:Label ID="LB_HRemark" runat="server" Text="" /></td>
                                        </tr>
                                    </table>
                                </div>








                                <label class="font-weight-bold">教材設定</label>
                                <div class="table-responsive">
                                    <table class="table table-bordered">
                                        <thead>
                                            <tr class="bg-light">
                                                <th class="text-center" style="width: 5%">No</th>
                                                <th style="width: 20%">教材名稱</th>
                                                <th style="width: 30%">上傳教材 <span class="text-info">(限PDF、mp3檔)</span></th>
                                                <th style="width: 40%">影片連結</th>
                                            </tr>
                                        </thead>
                                        <tbody>




                                            <asp:SqlDataSource ID="SDS_HCourseMaterial" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                            <asp:Repeater ID="Rpt_HCourseMaterial" runat="server" DataSourceID="SDS_HCourseMaterial" OnItemDataBound="Rpt_HCourseMaterial_ItemDataBound">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_No" runat="server" Text="" />
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HCMName" runat="server" Text='<%# Eval("HCMName") %>' />
                                                        </td>
                                                        <td>
                                                            <asp:HyperLink ID="HL_HCMaterial" runat="server" Target="_blank" NavigateUrl="#"><%# Eval("HCMaterial") %></asp:HyperLink>
                                                        </td>
                                                        <td>
                                                            <asp:HyperLink ID="HL_HCMLink" runat="server" Target="_blank" NavigateUrl="#">
                                                                <%--<%# Eval("HCMLink") %>--%>
                                                                <asp:Label ID="LB_HCMLink" runat="server" Text='<%# Eval("HCMLink") %>' />
                                                            </asp:HyperLink>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>





                                        </tbody>
                                    </table>
                                </div>









                                <label class="font-weight-bold">前導課程設定</label>
                                <div class="table-responsive">
                                    <table class="table table-bordered">
                                        <thead>

                                            <tr class="bg-light">
                                                <th class="text-center" style="width: 10%">No</th>
                                                <th style="width: 70%">前導課程名稱</th>
                                                <th class="text-right" style="width: 20%">折扣金額(元)</th>
                                            </tr>

                                        </thead>
                                        <tbody>

                                            <asp:SqlDataSource ID="SDS_HLeadingCourse" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                            <asp:Repeater ID="Rpt_HLeadingCourse" runat="server" DataSourceID="SDS_HLeadingCourse" OnItemDataBound="Rpt_HLeadingCourse_ItemDataBound">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex+1%>' />
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HLCourseID" runat="server" Text='<%# Eval("HTemplateName") %>' />
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HDiscount" runat="server" Text='<%# Eval("HDiscount") %>' />
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>


                                        </tbody>
                                    </table>
                                </div>








                                <label class="font-weight-bold">體系護持工作項目</label>
                                <div class="table-responsive">
                                    <table class="table table-bordered">
                                        <thead>
                                            <tr class="bg-light">
                                                <th class="text-center" style="width: 5%">No</th>
                                                <th style="width: 10%">所屬組別</th>
                                                <th style="width: 10%">任務職稱</th>
                                                <th class="d-none" style="width: 10%">人數</th>
                                                <th style="width: 40%">體系工作項目</th>
                                                <th style="width: 10%">負責組長</th>
                                                <th class="d-none" style="width: 15%">試務人員名單</th>
                                            </tr>
                                        </thead>
                                        <tbody>


                                            <asp:SqlDataSource ID="SDS_HTodoList" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                            <asp:Repeater ID="Rpt_HTodoList" runat="server" DataSourceID="SDS_HTodoList" OnItemDataBound="Rpt_HTodoList_ItemDataBound">
                                                <ItemTemplate>
                                                    <tbody>
                                                        <tr>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
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
                                                                <asp:TextBox ID="TB_HTaskContent" runat="server" class="form-control" placeholder="" Text='<%# Eval("HTaskContent") %>' Enabled="false"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:SqlDataSource ID="SDS_HGroupLeader" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand="select HMember.HID, HUserName,HAreaID, HPeriod, (HArea+'/'+HPeriod+' '+HUserName) as UserName from HMember Left Join HArea On HMember.HAreaID =HArea.HID order by HUserName"></asp:SqlDataSource>
                                                                <asp:DropDownList ID="DDL_HGroupLeader" runat="server" class="form-control js-example-basic-single" Text='<%# Eval("HGroupLeaderID") %>' Style="width: 100%" DataSourceID="SDS_HGroupLeader" DataTextField="UserName" DataValueField="HID" AppendDataBoundItems="true" Enabled="false">
                                                                    <asp:ListItem Value="0">-請選擇-</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td class="d-none">
                                                                <asp:SqlDataSource ID="SDS_HExamStaff" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HMember.HID, HUserName,HAreaID, HPeriod, (HArea+'/'+HPeriod+' '+HUserName) AS UserName FROM HMember LEFT JOIN HArea ON HMember.HAreaID =HArea.HID ORDER BY HUserName"></asp:SqlDataSource>
                                                                <asp:ListBox ID="LBox_HExamStaff" runat="server" class="form-control ListB_Multi" name="state" SelectionMode="Multiple" DataSourceID="SDS_HExamStaff" DataTextField="UserName" DataValueField="HID" Enabled="true" AppendDataBoundItems="true" Visible="false"></asp:ListBox>
                                                                <asp:Label ID="LB_HExamStaff" runat="server" Text='<%# Eval("HExamStaff") %>' Visible="true"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </ItemTemplate>
                                            </asp:Repeater>


                                        </tbody>
                                    </table>
                                </div>






                                <label class="m-b-0 font-weight-bold">內文</label>
                                <div class="table-responsive">

                                    <div>
                                        標題：
										<asp:Label ID="LB_HContentTitle" runat="server" Text="" />
                                    </div>
                                    內容：
									<div>
                                        <CKEditor:CKEditorControl ID="CKE_HContent" runat="server" ReadOnly="true"></CKEditor:CKEditorControl>
                                    </div>
                                </div>




                                <label class="font-weight-bold">
                                    簽辦意見 
                                </label>
                                <div class="table-responsive">
                                    <asp:TextBox ID="TB_HVOpinion" runat="server" class="form-control col-md-12 m-l-5" AutoComplete="off" Width="99%" Rows="10" TextMode="MultiLine"></asp:TextBox>
                                </div>







                                <label class="font-weight-bold">簽核紀錄</label>
                                <div class="table-responsive">
                                    <table class="table table-bordered">
                                        <thead>
                                            <tr class="bg-light">
                                                <th class="text-center" style="width: 10%">No</th>
                                                <th style="width: 30%">姓名</th>
                                                <th style="width: 20%">審核日期</th>
                                                <th style="width: 20%">審核結果</th>
                                                <th style="width: 20%">簽辦意見</th>
                                            </tr>
                                        </thead>
                                        <tbody>



                                            <asp:SqlDataSource ID="SDS_HCVerifyLog" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                            <asp:Repeater ID="Rpt_HCVerifyLog" runat="server" DataSourceID="SDS_HCVerifyLog" OnItemDataBound="Rpt_HCVerifyLog_ItemDataBound">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex+1%>' />
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HSignName" runat="server" Text='<%# Eval("HUserName") %>' />
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HVDate" runat="server" Text='<%# Eval("HVDate") %>' />
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HVResult" runat="server" Text='<%# Eval("HVResult") %>' />
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HVOpinion" runat="server" Text='<%# Eval("HVOpinion") %>' />
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>




                                        </tbody>
                                    </table>
                                </div>


                                <div class="table-responsive">
                                    <table class="table table-bordered">
                                        <thead>
                                            <tr class="bg-light">
                                                <th class="text-center" style="width: 10%">No</th>
                                                <th style="width: 30%">姓名</th>
                                                <th style="width: 20%">核准日期</th>
                                                <th style="width: 20%">核准結果</th>
                                                <th style="width: 20%">核准意見</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr class="d-none">
                                                <td class="text-center">
                                                    <asp:Label ID="LB_No" runat="server" Text="1" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="LB_HSignName" runat="server" Text="" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="LB_HVDate" runat="server" Text="" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="LB_HVResult" runat="server" Text="" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="LB_HVOpinion" runat="server" Text="" />
                                                </td>
                                            </tr>

                                        </tbody>
                                    </table>
                                </div>



                                <hr />



                                <div class="col-12 text-center">
                                    <asp:Button ID="Btn_Allow" runat="server" Text="通過" class="btn btn-success m-r-10" OnClick="Btn_Allow_Click" />
                                    <asp:Button ID="Btn_Deny" runat="server" Text="退審" class="btn btn-danger m-r-10" OnClick="Btn_Deny_Click" />



                                    <asp:Button ID="Btn_Cancel" runat="server" Text="取消" class="btn btn-inverse" OnClick="Btn_Cancel_Click" Btmessage="確定要取消嗎？沒有儲存的内容將不會變更" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

        </div>
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
    <!--datepicker-->
    <script src="js/bootstrap-datepicker.js"></script>
    <!--Select2-->
    <script src="js/select2.min.js"></script>
    <!--sumoselect-->
    <script src="js/jquery.sumoselect.min.js"></script>

    <!-- icheck -->
    <script src="assets/node_modules/icheck/icheck.min.js"></script>
    <script src="assets/node_modules/icheck/icheck.init.js"></script>
    
    <!--dropify js-->
    <script src="js/dropify.min.js"></script>

    <script>
        $(function () {
            //單選
            $('.js-example-basic-single').select2();
        });
    </script>


</asp:Content>

