<%@ Page Title="" Language="C#" MasterPageFile="~/HochiSCMaster.master" AutoEventWireup="true" CodeFile="HSCForumDetail.aspx.cs" Inherits="HSCForumDetail" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">




    <asp:Label ID="LB_HSCForumClassCID" runat="server" Text="" Visible="false"></asp:Label>


    <div class="middlepart">

        <p class="mb-2">
            <a href="HSCIndex.aspx">回專欄首頁</a> >      
            <asp:HyperLink ID="HL_HSCForumClassA" runat="server"></asp:HyperLink>
            <%--<asp:Label ID="LB_HSCForumClassA" runat="server" Text=""></asp:Label>--%>
            >
            <asp:HyperLink ID="HL_HSCForumClassB" runat="server"></asp:HyperLink>
            <%--<asp:Label ID="LB_HSCForumClassB" runat="server"></asp:Label>--%>
        </p>
        <div class="d-flex justify-content-between align-items-center mobile-wrap">
            <h4 class="detailtitle font-weight-bold">
                <asp:Literal ID="LTR_HSCForumClassC" runat="server"></asp:Literal>
            </h4>
            <div class="mb-3">
                <p class="mb-0 d-inline">
                    <asp:Label ID="LB_HPublic" runat="server" Text=""></asp:Label>討論區．
                  <asp:Label ID="LB_HNumbers" runat="server" Text=""></asp:Label><!--尚未完成-->
                    位成員
                </p>


                <asp:Label ID="LB_HSCFavorite" runat="server" Text="" Visible="false"></asp:Label>
                <asp:LinkButton ID="LBtn_HSCFClassFavorite" runat="server" CssClass="btn btn-danger" Style="background-color: #e24f4f; border-color: #e24f4f; border-radius: 10px; padding-left: 10px;" ToolTip="加入我的最愛" OnClick="LBtn_HSCFClassFavorite_Click">
                    <span class="ti-heart" style="margin-right:5px;"></span>
                    加入我的最愛
                </asp:LinkButton>

                <asp:LinkButton ID="LBtn_CancelFavorite" runat="server" CssClass="btn btn-danger" Style="background-color: #ccc; border-color: #ccc; border-radius: 10px; padding-left: 10px;" ToolTip="取消我的最愛" OnClick="LBtn_CancelFavorite_Click"><span class="ti-heart" style="margin-right:5px;"></span>取消我的最愛</asp:LinkButton>
                <asp:LinkButton ID="LBtn_HManager" runat="server" CssClass="btn btn-outline-purple" OnClick="LBtn_HManager_Click">管理員功能</asp:LinkButton>
            </div>
        </div>
        <div style="border-radius: 15px; background-color: rgba(255,255,255,0.8); padding: 10px">
            <asp:Label ID="LB_HSCMRule" runat="server" Text="" CssClass="rulestyle"></asp:Label>
        </div>
        <hr />
        <div class="comment topic mb-3">
            <%-- <label class="d-block font-weight-bold">發表主題</label>--%>
            <div class="d-flex justify-content-start align-items-center">
                <div class="profile-image">
                    <asp:Image ID="IMG_MsgImg" runat="server" CssClass="" ImageUrl="images/icon.png" />
                </div>
                <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="想發表甚麼主題"></asp:TextBox>
                <!--<textarea class="form-control ckeditor" placeholder="想發表甚麼主題"></textarea>-->
            </div>
            <%--<div class="mt-2 text-right">
                       <asp:LinkButton ID="LinkButton13" runat="server" CssClass="btn btn-purple"><span class="fa fa-paper-plane mr-2"></span>送出留言</asp:LinkButton>
                     </div>--%>
        </div>

        <hr />


        <!--主題內容  .topicarea start-->
        <asp:SqlDataSource ID="SDS_HSCTopic" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand=""
            ProviderName="System.Data.SqlClient"></asp:SqlDataSource>
        <asp:Repeater ID="Rpt_HSCTopic" runat="server"  OnItemDataBound="Rpt_HSCTopic_ItemDataBound">
            <ItemTemplate><%--DataSourceID="SDS_HSCTopic"--%>
                <div class="topicarea">
                    <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                    <%--<asp:Label ID="LB_HCTemplateID" runat="server" Text='<%# Eval("HCTemplateID") %>' Visible="false"></asp:Label>--%>
                    <div class="d-flex justify-content-between align-items-start">
                        <div class="d-flex justify-content-start align-items-center">
                            <div class="forum-type">
                                <asp:Label ID="LB_HSCForumClassB" runat="server" Text='<%# Eval("HSCForumClassB") %>'></asp:Label><!--次類別名稱-->
                            </div>
                            <div class="coursename">
                                <asp:Label ID="LB_HSCForumClassC" runat="server" Text='<%# Eval("HSCForumClassC") %>'></asp:Label><!--討論區名稱-->
                            </div>
                        </div>

                        <%--<div class="taglabel">
                            熱門
                        </div>--%>
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
                                        <asp:LinkButton ID="LBtn_Del" runat="server" CommandArgument='<%# Eval("HID") %>' CommandName='<%#  Container.ItemIndex%>' OnClick="LBtn_Del_Click"><span class="ti-trash mr-2"></span>刪除主題</asp:LinkButton></li>
                                    <li class="d-none">
                                        <asp:LinkButton ID="LBtn_Hide" runat="server" CommandArgument='<%# Eval("HID") %>' CommandName='<%#  Container.ItemIndex%>' OnClick="LBtn_Hide_Click"><span class="fa fa-eye-slash mr-2" style="font-size: 13px;"></span>隱藏主題</asp:LinkButton></li>
                                </ul>
                            </div>
                        </div>
                    </div>

                    <div>
                        <%-- class="d-flex justify-content-between align-items-start"--%>
                        <div class="author_byline">
                            <div class="author_img">
                                <img src="images/icons/profile_small.jpg" style="width: 80%; max-height: 20%" />
                            </div>
                            <div class="author_text">
                                <div>
                                    <asp:Label ID="LB_HMemberID" runat="server" Text='<%# Eval("HMemberID") %>' Visible="false"></asp:Label>
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
                            <div class="">
                                <span class="ti-lock"></span>您尚未報名此課程，所以暫時無法一起參與討論哦~
