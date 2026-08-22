Public Class Inky

    Inherits Ghost

    Private ReadOnly BlinkyRef As Blinky

    Public Sub New(
        gameMap As GameMap,
        pacman As PacMan,
        blinky As Blinky
    )

        MyBase.New(
            gameMap,
            pacman,
            11,
            14
        )

        BlinkyRef = blinky

    End Sub

    Protected Overrides Function GetChaseTarget() As Point

        ' Punto 2 tiles por delante de Pac-Man en su direccion actual
        Dim aheadX As Integer = Pacman.GetMapX()
        Dim aheadY As Integer = Pacman.GetMapY()

        Select Case Pacman.Direction
            Case Direction.Up
                aheadY -= 2
            Case Direction.Down
                aheadY += 2
            Case Direction.Left
                aheadX -= 2
            Case Direction.Right
                aheadX += 2
        End Select

        ' Vector desde Blinky hasta ese punto, duplicado
        Dim blinkyX As Integer = BlinkyRef.GetMapX()
        Dim blinkyY As Integer = BlinkyRef.GetMapY()

        Dim targetX As Integer = blinkyX + 2 * (aheadX - blinkyX)
        Dim targetY As Integer = blinkyY + 2 * (aheadY - blinkyY)

        Return New Point(targetX, targetY)

    End Function

    Protected Overrides Function GetScatterTarget() As Point

        Return New Point(26, 30)

    End Function

End Class
