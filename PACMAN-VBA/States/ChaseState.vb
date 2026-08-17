Public Class ChaseState

    Inherits GhostState

    Public Overrides Sub Update(ghost As Ghost)
        ghost.Chase()

    End Sub

End Class
