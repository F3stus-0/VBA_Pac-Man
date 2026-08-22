Public Class Pinky

    Inherits Ghost

    Public Sub New(
        gameMap As GameMap,
        pacman As PacMan
    )

        MyBase.New(
            gameMap,
            pacman,
            13,
            14
        )

    End Sub

    Protected Overrides Function GetChaseTarget() As Point

        Dim targetX As Integer = Pacman.GetMapX()
        Dim targetY As Integer = Pacman.GetMapY()

        Select Case Pacman.Direction

            Case Direction.Up
                targetY -= 4

            Case Direction.Down
                targetY += 4

            Case Direction.Left
                targetX -= 4

            Case Direction.Right
                targetX += 4

        End Select

        Return New Point(targetX, targetY)

    End Function

    Protected Overrides Function GetScatterTarget() As Point

        Return New Point(1, 0)

    End Function

End Class
