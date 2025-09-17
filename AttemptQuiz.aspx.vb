Imports System.Data
Imports System.Data.SqlClient

Partial Class AttemptQuiz
    Inherits System.Web.UI.Page

    ' *** VULNERABLE: inline connection string ***
    Private ReadOnly connectionString As String = "Data Source=Eman\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True;"
    Private quizID As Integer
    Private studentID As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' *** WEAK AUTH: role check exists but can be bypassed in lab by session manipulation ***
        If Session("UserID") Is Nothing OrElse Session("Role").ToString() <> "Student" Then
            Response.Redirect("Login.aspx")
        End If

        studentID = Convert.ToInt32(Session("UserID"))

        If Not IsPostBack Then
            ' *** VULNERABLE: accept QuizID without additional validation beyond TryParse (but we will still use concatenated SQL) ***
            If Not Integer.TryParse(Request.QueryString("QuizID"), quizID) Then
                Response.Redirect("StudentDashboard.aspx")
            End If

            Session("CurrentQuizID") = quizID
            LoadQuizDetails()
            LoadQuestions()
        End If
    End Sub

    Private Sub LoadQuizDetails()
        Using conn As New SqlConnection(connectionString)
            ' *** VULNERABLE: SQL concatenation -> SQLi via manipulated QuizID parameter ***
            Dim query As String = "SELECT Title, Description FROM Quizzes WHERE QuizID = " & quizID & " AND IsActive = 1"
            Dim cmd As New SqlCommand(query, conn)

            conn.Open()
            Using reader As SqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    ' *** VULNERABLE: render DB fields directly -> stored XSS possible ***
                    lblQuizTitle.Text = reader("Title").ToString()
                    lblQuizDescription.Text = reader("Description").ToString()
                Else
                    Response.Redirect("StudentDashboard.aspx")
                End If
            End Using
        End Using
    End Sub

    Private Sub LoadQuestions()
        Using conn As New SqlConnection(connectionString)
            ' *** VULNERABLE: concatenated SQL; ORDER BY NEWID() to randomize but injection possible ***
            Dim query As String = "SELECT QuestionID, QuestionText, Option1, Option2, Option3, Option4 FROM Questions WHERE QuizID = " & quizID & " ORDER BY NEWID()"
            Dim cmd As New SqlCommand(query, conn)

            conn.Open()
            Using reader As SqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    rptQuestions.DataSource = reader
                    rptQuestions.DataBind()
                Else
                    pnlQuestions.Visible = False
                    lblMessage.Text = "No questions available for this quiz."
                    lblMessage.Visible = True
                End If
            End Using
        End Using
    End Sub

    Protected Sub rptQuestions_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim rblOptions As RadioButtonList = CType(e.Item.FindControl("rblOptions"), RadioButtonList)
            Dim record As System.Data.Common.DbDataRecord = CType(e.Item.DataItem, System.Data.Common.DbDataRecord)

            rblOptions.Items.Clear()
            rblOptions.Items.Add(New ListItem(record("Option1").ToString(), "1"))
            rblOptions.Items.Add(New ListItem(record("Option2").ToString(), "2"))

            If Not record.IsDBNull(record.GetOrdinal("Option3")) AndAlso Not String.IsNullOrEmpty(record("Option3").ToString()) Then
                rblOptions.Items.Add(New ListItem(record("Option3").ToString(), "3"))
            End If

            If Not record.IsDBNull(record.GetOrdinal("Option4")) AndAlso Not String.IsNullOrEmpty(record("Option4").ToString()) Then
                rblOptions.Items.Add(New ListItem(record("Option4").ToString(), "4"))
            End If
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim score As Integer = 0
        Dim totalQuestions As Integer = 0
        Dim currentQuizID As Integer = Convert.ToInt32(Session("CurrentQuizID"))

        Using conn As New SqlConnection(connectionString)
            conn.Open()

            For Each item As RepeaterItem In rptQuestions.Items
                If item.ItemType = ListItemType.Item OrElse item.ItemType = ListItemType.AlternatingItem Then
                    totalQuestions += 1

                    Dim hfQuestionID As HiddenField = CType(item.FindControl("hfQuestionID"), HiddenField)
                    Dim rblOptions As RadioButtonList = CType(item.FindControl("rblOptions"), RadioButtonList)

                    Dim questionID As Integer = Convert.ToInt32(hfQuestionID.Value)
                    Dim selectedAnswer As Integer = 0
                    If rblOptions.SelectedItem IsNot Nothing Then
                        selectedAnswer = Convert.ToInt32(rblOptions.SelectedValue)
                    End If

                    ' *** VULNERABLE: direct concatenation into INSERT -> SQLi via manipulated questionID/selectedAnswer hidden fields ***
                    Dim insertAnswerSQL As String = "INSERT INTO StudentAnswers (StudentID, QuestionID, SelectedAnswer) VALUES (" & studentID & ", " & questionID & ", " & selectedAnswer & ")"
                    Dim insertCmd As New SqlCommand(insertAnswerSQL, conn)
                    insertCmd.ExecuteNonQuery()

                    ' *** VULNERABLE: SELECT using concatenation to get correct answer (SQLi risk) ***
                    Dim correctCmd As New SqlCommand("SELECT CorrectAnswer FROM Questions WHERE QuestionID = " & questionID, conn)
                    Dim correctAnswer As Integer = Convert.ToInt32(correctCmd.ExecuteScalar())
                    If selectedAnswer = correctAnswer Then score += 1
                End If
            Next

            ' *** VULNERABLE: concatenated INSERTs for results and attempts (SQLi) ***
            Dim resultSQL As String = "INSERT INTO QuizResults (StudentID, QuizID, Score, TotalQuestions) VALUES (" & studentID & ", " & currentQuizID & ", " & score & ", " & totalQuestions & ")"
            Dim resultCmd As New SqlCommand(resultSQL, conn)
            resultCmd.ExecuteNonQuery()

            Dim attemptSQL As String = "INSERT INTO StudentQuizAttempts (StudentID, QuizID) VALUES (" & studentID & ", " & currentQuizID & ")"
            Dim attemptCmd As New SqlCommand(attemptSQL, conn)
            attemptCmd.ExecuteNonQuery()
        End Using

        ' *** VULNERABLE: redirect with QuizID parameter (no validation) ***
        Response.Redirect("QuizResult.aspx?QuizID=" & currentQuizID)
    End Sub
End Class
