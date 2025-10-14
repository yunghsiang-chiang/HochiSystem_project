<%@ Page Title="" Language="C#" MasterPageFile="~/HochiSCMaster.master" AutoEventWireup="true" CodeFile="HSCGRecordDetail.aspx.cs" Inherits="HSCGRecordDetail" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .cusfunction .moodarea {
            top: -50px;
            width: 69%;
        }

        .replyfunction .msgmoodarea {
            width: 17% !important;
            right: 0 !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Label ID="LB_TID" runat="server" Text="" Visible="false"></asp:Label><!--主題HID-->
    <asp:Label ID="LB_HSCForumClassCID" runat="server" Text="" Visible="false"></asp:Label>

    <div class="middlepart">

        <div class="detailpart" style="padding-top: 0;">
            <!--id="twopart"-->
            <div>

                <div class="d-flex justify-content-between align-items-start">
                    <div class="d-flex justify-content-start align-items-center">
                        <div class="forum-type">
                            <asp:Label ID="LB_HSCSubClass" runat="server" Text="成長紀錄"></asp:Label>
                        </div>

                    </div>
                    <%--<div class="taglabel">
                        熱門
                    </div>--%>
                </div>


                <!--class="leftpart"-->
                <h1 class="headtitle mb-0">
                    <!--主題名稱-->
                    <asp:Literal ID="LTR_HTopicName" runat="server" Text=""></asp:Literal>
                </h1>




                <div class="author_byline">
                    <div class="author_img">
                        <asp:Label ID="LB_HImg" runat="server" Text="" Visible="false"></asp:Label>
                        <asp:Image ID="IMG_Creator" runat="server" Style="width: 80%; max-height: 20%" />
                    </div>
                    <div class="author_text">
                        <div>
                            <asp:Label ID="LB_HMember" runat="server" Text=""></asp:Label>
                        </div>
                        <div>
                            <asp:Label ID="LB_HDate" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                </div>


                <div class="d-flex justify-content-between align-items-center">
                    <div class="tag d-flex justify-content-start align-items-center">
                        <%--  <label class="d-block phead mb-0 mr-3"><span class="fa fa-tag mr-1"></span></label>--%>
                        <ul class="d-flex list-unstyled align-items-center justify-content-start detailkeyword">
                            <asp:Label ID="LB_HHashTag" runat="server" Text="" Visible="false"></asp:Label>
                            <asp:SqlDataSource ID="SDS_HHashTag" runat="server"></asp:SqlDataSource>

                            <asp:Repeater ID="Rpt_HHashTag" runat="server" DataSourceID="SDS_HHashTag">
                                <ItemTemplate>
                                    <li runat="server" id="Li_HHashTag" class="mr-2 btn btn-outline-purple btn-rounded">
                                        <asp:LinkButton ID="LBtn_HHashTag" runat="server" Text='<%# Eval("HHashTag") %>'></asp:LinkButton>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>

                        </ul>
                    </div>

                </div>
                <div class="text-right mb-2">
                    <ul class="d-flex list-unstyled align-items-center justify-content-end comment">
                        <li><span class="ti-comments mr-2"></span>
                            <asp:Label ID="LB_MsgSum" runat="server" Text="0"></asp:Label>
                        </li>
                        <li><span class="ti-face-smile mr-2"></span>
                            <asp:Label ID="LB_TopicSmileSum" runat="server" Text="0"></asp:Label>
                        </li>
                        <li><span class="ti-thumb-up mr-2"></span>
                            <asp:Label ID="LB_TopicThumbUpSum" runat="server" Text="0"></asp:Label>
                        </li>
                        <li ><span class="ti-heart mr-2"></span>
                            <asp:Label ID="LB_TopicHeartSum" runat="server" Text="0"></asp:Label>
                        </li>
                        <li><span class="ti-eye mr-2"></span>
                            <asp:Label ID="LB_TopicViewSum" runat="server" Text="0"></asp:Label>
                        </li>
                        <li><span class="ti-share mr-2"></span>
                            <asp:Label ID="LB_TopicShareSum" runat="server" Text="0"></asp:Label>
                        </li>
                    </ul>
                </div>
                <%--<div class="mb-3" style="border-bottom: 2px solid #d3d3d3;"></div>--%>


                <div class="content">


                    <div class="d-flex justify-content-end align-items-center">
                        <div class="cusfunction">
                            <asp:LinkButton ID="LBtn_HSCTopicMood" runat="server" CssClass="btn btn-default btn-detail btn-mood" OnClick="LBtn_HSCTopicMood_Click"><span class="ti-comments-smiley mr-2"></span>心情</asp:LinkButton>
                            <div id="Div_TFeelingArea" runat="server" class="moodarea justify-content-around align-items-center " visible="false">
                                <asp:LinkButton ID="LBtn_HTopicType1" runat="server" CssClass="btn btn-default btn-function" ToolTip="讚" TabIndex="1" OnClick="LBtn_HTopicType_Click"><span class="ti-thumb-up"></span></asp:LinkButton>
                                <asp:LinkButton ID="LBtn_HTopicType2" runat="server" CssClass="btn btn-default btn-function" ToolTip="愛心" TabIndex="2" OnClick="LBtn_HTopicType_Click"><span class="fa fa-heart text-danger"></span></asp:LinkButton>
                                <asp:LinkButton ID="LBtn_HTopicType3" runat="server" CssClass="btn btn-default btn-function" ToolTip="微笑" TabIndex="3" OnClick="LBtn_HTopicType_Click"><span class="ti-face-smile"></span></asp:LinkButton>
                            </div>
                            <asp:LinkButton ID="LBtn_Share" runat="server" CssClass="btn btn-default btn-detail" OnClientClick="Copy()"><span class="ti-share mr-2"></span>分享連結</asp:LinkButton>
                        </div>


                    </div>


                </div>
                <!--內容 /.content end-->

                <%--  <div class="mb-3" style="border-bottom: 2px solid #d3d3d3;"></div>--%>

                <!--留言  .comment start-->
                <!--成長紀錄的留言來自自己建立的成長紀錄，故此處功能隱藏-->
                <div class="comment mb-3 d-none" runat="server" id="Div_HSCGMsg" visible="false">
                    <label class="d-block font-weight-bold">建立留言</label>
                    <div>
                        <CKEditor:CKEditorControl ID="CKE_HContent" runat="server" class="form-control" Styles="height:100px"></CKEditor:CKEditorControl>
                    </div>
                    <div class="mt-2">
                        <asp:TextBox ID="TB_HVideoLink" runat="server" CssClass="form-control" placeholder="請輸入嵌入影片連結"></asp:TextBox>
                    </div>
                    <div class="mt-2">
                        <asp:FileUpload ID="FU_HFile1" runat="server" />
                        <asp:LinkButton ID="LBtn_HFile1" runat="server" CssClass="btn btn-outline-dark" OnClick="LBtn_HFile1_Click">上傳</asp:LinkButton>
                        <asp:Label ID="LB_HFileMsg1" runat="server" Text="" CssClass="text-danger"></asp:Label><!--提示訊息-->
                        <asp:LinkButton ID="LBtn_HFile1_Del" runat="server" ToolTip="移除已經上傳的檔案" Visible="false" OnClick="LBtn_HFile1_Del_Click"><i class="fa fa-times-circle" style="color:red" >刪除</i></asp:LinkButton>
                        <asp:Label ID="LB_HFile1" runat="server" Text="" Visible="false" CssClass="text-muted"></asp:Label><!--上傳檔案名稱-->
                    </div>
                    <div class="mt-2 text-right">
                        <asp:LinkButton ID="LBtn_HSubmitMsg" runat="server" CssClass="btn btn-purple" OnClick="LBtn_HSubmitMsg_Click"><span class="fa fa-paper-plane mr-2"></span>送出留言</asp:LinkButton>
                    </div>


                </div>
                <!--留言  /.comment end-->

                <div class="mb-3" style="border-bottom: 2px solid #d3d3d3;"></div>

                <!--留言紀錄顯示範圍 start-->
                <div class="comment_area" runat="server" id="Div_SCGMsg">
                    <asp:SqlDataSource ID="SDS_HSCGRMsg" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand="" ProviderName="System.Data.SqlClient"></asp:SqlDataSource>
                    <asp:Repeater ID="Rpt_HSCGRMsg" runat="server" DataSourceID="SDS_HSCGRMsg" OnItemDataBound="Rpt_HSCGRMsg_ItemDataBound">
                        <ItemTemplate>
                            <asp:Label ID="LB_HSCGRMsgID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>

                            <div class="d-flex justify-content-end">
                                <div class="morefunction d-none">
                                    <a id="itembtn-more01" class="btn_more"><span class="ti-more-alt"></span></a>
                                    <div class="edit_area item_area" style="display: none">
                                        <ul>
                                            <li runat="server" id="Li_MsgEdit">
                                                <asp:LinkButton ID="LBtn_MsgEdit" runat="server" class="Editreply" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_MsgEdit_Click" Visible="true"><span class="ti-pencil mr-2"></span>編輯留言
                                                </asp:LinkButton>
                                                <%--<a class="Editreply" data-toggle="modal" data-target="#Div_EditReply"><span class="ti-pencil mr-2"></span>編輯留言</a>--%>
                                            </li>
                                            <li runat="server" id="Li_MsgDel">
                                                <asp:LinkButton ID="LBtn_MsgDel" runat="server" class="deletereply" OnClick="LBtn_MsgDel_Click" CommandArgument='<%# Eval("HID") %>'><span class="ti-trash mr-2"></span>刪除留言</asp:LinkButton></li>
                                            <li runat="server" id="Li_MsgHide">
                                                <asp:LinkButton ID="LBtn_MsgHide" runat="server" class="hidereply" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_MsgHide_Click"><span class="fa fa-eye-slash mr-2" style="font-size:13px;"></span>隱藏留言</asp:LinkButton></li>
                                        </ul>
                                    </div>
                                </div>
                            </div>


                            <div class="row d-flex justify-content-start align-items-top">

                                <div class="col-md-2" style="width: 13%; text-align: center;">
                                    <div class="profile-image">
                                        <asp:Image ID="IMG_HImg" runat="server" CssClass="" ImageUrl="images/icons/profile_small.jpg" />
                                        <asp:Label ID="LB_HImg" runat="server" Text='<%# Eval("HImg") %>' Visible="false"></asp:Label>
                                    </div>
                                    <div class="info">
                                         <asp:Label ID="LB_HMemberID" runat="server" Text='<%# Eval("HMemberID") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="LB_HUserName" runat="server" Text='<%# Eval("HUserName") %>' CssClass="b-block mb-0"></asp:Label>
                                    </div>
                                    <div class="time">
                                        <asp:Label ID="LB_HCreateDT" runat="server" Text='<%# Eval("HCreateDT") %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="content col-md-10">
                                    <p>
                                        <asp:Label ID="LB_HContent" runat="server" CssClass="replycontent" Text='<%# Eval("HContent") %>'></asp:Label>
                                    </p>
                                    <div>
                                        <asp:Label ID="LB_Audio" runat="server" CssClass="replycontent" Text='<%# Eval("HFile1") %>' Visible="false"></asp:Label>
                                        <audio id="Audio1" runat="server" src="uploads/CourseMaterial/002_220810.mp3" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false"></audio>
                                    </div>
                                    <div class="text-left mb-2">
                                        <ul class="d-flex list-unstyled align-items-center justify-content-start comment">
                                            <li><span class="ti-thumb-up mr-2"></span>
                                                <asp:Label ID="LB_MsgThumbUpSum" runat="server" Text="0"></asp:Label>
                                            </li>
                                            <li><span class="ti-heart mr-2"></span>
                                                <asp:Label ID="LB_MsgHeartSum" runat="server" Text="0"></asp:Label>
                                            </li>
                                            <li><span class="ti-face-smile mr-2"></span>
                                                <asp:Label ID="LB_MsgSmileSum" runat="server" Text="0"></asp:Label>
                                            </li>
                                        </ul>
                                    </div>
                                    <div class="text-right mb-3">
                                        <div class="cusfunction replyfunction">
                                            <asp:LinkButton ID="LBtn_HSCTMsgMood" runat="server" CssClass="btn btn-default btn-detail btn-mood" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_HSCTMsgMood_Click"><span class="ti-comments-smiley mr-2"></span>心情</asp:LinkButton>
                                            <div id="Div_FeelingArea" runat="server" class="moodarea justify-content-around align-items-center msgmoodarea" visible="false">
                                                <asp:LinkButton ID="LBtn_HMsgType1" runat="server" CssClass="btn btn-default btn-function" ToolTip="讚" TabIndex="1" OnClick="LBtn_HMsgType_Click" CommandArgument='<%# Container.ItemIndex %>'><span class="ti-thumb-up"></span></asp:LinkButton>
                                                <asp:LinkButton ID="LBtn_HMsgType2" runat="server" CssClass="btn btn-default btn-function" ToolTip="愛心" TabIndex="2" OnClick="LBtn_HMsgType_Click" CommandArgument='<%# Container.ItemIndex %>'><span class="fa fa-heart text-danger"></span></asp:LinkButton>
                                                <asp:LinkButton ID="LBtn_HMsgType3" runat="server" CssClass="btn btn-default btn-function" ToolTip="微笑" TabIndex="3" OnClick="LBtn_HMsgType_Click" CommandArgument='<%# Container.ItemIndex %>'><span class="ti-face-smile"></span></asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="mb-3" style="border-bottom: 2px solid #d3d3d3;"></div>

                                    <!--建立回應 start-->
                                    <div class="d-flex justify-content-start align-items-top">

                                        <div class="col-md-12 mb-3">
                                            <label class="d-block font-weight-normal">回應留言</label>
                                            <div>
                                                <textarea id="TA_HMsgResponse" class="form-control" runat="server"></textarea>
                                            </div>

                                            <div class="mt-2 text-right">
                                                <asp:LinkButton ID="LBtn_HSubmitMsgResponse" runat="server" CssClass="btn btn-outline-purple" OnClick="LBtn_HSubmitMsgResponse_Click" CommandArgument='<%# Container.ItemIndex %>'><span class="fa fa-paper-plane mr-2"></span>送出回應</asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                    <!--建立回應 end-->


                                    <!--留言回應 start-->
                                    <asp:SqlDataSource ID="SDS_HSCGRMsgResponse" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand="" ProviderName="System.Data.SqlClient"></asp:SqlDataSource>
                                    <asp:Repeater ID="Rpt_HSCGRMsgResponse" runat="server" DataSourceID="SDS_HSCGRMsgResponse" OnItemDataBound="Rpt_HSCGRMsgResponse_ItemDataBound">
                                        <ItemTemplate>
                                            <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                            <div class="d-flex justify-content-start align-items-top teachermsg" runat="server" id="teachermsg">

                                                <div class="col-md-3" style="width: 13%; text-align: center;">
                                                    <div class="profile-image">
                                                        <asp:Image ID="IMG_HImg" runat="server" CssClass="" ImageUrl="images/icons/profile_small.jpg" />
                                                        <asp:Label ID="LB_HImg" runat="server" Text='<%# Eval("HImg") %>' Visible="false"></asp:Label>
                                                    </div>
                                                    <div class="info">
                                                        <asp:Label ID="LB_HUserName" runat="server" Text='<%# Eval("HUserName") %>' CssClass="b-block"></asp:Label>
                                                    </div>
                                                    <div class="time">
                                                        <asp:Label ID="LB_HCreateDT" runat="server" Text='<%# Eval("HCreateDT") %>'></asp:Label>
                                                    </div>
                                                </div>
                                                <div class="content col-md-9">
                                                    <p>
                                                        <asp:Label ID="LB_HMsgResponse" runat="server" Text='<%# Eval("HGRMRContent") %>'></asp:Label>
                                                    </p>
                                                </div>

                                                <div class="d-flex justify-content-end">
                                                    <div class="morefunction   replymorefunction">
                                                        <asp:LinkButton ID="LBtn_ReplyMore" runat="server" class="btn_more" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_ReplyMore_Click"><span class="ti-more-alt"></span></asp:LinkButton>
                                                        <div class="edit_area edit_area2 item_area" id="Div_ReplyArea" runat="server" visible="false">
                                                            <ul>
                                                                <li runat="server" id="Li_MsgResponseEdit">
                                                                    <asp:LinkButton ID="LBtn_MsgResponseEdit" runat="server" class="Editreply" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_MsgResponseEdit_Click" Visible="true"><span class="ti-pencil mr-2"></span>編輯回應
                                                                    </asp:LinkButton>
                                                                    <%--<a class="Editreply" data-toggle="modal" data-target="#Div_EditResponse"><span class="ti-pencil mr-2"></span>編輯回應</a>--%>
                                                                </li>
                                                                <li runat="server" id="Li_MsgResponseDel">
                                                                    <asp:LinkButton ID="LBtn_MsgResponseDel" runat="server" class="deletereply" OnClick="LBtn_MsgResponseDel_Click" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>'><span class="ti-trash mr-2"></span>刪除回應</asp:LinkButton>
                                                                </li>
                                                                <li runat="server" id="Li_MsgResponseHide">
                                                                    <asp:LinkButton ID="LBtn_MsgResponseHide" runat="server" class="hidereply" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_MsgResponseHide_Click"><span class="fa fa-eye-slash mr-2" style="font-size:13px;"></span>隱藏回應</asp:LinkButton></li>
                                                            </ul>
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <!--留言回應 end-->
                                </div>

                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <hr />
                </div>
                <!--留言紀錄顯示範圍 end-->

                <!--AE20240826_暫時先隱藏-->
                <div class="mt-3 d-none">
                    <asp:LinkButton ID="LinkButton28" runat="server" CssClass="btn btn-more">查看更多留言</asp:LinkButton>
                </div>
                <div class="mt-5"></div>

            </div>
        </div>
    </div>





    <!--編輯留言Modal-->
    <div id="Div_MsgEdit" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 75%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0" id="EditReplyTitle">編輯留言</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <asp:Label ID="LB_HSCTMsgID" runat="server" Text="" Visible="false"></asp:Label>
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
                                                    <CKEditor:CKEditorControl ID="CKE_HContent_Edit" runat="server" class="form-control"></CKEditor:CKEditorControl>
                                                </div>
                                            </div>
                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    影片嵌入連結
                                                </label>
                                                <div class="form-group">
                                                    <asp:TextBox ID="TB_HVideoLink_Edit" runat="server" class="form-control" placeholder=" 影片連結" AutoComplete="off"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    上傳檔案
                                                 
                                                </label>
                                                <div class="form-group">
                                                    <asp:FileUpload ID="FU_HFile1_Edit" runat="server" class="mb-1" />
                                                    <asp:LinkButton ID="LBtn_HFile1_Edit" runat="server" CssClass="btn btn-outline-gray" OnClick="LBtn_HFile1_Edit_Click">上傳</asp:LinkButton>
                                                    <asp:Label ID="LB_HFileMsg1_Edit" runat="server" Text="" CssClass="text-danger"></asp:Label><!--提示訊息-->
                                                    <asp:HyperLink ID="HL_HFile1_Edit" runat="server" Target="_blank"></asp:HyperLink>
                                                    <asp:Label ID="LB_HFile1_Edit" runat="server" Text="" Visible="false" CssClass="text-muted"></asp:Label><!--上傳檔案名稱-->
                                                    <asp:LinkButton ID="LBtn_HFile1_EDel" runat="server" ToolTip="移除已經上傳的檔案" Visible="false" OnClick="LBtn_HFile1_EDel_Click"><i class="fa fa-times-circle" style="color:red" >刪除</i></asp:LinkButton>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer text-center">
                        <asp:Button ID="Btn_MsgEditSubmit" runat="server" class="button button-purple" Text="儲存" OnClick="Btn_MsgEditSubmit_Click" />
                        <asp:Button ID="Btn_MsgEditCancel" runat="server" class="button button-gray" Text="取消" Btmessage="確定要取消嗎？輸入的資料將不會儲存" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!--編輯回應 Modal-->
    <div id="Div_MsgResponseEdit" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 75%;">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0" id="exampleModalLongTitle">編輯回應</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <asp:Label ID="LB_HSCGRMsgResponseID" runat="server" Text="" Visible="false"></asp:Label>
                <div class="modal-body" style="width: 100%; padding: 10px">

                    <div class="p-0">
                        <div class="row clearfix pl-2 pr-2">
                            <div class="col-lg-12 col-md-12">

                                <div class="form-group">

                                    <div class="col-md-12 mb-2">
                                        <div class="row clearfix">

                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    回應內容</label>
                                                <div class="form-group">
                                                    <textarea id="TA_HGRMRContent_Edit" class="form-control" runat="server"></textarea>
                                                    <%--<CKEditor:CKEditorControl ID="CKEditorControl3" runat="server" class="form-control"></CKEditor:CKEditorControl>--%>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                </div>

                            </div>


                        </div>

                    </div>


                    <div class="modal-footer text-center">
                        <asp:Button ID="Btn_MsgREitdSubmit" runat="server" class="button button-purple" Text="儲存" OnClick="Btn_MsgREitdSubmit_Click" />
                        <asp:Button ID="Btn_MsgREditSubmit" runat="server" class="button button-gray" Text="取消" Btmessage="確定要取消嗎？輸入的資料將不會儲存" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!--==================刪除留言==================--->
    <div id="Div_MsgDelModal" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 50%;">
            <div class="modal-content" style="width: 100%;" runat="server" id="Div_MsgDel" visible="false">

                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3>填寫刪除留言原因</h3>
                </div>
                <div class="modal-body" style="width: 100%;">
                    <div class="form-group">
                        <label for="">刪除留言原因</label>
                        <asp:TextBox ID="TB_HMsgReason_Del" runat="server" class="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                </div>
                <div class="modal-footer text-center">
                    <asp:Button ID="Btn_MsgDel" runat="server" class="button button-purple" Text="確認刪除" OnClick="Btn_MsgDel_Click" Btmessage="確定要刪除這則留言嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                    <asp:Button ID="Button10" runat="server" class="button button-gray" Text="關閉" />
                </div>
            </div>

            <div class="modal-content" style="width: 100%;" runat="server" id="Div_MsgResponseDel" visible="false">

                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3>填寫刪除回應原因</h3>
                </div>
                <div class="modal-body" style="width: 100%;">
                    <div class="form-group">
                        <label for="">刪除回應原因</label>
                        <asp:TextBox ID="TB_MsgResponse_Del" runat="server" class="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                </div>
                <div class="modal-footer text-center">
                    <asp:Button ID="Btn_MsgResponseDel" runat="server" class="button button-purple" Text="確認刪除" OnClick="Btn_MsgResponseDel_Click" Btmessage="確定要刪除這則回應嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                    <asp:Button ID="Button2" runat="server" class="button button-gray" Text="關閉" />
                </div>
            </div>
        </div>
    </div>


    <!--==================隱藏留言==================--->
    <div id="Div_MsgHideModal" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <div class="modal-dialog" role="document" style="max-width: 50%;">
            <div class="modal-content" style="width: 100%;" runat="server" id="Div_MsgHide" visible="false">

                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3>填寫隱藏留言原因</h3>
                </div>
                <div class="modal-body" style="width: 100%;">
                    <div class="form-group">
                        <label for="">隱藏留言原因</label>
                        <asp:TextBox ID="TB_HMsgReason_Hide" runat="server" class="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                </div>
                <div class="modal-footer text-center">
                    <asp:Button ID="Btn_MsgHide" runat="server" class="button button-purple" Text="確認隱藏" OnClick="Btn_MsgHide_Click" Btmessage="確定要隱藏這則留言嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                    <asp:Button ID="Button12" runat="server" class="button button-gray" Text="關閉" />
                </div>
            </div>

            <div class="modal-content" style="width: 100%;" runat="server" id="Div_MsgResponseHide" visible="false">

                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3>填寫隱藏回應言原因</h3>
                </div>
                <div class="modal-body" style="width: 100%;">
                    <div class="form-group">
                        <label for="">隱藏回應原因</label>
                        <asp:TextBox ID="TB_MsgResponse_Hide" runat="server" class="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                </div>
                <div class="modal-footer text-center">
                    <asp:Button ID="Btn_MsgResponseHide" runat="server" class="button button-purple" Text="確認隱藏" OnClick="Btn_MsgResponseHide_Click" Btmessage="確定要隱藏這則回應嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                    <asp:Button ID="Button3" runat="server" class="button button-gray" Text="關閉" />
                </div>
            </div>

        </div>
    </div>


    <script src="js/jquery-3.4.1.min.js"></script>
    <!-- 摺疊圖標 -->
    <script>
        $(function () {
            $(".panel-heading").click(function (e) {
                $(this).find("span.fa-chevron-down").toggleClass("fa-chevron-up");
                $(this).find("span.fa-chevron-up").toggleClass("fa-chevron-down");
            });

            $(".solutionList li").click(function () {
                $(".solutionList .linkactive").removeClass('linkactive');
                $(this).addClass('linkactive');
            });


        //    var btnmore = document.getElementById('itembtn-more01');
        //    btnmore.addEventListener('click', function () {
        //        var display = $('.edit_area').css('display');
        //        if (display == 'none') {
        //            $('.edit_area').css("display", "block");
        //        } else {
        //            $('.edit_area').css("display", "none");
        //        }

        //    });


        //    var btnmore = document.getElementById('itembtn-more02');
        //    btnmore.addEventListener('click', function () {
        //        var display = $('.edit_area2').css('display');
        //        if (display == 'none') {
        //            $('.edit_area2').css("display", "block");
        //        } else {
        //            $('.edit_area2').css("display", "none");
        //        }

        //    });


        //    $('.btn-mood').hover(function () {
        //        var display = $('.moodarea').css('display');
        //        if (display == 'block') {
        //            //$('.moodarea').css("display", "none");
        //        } else {
        //            $('.moodarea').css("display", "block");
        //        }

        //    });
        //});


    </script>

</asp:Content>

