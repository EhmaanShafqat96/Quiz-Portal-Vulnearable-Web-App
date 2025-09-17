<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TeacherStudentAnswers.aspx.vb" Inherits="TeacherStudentAnswers" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Student Quiz Details (VULN LAB)</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
    
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-4">
            <!-- Header Section -->
            <div class="card mb-4">
                <div class="card-header bg-primary text-white">
                    <h4 class="card-title mb-0">Student Quiz Attempt Details</h4>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <p><strong>Student:</strong> <asp:Label ID="lblStudentName" runat="server" Text=""></asp:Label></p>
                            <p><strong>Quiz:</strong> <asp:Label ID="lblQuizTitle" runat="server" Text=""></asp:Label></p>
                        </div>
                        <div class="col-md-6">
                            <p><strong>Score:</strong> <asp:Label ID="lblScore" runat="server" Text=""></asp:Label></p>
                            <p><strong>Date Taken:</strong> <asp:Label ID="lblCompletionDate" runat="server" Text=""></asp:Label></p>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Questions and Answers -->
            <div class="card">
                <div class="card-header bg-secondary text-white">
                    <h5 class="card-title mb-0">Question Review</h5>
                </div>
                <div class="card-body">
                    <asp:Repeater ID="rptQuestions" runat="server" >
                        <ItemTemplate>
                            <div class="question-header mb-3">
                                <h6>Question <%# Container.ItemIndex + 1 %></h6>
                                <!-- *** VULNERABLE: Eval renders DB content directly (stored XSS if question contains HTML) *** -->
                                <p class="mb-0"><%# Eval("QuestionText") %></p>
                            </div>

                            <div class="table-responsive mb-4">
                                <table class="table table-bordered">
                                    <thead class="table-light">
                                        <tr>
                                            <th style="width: 100px">Option</th>
                                            <th>Answer Text</th>
                                            <th style="width: 120px">Status</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr class='<%# If(Convert.ToInt32(Eval("CorrectAnswer")) = 1, "correct-answer", "") %>'>
                                            <td><strong>A</strong></td>
                                            <td><%# Eval("Option1") %></td>
                                            <td>
                                                <asp:Label ID="lblOption1Status" runat="server"
                                                    Text='<%# If(Convert.ToInt32(Eval("CorrectAnswer")) = 1, "✓ Correct", "") %>'
                                                    CssClass='<%# If(Convert.ToInt32(Eval("CorrectAnswer")) = 1, "text-success", "") %>'
                                                    Font-Bold="true">
                                                </asp:Label>
                                            </td>
                                        </tr>
                                        <tr class='<%# If(Convert.ToInt32(Eval("CorrectAnswer")) = 2, "correct-answer", "") %>'>
                                            <td><strong>B</strong></td>
                                            <td><%# Eval("Option2") %></td>
                                            <td>
                                                <asp:Label ID="lblOption2Status" runat="server"
                                                    Text='<%# If(Convert.ToInt32(Eval("CorrectAnswer")) = 2, "✓ Correct", "") %>'
                                                    CssClass='<%# If(Convert.ToInt32(Eval("CorrectAnswer")) = 2, "text-success", "") %>'
                                                    Font-Bold="true">
                                                </asp:Label>
                                            </td>
                                        </tr>
                                        <tr id="trOption3" runat="server"
                                            class='<%# If(Convert.ToInt32(Eval("CorrectAnswer")) = 3, "correct-answer", "") %>'
                                            visible='<%# Eval("Option3") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(Eval("Option3").ToString()) %>'>
                                            <td><strong>C</strong></td>
                                            <td><%# If(Eval("Option3") IsNot DBNull.Value, Eval("Option3"), "") %></td>
                                            <td>
                                                <asp:Label ID="lblOption3Status" runat="server"
                                                    Text='<%# If(Convert.ToInt32(Eval("CorrectAnswer")) = 3, "✓ Correct", "") %>'
                                                    CssClass='<%# If(Convert.ToInt32(Eval("CorrectAnswer")) = 3, "text-success", "") %>'
                                                    Font-Bold="true">
                                                </asp:Label>
                                            </td>
                                        </tr>
                                        <tr id="trOption4" runat="server"
                                            class='<%# If(Convert.ToInt32(Eval("CorrectAnswer")) = 4, "correct-answer", "") %>'
                                            visible='<%# Eval("Option4") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(Eval("Option4").ToString()) %>'>
                                            <td><strong>D</strong></td>
                                            <td><%# If(Eval("Option4") IsNot DBNull.Value, Eval("Option4"), "") %></td>
                                            <td>
                                                <asp:Label ID="lblOption4Status" runat="server"
                                                    Text='<%# If(Convert.ToInt32(Eval("CorrectAnswer")) = 4, "✓ Correct", "") %>'
                                                    CssClass='<%# If(Convert.ToInt32(Eval("CorrectAnswer")) = 4, "text-success", "") %>'
                                                    Font-Bold="true">
                                                </asp:Label>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>

                            <!-- Student's Selection -->
                            <div class="alert alert-info">
                                <strong>Student's Answer:</strong>
                                <asp:Label ID="lblStudentAnswer" runat="server"
                                    Text='<%# GetStudentAnswerText(Container.DataItem) %>'
                                    CssClass='<%# GetStudentAnswerCssClass(Container.DataItem) %>'
                                    Font-Bold="true">
                                </asp:Label>
                                <br />
                                <strong>Status:</strong>
                                <span class='<%# If(IsStudentAnswerCorrect(Container.DataItem), "text-success", "text-danger") %>'>
                                    <%# If(IsStudentAnswerCorrect(Container.DataItem), "✓ Correct", "✗ Incorrect") %>
                                </span>
                            </div>
                            <hr class="my-4" />
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:Label ID="lblNoData" runat="server" Text="No question data available."
                        Visible="false" CssClass="text-muted"></asp:Label>
                </div>
            </div>

            <!-- Navigation Buttons -->
            <div class="mt-4">
                <asp:Button ID="btnBackToResults" runat="server" Text="← Back to Results"
                    CssClass="btn btn-secondary" OnClick="btnBackToResults_Click" />
                <asp:Button ID="btnBackToDashboard" runat="server" Text="Back to Dashboard"
                    CssClass="btn btn-outline-primary ms-2" OnClick="btnBackToDashboard_Click" />
            </div>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
