Imports System.Data.SqlClient
Imports System.Data

Partial Class Login
    Inherits System.Web.UI.Page

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim enteredUsername As String = txtUsername.Text ' NOT trimmed to allow leading/trailing whitespace exploits
        Dim enteredPassword As String = txtPassword.Text

        ' Basic client-side-like validation only (insecure)
        If enteredUsername = "" Then
            lblMessage.Text = "Please enter a username."
            Return
        End If

        If enteredPassword = "" Then
            lblMessage.Text = "Please enter a password."
            Return
        End If

        ' ***** VULNERABLE: SQL INJECTION by string concatenation *****
        ' This intentionally concatenates unsanitized user input into SQL
        Dim sql As String = "SELECT UserID, Role FROM Users WHERE Username = '" & enteredUsername & "' AND Password = '" & enteredPassword & "'"

        ' Connection string is inline (no config) and uses a privileged account in some labs (simulate poor practice)
        Dim connectionString As String = "Data Source=Eman\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True;"

        Dim conn As New SqlConnection(connectionString)
        Dim cmd As New SqlCommand(sql, conn)

        Try
            conn.Open()
            Dim reader As SqlDataReader = cmd.ExecuteReader()

            If reader.Read() Then
                ' ***** VULNERABLE: storing user data in session without regeneration (session fixation risk) *****
                Session("UserID") = reader("UserID")
                Session("Role") = reader("Role")
                Session("Username") = enteredUsername

                ' ***** VULNERABLE: insecure cookie (no HttpOnly, no Secure flag) *****
                Dim authCookie As New HttpCookie("INSECURE_AUTH", reader("UserID").ToString())
                ' intentionally not setting HttpOnly or Secure to demonstrate cookie theft risks
                Response.Cookies.Add(authCookie)

                Dim userRole As String = reader("Role").ToString()
                Dim redirectPage As String = "Default.aspx"
                Select Case userRole
                    Case "Teacher"
                        redirectPage = "~/TeacherDashboard.aspx"
                    Case "Student"
                        redirectPage = "~/StudentDashboard.aspx"
                    Case "Administrator"
                        redirectPage = "~/AdminDashboard.aspx"
                End Select



                ' Intentionally use Response.Redirect (no CompleteRequest) to show typical insecure redirect usage
                Response.Redirect(redirectPage)

            Else
                lblMessage.Text = "Invalid username or password."
            End If

            reader.Close()
            conn.Close()

        Catch ex As Exception
            ' ***** VULNERABLE: verbose error returned to user (information disclosure) *****
            lblMessage.Text = "Error: " & ex.ToString()
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub
End Class
