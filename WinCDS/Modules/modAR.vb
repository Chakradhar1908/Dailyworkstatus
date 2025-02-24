﻿Module modAR
    '::::modAR
    ':::SUMMARY
    ': Functions for the general performance of the A/R module.
    ':::DESCRIPTION
    '
    '
    '$1.00 Grace..  Some stores may not want this, in which case 0 zere be used as a property instead
    Public Const LateGraceAmt As Decimal = 1.0#

    ' AR Status Constants
    Public Const arST_Open As String = "O"   ' Open
    Public Const arST_Clos As String = "C"    ' Closed
    Public Const arST_Void As String = "V"    ' Void
    Public Const arST_Writ As String = "W"    ' Write Off
    Public Const arST_Repo As String = "R"    ' Repo
    Public Const arST_Lega As String = "L"    ' Legal
    Public Const arST_Bank As String = "Y"    ' Bankruptcy

    ' AR Status Descriptions
    Public Const arSD_O As String = "Open"
    Public Const arSD_C As String = "Closed"
    Public Const arSD_V As String = "Void"
    Public Const arSD_W As String = "Write Off"
    Public Const arSD_R As String = "Repo"
    Public Const arSD_L As String = "Legal"
    Public Const arSD_Y As String = "Bankruptcy"


    ' [Transactions].[Type], MaxLength=50...
    ' Len50 looks like this:".................................................."
    ' But, we probably want to keep it a little bit shorter...
    Public Const arPT_New As String = "NewSale"
    Public Const arPT_Prv As String = "Previous Payments"
    Public Const arPT_PLC As String = "Previous L/C"
    Public Const arPT_L_C As String = "Late Charge"
    Public Const arPT_Mem As String = "Memo"
    Public Const arPT_PrB As String = "Previous Balance"

    Public Const arPT_stReO As String = "A/R Re-opened"
    Public Const arPT_stClo As String = "A/R Closed"
    Public Const arPT_stVoi As String = "A/R Void"
    Public Const arPT_stWtO As String = "A/R Write Off"
    Public Const arPT_stRep As String = "A/R Repo"
    Public Const arPT_stLeg As String = "A/R Legal"
    Public Const arPT_stBkr As String = "A/R Bankruptcy"

    Public Const arPT_Doc As String = "Doc Fees"
    Public Const arPT_Lif As String = "Life Ins."
    Public Const arPT_Acc As String = "Acc. Ins."
    Public Const arPT_Pro As String = "Prop. Ins."
    Public Const arPT_IUI As String = "IUI Ins."
    Public Const arPT_Int As String = "Interest Chg."
    Public Const arPT_Tax As String = "Int. Sls Tax"
    Public Const arPT_PTI As String = "Post Term Interest"

    Public Const arPT_crDoc As String = "Doc Credit"
    Public Const arPT_crPri As String = "Principal Credit"
    Public Const arPT_crL_C As String = "L/C Credit"
    Public Const arPT_crInt As String = "Interest Credit"
    Public Const arPT_crLif As String = "Life Credit"
    Public Const arPT_crAcc As String = "Acc. Credit"
    Public Const arPT_crPro As String = "Prop. Credit"
    Public Const arPT_crIUI As String = "IUI Credit"
    Public Const arPT_crTax As String = "Sales Tax Credit"

    Public Const arPT_crDoc2 As String = "Credit Doc"
    Public Const arPT_crPri2 As String = "Credit Princ."
    Public Const arPT_crL_C2 As String = "Credit L/C"
    Public Const arPT_crInt2 As String = "Credit Interest"
    Public Const arPT_crLif2 As String = "Credit Life"
    Public Const arPT_crAcc2 As String = "Credit Accident"
    Public Const arPT_crPro2 As String = "Credit Property"
    Public Const arPT_crIUI2 As String = "Credit IUI"
    Public Const arPT_crTax2 As String = "Credit Sales Tax"

    Public Const arPT_dbPri As String = "Principal Debit"
    Public Const arPT_dbL_C As String = "L/C Debit"
    Public Const arPT_dbInt As String = "Interest Debit"

    Public Const arPT_dbPri2 As String = "Debit Prin."
    Public Const arPT_dbL_C2 As String = "Debit L/C"
    Public Const arPT_dbInt2 As String = "Debit Interest"

    Public Const arPT_poDoc As String = "Doc Payoff"
    Public Const arPT_poPri As String = "Principal Payoff"
    Public Const arPT_poL_C As String = "L/C Payoff"
    Public Const arPT_poInt As String = "Interest Payoff"
    Public Const arPT_poLif As String = "Life Payoff"
    Public Const arPT_poAcc As String = "Acc. Payoff"
    Public Const arPT_poPro As String = "Prop. Payoff"
    Public Const arPT_poIUI As String = "IUI Payoff"
    Public Const arPT_poTax As String = "Sls. Tax Payoff"

    Public Const arPT_pyCash As String = "Cash"
    Public Const arPT_pyChck As String = "Check"
    Public Const arPT_pyDebt As String = "Debit Card"
    Public Const arPT_pyVisa As String = "Visa"
    Public Const arPT_pyMCrd As String = "Master CArd"
    Public Const arPT_pyDisc As String = "Discover"
    Public Const arPT_pyAmex As String = "AMEX"

    Public Const arPT_pyL_C As String = "L/C Payment"

    Public Const ArAddOn_Nil As String = ""
    Public Const ArAddOn_New As String = "New"
    Public Const ArAddOn_Add As String = "Add"
    Public Const ArAddOn_AdT As String = "AddToNew"
    Public Const ArAddOn_Rev As String = "Revolving"


    Public Const ArPayoffMethod_Rule_78 As String = "Rule 78"
    Public Const ArPayoffMethod_Rule78b As String = "Rule78b"
    Public Const ArPayoffMethod_ProRata As String = "ProRata"
    Public Const ArPayoffMethod_Anticip As String = "Anticip"

    Public Function AccountHasRecentSaleNotes(ByVal SaleNo As String) As Boolean
        '::::AccountHasRecentSaleNotes
        ':::SUMMARY
        ': Used to check whether the Customer account has recent Sale notes
        ':::DESCRIPTION
        ': This function checks whether the customer account has recent Sale Notes using Sale number
        ':::PARAETERS
        ': - SaleNo
        ':::RETURN
        ': Boolean
        Dim RS As ADODB.Recordset
        On Error Resume Next
        RS = GetRecordsetBySQL("SELECT Count(*) AS Cnt FROM SaleNotes WHERE datediff('d',NoteDate,Date()) Between 0 and 31 AND BillOSale=""" & ProtectSQL(SaleNo) & """")
        AccountHasRecentSaleNotes = (RS("Cnt").Value > 0)
        RS.Close()
        RS = Nothing
    End Function

    Public Function AccountHasRecentARNotes(ByVal MailIndex As Integer) As Boolean
        '::::AccountHasRecentARNotes
        ':::SUMMARY
        ': Check if customer has recent AR notes.
        ':::DESCRIPTION
        ': This function checks whether the customer account has recent Ar Notes.
        ':::PARAMETERS
        ': - MailIndex
        ':::RETURN
        ': Boolean
        Dim RS As ADODB.Recordset
        On Error Resume Next
        RS = GetRecordsetBySQL("SELECT Count(*) AS Cnt FROM ARNotes INNER JOIN InstallmentInfo on ARNotes.ArNo=InstallmentInfo.ArNo WHERE datediff('d',NoteDate,Date()) between 0 and 31 AND InstallmentInfo.MailIndex=" & ProtectSQL(MailIndex))
        AccountHasRecentARNotes = (RS("Cnt").Value > 0)
        RS.Close()
        RS = Nothing
    End Function

    Public ReadOnly Property UseAmericanNationalInsurance() As Boolean
        Get
            '  UseAmericanNationalInsurance = UseAmericanNationalInsurance Or IsTreehouse ' Treehouse doesn't use this anymore
            '  UseAmericanNationalInsurance = UseAmericanNationalInsurance Or IsBlueSky
            UseAmericanNationalInsurance = UseAmericanNationalInsurance Or IsMcClure
        End Get
    End Property

    Public ReadOnly Property UseAlabamaSection5_19_3() As Boolean
        Get
            UseAlabamaSection5_19_3 = IsSidesFurniture
        End Get
    End Property

    Public ReadOnly Property AlabamaFinanceCharges(ByVal CA As Decimal) As Decimal
        Get
            AlabamaFinanceCharges = Max(AlabamaFinanceChargesMin(CA), AlabamaFinanceChargesMax(CA))
        End Get
    End Property

    Public ReadOnly Property AlabamaFinanceChargesMin(ByVal CA As Decimal) As Decimal
        Get
            AlabamaFinanceChargesMin = IIf(CA <= 25, 4, 6)
        End Get
    End Property

    Public ReadOnly Property AlabamaFinanceChargesMax(ByVal CA As Decimal) As Decimal
        Get
            AlabamaFinanceChargesMax = IIf(CA < 750, 15 * CA / 100, 15 * 750 / 100) + IIf(CA < 750, 0, 10 * Max(0, (CA - 750)) / 100)
        End Get
    End Property

    Public Function ArAddOnAccount(ByVal vArNo As String) As String
        '::::ArAddOnAccount
        ':::SUMMARY
        ': Generates the appropriate ArNo for an Add on Account
        ':::DESCRIPTION
        ': Adds/modfies ArNo suffix by augmenting the final letter.
        ':
        ': Central way to calculate the appropriate add on account number to use for an add on.  Does not create the add on.
        ':::PARAMETERS
        ': - ArNo
        ':::RETURN
        ': String
        ArAddOnAccount = AugmentByRightLetter(vArNo)
        Do While ArNoExists(ArAddOnAccount)
            ArAddOnAccount = AugmentByRightLetter(ArAddOnAccount)
        Loop
    End Function

    Public ReadOnly Property AdjustedGracePeriod(Optional ByVal nDay As Object = 10, Optional ByVal StoreNo As Integer = 0) As Integer
        Get
            On Error Resume Next
            If VarType(nDay) = vbDate Then
                nDay = DateAndTime.Day(nDay)
            End If

            nDay = CInt(Val(nDay))
            AdjustedGracePeriod = StoreSettings(StoreNo).GracePeriod
            If True Then
                AdjustedGracePeriod = AdjustedGracePeriod - IIf(HasGracePeriod And nDay = 1, 1, 0)
            End If
        End Get
    End Property

    Public Function ArNoExists(ByVal ArNo As String, Optional ByVal StoreNo As Integer = 0) As Boolean
        '::::ArNoExists
        ':::SUMMARY
        ': Returns whether an ArNo exists (in the given store)
        ':::DESCRIPTION
        ': Returns True if the ArNo exists
        ':::PARAMETERS
        ': - ArNo - Indicates the Ar number.
        ': - StoreNo - Indicates the  Store number.
        ':::RETURN
        ': Boolean

        Dim RS As ADODB.Recordset
        RS = GetRecordsetBySQL("SELECT * FROM [InstallmentInfo] WHERE ArNo=""" & ProtectSQL(ArNo) & """", False, GetDatabaseAtLocation(StoreNo))
        If Not RS.EOF Then ArNoExists = True
        RS.Close()
        RS = Nothing
    End Function

    Public ReadOnly Property HasGracePeriod(Optional ByVal StoreNo As Integer = 0) As Boolean
        Get
            HasGracePeriod = StoreSettings(StoreNo).GracePeriod > 0
        End Get
    End Property

    Public Function GetArNoLastPayment(ByVal ArNo As String, Optional ByRef LastPay As Decimal = 0, Optional ByRef LastPayType As String = "", Optional ByRef LastPayDate As String = "") As Boolean
        '::::GetArNoLastPayment
        ':::SUMMARY
        ': Retrieve information on last payment on AR Account
        ':::DESCRIPTION
        ': This function is used to get the information of last payment on specifed Ar Number related to customer.
        ':::PARAMETERS
        ': - ArNo
        ': - LastPay - Returns Last Payment Currency.
        ': - LastPayType - Returns Last Payment Type.
        ': - LastPayDate - Returns Last Payment Date.
        ':::RETURN
        ': Boolean
        Dim R As ADODB.Recordset, T As String
        R = GetRecordsetBySQL("SELECT * FROM Transactions WHERE ArNo='" & ProtectSQL(ArNo) & "' ORDER BY TransDate DESC,TransactionID DESC", , GetDatabaseAtLocation())
        Do While Not R.EOF
            T = IfNullThenNilString(R("Type"))
            If ArTypeIsNewSale(T) Then
                GetArNoLastPayment = False
                DisposeDA(R)
                Exit Function
            ElseIf ArTypeIsPayment(T) Then
                LastPay = IfNullThenZeroCurrency(R("Credits"))
                LastPayType = T
                LastPayDate = IfNullThenNilString(R("TransDate"))
                'Debug.Print "lastpaydate = " & LastPayDate
                GetArNoLastPayment = True
                DisposeDA(R)

                Exit Function
            End If

            R.MoveNext
        Loop
        GetArNoLastPayment = False
    End Function

    Public Function ArTypeIsNewSale(ByVal pt As String) As Boolean
        '::::ArTypeIsNewSale
        ':::SUMMARY
        ': Whether AR Type is a New Sale Line
        ':::DESCRIPTION
        ': Returns Whether AR Transaction type is a New Sale entry
        ':::PARAMETERS
        ': - pt - Payment Type
        ':::RETURN
        ': Boolean
        ArTypeIsNewSale = IsInOpt(pt, IsInOptions.isinIgnoreCase + IsInOptions.isinLeftCheck, arPT_New)
    End Function

    Public Function ArTypeIsPayment(ByVal pt As String) As Boolean
        '::::ArTypeIsPayment
        ':::SUMMARY
        ': Whether ARType is a payment
        ':::DESCRIPTION
        ': This function is used to check whether the Ar type is Payment
        ':::PARAMETER
        ': - pt - Payment Type
        ':::RETURN
        ': Boolean

        ArTypeIsPayment = IsInOpt(pt, IsInOptions.isinIgnoreCase, arPT_pyCash, arPT_pyChck, arPT_pyDebt, arPT_pyVisa, arPT_pyMCrd, arPT_pyDisc, arPT_pyAmex)
    End Function

    Public Function GetArNoContractDate(ByVal ArNo As String, Optional ByVal StoreNo As Integer = 0) As Date
        '::::GetArNoContractDate
        ':::SUMMARY
        ': Contract date from Ar number
        ':::DESCRIPTION
        ': This function is used to get Contract date from transactions table in descending manner using Ar number through Sql statements.
        ':::PARAMETERS
        ': - ArNo
        ': - StoreNo
        ':::RETURN
        ': Date
        Dim V As String
        V = GetValueBySQL("SELECT TOP 1 [TransDate] FROM [Transactions] WHERE [ArNo]='" & ProtectSQL(ArNo) & "' AND Left([Type], 7) = '" & arPT_New & "' ORDER BY [TransDate] DESC", , GetDatabaseAtLocation(StoreNo))

        If IsDate(V) Then
            GetArNoContractDate = DateValue(V)
        Else
            GetArNoContractDate = NullDate
        End If
    End Function

    Public Function GetArNoFirstContractDate(ByVal ArNo As String, Optional ByVal StoreNo As Integer = 0) As Date
        '::::GetArNoFirstContractDate
        ':::SUMMARY
        ': The first Contract date by ArNo.
        ':::DESCRIPTION
        ': "Earliest" contract date, since there can be innumerable Add-Ons
        ':::PARAMETERS
        ': - ArNo
        ': - StoreNo
        ':::RETURN
        ': Date - Returns the first contract date.

        Dim V As String
        V = GetValueBySQL("SELECT TOP 1 [TransDate] FROM [Transactions] WHERE [ArNo]='" & ProtectSQL(ArNo) & "' AND Left([Type], 7) = '" & arPT_New & "' ORDER BY [TransDate] ASC", , GetDatabaseAtLocation(StoreNo))

        If IsDate(V) Then
            GetArNoFirstContractDate = DateValue(V)
        Else
            GetArNoFirstContractDate = NullDate
        End If
    End Function

    Public Function ArTypeIsStatusChange(ByVal pt As String) As Boolean
        '::::ArTypeIsStatusChange
        ':::SUMMARY
        ': Whether Payment Type is a status Change event
        ':::DESCRIPTION
        ': This function is used to check whether the Ar type is Status Change
        ':::PARAMETER
        ': - pt - Payment Type
        ':::RETURN
        ': Boolean
        If ArTypeIsNewSale(pt) Then ArTypeIsStatusChange = True : Exit Function
        ArTypeIsStatusChange = IsInOpt(pt, IsInOptions.isinIgnoreCase, arPT_stReO, arPT_stClo, arPT_stVoi, arPT_stWtO, arPT_stRep, arPT_stLeg, arPT_stBkr)
    End Function

    Public Function ArTypeIsContract(ByVal pt As String) As Boolean
        '::::ArTypeIsContract
        ':::SUMMARY
        ': Used to check whether the Ar Type is Contract or not.
        ':::DESCRIPTION
        ': This function is used to check whether the Ar type is Contract
        ':::PARAMETER
        ': - pt - Payment Type
        ':::RETURN
        ': Boolean
        ArTypeIsContract = IsInOpt(pt, IsInOptions.isinIgnoreCase + IsInOptions.isinLeftCheck, arPT_New, arPT_Prv, arPT_PLC, arPT_Doc, arPT_Lif, arPT_Acc, arPT_Pro, arPT_IUI, arPT_Int, arPT_Tax, arPT_poDoc, arPT_poPri, arPT_poL_C, arPT_poInt, arPT_poLif, arPT_poAcc, arPT_poPro, arPT_poIUI, arPT_poTax)
    End Function

    Public Function ArTypeIsPayoff(ByVal pt As String) As Boolean
        '::::ArTypeIsPayoff
        ':::SUMMARY
        ': Used to check whether the Ar Type is Payoff or not.
        ':::DESCRIPTION
        ': This function is used to check whether the Ar type is Payoff event
        ':::PARAMETER
        ': - pt - Payment Type
        ':::RETURN
        ': Boolean
        ArTypeIsPayoff = IsInOpt(pt, IsInOptions.isinIgnoreCase, arPT_poPri, arPT_poPri, arPT_poL_C, arPT_poInt, arPT_poLif, arPT_poAcc, arPT_poPro, arPT_poIUI, arPT_poTax)
    End Function

    Public Function ArTypeIsLCAdjust(ByVal pt As String) As Boolean
        '::::ArTypeIsLCAdjust
        ':::SUMMARY
        ': Whether Payment Type is a Late Charge Adjust event
        ':::DESCRIPTION
        ': This function is used to check whether the Ar type is LCAdjust
        ':::PARAMETER
        ': - pt - Payment Type
        ':::RETURN
        ': Boolean
        ArTypeIsLCAdjust = IsIn(pt, arPT_L_C, arPT_crL_C, arPT_crL_C2, arPT_dbL_C2, arPT_poL_C, arPT_pyL_C)
    End Function

    Public Function GetArNoLastAddOnAccountNo(ByVal ArNo As String, Optional ByVal StoreNo As Integer = 0) As String
        '::::GetArNoLastAddOnAccountNo
        ':::SUMMARY
        ': Used to get Last added Account number using Ar number.
        ':::DESCRIPTION
        ': Gets the AddOnAccount number for the given Add On account.
        ':::PARAMETERS
        ': - ArNo
        ': - StoreNo
        ':::RETURN
        ': String - The associated AddOnAccount number
        GetArNoLastAddOnAccountNo = Trim("" & GetValueBySQL("SELECT TOP 1 [Receipt] FROM [Transactions] WHERE [ArNo]=""" & ProtectSQL(ArNo) & """ AND Left([Type], 7) = '" & arPT_New & "' ORDER BY [TransDate] DESC", , GetDatabaseAtLocation(StoreNo)))
    End Function

    Public Function GetArNoLastAddOnPHP(ByVal ArNo As String, Optional ByVal StoreNo As Integer = 0) As String
        '::::GetArNoLastAddOnPHP
        ':::SUMMARY
        ': Used to get the specified Ar number's Last Payment History Profile information related to customer.
        ':::DESCRIPTION
        ': This function is used to retrieve the Last added Payment History Profile on AR Number stored in the associated AddOnRecord account
        ':::PARAMETERS
        ': - ArNo
        ': - StoreNo
        ':::RETURN
        ': String - Returns the 24-character PHP
        GetArNoLastAddOnPHP = Trim("" & GetValueBySQL("SELECT [PaymentHistoryProfile] FROM [InstallmentInfo] WHERE [ArNo]=""" & ProtectSQL(ArNo) & """", , GetDatabaseAtLocation(StoreNo)))
    End Function

    Public Function GetSaleNoFromArNo(ByVal mArNo As String) As String
        '::::GetSaleNoFromArNo
        ':::SUMMARY
        ': Used to get Sale number from ArNo
        ':::DESCRIPTION
        ': Returns the Sale number associated with the AR No.
        ':::PARAMETERS
        ': - ArNo
        ':::RETURN
        ': String - Returns the sale number
        Dim H As cHolding
        H = New cHolding
        ' Note: This only gets one (most recent?) sale, and there may be many.
        If H.Load(mArNo, "ArNo") Then
            GetSaleNoFromArNo = H.LeaseNo
        Else
            GetSaleNoFromArNo = mArNo
        End If
        DisposeDA(H)
    End Function

    Public Function ArStatusComboValueFromAccountStatus(ByVal S As String) As String
        '::::ArStatusComboValueFromAccountStatus
        ':::SUMMARY
        ': Used to get Ar status Combo value from Account status.
        ':::DESCRIPTION
        ': This function is used to select Ar Status combo value from Account Status.
        ':::PARAMETERS
        ': - S - Indiactes Ar Status as a String.
        ':::RETURN
        ': String - Returns the result as a String.
        Select Case S
            Case arST_Open : ArStatusComboValueFromAccountStatus = arSD_O
            Case arST_Clos : ArStatusComboValueFromAccountStatus = arSD_C
            Case arST_Void : ArStatusComboValueFromAccountStatus = arSD_V
            Case arST_Writ : ArStatusComboValueFromAccountStatus = arSD_W
            Case arST_Repo : ArStatusComboValueFromAccountStatus = arSD_R
            Case arST_Lega : ArStatusComboValueFromAccountStatus = arSD_L
            Case arST_Bank : ArStatusComboValueFromAccountStatus = arSD_Y
        End Select
    End Function

    Public Sub LoadArStatusCombo(ByVal mCbo As ComboBox)
        '::::LoadArStatusCombo
        ':::SUMMARY
        ': Load Ar Status from given list in ComboBox.
        ':::DESCRIPTION
        ': This function is used to load list of Ar statuses listed in ComboBox.
        ':::PARAMETERS
        ': - mCbo
        ':::RETURN

        mCbo.Items.Clear()
        'mCbo.AddItem arSD_O, 0
        mCbo.Items.Insert(0, arSD_O)
        'mCbo.AddItem arSD_C, 1
        mCbo.Items.Insert(1, arSD_C)
        'mCbo.AddItem arSD_V, 2
        mCbo.Items.Insert(2, arSD_V)
        'mCbo.AddItem arSD_W, 3
        mCbo.Items.Insert(3, arSD_W)
        'mCbo.AddItem arSD_R, 4
        mCbo.Items.Insert(4, arSD_R)
        'mCbo.AddItem arSD_L, 5
        mCbo.Items.Insert(5, arSD_L)
        'mCbo.AddItem arSD_Y, 6
        mCbo.Items.Insert(6, arSD_Y)
        'mCbo.ListIndex = 0
        mCbo.SelectedIndex = 0
    End Sub
End Module

