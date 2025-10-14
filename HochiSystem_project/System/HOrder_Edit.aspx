<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HOrder_Edit.aspx.cs" Inherits="HOrder_Edit" %>

<%--分頁--%>
<%@ Register TagPrefix="Page" TagName="Paging" Src="~/System/Paging_backend.ascx" %>
<%@ Register Assembly="MyWebControls" Namespace="MyWebControls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .table tbody tr td, .table tbody th td {
            word-break: break-word;
            white-space: normal;
        }

        .table th, .table td {
            padding: 0.2rem;
        }

        .table tbody td span {
            font-size: 14px !important;
        }

        .btn-outline-success {
            color: #00c292 !important;
        }

            .btn-outline-success:hover {
                color: #fff !important;
            }

        .pull-left h3 b {
            font-size: 1.15rem;
        }

        .bg-gray {
            background-color: #929292;
        }

        .hideinfo {
            padding: 15px 5px 5px 15px;
            margin-top: 0;
        }
    </style>
    <script>
        function printdiv(printpage) {
            var newstr = printpage.innerHTML;
            var oldstr = document.body.innerHTML;
            document.body.innerHTML = newstr;
            window.print();
            document.body.innerHTML = oldstr;
            return false;
        }
    </script>
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
            <!-- ============================================================== -->

            <asp:Panel ID="Panel_List" runat="server">
                <div class="block-header">
                    <div class="row">
                        <div class="col-lg-3 col-md-12 col-sm-4">
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>訂單管理</h2>
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item active">訂單管理</li>
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
                                <div class="row d-none">
                                    <!-- Column -->
                                    <div class="col-md-12 col-lg-2 col-xlg-2">
                                        <div class="box text-left">
                                            <button id="Btn_Add" runat="server" type="button" class="btn btn-outline-info" onclick="window.location.href='HCourseBookingB.aspx';"><i class="fa fa-plus"></i>幫他人報名</button>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12 col-lg-12 col-xlg-12">
                                        <div class="box form-group row m-b-0">
                                            <div class="col-md-3">
                                                <asp:TextBox ID="TB_Search" runat="server" class="form-control" placeholder="請輸入訂單代碼/學員姓名/發票收據號碼" AutoComplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3" style="max-width: 18%;">
                                                <asp:TextBox ID="TB_SearchDate" runat="server" class="form-control daterange" placeholder="選擇結帳日期區間" AutoComplete="off"></asp:TextBox>
                                                <%--選擇付款日期區間--%>
                                            </div>
                                            <div class="col-md-2 col-sm-2 p-r-0 p-l-0 m-r-5">
                                                <asp:DropDownList ID="DDL_HPMethod" runat="server" class="form-control">
                                                    <asp:ListItem Value="0">請選擇繳費帳戶</asp:ListItem>
                                                    <asp:ListItem Value="1">基金會</asp:ListItem>
                                                    <asp:ListItem Value="2">文化事業</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-md-2 col-sm-2 p-r-0 p-l-0">
                                                <asp:DropDownList ID="DDL_HStatus_Search" runat="server" class="form-control">
                                                    <asp:ListItem Value="0">請選擇訂單狀態</asp:ListItem>
                                                    <asp:ListItem Value="1">訂單成立</asp:ListItem>
                                                    <asp:ListItem Value="2">訂單取消</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>

                                            <div class="col-md-3 excel_outer" style="max-width: 20%; display: flex; align-items: center;">
                                                <asp:LinkButton ID="LBtn_Search" runat="server" OnClick="LBtn_Search_Click" class="btn btn-outline-secondary"><span class="btn-label"><i class="fa fa-search"></i></span>搜尋</asp:LinkButton>
                                                <asp:LinkButton ID="LBtn_SearchCancel" runat="server" OnClick="LBtn_SearchCancel_Click" class="btn btn-outline-secondary m-l-10"><span class="btn-label"><i class="fa fa-close"></i></span>取消</asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                              
                                <div class="table-responsive">
                                    <table class="table m-b-0 table-hover m-t-20">
                                        <thead>
                                            <tr class="font-weight-bold">
                                                <th class="text-center" style="width: 5%">執行</th>
                                                <th class="text-center" style="width: 5%">No</th>
                                                <th style="width: 10%">訂單代碼</th>
                                                <th class="text-center" style="width: 10%">學員姓名</th>
                                                <th class="text-center" style="width: 7%">繳費帳戶</th>
                                                <th class="text-right" style="width: 5%">使用點數</th>
                                                <th class="text-right" style="width: 8%">應付總金額</th>
                                                <th class="text-right" style="width: 8%">已付總金額<%--付款金額--%></th>
                                                <th class="text-center" style="width: 8%">付款方式</th>
                                                <th class="text-center" style="width: 12%">結帳日期	<%--付款時間--%></th>
                                                <th class="text-center" style="width: 10%">訂單狀態</th>
                                                <th class="text-center" style="width: 10%">付款狀態</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:SqlDataSource ID="SDS_HCMCB" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                            <asp:Repeater ID="Rpt_HCMCB" runat="server" OnItemDataBound="Rpt_HCMCB_ItemDataBound">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="text-center">
                                                            <asp:LinkButton ID="LBtn_Edit" runat="server" class="btn btn-sm btn-outline-success" ToolTip="編輯" OnClick="LBtn_Edit_Click" CommandArgument='<%# Eval("HOrderGroup")+","+Eval("HOrderNum") %>'><i class="icon-pencil"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="LBtn_Del" runat="server" class="btn btn-sm btn-outline-danger js-sweetalert d-none" OnClick="LBtn_Del_Click" CommandArgument='<%# Eval("HOrderGroup") %>' Btmessage="確定要刪除嗎？" OnClientClick='return confirm(this.getAttribute("btmessage"))'><i class="icon-trash"></i></asp:LinkButton>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HIndex" runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="LB_HOrderGroup" runat="server" Text='<%# Eval("HOrderGroup") %>'></asp:Label>
                                                            <asp:Label ID="LB_HOrderGroupSrc" runat="server" Text='<%# Eval("HOrderGroupSrc") %>' Visible="false"></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HUserName" runat="server" Text='<%# Eval("UserName") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HPMethod" runat="server" Text='<%# Eval("PMethod") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HUse" runat="server" Text='<%# Eval("HUse") %>'></asp:Label><!--使用點數-->
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_Total" runat="server" Text='<%# Eval("HPayAmt") %>'></asp:Label><!--總報名費用-->
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:Label ID="LB_HPayAmt" runat="server" Text='<%# Eval("HPayAmt") %>'></asp:Label><!--實際繳費金額-->
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HPayMethod" runat="server" Text='<%# Eval("PayMethod") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_PaymentDate" runat="server" Text='<%# Eval("HPaymentDate") %>'></asp:Label>
                                                            <asp:Label ID="LB_HCreateDT" runat="server" Text='<%# Eval("HCreateDT") %>' Visible="false"></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HStatus" runat="server" Text='<%# Eval("HStatus") %>'></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="LB_HItemStatus" runat="server" CssClass="label label-success" Text=""></asp:Label>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>

                                <!------------------分頁功能開始------------------>
                                <nav class="box text-right">
                                    <Page:Paging runat="server" ID="Pg_Paging" />
                                </nav>
                                <!------------------分頁功能結束------------------>



                          


                                

                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>






            <asp:Panel ID="Panel_Edit" runat="server" Visible="false">
                <asp:Label ID="LB_HID" runat="server" Text="" Visible="false"></asp:Label>

                <div class="block-header">
                    <div class="row">
                        <div class="col-lg-3 col-md-12 col-sm-4">
                            <h2><a onclick="history.back(-1)" class="btn btn-xs btn-link btn-toggle-fullwidth"><i class="fa fa-arrow-left"></i></a>訂單管理</h2>
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="#"><i class="icon-home"></i></a></li>
                                <li class="breadcrumb-item"><a href="HOrder_Edit.aspx">訂單管理</a></li>
                                <li class="breadcrumb-item active">編輯訂單管理</li>
                            </ul>
                        </div>
                    </div>
                </div>
                <!-- ============================================================== -->
                <!-- Start Page Content -->
                <!-- ============================================================== -->
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12">
                        <div class="card card-body">
                            <div class="row justify-content-end">
                                <button id="print" class="btn btn-info" onclick="printdiv(document.getElementById('Div_Print'));" target="_blank"><span><i class="fa fa-print mr-1"></i>列印</span></button>

                            </div>
                            <asp:Label ID="LB_HMemberID" runat="server" Text="" Visible="false"></asp:Label>
                            <asp:Label ID="LB_HStatus" runat="server" Text="" Visible="false"></asp:Label>
                            <div class="row printableArea" id="Div_Print">
                                <div class="col-md-12">
                                    <div class="pull-left">
                                        <h3><b>● 訂單主檔</b></h3>
                                    </div>
                                </div>
                                <label class="text-danger">*更改訂單狀態需重新產生點名單喔!</label>
                                <div class="col-md-12">
                                    <div class="row clearfix">
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">訂單狀態</label>&nbsp;&nbsp;
                                            <div class="form-group">
                                                <asp:DropDownList ID="DDL_HStatus" runat="server" class="form-control" Enabled="false">
                                                    <asp:ListItem Value="1">訂單成立</asp:ListItem>
                                                    <asp:ListItem Value="2">訂單取消</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">訂單代碼</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HOrderNum" runat="server" Visible="false"></asp:Label>
                                                <asp:Label ID="LB_HOrderGroup" runat="server" Text="" Visible="false"></asp:Label>
                                                <asp:Label ID="LB_Memo" runat="server" Text="(變更後)" CssClass="text-danger font-weight-normal" Visible="false"></asp:Label>
                                                <asp:Label ID="LB_OriOrderGroup" runat="server" Text="" Visible="false"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">訂購日期</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HBDate" runat="server" ReadOnly="true"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">綠界廠商訂單編號</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HMerchantTradeNo" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">綠界交易編號</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HTradeNo" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">繳費帳戶</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HPMethod" runat="server" Visible="true"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">付款方式</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HPayMethod" runat="server" Visible="true"></asp:Label><!--金流付款方式-->

                                            </div>
                                        </div>
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">超商代碼</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HPaymentNo" runat="server" Text="-" Visible="true"></asp:Label><!--金流付款方式-->

                                            </div>
                                        </div>

                                        <!--ATM櫃員機繳費資訊-->
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">ATM櫃員機繳費銀行代碼</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HATMBankCode" runat="server" Text="-" Visible="true"></asp:Label><!--金流付款方式-->

                                            </div>
                                        </div>

                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">ATM櫃員機繳費虛擬帳號</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HATMVAccount" runat="server" Text="-" Visible="true"></asp:Label><!--金流付款方式-->

                                            </div>
                                        </div>


                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">
                                                <asp:Label ID="LB_PayType" runat="server" Text="ATM櫃員機/超商"></asp:Label>繳費期限</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HExpireDate" runat="server" Text="-" Visible="true"></asp:Label>
                                            </div>
                                        </div>

                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">使用點數</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HUse" runat="server" Visible="true"></asp:Label>
                                                點

                                            </div>
                                        </div>
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">訂單總金額</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HPayAmt" runat="server" Visible="true" Text="0"></asp:Label>
                                                元

                                            </div>
                                        </div>
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">已付總金額</label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HECPAmount" runat="server" Visible="true" Text="0" ></asp:Label>
                                                元

                                            </div>
                                        </div>
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">結帳日期<%--付款時間--%></label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HPaymentDate" runat="server" Visible="true"></asp:Label>

                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">發票/收據開立狀態<%--發票狀態--%></label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HInvoiceStatus" runat="server" Visible="true"></asp:Label>

                                            </div>
                                        </div>
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">發票/收據號碼<%--發票單號--%></label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HInvoiceNo" runat="server" Visible="true"></asp:Label>

                                            </div>
                                        </div>
                                        <div class="col-md-2 col-sm-12">
                                            <label class="col-form-label font-weight-bold">發票/收據開立日期<%--發票日期--%></label>
                                            <div class="form-group">
                                                <asp:Label ID="LB_HInvoiceDate" runat="server" Visible="true"></asp:Label>

                                            </div>
                                        </div>

                                        <div class="col-md-2 col-sm-6">
                                            <label class="col-form-label font-weight-bold pb-0">是否上傳國稅局</label>
                                            <div class="form-group mb-2">
                                                <asp:Label ID="LB_HUploadIRS" runat="server" Visible="true"></asp:Label>

                                            </div>
                                        </div>



                                    </div>
                                </div>

                                <div class="col-md-12">
                                    <div class="pull-left">
                                        <h3><b>● 學員資料</b></h3>
                                    </div>
                                </div>
                                <div class="col-md-12 m-b-10">
                                    <div class="table-responsive" style="clear: both;">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr class="bg-light">
                                                    <th style="width: 10%;">學員類別</th>
                                                    <th style="width: 20%;">姓名</th>
                                                    <th style="width: 20%;">身分證字號</th>
                                                    <th style="width: 15%;">手機號碼	</th>
                                                    <th style="width: 25%;">電子信箱</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="LB_HType" runat="server" ReadOnly="true"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="LB_HUserName" runat="server" ReadOnly="true"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="LB_HPersonID_N" runat="server" ReadOnly="true"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="LB_HPhone" runat="server" ReadOnly="true"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="LB_HEmail" runat="server" ReadOnly="true"></asp:Label>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                                <div class="col-md-12 d-none">
                                    <div class="pull-left">
                                        <h3><b>● 護持者</b></h3>
                                    </div>
                                </div>
                                <div class="col-md-6 m-b-10 d-none">
                                    <div class="table-responsive" style="clear: both;">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr class="bg-light">
                                                    <th style="width: 40%;">姓名</th>
                                                    <th style="width: 40%;">身分證字號</th>
                                                    <th style="width: 20%;">性別</th>
                                                </tr>
                                            </thead>
                                            <tbody>


                                                <asp:Repeater ID="Rpt_HCBM" runat="server" OnItemDataBound="Rpt_HCBM_ItemDataBound">



                                                    <HeaderTemplate>
                                                    </HeaderTemplate>





                                                    <ItemTemplate>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="LB_HCGuide" runat="server" ReadOnly="true" Text='<%# Eval("HCGuide") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HPersonID_G" runat="server" ReadOnly="true" Text='<%# Eval("HPersonID") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HSex" runat="server" ReadOnly="true" Text='<%# Eval("HSex") %>'></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>

                                                    <FooterTemplate>
                                                    </FooterTemplate>




                                                </asp:Repeater>




                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                                <div class="col-md-12">
                                    <div class="pull-left">
                                        <h3><b>● 訂單明細</b> </h3>
                                    </div>
                                </div>

                                <div class="row col-12 mx-0">
                                      <asp:HiddenField ID="HF_HItemStatus" runat="server" />
                                    <asp:HiddenField ID="HF_HBookByDateYN" runat="server" />
                                    <asp:LinkButton ID="LBtn_Refund" runat="server" CssClass="btn btn-outline-primary" OnClick="LBtn_Refund_Click">取消報名</asp:LinkButton>

                                    <div class="table-responsive mt-3">
                                        <table class="table table-striped table-bordered" style="width: 100%">
                                            <thead>
                                                <tr>
                                                    <th class="text-center font-weight-bold" style="width: 3%">No</th>
                                                    <th class="font-weight-bold" style="width: 10%">項目編號</th>
                                                    <th class="font-weight-bold" style="width: 17%">
                                                        <asp:Label ID="LB_HCPkgHead" runat="server" Text="套裝" Visible="false"></asp:Label>課程名稱</th>
                                                    <th class="font-weight-bold" style="width: 18%"><asp:Label ID="LB_Booked" runat="server" Text="報名" Visible="false"></asp:Label>上課<asp:Label ID="LB_HCStart" runat="server" Text="起始" Visible="false"></asp:Label>日期</th>
                                                    <th class="font-weight-bold" style="width: 8%;">上課地點</th>
                                                    <th class="font-weight-bold" style="width: 5%;">參班與否</th>
                                                    <th class="font-weight-bold text-right" style="width: 5%;">基本費用</th>
                                                    <th class="font-weight-bold text-right" style="width: 6%;">折扣碼</th>
                                                    <th class="font-weight-bold text-right" style="width: 6%;">折扣金額</th>
                                                    <th class="font-weight-bold text-right" style="width: 8%;">小計</th>
                                                    <th class="font-weight-bold text-center" style="width: 10%;">付款狀態</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                                <!--單一課程-->
                                                <asp:SqlDataSource ID="SDS_BookingList" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_BookingList" runat="server" DataSourceID="SDS_BookingList" OnItemDataBound="Rpt_BookingList_ItemDataBound">
                                                    <ItemTemplate>

                                                        <asp:Label ID="LB_HCourseDonate" runat="server" Text='<%# Eval("HCourseDonate") %>' Visible="false"></asp:Label>
                                                        <asp:Label ID="LB_DCName" runat="server" Text='<%#Eval("DCName") %>' Visible="false"></asp:Label>
                                                        <asp:Label ID="LB_DCDateRange" runat="server" Text='<%#Eval("DCDateRange") %>' Visible="false"></asp:Label>
                                                        <tr>

                                                            <td data-title="No" class="text-center">
                                                                <asp:Label ID="LB_HIndex" runat="server" Text='<%# Eval("ROW") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="項目編號">
                                                                <asp:Label ID="LB_HOrderNum" runat="server" Text='<%#Eval("HOrderNum") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="課程名稱">
                                                                <asp:Label ID="LB_HCourseName" runat="server" Text='<%#Eval("HCourseName") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="上課日期" style="word-break: break-all;">
                                                                <asp:Label ID="LB_HDateRange" runat="server" Text='<%#Eval("HDateRange") %>'></asp:Label>
                                                                <asp:Label ID="LB_HBookedDate" runat="server" Text='<%#Eval("HBookedDate") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="上課地點">
                                                                <asp:Label ID="LB_Location" runat="server" Text='<%#Eval("Location") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="參班與否">
                                                                <asp:Label ID="LB_HAttend" runat="server" Text='<%#Eval("HAttend") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="基本費用" class="text-right">
                                                                <asp:Label ID="LB_HBCPoint" runat="server" Text='<%#Eval("HBCPoint") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="折扣碼" class="text-right">
                                                                <asp:Label ID="LB_HDCode" runat="server" Text='<%#Eval("HDCode") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="折扣金額" class="text-right">
                                                                <asp:Label ID="LB_HDPoint" runat="server" Text='<%#Eval("HDPoint") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="已付款金額" class="text-right">
                                                                <asp:Label ID="LB_HPMethod" runat="server" Text='<%#Eval("HPMethod") %>' Visible="false"></asp:Label>
                                                                <asp:Label ID="LB_HCorseDonate" runat="server" Text='<%#Eval("HCourseDonate") %>' Visible="false"></asp:Label>
                                                                <asp:Label ID="LB_HPoint" runat="server" Text='<%#Eval("HPoint") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="付款狀態" class="text-center">
                                                                <asp:Label ID="LB_HItemStatus" runat="server" Text='<%#Eval("HItemStatus") %>' Style="font-weight: 600;"></asp:Label>
                                                            </td>

                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>

                                                <!--AA20240204_套裝課程-->
                                                <asp:SqlDataSource ID="SDS_HCPackageList" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_HCPackageList" runat="server" DataSourceID="SDS_HCPackageList" OnItemDataBound="Rpt_HCPackageList_ItemDataBound">
                                                    <ItemTemplate>
                                                        <asp:Label ID="LB_HCPkgHID" runat="server" Text='<%# Eval("HCPkgHID") %>' Visible="false"></asp:Label>
                                                        <asp:Label ID="LB_HCourseDonate" runat="server" Text='<%# Eval("HCourseDonate") %>' Visible="false"></asp:Label>

                                                        <asp:Label ID="LB_HChangeStatus" runat="server" Text='<%#Eval("HChangeStatus") %>' Visible="false"></asp:Label>
                                                        <tr>

                                                            <td data-title="序" class="text-center mobile-left">
                                                                <asp:Label ID="LB_HIndex" runat="server" Text="1"></asp:Label>
                                                            </td>
                                                            <td data-title="項目編號">
                                                                <asp:Label ID="LB_HOrderNum" runat="server" Text="-"></asp:Label>
                                                            </td>
                                                            <td data-title="套裝課程名稱">
                                                                <asp:Label ID="LB_HCPkgName" runat="server" Text='<%#Eval("HCPkgName") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="上課起始日期" style="word-break: break-all;">
                                                                <asp:Label ID="LB_HDateRange" runat="server" Text='<%#Eval("HCStartDate") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="上課地點" style="word-break: break-all;">
                                                                <asp:Label ID="Label4" runat="server" Text="-"></asp:Label>
                                                            </td>

                                                            <td data-title="參班身分">
                                                                <asp:Label ID="LB_HAttend" runat="server" Text='<%#Eval("HAttend") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="基本費用" class="text-right">
                                                                <asp:Label ID="LB_HPkgPrice" runat="server" Text='<%#Eval("HPkgPrice") %>'></asp:Label>
                                                            </td>

                                                            <td data-title="折扣碼" class="text-right">
                                                                <asp:Label ID="LB_HDCode" runat="server" Text='<%#Eval("HDCode") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="折扣金額" class="text-right">
                                                                <asp:Label ID="LB_HDPoint" runat="server" Text='<%#Eval("HDPoint") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="小計" class="text-right">
                                                                <asp:Label ID="LB_HPMethod" runat="server" Text='<%#Eval("HPMethod") %>' Visible="false"></asp:Label>
                                                                <asp:Label ID="LB_HCorseDonate" runat="server" Text='<%#Eval("HCourseDonate") %>' Visible="false"></asp:Label>
                                                                <asp:Label ID="LB_HPkgSubTotal" runat="server" Text='<%#Eval("HPkgSubTotal") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="付款狀態" class="text-center mobile-left">
                                                                <asp:Label ID="LB_HItemStatus" runat="server" Text='<%#Eval("HItemStatus") %>'></asp:Label>
                                                            </td>

                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>


                                            </tbody>

                                        </table>
                                    </div>
                                </div>



                                <div class="col-md-12 pr-3">
                                    <div class="font-weight-normal mt-3 text-right pl-3">
                                        小計總金額：
									<asp:Label ID="LB_SubTotal" runat="server" Text="0" CssClass="font-weight-normal" Style="display: inline-block; width: 62px;"></asp:Label>
                                        元
                                    </div>
                                    <div class="font-weight-normal mt-1 text-right pl-3 d-none">
                                        折扣總金額：<asp:Label ID="LB_DTotal" runat="server" Text="0" CssClass="font-weight-normal"></asp:Label>
                                        元
                                    </div>
                                    <div class="font-weight-normal mt-1 text-right pl-3">
                                        點數支付(1點=10元)：
										<asp:Label ID="LB_PTotal" runat="server" Text="0" CssClass="font-weight-normal" Style="display: inline-block; width: 65px;"></asp:Label>元
                                    </div>
                                    <div id="Tr_Original" runat="server" visible="false" class="font-weight-normal mt-1 text-right pl-3">
                                        原訂單(<asp:Label ID="LB_OrderGroupSrc" runat="server" Text="Label"></asp:Label>)已付總金額：
										<asp:Label ID="LB_OriPaidAmount" runat="server" Text="0" Style="display: inline-block; width: 65px;"></asp:Label>元
                                    </div>
                                    <div class="d-flex justify-content-end align-items-center">
                                        <hr class="line" style="width: 26%; display: block; margin: 5px 0; text-align: right; border: 1px solid #000 !important;" />
                                    </div>
                                    <div class="font-weight-bold mt-0 text-right pl-3">
                                        訂單總金額：
									
										<asp:Label ID="LB_Total" runat="server" Text="0" Style="display: inline-block; width: 63px;"></asp:Label>
                                        <!--該訂單應付總金額-->
                                        <asp:Label ID="LB_AfterTotal" runat="server" Text="0" Style="display: inline-block; width: 63px;"></asp:Label>元<!--變更後訂單應付總金額-->
                                    </div>
                                    <div class="font-weight-normal mt-1 text-right pl-3">
                                        <asp:Label ID="LB_OriPaid" runat="server" Text="原訂單" CssClass="d-none" Visible="false"></asp:Label>已付總金額：
									<asp:Label ID="LB_Paid" runat="server" Text="0" Style="display: inline-block; width: 63px;"></asp:Label><!--該訂單已付總金額-->
                                        <asp:Label ID="LB_Supplementary" runat="server" Text="0" Visible="false" Style="display: inline-block; width: 63px;"></asp:Label>元<!--變更後訂單已付總金額-->
                                    </div>

                                    <div class="font-weight-normal mt-1 text-right pl-3 d-none" id="Supplement" runat="server" visible="false">
                                        變更後訂單已付<!--應補繳-->總金額：
                                   
                                        元
                                    </div>
                                    <div class="d-none">
                                        <asp:Label ID="LB_HCXLSubTotalSum" runat="server" Text="0" Style="display: inline-block; width: 70px;"></asp:Label>
                                        <asp:Label ID="LB_HCXLHandlingFee" runat="server" Text="0" Style="display: inline-block; width: 70px;"></asp:Label>
                                    </div>
                                    <div class="font-weight-bold mt-1 text-right pl-3 text-danger" id="Div_Refund" runat="server" visible="false">

                                        已退款總金額：    
                                        <asp:Label ID="LB_HCXLTotal" runat="server" Text="0" Style="display: inline-block; width: 63px;"></asp:Label>
                                        元
                                    </div>

                                    <div class="clearfix"></div>
                                </div>


                                <!--暫時隱藏-->
                                <div class="col-md-12 d-none">
                                    <div class="pull-left">
                                        <h3><b>● 發票明細</b></h3>
                                    </div>
                                </div>

                                <div class="row col-8 mx-0 d-none">
                                    <div class="table-responsive">
                                        <table class="table table-striped table-bordered" style="width: 100%">
                                            <thead>
                                                <tr>
                                                    <th class="text-center font-weight-bold" style="width: 4%">No</th>
                                                    <th class="font-weight-bold" style="width: 24%">項目編號</th>
                                                    <th class="font-weight-bold" style="width: 24%">發票/收據號碼<%--發票單號--%></th>
                                                    <th class="font-weight-bold" style="width: 24%">發票/收據開立日期<%--發票日期--%></th>
                                                    <th class="font-weight-bold" style="width: 24%;">發票/收據開立狀態<%--發票狀態--%></th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                                <asp:SqlDataSource ID="SDS_Invoice" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_Invoice" runat="server" DataSourceID="SDS_Invoice" OnItemDataBound="Rpt_Invoice_ItemDataBound">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td class="text-center">
                                                                <asp:Label ID="LB_HIndex" runat="server" Text='<%# Container.ItemIndex+1%>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HOrderNum" runat="server" Text='<%#Eval("HOrderNum") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HInvoiceNo" runat="server" Text='<%#Eval("HInvoiceNo") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HInvoiceDate" runat="server" Text='<%#Eval("HInvoiceDate") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="LB_HInvoiceStatus" runat="server" Text='<%#Eval("HInvoiceStatus") %>'></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>

                                        </table>
                                    </div>
                                </div>
                                <!--暫時隱藏-->

                                <asp:Panel ID="Panel_HChangeRecord" runat="server" Visible="false" CssClass="col-md-12">
                                    <div class="pull-left">
                                        <h3 class="mt-1"><b>● 訂單變更後紀錄</b></h3>
                                    </div>


                                    <asp:SqlDataSource ID="SDS_ChangeRecord" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                    <asp:Repeater ID="Rpt_ChangeRecord" runat="server" DataSourceID="SDS_ChangeRecord" OnItemDataBound="Rpt_ChangeRecord_ItemDataBound">
                                        <ItemTemplate>

                                            <asp:Label ID="LB_HOrderGroupSrc" runat="server" Text='<%# Eval("HOrderGroupSrc") %>' Visible="false"></asp:Label>


                                            <asp:LinkButton ID="LBtn_Info" runat="server" class="d-inline-block col-12 btn btn-secondary text-left moreinfo mt-0">
                                                <div class="row mr-0 ml-0">
                                                    <div class="d-flex justify-content-between col-md-12">
                                                        <div class="col-11 row">
                                                            <div class="col-md-3 col-sm-12">
                                                                訂單代碼：<asp:Label ID="LB_HOrderGroupNew" runat="server" CssClass="font-weight-normal" Text='<%# Eval("HOrderGroupNew") %>'></asp:Label>
                                                            </div>
                                                            <div class="col-md-3 col-sm-12">
                                                                變更日期：<asp:Label ID="LB_ChangeDT" runat="server" CssClass="font-weight-normal" Text='<%# Eval("ChangeDT") %>'></asp:Label>
                                                            </div>
                                                            <div class="col-md-3 col-sm-12">
                                                                補繳金額：<asp:Label ID="LB_BalancePaid" runat="server" CssClass="font-weight-normal" Text='<%# Eval("BalancePaid") %>'></asp:Label>元
                                                            </div>
                                                        </div>
                                                        <div class="col-md-1 pr-1 text-right">
                                                            展開 <i class="fa fa-angle-down"></i><span></span>
                                                        </div>
                                                    </div>

                                                </div>


                                            </asp:LinkButton>

                                            <div class="hideinfo" style="display: none;">
                                                <div class="row mx-0">
                                                    <div class="col-md-12 table-responsive pl-0 table-mobile">
                                                        <table class="table table-bordered" style="width: 100%">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center font-weight-bold" style="width: 3%">序</th>
                                                                    <th class="font-weight-bold" style="width: 10%">項目編號</th>
                                                                    <th class="font-weight-bold" style="width: 15%">課程名稱</th>
                                                                    <th class="font-weight-bold" style="width: 15%">上課日期</th>
                                                                    <th class="font-weight-bold" style="width: 7%;">上課地點</th>
                                                                    <th class="font-weight-bold" style="width: 7%;">參班身分</th>
                                                                    <th class="font-weight-bold text-right" style="width: 7%;">基本費用</th>
                                                                    <th class="font-weight-bold text-right" style="width: 7%;">付款金額</th>
                                                                    <th class="font-weight-bold text-right" style="width: 7%;">折扣碼</th>
                                                                    <th class="font-weight-bold text-right" style="width: 7%;">折扣金額</th>
                                                                    <th class="font-weight-bold text-right" style="width: 8%;">小計</th>
                                                                    <th class="font-weight-bold text-center" style="width: 10%;">付款狀態</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <asp:SqlDataSource ID="SDS_ChangeRecordDetail" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                                <asp:Repeater ID="Rpt_ChangeRecordDetail" runat="server" DataSourceID="SDS_ChangeRecordDetail" OnItemDataBound="Rpt_ChangeRecordDetail_ItemDataBound">
                                                                    <ItemTemplate>

                                                                        <asp:Label ID="LB_HCourseDonate" runat="server" Text='<%# Eval("HCourseDonate") %>' Visible="false"></asp:Label>
                                                                        <asp:Label ID="LB_HDCName" runat="server" Text='<%#Eval("HDCName") %>' Visible="false"></asp:Label>
                                                                        <asp:Label ID="LB_HDCDateRange" runat="server" Text='<%#Eval("HDCDateRange") %>' Visible="false"></asp:Label>
                                                                        <asp:Label ID="LB_HChangeStatus" runat="server" Text='<%#Eval("HChangeStatus") %>' Visible="false"></asp:Label>
                                                                        <tr>
                                                                            <td class="text-center mobile-left" data-title="序">
                                                                                <asp:Label ID="LB_HIndex" runat="server" Text='<%# Eval("ROW") %>'></asp:Label>
                                                                            </td>
                                                                            <td data-title="項目編號">
                                                                                <asp:Label ID="LB_HOrderNum" runat="server" Text='<%#Eval("HOrderNum") %>'></asp:Label>
                                                                            </td>
                                                                            <td data-title="課程名稱">
                                                                                <asp:Label ID="LB_HCourseName" runat="server" Text='<%#Eval("HDCName") %>'></asp:Label>
                                                                            </td>
                                                                            <td data-title="上課日期" style="word-break: break-all;">
                                                                                <asp:Label ID="LB_HDateRange" runat="server" Text='<%#Eval("HDCDateRange") %>'></asp:Label>
                                                                            </td>
                                                                            <td data-title="上課地點">
                                                                                <asp:Label ID="LB_HOCPlace" runat="server" Text='<%#Eval("HOCPlace") %>'></asp:Label>
                                                                            </td>
                                                                            <td data-title="參班身分">
                                                                                <asp:Label ID="LB_HAttend" runat="server" Text='<%#Eval("HAttend") %>'></asp:Label>
                                                                            </td>
                                                                            <td data-title="基本費用" class="text-right">
                                                                                <asp:Label ID="LB_HBCPoint" runat="server" Text='<%#Eval("HBCPoint") %>'></asp:Label>
                                                                            </td>
                                                                            <td data-title="付款金額" class="text-right">
                                                                                <asp:Label ID="LB_HPAmount" runat="server" Text='<%#Eval("HPAmount") %>'></asp:Label>
                                                                            </td>
                                                                            <td data-title="折扣碼" class="text-right">
                                                                                <asp:Label ID="LB_HDCode" runat="server" Text='<%#Eval("HDCode") %>'></asp:Label>
                                                                            </td>
                                                                            <td data-title="折扣金額" class="text-right">
                                                                                <asp:Label ID="LB_HDPoint" runat="server" Text='<%#Eval("HDPoint") %>'></asp:Label>
                                                                            </td>
                                                                            <td data-title="小計" class="text-right">
                                                                                <asp:Label ID="LB_HPMethod" runat="server" Text='<%#Eval("HPMethod") %>' Visible="false"></asp:Label>
                                                                                <asp:Label ID="LB_HCorseDonate" runat="server" Text='<%#Eval("HCourseDonate") %>' Visible="false"></asp:Label>
                                                                                <asp:Label ID="LB_HPoint" runat="server" Text='<%#Eval("HPoint") %>'></asp:Label>
                                                                            </td>
                                                                            <td data-title="付款狀態" class="text-center mobile-left">
                                                                                <asp:Label ID="LB_HItemStatus" runat="server" Text='<%#Eval("HItemStatus") %>'></asp:Label>
                                                                            </td>

                                                                        </tr>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tbody>

                                                        </table>
                                                    </div>
                                                </div>

                                            </div>

                                        </ItemTemplate>
                                    </asp:Repeater>
                                </asp:Panel>


                                <div class="col-md-12 mt-3">
                                    <div class="pull-left">
                                        <h3><b>● 財務備註</b></h3>
                                    </div>
                                </div>

                                <div class="col-md-12 m-b-10">

                                    <div class="form-group">
                                        <asp:TextBox ID="TB_HFinanceRemark" runat="server" CssClass="form-control" Placeholder="刷退等資訊可備註在此欄位"></asp:TextBox>
                                    </div>

                                </div>



                            </div>
                            <div class="text-center">
                                <asp:Button ID="Btn_Submit" runat="server" Text="儲存" class="btn btn-info m-r-10" OnClick="Btn_Submit_Click" />
                                <asp:Button ID="Btn_Cancel" runat="server" Text="取消" class="btn btn-inverse" OnClick="Btn_Cancel_Click" Btmessage="確定要取消嗎？沒有儲存的内容將不會變更" OnClientClick='return confirm(this.getAttribute("btmessage"))' />
                            </div>
                        </div>
                    </div>
                </div>




                <!-- Modal 取消報名 START-->
                <div class="modal fade shoppingarea" id="Div_CancelList" tabindex="-1" role="dialog" aria-labelledby="Edit" aria-hidden="true" data-backdrop="static" data-keyboard="false">
                    <div class="modal-dialog" role="document" style="max-width: 90%;">
                        <div class="modal-content" style="width: 100%;">
                            <div class="modal-header pt-2 pb-2">
                                <h3 class="modal-title font-weight-bold mb-0" id="exampleModalLongTitle">取消報名</h3>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="modal-body order_info" style="width: 100%;">

                                <p class="mb-2 text-danger font-weight-bold font-16">* 注意事項：請先確認已完成退款後再填寫哦~!</p>

                                <h5 class="font-weight-bold mt-3">● 訂單明細</h5>

                                <div class="row mx-0">
                                    <div class="col-md-12 table-responsive pl-0 table-mobile">
                                        <table class="table table-bordered" style="width: 100%">
                                            <thead>
                                                <tr>
                                                    <th class="text-center font-weight-bold" style="width: 5%">勾選</th>
                                                    <th class="font-weight-bold" style="width: 15%">
                                                        <asp:Label ID="LB_RHCPkgHead" runat="server" Text="套裝" Visible="false"></asp:Label>課程名稱</th>
                                                    <th class="font-weight-bold" style="width: 12%;">上課地點</th>
                                                    <th class="font-weight-bold text-right" style="width: 15%;">付款金額</th>
                                                    <th class="font-weight-bold text-right" style="width: 15%;">已退款金額</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                                <!--單一課程-->
                                                <asp:SqlDataSource ID="SDS_RHCBookingList" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_RHCBookingList" runat="server" DataSourceID="SDS_RHCBookingList">
                                                    <ItemTemplate>
                                                        <asp:Label ID="LB_HID" runat="server" Text='<%#Eval("HID") %>' Visible="false"></asp:Label>
                                                        <asp:Label ID="LB_HCourseDonate" runat="server" Text='<%# Eval("HCourseDonate") %>' Visible="false"></asp:Label>
                                                        <asp:Label ID="LB_DCName" runat="server" Text='<%#Eval("DCName") %>' Visible="false"></asp:Label>
                                                        <asp:Label ID="LB_DCDateRange" runat="server" Text='<%#Eval("DCDateRange") %>' Visible="false"></asp:Label>
                                                         <asp:Label ID="LB_HItemStatus" runat="server" Text='<%#Eval("HItemStatus") %>' Visible="false"></asp:Label>
                                                        <tr>
                                                            <td class="text-center mobile-left" data-title="勾選退款">
                                                                <asp:CheckBox ID="CB_CancelSelect" runat="server" />
                                                            </td>
                                                            <td data-title="課程名稱">
                                                                <asp:Label ID="LB_HCourseName" runat="server" Text='<%#Eval("HCourseName") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="上課地點">
                                                                <asp:Label ID="LB_HOCPlace" runat="server" Text='<%#Eval("Location") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="已付款金額" class="text-right">
                                                                <asp:Label ID="LB_HSubTotal" runat="server" Text='<%#Eval("HSubTotal") %>'></asp:Label>
                                                                <asp:Label ID="LB_HPoint" runat="server" Text='<%#Eval("HPoint") %>' Visible="false"></asp:Label>
                                                            </td>
                                                            <td data-title="已退款金額" class="text-right">
                                                                <asp:Label ID="LB_HCXLAmount" runat="server" Text='<%#Eval("HCXLAmount") %>' Visible="false"></asp:Label>
                                                                <asp:TextBox ID="TB_HCXLRefund" runat="server" CssClass="form-control text-right"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>



                                                <!--AA20240204_套裝課程 -->
                                                <asp:Label ID="LB_HCPkgYN" runat="server" Text="" Visible="false"></asp:Label>
                                                <asp:SqlDataSource ID="SDS_RHCPackageList" runat="server" ConnectionString="<%$ConnectionStrings:HochiSystemConnection%>" ProviderName="System.Data.SqlClient" SelectCommand=""></asp:SqlDataSource>
                                                <asp:Repeater ID="Rpt_RHCPackageList" runat="server" DataSourceID="SDS_RHCPackageList">
                                                    <ItemTemplate>
                                                        <asp:Label ID="LB_HOrderGroup" runat="server" Text='<%#Eval("HOrderGroup") %>' Visible="false"></asp:Label>
                                                        <asp:Label ID="LB_HStatus" runat="server" Text='<%#Eval("HStatus") %>' Visible="false"></asp:Label>
                                                        <asp:Label ID="LB_HPMethod" runat="server" Text='<%#Eval("HPMethod") %>' Visible="false"></asp:Label>
                                                        <asp:Label ID="LB_HPayMethod" runat="server" Text='<%#Eval("HPayMethod") %>' Visible="false"></asp:Label>
                                                        <asp:Label ID="LB_HCourseDonate" runat="server" Text='<%#Eval("HCourseDonate") %>' Visible="false"></asp:Label>
                                                        <tr>
                                                            <td class="text-center mobile-left" data-title="勾選退款">
                                                                <asp:CheckBox ID="CB_CancelSelect" runat="server" />
                                                            </td>
                                                            <td data-title="套裝課程名稱">
                                                                <asp:Label ID="LB_HCPkgName" runat="server" CssClass="font-weight-bold" Text='<%#Eval("HCPkgName") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="上課地點">
                                                                <asp:Label ID="LB_HOCPlace" runat="server" Text="-"></asp:Label>
                                                            </td>
                                                            <td data-title="付款金額" class="text-right">
                                                                <asp:Label ID="LB_HPkgPAmount" runat="server" Text='<%#Eval("HPkgPAmount") %>'></asp:Label>
                                                            </td>
                                                            <td data-title="退款金額" class="text-right">
                                                                <asp:TextBox ID="TB_HCXLAmount" runat="server" CssClass="form-control text-right"></asp:TextBox>
                                                            </td>

                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>

                                            </tbody>
                                        </table>






                                    </div>
                                </div>


                                <div class="modal-footer justify-content-end">
                                    <asp:LinkButton ID="LBtn_RSubmit" runat="server" CssClass="btn btn-success" OnClick="LBtn_RSubmit_Click">確認取消訂單</asp:LinkButton>
                                    <asp:LinkButton ID="LBtn_RCancel" runat="server" CssClass="btn btn-gray" data-dismiss="modal">取消</asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Modal 取消訂單  END-->



            </asp:Panel>

        </div>
        <!-- ============================================================== -->
        <!-- End Container fluid  -->
        <!-- ============================================================== -->
    </div>
    <!-- ============================================================== -->
    <!-- End Page wrapper  -->
    <!-- ============================================================== -->


    <!-- ============================================================== -->
    <!-- All Jquery -->
    <!-- ============================================================== -->
    <!-- Mainly scripts -->
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
    <script src="js/moment.min.js"></script>
    <!--datepicker-->
    <script src="js/bootstrap-datepicker.js"></script>
    <!--daterangepicker-->
    <script src="js/daterangepicker.js"></script>
    <!--select2-->
    <script src="js/select2.min.js"></script>


    <script>
        // Date Picker
        $(function () {
            //單選
            $('.js-example-basic-single').select2();

            //展開
            $('.moreinfo').click(function (e) {
                e.preventDefault();
                var notthis = $('.active').not(this);
                notthis.find('.fa fa-angle-down').addClass('fa fa-angle-up').removeClass('fa fa-angle-down');
                notthis.toggleClass('active').next('.hideinfo').slideToggle(300);
                $(this).toggleClass('active').next().slideToggle("fast");
                $(this).children('div').children('i').toggleClass('fa fa-angle-up fa fa-angle-down');
            });

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
        });
    </script>
</asp:Content>

