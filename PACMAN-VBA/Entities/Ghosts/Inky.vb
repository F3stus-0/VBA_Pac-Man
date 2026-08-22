Public Class Inky

    Inherits Ghost

    Public Sub New(
        gameMap As GameMap,
        pacman As PacMan
    )

        MyBase.New(
            gameMap,
            pacman,
            11,
            14
        )

    End Sub

    Protected Overrides Function GetScatterTarget() As Point

        Return New Point(26, 30)

    End Function

End Class
