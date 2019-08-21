﻿Public Class frmKitLevels
    Private mLocation As Integer
    Public Cancelled As Boolean
    Public AllowPartialKits As Boolean
    Public AllowAdjustedQuantities As Boolean
    Public AllowStatusChange As Boolean
    Public AllowItemStatusChange As Boolean
    Public AllowItemLocChange As Boolean
    Public ItemLoc() As Integer
    Public lblItemNumCount As Integer

    Public Function KitCost(Optional ByVal vType As String = "Landed", Optional ByVal Line As Integer = 0, Optional ByVal Style As String = "") As Decimal
        Dim I As Integer, S As String, C As CInvRec
        For I = 1 To ItemCount
            If (Line = 0 And Style = "") Or Line = I Or Style = ItemStyle(I) Then
                S = ItemStyle(I)
                C = New CInvRec
                If C.Load(S, "Style") Then
                    Select Case UCase(vType)
                        Case "LANDED" : KitCost = KitCost + C.Landed * ItemQuantityByStyle(S)
                        Case "COST" : KitCost = KitCost + C.Cost * ItemQuantityByStyle(S)
                        Case "ONSALE" : KitCost = KitCost + C.OnSale * ItemQuantityByStyle(S)
                        Case "LIST" : KitCost = KitCost + C.List * ItemQuantityByStyle(S)
                        Case Else
                            Err.Raise(-1, "", "Invalid vType in frmKitLevels.KitCost()")
                    End Select
                End If
                DisposeDA(C)
            End If
        Next
    End Function

    Public ReadOnly Property ItemQuantityByStyle(ByVal vStyle As String) As Double
        Get
            Dim I As Integer
            For I = 1 To ItemCount
                If ItemStyle(I) = vStyle Then ItemQuantityByStyle = ItemQuantity(I) : Exit Property
            Next
            ItemQuantityByStyle = Quantity
        End Get
    End Property

    Public ReadOnly Property ItemLocByStyle(ByVal vStyle As String) As Double
        Get
            Dim I As Integer
            For I = 1 To ItemCount
                If ItemStyle(I) = vStyle Then ItemLocByStyle = ItemLoc(I) : Exit Property
            Next
            ItemLocByStyle = Locations   '--> In vb 6, this property name is Location. Changed to Locations because Location is a keyword in vb.net
        End Get
    End Property

    Public ReadOnly Property ItemStatusByStyle(ByVal vStyle As String) As String
        Get
            Dim I As Integer
            For I = 1 To ItemCount
                If ItemStyle(I) = vStyle Then ItemStatusByStyle = ItemStatus(I) : Exit Property
            Next
            ItemStatusByStyle = status
        End Get
    End Property

    Public Sub LoadKit(ByVal vLoc As Integer, ByVal vStat As String, ByVal KitStyle As String, Optional ByVal Quan As Double = 1)
        Dim CI As cInvKit, I As Integer
        On Error Resume Next
        ClearItems()

        Locations = vLoc
        status = vStat

        CI = New cInvKit
        If CI.Load(KitStyle, "KitStyleNo") Then
            lblStyle.Text = KitStyle
            txtKitQuantity.Text = Quan

            For I = 1 To Setup_MaxKitItems
                If CI.Item(I) <> "" Then AddItem(CI.Item(I), Quan * CI.Quantity(I), Locations, CI.Quantity(I))
            Next
        End If
        DisposeDA(CI)
    End Sub

    Public Property status As String
        Get
            status = cmdStatus.Text
            If status = "" Then status = "ST"
        End Get
        Set(value As String)
            cmdStatus.Text = value
        End Set
    End Property

    Public Property Quantity As Double
        Get
            Quantity = Val(txtKitQuantity)
        End Get
        Set(value As Double)
            txtKitQuantity.Text = value
        End Set
    End Property

    Public ReadOnly Property ItemCount() As Integer
        Get
            Dim cmdItemStatusCount As Integer
            For Each ctrl In Me.Controls
                If Mid(ctrl.name, 1, 13) = "cmdItemStatus" Then
                    cmdItemStatusCount = cmdItemStatusCount + 1
                End If
            Next
            'ItemCount = cmdItemStatus.UBound
            ItemCount = cmdItemStatusCount
        End Get
    End Property

    '    Public Property Get ItemStyle(ByVal Index as integer) As String
    '    If Index <= 0 Then Exit Property
    '  If Index > ItemCount Then Exit Property
    '  ItemStyle = lblItem(Index)
    'End Property
    '    Public Property Let ItemStyle(ByVal Index as integer, ByVal vData As String)
    '    If Index <= 0 Then Exit Property
    '  If Index > ItemCount Then Exit Property
    '  lblItem(Index).Caption = vData
    'End Property


    Public ReadOnly Property ItemQuantity(ByVal Index As Integer) As Double
        Get
            If Index < 0 Then Exit Property
            If Index > ItemCount Then Exit Property

            'ItemQuantity = Val(txtItemQuan(Index))
            'ItemQuantity = txtItemQuan & Index & .text
        End Get

    End Property

    Public Property Locations() As Integer
        '--> In vb 6, this property name is Location. Changed to Locations because Location is a keyword in vb.net
        Get
            Locations = mLocation
            If Locations = 0 Then Locations = StoresSld
        End Get
        Set(value As Integer)
            mLocation = value
        End Set
    End Property

    Private Sub ClearItems()
        Dim I As Integer
        lblStyle.Text = ""
        txtKitQuantity.Text = "0"
        'lblItem(1) = ""
        lblItem.Text = ""
        'txtItemQuan(1) = "0"
        txtItemQuan.Text = "0"
        'lblItemLoc(1) = "0"
        lblItemLoc.Text = "0"
        'lblOnOrd(1) = "0"
        lblOnOrd.Text = "0"
        'lblItemAvail(1) = "0"
        lblItemAvail.Text = "0"
        'cmdItemLoc(1).Caption = "L" & Location
        cmdItemLoc.Text = "L" & Locations
        'cmdItemStatus(1).Caption = status
        cmdItemStatus.Text = status

        On Error Resume Next
        '    If lblItemNum.UBound >= 2 Then
        '        For I = lblItemNum.UBound To 2 Step -1
        '            Unload lblItemNum(I)
        '  Unload lblItem(I)
        '  Unload txtItemQuan(I)
        '  Unload lblItemLoc(I)
        '  Unload lblOnOrd(I)
        '  Unload lblItemAvail(I)
        '  Unload cmdItemLoc(I)
        '  Unload cmdItemStatus(I)
        'Next
        '    End If

        If lblItemNumCount >= 2 Then
            For I = lblItemNumCount To 2 Step -1
                Me.Controls.Item(lblItemNum.ToString & I).Hide()
                Me.Controls.Item(lblItem.ToString & I).Hide()
                Me.Controls.Item(txtItemQuan.ToString & I).Hide()
                Me.Controls.Item(lblItemLoc.ToString & I).Hide()
                Me.Controls.Item(lblOnOrd.ToString & I).Hide()
                Me.Controls.Item(lblItemAvail.ToString & I).Hide()
                Me.Controls.Item(cmdItemLoc.ToString & I).Hide()
                Me.Controls.Item(cmdItemStatus.ToString & I).Hide()
            Next
        End If

        'fraItems.Height = cmdItemStatus(1).Top
        fraItems.Height = cmdItemStatus.Top
        fraItems.Visible = False
        fraControls.Top = fraItems.Top + fraItems.Height
        'Height = Height - ScaleHeight + fraControls.Top + fraControls.Height
        Height = Height - Me.ClientSize.Height + fraControls.Top + fraControls.Height
        cmdStatus.Enabled = AllowStatusChange
    End Sub

    Private Sub AddItem(ByVal Style As String, ByRef Q As Double, Optional ByVal vLoc As Integer = 0, Optional ByVal SingleQuantity As Double = 0)
        Dim N As Integer, T As Integer
        Dim A As Double, B As Double, C As Double, D As String, E As Double
        Dim lblItemCount As Integer
        Dim labelItemText As String

        If vLoc = 0 Then vLoc = Locations
        GetItem(vLoc, Style, A, B, C, D, E)

        For Each ctrl As Control In Me.Controls
            If Mid(ctrl.Name, 1, 10) = "lblItemNum" Then
                lblItemNumCount = lblItemNumCount + 1
            End If
        Next

        For Each ctrl As Control In Me.Controls
            Dim lblItemNumber As String
            lblItemNumber = Mid(ctrl.Name, 8, 1)
            If Mid(ctrl.Name, 1, 7) = "lblItem" And IsNumeric(lblItemNumber) Then
                lblItemCount = lblItemCount + 1
            Else
                lblItemCount = 1
            End If

        Next
        'N = lblItemNum.UBound
        N = lblItemNumCount
        'If N > 1 Or lblItem(N) <> "" Then
        'If N > 1 Or lblItem.ToString & N & ".Text" <> "" Then

        If lblItemCount = 1 Then
            labelItemText = lblItem.Text
        Else
            labelItemText = Me.Controls.Item(lblItem.ToString & N).Text
            'Note: If the above line will not work, replace it with below three commented lines to get the text and store it in labelItemText variable.
            'Dim Lc As New Label
            'Lc.Name = "lblItem" & N
            'labelItemText = Lc.Text
        End If

        Dim ctrll As Control
        'If N > 1 Or lblItem.Text <> "" Then
        If N > 1 Or labelItemText <> "" Then
            'T = lblItemNum(N).Top + 240
            T = Me.Controls.Item(lblItemNum.ToString & N).Top + 240
            'Note: If the above line will not work, replace it with the below three commented lines to get top and increment with 240 and assign it to variable T.
            'Dim L As New Label
            'L.Name = "lblItemNum" & N
            'T = L.Top + 240

            N = N + 1
            'Load lblItemNum(N)
            'Me.Controls.Item(lblItemNum.ToString & N).Show()
            'Me.Controls.Item(lblItemNum.ToString & N).Hide()
            'lblItemNum(N).Top = T
            'Me.Controls.Item(lblItemNum.ToString & N).Top = T
            ctrll = New Label
            'ctrll.Name = lblItemNum.ToString & N
            ctrll.Name = "lblItemNum" & N
            ctrll.Top = T
            Me.Controls.Add(ctrll)
            ctrll.Hide()

            'Load lblItem(N)
            'Me.Controls.Item(lblItem.ToString & N).Show()
            'Me.Controls.Item(lblItem.ToString & N).Hide()
            'lblItem(N).Top = T
            'Me.Controls.Item(lblItem.ToString & N).Top = T
            ctrll = New Label
            ctrll.Name = "lblItem" & N
            ctrll.Top = T
            Me.Controls.Add(ctrll)
            ctrll.Hide()

            'Load txtItemQuan(N)
            'Me.Controls.Item(txtItemQuan.ToString & N).Show()
            'Me.Controls.Item(txtItemQuan.ToString & N).Hide()
            'txtItemQuan(N).Top = T
            'Me.Controls.Item(txtItemQuan.ToString & N).Top = T
            ctrll = New TextBox
            ctrll.Name = "txtItemQuan" & N
            ctrll.Top = T
            Me.Controls.Add(ctrll)
            ctrll.Hide()

            'Load lblItemLoc(N)
            'Me.Controls.Item(lblItemLoc.ToString & N).Show()
            'Me.Controls.Item(lblItemLoc.ToString & N).Hide()
            'lblItemLoc(N).Top = T
            'Me.Controls.Item(lblItemLoc.ToString & N).Top = T
            ctrll = New Label
            ctrll.Name = "lblItemLoc" & N
            ctrll.Top = T
            Me.Controls.Add(ctrll)
            ctrll.Hide()

            'Load lblOnOrd(N)
            'Me.Controls.Item(lblOnOrd.ToString & N).Show()
            'Me.Controls.Item(lblOnOrd.ToString & N).Hide()
            'lblOnOrd(N).Top = T
            'Me.Controls.Item(lblOnOrd.ToString & N).Top = T
            ctrll = New Label
            ctrll.Name = "lblOnOrd" & N
            ctrll.Top = T
            Me.Controls.Add(ctrll)
            ctrll.Hide()

            'Load lblItemAvail(N)
            'Me.Controls.Item(lblItemAvail.ToString & N).Show()
            'Me.Controls.Item(lblItemAvail.ToString & N).Hide()
            'lblItemAvail(N).Top = T
            'Me.Controls.Item(lblItemAvail.ToString & N).Top = T
            ctrll = New Label
            ctrll.Name = "lblItemAvail" & N
            ctrll.Top = T
            Me.Controls.Add(ctrll)
            ctrll.Hide()

            'Load cmdItemLoc(N)
            'Me.Controls.Item(cmdItemLoc.ToString & N).Show()
            'Me.Controls.Item(cmdItemLoc.ToString & N).Hide()
            'cmdItemLoc(N).Top = T
            'Me.Controls.Item(cmdItemLoc.ToString & N).Top = T
            ctrll = New Button
            ctrll.Name = "cmdItemLoc" & N
            ctrll.Top = T
            Me.Controls.Add(ctrll)
            ctrll.Hide()

            'Load cmdItemStatus(N)
            'Me.Controls.Item(cmdItemStatus.ToString & N).Show()
            'Me.Controls.Item(cmdItemStatus.ToString & N).Hide()
            'cmdItemStatus(N).Top = T
            'Me.Controls.Item(cmdItemStatus.ToString & N).Top = T
            ctrll = New Button
            ctrll.Name = "cmdItemStatus" & N
            ctrll.Top = T
            Me.Controls.Add(ctrll)
            ctrll.Hide()
        End If

        If N = 1 Then
            lblItemNum.Visible = True
            ToolTip1.SetToolTip(lblItemNum, D)
            lblItem.Visible = True
            lblItem.Text = Style
            ToolTip1.SetToolTip(lblItem, D)

            txtItemQuan.Visible = True
            txtItemQuan.Text = Math.Round(Q, 2)
            txtItemQuan.Tag = SingleQuantity
            txtItemQuan.Enabled = Not AllowAdjustedQuantities   '-> Locked replaced with Enabled. Because Locked propert not available at runtime in vb.net
            If txtItemQuan.Enabled = True Then
                'txtItemQuan.Appearance = 0         Property not available.
                'txtItemQuan.BackColor = &H8000000F   Hexadecimal not accepted in vb.net
                txtItemQuan.BorderStyle = 0
            Else
                'txtItemQuan.Appearance = 1
                'txtItemQuan.BackColor = &H80000005
                txtItemQuan.BorderStyle = 1
            End If

            If ShowST Then
                lblItemLoc.Visible = True
            Else
                lblItemLocCaption.Visible = False
                lblItemLoc.Visible = False
            End If

            lblItemLoc.Text = A
            lblOnOrd.Visible = True
            lblOnOrd.Text = B
            lblOnOrd.Tag = E
            lblItemAvail.Visible = True
            lblItemAvail.Text = C
            cmdItemLoc.Visible = True
            cmdItemLoc.Text = "L" & vLoc                ' original setup doesn't use property...  don't need update call
            cmdItemLoc.Enabled = AllowItemLocChange
            cmdItemStatus.Visible = True
            cmdItemStatus.Text = status
            cmdItemStatus.Enabled = AllowItemStatusChange

        ElseIf N > 1 Then
            '    'lblItemNum(N).Visible = True
            '    'lblItemNum(N) = N
            '    'lblItemNum(N).ToolTipText = D
            Me.Controls.Item(lblItemNum.ToString & N).Visible = True
            ToolTip1.SetToolTip(Me.Controls.Item(lblItemNum.ToString & N), D)
            '    'lblItem(N).Visible = True
            '    'lblItem(N) = Style
            '    'lblItem(N).ToolTipText = D
            Me.Controls.Item(lblItem.ToString & N).Visible = True
            Me.Controls.Item(lblItem.ToString & N).Text = Style
            ToolTip1.SetToolTip(Me.Controls.Item(lblItem.ToString & N), D)
            '    'txtItemQuan(N).Visible = True
            '    'txtItemQuan(N) = Round(Q, 2)
            '    'txtItemQuan(N).Tag = SingleQuantity
            '    'txtItemQuan(N).Locked = Not AllowAdjustedQuantities

            '    'If txtItemQuan(N).Locked Then
            '        'txtItemQuan(N).Appearance = 0   --> This is only for show some 3-d effects at runtime.
            '        'txtItemQuan(N).BackColor = &H8000000F
            '        'txtItemQuan(N).BorderStyle = 0   --> This property at runtime is not available. Only available at designtime.
            '    Else
            '        'txtItemQuan(N).Appearance = 1            --> appearance property not avaible for textbox 
            '        'txtItemQuan(N).BackColor = &H80000005    --> backcolor hexadecimal value not accepted in vb.net
            '        'txtItemQuan(N).BorderStyle = 1           --> borderstyle not available at runtime.    
            '    End If
            Me.Controls.Item(txtItemQuan.ToString & N).Visible = True
            Me.Controls.Item(txtItemQuan.ToString & N).Text = Math.Round(Q, 2)
            Me.Controls.Item(txtItemQuan.ToString & N).Tag = SingleQuantity
            Me.Controls.Item(txtItemQuan.ToString & N).Enabled = Not AllowAdjustedQuantities   '-> replaced Locked with Enabled. Locked will not available at runtime in vb.net
            If Me.Controls.Item(txtItemQuan.ToString & N).Enabled = True Then
                'txtItemQuan(N).Appearance = 0   --> This is only for show some 3-d effects at runtime.
                Me.Controls.Item(txtItemQuan.ToString & N).BackColor = Color.White  '->Hexadecimal is not accepted in me.controls.item style.
                ''txtItemQuan(N).BorderStyle = 0   --> Border style is available if it is a direct textbox control. For me.controls.item(txtItemQuan) style
                '--> Border style is not available. To get the borderstyle, instead of me.controls.item, use the below commented code.
                'Dim Tc As New TextBox
                'Tc.Name = "txtItemQuan" & N
                'Tc.BackColor = Color.White  '-> Hexadecimal is not accepted for backcolor.
                'Tc.BorderStyle = 0
                'Me.Controls.Add(Tc)
            Else
                'Appearance property is not available in vb.net
                'BorderStyle will not available using me.controls.item style. For this use Dim Tc as New TextBox style.
                Me.Controls.Item(txtItemQuan.ToString & N).BackColor = Color.White  '->Hexadecimal is not accepted in me.controls.item style.
            End If

            If ShowST Then
                '        'lblItemLoc(N).Visible = True
                Me.Controls.Item(lblItemLoc.ToString & N).Visible = True
            Else
                lblItemLocCaption.Visible = False
                '        'lblItemLoc(N).Visible = False
                Me.Controls.Item(lblItemLoc.ToString & N).Visible = False
            End If
            '    'lblItemLoc(N) = A
            Me.Controls.Item(lblItemLoc.ToString & N).Text = A
            '    'lblOnOrd(N).Visible = True
            '    'lblOnOrd(N) = B
            '    'lblOnOrd(N).Tag = E
            Me.Controls.Item(lblOnOrd.ToString & N).Visible = True
            Me.Controls.Item(lblOnOrd.ToString & N).Text = B
            Me.Controls.Item(lblOnOrd.ToString & B).Tag = E
            '    'lblItemAvail(N).Visible = True
            '    'lblItemAvail(N) = C
            Me.Controls.Item(lblItemAvail.ToString & N).Visible = True
            Me.Controls.Item(lblItemAvail.ToString & N).Text = C
            '    'cmdItemLoc(N).Visible = True
            '    'cmdItemLoc(N).Caption = "L" & vLoc                ' original setup doesn't use property...  don't need update call
            '    'cmdItemLoc(N).Enabled = AllowItemLocChange
            Me.Controls.Item(cmdItemLoc.ToString & N).Visible = True
            Me.Controls.Item(cmdItemLoc.ToString & N).Text = "L" & vLoc
            Me.Controls.Item(cmdItemLoc.ToString & N).Enabled = AllowItemLocChange
            '    'cmdItemStatus(N).Visible = True
            '    'cmdItemStatus(N).Caption = status
            '    'cmdItemStatus(N).Enabled = AllowItemStatusChange
            Me.Controls.Item(cmdItemStatus.ToString & N).Visible = True
            Me.Controls.Item(cmdItemStatus.ToString & N).Text = status
            Me.Controls.Item(cmdItemStatus.ToString & N).Enabled = AllowItemStatusChange
        End If

        HiLiteKitRow(N)

        'fraItems.Height = cmdItemStatus(N).Top + cmdItemStatus(N).Height + 60
        fraItems.Height = Me.Controls.Item(cmdItemStatus.ToString & N).Top + Me.Controls.Item(cmdItemStatus.ToString & N).Height + 60
        fraItems.Visible = True
        fraControls.Top = fraItems.Top + fraItems.Height
        'Height = Height - ScaleHeight + fraControls.Top + fraControls.Height + 120
        Height = Height - Me.ClientSize.Height + fraControls.Top + fraControls.Height + 120
    End Sub

    Private Sub GetItem(ByVal vLoc As Integer, ByVal Style As String, ByRef Loc As Double, ByRef OnOrd As Double, ByRef Avl As Double, ByRef Dsc As String, ByRef PreSold As Double)
        Dim cInv As CInvRec
        cInv = New CInvRec
        If cInv.Load(Style, "Style") Then
            Loc = cInv.QueryStock(vLoc)
            OnOrd = cInv.QueryOnOrder(vLoc)
            Avl = cInv.Available
            Dsc = cInv.Desc
            PreSold = cInv.PoSold
        End If
        DisposeDA(cInv)
    End Sub

    Public ReadOnly Property ShowST() As Boolean
        Get
            ShowST = False
        End Get
    End Property

    Private Sub HiLiteKitRow(ByVal Index As Integer)
        'Private Sub HiLiteKitRow(ByVal Currentobj As Object)
        Dim hlkrNormal As Integer, hlkrPink As Integer, hlkrCyan As Integer
        hlkrNormal = -2147483633
        hlkrPink = RGB(255, 200, 200)
        hlkrCyan = RGB(128, 255, 255)

        On Error Resume Next
        If ItemStatus(Index) = "PO" Then
            'txtItemQuan(Index).BackColor = IIf(LineOverSold(Index), hlkrCyan, hlkrNormal)
            If Index = 1 Then
                txtItemQuan.BackColor = IIf(LineOverSold(Index), hlkrCyan, hlkrNormal)
            ElseIf Index > 1 Then
                Me.Controls.Item(txtItemQuan.ToString & Index).BackColor = IIf(LineOverSold(Index), hlkrCyan, hlkrNormal)
            End If

        Else
            'txtItemQuan(Index).BackColor = IIf(LineOverSold(Index), hlkrPink, hlkrNormal)
            If Index = 1 Then
                txtItemQuan.BackColor = IIf(LineOverSold(Index), hlkrCyan, hlkrNormal)
            ElseIf Index > 1 Then
                Me.Controls.Item(txtItemQuan.ToString & Index).BackColor = IIf(LineOverSold(Index), hlkrCyan, hlkrNormal)
            End If

        End If
        '  lblItemAvail(N).BackColor = txtItemQuan(N).BackColor
    End Sub

    Public Function ItemStatus(ByVal Index As Integer, Optional ByVal Vdata As String = "") As String
        If Vdata = "" Then
            'Get property of vb6.0
            If Index <= 0 Then Exit Function
            If Index > ItemCount Then Exit Function
            'ItemStatus = cmdItemStatus(Index).Caption
            If Index = 1 Then
                ItemStatus = cmdItemStatus.Text
            Else
                ItemStatus = Me.Controls.Item(cmdItemStatus.ToString & Index).Text
            End If

        Else
            'Let property of vb6.0
            If Index <= 0 Then Exit Function
            'If Index > cmdItemStatus.UBound Then Exit Property
            If Index > ItemCount Then Exit Function
            'cmdItemStatus(Index).Caption = Vdata
            If Index = 1 Then
                cmdItemStatus.Text = Vdata
            Else
                Me.Controls.Item(cmdItemStatus.ToString & Index).Text = Vdata
            End If

        End If
    End Function

    Public Function ItemStyle(ByVal Index As Integer, Optional ByVal Vdata As String = "") As String
        If Vdata = "" Then
            'Get property of vb6.0
            If Index <= 0 Then Exit Function
            If Index > ItemCount Then Exit Function
            'ItemStyle = lblItem(Index)
            If Index = 1 Then
                ItemStyle = lblItem.Text
            Else
                ItemStyle = Me.Controls.Item(lblItem.ToString & Index).Text
            End If

        Else
            'Let property of vb6.0
            If Index <= 0 Then Exit Function
            If Index > ItemCount Then Exit Function
            'lblItem(Index).Caption = Vdata
            If Index = 1 Then
                lblItem.Text = Vdata
            Else
                Me.Controls.Item(lblItem.ToString & Index).Text = Vdata
            End If
        End If
    End Function

    Private Function LineOverSold(ByVal I As Integer) As Boolean
        'BFH20120726
        '  If ItemStatus(I) = "ST" And Val(txtItemQuan(I)) > Val(lblItemAvail(I)) Then LineOverSold = True
        'If ItemStatus(I) = "ST" And Val(txtItemQuan(I)) + BillOSale.ItemsSoldOnSale(lblItem(I)) > Val(lblItemLoc(I)) Then LineOverSold = True
        If I = 1 Then
            If ItemStatus(I) = "ST" And Val(txtItemQuan.Text) + BillOSale.ItemsSoldOnSale(lblItem.Text) > Val(lblItemLoc.Text) Then
                LineOverSold = True
            End If
        ElseIf I > 1 Then
            If ItemStatus(I) = "ST" And Val(Me.Controls.Item(txtItemQuan.ToString & I).Text) + BillOSale.ItemsSoldOnSale(Me.Controls.Item(lblItem.ToString & I).Text) > Val(Me.Controls.Item(lblItemLoc.ToString & I).Text) Then
                LineOverSold = True
            End If
        End If

        If I = 1 Then
            'If ItemStatus(I) = "PO" And Val(txtItemQuan(I)) > Val(lblOnOrd(I)) - Val(lblOnOrd(I).Tag) Then LineOverSold = True
            If ItemStatus(I) = "PO" And Val(txtItemQuan.Text) > Val(lblOnOrd.Text) - Val(lblOnOrd.Tag) Then
                LineOverSold = True
            End If
        ElseIf I > 1 Then
            If ItemStatus(I) = "PO" And Val(Me.Controls.Item(txtItemQuan.ToString & I).Text) > Val(Me.Controls.Item(lblOnOrd.ToString & I).Text) - Val(Me.Controls.Item(lblOnOrd.ToString & I).Tag) Then
                LineOverSold = True
            End If
        End If
    End Function

    Private Sub frmKitLevels_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetButtonImage(cmdOK)
        SetButtonImage(cmdCancel)
        'SetCustomFrame(Me, ncBasicTool)  -> This is not required. This is for changing U.I. (font and color) using modNeoCaption module.

        Cancelled = False

        mLocation = 0
        AllowStatusChange = True
        AllowItemStatusChange = False
        AllowItemLocChange = True

        AllowPartialKits = False
        AllowAdjustedQuantities = True ' IsDevelopment

        ClearItems()
    End Sub

    Private Sub txtItemQuan_TextChanged(sender As Object, e As EventArgs) Handles txtItemQuan.TextChanged
        HiLiteKitRow(1)
    End Sub
End Class
