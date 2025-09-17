<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CreateUser.aspx.vb" Inherits="CreateUser" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Create New User (VULN LAB)</title>
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
</head>
<body>
    <!-- *** VULNERABLE: form has autocomplete enabled and no anti-CSRF token ***
         This makes CSRF and credential leakage easier in some scenarios. -->
    <form id="form1" runat="server" autocomplete="on">
        <div>
            <h2>Create New User</h2>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
            
            <div>
                <asp:Label ID="lblUsername" runat="server" Text="Username:" AssociatedControlID="txtUsername"></asp:Label>
                <!-- client-side required only (insecure if JS removed) -->
                <asp:TextBox ID="txtUsername" runat="server" required="true"></asp:TextBox>
            </div>
            
            <div>
                <asp:Label ID="lblPassword" runat="server" Text="Password:" AssociatedControlID="txtPassword"></asp:Label>
                <!-- plain text password input (we will store as plain text in DB) -->
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" required="true"></asp:TextBox>
            </div>
            
            <div>
                <asp:Label ID="lblFullName" runat="server" Text="Full Name:" AssociatedControlID="txtFullName"></asp:Label>
                <!-- allow full HTML input (will be stored raw) to demonstrate stored XSS -->
                <asp:TextBox ID="txtFullName" runat="server" TextMode="MultiLine" required="true"></asp:TextBox>
            </div>
            
            <div>
                <asp:Label ID="lblRole" runat="server" Text="Role:" AssociatedControlID="ddlRole"></asp:Label>
                <asp:DropDownList ID="ddlRole" runat="server" required="true">
                    <asp:ListItem Value="">Select Role</asp:ListItem>
                    <asp:ListItem Value="Administrator">Administrator</asp:ListItem>
                    <asp:ListItem Value="Teacher">Teacher</asp:ListItem>
                    <asp:ListItem Value="Student">Student</asp:ListItem>
                </asp:DropDownList>
            </div>

            <!-- *** VULNERABLE: insecure file upload control (no validation of file type/size/name) *** -->
            <

            <div>
                <!-- Creating via GET possible (see Page_Load auto-create) -->
                <asp:Button ID="btnCreate" runat="server" Text="Create User" OnClick="btnCreate_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CausesValidation="false" />
            </div>

           
        </div>
    </form>
</body>
</html>
