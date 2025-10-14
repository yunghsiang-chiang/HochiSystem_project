<%@ Page Title="" Language="C#" MasterPageFile="~/HochiSCMaster.master" AutoEventWireup="true" CodeFile="HSCTopicDetail.aspx.cs" Inherits="HSCTopicDetail" MaintainScrollPositionOnPostback="true" ValidateRequest="False" %>

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

        #twopart .middlepart {
            margin-left:15%;
        }

        canvas{
            width:100% !important;
        }

        @media (max-width:767px){
            #twopart .middlepart{
                margin-left:0 !important;
            }
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <!--從專欄上傳的檔案-->
    <asp:HiddenField ID="hfFile1" runat="server" Value="" />
    <asp:HiddenField ID="hfFile2" runat="server" Value="" />
    <asp:HiddenField ID="hfFile3" runat="server" Value="" />


    <!--課後教材-->
    <asp:HiddenField ID="HF_HUP1" runat="server" Value="" />
    <asp:HiddenField ID="HF_HUP2" runat="server" Value="" />
    <asp:HiddenField ID="HF_HUP3" runat="server" Value="" />
    <asp:HiddenField ID="HF_HUP4" runat="server" Value="" />
    <asp:HiddenField ID="HF_HUP5" runat="server" Value="" />

    <div class="d-none"><%-- class="d-none"--%>
        <asp:Label ID="LB_TID" runat="server" Text=""></asp:Label><!--主題HID-->
        <asp:Label ID="LB_HSCForumClassID" runat="server" Text=""></asp:Label><!--討論區名稱HID-->
    </div>

    <div class="middlepart">

        <div class="detailpart">
            <div>

                <div class="d-flex justify-content-between align-items-start">
                    <div class="d-flex justify-content-start align-items-center">
                        <div class="forum-type">
                            <asp:Label ID="LB_HSCSubClass" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="coursename" id="Div_ForumName" runat="server">
                            <asp:Label ID="LB_HSCForumName" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                
                </div>


                <!--class="leftpart"-->
                <h1 class="headtitle mb-3">
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
                            <asp:Label ID="LB_HMemberID" runat="server" Text="" Visible="false"></asp:Label>
                            <asp:Label ID="LB_HMember" runat="server" Text=""></asp:Label>
                        </div>
                        <div>
                            <asp:Label ID="LB_HDate" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                </div>


                <div class="d-flex justify-content-between align-items-center" id="Div_HashTag" runat="server">
                    <div class="tag d-flex justify-content-start align-items-center">
                        <label class="d-block phead mb-0 mr-3"><span class="fa fa-tag mr-1"></span></label>
                        <ul class="d-flex list-unstyled align-items-center justify-content-start detailkeyword">
                            <asp:Label ID="LB_HHashTag" runat="server" Text="" Visible="false"></asp:Label>
                            <asp:Repeater ID="Rpt_HHashTag" runat="server">
                                <ItemTemplate>
                                    <li runat="server" id="Li_HHashTag" class="mr-2 btn btn-outline-purple btn-rounded">
                                        <asp:LinkButton ID="LBtn_HHashTag" runat="server" Text='<%# Container.DataItem %>' CommandArgument='<%# Container.DataItem %>' OnClick="LBtn_HHashTag_Click"></asp:LinkButton>
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

                        <li class="d-flex">
                            <asp:LinkButton ID="LBtn_ThumbsUp" runat="server" CssClass="d-flex align-items-center" ToolTip="讚" TabIndex="1" OnClick="LBtn_FeelingType_Click">
                                <span class='ti-thumb-up mr-2'></span>
                            </asp:LinkButton>
                            <asp:LinkButton ID="LBtn_ThumbsUpNum" runat="server" OnClick="LBtn_ThumbsUpNum_Click">
                                <asp:Label ID="LB_ThumbsUpNum" runat="server" Text="0"></asp:Label>
                            </asp:LinkButton>
                        </li>
                        <li class="d-flex">
                            <asp:LinkButton ID="LBtn_Love" runat="server" CssClass="d-flex align-items-center" ToolTip="愛心" TabIndex="2" OnClick="LBtn_FeelingType_Click">
                                <span class='ti-heart mr-2'></span>
                            </asp:LinkButton>
                            <asp:LinkButton ID="LBtn_LoveNum" runat="server" OnClick="LBtn_LoveNum_Click">
                                <asp:Label ID="LB_LoveNum" runat="server" Text="0"></asp:Label>
                            </asp:LinkButton>


                        </li>
                        <li class="d-flex">
                            <asp:LinkButton ID="LBtn_Smile" runat="server" CssClass="d-flex align-items-center" ToolTip="微笑" TabIndex="3" OnClick="LBtn_FeelingType_Click">
                                <span class='ti-face-smile mr-2'></span>
                            </asp:LinkButton>
                            <asp:LinkButton ID="LBtn_SmileNum" runat="server" OnClick="LBtn_SmileNum_Click">
                                <asp:Label ID="LB_SmileNum" runat="server" Text="0"></asp:Label>
                            </asp:LinkButton>
                        </li>

                        <li>
                            <asp:LinkButton ID="LBtn_Seen" runat="server" OnClick="LBtn_Seen_Click">
                                <span class="ti-eye mr-2"></span>
                                <asp:Label ID="LB_SeenNum" runat="server" Text=""></asp:Label>
                            </asp:LinkButton>

                        </li>


                        <li><span class="ti-share mr-2"></span>
                            <asp:Label ID="LB_TopicShareSum" runat="server" Text="0"></asp:Label>
                        </li>
                    </ul>
                </div>
                <div class="mb-3" style="border-bottom: 2px solid #d3d3d3;"></div>
                <!--內容 .content start-->
                <asp:HiddenField ID="HF_File1" runat="server" />
                <asp:HiddenField ID="HF_File2" runat="server" />
                <asp:HiddenField ID="HF_File3" runat="server" />
                <%--<asp:Label ID="LB_File1" runat="server" Text="Label" Visible="false"></asp:Label>
                <asp:Label ID="LB_File2" runat="server" Text="Label" Visible="false"></asp:Label>
                <asp:Label ID="LB_File3" runat="server" Text="Label" Visible="false"></asp:Label>--%>
                <div class="content">
                    <div class="img-fluid" style="max-width: 50%">
                        <asp:Image ID="IMG_File1" runat="server" CssClass="mb-2" Style="width: 80%" Visible="false" />
                        <asp:Image ID="IMG_File2" runat="server" CssClass="mb-2" Style="width: 80%" Visible="false" />
                        <asp:Image ID="IMG_File3" runat="server" CssClass="mb-2" Style="width: 80%" Visible="false" />

                    </div>

                    <asp:Label ID="LB_HContent" runat="server" Text="" Visible="true" CssClass="f-18 d-block mb-4"></asp:Label>


                    <!--專欄PDF顯示--->
                    <asp:Label ID="Pdf_Content1" runat="server" Text="" Visible="false"></asp:Label>
                    <asp:Label ID="Pdf_Content2" runat="server" Text="" Visible="false"></asp:Label>
                    <asp:Label ID="Pdf_Content3" runat="server" Text="" Visible="false"></asp:Label>


                    <!--課後教材PDF顯示-->
                    <asp:Label ID="Pdf_ACContent1" runat="server" Text="" CssClass="pdf_area"></asp:Label>
                    <asp:Label ID="Pdf_ACContent2" runat="server" Text="" CssClass="pdf_area"></asp:Label>
                    <asp:Label ID="Pdf_ACContent3" runat="server" Text="" CssClass="pdf_area"></asp:Label>
                    <asp:Label ID="Pdf_ACContent4" runat="server" Text="" CssClass="pdf_area"></asp:Label>
                    <asp:Label ID="Pdf_ACContent5" runat="server" Text="" CssClass="pdf_area"></asp:Label>

                    <!--專欄語音檔-->
                    <div class="mb-3 mt-2 text-center" id="Div_File" runat="server">
                        <%--<label class="d-block phead"><span class="fa fa-file mr-1"></span>檔案</label>--%>
                        <audio id="Audio1" runat="server" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false" style="width: 50% !important;">
                            <source id="Source1" runat="server" src="" type="audio/mpeg">
                            Your browser does not support the audio element.
                        </audio>
                        <audio id="Audio2" runat="server" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false" style="width: 50% !important;">
                            <source id="Source2" runat="server" src="" type="audio/mpeg">
                            Your browser does not support the audio element.
                        </audio>
                        <audio id="Audio3" runat="server" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false" style="width: 50% !important;">
                            <source id="Source3" runat="server" src="" type="audio/mpeg">
                            Your browser does not support the audio element.
                        </audio>
                    </div>


                    <!--課後教材語音檔-->
                    <audio id="ACMaterial_audio1" runat="server" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false" style="width: 50% !important;">
                        <source id="Source4" runat="server" src="" type="audio/mpeg">
                        Your browser does not support the audio element.
                    </audio>
                    <audio id="ACMaterial_audio2" runat="server" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false" style="width: 50% !important;">
                        <source id="Source5" runat="server" src="" type="audio/mpeg">
                        Your browser does not support the audio element.
                    </audio>
                    <audio id="ACMaterial_audio3" runat="server" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false" style="width: 50% !important;">
                        <source id="Source6" runat="server" src="" type="audio/mpeg">
                        Your browser does not support the audio element.
                    </audio>
                    <audio id="ACMaterial_audio4" runat="server" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false" style="width: 50% !important;">
                        <source id="Source7" runat="server" src="" type="audio/mpeg">
                        Your browser does not support the audio element.
                    </audio>
                    <audio id="ACMaterial_audio5" runat="server" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false" style="width: 50% !important;">
                        <source id="Source8" runat="server" src="" type="audio/mpeg">
                        Your browser does not support the audio element.
                    </audio>




                    <!--影片檔-->
                    <div class="mt-4" id="Div_Video" runat="server">
                        <div class="embed-responsive embed-responsive-16by9" style="width: 50%;">
                            <iframe class="embed-responsive-item" runat="server" id="iframe_video"></iframe>
                        </div>
                    </div>




                    <div class="d-flex justify-content-between align-items-center">
 
                        <asp:LinkButton ID="LBtn_Msg" runat="server" OnClick="LBtn_Msg_Click" CssClass="btn-msg"><span class="fa fa-paper-plane mr-2"></span>我要留言</asp:LinkButton>


                        <a class="btn btn-default btn-detail btn-copy tooltipped tooltipped-s" title="分享連結" data-clipboard-action="copy" data-clipboard-target="#sharelink" aria-label="Copied!"><span class="ti-share mr-2"></span>分享連結</a>
                        <div class="text-white" style="position: absolute; z-index: -1">
                            <div id="sharelink" style="display: inline;">
                                <asp:Label ID="LB_HLink" runat="server" Text=""></asp:Label>
                            </div>
                        </div>


                    </div>


                </div>
                <!--內容 /.content end-->



                <!--留言  .comment start-->
                <asp:Panel ID="Panel_Comment" runat="server" Visible="false">
                    <div class="mb-3 mt-2" style="border-bottom: 2px solid #d3d3d3;"></div>
                    <div class="comment mb-3" runat="server" id="Div_HSCTMsg">
                        <label class="d-block font-weight-bold">建立留言(回應主題)</label>
                        <span style="font-size: 0.9rem">回應內容中如有提問或建議需請大愛光老師指引，請記得勾選為提問哦~!</span>
                        <br />
                        <asp:CheckBox ID="CB_HQuestionYN" runat="server" Style="font-size: 0.9rem" Text="是否為提問" />
                        <div>
                            <CKEditor:CKEditorControl ID="CKE_HContent" runat="server" class="form-control" Styles="height:60px"></CKEditor:CKEditorControl>
                        </div>
                        <div class="mt-2">
                            <asp:TextBox ID="TB_HVideoLink" runat="server" CssClass="form-control" placeholder="請輸入嵌入影片連結"></asp:TextBox>
                        </div>
                        <div class="mt-2">
                            <span class="text-danger font-weight-normal d-block" style="font-size: 14px">*格式僅限jpg、jpeg、png、gif、heic、heif、mp3、pdf、docx、doc、xlsx、xls</span>
                            <asp:FileUpload ID="FU_HFile1" runat="server" />
                            <asp:LinkButton ID="LBtn_HFile1" runat="server" CssClass="btn btn-outline-dark" OnClick="LBtn_HFile1_Click">上傳</asp:LinkButton>
                            <asp:Label ID="LB_HFileMsg1" runat="server" Text=""></asp:Label><!--提示訊息-->
                            <asp:LinkButton ID="LBtn_HFile1_Del" runat="server" ToolTip="移除已經上傳的檔案" Visible="false" OnClick="LBtn_HFile1_Del_Click"><i class="fa fa-times-circle" style="color:red" >刪除</i></asp:LinkButton>
                            <asp:Label ID="LB_HFile1" runat="server" Text="" Visible="false" CssClass="text-muted"></asp:Label><!--上傳檔案名稱-->
                        </div>
                        <div class="mt-2 text-right">
                            <asp:LinkButton ID="LBtn_HSubmitMsg" runat="server" CssClass="btn-sendreply" OnClick="LBtn_HSubmitMsg_Click"><span class="fa fa-paper-plane mr-2"></span>送出留言</asp:LinkButton>
                            <asp:LinkButton ID="LBtn_HSubmitMsgCancel" runat="server" CssClass="btn-cancel" OnClick="LBtn_HSubmitMsgCancel_Click">X 取消</asp:LinkButton>
                        </div>


                    </div>
                </asp:Panel>
                <!--留言  /.comment end-->

                <div class="mb-3  mt-2" style="border-bottom: 2px solid #d3d3d3;"></div>

                <!--留言紀錄顯示範圍 start-->
                <div class="comment_area" runat="server" id="Div_TopicMsg">
                    <asp:SqlDataSource ID="SDS_HSCTMsgMySQL" runat="server" ConnectionString="<%$ ConnectionStrings:HochiEIPConnection %>" ProviderName="MySql.Data.MySqlClient" SelectCommand="">
