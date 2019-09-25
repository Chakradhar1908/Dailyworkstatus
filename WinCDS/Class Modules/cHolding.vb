﻿Public Class cHolding
    Private WithEvents mDataAccess As CDataAccess
    'Implements CDataAccess
    Public Index as integer
    Public Status As String
    Public LeaseNo As String
    Public Sale As Decimal
    Public Deposit As Decimal
    Public InitialLease As String
    Private mCurrentIndex as integer
    Public DataBase As String
    Public NonTaxable As Decimal
    Public LastPay As String
    Public Salesman As String
    Public Comm As String
    Public ArNo As String

    Private Structure HoldNew
        <VBFixedString(8)> Dim LeaseNo As String
        <VBFixedString(6)> Dim Index As String
        <VBFixedString(8)> Dim Sale As String
        <VBFixedString(8)> Dim Deposit As String
        <VBFixedString(1)> Dim Status As String
        <VBFixedString(1)> Dim Comm As String
        <VBFixedString(7)> Dim MargStart As String
    End Structure

    Public Function DataAccess() As CDataAccess
        DataAccess = mDataAccess
    End Function
    Public Sub Dispose()
        On Error Resume Next
        mDataAccess.Dispose
    End Sub
    Public Function Load(ByVal KeyVal As String, Optional ByVal KeyName As String = "") As Boolean
        ' Checks the database for a matching LeaseNo.
        ' Returns True if the load was successful, false otherwise.
        ' If a record was found, also loads the data into this object.

        Load = False
        ' Search for the Style
        If KeyName = "" Then
            DataAccess.Records_OpenIndexAt(KeyVal)
        ElseIf Left(KeyName, 1) = "#" Then
            ' This allows searching by AutoNumber - specialized to query by number
            ' since Access is exceptionally picky about quotation marks.
            DataAccess.Records_OpenFieldIndexAtNumber(Mid(KeyName, 2), KeyVal)
        Else
            DataAccess.Records_OpenFieldIndexAt(KeyName, KeyVal)
        End If

        ' Move to the first record if we can, and return success.
        If DataAccess.Records_Available Then Load = True
    End Function
    Public Function Save(Optional ByRef ErrDesc As String = "") As Boolean
        ErrDesc = ""
        Save = True
        On Error GoTo NoSave
        ' This instructs the class (in one simple call) to save its data members to the database.
        If Trim(InitialLease) = "" Then InitialLease = LeaseNo
        If DataAccess.CurrentIndex <= 0 Then            ' If we're already using the current record,
            DataAccess.Records_OpenIndexAt(InitialLease)   'there's no reason to re-open it.
        End If
        If DataAccess.Record_Count = 0 Then
            DataAccess.Records_Add()      ' Record not found.  This means we're adding a new one.
        End If

        DataAccess.Record_Update()      ' Then load our data into the recordset.
        DataAccess.Records_Update()     ' And finally, tell the class to save the recordset.
        Exit Function

