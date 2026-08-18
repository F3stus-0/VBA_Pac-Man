Public Class GameMap
    'Dimensions
    Public Const columns As Integer = 28
    Public Const rows As Integer = 36

    'Matrix for the Map
    'At least for now 0 will represent a wall and 1 the floor
    Public MazeMatrix(columns - 1, rows - 1) As Integer

    Public Sub New()
        'LoadMap()
    End Sub

End Class
