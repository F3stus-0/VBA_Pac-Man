''' <summary>
''' Tipos de tile que puede tener el mapa. Los objetos Ghost y PacMan pueden
''' usar GameMap.GetTile(x, y) para saber en que tipo de celda estan parados
''' o hacia donde se estan moviendo.
''' </summary>
Public Enum TileType
    Wall = 0
    Path = 1
    GhostHouseInterior = 2
    GhostHouseDoor = 3
End Enum
