Public Class GameForm

    Private ReadOnly Map As New GameMap()
    Private Pacman As Pacman

    Private Const TileSize As Integer = 20

    Private GameTimer As New Timer()

    Public Sub New()

        InitializeComponent()

        Me.DoubleBuffered = True
        Me.KeyPreview = True

        Me.ClientSize = New Size(
            GameMap.Width * TileSize,
            GameMap.Height * TileSize
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

        Me.Invalidate()

    End Sub

    Private Sub DrawMap(
        sender As Object,
        e As PaintEventArgs
    )

        Dim g = e.Graphics

        g.Clear(Color.Black)

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

        Dim pacmanSize As Integer = TileSize - 4

        Dim pacmanX As Integer =
            Pacman.X * TileSize + 2

        Dim pacmanY As Integer =
            Pacman.Y * TileSize + 2

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