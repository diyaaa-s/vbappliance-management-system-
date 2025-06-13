Imports System.Data.SqlClient

Public Class Form1
    Public Property LoggedInCustomerID As Integer

    Dim con As New SqlConnection("Data Source=DIYA_S\SQLEXPRESS;Initial Catalog=AMDB;Integrated Security=True")

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox2.UseSystemPasswordChar = True ' Mask the password input
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Check if username, password, or role is not selected
        If TextBox1.Text = "" Or TextBox2.Text = "" Then
            MessageBox.Show("Please enter both Username and Password.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not RadioButton1.Checked AndAlso Not RadioButton2.Checked Then
            MessageBox.Show("Please select a role (Admin or Staff).", "Role Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Prepare SQL command
        Dim cmd As New SqlCommand("SELECT UserID, Role FROM Users WHERE Username=@username AND Password=@password", con)
        cmd.Parameters.AddWithValue("@username", TextBox1.Text)
        cmd.Parameters.AddWithValue("@password", TextBox2.Text) ' Note: Plaintext, consider hashing in future

        Try
            con.Open()
            Dim reader As SqlDataReader = cmd.ExecuteReader()

            If reader.Read() Then
                Dim userID As Integer = Convert.ToInt32(reader("UserID"))
                Dim role As String = reader("Role").ToString()
                reader.Close()

                MessageBox.Show("Login Successful!", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Module1.LoggedInRole = role

                ' Role check based on radio button selection
                If role = "Admin" AndAlso RadioButton1.Checked Then
                    form2.Show() ' Show Admin form (form2)
                    Me.Hide()
                ElseIf role = "Staff" AndAlso RadioButton2.Checked Then
                    Form9.Show() ' Show Staff form (Form9)
                    Me.Hide()
                Else
                    MessageBox.Show("The selected role does not match the user role in the database.", "Role Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

            Else
                MessageBox.Show("Invalid Username or Password!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show("Error connecting to database: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        TextBox2.UseSystemPasswordChar = Not CheckBox1.Checked
    End Sub
End Class
