<%@ Page Title="" Language="C#" MasterPageFile="~/HochiSCMaster.master" AutoEventWireup="true" CodeFile="HSCForum.aspx.cs" Inherits="HSCForum" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .spinner {
            display: inline-block;
            width: 20px;
            height: 20px;
            border: 3px solid #ccc;
            border-top: 3px solid #7e4bc2;
            border-radius: 50%;
            animation: spin 0.6s linear infinite;
            vertical-align: middle;
            margin-right: 8px;
            margin-bottom: 5px;
        }

        #loading {
            display: block;
            text-align: center;
            margin-bottom: 50px;
            color: #895197;
        }

        @keyframes spin {
            to {
                transform: rotate(360deg);
            }
        }


        #twopart .middlepart {
            margin-left: 15%;
        }

        canvas {
            width: 100% !important;
        }

        @media (max-width:767px) {
            #twopart .middlepart {
                margin-left: 0 !important;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:Label ID="LB_HSCForumClassAID" runat="server" Text="" Visible="false"></asp:Label>
    <asp:Label ID="LB_HSCForumClassBID" runat="server" Text="" Visible="false"></asp:Label>
    <asp:HiddenField ID="HF_CurrentIndex" runat="server" Value="10" />

    <div class="middlepart">

        <p class="mb-2">
            <a href="HSCIndex.aspx">回專欄首頁</a> > 
             <asp:Label ID="LB_HSCForumClassA" runat="server" Text=""></asp:Label>
        </p>
        <div class="d-flex justify-content-between align-items-center">
            <h4 class="font-weight-bold">
                <asp:Literal ID="LTR_HSCForumClassB" runat="server"></asp:Literal>
            </h4>
        </div>



        <div class="discussarea">
            <!--討論區名稱-->
            <div class="col-md-12 sm-area">
                <asp:Label ID="LB_HCKeyWord" runat="server" Text="0" Visible="false"></asp:Label>
                <asp:Label ID="LB_HCKeyWordID" runat="server" Text="0" Visible="false"></asp:Label>

                <ul class="d-flex list-unstyled align-items-center justify-content-start ul_board mb-2">
                    <li class="d-none">討論區名稱關鍵字： </li>

                    <li runat="server" id="Li_HSCForumClassCAll" class="mr-2 btn btn-outline-purple btn-rounded" style="background-color: #7461b4; color: #fff">
                        <asp:LinkButton ID="LBtn_HSCForumClassCAll" runat="server" TabIndex="0" Text="所有討論區" CommandName="0" Style="color: #fff" OnClick="LBtn_HSCForumClassCAll_Click">
                        </asp:LinkButton>
                    </li>
                    <asp:SqlDataSource ID="SDS_HSCForumClassCKeyword" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                    <asp:Repeater ID="Rpt_HSCForumClassCKeyword" runat="server" OnItemDataBound="Rpt_HSCForumClassCKeyword_ItemDataBound" DataSourceID="SDS_HSCForumClassCKeyword">
                        <ItemTemplate>
                            <li runat="server" id="Li_HSCForumClassC" class="mr-2 btn btn-outline-purple btn-rounded">
                                <asp:LinkButton ID="LBtn_HSCForumClassC" runat="server" CommandName='<%# Eval("HSCForumClassC") %>' Text='<%# Eval("HSCForumClassC") %>' CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_HSCForumClassC_Click">
                                </asp:LinkButton>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>


                </ul>
            </div>

            <div class="d-none">目前已載入：<span id="currentCount">0</span> 筆</div>

            <!--熱門關鍵字次分類-->
            <div class="col-md-12 sm-area" runat="server" id="div_HSCKeyWord">
                <asp:Label ID="LB_HSCKeyWord" runat="server" Text="0" Visible="false"></asp:Label>
                <ul class="d-flex list-unstyled align-items-center justify-content-start ul_keyword">
                    <li runat="server" id="Li_SCKeyTitle" visible="false">進階搜尋： </li>
                    <li runat="server" id="Li_HSCKeyWordAll" class="mr-2 btn btn-outline-purple btn-rounded" visible="false">
                        <asp:LinkButton ID="LBtn_HSCKeyWordAll" runat="server" TabIndex="0" Text="所有" CommandName="0">
                        </asp:LinkButton>
                    </li>
                    <asp:SqlDataSource ID="SDS_HSCourseKeyWord" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT A.HID, A.HKeyWordID, B.HKeyword, A.HSKeyword, A.HRemark, A.HStatus FROM HSCourseKeyWord A INNER JOIN HCourseKeyWord B ON A.HKeyWordID=B.HID WHERE A.HKeyWordID=@HKeyWordID AND B.HStatus='1'">
                        <SelectParameters>
                            <asp:Parameter Name="HKeyWordID" Type="String" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    <asp:Repeater ID="Rpt_HSCourseKeyWord" runat="server" DataSourceID="SDS_HSCourseKeyWord">
                        <ItemTemplate>
                            <li runat="server" id="Li_HSCKeyWord" class="mr-2 btn btn-outline-purple btn-rounded">
                                <asp:LinkButton ID="LBtn_HSCKeyWord" runat="server" CommandName='<%# Eval("HSKeyword") %>' Text='<%# Eval("HSKeyword") %>'>
                                </asp:LinkButton>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>


        </div>
        <!--發表主題  .comment start-->
        <div class="comment topic mb-3 d-none">
            <div class="d-flex justify-content-start align-items-center">
                <div class="profile-image">
                    <asp:Image ID="IMG_MsgImg" runat="server" CssClass="" ImageUrl="images/icon.png" />
                </div>
                <asp:TextBox ID="TB_PublishTopic" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="想發表甚麼主題"></asp:TextBox>
            </div>
        </div>
        <!--發表主題  /.comment end-->

        <hr />

        <div class="d-flex justify-content-end align-items-center mb-1">
            <div>
                <label>
                    <input type="checkbox" name="" id="CB_Participate" runat="server" class="checkbox">
                    <span class="btn-box">
                        <span class="btncircle"></span>
                    </span>
                    <span class="text">只顯示可參與</span>
                </label>
            </div>
            <div class="ml-2">
                <label>
                    <input type="checkbox" name="" id="CB_Booking" runat="server" class="checkbox">
                    <span class="btn-box">
                        <span class="btncircle"></span>
                    </span>
                    <span class="text">只顯示已報名</span>
                </label>
            </div>
        </div>




        <%--<asp:GridView ID="GridView1" runat="server"></asp:GridView>--%>

        <asp:Panel ID="Panel_NoResult" runat="server" CssClass="text-center mt-3" Visible="false">
            <asp:Label ID="LB_Nodata" runat="server" Text="沒有查到相關資訊~請重新搜尋~謝謝!" CssClass="d-block text-danger mb-0 font-weight-bold"></asp:Label>
            <asp:Image ID="IMG_NoResult" runat="server" ImageUrl="~/images/nodata.png" Style="width: 60%" />

        </asp:Panel>

        <!--主題內容  .topicarea start-->
        <asp:SqlDataSource ID="SDS_HSCTopic" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand=""
            ProviderName="System.Data.SqlClient"></asp:SqlDataSource>
        <asp:Repeater ID="Rpt_HSCTopic" runat="server" OnItemDataBound="Rpt_HSCTopic_ItemDataBound">
            <ItemTemplate>
                <div class="topicarea" id="Div_Topicarea" runat="server" >
                    <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                    <div class="d-flex justify-content-between align-items-start">
                        <div class="d-flex justify-content-start align-items-center">
                            <div class="forum-type">
                                <asp:Label ID="LB_HSCForumClassB" runat="server" Text='<%# Eval("HSCForumClassB") %>'></asp:Label>
                                <!--次類別名稱-->
                            </div>
                            <div class="coursename">
                                <asp:Label ID="LB_HSCForumClassC" runat="server" Text='<%# Eval("HSCForumClassC") %>'></asp:Label>
                                <!--討論區名稱-->
                            </div>
                        </div>

                    </div>


                    <div class="mt-3  d-flex justify-content-between align-items-center">
                        <h5 class="font-weight-bold">
                            <!--主題名稱-->
                            <asp:Literal ID="LTR_HTopicName" runat="server" Text='<%# Eval("HTopicName") %>'></asp:Literal>
                        </h5>

                        <div class="mr-3 morefunction">
                            <asp:LinkButton ID="LBtn_More" runat="server" class="btn_more" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_More_Click"><span class="ti-more-alt"></span></asp:LinkButton>
                            <div id="Div_Editarea" runat="server" class="edit_area" visible="false">
                                <ul>
                                    <li>
                                        <asp:LinkButton ID="LBtn_Edit" runat="server" CommandArgument='<%# Eval("HID") %>' CommandName='<%#  Container.ItemIndex%>' OnClick="LBtn_Edit_Click"><span class="ti-pencil mr-2"></span>編輯主題</asp:LinkButton>
                                    </li>
                                    <li class="border-bottom-0">
                                        <asp:LinkButton ID="LBtn_Del" runat="server" CommandArgument='<%# Eval("HID") %>' CommandName='<%#  Container.ItemIndex%>' OnClick="LBtn_Del_Click"><span class="ti-trash mr-2"></span>刪除主題</asp:LinkButton>
                                    </li>
                                    <li class="d-none">
                                        <asp:LinkButton ID="LBtn_Hide" runat="server" CommandArgument='<%# Eval("HID") %>' CommandName='<%#  Container.ItemIndex%>' OnClick="LBtn_Hide_Click"><span class="fa fa-eye-slash mr-2" style="font-size: 13px;"></span>隱藏主題</asp:LinkButton>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>

                    <div>
                        <div class="author_byline">
                            <div class="author_img">
                                <asp:Label ID="LB_HImg" runat="server" Text='<%# Eval("HImg") %>' Visible="false"></asp:Label>
                                <asp:Image ID="IMG_Creator" runat="server" Style="width: 80%; max-height: 20%" />
                            </div>
                            <div class="author_text">
                                <div>
                                    <asp:Label ID="LB_HMember" runat="server" Text='<%# Eval("UserName") %>'></asp:Label>
                                    <!--中心 台中/2200 王曉明-->
                                </div>
                                <div>
                                    <asp:Label ID="LB_HDate" runat="server" Text='<%# Eval("HCreateDT") %>'></asp:Label>
                                </div>
                            </div>
                        </div>

                    </div>
                    <!--內容 .content start-->
                    <div class="content">


                        <div id="Div_NoPermission" runat="server" visible="false" style="background: #e0e0e0; border-radius: 10px; padding: 10px;">
                            <div class="">   <span class="ti-lock"></span>您尚未報名此課程，所以暫時無法一起參與討論哦~
                                           請點以下連結報名課程：
                                <asp:HyperLink ID="HL_QuickBook" runat="server" Target="_blank" NavigateUrl='<%# ToCourseListUrl(Eval("HTopicName")) %>'>立即報名</asp:HyperLink>
                            </div>
                        </div>


                        <div id="Div_Show" runat="server">
                            <asp:Image ID="IMG_Pic" runat="server" CssClass="picstyle" Style="max-height: 100px;" />
                            <asp:Label ID="LB_HContent" runat="server" Text='<%# Eval("HContent") %>' Visible="true" CssClass="f-18"></asp:Label>

                            <asp:LinkButton ID="LBtn_View" runat="server" OnClientClick="" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Eval("UserName") %>' OnClick="LBtn_View_Click">查看更多</asp:LinkButton>
                        </div>

                        <div class="mt-3">
                            <ul class="d-flex list-unstyled align-items-center justify-content-start comment">
                                <li>
                                    <asp:LinkButton ID="LBtn_MsgDetail" runat="server" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Eval("UserName") %>' OnClick="LBtn_View_Click">
                                        <span class="ti-comments mr-2"></span>
                                        <asp:Label ID="LB_MsgNum" runat="server" Text=""></asp:Label>
                                    </asp:LinkButton>
                                </li>
                                <li class="d-none">
                                    <asp:LinkButton ID="LBtn_MoodModal" runat="server" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_MoodModal_Click">
                                        <asp:Label ID="LB_MoodIcon" runat="server" class="ti-comments-smiley mr-2"></asp:Label>
                                        <asp:Label ID="LB_FeelingNum" runat="server" Text=""></asp:Label>
                                    </asp:LinkButton>
                                </li>
                                <li class="d-flex">
                                    <asp:LinkButton ID="LBtn_ThumbsUp" runat="server" CssClass="d-flex align-items-center" ToolTip="讚" TabIndex="1" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_FeelingType_Click">
                                        <asp:Label ID="LB_ThumbsUp" runat="server" Text="Label"></asp:Label>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="LBtn_ThumbsUpNum" runat="server" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_ThumbsUpNum_Click">
                                        <asp:Label ID="LB_ThumbsUpNum" runat="server" Text="0"></asp:Label>
                                    </asp:LinkButton>
                                </li>
                                <li class="d-flex">
                                    <asp:LinkButton ID="LBtn_Love" runat="server" CssClass="d-flex align-items-center" ToolTip="愛心" TabIndex="2" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_FeelingType_Click">
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="LBtn_LoveNum" runat="server" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_LoveNum_Click">
                                        <asp:Label ID="LB_LoveNum" runat="server" Text="0"></asp:Label>
                                    </asp:LinkButton>


                                </li>
                                <li class="d-flex">
                                    <asp:LinkButton ID="LBtn_Smile" runat="server" CssClass="d-flex align-items-center" ToolTip="微笑" TabIndex="3" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_FeelingType_Click">
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="LBtn_SmileNum" runat="server" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_SmileNum_Click">
                                        <asp:Label ID="LB_SmileNum" runat="server" Text="0"></asp:Label>
                                    </asp:LinkButton>
                                </li>

                                <li>
                                    <asp:LinkButton ID="LBtn_Seen" runat="server" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Eval("HTopicName") %>' OnClick="LBtn_Seen_Click">
                                        <span class="ti-eye mr-2"></span>
                                        <asp:Label ID="LB_SeenNum" runat="server" Text=""></asp:Label>
                                    </asp:LinkButton>
                                </li>
                                <li><span class="ti-share mr-2"></span>
                                    <asp:Label ID="LB_ShareNum" runat="server" Text=""></asp:Label>
                                </li>
                            </ul>
                        </div>
                        <hr class="mt-1 mb-1" />
                        <div class="d-none justify-content-end align-items-center ">
                            <div class="cusfunction">


                                <asp:LinkButton ID="LBtn_Msg" runat="server" CssClass="btn btn-default btn-detail" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_View_Click">
                                             <span class="ti-comment mr-2"></span>留言
                                </asp:LinkButton>

                                <a class="btn btn-default btn-detail btn-copy tooltipped tooltipped-s" title="分享連結" data-clipboard-action="copy" data-clipboard-target="#text_<%# Eval("HID")  %>_<%#Eval("HMemberID")  %>" aria-label="Copied!"><span class="ti-share mr-2"></span>分享連結</a>
                                <div class="text-white" style="position: absolute; z-index: -1">
                                    <div id="text_<%# Eval("HID")  %>_<%#Eval("HMemberID")   %>" style="display: inline;">
                                        <asp:Label ID="LB_HLink" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                            </div>


                        </div>




                    </div>
                    <!--內容 /.content end-->

                </div>
            </ItemTemplate>
        </asp:Repeater>
        <!--主題內容  /.topicarea end-->




        <div id="topicContainer"></div>
        <div id="loadingMsg" style="display: none;">
        </div>
        <div id="loading" style="display: none;">
            <span class="spinner"></span>載入中...
        </div>

        <%--<asp:Button ID="Btn_LoadMore" runat="server" Text="更多" OnClick="Btn_LoadMore_Click" Style="display: none;" />--%>
    </div>




    <!--分享連結觸發事件用--->
    <div class="d-none">
        <asp:Label ID="LB_ShareHID" runat="server" Text=""></asp:Label>
        <asp:Label ID="LB_MemberHID" runat="server" Text=""></asp:Label>
        <asp:Button ID="Btn_Share" runat="server" Text="Button" OnClick="Btn_Share_Click" />
    </div>


    <!--==================發表主題彈跳出的畫面==================--->
    <div id="ContentModal" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 75%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0" id="exampleModalLongTitle">
                        <!--發表主題-->
                        <!---240822_會議修改名稱以便區隔-->
                        新增專欄主題
                    </h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
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
                                                <label class="font-weight-bold">
                                                    主題名稱</label>
                                                <div class="form-group">
                                                    <asp:TextBox ID="TB_HTopicName" runat="server" class="form-control" placeholder="主題名稱" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-md-3 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    所屬討論區主類別</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:SqlDataSource ID="SDS_HSCForumClassA" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCFCMaster, HSCFCName, HSCFCLevel, HStatus FROM HSCForumClass WHERE HStatus=1 AND HSCFCMaster='0' AND HSCFCLevel='10'"></asp:SqlDataSource>
                                                    <asp:DropDownList ID="DDL_HSCForumClassA" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCForumClassA" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true" OnSelectedIndexChanged="DDL_HSCForumClassA_SelectedIndexChanged" AutoPostBack="true" Enabled="false">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-3 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    所屬討論區次類別</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:SqlDataSource ID="SDS_HSCForumClassB" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                    <asp:DropDownList ID="DDL_HSCForumClassB" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCForumClassB" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true" OnSelectedIndexChanged="DDL_HSCForumClassB_SelectedIndexChanged" AutoPostBack="true">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-3 col-sm-12">
                                                <label class="font-weight-bold">
                                                    所屬討論區名稱</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:SqlDataSource ID="SDS_HSCForumClassC" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                    <asp:DropDownList ID="DDL_HSCForumClassC" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCForumClassC" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-3 col-sm-12">
                                                <label class="font-weight-bold">
                                                    是否置頂</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:RadioButtonList ID="RBtn_HPinTop" runat="server" Style="vertical-align: top;" RepeatLayout="Flow" RepeatDirection="Horizontal">

                                                        <asp:ListItem class="item_margin" Value="1">&nbsp;是&nbsp;</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </div>
                                            </div>

                                            <div class="col-md-3 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    專欄分類</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:SqlDataSource ID="SDS_HSCClass" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCClassName FROM HSCClass WHERE HStatus=1"></asp:SqlDataSource>
                                                    <asp:DropDownList ID="DDL_HSCClass" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCClass" DataTextField="HSCClassName" DataValueField="HID" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-3 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    紀錄類型</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:SqlDataSource ID="SDS_HSCRecordType" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCRTName FROM HSCRecordType WHERE HStatus=1"></asp:SqlDataSource>
                                                    <asp:DropDownList ID="DDL_HSCRecordType" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCRecordType" DataTextField="HSCRTName" DataValueField="HID" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-6 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    九宮格類型</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:RadioButtonList ID="RBL_HSCJiugonggeTypeID" runat="server" CssClass="radiolist" Style="vertical-align: top" RepeatLayout="Flow" RepeatDirection="Horizontal">
                                                    </asp:RadioButtonList>
                                                </div>
                                            </div>
                                            <div class="col-md-6 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    成長進度</label>
                                                <div class="form-group">
                                                    <asp:DropDownList ID="DDL_HGProgress" runat="server" CssClass="form-control  js-example-basic-single" Width="100%" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                        <asp:ListItem Value="1">導師-立如松</asp:ListItem>
                                                        <asp:ListItem Value="2">導師-大愛手</asp:ListItem>
                                                        <asp:ListItem Value="3">其他</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-6 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    其他成長進度</label>
                                                <div class="form-group">
                                                    <asp:TextBox ID="TB_HOGProgress" runat="server" class="form-control" placeholder="其他" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>


                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    內容</label>
                                                <div class="form-group">
                                                    <CKEditor:CKEditorControl ID="CKE_Content" runat="server" class="form-control"></CKEditor:CKEditorControl>
                                                </div>
                                            </div>

                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    上傳檔案
                                                 <span class="text-danger font-weight-normal font-13">*格式僅限jpg、png、gif、heic、mp3、pdf<!--docx、doc、xlsx、xls--></span>
                                                </label>
                                                <div class="form-group">
                                                    <asp:FileUpload ID="FU_HFile1" runat="server" class="mb-1" />
                                                    <asp:LinkButton ID="LBtn_HFile1Upload" runat="server" CssClass="btn btn-outline-gray" OnClick="LBtn_HFile1Upload_Click">上傳</asp:LinkButton>
                                                    <asp:Label ID="LB_File1" runat="server" Text=""></asp:Label>
                                                    <br />
                                                    <asp:FileUpload ID="FU_HFile2" runat="server" class="mb-1" />
                                                    <asp:LinkButton ID="LBtn_HFile2Upload" runat="server" CssClass="btn btn-outline-gray" OnClick="LBtn_HFile2Upload_Click">上傳</asp:LinkButton>
                                                    <asp:Label ID="LB_File2" runat="server" Text=""></asp:Label>
                                                    <br />
                                                    <asp:FileUpload ID="FU_HFile3" runat="server" class="mb-1" />
                                                    <asp:LinkButton ID="LBtn_HFile3Upload" runat="server" CssClass="btn btn-outline-gray" OnClick="LBtn_HFile3Upload_Click">上傳</asp:LinkButton>
                                                    <asp:Label ID="LB_File3" runat="server" Text=""></asp:Label>
                                                </div>
                                            </div>

                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    影片嵌入連結
                                                </label>
                                                <div class="form-group">
                                                    <asp:TextBox ID="TB_HVideoLink" runat="server" class="form-control" placeholder=" 影片連結" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    HashTag標籤
                                                </label>
                                                <div class="form-group">
                                                    <asp:SqlDataSource ID="SDS_HSCHotHashTag" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCHHashTag, HStatus FROM HSCHotHashTag WHERE HStatus=1"></asp:SqlDataSource>
                                                    <asp:ListBox ID="LBox_HSCHotHashTag" runat="server" class="form-control select2-multiple" name="state" SelectionMode="Multiple" DataSourceID="SDS_HSCHotHashTag" DataTextField="HSCHHashTag" DataValueField="HID" Width="100%"></asp:ListBox>
                                                    <asp:TextBox ID="TB_HHashTag" runat="server" class="form-control mb-2" placeholder="自己輸入其他沒有在選項中的HashTag標籤(以逗號隔開)" AutoComplete="off"></asp:TextBox>

                                                </div>
                                            </div>



                                            <%--<div class="col-md-6 col-sm-12">
                                             <label class="font-weight-bold">
                                                 開放對象</label>
                                             <div class="form-group vertical-align-top">
                                                 <asp:RadioButtonList ID="RadioButtonList5" runat="server" Style="vertical-align: top;" RepeatLayout="Flow" RepeatDirection="Horizontal">
                                                     <asp:ListItem class="item_margin">&nbsp;全體同修&nbsp;</asp:ListItem>
                                                     <asp:ListItem class="item_margin">&nbsp;僅限班會&nbsp;</asp:ListItem>
                                                 </asp:RadioButtonList>
                                             </div>
                                         </div>--%>
                                        </div>
                                        <div class="row clearfix">
                                        </div>
                                    </div>

                                </div>

                            </div>
                        </div>

                    </div>


                    <div class="modal-footer text-center">
                        <asp:Button ID="Btn_Launch" runat="server" class="button button-green" Text="發表" OnClick="Btn_Launch_Click" />
                        <asp:Button ID="Btn_Close" runat="server" class="button button-gray" Text="關閉" OnClick="Btn_Close_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal  END-->

    <!--==================編輯主題彈跳出的畫面==================--->
    <div id="Div_Edit" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 75%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0" id="editModalLongTitle">編輯主題</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="modal-body" style="width: 100%; padding: 10px">
                    <asp:Label ID="LB_EditHID" runat="server" Text="" Visible="false"></asp:Label>
                    <div class="p-0">
                        <div class="row clearfix pl-2 pr-2">
                            <div class="col-lg-12 col-md-12">

                                <div class="form-group">

                                    <div class="col-md-12 mb-2">
                                        <div class="row clearfix">
                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    主題名稱</label>
                                                <div class="form-group">
                                                    <asp:TextBox ID="TB_HSCTopicNameM" runat="server" class="form-control" placeholder="主題名稱" AutoComplete="off" Text=""></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-md-3 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    所屬討論區主類別</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:DropDownList ID="DDL_HSCForumClassAM" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCForumClassA" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>

                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-3 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    所屬討論區次類別</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:DropDownList ID="DDL_HSCForumClassBM" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCForumClassB" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-3 col-sm-12">
                                                <label class="font-weight-bold">
                                                    所屬討論區名稱</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:DropDownList ID="DDL_HSCForumClassCM" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCForumClassC" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-3 col-sm-12">
                                                <label class="font-weight-bold">
                                                    是否置頂</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:RadioButtonList ID="RBtn_HPinTopM" runat="server" Style="vertical-align: top;" RepeatLayout="Flow" RepeatDirection="Horizontal">

                                                        <asp:ListItem class="item_margin" Value="1">&nbsp;是&nbsp;</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </div>
                                            </div>

                                            <div class="col-md-3 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    專欄分類</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:DropDownList ID="DDL_HSCClassM" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCClass" DataTextField="HSCClassName" DataValueField="HID" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-3 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    紀錄類型</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:DropDownList ID="DDL_HSCRecordTypeM" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCRecordType" DataTextField="HSCRTName" DataValueField="HID" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-6 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    九宮格類型</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:RadioButtonList ID="RBL_HSCJiugonggeTypeM" runat="server" CssClass="radiolist" Style="vertical-align: top" RepeatLayout="Flow" RepeatDirection="Horizontal">
                                                    </asp:RadioButtonList>
                                                </div>
                                            </div>
                                            <div class="col-md-6 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    成長進度</label>
                                                <div class="form-group">
                                                    <asp:DropDownList ID="DDL_HGProgressM" runat="server" CssClass="form-control  js-example-basic-single" Width="100%" AppendDataBoundItems="true">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                        <asp:ListItem Value="1">導師-立如松</asp:ListItem>
                                                        <asp:ListItem Value="2">導師-大愛手</asp:ListItem>
                                                        <asp:ListItem Value="3">其他</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-6 col-sm-12 d-none">
                                                <label class="font-weight-bold">
                                                    其他成長進度</label>
                                                <div class="form-group">
                                                    <asp:TextBox ID="TB_HOGProgressM" runat="server" class="form-control" placeholder="其他" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>


                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    內容</label>
                                                <div class="form-group">
                                                    <CKEditor:CKEditorControl ID="CKE_HContentM" runat="server" class="form-control"></CKEditor:CKEditorControl>
                                                </div>
                                            </div>

                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    上傳檔案
                                              <span class="text-danger font-weight-normal font-13">*格式僅限jpg、png、gif、mp3、pdf、docx、doc、xlsx、xls</span>
                                                </label>
                                                <div class="form-group">
                                                    <div class="form-group">
                                                        <asp:FileUpload ID="FU_HFileM1" runat="server" class="mb-1" />
                                                        <asp:LinkButton ID="LBtn_HFileM1Upload" runat="server" CssClass="btn btn-outline-gray" OnClick="LBtn_HFileM1Upload_Click">上傳</asp:LinkButton>
                                                        <asp:Label ID="LB_FileM1" runat="server" Text="" Visible="false"></asp:Label>
                                                        <asp:Image ID="IMG_File1" runat="server" Style="width: 15%; margin-bottom: 10px;" Visible="false" />
                                                        <audio id="Audio1" runat="server" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false" style="width: 50%">
                                                            <source id="Source1" runat="server" src="" type="audio/mpeg">
                                                            Your browser does not support the audio element.
                                                        </audio>
                                                        <br />
                                                        <asp:FileUpload ID="FU_HFileM2" runat="server" class="mb-1" />
                                                        <asp:LinkButton ID="LBtn_HFileM2Upload" runat="server" CssClass="btn btn-outline-gray" OnClick="LBtn_HFileM2Upload_Click">上傳</asp:LinkButton>
                                                        <asp:Label ID="LB_FileM2" runat="server" Text="" Visible="false"></asp:Label>
                                                        <asp:Image ID="IMG_File2" runat="server" Style="width: 15%; margin-bottom: 10px;" Visible="false" />
                                                        <audio id="Audio2" runat="server" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false" style="width: 50% !important;">
                                                            <source id="Source2" runat="server" src="" type="audio/mpeg">
                                                            Your browser does not support the audio element.
                                                        </audio>
                                                        <br />
                                                        <asp:FileUpload ID="FU_HFileM3" runat="server" class="mb-1" />
                                                        <asp:LinkButton ID="LBtn_HFileM3Upload" runat="server" CssClass="btn btn-outline-gray" OnClick="LBtn_HFileM3Upload_Click">上傳</asp:LinkButton>
                                                        <asp:Label ID="LB_FileM3" runat="server" Text="" Visible="false"></asp:Label>
                                                        <asp:Image ID="IMG_File3" runat="server" Style="width: 15%; margin-bottom: 10px;" Visible="false" />
                                                        <audio id="Audio3" runat="server" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false" style="width: 30% !important;">
                                                            <source id="Source3" runat="server" src="" type="audio/mpeg">
                                                            Your browser does not support the audio element.
                                                        </audio>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    影片嵌入連結
                                                </label>
                                                <div class="form-group">
                                                    <asp:TextBox ID="TB_HVideoLinkM" runat="server" class="form-control" placeholder=" 影片連結" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    HashTag標籤
                                                </label>
                                                <div class="form-group">
                                                    <asp:ListBox ID="LBox_HSCHotHashTagM" runat="server" class="form-control select2-multiple" name="state" SelectionMode="Multiple" DataSourceID="SDS_HSCHotHashTag" DataTextField="HSCHHashTag" DataValueField="HID" Width="100%"></asp:ListBox>

                                                    <asp:TextBox ID="TB_HHashTagM" runat="server" class="form-control mt-2" placeholder="自己輸入其他沒有在選項中的HashTag標籤(以逗號隔開)" AutoComplete="off"></asp:TextBox>

                                                </div>
                                            </div>

                                        </div>
                                        <div class="row clearfix">
                                        </div>
                                    </div>

                                </div>

                            </div>


                        </div>

                    </div>


                    <div class="modal-footer text-center">
                        <asp:Button ID="Btn_UPDSubmit" runat="server" class="button button-green" Text="儲存" OnClick="Btn_UPDSubmit_Click" />
                        <asp:Button ID="Btn_UPDCancel" runat="server" class="button button-gray" Text="取消" OnClick="Btn_UPDCancel_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal  END-->


    <!--==================刪除/隱藏主題==================--->
    <div id="Div_TopicReviseModal" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 50%;">
            <asp:Label ID="LB_ReviseHID" runat="server" Text="" Visible="false"></asp:Label>


            <div class="modal-content" style="width: 100%;" runat="server" id="Div_SCTopicDel" visible="false">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3>填寫刪除主題原因</h3>
                </div>
                <div class="modal-body" style="width: 100%;">

                    <div class="form-group">
                        <label for="">刪除主題原因</label>
                        <asp:TextBox ID="TB_HSCTopicDelReason" runat="server" class="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                </div>
                <div class="modal-footer text-center">
                    <asp:Button ID="Btn_HSCTopicDel" runat="server" class="button button-purple" Text="確認刪除" OnClick="Btn_HSCTopicDel_Click" Btmessage="確定要刪除這則主題嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                    <asp:Button ID="Btn_TopicDelClose" runat="server" class="button button-gray" Text="關閉" />
                </div>
            </div>

            <div class="modal-content" style="width: 100%;" runat="server" id="Div_SCTopicHide" visible="false">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3>填寫隱藏主題原因</h3>
                </div>
                <div class="modal-body" style="width: 100%;">
                    <div class="form-group">
                        <label for="">隱藏主題原因</label>
                        <asp:TextBox ID="TB_HSCTopicHideReason" runat="server" class="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                </div>
                <div class="modal-footer text-center">
                    <asp:Button ID="Btn_HSCTopicHide" runat="server" class="button button-purple" Text="確認隱藏" OnClick="Btn_HSCTopicHide_Click" Btmessage="確定要隱藏這則主題嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                    <asp:Button ID="Btn_TopicHideClose" runat="server" class="button button-gray" Text="關閉" />
                </div>
            </div>
        </div>
    </div>



     <asp:Label ID="LB_SCTopicID" runat="server" Text="Label" Visible="false"></asp:Label>
    <!--==================smiley==================--->
    <div id="Div_Smiley" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-dialog" role="document" style="max-width: 30%;">
                    <div class="modal-content" style="width: 100%;">
                       
                        <div class="modal-header" style="background: #fff; padding-bottom: 0px">
                            <button type="button" class="close" data-dismiss="modal" style="color: #484848;">&times;</button>
                            <ul class="nav nav-tabs" id="ParaTab" role="tablist">

                                <li class="nav-item">
                                    <asp:LinkButton ID="LBtn_ThumbsUpM" runat="server" class="nav-link  active show font-weight-bold" TabIndex="1" OnClick="LBtn_FeelingTypeM_Click">
                                <%--<span class="ti-thumb-up mr-2"></span>
                                <asp:Label ID="LB_ThumbsUpNum" runat="server" Text="Label"></asp:Label>--%>
                                    </asp:LinkButton>
                                </li>
                                <li class="nav-item">
                                    <asp:LinkButton ID="LBtn_HeartM" runat="server" class="nav-link font-weight-bold" TabIndex="2" OnClick="LBtn_FeelingTypeM_Click">
                               <%-- <span class="ti-heart mr-2"></span>
                                <asp:Label ID="LB_LoveNum" runat="server" Text="Label"></asp:Label>--%>
                                    </asp:LinkButton>
                                </li>
                                <li class="nav-item">
                                    <asp:LinkButton ID="LBtn_SmileM" runat="server" class="nav-link font-weight-bold" TabIndex="3" OnClick="LBtn_FeelingTypeM_Click">
                                <%--<span class="ti-face-smile mr-2"></span>
                                <asp:Label ID="LB_SmileNum" runat="server" Text="Label"></asp:Label>--%>
                                    </asp:LinkButton>
                                </li>
                            </ul>


                        </div>
                        <div class="modal-body" style="width: 100%; padding: 8px 20px">

                            <div class="tab-content">
                                <div class="tab-pane active">
                                    <div class="row">
                                        <ul class="memberlist">

                                            <asp:SqlDataSource ID="SDS_HSCTopicMood" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                            <asp:Repeater runat="server" ID="Rpt_HSCTopicMood" DataSourceID="SDS_HSCTopicMood" OnItemDataBound="Rpt_HSCTopicMood_ItemDataBound">
                                                <ItemTemplate>
                                                    <li>
                                                        <span class="d-inline-block" style="width: 6%; margin-right: 5px">
                                                            <asp:Image ID="Img_HImg" runat="server" Style="width: 100%;" />
                                                        </span>
                                                        <asp:Label ID="LB_HUsername" runat="server" Text='<%# Eval("HUserName") %>'></asp:Label>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>

                                        </ul>
                                    </div>

                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="LBtn_ThumbsUpM" EventName="click" />
                <asp:AsyncPostBackTrigger ControlID="LBtn_HeartM" EventName="click" />
                <asp:AsyncPostBackTrigger ControlID="LBtn_SmileM" EventName="click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>



    <!--==================瀏覽紀錄==================--->
    <div id="Div_SeenRecord" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog mw-70" role="document">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header" style="background: #fff; border-bottom: 1px solid #484848">
                    <asp:Label ID="LB_Head" runat="server" Text="Label" Style="font-size: 1.1rem; font-weight: bold; color: #000000"></asp:Label>
                    <button type="button" class="close" data-dismiss="modal" style="color: #484848;">&times;</button>
                </div>
                <div class="modal-body" style="width: 100%; padding: 8px 20px">



                    <div class="table-responsive">
                        <table id="example" class="table table-striped table-bordered" style="width: 100%">
                            <thead>
                                <tr>
                                    <th style="width: 25%">同修姓名</th>
                                    <th class="text-center" style="width: 25%">首次讀訊</th>
                                    <th class="text-center" style="width: 25%">最近讀訊</th>
                                    <th class="text-center" style="width: 25%">讀訊次數</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="Rpt_HSCTopic_View" runat="server" OnItemDataBound="Rpt_HSCTopic_View_ItemDataBound">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <asp:Label ID="LB_UserName" runat="server" Text='<%# Eval("HUserName") %>'></asp:Label>
                                            </td>
                                            <td class="text-center">
                                                <asp:Label ID="LB_FirstTime" runat="server" Text='<%# Eval("FirstTime") %>'></asp:Label>
                                            </td>
                                            <td class="text-center">
                                                <asp:Label ID="LB_LatestTime" runat="server" Text='<%# Eval("LatestTime") %>'></asp:Label>
                                            </td>
                                            <td class="text-center">
                                                <asp:Label ID="LB_HTimes" runat="server" Text='<%# Eval("HTimes") %>'></asp:Label>
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
    </div>

      <div class="d-none">
        <asp:Button ID="Btn_ShowContent" runat="server" Text="顯示可參與或已報名" OnClick="Btn_ShowContent_Click" />
    </div>

    <script src="js/jquery-3.4.1.min.js"></script>
    <!-- 摺疊圖標 -->
    <script>
        $(function () {
            //多選
            $('.select2-multiple').select2({
                placeholder: "選擇常用的HashTag標籤"
            });

            //新增主題內容
            var textBox = document.getElementById('<%= TB_PublishTopic.ClientID %>');
            textBox.addEventListener('click', function () {
               $('#ContentModal').modal('show');
            });


            // 讓兩個 CheckBox 共用同一個 handler（處理器）
            function onToggleView() {
                $('#<%=Btn_ShowContent.ClientID %>').click(); // 觸發後端事件
            }

            // 綁定事件：可參與 + 已報名
            $('#<%= CB_Participate.ClientID %>, #<%= CB_Booking.ClientID %>').on('change', onToggleView);

        });




    </script>
      <!--//AA20240129_複製功能JS套件-->
   <!--複製套件-->
   <script src="js/clipboard.js"></script>
   <script>
       var clipboard = new ClipboardJS('.btn-copy');
       clipboard.on('success', function (e) {
           console.log(e.text.split('=')[1]);
           document.getElementById("<%= LB_ShareHID.ClientID %>").value = e.text.split('=')[1];
           document.cookie = "TopicID=" + e.text.split('=')[1];
           document.getElementById("<%= Btn_Share.ClientID %>").click();
           alert("已成功複製連結~將連結分享給大家吧~!");


       });
   </script>



    <script>
        $(function () {
            //紀錄目前筆數
            document.getElementById("currentCount").innerText = $("#HF_CurrentIndex").val();
        });

        //目前頁面
        var skip = parseInt($("#HF_CurrentIndex").val(), 10) || 0;
        const take = 10;
        let isLoading = false;
        let allLoaded = false;

        async function loadMore() {
            if (isLoading || allLoaded) return;
            isLoading = true;
            document.getElementById("loadingMsg").style.display = "block";

            try {
                const res = await fetch(`ScrollPageLoadData.ashx?skip=${skip}&take=${take}`);
                if (!res.ok) throw new Error("資料載入失敗");
                const data = await res.json();

                if (data.length === 0) {
                    allLoaded = true;
                    document.getElementById("loadingMsg").innerText = "已載入全部資料";
                    return;
                }

                const container = document.getElementById("topicContainer");

                //顯示資料的html結構
                data.forEach(item => {
                    const div = document.createElement("div");
                    div.className = "topicarea";
                    div.innerHTML = `
  <div class="d-flex justify-content-between align-items-start">
    <div class="d-flex justify-content-start align-items-center">
      <div class="forum-type">
        <span class="span_HSCForumClassB">${item.HSCForumClassB || '大愛光老師專欄'}</span> 
      </div>
      <div class="coursename">
        <span class="span_HSCForumClassC">${item.HSCForumClassC || 'EIP'}</span> 
      </div>
    </div>
  </div>

  <div class="mt-3 d-flex justify-content-between align-items-center">
    <h5 class="font-weight-bold span_HTopicName">${item.HTopicName}</h5>
    <div class="mr-3 morefunction">
      <a  class="btn_more"><span class="ti-more-alt"></span></a>
   
    </div>
  </div>

  <div class="author_byline">
    <div class="author_img">
      <img class="img_creator" style="width: 80%; max-height: 20%" />
    </div>
    <div class="author_text">
      <div><span class="span_UserName">${item.UserName || ''}</span></div>
      <div><span class="span_HCreateDT">${item.HCreateDT || ''}</span></div>
    </div>
  </div>

  <div class="content">
    <div class="div_no_permission" style="display: none; background: #e0e0e0; border-radius: 10px; padding: 10px;">
      <span class="ti-lock"></span>您尚未報名此課程，暫時無法參與討論。<a href="HCourseList.aspx" target="_blank">立即報名</a>
    </div>

    <div class="div_show">
      <img class="img_pic" style="max-height: 100px;" />
      <div class="f-18 span_HContent">${item.HContent || ''}</div>
      <a class="a_view_more" href="HSCTopicDetail.aspx?TID=${item.HID}&F=1">查看更多</a>
      <small class="text-muted span_HCreateDT"></small>
    </div>
  </div>
`;


                    container.appendChild(div);
                });

                //數字加上去，表示已顯示過的，更新 hidden field
                skip += take;

                $("#HF_CurrentIndex").val(skip);  // 更新 hidden field

                // 顯示目前筆數
                document.getElementById("currentCount").innerText = skip;


            } catch (err) {
                console.error("載入失敗：", err);
                document.getElementById("loadingMsg").innerText = "載入失敗，請稍後再試";
            } finally {
                isLoading = false;
                document.getElementById("loadingMsg").style.display = "none";
            }
        }

        // 當滾動到底時自動觸發 loadMore
        window.addEventListener("scroll", () => {
            const nearBottom = window.innerHeight + window.scrollY >= document.body.offsetHeight - 200;
            if (nearBottom) {
                loadMore();
            }
        });

    </script>
</asp:Content>

