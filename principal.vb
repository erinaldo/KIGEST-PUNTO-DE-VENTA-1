﻿Imports System.IO

Public Class frmprincipal
    Public loged As Boolean
    'Public Pac As New frmaspirantes()
    Private Sub frmprincipal_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        End
    End Sub

    Private Sub frmprincipal_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Dim keyCTRL As Boolean
        Dim keyALT As Boolean
        Dim key1 As Boolean
        Try
            If e.Control And e.KeyCode = Keys.NumPad1 Then
                reciboconsfinal.PerformClick()
            ElseIf e.Control And e.KeyCode = Keys.NumPad2 Then
                facturabconsfinal.PerformClick()
            ElseIf e.Control And e.KeyCode = Keys.NumPad3 Then
                FacturaA.PerformClick()
            ElseIf e.Control And e.KeyCode = Keys.NumPad4 Then
                ConsultaDeProductos.PerformClick()
            End If
        Catch ex As Exception

        End Try

        'Select Case e.KeyCode
        '    Case Keys.ControlKey + Keys.NumPad1
        '        reciboconsfinal.PerformClick()
        '    Case Keys.F11
        '        facturabconsfinal.PerformClick()
        '    Case Keys.F12
        '        FacturaA.PerformClick()
        'End Select
    End Sub

    Private Sub frmprincipal_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress

    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed = True Then
            Me.Text = Application.ProductName & " - V" & System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString & " - Usuario: " & DatosAcceso.Cliente
            Me.TopMost = False

            cargar_valores_generales()
        Else
            Me.Text = "V- " & My.Application.Info.Version.Major & "." & My.Application.Info.Version.MajorRevision & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.MinorRevision & " - Usuario: " & DatosAcceso.Cliente
            Me.TopMost = False

            cargar_valores_generales()
        End If
    End Sub
    Private Sub cargar_valores_generales()
        Try
            Dim consulta As New MySql.Data.MySqlClient.MySqlDataAdapter("select * from fact_configuraciones", conexionPrinc)
            Dim tablacl As New DataTable
            Dim infocl() As DataRow
            consulta.Fill(tablacl)
            infocl = tablacl.Select("")
            If InStr(DatosAcceso.Moduloacc, "1") = False Then cmdclientes.Visible = False

            If InStr(DatosAcceso.Moduloacc, "2") = False Then cmdproductos.Visible = False
            If InStr(DatosAcceso.Moduloacc, "2a") = False Then ABMProductosToolStripMenuItem.Visible = False
            If InStr(DatosAcceso.Moduloacc, "2b") = False Then ConsultaDeProductos.Visible = False
            If InStr(DatosAcceso.Moduloacc, "2c") = False Then ManejoDePreciosToolStripMenuItem.Visible = False
            If InStr(DatosAcceso.Moduloacc, "2d") = False Then StockToolStripMenuItem.Visible = False
            If InStr(DatosAcceso.Moduloacc, "AR01") = False Then MostradorToolStripMenuItem.Visible = False
            If InStr(DatosAcceso.Moduloacc, "AR01") = False Then EtiquetasEnBlancoToolStripMenuItem.Visible = False

            If InStr(DatosAcceso.Moduloacc, "3") = False Then cmdfacturacion.Visible = False
            If InStr(DatosAcceso.Moduloacc, "3a") = False Then NuevaEfacturaToolStripMenuItem.Visible = False
            If InStr(DatosAcceso.Moduloacc, "3b") = False Then NuevoPedidoToolStripMenuItem.Visible = False
            If InStr(DatosAcceso.Moduloacc, "3c") = False Then reciboconsfinal.Visible = False
            If InStr(DatosAcceso.Moduloacc, "3d") = False Then facturabconsfinal.Visible = False
            If InStr(DatosAcceso.Moduloacc, "3e") = False Then FacturaA.Visible = False
            If InStr(DatosAcceso.Moduloacc, "3f") = False Then RemitosToolStripMenuItem.Visible = False

            If InStr(DatosAcceso.Moduloacc, "4") = False Then cmdadministracion.Visible = False
            If InStr(DatosAcceso.Moduloacc, "4a") = False Then ContableToolStripMenuItem1.Visible = False
            If InStr(DatosAcceso.Moduloacc, "4ba") = False Then CajaToolStripMenuItem.Visible = False
            If InStr(DatosAcceso.Moduloacc, "4bb") = False Then CajaDiariaToolStripMenuItem.Visible = False
            If InStr(DatosAcceso.Moduloacc, "4c") = False Then ProveedoresToolStripMenuItem.Visible = False
            If InStr(DatosAcceso.Moduloacc, "4d") = False Then AgendaDeVencimientosToolStripMenuItem.Visible = False

            If InStr(DatosAcceso.Moduloacc, "5") = False Then cmdtecnico.Visible = False
            If InStr(DatosAcceso.Moduloacc, "KIBIT") = False Then CLOUDSERVERToolStripMenuItem.Visible = False

            FacturaElectro.puntovtaelect = infocl(0)(2)
            FacturaElectro.cuit = infocl(1)(2)
            FacturaElectro.certificado = infocl(2)(2)
            FacturaElectro.passcertificado = infocl(3)(2)
            FacturaElectro.licencia = infocl(5)(2)

            Dim cons2 As New MySql.Data.MySqlClient.MySqlDataAdapter("select idvendedor, idtecnico, idAlmacen from cm_usuarios where id=" & DatosAcceso.UsuarioINT, conexionPrinc)
            Dim tabla2 As New DataTable
            Dim info2() As DataRow
            cons2.Fill(tabla2)
            info2 = tabla2.Select("")

            DatosAcceso.Vendedor = info2(0)(0)
            DatosAcceso.Tecnico = info2(0)(1)
            DatosAcceso.IdAlmacen = My.Settings.idAlmacen
            DatosAcceso.StockPpref = infocl(4)(2)
            DatosAcceso.idFacRap = My.Settings.idfacRap
            DatosAcceso.IdPtoVtaDef = My.Settings.idPtoVta


            Dim consAFIP As New MySql.Data.MySqlClient.MySqlDataAdapter("select * from cm_archivos order by id asc", conexionPrinc)
            Dim tablaAFIP As New DataTable
            Dim infoAFIP() As DataRow
            consAFIP.Fill(tablaAFIP)
            infoAFIP = tablaAFIP.Select("")

            Try
                If File.Exists(Application.StartupPath & "\" & infoAFIP(0)(1) & ".pfx") Then
                    My.Computer.FileSystem.DeleteFile(Application.StartupPath & "\" & infoAFIP(0)(1) & ".pfx")
                End If
                If File.Exists(Application.StartupPath & "\" & infoAFIP(1)(1) & ".lic") Then
                    My.Computer.FileSystem.DeleteFile(Application.StartupPath & "\" & infoAFIP(1)(1) & ".lic")
                End If
                If File.Exists(Application.StartupPath & "\" & infoAFIP(2)(1) & ".jpg") Then
                    My.Computer.FileSystem.DeleteFile(Application.StartupPath & "\" & infoAFIP(2)(1) & ".jpg")
                End If

                If tablaAFIP.Rows.Count <> 0 Then
                    If Not IO.File.Exists(Application.StartupPath & "\" & infoAFIP(0)(1) & ".pfx") Then
                        If Not IsDBNull(infoAFIP(0)(2)) Then
                            Dim certificado As Byte() = infoAFIP(0)(2)
                            Dim s As IO.FileStream
                            s = IO.File.Open(Application.StartupPath & "\" & infoAFIP(0)(1) & ".pfx", IO.FileMode.Append)
                            s.Write(certificado, 0, certificado.Length)
                            s.Close()
                        End If
                    End If
                    If Not IO.File.Exists(Application.StartupPath & "\" & infoAFIP(1)(1) & ".lic") Then
                        If Not IsDBNull(infoAFIP(1)(2)) Then
                            Dim licencia As Byte() = infoAFIP(1)(2)
                            Dim sL As IO.FileStream
                            sL = IO.File.Open(Application.StartupPath & "\" & infoAFIP(1)(1) & ".lic", IO.FileMode.Append)
                            sL.Write(licencia, 0, licencia.Length)
                            sL.Close()
                        End If
                    End If
                    If Not IO.File.Exists(Application.StartupPath & "\" & infoAFIP(2)(1) & ".jpg") Then
                        If Not IsDBNull(infoAFIP(2)(2)) Then
                            Dim logo As Byte() = infoAFIP(2)(2)
                            Dim sL As IO.FileStream
                            sL = IO.File.Open(Application.StartupPath & "\" & infoAFIP(2)(1) & ".jpg", IO.FileMode.Append)
                            sL.Write(logo, 0, logo.Length)
                            sL.Close()
                        End If
                    End If
                End If
                Reconectar()
                Dim consMONEDA As New MySql.Data.MySqlClient.MySqlDataAdapter("select nombre, cotizacion from fact_moneda where id=2", conexionPrinc)
                Dim tablaMONEDA As New DataTable
                consMONEDA.Fill(tablaMONEDA)
                lblPrincipalDolar.Text = tablaMONEDA.Rows(0).Item(0) & ": " & tablaMONEDA.Rows(0).Item(1)


            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub tmrcomprobarConexion_Tick(sender As Object, e As EventArgs) Handles tmrcomprobarConexion.Tick
        'chequeo cada 1 segundo el estado de conexion
        Reconectar()
        Try

            'If conexionPrinc.State = ConnectionState.Broken Or conexionPrinc.State = ConnectionState.Closed Or conexionPrinc.Database = "" Then
            '    pntitulo.Enabled = False
            '    lbltitulo.Text = Application.ProductName & "(desconectado)"
            'Else
            '    pntitulo.Enabled = True
            '    lbltitulo.Text = Application.ProductName
            'End If
            'compruebo las empresas
            lblstatusServer.Text = "Estado de servidor: " & conexionPrinc.ServerVersion & "-" & My.Settings.servidor & ": " & conexionPrinc.State.ToString
            lblstatusBDprinc.Text = "Base de datos principal: " & conexionPrinc.Database
            'lblstatcodus.Text = "Codigo de usuario: " & codus
            'lblcolaborativocon.Text = "Colaborativo con: " & conexionColab.Database
            'lblStatusEmp.Text = "Empresa Seleccionada: " & conexionEmp.State.ToString & ">>>" & conexionEmp.Database

        Catch ex As Exception
            lblstatusgral.Text = "Atencion: " & ex.Message
        End Try
    End Sub

    Private Sub CerrarConexionToolStripMenuItem_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub ReconectarToolStripMenuItem_Click(sender As Object, e As EventArgs)

        Try
            Reconectar()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Private Sub ToolStripSplitButton1_ButtonClick(sender As Object, e As EventArgs)
        Reconectar()
        lblstatusServer.Text = "Estado de servidor: " & conexionPrinc.State.ToString
        lblstatusBDprinc.Text = "Base de datos principal: " & conexionPrinc.Database
        ' lblStatusEmp.Text = "Empresa Seleccionada: " & conexionEmp.State.ToString & ">>>" & conexionEmp.Database
        'Label2.Text = ""
    End Sub


    Private Sub cmdsalir_Click(sender As Object, e As EventArgs)
        End
    End Sub

    Private Sub CientesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles cmdclientes.Click
        'If InStrRev(DatosAcceso.Moduloacc, 1) = 0 Then
        '    MsgBox("NO esta autorizado")
        '    Exit Sub
        'End If
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "frmaspirantes" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim clientes As New frmaspirantes
        clientes.MdiParent = Me
        clientes.Show()
    End Sub

    Private Sub ProductosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles cmdproductos.Click

    End Sub

    Private Sub NuevaVentaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NuevaVentaToolStripMenuItem.Click
        'If InStrRev(DatosAcceso.Moduloacc, 4) = 0 Then
        '    MsgBox("NO esta autorizado")
        '    Exit Sub
        'End If
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "nuevaventa" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim vta As New nuevaventa
        vta.MdiParent = Me
        vta.Show()
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub ContableToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ContableToolStripMenuItem1.Click
        'If InStrRev(DatosAcceso.Moduloacc, 5) = 0 Then
        '    MsgBox("NO esta autorizado")
        '    Exit Sub
        'End If
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "CONTABLE" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next
        Dim cont As New CONTABLE
        cont.MdiParent = Me
        cont.Show()
    End Sub

    Private Sub ProveedoresToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ProveedoresToolStripMenuItem.Click
        'If InStrRev(DatosAcceso.Moduloacc, 7) = 0 Then
        '    MsgBox("NO esta autorizado")
        '    Exit Sub
        'End If
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "proveedores" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim prov As New proveedores
        prov.MdiParent = Me
        prov.Show()
    End Sub

    Private Sub CajaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CajaToolStripMenuItem.Click
        'If InStrRev(DatosAcceso.Moduloacc, 6) = 0 Then
        '    MsgBox("NO esta autorizado")
        '    Exit Sub
        'End If
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "caja" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim caj As New caja
        caj.MdiParent = Me
        caj.Show()
    End Sub

    Private Sub ProduccionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ProduccionToolStripMenuItem.Click
        'If InStrRev(DatosAcceso.Moduloacc, 8) = 0 Then
        '    MsgBox("NO esta autorizado")
        '    Exit Sub
        'End If
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "produccion" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim tec As New produccion
        tec.MdiParent = Me
        tec.Show()
    End Sub

    Private Sub PedidosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PedidosToolStripMenuItem.Click
        'If InStrRev(DatosAcceso.Moduloacc, 3) = 0 Then
        '    MsgBox("NO esta autorizado")
        '    Exit Sub
        'End If
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "pedidos" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim tec As New Presupuestos
        tec.MdiParent = Me
        tec.Show()
    End Sub




    Private Sub NuevoPedidoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NuevoPedidoToolStripMenuItem.Click
        'If InStrRev(DatosAcceso.Moduloacc, 3) = 0 Then
        '    MsgBox("NO esta autorizado")
        '    Exit Sub
        'End If
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "pedidos" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim tec As New Presupuestos
        tec.MdiParent = Me
        tec.Show()
    End Sub

    Private Sub SincronizacionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SincronizacionToolStripMenuItem.Click
        Try
            SincronizarBD()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        'If InStrRev(DatosAcceso.Moduloacc, 4) = 0 Then
        '    MsgBox("NO esta autorizado")
        '    Exit Sub
        'End If
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "nuevaventa" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim vta As New nuevaventa
        vta.MdiParent = Me
        vta.Show()
    End Sub

    Private Sub FXConsFinalToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FXConsFinalToolStripMenuItem.Click
        'If InStrRev(DatosAcceso.Moduloacc, 4) = 0 Then
        '    MsgBox("NO esta autorizado")
        '    Exit Sub
        'End If
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "nuevaventa" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim vta As New nuevaventa
        vta.MdiParent = Me
        vta.fxINIC = True
        vta.Show()
    End Sub

    Private Sub NuevaVentaToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles NuevaVentaToolStripMenuItem1.Click
        'If InStrRev(DatosAcceso.Moduloacc, 4) = 0 Then
        '    MsgBox("NO esta autorizado")
        '    Exit Sub
        'End If
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "nuevaventa" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim vta As New nuevaventa
        vta.MdiParent = Me
        vta.Show()
    End Sub

    Private Sub NuevaEfacturaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NuevaEfacturaToolStripMenuItem.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "nuevaventa" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim vta As New nuevaventa
        vta.MdiParent = Me
        vta.Show()
    End Sub

    Private Sub AgendaDeVencimientosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AgendaDeVencimientosToolStripMenuItem.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "agendavencimientos" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim vta As New agendavencimientos
        vta.MdiParent = Me
        vta.Show()
    End Sub

    Private Sub reciboconsfinal_Click(sender As Object, e As EventArgs) Handles reciboconsfinal.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "puntoventa" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim vta As New puntoventa
        vta.MdiParent = Me
        vta.idfacrap = My.Settings.idfacRap
        vta.Idcliente = 9999
        vta.cargarCliente()
        vta.txtcodPLU.Focus()
        vta.Show()
    End Sub

    Private Sub facturabconsfinal_Click(sender As Object, e As EventArgs) Handles facturabconsfinal.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "puntoventa" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim vta As New puntoventa
        vta.MdiParent = Me
        vta.idfacrap = 2
        vta.Idcliente = 9999
        vta.cargarCliente()
        vta.cmdguardar.Enabled = False
        vta.cmdsolicitarcae.Enabled = True
        vta.txtcodPLU.Focus()
        vta.Show()
    End Sub

    Private Sub FacturaA_Click(sender As Object, e As EventArgs) Handles FacturaA.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "puntoventa" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim vta As New puntoventa
        vta.MdiParent = Me
        vta.idfacrap = 3
        vta.cmdguardar.Enabled = False
        vta.cmdsolicitarcae.Enabled = True
        vta.txtclierazon.Focus()
        vta.Show()
    End Sub

    Private Sub frmprincipal_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp

    End Sub

    Private Sub TecnicoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles cmdtecnico.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "buscarequipos" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim tec As New buscarequipos
        tec.MdiParent = Me
        'tec.idfacrap = 3
        'tec.cmdguardar.Enabled = False
        'tec.cmdsolicitarcae.Enabled = True
        'tec.txtclierazon.Focus()
        tec.Show()
    End Sub

    Private Sub ABMProductosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ABMProductosToolStripMenuItem.Click
        'If InStrRev(DatosAcceso.Moduloacc, 2) = 0 Then
        '    MsgBox("NO esta autorizado")
        '    Exit Sub
        'End If
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "productos" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim prod As New productos
        prod.MdiParent = Me
        prod.Show()
    End Sub

    Private Sub ConsultaDeProductosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConsultaDeProductos.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "busquedaprod" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next
        Dim prod As New busquedaprod
        prod.MdiParent = Me
        prod.Show()
    End Sub
    Private Sub ManejoDePreciosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ManejoDePreciosToolStripMenuItem.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "manejodeprecios" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim prod As New manejodeprecios
        prod.MdiParent = Me
        prod.Show()
    End Sub
    Private Sub MostradorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MostradorToolStripMenuItem.Click
        imprimirEtiquetas.Show()
    End Sub

    Private Sub StockToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StockToolStripMenuItem.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "manejoStock" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim prod As New manejoStock
        prod.MdiParent = Me
        prod.Show()
    End Sub

    Private Sub ImportarListaDePreciosToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "ImportacionPrecios" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim prod As New ImportacionPrecios
        prod.MdiParent = Me
        prod.Show()
    End Sub

    Private Sub EtiquetasEnBlancoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EtiquetasEnBlancoToolStripMenuItem.Click
        imprimirEtiquetasEnBlanco.Show()
    End Sub

    Private Sub ReimpresiónDeComprobantesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReimpresiónDeComprobantesToolStripMenuItem.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "reimpresionComprobantes" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim prod As New reimpresionComprobantes
        prod.MdiParent = Me
        prod.Show()
    End Sub

    Private Sub CajaDiariaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CajaDiariaToolStripMenuItem.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "CajaDiaria" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim caj As New CajaDiaria
        caj.MdiParent = Me
        caj.Show()
    End Sub

    Private Sub ConfiguracionDeTerminalToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConfiguracionDeTerminalToolStripMenuItem.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "ConfiguracionTerminal" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim caj As New ConfiguracionTerminal
        caj.MdiParent = Me
        caj.Show()
    End Sub

    Private Sub CLOUDSERVERToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CLOUDSERVERToolStripMenuItem.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "frmServiciosCloud" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim caj As New frmServiciosCloud
        caj.MdiParent = Me
        caj.Show()
    End Sub

    Private Sub InformesDeVentasToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InformesDeVentasToolStripMenuItem.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "informedeventas" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim caj As New informedeventas
        caj.MdiParent = Me
        caj.Show()
    End Sub

    Private Sub RemitosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemitosToolStripMenuItem.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "puntoventa" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim vta As New puntoventa
        vta.MdiParent = Me
        vta.idfacrap = 5
        vta.Idcliente = 9999
        vta.cargarCliente()
        vta.txtcodPLU.Focus()
        vta.Show()
    End Sub

    Private Sub ToolStripComboBox1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub lblPrincipalDolar_Click(sender As Object, e As EventArgs) Handles lblPrincipalDolar.Click
        Try
            Dim Cotizacion As String = InputBox("Ingrese nueva cotizacion de dolar", "Cambiar cotizacion")

            If IsNumeric(Cotizacion) Then

                Dim comandoUPD As New MySql.Data.MySqlClient.MySqlCommand("update fact_moneda set cotizacion = '" & Cotizacion.Replace(".", ",") & "' where id=2", conexionPrinc)
                comandoUPD.ExecuteNonQuery()

                lblPrincipalDolar.Text = "DOLAR: " & Cotizacion.Replace(".", ",")
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub lblPrincipalDolar_DoubleClick(sender As Object, e As EventArgs) Handles lblPrincipalDolar.DoubleClick

    End Sub
End Class