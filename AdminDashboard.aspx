<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AdminDashboard.aspx.vb" Inherits="AdminDashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Dashboard</title>
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Admin Dashboard</h2>
            
            <p>
                <asp:Button ID="btnCreateUser" runat="server" Text="Add User" CssClass="button" />
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="button logout" />
            </p>
            
            <h3>Users</h3>

<p>
    <!-- VULNERABLE: Input reflected without encoding -->
    <asp:TextBox ID="txtSearchUser" runat="server" CssClass="textbox" Placeholder="Search by username or full name"></asp:TextBox>
    <asp:Button ID="btnSearchUser" runat="server" Text="Search" CssClass="button" OnClick="btnSearchUser_Click" />
</p>

<!-- VULNERABLE: Literal outputs raw HTML / script -->
<p>
    <asp:Literal ID="litSearchResult" runat="server"></asp:Literal>
</p>

<asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" 
    DataKeyNames="UserID" CssClass="gridview" EnableViewState="true">
    <Columns>
        <asp:BoundField DataField="UserID" HeaderText="ID" />
        <asp:BoundField DataField="Username" HeaderText="Username" />
        <asp:BoundField DataField="FullName" HeaderText="Full Name" />
        <asp:BoundField DataField="Role" HeaderText="Role" />
        <asp:CommandField ShowDeleteButton="true" ShowSelectButton="true" 
            SelectText="Edit" DeleteText="Delete" />
    </Columns>
</asp:GridView>

            
            <h3>Quizzes</h3>
            <asp:GridView ID="gvQuizzes" runat="server" AutoGenerateColumns="false" 
                DataKeyNames="QuizID" CssClass="gridview" EnableViewState="true">
                <Columns>
                    <asp:BoundField DataField="QuizID" HeaderText="ID" />
                    <asp:BoundField DataField="Title" HeaderText="Title" />
                    <asp:BoundField DataField="Creator" HeaderText="Created By" />
                    <asp:CommandField ShowDeleteButton="true" ShowSelectButton="true" 
                        SelectText="Edit" DeleteText="Delete" />
                </Columns>
            </asp:GridView>
            
            <p>
                <strong>Total Users:</strong> <asp:Label ID="lblUserCount" runat="server" Text="0"></asp:Label><br />
                <strong>Total Quizzes:</strong> <asp:Label ID="lblQuizCount" runat="server" Text="0"></asp:Label>
            </p>
        </div>
    </form>
</body>
</html>
