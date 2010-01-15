Public Class frmMain
    Public OpenedTabs As New Dictionary(Of String, TabPage)
    Public XMLSettings As New Xml.XmlDocument()
    Public XMLPath As String = My.Application.Info.AssemblyName & ".config.xml"
    Public MyNick As String = ""
    Public Tabs As New Dictionary(Of String, Dictionary(Of String, RichTextBox))
    Dim Connection As IRC
    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Connection = New IRC("irc.freenode.net", "danopia`monochat", Nothing, "danopia using monochat client")
        'Connection = New IRC("zeus.devnode.org", "danopia`monochat", Nothing, "danopia using monochat client")
        'Connection = New IRC("localhost", "danopia`monochat", Nothing, "danopia using monochat client")
        'treChannels.Nodes.Add("@", "DevNode")
        'treChannels.Nodes("@").Expand()
    End Sub
    Public Sub AddNetwork(ByVal Display As String, ByVal Server As String, ByVal Name As String, ByVal Pass As String, ByVal RealName As String)
        SetStatus(1)
        LogIRC(Chr(3) & "3* Connecting to " & Server, Display)
        treChannels.Nodes(Display.ToLower).Tag = New IRC(Server, Name, Pass, RealName)
        treChannels.Nodes(Display.ToLower).Tag.Network = Display
    End Sub
    Public Sub SetStatus(ByVal State As Byte)
        Select Case State
            Case 0
                Me.Text = "MonoChat [Not connected]"
            Case 1
                Me.Text = "MonoChat [Connecting]"
            Case 2
                Me.Text = "MonoChat [Connected]"
            Case 3
                Me.Text = "MonoChat [Lagging]"
            Case 5
                Me.Text = "MonoChat"
        End Select
    End Sub
    Public Sub Log(ByVal Text As String, Optional ByVal Network As String = "Debug", Optional ByVal Tab As String = "")
        If Not Tabs.ContainsKey(Network.ToLower) Then
            Dim Box As New RichTextBox
            pnlScrollback.Controls.Add(Box)
            Box.Dock = DockStyle.Fill
            Box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Box.Margin = New System.Windows.Forms.Padding(0)
            Box.Name = Network
            Box.ReadOnly = True
            Box.TabIndex = 3
            Box.Text = "Log of " & Tab & vbNewLine
            Tabs.Add(Network.ToLower, New Dictionary(Of String, RichTextBox))
        End If
        If Not Tabs(Network.ToLower).ContainsKey(Tab.ToLower) Then
            Dim Box As New RichTextBox
            pnlScrollback.Controls.Add(Box)
            Box.Dock = DockStyle.Fill
            Box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Box.Margin = New System.Windows.Forms.Padding(0)
            Box.Name = Tab
            Box.ReadOnly = True
            Box.TabIndex = 3
            Box.Text = "Log of " & Tab & vbNewLine
            Tabs(Network.ToLower).Add(Tab.ToLower, Box)
        End If
        If Not treChannels.Nodes.ContainsKey(Network.ToLower) Then
            Dim Node As New TreeNode(Network)
            Node.Name = Network.ToLower
            treChannels.Nodes.Add(Node)
            Node.EnsureVisible()
        End If
        If Not treChannels.Nodes(Network.ToLower).Nodes.ContainsKey(Network.ToLower) And Not Tab = "" Then
            Dim Node As New TreeNode(Tab)
            Node.Name = Tab.ToLower
            treChannels.Nodes(Network.ToLower).Nodes.Add(Node)
            Node.EnsureVisible()
        End If
        Tabs(Network.ToLower)(Tab.ToLower).AppendText(Text)
        Tabs(Network.ToLower)(Tab.ToLower).BringToFront()
    End Sub
    Public Sub JoinedChannel(ByVal Channel As String)
        'Dim Node As New TreeNode(Channel)
        'Node.Name = Channel.ToLower
        'treChannels.Nodes("@").Nodes.Add(Node)
        'Node.EnsureVisible()
        'treChannels.Nodes.Add(Node)
    End Sub
    Public Sub LeftChannel(ByVal Channel As String)
        ' If treChannels.Nodes("@").Nodes.Find(Channel.ToLower, False).Length > 0 Then treChannels.Nodes("@").Nodes.Remove(treChannels.Nodes("@").Nodes(Channel.ToLower))
    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        For Each Node As TreeNode In treChannels.Nodes
            If Node.Level = 0 Then Node.Tag.Handle(False)
        Next
    End Sub
    Public Function GetIRCColor(ByVal ID As Byte) As Color
        Select Case ID
            Case 0 : Return Color.White
            Case 1 : Return Color.Black
            Case 2 : Return Color.DarkBlue
            Case 3 : Return Color.DarkGreen
            Case 4 : Return Color.Red
            Case 5 : Return Color.DarkRed
            Case 6 : Return Color.Violet
            Case 7 : Return Color.Orange
            Case 8 : Return Color.Yellow
            Case 9 : Return Color.Green
            Case 10 : Return Color.DarkCyan
            Case 11 : Return Color.Cyan
            Case 12 : Return Color.Blue
            Case 13 : Return Color.HotPink
            Case 14 : Return Color.DarkGray
            Case 15 : Return Color.LightGray
        End Select
    End Function
    Public Sub LogIRC(ByVal Data As String, Optional ByVal Network As String = "Debug", Optional ByVal Tab As String = "", Optional ByVal IncludePara As Boolean = True)
        'AddTab(Tab)
        'Dim LogBox As WebBrowser = OpenedTabs(Tab.ToLower).Controls(0)
        'LogBox.Size = Drawing.Size.Subtract(tbcLogs.Size, New Drawing.Size(14, 32))
        If Not Tabs.ContainsKey(Network.ToLower) Then
            Dim Box As New RichTextBox
            pnlScrollback.Controls.Add(Box)
            Box.Dock = DockStyle.Fill
            Box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Box.Margin = New System.Windows.Forms.Padding(0)
            Box.Name = Network
            Box.ReadOnly = True
            Box.TabIndex = 3
            Box.Text = "Log of " & Tab & vbNewLine
            Tabs.Add(Network.ToLower, New Dictionary(Of String, RichTextBox))
        End If
        If Not Tabs(Network.ToLower).ContainsKey(Tab.ToLower) Then
            Dim Box As New RichTextBox
            pnlScrollback.Controls.Add(Box)
            Box.Dock = DockStyle.Fill
            Box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Box.Margin = New System.Windows.Forms.Padding(0)
            Box.Name = Tab
            Box.ReadOnly = True
            Box.TabIndex = 3
            Box.Text = "Log of " & Tab & vbNewLine
            Tabs(Network.ToLower).Add(Tab.ToLower, Box)
        End If
        If Not treChannels.Nodes.ContainsKey(Network.ToLower) Then
            Dim Node As New TreeNode(Network)
            Node.Name = Network.ToLower
            treChannels.Nodes.Add(Node)
            Node.EnsureVisible()
        End If
        If Not treChannels.Nodes(Network.ToLower).Nodes.ContainsKey(Tab.ToLower) And Not Tab = "" Then
            Dim Node As New TreeNode(Tab)
            Node.Name = Tab.ToLower
            treChannels.Nodes(Network.ToLower).Nodes.Add(Node)
            Node.EnsureVisible()
        End If
        Data &= vbNewLine
        Dim Txt As RichTextBox = Tabs(Network.ToLower)(Tab.ToLower)
        Dim LastWasColor As Boolean = False
        Dim LastWasBGColor As Boolean = False
        Dim ColorEnabled As Boolean = False
        Dim BGColorEnabled As Boolean = False
        Dim OpenSpans As Byte = 0
        Dim OpenFonts As Byte = 0
        Dim WhichColor As Color = Color.Black
        Dim WhichBGColor As Color = Color.White
        Dim BoldEnabled As Boolean = False
        Dim UnderlineEnabled As Boolean = False
        Dim ThisChunk As String = ""
        Dim Output As String = ""
        Dim PlainText As String = ""
        For Each Chr As Char In Data
            If Asc(Chr) = 3 Then
                LastWasColor = True
                If ThisChunk.Length > 0 Then
                    Txt.AppendText(ThisChunk)
                    ThisChunk = ""
                End If
            ElseIf Asc(Chr) = 2 Then
                BoldEnabled = Not BoldEnabled
                If ThisChunk.Length > 0 Then
                    Txt.AppendText(ThisChunk)
                    ThisChunk = ""
                End If
                Txt.SelectionLength = 0
                Txt.SelectionStart = Txt.Text.Length
                Txt.SelectionFont = New Font(Txt.Font, IIf(BoldEnabled, FontStyle.Bold, 0) Or IIf(UnderlineEnabled, FontStyle.Underline, 0))
            ElseIf Asc(Chr) = 31 Then
                UnderlineEnabled = Not UnderlineEnabled
                If ThisChunk.Length > 0 Then
                    Txt.AppendText(ThisChunk)
                    ThisChunk = ""
                End If
                Txt.SelectionLength = 0
                Txt.SelectionStart = Txt.Text.Length
                Txt.SelectionFont = New Font(Txt.Font, IIf(BoldEnabled, FontStyle.Bold, 0) Or IIf(UnderlineEnabled, FontStyle.Underline, 0))
            ElseIf Asc(Chr) = 15 Then
                If ThisChunk.Length > 0 Then
                    Txt.AppendText(ThisChunk)
                    ThisChunk = ""
                End If
                BoldEnabled = False
                UnderlineEnabled = False
                ColorEnabled = False
                BGColorEnabled = False
                Txt.Select(Txt.Text.Length, 0)
                Txt.SelectionFont = New Font(Txt.Font, IIf(BoldEnabled, FontStyle.Bold, 0) Or IIf(UnderlineEnabled, FontStyle.Underline, 0))
                Txt.SelectionColor = Color.Black
                Txt.SelectionBackColor = Color.White
            ElseIf Char.IsNumber(Chr) And (LastWasColor Or LastWasBGColor) And ThisChunk.Length <= 1 Then
                ThisChunk &= Chr
            ElseIf LastWasColor And Chr = "," Then
                LastWasBGColor = True
                GoTo ColorCode
            ElseIf LastWasBGColor Then
                If ThisChunk = "" Then
                    WhichBGColor = Color.White
                Else
                    Dim BGColorID As Byte = 0
                    Byte.TryParse(ThisChunk, BGColorID)
                    WhichBGColor = GetIRCColor(BGColorID)
                End If
                BGColorEnabled = 1
                Txt.Select(Txt.Text.Length, 0)
                Txt.SelectionBackColor = WhichBGColor
                Txt.SelectionColor = WhichColor
                LastWasBGColor = False
                ThisChunk = Chr
                PlainText &= Chr
                Output &= Chr
            ElseIf LastWasColor Then
