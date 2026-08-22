Imports System.Drawing
Imports System.Drawing.Imaging
Public Class GameForm

    'Animations
    Private PacmanUp As Image
    Private PacmanDown As Image
    Private PacmanLeft As Image
    Private PacmanRight As Image

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
    Private Const StartingLives As Integer = 3
    Private Lives As Integer = StartingLives

    Private GameTimer As New Timer()
    Private IsGameOver As Boolean = False
    Private ShowingMenu As Boolean = True
    Private FrightenedTimeRemaining As Single = 0
    Private Const FrightenedDuration As Single = 6.0F
    Private GhostModeTimer As Integer = 0
    Private GhostScatter As Boolean = False
    Private Const ScatterDuration As Integer = 7000
    Private Const ChaseDuration As Integer = 20000
    Private GhostReleaseIndex As Integer = 1
    Private GhostReleaseTimer As Integer = 0

    Private Const GhostReleaseDelay As Integer = 3000

    Public Sub New()
        InitializeComponent()

        PacmanUp = Image.FromFile(
    IO.Path.Combine(Application.StartupPath, "Assets", "pacmanup.gif")
)

        PacmanDown = Image.FromFile(
    IO.Path.Combine(Application.StartupPath, "Assets", "pacmandown.gif")
)

        PacmanLeft = Image.FromFile(
    IO.Path.Combine(Application.StartupPath, "Assets", "pacmanleft.gif")
)

        PacmanRight = Image.FromFile(
    IO.Path.Combine(Application.StartupPath, "Assets", "pacmanright.gif")
)



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
        Inky = New Inky(Map, Pacman, Blinky)
        Carlos = New Carlos(Map, Pacman)


        Ghosts.Add(Blinky)
        Ghosts.Add(Pinky)
        Ghosts.Add(Inky)
        Ghosts.Add(Carlos)
        Blinky.InGhostHouse = False
        Pinky.InGhostHouse = True
        Inky.InGhostHouse = True
        Carlos.InGhostHouse = True


        ' Timer de juego
        GameTimer.Interval = 80
        AddHandler GameTimer.Tick, AddressOf GameTimer_Tick
        GameTimer.Start()

        ' Eventos de teclado y pintura
        AddHandler Me.KeyDown, AddressOf GameForm_KeyDown
        AddHandler Me.Paint, AddressOf DrawMap
    End Sub



    Private Sub GameForm_KeyDown(sender As Object, e As KeyEventArgs)
        If ShowingMenu Then
            If e.KeyCode = Keys.Enter Then
                RestartGame()
                ShowingMenu = False
            End If
            Return
        End If

        If IsGameOver Then
            If e.KeyCode = Keys.Enter Then
                RestartGame()
            End If
            Return
        End If

        Select Case e.KeyCode
            Case Keys.Up : Pacman.SetDirection(Direction.Up)
            Case Keys.Down : Pacman.SetDirection(Direction.Down)
            Case Keys.Left : Pacman.SetDirection(Direction.Left)
            Case Keys.Right : Pacman.SetDirection(Direction.Right)
        End Select
    End Sub

    Private Sub GameTimer_Tick(sender As Object, e As EventArgs)
        If ShowingMenu OrElse IsGameOver Then
            Me.Invalidate()
            Return
        End If

        Pacman.Update()
        For Each ghost As Ghost In Ghosts
            ghost.Update()
        Next
        CheckGhostCollisions()

        GhostReleaseTimer += GameTimer.Interval

        If GhostReleaseTimer >= GhostReleaseDelay Then

            GhostReleaseTimer = 0

            If GhostReleaseIndex < Ghosts.Count Then

                Ghosts(GhostReleaseIndex).IsLeavingHouse = True

                GhostReleaseIndex += 1

            End If

        End If

        Dim mapX As Integer = Pacman.GetMapX()
        Dim mapY As Integer = Pacman.GetMapY()

        If Map.Has_PowerPellet(mapX, mapY) Then
            Map.PowerPelletMap(mapX, mapY) = False
            Score += 50
            FrightenedTimeRemaining = FrightenedDuration

            For Each ghost As Ghost In Ghosts

                If Not TypeOf ghost.StateMachine.CurrentState Is EatenState Then
                    ghost.StateMachine.ChangeState(New FrighttenedState(), ghost)
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
                        ghost.StateMachine.ChangeState(New ChaseState(), ghost)
                    End If

                Next

            End If

        End If

        GhostModeTimer += GameTimer.Interval

        If GhostScatter Then

            If GhostModeTimer >= ScatterDuration Then

                GhostModeTimer = 0
                GhostScatter = False

                For Each ghost As Ghost In Ghosts

                    If Not TypeOf ghost.StateMachine.CurrentState Is FrighttenedState AndAlso
               Not TypeOf ghost.StateMachine.CurrentState Is EatenState Then

                        ghost.StateMachine.ChangeState(
                    New ChaseState(),
                    ghost
                )

                    End If

                Next

            End If

        Else

            If GhostModeTimer >= ChaseDuration Then

                GhostModeTimer = 0
                GhostScatter = True

                For Each ghost As Ghost In Ghosts

                    If Not TypeOf ghost.StateMachine.CurrentState Is FrighttenedState AndAlso
               Not TypeOf ghost.StateMachine.CurrentState Is EatenState Then

                        ghost.StateMachine.ChangeState(
                    New ScattterState(),
                    ghost
                )

                    End If

                Next

            End If

        End If



        Me.Invalidate()
    End Sub

    Private Sub CheckGhostCollisions()

        Dim pacPixelX As Single = Pacman.X * (TileSize / 2.0F)
        Dim pacPixelY As Single = Pacman.Y * (TileSize / 2.0F)

        ' Radio de colisión ~ mitad del tamaño del sprite de cada uno.
        ' pacmanSize/ghostSize = TileSize - 4, así que su suma de radios
        ' es aprox TileSize - 4 (cuando los dos círculos se tocan).
        Const CollisionDistance As Single = TileSize - 4
        Const CollisionDistanceSquared As Single = CollisionDistance * CollisionDistance

        For Each ghost As Ghost In Ghosts

            If ghost.InGhostHouse OrElse ghost.IsLeavingHouse Then
                Continue For
            End If

            Dim ghostPixelX As Single = ghost.X * (TileSize / 2.0F)
            Dim ghostPixelY As Single = ghost.Y * (TileSize / 2.0F)

            Dim dx As Single = pacPixelX - ghostPixelX
            Dim dy As Single = pacPixelY - ghostPixelY

            If (dx * dx + dy * dy) > CollisionDistanceSquared Then
                Continue For
            End If

            If TypeOf ghost.StateMachine.CurrentState Is FrighttenedState Then

                Score += 200
                ghost.StateMachine.ChangeState(New EatenState(), ghost)

            ElseIf TypeOf ghost.StateMachine.CurrentState Is EatenState Then

                Continue For

            Else

                PacmanDied()
                Return

            End If

        Next

    End Sub

    Private Sub PacmanDied()

        GameTimer.Stop()
        Lives -= 1

        If Lives <= 0 Then

            IsGameOver = True
            Me.Invalidate()

        Else

            ResetPositions()
            Me.Invalidate()

            ' Pequeña pausa antes de continuar, como en el Pac-Man clásico
            Dim resumeTimer As New Timer()
            resumeTimer.Interval = 1200
            AddHandler resumeTimer.Tick, Sub(s, e)
                                             resumeTimer.Stop()
                                             resumeTimer.Dispose()
                                             GameTimer.Start()
                                         End Sub
            resumeTimer.Start()

        End If

    End Sub

    Private Sub ResetPositions()

        Pacman = New PacMan(Map)
        Blinky = New Blinky(Map, Pacman)
        Pinky = New Pinky(Map, Pacman)
        Inky = New Inky(Map, Pacman, Blinky)
        Carlos = New Carlos(Map, Pacman)

        Ghosts.Clear()
        Ghosts.Add(Blinky)
        Ghosts.Add(Pinky)
        Ghosts.Add(Inky)
        Ghosts.Add(Carlos)

        Blinky.InGhostHouse = False
        Pinky.InGhostHouse = True
        Inky.InGhostHouse = True
        Carlos.InGhostHouse = True

        FrightenedTimeRemaining = 0
        GhostModeTimer = 0
        GhostScatter = False
        GhostReleaseIndex = 1
        GhostReleaseTimer = 0

    End Sub

    Private Sub RestartGame()

        Map.Reset()
        ResetPositions()

        Score = 0
        Lives = StartingLives
        IsGameOver = False

        GameTimer.Start()
        Me.Invalidate()

    End Sub

    Private Sub DrawMainMenu(g As Graphics)

        Dim width As Integer = Me.ClientSize.Width
        Dim height As Integer = Me.ClientSize.Height

        ' Fondo con degradado tipo arcade
        Using bg As New System.Drawing.Drawing2D.LinearGradientBrush(
        New Rectangle(0, 0, width, height),
        Color.FromArgb(10, 10, 40),
        Color.Black,
        System.Drawing.Drawing2D.LinearGradientMode.Vertical)
            g.FillRectangle(bg, 0, 0, width, height)
        End Using

        ' Titulo
        Using titleFont As New Font("Arial", 42, FontStyle.Bold)
            Dim title = "PAC-MAN"
            Dim size = g.MeasureString(title, titleFont)
            Dim titleX As Single = (width - size.Width) / 2
            Dim titleY As Single = height / 5

            Using shadowBrush As New SolidBrush(Color.FromArgb(120, 0, 0, 0))
                g.DrawString(title, titleFont, shadowBrush, titleX + 4, titleY + 4)
            End Using

            Using titleBrush As New SolidBrush(Color.Yellow)
                g.DrawString(title, titleFont, titleBrush, titleX, titleY)
            End Using
        End Using

        ' Pac-Man animado (boca abriendo y cerrando)
        Dim pacRadius As Integer = 30
        Dim pacCenterX As Integer = width / 2 - 120
        Dim pacCenterY As Integer = height / 2

        Dim mouthOpen As Single = 30 + 20 * CSng(Math.Sin(Environment.TickCount / 150.0))

        Using pacBrush As New SolidBrush(Color.Yellow)
            g.FillPie(
            pacBrush,
            pacCenterX - pacRadius,
            pacCenterY - pacRadius,
            pacRadius * 2,
            pacRadius * 2,
            mouthOpen / 2,
            360 - mouthOpen
        )
        End Using

        ' Fantasmitas decorativos
        Dim ghostColors As Color() = {Color.Red, Color.Pink, Color.Cyan, Color.Orange}

        For i = 0 To ghostColors.Length - 1
            Dim gx As Integer = pacCenterX + 70 + i * 55
            Dim gy As Integer = pacCenterY

            Using ghostBrush As New SolidBrush(ghostColors(i))
                g.FillEllipse(ghostBrush, gx - 20, gy - 20, 40, 40)
            End Using
        Next

        ' Texto parpadeante
        If (Environment.TickCount \ 500) Mod 2 = 0 Then
            Using promptFont As New Font("Arial", 20, FontStyle.Bold)
                Dim prompt = "PRESIONA ENTER PARA JUGAR"
                Dim size = g.MeasureString(prompt, promptFont)
                Using promptBrush As New SolidBrush(Color.White)
                    g.DrawString(prompt, promptFont, promptBrush,
                    CSng((width - size.Width) / 2),
                    CSng(height * 2 / 3))
                End Using
            End Using
        End If

        ' Controles
        Using controlsFont As New Font("Arial", 12)
            Dim controls = "Flechas para moverte  -  Come los puntos y evita a los fantasmas"
            Dim size = g.MeasureString(controls, controlsFont)
            Using controlsBrush As New SolidBrush(Color.Gray)
                g.DrawString(controls, controlsFont, controlsBrush,
                CSng((width - size.Width) / 2),
                CSng(height - 40))
            End Using
        End Using

    End Sub

    Private Sub DrawMap(sender As Object, e As PaintEventArgs)
        Dim g = e.Graphics
        g.Clear(Color.Black)

        If ShowingMenu Then
            DrawMainMenu(g)
            Return
        End If

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

        ' ==========================================
        ' DIBUJAR PAC-MAN
        ' ==========================================

        Dim LogicalSize As Single = TileSize / 2.0F

        Dim pacmanSize As Integer = TileSize

        Dim pacmanCenterX As Single =
    Pacman.X * LogicalSize

        Dim pacmanCenterY As Single =
    Pacman.Y * LogicalSize

        Dim pacmanX As Single =
    pacmanCenterX - pacmanSize / 2.0F

        Dim pacmanY As Single =
    pacmanCenterY - pacmanSize / 2.0F

        Dim pacmanSprite As Image = Nothing

        Select Case Pacman.Direction

            Case Direction.Up
                pacmanSprite = PacmanUp

            Case Direction.Down
                pacmanSprite = PacmanDown

            Case Direction.Left
                pacmanSprite = PacmanLeft

            Case Direction.Right
                pacmanSprite = PacmanRight

            Case Else
                pacmanSprite = PacmanRight

        End Select

        If pacmanSprite IsNot Nothing Then
            If pacmanSprite IsNot Nothing Then

                ImageAnimator.UpdateFrames(pacmanSprite)

                g.DrawImage(
            pacmanSprite,
            pacmanX,
            pacmanY,
            pacmanSize,
            pacmanSize
            )

            End If

        End If

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

        Using font As New Font("Arial", 16, FontStyle.Bold)
            Using brush As New SolidBrush(Color.White)
                g.DrawString("SCORE: " & Score, font, brush,
                     10, GameMap.Height * TileSize + 10)
                g.DrawString("VIDAS: " & Lives, font, brush,
                     220, GameMap.Height * TileSize + 10)
            End Using
        End Using

        If IsGameOver Then

            Using overlay As New SolidBrush(Color.FromArgb(180, 0, 0, 0))
                g.FillRectangle(overlay, 0, 0, Me.ClientSize.Width, Me.ClientSize.Height)
            End Using

            Using bigFont As New Font("Arial", 28, FontStyle.Bold)
                Dim text = "GAME OVER"
                Dim size = g.MeasureString(text, bigFont)
                Using brush As New SolidBrush(Color.Red)
                    g.DrawString(text, bigFont, brush,
                (Me.ClientSize.Width - size.Width) / 2,
                (Me.ClientSize.Height - size.Height) / 2 - 20)
                End Using
            End Using

            Using smallFont As New Font("Arial", 14)
                Dim text2 = "Score: " & Score & "   |   Presiona ENTER para reiniciar"
                Dim size2 = g.MeasureString(text2, smallFont)
                Using brush As New SolidBrush(Color.White)
                    g.DrawString(text2, smallFont, brush,
                (Me.ClientSize.Width - size2.Width) / 2,
                (Me.ClientSize.Height - size2.Height) / 2 + 20)
                End Using
            End Using

        End If



    End Sub

End Class
