﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDesignTag
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDesignTag))
        Me.cmbPageAlign = New System.Windows.Forms.ComboBox()
        Me.imgPrintHelper = New System.Windows.Forms.PictureBox()
        Me.fraBox = New System.Windows.Forms.GroupBox()
        Me.il = New System.Windows.Forms.ImageList(Me.components)
        Me.chkDollarSign = New System.Windows.Forms.CheckBox()
        Me.txtMultiple = New System.Windows.Forms.TextBox()
        Me.tmr = New System.Windows.Forms.Timer(Me.components)
        Me.cmbLayoutDimensions = New System.Windows.Forms.ComboBox()
        Me.txtCustomX = New System.Windows.Forms.TextBox()
        Me.txtCustomY = New System.Windows.Forms.TextBox()
        Me.chkHideCents = New System.Windows.Forms.CheckBox()
        Me.fraClip = New System.Windows.Forms.GroupBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.scrBoxX = New System.Windows.Forms.HScrollBar()
        Me.scrBoxY = New System.Windows.Forms.HScrollBar()
        Me.cmdLayoutTemplate = New System.Windows.Forms.ComboBox()
        Me.cmbLayoutName = New System.Windows.Forms.ComboBox()
        Me.cmbPropFontName = New System.Windows.Forms.ComboBox()
        Me.cmbPropFontColor = New System.Windows.Forms.ComboBox()
        Me.lblBy = New System.Windows.Forms.Label()
        Me.lstItems = New System.Windows.Forms.CheckedListBox()
        CType(Me.imgPrintHelper, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cmbPageAlign
        '
        Me.cmbPageAlign.FormattingEnabled = True
        Me.cmbPageAlign.Location = New System.Drawing.Point(48, 18)
        Me.cmbPageAlign.Name = "cmbPageAlign"
        Me.cmbPageAlign.Size = New System.Drawing.Size(121, 21)
        Me.cmbPageAlign.TabIndex = 0
        '
        'imgPrintHelper
        '
        Me.imgPrintHelper.Location = New System.Drawing.Point(23, 45)
        Me.imgPrintHelper.Name = "imgPrintHelper"
        Me.imgPrintHelper.Size = New System.Drawing.Size(100, 50)
        Me.imgPrintHelper.TabIndex = 1
        Me.imgPrintHelper.TabStop = False
        '
        'fraBox
        '
        Me.fraBox.Location = New System.Drawing.Point(23, 132)
        Me.fraBox.Name = "fraBox"
        Me.fraBox.Size = New System.Drawing.Size(200, 100)
        Me.fraBox.TabIndex = 2
        Me.fraBox.TabStop = False
        Me.fraBox.Text = "GroupBox1"
        '
        'il
        '
        Me.il.ImageStream = CType(resources.GetObject("il.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.il.TransparentColor = System.Drawing.Color.Transparent
        Me.il.Images.SetKeyName(0, "NoPic.bmp")
        Me.il.Images.SetKeyName(1, "NoPic2.bmp")
        '
        'chkDollarSign
        '
        Me.chkDollarSign.AutoSize = True
        Me.chkDollarSign.Location = New System.Drawing.Point(23, 253)
        Me.chkDollarSign.Name = "chkDollarSign"
        Me.chkDollarSign.Size = New System.Drawing.Size(81, 17)
        Me.chkDollarSign.TabIndex = 3
        Me.chkDollarSign.Text = "CheckBox1"
        Me.chkDollarSign.UseVisualStyleBackColor = True
        '
        'txtMultiple
        '
        Me.txtMultiple.Location = New System.Drawing.Point(29, 291)
        Me.txtMultiple.Name = "txtMultiple"
        Me.txtMultiple.Size = New System.Drawing.Size(100, 20)
        Me.txtMultiple.TabIndex = 4
        '
        'cmbLayoutDimensions
        '
        Me.cmbLayoutDimensions.FormattingEnabled = True
        Me.cmbLayoutDimensions.Location = New System.Drawing.Point(23, 396)
        Me.cmbLayoutDimensions.Name = "cmbLayoutDimensions"
        Me.cmbLayoutDimensions.Size = New System.Drawing.Size(121, 21)
        Me.cmbLayoutDimensions.TabIndex = 6
        '
        'txtCustomX
        '
        Me.txtCustomX.Location = New System.Drawing.Point(18, 423)
        Me.txtCustomX.Name = "txtCustomX"
        Me.txtCustomX.Size = New System.Drawing.Size(100, 20)
        Me.txtCustomX.TabIndex = 7
        '
        'txtCustomY
        '
        Me.txtCustomY.Location = New System.Drawing.Point(256, 12)
        Me.txtCustomY.Name = "txtCustomY"
        Me.txtCustomY.Size = New System.Drawing.Size(100, 20)
        Me.txtCustomY.TabIndex = 8
        '
        'chkHideCents
        '
        Me.chkHideCents.AutoSize = True
        Me.chkHideCents.Location = New System.Drawing.Point(256, 45)
        Me.chkHideCents.Name = "chkHideCents"
        Me.chkHideCents.Size = New System.Drawing.Size(81, 17)
        Me.chkHideCents.TabIndex = 9
        Me.chkHideCents.Text = "CheckBox1"
        Me.chkHideCents.UseVisualStyleBackColor = True
        '
        'fraClip
        '
        Me.fraClip.Location = New System.Drawing.Point(256, 79)
        Me.fraClip.Name = "fraClip"
        Me.fraClip.Size = New System.Drawing.Size(200, 100)
        Me.fraClip.TabIndex = 10
        Me.fraClip.TabStop = False
        Me.fraClip.Text = "GroupBox1"
        '
        'GroupBox1
        '
        Me.GroupBox1.Location = New System.Drawing.Point(256, 185)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(200, 100)
        Me.GroupBox1.TabIndex = 11
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "GroupBox1"
        '
        'scrBoxX
        '
        Me.scrBoxX.Location = New System.Drawing.Point(256, 301)
        Me.scrBoxX.Name = "scrBoxX"
        Me.scrBoxX.Size = New System.Drawing.Size(80, 22)
        Me.scrBoxX.TabIndex = 12
        '
        'scrBoxY
        '
        Me.scrBoxY.Location = New System.Drawing.Point(256, 334)
        Me.scrBoxY.Name = "scrBoxY"
        Me.scrBoxY.Size = New System.Drawing.Size(80, 22)
        Me.scrBoxY.TabIndex = 13
        '
        'cmdLayoutTemplate
        '
        Me.cmdLayoutTemplate.FormattingEnabled = True
        Me.cmdLayoutTemplate.Location = New System.Drawing.Point(235, 380)
        Me.cmdLayoutTemplate.Name = "cmdLayoutTemplate"
        Me.cmdLayoutTemplate.Size = New System.Drawing.Size(121, 21)
        Me.cmdLayoutTemplate.TabIndex = 14
        '
        'cmbLayoutName
        '
        Me.cmbLayoutName.FormattingEnabled = True
        Me.cmbLayoutName.Location = New System.Drawing.Point(235, 407)
        Me.cmbLayoutName.Name = "cmbLayoutName"
        Me.cmbLayoutName.Size = New System.Drawing.Size(121, 21)
        Me.cmbLayoutName.TabIndex = 15
        '
        'cmbPropFontName
        '
        Me.cmbPropFontName.FormattingEnabled = True
        Me.cmbPropFontName.Location = New System.Drawing.Point(235, 434)
        Me.cmbPropFontName.Name = "cmbPropFontName"
        Me.cmbPropFontName.Size = New System.Drawing.Size(121, 21)
        Me.cmbPropFontName.TabIndex = 16
        '
        'cmbPropFontColor
        '
        Me.cmbPropFontColor.FormattingEnabled = True
        Me.cmbPropFontColor.Location = New System.Drawing.Point(480, 18)
        Me.cmbPropFontColor.Name = "cmbPropFontColor"
        Me.cmbPropFontColor.Size = New System.Drawing.Size(121, 21)
        Me.cmbPropFontColor.TabIndex = 17
        '
        'lblBy
        '
        Me.lblBy.AutoSize = True
        Me.lblBy.Location = New System.Drawing.Point(477, 49)
        Me.lblBy.Name = "lblBy"
        Me.lblBy.Size = New System.Drawing.Size(39, 13)
        Me.lblBy.TabIndex = 18
        Me.lblBy.Text = "Label1"
        '
        'lstItems
        '
        Me.lstItems.FormattingEnabled = True
        Me.lstItems.Location = New System.Drawing.Point(26, 317)
        Me.lstItems.Name = "lstItems"
        Me.lstItems.Size = New System.Drawing.Size(92, 49)
        Me.lstItems.TabIndex = 19
        '
        'frmDesignTag
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.lstItems)
        Me.Controls.Add(Me.lblBy)
        Me.Controls.Add(Me.cmbPropFontColor)
        Me.Controls.Add(Me.cmbPropFontName)
        Me.Controls.Add(Me.cmbLayoutName)
        Me.Controls.Add(Me.cmdLayoutTemplate)
        Me.Controls.Add(Me.scrBoxY)
        Me.Controls.Add(Me.scrBoxX)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.fraClip)
        Me.Controls.Add(Me.chkHideCents)
        Me.Controls.Add(Me.txtCustomY)
        Me.Controls.Add(Me.txtCustomX)
        Me.Controls.Add(Me.cmbLayoutDimensions)
        Me.Controls.Add(Me.txtMultiple)
        Me.Controls.Add(Me.chkDollarSign)
        Me.Controls.Add(Me.fraBox)
        Me.Controls.Add(Me.imgPrintHelper)
        Me.Controls.Add(Me.cmbPageAlign)
        Me.Name = "frmDesignTag"
        Me.Text = "frmDesignTag"
        CType(Me.imgPrintHelper, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cmbPageAlign As ComboBox
    Friend WithEvents imgPrintHelper As PictureBox
    Friend WithEvents fraBox As GroupBox
    Friend WithEvents il As ImageList
    Friend WithEvents chkDollarSign As CheckBox
    Friend WithEvents txtMultiple As TextBox
    Friend WithEvents tmr As Timer
    Friend WithEvents cmbLayoutDimensions As ComboBox
    Friend WithEvents txtCustomX As TextBox
    Friend WithEvents txtCustomY As TextBox
    Friend WithEvents chkHideCents As CheckBox
    Friend WithEvents fraClip As GroupBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents scrBoxX As HScrollBar
    Friend WithEvents scrBoxY As HScrollBar
    Friend WithEvents cmdLayoutTemplate As ComboBox
    Friend WithEvents cmbLayoutName As ComboBox
    Friend WithEvents cmbPropFontName As ComboBox
    Friend WithEvents cmbPropFontColor As ComboBox
    Friend WithEvents lblBy As Label
    Friend WithEvents lstItems As CheckedListBox
End Class
