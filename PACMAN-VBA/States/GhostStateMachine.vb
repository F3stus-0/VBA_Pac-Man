Public Class GhostStateMachine

    Public CurrentState As GhostState

    Public Sub ChangeState(newState As GhostState)

        CurrentState = newState

    End Sub

    Public Sub Update(ghost As Ghost)

        If CurrentState IsNot Nothing Then
            CurrentState.Update(ghost)
        End If

    End Sub

End Class
