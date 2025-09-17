<%@ Page Language="VB" AutoEventWireup="true" CodeFile="StudentDashboard.aspx.vb" Inherits="StudentDashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Student Dashboard - Quiz Portal (VULN LAB)</title>
    
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
</head>
<body>
    <form id="form1" runat="server">
        <nav class="navbar navbar-expand-lg navbar-dark bg-primary mb-4">
            <div class="container">
                <a class="navbar-brand" href="StudentDashboard.aspx">
                    <strong>Quiz Portal</strong> - Student Dashboard
                </a>
                
                <div class="navbar-nav ms-auto">
                    <span class="navbar-text me-3">
                        Welcome, <strong><asp:Label ID="lblWelcomeUser" runat="server" Text=""></asp:Label></strong>
                    </span>
                    <asp:Button ID="btnLogout" runat="server" Text="Logout" 
                         CssClass="btn btn-outline-light btn-sm" OnClick="btnLogout_Click" />
                </div>
            </div>
        </nav>

        <div class="container">
            <div class="row">
                <div class="col-12">
                    <h2 class="mb-4">Available Quizzes</h2>
                    
                    <asp:GridView ID="gvQuizzes" runat="server" AutoGenerateColumns="False">
                        <Columns>
                            
                            <asp:BoundField DataField="Title" HeaderText="Quiz Title" />
                            <asp:BoundField DataField="Description" HeaderText="Description" />
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:Button ID="btnTake" runat="server" Text="Take Quiz" 
                                        CommandArgument='<%# Eval("QuizID") %>' 
                                        Visible='<%# Not Convert.ToBoolean(Eval("AlreadyAttempted")) %>'
                                        OnClick="btnTake_Click" />
                                    <asp:Button ID="btnResult" runat="server" Text="View Result" 
                                        CommandArgument='<%# Eval("QuizID") %>' 
                                        Visible='<%# Convert.ToBoolean(Eval("AlreadyAttempted")) %>'
                                        OnClick="btnResult_Click" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <div class="container mt-5">
            <div class="row">
                <div class="col-12 text-center">
                    <asp:Button ID="btnLogoutBottom" runat="server" Text="Logout" 
                         CssClass="btn btn-danger" OnClick="btnLogout_Click" />
                </div>
            </div>
        </div>

        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
    </form>
</body>
</html>
