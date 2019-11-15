﻿Public Class frmProgress
    Public AltPrg As Object
    Public LockOn As Boolean              ' this does a version modal
    Public PreventLockOn As Boolean

    Public Result As MsgBoxResult
    Public Event Done(ByVal Result As MsgBoxResult, ByRef StayOpen As Boolean)

    Public Sub Progress(Optional ByVal Value As Integer = -1, Optional ByVal Max As Integer = -1, Optional ByVal Cap As String = "#", Optional ByVal DoShow As Boolean = False, Optional ByVal vLockOn As Boolean = True, Optional ByVal vButtons As MsgBoxStyle = 0)
        Dim P
        Result = 0

        If AltPrg Is Nothing Then
            P = prg
        Else
            P = AltPrg
        End If
        'Me.ZOrder 0
        Me.BringToFront()


        LockOn = vLockOn

        On Error Resume Next
        If Value = -2 Then
            P.Visible = False
            '    PW.Visible = True
            '    PW.Active = True
        Else
            P.Visible = True
            If Not IsIn(Cap, "-", "#", " ", lblCaption.Text) Then lblCaption.Text = Trim(Cap)
            'If Max <> -1 Then P.Max = Max
            'If Value = -1 Then Value = P.Value + 1
            'If Value > P.Max Then Value = P.Max
            'P.Value = Value
        End If
        'P.Refresh
        'Me.Refresh
        If DoShow Then
            If Not PreventLockOn Then
                If Value = 1 Or Value = 0 Then
                    Show()
                End If
            End If
            Application.DoEvents()
        End If
    End Sub

    Public Sub ProgressClose(Optional ByVal WithEvent As Boolean = False)
        Result = 0
        RaiseEvent Done(0, False)
        'Unload Me
        Me.Close()
    End Sub

    Private Sub frmProgress_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        Width = 423
        Height = 100

        InitLineBorder(linBorder, 0, 0, Width, Height, 4)
        'lblCaption.Move 180, 180
        lblCaption.Location = New Point(180, 180)
        'prg.Move 180, 480, Width - 360, 360
        prg.Location = New Point(180, 480)
        prg.Size = New Size(Width - 360, 360)
    End Sub

    Public Sub Disposee()
        ProgressClose()
    End Sub

    Private Sub lblCaption_Click()
        prg_Click()
    End Sub

    Private Sub prg_Click()
        PreventLockOn = Not PreventLockOn
        SetAlwaysOnTop(Me, False)
    End Sub

    Private Sub frmProgress_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'prg.Min = 0
        'prg.Max = 100
        'prg.Value = 0
        prg.Text = 0
        'SetAlwaysOnTop Me
    End Sub
End Class