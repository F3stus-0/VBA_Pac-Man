Public Class GameMap

    Public Const Width As Integer = 28
    Public Const Height As Integer = 31

    Private MapMatrix(Width - 1, Height - 1) As Integer

    Public Sub New()

        For Y As Integer = 0 To Height - 1

            For X As Integer = 0 To Width - 1

                MapMatrix(X, Y) = Mapdata.ClassicPacMan(Y, X)

            Next

        Next

    End Sub

    Public Function IsWalkable(X As Integer, Y As Integer) As Boolean

        If X < 0 OrElse X >= Width Then
            Return False
        End If

        If Y < 0 OrElse Y >= Height Then
            Return False
        End If

        Return MapMatrix(X, Y) = 1

    End Function

End Class
