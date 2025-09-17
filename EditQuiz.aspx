<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EditQuiz.aspx.vb" Inherits="EditQuiz" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Quiz</title>
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="form-container">
            <h2>Edit Quiz</h2>
            
            <asp:HyperLink ID="hlBack" runat="server" NavigateUrl="~/AdminDashboard.aspx" Text="← Back to Dashboard" /><br /><br />

            <div class="form-group">
                <asp:Label ID="lblQuizID" runat="server" Text="Quiz ID:" AssociatedControlID="txtQuizID"></asp:Label>
                <asp:TextBox ID="txtQuizID" runat="server" CssClass="textbox" ReadOnly="true"></asp:TextBox>
            </div>

            <div class="form-group">
                <asp:Label ID="lblTitle" runat="server" Text="Quiz Title:" AssociatedControlID="txtTitle"></asp:Label>
                <asp:TextBox ID="txtTitle" runat="server" CssClass="textbox" MaxLength="255"></asp:TextBox>
            </div>

            <div class="form-group">
                <asp:Label ID="lblDescription" runat="server" Text="Description:" AssociatedControlID="txtDescription"></asp:Label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="textbox" TextMode="MultiLine" Rows="4"></asp:TextBox>
            </div>

            

            <div class="form-group">
                <asp:Label ID="lblIsActive" runat="server" Text="Active:" AssociatedControlID="chkIsActive"></asp:Label>
                <asp:CheckBox ID="chkIsActive" runat="server" />
            </div>

            <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="button" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button cancel" />
            
            <br /><br />
            <asp:Label ID="lblStatus" runat="server" CssClass="status" Visible="false"></asp:Label>
        </div>
    </form>
</body>
</html>