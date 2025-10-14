<%@ Page Title="" Language="C#" MasterPageFile="~/HochiSCMaster.master" AutoEventWireup="true" CodeFile="HSCGRecordForum.aspx.cs" Inherits="HSCGRecordForum" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">




    <asp:Label ID="LB_HSCForumClassCID" runat="server" Text="" Visible="false"></asp:Label>


    <div class="middlepart">

        <p class="mb-2">
            所有討論區 > 成長紀錄
        </p>
        <div class="d-flex justify-content-between align-items-center">
            <h4 class="font-weight-bold">
                <asp:Literal ID="LTR_HSCForumClassC" runat="server"></asp:Literal>
            </h4>

        </div>


        <!--成長紀錄主題內容  .topicarea start-->
        <asp:SqlDataSource ID="SDS_HSCGRecord" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand=""
            ProviderName="System.Data.SqlClient"></asp:SqlDataSource>
        <asp:Repeater ID="Rpt_HSCGRecord" runat="server" DataSourceID="SDS_HSCGRecord" OnItemDataBound="Rpt_HSCGRecord_ItemDataBound">
            <ItemTemplate>
                <div class="topicarea">
                    <asp:Label ID="LB_HID" runat="server" Text='<%# Eval("HID") %>' Visible="false"></asp:Label>
                    <div class="d-flex justify-content-between align-items-start">
                        <div class="d-flex justify-content-start align-items-center">

                            <div class="coursename">
                                <asp:Label ID="LB_HSCForumClassC" runat="server" Text="成長紀錄"></asp:Label><!--討論區名稱-->
                            </div>
                        </div>


                    </div>


                    <div class="mt-3  d-flex justify-content-between align-items-center">
                        <h5 class="font-weight-bold">
                            <!--主題名稱-->
                            <asp:Literal ID="LTR_HTopicName" runat="server" Text='<%# Eval("HTopicName") %>'></asp:Literal>
                        </h5>


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
                                </div>
                                <div>
                                    <asp:Label ID="LB_HDate" runat="server" Text='<%# Eval("HCreateDT") %>'></asp:Label>
                                </div>
                            </div>
                        </div>

                    </div>
                    <!--內容 .content start-->
                    <div class="content">

                        <div id="Div_Show" runat="server">

                            <asp:LinkButton ID="LBtn_View" runat="server" OnClientClick="" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_View_Click">查看更多</asp:LinkButton>

                            <div class="mt-3">
                                <ul class="d-flex list-unstyled align-items-center justify-content-start comment">
                                    <li>
                                        <asp:LinkButton ID="LBtn_MsgDetail" runat="server" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_View_Click">
                                            <span class="ti-comments mr-2"></span>
                                            <asp:Label ID="LB_MsgNum" runat="server" Text=""></asp:Label>
                                        </asp:LinkButton>
                                    </li>
                                    <li>
                                        <%-- <a href="javascript:void(0)" data-toggle="modal" data-target="#smiley">--%>
                                        <asp:LinkButton ID="LBtn_MoodModal" runat="server" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_MoodModal_Click">
                                            <span class="ti-comments-smiley mr-2"></span>
                                            <asp:Label ID="LB_FeelingNum" runat="server" Text=""></asp:Label>
                                        </asp:LinkButton>
                                    </li>
                                    <li><span class="ti-eye mr-2"></span>
                                        <asp:Label ID="LB_ViewNum" runat="server" Text=""></asp:Label>
                                    </li>
                                    <li><span class="ti-share mr-2"></span>
                                        <asp:Label ID="LB_ShareNum" runat="server" Text=""></asp:Label>
                                    </li>
                                </ul>
                            </div>
                            <hr class="mt-1 mb-1" />
                            <div class="d-flex justify-content-end align-items-center ">
                                <div class="cusfunction">
                                    <%--<asp:LinkButton ID="LinkButton30" runat="server" CssClass="btn btn-default btn-detail"><span class="ti-face-smile mr-2"></span>開心</asp:LinkButton>--%>
                                    <asp:LinkButton ID="LBtn_Feeling" runat="server" CssClass="btn btn-default btn-detail" CommandArgument='<%# Eval("HID") %>' CommandName='<%#  Container.ItemIndex%>' OnClick="LBtn_Feeling_Click"><span class="ti-comments-smiley mr-2"></span>心情</asp:LinkButton>
                                    <div id="Div_FeelingArea" runat="server" class="moodarea justify-content-around align-items-center " visible="false">
                                        <asp:LinkButton ID="LBtn_ThumbsUp" runat="server" CssClass="btn btn-default btn-function" ToolTip="讚" TabIndex="1" CommandArgument='<%# Eval("HID") %>' CommandName='<%#  Container.ItemIndex%>' OnClick="LBtn_FeelingType_Click"><span class="ti-thumb-up"></span></asp:LinkButton>
                                        <asp:LinkButton ID="LBtn_Love" runat="server" CssClass="btn btn-default btn-function" ToolTip="愛心" TabIndex="2" CommandArgument='<%# Eval("HID") %>' CommandName='<%#  Container.ItemIndex%>' OnClick="LBtn_FeelingType_Click"><span class="fa fa-heart text-danger"></span></asp:LinkButton>
                                        <asp:LinkButton ID="LBtn_Smile" runat="server" CssClass="btn btn-default btn-function" ToolTip="微笑" TabIndex="3" CommandArgument='<%# Eval("HID") %>' CommandName='<%#  Container.ItemIndex%>' OnClick="LBtn_FeelingType_Click"><span class="ti-face-smile"></span></asp:LinkButton>
                                    </div>

                                    <asp:LinkButton ID="LBtn_Msg" runat="server" CssClass="btn btn-default btn-detail" CommandArgument='<%# Eval("HID") %>' OnClick="LBtn_View_Click">
                  <span class="ti-comment mr-2"></span>留言
                                    </asp:LinkButton>

                                    <a class="btn btn-default btn-detail btn-copy tooltipped tooltipped-s" title="分享連結" data-clipboard-action="copy" data-argeument='<%# Eval("HID") %>' data-clipboard-target="#text_<%# Container.ItemIndex + 1 %>" aria-label="Copied!"><span class="ti-share mr-2"></span>分享連結</a>
                                    <div class="text-white" style="position:absolute;z-index:-1">
                                        <div id="text_<%# Container.ItemIndex + 1 %>" style="display: inline;">
                                            <asp:Label ID="LB_HLink" runat="server" Text=""></asp:Label>
                                        </div>
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

    </div>


    <!--分享連結觸發事件用--->
    <div class="d-none">
        <asp:Label ID="LB_ShareHID" runat="server" Text=""></asp:Label>
        <asp:Button ID="Btn_Share" runat="server" Text="Button" OnClick="Btn_Share_Click" />
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
                                                    <CKEditor:CKEditorControl ID="CKEditorControl1" runat="server" class="form-control"></CKEditor:CKEditorControl>
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
        <div class="modal-dialog" role="document" style="max-width: 30%;">
            <div class="modal-content" style="width: 100%;">
                <asp:Label ID="LB_SCTopicID" runat="server" Text="Label" Visible="false"></asp:Label>
                <div class="modal-header" style="background: #fff; padding-bottom: 0px">
                    <button type="button" class="close" data-dismiss="modal" style="color: #484848;">&times;</button>
                    <ul class="nav nav-tabs" id="ParaTab" role="tablist">

                        <li class="nav-item">
                            <asp:LinkButton ID="LBtn_ThumbsUp" runat="server" class="nav-link  active show font-weight-bold" TabIndex="1" OnClick="LBtn_FeelingTypeM_Click">
                             <%--<span class="ti-thumb-up mr-2"></span>
                             <asp:Label ID="LB_ThumbsUpNum" runat="server" Text="Label"></asp:Label>--%>
                            </asp:LinkButton>
                        </li>
                        <li class="nav-item">
                            <asp:LinkButton ID="LBtn_Heart" runat="server" class="nav-link font-weight-bold" TabIndex="2" OnClick="LBtn_FeelingTypeM_Click">
                            <%-- <span class="ti-heart mr-2"></span>
                             <asp:Label ID="LB_LoveNum" runat="server" Text="Label"></asp:Label>--%>
                            </asp:LinkButton>
                        </li>
                        <li class="nav-item">
                            <asp:LinkButton ID="LBtn_Smile" runat="server" class="nav-link font-weight-bold" TabIndex="3" OnClick="LBtn_FeelingTypeM_Click">
                             <%--<span class="ti-face-smile mr-2"></span>
                             <asp:Label ID="LB_SmileNum" runat="server" Text="Label"></asp:Label>--%>
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

                                    <asp:SqlDataSource ID="SDS_HSCGRecordMood" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                    <asp:Repeater runat="server" ID="Rpt_HSCGRecordMood" DataSourceID="SDS_HSCGRecordMood" OnItemDataBound="Rpt_HSCGRecordMood_ItemDataBound">
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




            //var btnmore = document.getElementById('btn-more');
            //btnmore.addEventListener('click', function () {
            //    var display = $('.edit_area').css('display');
            //    if (display == 'none') {
            //        $('.edit_area').css("display", "block");
            //    } else {
            //        $('.edit_area').css("display", "none");
            //    }

            //});

            //var itembtn = document.getElementById('itembtn-more');
            //itembtn.addEventListener('click', function () {
            //    var display = $('.item_area').css('display');
            //    if (display == 'none') {
            //        $('.item_area').css("display", "block");
            //    } else {
            //        $('.item_area').css("display", "none");
            //    }

            //});

            //var itembtn = document.getElementById('itembtn-more01');
            //itembtn.addEventListener('click', function () {
            //    var display = $('.item_area').css('display');
            //    if (display == 'none') {
            //        $('.item_area').css("display", "block");
            //    } else {
            //        $('.item_area').css("display", "none");
            //    }

            //});

            //$('.btn-mood').hover(function () {
            //    var display = $('.moodarea').css('display');
            //    if (display == 'block') {
            //        //$('.moodarea').css("display", "none");
            //    } else {
            //        $('.moodarea').css("display", "block");
            //    }

            //});


            //$('.Editreply').click(function () {
            //    $('#Div_EditReply').modal('show');
            //    $('#detail').modal('hide');
            //});

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

</asp:Content>

