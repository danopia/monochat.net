Public Class IRC
    Public Socket As New Net.Sockets.Socket(Net.Sockets.AddressFamily.InterNetwork, Net.Sockets.SocketType.Stream, Net.Sockets.ProtocolType.Tcp)
    Public MyNick As String = ""
    Public AntiFlood As Double = 0
    Public Stage As Byte = 0
    Public Network As String = "IRC"
    Public NickList As New Dictionary(Of String, List(Of String))
    Public WithEvents TmrAntiFlood As New Timers.Timer(1000)

    Private _buffer As String = ""

    Public Sub New(ByVal Server As String, ByVal Name As String, ByVal Pass As String, ByVal RealName As String)
        Dim Port As UShort = 6667
        If Server.Contains(":") Then
            Port = UShort.Parse(Server.Split(":")(1))
            Server = Server.Split(":")(0)
        End If
worked: Dim worked As Boolean = False
        Try
            Socket.Connect(Server, Port)
            worked = True
        Catch ex As Exception
        End Try
        If Not worked Then GoTo worked
        If Not Pass Is Nothing Then Send("PASS " & Pass)
        Send("NICK " & Name)
        Send("USER monochat localhost 0.0.0.0 :" & RealName)
        MyNick = Name
        Stage = 1
        frmMain.SetStatus(Stage)
    End Sub
    Public Sub Send(ByVal Data As String)
        Try
            If TmrAntiFlood.Enabled Then
                If AntiFlood < 4 Then
                    Socket.Send(System.Text.Encoding.UTF8.GetBytes(Data.Replace(vbCr, "").Replace(vbLf, "") & vbCrLf))
                    frmMain.LogIRC(Chr(3) & "14>>> " & Data, Network)
                ElseIf AntiFlood < 5 Then
                    Socket.Send(System.Text.Encoding.UTF8.GetBytes("PRIVMSG danopia :I spy a flood" & vbCrLf))
                    frmMain.LogIRC(Chr(3) & "3Flood prevented send of this packet: " & Data)
                Else
                    frmMain.LogIRC(Chr(3) & "3A big flood value (" & AntiFlood & " prevented send of this packet: " & Data)
                End If
                AntiFlood += 1 + (Data.Length / 256)
            Else
                Socket.Send(System.Text.Encoding.UTF8.GetBytes(Data.Replace(vbCr, "").Replace(vbLf, "") & vbCrLf))
            End If
        Catch ex As Exception
            frmMain.LogIRC(Chr(3) & "4Error sending packet", Network)
        End Try
    End Sub
    Public Function Handle(ByVal Blocking As Boolean) As Boolean
        Try
            Socket.Blocking = Blocking
        Catch ex As ObjectDisposedException
            Return False
        End Try
        Dim Buffer(512) As Byte
        Dim Length As Integer
        Try
            Length = Socket.Receive(Buffer, 512, Net.Sockets.SocketFlags.None)
        Catch ex As Exception
        End Try
        If Socket.Connected = False Then
            Exit Function
        End If
        If Length = 0 Then
            If Socket.Poll(1000, Net.Sockets.SelectMode.SelectError) Then
                Try
                    Socket.Shutdown(Net.Sockets.SocketShutdown.Both)
                    Exit Function
                Catch ex As Exception
                End Try
            End If
        End If
        _buffer &= System.Text.Encoding.UTF8.GetString(Buffer, 0, Length)

        While _buffer.Contains(vbNewLine)
            Length = _buffer.IndexOf(vbNewLine)
            HandlePacket(_buffer.Substring(0, Length))
            _buffer = _buffer.Remove(0, Length + vbNewLine.Length)
        End While
    End Function

    Public Sub HandlePacket(ByVal Packet As String)
        Try
            Packet = Packet.TrimEnd(vbCr, vbLf)
            'Packet = System.Web.HttpUtility.HtmlEncode(Packet)
            If Not String.IsNullOrEmpty(Packet) Then
                frmMain.LogIRC(Chr(3) & "14<<< " & Packet, Network)
                Dim Args() As String = Packet.Split(" ")
                Dim Command As String = Args(0)
                If Args(0).StartsWith(":") Then
                    Command = Args(1)
                    Args(0) = Args(0).TrimStart(":")
                    Packet = Packet.TrimStart(":")
                End If
                Command = Command.ToLower
                Dim Numeric As UShort = 0
                If UShort.TryParse(Command, Numeric) Then
                    Select Case Numeric
                        Case 439
                            frmMain.LogIRC(Chr(3) & "14Processing connection...", Network)
                        Case 433
                            frmMain.LogIRC(Chr(3) & "14Nick is being used.", Network)
                        Case 451
                            frmMain.LogIRC(Chr(3) & "14Please register first.", Network)
                        Case 1
                            frmMain.LogIRC(Chr(3) & "14Welcome.", Network)
                            'Send("MAP")
                            'Send("JOIN #l0gg3r,#botters,#devnode,#mononet")
                            Send("JOIN #l0gg3r,#botters,#mukt.in,#danopia,#mononet")
                            TmrAntiFlood.Enabled = True
                            Stage = 2
                            frmMain.SetStatus(Stage)
                        Case 353
                            Dim Channel As String = Args(4)
                            If NickList.ContainsKey(Channel.ToLower) Then
                                NickList(Channel.ToLower).AddRange(Packet.Substring(Packet.IndexOf(":") + 1).Trim().Replace("~", "").Replace("&", "").Replace("@", "").Replace("%", "").Replace("+", "").ToLower.Split(" "))
                            End If
                            'Case 6
                            '    If CurrentTimeStamp = Nothing Then
                            '        CurrentTimeStamp = Now
                            '        Database.ExecuteNonQuery("INSERT INTO stat_dates (date_unix) VALUES (" & ConvertTimestamp(CurrentTimeStamp) & ")")
                            '        CurrentTimeStampID = Database.ExecuteScalar("SELECT date_id FROM stat_dates WHERE date_unix = " & ConvertTimestamp(CurrentTimeStamp) & " LIMIT 1")
                            '    End If
                            '    Dim Regex As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(Packet, "([a-zA-Z.]+) +\(([0-9]+)\)")
                            '    Dim Server As String = Regex.Groups(1).Value
                            '    Dim Users As Integer = Regex.Groups(2).Value
                            '    Dim wcEng2 As ChartEngine = tabChart2.Tag
                            '    Dim wcEng3 As ChartEngine = tabChart3.Tag
                            '    Dim slChart As Chart = wcEng3.Charts(0)
                            '    Dim FoundIt As Boolean = False
                            '    Dim Color As Color = Nothing
                            '    For Each DataPoint As ChartPoint In slChart.Data
                            '        If DataPoint.XValue = Server Then
                            '            slChart.Data(slChart.Data.IndexOf(DataPoint)).YValue = Users
                            '            Color = CType(slChart, PieChart).Colors(slChart.Data.IndexOf(DataPoint))
                            '            FoundIt = True
                            '            Exit For
                            '        End If
                            '    Next
                            '    If Not FoundIt Then
                            '        slChart.Data.Add(New ChartPoint(Server, Users))
                            '        Color = CType(slChart, PieChart).Colors(slChart.Data.Count - 1)
                            '    End If
                            '    Color = Drawing.Color.FromArgb(Color.A, Color.R / 1.5, Color.G / 1.5, Color.B / 1.5)
                            '    slChart = wcEng2.Charts.FindChart(Server)
                            '    If slChart Is Nothing Then
                            '        slChart = New SmoothLineChart()
                            '        slChart.Name = Server
                            '        wcEng2.Charts.Add(slChart)
                            '        slChart.Line.Color = IIf(Color = Nothing, Drawing.Color.Red, Color)
                            '        slChart.Legend = Server
                            '        slChart.ShowLineMarkers = False
                            '        slChart.ShowLegend = True
                            '        slChart.DataLabels.Visible = False
                            '        slChart.Shadow.Visible = True
                            '    End If
                            '    slChart.Line.Color = IIf(Color = Nothing, Drawing.Color.Red, Color)
                            '    slChart.Fill.Color = IIf(Color = Nothing, Drawing.Color.Red, Color)
                            '    slChart.Data.Add(New ChartPoint(CurrentTimeStamp.ToShortTimeString, Users))
                            '    Dim ServerID As Integer = 0
                            '    If Database.Servers.ContainsKey(Server) Then
                            '        ServerID = Database.Servers(Server)
                            '    Else
                            '        Database.ExecuteNonQuery("INSERT INTO lgr_servers (server_name) VALUES (""" & Server.Replace("""", "\""") & """)")
                            '        ServerID = Database.ExecuteScalar("SELECT server_id FROM lgr_servers WHERE server_name = """ & Server.Replace("""", "\""") & """ LIMIT 1")
                            '        Database.Servers.Add(Server, ServerID)
                            '    End If
                            '    Database.ExecuteNonQuery("INSERT INTO stat_server_history (date_id, server_id, entry_users) VALUES (" & CurrentTimeStampID & ", " & ServerID & ", " & Users & ")")
                            'Case 7
                            '    If DoneCharts Then
                            '        UploadCharts()
                            '        CurrentTimeStamp = Nothing
                            '        CurrentTimeStampID = Nothing
                            '        DoneCharts = False
                            '    Else
                            '        DoneCharts = True
                            '    End If
                            'Case 251
                            '    If CurrentTimeStamp = Nothing Then
                            '        CurrentTimeStamp = Now
                            '        Database.ExecuteNonQuery("INSERT INTO stat_dates (date_unix) VALUES (" & ConvertTimestamp(CurrentTimeStamp) & ")")
                            '        CurrentTimeStampID = Database.ExecuteScalar("SELECT date_id FROM stat_dates WHERE date_unix = " & ConvertTimestamp(CurrentTimeStamp) & " LIMIT 1")
                            '    End If
                            '    Dim Regex As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(Packet, "[^0-9]([0-9]+) server")
                            '    Dim Servers As Integer = Regex.Groups(1).Value
                            '    Dim wcEng As ChartEngine = tabChart1.Tag
                            '    Dim slChart As Chart = wcEng.Charts.FindChart("Servers")
                            '    slChart.Data.Add(New ChartPoint(CurrentTimeStamp.ToShortTimeString, Servers))
                            '    Database.ExecuteNonQuery("INSERT INTO stat_network_history (date_id, net_server_count) VALUES (" & CurrentTimeStampID & ", " & Servers & ")")
                            'Case 252
                            '    Dim Opers As Integer = Args(3)
                            '    Dim wcEng As ChartEngine = tabChart1.Tag
                            '    Dim slChart As Chart = wcEng.Charts.FindChart("Opers")
                            '    slChart.Data.Add(New ChartPoint(CurrentTimeStamp.ToShortTimeString, Opers))
                            '    Database.ExecuteNonQuery("UPDATE stat_network_history SET net_oper_count = " & Opers & " WHERE date_id = " & CurrentTimeStampID & " LIMIT 1")
                            'Case 254
                            '    Dim Channels As Integer = Args(3)
                            '    Dim wcEng As ChartEngine = tabChart1.Tag
                            '    Dim slChart As Chart = wcEng.Charts.FindChart("Channels")
                            '    slChart.Data.Add(New ChartPoint(CurrentTimeStamp.ToShortTimeString, Channels))
                            '    Database.ExecuteNonQuery("UPDATE stat_network_history SET net_channel_count = " & Channels & " WHERE date_id = " & CurrentTimeStampID & " LIMIT 1")
                            'Case 266
                            '    Dim Regex As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(Packet, "Global Users: ([0-9]+)")
                            '    Dim Users As Integer = Regex.Groups(1).Value
                            '    Dim wcEng As ChartEngine = tabChart1.Tag
                            '    Dim slChart As Chart = wcEng.Charts.FindChart("Users")
                            '    Database.ExecuteNonQuery("UPDATE stat_network_history SET net_user_count = " & Users & " WHERE date_id = " & CurrentTimeStampID & " LIMIT 1")
                            '    slChart.Data.Add(New ChartPoint(CurrentTimeStamp.ToShortTimeString, Users))
                            '    If DoneCharts Then
                            '        UploadCharts()
                            '        CurrentTimeStamp = Nothing
                            '        CurrentTimeStampID = Nothing
                            '        DoneCharts = False
                            '    Else
                            '        DoneCharts = True
                            '    End If
                        Case Else
                            'frmMain.LogIRC(Numeric & " " & Packet)
                            frmMain.LogIRC(Packet.Substring(Packet.IndexOf(":") + 1), Network)
                    End Select
                Else
                    Select Case Command
                        Case "ping"
                            If Args.Length >= 2 Then
                                Send("PONG " & Args(1))
                            Else
                                Send("PONG")
                            End If
                        Case "notice"
                            If Not Args(1).ToLower = "auth" And Not Args(1).ToLower = "l0gg3r" Then
                                If Args(3).StartsWith(":") Then
                                    frmMain.LogIRC(Chr(3) & "5-" & Args(0).Split("!")(0) & "- " & Packet.Substring(Packet.IndexOf(":") + 1), Network, Args(2))
                                Else
                                    frmMain.LogIRC(Chr(3) & "5-" & Args(0).Split("!")(0) & "- " & Args(3), Network, Args(2))
                                End If
                            End If
                        Case "mode"
                            If (Args(3) = ":+e" Or Args(3) = ":+r") And Args(2).ToLower = "l0gg3r".ToLower Then
                                'Send("JOIN " & Config("autojoin"))
                                Send("JOIN #l0gg3r,#botters")
                            ElseIf Args(2).StartsWith("#") Then
                                frmMain.LogIRC(Chr(3) & "3* " & Args(0).Split("!")(0) & " sets mode: " & Packet.Substring(Packet.IndexOf(Args(3))), Network, Args(2))
                            End If
                        Case "join"
                            Dim Who As String = Args(0).Split("!"c)(0)
                            frmMain.LogIRC(Chr(3) & "3* Joins: " & Who & " (" & Args(0).Split("!")(1) & ")", Network, Args(2).TrimStart(":"))
                            If MyNick.ToLower = Who.ToLower Then
                                NickList.Add(Args(2).ToLower.TrimStart(":"), New List(Of String))
                                frmMain.JoinedChannel(Args(2).TrimStart(":"))
                            End If
                            If NickList.ContainsKey(Args(2).TrimStart(":").ToLower) Then NickList(Args(2).TrimStart(":").ToLower).Add(Who.ToLower)
                        Case "quit"
                            Dim Who As String = Args(0).Split("!"c)(0)
                            Dim PacketToSend As String = ""
                            If Args.Length >= 3 Then
                                If Args(2).StartsWith(":") Then
                                    PacketToSend = Chr(3) & "2* Quits: " & Who & " (" & Args(0).Split("!")(1) & ") (" & Packet.Substring(Packet.IndexOf(":") + 1) & ")"
                                Else
                                    PacketToSend = Chr(3) & "2* Quits: " & Who & " (" & Args(0).Split("!")(1) & ") (" & Args(2) & ")"
                                End If
                            Else
                                PacketToSend = Chr(3) & "2* Quits: " & Who & " (" & Args(0).Split("!")(1) & ")"
                            End If
                            For Each Channel As KeyValuePair(Of String, List(Of String)) In NickList
                                If Channel.Value.Contains(Who.ToLower) Then
                                    frmMain.LogIRC(PacketToSend, Network, Channel.Key)
                                    NickList(Channel.Key).Remove(Who.ToLower)
                                End If
                            Next
                        Case "part"
                            Dim Who As String = Args(0).Split("!"c)(0)
                            If NickList.ContainsKey(Args(2).ToLower) Then
                                If NickList(Args(2).ToLower).Contains(Who.ToLower) Then NickList(Args(2).ToLower).Remove(Who.ToLower)
                            End If
                            If MyNick.ToLower = Who.ToLower And NickList.ContainsKey(Args(2).ToLower) Then NickList.Remove(Args(2).ToLower)
                            If Args.Length >= 4 Then
                                If Args(3).StartsWith(":") Then
                                    frmMain.LogIRC(Chr(3) & "3* Parts: " & Who & " (" & Args(0).Split("!")(1) & ") (" & Packet.Substring(Packet.IndexOf(":") + 1) & ")", Network, Args(2))
                                Else
                                    frmMain.LogIRC(Chr(3) & "3* Parts: " & Who & " (" & Args(0).Split("!")(1) & ") (" & Args(3) & ")", Network, Args(2))
                                End If
                            Else
                                frmMain.LogIRC(Chr(3) & "3* Parts: " & Who & " (" & Args(0).Split("!")(1) & ")", Network, Args(2))
                            End If
                            If MyNick.ToLower = Who.ToLower Then
                                frmMain.LeftChannel(Args(2))
                            End If
                        Case "kick"
                            Dim WhoDoneIt As String = Args(0).Split("!"c)(0)
                            If MyNick.ToLower = Args(3).ToLower And NickList.ContainsKey(Args(2).ToLower) Then NickList.Remove(Args(2).ToLower)
                            If NickList.ContainsKey(Args(2).ToLower) Then
                                If NickList(Args(2).ToLower).Contains(Args(3).ToLower) Then NickList(Args(2).ToLower).Remove(Args(3).ToLower)
                            End If
                            If Args.Length >= 5 Then
                                If Args(4).StartsWith(":") Then
                                    frmMain.LogIRC(Chr(3) & "3* " & Args(3) & " was kicked by " & WhoDoneIt & " (" & Packet.Substring(Packet.IndexOf(":") + 1) & ")", Network, Args(2))
                                Else
                                    frmMain.LogIRC(Chr(3) & "3* " & Args(3) & " was kicked by " & WhoDoneIt & " (" & Args(4) & ")", Network, Args(2))
                                End If
                            Else
                                frmMain.LogIRC(Chr(3) & "3* " & Args(3) & " was kicked by " & WhoDoneIt, Network, Args(2))
                            End If
                            If MyNick.ToLower = Args(3).ToLower Then
                                frmMain.LeftChannel(Args(2))
                            End If
                        Case "privmsg"
                            Dim Who As String = Args(0).Split("!"c)(0)
                            Dim Ident As String = Args(0).Split("!"c, "@"c)(1)
                            Dim Host As String = Args(0).Split("@"c)(1)
                            Dim RawWho As String = Who
                            'Dim Color As String = ""
                            'If Host.StartsWith("fullcirclemagazine") Then Color = "14"
                            'If Host.StartsWith("freenode") Or Host.ToLower.StartsWith("devnode") Then Color = "4"
                            ''Color = NickColor(Who.ToLower, Color)
                            'If Not Color = "" Then
                            '    Who = "<span style=""color: " & Color & ";"">" & Who & "</span>"
                            'End If
                            'If Host.StartsWith("fullcirclemagazine") Or Host.ToLower.StartsWith("devnode") Then Who = "<strong>" & Who & "</strong>"
                            If Args(2).Contains("#") Then
                                If Packet.Contains(":") Then
                                    Dim Message As String = Packet.Substring(Packet.IndexOf(":") + 1)
                                    If Message.StartsWith("`brainfuck ") Then
                                        Dim Mode As String = Args(4).ToLower
                                        Select Case Mode
                                            Case "help"
                                                Send("PRIVMSG " & Args(2) & " :brainfuck consists of ><+-[]. See [[Brainfuck]] for info (LinkBot: [[[Brainfuck]]])")
                                            Case "run"
                                                Try
                                                    Dim RawMessage As String = Message
                                                    Dim Program As String = RawMessage.Substring(RawMessage.IndexOf(" :") + 2)
                                                    Program = Program.Substring(Program.IndexOf(" ") + 1)
                                                    Program = Program.Substring(Program.IndexOf(" ") + 1)
                                                    Dim Memory(29999) As Byte
                                                    Dim Pointer As UShort = 0
                                                    Dim NeverEndingLoop As Short = 0
                                                    Dim Output As String = ""
                                                    Dim Commands() As Char = Program.ToCharArray
                                                    For i As Integer = 0 To Commands.Length - 1
                                                        Dim CurrChr As Char = Commands(i)
                                                        Select Case CurrChr
                                                            Case ">"c
                                                                '++++++++++[>+++++++>++++++++++>+++>+<<<<-]>++.>+.+++++++..+++.>++.<<+++++++++++++++.>.+++.------.--------.>+.>.
                                                                If Pointer < (Memory.Length - 1) Then Pointer += 1 ' Else Pointer = 0
                                                            Case "<"c
                                                                If Pointer > 0 Then Pointer -= 1 ' Else Pointer = Memory.Length - 1
                                                            Case "+"c
                                                                If Memory(Pointer) < Byte.MaxValue Then Memory(Pointer) += 1 Else Memory(Pointer) = 0
                                                            Case "-"c
                                                                If Memory(Pointer) > Byte.MinValue Then Memory(Pointer) -= 1 Else Memory(Pointer) = Byte.MaxValue
                                                            Case "."c
                                                                Output &= Chr(Memory(Pointer))
                                                            Case ","c
                                                                ' To be implemented
                                                            Case "["c
                                                                If Memory(Pointer) = 0 Then
                                                                    Dim LoopCount As Integer = 0
                                                                    For j As Integer = i + 1 To Commands.Length - 1
                                                                        If Commands(j) = "]"c Then
                                                                            If LoopCount = 0 Then
                                                                                i = j
                                                                                GoTo LoopOk
                                                                            Else
                                                                                LoopCount -= 1
                                                                            End If
                                                                        ElseIf Commands(j) = "["c Then
                                                                            LoopCount += 1
                                                                        End If
                                                                    Next
                                                                End If
                                                            Case "]"c
                                                                NeverEndingLoop += 1
                                                                If NeverEndingLoop >= 100000 Then
                                                                    Send("PRIVMSG " & Args(2) & " :Never ending loop detected in brainfuck program.")
                                                                    GoTo StopBF
                                                                End If
                                                                Dim LoopCount As Integer = 0
                                                                For j As Integer = i - 1 To 0 Step -1
                                                                    If Commands(j) = "["c Then
                                                                        If LoopCount = 0 Then
                                                                            i = j - 1
                                                                            GoTo LoopOk
                                                                        Else
                                                                            LoopCount -= 1
                                                                        End If
                                                                    ElseIf Commands(j) = "]"c Then
                                                                        LoopCount += 1
                                                                    End If
                                                                Next
                                                                Send("PRIVMSG " & Args(2) & " :Invalid loop statement found in brainfuck program, check your ['s and ]'s.")
                                                                GoTo StopBF
