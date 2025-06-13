Imports System.Data.SqlClient
Imports System.Drawing.Printing

Public Class form8
    Dim con As New SqlConnection("Data Source=DIYA_S\SQLEXPRESS;Initial Catalog=AMDB;Integrated Security=True")
    Dim WithEvents PD As New PrintDocument
    Dim PPD As New PrintPreviewDialog

    Private Sub PaymentForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadOrders()
        LoadPayments()
        ComboBox2.Items.AddRange(New String() {"Cash", "Card", "Online"})
        TextBox1.Text = GetNextPaymentID()
        TextBox1.Visible = False
        Label1.Visible = False
    End Sub

    Private Sub LoadOrders()
        ComboBox1.Items.Clear()
        Dim cmd As New SqlCommand("SELECT OrderID FROM Orderss", con)
        con.Open()
        Dim reader = cmd.ExecuteReader()
        While reader.Read()
            ComboBox1.Items.Add(reader("OrderID").ToString())
        End While
        con.Close()
    End Sub

    Private Sub LoadCustomerName()
        If ComboBox1.SelectedItem Is Nothing Then Exit Sub
        Dim cmd As New SqlCommand("SELECT c.CustomerName, o.TotalPrice FROM Orderss o JOIN Customerss c ON o.CustomerID = c.CustomerID WHERE o.OrderID = @OrderID", con)
        cmd.Parameters.AddWithValue("@OrderID", ComboBox1.SelectedItem)
        con.Open()
        Dim reader = cmd.ExecuteReader()
        If reader.Read() Then
            TextBox3.Text = reader("CustomerName").ToString()
            TextBox4.Text = reader("TotalPrice").ToString() ' Set PaymentAmount from TotalPrice
        End If
        con.Close()
    End Sub

    Private Sub LoadPayments()
        Dim da As New SqlDataAdapter("SELECT * FROM Paymentss", con)
        Dim dt As New DataTable()
        da.Fill(dt)
        DataGridView1.DataSource = dt
    End Sub

    Private Function GetNextPaymentID() As String
        Dim nextID As Integer = 1
        Dim cmd As New SqlCommand("SELECT MAX(PaymentID) FROM Paymentss", con)
        con.Open()
        Dim result = cmd.ExecuteScalar()
        If Not IsDBNull(result) Then
            nextID = Convert.ToInt32(result) + 1
        End If
        con.Close()
        Return nextID.ToString()
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click 'Add
        If ComboBox1.SelectedItem = Nothing Or ComboBox2.SelectedItem = Nothing Or TextBox4.Text = "" Then
            MessageBox.Show("Please fill all fields.")
            Return
        End If
        Dim cmd As New SqlCommand("INSERT INTO Paymentss (OrderID, PaymentMethod, PaymentAmount, PaymentDate) VALUES (@OID, @Method, @Amt, @Date)", con)
        cmd.Parameters.AddWithValue("@ID", TextBox1.Text)
        cmd.Parameters.AddWithValue("@OID", ComboBox1.SelectedItem)
        cmd.Parameters.AddWithValue("@Method", ComboBox2.SelectedItem)
        cmd.Parameters.AddWithValue("@Amt", TextBox4.Text)
        cmd.Parameters.AddWithValue("@Date", DateTimePicker1.Value)
        con.Open()
        cmd.ExecuteNonQuery()
        con.Close()
        MessageBox.Show("Payment added successfully!")
        LoadPayments()
        ClearFields()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click 'Update
        Dim cmd As New SqlCommand("UPDATE Paymentss SET OrderID=@OID, PaymentMethod=@Method, PaymentAmount=@Amt, PaymentDate=@Date WHERE PaymentID=@ID", con)
        cmd.Parameters.AddWithValue("@ID", TextBox1.Text)
        cmd.Parameters.AddWithValue("@OID", ComboBox1.SelectedItem)
        cmd.Parameters.AddWithValue("@Method", ComboBox2.SelectedItem)
        cmd.Parameters.AddWithValue("@Amt", TextBox4.Text)
        cmd.Parameters.AddWithValue("@Date", DateTimePicker1.Value)
        con.Open()
        cmd.ExecuteNonQuery()
        con.Close()
        MessageBox.Show("Payment updated successfully!")
        LoadPayments()
        ClearFields()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click 'Delete
        Dim cmd As New SqlCommand("DELETE FROM Paymentss WHERE PaymentID=@ID", con)
        cmd.Parameters.AddWithValue("@ID", TextBox1.Text)
        con.Open()
        cmd.ExecuteNonQuery()
        con.Close()
        MessageBox.Show("Payment deleted successfully!")
        LoadPayments()
        ClearFields()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click 'Clear
        ClearFields()
    End Sub

    Private Sub ClearFields()
        TextBox1.Text = GetNextPaymentID()
        ComboBox1.SelectedIndex = -1
        ComboBox2.SelectedIndex = -1
        TextBox3.Clear()
        TextBox4.Clear()
        DateTimePicker1.Value = Date.Now
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        LoadCustomerName()
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim row = DataGridView1.Rows(e.RowIndex)
            TextBox1.Text = row.Cells("PaymentID").Value.ToString()
            ComboBox1.SelectedItem = row.Cells("OrderID").Value.ToString()
            ComboBox2.SelectedItem = row.Cells("PaymentMethod").Value.ToString()
            TextBox4.Text = row.Cells("PaymentAmount").Value.ToString()
            DateTimePicker1.Value = Convert.ToDateTime(row.Cells("PaymentDate").Value)
            LoadCustomerName()
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click 'Search
        Dim searchText = TextBox6.Text
        Dim da As New SqlDataAdapter("SELECT p.* FROM Paymentss p JOIN Orderss o ON p.OrderID = o.OrderID JOIN Customerss c ON o.CustomerID = c.CustomerID WHERE c.CustomerName LIKE @Search", con)
        da.SelectCommand.Parameters.AddWithValue("@Search", "%" & searchText & "%")
        Dim dt As New DataTable()
        da.Fill(dt)
        DataGridView1.DataSource = dt
    End Sub

    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged 'Live search
        Button5.PerformClick()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click 'Reset
        LoadPayments()
        TextBox6.Clear()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click 'Print
        PPD.Document = PD
        PPD.ShowDialog()
    End Sub

    Private Sub PD_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PD.PrintPage
        Dim fontHeader As New Font("Arial", 16, FontStyle.Bold)
        Dim fontSubHeader As New Font("Arial", 12, FontStyle.Bold)
        Dim fontContent As New Font("Arial", 11)
        Dim y As Integer = 50

        ' Bill Title
        e.Graphics.DrawString("APPLIANCE STORE", fontHeader, Brushes.Black, 220, y)
        y += 40
        e.Graphics.DrawString("Payment Receipt", fontSubHeader, Brushes.Black, 270, y)
        y += 40

        ' Print one selected row as a bill
        If DataGridView1.SelectedRows.Count > 0 Then
            Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)

            e.Graphics.DrawString("Payment ID: ", fontSubHeader, Brushes.Black, 50, y)
            e.Graphics.DrawString(row.Cells("PaymentID").Value.ToString(), fontContent, Brushes.Black, 200, y)
            y += 30

            e.Graphics.DrawString("Order ID: ", fontSubHeader, Brushes.Black, 50, y)
            e.Graphics.DrawString(row.Cells("OrderID").Value.ToString(), fontContent, Brushes.Black, 200, y)
            y += 30

            e.Graphics.DrawString("Customer Name: ", fontSubHeader, Brushes.Black, 50, y)
            e.Graphics.DrawString(TextBox3.Text, fontContent, Brushes.Black, 200, y)
            y += 30

            e.Graphics.DrawString("Payment Method: ", fontSubHeader, Brushes.Black, 50, y)
            e.Graphics.DrawString(row.Cells("PaymentMethod").Value.ToString(), fontContent, Brushes.Black, 200, y)
            y += 30

            e.Graphics.DrawString("Payment Amount: ", fontSubHeader, Brushes.Black, 50, y)
            e.Graphics.DrawString("₹" & row.Cells("PaymentAmount").Value.ToString(), fontContent, Brushes.Black, 200, y)
            y += 30

            e.Graphics.DrawString("Payment Date: ", fontSubHeader, Brushes.Black, 50, y)
            e.Graphics.DrawString(Convert.ToDateTime(row.Cells("PaymentDate").Value).ToShortDateString(), fontContent, Brushes.Black, 200, y)
            y += 50

            e.Graphics.DrawString("Thank you for your payment!", fontSubHeader, Brushes.Black, 180, y)
        Else
            e.Graphics.DrawString("Please select a payment row to print the bill.", fontContent, Brushes.Red, 100, y)
        End If
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

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork

    End Sub

    Private Sub Label8_Click(sender As Object, e As EventArgs) Handles Label8.Click
        TextBox1.Visible = True
        Label1.Visible = True
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        TextBox1.Visible = False
        Label1.Visible = False
    End Sub
End Class
