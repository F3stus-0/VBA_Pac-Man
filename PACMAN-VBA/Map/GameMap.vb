Public Class GameMap

    Public Const Width As Integer = 28
    Public Const Height As Integer = 31

    Private MapMatrix(Width - 1, Height - 1) As Integer

    Public Sub New()

        Dim classicMap As Integer(,) = MapData.GetClassicMap()

        For Y As Integer = 0 To Height - 1

            For X As Integer = 0 To Width - 1

                MapMatrix(X, Y) = classicMap(X, Y)

            Next

        Next

    End Sub

    ''' <summary>
    ''' Devuelve el tipo exacto de tile en una posicion.
    ''' Fuera del mapa siempre devuelve Wall.
    ''' </summary>
    Public Function GetTile(X As Integer, Y As Integer) As TileType

        If X < 0 OrElse X >= Width Then
            Return TileType.Wall
        End If

        If Y < 0 OrElse Y >= Height Then
            Return TileType.Wall
        End If

        Return CType(MapMatrix(X, Y), TileType)

    End Function

    ''' <summary>
    ''' Indica si una posicion puede ser atravesada.
    ''' </summary>
    Public Function IsWalkable(X As Integer, Y As Integer) As Boolean

        Dim tile = GetTile(X, Y)

        Return tile <> TileType.Wall

    End Function

    ''' <summary>
    ''' Indica si una posicion pertenece a la casa de fantasmas.
    ''' </summary>
    Public Function IsGhostHouseArea(X As Integer, Y As Integer) As Boolean

        Dim tile = GetTile(X, Y)

        Return tile = TileType.GhostHouseInterior OrElse
               tile = TileType.GhostHouseDoor

    End Function

End Class
