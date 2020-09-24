﻿Public Class clsServiceNotes
    Public ServiceNoteID As Integer
    Public ServiceCall As Integer
    Public MarginNo As Integer
    Public Note As String
    Public NoteDate As Date
    Public NoteType As Integer
    Private WithEvents mDataAccess As CDataAccess
    'Implements CDataAccess

    Private Const TABLE_NAME = "ServiceNotes"
    Private Const TABLE_INDEX = "ServiceNoteID"

    Public Function DataAccess() As CDataAccess
        DataAccess = mDataAccess
    End Function

    Public Function NoteTypeString() As String
        If IsNothing(NoteType) Then NoteType = 0
        Select Case NoteType
            Case 0
                NoteTypeString = "Note"
            Case 1
                NoteTypeString = "Parts Order"
            Case Else
                NoteTypeString = "Strange Note (" & NoteType & ")"
        End Select
    End Function
End Class
