﻿Imports QBFC10Lib
Module modQuickBooks_CommQBFC6
    Private mQBSM6 As QBFC6Lib.QBSessionManager
    Private mMsReq6 As QBFC6Lib.IMsgSetRequest
    Private mMsRsp6 As QBFC6Lib.IMsgSetResponse
    Private mQBActiveStore As Integer, mQBAS_JustSet As Boolean

    Public Function QB6_VendorQuery_Vendor(ByRef Vendor As String, Optional ByRef RET As Integer = 0, Optional ByRef RetMsg As String = "") As QBFC6Lib.IVendorRet
        Dim VQ As QBFC6Lib.IVendorQuery

        On Error Resume Next
        VQ = MSREQ6.AppendVendorQueryRq
        With VQ
            .ORVendorListQuery.FullNameList.Add(Vendor)
            '    .ORVendorListQuery.VendorListFilter.ORNameFilter.NameFilter.Name.SetValue Vendor
        End With
        VQ = Nothing

        QB6_VendorQuery_Vendor = QB6_SendRequestsGetRet(RET, RetMsg)
        Exit Function
    End Function

    Public ReadOnly Property MSREQ6() As QBFC6Lib.IMsgSetRequest
        Get
            If mMsReq6 Is Nothing Then
                mMsReq6 = QBSM6.CreateMsgSetRequest(QB_Country, QB_XML_MajorVer, QB_XML_MinorVer)
            End If
            MSREQ6 = mMsReq6
        End Get
    End Property

    Public ReadOnly Property QBSM6() As QBFC6Lib.QBSessionManager
        Get
            If mQBSM6 Is Nothing Then
                mQBSM6 = New QBFC6Lib.QBSessionManager
            End If
            QBSM6 = mQBSM6
        End Get
    End Property

    Public Function QB6_SendRequestsGetRet(Optional ByRef RET As Integer = 0, Optional ByRef RetMsg As String = "")
        Dim RL As Object, En As Integer, ES As String
        If QB6_SendRequests(ES, En) Then
            RET = StatusCode
            RetMsg = StatusMsg
            If RET = 0 Then
                If Not Resp.Detail Is Nothing Then
                    On Error Resume Next
                    QB6_SendRequestsGetRet = Resp.Detail
                    RL = Resp.Detail
                    QB6_SendRequestsGetRet = RL.GetAt(0)
                    RL = Nothing
                End If
            End If
        Else
            RetMsg = ES
            RET = -1
            Select Case En
                Case -2147220458
                    RET = -2
                Case -2147220445
                    MessageBox.Show("(" & Hex(En) & ") " & ES)
                Case Else
                    MessageBox.Show("(" & Hex(En) & ") " & ES)
            End Select
        End If
        QBSM6_Reset()
    End Function

    Public Sub QBSM6_Reset()
        '  Set mQBSM = Nothing
        ActiveLog("QB::QBSM6_Reset", 3)
        If mQBAS_JustSet Or POMode("EDIT", "REC") Then
            mQBAS_JustSet = False
        Else
            QBActiveStore = 0
        End If

        MsReq6_Reset()
    End Sub

    Public Sub MsReq6_Reset()
        mMsReq6 = Nothing
        mMsRsp6 = Nothing
    End Sub

    Private ReadOnly Property Resp() As QBFC6Lib.IResponse
        Get
            On Error Resume Next
            Resp = Response(0)
        End Get
    End Property

    Public ReadOnly Property ResponseList6() As QBFC6Lib.IResponseList
        Get
            If Not MSRSP6 Is Nothing Then
                ResponseList6 = MSRSP6.ResponseList
            End If
        End Get
    End Property

    Private ReadOnly Property Response(Index As Integer) As QBFC6Lib.IResponse
        Get
            On Error Resume Next
            Response = ResponseList6.GetAt(Index)
        End Get
    End Property

    Private ReadOnly Property StatusCode() As Integer
        Get
            On Error Resume Next
            StatusCode = Resp.StatusCode
        End Get
    End Property

    Private ReadOnly Property StatusMsg(Optional LDesc As String = "") As String
        Get
            On Error Resume Next
            StatusMsg = Resp.StatusMessage
        End Get
    End Property

    Public Function QB6_CustomerQuery_Name(ByRef Name As String, Optional ByRef RET As Integer = 0, Optional ByRef RetMsg As String = "") As QBFC6Lib.ICustomerRet
        On Error Resume Next
        Dim Q As QBFC6Lib.ICustomerQuery
        QBSM6_Reset()
        Q = MSREQ6.AppendCustomerQueryRq
        With Q
            Q.ORCustomerListQuery.FullNameList.Add(Name)
            '    Q.ORCustomerListQuery.CustomerListFilter.ORNameFilter.NameFilter.Name.SetValue Name
        End With
        Q = Nothing

        QB6_CustomerQuery_Name = QB6_SendRequestsGetRet(RET, RetMsg)
    End Function

    Public Function QB6_AccountQuery_All(Optional ByRef RET As Integer = 0, Optional ByRef RetMsg As String = "")
        On Error Resume Next
        QBSM6_Reset()
        MSREQ6.AppendAccountQueryRq() ' nothing to set
        QB6_AccountQuery_All = QB6_SendRequestsGetRetList(RET, RetMsg)
    End Function

    Public Function QB6_SendRequestsGetRetList(Optional ByRef RET As Integer = 0, Optional ByRef RetMsg As String = "")
        Dim RL As Object
        Dim T() As Object, N As Integer, En As Integer, ES As String
        If QB_SendRequests(ES, En) Then
            RET = StatusCode
            RetMsg = StatusMsg
            If RET = 0 Then
                If (Not Resp.Detail Is Nothing) Then
                    RL = Resp.Detail
                    ReDim T(RL.Count - 1)
                    For N = 0 To RL.Count - 1
                        T(N) = RL.GetAt(N)
                    Next
                End If
            End If
        Else
            RetMsg = ES
            If En = -2147220458 Then
                RET = -2
            Else
                RET = -1
            End If
        End If
        QB6_SendRequestsGetRetList = T
        QBSM6_Reset()
    End Function

    Public Function QB6_VendorQuery_All(Optional ByRef RET As Integer = 0)
        Dim ES As String
        On Error Resume Next
        QBSM6_Reset()
        MSREQ6.AppendVendorQueryRq() ' nothing to set
        QB6_VendorQuery_All = QB6_SendRequestsGetRetList(RET, ES)
    End Function

    Public Function QB6ObjectsExist(Optional ByRef Msg As String = "") As Boolean
        Dim X, Y
        On Error GoTo FailPoint
        Msg = "Could not create session manager (QBFC6)"
        X = New QBFC6Lib.QBSessionManager
        '  Set X = CreateObject("QBFC6.QBSessionManager")
        Msg = "Could not create MsgSetRequest (QBFC6)"
        Y = X.CreateMsgSetRequest(QB_Country, QB_XML_MajorVer, QB_XML_MinorVer)
        Msg = ""
        QB6ObjectsExist = True
