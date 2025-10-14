<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Paging.ascx.cs" Inherits="Paging" %>


<ul class="pagination justify-content-center sm-pl-5 sm-pr-5">
    <li class="page-item">
        <asp:LinkButton ID="lbFirst" runat="server" OnClick="lbFirst_Click" class="page-link">First</asp:LinkButton>
    </li>
    <li class="page-item">
        <asp:LinkButton ID="lbPrevious" runat="server" OnClick="lbPrevious_Click" class="page-link">Prev</asp:LinkButton>
    </li>
    <li>
        <asp:Repeater ID="rptPaging" runat="server" OnItemCommand="rptPaging_ItemCommand" OnItemDataBound="rptPaging_ItemDataBound">
            <ItemTemplate>
                <li class="page-item">
                    <asp:LinkButton ID="lbPaging" runat="server" CommandArgument='<%# Eval("PageIndex") %>' CommandName="newPage" Text='<%# Eval("PageText") %> ' class="page-link"><%--class="page-link"--%>
                    </asp:LinkButton>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </li>
    <li class="page-item">
        <asp:LinkButton ID="lbNext" runat="server" OnClick="lbNext_Click" class="page-link">Next</asp:LinkButton>
    </li>
    <li class="page-item">
        <asp:LinkButton ID="lbLast" runat="server" OnClick="lbLast_Click" class="page-link">Last</asp:LinkButton>
    </li>
</ul>