ColorCode:      If ThisChunk = "" Then
                    WhichColor = Color.Black
                Else
                    Dim ColorID As Byte = 0
                    Byte.TryParse(ThisChunk, ColorID)
                    WhichColor = GetIRCColor(ColorID)
                End If
                ColorEnabled = 1
                LastWasColor = False
                If Chr = ","c Then
                    ThisChunk = ""
                Else
                    Txt.Select(Txt.Text.Length, 0)
                    Txt.SelectionColor = WhichColor
                    ThisChunk = Chr
                    PlainText &= Chr
                    Output &= Chr
                End If
            Else
                ThisChunk &= Chr
                PlainText &= Chr
            End If
        Next
        Txt.AppendText(ThisChunk)
        Txt.Select(Txt.Text.Length, 0)
        Txt.ScrollToCaret()
        'Txt.BringToFront()

        'CType(OpenedTabs(Tab.ToLower).Tag, IO.StreamWriter).AutoFlush = True
        'If IncludePara Then
        '    LogBox.Document.Body.InnerHtml &= "<p>[" & Now.ToLongTimeString & "] " & Output & "</p>" & vbNewLine
        '    If Not Output.Contains(Config("identpass")) Then
        '        CType(OpenedTabs(Tab.ToLower).Tag, IO.StreamWriter).WriteLine("<p>[" & Now.ToLongTimeString & "] " & Output & "</p>")
        '        If CurrentEvents.ContainsKey(Tab.ToLower) Then CurrentEvents(Tab.ToLower).WriteLine("<p>[" & Now.Subtract(EventStart(CurrentEvents(Tab))).ToString.Split(".")(0) & "] " & Output & "</p>")
        '    End If
        'Else
        '    LogBox.Document.Body.InnerHtml &= "[" & Now.ToLongTimeString & "] " & Output & vbNewLine
        '    If Not Output.Contains(Config("identpass")) Then
        '        CType(OpenedTabs(Tab.ToLower).Tag, IO.StreamWriter).WriteLine("[" & Now.ToLongTimeString & "] " & Output)
        '        If CurrentEvents.ContainsKey(Tab.ToLower) Then CurrentEvents(Tab.ToLower).WriteLine("[" & Now.Subtract(EventStart(CurrentEvents(Tab))).ToString.Split(".")(0) & "] " & Output)
        '    End If
        'End If
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If Not String.IsNullOrEmpty(TextBox1.Text) Then
            For Each Line As String In TextBox1.Text.Split(vbCr(0), vbLf(0))
                If Not String.IsNullOrEmpty(Line) Then
                    If Line.StartsWith("/me ") Then
                        Dim Message As String = Line.Substring(Line.IndexOf(" ") + 1)
                        treChannels.SelectedNode.Parent.Tag.Send("PRIVMSG " & treChannels.SelectedNode.Text & " :ACTION " & Message & Chr(1))
                        LogIRC(Chr(3) & "6* " & treChannels.SelectedNode.Parent.Tag.MyNick & " " & Message, treChannels.SelectedNode.Parent.Text, treChannels.SelectedNode.Text)
                    ElseIf Line.StartsWith("/raw ") Or Line.StartsWith("/quote ") Then
                        treChannels.SelectedNode.Parent.Tag.Send(Line.Substring(Line.IndexOf(" ") + 1))
                    ElseIf Line = "/quit" Then
                        treChannels.SelectedNode.Parent.Tag.Send("QUIT")
                    ElseIf Line.StartsWith("/quit ") Then
                        treChannels.SelectedNode.Parent.Tag.Send("QUIT :" & Line.Substring(Line.IndexOf(" ") + 1))
                    Else
                        treChannels.SelectedNode.Parent.Tag.Send("PRIVMSG " & treChannels.SelectedNode.Text & " :" & Line)
                        LogIRC("<" & treChannels.SelectedNode.Parent.Tag.MyNick & "> " & Line, treChannels.SelectedNode.Parent.Text, treChannels.SelectedNode.Text)
                    End If
                End If
            Next
            TextBox1.Text = ""
        End If
    End Sub
    Private Sub TextBox1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        If TextBox1.Text.Contains(vbCr) Or TextBox1.Text.Contains(vbLf) Then Button1_Click(sender, e)
    End Sub
    Private Sub treChannels_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles treChannels.AfterSelect
        If e.Node.Parent Is Nothing Then
            If Tabs.ContainsKey(e.Node.Text.ToLower) Then
                Tabs(e.Node.Text.ToLower)("").BringToFront()
            End If
        Else
        If Tabs.ContainsKey(e.Node.Parent.Text.ToLower) Then
            If Tabs(e.Node.Parent.Text.ToLower).ContainsKey(e.Node.Text.ToLower) Then
                Tabs(e.Node.Parent.Text.ToLower)(e.Node.Text.ToLower).BringToFront()
            End If
        End If
        End If
    End Sub
    Private Sub tsiNewConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsiNewConnect.Click
        frmNewConnect.Show()
        frmNewConnect.BringToFront()
    End Sub
End Class
