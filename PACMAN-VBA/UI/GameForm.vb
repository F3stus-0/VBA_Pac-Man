Public Class GameForm

    Private ReadOnly Map As New GameMap()
    Private Pacman As PacMan

    Private Const TileSize As Integer = 24
    Private Const UHeight As Integer = 60
    Public Score As Integer = 0

    Private GameTimer As New Timer()

    Public Sub New()

        InitializeComponent()

        Me.DoubleBuffered = True
        Me.KeyPreview = True

        Me.ClientSize = New Size(
            GameMap.Width * TileSize,
            GameMap.Height * TileSize + UHeight
        )

        Me.Text = "PACMAN-VBA"

        ' Crear Pac-Man
        Pacman = New Pacman(Map)

        ' Configurar Timer
        GameTimer.Interval = 150

        AddHandler GameTimer.Tick, AddressOf GameTimer_Tick

        GameTimer.Start()

        ' Eventos
        AddHandler Me.KeyDown, AddressOf GameForm_KeyDown
        AddHandler Me.Paint, AddressOf DrawMap

    End Sub

    Private Sub GameForm_KeyDown(
        sender As Object,
        e As KeyEventArgs
    )

        Select Case e.KeyCode

            Case Keys.Up
                Pacman.SetDirection(Direction.Up)

            Case Keys.Down
                Pacman.SetDirection(Direction.Down)

            Case Keys.Left
                Pacman.SetDirection(Direction.Left)

            Case Keys.Right
                Pacman.SetDirection(Direction.Right)

        End Select

    End Sub

    Private Sub GameTimer_Tick(
    sender As Object,
    e As EventArgs
)
        Pacman.Update()

        ' ==========================================
        ' OBTENER TILE ACTUAL DE PAC-MAN
        ' ==========================================
        Dim mapX As Integer = Pacman.GetMapX()
        Dim mapY As Integer = Pacman.GetMapY()


        ' ==========================================
        ' COMER PELLET
        ' ==========================================
        If Map.Has_Pellet(mapX, mapY) Then

            Map.PacDotMap(mapX, mapY) = False

            Score += 100

        End If

        ' ==========================================
        ' REDIBUJAR
        ' ==========================================
        Me.Invalidate()

    End Sub

    Private Sub DrawMap(
        sender As Object,
        e As PaintEventArgs
    )

        Dim g = e.Graphics

        g.Clear(Color.Black)
        '==================================
        'Texto
        '==================================

        Using Font As New Font("Arial", 16, FontStyle.Bold)
            Using Brush As New SolidBrush(Color.White)
                g.DrawString(
                "SCORE: " & Score, Font, Brush, 10, GameMap.Height * TileSize + 10
                )
            End Using
        End Using

        ' ==========================================
        ' MAPA
        ' ==========================================

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

                        Using brush As New SolidBrush(
                            Color.FromArgb(33, 33, 222)
                        )

                            g.FillRectangle(
                                brush,
                                rect
                            )

                        End Using

                    Case TileType.Path

                        If Map.Has_Pellet(x, y) = True Then
                            Dim dotSize As Integer = TileSize \ 5

                            Dim dotX As Integer =
                                x * TileSize +
                                (TileSize - dotSize) \ 2

                            Dim dotY As Integer =
                                y * TileSize +
                                (TileSize - dotSize) \ 2

                            Using brush As New SolidBrush(
                                Color.FromArgb(255, 204, 0)
                            )

                                g.FillEllipse(
                                    brush,
                                    dotX,
                                    dotY,
                                    dotSize,
                                    dotSize
                                )

                            End Using
                        End If


                    Case TileType.GhostHouseInterior

                        Using brush As New SolidBrush(
                            Color.FromArgb(40, 40, 40)
                        )

                            g.FillRectangle(
                                brush,
                                rect
                            )

                        End Using

                    Case TileType.GhostHouseDoor

                        Using pen As New Pen(
                            Color.FromArgb(255, 184, 222),
                            2
                        )

                            g.DrawLine(
                                pen,
                                rect.Left,
                                rect.Top + TileSize \ 2,
                                rect.Right,
                                rect.Top + TileSize \ 2
                            )

                        End Using

                End Select

            Next

        Next

        ' ==========================================
        ' PAC-MAN
        ' ==========================================

        Dim LogicalSize As Integer = TileSize \ 2

        Dim pacmanSize As Integer = TileSize - 4

        Dim pacmanX As Integer =
        Pacman.X * LogicalSize -
        pacmanSize \ 2

        Dim pacmanY As Integer =
        Pacman.Y * LogicalSize -
        pacmanSize \ 2


        Using brush As New SolidBrush(Color.Yellow)

            g.FillEllipse(
        brush,
        pacmanX,
        pacmanY,
        pacmanSize,
        pacmanSize
        )

        End Using

    End Sub

End Class