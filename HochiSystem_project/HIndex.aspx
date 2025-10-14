<%@ Page Title="" Language="C#" MasterPageFile="~/HochiMaster.master" AutoEventWireup="true" CodeFile="HIndex.aspx.cs" Inherits="HIndex" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <style>
        #section5 ul li a {
            position: unset;
        }

        .list-wrap div.divitems {
            margin-top: 20px;
        }

        .button-gray {
            padding: 0 0.3rem !important;
            line-height: 2.7 !important;
        }

        @media (max-width: 992px) {
            .btn_inner {
                top: 0;
            }

            header {
                box-shadow: 0 0 5px rgba(0,0,0,.5);
            }
        }



        @media (min-width:960px) {
            .modal-content, .modal-body {
                width: 100%;
            }
        }
    </style>


    <div id="CourseLink" runat="server" class="container-fluid  courselink_area" style="background-color: #f6f6f6;" visible="true">
        <div class="container">
            <div class="row">
                <div class="col-md-12">

                    <ul id="Ul_Courselink" runat="server" visible="false" class="courselinklist">
                        <li class="course-item linktitle mr-2">【上課連結】</li>
                        <div class="courselinklist flex-row d-flex justify-content-start align-items-center flex-wrap">
                            <asp:SqlDataSource ID="SDS_HCourseLink" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                            <asp:Repeater ID="Rpt_HCourseLink" runat="server" OnItemDataBound="Rpt_HCourseLink_ItemDataBound">
                                <ItemTemplate>
                                    <asp:Label ID="LB_DateRange" runat="server" Text='<%# Eval("HDateRange") %>' Visible="false"></asp:Label>
                                    <asp:Label ID="LB_HAttend" runat="server" Text='<%# Eval("HAttend") %>' Visible="false"></asp:Label>
                                    <asp:Label ID="LB_CourseLink" runat="server" Text='<%# Eval("HCourseLink") %>' Visible="false"></asp:Label>
                                    <asp:Label ID="LB_HSATCourseLink" runat="server" Text='<%# Eval("HSATCourseLink") %>' Visible="false"></asp:Label>
                                    <asp:Label ID="LB_HSUNCourseLink" runat="server" Text='<%# Eval("HSUNCourseLink") %>' Visible="false"></asp:Label>

                                    <li class="course-item">
                                        <asp:Image ID="IMG_Zoom" runat="server" class="img-fluid" ImageUrl="images/icons/zoom.png" />
                                        <asp:LinkButton ID="LBtn_CourseLink" runat="server" CommandArgument='<%# Eval("HCourseID") +","+Eval("HCourseLink") %>' OnClick="LBtn_CourseLink_Click" Text='<%# Eval("HCName") %>'></asp:LinkButton>

                                        <asp:LinkButton ID="LBtn_SatCourseLink" runat="server" CommandArgument='<%# Eval("HCourseID") +","+Eval("HSATCourseLink") %>' OnClick="LBtn_CourseLink_Click" Text='<%# Eval("HCName") %>'></asp:LinkButton>

                                        <asp:LinkButton ID="LBtn_SunCourseLink" runat="server" CommandArgument='<%# Eval("HCourseID") +","+Eval("HSUNCourseLink") %>' OnClick="LBtn_CourseLink_Click" Text='<%# Eval("HCName") %>'></asp:LinkButton>
                                    </li>

                                </ItemTemplate>
                            </asp:Repeater>


                        </div>
                    </ul>

                    <ul id="Ul_TaskArea" runat="server" visible="false" class="courselinklist">
                        <li class="course-item linktitle mr-2">【護持者連結】</li>
                        <div class="mt-2 courselinklist flex-row d-flex justify-content-start align-items-center flex-wrap">
                            <asp:SqlDataSource ID="SDS_Task" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                            <asp:Repeater ID="Rpt_Task" runat="server" OnItemDataBound="Rpt_Task_ItemDataBound">
                                <ItemTemplate>
                                    <asp:Label ID="LB_DateRange" runat="server" Text='<%# Eval("HDateRange") %>' Visible="false"></asp:Label>
                                    <asp:Label ID="LB_HAttend" runat="server" Text='<%# Eval("HAttend") %>' Visible="false"></asp:Label>
                                    <asp:Label ID="LB_CourseLinkTask" runat="server" Text='<%# Eval("HCourseLinkTask") %>' Visible="false"></asp:Label>
                                    <li class="course-item">
                                        <asp:Image ID="IMG_Zoom" runat="server" class="img-fluid" ImageUrl="images/icons/zoom.png" />
                                        <asp:LinkButton ID="LBtn_CourseLinkTask" runat="server" CommandArgument='<%# Eval("HCourseLinkTask") %>' OnClick="LBtn_CourseLinkTask_Click" Text='<%# Eval("HCName") %>' Visible="true"></asp:LinkButton>
                                    </li>

                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </ul>

                    <ul id="Ul_RelayArea" runat="server" visible="false" class="courselinklist">
                        <li class="course-item linktitle mr-2">【慈場線連結】</li>
                        <div class="mt-2 courselinklist flex-row d-flex justify-content-start align-items-center flex-wrap">
                            <asp:SqlDataSource ID="SDS_Replay" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                            <asp:Repeater ID="Rpt_Replay" runat="server" OnItemDataBound="Rpt_Replay_ItemDataBound">
                                <ItemTemplate>
                                    <asp:Label ID="LB_DateRange" runat="server" Text='<%# Eval("HDateRange") %>' Visible="false"></asp:Label>
                                    <asp:Label ID="LB_HAttend" runat="server" Text='<%# Eval("HAttend") %>' Visible="false"></asp:Label>
                                    <asp:Label ID="LB_CourseLinkRelay" runat="server" Text='<%# Eval("HCourseLinkRelay") %>' Visible="false"></asp:Label>
                                    <li class="course-item">
                                        <asp:Image ID="IMG_Zoom" runat="server" class="img-fluid" ImageUrl="images/icons/zoom.png" />
                                        <asp:LinkButton ID="LBtn_CourseLinkRelay" runat="server" CommandArgument='<%# Eval("HCourseLinkRelay") %>' OnClick="LBtn_CourseLinkRelay_Click" Text='<%# Eval("HCName") %>' Visible="true"></asp:LinkButton>

                                    </li>

                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </ul>


                </div>
            </div>
        </div>
    </div>




    <!--banner-->
    <section id="section1" class="owl-carousel banner owl-theme">
        <asp:SqlDataSource ID="SDS_HIndexSlide" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand="SELECT HID, HTitle, HLink, HIMG, HStatus FROM HIndexSlide WHERE HStatus='1'"></asp:SqlDataSource>
        <asp:Repeater ID="Rpt_HIndexSlide" runat="server" DataSourceID="SDS_HIndexSlide" OnItemDataBound="Rpt_HIndexSlide_ItemDataBound">
            <ItemTemplate>
                <asp:Label ID="LB_SlideName" runat="server" Visible="false" Text='<%# Eval("HIMG") %>' />
                <asp:HyperLink ID="HL_Link" runat="server" NavigateUrl='<%# Eval("HLink") %>'>
                    <div class="el-background-img" id="slide" runat="server">
                    </div>
                </asp:HyperLink>
            </ItemTemplate>
        </asp:Repeater>
    </section>
    <!--banner-->


    <!--最新課程-->
    <section class="container-fluid index_activity pt-20" style="background-color: #fff">
        <div class="container" style="max-width: 1150px;">
            <h2 class="cus-h2">最新課程</h2>
            <div class="list-wrap row">
                <div class="col-md">
                    <div class="list-wrap row">
                        <asp:SqlDataSource ID="SDS_Course" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand="SELECT TOP(6) MAX(HCBatchNum) AS HCBatchNum,a.HCTemplateID, a.TypeName, a.HCourseName, a.HDateRange, a.HPMethod, MIN(b.Value) AS StartDate,HIRestriction, MAX(b.value) AS EndDate, IIF(a.HPMethod<>'0',a.HBCPoint*10,HBCPoint) AS HBCPoint, a.HSerial, a.HCDeadlineDay, a.HBookByDateYN, a.HCCPeriodYN, a.HCCPeriodDItem FROM HCourseList_View AS a Cross Apply SPLIT(Replace(HDateRange,' - ',','), ',') b WHERE  a.HVerifyStatus=2 AND a.HStatus=1 AND (a.HCourseName<>N'2024寒假春風化雨班會') AND a.HIndexShow=1  GROUP BY a.HCTemplateID, a.TypeName, a.HCourseName, a.HDateRange, a.HPMethod, a.HBCPoint, a.HSerial, a.HCDeadlineDay, HIRestriction ,a.HIndexShow,a.HIndexSort, a.HBookByDateYN, a.HCCPeriodYN, a.HCCPeriodDItem HAVING (IIF (HSerial='0', DATEADD(day, HCDeadlineDay*(-1), MIN(b.value)),DATEADD(day, HCDeadlineDay, MAX(b.value)))>(CONVERT(nvarchar, getdate(), 111)) )  ORDER BY a.HIndexSort ASC, IIF(DATEPART(Month,LEFT(CONVERT(nvarchar, getdate(), 111),10))=DATEPART(Month,MIN(b.Value)),'1' ,'2') ASC,IIF(DATEPART(YYYY,LEFT(CONVERT(nvarchar, getdate(), 111),10))=DATEPART(YYYY,MIN(b.Value)), '5', '6') ASC, StartDate ASC" ProviderName="System.Data.SqlClient"></asp:SqlDataSource>
                        <asp:Repeater ID="Rpt_Course" runat="server" DataSourceID="SDS_Course" OnItemDataBound="Rpt_Course_ItemDataBound">
                            <ItemTemplate>
                                <asp:Label ID="LB_HBCPoint" runat="server" Text='<%# Eval("HBCPoint") %>' Visible="false"></asp:Label>
                                <asp:Label ID="LB_HCBatchNum" runat="server" Text='<%# Eval("HCBatchNum") %>' Visible="false"></asp:Label>
                                <asp:Label ID="LB_HIRestriction" runat="server" Text='<%# Eval("HIRestriction") %>' Visible="false"></asp:Label>
                                <asp:Label ID="LB_HPMethod" runat="server" Text='<%# Eval("HPMethod") %>' Visible="false"></asp:Label>
                                <asp:Label ID="LB_BookedDates" runat="server" Text="" Visible="false"></asp:Label>
                                <div class="col-lg-4 col-md-6 divitems">
                                    <div class="grid-list-item">
                                        <div class="grid-list-label" id="NEW_item" runat="server" visible="false">最新</div>
                                        <div class="grid-list-cate">
                                            <asp:Label ID="LB_TypeName" runat="server" Text='<%# Eval("TypeName") %>'></asp:Label>
                                        </div>
                                        <h5 class="grid-list-title multiline-ellipsis"><%--text_overflow--%>
                                            <asp:Label ID="LB_HCourseName" runat="server" Text='<%# Eval("HCourseName") %>'></asp:Label></h5>
                                        <div class="grid-list-detail" style="height: 64px;">
                                            <p>
                                                <i class="fas fa-calendar"></i>
                                                <asp:Label ID="LB_HDateRange" runat="server" Text='<%# Eval("HDateRange") %>'></asp:Label>
                                            </p>
                                        </div>
                                        <div class="d-flex justify-content-around button_part">
                                            <asp:LinkButton ID="LBtn_CourseBooking" runat="server" CausesValidation="false" Visible="true" CssClass="font-weight-bold" OnClick="LBtn_CourseBooking_Click" CommandArgument='<%# Eval("HCTemplateID")+"&"+ Eval("HCourseName")+"&"+ Eval("HDateRange")+"&"+Eval("HPMethod") %>'><span class="fas fa-book-open mr-2"></span>加入報名清單</asp:LinkButton>
                                            <asp:LinkButton ID="LBtn_Pay" runat="server" CausesValidation="false" Visible="false" CssClass="button button-yellow" OnClick="LBtn_Pay_Click"><span class="fas fa-wallet mr-1"></span>尚未完成付款</asp:LinkButton>
                                            <asp:LinkButton ID="LBtn_QuickPay" runat="server" CausesValidation="false" Visible="false" CssClass="font-weight-bold" OnClick="LBtn_QuickPay_Click" Style="width: 100% !important" CommandArgument='<%#  Eval("HCourseName")+"^"+ Eval("HDateRange")+"^"+Eval("HCBatchNum") %>'><span class="fas fa-book-open mr-2"></span>直接報名</asp:LinkButton>
                                            <asp:LinkButton ID="LBtn_CourseDonate" runat="server" CausesValidation="false" Visible="true" CssClass="font-weight-bold" OnClick="LBtn_CourseDonate_Click" CommandArgument='<%# Eval("HCTemplateID")+"&"+ Eval("HCourseName")+"&"+ Eval("HDateRange")+"&"+Eval("HPMethod") %>' CommandName='<%# Eval("HCCPeriodDItem")%>'><span class="fa fa-hand-holding-heart mr-2"></span>加入捐款清單</asp:LinkButton>
                                        </div>
                                    </div>
                                </div>

                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
    </section>




    <!--EIP最新消息-->
    <section class="container-fluid index_news">
        <div class="container" style="max-width: 1150px;">
            <h2 class="cus-h2">最新消息</h2>
            <div class="list-wrap row">
                <div class="col-md">
                    <div class="list-wrap row">
                        <asp:SqlDataSource ID="SDS_HEIPBroadcast" runat="server" ConnectionString="<%$ ConnectionStrings:HochiEIPConnection %>" SelectCommand="SELECT b_id, b_subunitname, b_title, b_content, b_cdate, b_udate FROM broadcast WHERE (REPLACE(b_subunitname, ' ', '') LIKE '%課程%' OR REPLACE(b_subunitname, ' ', '') LIKE '%活動%') ORDER BY b_udate DESC LIMIT 6 OFFSET 0" ProviderName="MySql.Data.MySqlClient"></asp:SqlDataSource>
                        <!-- OR (b_title LIKE '%課程%' OR b_title LIKE '%活動%')) AND b_istop <> 1-->

                        <asp:Repeater ID="Rpt_HEIPBroadcast" runat="server" DataSourceID="SDS_HEIPBroadcast" OnItemDataBound="Rpt_HEIPBroadcast_ItemDataBound">
                            <ItemTemplate>
                                <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("b_id") %>' Visible="false"></asp:Label>
                                <asp:Label ID="LB_HModifyDT" runat="server" Text='<%# Eval("b_udate") %>' Visible="false"></asp:Label>
                                <div class="col-lg-4 col-md-6 mb-4">
                                    <div class="grid-list-item">
                                        <div class="grid-list-label" runat="server" id="NEW_item" visible="false">最新</div>
                                        <div class="grid-list-img" id="IMG_New" runat="server" visible="false" style="background-size: contain">
                                            <asp:Label ID="LB_NewsPic" runat="server" Visible="false"></asp:Label>
                                        </div>
                                        <div class="grid-list-cate">
                                            <i class="fa fa-folder pr-1"></i>
                                            <asp:Label ID="LB_HClass" runat="server" Text='<%# Eval("b_subunitname") %>' CssClass="classtag" Visible="true"></asp:Label>
                                            <i class="fa fa-calendar pr-1 ml-3"></i>
                                            <asp:Label ID="LB_MDate" runat="server" Text='<%# Eval("b_udate") %>' CssClass="datetag" Visible="true"></asp:Label>


                                        </div>
                                        <h5 class="grid-list-title  font-weight-bold">
                                            <asp:Label ID="LB_HTitle" runat="server" Text='<%# Eval("b_title") %>' Visible="true" CssClass="title_ellipsis"></asp:Label></h5>

                                        <%-- <p class="text_overflow">--%>
                                        <asp:Label ID="LB_HContent" runat="server" Text='<%# Eval("b_content") %>' Visible="true" CssClass="news-content text_overflow"></asp:Label>
                                        <%-- </p>--%>
                                        <asp:LinkButton ID="LBtn_Detail" runat="server" class="grid-list-link" CommandArgument='<%# Container.ItemIndex %>'>
                                           瞭解更多 <span class="ti-angle-right"></span></asp:LinkButton>

                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
    </section>


    <!--體驗表單-->
    <section id="section11" class="container-fluid" style="background-color: #f3f3f3">
        <div class="container center" style="max-width: 1150px;">
            <div class="row cus-row center">
                <div class="col-md-4">
                    <img src="images/about-img-01.jpg">
                </div>
                <div class="col-md-3 contact_title">
                    <h5>如有任何提問及需求，</h5>
                    <h5>請在此留言！</h5>
                </div>
                <div class="col-md-5">
                    <div style="margin-top: 10px" class="input-group mb-3">
                        <asp:TextBox ID="TB_Name" runat="server" class="form-control" placeholder="請輸入您的姓名"></asp:TextBox>
                    </div>
                    <div class="input-group mb-3">
                        <asp:TextBox ID="TB_Email" runat="server" class="form-control" placeholder="請輸入您的電子信箱"></asp:TextBox>
                    </div>
                    <div class="input-group mb-3">
                        <textarea class="form-control" id="TA_Content" runat="server" placeholder="請輸入您欲詢問的內容"></textarea>
                    </div>
                    <div class="input-group mb-0">
                        <asp:TextBox ID="TB_Captcha" runat="server" class="form-control" AutoComplete="off" placeholder="請輸入安全圖碼" Style="width: 50%"></asp:TextBox>
                        <div class="captcha-img form-group" style="width: 50%">
                            <img src="ValidNumber.ashx" alt="安全圖碼" style="width: 63%" />
                            <asp:LinkButton ID="LBtn_NewCaptcha" runat="server" alt="更換圖碼" title="Regenerate" class="captcha-btn"><i class="fas fa-sync-alt"></i></asp:LinkButton><!--更換圖碼-->
                        </div>
                    </div>

                    <div class="form-group cus-row">
                        <div>
                            <asp:CheckBox ID="CB_Subscribe" runat="server" Style="width: 16px; height: 16px; vertical-align: middle;" />
                            <span for="CB_Subscribe" style="font-size: 15px; color: #666666">我已閱讀並同意接受<a href="javascript:void(0)" style="text-decoration: underline; color: #9b9b9b; font-size: 14px">個資保護聲明</a></span>
                        </div>
                    </div>
                    <div class="cus-row end">
                        <asp:LinkButton ID="LBtn_Submit" runat="server" class="btn btn-primary m-0" Style="width: 100%" Text="送出" OnClick="LBtn_Submit_Click"></asp:LinkButton>
                        <%--<button type="button" class="btn btn-primary learn-submit" style="width: 100%">送出</button>--%>
                    </div>


                </div>
            </div>
        </div>
    </section>



    <asp:LinkButton ID="LBtn_FastRollcall" runat="server" class="side-btn-wrap side-rollcall" OnClick="LBtn_FastRollcall_Click"><%-- data-toggle="modal" data-target="#Div_Rollcall"--%>
        <div class="side-btn-box">
            <span class="side-btn-text">快速報到</span>
            <span class="side-btn-icon">
            <span class="side-btn-book side-icon"></span>
            </span>
        </div>
    </asp:LinkButton>

    <!--快速報到 Modal-->
    <div class="modal fade hochi-modal" id="Div_Rollcall" tabindex="-1" role="dialog" aria-labelledby="Div_Rollcall" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog" role="document" style="max-width: 60%;">
            <div class="modal-content">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title" id="member-edit-password-title">快速報到-今日課程</h5>
                    <button type="button" id="closeModalBtn" class="close" data-dismiss="modal" aria-label="關閉視窗">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="hochi-form">
                    <div class="modal-body p-2">

                        <div class="text-danger">
                            *請該課程會到慈場傳光點上課的師兄姊，務必先點選實體報到