FailPoint:
        If Msg <> "" Then Msg = Msg & ": " & Err.Description
        Y = Nothing
        X = Nothing
    End Function

    Public Function QB6_SendRequests(Optional ByRef ErrString As String = "", Optional ByRef ErrNo As Integer = 0, Optional ByRef OnErr As ENRqOnError = ENRqOnError.roeContinue) As Boolean
        On Error GoTo NoComm
        If Not QB6Startup(ErrNo, ErrString) Then Exit Function
        MSREQ6.Attributes.OnError = OnErr
        MSRSP6 = QBSM6.DoRequests(MSREQ6)
        '  Debug.Print mmsrsp6.ToXMLString

        QB6_SendRequests = True
        Exit Function ' no more auto-cleanup... persistant session

NoComm:
        ErrString = Err.Description
        ErrNo = Err.Number
        Err.Clear()
        QBShutdown()
    End Function

    Private Property MSRSP6() As QBFC6Lib.IMsgSetResponse
        Get
            MSRSP6 = mMsRsp6
        End Get
        Set(value As QBFC6Lib.IMsgSetResponse)
            mMsRsp6 = value
        End Set
    End Property

    Public Function QB6Startup(Optional ByRef RET As Integer = 0, Optional ByRef Msg As String = "") As Boolean
        Dim P As frmProgress
        Dim E As String
        On Error GoTo NoComm
        ActiveLog("QB::QBStartup", 1)
        If Not QBConnOpen Then
            P = New frmProgress
            P.Progress(0, , "Opening Connection to Quickbooks...", True, False)
            If QBUseRDS() Then
                QBSM6.OpenConnection2(QB_AppID, QB_AppNm, QBFC6Lib.ENConnectionType.ctRemoteQBD)
            Else
                QBSM6.OpenConnection2(QB_AppID, QB_AppNm, QBFC6Lib.ENConnectionType.ctLocalQBD)
            End If
            QBConnOpen = True
        End If
        If Not QBSessOpen Then
            If QBUseRDS() Then
                '      QBSM6.BeginSession "", omDontCare 'BFH20150325 - Added the file.. If you don't want it, leave it blank in the form
                QBSM6.BeginSession(QB_File, QBFC6Lib.ENOpenMode.omDontCare)
            Else
                QBSM6.BeginSession(QB_File, QBFC6Lib.ENOpenMode.omDontCare)
            End If
            QBSessOpen = True
        End If
        QB6Startup = True
        If Not P Is Nothing Then P.ProgressClose() : 
        P = Nothing
        Exit Function
