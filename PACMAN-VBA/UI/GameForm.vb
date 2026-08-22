Public Class GameForm

    Private ReadOnly Map As New GameMap()
    Private Pacman As PacMan
    Private Blinky As Blinky
    Private Pinky As Pinky
    Private Inky As Inky
    Private Carlos As Carlos

    Private ReadOnly Ghosts As New List(Of Ghost)

    Private Const TileSize As Integer = 24
    Private Const UHeight As Integer = 60
    Public Score As Integer = 0
    Private GameTimer As New Timer()
    Private FrightenedTimeRemaining As Single = 0
    Private Const FrightenedDuration As Single = 6.0F

    Public Sub New()
        InitializeComponent()

        Me.DoubleBuffered = True
        Me.KeyPreview = True

        Me.ClientSize = New Size(
            GameMap.Width * TileSize,
            GameMap.Height * TileSize + UHeight
        )

        Me.Text = "PACMAN-VBA"

        Pacman = New PacMan(Map)
        Blinky = New Blinky(Map, Pacman)
        Pinky = New Pinky(Map, Pacman)
        Inky = New Inky(Map, Pacman)
        Carlos = New Carlos(Map, Pacman)

        Ghosts.Add(Blinky)
        Ghosts.Add(Pinky)
        Ghosts.Add(Inky)
        Ghosts.Add(Carlos)

        ' Timer de juego
        GameTimer.Interval = 80
        AddHandler GameTimer.Tick, AddressOf GameTimer_Tick
        GameTimer.Start()

        ' Eventos de teclado y pintura
        AddHandler Me.KeyDown, AddressOf GameForm_KeyDown
        AddHandler Me.Paint, AddressOf DrawMap
    End Sub

    Private Sub GameForm_KeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.Up : Pacman.SetDirection(Direction.Up)
            Case Keys.Down : Pacman.SetDirection(Direction.Down)
            Case Keys.Left : Pacman.SetDirection(Direction.Left)
            Case Keys.Right : Pacman.SetDirection(Direction.Right)
        End Select
    End Sub

    Private Sub GameTimer_Tick(sender As Object, e As EventArgs)
        Pacman.Update()
        For Each ghost As Ghost In Ghosts
            ghost.Update()
        Next

        Dim mapX As Integer = Pacman.GetMapX()
        Dim mapY As Integer = Pacman.GetMapY()

        If Map.Has_PowerPellet(mapX, mapY) Then
            Map.PowerPelletMap(mapX, mapY) = False
            Score += 50
            FrightenedTimeRemaining = FrightenedDuration

            For Each ghost As Ghost In Ghosts

                If Not TypeOf ghost.StateMachine.CurrentState Is EatenState Then
                    ghost.StateMachine.ChangeState(New FrighttenedState())
                End If

            Next
        ElseIf Map.Has_Pellet(mapX, mapY) Then
            Map.PacDotMap(mapX, mapY) = False
            Score += 10
        End If

        If FrightenedTimeRemaining > 0 Then

            FrightenedTimeRemaining -= GameTimer.Interval / 1000.0F

            If FrightenedTimeRemaining <= 0 Then

                FrightenedTimeRemaining = 0

                For Each ghost As Ghost In Ghosts

                    If TypeOf ghost.StateMachine.CurrentState Is FrighttenedState Then
                        ghost.StateMachine.ChangeState(New ChaseState())
                    End If

                Next

            End If

        End If

        Me.Invalidate()
    End Sub

    Private Sub DrawMap(sender As Object, e As PaintEventArgs)
        Dim g = e.Graphics
        g.Clear(Color.Black)

        ' Dibujo del laberinto (paredes, caminos, pellets)
        For y = 0 To GameMap.Height - 1
            For x = 0 To GameMap.Width - 1
                Dim tile = Map.GetTile(x, y)
                Dim rect As New Rectangle(
                    x * TileSize,
                    y * TileSize,
                    TileSize,
                    TileSize
                )

                Select Case tile
                    Case TileType.Wall
                        Using b As New SolidBrush(Color.FromArgb(33, 33, 222))
                            g.FillRectangle(b, rect)
                        End Using

                    Case TileType.Path
                        If Map.Has_PowerPellet(x, y) Then
                            If (Environment.TickCount \ 200) Mod 2 = 0 Then
                                Dim dotSize = TileSize \ 2
                                Dim dotX = x * TileSize + (TileSize - dotSize) \ 2
                                Dim dotY = y * TileSize + (TileSize - dotSize) \ 2
                                Using b As New SolidBrush(Color.FromArgb(255, 204, 0))
                                    g.FillEllipse(b, dotX, dotY, dotSize, dotSize)
                                End Using
                            End If
                        ElseIf Map.Has_Pellet(x, y) Then
                            Dim dotSize = TileSize \ 5
                            Dim dotX = x * TileSize + (TileSize - dotSize) \ 2
                            Dim dotY = y * TileSize + (TileSize - dotSize) \ 2
                            Using b As New SolidBrush(Color.FromArgb(255, 204, 0))
                                g.FillEllipse(b, dotX, dotY, dotSize, dotSize)
                            End Using
                        End If

                    Case TileType.GhostHouseInterior
                        Using b As New SolidBrush(Color.FromArgb(40, 40, 40))
                            g.FillRectangle(b, rect)
                        End Using

                    Case TileType.GhostHouseDoor
                        Using pen As New Pen(Color.FromArgb(255, 184, 222), 2)
                            g.DrawLine(pen,
                                rect.Left, rect.Top + TileSize \ 2,
                                rect.Right, rect.Top + TileSize \ 2)
                        End Using
                End Select
            Next
        Next

        ' Dibujo de Pac-Man en el centro de su tile actual
        Dim LogicalSize As Single = TileSize / 2.0F

        Dim pacmanSize As Integer = TileSize - 4

        Dim pacmanCenterX As Single =
        Pacman.X * LogicalSize

        Dim pacmanCenterY As Single =
        Pacman.Y * LogicalSize

        Dim pacmanX As Single =
        pacmanCenterX - pacmanSize / 2.0F

        Dim pacmanY As Single =
        pacmanCenterY - pacmanSize / 2.0F

        Using brush As New SolidBrush(Color.Yellow)

            g.FillEllipse(
        brush,
        pacmanX,
        pacmanY,
        pacmanSize,
        pacmanSize
        )

        End Using

        'Fantasmas
        For Each ghost As Ghost In Ghosts

            Dim ghostSize As Integer = TileSize - 4

            Dim ghostCenterX As Single =
        ghost.X * (TileSize / 2.0F)

            Dim ghostCenterY As Single =
        ghost.Y * (TileSize / 2.0F)

            Dim ghostX As Single =
        ghostCenterX - ghostSize / 2.0F

            Dim ghostY As Single =
        ghostCenterY - ghostSize / 2.0F

            Dim ghostColor As Color = Color.Red

            If TypeOf ghost Is Pinky Then
                ghostColor = Color.Pink
            ElseIf TypeOf ghost Is Inky Then
                ghostColor = Color.Cyan
            ElseIf TypeOf ghost Is Carlos Then
                ghostColor = Color.Orange
            End If

            If TypeOf ghost.StateMachine.CurrentState Is FrighttenedState Then
                ghostColor = Color.Blue
            End If

            If TypeOf ghost.StateMachine.CurrentState Is EatenState Then
                ghostColor = Color.White
            End If

            Using brush As New SolidBrush(ghostColor)

                g.FillEllipse(
            brush,
            ghostX,
            ghostY,
            ghostSize,
            ghostSize
        )

            End Using

        Next

        ' (Opcional) Dibujo de UI: puntuación, etc.
        Using font As New Font("Arial", 16, FontStyle.Bold)
            Using brush As New SolidBrush(Color.White)
                g.DrawString("SCORE: " & Score, font, brush,
                             10, GameMap.Height * TileSize + 10)
            End Using
        End Using
    End Sub

End Class
