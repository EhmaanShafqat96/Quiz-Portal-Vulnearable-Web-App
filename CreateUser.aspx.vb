Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Partial Class CreateUser
    Inherits System.Web.UI.Page

    ' *** VULNERABLE: inline connection string and possibly high-privileged DB user ***
    Private ReadOnly connectionString As String = "Data Source=Eman\SQLEXPRESS;Initial Catalog=OnlineTest;Integrated Security=True;"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' *** WEAK AUTHENTICATION CHECK (vulnerable) ***
        ' Only checks that a session exists - does not verify role. This allows any logged-in user to reach create page.
        If Session("UserID") Is Nothing Then
            Response.Redirect("Login.aspx")
        End If

        ' *** VULNERABLE: automatic creation via GET parameters (demonstrates CSRF / unwanted create) ***
        ' Example usage (only for lab): CreateUser.aspx?autoCreate=1&u=test&pw=pass&fn=Joe&role=Student
        If Not IsPostBack AndAlso Request.QueryString("autoCreate") = "1" Then
            Dim u As String = If(Request.QueryString("u"), "")
            Dim pw As String = If(Request.QueryString("pw"), "")
            Dim fn As String = If(Request.QueryString("fn"), "")
            Dim r As String = If(Request.QueryString("role"), "Student")

            ' Minimal validation (intentionally weak)
            If u <> "" And pw <> "" Then
                CreateUserUnsafe(u, pw, fn, r, Nothing)
                lblMessage.Text = "User auto-created via GET (unsafe)."
            End If
        End If
    End Sub

    ' Click handler for the Create button
    Protected Sub btnCreate_Click(sender As Object, e As EventArgs)
        Dim username As String = txtUsername.Text ' not trimmed to allow whitespace tricks
        Dim password As String = txtPassword.Text
        Dim fullName As String = txtFullName.Text ' allow raw HTML (stored XSS)
        Dim role As String = ddlRole.SelectedValue

        If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) OrElse
       String.IsNullOrEmpty(fullName) OrElse String.IsNullOrEmpty(role) Then
            lblMessage.Text = "All fields are required."
            Return
        End If

        ' Use insecure DB operations (SQL injection + plain-text password storage)
        Using conn As New SqlConnection(connectionString)
            Try
                conn.Open()

                ' *** VULNERABLE: SQL injection via concatenated query for existence check ***
                Dim checkQuery As String = "SELECT COUNT(*) FROM Users WHERE Username = '" & username & "'"
                Dim checkCmd As New SqlCommand(checkQuery, conn)
                Dim exists As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                If exists > 0 Then
                    lblMessage.Text = "Username already exists. Please choose a different username."
                    Return
                End If

                ' *** VULNERABLE: SQL injection via concatenated INSERT, storing password as plain text and fullName raw (stored XSS) ***
                ' ✅ FIXED: Added missing single quote after role value
                Dim insertQuery As String = "INSERT INTO Users (Username, Password, FullName, Role) VALUES ('" & username & "', '" & password & "', '" & fullName & "', '" & role & "')"
                Dim insertCmd As New SqlCommand(insertQuery, conn)
                insertCmd.ExecuteNonQuery()

                ' *** VULNERABLE: create an insecure cookie with username (no HttpOnly/secure flags) ***
                Dim insecureCookie As New HttpCookie("lab_user", username)
                insecureCookie.Expires = DateTime.Now.AddHours(1)
                ' intentionally NOT setting HttpOnly or Secure
                Response.Cookies.Add(insecureCookie)

            Catch ex As Exception
                ' *** VULNERABLE: show detailed exception to user (information disclosure) ***
                lblMessage.Text = "Database error: " & ex.ToString()
                Return
            Finally
                If conn.State = ConnectionState.Open Then conn.Close()
            End Try
        End Using

        ' Redirect back to admin dashboard after creation
        Response.Redirect("AdminDashboard.aspx")
    End Sub

    ' Helper allow programmatic creation (used by auto-create GET above)
    Private Sub CreateUserUnsafe(u As String, pw As String, fn As String, r As String, avatarPath As String)
        Using conn As New SqlConnection(connectionString)
            conn.Open()
            ' ✅ FIXED: Added missing single quote after role value
            Dim insertQuery As String = "INSERT INTO Users (Username, Password, FullName, Role) VALUES ('" & u & "', '" & pw & "', '" & fn & "', '" & r & "')"
            Dim insertCmd As New SqlCommand(insertQuery, conn)
            insertCmd.ExecuteNonQuery()
            conn.Close()
        End Using
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("AdminDashboard.aspx")
    End Sub
End Class
