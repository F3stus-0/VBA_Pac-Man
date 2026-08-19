Public Class GameForm

    Private ReadOnly Map As New GameMap()
    Private Const TileSize As Integer = 20

    Public Sub New()

        InitializeComponent()

        Me.DoubleBuffered = True
        Me.ClientSize = New Size(GameMap.Width * TileSize, GameMap.Height * TileSize)
        Me.Text = "PACMAN-VBA"

        AddHandler Me.Paint, AddressOf DrawMap

    End Sub

    Private Sub DrawMap(sender As Object, e As PaintEventArgs)

        Dim g = e.Graphics
        g.Clear(Color.Black)

        For y = 0 To GameMap.Height - 1

            For x = 0 To GameMap.Width - 1

                Dim tile = Map.GetTile(x, y)
                Dim rect = New Rectangle(x * TileSize, y * TileSize, TileSize, TileSize)

                Select Case tile

                    Case TileType.Wall
                        Using brush As New SolidBrush(Color.FromArgb(33, 33, 222))
                            g.FillRectangle(brush, rect)
                        End Using

                    Case TileType.Path
                        Dim dotSize = TileSize \ 5
                        Dim dotX = x * TileSize + (TileSize - dotSize) \ 2
                        Dim dotY = y * TileSize + (TileSize - dotSize) \ 2
                        Using brush As New SolidBrush(Color.FromArgb(255, 204, 0))
                            g.FillEllipse(brush, dotX, dotY, dotSize, dotSize)
                        End Using

                    Case TileType.GhostHouseInterior
                        Using brush As New SolidBrush(Color.FromArgb(40, 40, 40))
                            g.FillRectangle(brush, rect)
                        End Using

                    Case TileType.GhostHouseDoor
                        Using pen As New Pen(Color.FromArgb(255, 184, 222), 2)
                            g.DrawLine(pen, rect.Left, rect.Top + TileSize \ 2, rect.Right, rect.Top + TileSize \ 2)
                        End Using

                End Select

            Next

        Next

    End Sub

End Class
