﻿Public Class Presupuestos

    Private Sub cmdnuevapers_Click(sender As Object, e As EventArgs) Handles cmdnuevapers.Click
        Dim i As Integer
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "nuevopedido" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        Dim tec As New nuevopedido
        tec.MdiParent = Me.MdiParent
        tec.Show()
    End Sub

    Private Sub pedidos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cargarDatosGrales()
        cargarPedidos()
    End Sub
    Private Sub cargarDatosGrales()
        Try

            Dim tablavend As New MySql.Data.MySqlClient.MySqlDataAdapter("select id, concat(apellido,', ', nombre) from fact_vendedor where activo =1", conexionPrinc)
            Dim readvend As New DataSet
            tablavend.Fill(readvend)
            cmbvendedor.DataSource = readvend.Tables(0)
            cmbvendedor.DisplayMember = readvend.Tables(0).Columns(1).Caption.ToString.ToUpper
            cmbvendedor.ValueMember = readvend.Tables(0).Columns(0).Caption.ToString
            cmbvendedor.SelectedIndex = -1

            If InStrRev(DatosAcceso.Moduloacc, "AR01") <> 0 Then
                cmdnvalistacarga.Visible = True
                cmdlistacarga.Visible = True
            End If
            dtpedidos.Columns(9).DefaultCellStyle.WrapMode = DataGridViewTriState.True
            dtpedidos.Columns(9).DefaultCellStyle.BackColor = Color.FromArgb(255, 128, 0)
            dtpedidos.Columns(8).DefaultCellStyle.WrapMode = DataGridViewTriState.True

        Catch ex As Exception
        End Try
    End Sub
    Public Sub cargarPedidos()
        Try
            Dim ConsultaEXT As String
            Dim desde As String = Format(CDate(dtpdesde.Value), "yyyy-MM-dd")
            Dim hasta As String = Format(CDate(dtphasta.Value), "yyyy-MM-dd")
            If cmbvendedor.SelectedIndex <> -1 Then
                ConsultaEXT &= " and vendedor= " & cmbvendedor.SelectedValue
            End If
            If txtbusqCliente.Text <> "" Then
                ConsultaEXT &= " and razon like '%" & txtbusqCliente.Text & "%'"
            End If
            If rdpendientes.Checked = True Then
                ConsultaEXT &= " and observaciones like 'PENDIENTE'"
            End If
            If rdfacturados.Checked = True Then
                ConsultaEXT &= " and observaciones like 'FACTURADO'"
            End If
            If rdImportar.Checked = True Then
                ConsultaEXT &= " and observaciones <> 'FACTURADO' and observaciones <> 'PENDIENTE'"
            End If

            Reconectar()
            Dim consulta As New MySql.Data.MySqlClient.MySqlDataAdapter("select fac.id, 
            concat(lpad(fac.ptovta,4,'0'),'-',lpad(fac.num_fact,8,'0')) as pedidonum,fac.fecha,
            (select concat(idclientes,'(',nomapell_razon,')') from fact_clientes where idclientes= fac.id_cliente) as razon, 
            (select direccion from fact_clientes where idclientes= fac.id_cliente) as direccion, 
            (select nombre from cm_localidad as loca, fact_clientes as cl where id=cl.dir_localidad and cl.idclientes=fac.id_cliente ) as localidad, 
            con.condicion, format(fac.total,2,'es_AR'), fac.observaciones as estado, fac.observaciones2 as observaciones 
            from fact_facturas as fac, fact_condventas as con where con.id=fac.condvta and fac.tipofact=995 " & ConsultaEXT &
            " and fac.fecha between '" & desde & "' and '" & hasta & "' order by fac.id desc", conexionPrinc)
            Dim tablaped As New DataTable
            consulta.Fill(tablaped)

            dtpedidos.DataSource = tablaped
            dtpedidos.Columns(0).Visible = False
        Catch ex As Exception

        End Try
    End Sub

    Private Sub cmdmodificar_Click(sender As Object, e As EventArgs) Handles cmdmodificar.Click
        Try
            Dim i As Integer
            For i = 0 To Me.MdiChildren.Length - 1
                If MdiChildren(i).Name = "nuevopedido" Then
                    MsgBox("Existe un pedido en curso, cierre primero el pedido en curso e intente nuevamente", vbCritical)
                    Me.MdiChildren(i).BringToFront()
                    Exit Sub
                End If
            Next

            Dim tec As New nuevopedido
            tec.MdiParent = Me.MdiParent
            tec.modificar = True
            tec.idFactura = dtpedidos.CurrentRow.Cells(0).Value
            tec.Show()
            'For Each fila As DataGridViewRow In dtpedidos.Rows
            '    Dim tec As New nuevopedido
            '    tec.MdiParent = Me.MdiParent
            '    tec.modificar = True
            '    tec.idFactura = dtpedidos.CurrentRow.Cells(0).Value
            '    tec.Show()
            '    'System.Threading.Thread.Sleep(5000)
            '    tec.cmdguardar.PerformClick()
            '    System.Threading.Thread.Sleep(500)
            '    tec.Close()
            'Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub cmdsalir_Click(sender As Object, e As EventArgs) Handles cmdsalir.Click
        Me.Close()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        cargarPedidos()

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles cmdnvalistacarga.Click
        Try
            Try
                Dim i As Integer
                For i = 0 To Me.MdiChildren.Length - 1
                    If MdiChildren(i).Name = "nuevalistadecarga" Then
                        MsgBox("Existe un pedido en curso, cierre primero el pedido en curso e intente nuevamente", vbCritical)
                        Me.MdiChildren(i).BringToFront()
                        Exit Sub
                    End If
                Next

                Dim tec As New nuevalistadecarga
                tec.MdiParent = Me.MdiParent
                tec.Show()
            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try
    End Sub

    Private Sub cmdeliminar_Click(sender As Object, e As EventArgs) Handles cmdeliminar.Click
        Try
            If MsgBox("Esta seguro que desa eliminar este pedido?", vbQuestion + vbYesNo) = vbNo Then
                Exit Sub
            End If
            Dim idped As Integer = dtpedidos.CurrentRow.Cells(0).Value
            Dim delItm As String = "delete from fact_items where id_fact=" & idped
            Dim delPed As String = "delete from fact_facturas where id=" & idped
            Reconectar()
            Dim comandoDelItm As New MySql.Data.MySqlClient.MySqlCommand(delItm, conexionPrinc)
            comandoDelItm.ExecuteNonQuery()

            Reconectar()
            Dim comandoDelPed As New MySql.Data.MySqlClient.MySqlCommand(delPed, conexionPrinc)
            comandoDelPed.ExecuteNonQuery()

            cargarPedidos()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub txtbusqCliente_KeyUp(sender As Object, e As KeyEventArgs) Handles txtbusqCliente.KeyUp

        If e.KeyCode = Keys.Enter Then
            cargarPedidos()
        End If
    End Sub

    Private Sub txtbusqCliente_TextChanged(sender As Object, e As EventArgs) Handles txtbusqCliente.TextChanged

    End Sub

    Private Sub cmdlistacarga_Click(sender As Object, e As EventArgs) Handles cmdlistacarga.Click
        Try
            selListaCarga.Show()
            'selListaCarga.TopMost = True
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        Dim EnProgreso As New Form
        Try
            EnProgreso.ControlBox = False
            EnProgreso.FormBorderStyle = Windows.Forms.FormBorderStyle.Fixed3D
            EnProgreso.Size = New Point(430, 30)
            EnProgreso.StartPosition = FormStartPosition.CenterScreen
            EnProgreso.TopMost = True
            Dim Etiqueta As New Label
            Etiqueta.AutoSize = True
            Etiqueta.Text = "La consulta esta en progreso, esto puede tardar unos momentos, por favor espere ..."
            Etiqueta.Location = New Point(5, 5)
            EnProgreso.Controls.Add(Etiqueta)

            EnProgreso.Show()
            Application.DoEvents()

            Dim tabFac As New MySql.Data.MySqlClient.MySqlDataAdapter
            Dim tabEmp As New MySql.Data.MySqlClient.MySqlDataAdapter
            Dim fac As New datosfacturas

            Dim desde As String = Format(CDate(dtpdesde.Value), "yyyy-MM-dd")
            Dim hasta As String = Format(CDate(dtphasta.Value), "yyyy-MM-dd")
            Dim parambusq As String = ""
            Dim tipopedTEXT As String = ""
            Dim vendedor As String
            If cmbvendedor.SelectedValue = 0 Then
                vendedor = "TODOS"
                parambusq = " and fac.vendedor like '%'"
            Else
                vendedor = cmbvendedor.Text
                parambusq = " and fac.vendedor = " & cmbvendedor.SelectedValue
            End If

            If rdfacturados.Checked = True Then
                tipopedTEXT = "Tipo de pedido: FACTURADOS"
            End If
            If rdpendientes.Checked = True Then
                tipopedTEXT = "Tipo de pedido: PENDIENTES"
            End If
            If rdImportar.Checked = True Then
                MsgBox("no se puede imprimir lista de pedidos a importar")
                Exit Sub
            End If

            'MsgBox(vendedor)
            Reconectar()
            tabEmp.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("SELECT  " _
            & "emp.nombrefantasia as empnombre,emp.razonsocial as emprazon,emp.direccion as empdire, emp.localidad as emploca, " _
            & "emp.cuit as empcuit, emp.ingbrutos as empib, emp.ivatipo as empcontr,emp.inicioact as empinicioact, emp.drei as empdrei,emp.logo as emplogo " _
            & "FROM fact_empresa as emp where emp.id=1", conexionPrinc)

            tabEmp.Fill(fac.Tables("membreteenca"))
            Reconectar()
            tabFac.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand("SELECT 
            fac.id, concat(fis.abrev,' ',lpad(fac.ptovta,4,'0'),'-',lpad(fac.num_fact,8,'0')) as factnum ,fac.fecha,fac.razon,fac.direccion, 
            fac.localidad, con.condicion,
            case when fis.debcred='C' then 
            concat('-',format(fac.total,2,'es_AR')) 
            else format(fac.total, 2,'es_AR') end as total, 
            fac.observaciones from fact_conffiscal As fis, fact_facturas As fac, fact_condventas as con 
            where fis.donfdesc = fac.tipofact And con.id = fac.condvta And fis.ptovta = fac.ptovta 
            And fac.tipofact=995 
            And fac.fecha between '" & desde & "' and '" & hasta & "' " & parambusq, conexionPrinc)
            MsgBox(tabFac.SelectCommand.CommandText)
            tabFac.Fill(fac.Tables("listadofacturas"))
            Dim imprimirx As New imprimirFX
            Dim parameters As New List(Of Microsoft.Reporting.WinForms.ReportParameter)()
            parameters.Add(New Microsoft.Reporting.WinForms.ReportParameter("vendedor", vendedor))
            parameters.Add(New Microsoft.Reporting.WinForms.ReportParameter("fecha", "DESDE: " & desde & " HASTA " & hasta))
            parameters.Add(New Microsoft.Reporting.WinForms.ReportParameter("tipopedido", tipopedTEXT))
            parameters.Add(New Microsoft.Reporting.WinForms.ReportParameter("tipodeinforme", "LISTADO DE PEDIDOS"))
            With imprimirx
                .MdiParent = Me.MdiParent
                .rptfx.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local
                .rptfx.LocalReport.ReportPath = System.Environment.CurrentDirectory & "\reportes\listadofacturas.rdlc"
                .rptfx.LocalReport.DataSources.Clear()
                .rptfx.LocalReport.DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("membreteenca", fac.Tables("membreteenca")))
                .rptfx.LocalReport.DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("items", fac.Tables("listadofacturas")))
                .rptfx.LocalReport.SetParameters(parameters)
                .rptfx.DocumentMapCollapsed = True
                .rptfx.RefreshReport()
                .Show()
            End With
            EnProgreso.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub rdImportar_CheckedChanged(sender As Object, e As EventArgs) Handles rdImportar.CheckedChanged
        Try
            If rdImportar.Checked = True Then
                cmdmodificar.Text = "Importar"
                dtpedidos.Columns(8).DefaultCellStyle.BackColor = Color.FromArgb(255, 128, 0)
                dtpedidos.Columns(9).DefaultCellStyle.BackColor = Color.White
            Else
                cmdmodificar.Text = "Modificar"
                dtpedidos.Columns(9).DefaultCellStyle.BackColor = Color.FromArgb(255, 128, 0)
                dtpedidos.Columns(8).DefaultCellStyle.BackColor = Color.White
            End If
        Catch ex As Exception
        End Try
        cargarPedidos()
    End Sub

    Private Sub rdfacturados_CheckedChanged(sender As Object, e As EventArgs) Handles rdfacturados.CheckedChanged
        cargarPedidos()
    End Sub

    Private Sub rdpendientes_CheckedChanged(sender As Object, e As EventArgs) Handles rdpendientes.CheckedChanged
        cargarPedidos()
    End Sub

    Private Function ObtenerIdCliente(ClienteTexto As String) As Integer
        Dim idCliente As Integer = Mid(ClienteTexto, 1, InStr(ClienteTexto, "(") - 1)
        Return idCliente
    End Function

    Private Function CantFilasSel() As Integer
        Dim filasSel As Integer
        For Each filas As DataGridViewRow In dtpedidos.Rows
            If filas.Selected = True Then
                filasSel += 1
            End If
        Next
        Return filasSel
    End Function

    Private Function MismoVendedor() As Boolean
        Try
            Dim tabFac As New MySql.Data.MySqlClient.MySqlDataAdapter
            Dim fac As DataTable
            Dim pedidosList As String = ""
            For Each pedido As DataGridViewRow In dtpedidos.Rows
                If pedido.Selected = True And pedidosList = "" Then
                    pedidosList = pedido.Cells(0).Value
                ElseIf pedido.Selected = True And pedidosList <> "" Then
                    pedidosList &= "," & pedido.Cells(0).Value
                End If
            Next
            Dim consulta As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT distinct(vendedor) FROM fact_facturas where id in (" & pedidosList & ")", conexionPrinc)
            Dim tablaped As New DataTable
            consulta.Fill(tablaped)
            'MsgBox(consulta.SelectCommand.CommandText)
            If tablaped.Rows.Count > 1 Then
                Return False
            ElseIf tablaped.Rows.Count = 1 Then
                Return True
            End If
        Catch ex As Exception

        End Try
    End Function

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If dtpedidos.CurrentRow.Cells(8).Value = "FACTURADO" Then
            MsgBox("El pedido ya fue facturado")
            Exit Sub
        End If

        If MismoVendedor() = False Then
            MsgBox("los pedidos deben ser del mismo vendedor")
            Exit Sub
        End If

        'MsgBox(ObtenerIdCliente(dtpedidos.CurrentRow.Cells(3).Value))
        Dim i As Integer
        Dim FilasSelecc As Integer = CantFilasSel()
        For i = 0 To Me.MdiChildren.Length - 1
            If MdiChildren(i).Name = "puntoventa" Then
                Me.MdiChildren(i).BringToFront()
                Exit Sub
            End If
        Next

        'MsgBox(filasSel)
        Dim vta As New puntoventa
        vta.MdiParent = Me.MdiParent
        vta.idfacrap = My.Settings.idfacRap
        If FilasSelecc > 1 Then
            Dim itemPedido As String
            Dim ptovtapedido As String
            vta.Idcliente = 9999
            For Each filas As DataGridViewRow In dtpedidos.Rows
                If filas.Selected = True Then
                    itemPedido = CInt(Strings.Right(filas.Cells(1).Value, 8))
                    ptovtapedido = CInt(Strings.Left(filas.Cells(1).Value, 4))
                    vta.dtpedidosfact.Rows.Add(filas.Cells(0).Value, ptovtapedido & "-" & itemPedido)
                    vta.CargarPedidoRemoto(itemPedido, ptovtapedido)
                End If
            Next
            vta.Show()

        ElseIf CantFilasSel() = 1 Then
            Dim ptovtapedido As String = CInt(Strings.Left(dtpedidos.CurrentRow.Cells(1).Value, 4))
            Dim itemPedido As String = CInt(Strings.Right(dtpedidos.CurrentRow.Cells(1).Value, 8))
            vta.Idcliente = ObtenerIdCliente(dtpedidos.CurrentRow.Cells(3).Value)
            vta.cargarCliente()
            vta.txtcodPLU.Focus()
            vta.dtpedidosfact.Rows.Add(dtpedidos.CurrentRow.Cells(0).Value, ptovtapedido & "-" & itemPedido)
            vta.CargarPedidoRemoto(itemPedido, ptovtapedido)
            vta.Show()
        End If
    End Sub

    Private Sub cmbvendedor_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cmbvendedor.SelectionChangeCommitted
        cargarPedidos()
    End Sub

    Private Sub Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Panel2.Paint

    End Sub
End Class