LoopOk:                                                         End Select
                                                    Next
                                                    If Output = "" Then
                                                        Send("PRIVMSG " & Args(2) & " :No output from brainfuck program.")
                                                    Else
                                                        Send("PRIVMSG " & Args(2) & " :Output from brainfuck program: " & Output)
                                                    End If
                                                Catch ex As Exception
                                                    Send("PRIVMSG " & Args(2) & " :An error has occured while running the brainfuck program.")
                                                End Try
                                                Exit Select
StopBF:                                         Send("PRIVMSG " & Args(2) & " :Execution of brainfuck program was halted.")
                                        End Select

                                        '> 	increment the pointer (to point to the next cell to the right).
                                        '< 	decrement the pointer (to point to the next cell to the left).
                                        '+ 	increment (increase by one) the byte at the pointer.
                                        '- 	decrement (decrease by one) the byte at the pointer.
                                        '. 	output the value of the byte at the pointer.
                                        ', 	accept one byte of input, storing its value in the byte at the pointer.
                                        '[ 	jump forward to the command after the corresponding ] if the byte at the pointer is zero.
                                        '] 	jump back to the command after the corresponding [ if the byte at the pointer is nonzero.> 	++ptr;

                                        '< 	--ptr;
                                        '+ 	++(*ptr);
                                        '- 	--(*ptr);
                                        '. 	putchar(*ptr);
                                        ', 	*ptr=getchar();
                                        '[ 	while (*ptr) {
                                        '] 	}

                                        'ElseIf Message.StartsWith("`validate ") And Capab("htmlvalidate") Then
                                        '    Message = Message.Substring(Message.IndexOf(" ") + 1)
                                        '    Dim Content As String = New System.Net.WebClient().DownloadString("http://validator.w3.org/check?uri=" & System.Web.HttpUtility.UrlEncode(Message) & "&charset=%28detect+automatically%29&doctype=Inline&group=0")
                                        '    If Content.Contains("<h3 id=""congrats"">Congratulations</h3>") Then
                                        '        Send("PRIVMSG " & Args(2) & " :Site is syntax-wise valid.")
                                        '    Else
                                        '        Send("PRIVMSG " & Args(2) & " :Site is syntax-wise INVALID.")
                                        '    End If
                                        '    'Send("PRIVMSG " & Args(2) & " :Site is syntax-wise " & IIf(New System.Net.WebClient().DownloadString("http://validator.w3.org/check?uri=" & System.Web.HttpUtility.UrlEncode(Message.Substring(Message.IndexOf(" ") + 1)) & "&charset=%28detect+automatically%29&doctype=Inline&group=0").Contains("<h3 id=""congrats"">Congratulations</h3>"), "valid!", "INVALID."))
                                        'ElseIf Message.StartsWith("`webui ") And (Host = "unaffiliated/danopia/bot/danobot" Or Host = "danopia.unaffiliated.bot" Or Host = Config("admin")) Then
                                        '    Dim Mode As String = Args(4).ToLower
                                        '    Select Case Mode
                                        '        Case "colorlist"
                                        '            Dim Matches As Xml.XmlNodeList = XMLSettings.SelectNodes("config/colors/color")
                                        '            Dim Colors As New List(Of String)
                                        '            For Each Node As Xml.XmlNode In Matches
                                        '                If Node.InnerText.ToLower.EndsWith(";font-style:italic") Then
                                        '                    If Not Colors.Contains(Node.InnerText.Substring(0, Node.InnerText.ToLower.IndexOf(";font-style:italic")).ToLower) Then Colors.Add(Node.InnerText.Substring(0, Node.InnerText.ToLower.IndexOf(";font-style:italic")).ToLower)
                                        '                Else
                                        '                    If Not Colors.Contains(Node.InnerText.ToLower) Then Colors.Add(Node.InnerText.ToLower)
                                        '                End If
                                        '            Next
                                        '            Send("PRIVMSG #l0gg3r :`webuiresponse colorlist " & Join(Colors.ToArray, " "))
                                        '        Case "colorrequest"
                                        '            Dim Nick As String = Args(5)
                                        '            Dim NewColor As String = Args(6).ToLower
                                        '            Dim IsBot As Boolean = Boolean.Parse(Args(7))
                                        '            Dim Owner As String = ""
                                        '            If IsBot Then Owner = Args(8)
                                        '            Dim MD5er As New System.Security.Cryptography.MD5CryptoServiceProvider()
                                        '            Dim Code As String = Convert.ToBase64String(MD5er.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Nick & NewColor & IsBot & Rnd())))
                                        '            Send("PRIVMSG #l0gg3r :A WebUI user requested that " & Nick & "'s nick color in the frmMain.LogIRCs be changed to " & NewColor & IIf(IsBot, ";font-style:italic", "") & ". I have PMed him a code to send back (" & Code & "), or just say `approve " & Code)
                                        '            Send("PRIVMSG " & Nick & " :Say """ & Code & """ without the quotes back to me to make that your color in my frmMain.LogIRCs.")
                                        '            ColorRequests.Add(Code, New ColorRequest(Nick, Nick.ToLower, Color, IsBot, Owner, Owner.ToLower()))
                                        '    End Select
                                        'ElseIf Message = "`meep" And (Host = Config("admin") Or Args(2).ToLower = "#meep" Or Args(2).ToLower = "#flood") And Capab("meepnet") Then
                                        '    Send("PRIVMSG " & Args(2) & " :meep")
                                        'ElseIf Message.StartsWith("`approve ") And Host = Config("admin") Then
                                        '    XMLSettings.Load(XMLPath)
                                        '    'Dim Nick As String = Args(5).ToLower
                                        '    'Dim NewColor As String = Args(6).ToLower
                                        '    'Dim Matches As Xml.XmlNodeList = XMLSettings.SelectNodes("config/colors/color[@nick='" & Nick & "']")
                                        '    'If Matches.Count > 0 Then
                                        '    '    'Matches(0).InnerText=
                                        '    'Else
                                        '    '    'Return DefaultColor
                                        '    'End If
                                        '    'nickcolor
                                        '    XMLSettings.Load(XMLPath)
                                        'ElseIf Message.StartsWith("`weather ") And Capab("weather") Then
                                        '    Send("PRIVMSG " & Args(2) & " :" & Weather.GrabCurrent(Message.Substring(Message.IndexOf(" ") + 1)))
                                        'ElseIf Message = "`reload" And Host = Config("admin") Then
                                        '    XMLSettings.Load(XMLPath)
                                        '    Send("PRIVMSG " & Args(2) & " :Config file reloaded.")
                                        'ElseIf Message.StartsWith("`start ") And (Host.ToLower.EndsWith(".devnode.staff") Or Host.StartsWith("fullcirclemagazine") Or Host = Config("admin")) And Capab("specialevents") Then
                                        '    If CurrentEvents.ContainsKey(Args(2).ToLower) Then
                                        '        Send("PRIVMSG " & Args(2) & " :There is already an event going on.")
                                        '    Else
                                        '        Dim Path As String = String.Format(IO.Path.Combine(My.Application.Info.DirectoryPath, Config("sfrmMain.LogIRCpath")), Args(2).TrimStart("#"), IO.Path.GetFileName(Message.Substring(Message.IndexOf(" ") + 1)).ToLower) & "." & Config("extension")
                                        '        If IO.File.Exists(Path) Then
                                        '            Send("PRIVMSG " & Args(2) & " :Special event frmMain.LogIRC already exists.")
                                        '        Else
                                        '            Dim Dirs() As String = Path.Substring(3, Path.LastIndexOfAny(New Char() {"/"c, "\"c}) - 3).Split("/"c, "\"c)
                                        '            Dim Dir As String = Path.Substring(0, 2)
                                        '            For Each SubDir As String In Dirs
                                        '                Dir &= "/" & SubDir
                                        '                If Not IO.Directory.Exists(Dir) Then IO.Directory.CreateDirectory(Dir)
                                        '            Next
                                        '            Dim EventWriter As New IO.StreamWriter(Path, False)
                                        '            EventWriter.AutoFlush = True
                                        '            EventWriter.WriteLine("<html><head><title>IRC frmMain.LogIRC from " & Args(2) & " (Special Event) - Captured by " & Config("nick") & "</title><style>" & Config("filecss") & "</style></head><body>" & vbNewLine & "<p>frmMain.LogIRC started " & Now.ToLongDateString & " @ " & Now.ToLongTimeString & " on " & Config("server") & " " & Args(2) & " (Special Event) and frmMain.LogIRCged by " & Config("nick") & "</p>" & vbNewLine & _
                                        '            "<p style=""color:#090;"">* Special event frmMain.LogIRC requested by " & Who & "</p>" & vbNewLine & _
                                        '            "<p style=""color:#090;"">* Timestamps are relative to now</p>")
                                        '            EventStart.Add(EventWriter, Now)
                                        '            CurrentEvents.Add(Args(2).ToLower, EventWriter)
                                        '            Send("PRIVMSG " & Args(2) & " :Special event frmMain.LogIRCging started, frmMain.LogIRCging to filename " & IO.Path.GetFileName(Path))
                                        '            Send("NAMES " & Args(2))
                                        '        End If
                                        '    End If
                                        'ElseIf Message.StartsWith("`end") And (Host.ToLower.EndsWith(".devnode.staff") Or Host.StartsWith("fullcirclemagazine")) And Capab("specialevents") Then
                                        '    If CurrentEvents.ContainsKey(Args(2).ToLower) Then
                                        '        Dim EventWriter As IO.StreamWriter = CurrentEvents(Args(2).ToLower)
                                        '        EventWriter.WriteLine("<p style=""color:#090;"">* Special event frmMain.LogIRCging stopped (" & Now.ToLongTimeString & ") by " & Who & "</p></body></html>")
                                        '        EventWriter.Close()
                                        '        EventStart.Remove(EventWriter)
                                        '        CurrentEvents.Remove(Args(2).ToLower)
                                        '        Send("PRIVMSG " & Args(2) & " :Special event frmMain.LogIRCging stopped.")
                                        '    Else
                                        '        Send("PRIVMSG " & Args(2) & " :What event?")
                                        '    End If
                                        'ElseIf Message = "`color" Then
                                        '    Send("PRIVMSG " & Args(2) & " :" & RawWho & ", your nick is HTML color """ & IIf(Color = "", "black", Color) & """ in my frmMain.LogIRCs.")
                                        'ElseIf Message = "`setcolor" And Host = Config("admin") Then
                                        '    Send("PRIVMSG " & Args(2) & " :" & RawWho & ", your nick is HTML color """ & IIf(Color = "", "black", Color) & """ in my frmMain.LogIRCs.")
                                    End If
                                    If Message.StartsWith(Chr(1) & "ACTION ") Then
                                        Message = Message.Substring(8).TrimEnd(Chr(1))
                                        frmMain.LogIRC(Chr(3) & "6* " & Who & " " & Message, Network, Args(2))
                                    ElseIf Message.StartsWith(Chr(1)) Then
                                        Message = Message.Trim(Chr(1))
                                        If Message.Contains(" ") Then
                                            frmMain.LogIRC(Chr(3) & "4[" & Who & ":" & Args(2) & " " & Message.Split(" ")(0) & "] " & Message.Substring(Message.IndexOf(" ") + 1), Network, Args(2))
                                        Else
                                            frmMain.LogIRC(Chr(3) & "4[" & Who & ":" & Args(2) & " " & Message & "]", Network, Args(2))
                                        End If
                                    Else
                                        frmMain.LogIRC("<" & Who & "> " & Message, Network, Args(2))
                                    End If
                                Else
                                    frmMain.LogIRC("<" & Who & "> " & Args(3), Network, Args(2))
                                End If
                            Else
                                If Packet.Contains(":") Then
                                    frmMain.LogIRC("<" & Who & "> " & Packet.Substring(Packet.IndexOf(":") + 1), Network, Args(2))
                                Else
                                    frmMain.LogIRC("<" & Who & "> " & Args(3), Network, Args(2))
                                End If
                            End If
                        Case "nick"
                            Dim Who As String = Args(0).Split("!"c)(0)
                            If MyNick.ToLower = Who.ToLower Then MyNick = Args(2).Substring(1)
                            For Each Channel As KeyValuePair(Of String, List(Of String)) In NickList
                                If Channel.Value.Contains(Who.ToLower) Then
                                    frmMain.LogIRC(Chr(3) & "3* " & Who & " is now known as " & Args(2).Substring(1), Network, Channel.Key)
                                    NickList(Channel.Key).Remove(Who.ToLower)
                                    NickList(Channel.Key).Add(Args(2).Substring(1).ToLower)
                                End If
                            Next
                        Case "error"
                            frmMain.LogIRC(Chr(3) & "4* " & Packet.Substring(Packet.IndexOf(":") + 1), Network)
                            Socket.Close()
                        Case Else
                    End Select
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub TmrAntiFlood_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TmrAntiFlood.Elapsed
        If AntiFlood > 0 Then AntiFlood -= 1 Else AntiFlood = 0
    End Sub
End Class
