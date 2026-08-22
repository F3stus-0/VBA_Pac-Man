Public Class Carlos

    Inherits Ghost

    Public Sub New(
        gameMap As GameMap,
        pacman As PacMan
    )

        MyBase.New(
            gameMap,
            pacman,
            15,
            14
        )

    End Sub

    Protected Overrides Function GetChaseTarget() As Point

        Dim dx As Integer =
            Pacman.GetMapX() - GetMapX()

        Dim dy As Integer =
            Pacman.GetMapY() - GetMapY()

        Dim distance As Double =
            Math.Sqrt(dx * dx + dy * dy)

        If distance >= 8 Then

            Return New Point(
                Pacman.GetMapX(),
                Pacman.GetMapY()
            )

        End If

        Return GetScatterTarget()

    End Function

    Protected Overrides Function GetScatterTarget() As Point

        Return New Point(1, 30)

    End Function

End Class
