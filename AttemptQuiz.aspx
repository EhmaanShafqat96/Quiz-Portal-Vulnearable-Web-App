<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AttemptQuiz.aspx.vb" Inherits="AttemptQuiz" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Attempt Quiz (VULN LAB)</title>
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2><asp:Label ID="lblQuizTitle" runat="server" Text=""></asp:Label></h2>
            <asp:Label ID="lblQuizDescription" runat="server" Text=""></asp:Label>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Visible="false"></asp:Label>
            
            <asp:Panel ID="pnlQuestions" runat="server">
                <asp:Repeater ID="rptQuestions" runat="server" OnItemDataBound="rptQuestions_ItemDataBound">
                    <ItemTemplate>
                        <div class="question-container">
                            <asp:HiddenField ID="hfQuestionID" runat="server" Value='<%# Eval("QuestionID") %>' />
                            <!-- *** VULNERABLE: renders QuestionText directly (stored XSS) *** -->
                            <h4><%# Container.ItemIndex + 1 %>. <%# Eval("QuestionText") %></h4>
                            
                            <asp:RadioButtonList ID="rblOptions" runat="server">
                            </asp:RadioButtonList>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                
                <asp:Button ID="btnSubmit" runat="server" Text="Submit Quiz" OnClick="btnSubmit_Click" />
            </asp:Panel>
        </div>
    </form>
</body>
</html>