請點以下連結報名課程：<a href="HCourseList.aspx" target="_blank">立即報名</a>
                            </div>
                        </div>


                        <div id="Div_Show" runat="server">
                            <%--<img class="picstyle" src="images/about-img-01.jpg" style="max-height: 100px;" />--%>
                            <asp:Image ID="IMG_Pic" runat="server" CssClass="picstyle" Style="max-height: 100px;" />
                            <asp:Label ID="LB_HContent" runat="server" Text='<%# Eval("HContent") %>' Visible="true" CssClass="f-18"></asp:Label>

                            <asp:LinkButton ID="LBtn_View" runat="server" CommandArgument='<%# Eval("HID") %>'  CommandName='<%# Eval("UserName") %>' OnClick="LBtn_View_Click">查看更多</asp:LinkButton>

                            <div class="mt-3">
                                <ul class="d-flex list-unstyled align-items-center justify-content-start comment">
                                    <li>
                                          <span class="ti-comments mr-1"></span>
                                        <asp:LinkButton ID="LBtn_MsgDetail" runat="server" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_View_Click">
                                            <asp:Label ID="LB_MsgNum" runat="server" Text=""></asp:Label>
                                        </asp:LinkButton>
                                    </li>
                                    <li class="d-none">
                                        <%-- <a href="javascript:void(0)" data-toggle="modal" data-target="#smiley">--%>
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
                                        <%--<asp:Label ID="LB_Loveicon" runat="server" class="fa fa-heart text-danger mr-2"></asp:Label>--%>
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
                                         <span class="ti-eye mr-1"></span>
                                        <asp:LinkButton ID="LBtn_Seen" runat="server" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Eval("HTopicName") %>' OnClick="LBtn_Seen_Click" CausesValidation="false">
                                            <asp:Label ID="LB_SeenNum" runat="server" Text=""></asp:Label>
                                        </asp:LinkButton>
                                    </li>
                                    <li><span class="ti-share mr-1"></span>
                                        <asp:Label ID="LB_ShareNum" runat="server" Text=""></asp:Label>
                                    </li>
                                </ul>
                            </div>
                            <hr class="mt-1 mb-1 d-none" />
                            <div class="d-none justify-content-end align-items-center ">
                                <div class="cusfunction">
                                
                                    <asp:LinkButton ID="LBtn_Msg" runat="server" CssClass="btn btn-default btn-detail" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_View_Click">
             <span class="ti-comment mr-2"></span>留言
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="LBtn_Share" runat="server" CssClass="btn btn-default btn-detail"><span class="ti-share mr-2"></span>分享</asp:LinkButton>
                                </div>


                            </div>

                        </div>
                    </div>
                    <!--內容 /.content end-->

                </div>
            </ItemTemplate>
        </asp:Repeater>
        <!--主題內容  /.topicarea end-->




    </div>


    <asp:SqlDataSource ID="SDS_HSCHotHashTag" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand="SELECT HID, HSCHHashTag FROM HSCHotHashTag WHERE HStatus=1" ProviderName="System.Data.SqlClient"></asp:SqlDataSource>



    <div class="rightpart">
        <div class="sidearea">
            <h1 class="headtitle hottag mt-0"><span class="ti-heart mr-1"></span>熱門標籤
            </h1>
            <div class="newlist">
                <ul class="d-flex list-unstyled align-items-center justify-content-start hotkeyword">

                    <asp:SqlDataSource ID="SDS_HSCHotHashTagTOP" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand="SELECT TOP(20) Count(*) AS Num, value AS HashTagName FROM HSCTopic CROSS APPLY Split(HHashTag, ',') WHERE value<>'' AND HStatus=1 GROUP BY value ORDER BY Num DESC" ProviderName="System.Data.SqlClient"></asp:SqlDataSource>
                    <asp:Repeater ID="Rpt_HSCHotHashTag" runat="server" DataSourceID="SDS_HSCHotHashTagTOP">
                        <ItemTemplate>

                            <li runat="server" id="Li_HSCHotHashTag" class="mr-2 btn btn-outline-purple btn-rounded">
                                <asp:LinkButton ID="LBtn_HSCHotHashTag" runat="server" Text='<%# Eval("HashTagName") %>' OnClick="LBtn_HSCHotHashTag_Click" CommandArgument='<%# Eval("HashTagName") %>'></asp:LinkButton>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </div>
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
                        新增討論區主題</h5>
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
                                                    <asp:DropDownList ID="DDL_HSCForumClassA" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCForumClassA" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true" OnSelectedIndexChanged="DDL_HSCForumClassA_SelectedIndexChanged" AutoPostBack="true">
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
                                                    <asp:DropDownList ID="DDL_HSCForumClassC" runat="server" CssClass="form-control js-example-basic-single" Style="width: 100%" DataSourceID="SDS_HSCForumClassC" DataTextField="HSCFCName" DataValueField="HID" AppendDataBoundItems="true" Enabled="false">
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
                                                    <asp:SqlDataSource ID="SDS_HashTag" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT HID, HSCHHashTag, HStatus FROM HSCHotHashTag WHERE HStatus=1"></asp:SqlDataSource>
                                                    <asp:ListBox ID="LBox_HSCHotHashTag" runat="server" class="form-control select2-multiple" name="state" SelectionMode="Multiple" DataSourceID="SDS_HashTag" DataTextField="HSCHHashTag" DataValueField="HID" Width="100%"></asp:ListBox>
                                                    <asp:TextBox ID="TB_HHashTag" runat="server" class="form-control mt-2" placeholder="自己輸入其他沒有在選項中的HashTag標籤(以逗號隔開)" AutoComplete="off"></asp:TextBox>
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
                                            <span class="text-danger font-weight-normal font-13">*格式僅限jpg、png、gif、mp3</span>
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

    <!--==================編輯回應==================--->
    <div id="Div_EditReply" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 50%;">
            <div class="modal-content" style="width: 100%;">

                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3>編輯回應</h3>
                </div>
                <div class="modal-body" style="width: 100%;">

                    <div class="form-group">
                        <label for="">回應內容</label>
                        <asp:TextBox ID="TextBox11" runat="server" class="form-control" Text="映青要載我們到基隆為婆婆做大愛手，今天又正好是泰潁生日，所以就到新竹北來為他慶生，唱完生日快樂歌，泰潁說：生日就是要為媽媽做本靈光大愛手，當然我這個阿嬤也有同等待遇，接受他生日的回饋。" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="">編輯原因</label>
                        <asp:TextBox ID="TB_Reason" runat="server" class="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                </div>
                <div class="modal-footer text-center">
                    <asp:Button ID="Button8" runat="server" class="button button-purple" Text="儲存" />
                    <asp:Button ID="Button7" runat="server" class="button button-gray" Text="關閉" />
                </div>
            </div>
        </div>
    </div>


    <!--==================刪除回應==================--->
    <div id="Div_DeleteReply" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 50%;">
            <div class="modal-content" style="width: 100%;">

                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3>填寫刪除回應原因</h3>
                </div>
                <div class="modal-body" style="width: 100%;">
                    <div class="form-group">
                        <label for="">刪除原因</label>
                        <asp:TextBox ID="TextBox13" runat="server" class="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                </div>
                <div class="modal-footer text-center">
                    <asp:Button ID="Button9" runat="server" class="button button-purple" Text="確認刪除" />
                    <asp:Button ID="Button10" runat="server" class="button button-gray" Text="關閉" />
                </div>
            </div>
        </div>
    </div>


    <!--==================隱藏回應==================--->
    <div id="Div_HideReply" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 50%;">
            <div class="modal-content" style="width: 100%;">

                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3>填寫隱藏回應原因</h3>
                </div>
                <div class="modal-body" style="width: 100%;">
                    <div class="form-group">
                        <label for="">隱藏原因</label>
                        <asp:TextBox ID="TextBox12" runat="server" class="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                </div>
                <div class="modal-footer text-center">
                    <asp:Button ID="Button11" runat="server" class="button button-purple" Text="確認隱藏" />
                    <asp:Button ID="Button12" runat="server" class="button button-gray" Text="關閉" />
                </div>
            </div>
        </div>
    </div>



    <!--留言Modal-->
    <div id="discuss" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 75%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0" id="exampleModalLongTitle">留言</h5>
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
                                                    留言內容</label>
                                                <div class="form-group">
                                                    <textarea id="TextArea1" cols="20" rows="2"></textarea>
                                                </div>
                                            </div>

                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    上傳檔案
                                                    <span class="text-danger font-weight-normal font-13">*格式僅限jpg、png、gif、mp3、pdf、docx、doc、xlsx、xls)</span>
                                                </label>
                                                <div class="form-group">
                                                    <asp:FileUpload ID="FileUpload6" runat="server" class="mb-1" /><asp:LinkButton ID="LinkButton38" runat="server" CssClass="btn btn-outline-gray">上傳</asp:LinkButton>
                                                </div>
                                            </div>

                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    影片嵌入連結
                                                </label>
                                                <div class="form-group">
                                                    <asp:TextBox ID="TextBox8" runat="server" class="form-control" placeholder=" 影片連結" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>



                                            <!--  <div class="col-md-6 col-sm-12">
                                                <label class="font-weight-bold">
                                                    開放對象</label>
                                                <div class="form-group vertical-align-top">
                                                    <asp:RadioButtonList ID="RadioButtonList10" runat="server" Style="vertical-align: top;" RepeatLayout="Flow" RepeatDirection="Horizontal">
                                                        <asp:ListItem class="item_margin">&nbsp;全體同修&nbsp;</asp:ListItem>
                                                        <asp:ListItem class="item_margin">&nbsp;僅限班會&nbsp;</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </div>
                                            </div>-->
                                        </div>
                                        <div class="row clearfix">
                                        </div>
                                    </div>

                                </div>

                            </div>


                        </div>

                    </div>


                    <div class="modal-footer text-center">
                        <asp:Button ID="Button4" runat="server" class="button button-purple" Text="送出留言" />
                        <asp:Button ID="Button5" runat="server" class="button button-gray" Text="關閉" />
                    </div>
                </div>
            </div>
        </div>
    </div>



    <!--==================smiley==================--->
    <div id="Div_Smiley" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-dialog" role="document" style="max-width: 30%;">
                    <div class="modal-content" style="width: 100%;">
                        <asp:Label ID="LB_SCTopicID" runat="server" Text="Label" Visible="false"></asp:Label>
                        <div class="modal-header" style="background: #fff; padding-bottom: 0px">
                            <button type="button" class="close" data-dismiss="modal" style="color: #484848;">&times;</button>
                            <ul class="nav nav-tabs" id="ParaTab" role="tablist">

                                <li class="nav-item">
                                    <asp:LinkButton ID="LBtn_ThumbsUpM" runat="server" class="nav-link  active show font-weight-bold" TabIndex="1" OnClick="LBtn_FeelingTypeM_Click">
                                    </asp:LinkButton>
                                </li>
                                <li class="nav-item">
                                    <asp:LinkButton ID="LBtn_HeartM" runat="server" class="nav-link font-weight-bold" TabIndex="2" OnClick="LBtn_FeelingTypeM_Click">
                                    </asp:LinkButton>
                                </li>
                                <li class="nav-item">
                                    <asp:LinkButton ID="LBtn_SmileM" runat="server" class="nav-link font-weight-bold" TabIndex="3" OnClick="LBtn_FeelingTypeM_Click">
                                    </asp:LinkButton>
                                </li>
                            </ul>

                            <%--<ul class="d-flex list-unstyled align-items-center justify-content-start comment">
                    <li><span class="ti-thumb-up mr-2"></span>6</li>
                    <li><span class="ti-heart mr-2"></span>1</li>
                    <li><span class="ti-face-smile mr-2"></span>1</li>
                </ul>--%>
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
                                                    <li>
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
    <asp:UpdatePanel ID="UPD_Seen" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
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
        </ContentTemplate>
</asp:UpdatePanel>


    <script src="js/jquery-3.4.1.min.js"></script>
    <!-- 摺疊圖標 -->
    <script>
        $(function () {

            //多選
            $('.select2-multiple').select2({
                placeholder: "選擇常用的HashTag標籤"
            });



            $(".panel-heading").click(function (e) {
                $(this).find("span.fa-chevron-down").toggleClass("fa-chevron-up");
                $(this).find("span.fa-chevron-up").toggleClass("fa-chevron-down");
            });

            $(".solutionList li").click(function () {
                $(".solutionList .linkactive").removeClass('linkactive');
                $(this).addClass('linkactive');
            });


            var textBox = document.getElementById('<%= TextBox1.ClientID %>');

            textBox.addEventListener('click', function () {

                $('#ContentModal').modal('show');
            });

        });


    </script>


</asp:Content>

