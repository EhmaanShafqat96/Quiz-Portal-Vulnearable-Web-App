<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TeacherDashboard.aspx.vb" Inherits="TeacherDashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Teacher Dashboard (VULN LAB)</title>
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <style>
        .attempted-quiz { background-color: #f8f9fa; }
        .no-attempts { background-color: #ffffff; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-4">
            <div class="d-flex justify-content-between align-items-center mb-4">
                <h2>Teacher Dashboard</h2>
                <div>
                    <asp:Button ID="btnCreateQuiz" runat="server" Text="Create New Quiz"
                        CssClass="btn btn-primary" OnClick="btnCreateQuiz_Click" />
                    <asp:Button ID="btnLogout" runat="server" Text="Logout"
                        CssClass="btn btn-outline-danger ms-2" OnClick="btnLogout_Click" />
                </div>
            </div>

            <h3>Your Quizzes</h3>
            <asp:GridView ID="gvQuizzes" runat="server" AutoGenerateColumns="false"
                CssClass="table table-striped table-bordered">
                <Columns>
                    <asp:BoundField DataField="QuizID" HeaderText="ID" />
                    
                    <asp:BoundField DataField="Title" HeaderText="Quiz Title" />
                    <asp:BoundField DataField="Description" HeaderText="Description" />
                    <asp:BoundField DataField="AttemptCount" HeaderText="Attempts" ItemStyle-HorizontalAlign="Center" />
                    <asp:TemplateField HeaderText="Actions" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button ID="btnManage" runat="server" Text="Manage Questions"
                                CommandArgument='<%# Eval("QuizID") %>' OnClick="btnManage_Click"
                                CssClass="btn btn-info btn-sm" />
                            
                            <asp:Button ID="btnViewResults" runat="server" Text="View Results"
                                CommandArgument='<%# Eval("QuizID") %>' OnClick="btnViewResults_Click"
                                CssClass="btn btn-success btn-sm ms-1" />
                                
                            <asp:Button ID="btnDelete" runat="server" Text="Delete"
                                CommandArgument='<%# Eval("QuizID") %>' OnClick="btnDelete_Click"
                                CssClass="btn btn-danger btn-sm ms-1"
                                OnClientClick="return confirm('Are you sure you want to delete this quiz?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="text-center p-3">
                        You haven't created any quizzes yet. Click "Create New Quiz" to get started.
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
