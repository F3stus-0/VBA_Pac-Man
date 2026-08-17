Public Class Ghost

    Public StateMachine As GhostStateMachine

    Public Sub New()

        StateMachine = New GhostStateMachine()

    End Sub

    Public Sub Update()

        StateMachine.Update(Me)

    End Sub

    Public Overridable Sub Chase()

    End Sub

    Public Overridable Sub Scatter()

    End Sub

    Public Overridable Sub Frightened()

    End Sub

    Public Overridable Sub Eaten()

    End Sub

End Class
