Imports System.Data.SqlClient

Public Class Form3
    Dim con As New SqlConnection("Data Source=DIYA_S\SQLEXPRESS;Initial Catalog=AMDB;Integrated Security=True")

    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Button5.Visible = False
        Button6.Visible = False
        LoadProducts()
        LoadSuppliers()
        LoadCategories()
        HighlightLowStock()
    End Sub
    Private Sub LoadProducts()
        Dim query As String = "SELECT ProductID, ProductName, Category, Price, StockQuantity, SupplierID FROM Products"
        Dim adapter As New SqlDataAdapter(query, con)
        Dim dt As New DataTable()
        adapter.Fill(dt)
        DataGridView1.DataSource = dt
        HighlightLowStock()
    End Sub

    Private Sub LoadSuppliers()
        Dim query As String = "SELECT SupplierID, SupplierName FROM Supplierss"
        Dim adapter As New SqlDataAdapter(query, con)
        Dim dt As New DataTable()
        adapter.Fill(dt)
        ComboBox1.DataSource = dt
        ComboBox1.DisplayMember = "SupplierName"
        ComboBox1.ValueMember = "SupplierID"
        ComboBox1.SelectedIndex = -1
    End Sub

    Private Sub LoadCategories()
        ComboBox2.Items.Clear()
        Dim query As String = "SELECT DISTINCT Category FROM Products"
        Dim cmd As New SqlCommand(query, con)
        con.Open()
        Dim reader As SqlDataReader = cmd.ExecuteReader()
        While reader.Read()
            ComboBox2.Items.Add(reader("Category").ToString())
        End While
        reader.Close()
        con.Close()
        ComboBox2.SelectedIndex = -1
    End Sub

    Private Sub HighlightLowStock()
        For Each row As DataGridViewRow In DataGridView1.Rows
            If Not row.IsNewRow AndAlso Convert.ToInt32(row.Cells("StockQuantity").Value) < 5 Then
                row.DefaultCellStyle.BackColor = Color.MistyRose
                row.DefaultCellStyle.ForeColor = Color.Red
            Else
                row.DefaultCellStyle.BackColor = Color.White
                row.DefaultCellStyle.ForeColor = Color.Black
            End If
        Next
    End Sub

    Private Sub ClearFields()
        TextBox1.Clear()
        ComboBox1.SelectedIndex = -1
        TextBox2.Clear()
        ComboBox2.SelectedIndex = -1
        TextBox3.Clear()
        TextBox4.Clear()
    End Sub

    ' ADD
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ComboBox1.SelectedIndex = -1 OrElse TextBox2.Text = "" OrElse ComboBox2.Text = "" OrElse TextBox3.Text = "" OrElse TextBox4.Text = "" Then
            MessageBox.Show("Please fill all fields.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim query As String = "INSERT INTO Products (ProductName, Category, Price, StockQuantity, SupplierID) VALUES (@name, @category, @price, @stock, @supplierID)"
        Dim cmd As New SqlCommand(query, con)
        cmd.Parameters.AddWithValue("@name", TextBox2.Text)
        cmd.Parameters.AddWithValue("@category", ComboBox2.Text)
        cmd.Parameters.AddWithValue("@price", Convert.ToDecimal(TextBox3.Text))
        cmd.Parameters.AddWithValue("@stock", Convert.ToInt32(TextBox4.Text))
        cmd.Parameters.AddWithValue("@supplierID", Convert.ToInt32(ComboBox1.SelectedValue))

        con.Open()
        cmd.ExecuteNonQuery()
        con.Close()

        MessageBox.Show("Product added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        LoadProducts()
        LoadCategories()
        ClearFields()
    End Sub

    ' AUTO-FILL ON PRODUCT ID CHANGE
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If TextBox1.Text.Trim() = "" Then
            ClearFields()
            Return
        End If

        Dim productID As Integer
        If Integer.TryParse(TextBox1.Text.Trim(), productID) Then
            Dim query As String = "SELECT * FROM Products WHERE ProductID = @id"
            Dim cmd As New SqlCommand(query, con)
            cmd.Parameters.AddWithValue("@id", productID)

            Dim dt As New DataTable()
            Dim adapter As New SqlDataAdapter(cmd)
            adapter.Fill(dt)

            If dt.Rows.Count > 0 Then
                Dim row As DataRow = dt.Rows(0)
                TextBox2.Text = row("ProductName").ToString()
                ComboBox2.Text = row("Category").ToString()
                TextBox3.Text = row("Price").ToString()
                TextBox4.Text = row("StockQuantity").ToString()
                ComboBox1.SelectedValue = row("SupplierID")
            Else
                TextBox2.Clear()
                ComboBox2.SelectedIndex = -1
                TextBox3.Clear()
                TextBox4.Clear()
                ComboBox1.SelectedIndex = -1
            End If
        End If
    End Sub

    ' EDIT
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If TextBox1.Text = "" OrElse TextBox2.Text = "" OrElse ComboBox2.Text = "" OrElse TextBox3.Text = "" OrElse TextBox4.Text = "" OrElse ComboBox1.SelectedIndex = -1 Then
            MessageBox.Show("Please enter Product ID and fill all fields.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim query As String = "UPDATE Products SET ProductName = @name, Category = @category, Price = @price, StockQuantity = @stock, SupplierID = @supplierID WHERE ProductID = @id"
        Dim cmd As New SqlCommand(query, con)
        cmd.Parameters.AddWithValue("@name", TextBox2.Text)
        cmd.Parameters.AddWithValue("@category", ComboBox2.Text)
        cmd.Parameters.AddWithValue("@price", Convert.ToDecimal(TextBox3.Text))
        cmd.Parameters.AddWithValue("@stock", Convert.ToInt32(TextBox4.Text))
        cmd.Parameters.AddWithValue("@supplierID", Convert.ToInt32(ComboBox1.SelectedValue))
        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(TextBox1.Text))

        con.Open()
        Dim rowsAffected = cmd.ExecuteNonQuery()
        con.Close()

        If rowsAffected > 0 Then
            MessageBox.Show("Product updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadProducts()
            LoadCategories()
            ClearFields()
        Else
            MessageBox.Show("Product ID not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub



    ' CLEAR
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ClearFields()
    End Sub

    ' SEARCH (BUTTON)
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click

        Dim query As String = "SELECT * FROM Products WHERE ProductName LIKE @search OR Category LIKE @search"
        Dim adapter As New SqlDataAdapter(query, con)
        adapter.SelectCommand.Parameters.AddWithValue("@search", "%" & TextBox5.Text & "%")
        Dim dt As New DataTable()
        adapter.Fill(dt)
        DataGridView1.DataSource = dt
        HighlightLowStock()
    End Sub

    ' LIVE SEARCH
    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TextBox5.TextChanged
        Dim query As String = "SELECT * FROM Products WHERE ProductName LIKE @search OR Category LIKE @search"
        Dim adapter As New SqlDataAdapter(query, con)
        adapter.SelectCommand.Parameters.AddWithValue("@search", "%" & TextBox5.Text & "%")
        Dim dt As New DataTable()
        adapter.Fill(dt)
        DataGridView1.DataSource = dt
        HighlightLowStock()
    End Sub

    ' RESET SEARCH
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        TextBox5.Text = ""
        LoadProducts()
        HighlightLowStock()
    End Sub

    ' BACK
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Me.Hide()

        If Module1.LoggedInRole = "Admin" Then
            form2.Show()
        ElseIf Module1.LoggedInRole.ToLower() = "staff" Then
            Form9.Show()
        Else
            MessageBox.Show("Unknown user role. Cannot navigate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Show()
        End If
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub
End Class
