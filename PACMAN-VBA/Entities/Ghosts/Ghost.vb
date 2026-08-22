Public Class Ghost

    Private Const LogicalUnitsPerTile As Integer = 2
    Private Const TileSize As Integer = 24

    Public Property X As Integer
    Public Property Y As Integer

    Private Const GhostSpeed As Single = 1
    Public Property Direction As Direction

    Public ReadOnly Property StateMachine As GhostStateMachine

    Public Property InGhostHouse As Boolean
    Public Property IsLeavingHouse As Boolean

    Protected ReadOnly Map As GameMap
    Protected ReadOnly Pacman As PacMan

    Public Sub New(
        gameMap As GameMap,
        pacman As PacMan,
        startX As Integer,
        startY As Integer
    )

        Map = gameMap
        Me.Pacman = pacman

        X = startX * LogicalUnitsPerTile + 1
        Y = startY * LogicalUnitsPerTile + 1

        Direction = Direction.Left

        InGhostHouse = True
        IsLeavingHouse = False

        StateMachine = New GhostStateMachine(New ChaseState())

    End Sub

    Public Sub Update()

        If InGhostHouse AndAlso Not IsLeavingHouse Then
            Return
        End If

        If IsLeavingHouse Then
            LeaveGhostHouse()
            Return
        End If

        StateMachine.Update(Me)

    End Sub

    Private Sub LeaveGhostHouse()

        ' First move up through the ghost-house door.

        If GetMapX() < 14 Then
            Direction = Direction.Right
            MoveOneStep()
        End If

        If GetMapX() > 14 Then
            Direction = Direction.Left
            MoveOneStep()
        End If

        If GetMapY() > 11 Then
            Direction = Direction.Up
            MoveOneStep()
            Return
        End If

        ' Once outside, move sideways.
        IsLeavingHouse = False
        InGhostHouse = False

        Direction = Direction.Left

        StateMachine.ChangeState(
        New ChaseState(),
        Me
    )

    End Sub

    Public Overridable Sub Chase()

        MoveTowards(GetChaseTarget())

    End Sub

    Public Overridable Sub Scatter()

        MoveTowards(GetScatterTarget())

    End Sub

    Public Overridable Sub Frightened()

        MoveRandomly()

    End Sub

    Public Overridable Sub Eaten()

        MoveTowards(New Point(13, 14))

        If GetMapX() = 13 AndAlso
       GetMapY() = 14 Then

            InGhostHouse = True
            IsLeavingHouse = False

            Direction = Direction.Up

        End If

        If InGhostHouse Then
            IsLeavingHouse = True
        End If

    End Sub

    Protected Overridable Function GetChaseTarget() As Point

        Return New Point(
            Pacman.GetMapX(),
            Pacman.GetMapY()
        )

    End Function

    Protected Overridable Function GetScatterTarget() As Point

        Return New Point(26, 0)

    End Function

    Private Sub MoveTowards(target As Point)

        If IsCenteredOnTile() Then

            Dim bestDirection As Direction = ChooseDirection(target)

            If bestDirection <> Direction.None Then
                Direction = bestDirection
            End If

        End If

        MoveOneStep()

    End Sub

    Private Function ChooseDirection(target As Point) As Direction

        Dim currentX As Integer = GetMapX()
        Dim currentY As Integer = GetMapY()

        Dim possibleDirections As New List(Of Direction)

        AddDirectionIfPossible(possibleDirections, Direction.Up)
        AddDirectionIfPossible(possibleDirections, Direction.Left)
        AddDirectionIfPossible(possibleDirections, Direction.Down)
        AddDirectionIfPossible(possibleDirections, Direction.Right)

        If possibleDirections.Count = 0 Then
            Return Direction.None
        End If

        Dim bestDirection As Direction = Direction.None
        Dim bestDistance As Double = Double.MaxValue

        For Each dir As Direction In possibleDirections

            ' Ghosts normally don't immediately reverse direction.
            If dir = OppositeDirection(Direction) Then
                Continue For
            End If

            Dim testX As Integer = currentX
            Dim testY As Integer = currentY

            Select Case dir

                Case Direction.Up
                    testY -= 1

                Case Direction.Down
                    testY += 1

                Case Direction.Left
                    testX -= 1

                Case Direction.Right
                    testX += 1

            End Select

            Dim distance As Double =
                Math.Pow(testX - target.X, 2) +
                Math.Pow(testY - target.Y, 2)

            If distance < bestDistance Then
                bestDistance = distance
                bestDirection = dir
            End If

        Next

        Return bestDirection

    End Function

    Private Sub MoveRandomly()

        If IsCenteredOnTile() Then

            Dim possibleDirections As New List(Of Direction)

            AddDirectionIfPossible(possibleDirections, Direction.Up)
            AddDirectionIfPossible(possibleDirections, Direction.Left)
            AddDirectionIfPossible(possibleDirections, Direction.Down)
            AddDirectionIfPossible(possibleDirections, Direction.Right)

            ' Nunca permitir 180° durante el movimiento normal
            possibleDirections.Remove(OppositeDirection(Direction))

            If possibleDirections.Count > 0 Then

                Dim random As New Random()

                Direction =
                possibleDirections(random.Next(possibleDirections.Count))

            End If

        End If

        MoveOneStep()

    End Sub

    Private Sub AddDirectionIfPossible(
    directions As List(Of Direction),
    directionToCheck As Direction
)

        Dim testX As Integer = GetMapX()
        Dim testY As Integer = GetMapY()

        Select Case directionToCheck

            Case Direction.Up
                testY -= 1

            Case Direction.Down
                testY += 1

            Case Direction.Left
                testX -= 1

            Case Direction.Right
                testX += 1

        End Select

        If Not IsLeavingHouse AndAlso
       Not TypeOf StateMachine.CurrentState Is EatenState Then

            If IsGhostHouseTile(testX, testY) Then
                Return
            End If

        End If

        If Map.IsGhostWalkable(testX, testY) Then
            directions.Add(directionToCheck)
        End If

    End Sub

    Private Function IsGhostHouseTile(
    mapX As Integer,
    mapY As Integer
) As Boolean

        ' Ghost house interior.
        ' Adjust these coordinates if your matrix uses different ones.

        If mapY >= 13 AndAlso mapY <= 15 AndAlso
       mapX >= 11 AndAlso mapX <= 16 Then

            Return True

        End If

        Return False

    End Function

    Private Sub MoveOneStep()

        Select Case Direction

            Case Direction.Up
                Y -= GhostSpeed

            Case Direction.Down
                Y += GhostSpeed

            Case Direction.Left
                X -= GhostSpeed

            Case Direction.Right
                X += GhostSpeed

        End Select

    End Sub

    Private Function OppositeDirection(
        directionToCheck As Direction
    ) As Direction

        Select Case directionToCheck

            Case Direction.Up
                Return Direction.Down

            Case Direction.Down
                Return Direction.Up

            Case Direction.Left
                Return Direction.Right

            Case Direction.Right
                Return Direction.Left

        End Select

        Return Direction.None

    End Function

    Public Function GetMapX() As Integer
        Return X \ LogicalUnitsPerTile
    End Function

    Public Function GetMapY() As Integer
        Return Y \ LogicalUnitsPerTile
    End Function

    Public Function IsCenteredOnTile() As Boolean

        Return X Mod LogicalUnitsPerTile = 1 AndAlso
               Y Mod LogicalUnitsPerTile = 1

    End Function

    Public Sub ReverseDirection()

        Direction = OppositeDirection(Direction)

    End Sub

End Class