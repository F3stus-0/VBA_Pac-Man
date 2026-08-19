Public Class PacMan

    Public Property X As Integer
    Public Property Y As Integer

    Public Property Direction As Direction
    Public Property NextDirection As Direction

    Private ReadOnly Map As GameMap

    Public Sub New(gameMap As GameMap)

        Map = gameMap

        ' Posición inicial
        X = 13
        Y = 24

        ' Dirección inicial
        Direction = Direction.None
        NextDirection = Direction.None

    End Sub

    Public Sub SetDirection(newDirection As Direction)

        NextDirection = newDirection

    End Sub

    Public Sub Update()

        ' Intentar cambiar de dirección
        If CanMove(NextDirection) Then

            Direction = NextDirection

        End If

        ' Intentar avanzar
        If CanMove(Direction) Then

            Select Case Direction

                Case Direction.Up

                    Y -= 1

                Case Direction.Down

                    Y += 1

                Case Direction.Left

                    X -= 1

                Case Direction.Right

                    X += 1

            End Select

        End If

    End Sub

    Private Function CanMove(
        directionToCheck As Direction
    ) As Boolean

        Dim newX As Integer = X
        Dim newY As Integer = Y

        Select Case directionToCheck

            Case Direction.Up

                newY -= 1

            Case Direction.Down

                newY += 1

            Case Direction.Left

                newX -= 1

            Case Direction.Right

                newX += 1

            Case Direction.None

                Return False

        End Select

        Return Map.IsWalkable(newX, newY)

    End Function

End Class