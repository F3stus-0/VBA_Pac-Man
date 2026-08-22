Public Class GhostStateMachine

    Public Property CurrentState As GhostState

    Public Sub New(initialState As GhostState)
        CurrentState = initialState
    End Sub

    Public Sub ChangeState(
    newState As GhostState,
    ghost As Ghost
)

        If newState Is Nothing Then
            Return
        End If

        If Not ghost.IsLeavingHouse Then
            ghost.ReverseDirection()
        End If

        CurrentState = newState

    End Sub

    Public Sub Update(ghost As Ghost)

        If CurrentState IsNot Nothing Then
            CurrentState.Update(ghost)
        End If

    End Sub

End Class
