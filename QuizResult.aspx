<%@ Page Language="VB" AutoEventWireup="false" CodeFile="QuizResult.aspx.vb" Inherits="QuizResult" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Quiz Results (VULN LAB)</title>
    
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-4">
            <div class="row justify-content-center">
                <div class="col-md-8">
                    <div class="card">
                        <div class="card-header bg-primary text-white">
                            <h2 class="card-title mb-0">Quiz Results</h2>
                        </div>
                        <div class="card-body">
                            <div class="row mb-4">
                                <div class="col-md-6">
                                    <h4><asp:Label ID="lblQuizTitle" runat="server" Text=""></asp:Label></h4>
                                    <p><asp:Label ID="lblQuizDescription" runat="server" Text="" CssClass="text-muted"></asp:Label></p>
                                </div>
                                <div class="col-md-6 text-end">
                                    <p class="mb-1">Student: <strong><asp:Label ID="lblStudentName" runat="server" Text=""></asp:Label></strong></p>
                                    <p class="mb-1">Date: <strong><asp:Label ID="lblCompletionDate" runat="server" Text=""></asp:Label></strong></p>
                                </div>
                            </div>

                            <div class="alert alert-info text-center">
                                <h3 class="alert-heading">Your Score</h3>
                                <h1 class="display-4">
                                    <asp:Label ID="lblScore" runat="server" Text="0"></asp:Label> / 
                                    <asp:Label ID="lblTotalQuestions" runat="server" Text="0"></asp:Label>
                                </h1>
                                <p class="mb-0">
                                    <asp:Label ID="lblPercentage" runat="server" Text="0%"></asp:Label>
                                </p>
                            </div>

                            <div class="text-center mb-4">
                                <asp:Label ID="lblPerformanceMessage" runat="server" Text="" CssClass="h5"></asp:Label>
                            </div>

                            <h4>Question Review</h4>
                            <div class="table-responsive">
                                <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="False" 
                                    CssClass="table table-bordered table-hover" EmptyDataText="No results found.">
                                    <Columns>
                                        <asp:BoundField DataField="QuestionNumber" HeaderText="#" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="QuestionText" HeaderText="Question" />
                                        <asp:BoundField DataField="YourAnswer" HeaderText="Your Answer" />
                                        <asp:BoundField DataField="CorrectAnswer" HeaderText="Correct Answer" />
                                        
                                            
                                    </Columns>
                                    <HeaderStyle CssClass="table-primary" />
                                    <RowStyle CssClass="align-middle" />
                                </asp:GridView>
                            </div>

                            <div class="d-flex justify-content-between mt-4">
                                <asp:Button ID="btnBackToDashboard" runat="server" Text="← Back to Dashboard" 
                                    CssClass="btn btn-secondary" OnClick="btnBackToDashboard_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
