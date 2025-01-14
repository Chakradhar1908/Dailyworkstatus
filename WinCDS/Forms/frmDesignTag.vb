﻿Imports stdole
Public Class frmDesignTag
    Private mIsExternal As Boolean
    Private mLoadedForDisplay As Boolean
    Private Const Form_MinWidth As Integer = 12000
    Private Const Form_MinHeight As Integer = 8175

    Private Const TwipsPerInch As Integer = 1440
    Private Const CurrentTagLayoutVersion As String = "1.2"
    Private Const DefaultFields As Integer = 12

    Private Const SnapIncr As Integer = 120
    Private Const FontIncr As Double = 0.5

    Private Const TAGDIM_MAX_X As Single = 11 '8.5    ' 11 b/c of landscape
    Private Const TAGDIM_MAX_Y As Single = 11

    Private Const TAGDIM_L_X As Single = 8.5
    Private Const TAGDIM_L_Y As Single = 11
    Private Const TAGDIM_M_X As Single = 3
    Private Const TAGDIM_M_Y As Single = 4
    Private Const TAGDIM_S_X As Single = 1
    Private Const TAGDIM_S_Y As Single = 2

    Private Const TAGDIM_C_X As Single = 3    ' these are the initial defaults for custom
    Private Const TAGDIM_C_Y As Single = 3    ' which of course can be adjusted then

    Private OrigPrinter As String
    Private PrintingStyle As String
    Private RN As Integer

    Private lstItemsAdjust As Boolean
    Private mAllowPrintMany As Boolean

    Private OrigX As Single, OrigY As Single
    Private inX As Single, inY As Single
    Private Moving As Boolean

    Private CurrentField As Integer
    Private Fields() As TagItemLayout

    Private Enum LayoutAlign
        lyaPosition = 0
        lyaLeft = 1
        lyaCenter = 2
        lyaRight = 3
        lyaPositionR = 4
    End Enum

    Private Structure TagItemLayout
        Dim Name As String
        Dim Caption As String
        Dim Visible As Boolean
        Dim ToolTipText As String

        Dim FontName As String
        Dim FontSize As Single
        Dim FontColor As String
        Dim CharSpecify As String

        Dim Left As Integer
        Dim Top As Integer
        Dim Alignment As LayoutAlign

        Dim ExtraFieldType As Integer
        Dim PicWidth As Integer
        Dim PicHeight As Integer
        Dim PicLock As Boolean
    End Structure

    Public Function PrintCustomTags(ByVal Style As String, Optional ByVal Quantity As Integer = 1, Optional ByVal TemplateName As String = "", Optional ByVal External As Boolean = True) As Boolean
        Dim I As Integer
        TagLog("PrintCustomTags Style=" & Style & ", Qty=" & Quantity & ", Template=" & TemplateName)
        If Quantity < 0 Then Exit Function
        If External And TemplateName = "" Then Exit Function
        mIsExternal = External
        If TemplateName <> "" Then LoadTagLayout(TemplateName)
        PrintingStyle = Style
        RefreshItemCaptions(Style)

        For I = 1 To Quantity
            PrintCurrentTag(cmbPageAlign.SelectedIndex, False, IIf(Quantity = 1, -1, I - 1)) ' for 1 tag, print it w/ alignment options.  Multiple tags go where they specified.
        Next
        Printer.EndDoc()  ' an extra one, because it never hurts
        PrintingStyle = ""
        PrintCustomTags = True
    End Function

    Private ReadOnly Property MaxField() As Integer
        Get
            On Error Resume Next
            MaxField = UBound(Fields)
        End Get
    End Property

    Private Function IsCharBold(ByVal Specify As String) As Boolean
        IsCharBold = InStr(Specify, "B") > 0
    End Function

    Private Function IsCharItalic(ByVal Specify As String) As Boolean
        IsCharItalic = InStr(Specify, "I") > 0
    End Function

    Private Function IsCharUnderline(ByVal Specify As String) As Boolean
        IsCharUnderline = InStr(Specify, "U") > 0
    End Function

    Private Sub PrintCurrentTag(Optional ByVal PageAlign As Integer = 0, Optional ByVal WithBox As Boolean = True, Optional ByVal PageLoc As Integer = -1)
        Dim I As Integer
        Dim XOff As Integer, YOff As Integer, EoP As Boolean
        Dim X As Integer, Y As Integer
        Dim W As Integer, H As Integer
        Dim C As String

        PreparePrinter()

        ' ----- logging
        TagLog("PrintCurrentTag CurrentPrinter=" & Printer.DeviceName & ", Orientation=" & Printer.Orientation & ", ScaleMode=" & Printer.ScaleMode)
        TagLog("PrintCurrentTag PrinterDimensions=" & Printer.Width & "x" & Printer.Height & ", Scale=" & Printer.ScaleWidth & "x" & Printer.ScaleHeight)
        'Printer.ScaleMode = vbInches ' show in inches
        Printer.ScaleMode = VBRUN.ScaleModeConstants.vbInches
        TagLog("PrintCurrentTag PrinterDimensions=" & Printer.Width & """x" & Printer.Height & """, Scale=" & Printer.ScaleWidth & """x" & Printer.ScaleHeight & """")
        'Printer.ScaleMode = vbTwips
        Printer.ScaleMode = VBRUN.ScaleModeConstants.vbTwips
        ' -----

        GetPageAlignOffsets(PageAlign, PageLoc, XOff, YOff, EoP)
        On Error Resume Next
        'If WithBox Then Printer.Line(XOff, YOff)-(XOff + TagWidth, YOff + TagHeight), , B
        If WithBox Then Printer.Line(XOff, YOff, XOff + TagWidth, YOff + TagHeight, , True)

        'For I = MaxField To 1 Step -1
        For I = MaxField To 0 Step -1
            If Fields(I).Visible Then
                Printer.FontName = Fields(I).FontName
                Printer.FontSize = Fields(I).FontSize
                Printer.FontBold = IsCharBold(Fields(I).CharSpecify)
                Printer.FontItalic = IsCharItalic(Fields(I).CharSpecify)
                Printer.FontUnderline = IsCharUnderline(Fields(I).CharSpecify)
                Printer.ForeColor = IIf(Fields(I).FontColor = "", Color.Black, Fields(I).FontColor)
                Printer.FontTransparent = False
                Y = Fields(I).Top
                X = Fields(I).Left
                C = ParseTagCode(Fields(I).Caption)
                If IsIn(Fields(I).Name, "Description", "Comments") Then C = WrapLongText(C, 46)
                If Fields(I).Name = "Bar Code" Then C = PrepareBarcode(C)
                W = Printer.TextWidth(C)
                H = Printer.TextHeight(C)
                Select Case Fields(I).Alignment
                    Case LayoutAlign.lyaPosition
                    Case LayoutAlign.lyaLeft : X = 0
                    Case LayoutAlign.lyaRight : X = TagWidth - W
                    Case LayoutAlign.lyaCenter : X = (TagWidth - W) / 2
                    Case LayoutAlign.lyaPositionR : X = X - W
                End Select
                If Fields(I).ExtraFieldType = 0 Then
                    Printer.CurrentX = XOff + X
                    Printer.CurrentY = YOff + Y
                    TagLog("Printing...  Loc=" & Printer.CurrentX & "x" & Printer.CurrentY, 7)
                    If IsIn(Fields(I).Name, "Description", "Comments") Then
                        Dim LLL As Object
                        For Each LLL In Split(C, vbCrLf)
                            Select Case Fields(I).Alignment
                                Case LayoutAlign.lyaPositionR
                                Case LayoutAlign.lyaLeft : X = 0
                                Case LayoutAlign.lyaRight : X = TagWidth - W
                                Case LayoutAlign.lyaCenter : X = (TagWidth - W) / 2
                                Case LayoutAlign.lyaPositionR : X = X - W
                            End Select
                            Printer.CurrentX = XOff + X
                            Printer.Print(LLL)
                        Next
                    Else
                        Printer.Print(C)
                    End If
                Else
                    'Dim P As IPictureDisp
                    Dim pW As Integer, pH As Integer, aPW As Integer, aPH As Integer
                    Dim P As Image, Caption As String

                    'P = LoadItemImage(Fields(I).Caption)
                    'P = LoadItemImageNew(Fields(I).Caption)
                    Caption = ImageFileName(Fields(I).Caption)
                    If Caption = "" Then
                        P = il.Images(0)
                    ElseIf Caption = "0" Then
                        P = il.Images(1)
                    Else
                        If Dir(Caption) = "" Then
                            P = il.Images(0)
                        Else
                            P = Image.FromFile(Caption)
                        End If
                    End If

                    pW = P.Width
                    pH = P.Height
                    If Fields(I).PicWidth > 0 Then pW = Fields(I).PicWidth
                    If Fields(I).PicHeight > 0 Then pH = Fields(I).PicHeight
                    aPW = pW
                    aPH = pH

                    If Fields(I).PicLock Then
                        'imgPrintHelper.Picture = P
                        imgPrintHelper.Image = P
                        MaintainPictureRatio(imgPrintHelper, pW, pH, False)
                        X = X + (aPW - pW) / 2
                        Y = Y + (aPH - pH) / 2
                    End If


                    'If P <> 0 Then Printer.PaintPicture P, XOff + X, YOff + Y, pW, pH
                    If P IsNot Nothing Then Printer.PaintPicture(P, XOff + X, YOff + Y, pW, pH)

                    P = Nothing
                End If
            End If
        Next
        If EoP Then Printer.EndDoc()
    End Sub

    Private ReadOnly Property TagWidth() As Single
        Get
            TagWidth = fraBox.Width
        End Get
    End Property

    Private ReadOnly Property TagHeight() As Single
        Get
            TagHeight = fraBox.Height
        End Get
    End Property

    Private Function LoadItemImage(ByVal Caption As String) As IPictureDisp
        Caption = ImageFileName(Caption)
        If Caption = "" Then
            'LoadItemImage = il.ListImages("invalid").Picture
            LoadItemImage = il.Images("invalid")
        ElseIf Caption = "0" Then
            'LoadItemImage = il.ListImages("blank").Picture
            LoadItemImage = il.Images("blank")
        Else
            If Dir(Caption) = "" Then
                'LoadItemImage = il.ListImages("invalid").Picture
                LoadItemImage = il.Images("invalid")
            Else
                LoadItemImage = LoadPictureStd(Caption)
            End If
        End If
    End Function

    Private Function LoadItemImageNew(ByVal Caption As String) As Image
        Caption = ImageFileName(Caption)
        If Caption = "" Then
            'LoadItemImage = il.ListImages("invalid").Picture
            'LoadItemImageNew = il.Images("invalid")
            LoadItemImageNew = il.Images(0)
        ElseIf Caption = "0" Then
            'LoadItemImage = il.ListImages("blank").Picture
            'LoadItemImageNew = il.Images("blank")
            LoadItemImageNew = il.Images(1)
        Else
            If Dir(Caption) = "" Then
                'LoadItemImage = il.ListImages("invalid").Picture
                'LoadItemImageNew = il.Images("invalid")
                LoadItemImageNew = il.Images(0)
            Else
                LoadItemImageNew = LoadPictureStdNew(Caption)
            End If
        End If
    End Function

    Private Function ImageFileName(ByVal Caption As String, Optional ByVal LookIn As String = "") As String
        If Caption = "#" Then Caption = CStr(RN)
        If Caption = "0" Then ImageFileName = Caption : Exit Function
        ImageFileName = FXFile(Caption)
        '
        '  If InStr(Caption, ":") Then
        '    ImageFileName = Caption
        '  Else
        '    ImageFileName = IIf(LookIn = "", PXFolder, LookIn) & Caption
        '  End If
        '
        '  If Dir(ImageFileName) = "" Then
        '    If InStr(ImageFileName, ".") = 0 Then
        '      If Dir(ImageFileName & ".bmp") <> "" Then ImageFileName = ImageFileName & ".bmp": Exit Function
        '      If Dir(ImageFileName & ".jpg") <> "" Then ImageFileName = ImageFileName & ".jpg": Exit Function
        '      If Dir(ImageFileName & ".gif") <> "" Then ImageFileName = ImageFileName & ".gif": Exit Function
        '      If Dir(ImageFileName & ".png") <> "" Then ImageFileName = ImageFileName & ".png": Exit Function
        '    End If
        '    ImageFileName = ""
        '  End If
    End Function

    Private Function ParseTagCode(ByVal Str As String) As String
        Dim N As Integer, Style As String, Sp As String, L As Integer, K As Integer
        Dim F As String, I As Integer, Tot As Decimal

        If Microsoft.VisualBasic.Left(Str, 1) <> "#" Then ParseTagCode = ParseExtraFieldToken(Str) : Exit Function
        Str = Mid(Str, 2)
        N = InStr(Str, ":")

        If N <= 0 Then ParseTagCode = "#" & Str : Exit Function

        F = LCase(Replace(Mid(Str, 1, N - 1), " ", ""))
        '  If F = "onsale" Then Stop
        Sp = Mid(Str, N + 1)
        ParseLineKey(Sp, L, K)
        If K = 0 Then
            Select Case F
                Case "list", "listprice", "landed", "onsale", "onsaleprice", "sale", "saleprice"
                    For I = 1 To 10
                        Style = GetMultipleStyle(L, I)
                        If Style <> "" Then Tot = Tot + GetPrice(GetItemField(Style, F))
                    Next
                    ParseTagCode = lCurrencyFormat(Tot)
                Case Else
                    Style = GetMultipleStyle(L, 1)
                    ParseTagCode = GetItemField(Style, F)
            End Select
        Else
            Style = GetMultipleStyle(L, K)
            ParseTagCode = GetItemField(Style, F)
        End If
    End Function

    Private Function lCurrencyFormat(ByVal C As Decimal) As String
        If chkDollarSign.Checked = False Then
            lCurrencyFormat = CurrencyFormat(C, True)
        Else
            lCurrencyFormat = FormatCurrency(C)
        End If
    End Function

    Private Function GetItemField(ByVal Style As String, ByVal Field As String) As String
        Dim CI As CInvRec
        If Style = "" Then GetItemField = "[NO STYLE #" & Style & "]" : Exit Function
        CI = New CInvRec
        If CI.Load(Style, "Style") Then
            Select Case Field
                Case "barcode" : GetItemField = PrepareBarcode(CI.Style)
                Case "style", "styleno" : GetItemField = IIf(StoreSettings.bStyleNoInCode, ConvertCostToCode(CI.Style), CI.Style)
                Case "vendor", "mfg" : GetItemField = CI.Vendor
                Case "vendorno", "mfgno" : GetItemField = CI.VendorNo
                Case "desc", "description" : GetItemField = CI.Desc
                Case "code" : GetItemField = CI.GetItemCode
                Case "onsale", "onsaleprice", "sale", "saleprice"
                    GetItemField = lCurrencyFormat(CI.OnSale)
                Case "list", "listprice" : GetItemField = lCurrencyFormat(CI.List)
                Case "landed" : GetItemField = IIf(StoreSettings.bCostInCode, ConvertCostToCode(lCurrencyFormat(CI.Landed)), lCurrencyFormat(CI.Landed))
                Case "stock", "instock" : GetItemField = CI.QueryStock(StoresSld)
                Case "onhand" : GetItemField = CI.OnHand
                Case "available" : GetItemField = CI.Available
                Case Else : GetItemField = "[UNKNOWN CODE: " & Field & "]"
            End Select
        Else
            GetItemField = "[INVALID STYLE: " & Style & "]"
        End If

        DisposeDA(CI)
    End Function

    Public Function GetMultipleStyle(ByVal L As Integer, ByVal K As Integer) As String
        Dim R As String, X As Object

        R = txtMultiple.Text
        R = Replace(R, vbCr, "")
        R = Replace(R, vbLf, "|")
        '  R = Replace(R, " ", "")

        X = Split(R, "|")
        L = L - 1
        If L < 0 Or L > UBound(X) Then Exit Function

        X = Split(X(L), ",")
        K = K - 1
        If K < 0 Or K > UBound(X) Then Exit Function

        GetMultipleStyle = Trim(X(K))
    End Function

    Private Sub ParseLineKey(ByVal S As String, ByRef Line As Integer, ByRef vKEY As Integer)
        Dim K As String
        On Error Resume Next
        Line = Val(S)
        K = IIf("" & Line = S, "`", LCase(Mid(S, Len("" & Line) + 1)))
        vKEY = Asc(K) - Asc("`")  ' a = 1
        If vKEY < 0 Or vKEY > 10 Then vKEY = 0
    End Sub

    Private Function ParseExtraFieldToken(ByVal S As String) As String
        Dim T As String
        ParseExtraFieldToken = S
        If Microsoft.VisualBasic.Left(S, 1) <> "@" Then Exit Function
        If PrintingStyle = "" Then Exit Function
        T = Replace(LCase(Mid(S, 2)), " ", "")
        ParseExtraFieldToken = GetItemField(PrintingStyle, T)
        If Microsoft.VisualBasic.Left(ParseExtraFieldToken, 8) = "[UNKNOWN" Then ParseExtraFieldToken = S
    End Function

    Private Sub GetPageAlignOffsets(ByVal PageAlign As Integer, ByVal PageLoc As Integer, ByRef XOffset As Integer, ByRef YOffset As Integer, ByRef EndOfPage As Boolean)
        Dim XCenter As Integer, YCenter As Integer
        Dim XMax As Integer, YMax As Integer
        XCenter = (Printer.Width - TagWidth) / 2
        YCenter = (Printer.Height - TagHeight) / 2
        XMax = (Printer.Width - TagWidth)
        YMax = (Printer.Height - TagHeight)

        TagLog("GetPageAlignOffsets PageAlign=" & PageAlign & ", Pageloc=" & PageLoc & ", XOffset=" & XOffset & ", YOffset=" & YOffset & ", EndOfPage=" & EndOfPage)
        TagLog("GetPageAlignOffsets XCenter=" & XCenter & ", YCenter=" & YCenter & ", XMax=" & XMax & ", YMax=" & YMax)

        If PageLoc < 0 Then
            EndOfPage = True
            Select Case PageAlign
                Case 0 : XOffset = 0 : YOffset = 0
                Case 1 : XOffset = XCenter : YOffset = 0
                Case 2 : XOffset = XMax : YOffset = 0
                Case 3 : XOffset = 0 : YOffset = YCenter
                Case 4 : XOffset = XCenter : YOffset = YCenter
                Case 5 : XOffset = XMax : YOffset = YCenter
                Case 6 : XOffset = 0 : YOffset = YMax
                Case 7 : XOffset = XCenter : YOffset = YMax
                Case 8 : XOffset = XMax : YOffset = YMax
            End Select
        Else
            Dim Mx As Integer, mY As Integer, Tot As Integer
            Mx = Int(CDbl(Printer.ScaleWidth) / CDbl(TagWidth + 120))
            mY = Int(CDbl(Printer.ScaleHeight) / CDbl(TagHeight + 120))
            Tot = Mx * mY
            PageLoc = PageLoc Mod Tot
            XOffset = Int(PageLoc Mod Mx) * (TagWidth + 120)
            YOffset = Int(PageLoc / Mx) * (TagHeight + 120)
            EndOfPage = (PageLoc = Tot - 1)
        End If

        TagLog("GetPageAlignOffsets PageAlign=" & PageAlign & ", Pageloc=" & PageLoc & ", XOffset=" & XOffset & ", YOffset=" & YOffset & ", EndOfPage=" & EndOfPage)
    End Sub

    Private Function TagLog(ByVal Msg As String, Optional ByVal Importance As Integer = 3) As Boolean
        ActiveLog("frmDesignTag::" & Msg, Importance)
        TagLog = True
    End Function

    Private Sub LoadTagLayout(ByVal TagName As String, Optional ByVal AsTemplate As Boolean = False)
        Dim FN As String, Opts As String
        If TagName = "-Select From List-" Or TagName = "(Default)" Then Exit Sub

        FN = TagLayoutFileName(TagName, AsTemplate)
        Opts = ReadFile(FN, 1, 1)

        If InStr(Opts, "Version=" & CurrentTagLayoutVersion) = 0 Then ConvertTagLayout(FN)

        DoLoadTagLayout(FN, AsTemplate)
    End Sub

    Private Sub DoLoadTagLayout(ByVal FN As String, Optional ByVal AsTemplate As Boolean = False)
        Dim LL As Object, F As Object, L As String
        Dim TagSize As String, cX As Double, cy As Double
        Dim I As Integer, N As Integer

        ActiveLog("frmDesignTag::DoLoadTagLayout - FN: " & FN, 8)

        TagSize = ReadFile(FN, 1, 1)

        chkDollarSign.Checked = False
        '<CT>
        frmDesignTag_Load(Me, New EventArgs)
        '</CT>

        On Error Resume Next
        cmbPageAlign.Text = "Center"
        LL = Split(TagSize, ";")
        For Each F In LL
            ActiveLog("frmDesignTag::ParseGeneralOption - F = " & F, 9)
            ParseGeneralOption(F)
        Next

        ActiveLog("frmDesignTag::DoLoadTagLayout - RemoveAllExtraFields", 8)
        RemoveAllExtraFields()

        I = 1
        N = CountFileLines(FN)
        For I = I To N - 1
            L = ReadFile(FN, I + 1, 1)
            If L <> "" Then
                ActiveLog("frmDesignTag::ProcessField - L[I=" & I & "] = " & L, 8)

                LL = Split(L, ",")

                'If I > MaxField Then
                If I > MaxField + 1 Then
                    ActiveLog("frmDesignTag::ProcessField - L = " & L, 8)
                    AddExtraField()
                End If

                On Error Resume Next
                Fields(I - 1).Visible = CBool(LL(1))
                'Fields(I).Visible = CBool(LL(1))
                Fields(I - 1).FontName = LL(2)
                Fields(I - 1).FontSize = Val(LL(3))
                Fields(I - 1).FontColor = Val(LL(4))
                Fields(I - 1).Left = Val(LL(5))
                Fields(I - 1).Top = Val(LL(6))
                Fields(I - 1).Alignment = Val(LL(7))
                Fields(I - 1).CharSpecify = LL(8)
                If I > DefaultFields - 1 Then
                    Fields(I - 1).Caption = LL(0)
                End If
                Fields(I - 1).ExtraFieldType = Val(LL(9))
                Fields(I - 1).PicWidth = Val(LL(10))
                Fields(I - 1).PicHeight = Val(LL(11))
                Fields(I - 1).PicLock = LL(12)
                On Error GoTo 0
            End If
        Next
        ActiveLog("frmDesignTag::DoLoadTagLayout - Complete!", 8)
    End Sub

    Private Sub AddExtraField()
        Dim N As Integer

        N = UBound(Fields) + 1
        'N = N - 1
        'ReDim Preserve Fields(1 To N)
        ReDim Preserve Fields(0 To N)
        Fields(N).Name = "Extra " & N - DefaultFields
        Fields(N).FontName = Fields(N - 1).FontName
        Fields(N).FontSize = Fields(N - 1).FontSize
        Fields(N).Caption = Fields(N).Name
        Fields(N).ToolTipText = "A text label"
        Fields(N).Visible = True
        Fields(N).Left = 0
        Fields(N).Top = 0

        InitItemList()
        RefreshFields()
    End Sub

    Private Sub RefreshFields()
        tmr.Enabled = False
        tmr.Interval = 10
        tmr.Enabled = True
    End Sub

    Private Sub InitItemList()
        Dim N As Integer
        Dim Addeditem As Integer

        lstItemsAdjust = True
        lstItems.Items.Clear()

        For N = 0 To MaxField
            'lstItems.AddItem Fields(N).Name
            'lstItems.itemData(lstItems.NewIndex) = N
            'lstItems.Selected(lstItems.NewIndex) = Fields(N).Visible
            Addeditem = lstItems.Items.Add(New ItemDataClass(Fields(N).Name, N))
            lstItems.SetSelected(Addeditem, Fields(N).Visible)
        Next

        'If lstItems.Selected(0) Then
        '    lstItems.Selected(0) = False
        '    lstItems.Selected(0) = True
        'Else
        '    lstItems.Selected(0) = False
        'End If
        If lstItems.GetSelected(0) = True Then
            lstItems.SetSelected(0, False)
            lstItems.SetSelected(0, True)
        Else
            lstItems.SetSelected(0, False)
        End If
        lstItemsAdjust = False
    End Sub

    Private Sub RemoveAllExtraFields()
        Do While MaxField > DefaultFields - 1
            RemoveExtraField()
        Loop
        InitItemList()
        RefreshFields()
    End Sub

    Private Sub RemoveExtraField(Optional ByVal N As Integer = -1)
        Dim I As Integer, X As Integer, C As Integer
        C = ExtraFieldCount()
        If C = 0 Then Exit Sub
        If N = -1 Or N > C Then N = C
        If N = C Then
            'ReDim Preserve Fields(1 To UBound(Fields) - 1)
            ReDim Preserve Fields(0 To UBound(Fields) - 1)
        Else
            X = DefaultFields + 1
            For I = DefaultFields + 1 To MaxField
                If I + 1 <> N Then Fields(I) = Fields(X)
                X = X + 1
            Next
        End If
        InitItemList()
        RefreshFields()
    End Sub

    Private Function ExtraFieldCount() As Integer
        ExtraFieldCount = MaxField - (DefaultFields - 1)
        If ExtraFieldCount < 0 Then ExtraFieldCount = 0
    End Function

    Private Sub ParseGeneralOption(ByVal Str As String)
        Dim N As Integer, F As String, R As String, tS As String, LL As Object
        N = InStr(Str, "=")
        If N <= 0 Then Exit Sub
        R = Mid(Str, N + 1)
        F = LCase(Mid(Str, 1, N - 1))

        Select Case F
            Case "tagsize"
                Select Case LCase(Microsoft.VisualBasic.Left(R, 1))
                    Case "l" : cmbLayoutDimensions.SelectedIndex = 0
                    Case "m" : cmbLayoutDimensions.SelectedIndex = 1
                    Case "s" : cmbLayoutDimensions.SelectedIndex = 2
                    Case Else ' custom
                        cmbLayoutDimensions.SelectedIndex = 3
                        tS = Trim(Mid(R, 3))
                        LL = Split(tS, "x")
                        txtCustomX.Text = FormatQuantity(Val(LL(0)), , False)
                        'txtCustomX_Validate(False)
                        txtCustomX_Validating(txtCustomX, New System.ComponentModel.CancelEventArgs)
                        txtCustomY.Text = FormatQuantity(Val(LL(1)), , False)
                        'txtCustomY_Validate(False)
                        txtCustomY_Validating(txtCustomY, New System.ComponentModel.CancelEventArgs)
                End Select
            Case "dollarsign" : chkDollarSign.Checked = IIf(LCase(R) = "true", True, False)
            Case "hidecents" : chkHideCents.Checked = IIf(LCase(R) = "true", True, False)
            Case "pagealign"
                On Error Resume Next
                cmbPageAlign.Text = R
            Case "version"      ' do nothing
                '    Case "dymo":        cmbDYMO.Text = R
            Case Else : Debug.Print("ParseGeneralOption -- Unknown Option: " & F)
        End Select
    End Sub

    Private Sub ConvertTagLayout(ByVal FN As String)
        If InStr(ReadFile(FN, 1, 1), "Version=") = 0 Then ConvertTagLayout_1_0(FN)
        If InStr(ReadFile(FN, 1, 1), "Version=1.1") = 0 Then ConvertTagLayout_1_1(FN)
        '  If InStr(ReadFile(FN, 1, 1), "Version=1.2") = 0 Then ConvertTagLayout_1_2 FN
        '  If InStr(ReadFile(FN, 1, 1), "Version=1.3") = 0 Then ConvertTagLayout_1_3 FN
        '  If InStr(ReadFile(FN, 1, 1), "Version=1.4") = 0 Then ConvertTagLayout_1_4 FN
    End Sub

    Private Sub ConvertTagLayout_1_1(ByVal FN As String)
        Dim Out As String, I As Integer, L As String, N As Integer

        I = 1
        N = CountFileLines(FN)
        For I = 1 To N
            L = ReadFile(FN, I, 1)
            If I = 1 Then L = Replace(L, "Version=1.1", "Version=1.2")
            If I = 11 Then L = L & vbCrLf & "Comments,False,,14,0,0,3300,0,,0,0,0,False"

            If L <> "" Then Out = Out & L & vbCrLf
        Next
        WriteFile(FN, Out, True)
        '  Debug.Print Out
    End Sub

    Private Sub ConvertTagLayout_1_0(ByVal FN As String)
        Dim Out As String, I As Integer, L As String, N As Integer

        I = 1
        N = CountFileLines(FN)
        For I = 1 To N
            L = ReadFile(FN, I, 1)
            If I = 1 Then L = L & ";Version=1.1"
            If I = 11 Then L = L & vbCrLf & "PackPrice,False,,14,0,0,3300,0,,0,0,0,False"

            If L <> "" Then Out = Out & L & vbCrLf
        Next
        WriteFile(FN, Out, True)
        '  Debug.Print Out
    End Sub

    Private Function TagLayoutFileName(ByVal TagName As String, Optional ByVal AsTemplate As Boolean = False) As String
        If Not AsTemplate Then
            TagLayoutFileName = TagLayoutFolder() & "taglayout-" & TagName & ".txt"
        Else
            TagLayoutFileName = TagLayoutFolder() & "TT-" & TagName & ".txt"
        End If
    End Function

    Private Sub txtCustomX_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtCustomX.Validating
        Dim L As Double, F As String, R As Integer
        L = Val(txtCustomX.Text)
        If L > TAGDIM_MAX_X Then
            F = Format(TAGDIM_MAX_X, "0.00")
        ElseIf L <= 0 Then
            F = Format(1, "0.00")
        Else
            F = Format(L, "0.00")
        End If
        If F <> txtCustomX.Text Then
            R = txtCustomX.SelectionStart
            txtCustomX.Text = F
            txtCustomX.SelectionStart = R
            '    Exit Sub
        End If
        SetBoxDimensions(TwipsPerInch * Val(txtCustomX.Text), fraBox.Height)
    End Sub

    Private Sub SetBoxDimensions(ByVal W As Integer, ByVal H As Integer)
        Dim MAXBOX_W As Integer, MAXBOX_H As Integer

        MAXBOX_W = fraClip.Width - 240
        MAXBOX_H = fraClip.Height - 240

        'fraBox.Move 120, 120, W, H
        fraBox.Location = New Point(120, 120)
        fraBox.Size = New Size(W, H)

        scrBoxX.Enabled = False
        scrBoxY.Enabled = False

        If W > MAXBOX_W Then
            scrBoxX.Enabled = True
            scrBoxX.Maximum = fraBox.Width - MAXBOX_W
            scrBoxX.SmallChange = 120
            scrBoxX.LargeChange = 1200
            scrBoxX.Value = 0
        End If
        If H > MAXBOX_H Then
            scrBoxY.Enabled = True
            scrBoxY.Maximum = fraBox.Height - MAXBOX_H
            scrBoxY.SmallChange = 120
            scrBoxY.LargeChange = 1200
            scrBoxY.Value = 0
        End If

        RefreshFields()    'fixes alignment
    End Sub

    Private Sub txtCustomY_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtCustomY.Validating
        Dim L As Double, F As String, R As Integer
        L = Val(txtCustomY.Text)
        If L > TAGDIM_MAX_Y Then
            F = Format(TAGDIM_MAX_Y, "0.00")
        ElseIf L <= 0 Then
            F = Format(1, "0.00")
        Else
            F = Format(L, "0.00")
        End If
        If F <> txtCustomY.Text Then
            R = txtCustomY.SelectionStart
            txtCustomY.Text = F
            txtCustomY.SelectionStart = R
            '    Exit Sub
        End If
        SetBoxDimensions(fraBox.Width, TwipsPerInch * Val(txtCustomY.Text))
    End Sub

    Private Sub frmDesignTag_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ActiveLog("frmDesignTag::Form_Load - Start")
        OrigPrinter = Printer.DeviceName
        RN = 0

        ActiveLog("frmDesignTag::Form_Load - Initial")
        ActiveLog("frmDesignTag::Form_Load - Loading Page Aligns")
        LoadPageAlignments
        ActiveLog("frmDesignTag::Form_Load - Loading Layout Types")
        LoadLayoutTypes
        ActiveLog("frmDesignTag::Form_Load - Loading Layout Templates")
        LoadLayoutTemplates
        ActiveLog("frmDesignTag::Form_Load - Loading Custom Layouts")
        LoadCustomLayouts

        ActiveLog("frmDesignTag::Form_Load - Loading Fonts")
        LoadFonts
        ActiveLog("frmDesignTag::Form_Load - Loading Colors")
        LoadColors

        ActiveLog("frmDesignTag::Form_Load - Initing Fields")
        InitFields
        ActiveLog("frmDesignTag::Form_Load - Loading Default Arrangement")
        LoadDefaultArrangement
        ActiveLog("frmDesignTag::Form_Load - Complete")
    End Sub

    Private Sub LoadPageAlignments()
        cmbPageAlign.Items.Clear()
        cmbPageAlign.Items.Insert(0, "Top Left")
        cmbPageAlign.Items.Insert(1, "Top Center")
        cmbPageAlign.Items.Insert(2, "Top Right")
        cmbPageAlign.Items.Insert(3, "Center Left")
        cmbPageAlign.Items.Insert(4, "Center")
        cmbPageAlign.Items.Insert(5, "Center Right")
        cmbPageAlign.Items.Insert(6, "Bottom Left")
        cmbPageAlign.Items.Insert(7, "Bottom Center")
        cmbPageAlign.Items.Insert(8, "Bottom Right")
        cmbPageAlign.SelectedIndex = 4
    End Sub

    Private Sub LoadLayoutTypes()
        On Error Resume Next
        cmbLayoutDimensions.Items.Clear()
        cmbLayoutDimensions.Items.Add("Large (" & TAGDIM_L_X & """ x " & TAGDIM_L_Y & """)")
        cmbLayoutDimensions.Items.Add("Medium (" & TAGDIM_M_X & """ x " & TAGDIM_M_Y & """)")
        cmbLayoutDimensions.Items.Add("Small (" & TAGDIM_S_X & """ x " & TAGDIM_S_Y & """)")
        cmbLayoutDimensions.Items.Add("Custom")
        cmbLayoutDimensions.SelectedIndex = 1
    End Sub

    Private Sub LoadLayoutTemplates()
        LoadTagLayoutTemplatesToComboBox(cmdLayoutTemplate)
    End Sub

    Private Sub LoadCustomLayouts()
        LoadCustomTagLayoutsToComboBox(cmbLayoutName)
    End Sub

    Private Sub LoadFonts()
        Dim N As Object
        cmbPropFontName.Items.Clear()
        For N = 0 To Printer.FontCount - 1
            cmbPropFontName.Items.Add(Printer.Fonts(N))
        Next
    End Sub

    Private Sub LoadColors()
        cmbPropFontColor.items.Clear
        'cmbPropFontColor.AddItem "Black": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = 0
        cmbPropFontColor.Items.Add(New ItemDataClass("Black", 0))
        'cmbPropFontColor.AddItem "White": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &HFFFFFF
        cmbPropFontColor.Items.Add(New ItemDataClass("White", &HFFFFFF))
        'cmbPropFontColor.AddItem "Yellow": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &HFFFF&
        cmbPropFontColor.Items.Add(New ItemDataClass("Yellow", &HFFFF&))
        'cmbPropFontColor.AddItem "Fuchsia": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &HFF00FF
        cmbPropFontColor.Items.Add(New ItemDataClass("Fuchsia", &HFF00FF))
        'cmbPropFontColor.AddItem "Red": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &HFF&
        cmbPropFontColor.Items.Add(New ItemDataClass("Red", &HFF&))
        'cmbPropFontColor.AddItem "Silver": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &HC0C0C0
        cmbPropFontColor.Items.Add(New ItemDataClass("Silver", &HC0C0C0))
        'cmbPropFontColor.AddItem "Gray": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &H808080
        cmbPropFontColor.Items.Add(New ItemDataClass("Gray", &H808080))
        'cmbPropFontColor.AddItem "Olive": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &H808000
        cmbPropFontColor.Items.Add(New ItemDataClass("Olive", &H808000))
        'cmbPropFontColor.AddItem "Purple": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &H800080
        cmbPropFontColor.Items.Add(New ItemDataClass("Purple", &H800080))
        'cmbPropFontColor.AddItem "Maroon": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &H80&
        cmbPropFontColor.Items.Add(New ItemDataClass("Maroon", &H80&))
        'cmbPropFontColor.AddItem "Aqua": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &HFFFF00
        cmbPropFontColor.Items.Add(New ItemDataClass("Aqua", &HFFFF00))
        'cmbPropFontColor.AddItem "Lime": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &HFF00&
        cmbPropFontColor.Items.Add(New ItemDataClass("Lime", &HFF00&))
        'cmbPropFontColor.AddItem "Green": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &H8000&
        cmbPropFontColor.Items.Add(New ItemDataClass("Green", &H8000&))
        'cmbPropFontColor.AddItem "Blue": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &HFF0000
        cmbPropFontColor.Items.Add(New ItemDataClass("Blue", &HFF0000))
        'cmbPropFontColor.AddItem "Navy": cmbPropFontColor.itemData(cmbPropFontColor.NewIndex) = &H800000
        cmbPropFontColor.Items.Add(New ItemDataClass("Navy", &H800000))
    End Sub

    Private Sub InitFields()
        On Error Resume Next
        Dim N As Integer, R As TagItemLayout

        'ReDim Fields(1 To DefaultFields)
        ReDim Fields(0 To DefaultFields - 1)

        For N = LBound(Fields) To UBound(Fields)
            Select Case N
                'Case 1
                Case 0
                    Fields(N).Name = "Bar Code"
                    Fields(N).FontName = FONT_C39_HALFINCH
                    Fields(N).FontSize = 30
                    Fields(N).Caption = "0000000000"
                    Fields(N).ToolTipText = "Your Item's Barcode"
                    Fields(N).Visible = True
                'Case 2
                Case 1
                    Fields(N).Name = "Style No"
                    Fields(N).FontName = "Arial"
                    Fields(N).FontSize = 10
                    Fields(N).Caption = "Style"
                    Fields(N).ToolTipText = "Your Item's Style Number"
                    Fields(N).Visible = True
                'Case 3
                Case 2
                    Fields(N).Name = "Description"
                    Fields(N).FontName = "Arial"
                    Fields(N).FontSize = 9
                    Fields(N).Caption = "This is your Item's Description.  Up to 3 lines!"
                    Fields(N).ToolTipText = "This is your Item's Description.  Up to 3 lines!"
                    Fields(N).Visible = True
                'Case 4
                Case 3
                    Fields(N).Name = "Code"
                    Fields(N).FontName = "Arial"
                    Fields(N).FontSize = 10
                    Fields(N).Caption = "Code"
                    Fields(N).ToolTipText = "This is the item code."
                    Fields(N).Visible = True
                'Case 5
                Case 4
                    Fields(N).Name = "List Price"
                    Fields(N).FontName = "Arial"
                    Fields(N).FontSize = 12
                    Fields(N).Caption = "List Price"
                    Fields(N).ToolTipText = "The Item's List Price"
                    Fields(N).Visible = True
                'Case 6
                Case 5
                    Fields(N).Name = "On Sale Price"
                    Fields(N).FontName = "Arial"
                    Fields(N).FontSize = 14
                    Fields(N).Caption = "On Sale"
                    Fields(N).ToolTipText = "The Item's On Sale Price"
                    Fields(N).Visible = True
                'Case 7
                Case 6
                    Fields(N).Name = "Stock"
                    Fields(N).FontName = "Arial"
                    Fields(N).FontSize = 14
                    Fields(N).Caption = "Stock"
                    Fields(N).ToolTipText = "The Amount of Stock Available"
                    Fields(N).Visible = True
                'Case 8
                Case 7
                    Fields(N).Name = "Landed"
                    Fields(N).FontName = "Arial"
                    Fields(N).FontSize = 14
                    Fields(N).Caption = "Landed"
                    Fields(N).ToolTipText = "The Landed Price"
                    Fields(N).Visible = True
                'Case 9
                Case 8
                    Fields(N).Name = "Vendor"
                    Fields(N).FontName = "Arial"
                    Fields(N).FontSize = 14
                    Fields(N).Caption = "Vendor"
                    Fields(N).ToolTipText = "The name of the manufacturer"
                    Fields(N).Visible = True
                'Case 10
                Case 9
                    Fields(N).Name = "VendorNo"
                    Fields(N).FontName = "Arial"
                    Fields(N).FontSize = 14
                    Fields(N).Caption = "VendorNo"
                    Fields(N).ToolTipText = "The manufacturer's three digit number code"
                    Fields(N).Visible = True
                'Case 11
                Case 10
                    Fields(N).Name = "PackPrice"
                    Fields(N).FontName = "Arial"
                    Fields(N).FontSize = 14
                    Fields(N).Caption = "PackPrice"
                    Fields(N).ToolTipText = "The Package Price"
                    Fields(N).Visible = True
                'Case 12
                Case 11
                    Fields(N).Name = "Comments"
                    Fields(N).FontName = "Arial"
                    Fields(N).FontSize = 14
                    Fields(N).Caption = "Comments"
                    Fields(N).ToolTipText = "Comments Field"
                    Fields(N).Visible = True
            End Select
        Next

        InitItemList()
        RefreshFields()
    End Sub

    Private Sub LoadDefaultArrangement()
        Dim I As Integer, Y As Integer
        Y = 100
        On Error Resume Next
        'For I = 1 To MaxField
        For I = 0 To MaxField
            Fields(I).Left = 0
            Fields(I).Top = Y
            Y = Y + 400
        Next
        RefreshFields()
    End Sub

    Private Sub cmbLayoutDimensions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbLayoutDimensions.SelectedIndexChanged
        Dim W As Integer, H As Integer

        txtCustomX.Visible = cmbLayoutDimensions.SelectedIndex = 3
        txtCustomY.Visible = cmbLayoutDimensions.SelectedIndex = 3
        lblBy.Visible = cmbLayoutDimensions.SelectedIndex = 3
        Select Case cmbLayoutDimensions.SelectedIndex
            Case 0 : W = TAGDIM_L_X * TwipsPerInch : H = TAGDIM_L_Y * TwipsPerInch
            Case 1 : W = TAGDIM_M_X * TwipsPerInch : H = TAGDIM_M_Y * TwipsPerInch
            Case 2 : W = TAGDIM_S_X * TwipsPerInch : H = TAGDIM_S_Y * TwipsPerInch
            Case Else
                txtCustomX.Text = FormatQuantity(TAGDIM_C_X, , False)
                txtCustomY.Text = FormatQuantity(TAGDIM_C_Y, , False)
        End Select
        LoadLayoutTemplates()
        If W <> 0 And H <> 0 Then SetBoxDimensions(W, H)
    End Sub

    Private Sub lstItems_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles lstItems.ItemCheck
        Dim I As Integer, F As Integer, Chg As Boolean, Show As Boolean
        If lstItemsAdjust Then Exit Sub

        On Error Resume Next
        For I = 0 To lstItems.Items.Count - 1
            F = I + 1
            'Show = lstItems.Selected(I)
            Show = lstItems.GetSelected(I)
            Fields(F).Visible = Show
        Next
        RefreshFields()
    End Sub

    Private Sub RefreshItemCaptions(Optional ByVal Style As String = "")
        Dim I As Integer, C As CInvRec, K As cInvKit, X As String, Kit As Boolean
        C = New CInvRec
        If Style <> "" Then
            If Microsoft.VisualBasic.Left(Style, 4) = KIT_PFX Then
                K = New cInvKit
                If Not K.Load(Style, "KitStyleNo") Then
                    Style = ""
                    MessageBox.Show("Kit not found.", "Tag Designer")
                Else
                    RN = 0
                    Kit = True
                    C.Load(K.Item1, "Style")
                End If
            Else
                If Not C.Load(Style, "Style") Then
                    Style = ""
                    MessageBox.Show("Style not found.", "Tag Designer")
                Else
                    RN = C.RN
                    Kit = False
                End If
            End If
        End If
        If Style = "" Then RN = 0

        For I = LBound(Fields) To UBound(Fields)
            'If I > DefaultFields Then
            If I > DefaultFields - 1 Then
                If Fields(I).ExtraFieldType = 1 And Fields(I).Caption = "#" Then
                    Fields(I).Caption = RN
                Else
                    X = Fields(I).Caption
                End If
            Else
                If Style = "" Then
                    X = Fields(I).Name
                    If I = 1 Then X = "0000000000"    ' put something in barcode
                Else
                    If Kit Then
                        Select Case I       ' for kits
                            Case 0 : X = Replace(K.KitStyleNo, " ", "_") 'PrepareBarcode(C.Style)
                            Case 1 : X = IIf(StoreSettings.bStyleNoInCode, ConvertCostToCode(K.KitStyleNo), K.KitStyleNo)
                            Case 2 : X = K.Heading
                            Case 3 : X = C.GetItemCode   ' from first item
                            Case 4 : X = CurrencyFormat(K.List, chkHideCents.Checked = True, chkDollarSign.Checked = True)
                            Case 5 : X = CurrencyFormat(K.OnSale, chkHideCents.Checked = True, chkDollarSign.Checked = True)
                            Case 6 : X = ""
                            Case 7 : X = IIf(StoreSettings.bCostInCode, ConvertCostToCode(CurrencyFormat(K.Landed, chkHideCents.Checked = True, False)), CurrencyFormat(K.Landed, chkHideCents.Checked = True, False))
                            Case 8 : X = C.Vendor        ' from first item
                            Case 9 : X = C.VendorNo      ' from first item
                            Case 10 : X = CurrencyFormat(K.PackPrice, chkHideCents.Checked = True, chkDollarSign.Checked = True)
                            Case 11 : X = WrapLongText(K.MemoArea, 46)
                            Case Else : X = ""
                        End Select
                    Else
                        Select Case I       ' for items
                            Case 0 : X = Replace(C.Style, " ", "_") 'PrepareBarcode(C.Style)
                            Case 1 : X = IIf(StoreSettings.bStyleNoInCode, ConvertCostToCode(C.Style), C.Style)
                            Case 2 : X = WrapLongText(C.Desc, 46)
                            Case 3 : X = C.GetItemCode
                            Case 4 : X = CurrencyFormat(C.List, chkHideCents.Checked = True, chkDollarSign.Checked = True)
                            Case 5 : X = CurrencyFormat(C.OnSale, chkHideCents.Checked = True, chkDollarSign.Checked = True)
                            Case 6 : X = C.QueryStock(StoresSld)
                            Case 7 : X = IIf(StoreSettings.bCostInCode, ConvertCostToCode(CurrencyFormat(C.Landed, chkHideCents.Checked = True, False)), CurrencyFormat(C.Landed, chkHideCents.Checked = True, False))
                            Case 8 : X = C.Vendor
                            Case 9 : X = C.VendorNo
                            Case 10 : X = CurrencyFormat(C.OnSale, chkHideCents.Checked = True, chkDollarSign.Checked = True)
                            Case 11 : X = WrapLongText(C.Comments, 46)
                            Case Else : X = ""
                        End Select
                    End If
                End If
                Fields(I).Caption = X
            End If
        Next

        DisposeDA(C, K)

        RefreshFields()
    End Sub

    Public Sub PreparePrinter()
        On Error Resume Next
        If IsDoubleR Then
        Else
            If Printer.DeviceName Like "*DYMO*" Then
                '    Printer.PaperSize = vbPRPSUser
                TagLog("PreparePrinter DYMO Sz: " & Printer.Width & """x" & Printer.Height & """")
                '    Printer.PaperSize = DYMO_PaperSize_30256
                Printer.ScaleMode = VBRUN.ScaleModeConstants.vbInches
                TagLog("PreparePrinter DYMO Pre-Sz: " & Printer.Width & """x" & Printer.Height & """")
                Printer.Width = txtCustomX.Text
                Printer.Height = txtCustomY.Text
                TagLog("PreparePrinter DYMO Set-Sz: " & Printer.Width & """x" & Printer.Height & """")
                '    Printer.Orientation = vbPRORLandscape
                Printer.ScaleMode = VBRUN.ScaleModeConstants.vbTwips
            End If
        End If
    End Sub

End Class
