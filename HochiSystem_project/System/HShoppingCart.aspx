<%@ Page Title="" Language="C#" MasterPageFile="~/HochiMaster.master" AutoEventWireup="true" CodeFile="HShoppingCart.aspx.cs" Inherits="HShoppingCart" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <link rel="stylesheet" href="css/shoppingcart.css?230214">
    <style>
        .side-btn-wrap {
            display: none !important;
        }

        .cb_outer label {
            margin-left: 5px;
        }

        .cb_outer input[type=checkbox] {
            margin-top: 4px;
        }

        /*AA20240222*/
        .updirs {
            max-width: 12%;
        }


        @media (max-width:767px) {
            .mobile-left {
                text-align: left !important;
            }
        }
    </style>


    <div id="divProgress" style="text-align: center; display: none; position: fixed; top: 50%; left: 50%; background-color: transparent;">
        <asp:Image ID="imgLoading" runat="server" ImageUrl="~/images/Spin.gif" />
    </div>
    <div id="divMaskFrame" style="background-color: #000; display: none; left: 0px; position: absolute; top: 0px;">
    </div>

    <div class="warp shoppingarea">
        <main class="container-fluid">
            <div class="container  pb-5">
                <div class="page-content">
                    <!--階段1、報名/捐款清單-->
                    <div class="process-box">
                        <div class="process-item" id="DIV_Cart" runat="server">報名/捐款清單</div>
                        <div class="process-item" id="DIV_Data" runat="server">填寫資料/繳交方式</div>
                        <div class="process-item" id="DIV_Check" runat="server">確認結帳</div>
                        <div class="process-item" id="DIV_Success" runat="server">完成報名</div>
                    </div>

                    <div class="container mt-4 cart">
                        <div>
                            <asp:Label ID="LB_Note" runat="server" Text="" CssClass="mb-0 text-danger" Visible="true">*折扣金額：系統會先預設計算前導課程之折扣金額</asp:Label><br />
                            <asp:Label ID="LB_HNotice" runat="server" Text="" CssClass="mb-0 text-danger" Visible="false">*此為基本費用，歡迎自行調整金額</asp:Label>
                        </div>
                        <div class="flex flex-column flex-lg-row">

                            <!--報名/捐款清單區域  START-->
                            <asp:Panel ID="Panel_Cart" runat="server" Style="width: 100%">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                    <ContentTemplate>
                                        <div class="cart-left mr-lg-3">
                                            <asp:Label ID="LB_Navtab" runat="server" Text="" Visible="false"></asp:Label>
                                            <asp:Label ID="LB_HAreaID" runat="server" Text="" Visible="false"></asp:Label>

                                            <ul class="nav nav-tabs align-items-baseline cultural_bg" role="tablist">
                                                <%-- <li class="nav-item">
                                                    <asp:LinkButton ID="LBtn_Foundation" runat="server" class="nav-link cart-tab-title" OnClick="LBtn_Foundation_Click">
                                                <span class="hidden-xs-down">傳光<!--(尋光階~一階)--></span>
                                                    </asp:LinkButton>
                                                </li>--%>
                                                <li class="nav-item">
                                                    <asp:LinkButton ID="LBtn_Cultural" runat="server" class="nav-link cart-tab-title culture_btn" Enabled="false" OnClick="LBtn_Cultural_Click">
                                                 <span class="hidden-xs-down">玉成<!--(二~七階)--></span>
                                                    </asp:LinkButton>
                                                </li>
                                                <%--<li class="nav-item mb-0">
                                                    <span class="text-danger" style="font-size: 0.95rem;">&nbsp;&nbsp;&nbsp;*請按傳光或玉成進行結帳</span>
                                                </li>--%>
                                            </ul>


                                            <div class="cus-cart-table">
                                                <asp:Panel ID="Panel_Cultural" runat="server" CssClass="mb-0">
                                                    <div class="font-weight-normal pl-2" style="font-size: 0.95rem;">1.報名課程繳費</div>
                                                    <asp:Label ID="LB_NoCourseC" runat="server" Text="尚未加入要報名的課程~" CssClass="text-gray ml-3" Visible="false"></asp:Label>
                                                    <div class="table-responsive mt-2  table-mobile table-all" id="Div_Cultural" runat="server">

                                                        <table class="table table-hover table-bordered cart_table">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center" style="width: 5%">勾選</th>
                                                                    <%-- <th class="text-center" style="width: 5%">類型</th>--%>
                                                                    <th style="width: 18%">課程名稱</th>
                                                                    <th style="width: 15%">上課地點</th>
                                                                    <th class="text-left" style="width: 14%">是否參班</th>
                                                                    <th class="text-right" style="width: 8%">基本費用</th>
                                                                    <th class="text-right" style="width: 9%">課程費用</th>
                                                                    <th style="width: 10%">折扣碼</th>
                                                                    <th class="text-right" style="width: 8%">折扣金額</th>
                                                                    <th class="text-right" style="width: 6%">小計</th>
                                                                    <th class="text-center" style="width: 7%">執行</th>
                                                                </tr>
                                                            </thead>

                                                            <!--單一課程-->
                                                            <tbody>
                                                                <asp:SqlDataSource ID="SDS_Cultural" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                                <asp:Repeater ID="Rpt_Cultural" runat="server" DataSourceID="SDS_Cultural" OnItemDataBound="Rpt_Cultural_ItemDataBound">
                                                                    <ItemTemplate>

                                                                        <tr>
                                                                            <asp:Label ID="LB_ShoppingCartIDC" runat="server" Text='<%# Eval("ShoppingCartID") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_CTemplateIDC" runat="server" Text='<%# Eval("HCTemplateID") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_CourseNameC" runat="server" Text='<%# Eval("HCourseName") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_CourseIDC" runat="server" Text='<%# Eval("HCourseID") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_DateRangeC" runat="server" Text='<%# Eval("HDateRange") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_SCPrice" runat="server" Text='<%# Eval("SCPrice") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_HLDiscountC" runat="server" Text='<%# Eval("LDiscount") %>' Visible="false"></asp:Label><!--前導課程折扣金額-->
                                                                            <asp:Label ID="LB_HAttend" runat="server" Text='<%# Eval("HAttend") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_HExamContentIDC" runat="server" Text='<%# Eval("HExamContentID") %>' Visible="false"></asp:Label>

                                                                            <td class="text-center mobile-left" data-title="勾選繳費">
                                                                                <asp:CheckBox ID="CB_SelectC" runat="server" Checked='<%# Eval("HSelect") %>' OnCheckedChanged="CB_SelectC_CheckedChanged" AutoPostBack="true" /><%--CssClass="check" data-checkbox="icheckbox_flat-purple--%>
                                                                            </td>
                                                                            <%-- <td class="text-center" data-title="類型">
                                                                        <asp:Label ID="LB_HCPkgYN" runat="server" CssClass="title" Text="單一"></asp:Label>
                                                                    </td>--%>
                                                                            <td data-title="課程名稱">
                                                                                <asp:LinkButton ID="LBtn_HCourseNameC" runat="server" OnClick="LBtn_HCourseNameC_Click" Enabled="false"><%# Eval("HCourseName") %></asp:LinkButton>
                                                                            </td>

                                                                            <td data-title="上課地點">
                                                                                <!--選項僅該課程名稱的上課地點-->
                                                                                <asp:DropDownList ID="DDL_HOCPlaceC" runat="server" CssClass="form-control  js-example-basic-single" Style="width: 100%" AutoPostBack="true" OnSelectedIndexChanged="DDL_HOCPlaceC_SelectedIndexChanged">
                                                                                    <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                                <%--<asp:Label ID="LB_HOCPlaceC" runat="server" Text=""></asp:Label>--%>
                                                                            </td>

                                                                            <td data-title="是否參班">
                                                                                <asp:DropDownList ID="DDL_HAttendC" runat="server" OnSelectedIndexChanged="DDL_HAttendC_SelectedIndexChanged" AutoPostBack="true" class="form-control  js-example-basic-single" Style="width: 100%;">
                                                                                    <asp:ListItem Value="1">參班【一般】</asp:ListItem>
                                                                                    <asp:ListItem Value="2" Enabled="false">參班【學青(25歲以下在學青年)】</asp:ListItem>
                                                                                    <asp:ListItem Value="3" Enabled="false">參班【經濟困難(需經光團1號導師審核通過)】</asp:ListItem>
                                                                                    <asp:ListItem Value="4" Enabled="false">參班【護持出家師父及全心奉獻各修行組織團體之清修人士(請光團1號導師協助確認)】</asp:ListItem>
                                                                                    <asp:ListItem Value="5">純護持(非班員)</asp:ListItem>
                                                                                    <%--護持體系專業--%>
                                                                                    <asp:ListItem Value="6">參班兼護持</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td class="text-right" data-title="基本費用">
                                                                                <asp:Label ID="LB_HBCPointC" runat="server" Text='<%# Eval("CPrice") %>'></asp:Label>
                                                                            </td>
                                                                            <td class="text-right" data-title="課程費用">
                                                                                <asp:TextBox ID="TB_HPAmountC" runat="server" class="form-control pr-1 text-right" AutoComplete="off" placeholder="輸入金額" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Text='<%# Eval("CPrice") %>' OnTextChanged="TB_HPAmountC_TextChanged" AutoPostBack="true" Style="width: 100%;"></asp:TextBox>
                                                                            </td>
                                                                            <td data-title="折扣碼">
                                                                                <asp:TextBox ID="TB_HDCodeC" runat="server" class="form-control pl-1 text-left" AutoComplete="off" placeholder="輸入折扣碼" OnTextChanged="TB_HDCodeC_TextChanged" AutoPostBack="true" Style="width: 100%;"></asp:TextBox>
                                                                            </td>
                                                                            <td class="text-right" data-title="折扣金額">
                                                                                <asp:Label ID="LB_HDPointC" runat="server" Text="0" Visible="false"></asp:Label><!--折扣碼折扣金額-->
                                                                                <asp:Label ID="LB_HDiscountC" runat="server" Text="0"></asp:Label><!--總折扣金額(折扣碼金額+前導課程折扣金額)-->
                                                                            </td>
                                                                            <td class="text-right" data-title="小計">
                                                                                <asp:Label ID="LB_HSubTotalC" runat="server" Text='<%# Eval("CPrice") %>'></asp:Label><%--Text='<%# Eval("CPrice") %>'--%>
                                                                            </td>
                                                                            <td class="text-center" data-title="執行">
                                                                                <asp:LinkButton ID="LBtn_DelC" runat="server" CssClass="btn btn-outline-danger" OnClick="LBtn_DelC_Click" CommandArgument='<%# Eval("ShoppingCartID") %>'>刪除</asp:LinkButton>
                                                                            </td>
                                                                        </tr>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tbody>

                                                            <!--套裝課程-->
                                                            <%--<tbody class="d-none">
                                                        <asp:SqlDataSource ID="SDS_HCoursePackageC" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:Repeater ID="Rpt_HCoursePackageC" runat="server" DataSourceID="SDS_HCoursePackageC" OnItemDataBound="Rpt_HCoursePackageC_ItemDataBound">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <asp:Label ID="LB_CourseIDC" runat="server" Text="0" Visible="false"></asp:Label>
                                                                    <asp:Label ID="LB_HCPkgHID" runat="server" Text='<%# Eval("HCPkgHID") %>' Visible="false"></asp:Label>
                                                                    <asp:Label ID="LB_HLDiscountPkgC" runat="server" Text='<%# Eval("LDiscount") %>' Visible="false"></asp:Label><!--前導課程折扣金額-->
                                                                    <asp:Label ID="LB_HAttendPkgC" runat="server" Text='<%# Eval("HAttend") %>' Visible="false"></asp:Label>
                                                                    <td class="text-center mobile-left" data-title="勾選繳費">
                                                                        <asp:CheckBox ID="CB_SelectPkgC" runat="server" Checked='<%# Eval("HSelect") %>' OnCheckedChanged="CB_SelectPkgC_CheckedChanged" AutoPostBack="true" />
                                                                    </td>
                                                                    <td class="text-center" data-title="類型">
                                                                        <asp:Label ID="LB_HCPkgYN" runat="server" CssClass="title" Text="套裝"></asp:Label>
                                                                    </td>
                                                                    <td data-title="課程名稱">
                                                                        <asp:Label ID="LB_HCourseNamePkgC" runat="server" CssClass="title" Text='<%# Eval("HCPkgName") %>'></asp:Label>
                                                                    </td>

                                                                    <td data-title="上課地點" class="text-center">
                                                                        <asp:LinkButton ID="LBtn_HPlacePkgC" runat="server" CssClass="btn btn-outline-info" CommandArgument='<%# Eval("HCPkgHID") %>' OnClick="LBtn_HPlacePkgC_Click">選擇上課地點</asp:LinkButton>
                                                                    </td>

                                                                    <td data-title="是否參班">
                                                                        <asp:DropDownList ID="DDL_HAttendPkgC" runat="server" OnSelectedIndexChanged="DDL_HAttendPkgC_SelectedIndexChanged" AutoPostBack="true" class="form-control  js-example-basic-single" Style="width: 100%;">
                                                                            <asp:ListItem Value="1">參班【一般】</asp:ListItem>
                                                                            <asp:ListItem Value="5">純護持(非班員)</asp:ListItem>
                                                                            <asp:ListItem Value="6">參班兼護持</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td class="text-right" data-title="基本費用">
                                                                        <asp:Label ID="LB_HBCPrice" runat="server" Text='<%# Eval("PkgCPrice") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-right" data-title="課程費用">
                                                                        <asp:TextBox ID="TB_HPAmountPkgC" runat="server" class="form-control pr-1 text-right" AutoComplete="off" placeholder="輸入金額" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Text='<%# Eval("PkgCPrice") %>' OnTextChanged="TB_HPAmountPkgC_TextChanged" AutoPostBack="true" Style="width: 100%;"></asp:TextBox>

                                                                    </td>
                                                                    <td data-title="折扣碼">
                                                                        <asp:TextBox ID="TB_HDCodePkgC" runat="server" class="form-control pl-1 text-left" AutoComplete="off" placeholder="輸入折扣碼" OnTextChanged="TB_HDCodePkgC_TextChanged" AutoPostBack="true" Style="width: 100%;"></asp:TextBox>
                                                                    </td>
                                                                    <td class="text-right" data-title="折扣金額">
                                                                        <asp:Label ID="LB_HDPointPkgC" runat="server" Text="0" Visible="false"></asp:Label><!--折扣碼折扣金額-->
                                                                        <asp:Label ID="LB_HDiscountPkgC" runat="server" Text=""></asp:Label><!--總折扣金額(折扣碼金額+前導課程折扣金額)-->
                                                                    </td>
                                                                    <td class="text-right" data-title="小計">
                                                                        <asp:Label ID="LB_HSubTotalPkgC" runat="server" Text='<%# Eval("PkgCPrice") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center" data-title="執行">
                                                                        <asp:LinkButton ID="LBtn_DelPkgC" runat="server" CssClass="btn btn-outline-danger" OnClick="LBtn_DelPkgC_Click" CommandArgument='<%# Eval("HCPkgHID") %>'>刪除</asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>--%>
                                                        </table>
                                                    </div>







                                                </asp:Panel>



                                                <div class="custom_border mt-2"></div>


                                                <ul class="nav nav-tabs align-items-baseline mt-3 foundation_bg" role="tablist">
                                                    <li class="nav-item">
                                                        <asp:LinkButton ID="LBtn_Foundation" runat="server" class="nav-link cart-tab-title foundation_btn" Enabled="false" OnClick="LBtn_Foundation_Click">
                                                <span class="hidden-xs-down">傳光<!--(尋光階~一階)--></span>
                                                        </asp:LinkButton>
                                                    </li>

                                                </ul>






                                                <asp:Panel ID="Panel_Foundation" runat="server" CssClass="mb-2" Visible="true">



                                                    <div class="font-weight-normal pl-2 pt-2" style="font-size: 0.95rem;">1.報名課程繳費</div>

                                                    <asp:Label ID="LB_NoCourseF" runat="server" Text="尚未加入要報名的課程~" CssClass="text-gray ml-3" Visible="false"></asp:Label>


                                                    <div class="table-responsive mt-2  table-mobile table-all" id="Div_Foundation" runat="server">
                                                        <table class="table table-hover table-bordered cart_table">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center" style="width: 5%">勾選</th>
                                                                    <%-- <th style="width: 5%">類型</th>--%>
                                                                    <th style="width: 18%">課程名稱</th>
                                                                    <th style="width: 15%">上課地點</th>
                                                                    <th class="text-left" style="width: 14%">是否參班</th>
                                                                    <th class="text-right" style="width: 8%">基本費用</th>
                                                                    <th class="text-right" style="width: 9%">課程費用</th>
                                                                    <th style="width: 10%">折扣碼</th>
                                                                    <th class="text-right" style="width: 8%">折扣金額</th>
                                                                    <th class="text-right" style="width: 6%">小計</th>
                                                                    <th class="text-center" style="width: 7%">執行</th>
                                                                </tr>
                                                            </thead>

                                                            <!--單一課程-->
                                                            <tbody>
                                                                <asp:SqlDataSource ID="SDS_FoundationC" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                                <asp:Repeater ID="Rpt_FoundationC" runat="server" DataSourceID="SDS_FoundationC" OnItemDataBound="Rpt_FoundationC_ItemDataBound">
                                                                    <ItemTemplate>
                                                                        <tr>

                                                                            <asp:Label ID="LB_ShoppingCartIDFC" runat="server" Text='<%# Eval("ShoppingCartID") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_CTemplateIDFC" runat="server" Text='<%# Eval("HCTemplateID") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_CourseIDFC" runat="server" Text='<%# Eval("HCourseID") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_CourseNameFC" runat="server" Text='<%# Eval("HCourseName") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_DateRangeFC" runat="server" Text='<%# Eval("HDateRange") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_SFCPrice" runat="server" Text='<%# Eval("SFCPrice") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_HLDiscountFC" runat="server" Text='<%# Eval("LDiscount") %>' Visible="false"></asp:Label><!--前導課程折扣金額-->
                                                                            <asp:Label ID="LB_HAttend" runat="server" Text='<%# Eval("HAttend") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="LB_HExamContentIDFC" runat="server" Text='<%# Eval("HExamContentID") %>' Visible="false"></asp:Label>
                                                                            <td class="text-center mobile-left" data-title="勾選繳費">
                                                                                <asp:CheckBox ID="CB_SelectFC" runat="server" Checked='<%# Eval("HSelect") %>' OnCheckedChanged="CB_SelectFC_CheckedChanged" AutoPostBack="true" /><%--   CssClass="check" data-checkbox="icheckbox_flat-purple"--%>
                                                                            </td>
                                                                            <%-- <td data-title="類型">
                                                                        <asp:Label ID="LB_HCourseType" runat="server" CssClass="title" Text="單一"></asp:Label>
                                                                    </td>--%>
                                                                            <td data-title="課程名稱">
                                                                                <%--<asp:Label ID="LB_HCourseNameFC" runat="server" data-toggle="modal"
                                                                                    data-target="#subjectModal"
                                                                                    Style="cursor: pointer;" CssClass="title text-info" Text='<%# Eval("HCourseName") %>'></asp:Label>--%>
                                                                                <asp:LinkButton ID="LBtn_HCourseNameFC" runat="server" OnClick="LBtn_HCourseNameFC_Click" Enabled="false"><%# Eval("HCourseName") %></asp:LinkButton>
                                                                            </td>

                                                                            <td data-title="上課地點">
                                                                                <asp:DropDownList ID="DDL_HOCPlaceFC" runat="server" CssClass="form-control  js-example-basic-single" Style="width: 100%" AutoPostBack="true" OnSelectedIndexChanged="DDL_HOCPlaceFC_SelectedIndexChanged">
                                                                                    <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                                <%--<asp:Label ID="LB_HOCPlaceFC" runat="server" Text=""></asp:Label>--%>
                                                                            </td>
                                                                            <td data-title="是否參班">
                                                                                <asp:DropDownList ID="DDL_HAttendFC" runat="server" OnSelectedIndexChanged="DDL_HAttendFC_SelectedIndexChanged" AutoPostBack="true" class="form-control js-example-basic-single" Style="width: 100%;">
                                                                                    <asp:ListItem Value="1">參班【一般】</asp:ListItem>
                                                                                    <asp:ListItem Value="2" Enabled="false">參班【學青(25歲以下在學青年)】</asp:ListItem>
                                                                                    <asp:ListItem Value="3" Enabled="false">參班【經濟困難(需經光團1號導師審核通過)】</asp:ListItem>
                                                                                    <asp:ListItem Value="4" Enabled="false">參班【護持出家師父及全心奉獻各修行組織團體之清修人士(請光團1號導師協助確認)】</asp:ListItem>
                                                                                    <asp:ListItem Value="5">純護持(非班員)</asp:ListItem>
                                                                                    <%--護持體系專業--%>
                                                                                    <asp:ListItem Value="6">參班兼護持</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td class="text-right" data-title="基本費用">
                                                                                <asp:Label ID="LB_HBCPointFC" runat="server" Text='<%# Eval("FCPrice") %>'></asp:Label>
                                                                            </td>
                                                                            <td class="text-right" data-title="課程費用">
                                                                                <asp:TextBox ID="TB_HPAmountFC" runat="server" class="form-control pr-1 text-right" AutoComplete="off" placeholder="輸入金額" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Text='<%# Eval("FCPrice") %>' OnTextChanged="TB_HPAmountFC_TextChanged" AutoPostBack="true" Style="width: 100%;"></asp:TextBox>
                                                                            </td>
                                                                            <td data-title="折扣碼">
                                                                                <asp:TextBox ID="TB_HDCodeFC" runat="server" class="form-control pl-1 text-left" AutoComplete="off" placeholder="輸入折扣碼" OnTextChanged="TB_HDCodeFC_TextChanged" AutoPostBack="true" Style="width: 100%;"></asp:TextBox>
                                                                            </td>
                                                                            <td class="text-right" data-title="折扣金額">
                                                                                <asp:Label ID="LB_HDPointFC" runat="server" Text="0" Visible="false"></asp:Label><!--折扣碼折扣金額-->
                                                                                <asp:Label ID="LB_HDiscountFC" runat="server" Text=""></asp:Label><!--總折扣金額(折扣碼金額+前導課程折扣金額)-->
                                                                            </td>
                                                                            <td class="text-right" data-title="小計">
                                                                                <asp:Label ID="LB_HSubTotalFC" runat="server" Text='<%# Eval("FCPrice") %>'></asp:Label>
                                                                            </td>
                                                                            <td class="text-center" data-title="執行">
                                                                                <asp:LinkButton ID="LBtn_DelFC" runat="server" CssClass="btn btn-outline-danger" OnClick="LBtn_DelFC_Click" CommandArgument='<%# Eval("ShoppingCartID") %>'>刪除</asp:LinkButton>
                                                                            </td>
                                                                        </tr>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tbody>


                                                            <!--套裝課程-->
                                                            <%--<tbody class="d-none">
                                                        <asp:SqlDataSource ID="SDS_HCoursePackageFC" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                        <asp:Repeater ID="Rpt_HCoursePackageFC" runat="server" DataSourceID="SDS_HCoursePackageFC" OnItemDataBound="Rpt_HCoursePackageFC_ItemDataBound">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <asp:Label ID="LB_CourseIDFC" runat="server" Text="0" Visible="false"></asp:Label>
                                                                    <asp:Label ID="LB_HCPkgHIDFC" runat="server" Text='<%# Eval("HCPkgHID") %>' Visible="false"></asp:Label>
                                                                    <asp:Label ID="LB_HLDiscountPkgFC" runat="server" Text='<%# Eval("LDiscount") %>' Visible="false"></asp:Label><!--前導課程折扣金額-->
                                                                    <asp:Label ID="LB_HAttendPkgFC" runat="server" Text='<%# Eval("HAttend") %>' Visible="false"></asp:Label>
                                                                    <td class="text-center mobile-left" data-title="勾選繳費">
                                                                        <asp:CheckBox ID="CB_SelectPkgFC" runat="server" Checked='<%# Eval("HSelect") %>' OnCheckedChanged="CB_SelectPkgFC_CheckedChanged" AutoPostBack="true" />
                                                                    </td>
                                                                    <td class="text-center" data-title="類型">
                                                                        <asp:Label ID="LB_HCPkgYN" runat="server" CssClass="title" Text="套裝"></asp:Label>
                                                                    </td>
                                                                    <td data-title="課程名稱">
                                                                        <asp:Label ID="LB_HCourseNamePkgFC" runat="server" CssClass="title" Text='<%# Eval("HCPkgName") %>'></asp:Label>
                                                                    </td>

                                                                    <td data-title="上課地點" class="text-center">
                                                                        <asp:LinkButton ID="LBtn_HPlacePkgFC" runat="server" CssClass="btn btn-outline-info" CommandArgument='<%# Eval("HCPkgHID") %>' OnClick="LBtn_HPlacePkgFC_Click">選擇上課地點</asp:LinkButton>
                                                                    </td>

                                                                    <td data-title="是否參班">
                                                                        <asp:DropDownList ID="DDL_HAttendPkgFC" runat="server" OnSelectedIndexChanged="DDL_HAttendPkgFC_SelectedIndexChanged" AutoPostBack="true" class="form-control  js-example-basic-single" Style="width: 100%;">
                                                                            <asp:ListItem Value="1">參班【一般】</asp:ListItem>
                                                                            <asp:ListItem Value="5">純護持(非班員)</asp:ListItem>
                                                                            <asp:ListItem Value="6">參班兼護持</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td class="text-right" data-title="基本費用">
                                                                        <asp:Label ID="LB_HBCPriceFC" runat="server" Text='<%# Eval("PkgCPrice") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-right" data-title="課程費用">
                                                                        <asp:TextBox ID="TB_HPAmountPkgFC" runat="server" class="form-control pr-1 text-right" AutoComplete="off" placeholder="輸入金額" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Text='<%# Eval("PkgCPrice") %>' OnTextChanged="TB_HPAmountPkgFC_TextChanged" AutoPostBack="true" Style="width: 100%;"></asp:TextBox>

                                                                    </td>
                                                                    <td data-title="折扣碼">
                                                                        <asp:TextBox ID="TB_HDCodePkgFC" runat="server" class="form-control pl-1 text-left" AutoComplete="off" placeholder="輸入折扣碼" OnTextChanged="TB_HDCodePkgFC_TextChanged" AutoPostBack="true" Style="width: 100%;"></asp:TextBox>
                                                                    </td>
                                                                    <td class="text-right" data-title="折扣金額">
                                                                        <asp:Label ID="LB_HDPointPkgFC" runat="server" Text="0" Visible="false"></asp:Label><!--折扣碼折扣金額-->
                                                                        <asp:Label ID="LB_HDiscountPkgFC" runat="server" Text=""></asp:Label><!--總折扣金額(折扣碼金額+前導課程折扣金額)-->
                                                                    </td>
                                                                    <td class="text-right" data-title="小計">
                                                                        <asp:Label ID="LB_HSubTotalPkgFC" runat="server" Text='<%# Eval("PkgCPrice") %>'></asp:Label>
                                                                    </td>
                                                                    <td class="text-center" data-title="執行">
                                                                        <asp:LinkButton ID="LBtn_DelPkgFC" runat="server" CssClass="btn btn-outline-danger" OnClick="LBtn_DelPkgFC_Click" CommandArgument='<%# Eval("HCPkgHID") %>'>刪除</asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>--%>
                                                        </table>
                                                    </div>




                                                    <asp:Panel ID="Panel_Donation" runat="server">
                                                        <div class="font-weight-normal pl-2 pt-2" style="font-size: 0.95rem;">2. 捐款護持班會</div>

                                                        <asp:Label ID="LB_NoCourseD" runat="server" Text="尚未加入要捐款護持的班會~" CssClass="text-gray ml-3" Visible="false"></asp:Label>

                                                        <div class="table-responsive mt-2  table-mobile table-all" id="Div_Donation" runat="server">
                                                            <table class="table table-hover table-bordered cart_table">
                                                                <thead>
                                                                    <tr>
                                                                        <th class="text-center" style="width: 5%">勾選</th>
                                                                        <th style="width: 71%">課程名稱</th>
                                                                        <th class="text-right" style="width: 10%">捐款金額</th>
                                                                        <th class="text-right" style="width: 7%">小計</th>
                                                                        <th class="text-center" style="width: 7%">執行</th>
                                                                    </tr>
                                                                </thead>

                                                                <tbody>
                                                                    <asp:SqlDataSource ID="SDS_FoundationD" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                                    <asp:Repeater ID="Rpt_FoundationD" runat="server" DataSourceID="SDS_FoundationD" OnItemDataBound="Rpt_FoundationD_ItemDataBound">
                                                                        <ItemTemplate>

                                                                            <tr>

                                                                                <asp:Label ID="LB_ShoppingCartIDFD" runat="server" CssClass="title" Text='<%# Eval("ShoppingCartID") %>' Visible="false"></asp:Label>
                                                                                <asp:Label ID="LB_CTemplateIDFD" runat="server" Text='<%# Eval("HCTemplateID") %>' Visible="false"></asp:Label>
                                                                                <asp:Label ID="LB_CourseNameFD" runat="server" Text='<%# Eval("HCourseName") %>' Visible="false"></asp:Label>
                                                                                <asp:Label ID="LB_CourseIDFD" runat="server" Text='<%# Eval("HCourseID") %>' Visible="false"></asp:Label>
                                                                                <asp:Label ID="LB_DateRangeFD" runat="server" Text='<%# Eval("HDateRange") %>' Visible="false"></asp:Label>

                                                                                <td class="text-center mobile-left" data-title="勾選捐款">
                                                                                    <asp:CheckBox ID="CB_SelectFD" runat="server" Checked='<%# Eval("HSelect") %>' OnCheckedChanged="CB_SelectFD_CheckedChanged" AutoPostBack="true" />
                                                                                </td>
                                                                                <%--CssClass="check" data-checkbox="icheckbox_flat-purple"--%>
                                                                                <td data-title="課程名稱">
                                                                                    <asp:Label ID="LB_HCourseNameFD" runat="server" CssClass="title" Text='<%# Eval("HCourseName") %>'></asp:Label>
                                                                                </td>

                                                                                <asp:Label ID="LB_HBCPointFD" runat="server" Text="100" Visible="false"></asp:Label>

                                                                                <td class="text-right" data-title="捐款金額">
                                                                                    <asp:TextBox ID="TB_PAmountFD" runat="server" class="form-control pr-1 text-right" AutoComplete="off" placeholder="輸入金額" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" Text="100" OnTextChanged="TB_HCourseDonateFD_TextChanged" AutoPostBack="true" Style="width: 100%;"></asp:TextBox>
                                                                                </td>
                                                                                <td class="text-right" data-title="小計">
                                                                                    <asp:Label ID="LB_SubTotalFD" runat="server" Text="100" Visible="true"></asp:Label>
                                                                                </td>
                                                                                <td class="text-center" data-title="執行">
                                                                                    <asp:LinkButton ID="LBtn_DelFD" runat="server" CssClass="btn btn-outline-danger" OnClick="LBtn_DelFD_Click" CommandArgument='<%# Eval("ShoppingCartID") %>'>刪除</asp:LinkButton>
                                                                                </td>
                                                                            </tr>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                                </tbody>
                                                            </table>
                                                        </div>


                                                        <div class="flex flex-row d-none">
                                                            <div class="w-100 flex flex-row align-items-center ">
                                                                <div class="carttable  carttable_head flex flex-column justify-content-between w-100 flex-lg-row justify-content-lg-between align-items-lg-center">
                                                                    <div class="flex flex-column w3 text-center">
                                                                        勾選
                                                            <asp:CheckBox ID="CheckBox5" runat="server" CssClass="check d-none" />
                                                                    </div>
                                                                    <div class="flex flex-column w67">
                                                                        課程名稱
                                                                    </div>
                                                                    <div class="flex flex-row flex-lg-column w14">
                                                                        <%--上課地點--%>
                                                                    </div>

                                                                    <div class="flex flex-row flex-lg-column w10 text-right">
                                                                        捐款金額
                                                                    </div>

                                                                    <div class="flex flex-row flex-lg-column text-center w6 text-center">
                                                                        執行
                                                                    </div>
                                                                </div>

                                                            </div>
                                                        </div>


                                                    </asp:Panel>

                                                </asp:Panel>

                                            </div>



                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                    </Triggers>
                                </asp:UpdatePanel>
                            </asp:Panel>
                            <!--報名/捐款清單區域  END-->

                            <!--填寫資料區域  START-->
                            <asp:Panel ID="Panel_FillData" runat="server" Visible="false" Style="width: 100%;">

                                <div class="cart-left mr-lg-3">

                                    <h4 class="heading" data-testid="paymentTitleText">● 課程內容</h4>

                                    <!--220926-調整後版面 START-->
                                    <div class="list-wrap row table-responsive m-auto">
                                        <table class="table table-bordered table-striped courselist">
                                            <%-- id="demo-foo-addrow"--%>
                                            <thead>
                                                <tr class="font-weight-bold">
                                                    <td class="text-center" style="width: 5%">No</td>
                                                    <td style="width: 58%">課程名稱</td>
                                                    <td class="text-center" style="width: 15%">上課地點</td>
                                                    <td class="text-center" style="width: 12%">參班身分</td>
                                                    <td class="text-center" style="width: 10%">報名資訊</td>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:SqlDataSource ID="SDS_FillData" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand="" ProviderName="System.Data.SqlClient"></asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_FillData" runat="server" DataSourceID="SDS_FillData" OnItemDataBound="Rpt_FillData_ItemDataBound">
                                                    <ItemTemplate>
                                                        <asp:Label ID="LB_ShoppingCartHID" runat="server" Text='<%# Eval("ShoppingCartID") %>' Visible="false"></asp:Label>
                                                        <asp:Label ID="LB_HCourseID" runat="server" Text='<%# Eval("HCourseID") %>' Visible="false"></asp:Label>
                                                        <asp:Label ID="LB_HRollcallYN" runat="server" Text='<%# Eval("HRollcallYN") %>' Visible="false"></asp:Label>
                                                        <tr>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_No" runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label>
                                                            </td>
                                                            <td class="text-left" style="white-space: normal; word-break: break-all;">
                                                                <asp:Label ID="LB_HCourseName" runat="server" Text='<%# Eval("HCourseName") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_HPlaceName" runat="server" Text='<%# Eval("HPlaceName") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_HAttend" runat="server" Text='<%# Eval("HAttend") %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:LinkButton ID="LBtn_FillIn" runat="server" CausesValidation="false" Visible="true" CssClass="button button-green" CommandArgument='<%# Eval("ShoppingCartID") %>' OnClick="LBtn_FillIn_Click"><span class="ti-pencil mr-2"></span>填寫</asp:LinkButton>
                                                            </td>
                                                        </tr>


                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>

                                    <!--220926-調整後版面 END-->


                                    <asp:Label ID="LB_HOrderGroup" runat="server" Text="" Visible="false"></asp:Label><!--訂單代碼-->
                                    <asp:Label ID="LB_HMerchantTradeNo" runat="server" Text="" Visible="false"></asp:Label><!--傳給綠界的廠商交易編號-->

                                    <asp:Panel ID="Panel_PayMethod" runat="server">
                                        <h4 class="heading mt-3" data-testid="paymentTitleText">● 繳交方式</h4>

                                        <div class="row mx-0 payment">
                                            <div class="col-5 area">
                                                <div class="form-group">
                                                    <asp:DropDownList ID="DDL_HPayMethod" runat="server" class="form-control" OnTextChanged="DDL_HPayMethod_TextChanged" AutoPostBack="true">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                        <asp:ListItem Value="1">線上刷卡</asp:ListItem>
                                                        <asp:ListItem Value="2">線上ATM</asp:ListItem>
                                                        <asp:ListItem Value="3">超商</asp:ListItem>
                                                        <asp:ListItem Value="4">ATM櫃員機</asp:ListItem>
                                                        <asp:ListItem Value="9" Style="display: none">其它</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-7">
                                                <div class="img-fluid">
                                                    <span class="credit-card-issuer">
                                                        <img alt="VISA" src="data:image/png;base64,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"><img alt="MasterCard" src="data:image/png;base64,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"><img alt="JCB" src="data:image/png;base64,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"><img alt="UnionPay" src="data:image/png;base64,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"></span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mx-0">
                                            <p class="mt-2 mb-0 pl-2">確認結帳後將會連結至第三方金流服務進行付款，您所有的交易資訊將獲得安全保護。</p>
                                            <p class="mt-2 mb-0 pl-2">超商條碼由於四大超商系統的關係, 訂單狀態會延遲3~4日更新。造成不便, 還請見諒。</p>

                                        </div>



                                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                            <ContentTemplate>
                                                <asp:Panel ID="Panel_Invoice" runat="server" CssClass="mt-4" Visible="true">
                                                    <h4 class="heading mb-0" data-testid="paymentTitleText">● 發票/收據選項</h4>
                                                    <div class="row mx-0">
                                                        <p class="mt-2 mb-2 pl-2">預設為本人姓名及身分證資料，若要開立統編，請選擇公司戶。</p>
                                                    </div>

                                                    <div class="row mx-0 payment">
                                                        <div class="col-md-2 sm-6 area">
                                                            <div class="form-group">
                                                                <label for="">發票/收據類型</label>
                                                                <asp:DropDownList ID="DDL_InvoiceType" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DDL_InvoiceType_SelectedIndexChanged" Style="width: 100%">
                                                                    <asp:ListItem Value="1" Selected>個人戶</asp:ListItem>
                                                                    <asp:ListItem Value="2">公司戶</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <%--<asp:Panel ID="Panel_ICompany" runat="server" CssClass="d-flex" Visible="false">--%>
                                                        <div class="col-4 sm-6 area" id="Div_TaxID" runat="server" visible="false">
                                                            <div class="form-group">
                                                                <label for="">統一編號</label>
                                                                <asp:TextBox ID="TB_TaxID" runat="server" CssClass="form-control pl-2" Placeholder="請輸入統一編號" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;" MaxLength="8"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-6 sm-12 area" id="Div_InvoiceTitle" runat="server" visible="false">
                                                            <div class="form-group">
                                                                <label for="">發票/收據抬頭</label>
                                                                <asp:TextBox ID="TB_InvoiceTitle" runat="server" CssClass="form-control pl-2" Placeholder="請輸入發票/收據抬頭"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <!--EA20240125_上傳國稅局-->
                                                        <div class="col-2 sm-6 area updirs" id="Div_HUploadIRS" runat="server" visible="true">
                                                            <div class="form-group">
                                                                <label for="">是否上傳國稅局</label>
                                                                <div class="form-group">
                                                                    <asp:DropDownList ID="DDL_HUploadIRS" runat="server" class="form-control" Style="width: 100%" OnSelectedIndexChanged="DDL_HUploadIRS_SelectedIndexChanged" AutoPostBack="true">
                                                                        <asp:ListItem Value="99">請選擇</asp:ListItem>
                                                                        <asp:ListItem Value="0">否</asp:ListItem>
                                                                        <asp:ListItem Value="1">是</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>

                                                                <div class="form-group cb_outer d-none">
                                                                    <asp:CheckBox ID="CB_HUploadIRS" runat="server" Text="本人願意將捐款資料上傳國稅局" OnCheckedChanged="CB_HUploadIRS_CheckedChanged" AutoPostBack="true" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-3 sm-6 area" id="Div_HPersonID" runat="server" visible="false">
                                                            <div class="form-group">
                                                                <label for="">身分證字號(台灣)</label>
                                                                <asp:Label ID="LB_NoticePersonID" runat="server" Text="" CssClass="text-danger" Style="font-size: 14px" Visible="false"></asp:Label>
                                                                <div class="form-group">
                                                                    <asp:TextBox ID="TB_HPersonID" runat="server" class="form-control" placeholder="" AutoComplete="off" MaxLength="10" Style="width: 100%; padding: 4px;" AutoPostBack="true" OnTextChanged="TB_HPersonID_TextChanged"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <%--</asp:Panel>--%>
                                                    </div>
                                                </asp:Panel>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="DDL_InvoiceType" EventName="SelectedIndexChanged" />
                                                <asp:AsyncPostBackTrigger ControlID="CB_HUploadIRS" EventName="CheckedChanged" />
                                                <asp:AsyncPostBackTrigger ControlID="TB_HPersonID" EventName="TextChanged" />
                                            </Triggers>

                                        </asp:UpdatePanel>
                                    </asp:Panel>

                                    <%--<div class="member-section-title mt-4">繳費須知</div>--%>

                                    <h4 class="heading mt-3" data-testid="paymentTitleText">● 報名須知</h4>


                                    <div class="row mx-0">
                                        <div class="col-12 area">
                                            <div class="form-group notify-section mb-2">
                                                <%--<p>● 幫他人報名</p>
												<ul class="list-none">
													<li>1. 幫他人報名後，請聯繫本人已幫他加入報名/捐款清單裡，再請同修前往報名/捐款清單進行結帳。</li>
													
												</ul>--%>
                                                <h6 class="font-weight-bold mb-0">【各繳交方式總金額上限】</h6>
                                                <p class="font-weight-normal mb-0">&nbsp;&nbsp;&nbsp;線上刷卡(信用卡)：199,999元</p>
                                                <p class="font-weight-normal mb-0">&nbsp;&nbsp;&nbsp;線上ATM(網路ATM)：49,999元</p>
                                                <p class="font-weight-normal mb-0">&nbsp;&nbsp;&nbsp;ATM櫃員機：49,999元</p>
                                                <p class="font-weight-normal mb-0">&nbsp;&nbsp;&nbsp;超商：20,000元</p>
                                                <p></p>
                                                <span class="fa fa-bell" style="color: #ffc107;"></span>
                                                <h6 class="font-weight-bold mb-0 d-inline-block ml-2" style="text-decoration: underline">選擇課程，請參閱課程公告，確認參班資格，如參班身分不符合，退款規則同下</h6>
                                                <h6 class="font-weight-bold mt-2 mb-0">【退款規則】</h6>
                                                <p class="font-weight-normal mb-0">&nbsp;&nbsp;&nbsp;為維護您的個人權益，請詳閱以下退款規則：</p>
                                                <p class="font-weight-normal mb-0 text-indent">&nbsp;&nbsp;&nbsp;1. 線上刷卡：繳費完成7天內開課前申請退費者，全額退刷，不扣手續費。</p>
                                                <p class="font-weight-normal mb-0 text-indent">
                                                    &nbsp;&nbsp;&nbsp;2. 線上ATM、超商：繳費完成7天內開課前申請退費者，須扣手續費每筆56元。(為綠界金流手續費26元和退款銀行匯款30元)
                                                </p>
                                                <p class="font-weight-normal mb-0 text-indent">
                                                    &nbsp;&nbsp;&nbsp;3. 若您的訂單繳交方式使用非信用卡結帳，請提供完整銀行名稱、銀行代碼、銀行帳號與戶名資訊，退款總金額將退款至您提供的銀行資料。
                                                </p>
                                                <p class="font-weight-normal mb-0 text-indent">
                                                    &nbsp;&nbsp;&nbsp;4. 因第三方支付平台於每日20:15-20:30進行關帳作業，繳交方式為信用卡者，此時段將暫停使用取消課程功能。
                                                </p>
                                                <p class="font-weight-normal mb-0 text-indent">
                                                    &nbsp;&nbsp;&nbsp;5. 一張訂單僅限退款一次。
                                                </p>
                                                <p class="text-danger font-weight-normal mb-0 d-none">*課程費用為本人報名課程費用，請勿輸入幫他人報名之課程費用。</p>
                                            </div>
                                        </div>
                                        <div class="col-12 text-right">
                                            <div class="form-group">
                                                <div>
                                                    <asp:CheckBox ID="CB_Agree" runat="server" />
                                                    <label>我已詳細閱讀並同意以上須知</label>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                </div>




                            </asp:Panel>
                            <!--填寫資料區域  END-->


                            <!--確認結帳區域  START-->
                            <%--<asp:Panel ID="Panel_Check" runat="server" Visible="false" Style="width: 100%">
								
							</asp:Panel>--%>
                            <!--確認結帳區域  END-->



                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <div class="sidecart mt-5 mt-lg-0">
                                        <h4 class="font-weight-bold mb-0 border-bottom">我的訂單</h4>
                                        <div class="d-flex justify-content-between pt-2 ">
                                            <div>總金額(<asp:Label ID="LB_Num" runat="server" Text="0"></asp:Label>筆)</div>
                                            <div class="text-right">
                                                NT$
												<asp:Label ID="LB_Total" runat="server" Text="0"></asp:Label>
                                            </div>
                                        </div>

                                        <!--剩餘點數-->
                                        <div class="d-flex justify-content-between pt-2 " style="font-size: 0.9rem;" id="DIV_RemainPoint" runat="server" visible="false">
                                            <div>剩餘金額<asp:Label ID="LB_RemainPoints" runat="server" Text="0" Visible="false"></asp:Label></div>
                                            <div class="text-right">
                                                NT$
												<asp:Label ID="LB_Remain" runat="server" Text="0"></asp:Label>
                                            </div>
                                        </div>

                                        <!--補繳金額-->
                                        <div class="d-flex justify-content-between pt-2 font-weight-bold " id="DIV_Difference" runat="server" visible="false">
                                            <div>尚補差額</div>
                                            <div class="text-right">
                                                NT$
												<asp:Label ID="LB_Difference" runat="server" Text="0"></asp:Label>
                                            </div>
                                        </div>

                                        <div class="btn-part mt-2">
                                            <asp:Button ID="Btn_Back" runat="server" class="button button-gray text-center mb-2" Text="上一步" Style="width: 100%" OnClick="Btn_Back_Click" Visible="false"></asp:Button>

                                            <asp:Button ID="Btn_FillData" runat="server" class="button button-green text-center" Text="填寫資料/繳交方式" Style="width: 100%" OnClick="Btn_FillData_Click"></asp:Button>

                                            <asp:Button ID="Btn_CheckOut" runat="server" class="button button-green text-center ml-0" Text="確認結帳" Style="width: 100%" OnClick="Btn_CheckOut_Click" OnClientClick="ShowProgressBar();" Visible="false"></asp:Button>

                                            <%--<asp:Button ID="Btn_Next" runat="server" class="button button-green text-center" Text="下一步" Style="width: 100%; margin-left: 0;" OnClick="Btn_Next_Click" Visible="false"></asp:Button>--%>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="Btn_Back" />
                                    <asp:PostBackTrigger ControlID="Btn_FillData" />
                                    <asp:PostBackTrigger ControlID="Btn_CheckOut" />
                                    <%--<asp:PostBackTrigger ControlID="Btn_Next" />--%>
                                    <asp:AsyncPostBackTrigger ControlID="DDL_HPayMethod" EventName="TextChanged" />

                                </Triggers>

                            </asp:UpdatePanel>



                        </div>
                    </div>

                </div>
            </div>
        </main>
    </div>





    <!-- Modal FillIn填寫資料 START-->
    <div class="modal fade shoppingarea" id="Edit" tabindex="-1" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" role="document">
            <div class="modal-content" style="width: 100%;">

                <asp:Label ID="LB_CourseID" runat="server" Text="" Visible="false"></asp:Label>
                <asp:Label ID="LB_ShoppingCartID" runat="server" Text="" Visible="false"></asp:Label>
                <asp:Label ID="LB_SystemID" runat="server" Text="" Visible="false"></asp:Label>
                <asp:Label ID="LB_Type" runat="server" Text="" Visible="false"></asp:Label>
                <asp:Label ID="LB_CTemplateID" runat="server" Text="" Visible="false"></asp:Label>
                <asp:Label ID="LB_MDateRange" runat="server" Text="" Visible="false"></asp:Label>
                <asp:Label ID="LB_MPMethod" runat="server" Text="" Visible="false"></asp:Label>

                <div class="modal-header pt-2 pb-2">
                    <h5 class="modal-title font-weight-bold mb-0" id="exampleModalLongTitle">
                        <span class="text-info">
                            <asp:Label ID="LB_MCourseName" runat="server" Text=""></asp:Label><!--課程名稱-->
                            <asp:Label ID="LB_HPlaceName" runat="server" Text=""></asp:Label><!--上課地點-->
                        </span>
                        -報名資料填寫</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body" style="width: 100%;">

                    <div id="DIV_Support" runat="Server" visible="true">
                        <div class="member-section-title">課程護持</div>
                        <div class="row mx-0">
                            <div class="col-md-4 area">
                                <div class="form-group">
                                    <asp:SqlDataSource ID="SDS_HCGuide" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand=""></asp:SqlDataSource>
                                    <!--AppendDataBoundItems="true" DataSourceID="SDS_Memeber" DataTextField="MemberName" DataValueField="HID"-->
                                    <asp:DropDownList ID="DDL_HCGuide" runat="server" class="js-example-basic-single form-control" AppendDataBoundItems="true" DataSourceID="SDS_HCGuide" DataTextField="HCGuideItem" DataValueField="HMemberID" Stle="width:100% !important;">
                                        <asp:ListItem Value="0">請選擇護持者</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                        </div>
                    </div>



                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div class="member-section-title">體系專業護持資訊</div>
                            <h6 class="text-danger font-weight-normal mt-1 mb-2" style="font-size: 14px">* 填寫完體系專業護持資訊要記得<span class="font-weight-bold">按綠色的加號</span>，才會建立成功哦~!</h6>
                            <div class="row mx-0">
                                <div class="col-md-12 table-responsive table-mobile table-all">
                                    <table class="table table-striped support_table" style="width: 100%">
                                        <thead>
                                            <tr>
                                                <th style="width: 15%">體系專業護持組別</th>
                                                <th style="width: 15%">職稱</th>
                                                <th style="width: 8%; text-align: center">同上課日期</th>
                                                <th style="width: 34%">護持日期</th>
                                                <th style="width: 23%">護持時間</th>
                                                <th style="width: 5%">執行</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Label ID="LB_HDateRange_Add" runat="server" Text="" Visible="false" CssClass="form-control"></asp:Label>

                                            <tr>

                                                <td data-title="護持組別">
                                                    <asp:SqlDataSource ID="SDS_HGroupName" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand="">
                                                      
                                                    </asp:SqlDataSource>
                                                    <asp:DropDownList ID="DDL_HGroupName_Add" runat="server" class="form-control js-example-basic-single" DataSourceID="SDS_HGroupName" DataTextField="HGroupName" DataValueField="HGroupID" AppendDataBoundItems="true" OnSelectedIndexChanged="DDL_HGroupName_Add_SelectedIndexChanged" AutoPostBack="true" Style="width: 100%">
                                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td data-title="職稱">
                                                    <asp:DropDownList ID="DDL_HTask_Add" runat="server" class="form-control" Style="width: 100%" Enabled="false">
                                                        
                                                    </asp:DropDownList>
                                                </td>
                                                <td class="text-center" data-title="同上課日期">
                                                    <asp:CheckBox ID="CB_HSameAsCourse_Add" runat="server" CssClass="check" OnCheckedChanged="CB_HSameAsCourse_Add_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td data-title="護持日期">
                                                    <asp:TextBox ID="TB_HGDay_Add" runat="server" CssClass="datemultipicker form-control disabledate" Placeholder="可多選多個日期"></asp:TextBox>
                                                </td>
                                                <td data-title="護持時間" class="d-flex align-items-center">
                                                    <asp:TextBox ID="TB_GSTime_Add" runat="server" class="form-control pl-1 timepicker" AutoComplete="off" placeholder="開始時間"></asp:TextBox>&nbsp;~&nbsp;
                                                    <asp:TextBox ID="TB_GETime_Add" runat="server" class="form-control pl-1 timepicker" AutoComplete="off" placeholder="結束時間"></asp:TextBox>
                                                </td>
                                                <td data-title="執行">
                                                    <asp:LinkButton ID="LBtn_HTeam_Add" runat="server" class="btn btn-sm btn-outline-success" OnClick="LBtn_HTeam_Add_Click" CausesValidation="false"><i class="fa fa-plus text-success"></i></asp:LinkButton>
                                                </td>
                                            </tr>
                                            <asp:Repeater ID="Rpt_Support" runat="server">
                                                <ItemTemplate>
                                                  
                                                    <tr>

                                                        <td data-title="護持組別">
                                                            <asp:DropDownList ID="DDL_HGroupName" runat="server" class="form-control" DataSourceID="SDS_HGroupName" DataTextField="HGroupName" DataValueField="HGroupID" AppendDataBoundItems="true" Enabled="true" Visible="false">
                                                                <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="LB_HGroupName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HGroupName") %>' Visible="true" CssClass="form-control" Style="background-color: #e9ecef; line-height: 30px; padding-left: 7px;"></asp:Label>
                                                            <asp:Label ID="LB_HGroupID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HGroupID") %>' Visible="false"></asp:Label>
                                                        </td>
                                                        <td data-title="職稱">
                                                            <asp:DropDownList ID="DDL_HTask" runat="server" class="form-control js-example-basic-single" Style="width: 100%" Enabled="false" AppendDataBoundItems="true" Visible="false">
                                                            </asp:DropDownList>
                                                            <asp:Label ID="LB_HTask" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HTask") %>' Visible="true" CssClass="form-control" Style="background-color: #e9ecef; line-height: 30px; padding-left: 7px;"></asp:Label>
                                                            <asp:Label ID="LB_HTaskID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HTaskID") %>' Visible="false"></asp:Label>
                                                        </td>
                                                        <td data-title="同上課日期" class="text-center">
                                                            <asp:CheckBox ID="CB_HSameAsCourse" runat="server" CssClass="check" Checked='<%# DataBinder.Eval(Container.DataItem, "HSameAsCourse") %>' Visible="true" Enabled="false" />
                                                        </td>
                                                        <td data-title="護持日期">
                                                            <asp:TextBox ID="TB_HGDay" runat="server" CssClass="form-control" Text='<%# DataBinder.Eval(Container.DataItem, "HGDay") %>' Enabled="false"></asp:TextBox>
                                                        </td>
                                                        <td data-title="護持時間" class="d-flex align-items-center">
                                                            <asp:TextBox ID="TB_GSTime" runat="server" class="form-control pl-1 timepicker" AutoComplete="off" placeholder="開始時間" Text='<%# DataBinder.Eval(Container.DataItem, "HGSTime") %>' Enabled="false"></asp:TextBox>&nbsp;~&nbsp;
                                                             <asp:TextBox ID="TB_GETime" runat="server" class="form-control pl-1 timepicker" AutoComplete="off" placeholder="結束時間" Text='<%# DataBinder.Eval(Container.DataItem, "HGETime") %>' Enabled="false"></asp:TextBox>
                                                        </td>
                                                        <td style="display: none">
                                                            <asp:Label ID="LB_Add" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Add") %>' Visible="false"></asp:Label>
                                                        </td>
                                                        <td style="display: none">
                                                            <asp:Label ID="LB_HID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HID") %>' Visible="false"></asp:Label>
                                                        </td>
                                                        <td style="display: none">
                                                            <asp:Label ID="LB_Update" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Update") %>' Visible="false"></asp:Label>
                                                        </td>
                                                        <td data-title="執行">
                                                            <asp:LinkButton ID="LBtn_Del" runat="server" CausesValidation="false" class="btn btn-sm btn-outline-danger" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_Del_Click"><i class="fa fa-trash text-danger"></i></asp:LinkButton><!--刪除-->
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>

                                    </table>
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="DDL_HGroupName_Add" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>



                    <div>
                     
                        <div class="member-section-title">幫他人報名 (非必填) </div>
                        <div class="row mx-0">

                            <h6 class="text-danger font-weight-normal mt-1 mb-2" style="font-size: 14px">*此功能僅幫他人加入報名/捐款清單，<span class="font-weight-bold">【無法幫他人繳費】</span>。須由本人自己登入完成結帳才算報名成功。</h6>
                            <div class="col-md-12 table-responsive mb-2">

                                <!--下拉選單(多選) START-->
                                <asp:SqlDataSource ID="SDS_HOtherMember" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand=""></asp:SqlDataSource>

                                <asp:ListBox ID="LBox_HOtherMember" runat="server" class="form-control select2-multiple" DataSourceID="SDS_HOtherMember" AppendDataBoundItems="true" DataTextField="HOther" DataValueField="HID" Style="width: 100%; height: 32px !important;" SelectionMode="Multiple"></asp:ListBox>
                                <!--下拉選單(多選) END-->

                                <!--隱藏--改用多選的下拉選單-->
                                <%--<table class="table table-striped d-none">
                                    <thead>
                                        <tr>
                                            <th style="width: 5%">執行</th>
                                            <th style="width: 95%">學員姓名</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:LinkButton ID="LBtn_OtherBooking" runat="server" class="btn btn-sm btn-outline-success" OnClick="LBtn_OtherBooking_Click" CausesValidation="false"><i class="fa fa-plus text-success"></i></asp:LinkButton>
                                            </td>
                                        </tr>
                                        <asp:Repeater ID="Rpt_Other" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <asp:LinkButton ID="LBtn_OtherDel" runat="server" CausesValidation="false" class="btn btn-sm btn-outline-danger" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_OtherDel_Click"><i class="fa fa-trash text-danger"></i></asp:LinkButton><!--刪除-->
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="DDL_HOtherMember" runat="server" class="form-control" DataSourceID="SDS_HOtherMember" DataTextField="HOther" DataValueField="HID" AppendDataBoundItems="true" Enabled="false">
                                                        </asp:DropDownList>
                                                        <asp:Label ID="LB_HOtherMemberID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HOtherMember") %>' Visible="false" CssClass="form-control" Style="background-color: #e9ecef"></asp:Label>
                                                    </td>
                                                    <td style="display: none">
                                                        <asp:Label ID="LB_Add" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Add") %>' Visible="false"></asp:Label>
                                                    </td>
                                                    <td style="display: none">
                                                        <asp:Label ID="LB_HID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HID") %>' Visible="false"></asp:Label>
                                                    </td>
                                                    <td style="display: none">
                                                        <asp:Label ID="LB_Update" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Update") %>' Visible="false"></asp:Label>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>--%>
                            </div>


                        </div>
                    </div>











                    <!--課程類別是活動才會出現(先隱藏)-->
                    <div runat="server" id="DIV_ActivityDetail" visible="false" class="d-none">
                        <div class="member-section-title">親朋好友報名</div>
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                            <ContentTemplate>
                                <div class="row mx-0">
                                    <div class="col-md-12 table-responsive">
                                        <table class="table table-striped">
                                            <thead>
                                                <tr>

                                                    <th style="width: 25%">姓名</th>
                                                    <th style="width: 17%">性別</th>
                                                    <th style="width: 25%">年齡</th>
                                                    <th style="width: 25%">與報名者關係</th>
                                                    <th style="width: 5%">執行</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>

                                                    <td>
                                                        <asp:TextBox ID="TB_HAName" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="DDL_HASex" runat="server" class="form-control" Style="width: 100%;">
                                                            <asp:ListItem Value="1">男</asp:ListItem>
                                                            <asp:ListItem Value="2">女</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TB_HAAge" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:SqlDataSource ID="SDS_HARelation" runat="server" ConnectionString="<%$ ConnectionStrings:HochiSystemConnection %>" SelectCommand="SELECT HID, HRelation, HStatus FROM  HRelation WHERE HStatus=1"></asp:SqlDataSource>
                                                        <asp:DropDownList ID="DDL_HARelation" runat="server" class="form-control" Style="width: 100%;" AppendDataBoundItems="true" DataSourceID="SDS_HARelation" DataTextField="HRelation" DataValueField="HID">
                                                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:LinkButton ID="LBtn_Activity_Add" runat="server" class="btn btn-sm btn-outline-success" CausesValidation="false" OnClick="LBtn_Activity_Add_Click"><i class="fa fa-plus text-success"></i></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <asp:Repeater ID="Rpt_Activity" runat="server">
                                                    <ItemTemplate>
                                                        <tr>

                                                            <td>
                                                                <asp:Label ID="LB_HAName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HAName") %>' Visible="true" CssClass="form-control" Style="background-color: #e9ecef"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HASex" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HASex") %>' Visible="true" CssClass="form-control" Style="background-color: #e9ecef"></asp:Label>
                                                                <asp:Label ID="LB_HASexID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HASexID") %>' Visible="false" CssClass="form-control" Style="background-color: #e9ecef"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HAAge" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HAAge") %>' Visible="true" CssClass="form-control" Style="background-color: #e9ecef"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HARelation" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HARelation") %>' Visible="true" CssClass="form-control" Style="background-color: #e9ecef"></asp:Label>
                                                                <asp:Label ID="LB_HARelationID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HARelationID") %>' Visible="false"></asp:Label>
                                                            </td>
                                                            <td style="display: none">
                                                                <asp:Label ID="LB_Add" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Add") %>' Visible="false"></asp:Label>
                                                            </td>
                                                            <td style="display: none">
                                                                <asp:Label ID="LB_HID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "HID") %>' Visible="false"></asp:Label>
                                                            </td>
                                                            <td style="display: none">
                                                                <asp:Label ID="LB_Update" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Update") %>' Visible="false"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:LinkButton ID="LBtn_ActivityDel" runat="server" CausesValidation="false" class="btn btn-sm btn-outline-danger" CommandArgument='<%# Container.ItemIndex %>' OnClick="LBtn_ActivityDel_Click"><i class="fa fa-trash text-danger"></i></asp:LinkButton><!--刪除-->
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>


                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="DDL_HGroupName_Add" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>










                    <div class="member-section-title">
                        備註
                    </div>
                    <h6 class="font-weight-normal mt-1 mb-2" style="font-size: 14px">※可填寫身心靈需要留意的特殊狀況，登記住宿請於繳費完成後，由右上角「學員專區」>「已報名課程」>點該課程的「詳細內容」填寫「住宿資訊」。</h6>
                  
                    <div class="row mx-0">
                        <div class="col-12 area">
                            <div class="form-group">
                                <textarea id="TA_Remark" cols="20" rows="3" style="width: 100%; height: 100px" class="form-control" runat="server"></textarea>
                            </div>
                        </div>
                    </div>




                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="LBtn_Submit" runat="server" class="button button-green" Btmessage="有建立護持工作項目嗎?選完要記得按新增哦~!" OnClientClick='return confirm(this.getAttribute("btmessage"))' OnClick="LBtn_Submit_Click">儲存<i class="fas fa-angle-double-right pl-2"></i></asp:LinkButton>
                    <button type="button" class="button button-gray" data-dismiss="modal">關閉</button>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal 填寫資料  END-->







  






    <!-- Modal -->
    <div class="modal fade" id="subjectModal" tabindex="-1" role="dialog" aria-labelledby="subjectModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">

            <!-- UpdateMode="Conditional"為後端有update()時才會改變-->
            <asp:UpdatePanel ID="UP_SubjectModal" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="subjectModalLabel">選擇報考科目</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>





                                                            <table class="table table-hover table-bordered cart_table">
                                                                <tbody>
                                                                    <asp:SqlDataSource ID="SDS_ExamSubject" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                                    <asp:Repeater ID="Rpt_ExamSubject" runat="server" DataSourceID="SDS_ExamSubject" OnItemDataBound="Rpt_ExamSubject_ItemDataBound">
                                                                        <ItemTemplate>
                                                                            <tr>
                                                                                <asp:Label ID="LB_HExamContentID" runat="server" CssClass="title" Text='<%# Eval("HExamContentID") %>' Visible="false"></asp:Label>
                                                                                <asp:Label ID="LB_HExamSubjectID" runat="server" CssClass="title" Text='<%# Eval("HExamSubjectID") %>' Visible="false"></asp:Label>
                                                                                

                                                                                <td class="text-center mobile-left" data-title="勾選科目">
                                                                                    <asp:CheckBox ID="CB_SelectSubject" runat="server" OnCheckedChanged="CB_SelectSubject_CheckedChanged" AutoPostBack="true" />
                                                                                </td>
                                                                                <%--CssClass="check" data-checkbox="icheckbox_flat-purple"--%>
                                                                                <td data-title="檢覈科目名稱">
                                                                                    <asp:Label ID="LB_HExamSubjectName" runat="server" CssClass="title" Text='<%# Eval("HExamSubjectName") %>'></asp:Label>
                                                                                </td>

                                                                            </tr>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                                </tbody>
                                                            </table>





                        <div class="modal-footer">
                            <asp:Button ID="Btn_SubmitSubject" runat="server" Text="儲存選擇" OnClick="Btn_SubmitSubject_Click" Visible="false" />
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">關閉</button>

                        </div>
                    </div>

                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="Btn_Back" />
                    <asp:AsyncPostBackTrigger ControlID="DDL_HPayMethod" EventName="TextChanged" />
                </Triggers>
            </asp:UpdatePanel>

        </div>
    </div>










    <script src="js/jquery-3.4.1.min.js"></script>
    <script src="js/popper.js"></script>
    <script src="bootstrap-4.4.1/js/bootstrap.min.js"></script>
    <script src="js/popper.js"></script>
    <script src="fonts/fontawesome-5.12.0/js/fontawesome.min.js"></script>
    <script src="fonts/fontawesome-5.12.0/js/brands.min.js"></script>
    <!--固定導覽列-->
    <script src="js/sticky.js"></script>
    <!-- icheck -->
    <script src="assets/icheck/icheck.min.js"></script>
    <script src="assets/icheck/icheck.init.js"></script>
    <script src="js/select2/js/select2.min.js"></script>

    <!--daterangepicker JavaScript -->
    <script src="js/moment.min.js"></script>
    <script src="js/daterangepicker.js"></script>
    <!--datepicker-->
    <link href="../system/css/datepicker.min.css" rel="stylesheet" />
    <script src="../system/js/datepicker.min.js"></script>
    <script src="../system/dist/js/i18n/datepicker.en.js"></script>
    <!--timepicker JavaScript -->
    <script src="js/jquery.timepicker.min.js"></script>


  



    <!-- back to top start -->
    <script>

        if ($('#back-to-top').length) {
            var scrollTrigger = 100, // px
                backToTop = function () {
                    var scrollTop = $(window).scrollTop();
                    if (scrollTop > scrollTrigger) {
                        $('#back-to-top').addClass('show');
                    } else {
                        $('#back-to-top').removeClass('show');
                    }
                };
            backToTop();
            $(window).on('scroll', function () {
                backToTop();
            });
            $('#back-to-top').on('click', function (e) {
                e.preventDefault();
                $('html,body').animate({
                    scrollTop: 0
                }, 700);
            });
        }
    </script>
    <!-- back to top end -->

    <script>
        $(function () {
            //$(".datepicker").datepicker({
            //	format: 'yyyy/mm/dd',
            //	autoclose: true,
            //	toggleActive: false,
            //	todayHighlight: true,
            //	orientation: 'bottom auto',
            //});

            //單選
            $('.js-example-basic-single').select2();
            $('.select2-multiple').select2();

            //多選日期
            $('.datemultipicker').datepicker({
                language: 'en',
                dateFormat: 'yyyy/mm/dd',
                multipleDates: true,
                minDate: new Date(),
                todayHighlight: true,
                orientation: 'bottom auto',
                //startDate: new Date(),
            });

            //時間選取器
            $('.timepicker').timepicker({
                timeFormat: 'HH:mm',
                interval: 30,
                minTime: '4',
                maxTime: '11:00 pm',
                //defaultTime: '4',
                startTime: '04:00',
                dynamic: false,//是否顯示項目，使第一個項目按時間順序緊接在所選時間之後
                dropdown: true,//是否顯示時間條目的下拉列表
                scrollbar: true//是否顯示捲軸
            });
        });
    </script>

    <script>
        // 顯示讀取遮罩
        function ShowProgressBar() {
            displayProgress();
            displayMaskFrame();
        }

        // 隱藏讀取遮罩
        function HideProgressBar() {
            var progress = $('#divProgress');
            var maskFrame = $("#divMaskFrame");
            progress.hide();
            maskFrame.hide();
        }

        // 顯示讀取畫面
        function displayProgress() {
            var w = $(document).width();
            var h = $(window).height();
            var progress = $('#divProgress');
            progress.css({ "z-index": 999999, "top": (h / 2) - (progress.height() / 2), "left": (w / 2) - (progress.width() / 2) });
            progress.show();
        }
        // 顯示遮罩畫面
        function displayMaskFrame() {
            var w = $(window).width();
            var h = $(document).height();
            var maskFrame = $("#divMaskFrame");
            maskFrame.css({ "z-index": 999998, "opacity": 0.7, "width": w, "height": h });
            maskFrame.show();
        }
    </script>

    <script>
        function myFunction() {
            let text = "提醒您：您勾選的課程上課地點與您的區屬不同哦~\n 若已確認報名此上課地點，請按【確定】。\n若想再確認一次請按【取消】。";
            //let text = "您勾選的課程上課地點與您的區屬不同哦~\n再請確認地點是否正確";
            if (confirm(text) == true) {
                return;
            } else {
                javascript: history.back(-1);
                //javascript: window.location.href = "HShoppingCart.aspx";
                return;
            }
        }
    </script>

</asp:Content>

