﻿Public Class ArCard
    Private Const FRM_W As Integer = 10525
    Private Const FRM_H_MIN As Integer = 7290
    Private Const FRM_H_MAX As Integer = 13000

    Private WithEvents mDBAccess As CDbAccessGeneral
    Private WithEvents mDBAccessTransactions As CDbAccessGeneral

    Public PayCount As Integer
    Private PayLog() As PaymentRecord ' Array for tracking payments, for receipts

    Private altPayType As String    ' used for a couple specialty operations like payoffs
    Public ArNo As String
    Public Status As String
    Public MailRec As Integer
    Public DocCredit As Decimal
    Public LifeCredit As Decimal
    Public AccidentCredit As Decimal
    Public PropertyCredit As Decimal
    Public IUICredit As Decimal
    Public InterestCredit As Decimal
    Public InterestTaxCredit As Decimal

    ' These need to be eliminated, to speed up the whole program.
    'GDM Form variable to indicate that we are processing a move button
    Public Shared UsingMoveButton As Boolean
    Public f_strDirection As String 'GDM

    Private Const NON_ALERT_COLOR As Integer = &H6666CC
    Private Const ALERT_COLOR As Integer = &H6666CC ' '&HC0&

    Dim Mail As MailNew
    Dim Mail2 As MailNew2

    Dim mArNo As String
    Dim CustRec As Integer
    Dim CashOpt As Integer

    Public INTEREST As Decimal
    Public InterestTax As Decimal
    Public DocFee As Decimal
    Public Life As Decimal
    Public Accident As Decimal
    Public Prop As Decimal
    Public IUI As Decimal

    Dim Charges As Decimal
    Dim Credits As Decimal
    Dim Balance As Decimal
    Dim TotPaid As Decimal
    Dim Financed As Decimal
    Dim TransType As String
    Dim Payoff As String
    Dim PayoffSameAsCash As Boolean
    Dim StatusChg As String
    Dim Receipt As String
    Dim NewTypee As String

    Dim TransDate As String
    Dim LastPayDate As String
    Dim LastPay As String
    Dim LateChargeBal As String
    Dim Months As String
    Dim SendNotice As String
    Dim Counter As Integer

    Dim Approval As String

    Dim OpenFormAs As String
    Dim InterestDebit As Decimal  ' For Bankruptcy transactions
    Dim InterestCreditRevolving As Decimal

    Dim DoRecordAccountClosed As Boolean

    Dim cmdApplyValue As Boolean                 ' Used to determine whether this button has been clicked.
    Dim cmdReceiptValue As Boolean               ' Future Languages do not support command button value property
    Public PriorBal As Decimal

    Public Sub ShowArCardForDisplayOnly(ByVal nArNo As String, Optional ByVal Modal As Boolean = True, Optional ByVal AllowClose As Boolean = False, Optional ByVal AllowContractChange As Boolean = False)
        Dim OldAR As String
        If nArNo <> "" Then LoadArNo(nArNo)

        cmdApply.Enabled = AllowContractChange
        'cmdCreditApp.Enabled = False
        'cmdDetail.Enabled = False
        cmdEdit.Enabled = False
        cmdExport.Enabled = False
        cmdFields.Enabled = False
        cmdMakeSameAsCash.Enabled = False
        cmdMoveFirst.Enabled = False
        cmdMoveLast.Enabled = False
        cmdMoveNext.Enabled = False
        cmdMovePrevious.Enabled = False
        cmdPayoff.Enabled = False
        cmdReceipt.Enabled = False
        cmdReprintContract.Enabled = False
        'Notes_Open.Enabled = False
        cmdPrint.Enabled = False
        'cmdPrintCard.Enabled = False
        'cmdPrintLabel.Enabled = False
        cmdCancel.Enabled = AllowClose

        fraPaymentOptions.Visible = False
        fraEditOptions.Visible = False

        'Show IIf(Modal, 1, 0)
        If Modal = True Then
            Me.ShowDialog()
        Else
            Me.Show()
        End If

        If OldAR <> "" Then ArSelect = OldAR

        '  DisposeDA RS
    End Sub

    Public Function LoadArNo(Optional ByVal vArNo As String = "") As Boolean
        If IsRevolvingCharge(vArNo) Then DoRevolvingProcessAccount(Today, vArNo, False) ' in case interest is due..  applies interest, no statement until statement date

        mDBAccess_Init(vArNo)
        mDBAccess.GetRecord()    ' this gets the record
        mDBAccess.dbClose()
        mDBAccess = Nothing

        If mArNo <> "-1" Then 'not found
            GetCustomer()
            mDBAccessTransactions_Init(mArNo)
            mDBAccessTransactions.GetRecord()    ' this gets the record
            mDBAccessTransactions.dbClose()
            mDBAccessTransactions = Nothing
            GetPayoff
            GetAgeing
            filFile_Click
        End If

        txtPaymentHistory.Text = WrapLongText(GetArCreditHistory(mArNo, Today, 24), 12) ' GetPaymentHistorySimple(vArNo, Date, 24)
    End Function

    Private Sub mDBAccess_Init(ByVal Tid As String)
        Dim SQL As String

        mDBAccess = New CDbAccessGeneral
        mDBAccess.dbOpen(GetDatabaseAtLocation())
        SQL = ""
        SQL = SQL & "SELECT InstallmentInfo.*, ArApp.HisSS, ArApp.ApprovalTerms, ArApp.CreditLimit"
        SQL = SQL & " FROM InstallmentInfo LEFT JOIN ArApp ON (InstallmentInfo.MailIndex=Val(iif(isnull(ArApp.MailIndex),0,ArApp.MailIndex)))"
        SQL = SQL & " WHERE (Status<>'V')"  'BFH20080215 - ALLOW OPENING VOIDS || 20080305 - removed again
        '    SQL = SQL & " WHERE Status<>'V'"
        ' GDM BEGINNING OF CHANGE 3/29/2001
        ' Change the sql string based on the move option (f_strDirection)
        If UsingMoveButton Then
            Select Case f_strDirection
                Case "First"    'Get first record in Table
                    mDBAccess.SQL = SQL & " ORDER BY InstallmentInfo.ArNo"
                    Exit Sub
                Case "Last"     'Get current record and all records beyond
                    mDBAccess.SQL = SQL & " AND InstallmentInfo.ArNo  >=""" & ProtectSQL(Tid) & """ ORDER BY InstallmentInfo.ArNo"
                    Exit Sub
                Case "Previous" 'Get all records up to and including current record
                    mDBAccess.SQL = SQL & " AND InstallmentInfo.ArNo  <=""" & ProtectSQL(Tid) & """ ORDER BY InstallmentInfo.ArNo"
                    Exit Sub
                Case "Next"     'Get current record and next record only
                    mDBAccess.SQL = SQL & " AND InstallmentInfo.ArNo  >=""" & ProtectSQL(Tid) & """ ORDER BY InstallmentInfo.ArNo"
                    Exit Sub
            End Select
        End If
        ' GDM END OF CHANGE
        mDBAccess.SQL = SQL & " AND InstallmentInfo.ArNo  =""" & ProtectSQL(Tid) & """"
    End Sub

    Private Sub mDBAccessTransactions_GetRecordEvent(RS As ADODB.Recordset) Handles mDBAccessTransactions.GetRecordEvent   ' called if record is found
        Dim Row As Integer
        Dim Lastrow As Integer

        UGridIO1.Clear() 'IMP NOTE: This Clear method is to clear the ugridio1 data(rows and cols) using AxDataGrid1.ClearFields() in Clear method. But it is not working. So to clear it, below For loop is added. This for loop is not in vb6.0 code.

        Lastrow = UGridIO1.LastRowUsed
        For r = 0 To Lastrow
            UGridIO1.Row = r
            For c = 0 To 5
                On Error Resume Next
                UGridIO1.Col = c
                UGridIO1.Text = ""
            Next
        Next

        Do While Not RS.EOF
            TransDate = IfNullThenNilString(RS("TransDate").Value)
            TransType = IfNullThenNilString(RS("Type").Value)

            Charges = IfNullThenZeroCurrency(RS("Charges").Value)
            Credits = IfNullThenZeroCurrency(RS("Credits").Value)
            Balance = IfNullThenZeroCurrency(RS("Balance").Value)

            'bfh20080518 - Added "MasterCard" b/c CC processing has been omitting the space in card desc
            'bfh20080522 - Visa/VISA apparently as well..
            If IsIn(TransType, "Cash", "Check", "Visa", "VISA", "Master Card", "MasterCard", "Discover Card", "Amercian Exp.") Then
                LastPay = CurrencyFormat(Credits)
                LastPayDate = TransDate
            End If

            UGridIO1.SetValueDisplay(Row, 0, DateFormat(TransDate))
            UGridIO1.SetValueDisplay(Row, 1, TransType)
            UGridIO1.SetValueDisplay(Row, 2, CurrencyFormat(Charges))
            UGridIO1.SetValueDisplay(Row, 3, CurrencyFormat(Credits))
            UGridIO1.SetValueDisplay(Row, 4, CurrencyFormat(Balance))
            UGridIO1.SetValueDisplay(Row, 5, IfNullThenNilString(RS!Receipt))

            ' These lines deal with interest that's already been paid off.
            If TransType = arPT_poInt Then INTEREST = 0
            If Microsoft.VisualBasic.Left(TransType, 7) = arPT_New Then INTEREST = 0 ' added 12192007 jk If no interest charges on add on
            If TransType = arPT_Int Then INTEREST = Charges
            UGridIO1.Refresh()
            Row = Row + 1

            RS.MoveNext()
        Loop
    End Sub

    Public Sub GetCustomer()
        'MousePointer = 11
        Me.Cursor = Cursors.WaitCursor

        On Error GoTo HandleErr

        If ARPaySetUp.AccountFound = "Y" Then MailRec = ARPaySetUp.MailRec

        Dim RS As ADODB.Recordset
        RS = getRecordsetByTableLabelIndexNumber("Mail", "Index", CStr(MailRec))
        If (RS.RecordCount <> 0) Then CopyMailRecordsetToMailNew(RS, Mail)
        RS.Close()
        RS = Nothing

        CopyMailRecordsetToMailNew2(Nothing, Mail2)
        RS = getRecordsetByTableLabelIndexNumber("MailShipTo", "Index", CStr(Mail.Index))
        If (RS.RecordCount <> 0) Then CopyMailRecordsetToMailNew2(RS, Mail2)
        RS.Close()
        RS = Nothing

        GetCust
        ClearPayments()
        'MousePointer = 0
        Me.Cursor = Cursors.Default
        Exit Sub

        'Does Not Find Customer
        If MessageBox.Show("Name Not In Data Base:  Try Again?", "WinCDS", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes Then
            ' Retry
            Exit Sub
        End If
        Exit Sub

HandleErr:
        If Err.Number = 53 Then
            CustRec = "999"
            Resume Next
        ElseIf Err.Number = 52 Then
            Resume Next
        End If
    End Sub

    Private Sub ClearPayments()
        'ReDim PayLog(0)
        Erase PayLog
        PayCount = 0
    End Sub

    Private Sub mDBAccessTransactions_Init(Tid As String)
        mDBAccessTransactions = New CDbAccessGeneral
        mDBAccessTransactions.dbOpen(GetDatabaseAtLocation())
        mDBAccessTransactions.SQL =
            "SELECT Transactions.*" _
            & " From Transactions" _
            & " WHERE (((Transactions.ArNo)=""" & ProtectSQL(mArNo) & """))" _
            & " ORDER BY  Transactions.ArNo, Transactions.transactionId"
    End Sub

    Public Sub GetPayoff(Optional ByVal AsOfDate As Date = NullDate)
        Dim ppoLife As Boolean, ppoAcc As Boolean, ppoProp As Boolean, ppoIUI As Boolean
        Dim C As cArTreehouse, N As Integer, RN As Integer, A As Double, B As Double
        Dim LA As String, SAC As Boolean

        CheckNullDate(AsOfDate, PayoffAsOfDate)

        If IsRevolvingCharge(ArNo) Then
            Dim InstAcct As New cInstallment
            InstAcct.Load(ArNo)
            lblTotalPayoff.Text = CurrencyFormat(InstAcct.GetPayoffRevolving(AsOfDate))
            DisposeDA(InstAcct)
            Exit Sub
        End If

        InterestCredit = 0
        InterestTaxCredit = 0
        DocCredit = 0
        LifeCredit = 0
        AccidentCredit = 0
        PropertyCredit = 0
        IUICredit = 0

        GetPreviousPayoff(ArNo, ppoLife, ppoAcc, ppoProp, ppoIUI)

        If ARPaySetUp.AccountFound = "Y" Then
            INTEREST = ARPaySetUp.INTEREST
            Life = ARPaySetUp.Life
            Accident = ARPaySetUp.Accident
            Prop = ARPaySetUp.Prop
            IUI = ARPaySetUp.IUI
            InterestTax = ARPaySetUp.InterestTax
        End If

        If AlreadyMadeSameAsCash() Then
            DocCredit = 0
            LifeCredit = 0
            AccidentCredit = 0
            PropertyCredit = 0
            IUICredit = 0
            InterestCredit = 0
            InterestTaxCredit = 0
            lblTotalPayoff.Text = CurrencyFormat(GetPrice(lblBalance.Text))
            If AddOnAcc.Typee = ArAddOn_New Then ARPaySetUp.txtPrevBalance.Text = Format((GetPrice(lblBalance.Text)) - (LifeCredit) - (AccidentCredit) - (PropertyCredit) - (IUICredit) - (InterestCredit) - (InterestTaxCredit), "###,###.00")
            Exit Sub
        End If

        If CashOptPaidOff(True) And Status = "O" And Not AlreadyMadeSameAsCash() Then
            SAC = True    ' same as cash, for interest and interest tax
            If (Not PayInsuranceAfter30Days() Or DateDiff("d", DateValue(txtDelivery.Text), Today) <= 30) Then
                LifeCredit = Life
                AccidentCredit = Accident
                PropertyCredit = Prop
                IUICredit = IUI
                InterestCredit = INTEREST
                InterestTaxCredit = InterestTax
                lblTotalPayoff.Text = CurrencyFormat((GetPrice(lblBalance.Text)) - (LifeCredit) - (AccidentCredit) - (PropertyCredit) - (IUICredit) - (InterestCredit) - (InterestTaxCredit))
                If AddOnAcc.Typee = ArAddOn_New Then ARPaySetUp.txtPrevBalance.Text = Format((GetPrice(lblBalance.Text)) - (LifeCredit) - (AccidentCredit) - (PropertyCredit) - (IUICredit) - (InterestCredit) - (InterestTaxCredit), "###,###.00")
                Exit Sub
            End If
        End If


        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        '  If Val(DocFee) > 0 And getprice(lblBalance) > 0 Then
        '    DocCredit = ProRata(DocFee, Val(txtMonths), txtLastPay)
        '  End If

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        If Val(Life) > 0 And GetPrice(lblBalance.Text) > 0 Then
            Select Case LifePayoffMethod()
                Case ArPayoffMethod_ProRata : LifeCredit = ProRata(Life, Val(txtMonths.Text), txtLastPay.Text, AsOfDate)
                Case ArPayoffMethod_Rule_78 : LifeCredit = Rule78(DateValue(txtDelivery.Text), GetPrice(Life), Val(txtMonths.Text), , AsOfDate)
                Case ArPayoffMethod_Anticip
                    N = Val(txtMonths)
                    RN = CountMonths(AsOfDate, DateValue(txtLastPay.Text), True)
                    C = New cArTreehouse
                    A = C.LifeRate(N, False)
                    B = C.LifeRate(RN, False)
                    C = Nothing
                    LifeCredit = RuleOfAnticipationForTreehouse(GetPrice(txtFinanced.Text), GetPrice(txtMonthlyPayment.Text), Life, N, RN, A, B)

                    If txtPayMemo.Text = "*" And IsDevelopment() Then
                        LA = ""
                        LA = LA & "Orig Loan: " & txtFinanced.Text & vbCrLf
                        LA = LA & "Month Pmt: " & txtMonthlyPayment.Text & vbCrLf
                        LA = LA & "Orig Prem: " & Life & vbCrLf
                        LA = LA & "Orig Term: " & N & vbCrLf
                        LA = LA & "Remain Tm: " & RN & vbCrLf
                        LA = LA & "Orig Rate: " & A & vbCrLf
                        LA = LA & "Remain Rt: " & B & vbCrLf
                        LA = LA & vbCrLf
                        LA = LA & "LIFE CREDIT = " & LifeCredit & vbCrLf
                        'MsgBox LA
                        MessageBox.Show(LA)
                    End If

                Case Else : If IsDevelopment() Then MessageBox.Show("Unknown LifePayoffMethod: " & LifePayoffMethod())
            End Select
        End If
        If LifeCredit > Life Then LifeCredit = Life
        If MinLifeCredit() > 0 And GetPrice(LifeCredit) < MinLifeCredit() Then LifeCredit = 0  ' minimum refund amount
        If ppoLife Then LifeCredit = 0

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If Val(Accident) > 0 And GetPrice(lblBalance.Text) > 0 Then
            Select Case AccPayoffMethod()
                Case ArPayoffMethod_ProRata : AccidentCredit = ProRata(Accident, Val(txtMonths), txtLastPay.Text, AsOfDate)
                Case ArPayoffMethod_Rule_78 : AccidentCredit = Rule78(DateValue(txtDelivery.Text), Accident, Val(txtMonths.Text), , AsOfDate)
                Case ArPayoffMethod_Rule78b : AccidentCredit = Rule78(DateValue(txtDelivery.Text), Accident, Val(txtMonths.Text), True, AsOfDate)
                Case ArPayoffMethod_Anticip
                    N = Val(txtMonths)
                    RN = CountMonths(AsOfDate, DateValue(txtLastPay.Text), True)
                    C = New cArTreehouse
                    A = C.DisabilityRate(N)
                    B = C.DisabilityRate(RN)
                    C = Nothing
                    AccidentCredit = RuleOfAnticipationForTreehouse(GetPrice(txtFinanced.Text), GetPrice(txtMonthlyPayment.Text), Accident, N, RN, A, B)

                    If txtPayMemo.Text = "*" And IsDevelopment() Then
                        LA = ""
                        LA = LA & "Orig Loan: " & txtFinanced.Text & vbCrLf
                        LA = LA & "Month Pmt: " & txtMonthlyPayment.Text & vbCrLf
                        LA = LA & "Orig Prem: " & Accident & vbCrLf
                        LA = LA & "Orig Term: " & N & vbCrLf
                        LA = LA & "Remain Tm: " & RN & vbCrLf
                        LA = LA & "Orig Rate: " & A & vbCrLf
                        LA = LA & "Remain Rt: " & B & vbCrLf
                        LA = LA & vbCrLf
                        LA = LA & "ACC CREDIT = " & AccidentCredit & vbCrLf
                        'MsgBox LA
                        MessageBox.Show(LA)
                    End If

                Case Else : If IsDevelopment() Then MessageBox.Show("Unknown AccPayoffMethod: " & AccPayoffMethod())
            End Select
        End If
        If AccidentCredit > Accident Then AccidentCredit = Accident
        If MinAccCredit() > 0 And GetPrice(AccidentCredit) < MinAccCredit() Then AccidentCredit = 0  ' minimum refund amount
        If ppoAcc Then AccidentCredit = 0

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If Val(Prop) > 0 And GetPrice(lblBalance.Text) > 0 Then
            Select Case PropPayoffMethod()
                Case ArPayoffMethod_ProRata : PropertyCredit = ProRata(Prop, Val(txtMonths.Text), txtLastPay.Text, AsOfDate)
                Case ArPayoffMethod_Rule_78 : PropertyCredit = Rule78(DateValue(txtDelivery.Text), Prop, Val(txtMonths.Text), , AsOfDate)
                Case ArPayoffMethod_Rule78b : PropertyCredit = Rule78(DateValue(txtDelivery.Text), Prop, Val(txtMonths.Text), True, AsOfDate)
                Case Else : If IsDevelopment() Then MessageBox.Show("Unknown PropPayoffMethod: " & PropPayoffMethod())
            End Select
        End If
        If PropertyCredit > Prop Then PropertyCredit = Prop
        If MinPropCredit() > 0 And GetPrice(PropertyCredit) < MinPropCredit() Then PropertyCredit = 0  ' minimum refund amount
        If ppoProp Then PropertyCredit = 0

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If Val(IUI) > 0 And GetPrice(lblBalance.Text) > 0 Then
            Select Case IUIPayoffMethod()
                Case ArPayoffMethod_ProRata : IUICredit = ProRata(IUI, Val(txtMonths.Text), txtLastPay.Text, AsOfDate)
                Case ArPayoffMethod_Rule_78 : IUICredit = Rule78(DateValue(txtDelivery.Text), IUI, Val(txtMonths.Text), , AsOfDate)
                Case ArPayoffMethod_Rule78b : IUICredit = Rule78(DateValue(txtDelivery.Text), IUI, Val(txtMonths.Text), True, AsOfDate)
                Case Else : If IsDevelopment() Then MessageBox.Show("Unknown IUIPayoffMethod: " & IUIPayoffMethod())
            End Select
        End If
        If IUICredit > IUI Then IUICredit = IUI
        If MinIUICredit() > 0 And GetPrice(IUICredit) < MinIUICredit() Then IUICredit = 0  ' minimum refund amount
        If ppoIUI Then IUICredit = 0

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If CLng(GetPrice(lblBalance.Text)) <> 0 And GetPrice(lblBalance.Text) > 0 Then  'added 12-18-2002 for closed accounts
            If Val(INTEREST) >= 0 Then
                'InterestCredit = Rule78(DateValue(txtDelivery.Text), GetPrice(INTEREST), Val(txtMonths.Text), , AsOfDate)
                InterestCredit = Rule78(Date.Parse(DateValue(txtDelivery.Text)), GetPrice(INTEREST), Val(txtMonths.Text), , AsOfDate)
                'InterestCredit = Rule78(DateTime.ParseExact(DateValue(txtDelivery.Text), "MM-dd-yyyy", Nothing), GetPrice(INTEREST), Val(txtMonths.Text), , AsOfDate)
                'InterestCredit = Rule78(#09/23/2020#, GetPrice(INTEREST), Val(txtMonths.Text), , AsOfDate)
            End If
        End If

        If IsElmore And GetPrice(txtRate.Text) <> 0 Then
            '10% of Balance
            If (lblBalance.Text * 0.1) > 25.0# Then
                InterestCredit = InterestCredit - 25.0#
            Else
                InterestCredit = (lblBalance.Text * 0.1)
            End If
        End If
        If SAC Then InterestCredit = INTEREST
        If InterestCredit > INTEREST Then InterestCredit = INTEREST
        If InterestCredit < 0 Then InterestCredit = 0   'no negative numbers

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If CLng(GetPrice(lblBalance.Text)) <> 0 Then
            If Val(InterestTax) >= 0 Then
                InterestTaxCredit = ProRata(InterestTax, Val(txtMonths.Text), txtLastPay.Text, AsOfDate)  ' BFH20080703 - this should probably refund the full amount??
            End If
        End If
        If SAC Then InterestTaxCredit = InterestTax
        If InterestTaxCredit < 0 Then InterestTaxCredit = 0

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        lblTotalPayoff.Text = CurrencyFormat((GetPrice(lblBalance.Text)) - (LifeCredit) - (AccidentCredit) - (PropertyCredit) - (IUICredit) - (InterestCredit) - (InterestTaxCredit))
        If GetPrice(lblTotalPayoff.Text) < 0 Then lblTotalPayoff.Text = CurrencyFormat(0)

        If AddOnAcc.Typee = ArAddOn_New Then
            ARPaySetUp.txtPrevBalance.Text = Format((GetPrice(lblBalance.Text)) - (LifeCredit) - (AccidentCredit) - (PropertyCredit) - (IUICredit) - (InterestCredit) - (InterestTaxCredit), "###,###.00")
            'BFH20071219 - Negative Previous Balances Allowed...
            '    If ARPaySetUp.txtPrevBalance.Text < 0 Then ARPaySetUp.txtPrevBalance = ".00"
        End If
    End Sub

    Private Function MinIUICredit() As Decimal
        If IsBoyd Then MinIUICredit = 2
        If IsTreehouse Or IsBlueSky Then MinIUICredit = 1
    End Function

    Private Function IUIPayoffMethod() As String
        IUIPayoffMethod = ArPayoffMethod_Rule_78
        If IsTreehouse Or IsBlueSky Then IUIPayoffMethod = ArPayoffMethod_ProRata
    End Function

    Private Function MinPropCredit() As Decimal
        If IsBoyd Then MinPropCredit = 2
        If IsTreehouse Or IsBlueSky Then MinPropCredit = 1
    End Function

    Private Function PropPayoffMethod() As String
        PropPayoffMethod = ArPayoffMethod_Rule_78
        '  If IsMidSouth Then PropPayoffMethod = "Rule 78b" ' Or IsLott
        If IsTreehouse Or IsBlueSky Then PropPayoffMethod = ArPayoffMethod_ProRata
        If UseAmericanNationalInsurance Then PropPayoffMethod = ArPayoffMethod_ProRata
    End Function

    Private Function MinAccCredit() As Decimal
        If IsBoyd Then MinAccCredit = 2
        If IsTreehouse Or IsBlueSky Then MinAccCredit = 1
    End Function

    Private Function AccPayoffMethod() As String
        AccPayoffMethod = ArPayoffMethod_Rule_78
        If IsTreehouse Or IsBlueSky Then AccPayoffMethod = ArPayoffMethod_Anticip
        '  If IsMidSouth Then AccPayoffMethod = "Rule 78b" ' Or IsLott
    End Function

    Private Function MinLifeCredit() As Decimal
        If IsBoyd Then MinLifeCredit = 2
        If IsTreehouse Or IsBlueSky Then MinLifeCredit = 1
    End Function

    Private Function LifePayoffMethod() As String
        LifePayoffMethod = ArPayoffMethod_Rule_78
        '  If IsMidSouth Then LifePayoffMethod = "ProRata" ' Or IsLott
        If IsTreehouse Or IsBlueSky Then LifePayoffMethod = ArPayoffMethod_Anticip
    End Function

    Private Function PayInsuranceAfter30Days() As Boolean
        '  PayInsuranceAfter30Days = IsMidSouth ' Or IsLott
    End Function

    Private Function CashOptPaidOff(Optional ByVal AssumePaid As Boolean = False) As Boolean
        Dim CashOptOKDate As Date, PayOffBal As Decimal
        If Val(txtSameAsCash.Text) = 0 Then Exit Function
        CashOptOKDate = DateAdd("m", Val(txtSameAsCash), DateValue(txtDelivery.Text)) 'txtFirstPay)) ' MJK 20140118
        If DateAfter(Today, CashOptOKDate) Then Exit Function
        If AssumePaid Then CashOptPaidOff = True : Exit Function
        PayOffBal = GetPrice(txtFinanced.Text) - Life - Accident - Prop - IUI - INTEREST - InterestTax
        CashOptPaidOff = PayOffBal <= TotPaid
    End Function

    Public Sub GetAgeing()
        Dim LateChargeBal As Decimal
        LateChargeBal = GetPrice(lblLateCharge.Text)

        Dim AR As Decimal, L0 As Decimal, L30 As Decimal, L60 As Decimal, L90 As Decimal
        Dim NDD As String
        If IsRevolvingCharge(ArNo) Then
            ' Aging needs to be rethought for revolving accounts; amounts due are based on the date of each sale.
            SetAgeingVisible(False)
        Else
            ComputeAgeing(Today, DateValue(txtFirstPay.Text), Val(txtMonths.Text), Val(txtPaidBy.Text), txtPayPeriod.Text = "W",
      GetPrice(txtMonthlyPayment.Text), TotPaid, Financed, Balance, False, False,
      AR, L0, L30, L60, L90, , , , , , , NDD)
            SetAgeingVisible(True)
            txtNextDue.Text = NDD
        End If

        If AR < 0 Then
            '    AR = 0   ' BFH20140227
            lblTotDue.Text = "TotDue: " & CurrencyFormat(AR)
        ElseIf -AR > Balance Then
            lblTotDue.Text = "TotDue: " & CurrencyFormat(AR)
        Else
            lblTotDue.Text = "TotDue: " & CurrencyFormat(AR + LateChargeBal)
        End If
        lblArrearages.Text = "Arrearages: " & CurrencyFormat(AR)

        lblLate0.Text = CurrencyFormat(L0)
        lblLate31.Text = CurrencyFormat(L30)
        lblLate61.Text = CurrencyFormat(L60)
        lblLate91.Text = CurrencyFormat(L90)
        lblBalance.Text = CurrencyFormat(lblBalance)
        lblLateCharge.Text = CurrencyFormat(lblLateCharge)
    End Sub

    Public Sub SetAgeingVisible(ByVal Vis As Boolean)
        lbl0030.Visible = Vis
        lblLate0.Visible = Vis
        lbl3160.Visible = Vis
        lblLate31.Visible = Vis
        lbl6190.Visible = Vis
        lblLate61.Visible = Vis
        lblOver91.Visible = Vis
        lblLate91.Visible = Vis
        lblArrearages.Visible = Vis
        lblTotDue.Visible = Vis
    End Sub

    Private Sub filFile_Click()
        On Error GoTo ClearLetter
        cmdEdit.Enabled = True
        cmdPrint.Enabled = True
        cmdExport.Enabled = True

        rtfFile.LoadFile(filFile.Path & "\" & filFile.FileName)
        ReplaceLetterTokens(rtfFile)

        Exit Sub
ClearLetter:
        rtfFile.SelectionStart = 0
        rtfFile.SelectionLength = Len(rtfFile.Text)
        rtfFile.SelectedText = ""
        rtfFile.Tag = ""
    End Sub

    Public Sub ReplaceLetterTokens(ByRef rtb As RichTextBox)
        Dim L As Object, I As Object, Op As Object
        'Op = MousePointer
        Op = Cursor
        'MousePointer = vbHourglass
        Cursor = Cursors.WaitCursor
        '01-04:  store (name, add, city, phone)
        '05-08:  lblaccount, [txtlastpay], Trim(lblFirstName), Trim(lblLastName)
        '09-14:  lblAddress, city, zip, tele1, tele2, lblSSN (BFH20050516: was ArApp.SS)
        '15:     Format(lblBalance, "$#,##0.00")    'Balance
        '16:     txtPaidBy
        '17:     Format(txtMonthlyPayment, "$#,##0.00")    'Payment (T&C)
        '18:     Format(txtLateChargeAmount, "$#,##0.00")     'Late Charge (T&C)
        '19:     Format(txtFinanced, "$#,##0.00")     'Amt Financed
        '20-22:  txtMonths, LastPayDate, LastPay
        '23:     Format(GetPrice(lblLate31) + GetPrice(lblLate61) + GetPrice(lblLate91), "$#,##0.00")
        '24:     Format(GetPrice(txtMonthlyPayment) + GetPrice(txtLateChargeAmount), "$#,##0.00")  ' Payment + Late Charge
        '25-26:  txtPaidBy, lblArrearages
        '27:     Format(GetPrice(lblLate0) + GetPrice(lblLate31) + GetPrice(lblLate61) + GetPrice(lblLate91), "$#,##0.00") ' Total due
        '28:     dateformat(Now)
        '29:     txtDelivery
        '30-31:  Credit Limit, Approval Terms (both from ArApp, like SSN was)
        '32:     Last Payment Made (Payment History)
        '33:     Last Payment Made Date (Payment History)
        '34:     LastPayment Made Type (Payment History)

        'ReDim L(1 To 34)
        ReDim L(0 To 33)
        L(0) = StoreSettings.Name : L(1) = StoreSettings.Address : L(2) = StoreSettings.City : L(3) = StoreSettings.Phone
        L(4) = lblAccount.Text : L(5) = cboStatus.Text : L(6) = Trim(lblFirstName.Text) : L(7) = Trim(lblLastName.Text)
        L(8) = lblAddress.Text : L(9) = lblCity.Text : L(10) = lblZip.Text : L(11) = lblTele1.Text : L(12) = lblTele2.Text : L(13) = lblSSN.Text
        L(14) = CurrencyFormat(lblBalance.Text, , True)
        L(15) = txtPaidBy.Text
        L(16) = CurrencyFormat(txtMonthlyPayment.Text, , True)
        L(17) = CurrencyFormat(txtLateChargeAmount.Text, , True)
        L(18) = CurrencyFormat(txtFinanced.Text, , True)
        L(19) = txtMonths.Text : L(20) = txtLastPay.Text : L(21) = GetPrice(txtFinanced.Text) - GetPrice(txtMonthlyPayment.Text) * (GetPrice(txtMonths.Text) - 1)
        L(22) = CurrencyFormat(GetPrice(lblLate31.Text) + GetPrice(lblLate61.Text) + GetPrice(lblLate91.Text), , True)
        L(23) = CurrencyFormat(GetPrice(txtMonthlyPayment.Text) + GetPrice(txtLateChargeAmount.Text), , True)
        L(24) = "" : L(25) = Mid(lblArrearages.Text, 13)
        L(26) = CurrencyFormat(GetPrice(lblLate0.Text) + GetPrice(lblLate31.Text) + GetPrice(lblLate61.Text) + GetPrice(lblLate91.Text) + GetPrice(LateChargeBal), , True)
        L(27) = DateFormat(Now)
        L(28) = txtDelivery.Text
        L(29) = lblCreditLimit.Text : L(30) = lblApprovalTerms.Text

        Dim LPAmt As Decimal, LPTyp As String, LPDat As String
        If GetArNoLastPayment(L(5), LPAmt, LPTyp, LPDat) Then
            L(31) = CurrencyFormat(LPAmt)
            L(32) = LPDat
            L(33) = LPTyp
        Else
            L(31) = "[NONE]"
            L(32) = "[NEVER]"
            L(33) = "[N/A]"
        End If

        '  L = Array(frmSetup .StoreName, frmSetup .StoreAddress, frmSetup .StoreCity, frmSetup .StorePhone, _
        '            lblAccount, "", Trim(lblFirstName), Trim(lblLastName), _
        '            lblAddress, lblCity, lblZip, lblTele1, lblTele2, lblSSN, _
        '            Format(lblBalance, "$#,##0.00"), _
        '            txtPaidBy, _
        '            Format(txtMonthlyPayment, "$#,##0.00"), Format(txtLateChargeAmount, "$#,##0.00"), _
        '            Format(txtFinanced, "$#,##0.00"), _
        '            txtMonths, LastPayDate, LastPay, _
        '            Format(GetPrice(lblLate31) + GetPrice(lblLate61) + GetPrice(lblLate91), "$#,##0.00"), _
        '            Format(GetPrice(txtMonthlyPayment) + GetPrice(txtLateChargeAmount), "$#,##0.00"), _
        '            txtPaidBy, lblArrearages, _
        '            Format(GetPrice(lblLate0) + GetPrice(lblLate31) + GetPrice(lblLate61) + GetPrice(lblLate91), "$#,##0.00"), _
        '            DateFormat(Now), _
        '            txtDelivery, _
        '            lblCreditLimit, lblApprovalTerms _
        '            )

        rtb.SelectionLength = 0
        For I = LBound(L) To UBound(L)
            Do While rtb.Find("%" & Format(I, "00"), 1, -1, RichTextBoxFinds.WholeWord) <> -1
                'rtb.SelText = L(I)
                rtb.SelectedText = L(I)
                'rtb.SelLength = 0
                rtb.SelectionLength = 0
            Loop
        Next

        'MousePointer = Op
        Cursor = Op
        Exit Sub
ErrorHandler:
        'MousePointer = Op
        Cursor = Op
        'rtb.SelStart = 0
        rtb.SelectionStart = 0
        'rtb.SelLength = Len(rtb)
        rtb.SelectionLength = Len(rtb)
        'rtb.SelText = ""
        rtb.SelectedText = ""
    End Sub

    Public Sub GetCust()
        'FINDS OLD CUSTOMER & CONTINUES ON
        lblFirstName.Text = Mail.First
        lblLastName.Text = Mail.Last
        lblAddress.Text = Mail.Address
        lblAddAddress.Text = Mail.AddAddress
        lblCity.Text = Mail.City
        lblZip.Text = Mail.Zip
        lblTele1.Text = DressAni(CleanAni(Mail.Tele))
        lblTele2.Text = DressAni(CleanAni(Mail.Tele2))
        lblTele3.Text = DressAni(CleanAni(Mail2.Tele3))
        SetTelephoneCaptions(Mail.PhoneLabel1, Mail.PhoneLabel2, Mail2.PhoneLabel3)

        If ARPaySetUp.AccountFound <> "Y" Then
            lblAccount.Text = Trim(ArNo)
        End If
    End Sub

    Private Sub SetTelephoneCaptions(ByVal Lbl1 As String, ByVal Lbl2 As String, ByVal Lbl3 As String)
        Dim Longest As Integer
        If Trim(Lbl1) = "" Then Lbl1 = "Tele 1: "
        If Trim(Lbl2) = "" Then Lbl2 = "Tele 2: "
        If Trim(Lbl3) = "" Then Lbl3 = "Tele 3: "
        If Microsoft.VisualBasic.Right(Trim(Lbl1), 1) <> ":" Then Lbl1 = Lbl1 & ": "
        If Microsoft.VisualBasic.Right(Trim(Lbl2), 1) <> ":" Then Lbl2 = Lbl2 & ": "
        If Microsoft.VisualBasic.Right(Trim(Lbl3), 1) <> ":" Then Lbl3 = Lbl3 & ": "
        lblTele1Caption.Text = Lbl1
        lblTele2Caption.Text = Lbl2
        lblTele3Caption.Text = Lbl3
        Longest = Max(lblTele1Caption.Width, lblTele2Caption.Width, lblTele3Caption.Width)
        lblTele1.Left = lblTele1Caption.Left + Longest + 60
        lblTele2.Left = lblTele2Caption.Left + Longest + 60
        lblTele3.Left = lblTele3Caption.Left + Longest + 60
    End Sub

    Private ReadOnly Property PayoffAsOfDate() As Date
        Get
            PayoffAsOfDate = Today     ' DEFAULT VALUE

            If OrderMode("A") Then
                If IsFormLoaded("BillOSale") Then
                    If IsDate(BillOSale.dteSaleDate.Value) Then
                        PayoffAsOfDate = DateValue(BillOSale.dteSaleDate.Value)
                    End If
                End If
            End If

            PayoffAsOfDate = DateValue(PayoffAsOfDate)
        End Get
    End Property

    Private Sub GetPreviousPayoff(ByVal ArNo As String, ByRef ppoLife As Boolean, ByRef ppoAcc As Boolean, ByRef ppoProp As Boolean, ByRef ppoIUI As Boolean) ', byRef ppoInt as boolean)
        Dim RS As ADODB.Recordset
        Dim CL As Boolean, CA As Boolean, cP As Boolean, cU As Boolean ' , cI as boolean
        RS = GetRecordsetBySQL("SELECT * FROM [Transactions] WHERE ArNo='" & ArNo & "' AND LCase(Left(Type,4)) IN ('Life','Acc.','Prop','IUI ') ORDER BY [TransactionID] DESC")

        ppoLife = False
        ppoAcc = False
        ppoProp = False
        ppoIUI = False

        Do While Not RS.EOF
            Select Case IfNullThenNilString(RS("Type"))
                Case arPT_Lif : CL = True
                Case arPT_poLif : If Not CL Then ppoLife = True : CL = True

                Case arPT_Acc : CA = True
                Case arPT_poAcc : If Not CA Then ppoAcc = True : CA = True

                Case arPT_Pro : cP = True
                Case arPT_poPro : If Not cP Then ppoProp = True : cP = True

                Case arPT_IUI : cU = True
                Case arPT_poIUI : If Not cU Then ppoIUI = True : cU = True

                    '      Case arPT_Int:        CI = True
                    '      Case arPT_poInt:      If Not CI Then ppoInt = True: CI = True
            End Select
            RS.MoveNext
        Loop

        RS = Nothing
    End Sub

    Private Function AlreadyMadeSameAsCash() As Boolean
        Dim R As ADODB.Recordset
        R = GetRecordsetBySQL("SELECT * FROM [Transactions] WHERE [ArNo]=""" & ProtectSQL(ArNo) & """ ORDER BY [TransDate] ASC, [TransactionID] ASC", , GetDatabaseAtLocation())
        Do While Not R.EOF
            If LCase(Microsoft.VisualBasic.Left(R("Type").Value & "", 7)) = LCase(arPT_New) Then AlreadyMadeSameAsCash = False
            If LCase(Microsoft.VisualBasic.Left(R("Receipt").Value & "", 12)) = "same as cash" Then AlreadyMadeSameAsCash = True
            R.MoveNext
        Loop
        DisposeDA(R)
    End Function

    Public Function QueryPayLogSale(ByVal I As Integer) As String
        If I > PayCount Then Exit Function
        QueryPayLogSale = PayLog(I - 1).SaleNo
    End Function

    Public Function QueryPayLogAmount(ByVal I As Integer) As Decimal
        If I > PayCount Then Exit Function
        QueryPayLogAmount = PayLog(I - 1).Amount
    End Function

    Public Sub GetCustomerAccount()
        ArNo = -1
        Show()

        If ARPaySetUp.AccountFound = "" Then  'show entry form
TryAgain:
            ArCheck.Text = "Installment Customer"
            ArCheck.lblInstructions.Text = "Customer Account Number"
            'ArCheck.HelpContextID = HelpContextID

            ArCheck.ShowDialog(Me)
            ArNo = IIf(ArCheck.Customer = "", 0, ArCheck.Customer)
            mArNo = ArNo

            mDBAccess_Init(ArNo)
            mDBAccess.GetRecord()    ' this gets the record
            mDBAccess.dbClose()
            mDBAccess = Nothing

            If ArNo <> "-1" And ArNo <> "0" Then 'not found
                GetCustomer()
                mDBAccessTransactions_Init(ArNo)
                mDBAccessTransactions.GetRecord()    ' this gets the record
                mDBAccessTransactions.dbClose()
                mDBAccessTransactions = Nothing
                GetPayoff()
                GetAgeing()
                filFile_Click()
            ElseIf ArNo = 0 Then
                'Unload Me
                Me.Close()
            Else 'If ArNo = "-1" Then 'not found
                If MessageBox.Show("Incorrect Account Number.  Try again?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes Then
                    GoTo TryAgain
                Else
                    'Unload Me
                    Me.Close()
                    MainMenu.Show()
                End If
            End If
        End If
    End Sub

    Public Sub VoidAccount()
        If ArNo <> "0" And Status <> arST_Void Then
            If MessageBox.Show("Are you sure you want to void this installment contract?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = DialogResult.OK Then
                mDBAccess_Init(ArNo)
                mDBAccess.SetRecord()    ' this sets the record, it will set either
                mDBAccess.dbClose()
                mDBAccess = Nothing
            End If
        End If

        ' *** do something with money ***
        'Unload Me
        Me.Close()
        MainMenu.Show()
    End Sub

    Private Sub mDBAccess_GetRecordEvent(RS As ADODB.Recordset) Handles mDBAccess.GetRecordEvent    ' called if record is found
        'Debug.Print "ArCard.mDBAccess_GetRecordEvent"
        Dim Tid As String
        lblBalance.Text = ""
        lblLateCharge.Text = ""
        'BackColor = &H8000000F

        Status = RS("Status").Value

        On Error Resume Next
        Err.Clear()

        'NOTE: This If block is not in vb6.0. Added here because in vb6.0, ArCard form load event is executing for Order Entry -> Payment on Account ->Store finance payment option.
        'But in vb.net, it is not executing. Loading combobox is happening in form load event. In vb.net form load is not executing which is happening in vb6.0.
        'So, combobox becomes empty in vb.net. So added this If block to fill the combobox (cbostatus).
        If cboStatus.Items.Count = 0 Then
            LoadArStatusCombo(cboStatus)
        End If
        cboStatus.SelectedIndex = 0
        cboStatus.Text = ArStatusComboValueFromAccountStatus(Status)
        'If IsIn(Status, "W", "R", "L", "Y") Then BackColor = ALERT_COLOR
        If IsIn(Status, "W", "R", "L", "Y") Then BackColor = Color.Blue
        If Err.Number <> 0 Then MessageBox.Show("Invalid Installment Info Status: [" & Status & "]", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        On Error GoTo 0

        EnableFrame(Me, fraPrint, Status <> arST_Void)
        EnableFrame(Me, fraNav, Status <> arST_Void)
        EnableFrame(Me, fraPaymentOptions, Status <> arST_Void)
        EnableFrame(Me, fraEditOptions, Status <> arST_Void)
        EnableFrame(Me, fraTerms, Status <> arST_Void)
        EnableFrame(Me, fraBalance, Status <> arST_Void)
        EnableFrame(Me, fraPrintType, Status <> arST_Void)

        cmdCreditApp.Enabled = Status <> arST_Void
        cmdDetail.Enabled = Status <> arST_Void
        Notes_Open.Enabled = Status <> arST_Void
        cmdPayoff.Enabled = Status <> arST_Void And Not IsRevolvingCharge(RS("ArNo").Value)
        cmdPrint.Enabled = Status <> arST_Void
        cmdPrintCard.Enabled = Status <> arST_Void
        cmdReceipt.Enabled = Status <> arST_Void
        cmdReprintContract.Enabled = Not IsRevolvingCharge(RS("ArNo").Value) ' no contract for revolving accounts MJK20140218

        mArNo = IfNullThenNilString(RS("ArNo").Value)
        ' Save new Account Record value in Arno
        ArNo = mArNo
        MailRec = IfNullThenNilString(RS("MailIndex").Value)
        lblAccount.Text = IfNullThenNilString(RS("ArNo").Value)
        ' lblLastName = IfNullThenNilString(rs!LastName)
        Status = IfNullThenNilString(RS("Status").Value)
        txtFinanced.Text = CurrencyFormat(IfNullThenZeroCurrency(RS("Financed").Value))
        Financed = IfNullThenZeroCurrency(RS("Financed").Value)
        txtMonths.Text = IfNullThenNilString(RS("Months").Value)
        Months = txtMonths.Text
        txtRate.Text = IfNullThenNilString(RS("Rate").Value)
        lblAPR.Text = FormatQuantity(IfNullThenZeroDouble(RS("APR").Value), 2) & " APR"

        ReCalcArCreditHistory

        txtPayPeriod.Text = IfNullThenNilString(RS("Period").Value)
        txtMonthlyPayment.Text = CurrencyFormat(IfNullThenNilString(RS("PerMonth").Value))
        txtLateChargeAmount.Text = CurrencyFormat(IfNullThenNilString(RS("LateCharge").Value))
        txtPaidBy.Text = IfNullThenNilString(RS("LateDueOn").Value)
        txtDelivery.Text = DateFormat(IfNullThenNilString(RS("DeliveryDate").Value))
        txtFirstPay.Text = DateFormat(IfNullThenNilString(RS("FirstPayment").Value))
        txtSameAsCash.Text = IfNullThenNilString(RS("CashOpt").Value)
        CashOpt = GetPrice(txtSameAsCash.Text)
        cmdMakeSameAsCash.Enabled = ArMode("Edit") And Not IsIn(Status, "V", "C", "W", "L", "R") 'And getprice(txtSameAsCash) > 0
        Balance = IfNullThenZeroCurrency(RS("Balance").Value)
        lblBalance.Text = CurrencyFormat(Balance) 'IfNullThenNilString(RS!Balance))
        TotPaid = IfNullThenZeroCurrency(RS("TotPaid").Value)
        ' allow for opening late charge balance
        lblLateCharge.Text = CurrencyFormat(IfNullThenNilString(RS("LateChargeBal").Value))
        LateChargeBal = lblLateCharge.Text
        txtPaidBy.Enabled = Not IsRevolvingCharge(RS("ArNo").Value)
        If IsRevolvingCharge(RS("ArNo").Value) Then
            ''txtLateChargeAmount = CurrencyFormat(GetPrice(lblBalance) * GetPrice(txtRate) / 100)
            'txtLateChargeAmount = CalculateRevolvingInterest(RS!ArNo) ' as of the next cycle? ' Maintain the field in ArCard and frmRevolving as balances change..
        End If
        'lastpay
        txtLastPay.Text = DateAdd("m", Months - 1, txtFirstPay.Text)

        SendNotice = IfNullThenNilString(RS("SendNotice").Value)
        If SendNotice = "" Or Val(SendNotice) = 0 Then
            'chkSendAllMail.Value = 1
            chkSendAllMail.Checked = True
        Else
            'chkSendAllMail.Value = 0
            chkSendAllMail.Checked = False
        End If


        On Error Resume Next
        INTEREST = IfNullThenZeroCurrency(RS("INTEREST").Value)
        InterestTax = IfNullThenZeroCurrency(RS("InterestSalesTax").Value)
        Life = IfNullThenZeroCurrency(RS("Life").Value)
        Accident = IfNullThenZeroCurrency(RS("Accident").Value)
        Prop = IfNullThenZeroCurrency(RS("Prop").Value)
        IUI = IfNullThenZeroCurrency(RS("IUI").Value)
        PriorBal = GetPrice(lblBalance.Text)

        lblSSN.Text = IfNullThenNilString(RS("HisSS").Value)
        lblCreditLimit.Text = FormatCurrency(IfNullThenZeroCurrency(RS("CreditLimit").Value))
        lblApprovalTerms.Text = IfNullThenNilString(RS("ApprovalTerms").Value)

        ' Form has been filled, time to compare master record with children.
        ' Financed should equal ChildFinanced + Interest + DocFee + ...?
        '   but docfee is recorded in Transactions and nowhere else.
        ' Balance should equal ChildCharges + ...?
        ' This doesn't have to be right forever.  It's only here to make sure payments are getting recorded on sales.
        If IsRevolvingCharge(ArNo) Then
            Dim ChildCharges As Decimal, ChildFinanced As Decimal
            Dim Inst As New cInstallment
            Inst.Load(ArNo)
            ChildFinanced = Inst.SaleFinanced()
            ChildCharges = Inst.SaleBalance()
            DisposeDA(Inst)
            ' Mismatch may be OK.  Accounts can be added with balances not tracked by sale.
            '    If GetPrice(lblBalance) <> ChildCharges + GetPrice(LateChargeBal) + INTEREST Then
            '      Debug.Print "Balance mismatch on Account " & ArNo; " - Balance " & lblBalance & " (" & ChildCharges & " + " & GetPrice(LateChargeBal) & " + " & GetPrice(INTEREST) & ")"
            '    End If
            LoadSaleBalances
            cmdSaleTotals.Visible = True
        Else
            UGrSaleTotals.MaxRows = 1
            UGrSaleTotals.Clear() 'IMP NOTE: This Clear method is to clear the ugridio1 data(rows and cols) using AxDataGrid1.ClearFields() in Clear method. But it is not working. So to clear it, below For loop is added. This for loop is not in vb6.0 code.

            Dim Lastrow As Integer
            Lastrow = UGrSaleTotals.LastRowUsed
            For r = 0 To Lastrow
                UGrSaleTotals.Row = r
                For c = 0 To 2
                    On Error Resume Next
                    UGrSaleTotals.Col = c
                    UGrSaleTotals.Text = ""
                Next
            Next

            cmdSaleTotals.Visible = False
        End If
    End Sub

    Public Sub ReCalcArCreditHistory(Optional ByVal ChooseDate As Boolean = False)
        Dim D As String

        If ChooseDate Then
            If IsDate(Trim(Mid(lblPaymentHistory.Text, 7))) Then D = MonthAdd(DateValue(Trim(Mid(lblPaymentHistory.Text, 7))), 2)
            D = SelectDate(D)
            If D = NullDate Then D = Today
        Else
            D = Today
        End If

        txtPaymentHistory.Text = WrapLongText(GetArCreditHistory(mArNo, D, 24), 12)
        'Debug.Print "ArCard PHP: " & Trim(Replace(txtPaymentHistory, vbCrLf, ""))
        lblPaymentHistory.Text = "Date: " & DateFormat(D)
        'lblPaymentHistory.ToolTipText = "Effective Date: " & DateFormat(D)
        ToolTip1.SetToolTip(lblPaymentHistory, "Effective Date: " & DateFormat(D))
        'txtPaymentHistory.Visible = Trim(txtPaymentHistory.Text) <> ""
        'lblPaymentHistory.Visible = txtPaymentHistory.Visible
        If Trim(txtPaymentHistory.Text) <> "" Then
            txtPaymentHistory.Visible = True
            lblPaymentHistory.Visible = True
        Else
            txtPaymentHistory.Visible = False
            lblPaymentHistory.Visible = False
        End If

    End Sub

    Private Sub LoadSaleBalances()
        Dim H As New cHolding, Row As Integer, RunningTotal As Decimal, StatementDay As Date
        StatementDay = DateAdd("d", RevolvingStatementDay() - DateAndTime.Day(Today), Today)
        H.Load(ArNo, "ArNo")

        UGrSaleTotals.Clear()
        UGrSaleTotals.MaxRows = Max(H.DataAccess.Record_Count, 1)
        Do Until H.DataAccess.Record_EOF
            '      If H.Sale - H.Deposit > 0 Then
            UGrSaleTotals.SetValueDisplay(Row, 0, H.LeaseNo)
            UGrSaleTotals.SetValueDisplay(Row, 1, FormatCurrency(H.Sale - H.Deposit))
            UGrSaleTotals.SetValueDisplay(Row, 2, FormatCurrency(H.CalculateRevolvingInterest(IfNullThenZero(CashOpt), StatementDay, Val(txtRate))))
            RunningTotal = RunningTotal + H.Sale - H.Deposit
            Row = Row + 1
            '     End If
            H.DataAccess.Records_MoveNext()
        Loop

        If INTEREST > 0 Then
            UGrSaleTotals.MaxRows = UGrSaleTotals.MaxRows + 1
            UGrSaleTotals.SetValueDisplay(Row, 0, "Interest")
            UGrSaleTotals.SetValueDisplay(Row, 1, FormatCurrency(INTEREST))
            UGrSaleTotals.SetValueDisplay(Row, 2, FormatCurrency(INTEREST * Val(txtRate) / 100))
            RunningTotal = RunningTotal + INTEREST
            Row = Row + 1
        End If

        If Balance > RunningTotal Then
            UGrSaleTotals.MaxRows = UGrSaleTotals.MaxRows + 1
            UGrSaleTotals.SetValueDisplay(Row, 0, "Account")
            UGrSaleTotals.SetValueDisplay(Row, 1, FormatCurrency(Balance - RunningTotal))
            UGrSaleTotals.SetValueDisplay(Row, 2, FormatCurrency((Balance - RunningTotal * Val(txtRate) / 100) * RevolvingMonthsInterest(IfNullThenZeroDate(txtDelivery), StatementDay, IfNullThenZero(CashOpt))))
            Row = Row + 1
        End If

        UGrSaleTotals.Refresh()
        DisposeDA(H)
    End Sub

    Private Sub ClearVars()
        ArNo = ""
        MailRec = 0
        DocCredit = 0 : LifeCredit = 0 : AccidentCredit = 0 : PropertyCredit = 0 : IUICredit = 0 : InterestCredit = 0 : InterestTaxCredit = 0

        mArNo = ""
        CustRec = 0 : PriorBal = 0 : CashOpt = 0 : INTEREST = 0 : InterestTax = -0 : Life = 0 : Accident = 0 : Prop = 0 : IUI = 0
        Charges = 0 : Credits = 0 : Balance = 0 : TotPaid = 0 : Status = "" : TransType = ""
        Payoff = "" : PayoffSameAsCash = False : StatusChg = ""
        Receipt = "" : NewTypee = ""

        TransDate = "" : LastPayDate = "" : LastPay = "" : LateChargeBal = ""
        Months = "" : SendNotice = "" : Counter = 0
        ClearPayments()
    End Sub

    Private Sub CloseForm()
        UGrSaleTotals.Visible = False
        UGridIO1.Visible = False
        Notes_Frame.Visible = False

        OpenFormAs = ""
        'Height = 6800
        'Height = UGridIO1.Top + (Height - ScaleHeight)
        Height = UGridIO1.Top + (Height - Me.ClientSize.Height)
    End Sub

    Private Sub ArCard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim Screen As System.Drawing.Rectangle
        ClearVars()
        LoadArStatusCombo(cboStatus)

        StatusChg = ""
        'filFile = FXFolder()
        filFile.Path = FXFolder()
        cboStatus.Enabled = False
        txtPayMemo.Visible = False 'Pay memo
        lblPayMemo.Visible = False

        cmdHistory.Visible = False ' IsMichaels

        Text = "Customer Account Card:  " & StoreSettings.Name & "  " & StoreSettings.Address
        CloseForm()
        Top = 0
        Left = (Screen.Width - Width) / 2  'NOTE: If not work, refer ShowFormCenter() of MainMenu4.vb form.
        Text = "Customer Account Card:  " & StoreSettings.Name & "  " & StoreSettings.Address

        '  cmdReceipt.Enabled = (not armode("Edit")) ' BFH20061218  ' off again, BFH20070125
        cmdMakeSameAsCash.Visible = ArMode("Edit")

        If ArMode("Edit") Then
            cboStatus.Enabled = True

            Text = "EDIT Customer Accounts:  " & StoreSettings.Name & "  " & StoreSettings.Address
            fraPaymentOptions.Visible = False
            fraEditOptions.Visible = True
            optEditType17.Visible = StoreSettings.bInstallmentInterestIsTaxable
            optEditType18.Visible = UseIUI() ' Credit IUI

            cmdPayoff.Enabled = False
        Else
            optPayType8.Visible = False
            fraPaymentOptions.Visible = True
            fraEditOptions.Visible = False
        End If

        If ArMode("P") Then Text = "Payment on Account:  " & StoreSettings.Name & "  " & StoreSettings.Address

        UGridIO1.GetDBGrid.AllowUpdate = False
        UGridIO1.AddColumn(0, "Date", 1350, True, False, MSDataGridLib.AlignmentConstants.dbgRight)
        UGridIO1.AddColumn(1, "Type", 1350, True, False, MSDataGridLib.AlignmentConstants.dbgLeft)
        UGridIO1.AddColumn(2, "Charges", 1250, True, False, MSDataGridLib.AlignmentConstants.dbgRight)
        UGridIO1.AddColumn(3, "Credits", 1250, True, False, MSDataGridLib.AlignmentConstants.dbgRight)
        UGridIO1.AddColumn(4, "Balance", 1250, True, False, MSDataGridLib.AlignmentConstants.dbgRight)
        UGridIO1.AddColumn(5, "Pay Memo/Receipt", 2700, True, False, MSDataGridLib.AlignmentConstants.dbgRight)
        UGridIO1.MaxCols = 6
        UGridIO1.MaxRows = 500
        UGridIO1.Initialize()
        UGridIO1.Activated = True
        UGridIO1.Refresh()
        UGridIO1.Col = 0
        UGridIO1.Row = 0

        PayOpt = 1
        DDate.Value = DateFormat(Today)

        DoPrintType()

        'cmdSaleTotals.Move cmdPayoff.Left, cmdPayoff.Top, cmdPayoff.Width, cmdPayoff.Height
        cmdSaleTotals.Location = New Point(cmdPayoff.Left, cmdPayoff.Top)
        cmdSaleTotals.Size = New Size(cmdPayoff.Width, cmdPayoff.Height)
        'UGrSaleTotals.Move UGridIO1.Left, UGridIO1.Top, UGridIO1.Width, UGridIO1.Height
        UGrSaleTotals.Location = New Point(UGridIO1.Left, UGridIO1.Top)
        UGrSaleTotals.Size = New Size(UGridIO1.Width, UGridIO1.Height)
        UGrSaleTotals.GetDBGrid.AllowUpdate = False
        UGrSaleTotals.AddColumn(0, "Sale", 1350, True, False, MSDataGridLib.AlignmentConstants.dbgRight)
        UGrSaleTotals.AddColumn(1, "Balance", 1350, True, False, MSDataGridLib.AlignmentConstants.dbgLeft)
        UGrSaleTotals.AddColumn(2, "Next Interest*", 1350, True, False, MSDataGridLib.AlignmentConstants.dbgLeft)
        UGrSaleTotals.MaxCols = 3
        UGrSaleTotals.MaxRows = 1
        UGrSaleTotals.Initialize()
        UGrSaleTotals.Activated = True
        UGrSaleTotals.Refresh()
        UGrSaleTotals.Col = 0
        UGrSaleTotals.Row = 0
    End Sub

    Private Sub DoPrintType()
        opt30323.Checked = (DefaultMailingLabelType() = 30323)
        opt30252.Checked = (DefaultMailingLabelType() <> 30323)
    End Sub

    Public Property PayOpt() As Integer
        Get
            Dim I As Integer
            If Not fraPaymentOptions.Visible Then Exit Property
            'For I = optPayType.LBound To optPayType.UBound
            '    If optPayType(I) Then PayOpt = I : Exit Property
            'Next
            If optPayType1.Checked = True Then PayOpt = I : Exit Property
            If optPayType2.Checked = True Then PayOpt = I : Exit Property
            If optPayType3.Checked = True Then PayOpt = I : Exit Property
            If optPayType4.Checked = True Then PayOpt = I : Exit Property
            If optPayType5.Checked = True Then PayOpt = I : Exit Property
            If optPayType6.Checked = True Then PayOpt = I : Exit Property
            If optPayType7.Checked = True Then PayOpt = I : Exit Property
            If optPayType8.Checked = True Then PayOpt = I : Exit Property
            If optPayType9.Checked = True Then PayOpt = I : Exit Property
        End Get
        Set(value As Integer)
            'Dim I as integer
            'For I = optPayType.LBound To optPayType.UBound
            '    optPayType(I) = (vData = I)
            'Next
            If value = 1 Then
                optPayType1.Checked = True
                optPayType2.Checked = False
                optPayType3.Checked = False
                optPayType4.Checked = False
                optPayType5.Checked = False
                optPayType6.Checked = False
                optPayType7.Checked = False
                optPayType8.Checked = False
                optPayType9.Checked = False
            End If

            If value = 2 Then
                optPayType2.Checked = True
                optPayType1.Checked = False
                optPayType3.Checked = False
                optPayType4.Checked = False
                optPayType5.Checked = False
                optPayType6.Checked = False
                optPayType7.Checked = False
                optPayType8.Checked = False
                optPayType9.Checked = False
            End If
            If value = 3 Then
                optPayType3.Checked = True
                optPayType1.Checked = False
                optPayType2.Checked = False
                optPayType4.Checked = False
                optPayType5.Checked = False
                optPayType6.Checked = False
                optPayType7.Checked = False
                optPayType8.Checked = False
                optPayType9.Checked = False
            End If
            If value = 4 Then
                optPayType4.Checked = True
                optPayType1.Checked = False
                optPayType2.Checked = False
                optPayType3.Checked = False
                optPayType5.Checked = False
                optPayType6.Checked = False
                optPayType7.Checked = False
                optPayType8.Checked = False
                optPayType9.Checked = False
            End If
            If value = 5 Then
                optPayType5.Checked = True
                optPayType1.Checked = False
                optPayType2.Checked = False
                optPayType3.Checked = False
                optPayType4.Checked = False
                optPayType6.Checked = False
                optPayType7.Checked = False
                optPayType8.Checked = False
                optPayType9.Checked = False
            End If
            If value = 6 Then
                optPayType6.Checked = True
                optPayType1.Checked = False
                optPayType2.Checked = False
                optPayType3.Checked = False
                optPayType4.Checked = False
                optPayType5.Checked = False
                optPayType7.Checked = False
                optPayType8.Checked = False
                optPayType9.Checked = False
            End If
            If value = 7 Then
                optPayType7.Checked = True
                optPayType1.Checked = False
                optPayType2.Checked = False
                optPayType3.Checked = False
                optPayType4.Checked = False
                optPayType5.Checked = False
                optPayType6.Checked = False
                optPayType8.Checked = False
                optPayType9.Checked = False
            End If
            If value = 8 Then
                optPayType8.Checked = True
                optPayType1.Checked = False
                optPayType2.Checked = False
                optPayType3.Checked = False
                optPayType4.Checked = False
                optPayType5.Checked = False
                optPayType6.Checked = False
                optPayType7.Checked = False
                optPayType9.Checked = False
            End If
            If value = 9 Then
                optPayType9.Checked = True
                optPayType1.Checked = False
                optPayType2.Checked = False
                optPayType3.Checked = False
                optPayType4.Checked = False
                optPayType5.Checked = False
                optPayType6.Checked = False
                optPayType7.Checked = False
                optPayType8.Checked = False
            End If
        End Set
    End Property
End Class