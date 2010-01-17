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
        frmMain.LogIRC(Chr(3) & "14<<< " & Packet, Network)

        Dim Args As New Generic.List(Of String)
        If Packet.Contains(" :") Then
            Args.AddRange(Packet.Substring(0, Packet.IndexOf(" :")).Split(" "))
            Args.Add(Packet.Substring(Packet.IndexOf(" :") + 2))
        Else
            Args.AddRange(Packet.Split(" "))
        End If

        Dim Origin As String = Nothing
        Dim Nick As String = Nothing
        Dim Ident As String = Nothing
        Dim Host As String = Nothing

        If Args(0).StartsWith(":") Then
            Origin = Args(0).Substring(1)
            Args.RemoveAt(0)

            Nick = Origin.Split("!"c)(0)
            If Origin.Contains("!") Then
                Ident = Origin.Split("!"c)(1)
                Host = Ident.Split("@"c)(1)
                Ident = Ident.Split("@"c)(0)
            End If
        End If

        Dim Command As String = Args(0).ToLower

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
                    Send("JOIN #programming")
                    TmrAntiFlood.Enabled = True
                    Stage = 2
                    frmMain.SetStatus(Stage)
                Case 353
                    Dim Channel As String = Args(4)
                    If NickList.ContainsKey(Channel.ToLower) Then
                        NickList(Channel.ToLower).AddRange(Args(5).Trim().Replace("~", "").Replace("&", "").Replace("@", "").Replace("%", "").Replace("+", "").ToLower.Split(" "))
                    End If
                Case Else
                    frmMain.LogIRC(Join(Args.ToArray, " "), Network)
            End Select
        Else
            Select Case Command

                Case "ping"
                    If Args.Count >= 2 Then
                        Send("PONG " & Args(1))
                    Else
                        Send("PONG")
                    End If

                Case "notice"
                    If Not Args(1).ToLower = "auth" And Not Args(1).ToLower = MyNick.ToLower Then
                        frmMain.LogIRC(Chr(3) & "5-" & Nick & "- " & Args(3), Network, Args(2))
                    End If

                Case "mode"
                    If (Args(2) = "+e" Or Args(2) = "+r") And Args(1).ToLower = MyNick.ToLower Then
                        Send("JOIN #l0gg3r,#botters")
                    ElseIf Args(1).StartsWith("#") Then
                        frmMain.LogIRC(Chr(3) & "3* " & Nick & " sets mode: " & Args(2), Network, Args(1))
                    End If

                Case "join"
                    frmMain.LogIRC(Chr(3) & "3* Joins: " & Nick & " (" & Host & ")", Network, Args(1))
                    If MyNick.ToLower = Nick.ToLower Then
                        NickList.Add(Args(1).ToLower, New List(Of String))
                        frmMain.JoinedChannel(Args(1))
                    End If
                    If NickList.ContainsKey(Args(1).ToLower) Then NickList(Args(1).ToLower).Add(Nick.ToLower)

                Case "quit"
                    Dim PacketToSend As String = ""
                    If Args.Count >= 3 Then
                        PacketToSend = Chr(3) & "2* Quits: " & Nick & " (" & Host & ") (" & Args(2) & ")"
                    Else
                        PacketToSend = Chr(3) & "2* Quits: " & Nick & " (" & Host & ")"
                    End If
                    For Each Channel As KeyValuePair(Of String, List(Of String)) In NickList
                        If Channel.Value.Contains(Nick.ToLower) Then
                            frmMain.LogIRC(PacketToSend, Network, Channel.Key)
                            NickList(Channel.Key).Remove(Nick.ToLower)
                        End If
                    Next

                Case "part"
                    If NickList.ContainsKey(Args(1).ToLower) Then
                        If NickList(Args(1).ToLower).Contains(Nick.ToLower) Then NickList(Args(1).ToLower).Remove(Nick.ToLower)
                    End If
                    If MyNick.ToLower = Nick.ToLower And NickList.ContainsKey(Args(1).ToLower) Then NickList.Remove(Args(1).ToLower)
                    If Args.Count >= 3 Then
                        frmMain.LogIRC(Chr(3) & "3* Parts: " & Nick & " (" & Host & ") (" & Args(2) & ")", Network, Args(1))
                    Else
                        frmMain.LogIRC(Chr(3) & "3* Parts: " & Nick & " (" & Host & ")", Network, Args(1))
                    End If
                    If MyNick.ToLower = Nick.ToLower Then
                        frmMain.LeftChannel(Args(1))
                    End If

                Case "kick"
                    If MyNick.ToLower = Args(3).ToLower And NickList.ContainsKey(Args(2).ToLower) Then NickList.Remove(Args(2).ToLower)
                    If NickList.ContainsKey(Args(2).ToLower) Then
                        If NickList(Args(2).ToLower).Contains(Args(3).ToLower) Then NickList(Args(2).ToLower).Remove(Args(3).ToLower)
                    End If
                    If Args.Count >= 5 Then
                        frmMain.LogIRC(Chr(3) & "3* " & Args(3) & " was kicked by " & Nick & " (" & Args(4) & ")", Network, Args(2))
                    Else
                        frmMain.LogIRC(Chr(3) & "3* " & Args(3) & " was kicked by " & Nick, Network, Args(2))
                    End If
                    If MyNick.ToLower = Args(3).ToLower Then
                        frmMain.LeftChannel(Args(2))
                    End If

                Case "privmsg"
                    If Args(1).Contains("#") Then
                        Dim Message As String = Args(2)
                        Dim MsgArgs As New Generic.List(Of String)(Message.Split(" "))
                        If Message.StartsWith("`brainfuck ") Then
                            Dim Mode As String = MsgArgs(0).ToLower
                            Select Case Mode
                                Case "help"
                                    Send("PRIVMSG " & Args(1) & " :brainfuck consists of ><+-[]. See [[Brainfuck]] for info (LinkBot: [[[Brainfuck]]])")
                                Case "run"
                                    Try
                                        Dim Program As String = Message
                                        Program = Program.Substring(Program.IndexOf(" ") + 1) ' FAILURE
                                        Program = Program.Substring(Program.IndexOf(" ") + 1) ' FAILURE

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
                                                        Send("PRIVMSG " & Args(1) & " :Never ending loop detected in brainfuck program.")
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
                                                    Send("PRIVMSG " & Args(1) & " :Invalid loop statement found in brainfuck program, check your ['s and ]'s.")
                                                    GoTo StopBF
