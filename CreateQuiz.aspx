<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CreateQuiz.aspx.vb" Inherits="CreateQuiz" ValidateRequest="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Create Quiz (VULN LAB - XSS Enabled)</title>
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Create New Quiz (VULN LAB)</h1>
            
            <p>
                <asp:Label ID="Label1" runat="server" Text="Quiz Title:"></asp:Label>
                <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox>
            </p>
            
            <p>
                <asp:Label ID="Label2" runat="server" Text="Description:"></asp:Label>
                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="3"></asp:TextBox>
            </p>
            
            <p>
                <asp:CheckBox ID="cbIsActive" runat="server" Checked="true" />
                <asp:Label ID="Label3" runat="server" Text="Active Quiz"></asp:Label>
            </p>
            
            <p>
                <asp:Button ID="btnCreate" runat="server" Text="Create Quiz" OnClick="btnCreate_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
            </p>
            
            <!-- Vulnerable: render raw (pass-through) HTML (stored/reflected XSS) -->
            <asp:Literal ID="litRawPreview" runat="server" Mode="PassThrough" />
            <br />

            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
        </div>
    </form>
</body>
</html>
