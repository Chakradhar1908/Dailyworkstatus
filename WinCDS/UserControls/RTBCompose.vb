﻿'Imports stdole
Public Class RTBCompose
    '
    'RTBCompose
    '==========
    '
    'UserControl that adds some buttons and logic to a RichTextBox
    'to make a simple RTF editor easily dropped into many programs.
    '
    'Plenty of room for customization and programs can set the
    'SendButton property False to suppress the use of the send
    'button included here.
    '

    'Defined here because we're late binding to Shell32.dll:
    Private Const ssfMYPICTURES As Integer = &H27&

    'This is a Shell Special Folder vDir value used to find a special
    'folder to use as the initial place to browse for pictures.
    '
    'You could also do something like make a String property that
    'accepts and returns the actual path to use/last used.  The main
    'program might use this with an INI file, etc. to persist the
    'last folder used across runs of the program.
    Private Const PICTURES_DEFAULT As Integer = ssfMYPICTURES

    Private Const WM_PASTE As Integer = &H302
    Private Const EM_SETMARGINS = &HD3&
    Private Const EC_LEFTMARGIN = &H1&
    Private Const EC_RIGHTMARGIN = &H2&

    Private Declare Function SendMessage Lib "USER32" Alias "SendMessageW" (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    Private CommonDlgs As CommonDlgs

    Private Const MARGINS_DEFAULT As Integer = 4 'Pixels.
    Private Const SENDBUTTON_DEFAULT As Boolean = False

    'Metrics used for resizing.  See UserControl_Initialize for the
    'calculation of these values:
    Private ButtonGroupsGap As Single        'Gap width between grouped buttons.
    'Buttons are adjacent within groups.
    Private ButtonTop As Single
    Private ButtonHeight As Single
    Private ButtonWidth As Single
    Private MinWidthNoSendButton As Single   'Used by MinWidth property.
    Private MinWidthWithSendButton As Single 'Used by MinWidth property.

    Private ChangeSelection As Boolean       'Set False by RTB_SelChange event handler
    'to prevent selection changes when only
    'the states of CheckBox buttons are
    'altered there.

    'Private mBackColor As integer
    Private mBackColor As Color
    Private mIsDirty As Boolean
    Private mMargins As Integer
    Private mSendButton As Boolean

    'Private WithEvents mFont As StdFont
    Private mFont As Font
    Public FileName As String

    '=== Public events ===========================================

    Public Event IsDirtyChanged()
    Public Event SendClicked()
    Dim CurrentFont As Font
    Dim NewStyle As FontStyle

    '=== Public properties =======================================

    Public Overrides Property BackColor() As System.Drawing.Color
        Get
            BackColor = mBackColor
        End Get
        Set(value As Color)
            mBackColor = value
            RTB.BackColor = mBackColor
            'PropertyChanged "BackColor"
        End Set
    End Property

    'Public Property Font() As StdFont
    '    Get
    '        Font = mFont
    '    End Get
    '    Set(value As StdFont)
    '        With mFont
    '            .Bold = Font.Bold
    '            .Italic = Font.Italic
    '            .Name = Font.Name
    '            .Size = Font.Size
    '        End With
    '        'PropertyChanged "Font"
    '    End Set
    'End Property

    Public Overrides Property Font() As Font
        Get
            Font = mFont
        End Get
        Set(value As Font)
            With mFont
                '.Bold = Font.Bold
                '.Italic = Font.Italic
                '.Name = Font.Name
                '.Size = Font.Size
            End With
            If Font.Bold = True Then
                mFont = New Font(Font.Name, Font.Size, FontStyle.Bold)
            End If
            If Font.Italic = True Then
                mFont = New Font(Font.Name, Font.Size, FontStyle.Italic)
            End If
            'PropertyChanged "Font"
            mFont_FontChanged("Font")
        End Set
    End Property

    Public Property IsDirty() As Boolean
        'Returned True when any change (via SelChange event) has
        'been made after last set False (via this property or
        'via LoadFile, SaveFile, Text, or RTFText properties).
        Get
            IsDirty = mIsDirty
        End Get
        Set(value As Boolean)
            SetIsDirty(value)
        End Set
    End Property

    Private Sub SetIsDirty(ByVal RHS As Boolean)
        If RHS <> mIsDirty Then
            mIsDirty = RHS
            RaiseEvent IsDirtyChanged()
        End If
    End Sub

    Public Property Margins() As Integer
        Get
            Margins = mMargins
        End Get
        Set(value As Integer)
            value = value And &HFFFF&
            mMargins = value

            SendMessage(RTB.Handle, EM_SETMARGINS, EC_LEFTMARGIN Or EC_RIGHTMARGIN,
                        mMargins _
                     Or ((mMargins And &H7FFF) * &H10000) _
                     Or IIf(CBool(mMargins And &H8000&), &H80000000, 0))
            'PropertyChanged "Margins"
        End Set
    End Property

    Public ReadOnly Property MINWIDTH() As Single
        Get
            'Useful for parent container (Form) resize routines.  Returns
            'the minimum width that fits the toolbar buttons.  Value
            'returned is in the ScaleMode of the UserControl (normally
            'Twips unless you change its ScaleMode).

            If mSendButton Then
                MINWIDTH = MinWidthWithSendButton
            Else
                MINWIDTH = MinWidthNoSendButton
            End If
        End Get
    End Property

    Public Property SendButton() As Boolean
        Get
            SendButton = mSendButton
        End Get
        Set(value As Boolean)
            mSendButton = value
            cmdSend.Visible = mSendButton
            'PropertyChanged "SendButton"
        End Set
    End Property

    'Marked "Procedure Id = Text" in Tools|Procedure Attributes:
    Public Overrides Property Text() As String
        Get
            Text = RTB.Text
        End Get
        Set(value As String)
            RTB.Text = value
            SetIsDirty(False)
            'PropertyChanged "Text"
        End Set
    End Property

    'Marked "Don't show in Property Browser" in Tools|Procedure Attributes:
    Public Property TextRTF() As String
        Get
            TextRTF = RTB.Rtf
        End Get
        Set(value As String)
            RTB.Rtf = value
            SetIsDirty(False)
        End Set
    End Property

    'Public Sub LoadFile(Optional ByVal vFileName As String = "", Optional ByVal FileType As LoadSaveConstants = rtfRTF)
    Public Sub LoadFile(Optional ByVal vFileName As String = "", Optional ByVal FileType As RichTextBoxStreamType = RichTextBoxStreamType.RichText)
        If vFileName = "" Then vFileName = FileName Else FileName = vFileName
        If vFileName = "" Then Err.Raise(-1, , "No File passed to RTBCompose.LoadFile")

        RTB.LoadFile(FileName, FileType)
        'RTB.LoadFile(FileName, RichTextBoxStreamType.RichText)
        SetIsDirty(False)
    End Sub

    Public Sub SaveFile(Optional ByVal vFileName As String = "", Optional ByVal FileType As RichTextBoxStreamType = RichTextBoxStreamType.RichText)
        If vFileName = "" Then vFileName = FileName Else FileName = vFileName
        If vFileName = "" Then Err.Raise(-1, , "No File passed to RTBCompose.SaveFile")

        RTB.SaveFile(FileName, FileType)
        SetIsDirty(False)
    End Sub

    Private Sub chkBold_CheckedChanged(sender As Object, e As EventArgs) Handles chkBold.CheckedChanged
        If ChangeSelection Then
            With RTB
                'Select range if none selected?  Find range to change?
                '.SelBold = chkBold.Value = vbChecked
                '.SelectionFont.Bold = chkBold.Checked = True
                '.SelectionFont = New Font(.SelectionFont,.SelectionFont.Style ^ FontStyle.Bold)
                If .SelectionFont IsNot Nothing Then
                    CurrentFont = .SelectionFont
                    If .SelectionFont.Bold = True Then
                        NewStyle = CurrentFont.Style - FontStyle.Bold
                    Else
                        NewStyle = CurrentFont.Style + FontStyle.Bold
                    End If
                    .SelectionFont = New Font(CurrentFont.FontFamily, CurrentFont.Size, NewStyle)
                End If
                '.SetFocus
                .Select()
            End With
        End If
    End Sub

    Private Sub chkItalic_CheckedChanged(sender As Object, e As EventArgs) Handles chkItalic.CheckedChanged
        If ChangeSelection Then
            With RTB
                '.SelItalic = chkItalic.Value = vbChecked
                If .SelectionFont IsNot Nothing Then
                    CurrentFont = .SelectionFont
                    If .SelectionFont.Italic = True Then
                        NewStyle = CurrentFont.Style - FontStyle.Italic
                    Else
                        NewStyle = CurrentFont.Style + FontStyle.Italic
                    End If
                    .SelectionFont = New Font(CurrentFont.FontFamily, CurrentFont.Size, NewStyle)
                End If

                '.SetFocus
                .Select()
            End With
        End If
    End Sub

    Private Sub chkStrikeThru_CheckedChanged(sender As Object, e As EventArgs) Handles chkStrikeThru.CheckedChanged
        If ChangeSelection Then
            With RTB
                '.SelStrikeThru = chkStrikeThru.Value = vbChecked
                If .SelectionFont IsNot Nothing Then
                    CurrentFont = .SelectionFont
                    If .SelectionFont.Strikeout = True Then
                        NewStyle = CurrentFont.Style - FontStyle.Strikeout
                    Else
                        NewStyle = CurrentFont.Style + FontStyle.Strikeout
                    End If
                    .SelectionFont = New Font(CurrentFont.FontFamily, CurrentFont.Size, NewStyle)
                End If

                '.SetFocus
                .Select()
            End With
        End If
    End Sub

    Private Sub chkUnderline_CheckedChanged(sender As Object, e As EventArgs) Handles chkUnderline.CheckedChanged
        If ChangeSelection Then
            With RTB
                '.SelUnderline = chkUnderline.Value = vbChecked
                If .SelectionFont IsNot Nothing Then
                    CurrentFont = .SelectionFont
                    If .SelectionFont.Underline = True Then
                        NewStyle = CurrentFont.Style - FontStyle.Underline
                    Else
                        NewStyle = CurrentFont.Style + FontStyle.Underline
                    End If
                    .SelectionFont = New Font(CurrentFont.FontFamily, CurrentFont.Size, NewStyle)
                End If

                '.SetFocus
            End With
        End If
    End Sub

    Private Sub cmdCenter_Click(sender As Object, e As EventArgs) Handles cmdCenter.Click
        With RTB
            '.SelAlignment = rtfCenter
            .SelectionAlignment = HorizontalAlignment.Center
            '.SetFocus
            .Select()
        End With

    End Sub

    Private Sub cmdCopy_Click(sender As Object, e As EventArgs) Handles cmdCopy.Click
        'With RTB
        Clipboard.Clear()
        'Clipboard.SetText.SelRTF, vbCFRTF
        'Clipboard.SetText.SelText, vbCFText
        Clipboard.SetText(RTB.SelectedRtf, TextDataFormat.UnicodeText)
        Clipboard.SetText(RTB.SelectedText, TextDataFormat.Text)

        '.SetFocus
        RTB.Select()
        'End With
    End Sub

    Private Sub cmdCut_Click(sender As Object, e As EventArgs) Handles cmdCut.Click
        'With RTB
        Clipboard.Clear()
        'Clipboard.SetText.SelRTF, vbCFRTF
        'Clipboard.SetText.SelText, vbCFText
        Clipboard.SetText(RTB.SelectedRtf, TextDataFormat.Rtf)
        Clipboard.SetText(RTB.SelectedText, TextDataFormat.Text)
        '.SelRTF = ""
        RTB.SelectedRtf = ""

        '.SetFocus
        RTB.Select()
        'End With
    End Sub

    Private Sub cmdDelete_Click(sender As Object, e As EventArgs) Handles cmdDelete.Click
        'With RTB
        '.SelRTF = ""
        RTB.SelectedRtf = ""
        '.SetFocus
        RTB.Select()
        'End With
    End Sub

    Private Sub cmdFontColor_Click(sender As Object, e As EventArgs) Handles cmdFontColor.Click
        'CommonDlgs = New CommonDlgs
        With CommonDlgs
            .DialogTitle = "Choose font and color"
            .Flags = CommonDlgs.FontsConstants.cdlCFEffects _
                  Or CommonDlgs.FontsConstants.cdlCFForceFontExist _
                  Or CommonDlgs.FontsConstants.cdlCFInitFont _
                  Or CommonDlgs.FontsConstants.cdlCFNoScriptSel _
                  Or CommonDlgs.FontsConstants.cdlCFScreenFonts

            'If IsNull(RTB.SelFontName) Then
            If IsNothing(RTB.SelectionFont.Name) Then
                .FontName = ""
            Else
                .FontName = RTB.SelectionFont.Name
            End If
            'If IsNull(RTB.SelFontSize) Then
            If IsNothing(RTB.SelectionFont.Size) Then
                .FontSize = 0
            Else
                .FontSize = RTB.SelectionFont.Size
            End If
            'If IsNull(RTB.SelBold) Then
            If IsNothing(RTB.SelectionFont.Bold) Then
                .FontBold = False
            Else
                .FontBold = RTB.SelectionFont.Bold
            End If
            'If IsNull(RTB.SelItalic) Then
            If IsNothing(RTB.SelectionFont.Italic) Then
                .FontItalic = False
            Else
                .FontItalic = RTB.SelectionFont.Italic
            End If
            'If IsNull(RTB.SelStrikeThru) Then
            If IsNothing(RTB.SelectionFont.Strikeout) Then
                .FontStrikeThru = False
            Else
                .FontStrikeThru = RTB.SelectionFont.Strikeout
            End If
            'If IsNull(RTB.SelUnderline) Then
            If IsNothing(RTB.SelectionFont.Underline) Then
                .FontUnderline = False
            Else
                .FontUnderline = RTB.SelectionFont.Underline
            End If
            'If IsNull(RTB.SelColor) Then
            'If IsNull(RTB.SelColor) Then
            If IsNothing(RTB.SelectionColor) Then
                .Color = Color.Black
            Else
                .Color = RTB.SelectionColor
            End If

            Dim Gr As Graphics
            Dim H As IntPtr
            Dim Hdc As Integer
            Gr = Parent.CreateGraphics
            H = Gr.GetHdc
            Hdc = H.ToInt32

            'If .ShowFont(Parent.hwnd, Parent.hDC) Then
            If .ShowFont(Parent.Handle, Hdc) Then
                'RTB.SelFontName = .FontName
                'RTB.SelFontSize = .FontSize
                'RTB.Font = New Drawing.Font(.FontName, .FontSize, .FontBold)
                RTB.SelectionFont = New Drawing.Font(.FontName, .FontSize)
                'RTB.SelBold = .FontBold
                If .FontBold = True Then
                    RTB.SelectionFont = New Drawing.Font(.FontName, .FontSize, FontStyle.Bold)
                End If
                'chkBold.Value = IIf(.FontBold, vbChecked, vbUnchecked)
                chkBold.Checked = IIf(.FontBold, True, False)
                'RTB.SelItalic = .FontItalic
                If .FontBold = True And .FontItalic = True Then
                    RTB.SelectionFont = New Drawing.Font(.FontName, .FontSize, FontStyle.Bold And FontStyle.Italic)
                ElseIf .FontBold = True And .FontItalic = False Then
                    RTB.SelectionFont = New Drawing.Font(.FontName, .FontSize, FontStyle.Bold)
                ElseIf .FontBold = False And .FontItalic = True Then
                    RTB.SelectionFont = New Drawing.Font(.FontName, .FontSize, FontStyle.Italic)
                End If
                'chkItalic.Value = IIf(.FontItalic, vbChecked, vbUnchecked)
                chkItalic.Checked = IIf(.FontItalic, True, False)
                'RTB.SelStrikeThru = .FontStrikeThru
                If .FontStrikeThru = True Then
                    RTB.SelectionFont = New Drawing.Font(.FontName, .FontSize, FontStyle.Strikeout)
                End If

                'chkStrikeThru.Value = IIf(.FontStrikeThru, vbChecked, vbUnchecked)
                chkStrikeThru.Checked = IIf(.FontStrikeThru, True, False)
                'RTB.SelUnderline = .FontUnderline
                If .FontUnderline = True Then
                    RTB.SelectionFont = New Drawing.Font(.FontName, .FontSize, FontStyle.Underline)
                End If
                'chkUnderline.Value = IIf(.FontUnderline, vbChecked, vbUnchecked)
                chkUnderline.Checked = IIf(.FontUnderline, True, False)
                'RTB.SelColor = .Color
                RTB.SelectionColor = .Color
                'With RTB
                '    .SelStart = .SelStart + .SelLength
                'End With
                RTB.SelectionStart = RTB.SelectionStart + RTB.SelectionLength
                lblFontColor.Text = RTB.SelectionFont.Name & " " & CStr(Int(RTB.SelectionFont.Size))
            End If
        End With

        'RTB.SetFocus
        RTB.Select()
    End Sub

    Private Sub cmdLeft_Click(sender As Object, e As EventArgs) Handles cmdLeft.Click
        'With RTB
        '.SelAlignment = rtfLeft
        RTB.SelectionAlignment = HorizontalAlignment.Left
        '.SetFocus
        RTB.Select()
        'End With
    End Sub

    Private Sub cmdPaste_Click(sender As Object, e As EventArgs) Handles cmdPaste.Click
        'With RTB
        '    If Clipboard.GetFormat(vbCFRTF) Then
        '        .SelRTF = Clipboard.GetText(vbCFRTF)
        '    ElseIf Clipboard.GetFormat(vbCFText) Then
        '        .SelText = Clipboard.GetText(vbCFText)
        '    ElseIf Clipboard.GetFormat(vbCFBitmap) Or Clipboard.GetFormat(vbCFDIB) Then
        '        SendMessage.hwnd, WM_PASTE, 0, 0
        'End If
        '    .SetFocus

        If Clipboard.GetText(TextDataFormat.Rtf) <> "" Then
            RTB.SelectedRtf = Clipboard.GetText(TextDataFormat.Rtf)
        ElseIf Clipboard.GetText(TextDataFormat.Text) <> "" Then
            RTB.SelectedText = Clipboard.GetText(TextDataFormat.Text)
        ElseIf Clipboard.GetImage IsNot Nothing Then
            SendMessage(RTB.Handle, WM_PASTE, 0, 0)
        End If
        RTB.Select()
    End Sub

    Private Sub cmdRight_Click(sender As Object, e As EventArgs) Handles cmdRight.Click
        'With RTB
        '    .SelAlignment = rtfRight

        '    .SetFocus
        'End With
        RTB.SelectionAlignment = HorizontalAlignment.Right
        RTB.Select()
    End Sub

    Private Sub cmdSend_Click(sender As Object, e As EventArgs) Handles cmdSend.Click
        RaiseEvent SendClicked()
        'RTB.SetFocus
        RTB.Select()
    End Sub

    Private Sub mFont_FontChanged(ByVal PropertyName As String)
        With RTB
            .Font = mFont
            lblFontColor.Text = .Font.Name & " " & CStr(Int(.Font.Size))
        End With
        Refresh()
    End Sub

    Private Sub RTB_TextChanged(sender As Object, e As EventArgs) Handles RTB.TextChanged
        SetIsDirty(True)
    End Sub

    Private Sub RTB_KeyPress(sender As Object, e As KeyPressEventArgs) Handles RTB.KeyPress
        'Const CtrlB As Integer = 2
        'Const CtrlI As Integer = 9
        'Const CtrlU As Integer = 21

        Select Case e.KeyChar
            'Case CtrlB
            Case Convert.ToChar(2)
                If chkBold.Checked = False Then
                    chkBold.Checked = True
                Else
                    chkBold.Checked = False
                End If
            'Case CtrlI
            Case Convert.ToChar(9)
                If chkItalic.Checked = False Then
                    chkItalic.Checked = True
                Else
                    chkItalic.Checked = False
                End If
            'Case CtrlU
            Case Convert.ToChar(21)
                If chkUnderline.Checked = False Then
                    chkUnderline.Checked = True
                Else
                    chkUnderline.Checked = False
                End If
            Case Else
                Exit Sub
        End Select
        'KeyAscii = 0
        e.KeyChar = Convert.ToChar(0)
    End Sub

    Private Sub RTB_SelectionChanged(sender As Object, e As EventArgs) Handles RTB.SelectionChanged
        Dim Selected As Boolean
        'Dim ForeBrightness as integer
        Dim ForeBrightness As Color

        ChangeSelection = False
        With RTB
            'IIf() treats Null as False here:
            On Error Resume Next
            chkBold.Checked = IIf(.SelectionFont.Bold, True, False)
            chkItalic.Checked = IIf(.SelectionFont.Italic, True, False)
            chkStrikeThru.Checked = IIf(.SelectionFont.Strikeout, True, False)
            chkUnderline.Checked = IIf(.SelectionFont.Underline, True, False)

            Selected = RTB.SelectionLength > 0
            cmdCut.Enabled = Selected
            cmdCopy.Enabled = Selected
            cmdDelete.Enabled = Selected

            If IsNothing(.SelectionFont.Name) Then
                lblFontColor.Text = ""
            ElseIf IsNothing(.SelectionFont.Size) Then
                lblFontColor.Text = .SelectionFont.Name
            Else
                lblFontColor.Text = .SelectionFont.Name & " " & CStr(Int(.SelectionFont.Size))
            End If

            If IsNothing(.SelectionColor) Then
                lblFontColor.ForeColor = Color.Black
                lblFontColor.BackColor = Color.White
            Else
                ForeBrightness = .SelectionColor
                With lblFontColor
                    .ForeColor = ForeBrightness
                    ForeBrightness = Color.FromArgb(((ForeBrightness.ToArgb And &HFF0000) \ &H10000) + ((ForeBrightness.ToArgb And &HFF00&) \ &H100&) + (ForeBrightness.ToArgb And &HFF&))
                    If ForeBrightness.ToArgb > 400 Then
                        .BackColor = Color.Black
                    Else
                        .BackColor = Color.White
                    End If
                End With
            End If
        End With
        ChangeSelection = True
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'Grab a few button bar item dimensions used in resizing here
        'and in the Parent (Form) via the MinWidth property.

        'Buttons are all the same sizes, use this as a representative.
        'Also it is the last button before the "floating" button at
        'the right which is optional (see SendButton property).
        '
        'If you remove this button then replace this by another.
        With cmdPicture
            ButtonTop = .Top
            ButtonHeight = .Height
            ButtonWidth = .Width

            'Buttons used here are two that have a "Group Gap"
            'between them:
            ButtonGroupsGap = .Left - cmdRight.Left - ButtonWidth

            'These also need changes if you remove or add buttons:
            MinWidthNoSendButton = .Left + ButtonWidth
            MinWidthWithSendButton = MinWidthNoSendButton + ButtonGroupsGap + ButtonGroupsGap
        End With

        'Set up the RTB's default Font property.
        'mFont = New StdFont
        mFont = New Font("MS Sans Serif", 8, FontStyle.Regular)
        RTB.Font = New Drawing.Font(mFont.Name, mFont.Size, mFont.Style)

        lblFontColor.Text = mFont.Name & " " & CStr(Int(mFont.Size))
        ChangeSelection = True

        UserControl_InitProperties() 'Explicitly calling. Cause, in vb.net InitProperties event will not available.
        UserControl_ReadProperties() 'Explicitly calling. Cause, in vb.net ReadProperties event wil not available.
    End Sub

    Private Sub UserControl_InitProperties()
        'Defaults when new control instance created:
        mBackColor = RTB.BackColor
        Margins = MARGINS_DEFAULT
        SendButton = SENDBUTTON_DEFAULT
    End Sub

    Private Sub UserControl_ReadProperties()
        'On Error Resume Next
        CommonDlgs = New CommonDlgs
        'With PropBag
        '    BackColor = .ReadProperty("BackColor", vbWindowBackground)
        'Set Font = .ReadProperty("Font")
        'Margins = .ReadProperty("Margins", MARGINS_DEFAULT)
        '    SendButton = .ReadProperty("SendButton", SENDBUTTON_DEFAULT)
        '    Text = .ReadProperty("Text", "")
        'End With
    End Sub

    Private Sub RTBCompose_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        Dim RTBTop As Single

        'Use ButtonTop as padding height, 1 for the top of the buttons,
        'and 2 more for the gap below the button bar:
        RTBTop = ButtonHeight + (3 * ButtonTop)
        'RTB.Move 0, RTBTop, ScaleWidth, ScaleHeight - RTBTop
        RTB.Location = New Point(0, RTBTop)
        RTB.Size = New Size(Me.Width, Me.Height - RTBTop)

        'This will need updating if you remove the Send button from this
        'UserControl:
        If mSendButton Then
            'cmdSend.Left = ScaleWidth - ButtonWidth
            cmdSend.Left = Width - ButtonWidth
        End If
    End Sub

    Public Function asHtml() As String
        asHtml = RtfToHtml(RTB)
    End Function

    Private Sub cmdPicture_Click(sender As Object, e As EventArgs) Handles cmdPicture.Click
        With CommonDlgs
            .DialogTitle = "Picture to insert"
            .Filter = "Pictures (*.jpg;*.gif;*.bmp)|*.jpg;*.jpeg;*.gif;*.bmp"
            If Len(.InitDir) = 0 Then
                With CreateObject("Shell.Application").Namespace(PICTURES_DEFAULT).Self
                    CommonDlgs.InitDir = .Path
                End With
            End If
            .Flags = CommonDlgs.FileOpenConstants.cdlOFNExplorer _
              Or CommonDlgs.FileOpenConstants.cdlOFNFileMustExist _
              Or CommonDlgs.FileOpenConstants.cdlOFNPathMustExist _
              Or CommonDlgs.FileOpenConstants.cdlOFNLongNames _
              Or CommonDlgs.FileOpenConstants.cdlOFNShareAware
            If .ShowOpen(Parent.Handle) Then
                Clipboard.Clear()
                'Clipboard.SetData LoadPictureStd(.FileName)
                Clipboard.SetData(DataFormats.Bitmap, LoadPictureStd(.FileName))
                'SendMessage RTB.hwnd, WM_PASTE, 0, 0
                SendMessage(RTB.Handle, WM_PASTE, 0, 0)
            End If
        End With

        'RTB.SetFocus
        RTB.Select()
    End Sub

End Class
