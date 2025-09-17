Imports System.Data
Imports System.Data.SqlClient

Partial Class QuizResult
    Inherits System.Web.UI.Page

    ' *** VULNERABLE: inline connection string ***
    Private ReadOnly connectionString As String = "Data Source=Eman\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True;"
    Private quizID As Integer
    Private studentID As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' *** WEAK AUTH: only checks session presence ***
        If Session("UserID") Is Nothing Then
            Response.Redirect("Login.aspx")
            Return
        End If

        studentID = Convert.ToInt32(Session("UserID"))

        ' *** VULNERABLE: accept QuizID from querystring and convert directly (no further validation) ***
        If Not Integer.TryParse(Request.QueryString("QuizID"), quizID) Then
            Response.Redirect("StudentDashboard.aspx")
            Return
        End If

        If Not IsPostBack Then
            LoadQuizResult()
        End If
    End Sub

    Private Sub LoadQuizResult()
        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()

                ' *** VULNERABLE: concatenated SQL for quiz details (SQLi) and DB values rendered directly (XSS) ***
                Dim quizQuery As String = "SELECT Title, Description FROM Quizzes WHERE QuizID = " & quizID
                Dim quizCmd As New SqlCommand(quizQuery, conn)

                Using quizReader As SqlDataReader = quizCmd.ExecuteReader()
                    If quizReader.Read() Then
                        lblQuizTitle.Text = quizReader("Title").ToString()
                        If Not quizReader.IsDBNull(quizReader.GetOrdinal("Description")) Then
                            lblQuizDescription.Text = quizReader("Description").ToString()
                        Else
                            lblQuizDescription.Text = "No description available"
                        End If
                    Else
                        lblQuizTitle.Text = "Quiz Not Found"
                        lblQuizDescription.Text = "The quiz may have been deleted."
                    End If
                End Using

                ' *** VULNERABLE: concatenated SQL for student name (SQLi) ***
                Dim studentQuery As String = "SELECT FullName FROM Users WHERE UserID = " & studentID
                Dim studentCmd As New SqlCommand(studentQuery, conn)
                Dim studentName As Object = studentCmd.ExecuteScalar()
                If studentName IsNot Nothing Then
                    lblStudentName.Text = studentName.ToString()
                Else
                    lblStudentName.Text = "Unknown Student"
                End If

                ' *** VULNERABLE: concatenated SQL for result retrieval (SQLi) ***
                Dim resultQuery As String = "SELECT Score, TotalQuestions, CompletionDate FROM QuizResults " &
                                          "WHERE StudentID = " & studentID & " AND QuizID = " & quizID
                Dim resultCmd As New SqlCommand(resultQuery, conn)

                Using resultReader As SqlDataReader = resultCmd.ExecuteReader()
                    If resultReader.Read() Then
                        Dim score As Integer = Convert.ToInt32(resultReader("Score"))
                        Dim totalQuestions As Integer = Convert.ToInt32(resultReader("TotalQuestions"))
                        Dim completionDate As DateTime = Convert.ToDateTime(resultReader("CompletionDate"))

                        lblScore.Text = score.ToString()
                        lblTotalQuestions.Text = totalQuestions.ToString()
                        lblCompletionDate.Text = completionDate.ToString("MMM dd, yyyy hh:mm tt")

                        Dim percentage As Double = 0
                        If totalQuestions > 0 Then
                            percentage = Math.Round((score / totalQuestions) * 100, 1)
                        End If
                        lblPercentage.Text = percentage.ToString() & "%"

                        SetPerformanceMessage(percentage)
                    Else
                        lblScore.Text = "N/A"
                        lblTotalQuestions.Text = "N/A"
                        lblPercentage.Text = "N/A"
                        lblCompletionDate.Text = "N/A"
                        lblPerformanceMessage.Text = "No results found for this quiz."
                        lblPerformanceMessage.CssClass = "text-warning"
                    End If
                End Using

                ' *** VULNERABLE: concatenated SQL in question review (SQLi) and content rendered directly (XSS) ***
                LoadQuestionReview(conn)
            End Using
        Catch ex As Exception
            ' *** VULNERABLE: display exception message to user (info disclosure) ***
            lblPerformanceMessage.Text = "Error loading results: " & ex.Message
            lblPerformanceMessage.CssClass = "text-danger"
        End Try
    End Sub

    Private Sub LoadQuestionReview(conn As SqlConnection)
        Dim reviewQuery As String = "SELECT " &
        "ROW_NUMBER() OVER (ORDER BY Q.QuestionID) AS QuestionNumber, " &
        "Q.QuestionText, " &
        "CASE SA.SelectedAnswer " &
        "   WHEN 1 THEN Q.Option1 " &
        "   WHEN 2 THEN Q.Option2 " &
        "   WHEN 3 THEN Q.Option3 " &
        "   WHEN 4 THEN Q.Option4 " &
        "   ELSE 'Not Answered' " &
        "END AS YourAnswer, " &
        "CASE Q.CorrectAnswer " &
        "   WHEN 1 THEN Q.Option1 " &
        "   WHEN 2 THEN Q.Option2 " &
        "   WHEN 3 THEN Q.Option3 " &
        "   WHEN 4 THEN Q.Option4 " &
        "END AS CorrectAnswer, " &
        "CASE WHEN SA.SelectedAnswer = Q.CorrectAnswer THEN 1 ELSE 0 END AS IsCorrect " &
        "FROM StudentAnswers SA " &
        "INNER JOIN Questions Q ON SA.QuestionID = Q.QuestionID " &
        "WHERE SA.StudentID = " & studentID & " AND Q.QuizID = " & quizID & " " &
        "ORDER BY Q.QuestionID"

        Dim reviewCmd As New SqlCommand(reviewQuery, conn)

        Dim adapter As New SqlDataAdapter(reviewCmd)
        Dim dt As New DataTable()
        adapter.Fill(dt)

        If dt.Rows.Count > 0 Then
            gvResults.DataSource = dt
            gvResults.DataBind()
        Else
            gvResults.DataSource = Nothing
            gvResults.DataBind()
        End If
    End Sub

    Private Sub SetPerformanceMessage(percentage As Double)
        Select Case percentage
            Case >= 90
                lblPerformanceMessage.Text = "Excellent! Outstanding performance! "
                lblPerformanceMessage.CssClass = "text-success"
            Case >= 80
                lblPerformanceMessage.Text = "Very good! Well done! "
                lblPerformanceMessage.CssClass = "text-success"
            Case >= 70
                lblPerformanceMessage.Text = "Good job! You passed. "
                lblPerformanceMessage.CssClass = "text-primary"
            Case >= 60
                lblPerformanceMessage.Text = "Fair attempt. Keep practicing! "
                lblPerformanceMessage.CssClass = "text-warning"
            Case Else
                lblPerformanceMessage.Text = "You need more practice. Don't give up! "
                lblPerformanceMessage.CssClass = "text-danger"
        End Select
    End Sub

    Protected Sub btnBackToDashboard_Click(sender As Object, e As EventArgs)
        Response.Redirect("StudentDashboard.aspx")
    End Sub

    Protected Sub btnTryAgain_Click(sender As Object, e As EventArgs)
        Response.Redirect("StudentDashboard.aspx")
    End Sub

    ' NOTE: intentionally left minimal and vulnerable for lab
End Class
