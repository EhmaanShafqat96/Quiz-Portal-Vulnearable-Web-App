Imports System.Data.SqlClient

Partial Class EditUser
    Inherits System.Web.UI.Page

    ' VULNERABLE: Hardcoded connection string
    Dim connString As String = "Data Source=Eman\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True;"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("UserID") Is Nothing Then
            Response.Redirect("Login.aspx")
        End If

        If Not IsPostBack Then
            ' VULNERABLE: Takes user ID directly from URL parameter without validation or authorization check.
            ' This is a classic Insecure Direct Object Reference (IDOR).
            Dim userId As String = Request.QueryString("userid")

            If String.IsNullOrEmpty(userId) Then
                lblStatus.Text = "Error: No User ID provided."
                lblStatus.ForeColor = System.Drawing.Color.Red
                Return
            End If

            ViewState("UserID") = userId ' Store for later use

            ' VULNERABLE: SQL Injection vulnerability. The userId is concatenated directly into the query.
            Dim sql As String = "SELECT Username, FullName, Password, Role FROM Users WHERE UserID = " & userId

            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand(sql, conn)
                    Try
                        conn.Open()
                        Dim reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            ' Pre-populate the form with user data
                            txtUsername.Text = reader("Username").ToString()
                            txtFullName.Text = reader("FullName").ToString()
                            txtPassword.Text = reader("Password").ToString
                            ddlRole.SelectedValue = reader("Role").ToString()
                        Else
                            lblStatus.Text = "Error: User not found."
                            lblStatus.ForeColor = System.Drawing.Color.Red
                        End If
                        reader.Close()
                    Catch ex As Exception
                        ' VULNERABLE: Detailed error message disclosure
                        lblStatus.Text = "Error loading user: " & ex.Message
                        lblStatus.ForeColor = System.Drawing.Color.Red
                    End Try
                End Using
            End Using
        End If
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Dim userId As String = ViewState("UserID").ToString()
        Dim newUsername As String = txtUsername.Text.Trim().Replace("'", "''")
        Dim newFullName As String = txtFullName.Text.Trim().Replace("'", "''")
        Dim newPassword As String = txtPassword.Text.Trim().Replace("'", "''")
        Dim newRole As String = ddlRole.SelectedValue.Replace("'", "''")

        ' VULNERABLE: But now with escaped quotes to avoid syntax errors
        Dim sql As String = String.Format("UPDATE Users SET Username = '{0}', FullName = '{1}', Password = '{2}', Role = '{3}' WHERE UserID = {4}",
                                     newUsername, newFullName, newPassword, newRole, userId)

        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand(sql, conn)
                Try
                    conn.Open()
                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
                    If rowsAffected > 0 Then
                        lblStatus.Text = "User updated successfully!"
                        lblStatus.ForeColor = System.Drawing.Color.Green
                    Else
                        lblStatus.Text = "Error: Update failed."
                        lblStatus.ForeColor = System.Drawing.Color.Red
                    End If
                Catch ex As Exception
                    lblStatus.Text = "Error updating user: " & ex.Message
                    lblStatus.ForeColor = System.Drawing.Color.Red
                End Try
            End Using
        End Using
    End Sub
End Class