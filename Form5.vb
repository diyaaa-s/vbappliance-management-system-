Imports System.Data.SqlClient

Public Class FORM5
    Dim con As New SqlConnection("Data Source=DIYA_S\SQLEXPRESS;Initial Catalog=AMDB;Integrated Security=True")

    ' Load customers into DataGridView
    Private Sub LoadCustomers()
        Dim cmd As New SqlCommand("SELECT * FROM Customerss", con)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)
        DataGridView1.DataSource = dt
    End Sub

    ' Clear all fields
    Private Sub ClearFields()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox5.Clear()
        TextBox6.Clear()
    End Sub

    ' Form Load
    Private Sub CustomerManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Button3.Visible = False
        LoadCustomers()
    End Sub

    ' Add Customer
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        ' Validate Contact Number length
        If TextBox3.Text.Length > 10 Then
            MessageBox.Show("Contact Number cannot exceed 10 digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If


        ' (rest of your code remains unchanged)

        If con.State = ConnectionState.Closed Then con.Open()
        Dim query As String = "INSERT INTO Customerss (CustomerName, ContactNumber, Email, Address) VALUES (@name, @number, @email, @address)"
        Dim cmd As New SqlCommand(query, con)
        cmd.Parameters.AddWithValue("@name", TextBox2.Text)
        cmd.Parameters.AddWithValue("@number", TextBox3.Text)
        cmd.Parameters.AddWithValue("@email", TextBox4.Text)
        cmd.Parameters.AddWithValue("@address", TextBox5.Text)
        cmd.ExecuteNonQuery()
        MessageBox.Show("Customer added successfully")
        LoadCustomers()
        ClearFields()
        con.Close()

    End Sub

    ' Edit Customer
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If con.State = ConnectionState.Closed Then con.Open()
        Dim query As String = "UPDATE Customerss SET CustomerName=@name, ContactNumber=@number, Email=@email, Address=@address WHERE CustomerID=@id"
        Dim cmd As New SqlCommand(query, con)
        cmd.Parameters.AddWithValue("@id", TextBox1.Text)
        cmd.Parameters.AddWithValue("@name", TextBox2.Text)
        cmd.Parameters.AddWithValue("@number", TextBox3.Text)
        cmd.Parameters.AddWithValue("@email", TextBox4.Text)
        cmd.Parameters.AddWithValue("@address", TextBox5.Text)
        cmd.ExecuteNonQuery()
        MessageBox.Show("Customer updated successfully")
        LoadCustomers()
        ClearFields()
        con.Close()
    End Sub

    ' Delete Customer
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If con.State = ConnectionState.Closed Then con.Open()
        Dim query As String = "DELETE FROM Customerss WHERE CustomerID=@id"
        Dim cmd As New SqlCommand(query, con)
        cmd.Parameters.AddWithValue("@id", TextBox1.Text)
        cmd.ExecuteNonQuery()
        MessageBox.Show("Customer deleted successfully")
        LoadCustomers()
        ClearFields()
        con.Close()
    End Sub

    ' Clear
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ClearFields()
    End Sub

    ' Live Search functionality (Search by Customer Name)
    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged
        Dim cmd As New SqlCommand("SELECT * FROM Customerss WHERE CustomerName LIKE @search", con)
        cmd.Parameters.AddWithValue("@search", "%" & TextBox6.Text & "%")
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)
        DataGridView1.DataSource = dt
    End Sub

    ' Reset (Load customers and clear fields)
    Private Sub Button6_Click(sender As Object, e As EventArgs)
        LoadCustomers
        ClearFields
    End Sub

    ' Populate fields on Customer ID input (automatic population)
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If TextBox1.Text <> "" Then
            Dim cmd As New SqlCommand("SELECT * FROM Customerss WHERE CustomerID = @id", con)
            cmd.Parameters.AddWithValue("@id", TextBox1.Text)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)

            ' Check if the table has rows and debug the column names
            If dt.Rows.Count > 0 Then
                ' Debug: Display all column names
                For Each col As DataColumn In dt.Columns
                    Console.WriteLine("Column Name: " & col.ColumnName)
                Next

                ' Now populate the fields with data, but check for column existence
                TextBox2.Text = dt.Rows(0)("CustomerName").ToString()

                ' Check if 'ContactNumber' column exists before accessing it
                If dt.Columns.Contains("ContactNumber") Then
                    TextBox3.Text = dt.Rows(0)("ContactNumber").ToString()
                Else
                    MessageBox.Show("ContactNumber column not found in the table.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

                ' Check if 'Email' column exists before accessing it
                If dt.Columns.Contains("Email") Then
                    TextBox4.Text = dt.Rows(0)("Email").ToString()
                Else
                    MessageBox.Show("Email column not found in the table.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

                TextBox5.Text = dt.Rows(0)("Address").ToString()
            End If
        End If
    End Sub


    ' Populate fields on row click
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            TextBox1.Text = row.Cells("CustomerID").Value.ToString()
            TextBox2.Text = row.Cells("CustomerName").Value.ToString()
            TextBox3.Text = row.Cells("ContactNumber").Value.ToString()
            TextBox4.Text = row.Cells("Email").Value.ToString()
            TextBox5.Text = row.Cells("Address").Value.ToString()
        End If
    End Sub

    ' Navigate to other forms based on user role
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Me.Hide()

        If Module1.LoggedInRole = "Admin" Then
            form2.Show()
        ElseIf Module1.LoggedInRole.ToLower() = "staff" Then
            Form9.Show()
        Else
            MessageBox.Show("Unknown user role. Cannot navigate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Show() ' Optional: Show this form again if role is invalid
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Me.Hide()
        Form4.Show()

    End Sub
End Class
