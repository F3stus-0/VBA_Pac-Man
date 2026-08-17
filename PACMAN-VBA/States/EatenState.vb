Public Class EatenState
    Inherits GhostState

    Public Overrides Sub Update(ghost As Ghost)
        ghost.Eaten()

    End Sub

End Class
