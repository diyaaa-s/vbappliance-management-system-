Imports System.Data.SqlClient

Public Class Form4
    Dim con As New SqlConnection("Data Source=DIYA_S\SQLEXPRESS;Initial Catalog=AMDB;Integrated Security=True")
    Dim cmd As SqlCommand
    Dim da As SqlDataAdapter
    Dim dt As DataTable

    ' Auto-generate Order ID
    Private Sub GenerateOrderID()
        con.Open()
        cmd = New SqlCommand("SELECT ISNULL(MAX(OrderID), 0) + 1 FROM Orderss", con)
        TextBox1.Text = cmd.ExecuteScalar().ToString()
        con.Close()
    End Sub

    ' Load ComboBoxes
    Private Sub LoadComboBoxes()
        ' Load Customers
        con.Open()
        cmd = New SqlCommand("SELECT CustomerID, CustomerName FROM Customerss", con)
        da = New SqlDataAdapter(cmd)
        dt = New DataTable()
        da.Fill(dt)
        ComboBox1.DataSource = dt
        ComboBox1.DisplayMember = "CustomerName"
        ComboBox1.ValueMember = "CustomerID"
        ComboBox1.SelectedIndex = -1 ' <-- Prevent default selection
        con.Close()

        ' Load Products
        con.Open()
        cmd = New SqlCommand("SELECT ProductID, ProductName FROM Products", con)
        da = New SqlDataAdapter(cmd)
        dt = New DataTable()
        da.Fill(dt)
        ComboBox2.DataSource = dt
        ComboBox2.DisplayMember = "ProductName"
        ComboBox2.ValueMember = "ProductID"
        ComboBox2.SelectedIndex = -1 ' <-- Prevent default selection
        con.Close()

        ' Load Order Status
        ComboBox3.Items.Clear()
        ComboBox3.Items.AddRange({"Pending", "Processing", "Delivered"})
        ComboBox3.SelectedIndex = -1 ' Optional: no default status selected
    End Sub

    ' Load all Orders
    Private Sub LoadOrders()

        If con.State = ConnectionState.Open Then
            con.Close()
        End If
        con.Open()

        cmd = New SqlCommand("SELECT o.OrderID, c.CustomerName, p.ProductName, od.Quantity, od.SubTotal AS TotalPrice, o.OrderDate, o.OrderStatus 
                              FROM Orderss o
                              JOIN Customerss c ON o.CustomerID = c.CustomerID
                              JOIN OrderDetailss od ON o.OrderID = od.OrderID
                              JOIN Products p ON od.ProductID = p.ProductID", con)
        da = New SqlDataAdapter(cmd)
        dt = New DataTable()
        da.Fill(dt)
        DataGridView1.DataSource = dt
        con.Close()
    End Sub

    ' Calculate total price
    Private Sub CalculateTotal()
        Dim total As Decimal = 0

        If ComboBox2.SelectedValue IsNot Nothing AndAlso IsNumeric(TextBox2.Text) Then
            Using con As New SqlConnection("Data Source=DIYA_S\SQLEXPRESS;Initial Catalog=AMDB;Integrated Security=True")
                con.Open()
                Using cmd As New SqlCommand("SELECT Price FROM Products WHERE ProductID = @pid", con)
                    cmd.Parameters.AddWithValue("@pid", ComboBox2.SelectedValue)

                    Dim price As Object = cmd.ExecuteScalar()
                    If price IsNot Nothing AndAlso IsNumeric(price) Then
                        total = Convert.ToDecimal(price) * Convert.ToDecimal(TextBox2.Text)
                    End If
                End Using
            End Using
        End If

        TextBox3.Text = total.ToString("0.00")
    End Sub


    ' Add Order
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox2.Text = "" OrElse CInt(TextBox2.Text) <= 0 Then
            MessageBox.Show("Enter valid quantity.")
            Exit Sub
        End If

        con.Open()
        ' Insert into Orderss
        ' Insert into Orderss
        cmd = New SqlCommand("INSERT INTO Orderss(CustomerID, OrderDate, OrderStatus, TotalPrice) VALUES(@cid, @odate, @status, @total); SELECT SCOPE_IDENTITY();", con)
        cmd.Parameters.AddWithValue("@cid", ComboBox1.SelectedValue)
        cmd.Parameters.AddWithValue("@odate", DateTimePicker1.Value)
        cmd.Parameters.AddWithValue("@status", ComboBox3.Text)
        cmd.Parameters.AddWithValue("@total", TextBox3.Text)

        ' Get the newly generated OrderID
        Dim newOrderID As Integer = Convert.ToInt32(cmd.ExecuteScalar())
        TextBox1.Text = newOrderID.ToString() ' Optional: update TextBox1 with new OrderID

        ' Insert into OrderDetailss
        cmd = New SqlCommand("INSERT INTO OrderDetailss(OrderID, ProductID, Quantity, SubTotal) VALUES(@oid, @pid, @qty, @sub)", con)
        cmd.Parameters.AddWithValue("@oid", newOrderID)
        cmd.Parameters.AddWithValue("@pid", ComboBox2.SelectedValue)
        cmd.Parameters.AddWithValue("@qty", TextBox2.Text)
        cmd.Parameters.AddWithValue("@sub", TextBox3.Text)
        cmd.ExecuteNonQuery()
        con.Close()

        MessageBox.Show("Order added successfully!")
        ClearFields()
        LoadOrders()
        GenerateOrderID()

    End Sub

    ' Update Order
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        con.Open()
        ' Update Orderss
        cmd = New SqlCommand("UPDATE Orderss SET CustomerID=@cid, OrderDate=@odate, OrderStatus=@status, TotalPrice=@total WHERE OrderID=@oid", con)
        cmd.Parameters.AddWithValue("@cid", ComboBox1.SelectedValue)
        cmd.Parameters.AddWithValue("@odate", DateTimePicker1.Value)
        cmd.Parameters.AddWithValue("@status", ComboBox3.Text)
        cmd.Parameters.AddWithValue("@total", TextBox3.Text)
        cmd.Parameters.AddWithValue("@oid", TextBox1.Text)
        cmd.ExecuteNonQuery()

        ' Update OrderDetailss
        cmd = New SqlCommand("UPDATE OrderDetailss SET ProductID=@pid, Quantity=@qty, SubTotal=@sub WHERE OrderID=@oid", con)
        cmd.Parameters.AddWithValue("@pid", ComboBox2.SelectedValue)
        cmd.Parameters.AddWithValue("@qty", TextBox2.Text)
        cmd.Parameters.AddWithValue("@sub", TextBox3.Text)
        cmd.Parameters.AddWithValue("@oid", TextBox1.Text)
        cmd.ExecuteNonQuery()
        con.Close()

        MessageBox.Show("Order updated!")
        ClearFields()
        LoadOrders()
    End Sub

    ' Delete Order
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        con.Open()
        cmd = New SqlCommand("DELETE FROM OrderDetailss WHERE OrderID=@oid", con)
        cmd.Parameters.AddWithValue("@oid", TextBox1.Text)
        cmd.ExecuteNonQuery()

        cmd = New SqlCommand("DELETE FROM Orderss WHERE OrderID=@oid", con)
        cmd.Parameters.AddWithValue("@oid", TextBox1.Text)
        cmd.ExecuteNonQuery()
        con.Close()

        MessageBox.Show("Order deleted!")
        ClearFields()
        LoadOrders()
        GenerateOrderID()
    End Sub

    ' Clear Form
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ClearFields()
        GenerateOrderID()
    End Sub

    ' Clear method
    Private Sub ClearFields()
        ComboBox1.SelectedIndex = -1
        ComboBox2.SelectedIndex = -1
        ComboBox3.SelectedIndex = -1
        TextBox2.Clear()
        TextBox3.Clear()
        DateTimePicker1.Value = Now
    End Sub

    ' Calculate total on quantity change
    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        CalculateTotal()
    End Sub

    ' Filter by date
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        con.Open()
        cmd = New SqlCommand("SELECT o.OrderID, c.CustomerName, p.ProductName, od.Quantity, od.SubTotal AS TotalPrice, o.OrderDate, o.OrderStatus 
                              FROM Orderss o
                              JOIN Customerss c ON o.CustomerID = c.CustomerID
                              JOIN OrderDetailss od ON o.OrderID = od.OrderID
                              JOIN Products p ON od.ProductID = p.ProductID
                              WHERE o.OrderDate BETWEEN @from AND @to", con)
        cmd.Parameters.AddWithValue("@from", DateTimePicker1.Value)
        cmd.Parameters.AddWithValue("@to", DateTimePicker2.Value)
        da = New SqlDataAdapter(cmd)
        dt = New DataTable()
        da.Fill(dt)
        DataGridView1.DataSource = dt
        con.Close()
    End Sub

    ' Search
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        con.Open()
        cmd = New SqlCommand("SELECT o.OrderID, c.CustomerName, p.ProductName, od.Quantity, od.SubTotal AS TotalPrice, o.OrderDate, o.OrderStatus 
                              FROM Orderss o
                              JOIN Customerss c ON o.CustomerID = c.CustomerID
                              JOIN OrderDetailss od ON o.OrderID = od.OrderID
                              JOIN Products p ON od.ProductID = p.ProductID
                              WHERE o.OrderID LIKE @search OR c.CustomerName LIKE @search", con)
        cmd.Parameters.AddWithValue("@search", "%" & TextBox4.Text & "%")
        da = New SqlDataAdapter(cmd)
        dt = New DataTable()
        da.Fill(dt)
        DataGridView1.DataSource = dt
        con.Close()
    End Sub

    ' Reset
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        LoadOrders()
        TextBox4.Clear()
    End Sub

    ' Form Load
    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadComboBoxes()
        LoadOrders()
        GenerateOrderID()
    End Sub

    ' Populate fields on grid click
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            TextBox1.Text = row.Cells("OrderID").Value.ToString()
            ComboBox1.Text = row.Cells("CustomerName").Value.ToString()
            ComboBox2.Text = row.Cells("ProductName").Value.ToString()
            TextBox2.Text = row.Cells("Quantity").Value.ToString()
            TextBox3.Text = row.Cells("TotalPrice").Value.ToString()
            DateTimePicker1.Value = Convert.ToDateTime(row.Cells("OrderDate").Value)
            ComboBox3.Text = row.Cells("OrderStatus").Value.ToString()
        End If
    End Sub
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If TextBox1.Text <> "" AndAlso IsNumeric(TextBox1.Text) Then
            If con.State = ConnectionState.Open Then con.Close()
            con.Open()
            cmd = New SqlCommand("SELECT o.OrderID, c.CustomerName, p.ProductName, od.Quantity, od.SubTotal AS TotalPrice, o.OrderDate, o.OrderStatus 
                              FROM Orderss o
                              JOIN Customerss c ON o.CustomerID = c.CustomerID
                              JOIN OrderDetailss od ON o.OrderID = od.OrderID
                              JOIN Products p ON od.ProductID = p.ProductID
                              WHERE o.OrderID = @oid", con)
            cmd.Parameters.AddWithValue("@oid", TextBox1.Text)
            Dim reader As SqlDataReader = cmd.ExecuteReader()
            If reader.Read() Then
                ComboBox1.Text = reader("CustomerName").ToString()
                ComboBox2.Text = reader("ProductName").ToString()
                TextBox2.Text = reader("Quantity").ToString()
                TextBox3.Text = reader("TotalPrice").ToString()
                DateTimePicker1.Value = Convert.ToDateTime(reader("OrderDate"))
                ComboBox3.Text = reader("OrderStatus").ToString()
            End If
            con.Close()
        End If
    End Sub

    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged
        con.Open()
        cmd = New SqlCommand("SELECT o.OrderID, c.CustomerName, p.ProductName, od.Quantity, od.SubTotal AS TotalPrice, o.OrderDate, o.OrderStatus 
                          FROM Orderss o
                          JOIN Customerss c ON o.CustomerID = c.CustomerID
                          JOIN OrderDetailss od ON o.OrderID = od.OrderID
                          JOIN Products p ON od.ProductID = p.ProductID
                          WHERE o.OrderID LIKE @search OR c.CustomerName LIKE @search", con)
        cmd.Parameters.AddWithValue("@search", "%" & TextBox4.Text & "%")
        da = New SqlDataAdapter(cmd)
        dt = New DataTable()
        da.Fill(dt)
        DataGridView1.DataSource = dt
        con.Close()
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
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
        Form8.Show()

    End Sub
End Class
