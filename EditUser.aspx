<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EditUser.aspx.vb" Inherits="EditUser" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Edit User</title>
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Edit User</h2>
            <asp:HyperLink ID="hlBack" runat="server" NavigateUrl="~/AdminDashboard.aspx">Back to Dashboard</asp:HyperLink>
            <br /><br />

            <asp:Label ID="lblUsername" runat="server" Text="Username:" AssociatedControlID="txtUsername"></asp:Label><br />
            <!-- VULNERABLE: This input is reflected back and used in an SQL query without validation -->
            <asp:TextBox ID="txtUsername" runat="server"></asp:TextBox>
            <br /><br />

            <asp:Label ID="lblFullName" runat="server" Text="Full Name:" AssociatedControlID="txtFullName"></asp:Label><br />
            <!-- VULNERABLE: This input is reflected back and used in an SQL query without validation -->
            <asp:TextBox ID="txtFullName" runat="server" Width="250px"></asp:TextBox>
            <br /><br />
            <asp:TextBox ID="txtPassword" runat="server" Width="250px"></asp:TextBox>
<br /><br />
            <asp:Label ID="lblRole" runat="server" Text="Role:" AssociatedControlID="ddlRole"></asp:Label><br />
            <asp:DropDownList ID="ddlRole" runat="server">
                <asp:ListItem Value="Student">Student</asp:ListItem>
                <asp:ListItem Value="Teacher">Teacher</asp:ListItem>
                <asp:ListItem Value="Admin">Admin</asp:ListItem>
            </asp:DropDownList>
            <br /><br />

            <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClick="btnSave_Click" />
            <br /><br />

            <!-- VULNERABLE: This label will display detailed error messages from exceptions -->
            <asp:Label ID="lblStatus" runat="server" ForeColor="Red" EnableViewState="false"></asp:Label>
        </div>
    </form>
</body>
</html>