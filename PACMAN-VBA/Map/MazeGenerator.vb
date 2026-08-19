Imports System
Imports System.Collections.Generic

''' <summary>
''' Genera un mapa de Pac-Man de forma procedural, garantizando:
'''   - Que no existan callejones sin salida (todo tile de camino tiene
'''     al menos 2 conexiones).
'''   - Que exista una casa de fantasmas fija en el centro del mapa,
'''     con una puerta conectada a un pasillo real.
'''
''' Como funciona:
'''   1. Se reservan las celdas centrales para la casa de fantasmas: el
'''      algoritmo de talla NUNCA entra ahi, asi que nunca deja un pasillo
'''      cortado a medias contra esa zona.
'''   2. Se talla la mitad izquierda del mapa con backtracking recursivo
'''      (esto da un arbol: todavia tiene callejones sin salida).
'''   3. Se recorre la grilla buscando celdas con una sola conexion y se
'''      les abre una pared extra hacia un vecino valido, creando ciclos.
'''      Esto elimina los callejones sin salida.
'''   4. Se convierte todo a tiles, se espeja la mitad derecha, y se
'''      "estampa" el rectangulo exacto de la casa de fantasmas + su
'''      puerta (conectada al pasillo que quedo justo encima).
''' </summary>
Public Class MazeGenerator

    Public Const GhostHouseWidth As Integer = 8
    Public Const GhostHouseHeight As Integer = 5

    Private ReadOnly rnd As New Random()
    Private ReadOnly halfCols As Integer
    Private ReadOnly rows As Integer
    Private cells As MazeCell(,)
    Private reserved As Boolean(,)

    ''' <summary>
    ''' halfCols: celdas de ancho de la MITAD izquierda del mapa.
    ''' rows: celdas de alto del mapa completo.
    ''' Ancho final en tiles = halfCols * 4. Alto final = rows * 2 + 1.
    ''' Para el mapa clasico de 28x31: halfCols = 7, rows = 15.
    ''' </summary>
    Public Sub New(halfCols As Integer, rows As Integer)
        Me.halfCols = halfCols
        Me.rows = rows
    End Sub

    Public Function Generate() As Integer(,)

        cells = New MazeCell(halfCols - 1, rows - 1) {}
        reserved = New Boolean(halfCols - 1, rows - 1) {}

        For x = 0 To halfCols - 1
            For y = 0 To rows - 1
                cells(x, y) = New MazeCell()
            Next
        Next

        MarkGhostHouseReservedCells()

        CarveFrom(0, 0)

        RemoveDeadEnds()

        Return BuildTileMap()

    End Function

    ' ---------- Reserva de la casa de fantasmas ----------

    Private Sub MarkGhostHouseReservedCells()

        ' Bloquea las celdas (en la mitad izquierda) que caen sobre o cerca
        ' del centro del mapa, para que el laberinto nunca las talle.
        Dim x0 = Math.Max(0, halfCols - 3)
        Dim x1 = halfCols - 1
        Dim y0 = rows \ 2 - 2
        Dim y1 = rows \ 2 + 2

        For x = x0 To x1
            For y = y0 To y1
                If InBounds(x, y) Then reserved(x, y) = True
            Next
        Next

    End Sub

    ' ---------- Talla del laberinto (recursive backtracking) ----------

    Private Sub CarveFrom(x As Integer, y As Integer)

        cells(x, y).Visited = True

        Dim directions = New List(Of Integer) From {0, 1, 2, 3} ' 0=Up 1=Right 2=Down 3=Left
        Shuffle(directions)

        For Each direction In directions

            Dim nx = x
            Dim ny = y
            Offset(direction, nx, ny)

            If Not InBounds(nx, ny) Then Continue For
            If reserved(nx, ny) Then Continue For
            If cells(nx, ny).Visited Then Continue For

            RemoveWallBetween(x, y, nx, ny, direction)
            CarveFrom(nx, ny)

        Next

    End Sub

    ' ---------- Eliminacion de callejones sin salida ----------

    Private Sub RemoveDeadEnds()

        Dim changed As Boolean = True
        Dim safety As Integer = 0

        While changed AndAlso safety < 8
            changed = False
            safety += 1

            For x = 0 To halfCols - 1
                For y = 0 To rows - 1

                    If reserved(x, y) Then Continue For
                    If OpenSideCount(x, y) >= 2 Then Continue For

                    Dim candidates As New List(Of Integer)

                    For direction = 0 To 3
                        Dim nx = x
                        Dim ny = y
                        Offset(direction, nx, ny)

                        If Not InBounds(nx, ny) Then Continue For
                        If reserved(nx, ny) Then Continue For
                        If IsWallOpen(x, y, direction) Then Continue For

                        candidates.Add(direction)
                    Next

                    If candidates.Count > 0 Then
                        Dim chosenDirection = candidates(rnd.Next(candidates.Count))
                        Dim nx = x
                        Dim ny = y
                        Offset(chosenDirection, nx, ny)
                        RemoveWallBetween(x, y, nx, ny, chosenDirection)
                        changed = True
                    End If

                Next
            Next

        End While

    End Sub

    Private Function OpenSideCount(x As Integer, y As Integer) As Integer
        Dim c = cells(x, y)
        Dim n = 0
        If Not c.WallUp Then n += 1
        If Not c.WallRight Then n += 1
        If Not c.WallDown Then n += 1
        If Not c.WallLeft Then n += 1
        Return n
    End Function

    Private Function IsWallOpen(x As Integer, y As Integer, direction As Integer) As Boolean
        Dim c = cells(x, y)
        Select Case direction
            Case 0 : Return Not c.WallUp
            Case 1 : Return Not c.WallRight
            Case 2 : Return Not c.WallDown
            Case Else : Return Not c.WallLeft
        End Select
    End Function

    ' ---------- Utilidades sobre celdas ----------

    Private Sub Offset(direction As Integer, ByRef x As Integer, ByRef y As Integer)
        Select Case direction
            Case 0 : y -= 1
            Case 1 : x += 1
            Case 2 : y += 1
            Case 3 : x -= 1
        End Select
    End Sub

    Private Function InBounds(x As Integer, y As Integer) As Boolean
        Return x >= 0 AndAlso x < halfCols AndAlso y >= 0 AndAlso y < rows
    End Function

    Private Sub RemoveWallBetween(x As Integer, y As Integer, nx As Integer, ny As Integer, direction As Integer)
        Select Case direction
            Case 0
                cells(x, y).WallUp = False
                cells(nx, ny).WallDown = False
            Case 1
                cells(x, y).WallRight = False
                cells(nx, ny).WallLeft = False
            Case 2
                cells(x, y).WallDown = False
                cells(nx, ny).WallUp = False
            Case 3
                cells(x, y).WallLeft = False
                cells(nx, ny).WallRight = False
        End Select
    End Sub

    Private Sub Shuffle(list As List(Of Integer))
        For i = list.Count - 1 To 1 Step -1
            Dim j = rnd.Next(i + 1)
            Dim tmp = list(i)
            list(i) = list(j)
            list(j) = tmp
        Next
    End Sub

    ' ---------- Conversion a tiles ----------

    Private Function BuildTileMap() As Integer(,)

        Dim halfTileWidth = halfCols * 2
        Dim fullWidth = halfTileWidth * 2
        Dim tileHeight = rows * 2 + 1

        Dim tiles(fullWidth - 1, tileHeight - 1) As Integer ' 0 = Wall por defecto

        For x = 0 To halfCols - 1
            For y = 0 To rows - 1

                If reserved(x, y) Then Continue For

                Dim c = cells(x, y)
                Dim tx = x * 2
                Dim ty = y * 2 + 1

                tiles(tx, ty) = CInt(TileType.Path)
                tiles(tx + 1, ty) = CInt(TileType.Path)

                If Not c.WallRight AndAlso x < halfCols - 1 AndAlso Not reserved(x + 1, y) Then
                    tiles(tx + 2, ty) = CInt(TileType.Path)
                End If

                If Not c.WallDown AndAlso y < rows - 1 AndAlso Not reserved(x, y + 1) Then
                    tiles(tx, ty + 1) = CInt(TileType.Path)
                    tiles(tx + 1, ty + 1) = CInt(TileType.Path)
                End If

                If Not c.WallUp AndAlso y > 0 AndAlso Not reserved(x, y - 1) Then
                    tiles(tx, ty - 1) = CInt(TileType.Path)
                    tiles(tx + 1, ty - 1) = CInt(TileType.Path)
                End If

            Next
        Next

        ' Espejar la mitad izquierda hacia la derecha (simetria clasica)
        For x = 0 To halfTileWidth - 1
            For y = 0 To tileHeight - 1
                tiles(fullWidth - 1 - x, y) = tiles(x, y)
            Next
        Next

        ' Tunel horizontal a media altura
        Dim tunnelY = tileHeight \ 2
        tiles(0, tunnelY) = CInt(TileType.Path)
        tiles(fullWidth - 1, tunnelY) = CInt(TileType.Path)

        StampGhostHouse(tiles, fullWidth, tileHeight)

        Return tiles

    End Function

    ''' <summary>
    ''' Dibuja el rectangulo exacto de la casa de fantasmas y su puerta,
    ''' ya centrado, sobre el grid de tiles final.
    ''' </summary>
    Private Sub StampGhostHouse(tiles As Integer(,), fullWidth As Integer, tileHeight As Integer)

        Dim x0 = fullWidth \ 2 - GhostHouseWidth \ 2
        Dim x1 = x0 + GhostHouseWidth - 1
        Dim y0 = tileHeight \ 2 - GhostHouseHeight \ 2
        Dim y1 = y0 + GhostHouseHeight - 1

        For x = x0 To x1
            For y = y0 To y1
                tiles(x, y) = CInt(TileType.GhostHouseInterior)
            Next
        Next

        ' Puerta de 2 tiles justo arriba del rectangulo, conectada al
        ' pasillo que la talla siempre deja libre en esa fila.
        Dim doorX0 = fullWidth \ 2 - 1
        Dim doorX1 = fullWidth \ 2

        tiles(doorX0, y0 - 1) = CInt(TileType.GhostHouseDoor)
        tiles(doorX1, y0 - 1) = CInt(TileType.GhostHouseDoor)

    End Sub

End Class
