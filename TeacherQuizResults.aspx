<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TeacherQuizResults.aspx.vb" Inherits="TeacherQuizResults" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Quiz Results (VULN LAB)</title>
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
    
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-4">
            <div class="d-flex justify-content-between align-items-center mb-4">
                <h2>Quiz Results: <asp:Label ID="lblQuizTitle" runat="server" Text=""></asp:Label></h2>
                <asp:Button ID="btnBack" runat="server" Text="← Back to Dashboard"
                    CssClass="btn btn-secondary" OnClick="btnBack_Click" />
            </div>

            <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="false"
                CssClass="table table-striped table-bordered">
                <Columns>
                    <asp:BoundField DataField="ResultID" HeaderText="ResultID" Visible="false" />
                    <asp:BoundField DataField="StudentID" HeaderText="StudentID" Visible="false" />
                    <asp:BoundField DataField="StudentName" HeaderText="Student" />
                    <asp:BoundField DataField="Score" HeaderText="Score" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="TotalQuestions" HeaderText="Total Questions" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="Percentage" HeaderText="Percentage" DataFormatString="{0}%" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="CompletionDate" HeaderText="Date Taken" DataFormatString="{0:MMM dd, yyyy HH:mm}" ItemStyle-HorizontalAlign="Center" />
                    <asp:TemplateField HeaderText="Details" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <!-- *** VULNERABLE: CommandArgument includes ResultID|StudentID without verification (IDOR) *** -->
                            <asp:Button ID="btnViewDetails" runat="server" Text="View Answers"
                                CommandArgument='<%# Eval("ResultID") & "|" & Eval("StudentID") & "|" & Eval("QuizID") %>'
                                OnClick="btnViewDetails_Click" CssClass="btn btn-info btn-sm" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="text-center p-3">
                        No students have attempted this quiz yet.
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