NoComm:
        RET = Err.Number
        Msg = Err.Description
        Err.Clear()
        QB6Shutdown()
        QB6Startup = False
        If Not P Is Nothing Then P.ProgressClose() : 
        P = Nothing
    End Function

    Public Function QB6Shutdown()
        ActiveLog("QB::QBShutdown", 1)
        If QBSessOpen Then QBSM6.EndSession() : QBSessOpen = False
        If QBConnOpen Then QBSM6.CloseConnection() : QBConnOpen = False
        mQBSM6 = Nothing
    End Function

    Public Function QB6CreateDeposit(
    Optional ByVal TxnDate As String = "",
    Optional ByVal AcctRef_ListID As String = "", Optional ByVal AcctRef_FullName As String = "",
    Optional ByVal FromAccount_ListID As String = "", Optional ByVal FromAccount_FullName As String = "",
    Optional ByVal Memo As String = "",
    Optional ByVal CheckNumber As String = "",
    Optional ByVal PayMethRef_ListID As String = "", Optional ByVal PayMethRef_FullName As String = "",
    Optional ByVal Amount As String = "",
    Optional ByVal EntityRef_ListID As String = "", Optional ByVal EntityRef_FullName As String = "",
    Optional ByVal ClassRef_ListID As String = "", Optional ByVal ClassRef_FullName As String = "") As Boolean
        Dim R As Integer, M As String
        QBSM6_Reset()
        QB6_AppendDespositAdd _
      (TxnDate, AcctRef_ListID, AcctRef_FullName, FromAccount_ListID, FromAccount_FullName,
      Memo, CheckNumber, PayMethRef_ListID, PayMethRef_FullName,
      Amount, EntityRef_ListID, EntityRef_FullName, ClassRef_ListID, ClassRef_FullName)

        QB6CreateDeposit = (QB6_SendRequestsSingle(R, M) <> 0) And R = 0
        QBSM6_Reset()
    End Function

    Public Function QB6_SendRequestsSingle(Optional ByRef RET As Integer = 0, Optional ByRef RetMsg As String = "", Optional ByVal Notify As Boolean = True) As Integer
        Dim RR As QBFC6Lib.IResponse, II As Integer

        If QB6_SendRequests(RetMsg, RET, ENRqOnError.roeContinue) Then
            For II = 0 To ResponseCount - 1
                RR = ResponseList6.GetAt(II)
                If RR.StatusCode <> 0 Then
                    RetMsg = RetMsg & IIf(Len(RetMsg) > 0, vbCrLf, "")
                    RetMsg = RetMsg & "Error processing " & RR.Type.GetAsString & ": " & RR.StatusMessage
                End If
            Next
            If Len(RetMsg) > 0 Then
                If Notify Then
                    If MsgBox("Error(s): " & vbCrLf & RetMsg, vbInformation, "Error(s)", , , 6) = vbCancel Then
                        QB6_SendRequestsSingle = -1
                        Exit Function
                    End If
                End If
            Else
                QB6_SendRequestsSingle = QB6_SendRequestsSingle + 1
            End If
        Else
            If Notify Then MessageBox.Show("Error communicating with QuickBooks (nothing done):" & vbCrLf & RetMsg)
        End If
    End Function

    Public Function QB6_AppendDespositAdd(
    Optional ByVal TxnDate As String = "",
    Optional ByVal AcctRef_ListID As String = "", Optional ByVal AcctRef_FullName As String = "",
    Optional ByVal FromAccount_ListID As String = "", Optional ByVal FromAccount_FullName As String = "",
    Optional ByVal Memo As String = "",
    Optional ByVal CheckNumber As String = "",
    Optional ByVal PayMethRef_ListID As String = "", Optional ByVal PayMethRef_FullName As String = "",
    Optional ByVal Amount As String = "",
    Optional ByVal EntityRef_ListID As String = "", Optional ByVal EntityRef_FullName As String = "",
    Optional ByVal ClassRef_ListID As String = "", Optional ByVal ClassRef_FullName As String = "") As Boolean
        On Error Resume Next
        Err.Clear()
        Dim Dep As QBFC6Lib.IDepositAdd
        Dep = MSREQ6.AppendDepositAddRq
        With Dep
            If AcctRef_ListID = "" And AcctRef_FullName = "" Then
                Err.Raise(-1, , "Must specify customer for invoice")
            End If
            If FromAccount_ListID = "" And FromAccount_FullName = "" Then
                Err.Raise(-1, , "Must specify customer for invoice")
            End If

            IfNNSetValue(.TxnDate, TxnDate)
            IfNNSetValue(.DepositToAccountRef.ListID, AcctRef_ListID)
            IfNNSetValue(.DepositToAccountRef.FullName, AcctRef_FullName)
            IfNNSetValue(.Memo, Memo)

            With .DepositLineAddList.Append.ORDepositLineAdd.DepositInfo
                IfNNSetValue(.EntityRef.ListID, EntityRef_ListID)
                IfNNSetValue(.EntityRef.FullName, EntityRef_FullName)
                IfNNSetValue(.AccountRef.ListID, FromAccount_ListID)
                IfNNSetValue(.AccountRef.FullName, FromAccount_FullName)
                IfNNSetValue(.Memo, Memo)
                IfNNSetValue(.CheckNumber, CheckNumber)
                IfNNSetValue(.PaymentMethodRef.ListID, PayMethRef_ListID)
                IfNNSetValue(.PaymentMethodRef.FullName, PayMethRef_FullName)
                IfNNSetValue(.ClassRef.ListID, ClassRef_ListID)
                IfNNSetValue(.ClassRef.FullName, ClassRef_FullName)
                IfNNSetValue(.Amount, Amount)
            End With
        End With
        Dep = Nothing
        QB6_AppendDespositAdd = True
    End Function

    Public Function QB6CreateJournalEntry(
    Optional ByVal TxnDate As String = "", Optional ByVal RefNumber As String = "",
    Optional ByVal Memo As String = "", Optional ByVal IsAdjustment As String = "",
    Optional ByVal DebitTxnLineID As String = "",
    Optional ByVal DebitAccountRef_ListID As String = "", Optional ByVal DebitAccountRef_FullName As Object = "",
    Optional ByVal DebitAmount As String = "", Optional ByVal DebitMemo As String = "",
    Optional ByVal DebitEntityRef_ListID As String = "", Optional ByVal DebitEntityRef_FullName As Object = "",
    Optional ByVal DebitClassRef_ListID As String = "", Optional ByVal DebitClassRef_FullName As Object = "",
    Optional ByVal CreditTxnLineID As String = "",
    Optional ByVal CreditAccountRef_ListID As String = "", Optional ByVal CreditAccountRef_FullName As Object = "",
    Optional ByVal CreditAmount As String = "", Optional ByVal CreditMemo As String = "",
    Optional ByVal CreditEntityRef_ListID As String = "", Optional ByVal CreditEntityRef_FullName As Object = "",
    Optional ByVal CreditClassRef_ListID As String = "", Optional ByVal CreditClassRef_FullName As Object = "") As Boolean

        Dim E As Integer, S As String
        QBSM6_Reset()

        QB6_AppendJournalEntryAdd _
      (TxnDate, RefNumber, Memo, IsAdjustment,
      DebitTxnLineID, DebitAccountRef_ListID, DebitAccountRef_FullName,
      DebitAmount, DebitMemo, DebitEntityRef_ListID, DebitEntityRef_FullName,
      DebitClassRef_ListID, DebitClassRef_FullName,
      CreditTxnLineID, CreditAccountRef_ListID, CreditAccountRef_FullName,
      CreditAmount, CreditMemo, CreditEntityRef_ListID, CreditEntityRef_FullName,
      CreditClassRef_ListID, CreditClassRef_FullName)

        QB6CreateJournalEntry = (QB6_SendRequestsSingle(E, S) <> 0) And E = 0
        QBSM6_Reset()
    End Function

    Public Function QB6_AppendJournalEntryAdd(
    Optional ByVal TxnDate As String = "", Optional ByVal RefNumber As String = "",
    Optional ByVal Memo As String = "", Optional ByVal IsAdjustment As String = "",
    Optional ByVal DebitTxnLineID As String = "",
    Optional ByVal DebitAccountRef_ListID As String = "", Optional ByVal DebitAccountRef_FullName As Object = "",
    Optional ByVal DebitAmount As String = "", Optional ByVal DebitMemo As String = "",
    Optional ByVal DebitEntityRef_ListID As String = "", Optional ByVal DebitEntityRef_FullName As Object = "",
    Optional ByVal DebitClassRef_ListID As String = "", Optional ByVal DebitClassRef_FullName As Object = "",
    Optional ByVal CreditTxnLineID As String = "",
    Optional ByVal CreditAccountRef_ListID As String = "", Optional ByVal CreditAccountRef_FullName As Object = "",
    Optional ByVal CreditAmount As String = "", Optional ByVal CreditMemo As String = "",
    Optional ByVal CreditEntityRef_ListID As String = "", Optional ByVal CreditEntityRef_FullName As Object = "",
    Optional ByVal CreditClassRef_ListID As String = "", Optional ByVal CreditClassRef_FullName As Object = "") As Boolean

        On Error Resume Next
        Err.Clear()
        Dim Jrn As QBFC6Lib.IJournalEntryAdd
        Jrn = MSREQ6.AppendJournalEntryAddRq
        With Jrn
            IfNNSetValue(.TxnDate, TxnDate)
            IfNNSetValue(.RefNumber, RefNumber)
            IfNNSetValue(.Memo, Memo)
            IfNNSetValue(.IsAdjustment, IsAdjustment)
            Dim L As QBFC6Lib.IORJournalLine

            If DebitTxnLineID <> "" Or DebitAccountRef_ListID <> "" Or DebitAccountRef_FullName <> "" Or DebitAmount <> "" Or DebitMemo <> "" Or DebitEntityRef_ListID <> "" Or DebitEntityRef_FullName <> "" Or DebitClassRef_ListID <> "" Or DebitClassRef_FullName <> "" Then
                L = .ORJournalLineList.Append
                With L.JournalDebitLine
                    IfNNSetValue(.TxnLineID, DebitTxnLineID)
                    IfNNSetValue(.AccountRef.ListID, DebitAccountRef_ListID)
                    IfNNSetValue(.AccountRef.FullName, DebitAccountRef_FullName)
                    IfNNSetValue(.Amount, DebitAmount)
                    IfNNSetValue(.Memo, DebitMemo)
                    IfNNSetValue(.EntityRef.ListID, DebitEntityRef_ListID)
                    IfNNSetValue(.EntityRef.FullName, DebitEntityRef_FullName)
                    IfNNSetValue(.ClassRef.ListID, DebitClassRef_ListID)
                    IfNNSetValue(.ClassRef.FullName, DebitClassRef_FullName)
                End With
                L = Nothing
            End If
            If CreditTxnLineID <> "" Or CreditAccountRef_ListID <> "" Or CreditAccountRef_FullName <> "" Or CreditAmount <> "" Or CreditMemo <> "" Or CreditEntityRef_ListID <> "" Or CreditEntityRef_FullName <> "" Or CreditClassRef_ListID <> "" Or CreditClassRef_FullName <> "" Then
                L = .ORJournalLineList.Append
                With L.JournalCreditLine
                    IfNNSetValue(.TxnLineID, CreditTxnLineID)
                    IfNNSetValue(.AccountRef.ListID, CreditAccountRef_ListID)
                    IfNNSetValue(.AccountRef.FullName, CreditAccountRef_FullName)
                    IfNNSetValue(.Amount, CreditAmount)
                    IfNNSetValue(.Memo, CreditMemo)
                    IfNNSetValue(.EntityRef.ListID, CreditEntityRef_ListID)
                    IfNNSetValue(.EntityRef.FullName, CreditEntityRef_FullName)
                    IfNNSetValue(.ClassRef.ListID, CreditClassRef_ListID)
                    IfNNSetValue(.ClassRef.FullName, CreditClassRef_FullName)
                End With
            End If
            L = Nothing
        End With
        Jrn = Nothing
        QB6_AppendJournalEntryAdd = True
    End Function

End Module
