Public Class PacMan

    ' 2 unidades lógicas = 1 tile (24 px). 1 unidad = 12 px.
    Private Const LogicalUnitsPerTile As Integer = 2
    Private Const TileSize As Integer = 24
    Private Const PacmanSize As Integer = 20
    Public Property X As Integer  ' Posición lógica (centro X)
    Public Property Y As Integer  ' Posición lógica (centro Y)
    Public Property Direction As Direction
    Public Property NextDirection As Direction

    Private ReadOnly Map As GameMap

    Public Sub New(gameMap As GameMap)
        Map = gameMap

        ' Posición inicial en tile (13,24), centrado (+1 en cada coordenada lógica)
        X = 13 * LogicalUnitsPerTile + 1
        Y = 24 * LogicalUnitsPerTile + 1

        Direction = Direction.None
        NextDirection = Direction.None
    End Sub

    Public Sub SetDirection(newDirection As Direction)
        NextDirection = newDirection
    End Sub

    Public Sub Update()
        ' Intentar cambiar dirección en centro de tile:
        If IsCenteredOnTile() Then
            If CanMove(NextDirection) Then
                Direction = NextDirection
            End If
        End If

        ' Avanzar en la dirección actual si es posible:
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

        If directionToCheck = Direction.None Then
            Return False
        End If

        ' ==========================================
        ' POSICIÓN ACTUAL EN PÍXELES
        ' ==========================================

        Dim currentPixelX As Single =
        X * (TileSize / 2.0F)

        Dim currentPixelY As Single =
        Y * (TileSize / 2.0F)

        ' ==========================================
        ' POSICIÓN FUTURA
        ' ==========================================

        Dim newPixelX As Single = currentPixelX
        Dim newPixelY As Single = currentPixelY

        Dim logicalStep As Single =
        TileSize / 2.0F

        Select Case directionToCheck

            Case Direction.Up
                newPixelY -= logicalStep

            Case Direction.Down
                newPixelY += logicalStep

            Case Direction.Left
                newPixelX -= logicalStep

            Case Direction.Right
                newPixelX += logicalStep

        End Select

        ' ==========================================
        ' BORDES DE PAC-MAN
        ' ==========================================

        Dim halfSize As Single =
        PacmanSize / 2.0F


        Dim left As Single =
        newPixelX - halfSize

        Dim right As Single =
        newPixelX + halfSize

        Dim top As Single =
        newPixelY - halfSize

        Dim bottom As Single =
        newPixelY + halfSize

        ' ==========================================
        ' TILES QUE TOCA PAC-MAN
        ' ==========================================

        Dim leftTile As Integer =
        CInt(Math.Floor(left / TileSize))

        Dim rightTile As Integer =
        CInt(Math.Floor((right - 0.01F) / TileSize))

        Dim topTile As Integer =
        CInt(Math.Floor(top / TileSize))

        Dim bottomTile As Integer =
        CInt(Math.Floor((bottom - 0.01F) / TileSize))

        ' ==========================================
        ' LÍMITES
        ' ==========================================

        If leftTile < 0 OrElse
       rightTile >= GameMap.Width OrElse
       topTile < 0 OrElse
       bottomTile >= GameMap.Height Then

            Return False

        End If

        ' ==========================================
        ' COMPROBAR TODOS LOS TILES TOCADOS
        ' ==========================================

        For tileY As Integer = topTile To bottomTile

            For tileX As Integer = leftTile To rightTile

                If Not Map.IsWalkable(tileX, tileY) Then

                    Return False

                End If

            Next

        Next


        Return True

    End Function

    ' Devuelve el índice de columna de Pac-Man en el mapa:
    Public Function GetMapX() As Integer
        Return X \ LogicalUnitsPerTile
    End Function

    ' Devuelve el índice de fila de Pac-Man en el mapa:
    Public Function GetMapY() As Integer
        Return Y \ LogicalUnitsPerTile
    End Function

    ' ¿Está Pac-Man centrado horizontalmente en un tile?
    Public Function IsCenteredX() As Boolean
        Return X Mod LogicalUnitsPerTile = 1
    End Function

    ' ¿Está Pac-Man centrado verticalmente en un tile?
    Public Function IsCenteredY() As Boolean
        Return Y Mod LogicalUnitsPerTile = 1
    End Function

    ' ¿Está Pac-Man centrado en ambos ejes (centro completo de tile)?
    Public Function IsCenteredOnTile() As Boolean
        Return IsCenteredX() AndAlso IsCenteredY()
    End Function

End Class
