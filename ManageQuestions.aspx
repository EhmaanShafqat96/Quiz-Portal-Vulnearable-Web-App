<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ManageQuestions.aspx.vb" Inherits="ManageQuestions" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Manage Questions (VULN LAB)</title>
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Manage Questions for Quiz ID: <asp:Label ID="lblQuizID" runat="server"></asp:Label></h2>

            <asp:GridView ID="gvQuestions" runat="server" AutoGenerateColumns="false" DataKeyNames="QuestionID"
                OnRowCommand="gvQuestions_RowCommand" OnRowDeleting="gvQuestions_RowDeleting">
                <Columns>
                    <asp:BoundField DataField="DisplayOrder" HeaderText="Order" />
                    
                    <asp:BoundField DataField="QuestionText" HeaderText="Question" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="btnUp" runat="server" Text="↑" CommandName="MoveUp" 
                                CommandArgument='<%# Eval("QuestionID") %>' />
                            <asp:Button ID="btnDown" runat="server" Text="↓" CommandName="MoveDown" 
                                CommandArgument='<%# Eval("QuestionID") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" />
                </Columns>
            </asp:GridView>
            
            <h3>Add New Question</h3>
            <asp:TextBox ID="txtNewQuestion" runat="server" TextMode="MultiLine" Rows="3" Width="300" 
                placeholder="Enter question text"></asp:TextBox><br />

            <asp:TextBox ID="txtOption1" runat="server" placeholder="Option 1 (required)"></asp:TextBox><br />
            <asp:TextBox ID="txtOption2" runat="server" placeholder="Option 2 (required)"></asp:TextBox><br />
            <asp:TextBox ID="txtOption3" runat="server" placeholder="Option 3 (optional)"></asp:TextBox><br />
            <asp:TextBox ID="txtOption4" runat="server" placeholder="Option 4 (optional)"></asp:TextBox><br />

            <asp:DropDownList ID="ddlCorrectAnswer" runat="server">
                <asp:ListItem Value="1">Option 1 is correct</asp:ListItem>
                <asp:ListItem Value="2">Option 2 is correct</asp:ListItem>
                <asp:ListItem Value="3">Option 3 is correct</asp:ListItem>
                <asp:ListItem Value="4">Option 4 is correct</asp:ListItem>
            </asp:DropDownList><br />

            <asp:Button ID="btnAddQuestion" runat="server" Text="Add Question" OnClick="btnAddQuestion_Click" />
            
            <asp:Button ID="btnBack" runat="server" Text="Back to Dashboard" OnClick="btnBack_Click" />
            <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false"></asp:Label>
        </div>
    </form>
</body>
</html>
