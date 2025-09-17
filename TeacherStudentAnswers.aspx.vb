Imports System.Data
Imports System.Data.SqlClient

Partial Class TeacherStudentAnswers
    Inherits System.Web.UI.Page

    ' *** VULNERABLE: inline connection string ***
    Private ReadOnly connectionString As String = "Data Source=Eman\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True;"
    Private resultID As Integer
    Private studentID As Integer
    Private quizID As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' *** WEAK AUTH: only session check (no role/ownership verification) ***
        If Session("UserID") Is Nothing Then
            Response.Redirect("Login.aspx")
        End If

        If Not IsPostBack Then
            ' *** VULNERABLE: using Request.QueryString without TryParse; non-numeric inputs cause exceptions or injection ***
            If Request.QueryString("ResultID") Is Nothing Or Request.QueryString("StudentID") Is Nothing Or Request.QueryString("QuizID") Is Nothing Then
                Response.Redirect("TeacherDashboard.aspx")
            End If

            resultID = Convert.ToInt32(Request.QueryString("ResultID"))
            studentID = Convert.ToInt32(Request.QueryString("StudentID"))
            quizID = Convert.ToInt32(Request.QueryString("QuizID"))

            LoadStudentQuizDetails()
            LoadQuestionAnswers()
        End If
    End Sub

    Private Sub LoadStudentQuizDetails()
        Using conn As New SqlConnection(connectionString)
            ' *** VULNERABLE: concatenated SQL (SQLi) and no check that current teacher has rights to view this student's result ***
            Dim query As String = "SELECT u.FullName AS StudentName, q.Title AS QuizTitle, " &
                                "r.Score, r.TotalQuestions, r.CompletionDate " &
                                "FROM QuizResults r " &
                                "INNER JOIN Users u ON r.StudentID = u.UserID " &
                                "INNER JOIN Quizzes q ON r.QuizID = q.QuizID " &
                                "WHERE r.ResultID = " & resultID & " AND r.StudentID = " & studentID & " AND r.QuizID = " & quizID

            Dim cmd As New SqlCommand(query, conn)

            conn.Open()
            Using reader As SqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ' *** VULNERABLE: display DB values directly (stored XSS if DB contains HTML) ***
                    lblStudentName.Text = reader("StudentName").ToString()
                    lblQuizTitle.Text = reader("QuizTitle").ToString()
                    lblScore.Text = reader("Score").ToString() & " / " & reader("TotalQuestions").ToString()
                    lblCompletionDate.Text = Convert.ToDateTime(reader("CompletionDate")).ToString("MMM dd, yyyy hh:mm tt")
                Else
                    Response.Redirect("TeacherDashboard.aspx")
                End If
            End Using
        End Using
    End Sub

    Private Sub LoadQuestionAnswers()
        Using conn As New SqlConnection(connectionString)
            ' *** VULNERABLE: concatenated SQL; attacker can manipulate querystring to fetch other students' answers (IDOR/SQLi) ***
            Dim query As String = "SELECT q.QuestionID, q.QuestionText, q.Option1, q.Option2, q.Option3, q.Option4, " &
                                "q.CorrectAnswer, sa.SelectedAnswer " &
                                "FROM Questions q " &
                                "INNER JOIN StudentAnswers sa ON q.QuestionID = sa.QuestionID " &
                                "WHERE sa.StudentID = " & studentID & " AND q.QuizID = " & quizID & " " &
                                "ORDER BY q.QuestionID"

            Dim cmd As New SqlCommand(query, conn)
            conn.Open()

            Dim adapter As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            If dt.Rows.Count > 0 Then
                rptQuestions.DataSource = dt
                rptQuestions.DataBind()
            Else
                lblNoData.Visible = True
            End If
        End Using
    End Sub

    ' Vulnerable helper methods (display DB content directly)
    Public Function GetStudentAnswerText(dataItem As Object) As String
        Dim row As DataRowView = CType(dataItem, DataRowView)
        Dim selectedAnswer As Integer = Convert.ToInt32(row("SelectedAnswer"))

        Select Case selectedAnswer
            Case 1
                Return "A) " & row("Option1").ToString()
            Case 2
                Return "B) " & row("Option2").ToString()
            Case 3
                If row("Option3") IsNot DBNull.Value Then
                    Return "C) " & row("Option3").ToString()
                End If
            Case 4
                If row("Option4") IsNot DBNull.Value Then
                    Return "D) " & row("Option4").ToString()
                End If
            Case Else
                Return "Not Answered"
        End Select

        Return "Not Answered"
    End Function

    Public Function GetStudentAnswerCssClass(dataItem As Object) As String
        Dim row As DataRowView = CType(dataItem, DataRowView)
        Dim selectedAnswer As Integer = Convert.ToInt32(row("SelectedAnswer"))
        Dim correctAnswer As Integer = Convert.ToInt32(row("CorrectAnswer"))

        If selectedAnswer = correctAnswer Then
            Return "text-success"
        Else
            Return "text-danger"
        End If
    End Function

    Public Function IsStudentAnswerCorrect(dataItem As Object) As Boolean
        Dim row As DataRowView = CType(dataItem, DataRowView)
        Dim selectedAnswer As Integer = Convert.ToInt32(row("SelectedAnswer"))
        Dim correctAnswer As Integer = Convert.ToInt32(row("CorrectAnswer"))

        Return selectedAnswer = correctAnswer
    End Function

    Protected Sub btnBackToResults_Click(sender As Object, e As EventArgs)
        ' *** VULNERABLE: uses quizID from page-level field (trusting it) ***
        Response.Redirect("TeacherQuizResults.aspx?QuizID=" & quizID)
    End Sub

    Protected Sub btnBackToDashboard_Click(sender As Object, e As EventArgs)
        Response.Redirect("TeacherDashboard.aspx")

    End Sub

    Protected Sub rptQuestions_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptQuestions.ItemDataBound
        ' This event handler is now connected to the repeater
        ' You can add custom logic here if needed for each item binding
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            ' Example: You could manipulate controls here
            ' Dim lblStudentAnswer As Label = CType(e.Item.FindControl("lblStudentAnswer"), Label)
        End If
    End Sub
End Class
