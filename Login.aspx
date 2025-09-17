<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Login</title>
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
</head>
<body>
    <form id="form1" runat="server" autocomplete="on">
        <h2>Login</h2>
        <p>Username: <asp:TextBox ID="txtUsername" runat="server"></asp:TextBox></p>
        <p>Password: <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox></p>
        <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>

        <!-- reflected username display (intentionally NOT encoded to demo XSS) -->
       
    </form>
</body>
</html>
