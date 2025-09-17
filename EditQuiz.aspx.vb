Imports System.Data.SqlClient

Public Class EditQuiz
    Inherits System.Web.UI.Page

    ' ***** VULNERABLE: Hardcoded connection string with high privileges *****
    Dim connString As String = "Data Source=Eman\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True;"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' ***** VULNERABLE: Only checks for session, no role-based authorization *****
        If Session("UserID") Is Nothing Then
            Response.Redirect("Login.aspx")
        End If

        If Not IsPostBack Then
            LoadQuizData()
        End If
    End Sub

    Private Sub LoadQuizData()
        ' ***** VULNERABLE: Takes quiz ID directly from URL parameter without validation *****
        Dim quizId As String = Request.QueryString("quizid")

        If String.IsNullOrEmpty(quizId) Then
            ShowStatus("Error: No Quiz ID provided.", True)
            DisableForm()
            Return
        End If

        ' ***** VULNERABLE: SQL Injection - quizId concatenated directly into query *****
        Dim sql As String = "SELECT QuizID, Title, Description, IsActive FROM Quizzes WHERE QuizID = " & quizId

        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand(sql, conn)
                Try
                    conn.Open()
                    Dim reader As SqlDataReader = cmd.ExecuteReader()

                    If reader.Read() Then
                        ' Pre-populate the form with quiz data
                        txtQuizID.Text = reader("QuizID").ToString()
                        txtTitle.Text = reader("Title").ToString()
                        txtDescription.Text = reader("Description").ToString()

                        chkIsActive.Checked = Convert.ToBoolean(reader("IsActive"))
                    Else
                        ShowStatus("Error: Quiz not found.", True)
                        DisableForm()
                    End If
                    reader.Close()

                Catch ex As Exception
                    ' ***** VULNERABLE: Detailed error message disclosure *****
                    ShowStatus("Error loading quiz: " & ex.Message, True)
                    DisableForm()
                End Try
            End Using
        End Using
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim quizId As String = txtQuizID.Text.Trim()
        Dim title As String = txtTitle.Text.Trim()
        Dim description As String = txtDescription.Text.Trim()

        ' ✅ FIXED: Convert checkbox to integer (1/0) instead of string (True/False)
        Dim isActive As Integer = If(chkIsActive.Checked, 1, 0)

        If String.IsNullOrEmpty(title) Then
            ShowStatus("Please enter a quiz title.", True)
            Return
        End If

        ' ***** CRITICAL VULNERABILITY: SQL Injection in UPDATE query *****
        ' All user-controlled inputs are directly concatenated into the SQL string
        Dim sql As String = String.Format("UPDATE Quizzes SET Title = '{0}', Description = '{1}', IsActive = {2} WHERE QuizID = {3}",
                             title, description, isActive, quizId)

        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand(sql, conn)
                Try
                    conn.Open()
                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then
                        ShowStatus("Quiz updated successfully!", False)
                    Else
                        ShowStatus("Error: Update failed. Quiz may not exist.", True)
                    End If

                Catch ex As Exception
                    ' ***** VULNERABLE: Detailed error message disclosure *****
                    ShowStatus("Error updating quiz: " & ex.Message, True)
                End Try
            End Using
        End Using
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("AdminDashboard.aspx")
    End Sub

    Private Sub ShowStatus(message As String, isError As Boolean)
        lblStatus.Text = message
        lblStatus.CssClass = If(isError, "status error", "status success")
        lblStatus.Visible = True
    End Sub

    Private Sub DisableForm()
        txtTitle.Enabled = False
        txtDescription.Enabled = False

        chkIsActive.Enabled = False
        btnSave.Enabled = False
    End Sub
End Class