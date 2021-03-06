﻿Imports System.Data
Imports System.IO
Imports System.Data.OleDb
Imports System.Drawing.Printing

'Imports Excel = Microsoft.Office.Interop.Excel
'Imports System.Runtime.InteropServices
Module funciones_Globales
    Public idFactura As Integer
    Private Sub ImprimirTiketVenta(ByVal sender As System.Object, ByVal e As PrintPageEventArgs)
        ' letra
        'Dim font1 As New Font("EAN-13", 40)
        Dim printfont As New Font("Courier New", 6)
        Dim font3 As New Font("Courier New", 8)
        Dim font4 As New Font("Courier New", 18)
        Dim font5 As New Font("Courier New", 6)
        Dim fontCAE As New Font("Courier New", 5.3, FontStyle.Italic)

        Dim alto As Single = 0
        Dim topMargin As Double '= e.MarginBounds.Top
        Dim yPos As Double = 0
        Dim count As Integer = 0
        Dim texto As String = ""

        Dim codigo As String = ""
        Dim unidad As String = ""
        Dim detalle As String = ""
        Dim valoruni As String = ""
        Dim valortot As String = ""
        Dim tabulacion As String = ""
        Dim compensador As Integer = 0
        Dim total As String = ""
        Dim lvalor As String
        Dim lineatotal As String
        Dim tabFac As New MySql.Data.MySqlClient.MySqlDataAdapter
        Dim tabEmp As New MySql.Data.MySqlClient.MySqlDataAdapter
        Dim ivaProd As String = ""
        Dim fac As New datosfacturas

        Dim facTotal As String = ""
        Dim facSubtotal As String = ""
        Dim FacIva21 As String = ""
        Dim FacIva105 As String = ""

        Dim facCAE As String = ""
        Dim facVtoCAE As String = ""
        Dim facCodBARRA As String = ""



        Reconectar()

        tabEmp.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("SELECT  
        emp.nombrefantasia as empnombre,emp.razonsocial as emprazon,emp.direccion as empdire, emp.localidad as emploca, 
        emp.cuit as empcuit, emp.ingbrutos as empib, emp.ivatipo as empcontr,emp.inicioact as empinicioact, emp.drei as empdrei,emp.logo as emplogo, 
        concat(fis.abrev,' ', LPAD(fac.ptovta,4,'0'),'-',lpad(fac.num_fact,8,'0')) as facnum, fac.fecha as facfech, 
        concat(fac.id_cliente,'-',fac.razon) as facrazon, fac.direccion as facdire, fac.localidad as facloca, fac.tipocontr as factipocontr,fac.cuit as faccuit, 
        concat(vend.apellido,', ',vend.nombre) as facvend, condvent.condicion as faccondvta, fac.observaciones2 as facobserva,format(fac.iva105,2,'es_AR') as iva105, format(fac.iva21,2,'es_AR') as iva21,
        '','',fis.donfdesc, fac.cae, fis.letra as facletra, fis.codfiscal as faccodigo, fac.vtocae, fac.codbarra, format(fac.total,2,'es_AR'),format(fac.subtotal,2,'es_AR')   
        FROM fact_vendedor as vend, fact_clientes as cl, fact_conffiscal as fis, fact_empresa as emp, fact_facturas as fac,fact_condventas as condvent  
        where vend.id=fac.vendedor and cl.idclientes=fac.id_cliente and emp.id=1 and fis.donfdesc=fac.tipofact and condvent.id=fac.condvta and fac.ptovta=fis.ptovta and fac.id=" & IdFactura, conexionPrinc)

        Dim tablaEmpresa As New DataTable
        tabEmp.Fill(tablaEmpresa)

        Reconectar()

        tabFac.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("select 
            plu,
            format(replace(cantidad,',','.'),2,'es_AR') as cant, descripcion, 
            format(replace(iva,',','.'),2,'es_AR') as iva ,
            format(replace(punit,',','.'),2,'es_AR') as punit ,
            format(replace(ptotal,',','.'),2,'es_AR') as ptotal 
            from fact_items where id_fact=" & IdFactura, conexionPrinc)
        Dim tablaProd As New DataTable
        tabFac.Fill(tablaProd)

        facTotal = tablaEmpresa.Rows(0).Item(30)
        facSubtotal = tablaEmpresa.Rows(0).Item(31)
        FacIva21 = tablaEmpresa.Rows(0).Item(21)
        FacIva105 = ""

        facCAE = tablaEmpresa.Rows(0).Item(25)
        facCodBARRA = tablaEmpresa.Rows(0).Item(29)
        facVtoCAE = tablaEmpresa.Rows(0).Item(28)


        e.Graphics.DrawImage(Image.FromFile(Application.StartupPath & "\logo2.jpg"), 5, 15)
        'fac.Tables("factura_enca")["empnombre"].tostring()

        e.Graphics.DrawString("Razón social: " & tablaEmpresa.Rows(0).Item(0), font5, Brushes.Black, 0, 100) 'RAZON SOCIAL
        e.Graphics.DrawString("Tiket N°: " & tablaEmpresa.Rows(0).Item(10), font5, Brushes.Black, 0, 110) '
        e.Graphics.DrawString("Fecha: " & tablaEmpresa.Rows(0).Item(11).ToString, font5, Brushes.Black, 0, 120) '
        e.Graphics.DrawString("Ciente: " & tablaEmpresa.Rows(0).Item(12), font5, Brushes.Black, 0, 130) '
        e.Graphics.DrawString("#Articulos:" & tablaProd.Rows.Count, font5, Brushes.Black, 0, 140) '
        e.Graphics.DrawString(StrDup(65, "#"), font5, Brushes.Black, 0, 150)
        e.Graphics.DrawString("### TIKET NO VALIDO COMO FACTURA ###", font5, Brushes.Black, 0, 160)
        e.Graphics.DrawString(StrDup(65, "#"), font5, Brushes.Black, 0, 170)
        'e.Graphics.DrawString(StrDup(65, "*"), font5, Brushes.Black, 0, 180)
        'e.Graphics.DrawString("CODIGO      |    CANT    | PRE. UNI | PRE. TOTAL ", font5, Brushes.Black, 0, 190)
        'e.Graphics.DrawString("DESCRIPCION", font5, Brushes.Black, 0, 200)
        'e.Graphics.DrawString(StrDup(65, "*"), font5, Brushes.Black, 0, 210)

        Dim i As Integer
        Dim j As Integer
        Dim car As Integer

        For i = 0 To tablaProd.Rows.Count - 1
            codigo = tablaProd.Rows(i).Item(0)
            unidad = tablaProd(i).Item(1)
            detalle = tablaProd(i).Item(2)
            valoruni = tablaProd(i).Item(4)
            valortot = FormatNumber(tablaProd(i).Item(5), 2)
            ivaProd = tablaProd(i).Item(3)
            texto = unidad & " x " & valoruni & Chr(9) & "  (" & ivaProd & ")"
            yPos = 190 + topMargin + (count * printfont.GetHeight(e.Graphics)) ' Calcula la posición en la que se escribe la línea            


            If detalle.Length <= 25 Then
                car = 25 - detalle.Length
                For j = 0 To car
                    detalle &= " "
                Next
            Else
                car = detalle.Length - 25
                detalle = detalle.Remove(26, car - 1)
            End If

            If valortot.Length <= 7 Then
                car = 7 - valortot.Length
                For j = 0 To car
                    valortot = " " & valortot
                Next

            End If


            'If Not row.IsNewRow Then
            e.Graphics.DrawString(texto, printfont, System.Drawing.Brushes.Black, 0, yPos)
            count += 1
            yPos = yPos + 10
            e.Graphics.DrawString(detalle & "  " & valortot, printfont, System.Drawing.Brushes.Black, 0, yPos)
            'total += valor
            'End If

            count += 1

        Next

        If FacIva21.Length <= 7 Then
            car = 7 - FacIva21.Length
            For j = 0 To car
                FacIva21 = " " & FacIva21
            Next

        End If


        If facSubtotal.Length <= 7 Then
            car = 7 - facSubtotal.Length
            For j = 0 To car
                facSubtotal = " " & facSubtotal
            Next
        End If

        If facTotal.Length <= 7 Then
            car = 7 - facTotal.Length
            For j = 0 To car
                facTotal = " " & facTotal
            Next
        End If




        yPos += 20
        Dim textosub As String = "Subtotal"
        Dim textoIva21 As String = "Alicuota 21%"
        Dim textoTotal As String = "Total"



        Dim lineaSep = StrDup(27, " ")
        e.Graphics.DrawString(lineaSep & "__________", printfont, System.Drawing.Brushes.Black, 0, yPos)
        Dim XXX As Integer = 0

        XXX = 27 - (textosub.Length + facSubtotal.Length)
        lineatotal = StrDup(XXX, ".")
        yPos += 10
        e.Graphics.DrawString(textosub & lineatotal & facSubtotal, font3, System.Drawing.Brushes.Black, 0, yPos)

        XXX = 27 - (textoIva21.Length + FacIva21.Length)
        lineatotal = StrDup(XXX, ".")
        yPos += 10
        e.Graphics.DrawString(textoIva21 & lineatotal & FacIva21, font3, System.Drawing.Brushes.Black, 0, yPos)

        XXX = 27 - (textoTotal.Length + facTotal.Length)
        lineatotal = StrDup(XXX, ".")
        yPos += 10
        e.Graphics.DrawString(textoTotal & lineatotal & facTotal, font3, System.Drawing.Brushes.Black, 0, yPos)
        yPos += 30

        e.Graphics.DrawString("Gracias por tu compra!!!", font3, System.Drawing.Brushes.Black, 15, yPos)



    End Sub

    Private Sub ImprimirTiketFiscal(ByVal sender As System.Object, ByVal e As PrintPageEventArgs)
        ' letra
        'Dim font1 As New Font("EAN-13", 40)
        Dim printfont As New Font("Courier New", 6)
        Dim font3 As New Font("Courier New", 8)
        Dim font4 As New Font("Courier New", 18)
        Dim font5 As New Font("Courier New", 6)
        Dim fontCAE As New Font("Courier New", 5.3, FontStyle.Italic)

        Dim alto As Single = 0
        Dim topMargin As Double '= e.MarginBounds.Top
        Dim yPos As Double = 0
        Dim count As Integer = 0
        Dim texto As String = ""

        Dim codigo As String = ""
        Dim unidad As String = ""
        Dim detalle As String = ""
        Dim valoruni As String = ""
        Dim valortot As String = ""
        Dim tabulacion As String = ""
        Dim compensador As Integer = 0
        Dim total As String = ""
        Dim lvalor As String
        Dim lineatotal As String
        Dim tabFac As New MySql.Data.MySqlClient.MySqlDataAdapter
        Dim tabEmp As New MySql.Data.MySqlClient.MySqlDataAdapter
        Dim ivaProd As String = ""
        Dim fac As New datosfacturas

        Dim facTotal As String = ""
        Dim facSubtotal As String = ""
        Dim FacIva21 As String = ""
        Dim FacIva105 As String = ""

        Dim facCAE As String = ""
        Dim facVtoCAE As String = ""
        Dim facCodBARRA As String = ""



        Reconectar()

        tabEmp.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("SELECT  
        emp.nombrefantasia as empnombre,emp.razonsocial as emprazon,emp.direccion as empdire, emp.localidad as emploca, 
        emp.cuit as empcuit, emp.ingbrutos as empib, emp.ivatipo as empcontr,emp.inicioact as empinicioact, emp.drei as empdrei,emp.logo as emplogo, 
        concat(fis.abrev,' ', LPAD(fac.ptovta,4,'0'),'-',lpad(fac.num_fact,8,'0')) as facnum, fac.fecha as facfech, 
        concat(fac.id_cliente,'-',fac.razon) as facrazon, fac.direccion as facdire, fac.localidad as facloca, fac.tipocontr as factipocontr,fac.cuit as faccuit, 
        concat(vend.apellido,', ',vend.nombre) as facvend, condvent.condicion as faccondvta, fac.observaciones2 as facobserva,format(fac.iva105,2,'es_AR') as iva105, format(fac.iva21,2,'es_AR') as iva21,
        '','',fis.donfdesc, fac.cae, fis.letra as facletra, fis.codfiscal as faccodigo, fac.vtocae, fac.codbarra, format(fac.total,2,'es_AR'),format(fac.subtotal,2,'es_AR')   
        FROM fact_vendedor as vend, fact_clientes as cl, fact_conffiscal as fis, fact_empresa as emp, fact_facturas as fac,fact_condventas as condvent  
        where vend.id=fac.vendedor and cl.idclientes=fac.id_cliente and emp.id=1 and fis.donfdesc=fac.tipofact and condvent.id=fac.condvta and fac.ptovta=fis.ptovta and fac.id=" & idFactura, conexionPrinc)

        Dim tablaEmpresa As New DataTable
        tabEmp.Fill(tablaEmpresa)

        Reconectar()

        tabFac.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("select 
            plu,
            format(replace(cantidad,',','.'),2,'es_AR') as cant, descripcion, 
            format(replace(iva,',','.'),2,'es_AR') as iva ,
            format(replace(punit,',','.'),2,'es_AR') as punit ,
            format(replace(ptotal,',','.'),2,'es_AR') as ptotal 
            from fact_items where id_fact=" & idFactura, conexionPrinc)
        Dim tablaProd As New DataTable
        tabFac.Fill(tablaProd)

        facTotal = tablaEmpresa.Rows(0).Item(30)
        facSubtotal = tablaEmpresa.Rows(0).Item(31)
        FacIva21 = tablaEmpresa.Rows(0).Item(21)
        FacIva105 = ""

        facCAE = tablaEmpresa.Rows(0).Item(25)
        facCodBARRA = tablaEmpresa.Rows(0).Item(29)
        facVtoCAE = tablaEmpresa.Rows(0).Item(28)
        Dim TipoFact As Integer = tablaEmpresa.Rows(0).Item(24)

        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(1), font5, Brushes.Black, 0, 100) 'RAZON SOCIAL
        e.Graphics.DrawString("CUIT Nro: " & tablaEmpresa.Rows(0).Item(4), font5, Brushes.Black, 0, 110) '
        e.Graphics.DrawString("Ing. Brutos: " & tablaEmpresa.Rows(0).Item(5).ToString, font5, Brushes.Black, 0, 120) '
        e.Graphics.DrawString("Domicilio: " & tablaEmpresa.Rows(0).Item(2), font5, Brushes.Black, 0, 130)
        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(3), font5, Brushes.Black, 0, 140) '
        e.Graphics.DrawString("Inicio de actividades: " & tablaEmpresa.Rows(0).Item(7), font5, Brushes.Black, 0, 150)
        e.Graphics.DrawString("IVA " & tablaEmpresa.Rows(0).Item(6), font5, Brushes.Black, 0, 160)

        e.Graphics.DrawString(StrDup(65, "*"), font5, Brushes.Black, 0, 170)
        e.Graphics.DrawString("FACTURA '" & tablaEmpresa.Rows(0).Item(26) & "' (" & tablaEmpresa.Rows(0).Item(27) & ")", font5, Brushes.Black, 0, 180)
        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(10).ToString, font5, Brushes.Black, 0, 190)
        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(11).ToString, font5, Brushes.Black, 0, 200)
        e.Graphics.DrawString(StrDup(65, "*"), font5, Brushes.Black, 0, 210)

        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(12), font5, Brushes.Black, 0, 220)
        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(13), font5, Brushes.Black, 0, 230)
        e.Graphics.DrawString(tablaEmpresa.Rows(0).Item(14), font5, Brushes.Black, 0, 240)
        e.Graphics.DrawString("CUIT Nro: " & tablaEmpresa.Rows(0).Item(16), font5, Brushes.Black, 0, 250)
        e.Graphics.DrawString("IVA " & tablaEmpresa.Rows(0).Item(15), font5, Brushes.Black, 0, 260)
        e.Graphics.DrawString("CONDICION DE VENTA " & tablaEmpresa.Rows(0).Item(18), font5, Brushes.Black, 0, 270)
        e.Graphics.DrawString(StrDup(65, "*"), font5, Brushes.Black, 0, 280)

        Dim i As Integer
        Dim j As Integer
        Dim car As Integer

        For i = 0 To tablaProd.Rows.Count - 1
            codigo = tablaProd.Rows(i).Item(0)
            unidad = tablaProd(i).Item(1)
            detalle = tablaProd(i).Item(2)
            valoruni = tablaProd(i).Item(4)
            valortot = FormatNumber(tablaProd(i).Item(5), 2)
            ivaProd = tablaProd(i).Item(3)
            texto = unidad & " x " & valoruni & Chr(9) & "  (" & ivaProd & ")"
            yPos = 290 + topMargin + (count * printfont.GetHeight(e.Graphics)) ' Calcula la posición en la que se escribe la línea            


            If detalle.Length <= 25 Then
                car = 25 - detalle.Length
                For j = 0 To car
                    detalle &= " "
                Next
            Else
                car = detalle.Length - 25
                detalle = detalle.Remove(26, car - 1)
            End If

            If valortot.Length <= 7 Then
                car = 7 - valortot.Length
                For j = 0 To car
                    valortot = " " & valortot
                Next

            End If


            'If Not row.IsNewRow Then
            e.Graphics.DrawString(texto, printfont, System.Drawing.Brushes.Black, 0, yPos)
            count += 1
            yPos = yPos + 10
            e.Graphics.DrawString(detalle & "  " & valortot, printfont, System.Drawing.Brushes.Black, 0, yPos)
            'total += valor
            'End If
            count += 1

        Next
        If FacIva21.Length <= 7 Then
            car = 7 - FacIva21.Length
            For j = 0 To car
                FacIva21 = " " & FacIva21
            Next

        End If


        If facSubtotal.Length <= 7 Then
            car = 7 - facSubtotal.Length
            For j = 0 To car
                facSubtotal = " " & facSubtotal
            Next
        End If

        If facTotal.Length <= 7 Then
            car = 7 - facTotal.Length
            For j = 0 To car
                facTotal = " " & facTotal
            Next
        End If

        yPos += 20
        Dim textosub As String = "Subtotal"
        Dim textoIva21 As String = "Alicuota 21%"
        Dim textoTotal As String = "Total"



        Dim lineaSep = StrDup(27, " ")
        e.Graphics.DrawString(lineaSep & "__________", printfont, System.Drawing.Brushes.Black, 0, yPos)
        Dim XXX As Integer = 0
        If TipoFact < 3 Then

            XXX = 27 - (textosub.Length + facSubtotal.Length)
            lineatotal = StrDup(XXX, ".")
            yPos += 10
            e.Graphics.DrawString(textosub & lineatotal & facSubtotal, font3, System.Drawing.Brushes.Black, 0, yPos)

            XXX = 27 - (textoIva21.Length + FacIva21.Length)
            lineatotal = StrDup(XXX, ".")
            yPos += 10
            e.Graphics.DrawString(textoIva21 & lineatotal & FacIva21, font3, System.Drawing.Brushes.Black, 0, yPos)

            XXX = 27 - (textoTotal.Length + facTotal.Length)
            lineatotal = StrDup(XXX, ".")
            yPos += 10

        Else
            XXX = 27 - (textosub.Length + facTotal.Length)
            lineatotal = StrDup(XXX, ".")
            yPos += 10
            e.Graphics.DrawString(textosub & lineatotal & facTotal, font3, System.Drawing.Brushes.Black, 0, yPos)

            XXX = 27 - (textoIva21.Length + 1)
            lineatotal = StrDup(XXX, ".")
            yPos += 10
            e.Graphics.DrawString(textoIva21 & lineatotal & "0", font3, System.Drawing.Brushes.Black, 0, yPos)

            XXX = 27 - (textoTotal.Length + facTotal.Length)
            lineatotal = StrDup(XXX, ".")
            yPos += 10
            e.Graphics.DrawString(textoTotal & lineatotal & facTotal, font3, System.Drawing.Brushes.Black, 0, yPos)
            yPos += 30
        End If



        e.Graphics.DrawString("COMPROBANTE AUTORIZADO POR WEB SERVICE", fontCAE, System.Drawing.Brushes.Black, 0, yPos)
        yPos += 10

        e.Graphics.DrawString("CAE: " & facCAE, fontCAE, System.Drawing.Brushes.Black, 0, yPos)
        yPos += 10

        e.Graphics.DrawString("F. Vto CAE: " & facVtoCAE, fontCAE, System.Drawing.Brushes.Black, 0, yPos)
        yPos += 10
        e.Graphics.DrawString(facCodBARRA, fontCAE, System.Drawing.Brushes.Black, 0, yPos)
        yPos += 10

        e.Graphics.DrawString(My.Settings.TextoPieTiket, font3, System.Drawing.Brushes.Black, 15, yPos)
        yPos += 10
        e.Graphics.DrawString("Gracias por tu compra!!!", font3, System.Drawing.Brushes.Black, 15, yPos)



    End Sub

    Public Sub ImprimirFactura(idfact As Integer, ptovta As Integer, condvta As Integer)
        Try
            'Dim tabIVComp As New MySql.Data.MySqlClient.MySqlDataAdapter
            Dim tabFac As New MySql.Data.MySqlClient.MySqlDataAdapter
            Dim tabEmp As New MySql.Data.MySqlClient.MySqlDataAdapter
            Dim fac As New datosfacturas

            Reconectar()

            tabEmp.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("SELECT  
emp.nombrefantasia as empnombre,emp.razonsocial as emprazon,emp.direccion as empdire, emp.localidad as emploca, 
emp.cuit as empcuit, emp.ingbrutos as empib, emp.ivatipo as empcontr,emp.inicioact as empinicioact, emp.drei as empdrei,emp.logo as emplogo, 
concat(fis.abrev,' ', LPAD(fac.ptovta,4,'0'),'-',lpad(fac.num_fact,8,'0')) as facnum, fac.fecha as facfech, 
concat(fac.id_cliente,'-',fac.razon) as facrazon, fac.direccion as facdire, fac.localidad as facloca, fac.tipocontr as factipocontr,fac.cuit as faccuit, 
concat(vend.apellido,', ',vend.nombre) as facvend, condvent.condicion as faccondvta, fac.observaciones2 as facobserva,format(fac.iva105,2,'es_AR') as iva105, format(fac.iva21,2,'es_AR') as iva21,
'','',fis.donfdesc, fac.cae, fis.letra as facletra, fis.codfiscal as faccodigo, fac.vtocae, fac.codbarra 
FROM fact_vendedor as vend, fact_clientes as cl, fact_conffiscal as fis, fact_empresa as emp, fact_facturas as fac,fact_condventas as condvent  
where vend.id=fac.vendedor and cl.idclientes=fac.id_cliente and emp.id=1 and fis.donfdesc=fac.tipofact and condvent.id=fac.condvta and fac.id=" & idfact, conexionPrinc)

            tabEmp.Fill(fac.Tables("factura_enca"))
            Reconectar()

            tabFac.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("select 
            plu,
            format(replace(cantidad,',','.'),2,'es_AR') as cant, descripcion, 
            format(replace(iva,',','.'),2,'es_AR') as iva ,
            format(replace(punit,',','.'),2,'es_AR') as punit ,
            format(replace(ptotal,',','.'),2,'es_AR') as ptotal 
            from fact_items where id_fact=" & idfact, conexionPrinc)
            tabFac.Fill(fac.Tables("facturax"))

            Dim direccionReport As String
            If ptovta <> FacturaElectro.puntovtaelect Then
                direccionReport = System.Environment.CurrentDirectory & "\reportes\facturax.rdlc"
            Else
                direccionReport = System.Environment.CurrentDirectory & "\reportes\facturaelectro.rdlc"
            End If
            If My.Settings.ImprTikets = 1 And condvta = 1 Then
                Dim PrintTxt As New PrintDocument
                Dim pgSize As New PaperSize
                pgSize.RawKind = Printing.PaperKind.Custom
                pgSize.Width = 147 '196.8 '
                idFactura = idfact
                'pgSize.Height = 173.23 '100
                PrintTxt.DefaultPageSettings.PaperSize = pgSize
                ' evento print

                If ptovta <> FacturaElectro.puntovtaelect Then
                    AddHandler PrintTxt.PrintPage, AddressOf ImprimirTiketVenta
                    PrintTxt.PrinterSettings.PrinterName = My.Settings.ImprTiketsNombre
                    PrintTxt.Print()
                Else
                    AddHandler PrintTxt.PrintPage, AddressOf ImprimirTiketFiscal
                    PrintTxt.PrinterSettings.PrinterName = My.Settings.ImprTiketsNombre
                    PrintTxt.Print()
                End If
            Else

                Using Imprimir As New ImprimirDirecto()
                    Imprimir.Run(fac.Tables("factura_enca"), fac.Tables("facturax"), direccionReport)
                    Imprimir.Run(fac.Tables("factura_enca"), fac.Tables("facturax"), direccionReport)
                End Using
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            'EnProgreso.Close()
        End Try

    End Sub
    Public Sub ImprimirRemito(idremito As Integer)
        Try
            Dim idFactura As Integer = idremito
            'Dim tabIVComp As New MySql.Data.MySqlClient.MySqlDataAdapter
            Dim tabFac As New MySql.Data.MySqlClient.MySqlDataAdapter
            Dim tabEmp As New MySql.Data.MySqlClient.MySqlDataAdapter
            Dim fac As New datosfacturas

            Reconectar()

            tabEmp.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("SELECT  " _
            & "emp.nombrefantasia as empnombre,emp.razonsocial as emprazon,emp.direccion as empdire, emp.localidad as emploca, " _
            & "emp.cuit as empcuit, emp.ingbrutos as empib, emp.ivatipo as empcontr,emp.inicioact as empinicioact, emp.drei as empdrei,emp.logo as emplogo, " _
            & "concat(fis.abrev,' ', LPAD(fac.ptovta,4,'0'),'-',lpad(fac.num_fact,8,'0')) as facnum, fac.fecha as facfech, " _
            & "concat(fac.id_cliente,'-',fac.razon,' - tel: ',cl.telefono) as facrazon, fac.direccion as facdire, fac.localidad as facloca, fac.tipocontr as factipocontr,fac.cuit as faccuit, " _
            & "concat(vend.apellido,', ',vend.nombre) as facvend, fac.condvta as faccondvta, fac.observaciones2 as facobserva,fac.iva105, fac.iva21, fac.total,'',fis.donfdesc as descfact " _
            & "FROM fact_vendedor as vend, fact_clientes as cl, fact_conffiscal as fis, fact_empresa as emp, fact_facturas as fac  " _
            & "where vend.id=fac.vendedor and cl.idclientes=fac.id_cliente and emp.id=1 and fis.donfdesc=fac.tipofact and fis.ptovta=fac.ptovta and fac.id=" & idFactura, conexionPrinc)

            tabEmp.Fill(fac.Tables("factura_enca"))
            Reconectar()

            tabFac.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("select " _
            & "cantidad as cant, descripcion, iva ,punit ,ptotal as ptotal from fact_items where id_fact=" & idFactura, conexionPrinc)
            tabFac.Fill(fac.Tables("facturax"))
            Dim imprimirx As New imprimirFX
            With imprimirx
                .MdiParent = frmprincipal
                .rptfx.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local
                .rptfx.LocalReport.ReportPath = System.Environment.CurrentDirectory & "\reportes\remito.rdlc"
                '.rptfx.LocalReport.ReportPath = System.Environment.CurrentDirectory & "\facturax.rdlc"
                .rptfx.LocalReport.DataSources.Clear()
                .rptfx.LocalReport.DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("encabezado", fac.Tables("factura_enca")))
                .rptfx.LocalReport.DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("items", fac.Tables("facturax")))
                .rptfx.DocumentMapCollapsed = True
                .rptfx.RefreshReport()
                .Show()
            End With
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    'Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As Integer, <MarshalAs(UnmanagedType.LPWStr)> ByVal lParam As String) As Int32
    'End Function
    Public Function RepararNumeracionComprobantes() As Boolean
        Try
            Dim tablaComp As New DataTable
            Dim i As Integer
            Reconectar()
            Dim consulta As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT max(num_fact) as numfact,tipofact,ptovta 
                                                                    FROM fact_facturas  group by tipofact, ptovta ", conexionPrinc)
            consulta.Fill(tablaComp)

            For i = 0 To tablaComp.Rows.Count - 1
                Reconectar()
                Dim num_Fact As Integer = tablaComp.Rows(i).Item(0)
                Dim tipo_Fact As Integer = tablaComp.Rows(i).Item(1)
                Dim punto_Venta As Integer = tablaComp.Rows(i).Item(2)

                Dim comandocaj As New MySql.Data.MySqlClient.MySqlCommand("update fact_conffiscal set confnume=" & num_Fact & " 
            where donfdesc=" & tipo_Fact & " and " & "ptovta=" & punto_Venta & "
            ", conexionPrinc)
                comandocaj.ExecuteNonQuery()
            Next
            Return False
        Catch ex As Exception
            Return True
        End Try
    End Function
    Public Function ExisteProducto(ByVal codigo As String) As Boolean
        Reconectar()
        Dim sqlQuery As String = "select id from fact_insumos where cod_bar like '" & codigo & "' or codigo like '" & codigo & "'"
        Dim ConsultaProd As New MySql.Data.MySqlClient.MySqlDataAdapter(sqlQuery, conexionPrinc)
        Dim readProd As New DataTable
        ConsultaProd.Fill(readProd)

        If readProd.Rows.Count <> 0 Then
            Return True
        Else
            Return False
        End If

    End Function
    Public Function IdProducto(ByVal codigo As String) As Integer
        Reconectar()

        Dim sqlQuery As String = "select id from fact_insumos where cod_bar like '" & codigo & "' or codigo like '" & codigo & "' limit 0,1"
        Dim ConsultaProd As New MySql.Data.MySqlClient.MySqlDataAdapter(sqlQuery, conexionPrinc)
        Dim readProd As New DataTable
        ConsultaProd.Fill(readProd)
        Dim filasProd() As DataRow
        filasProd = readProd.Select("")
        If readProd.Rows.Count <> 0 Then
            Return filasProd(0)(0)
        End If

    End Function

    Public Sub cerrar_Conexiones()
        Try
            'conexionEmp.Close()
            conexionPrinc.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Public Sub abrir_Conexiones()
        Try
            'conexionEmp.Open()
            conexionPrinc.Open()
            conexionPrinc.ChangeDatabase(database)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Public Sub Reconectar()
        Try
            conexionPrinc.Close()
            conexionPrinc.Open()
            conexionPrinc.ChangeDatabase(database)
        Catch ex As Exception

        End Try
    End Sub

    Public Function reemplazar_espacios(ByRef texto As String) As String
        Dim reemplazo As String
        Try
            reemplazo = Replace(texto, " ", "_").ToUpper
            Return reemplazo
        Catch ex As Exception
            Return "error"
        End Try
    End Function
    Public Function ValidaControles(ByVal ctrlContenedor As Form) As Boolean
        'Recorre todos y cada uno de los controles contenidos en el contenedor
        For Each Control As Control In ctrlContenedor.Controls
            'Si el control que se esta revisando es un textbox
            If TypeOf Control Is TextBox Then
                'Verificamos que tenga informacion
                If Trim(Control.Text) = "" Then
                    'Si no tiene informacion mandamos un MSGBOX con el mensaje apropiado el cual se encuentra en el tag del control
                    MsgBox(Control.Tag, MsgBoxStyle.OkOnly + MsgBoxStyle.Information, Application.ProductName)
                    'Pone el foco en el control
                    Control.BackColor = Color.Salmon
                    Control.Focus()
                    'Regresa un falso indicando que los controles no estan llenados correctamente

                    Exit For
                    Return False
                End If
                'Si el control que se esta revisando es un ComboBox   
            ElseIf TypeOf Control Is ComboBox Then
                'Hago un tipo cast para convertirlo a ComboBOx porque por alguna extraña razon no puedo ingresar a sus propiedades correctamente     
                Dim aux As ComboBox = Control
                'Si no tienen ningun elemento seleccionado el combobox mandamos un MSGBOX con el mensaje apropiado el cual se encuentra en el tag del control
                If aux.SelectedValue = Nothing Then
                    MsgBox(Control.Tag, MsgBoxStyle.OkOnly + MsgBoxStyle.Information, Application.ProductName)
                    'Pone el foco en el control 
                    Control.Focus()
                    'Regresa un falso indicando que los controles no estan llenados correctamente
                    Return False
                    Exit For
                End If
            End If
        Next
        'Si no hubo ningun problema con los controles regresamos un true indicando que los controles estan llenados correctamente
        Return True
    End Function
    'convertir binario a imágen
    Public Function Bytes_Imagen(ByVal Imagen As Byte()) As Image
        Try
            'si hay imagen
            If Not Imagen Is Nothing Then
                'caturar array con memorystream hacia Bin
                Dim Bin As New MemoryStream(Imagen)
                'con el método FroStream de Image obtenemos imagen
                Dim Resultado As Image = Image.FromStream(Bin)
                'y la retornamos
                Return Resultado
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    'convertir imagen a binario
    Public Function Imagen_Bytes(ByVal Imagen As Image) As Byte()
        'si hay imagen
        If Not Imagen Is Nothing Then
            'variable de datos binarios en stream(flujo)
            Dim Bin As New MemoryStream
            'convertir a bytes
            Imagen.Save(Bin, Imaging.ImageFormat.Jpeg)
            'retorna binario
            Return Bin.GetBuffer
        Else
            Return Nothing
        End If
    End Function

    'Public Function archivo_Bytes(ByVal Imagen As Image) As Byte()
    '    'si hay imagen
    '    If Not Imagen Is Nothing Then
    '        'variable de datos binarios en stream(flujo)
    '        Dim Bin As New MemoryStream
    '        'convertir a bytes
    '        Imagen.Save(Bin, Imaging.ImageFormat.Jpeg)
    '        'retorna binario
    '        Return Bin.GetBuffer
    '    Else
    '        Return Nothing
    '    End If
    'End Function

    Public Function remplazarcoma(ByVal cadena As String) As String
        Dim pos As Integer
        pos = InStr(cadena, ".")
        If pos <> 0 Then
            Mid(cadena, pos, pos + 1) = ","
            remplazarcoma = cadena

        Else : remplazarcoma = cadena
        End If

    End Function

    Public Function remplazarPunto(ByVal cadena As String) As String
        Dim pos As Integer
        pos = InStr(cadena, ",")
        If pos <> 0 Then
            Mid(cadena, pos, pos + 1) = "."
            remplazarPunto = cadena

        Else : remplazarPunto = cadena
        End If

    End Function

    Public Function PonerComodinSQL(ByVal cadena As String) As String
        Dim pos As Integer
        pos = InStr(cadena, " ")
        If pos <> 0 Then
            Mid(cadena, pos, pos + 1) = "%"
            PonerComodinSQL = cadena

        Else : PonerComodinSQL = cadena
        End If

    End Function

    Public Function SincronizarBD() As Boolean
        Try
            Dim obj As Object
            Dim archivo As Object
            obj = CreateObject("Scripting.FileSystemObject")
            archivo = obj.createtextfile(Application.StartupPath & "\sincro.xml", True)
            archivo.writeline("<?xml version='1.0' encoding='UTF-8'?>")
            archivo.writeline("<job version='11.1'>")
            archivo.writeline("<syncjob>")
            archivo.writeline("<fkcheck check='yes' />")
            archivo.writeline("<twowaysync twoway='no' />")
            archivo.writeline("<passwords isencoded='no' />")
            archivo.writeline("<source>")
            archivo.writeline("<host>" & DatosAcceso.CLOUDserv & "</host>")
            archivo.writeline("<user>" & DatosAcceso.usuario & "</user>")
            archivo.writeline("<pwd>" & DatosAcceso.pass & "</pwd>")
            archivo.writeline("<port>" & DatosAcceso.puerto & "</port>")
            archivo.writeline("<compressed>1</compressed>")
            archivo.writeline("<ssl>0</ssl>")
            archivo.writeline("<sslauth>0</sslauth>")
            archivo.writeline("<clientkey></clientkey>")
            archivo.writeline("<clientcert></clientcert>")
            archivo.writeline("<cacert></cacert>")
            archivo.writeline("<cipher></cipher>")
            archivo.writeline("<charset></charset>")
            archivo.writeline("<database>" & DatosAcceso.bd & "</database>")
            archivo.writeline("</source>")
            archivo.writeline("<target>")
            archivo.writeline("<host>" & DatosAcceso.RESPserv & "</host>")
            archivo.writeline("<user>" & DatosAcceso.usuario & "</user>")
            archivo.writeline("<pwd>" & DatosAcceso.pass & "</pwd>")
            archivo.writeline("<port>" & DatosAcceso.puerto & "</port>")
            archivo.writeline("<compressed>1</compressed>")
            archivo.writeline("<ssl>0</ssl>")
            archivo.writeline("<sslauth>0</sslauth>")
            archivo.writeline("<clientkey></clientkey>")
            archivo.writeline("<clientcert></clientcert>")
            archivo.writeline("<cacert></cacert>")
            archivo.writeline("<cipher></cipher>")
            archivo.writeline("<charset></charset>")
            archivo.writeline("<database>" & DatosAcceso.bd & "</database>")
            archivo.writeline("</target>")
            archivo.writeline("<tables all='yes'/>")
            archivo.writeline("<sync_action type='directsync'/>")
            archivo.writeline("<abortonerror abort='no' />")
            archivo.writeline("<sendreport send='yes' when='onerror'><smtp>")
            archivo.writeline("<displayname>KIBIT SERVICIO DE SINCRONIZACION</displayname>")
            archivo.writeline("<toemail>info@kibit.com.ar</toemail>")
            archivo.writeline("<fromemail>info@kibit.com.ar</fromemail>")
            archivo.writeline("<replyemail>info@kibit.com.ar</replyemail>")
            archivo.writeline("<host>mail.kibit.com.ar</host>")
            archivo.writeline("<encryption>no</encryption>")
            archivo.writeline("<port>25</port>")
            archivo.writeline("<auth required='yes'>")
            archivo.writeline("<user>info@kibit.com.ar</user>")
            archivo.writeline("<pwd>Narinas1830</pwd>")
            archivo.writeline("</auth>")
            archivo.writeline("<subject>yyyy-mm-dd hh:mm:ss</subject>")
            archivo.writeline("</smtp>")
            archivo.writeline("</sendreport></syncjob>")
            archivo.writeline("</job>")
            archivo.close()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function SincronizarTabla(ByRef tabla As String) As Boolean
        Try
            Dim obj As Object
            Dim archivo As Object
            obj = CreateObject("Scripting.FileSystemObject")
            archivo = obj.createtextfile(Application.StartupPath & "\sincro.xml", True)
            archivo.writeline("<?xml version='1.0' encoding='UTF-8'?>")
            archivo.writeline("<job version='11.1'>")
            archivo.writeline("<syncjob>")
            archivo.writeline("<fkcheck check='yes' />")
            archivo.writeline("<twowaysync twoway='no' />")
            archivo.writeline("<source>")
            archivo.writeline("<host>" & DatosAcceso.CLOUDserv & "</host>")
            archivo.writeline("<user>" & DatosAcceso.usuario & "</user>")
            archivo.writeline("<pwd>" & DatosAcceso.pass & "</pwd>")
            archivo.writeline("<port>" & DatosAcceso.puerto & "</port>")
            archivo.writeline("<compressed>1</compressed>")
            archivo.writeline("<ssl>0</ssl>")
            archivo.writeline("<sslauth>0</sslauth>")
            archivo.writeline("<clientkey></clientkey>")
            archivo.writeline("<clientcert></clientcert>")
            archivo.writeline("<cacert></cacert>")
            archivo.writeline("<cipher></cipher>")
            archivo.writeline("<charset></charset>")
            archivo.writeline("<database>" & DatosAcceso.bd & "</database>")
            archivo.writeline("</source>")
            archivo.writeline("<target>")
            archivo.writeline("<host>" & DatosAcceso.RESPserv & "</host>")
            archivo.writeline("<user>" & DatosAcceso.usuario & "</user>")
            archivo.writeline("<pwd>" & DatosAcceso.pass & "</pwd>")
            archivo.writeline("<port>" & DatosAcceso.puerto & "</port>")
            archivo.writeline("<compressed>1</compressed>")
            archivo.writeline("<ssl>0</ssl>")
            archivo.writeline("<sslauth>0</sslauth>")
            archivo.writeline("<clientkey></clientkey>")
            archivo.writeline("<clientcert></clientcert>")
            archivo.writeline("<cacert></cacert>")
            archivo.writeline("<cipher></cipher>")
            archivo.writeline("<charset></charset>")
            archivo.writeline("<database>" & DatosAcceso.bd & "</database>")
            archivo.writeline("</target>")

            archivo.writeline("<tables all='no'>")
            archivo.writeline("<table>")
            archivo.writeline("<name>`" & tabla & "`</name>'")
            archivo.writeline("<columns all='yes' />")
            archivo.writeline("</table>")
            archivo.writeline("</tables>")
            archivo.writeline("<sync_action type='directsync'/>")
            archivo.writeline("<abortonerror abort='no' />")
            archivo.writeline("<sendreport send='yes' when='onerror'><smtp>")
            archivo.writeline("<displayname>KIBIT SERVICIO DE SINCRONIZACION</displayname>")
            archivo.writeline("<toemail>info@kibit.com.ar</toemail>")
            archivo.writeline("<fromemail>info@kibit.com.ar</fromemail>")
            archivo.writeline("<replyemail>info@kibit.com.ar</replyemail>")
            archivo.writeline("<host>mail.kibit.com.ar</host>")
            archivo.writeline("<encryption>no</encryption>")
            archivo.writeline("<port>25</port>")
            archivo.writeline("<auth required='yes'>")
            archivo.writeline("<user>info@kibit.com.ar</user>")
            archivo.writeline("<pwd>Hacha123</pwd>")
            archivo.writeline("</auth>")
            archivo.writeline("<subject>yyyy-mm-dd hh:mm:ss</subject>")
            archivo.writeline("</smtp>")
            archivo.writeline("</sendreport></syncjob>")
            archivo.writeline("</job>")
            archivo.close()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function CompletarCeros(ByVal numero As Integer, ByVal tip As Integer) As String
        Select Case tip
            Case 1
                Return Format(numero, "0000000000")
            Case 2
                Return Format(numero, "0000")
        End Select
    End Function

    Public Function RestringirNumerosFact(ByVal tipo As String, ByVal numero As String, ByVal Ptovta As Integer) As Boolean
        Try
            Reconectar()
            Dim consulta As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT id from fact_facturas where num_fact=" & numero & " and tipofact=" & tipo & " and ptovta=" & Ptovta, conexionPrinc)
            Dim tablacl As New DataTable
            Dim infocl() As DataRow
            consulta.Fill(tablacl)

            If tablacl.Rows.Count <> 0 Then
                If RepararNumeracionComprobantes() = True Then
                    Return False
                Else
                    Return True
                End If

            End If
            If tablacl.Rows.Count = 0 Then Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function ObtenerNumerosFact(ByVal tipo As String, ByVal PtoVta As Integer) As Integer
        Try
            Reconectar()
            Dim consulta As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT confnume+1 from fact_conffiscal where donfdesc=" & tipo & " and ptovta= " & PtoVta, conexionPrinc)
            Dim tablacl As New DataTable
            Dim infocl() As DataRow
            consulta.Fill(tablacl)
            infocl = tablacl.Select()
            Return infocl(0)(0)
        Catch ex As Exception

        End Try
    End Function

    Public Sub GenerarExcel(ByRef ElGrid As DataGridView)
        'Creamos las variables
        Dim exApp As New Microsoft.Office.Interop.Excel.Application
        Dim exLibro As Microsoft.Office.Interop.Excel.Workbook
        Dim exHoja As Microsoft.Office.Interop.Excel.Worksheet
        Try
            'Añadimos el Libro al programa, y la hoja al libro
            exLibro = exApp.Workbooks.Add
            exHoja = exLibro.Worksheets.Add()
            ' ¿Cuantas columnas y cuantas filas?
            Dim NCol As Integer = ElGrid.ColumnCount
            Dim NRow As Integer = ElGrid.RowCount
            'Aqui recorremos todas las filas, y por cada fila todas las columnas y vamos escribiendo.
            For i As Integer = 1 To NCol
                exHoja.Cells.Item(1, i) = ElGrid.Columns(i - 1).HeaderText.ToString
                'exHoja.Cells.Item(1, i).HorizontalAlignment = 3
            Next
            For Fila As Integer = 0 To NRow - 1
                For Col As Integer = 0 To NCol - 1
                    exHoja.Cells.Item(Fila + 2, Col + 1) = ElGrid.Rows(Fila).Cells(Col).Value
                Next
            Next
            'Titulo en negrita, Alineado al centro y que el tamaño de la columna se ajuste al texto
            exHoja.Rows.Item(1).Font.Bold = 1
            exHoja.Rows.Item(1).HorizontalAlignment = 3
            exHoja.Columns.AutoFit()
            'Aplicación visible
            exApp.Application.Visible = True
            exHoja = Nothing
            exLibro = Nothing
            exApp = Nothing
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error al exportar a Excel")
        End Try
    End Sub

    Public Function ComprobarCliente(ByVal parametro As String) As Boolean

        Dim busqtxt As String
        parametro = parametro.Replace(" ", "%")
        busqtxt = " where nomapell_razon like @busq or dir_domicilio like @busq or cuit like @busq or telefono like @busq or celular like @busq"

        Try
            Reconectar()
            Dim consulta As New MySql.Data.MySqlClient.MySqlDataAdapter("select idclientes as Cuenta, nomapell_razon as Cliente, codClie from fact_clientes where nomapell_razon like @busq or dir_domicilio like @busq or cuit like @busq or telefono like @busq or celular like @busq", conexionPrinc)
            consulta.SelectCommand.Parameters.Add(New MySql.Data.MySqlClient.MySqlParameter("@busq", MySql.Data.MySqlClient.MySqlDbType.Text))
            consulta.SelectCommand.Parameters("@busq").Value = "%" & parametro & "%"
            Dim tablaPers As New DataTable
            consulta.Fill(tablaPers)

            If tablaPers.Rows.Count = 0 Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function GetDataExcel(
  ByVal fileName As String, ByVal sheetName As String) As DataTable
        Try


            ' Comprobamos los parámetros.
            '
            If ((String.IsNullOrEmpty(fileName)) OrElse
          (String.IsNullOrEmpty(sheetName))) Then _
          Throw New ArgumentNullException()


            Dim extension As String = IO.Path.GetExtension(fileName)

            Dim connString As String = "Data Source=" & fileName

            If (extension = ".xls") Then
                connString &= ";Provider=Microsoft.Jet.OLEDB.4.0;" &
                       "Extended Properties='Excel 8.0;HDR=YES;IMEX=1;Allow Formula=false'"

            ElseIf (extension = ".xlsx") Then
                connString &= ";Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;Allow Formula=false'"
            Else
                Throw New ArgumentException(
                  "La extensión " & extension & " del archivo no está permitida.")
            End If

            Using conexion As New OleDbConnection(connString)

                Dim sql As String = "SELECT * FROM [" & sheetName & "$]"
                Dim adaptador As New OleDbDataAdapter(sql, conexion)

                Dim dt As New DataTable("Excel")

                adaptador.Fill(dt)

                Return dt

            End Using

        Catch ex As Exception
            MsgBox(ex.Message)

        End Try

    End Function

    Public Function ComprobarStock(ByRef codigo As String, ByRef cant As String) As Boolean
        Try
            Reconectar()
            Dim consulta As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT sum(stock) FROM fact_insumos_lotes where idproducto=" & codigo & " and idalmacen= " & DatosAcceso.IdAlmacen, conexionPrinc)
            Dim tablacl As New DataTable
            Dim infocl() As DataRow
            consulta.Fill(tablacl)
            infocl = tablacl.Select("")
            ' MsgBox(infocl(0)(0) & ">= " & cant)
            If infocl(0)(0) >= cant Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function QuitarStock(ByRef codigo As String, ByRef cant As String, ByRef idgtia As String) As Boolean
        Dim i As Integer
        Dim lotes As Integer = 0
        Try
            Reconectar()
            If idgtia = 0 Then
                Dim consultastock As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT id, stock FROM fact_insumos_lotes " _
                & "where stock >0 and idproducto=" & codigo & " order by id asc", conexionPrinc)
                Dim tablastock As New DataTable
                Dim infostock() As DataRow
                consultastock.Fill(tablastock)
                infostock = tablastock.Select("")
                'lotes = tablastock.Rows.Count
                Do Until cant = 0
                    If infostock(lotes)(1) <= cant Then
                        cant = cant - infostock(lotes)(1)
                        Reconectar()
                        Dim updstock As New MySql.Data.MySqlClient.MySqlCommand("update fact_insumos_lotes set stock=0 where id=" & infostock(lotes)(0), conexionPrinc)
                        updstock.ExecuteNonQuery()
                        lotes += 1
                    ElseIf infostock(lotes)(1) > cant Then
                        Reconectar()
                        Dim updstock As New MySql.Data.MySqlClient.MySqlCommand("update fact_insumos_lotes set stock=stock-" & cant & " where id=" & infostock(lotes)(0), conexionPrinc)
                        updstock.ExecuteNonQuery()
                        cant = 0
                    End If
                Loop
            End If

        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function QuitarGarantia(ByRef serie As String) As Integer
        Try
            Dim consultapedidoitems As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT ins.id " _
            & "FROM fact_gtia as gtia, fact_insumos_lotes as ins " _
            & "where ins.idproducto=gtia.idproducto and gtia.serie like '" & serie & "'", conexionPrinc)
            ' MsgBox(consultapedidoitems.SelectCommand.CommandText)
            Dim tablaitm As New DataTable
            Dim infoitm() As DataRow
            consultapedidoitems.Fill(tablaitm)
            infoitm = tablaitm.Select("")
            If tablaitm.Rows.Count = 0 Then
                MsgBox("No se encuentra el producto en la tabla de garantias")
                Return 0
            Else
                Return infoitm(0)(0)
            End If
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Function ComprobarGarantia(ByRef serie As String) As String
        Try
            Dim consultapedidoitems As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT prod.codigo " _
                & "FROM fact_gtia as gtia, fact_insumos as prod " _
                & "where prod.codigo=gtia.codigo and gtia.serie like '" & serie & "'", conexionPrinc)
            ' MsgBox(consultapedidoitems.SelectCommand.CommandText)
            Dim tablaitm As New DataTable
            Dim infoitm() As DataRow
            consultapedidoitems.Fill(tablaitm)
            infoitm = tablaitm.Select("")
            If tablaitm.Rows.Count = 0 Then
                'MsgBox("No se encuentra el producto en la tabla de garantias")
                Return 0
            Else
                Return infoitm(0)(0)
            End If
        Catch ex As Exception
            Return 0
        End Try
    End Function
    Public Sub EnviarMail(ByVal mensaje As String, ByVal para As String, ByVal asunto As String)
        Dim mje As New System.Net.Mail.MailMessage()
        Dim SMTP As New System.Net.Mail.SmtpClient

        Reconectar()
        Dim consultaDtosMail As New MySql.Data.MySqlClient.MySqlDataAdapter("select texto1 from tecni_datosgenerales where id>=26 and id<=30", conexionPrinc)
        Dim tablaDtosMail As New DataTable
        Dim infoDtosMail() As DataRow
        consultaDtosMail.Fill(tablaDtosMail)
        infoDtosMail = tablaDtosMail.Select("")

        SMTP.Credentials = New System.Net.NetworkCredential(infoDtosMail(0)(0).ToString, infoDtosMail(1)(0).ToString)
        SMTP.Host = infoDtosMail(2)(0).ToString
        SMTP.Port = infoDtosMail(3)(0).ToString
        SMTP.EnableSsl = False

        mje.[To].Add(para.ToLower)
        mje.From = New System.Net.Mail.MailAddress(infoDtosMail(0)(0).ToString, infoDtosMail(4)(0).ToString, System.Text.Encoding.UTF8)
        mje.Subject = asunto
        mje.SubjectEncoding = System.Text.Encoding.UTF8
        mje.Body = mensaje
        mje.BodyEncoding = System.Text.Encoding.UTF8
        mje.Priority = System.Net.Mail.MailPriority.Normal
        mje.IsBodyHtml = False
        Try
            SMTP.Send(mje)
            MessageBox.Show("Mensaje enviado correctamente", "Exito!", MessageBoxButtons.OK)
        Catch ex As System.Net.Mail.SmtpException
            MessageBox.Show(ex.ToString, "Error!", MessageBoxButtons.OK)
        End Try
    End Sub

    Public Function CalcularCadenaEAN13(ByVal chaine As String) As String

        Dim i, checksum, first As Integer
        Dim CodeBarre As String
        Dim tableA As Boolean

        CalcularCadenaEAN13 = ""
        If Len(chaine) = 12 Then
            For i = 1 To 12
                If Asc(Mid(chaine, i, 1)) < 48 Or Asc(Mid(chaine, i, 1)) > 57 Then
                    i = 0
                    Exit For
                End If
            Next
            If i = 13 Then
                For i = 2 To 12 Step 2
                    checksum = checksum + Val(Mid(chaine, i, 1))
                Next
                checksum = checksum * 3
                For i = 1 To 11 Step 2
                    checksum = checksum + Val(Mid(chaine, i, 1))
                Next
                chaine = chaine & (10 - checksum Mod 10) Mod 10
                CodeBarre = Left(chaine, 1) & Chr(65 + Val(Mid(chaine, 2, 1)))
                first = Val(Left(chaine, 1))
                For i = 3 To 7
                    tableA = False
                    Select Case i
                        Case 3
                            Select Case first
                                Case 0 To 3
                                    tableA = True
                            End Select
                        Case 4
                            Select Case first
                                Case 0, 4, 7, 8
                                    tableA = True
                            End Select
                        Case 5
                            Select Case first
                                Case 0, 1, 4, 5, 9
                                    tableA = True
                            End Select
                        Case 6
                            Select Case first
                                Case 0, 2, 5, 6, 7
                                    tableA = True
                            End Select
                        Case 7
                            Select Case first
                                Case 0, 3, 6, 8, 9
                                    tableA = True
                            End Select
                    End Select
                    If tableA Then
                        CodeBarre = CodeBarre & Chr(65 + Val(Mid(chaine, i, 1)))
                    Else
                        CodeBarre = CodeBarre & Chr(75 + Val(Mid(chaine, i, 1)))
                    End If
                Next
                CodeBarre = CodeBarre & "*"
                For i = 8 To 13
                    CodeBarre = CodeBarre & Chr(97 + Val(Mid(chaine, i, 1)))
                Next
                CodeBarre = CodeBarre & "+"
                CalcularCadenaEAN13 = CodeBarre
            End If
        End If
    End Function

    Public Function CalcularVerificadorEAN13(ByVal EAN As String)             'calcula digito verificador del EAN13
        '**OK**
        Dim dvc As String = ""      'digito verificador calculado
        Dim iSum As Integer = 0
        Dim iDigit As Integer = 0

        For i As Integer = EAN.Length To 1 Step -1
            iDigit = Convert.ToInt32(EAN.Substring(i - 1, 1))
            If (EAN.Length - i + 1) Mod 2 <> 0 Then
                iSum += iDigit * 3
            Else
                iSum += iDigit
            End If
        Next

        Dim iCheckSum As Integer = (10 - (iSum Mod 10)) Mod 10
        dvc = iCheckSum.ToString()

        Return dvc

    End Function
End Module

