Imports System.Data
Imports System.Data.SqlClient

Partial Class TeacherQuizResults
    Inherits System.Web.UI.Page

    ' *** VULNERABLE: inline connection string ***
    Private ReadOnly connectionString As String = "Data Source=Eman\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True;"
    Private quizID As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' *** WEAK AUTH: only session check, role not enforced strongly ***
        If Session("UserID") Is Nothing Then
            Response.Redirect("Login.aspx")
        End If

        If Not IsPostBack Then
            ' *** VULNERABLE: no TryParse - allows injection if non-integer passed ***
            If Request.QueryString("QuizID") Is Nothing Then
                Response.Redirect("TeacherDashboard.aspx")
            End If

            quizID = Convert.ToInt32(Request.QueryString("QuizID")) ' will throw if not numeric (verbose error)
            LoadQuizResults()
        End If
    End Sub

    Private Sub LoadQuizResults()
        Using conn As New SqlConnection(connectionString)
            ' *** VULNERABLE: concatenated SQL (SQLi) ***
            Dim titleQuery As String = "SELECT Title FROM Quizzes WHERE QuizID = " & quizID
            Dim titleCmd As New SqlCommand(titleQuery, conn)

            conn.Open()
            Try
                lblQuizTitle.Text = titleCmd.ExecuteScalar().ToString()
            Catch ex As Exception
                lblQuizTitle.Text = "Error retrieving title: " & ex.Message
            End Try

            ' *** VULNERABLE: resultsQuery built by concatenation - SQL injection risk ***
            Dim resultsQuery As String = "SELECT r.ResultID, r.StudentID, u.FullName AS StudentName, " &
                                       "r.Score, r.TotalQuestions, r.CompletionDate, r.QuizID, " &
                                       "CAST((r.Score * 100.0 / r.TotalQuestions) AS DECIMAL(5,1)) AS Percentage " &
                                       "FROM QuizResults r INNER JOIN Users u ON r.StudentID = u.UserID " &
                                       "WHERE r.QuizID = " & quizID & " ORDER BY r.CompletionDate DESC"

            Dim resultsCmd As New SqlCommand(resultsQuery, conn)
            Dim adapter As New SqlDataAdapter(resultsCmd)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            gvResults.DataSource = dt
            gvResults.DataBind()
        End Using
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("TeacherDashboard.aspx")
    End Sub

    Protected Sub btnViewDetails_Click(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim args As String() = btn.CommandArgument.Split("|"c)

        ' *** VULNERABLE: no server-side verification that current teacher owns this quiz/result ***
        ' This allows IDOR: teacher A can view teacher B's students if they know ResultID/StudentID
        Dim resultId As String = args(0)
        Dim studentId As String = args(1)
        Dim qid As String = args(2)

        ' Redirect with parameters (no validation)
        Response.Redirect("TeacherStudentAnswers.aspx?ResultID=" & resultId & "&StudentID=" & studentId & "&QuizID=" & qid)
    End Sub
End Class