LoopOk:                                             End Select
                                        Next
                                        If Output = "" Then
                                            Send("PRIVMSG " & Args(1) & " :No output from brainfuck program.")
                                        Else
                                            Send("PRIVMSG " & Args(1) & " :Output from brainfuck program: " & Output)
                                        End If
                                    Catch ex As Exception
                                        Send("PRIVMSG " & Args(1) & " :An error has occured while running the brainfuck program.")
                                    End Try
                                    Exit Select
StopBF:                             Send("PRIVMSG " & Args(1) & " :Execution of brainfuck program was halted.")
                            End Select

                        End If
                        If Message.StartsWith(Chr(1) & "ACTION ") Then
                            Message = Message.Substring(8).TrimEnd(Chr(1))
                            frmMain.LogIRC(Chr(3) & "6* " & Nick & " " & Message, Network, Args(1))
                        ElseIf Message.StartsWith(Chr(1)) Then
                            Message = Message.Trim(Chr(1))
                            If Message.Contains(" ") Then
                                frmMain.LogIRC(Chr(3) & "4[" & Nick & ":" & Args(1) & " " & Message.Split(" ")(0) & "] " & Message.Substring(Message.IndexOf(" ") + 1), Network, Args(1))
                            Else
                                frmMain.LogIRC(Chr(3) & "4[" & Nick & ":" & Args(1) & " " & Message & "]", Network, Args(1))
                            End If
                        Else
                            frmMain.LogIRC("<" & Nick & "> " & Message, Network, Args(1))
                        End If
                    Else
                        frmMain.LogIRC("<" & Nick & "> " & Args(2), Network, Args(1))
                    End If
                Case "nick"
                    If MyNick.ToLower = Nick.ToLower Then MyNick = Args(1)
                    For Each Channel As KeyValuePair(Of String, List(Of String)) In NickList
                        If Channel.Value.Contains(Nick.ToLower) Then
                            frmMain.LogIRC(Chr(3) & "3* " & Nick & " is now known as " & Args(1), Network, Channel.Key)
                            NickList(Channel.Key).Remove(Nick.ToLower)
                            NickList(Channel.Key).Add(Args(1).ToLower)
                        End If
                    Next
                Case "error"
                    frmMain.LogIRC(Chr(3) & "4* " & Args(1), Network)
                    Socket.Close()
                Case Else
            End Select
        End If
    End Sub
    Private Sub TmrAntiFlood_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TmrAntiFlood.Elapsed
        If AntiFlood > 0 Then AntiFlood -= 1 Else AntiFlood = 0
    End Sub
End Class
