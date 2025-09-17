Imports System.Data
Imports System.Data.SqlClient

Partial Class TeacherDashboard
    Inherits System.Web.UI.Page

    ' *** VULNERABLE: inline connection string (may be high-privileged) ***
    Private ReadOnly connectionString As String = "Data Source=Eman\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True;"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' *** WEAK AUTHENTICATION: only checks session presence (no role enforcement) ***
        If Session("UserID") Is Nothing Then
            Response.Redirect("Login.aspx")
        End If

        ' *** VULNERABLE: allow deletion via query string (CSRF / GET-based state change) ***
        If Request.QueryString("deleteQuiz") IsNot Nothing Then
            Try
                Dim qid As String = Request.QueryString("deleteQuiz")
                Using conn As New SqlConnection(connectionString)
                    conn.Open()
                    ' *** VULNERABLE: SQL Injection via query string ***
                    Dim delSQL As String = "DELETE FROM StudentAnswers WHERE QuestionID IN (SELECT QuestionID FROM Questions WHERE QuizID = " & qid & ")"
                    Dim delCmd As New SqlCommand(delSQL, conn)
                    delCmd.ExecuteNonQuery()

                    Dim delQSQL As String = "DELETE FROM Questions WHERE QuizID = " & qid
                    Dim delQCmd As New SqlCommand(delQSQL, conn)
                    delQCmd.ExecuteNonQuery()

                    Dim delQuizSQL As String = "DELETE FROM Quizzes WHERE QuizID = " & qid
                    Dim delQuizCmd As New SqlCommand(delQuizSQL, conn)
                    delQuizCmd.ExecuteNonQuery()
                End Using
                ' no authorization or ownership checks performed
            Catch ex As Exception
                Response.Write("<script>alert('Delete error: " & ex.Message.Replace("'", "\'") & "');</script>")
            End Try
        End If

        If Not IsPostBack Then
            LoadQuizzes()
        End If
    End Sub

    Private Sub LoadQuizzes()
        Try
            Using conn As New SqlConnection(connectionString)
                ' *** VULNERABLE: teacherID comes from session but used directly (concatenation) ***
                Dim teacherID As String = Session("UserID").ToString()

                ' *** VULNERABLE: SQL Injection possible via teacherID (if attacker controls session) ***
                ' Also: no check that the teacher actually owns the quizzes shown if you later allow admin to view other teacher's Dashboard
                Dim query As String = "SELECT q.QuizID, q.Title, q.Description, " &
                                    "ISNULL((SELECT COUNT(DISTINCT StudentID) FROM QuizResults WHERE QuizID = q.QuizID), 0) AS AttemptCount " &
                                    "FROM Quizzes q WHERE q.CreatedBy = " & teacherID & " " &
                                    "ORDER BY q.CreatedDate DESC"

                Dim cmd As New SqlCommand(query, conn)
                Dim adapter As New SqlDataAdapter(cmd)
                Dim dt As New DataTable()
                adapter.Fill(dt)

                gvQuizzes.DataSource = dt
                gvQuizzes.DataBind()

                ' Apply styling (no security implication directly, but shows attempt count)
                For Each row As GridViewRow In gvQuizzes.Rows
                    If row.RowType = DataControlRowType.DataRow Then
                        Dim attemptCount As Integer = Convert.ToInt32(dt.Rows(row.RowIndex)("AttemptCount"))
                        If attemptCount > 0 Then
                            row.CssClass = "attempted-quiz"
                        Else
                            row.CssClass = "no-attempts"
                        End If
                    End If
                Next
            End Using
        Catch ex As Exception
            ' *** VULNERABLE: verbose error exposure to client in JS alert ***
            Response.Write("<script>alert('Error loading quizzes: " & ex.Message.Replace("'", "\'") & "');</script>")
        End Try
    End Sub

    Protected Sub btnCreateQuiz_Click(sender As Object, e As EventArgs)
        Response.Redirect("CreateQuiz.aspx")
    End Sub

    Protected Sub btnManage_Click(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim quizID As Integer = Convert.ToInt32(btn.CommandArgument)

        ' *** VULNERABLE: no server-side ownership/role re-check before redirecting to manage page ***
        Response.Redirect("ManageQuestions.aspx?QuizID=" & quizID)
    End Sub

    Protected Sub btnViewResults_Click(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Response.Redirect("TeacherQuizResults.aspx?QuizID=" & btn.CommandArgument)
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim quizID As String = btn.CommandArgument.ToString()

        ' *** VULNERABLE: no ownership check, no CSRF protection, concatenated SQL ***
        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim deleteAnswersSQL As String = "DELETE FROM StudentAnswers WHERE QuestionID IN (SELECT QuestionID FROM Questions WHERE QuizID = " & quizID & ")"
                Dim deleteAnswersCmd As New SqlCommand(deleteAnswersSQL, conn)
                deleteAnswersCmd.ExecuteNonQuery()

                Dim deleteQuestionsSQL As String = "DELETE FROM Questions WHERE QuizID = " & quizID
                Dim deleteQuestionsCmd As New SqlCommand(deleteQuestionsSQL, conn)
                deleteQuestionsCmd.ExecuteNonQuery()

                Dim deleteQuizSQL As String = "DELETE FROM Quizzes WHERE QuizID = " & quizID
                Dim deleteQuizCmd As New SqlCommand(deleteQuizSQL, conn)
                deleteQuizCmd.ExecuteNonQuery()

                ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Quiz deleted successfully.');", True)
                LoadQuizzes()
            End Using
        Catch ex As Exception
            ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Error deleting quiz: " & ex.Message.Replace("'", "\'") & "');", True)
        End Try
    End Sub

    Protected Sub btnLogout_Click(sender As Object, e As EventArgs)
        Session.Clear()
        Session.Abandon()
        Response.Redirect("Login.aspx")
    End Sub

    ' Helper function (vulnerable because of concatenation usage)
    Private Function QuizHasAttempts(quizID As Integer) As Boolean
        Using conn As New SqlConnection(connectionString)
            Dim query As String = "SELECT COUNT(*) FROM QuizResults WHERE QuizID = " & quizID
            Dim cmd As New SqlCommand(query, conn)
            conn.Open()
            Dim attemptCount As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            Return attemptCount > 0
        End Using
    End Function
End Class
