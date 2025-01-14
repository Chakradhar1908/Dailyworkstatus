﻿Module modINET
    Public Const INETTimeout_Default As Integer = 15
    Public INETTimeout As Integer

    Public Function INETGET(ByVal URL As String, Optional ByVal exHeader As String = "") As String
        Dim F As frmINet, Res As String
        F = New frmINet
        If INETTimeout = 0 Then INETTimeout = INETTimeout_Default
        F.inet.RequestTimeout = INETTimeout
        Res = F.MakeRequest(URL, "GET", "", exHeader)
        INETTimeout = INETTimeout_Default
        'Unload F
        F.Close()

        INETGET = Res
    End Function

    Public Function INETPOST(ByVal URL As String, ByVal sData As String, Optional ByVal exHeader As String = "") As String
        Dim F As frmINet, Res As String
        F = New frmINet
        If INETTimeout = 0 Then INETTimeout = INETTimeout_Default
        F.inet.RequestTimeout = INETTimeout
        Res = F.MakeRequest(URL, "POST", sData, exHeader)
        INETTimeout = INETTimeout_Default
        'Unload F
        F.Close()

        INETPOST = Res
    End Function

    Public Function INETRequestOnly(ByVal URL As String, Optional ByVal exHeader As String = "") As String
        Dim F As frmINet, Res As String
        F = New frmINet
        If INETTimeout = 0 Then INETTimeout = INETTimeout_Default
        F.inet.RequestTimeout = INETTimeout
        Res = F.MakeRequestOnly(URL, exHeader)
        INETTimeout = INETTimeout_Default
        'Unload F
        F.Close()

        INETRequestOnly = Res
    End Function

End Module
