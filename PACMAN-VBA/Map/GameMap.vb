Public Class GameMap

    Public Const Width As Integer = 28
    Public Const Height As Integer = 31

    Private MapMatrix(Width - 1, Height - 1) As Integer
    Public PacDotMap(Width - 1, Height - 1) As Boolean
    Public PowerPelletMap(Width - 1, Height - 1) As Boolean

    ' Classic 4-corner power pellet spots — adjust if your maze layout differs
    Private ReadOnly PowerPelletPositions As Point() = {
    New Point(1, 3),
    New Point(26, 3),
    New Point(1, 28),
    New Point(26, 28)
}

    Public Sub New()

        Dim classicMap As Integer(,) = MapData.GetClassicMap()


        For Y As Integer = 0 To Height - 1
            For X As Integer = 0 To Width - 1
                MapMatrix(X, Y) = classicMap(X, Y)
            Next

        Next
        For Y As Integer = 0 To Height - 1

            For X As Integer = 0 To Width - 1
                If classicMap(X, Y) = CInt(TileType.Path) Then
                    PacDotMap(X, Y) = True
                Else
                    PacDotMap(X, Y) = False
                End If
            Next
        Next

        For Each p In PowerPelletPositions
            If GetTile(p.X, p.Y) = TileType.Path Then
                PowerPelletMap(p.X, p.Y) = True
                PacDotMap(p.X, p.Y) = False ' don't double-count as a normal pellet
            End If
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

    Public Function Has_Pellet(X As Integer, Y As Integer) As Boolean
        Return PacDotMap(X, Y)
    End Function

    Public Function Has_PowerPellet(X As Integer, Y As Integer) As Boolean
        Return PowerPelletMap(X, Y)
    End Function

    ''' <summary>
    ''' Indica si una posicion puede ser atravesada.
    ''' </summary>
    Public Function IsWalkable(X As Integer, Y As Integer) As Boolean

        Dim tile = GetTile(X, Y)

        Return tile <> TileType.Wall And tile <> TileType.GhostHouseDoor

    End Function
    Public Function IsGhostWalkable(X As Integer, Y As Integer) As Boolean

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
