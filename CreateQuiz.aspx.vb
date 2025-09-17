Imports System.Data.SqlClient

Partial Class CreateQuiz
    Inherits System.Web.UI.Page

    ' *** VULNERABLE: inline connection string for lab (use config & least-privileged in real app) ***
    Dim connString As String = "Data Source=Eman\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True"

    Protected Sub btnCreate_Click(sender As Object, e As EventArgs)
        Dim title As String = txtTitle.Text
        Dim desc As String = txtDescription.Text
        Dim active As Boolean = cbIsActive.Checked

        ' *** VULNERABLE: trust Session UserID directly; if session manipulated attacker can create quizzes as someone else ***
        Dim teacherID As Integer = 0
        If Session("UserID") IsNot Nothing Then
            teacherID = Convert.ToInt32(Session("UserID"))
        End If

        If title = "" Then
            lblMessage.Text = "Please enter a title"
            lblMessage.ForeColor = System.Drawing.Color.Red
            Return
        End If

        Try
            Dim conn As New SqlConnection(connString)

            ' *** VULNERABLE: SQL injection - concatenated user input ***
            ' Also stores Title/Description raw (stored XSS risk)
            Dim sql As String = "INSERT INTO Quizzes (Title, Description, CreatedBy, IsActive) " &
                                "VALUES ('" & title & "', '" & desc & "', " & teacherID & ", " & IIf(active, "1", "0") & "); SELECT SCOPE_IDENTITY();"
            Dim cmd As New SqlCommand(sql, conn)

            conn.Open()
            Dim newIDObj As Object = cmd.ExecuteScalar()
            Dim newID As Integer = 0
            If newIDObj IsNot Nothing Then
                newID = Convert.ToInt32(newIDObj)
            End If
            conn.Close()

            ' VULNERABLE: show raw preview (reflected XSS) — intentionally unsafe for lab
            litRawPreview.Text = "<h3>Preview (raw output):</h3>" & desc

            ' Keep the original redirect so the stored value can be seen later (ManageQuestions.aspx)
            Response.Redirect("ManageQuestions.aspx?QuizID=" & newID & "&success=true")
        Catch ex As Exception
            ' VULNERABLE: verbose error returned to user (information disclosure)
            lblMessage.Text = "Error creating quiz: " & ex.Message
            lblMessage.ForeColor = System.Drawing.Color.Red
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("TeacherDashboard.aspx")
    End Sub
End Class
