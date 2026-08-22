Public Class GhostStateMachine

    Public Property CurrentState As GhostState

    Public Sub New(initialState As GhostState)
        CurrentState = initialState
    End Sub

    Public Sub ChangeState(newState As GhostState, ghost As Ghost)

        If newState Is Nothing Then
            Return
        End If

        ' Cambiar de estado permite un giro de 180°
        ghost.ReverseDirection()

        CurrentState = newState

    End Sub

    Public Sub Update(ghost As Ghost)

        If CurrentState IsNot Nothing Then
            CurrentState.Update(ghost)
        End If

    End Sub

End Class