因為系統會記錄第一次報到的狀態。
                        </div>
                        <div class="table-respponsive">
                            <table class="table table-bordered table-striped table-rollcall mb-0 table-mobile">
                                <thead>
                                    <tr class="font-weight-bold">
                                        <th>課程名稱</th>
                                        <th class="text-center d-none">報到狀態</th>
                                        <th class="text-center">實體報到</th>
                                        <th class="text-center">線上報到</th>
                                        <th class="text-center">請假</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:SqlDataSource ID="SDS_HRollcall" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                    <asp:Repeater ID="Rpt_HRollcall" runat="server" DataSourceID="SDS_HRollcall" OnItemDataBound="Rpt_HRollcall_ItemDataBound">
                                        <ItemTemplate>
                                            <asp:Label ID="LB_HCourseID" runat="server" CssClass="rocallname" Text='<%# Eval("HCourseID") %>' Visible="false"></asp:Label>

                                            <asp:Label ID="LB_HAttend" runat="server" CssClass="rocallname" Text='<%# Eval("HAttend") %>' Visible="false"></asp:Label>

                                            <tr id="Tr_Course" runat="server">
                                                <td data-title="課程名稱" class="mobile-left">
                                                    <asp:Label ID="LB_HCourseName" runat="server" CssClass="rocallname" Text='<%# Eval("HCourseName") %>'></asp:Label>
                                                    <br />
                                                    <span class="fa fa-map-marker-alt text-gray rollcallplace"></span>
                                                    <asp:Label ID="LB_HPlaceName" runat="server" CssClass="rollcallplace" Text='<%# Eval("HPlaceName") %>'></asp:Label>
                                                </td>
                                                <td data-title="報到狀態" class="text-center d-none">
                                                    <asp:Label ID="LB_HAStatus" runat="server" Text=""></asp:Label>
                                                </td>

                                                <td data-title="實體報到" class="text-center mobile-left">
                                                    <asp:Button ID="Btn_Rollcall" runat="server" CssClass="btn btn-success p-2" Text="實體報到" CommandArgument='<%# Eval("HCourseID") %>' OnClick="Btn_Rollcall_Click" />
                                                </td>
                                                <td data-title="線上報到" class="text-center mobile-left ">
                                                    <div class="d-flex flex-column flex-wrap align-items-center justify-content-around online-rollcall">
                                                        <asp:LinkButton ID="LBtn_CourseLink" runat="server" CssClass="btn button-purple p-2 w-auto" CommandArgument='<%# Eval("HCourseID") +","+Eval("HCourseLink") %>' OnClick="LBtn_MCourseLink_Click" Text="線上報到"></asp:LinkButton>

                                                        <asp:LinkButton ID="LBtn_SatCourseLink" runat="server" CssClass="btn button-purple p-2 w-auto" CommandArgument='<%# Eval("HCourseID") +","+Eval("HSATCourseLink") %>' OnClick="LBtn_MCourseLink_Click" Text="線上報到" Visible="false"></asp:LinkButton>

                                                        <asp:LinkButton ID="LBtn_SunCourseLink" runat="server" CssClass="btn button-purple p-2 w-auto" CommandArgument='<%# Eval("HCourseID") +","+Eval("HSUNCourseLink") %>' OnClick="LBtn_MCourseLink_Click" Text="線上報到" Visible="false"></asp:LinkButton>

                                                        <asp:LinkButton ID="LBtn_CourseLinkTask" runat="server" CssClass="btn button-purple p-2 w-auto" CommandArgument='<%# Eval("HCourseLinkTask") %>' OnClick="LBtn_MCourseLinkTask_Click" Text="護持者連結" Visible="false"></asp:LinkButton>

                                                        <asp:LinkButton ID="LBtn_CourseLinkRelay" runat="server" CssClass="btn button-purple p-2 w-auto" CommandArgument='<%# Eval("HCourseLinkRelay") %>' OnClick="LBtn_MCourseLinkRelay_Click" Text="慈場線連結" Visible="false"></asp:LinkButton>
                                                    </div>
                                                </td>
                                                <td data-title="請假" class="text-center mobile-left">
                                                    <asp:Button ID="Btn_DayOff" runat="server" CssClass="btn btn-outline-primary p-2" Text="請假" CommandArgument='<%# Eval("HCourseID") %>' OnClick="Btn_DayOff_Click" />
                                                </td>

                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div class="modal-footer justify-content-center">

                        <a id="ModalClose" class="button button-gray p-2" style="line-height: 1rem;" href="javascript:void(0);" data-dismiss="modal"><i class="fas fa-window-close pr-2"></i>關閉視窗</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="pnQuickLead" runat="server" Visible="false">
        <a id="lnkQuickLead" runat="server"
            class="quick-lead-fab"
            href="#"
            title="新朋友 CRM 接引紀錄">
            <i class="fas fa-user-plus" aria-hidden="true"></i>
        </a>
    </asp:Panel>

    <style>
        /* 浮動按鈕：不受父層版型影響 */
        .quick-lead-fab {
            position: fixed;
            right: 16px;
            bottom: 20px;
            width: 54px;
            height: 54px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            text-decoration: none !important;
            background: #28a745; /* 綠色 */
            color: #fff !important; /* 白字 */
            box-shadow: 0 6px 16px rgba(0,0,0,.25);
            z-index: 1050; /* 蓋過大多數元件 */
        }

            .quick-lead-fab:hover {
                filter: brightness(1.06);
            }

            .quick-lead-fab i {
                font-size: 22px;
                line-height: 1;
            }
    </style>


    <script src="js/jquery-3.4.1.min.js"></script>
    <script src="js/popper.js"></script>
    <script src="bootstrap-4.4.1/js/bootstrap.min.js"></script>
    <script src="fonts/fontawesome-5.12.0/js/fontawesome.min.js"></script>
    <script src="fonts/fontawesome-5.12.0/js/brands.min.js"></script>
    <script src="js/owl.carousel.min.js"></script>
    <script src="js/custom.js"></script>
    <script src="js/jquery-modal-video.min.js"></script>
    <script>
        $(function () {
            var transient = {};

            $(".owl-carousel.banner").owlCarousel({
                items: 1,
                dots: false,
                loop: true,
                autoplay: true,
                autoplayTimeout: 5000,
                responsive: {
                    0: {
                        nav: false,
                        autoplay: true,
                    },
                    768: {
                        nav: true,
                        autoplay: true,
                    }
                },
            });

            $(".owl-carousel.logo").owlCarousel({
                items: 5,
                slideBy: 1,
                dots: false,
                nav: true,
                margin: 125,
                loop: true,
                autoplay: true,
                autoplayTimeout: 3000,
                responsive: {
                    0: {
                        items: 3,
                        margin: 0,
                        slideBy: 'page',
                        autoplay: true,
                        onDrag: onDrag.bind(transient),
                        onDragged: onDragged.bind(transient)
                    },
                    960: {
                        items: 5,
                        autoplay: true,
                    }
                },
            });
            //$(".js-modal-btn").modalVideo();

            function onDrag(event) {
                this.initialCurrent = event.relatedTarget.current();
            }

            function onDragged(event) {
                var owl = event.relatedTarget;
                var draggedCurrent = owl.current();

                if (draggedCurrent > this.initialCurrent) {
                    owl.current(this.initialCurrent);
                    owl.next();
                } else if (draggedCurrent < this.initialCurrent) {
                    owl.current(this.initialCurrent);
                    owl.prev();
                }
            }


        });


        // if menu is open then true, if closed then false
        // we start with false
        var open = false;
        // just a function to print out message
        function isOpen() {
            if (open) {
                $("select").css('background-image', "url('images/form-arrow-on.png')");
            } else {
                $("select").css('background-image', "url('images/form-arrow.png')");
            }
        }
        // on each click toggle the "open" variable
        $("select").on("click", function () {
            open = !open;
            isOpen();
        });
        // on each blur toggle the "open" variable
        // fire only if menu is already in "open" state
        $("select").on("blur", function () {
            if (open) {
                open = !open;
                isOpen();
            }
        });
        // on ESC key toggle the "open" variable only if menu is in "open" state
        $(document).keyup(function (e) {
            if (e.keyCode == 27) {
                if (open) {
                    open = !open;
                    isOpen();
                }
            }
        });
    </script>
    <script>
        // 關閉 modal 並更新歷史記錄
        document.getElementById("closeModalBtn").addEventListener("click", function () {
            window.history.replaceState({ modalClosed: true }, document.title, window.location.href);
        });

        document.getElementById("ModalClose").addEventListener("click", function () {
            window.history.replaceState({ modalClosed: true }, document.title, window.location.href);
        });

        // 檢查歷史記錄中的狀態
        window.addEventListener("popstate", function (event) {
            if (event.state && event.state.modalClosed) {
                // 不顯示 modal
            } else {
                $('#Div_Rollcall').modal('show');
            }
        });
    </script>


    <script>
        //--日期格式轉換
        document.addEventListener("DOMContentLoaded", function () {
            let gDateElements = document.querySelectorAll("[id*='Rpt_Course'][id*='LB_HDateRange']");
            gDateElements.forEach(label => {
                //console.log(label.innerText); // 或 label.textContent

                let gDateValue = label.innerText.trim(); // 取得日期字串並去除空白
                let gReturnDate;

                if (gDateValue.includes(",")) {
                    // 如果包含逗號（,），執行 formatDates()
                    gReturnDate = formatDates(gDateValue);
                    label.innerText = gReturnDate;
                }
                else if (gDateValue.match(/^\d{4}\/\d{2}\/\d{2} - \d{4}\/\d{2}\/\d{2}$/)) {
                    // 如果是 YYYY/MM/DD-YYYY/MM/DD，執行 formatDashDates()
                    gReturnDate = formatDashDates(gDateValue);
                    label.innerText = gReturnDate;

                } else {
                    // 否則，顯示原始值
                    //console.log(`未變更日期 gReturnDate[${index}] = ${gDateValue}`);
                }
            });

        });

        // 格式化逗號分隔的日期
        function formatDates(dateString) {
            if (!dateString) return ""; // 避免空值錯誤

            const dates = dateString.split(','); // 解析日期字串
            const currentYear = new Date().getFullYear(); // 取得當前年份

            return dates.map(date => {
                let [year, month, day] = date.split('/'); // 拆解日期
                return (parseInt(year) === currentYear) ? `${month}/${day}` : `${year}/${month}/${day}`;
            }).join(',');
        }

        // 處理 YYYY/MM/DD-YYYY/MM/DD 格式，轉換為 MM/DD-MM/DD
        // 處理 YYYY/MM/DD-YYYY/MM/DD 格式，轉換為 MM/DD-MM/DD 或 YYYY/MM/DD-YYYY/MM/DD
        function formatDashDates(dateString) {
            if (!dateString) return ""; // 避免空值錯誤

            let [startDate, endDate] = dateString.split('-').map(date => date.trim()); // 移除可能的空白
            let [startYear, startMonth, startDay] = startDate.split('/').map(Number); // 轉為數值
            let [endYear, endMonth, endDay] = endDate.split('/').map(Number); // 轉為數值

            if (startYear === endYear) {
                // 如果起始年和結束年相同，只顯示 MM/DD-MM/DD
                return `${startMonth}/${startDay}-${endMonth}/${endDay}`;
            } else {
                // 如果跨年，顯示 MM/DD/YYYY-MM/DD/YYYY
                return `${startYear}/${startMonth}/${startDay} - ${endYear}/${endMonth}/${endDay}`;
            }
        }



    </script>


    <script>
        function Notice() {
            let text = "此課程已存在未付款訂單中，系統將為您導向訂單紀錄頁面進行結帳~\n若同意請按【確定】。\n若想下次再進行結帳請按【取消】。";
            //let text = "您勾選的課程上課地點與您的區屬不同哦~\n再請確認地點是否正確";
            if (confirm(text) == true) {
                window.location = "HMember_Order.aspx";
                return;
            } else {
                //javascript: history.back(-1);
                return;
            }
        }
    </script>

    <script>
        function NoticeCCPeriod(gDItemID) {

            let text = "此課程僅開放使用信用卡授權付款方式，系統將為您導向信用卡授權申請頁面~\n若同意請按【確定】。\n若想下次再進行申請請按【取消】。";
            if (confirm(text) == true) {
                window.location = "HMember_CCPeriod.aspx?Apply=1&DItem=" + gDItemID;
                return;
            } else {
                //javascript: history.back(-1);
                return;
            }
        }
    </script>

</asp:Content>

