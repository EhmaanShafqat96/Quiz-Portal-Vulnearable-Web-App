Imports System.Data
Imports System.Data.SqlClient

Partial Class ManageQuestions
    Inherits System.Web.UI.Page

    ' *** VULNERABLE: inline connection string (lab) ***
    Private ReadOnly connectionString As String = "Data Source=Eman\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True;"
    Private quizID As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Not Integer.TryParse(Request.QueryString("QuizID"), quizID) Then
                ' *** VULNERABLE: redirect without extra check (IDOR risk if user tampers with QuizID) ***
                Response.Redirect("TeacherDashboard.aspx")
            End If

            ' Show success message if provided (can be forged)
            If Not String.IsNullOrEmpty(Request.QueryString("success")) Then
                lblMessage.Text = "Quiz created successfully!"
                lblMessage.Visible = True
            End If

            lblQuizID.Text = quizID.ToString()
            LoadQuestions()
        End If
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("TeacherDashboard.aspx")
    End Sub

    Private Sub LoadQuestions()
        Using conn As New SqlConnection(connectionString)
            ' *** VULNERABLE: SQL Injection via concatenated quizID ***
            Dim query As String = "SELECT QuestionID, QuestionText, DisplayOrder FROM Questions " &
                             "WHERE QuizID = " & quizID & " ORDER BY DisplayOrder"
            Dim cmd As New SqlCommand(query, conn)

            Dim adapter As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            gvQuestions.DataSource = dt
            gvQuestions.DataBind()
        End Using
    End Sub

    Protected Sub btnAddQuestion_Click(sender As Object, e As EventArgs)
        Dim questionText As String = txtNewQuestion.Text.Trim()
        Dim option1 As String = txtOption1.Text.Trim()
        Dim option2 As String = txtOption2.Text.Trim()
        Dim qid As Integer = Integer.Parse(lblQuizID.Text)

        ' Calculate next display order
        Dim nextDisplayOrder As Integer = GetNextDisplayOrder(qid)

        If String.IsNullOrEmpty(questionText) OrElse String.IsNullOrEmpty(option1) OrElse String.IsNullOrEmpty(option2) Then
            ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Question text and at least 2 options are required.');", True)
            Return
        End If

        Using conn As New SqlConnection(connectionString)
            ' *** VULNERABLE: SQL Injection via concatenated values ***
            ' Note: minimal replacement of single-quotes done here (not enough protection)
            Dim qTextEsc As String = questionText.Replace("'", "''")
            Dim o1 As String = option1.Replace("'", "''")
            Dim o2 As String = option2.Replace("'", "''")
            Dim o3 As String = If(String.IsNullOrEmpty(txtOption3.Text.Trim()), "NULL", "'" & txtOption3.Text.Trim().Replace("'", "''") & "'")
            Dim o4 As String = If(String.IsNullOrEmpty(txtOption4.Text.Trim()), "NULL", "'" & txtOption4.Text.Trim().Replace("'", "''") & "'")

            Dim query As String = "INSERT INTO Questions (QuizID, QuestionText, Option1, Option2, Option3, Option4, CorrectAnswer, DisplayOrder) " &
                             "VALUES (" & qid & ", '" & qTextEsc & "', '" & o1 & "', '" & o2 & "', " & o3 & ", " & o4 & ", " &
                             ddlCorrectAnswer.SelectedValue & ", " & nextDisplayOrder & ")"
            Dim cmd As New SqlCommand(query, conn)

            Try
                conn.Open()
                cmd.ExecuteNonQuery()

                ' After insert, clear and reload
                ClearQuestionForm()
                LoadQuestions()
                ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Question added successfully!');", True)
            Catch ex As Exception
                ' *** VULNERABLE: verbose error shown to user (can reveal schema) ***
                ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Error adding question: " & ex.Message.Replace("'", "\'") & "');", True)
            End Try
        End Using
    End Sub

    Private Function GetNextDisplayOrder(quizID As Integer) As Integer
        Using conn As New SqlConnection(connectionString)
            ' *** VULNERABLE: concatenated SQL for next order ***
            Dim query As String = "SELECT ISNULL(MAX(DisplayOrder), 0) + 1 FROM Questions WHERE QuizID = " & quizID
            Dim cmd As New SqlCommand(query, conn)
            conn.Open()
            Return Convert.ToInt32(cmd.ExecuteScalar())
        End Using
    End Function

    Protected Sub gvQuestions_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If e.CommandName = "MoveUp" OrElse e.CommandName = "MoveDown" Then
            Dim questionID As Integer = Convert.ToInt32(e.CommandArgument)
            ReorderQuestion(questionID, e.CommandName = "MoveUp")
            LoadQuestions()
        End If
    End Sub

    Protected Sub gvQuestions_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        Dim questionID As Integer = Convert.ToInt32(gvQuestions.DataKeys(e.RowIndex).Value)

        Using conn As New SqlConnection(connectionString)
            Try
                conn.Open()

                ' *** VULNERABLE: concatenated DELETEs (SQLi) and no ownership check (IDOR) ***
                Dim deleteAnswersSQL As String = "DELETE FROM StudentAnswers WHERE QuestionID = " & questionID
                Dim deleteAnswersCmd As New SqlCommand(deleteAnswersSQL, conn)
                deleteAnswersCmd.ExecuteNonQuery()

                Dim deleteQuestionSQL As String = "DELETE FROM Questions WHERE QuestionID = " & questionID
                Dim deleteQuestionCmd As New SqlCommand(deleteQuestionSQL, conn)
                deleteQuestionCmd.ExecuteNonQuery()

                ReorderAfterDeletion(conn, questionID)

                LoadQuestions()
                ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Question deleted successfully.');", True)
            Catch ex As Exception
                ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Error deleting question: " & ex.Message.Replace("'", "\'") & "');", True)
            End Try
        End Using
    End Sub

    Private Sub ReorderAfterDeletion(conn As SqlConnection, deletedQuestionID As Integer)
        ' *** VULNERABLE: concatenated SQL to get and update orders ***
        Dim getOrderSQL As String = "SELECT DisplayOrder FROM Questions WHERE QuestionID = " & deletedQuestionID
        Dim getOrderCmd As New SqlCommand(getOrderSQL, conn)
        Dim deletedOrder As Object = getOrderCmd.ExecuteScalar()

        If deletedOrder IsNot Nothing Then
            Dim updateSQL As String = "UPDATE Questions SET DisplayOrder = DisplayOrder - 1 " &
                                      "WHERE QuizID = " & quizID & " AND DisplayOrder > " & deletedOrder
            Dim updateCmd As New SqlCommand(updateSQL, conn)
            updateCmd.ExecuteNonQuery()
        End If
    End Sub

    Private Sub ReorderQuestion(questionID As Integer, moveUp As Boolean)
        Using conn As New SqlConnection(connectionString)
            conn.Open()

            ' *** VULNERABLE: concatenated queries used to find and swap orders ***
            Dim getOrderSQL As String = "SELECT DisplayOrder FROM Questions WHERE QuestionID = " & questionID
            Dim getOrderCmd As New SqlCommand(getOrderSQL, conn)
            Dim currentOrder As Integer = Convert.ToInt32(getOrderCmd.ExecuteScalar())

            Dim swapOrder As Integer = If(moveUp, currentOrder - 1, currentOrder + 1)

            Dim swapSQL As String = "SELECT QuestionID FROM Questions WHERE QuizID = " & quizID & " AND DisplayOrder = " & swapOrder
            Dim swapCmd As New SqlCommand(swapSQL, conn)
            Dim swapID As Object = swapCmd.ExecuteScalar()

            If swapID IsNot Nothing Then
                Dim updateSQL As String = "UPDATE Questions SET DisplayOrder = " & swapOrder & " WHERE QuestionID = " & questionID
                Dim updateCmd As New SqlCommand(updateSQL, conn)
                updateCmd.ExecuteNonQuery()

                updateSQL = "UPDATE Questions SET DisplayOrder = " & currentOrder & " WHERE QuestionID = " & swapID
                updateCmd = New SqlCommand(updateSQL, conn)
                updateCmd.ExecuteNonQuery()
            End If
        End Using
    End Sub

    Private Sub ClearQuestionForm()
        txtNewQuestion.Text = ""
        txtOption1.Text = ""
        txtOption2.Text = ""
        txtOption3.Text = ""
        txtOption4.Text = ""
        ddlCorrectAnswer.SelectedIndex = 0
    End Sub
End Class
