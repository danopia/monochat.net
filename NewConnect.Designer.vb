<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNewConnect
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnOk = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.lblDisplay = New System.Windows.Forms.Label
        Me.lblServer = New System.Windows.Forms.Label
        Me.lblPass = New System.Windows.Forms.Label
        Me.lblReal = New System.Windows.Forms.Label
        Me.lblNick = New System.Windows.Forms.Label
        Me.txtDisplay = New System.Windows.Forms.TextBox
        Me.txtServer = New System.Windows.Forms.TextBox
        Me.txtPass = New System.Windows.Forms.TextBox
        Me.txtReal = New System.Windows.Forms.TextBox
        Me.txtNick = New System.Windows.Forms.TextBox
        Me.cmbProfile = New System.Windows.Forms.ComboBox
        Me.lblProfile = New System.Windows.Forms.Label
        Me.cmbProto = New System.Windows.Forms.ComboBox
        Me.lblProto = New System.Windows.Forms.Label
        Me.txtAjoin = New System.Windows.Forms.TextBox
        Me.lblAjoin = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'btnOk
        '
        Me.btnOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOk.Location = New System.Drawing.Point(193, 222)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(67, 23)
        Me.btnOk.TabIndex = 0
        Me.btnOk.Text = "&OK"
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(266, 222)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(67, 23)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "&Cancel"
        '
        'lblDisplay
        '
        Me.lblDisplay.AutoSize = True
        Me.lblDisplay.Location = New System.Drawing.Point(12, 15)
        Me.lblDisplay.Name = "lblDisplay"
        Me.lblDisplay.Size = New System.Drawing.Size(50, 13)
        Me.lblDisplay.TabIndex = 2
        Me.lblDisplay.Text = "&Network:"
        '
        'lblServer
        '
        Me.lblServer.AutoSize = True
        Me.lblServer.Location = New System.Drawing.Point(12, 41)
        Me.lblServer.Name = "lblServer"
        Me.lblServer.Size = New System.Drawing.Size(41, 13)
        Me.lblServer.TabIndex = 3
        Me.lblServer.Text = "&Server:"
        '
        'lblPass
        '
        Me.lblPass.AutoSize = True
        Me.lblPass.Location = New System.Drawing.Point(12, 67)
        Me.lblPass.Name = "lblPass"
        Me.lblPass.Size = New System.Drawing.Size(56, 13)
        Me.lblPass.TabIndex = 4
        Me.lblPass.Text = "&Password:"
        '
        'lblReal
        '
        Me.lblReal.AutoSize = True
        Me.lblReal.Location = New System.Drawing.Point(12, 93)
        Me.lblReal.Name = "lblReal"
        Me.lblReal.Size = New System.Drawing.Size(58, 13)
        Me.lblReal.TabIndex = 5
        Me.lblReal.Text = "&Realname:"
        '
        'lblNick
        '
        Me.lblNick.AutoSize = True
        Me.lblNick.Location = New System.Drawing.Point(12, 119)
        Me.lblNick.Name = "lblNick"
        Me.lblNick.Size = New System.Drawing.Size(32, 13)
        Me.lblNick.TabIndex = 6
        Me.lblNick.Text = "N&ick:"
        '
        'txtDisplay
        '
        Me.txtDisplay.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDisplay.Location = New System.Drawing.Point(76, 12)
        Me.txtDisplay.Name = "txtDisplay"
        Me.txtDisplay.Size = New System.Drawing.Size(257, 20)
        Me.txtDisplay.TabIndex = 7
        '
        'txtServer
        '
        Me.txtServer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtServer.Location = New System.Drawing.Point(76, 38)
        Me.txtServer.Name = "txtServer"
        Me.txtServer.Size = New System.Drawing.Size(257, 20)
        Me.txtServer.TabIndex = 8
        '
        'txtPass
        '
        Me.txtPass.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPass.Location = New System.Drawing.Point(76, 64)
        Me.txtPass.Name = "txtPass"
        Me.txtPass.Size = New System.Drawing.Size(257, 20)
        Me.txtPass.TabIndex = 9
        Me.txtPass.UseSystemPasswordChar = True
        '
        'txtReal
        '
        Me.txtReal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtReal.Location = New System.Drawing.Point(76, 90)
        Me.txtReal.Name = "txtReal"
        Me.txtReal.Size = New System.Drawing.Size(257, 20)
        Me.txtReal.TabIndex = 10
        '
        'txtNick
        '
        Me.txtNick.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtNick.Location = New System.Drawing.Point(76, 116)
        Me.txtNick.Name = "txtNick"
        Me.txtNick.Size = New System.Drawing.Size(257, 20)
        Me.txtNick.TabIndex = 11
        '
        'cmbProfile
        '
        Me.cmbProfile.FormattingEnabled = True
        Me.cmbProfile.Location = New System.Drawing.Point(76, 195)
        Me.cmbProfile.MaxDropDownItems = 50
        Me.cmbProfile.Name = "cmbProfile"
        Me.cmbProfile.Size = New System.Drawing.Size(257, 21)
        Me.cmbProfile.TabIndex = 12
        Me.cmbProfile.Text = "New Profile"
        '
        'lblProfile
        '
        Me.lblProfile.AutoSize = True
        Me.lblProfile.Location = New System.Drawing.Point(12, 201)
        Me.lblProfile.Name = "lblProfile"
        Me.lblProfile.Size = New System.Drawing.Size(39, 13)
        Me.lblProfile.TabIndex = 13
        Me.lblProfile.Text = "Pro&file:"
        '
        'cmbProto
        '
        Me.cmbProto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbProto.FormattingEnabled = True
        Me.cmbProto.Items.AddRange(New Object() {"IRC", "MonoNet", "M-Talk"})
        Me.cmbProto.Location = New System.Drawing.Point(76, 168)
        Me.cmbProto.Name = "cmbProto"
        Me.cmbProto.Size = New System.Drawing.Size(257, 21)
        Me.cmbProto.TabIndex = 14
        '
        'lblProto
        '
        Me.lblProto.AutoSize = True
        Me.lblProto.Location = New System.Drawing.Point(12, 171)
        Me.lblProto.Name = "lblProto"
        Me.lblProto.Size = New System.Drawing.Size(49, 13)
        Me.lblProto.TabIndex = 15
        Me.lblProto.Text = "Pro&tocol:"
        '
        'txtAjoin
        '
        Me.txtAjoin.Location = New System.Drawing.Point(76, 142)
        Me.txtAjoin.Name = "txtAjoin"
        Me.txtAjoin.Size = New System.Drawing.Size(257, 20)
        Me.txtAjoin.TabIndex = 16
        '
        'lblAjoin
        '
        Me.lblAjoin.AutoSize = True
        Me.lblAjoin.Location = New System.Drawing.Point(12, 145)
        Me.lblAjoin.Name = "lblAjoin"
        Me.lblAjoin.Size = New System.Drawing.Size(51, 13)
        Me.lblAjoin.TabIndex = 17
        Me.lblAjoin.Text = "A&uto join:"
        '
        'frmNewConnect
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(345, 257)
        Me.Controls.Add(Me.lblAjoin)
        Me.Controls.Add(Me.txtAjoin)
        Me.Controls.Add(Me.lblProto)
        Me.Controls.Add(Me.cmbProto)
        Me.Controls.Add(Me.lblProfile)
        Me.Controls.Add(Me.cmbProfile)
        Me.Controls.Add(Me.txtNick)
        Me.Controls.Add(Me.txtReal)
        Me.Controls.Add(Me.txtPass)
        Me.Controls.Add(Me.txtServer)
        Me.Controls.Add(Me.txtDisplay)
        Me.Controls.Add(Me.lblNick)
        Me.Controls.Add(Me.lblReal)
        Me.Controls.Add(Me.lblPass)
        Me.Controls.Add(Me.lblServer)
        Me.Controls.Add(Me.lblDisplay)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOk)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmNewConnect"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Connect to New Server"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnOk As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents lblDisplay As System.Windows.Forms.Label
    Friend WithEvents lblServer As System.Windows.Forms.Label
    Friend WithEvents lblPass As System.Windows.Forms.Label
    Friend WithEvents lblReal As System.Windows.Forms.Label
    Friend WithEvents lblNick As System.Windows.Forms.Label
    Friend WithEvents txtDisplay As System.Windows.Forms.TextBox
    Friend WithEvents txtServer As System.Windows.Forms.TextBox
    Friend WithEvents txtPass As System.Windows.Forms.TextBox
    Friend WithEvents txtReal As System.Windows.Forms.TextBox
    Friend WithEvents txtNick As System.Windows.Forms.TextBox
    Friend WithEvents cmbProfile As System.Windows.Forms.ComboBox
    Friend WithEvents lblProfile As System.Windows.Forms.Label
    Friend WithEvents cmbProto As System.Windows.Forms.ComboBox
    Friend WithEvents lblProto As System.Windows.Forms.Label
    Friend WithEvents txtAjoin As System.Windows.Forms.TextBox
    Friend WithEvents lblAjoin As System.Windows.Forms.Label

End Class