</asp:SqlDataSource>

                    <asp:SqlDataSource ID="SDS_HSCTMsg" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand="" ProviderName="System.Data.SqlClient"></asp:SqlDataSource>
                    <asp:Repeater ID="Rpt_HSCTMsg" runat="server"  OnItemDataBound="Rpt_HSCTMsg_ItemDataBound"><%--DataSourceID="SDS_HSCTMsg"--%>
                        <ItemTemplate>
                            <asp:Label ID="LB_HSCTMsgID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                            <asp:Label ID="LB_HStatus" runat="server" Text='<%# Eval("HStatus") %>' Visible="false"></asp:Label>

                            <div class="d-flex justify-content-end" id="Div_Editmore" runat="server">
                                <div class="morefunction">
                                    <asp:LinkButton ID="LBtn_More" runat="server" class="btn_more" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_More_Click"><span class="ti-more-alt"></span></asp:LinkButton>
                                    <div class="edit_area item_area" id="Div_Editarea" runat="server" visible="false">
                                        <ul>
                                            <li runat="server" id="Li_MsgEdit">
                                                <asp:LinkButton ID="LBtn_MsgEdit" runat="server" class="Editreply" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_MsgEdit_Click" Visible="true"><span class="ti-pencil mr-2"></span>編輯留言
                                                </asp:LinkButton>
                                            </li>
                                            <li runat="server" id="Li_MsgDel">
                                                <asp:LinkButton ID="LBtn_MsgDel" runat="server" class="deletereply" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_MsgDel_Click"><span class="ti-trash mr-2"></span>刪除留言</asp:LinkButton></li>
                                            <li runat="server" id="Li_MsgHide">
                                                <asp:LinkButton ID="LBtn_MsgHide" runat="server" class="hidereply" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_MsgHide_Click"><span class="fa fa-eye-slash mr-2" style="font-size:13px;"></span>隱藏留言</asp:LinkButton></li>
                                            <li runat="server" id="Li_MsgHideCancel">
                                                <asp:LinkButton ID="LBtn_MsgHideCancel" runat="server" class="hidereply" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_MsgHideCancel_Click"><span class="fa fa-eye mr-2" style="font-size:13px;"></span>取消隱藏</asp:LinkButton></li>
                                        </ul>
                                    </div>
                                </div>
                            </div>


                            <div class="row d-flex justify-content-start align-items-top divscmsgpadding" id="Div_SCMsg" runat="server">
                                <asp:Label ID="LB_EIPinfo" runat="server" Text='<%# Eval("HUserName") %>' Visible="false"></asp:Label>
                                <div class="col-md-2 comment_head" style="text-align: center;">
                                    <div class="profile-image">
                                        <asp:Image ID="IMG_HImg" runat="server" CssClass="" ImageUrl="images/icon.png" />
                                        <asp:Label ID="LB_HImg" runat="server" Text='<%# Eval("HImg") %>' Visible="false"></asp:Label>
                                    </div>
                                    <div class="info mt-2">
                                        <asp:Label ID="LB_HMemberID" runat="server" Text='<%# Eval("HMemberID") %>' Visible="false"></asp:Label>
                                             <asp:Label ID="LB_HUserName" runat="server" Text='<%# Eval("HUserName") %>' Visible="true"></asp:Label>
                                    </div>
                                    <div class="time">
                                        <asp:Label ID="LB_HCreateDT" runat="server" Text='<%# Eval("HCreateDT") %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="content col-md-10"  style="border-bottom: 2px solid #d3d3d3;">
                                     <asp:Label ID="LB_HQuestionYN" runat="server" Text='<%# Eval("HQuestionYN") %>' Visible="false"></asp:Label>
                                  <div class="questionmark" id="Div_Question" runat="server" visible="false">
                                      Q
                                  </div>
                                    <div id="Div_Status" runat="server" class="statustag" visible="false">
                                        此留言已被隱藏
                                    </div>
                                    <p class="mb-0">
                                        <asp:Label ID="LB_HContent" runat="server" CssClass="replycontent" Text='<%# Eval("HContent") %>'></asp:Label>
                                    </p>
                                    <div>
                                        <asp:Label ID="LB_File1" runat="server" Text='<%# Eval("HFile1") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="LB_HVideoLink" runat="server" Text='<%# Eval("HVideoLink") %>' Visible="false"></asp:Label>
                                        <!--圖片顯示-->
                                        <div class="img-fluid" runat="server" id="Div_Image" visible="false">
                                            <asp:Image ID="IMG_File1" runat="server" Style="width: 30%; margin-bottom: 5px;" ImageUrl='<%# Eval("HFile1") %>' Visible="true" />
                                        </div>

                                        <!--影片-->
                                        <div class="mt-4" id="Div_Video" runat="server" visible="false">
                                            <div class="embed-responsive embed-responsive-16by9" style="width: 50%;">
                                                <iframe class="embed-responsive-item" runat="server" id="iframe_video"></iframe>
                                            </div>
                                        </div>

                                        <!--語音檔-->

                                        <audio id="Audio_Reply" runat="server" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false" style="width: 50% !important;">
                                            <source id="Source_Reply" runat="server" src="" type="audio/mpeg">
                                            Your browser does not support the audio element.
                                        </audio>


                                        <!--其他文件檔-->
                                        <div runat="server" id="Div_Document" visible="false">
                                            <span class="fa fa-file mr-1"></span>
                                            <asp:Label ID="LB_Document" runat="server" Text="檔案" Visible="true"></asp:Label>
                                            <asp:LinkButton ID="LBtn_DownloadFile" runat="server" CssClass="btn btn-outline-info p-1" Visible="true" OnClick="LBtn_DownloadFile_Click" CommandArgument='<%# Eval("HFile1") %>'><span class="fa fa-download mr-1"></span>下載檔案</asp:LinkButton>
                                        </div>


                                    </div>
                                    <div class="d-flex justify-content-between align-items-center mt-2">
                                        <div class="text-left">
                                            <ul class="d-flex list-unstyled align-items-center justify-content-start comment">
                                                <li class="d-flex">
                                                    <asp:LinkButton ID="LBtn_MsgThumbsUp" runat="server" CssClass="d-flex align-items-center" ToolTip="讚" TabIndex="1" OnClick="LBtn_HMsgType_Click" CommandArgument='<%# Container.ItemIndex %>'>
                                                    <span class='ti-thumb-up mr-2'></span>
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="LBtn_MsgThumbsUpNum" runat="server" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_MsgThumbsUpNum_Click">
                                                        <asp:Label ID="LB_MsgThumbUpSum" runat="server" Text="0"></asp:Label>
                                                    </asp:LinkButton>

                                                </li>
                                                <li class="d-flex">
                                                    <asp:LinkButton ID="LBtn_MsgHeart" runat="server" CssClass="d-flex align-items-center" ToolTip="愛心" TabIndex="2" OnClick="LBtn_HMsgType_Click" CommandArgument='<%# Container.ItemIndex %>'>
                                                <span class="ti-heart mr-2"></span>
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="LBtn_MsgLoveNum" runat="server" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_MsgLoveNum_Click">
                                                        <asp:Label ID="LB_MsgHeartSum" runat="server" Text="0"></asp:Label>
                                                    </asp:LinkButton>
                                                </li>
                                                <li class="d-flex">
                                                    <asp:LinkButton ID="LBtn_MsgSmile" runat="server" CssClass="d-flex align-items-center" ToolTip="笑臉" TabIndex="3" OnClick="LBtn_HMsgType_Click" CommandArgument='<%# Container.ItemIndex %>'>
                                                <span class="ti-face-smile mr-2"></span>
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="LBtn_MsgSmileNum" runat="server" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_MsgSmileNum_Click">
                                                        <asp:Label ID="LB_MsgSmileSum" runat="server" Text="0"></asp:Label>
                                                    </asp:LinkButton>
                                                </li>
                                            </ul>
                                        </div>
                                        <div class="text-right">
                                            <asp:LinkButton ID="LBtn_Reply" runat="server" OnClick="LBtn_Reply_Click" CssClass="btn-msg" CommandArgument='<%# Container.ItemIndex %>'><span class="fa fa-reply mr-2"></span>我要回應</asp:LinkButton>
                                            <div class="cusfunction replyfunction  d-none">
                                                <asp:LinkButton ID="LBtn_HSCTMsgMood" runat="server" CssClass="btn btn-default btn-detail btn-mood" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_HSCTMsgMood_Click"><span class="ti-comments-smiley mr-2"></span>心情</asp:LinkButton>
                                                <div id="Div_FeelingArea" runat="server" class="moodarea justify-content-around align-items-center msgmoodarea" visible="false">
                                                    <asp:LinkButton ID="LBtn_HMsgType1" runat="server" CssClass="btn btn-default btn-function" ToolTip="讚" TabIndex="1" OnClick="LBtn_HMsgType_Click" CommandArgument='<%# Container.ItemIndex %>'><span class="ti-thumb-up"></span></asp:LinkButton>
                                                    <asp:LinkButton ID="LBtn_HMsgType2" runat="server" CssClass="btn btn-default btn-function" ToolTip="愛心" TabIndex="2" OnClick="LBtn_HMsgType_Click" CommandArgument='<%# Container.ItemIndex %>'><span class="fa fa-heart text-danger"></span></asp:LinkButton>
                                                    <asp:LinkButton ID="LBtn_HMsgType3" runat="server" CssClass="btn btn-default btn-function" ToolTip="微笑" TabIndex="3" OnClick="LBtn_HMsgType_Click" CommandArgument='<%# Container.ItemIndex %>'><span class="ti-face-smile"></span></asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="mb-3"></div>

                                    <!--建立回應 start-->
                                    <asp:Panel ID="Panel_ReplyMsg" runat="server" Visible="false">
                                        <div class="d-flex justify-content-start align-items-top">

                                            <div class="col-md-12 mb-3">
                                                <label class="d-block font-weight-normal">回應留言</label>
                                                <div>
                                                    <textarea id="TA_HMsgResponse" class="form-control" runat="server"></textarea>
                                                </div>

                                                <div class="mt-2 text-right">
                                                    <asp:LinkButton ID="LBtn_HSubmitMsgResponse" runat="server" CssClass="btn-sendreply" OnClick="LBtn_HSubmitMsgResponse_Click" CommandArgument='<%# Container.ItemIndex %>'><span class="fa fa-paper-plane mr-2"></span>送出回應</asp:LinkButton>


                                                    <asp:LinkButton ID="LBtn_VoiceMsg" runat="server" Visible="false" CssClass="btn btn-outline-purple" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_VoiceMsg_Click">
                                                    <span class="fa fa-microphone mr-2"></span>語音回應</asp:LinkButton>

                                                    <asp:LinkButton ID="LBtn_HSubmitMsgResponseCancel" runat="server" CssClass="btn-cancel" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_HSubmitMsgResponseCancel_Click">X 取消</asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                    <!--建立回應 end-->


                                    <!--留言回應 start-->
                                    <asp:SqlDataSource ID="SDS_HSCTMsgResponse" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand="" ProviderName="System.Data.SqlClient"></asp:SqlDataSource>
                                    <asp:Repeater ID="Rpt_HSCTMsgResponse" runat="server"  OnItemDataBound="Rpt_HSCTMsgResponse_ItemDataBound"><%--DataSourceID="SDS_HSCTMsgResponse"--%>
                                        <ItemTemplate>
                                            <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                                            <asp:Label ID="LB_HStatus" runat="server" Text='<%# Eval("HStatus") %>' Visible="false"></asp:Label>
                                            <div class="d-flex justify-content-start align-items-top teachermsg" runat="server" id="teachermsg">

                                                <div class="col-md-3 personinfo" style="text-align: center;">
                                                    <div class="profile-image">
                                                        <asp:Image ID="IMG_HImg" runat="server" CssClass="" ImageUrl="images/icon.png" />
                                                        <asp:Label ID="LB_HImg" runat="server" Text='<%# Eval("HImg") %>' Visible="false"></asp:Label>
                                                    </div>
                                                    <div class="info">
                                                        <asp:Label ID="LB_HMemberID" runat="server" Text='<%# Eval("HMemberID") %>' Visible="false"></asp:Label>
                                                        <asp:Label ID="LB_HUserName" runat="server" Text='<%# Eval("HUserName") %>' CssClass="b-block"></asp:Label>
                                                    </div>
                                                    <div class="time">
                                                        <asp:Label ID="LB_HCreateDT" runat="server" Text='<%# Eval("HCreateDT") %>'></asp:Label>
                                                    </div>
                                                </div>
                                                <div class="content col-md-9">
                                                    <div id="Div_Status" runat="server" class="statustag" visible="false">
                                                        此回應已被隱藏
                                                    </div>

                                                    <asp:Label ID="LB_HMsgResponse" runat="server" Text='<%# Eval("HMsgResponse") %>'></asp:Label>
                                                    <asp:Label ID="LB_HVoice" runat="server" Text='<%# Eval("HVoice") %>' Visible="false"></asp:Label>
                                                    <!--語音檔-->
                                                    <audio id="AudioResponse" runat="server" controls="controls" controlslist="nodownload" oncontextmenu="return false" visible="false" style="width: 60% !important;">
                                                        <source id="Source_Reply" runat="server" src="" type="audio/mpeg">
                                                        Your browser does not support the audio element.
                                                    </audio>



                                                </div>

                                                <div class="d-flex justify-content-end" id="Div_Editmore" runat="server">
                                                    <div class="morefunction  replymorefunction">
                                                        <asp:LinkButton ID="LBtn_ReplyMore" runat="server" class="btn_more" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_ReplyMore_Click"><span class="ti-more-alt"></span></asp:LinkButton>


                                                        <div class="edit_area edit_area2 item_area" id="Div_ReplyArea" runat="server" visible="false">
                                                            <ul>
                                                                <li runat="server" id="Li_MsgResponseEdit">
                                                                    <asp:LinkButton ID="LBtn_MsgResponseEdit" runat="server" class="Editreply" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_MsgResponseEdit_Click" Visible="true"><span class="ti-pencil mr-2"></span>編輯回應
                                                                    </asp:LinkButton>
                                                                </li>
                                                                <li runat="server" id="Li_MsgResponseDel">
                                                                    <asp:LinkButton ID="LBtn_MsgResponseDel" runat="server" class="deletereply" OnClick="LBtn_MsgResponseDel_Click" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>'><span class="ti-trash mr-2"></span>刪除回應</asp:LinkButton>
                                                                </li>
                                                                <li runat="server" id="Li_MsgResponseHide">
                                                                    <asp:LinkButton ID="LBtn_MsgResponseHide" runat="server" class="hidereply" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_MsgResponseHide_Click"><span class="fa fa-eye-slash mr-2" style="font-size:13px;"></span>隱藏回應</asp:LinkButton></li>

                                                                <li runat="server" id="Li_MsgResponseHideCancel">
                                                                    <asp:LinkButton ID="LBtn_MsgResponseHideCancel" runat="server" class="hidereply" CommandArgument='<%# Eval("HID") %>' CommandName='<%# Container.ItemIndex%>' OnClick="LBtn_MsgResponseHideCancel_Click"><span class="fa fa-eye mr-2" style="font-size:13px;"></span>取消隱藏</asp:LinkButton></li>
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

                                            <hr />

                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    編輯原因(非本人編輯時請填寫原因)
                                                </label>
                                                <div class="form-group">
                                                    <asp:TextBox ID="TB_HMsgReason_Edit" runat="server" class="form-control" placeholder=" 編輯原因" AutoComplete="off"></asp:TextBox>
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

    <!--編輯回應 &Modal-->
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
                <asp:Label ID="LB_HSCTMsgResponseID" runat="server" Text="" Visible="false"></asp:Label>
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
                                                    <textarea id="TA_HMsgResponse_Edit" class="form-control" runat="server"></textarea>
                                                    <%--<CKEditor:CKEditorControl ID="CKEditorControl3" runat="server" class="form-control"></CKEditor:CKEditorControl>--%>
                                                </div>
                                            </div>

                                            <hr />

                                            <div class="col-md-12 col-sm-12">
                                                <label class="font-weight-bold">
                                                    編輯原因(非本人編輯時請填寫原因)
                                                </label>
                                                <div class="form-group">
                                                    <asp:TextBox ID="TB_HMsgResReason_Edit" runat="server" class="form-control" placeholder=" 編輯原因" AutoComplete="off"></asp:TextBox>
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

    <!--分享連結觸發事件用--->
    <div class="d-none">
        <asp:Label ID="LB_ShareHID" runat="server" Text=""></asp:Label>
        <asp:Button ID="Btn_Share" runat="server" Text="Button" OnClick="Btn_Share_Click" />
    </div>



    <!--語音回應-->
    <div class="modal hochi-modal" id="AudioModal" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="Edit" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content" style="width: 100%;">
                <div class="modal-header p-2" style="background: #8cbcd4">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="mb-0">語音回應</h4>
                </div>
                <div class="modal-body" style="width: 100%; padding: 10px 20px">
                    <p style="clear: both;">
                        <button type="button" class="btn btn-info" onclick="getStream('audio');return false;" id="startRecord">開始錄音</button>
                        <audio muted autoplay="autoplay" id="recordaudio" name="recordaudio" style="display: none"></audio>
                        <img id="recording" src="images/voice-recording.gif" width="120" height="68" style="display: none">
                        <img id="pauseimg" src="images/voice-pause.gif" width="120" height="68" style="display: none">
                        <p id="timer" style="display: none">0:0</p>
                        <div id='tempRecord'></div>
                        <button class="btn btn-light" onclick="func_pauserecord();return false;" id='pauseRecord' style="display: none">暫停錄音</button>
                        <button class="btn btn-light" onclick="func_resumerecord();return false;" id='resumeRecord' style="display: none">繼續錄音</button>
                        <button class="btn btn-light" onclick="func_stoprecord();return false;" id='stopRecord' style="display: none">停止錄音</button>
                        <button class="btn btn-light" onclick="func_reRecord();return false;" id='reRecord' style="display: none">重新錄音</button>
                        <button class="btn btn-light" onclick="func_saveaudioreply();return false;" id='sentRecord' style="display: none">上傳錄音</button>
                        <%-- <asp:Button ID="Btn_SendVoiceMsg" runat="server" class="btn btn-light sentRecord" style="display: none" Text="上傳錄音" OnClick="Btn_SendVoiceMsg_Click" />--%>
                    </p>
                </div>
                <div class="modal-footer p-2">
                    <button type="button" class="button button-gray" data-dismiss="modal" onclick="func_stoprecord();return false;">關閉視窗</button>
                </div>
            </div>
        </div>
    </div>



    <!--==================心情名單==================--->
    <div id="Div_Smiley" class="modal fade hochi-modal" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <!-- Modal START-->
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Label ID="LB_HSCType" runat="server" Text=""></asp:Label><!-- 1=HSCTopic，2=HSCTMsg-->
                <div class="modal-dialog" role="document" style="max-width: 30%;">
                    <div class="modal-content" style="width: 100%;">

                        <div class="modal-header" style="background: #fff; padding-bottom: 0px">
                            <button type="button" class="close" data-dismiss="modal" style="color: #484848;">&times;</button>
                            <ul class="nav nav-tabs" id="ParaTab" role="tablist">
                                <li class="nav-item">
                                    <asp:LinkButton ID="LBtn_ThumbsUpM" runat="server" class="feelinglink nav-link  active show font-weight-bold" TabIndex="1" OnClick="LBtn_FeelingTypeM_Click"><%--OnClick="LBtn_FeelingTypeM_Click"--%>
                                    </asp:LinkButton>
                                </li>
                                <li class="nav-item">
                                    <asp:LinkButton ID="LBtn_HeartM" runat="server" class="feelinglink nav-link font-weight-bold" TabIndex="2" OnClick="LBtn_FeelingTypeM_Click"><%--OnClick="LBtn_FeelingTypeM_Click"--%>
                                    </asp:LinkButton>
                                </li>
                                <li class="nav-item">
                                    <asp:LinkButton ID="LBtn_SmileM" runat="server" class="feelinglink nav-link font-weight-bold" TabIndex="3" OnClick="LBtn_FeelingTypeM_Click"><%----%>
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



        });


    </script>

    <!--//AA20240129_複製功能JS套件-->
    <!--複製套件-->
    <script src="js/clipboard.js"></script>
    <script>
        var clipboard = new ClipboardJS('.btn-copy');
        clipboard.on('success', function (e) {
            //console.log(e);
            console.log(e.text.split('=')[1] + "///" + e.text);
            ////////document.getElementById("<%= LB_ShareHID.ClientID %>").value = e.text.split('=')[1];
            document.cookie = document.getElementById("<%= LB_HLink.ClientID %>").value;
            document.getElementById("<%= Btn_Share.ClientID %>").click();
            alert("已成功複製連結~將連結分享給大家吧~!");


        });
    </script>


    <script>
        /*語音回應*/
        //語音回應
        function func_audioreply(strId, strMemberID) {
            strMemberID = strMemberID;
            strSCTMsgID = strId;

            //console.log("strMemberID=" + strMemberID + ", strSCTMsgID=" + strId);

            var infoModal = $('#AudioModal');
            infoModal.modal('show');
        }

        var theStream;
        var theRecorder;
        var recordedChunks = [];
        var timer = null,
            interval = 1000,
            value = 0;
        function getUserMedia(options, successCallback, failureCallback) {
            var api = navigator.getUserMedia || navigator.webkitGetUserMedia ||
                navigator.mozGetUserMedia || navigator.msGetUserMedia;
            if (api) {
                return api.bind(navigator)(options, successCallback, failureCallback);
            }
        }

        function getStream(type) {
            if (!navigator.getUserMedia && !navigator.webkitGetUserMedia &&
                !navigator.mozGetUserMedia && !navigator.msGetUserMedia) {
                alert('您的瀏覽器不支此功能.');
                return;
            }

            $("#stopRecord").show();
            $("#startRecord").hide();
            $("#pauseRecord").show();

            $("#recording").show();
            $("#timer").show();
            //顯示錄音的時間
            if (timer !== null) return;
            timer = setInterval(function () {
                value = value + 1;
                document.getElementById("timer").innerHTML = Math.floor(value / 60) + ":" + value % 60;
            }, interval);

            var constraints = {};
            constraints[type] = true;
            getUserMedia(constraints, function (stream) {
                //var mediaControl = document.querySelector(type);
                var mediaControl = document.querySelector("#recordaudio");

                if ('srcObject' in mediaControl) {
                    mediaControl.srcObject = stream;

                } else if (navigator.mozGetUserMedia) {
                    mediaControl.mozSrcObject = stream;
                }
                theStream = stream;
                try {
                    recorder = new MediaRecorder(stream, { mimeType: "audio/webm" });
                } catch (e) {
                    console.error('錄音發音異常: ' + e);
                    return;
                }
                theRecorder = recorder;
                //    console.log('MediaRecorder created');
                recorder.ondataavailable = recorderOnDataAvailable;
                recorder.start(100);
            }, function (err) {
                alert('發生錯誤: ' + err);
            });
        }
        //在音頻數據可用時，會將音頻數據分段（chunked）保存在audioChunks陣列中
        function recorderOnDataAvailable(event) {
            if (event.data.size == 0) return;
            recordedChunks.push(event.data);
        }





        function func_saveaudioreply() {


            //console.log("RespondedMemberID=" + strMemberID);

            var blob = new Blob(recordedChunks, { type: "audio/mp3" });
            var form_data = new FormData();
            form_data.append('audioFile', blob);
            form_data.append('HSCTMsgID', strSCTMsgID);
            //form_data.append('RespondedMemberID', strMemberID);
            //form_data.append('type', 'savereply');
            //form_data.append('reply', '');
            if (blob.size == 0)
                return;

            $.ajax({
                url: 'HUploadVoiceMsg.aspx',
                method: 'post',
                cache: false,
                contentType: false,
                processData: false,
                data: form_data,
                success: function (data) {
                    //console.log('data=' + data);

                    location = location;
                    //window.location.reload(true);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("無法顯示！");
                }
            });
            //setTimeout() here is needed for Firefox.
            //setTimeout(function () {
            //    (window.URL || window.webkitURL).revokeObjectURL(url);
            //}, 100);
        }


        function func_stoprecord() {
            if (typeof (theRecorder) != "undefined") {
                theRecorder.stop();
                theStream.getTracks()[0].stop();
            }
            c = 0;
            clearInterval(timer);
            $("#recording").hide();
            $("#timer").hide();
            var blob = new Blob(recordedChunks, { type: "audio/webm" });
            var url = (window.URL || window.webkitURL).createObjectURL(blob);
            var x = document.createElement("AUDIO");
            x.setAttribute("src", url);
            x.setAttribute("controls", "controls");
            x.setAttribute("controlslist", "nodownload");
            $("#tempRecord").append(x);
            //audiojs.createAll();
            $("#stopRecord").hide();
            $("#pauseRecord").hide();
            $("#reRecord").show();
            //$("#sentRecord").show();
            $("#sentRecord").show();
            $("#pauseimg").hide();
            $("#resumeRecord").hide();

        }
        function func_reRecord() {
            $("#tempRecord").empty();
            $("#recording").show();
            $("#timer").show();
            $("#stopRecord").show();
            $("#reRecord").hide();
            $("#sentRecord").hide();
            document.getElementById("timer").innerHTML = '0:0';
            recordedChunks = [];
            timer = null;
            value = 0;
            getStream('audio');
        }
        function func_pauserecord() {

            theRecorder.pause();
            clearInterval(timer);
            timer = null
            $("#pauseimg").show();
            $("#recording").hide();
            $("#resumeRecord").show();
            $("#pauseRecord").hide();

        }
        function func_resumerecord() {
            if (timer !== null) return;
            timer = setInterval(function () {
                value = value + 1;
                document.getElementById("timer").innerHTML = Math.floor(value / 60) + ":" + value % 60;
            }, interval);
            theRecorder.resume();

            $("#resumeRecord").hide();
            $("#pauseRecord").show();
            $("#pauseimg").hide();
            $("#recording").show();
        }
    </script>




    <!-- PDF viewer-->
    <script type="text/javascript" src="js/pdf.js"></script>
    <script>
        //var url = $('#ctl00_ContentPlaceHolder1_hfFile1').val();
        var url1 = $('#<%=hfFile1.ClientID%>').val();
       <%-- var url2 = $('#<%=hfFile2.ClientID%>').val();
        var url3 = $('#<%=hfFile3.ClientID%>').val();--%>

        var pdfDoc = null,
            pageNum = 1,
            pageRendering = false,
            pageNumPending = null,
            scale = 1,
            zoomRange = 0.25,
            canvas = document.getElementById('the-canvas'),
            ctx = canvas.getContext('2d');

        /**
         * Get page info from document, resize canvas accordingly, and render page.
         * @param num Page number.
         */
        function renderPage(num, scale) {
            pageRendering = true;
            // Using promise to fetch the page
            pdfDoc.getPage(num).then(function (page) {
                var viewport = page.getViewport(scale);
                canvas.height = viewport.height;
                canvas.width = viewport.width;

                // Render PDF page into canvas context
                var renderContext = {
                    canvasContext: ctx,
                    viewport: viewport
                };
                var renderTask = page.render(renderContext);

                // Wait for rendering to finish
                renderTask.promise.then(function () {
                    pageRendering = false;
                    if (pageNumPending !== null) {
                        // New page rendering is pending
                        renderPage(pageNumPending);
                        pageNumPending = null;
                    }
                });
            });

            // Update page counters
            document.getElementById('page_num').value = num;
        }

        /**
         * If another page rendering in progress, waits until the rendering is
         * finised. Otherwise, executes rendering immediately.
         */
        function queueRenderPage(num) {
            if (pageRendering) {
                pageNumPending = num;
            } else {
                renderPage(num, scale);
            }
        }

        /**
         * Displays previous page.
         */
        function onPrevPage() {
            if (pageNum <= 1) {
                return;
            }
            pageNum--;
            var scale = pdfDoc.scale;
            queueRenderPage(pageNum, scale);
            return false;
        }
        document.getElementById('prev').addEventListener('click', onPrevPage);

        /**
         * Displays next page.
         */
        function onNextPage() {
            if (pageNum >= pdfDoc.numPages) {
                return;
            }
            pageNum++;
            var scale = pdfDoc.scale;
            queueRenderPage(pageNum, scale);
            return false;
        }
        document.getElementById('next').addEventListener('click', onNextPage);

        /**
         * Zoom in page.
         */
        function onZoomIn() {
            if (scale >= pdfDoc.scale) {
                return;
            }
            scale += zoomRange;
            var num = pageNum;
            renderPage(num, scale);
            return false;
        }
        document.getElementById('zoomin').addEventListener('click', onZoomIn);

        /**
         * Zoom out page.
         */
        function onZoomOut() {
            if (scale >= pdfDoc.scale) {
                return;
            }
            scale -= zoomRange;
            var num = pageNum;
            queueRenderPage(num, scale);
            return false;
        }
        document.getElementById('zoomout').addEventListener('click', onZoomOut);

        /**
         * Zoom fit page.
         */
        function onZoomFit() {
            if (scale >= pdfDoc.scale) {
                return;
            }
            scale = 1;
            var num = pageNum;
            queueRenderPage(num, scale);
            return false;
        }
        document.getElementById('zoomfit').addEventListener('click', onZoomFit);


        /**
         * Asynchronously downloads PDF.
         */
        PDFJS.getDocument(url1).then(function (pdfDoc_) {
            pdfDoc = pdfDoc_;
            var documentPagesNumber = pdfDoc.numPages;
            document.getElementById('page_count').textContent = '/ ' + documentPagesNumber;

            $('#page_num').on('change', function () {
                var pageNumber = Number($(this).val());

                if (pageNumber > 0 && pageNumber <= documentPagesNumber) {
                    queueRenderPage(pageNumber, scale);
                }

            });

            // Initial/first page rendering
            renderPage(pageNum, scale);
        });


    </script>




    <!-- PDF viewer-->
    <script type="text/javascript" src="js/pdf.js"></script>
    <script>
        var url = "";
        var url1 = $('#<%=HF_HUP1.ClientID%>').val();
        var url2 = $('#<%=HF_HUP2.ClientID%>').val();
        var url3 = $('#<%=HF_HUP3.ClientID%>').val();
        var url4 = $('#<%=HF_HUP4.ClientID%>').val();
        var url5 = $('#<%=HF_HUP5.ClientID%>').val();
        var url6 = $('#<%=hfFile1.ClientID%>').val();
        var url7 = $('#<%=hfFile2.ClientID%>').val();
        var url8 = $('#<%=hfFile3.ClientID%>').val();


        console.log("url6=" + url6);

        if (url1 != "") {
            PDF(1);
        }

        if (url2 != "") {
            PDF(2);
        }

        if (url3 != "") {
            PDF(3);
        }

        if (url4 != "") {
            PDF(4);
        }

        if (url5 != "") {
            PDF(5);
        }

        if (url6 != "") {
            PDF(6);
        }

        if (url7 != "") {
            PDF(7);
        }

        if (url8 != "") {
            PDF(8);
        }

        function PDF(No) {
            //var url = $('#ctl00_ContentPlaceHolder1_hfFile1').val();
            console.log("PDF=" + $('#ctl00_ContentPlaceHolder1_hfFile1').val());

            var pdfDoc = null,
                pageNum = 1,
                pageRendering = false,
                pageNumPending = null,
                scale = 1,
                zoomRange = 0.25,
                canvas = document.getElementById('the-canvas' + No),
                ctx = canvas.getContext('2d');

            /**
             * Get page info from document, resize canvas accordingly, and render page.
             * @param num Page number.
             */
            function renderPage(num, scale) {
                pageRendering = true;
                // Using promise to fetch the page
                pdfDoc.getPage(num).then(function (page) {
                    var viewport = page.getViewport(scale);
                    canvas.height = viewport.height;
                    canvas.width = viewport.width;

                    // Render PDF page into canvas context
                    var renderContext = {
                        canvasContext: ctx,
                        viewport: viewport
                    };
                    var renderTask = page.render(renderContext);

                    // Wait for rendering to finish
                    renderTask.promise.then(function () {
                        pageRendering = false;
                        if (pageNumPending !== null) {
                            // New page rendering is pending
                            renderPage(pageNumPending);
                            pageNumPending = null;
                        }
                    });
                });

                // Update page counters
                document.getElementById('page_num' + No).value = num;
            }

            /**
             * If another page rendering in progress, waits until the rendering is
             * finised. Otherwise, executes rendering immediately.
             */
            function queueRenderPage(num) {
                if (pageRendering) {
                    pageNumPending = num;
                } else {
                    renderPage(num, scale);
                }
            }

            /**
             * Displays previous page.
             */
            function onPrevPage() {
                if (pageNum <= 1) {
                    return;
                }
                pageNum--;
                var scale = pdfDoc.scale;
                queueRenderPage(pageNum, scale);
                return false;
            }
            document.getElementById('prev' + No).addEventListener('click', onPrevPage);

            /**
             * Displays next page.
             */
            function onNextPage() {
                if (pageNum >= pdfDoc.numPages) {
                    return;
                }
                pageNum++;
                var scale = pdfDoc.scale;
                queueRenderPage(pageNum, scale);
                return false;
            }
            document.getElementById('next' + No).addEventListener('click', onNextPage);

            /**
             * Zoom in page.
             */
            function onZoomIn() {
                if (scale >= pdfDoc.scale) {
                    return;
                }
                scale += zoomRange;
                var num = pageNum;
                renderPage(num, scale);
                return false;
            }
            document.getElementById('zoomin' + No).addEventListener('click', onZoomIn);

            /**
             * Zoom out page.
             */
            function onZoomOut() {
                if (scale >= pdfDoc.scale) {
                    return;
                }
                scale -= zoomRange;
                var num = pageNum;
                queueRenderPage(num, scale);
                return false;
            }
            document.getElementById('zoomout' + No).addEventListener('click', onZoomOut);

            /**
             * Zoom fit page.
             */
            function onZoomFit() {
                if (scale >= pdfDoc.scale) {
                    return;
                }
                scale = 1;
                var num = pageNum;
                queueRenderPage(num, scale);
                return false;
            }
            document.getElementById('zoomfit' + No).addEventListener('click', onZoomFit);


            /**
             * Asynchronously downloads PDF.
             */
            if (No == 1) {
                url = url1;
            }
            else if (No == 2) {
                url = url2;
            }
            else if (No == 3) {
                url = url3;
            }
            else if (No == 4) {
                url = url4;
            }
            else if (No == 5) {
                url = url5;
            }
            else if (No == 6) {
                url = url6;
            }
            else if (No == 7) {
                url = url7;
            }
            else if (No == 8) {
                url = url8;
            }

            PDFJS.getDocument(url).then(function (pdfDoc_) {
                pdfDoc = pdfDoc_;
                var documentPagesNumber = pdfDoc.numPages;
                document.getElementById('page_count' + No).textContent = '/ ' + documentPagesNumber;

                $('#page_num' + No).on('change', function () {
                    var pageNumber = Number($(this).val());

                    if (pageNumber > 0 && pageNumber <= documentPagesNumber) {
                        queueRenderPage(pageNumber, scale);
                    }

                });

                // Initial/first page rendering
                renderPage(pageNum, scale);
            });

        }





    </script>



</asp:Content>

