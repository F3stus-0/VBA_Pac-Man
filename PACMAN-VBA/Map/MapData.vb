Public Class MapData

    ''' <summary>
    ''' Genera un nuevo mapa de Pac-Man de forma procedural, con el tamaño
    ''' clasico de 28x31 tiles.
    ''' </summary>
    Public Shared Function GenerateRandomMap() As Integer(,)

        Dim generator As New MazeGenerator(7, 15)
        Return generator.Generate()

    End Function

End Class
