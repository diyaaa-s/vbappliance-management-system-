Imports System.Data.SqlClient

Public Class FORM7
    Dim con As New SqlConnection("Data Source=DIYA_S\SQLEXPRESS;Initial Catalog=AMDB;Integrated Security=True")

    ' Load Customer Report based on Date Range
    Private Sub LoadCustomerReport(startDate As Date, endDate As Date)
        Dim query As String = "SELECT CustomerID, CustomerName, Address, ContactNumber, Email FROM Customerss"
        Dim cmd As New SqlCommand(query, con)

        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)
        DataGridView1.DataSource = dt
    End Sub

    ' Load Order Report based on Date Range
    Private Sub LoadOrderReport(startDate As Date, endDate As Date)
        Dim query As String = "SELECT Orderss.OrderID, Customerss.CustomerName, Products.ProductName, OrderDetailss.Quantity, OrderDetailss.SubTotal AS TotalPrice, Orderss.OrderDate " &
                              "FROM Orderss " &
                              "INNER JOIN OrderDetailss ON Orderss.OrderID = OrderDetailss.OrderID " &
                              "INNER JOIN Customerss ON Orderss.CustomerID = Customerss.CustomerID " &
                              "INNER JOIN Products ON OrderDetailss.ProductID = Products.ProductID " &
                              "WHERE Orderss.OrderDate BETWEEN @startDate AND @endDate"
        Dim cmd As New SqlCommand(query, con)
        cmd.Parameters.AddWithValue("@startDate", startDate)
        cmd.Parameters.AddWithValue("@endDate", endDate)

        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)
        DataGridView1.DataSource = dt
    End Sub

    ' Load Product Report based on Date Range
    Private Sub LoadProductReport(startDate As Date, endDate As Date)
        Dim query As String = "SELECT Products.ProductID, Products.ProductName, Products.Category, Products.Price, Products.StockQuantity, Supplierss.SupplierName " &
                              "FROM Products " &
                              "INNER JOIN Supplierss ON Products.SupplierID = Supplierss.SupplierID"
        Dim cmd As New SqlCommand(query, con)

        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)
        DataGridView1.DataSource = dt
    End Sub

    ' Generate Report Button Click
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim startDate As Date = DateTimePicker1.Value
        Dim endDate As Date = DateTimePicker2.Value

        If ComboBox1.SelectedItem.ToString() = "Customer Report" Then
            LoadCustomerReport(startDate, endDate)
        ElseIf ComboBox1.SelectedItem.ToString() = "Order Report" Then
            LoadOrderReport(startDate, endDate)
        ElseIf ComboBox1.SelectedItem.ToString() = "Product Report" Then
            LoadProductReport(startDate, endDate)
        Else
            MessageBox.Show("Please select a valid report type.")
        End If
    End Sub

    ' Reset Form Button Click
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ComboBox1.SelectedIndex = -1
        DateTimePicker1.Value = Date.Today
        DateTimePicker2.Value = Date.Today
        DataGridView1.DataSource = Nothing
    End Sub


    Private Sub Form7_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Add report types to ComboBox
        ComboBox1.Items.Add("Customer Report")
        ComboBox1.Items.Add("Order Report")
        ComboBox1.Items.Add("Product Report")

        ' Optionally, you can set a default selection if desired
        ComboBox1.SelectedIndex = 0 ' This will set the first item as selected (Customer Report)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
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
