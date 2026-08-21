Public Class PacMan

    ' ==========================================
    ' CONFIGURACIÓN
    ' ==========================================

    ' Cada tile del mapa tiene 2 posiciones lógicas.

    ' Tile de mapa:
    ' 24 x 24 px

    ' Posición lógica:
    ' 12 x 12 px

    ' Por lo tanto:
    ' 2 posiciones lógicas = 1 tile

    Private Const LogicalUnitsPerTile As Integer = 2


    ' ==========================================
    ' POSICIÓN
    ' ==========================================

    ' X e Y NO son coordenadas del mapa.

    ' Son coordenadas lógicas.

    ' Ejemplo:

    ' X = 26
    ' significa:
    ' 26 * 12 = 312 píxeles

    ' Eso corresponde al tile:
    ' 26 \ 2 = 13

    Public Property X As Integer
    Public Property Y As Integer


    ' ==========================================
    ' DIRECCIONES
    ' ==========================================

    Public Property Direction As Direction
    Public Property NextDirection As Direction


    ' ==========================================
    ' MAPA
    ' ==========================================

    Private ReadOnly Map As GameMap


    ' ==========================================
    ' CONSTRUCTOR
    ' ==========================================

    Public Sub New(gameMap As GameMap)

        Map = gameMap

        ' ======================================
        ' POSICIÓN INICIAL
        ' ======================================

        ' Tile original:

        ' X = 13
        ' Y = 24

        ' Convertimos a unidades lógicas:

        ' 13 * 2 = 26
        ' 24 * 2 = 48

        X = 13 * LogicalUnitsPerTile + 1
        Y = 24 * LogicalUnitsPerTile + 1


        ' ======================================
        ' DIRECCIÓN INICIAL
        ' ======================================

        Direction = Direction.None
        NextDirection = Direction.None

    End Sub


    ' ==========================================
    ' CAMBIAR DIRECCIÓN
    ' ==========================================

    Public Sub SetDirection(newDirection As Direction)

        NextDirection = newDirection

    End Sub


    ' ==========================================
    ' ACTUALIZAR PAC-MAN
    ' ==========================================

    Public Sub Update()

        ' ==========================================
        ' INTENTAR CAMBIAR DE DIRECCIÓN
        ' ==========================================

        If IsCenteredOnTile() Then

            If CanMove(NextDirection) Then

                Direction = NextDirection

            End If

        End If


        ' ==========================================
        ' AVANZAR
        ' ==========================================

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


    ' ==========================================
    ' COMPROBAR SI PAC-MAN PUEDE MOVERSE
    ' ==========================================

    Private Function CanMove(
        directionToCheck As Direction
    ) As Boolean


        ' ======================================
        ' SIN DIRECCIÓN = NO MOVER
        ' ======================================

        If directionToCheck = Direction.None Then

            Return False

        End If


        ' ======================================
        ' POSICIÓN LÓGICA QUE TENDRÍA
        ' ======================================

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

        End Select


        ' ======================================
        ' CONVERTIR POSICIÓN LÓGICA A TILE
        ' ======================================

        ' Ejemplo:

        ' 26 \ 2 = 13
        ' 27 \ 2 = 13
        ' 28 \ 2 = 14

        ' De esta forma sabemos en qué tile
        ' está la nueva posición.

        Dim mapX As Integer =
            newX \ LogicalUnitsPerTile

        Dim mapY As Integer =
            newY \ LogicalUnitsPerTile


        ' ======================================
        ' COMPROBAR LÍMITES
        ' ======================================

        If mapX < 0 OrElse
           mapX >= GameMap.Width OrElse
           mapY < 0 OrElse
           mapY >= GameMap.Height Then

            Return False

        End If


        ' ======================================
        ' COMPROBAR PARED
        ' ======================================

        If Not Map.IsWalkable(mapX, mapY) Then

            Return False

        End If


        ' ======================================
        ' SI LLEGAMOS AQUÍ:
        ' SE PUEDE MOVER
        ' ======================================

        Return True

    End Function


    ' ==========================================
    ' OBTENER TILE ACTUAL
    ' ==========================================

    Public Function GetMapX() As Integer

        Return X \ LogicalUnitsPerTile

    End Function


    Public Function GetMapY() As Integer

        Return Y \ LogicalUnitsPerTile

    End Function


    ' ==========================================
    ' SABER SI ESTÁ EN EL CENTRO DE UN TILE
    ' ==========================================

    ' Esto será MUY útil para las intersecciones.

    ' Una posición lógica par representa el borde
    ' de un tile.

    ' Una posición lógica impar representa el centro
    ' del tile.

    ' Ejemplo:

    ' Tile 13:

    ' X = 26 -> inicio
    ' X = 27 -> centro
    ' X = 28 -> inicio del siguiente tile

    Public Function IsCenteredX() As Boolean

        Return X Mod LogicalUnitsPerTile = 1

    End Function


    Public Function IsCenteredY() As Boolean

        Return Y Mod LogicalUnitsPerTile = 1

    End Function


    Public Function IsCenteredOnTile() As Boolean

        Return IsCenteredX() AndAlso IsCenteredY()

    End Function

End Class