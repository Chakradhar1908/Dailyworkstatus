﻿Imports WinCDS.clsPDFPrinter
Public Class cPrinter
    Private Const PI As Double = 3.14159265358979
    Private Const PI_2 As Double = 6.28318530717958
    Private mBuildPDF As Boolean
    Private mPreviewImage As Object
    Private mDocTitle As String
    Private mDocFile As String
    Private mDocKeywords As String
    Private pW As Integer, pH As Integer
    Private PDFPrinter As clsPDFPrinter

    Public Sub New()
        OutputToPrinter = True
        Orientation = VBRUN.PrinterObjectConstants.vbPRORPortrait
    End Sub

    Public Property Orientation() As Integer
        Get
            On Error Resume Next
            Orientation = oPrinter.Orientation
        End Get
        Set(value As Integer)
            On Error Resume Next
            oPrinter.Orientation = value
        End Set
    End Property

    Public ReadOnly Property oPrinter() As Object
        Get
            If BuildDLS Then
                oPrinter = DYMOObject
            ElseIf Preview Then
                oPrinter = PreviewImage
            Else
                oPrinter = Printer
            End If
        End Get
    End Property

    Public ReadOnly Property DYMOObject(Optional ByVal Reset As Boolean = False) As Object 'As Dymo.LabelEngine
        Get
            On Error Resume Next
            Static dymoAddInObj  'As Dymo.LabelEngine
            If Reset Then dymoAddInObj = Nothing
            If dymoAddInObj Is Nothing Then
                dymoAddInObj = CreateObject("DYMO.LabelEngine")
                dymoAddInObj.NewLabel(DateTimeStamp)
                'DYMO_Label_Framework.ope
                'Dim X As DYMO_Label_Framework.Label
            End If
            DYMOObject = dymoAddInObj
        End Get
    End Property

    Public ReadOnly Property Preview() As Boolean
        Get
            Preview = Not (mPreviewImage Is Nothing)
        End Get
    End Property

    Public ReadOnly Property PreviewImage() As Object
        Get
            PreviewImage = mPreviewImage
        End Get
    End Property

    Public ReadOnly Property BuildDLS() As Boolean
        Get
            BuildDLS = False And IsDymo And HasDLS
        End Get
    End Property

    Public ReadOnly Property IsDymo() As Boolean
        Get
            IsDymo = IsInStr(DeviceName, "DYMO")
        End Get
    End Property

    Public ReadOnly Property DeviceName() As String
        Get
            On Error Resume Next
            DeviceName = Printer.DeviceName
        End Get
    End Property

    Public ReadOnly Property HasDLS() As Boolean
        Get
            Static vValue As TriState
            On Error Resume Next
            If vValue = vbFalse Then vValue = IIf(IsNotNothing(CreateObject("DYMO.LabelEngine")), vbTrue, vbUseDefault)
            HasDLS = (vValue = vbTrue)
        End Get
    End Property

    Public Sub SetPrintToPDF(Optional ByVal vDocTitle As String = "", Optional ByVal vKeywords As String = "")
        If vDocTitle <> "" Then DocTitle = vDocTitle
        DocKeywords = vKeywords
        OutputToPrinter = True

        PDFInit()

        OutputObject = Me
    End Sub

    Public Property DocTitle() As String
        Get
            DocTitle = mDocTitle
        End Get
        Set(value As String)
            mDocTitle = value
        End Set
    End Property

    Public Property DocKeywords() As String
        Get
            DocKeywords = mDocKeywords
        End Get
        Set(value As String)
            mDocKeywords = value
        End Set
    End Property

    Public ReadOnly Property PDFSupportFolderExists() As Boolean
        Get
            PDFSupportFolderExists = FolderExists(PDFSupportFolder)
        End Get
    End Property

    Public ReadOnly Property PDFSupportFolder(Optional ByVal WithTrailingBS As Boolean = True) As String
        Get
            PDFSupportFolder = CleanPath(PDFFontsFolder, , False)
            If Not WithTrailingBS Then PDFSupportFolder = Left(PDFSupportFolder, Len(PDFSupportFolder) - 1)
        End Get
    End Property

    Private ReadOnly Property ToDesktop() As Boolean
        Get
            ToDesktop = False
        End Get
    End Property

    Public ReadOnly Property Keywords() As String
        Get
            Keywords = DocKeywords & IIf(Len(DocKeywords) = 0, "", ",") & ProgramName & ",report,reports,archive,archived report," & CompanyName
        End Get
    End Property

    Public ReadOnly Property DocFile() As String
        Get
            DocFile = mDocFile
        End Get
    End Property

    Private Sub PDFInit()
        If Not PDFSupportFolderExists Then Exit Sub

        mBuildPDF = True

        If ToDesktop Then
            mDocFile = UIOutputFolder() & "Report-" & DateTimeStamp() & ".pdf"
        Else
            mDocFile = ReportsFolder(Replace(DocTitle, " ", "")) & "Report-" & DateTimeStamp() & ".pdf"
        End If

        PDFPrinter = New clsPDFPrinter

        PDFPrinter.PDFTitle = DocTitle
        PDFPrinter.PDFAuthor = StoreSettings.Name & " - " & ProgramName
        PDFPrinter.PDFSubject = DocTitle & " - Archived Report"
        PDFPrinter.PDFCreator = SoftwareVersion(True, True, True)
        PDFPrinter.PDFProducer = SoftwareVersion(True, False, True)
        PDFPrinter.PDFKeywords = Keywords
        PDFPrinter.PDFView = False ' do not open the PDF file automatically

        PDFPrinter.PDFFileName = DocFile

        PDFPrinter.PDFLoadAfm = PDFSupportFolder(False)
        PDFPrinter.PDFConfirm = False
        PDFPrinter.PDFView = True
        'PDFPrinter.PDFFiligran = "P D F P r i n t e r   D e m o"

        'PDFPrinter.PDFSetViewerPreferences = VIEW_FITWINDOW
        PDFPrinter.PDFFormatPage = PDFFormatPgStr.FORMAT_LETTER
        PDFPrinter.PDFOrientation = PDFOrientationStr.ORIENT_PORTRAIT
        PDFPrinter.PDFSetUnit = PDFUnitStr.UNIT_PT
        PDFPrinter.PDFSetZoomMode = PDFZoomMd.ZOOM_REAL
        PDFPrinter.PDFSetLayoutMode = PDFLayoutMd.LAYOUT_DEFAULT
        PDFPrinter.PDFUseOutlines = False
        PDFPrinter.PDFUseThumbs = True

        PDFPrinter.PDFBeginDoc()

        PDFPrinter.PDFSetBookmark("Signet 1", 0, 40)
        PDFPrinter.PDFSetBookmark("Sous-Signet 2", 1, 60)

        PDFPrinter.PDFSetLineStyle = PDFStyleLgn.pPDF_SOLID
        PDFPrinter.PDFSetLineWidth = 1
    End Sub

End Class
