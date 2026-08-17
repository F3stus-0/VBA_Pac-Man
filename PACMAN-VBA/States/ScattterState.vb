Public Class ScattterState
    Inherits GhostState

    Public Overrides Sub Update(ghost As Ghost)
        ghost.Scatter()

    End Sub

End Class
