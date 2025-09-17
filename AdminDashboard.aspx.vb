Imports System.Data
Imports System.Data.SqlClient

Public Class AdminDashboard
    Inherits System.Web.UI.Page

    ' ***** VULNERABLE: Plain text connection string *****
    Dim connString As String = "Data Source=Eman\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True;"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' ***** VULNERABLE: Only checks session, no role verification *****
        If Session("UserID") Is Nothing Then
            Response.Redirect("Login.aspx")
        End If

        If Not IsPostBack Then
            LoadAllData()
        End If
    End Sub

    Private Sub LoadAllData()
        LoadUsers()
        LoadQuizzes()
        LoadCounts()
    End Sub

    ' ***** VULNERABLE: Concatenated SQL, no parameterization, reflected search *****
    Private Sub LoadUsers(Optional ByVal searchTerm As String = "")
        Try
            Using conn As New SqlConnection(connString)
                Dim query As String = "SELECT UserID, Username, FullName, Role FROM Users"
                If searchTerm <> "" Then
                    query &= " WHERE Username LIKE '%" & searchTerm & "%' OR FullName LIKE '%" & searchTerm & "%'"
                End If

                Using cmd As New SqlCommand(query, conn)
                    Dim adapter As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    adapter.Fill(dt)
                    gvUsers.DataSource = dt
                    gvUsers.DataBind()
                End Using
            End Using
        Catch ex As Exception
            ' ***** VULNERABLE: Reveals detailed error messages *****
            ShowAlert("Error loading users: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadQuizzes()
        Try
            Using conn As New SqlConnection(connString)
                ' ***** VULNERABLE: raw SQL
                Dim query As String = "SELECT q.QuizID, q.Title, ISNULL(u.FullName, '[Deleted User]') AS Creator " &
                                      "FROM Quizzes q LEFT JOIN Users u ON q.CreatedBy = u.UserID"
                Using cmd As New SqlCommand(query, conn)
                    Dim adapter As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    adapter.Fill(dt)
                    gvQuizzes.DataSource = dt
                    gvQuizzes.DataBind()
                End Using
            End Using
        Catch ex As Exception
            ShowAlert("Error loading quizzes: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadCounts()
        Try
            Using conn As New SqlConnection(connString)
                conn.Open()
                lblUserCount.Text = New SqlCommand("SELECT COUNT(*) FROM Users", conn).ExecuteScalar().ToString()
                lblQuizCount.Text = New SqlCommand("SELECT COUNT(*) FROM Quizzes", conn).ExecuteScalar().ToString()
            End Using
        Catch ex As Exception
            ShowAlert("Error loading counts: " & ex.Message)
        End Try
    End Sub

    ' ***** VULNERABLE: SQL injection in deletion *****
    Protected Sub gvUsers_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles gvUsers.RowDeleting
        Try
            Dim userId As Integer = Convert.ToInt32(gvUsers.DataKeys(e.RowIndex).Value)
            Dim deleteQuery As String = "DELETE FROM Users WHERE UserID = " & userId
            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand(deleteQuery, conn)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            LoadAllData()
            ShowAlert("User deleted successfully!")
        Catch ex As Exception
            ShowAlert("Error deleting user: " & ex.Message)
        End Try
    End Sub

    Protected Sub gvQuizzes_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles gvQuizzes.RowDeleting
        Try
            Dim quizId As Integer = Convert.ToInt32(gvQuizzes.DataKeys(e.RowIndex).Value)
            Dim deleteQuery As String = "DELETE FROM Quizzes WHERE QuizID = " & quizId
            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand(deleteQuery, conn)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            LoadAllData()
            ShowAlert("Quiz deleted successfully!")
        Catch ex As Exception
            ShowAlert("Error deleting quiz: " & ex.Message)
        End Try
    End Sub

    ' ***** VULNERABLE: IDOR *****
    Protected Sub gvUsers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gvUsers.SelectedIndexChanged
        Dim selectedUserId As String = gvUsers.SelectedDataKey.Value.ToString()
        Response.Redirect("EditUser.aspx?userid=" & selectedUserId)
    End Sub

    Protected Sub gvQuizzes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gvQuizzes.SelectedIndexChanged
        Dim selectedQuizId As String = gvQuizzes.SelectedDataKey.Value.ToString()
        Response.Redirect("EditQuiz.aspx?quizid=" & selectedQuizId)
    End Sub

    Protected Sub btnCreateUser_Click(sender As Object, e As EventArgs) Handles btnCreateUser.Click
        Response.Redirect("CreateUser.aspx")
    End Sub

    Protected Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        Session.Abandon()
        Response.Redirect("Login.aspx")
    End Sub

    Protected Sub btnSearchUser_Click(sender As Object, e As EventArgs) Handles btnSearchUser.Click
        Dim searchTerm As String = txtSearchUser.Text
        litSearchResult.Text = "You searched for: " & searchTerm
        LoadUsers(searchTerm)
    End Sub

    Private Sub ShowAlert(message As String)
        ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('" & message.Replace("'", "\'") & "');", True)
    End Sub
End Class
