<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form9
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form9))
        Label3 = New Label()
        Label2 = New Label()
        Button4 = New Button()
        Button3 = New Button()
        Button1 = New Button()
        Button5 = New Button()
        SuspendLayout()
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.BackColor = Color.AliceBlue
        Label3.Font = New Font("Showcard Gothic", 14F)
        Label3.Location = New Point(735, 217)
        Label3.Name = "Label3"
        Label3.Size = New Size(157, 30)
        Label3.TabIndex = 13
        Label3.Text = "Dashboard"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.BackColor = Color.AliceBlue
        Label2.Font = New Font("Showcard Gothic", 15F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label2.Location = New Point(735, 115)
        Label2.Name = "Label2"
        Label2.Size = New Size(328, 31)
        Label2.TabIndex = 12
        Label2.Text = "Welcome to AppliStock"
        ' 
        ' Button4
        ' 
        Button4.BackColor = Color.MediumPurple
        Button4.Font = New Font("Segoe UI", 9F, FontStyle.Bold)
        Button4.ForeColor = SystemColors.ControlLightLight
        Button4.Location = New Point(625, 323)
        Button4.Name = "Button4"
        Button4.Size = New Size(159, 29)
        Button4.TabIndex = 11
        Button4.Text = "CUSTOMERS"
        Button4.UseVisualStyleBackColor = False
        ' 
        ' Button3
        ' 
        Button3.BackColor = Color.MediumPurple
        Button3.Font = New Font("Segoe UI", 9F, FontStyle.Bold)
        Button3.ForeColor = SystemColors.ControlLightLight
        Button3.Location = New Point(849, 323)
        Button3.Name = "Button3"
        Button3.Size = New Size(139, 29)
        Button3.TabIndex = 10
        Button3.Text = "NEW ORDERS"
        Button3.UseVisualStyleBackColor = False
        ' 
        ' Button1
        ' 
        Button1.BackColor = Color.MediumPurple
        Button1.Font = New Font("Segoe UI", 9F, FontStyle.Bold)
        Button1.ForeColor = SystemColors.ControlLightLight
        Button1.Location = New Point(1039, 323)
        Button1.Name = "Button1"
        Button1.Size = New Size(159, 29)
        Button1.TabIndex = 14
        Button1.Text = "PAYMENT"
        Button1.UseVisualStyleBackColor = False
        ' 
        ' Button5
        ' 
        Button5.Location = New Point(869, 447)
        Button5.Name = "Button5"
        Button5.Size = New Size(94, 29)
        Button5.TabIndex = 15
        Button5.Text = "LOGOUT"
        Button5.UseVisualStyleBackColor = True
        ' 
        ' Form9
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), Image)
        BackgroundImageLayout = ImageLayout.Center
        ClientSize = New Size(1838, 862)
        Controls.Add(Button5)
        Controls.Add(Button1)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Button4)
        Controls.Add(Button3)
        Name = "Form9"
        Text = "Form9"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Button4 As Button
    Friend WithEvents Button3 As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents Button5 As Button
End Class
