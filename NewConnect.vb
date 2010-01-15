Imports System.Windows.Forms
Public Class frmNewConnect
    Public Config As New Xml.XmlDocument
    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        frmMain.AddNetwork(txtDisplay.Text, txtServer.Text, txtNick.Text, txtPass.Text, txtReal.Text)
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub frmNewConnect_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        cmbProto.SelectedIndex = 0
        Diagnostics.Debug.WriteLine(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData)
        'If Not IO.File.Exists(my.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData & ) Then

        'End If
        'Config.Load()
    End Sub
End Class
