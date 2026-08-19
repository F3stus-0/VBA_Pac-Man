Public Class GameMap

    Public Const Width As Integer = 28
    Public Const Height As Integer = 31

    Private MapMatrix(Width - 1, Height - 1) As Integer

    Public Sub New()

        Dim generated As Integer(,) = MapData.GenerateRandomMap()

        For Y As Integer = 0 To Height - 1

            For X As Integer = 0 To Width - 1

                MapMatrix(X, Y) = generated(X, Y)

            Next

        Next

    End Sub

    ''' <summary>
    ''' Devuelve el tipo exacto de tile en esa posicion. Fuera de rango
    ''' siempre devuelve Wall, asi que es seguro consultar posiciones
    ''' vecinas sin chequear limites primero.
    ''' </summary>
    Public Function GetTile(X As Integer, Y As Integer) As TileType

        If X < 0 OrElse X >= Width Then Return TileType.Wall
        If Y < 0 OrElse Y >= Height Then Return TileType.Wall

        Return CType(MapMatrix(X, Y), TileType)

    End Function

    ''' <summary>
    ''' Camino transitable normal (para PacMan y para fantasmas fuera de
    ''' su casa). Incluye la puerta de la casa de fantasmas: PacMan no
    ''' deberia cruzarla, eso se controla con IsGhostHouseArea.
    ''' </summary>
    Public Function IsWalkable(X As Integer, Y As Integer) As Boolean

        Dim tile = GetTile(X, Y)
        Return tile <> TileType.Wall

    End Function

    ''' <summary>
    ''' True si la celda es parte de la casa de fantasmas (interior o
    ''' puerta). Util para que PacMan no pueda entrar, y para que los
    ''' fantasmas sepan cuando ya salieron.
    ''' </summary>
    Public Function IsGhostHouseArea(X As Integer, Y As Integer) As Boolean

        Dim tile = GetTile(X, Y)
        Return tile = TileType.GhostHouseInterior OrElse tile = TileType.GhostHouseDoor

    End Function

End Class
