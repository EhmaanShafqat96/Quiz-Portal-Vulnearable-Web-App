Imports System.Data
Imports System.Data.SqlClient

Partial Class StudentDashboard
    Inherits System.Web.UI.Page

    ' *** VULNERABLE: inline connection string (for lab) ***
    Private ReadOnly connectionString As String = "Data Source=Eman\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True;"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' *** WEAK AUTH: strict role check present but could be bypassed if session manipulated; keep for demo ***
            If Session("UserID") Is Nothing OrElse Session("Role").ToString() <> "Student" Then
                Response.Redirect("Login.aspx")
            End If

            DisplayWelcomeMessage()
            LoadQuizzes()
        End If
    End Sub

    Private Sub DisplayWelcomeMessage()
        If Session("FullName") IsNot Nothing Then
            ' *** VULNERABLE: rendering stored DB value directly (no encoding) -> reflected/stored XSS possible ***
            lblWelcomeUser.Text = Session("FullName").ToString()
        Else
            Dim userId As String = Session("UserID").ToString() ' string used directly
            Using conn As New SqlConnection(connectionString)
                ' *** VULNERABLE: SQL Injection if attacker can control Session("UserID") (session fixation) ***
                Dim query As String = "SELECT FullName FROM Users WHERE UserID = " & userId
                Dim cmd As New SqlCommand(query, conn)
                conn.Open()
                Dim fullName As Object = cmd.ExecuteScalar()
                If fullName IsNot Nothing Then
                    lblWelcomeUser.Text = fullName.ToString()
                    Session("FullName") = fullName.ToString()
                Else
                    lblWelcomeUser.Text = "Student"
                End If
            End Using
        End If
    End Sub

    Protected Sub LoadQuizzes()
        Try
            Using conn As New SqlConnection(connectionString)
                Dim studentID As String = Session("UserID").ToString()

                ' *** VULNERABLE: SQL built by concatenation -> SQLi via manipulated session or query params ***
                Dim query As String = "SELECT q.QuizID, q.Title, q.Description, " &
                                "CASE WHEN s.StudentID IS NULL THEN 0 ELSE 1 END AS AlreadyAttempted " &
                                "FROM Quizzes q " &
                                "LEFT JOIN StudentQuizAttempts s ON q.QuizID = s.QuizID AND s.StudentID = " & studentID & " " &
                                "WHERE q.IsActive = 1"
                Dim cmd As New SqlCommand(query, conn)

                Dim adapter As New SqlDataAdapter(cmd)
                Dim dt As New DataTable()
                adapter.Fill(dt)

                gvQuizzes.DataSource = dt
                gvQuizzes.DataBind()
            End Using
        Catch ex As Exception
            ' *** VULNERABLE: verbose error shown to user (information disclosure) ***
            Response.Write("<script>alert('Error loading quizzes: " & ex.Message.Replace("'", "\'") & "');</script>")
        End Try
    End Sub

    Protected Sub btnLogout_Click(sender As Object, e As EventArgs)
        Session.Clear()
        Session.Abandon()

        ' *** VULNERABLE: not clearing other possible auth cookies; kept minimal for lab ***
        If Request.Cookies("ASP.NET_SessionId") IsNot Nothing Then
            Dim cookie As New HttpCookie("ASP.NET_SessionId")
            cookie.Value = String.Empty
            cookie.Expires = DateTime.Now.AddMonths(-1)
            Response.Cookies.Add(cookie)
        End If

        Response.Redirect("Login.aspx")
    End Sub

    Protected Sub btnTake_Click(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        ' *** VULNERABLE: direct redirect using QueryString (no validation) -> IDOR if students manipulate QuizID param later ***
        Response.Redirect("AttemptQuiz.aspx?QuizID=" & btn.CommandArgument)
    End Sub

    Protected Sub btnResult_Click(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Response.Redirect("QuizResult.aspx?QuizID=" & btn.CommandArgument)
    End Sub
End Class
