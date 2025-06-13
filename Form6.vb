Imports System.Data.SqlClient

Public Class form6
    Dim con As New SqlConnection("Data Source=DIYA_S\SQLEXPRESS;Initial Catalog=AMDB;Integrated Security=True")

    ' Load suppliers into DataGridView
    Private Sub LoadSuppliers()
        Dim cmd As New SqlCommand("SELECT * FROM Supplierss", con)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)
        DataGridView1.DataSource = dt
    End Sub

    ' Auto-populate fields when Supplier ID is entered
    Private Sub AutoPopulateFields(supplierId As Integer)
        Dim cmd As New SqlCommand("SELECT * FROM Supplierss WHERE SupplierID = @id", con)
        cmd.Parameters.AddWithValue("@id", supplierId)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)

        If dt.Rows.Count > 0 Then
            TextBox1.Text = dt.Rows(0)("SupplierID").ToString()
            TextBox2.Text = dt.Rows(0)("SupplierName").ToString()
            TextBox3.Text = dt.Rows(0)("ContactInfo").ToString()
            TextBox4.Text = dt.Rows(0)("SupplierEmail").ToString()
            TextBox5.Text = dt.Rows(0)("SupplierAddress").ToString()
            ComboBox1.Text = dt.Rows(0)("SupplierStatus").ToString()
        End If
    End Sub

    ' Form Load
    Private Sub SupplierManagementForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Load status options
        ComboBox1.Items.Clear()
        ComboBox1.Items.Add("Active")
        ComboBox1.Items.Add("Inactive")

        ' Load supplier data
        LoadSuppliers()
    End Sub



    ' Add Supplier
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If con.State = ConnectionState.Closed Then con.Open()
        Dim query As String = "INSERT INTO Supplierss (SupplierName, ContactInfo, SupplierEmail, SupplierAddress, SupplierStatus) VALUES (@name, @contact, @email, @address, @status)"
        Dim cmd As New SqlCommand(query, con)
        cmd.Parameters.AddWithValue("@name", TextBox2.Text)
        cmd.Parameters.AddWithValue("@contact", TextBox3.Text)
        cmd.Parameters.AddWithValue("@email", TextBox4.Text)
        cmd.Parameters.AddWithValue("@address", TextBox5.Text)
        cmd.Parameters.AddWithValue("@status", ComboBox1.Text)
        cmd.ExecuteNonQuery()
        MessageBox.Show("Supplier added successfully")
        LoadSuppliers()
        ClearFields()
        con.Close()
    End Sub

    ' Edit Supplier
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If con.State = ConnectionState.Closed Then con.Open()
        Dim query As String = "UPDATE Supplierss SET SupplierName=@name, ContactInfo=@contact, SupplierEmail=@email, SupplierAddress=@address, SupplierStatus=@status WHERE SupplierID=@id"
        Dim cmd As New SqlCommand(query, con)
        cmd.Parameters.AddWithValue("@id", TextBox1.Text)
        cmd.Parameters.AddWithValue("@name", TextBox2.Text)
        cmd.Parameters.AddWithValue("@contact", TextBox3.Text)
        cmd.Parameters.AddWithValue("@email", TextBox4.Text)
        cmd.Parameters.AddWithValue("@address", TextBox5.Text)
        cmd.Parameters.AddWithValue("@status", ComboBox1.Text)
        cmd.ExecuteNonQuery()
        MessageBox.Show("Supplier updated successfully")
        LoadSuppliers()
        ClearFields()
        con.Close()
    End Sub

    ' Delete Supplier
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If con.State = ConnectionState.Closed Then con.Open()
        Dim query As String = "DELETE FROM Supplierss WHERE SupplierID=@id"
        Dim cmd As New SqlCommand(query, con)
        cmd.Parameters.AddWithValue("@id", TextBox1.Text)
        cmd.ExecuteNonQuery()
        MessageBox.Show("Supplier deleted successfully")
        LoadSuppliers()
        ClearFields()
        con.Close()
    End Sub

    ' Clear fields
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ClearFields()
    End Sub

    ' Live Search: Triggered on TextChanged event of the search TextBox
    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged
        Dim cmd As New SqlCommand("SELECT * FROM Supplierss WHERE SupplierName LIKE @search OR SupplierID LIKE @search", con)
        cmd.Parameters.AddWithValue("@search", "%" & TextBox6.Text & "%")
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)
        DataGridView1.DataSource = dt
    End Sub

    ' Reset fields
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        LoadSuppliers()
        ClearFields()
    End Sub

    ' Populate fields on row click
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            TextBox1.Text = row.Cells("SupplierID").Value.ToString()
            TextBox2.Text = row.Cells("SupplierName").Value.ToString()
            TextBox3.Text = row.Cells("ContactInfo").Value.ToString()
            TextBox4.Text = row.Cells("SupplierEmail").Value.ToString()
            TextBox5.Text = row.Cells("SupplierAddress").Value.ToString()
            ComboBox1.Text = row.Cells("SupplierStatus").Value.ToString()
        End If
    End Sub

    ' Highlight inactive suppliers
    Private Sub HighlightInactiveSuppliers()
        For Each row As DataGridViewRow In DataGridView1.Rows
            If row.Cells("SupplierStatus").Value IsNot Nothing AndAlso row.Cells("SupplierStatus").Value.ToString() = "Inactive" Then
                row.DefaultCellStyle.BackColor = Color.LightSalmon
            End If
        Next
    End Sub

    ' Auto Populate Supplier Info when Supplier ID is entered
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If Integer.TryParse(TextBox1.Text, 0) Then
            AutoPopulateFields(Convert.ToInt32(TextBox1.Text))
        End If
    End Sub

    ' Clear all fields
    Private Sub ClearFields()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox5.Clear()
        ComboBox1.SelectedIndex = -1
        TextBox6.Clear()
    End Sub




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
End Class
