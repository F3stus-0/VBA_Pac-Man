Public Class FrighttenedState
    Inherits GhostState

    Public Overrides Sub Update(ghost As Ghost)
        ghost.Frightened()

    End Sub

End Class