NoSave:
        ErrDesc = Err.Description
        Err.Clear()
        Save = False
    End Function
    Public Function Void() As Boolean
        ' Make sure this holding record is able to be voided.
        If Trim(LeaseNo) = "" Then Exit Function
        If Status = "V" Then MsgBox("This sale is already void.", vbInformation) : Void = True : Exit Function

        LogFile("VoidSale", "cHolding.Void() - BEFORE VOID  - LeaseNo=" & LeaseNo & ", Status=" & Status & ", Sale=" & Sale & ", Desposit=" & Deposit, False)
        If OrdVoid.VoidOrder(LeaseNo) Then
            ' The Margin records voided nicely, so we void the Holding record too.
            LogFile("VoidSale", "cHolding.Void() - AFTER VOID   - LeaseNo=" & LeaseNo & ", Status=" & Status & ", Sale=" & Sale & ", Desposit=" & Deposit, False)
            Status = "V"
            Sale = 0 '"0.00"
            Deposit = 0 '"0.00"
            Save()
            LogFile("VoidSale", "cHolding.Void() - AFTER SAVE   - LeaseNo=" & LeaseNo & ", Status=" & Status & ", Sale=" & Sale & ", Desposit=" & Deposit, False)

            ' Void out any non-received purchase orders.
            ExecuteRecordsetBySQL("UPDATE PO SET PrintPO='v' WHERE LeaseNo='" & Trim(LeaseNo) & "'", , GetDatabaseInventory)

            Void = True
        Else
            LogFile("VoidSale", "cHolding.Void() - AFTER FAILED - LeaseNo=" & LeaseNo & ", Status=" & Status & ", Sale=" & Sale & ", Desposit=" & Deposit, False)
            ' The Margin records couldn't be voided.
            ' Whatever called this can handle the failure messages..
            Void = False
        End If

        If True Then
            Dim RS As ADODB.Recordset
            RS = GetRecordsetBySQL("SELECT * FROM HOLDING WHERE LeaseNo='" & LeaseNo & "'", , DataBase)
            LogFile("VoidSale", "cHolding.Void() - VERIFICATION - LeaseNo=" & RS("LeaseNo").Value & ", Status=" & RS("Status").Value & ", Sale=" & RS("Sale").Value & ", Desposit=" & RS("Deposit").Value, False)
            DisposeDA(RS)
        End If
    End Function

    Public Function InterestChargedInPeriod(ByRef ChargeDate As Date, ByRef MonthsBack As Long) As Currency
        ' Interest charged within X months before a date
        Dim GM As CGrossMargin
  Set GM = New CGrossMargin
  GM.Load LeaseNo
  Do Until GM.DataAccess.Record_EOF
            If DateAdd("m", MonthsBack, GM.SellDte) > ChargeDate And GM.SellDte <= ChargeDate And GM.Style = "INTEREST" Then
                InterestChargedInPeriod = InterestChargedInPeriod + GM.SellPrice * IIf(InStr(1, GM.Desc, "CREDIT", vbTextCompare) > 0, -1, 1)
            End If
            GM.DataAccess.Records_MoveNext()
        Loop
        DisposeDA GM
End Function

    Public Function AddInterest(ByRef Amount As Currency, ByRef PayDate As Date) As Boolean
        If Amount = 0 Then Exit Function
        ' INTEREST is only for tracking purposes, and does not affect sale balance.
        ' This is only here so we can undo interest if a sale is paid within 90 days.
        'Sale = Sale + Amount

        Dim GM As CGrossMargin, GM2 As CGrossMargin
        If LeaseNo = "" Then
            MsgBox "Attempting to add interest before setting sale number.", vbCritical, "Error"
    Exit Function
        End If
  Set GM = New CGrossMargin
  Set GM2 = New CGrossMargin
  If GM2.Load(LeaseNo, "SaleNo") Then
            GM.SaleNo = LeaseNo
            GM.Quantity = 1
            GM.Style = "INTEREST" ' This may have to change, it might show up on reports and be a pain to get rid of.
            If Amount > 0 Then
                GM.Desc = "INTEREST CHARGE " & PayDate
            Else
                GM.Desc = "INTEREST CREDIT " & PayDate
                Amount = Abs(Amount)
            End If
            If Installment Then
                GM.Commission = "X" ' Commission on interest should not happen
            End If
            GM.Cost = 0
            GM.ItemFreight = 0
            GM.SellPrice = Amount
            GM.Status = "DEL" ' right or not?
            GM.Location = 0
            GM.SellDte = PayDate
            GM.Store = StoresSld 'StoreNumber
            GM.Name = GM2.Name
            GM.Salesman = GM2.Salesman
            GM.ShipDte = PayDate
            GM.Phone = GM2.Phone
            GM.Index = GM2.Index
            GM.SalesSplit = GM2.SalesSplit
            GM.PackSaleGM = GM2.PackSaleGM
            GM.Save()

            If Not IsNumeric(GM.MarginLine) Then
                ' Originally there were occasional issues when a payment overlapped sales;
                ' the first payment wouldn't get saved.
                ' I haven't seen it since adding DisposeDA below, but don't know why it happened
                ' and that change SHOULD NOT have fixed it.
                ' This message should catch it if it happens again.
                MsgBox "Problem saving interest - Record failed for Sale " & LeaseNo & ".", vbCritical, "Error"
      AddInterest = False
            Else
                AddInterest = True
            End If
        Else
            ' This should never happen; empty sale except this payment?
            MsgBox "Problem saving interest - no records found for Sale " & LeaseNo & ".", vbCritical, "Error"
    AddInterest = False
        End If
        DisposeDA GM, GM2
End Function

End Class
