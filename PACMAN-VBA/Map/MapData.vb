Public Class MapData

    Private Const MapWidth As Integer = 28
    Private Const MapHeight As Integer = 31

    ''' <summary>
    ''' Mapa fijo de Pac-Man de 28 columnas x 31 filas.
    ''' # = pared
    ''' . = camino
    ''' G = interior de la casa de fantasmas
    ''' - = puerta de la casa de fantasmas
    ''' </summary>
    Public Shared Function GetClassicMap() As Integer(,)

        Dim rows As String() = {
            "############################",
            "#............##............#",
            "#.####.#####.##.#####.####.#",
            "#.####.#####.##.#####.####.#",
            "#.####.#####.##.#####.####.#",
            "#..........................#",
            "#.####.##.########.##.####.#",
            "#.####.##.########.##.####.#",
            "#......##....##....##......#",
            "######.#####.##.#####.######",
            "######.#####.##.#####.######",
            "######.##..........##.######",
            "######.##.###GG###.##.######",
            "######.##.GGGGGGGG.##.######",
            "..........GGGGGGGG..........",
            "######.##.GGGGGGGG.##.######",
            "######.##.GGGGGGGG.##.######",
            "######.##.###-####.##.######",
            "######.##..........##.######",
            "######.#####.##.#####.######",
            "######.#####.##.#####.######",
            "#............##............#",
            "#.####.#####.##.#####.####.#",
            "#.####.#####.##.#####.####.#",
            "#...##................##...#",
            "###.##.##.########.##.##.###",
            "###.##.##.########.##.##.###",
            "#......##....##....##......#",
            "#.##########.##.##########.#",
            "#..........................#",
            "############################"
        }

        ' Crear matriz de 28 x 31
        Dim result(MapWidth - 1, MapHeight - 1) As Integer

        ' Convertir el mapa de texto a la matriz
        For Y As Integer = 0 To MapHeight - 1

            ' Comprobar que la fila tenga exactamente 28 caracteres
            If rows(Y).Length <> MapWidth Then

                Throw New Exception(
                    "Error en el mapa: la fila " &
                    Y &
                    " tiene " &
                    rows(Y).Length &
                    " caracteres. Debe tener exactamente " &
                    MapWidth &
                    "."
                )

            End If

            For X As Integer = 0 To MapWidth - 1

                Select Case rows(Y)(X)

                    Case "#"c
                        result(X, Y) = CInt(TileType.Wall)

                    Case "."c
                        result(X, Y) = CInt(TileType.Path)

                    Case "G"c
                        result(X, Y) = CInt(TileType.GhostHouseInterior)

                    Case "-"c
                        result(X, Y) = CInt(TileType.GhostHouseDoor)

                    Case Else
                        result(X, Y) = CInt(TileType.Wall)

                End Select

            Next

        Next

        Return result

    End Function

End Class
