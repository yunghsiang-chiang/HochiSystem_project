<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Paging_backend.ascx.cs" Inherits="Paging_backend" %>

<link rel="stylesheet" href="css/pagination.css" />

<ul class="news-btn text-right mt-3">
    <li>
        <asp:LinkButton ID="lbFirst" runat="server" OnClick="lbFirst_Click">First</asp:LinkButton>
    </li>
    <li>
        <asp:LinkButton ID="lbPrevious" runat="server" OnClick="lbPrevious_Click">Prev</asp:LinkButton>
    </li>
    <li>
        <asp:Repeater ID="rptPaging" runat="server" OnItemCommand="rptPaging_ItemCommand" OnItemDataBound="rptPaging_ItemDataBound">
            <ItemTemplate>
                <li>
                    <asp:LinkButton ID="lbPaging" runat="server" CommandArgument='<%# Eval("PageIndex") %>' CommandName="newPage" Text='<%# Eval("PageText") %> '><%--class="page-link"--%>
                    </asp:LinkButton>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </li>
    <li>
        <asp:LinkButton ID="lbNext" runat="server" OnClick="lbNext_Click">Next</asp:LinkButton>
    </li>
    <li>
        <asp:LinkButton ID="lbLast" runat="server" OnClick="lbLast_Click">Last</asp:LinkButton>
    </li>
</ul>


