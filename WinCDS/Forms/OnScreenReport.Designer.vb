﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OnScreenReport
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
        Me.UGridIO1 = New WinCDS.UGridIO()
        Me.cmbManuf = New System.Windows.Forms.ComboBox()
        Me.lblCaption = New System.Windows.Forms.Label()
        Me.lblPrevBal2 = New System.Windows.Forms.Label()
        Me.lblPrevBal = New System.Windows.Forms.Label()
        Me.lblBalDue2 = New System.Windows.Forms.Label()
        Me.UGridIO2 = New WinCDS.UGridIO()
        Me.fraControls1 = New System.Windows.Forms.GroupBox()
        Me.txtLocation = New System.Windows.Forms.TextBox()
        Me.cmdMenu = New System.Windows.Forms.Button()
        Me.cmdPrint = New System.Windows.Forms.Button()
        Me.cmdNext = New System.Windows.Forms.Button()
        Me.cmdAllStores = New System.Windows.Forms.Button()
        Me.fraControls2 = New System.Windows.Forms.GroupBox()
        Me.cmdPrint2 = New System.Windows.Forms.Button()
        Me.cmdMenu2 = New System.Windows.Forms.Button()
        Me.cmdNext2 = New System.Windows.Forms.Button()
        Me.cmdApply = New System.Windows.Forms.Button()
        Me.cmdReturn = New System.Windows.Forms.Button()
        Me.cmbGrid2 = New System.Windows.Forms.ComboBox()
        Me.cmdAdjustTax = New System.Windows.Forms.Button()
        Me.lblDiffTax = New System.Windows.Forms.Label()
        Me.lblBalDue = New System.Windows.Forms.Label()
        Me.txtDiffTax0 = New System.Windows.Forms.TextBox()
        Me.txtBalDue = New System.Windows.Forms.TextBox()
        Me.lblRate0 = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.cmdAdd = New System.Windows.Forms.CheckBox()
        Me.fraControls1.SuspendLayout()
        Me.fraControls2.SuspendLayout()
        Me.SuspendLayout()
        '
        'UGridIO1
        '
        Me.UGridIO1.Activated = False
        Me.UGridIO1.Col = 1
        Me.UGridIO1.firstrow = 1
        Me.UGridIO1.Loading = False
        Me.UGridIO1.Location = New System.Drawing.Point(12, 12)
        Me.UGridIO1.MaxCols = 2
        Me.UGridIO1.MaxRows = 10
        Me.UGridIO1.Name = "UGridIO1"
        Me.UGridIO1.Row = 0
        Me.UGridIO1.Size = New System.Drawing.Size(776, 159)
        Me.UGridIO1.TabIndex = 1
        '
        'cmbManuf
        '
        Me.cmbManuf.FormattingEnabled = True
        Me.cmbManuf.Location = New System.Drawing.Point(12, 192)
        Me.cmbManuf.Name = "cmbManuf"
        Me.cmbManuf.Size = New System.Drawing.Size(121, 21)
        Me.cmbManuf.TabIndex = 2
        Me.cmbManuf.Visible = False
        '
        'lblCaption
        '
        Me.lblCaption.AutoSize = True
        Me.lblCaption.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCaption.Location = New System.Drawing.Point(307, 215)
        Me.lblCaption.Name = "lblCaption"
        Me.lblCaption.Size = New System.Drawing.Size(95, 31)
        Me.lblCaption.TabIndex = 3
        Me.lblCaption.Text = "Label1"
        '
        'lblPrevBal2
        '
        Me.lblPrevBal2.AutoSize = True
        Me.lblPrevBal2.Location = New System.Drawing.Point(504, 222)
        Me.lblPrevBal2.Name = "lblPrevBal2"
        Me.lblPrevBal2.Size = New System.Drawing.Size(69, 13)
        Me.lblPrevBal2.TabIndex = 4
        Me.lblPrevBal2.Text = "Previous Bal:"
        '
        'lblPrevBal
        '
        Me.lblPrevBal.AutoSize = True
        Me.lblPrevBal.Location = New System.Drawing.Point(611, 224)
        Me.lblPrevBal.Name = "lblPrevBal"
        Me.lblPrevBal.Size = New System.Drawing.Size(39, 13)
        Me.lblPrevBal.TabIndex = 5
        Me.lblPrevBal.Text = "Label1"
        '
        'lblBalDue2
        '
        Me.lblBalDue2.AutoSize = True
        Me.lblBalDue2.Location = New System.Drawing.Point(316, 271)
        Me.lblBalDue2.Name = "lblBalDue2"
        Me.lblBalDue2.Size = New System.Drawing.Size(72, 13)
        Me.lblBalDue2.TabIndex = 6
        Me.lblBalDue2.Text = "Balance Due:"
        '
        'UGridIO2
        '
        Me.UGridIO2.Activated = False
        Me.UGridIO2.Col = 1
        Me.UGridIO2.firstrow = 1
        Me.UGridIO2.Loading = False
        Me.UGridIO2.Location = New System.Drawing.Point(12, 234)
        Me.UGridIO2.MaxCols = 2
        Me.UGridIO2.MaxRows = 10
        Me.UGridIO2.Name = "UGridIO2"
        Me.UGridIO2.Row = 0
        Me.UGridIO2.Size = New System.Drawing.Size(776, 96)
        Me.UGridIO2.TabIndex = 7
        '
        'fraControls1
        '
        Me.fraControls1.Controls.Add(Me.txtLocation)
        Me.fraControls1.Controls.Add(Me.cmdMenu)
        Me.fraControls1.Controls.Add(Me.cmdPrint)
        Me.fraControls1.Controls.Add(Me.cmdNext)
        Me.fraControls1.Controls.Add(Me.cmdAllStores)
        Me.fraControls1.Location = New System.Drawing.Point(12, 353)
        Me.fraControls1.Name = "fraControls1"
        Me.fraControls1.Size = New System.Drawing.Size(274, 67)
        Me.fraControls1.TabIndex = 8
        Me.fraControls1.TabStop = False
        '
        'txtLocation
        '
        Me.txtLocation.Location = New System.Drawing.Point(117, 34)
        Me.txtLocation.Name = "txtLocation"
        Me.txtLocation.Size = New System.Drawing.Size(79, 20)
        Me.txtLocation.TabIndex = 4
        Me.txtLocation.Visible = False
        '
        'cmdMenu
        '
        Me.cmdMenu.Location = New System.Drawing.Point(202, 16)
        Me.cmdMenu.Name = "cmdMenu"
        Me.cmdMenu.Size = New System.Drawing.Size(65, 35)
        Me.cmdMenu.TabIndex = 3
        Me.cmdMenu.Text = "&Menu"
        Me.cmdMenu.UseVisualStyleBackColor = True
        '
        'cmdPrint
        '
        Me.cmdPrint.Location = New System.Drawing.Point(137, 16)
        Me.cmdPrint.Name = "cmdPrint"
        Me.cmdPrint.Size = New System.Drawing.Size(65, 35)
        Me.cmdPrint.TabIndex = 2
        Me.cmdPrint.Text = "&Print"
        Me.cmdPrint.UseVisualStyleBackColor = True
        '
        'cmdNext
        '
        Me.cmdNext.Location = New System.Drawing.Point(72, 16)
        Me.cmdNext.Name = "cmdNext"
        Me.cmdNext.Size = New System.Drawing.Size(65, 35)
        Me.cmdNext.TabIndex = 1
        Me.cmdNext.Text = "&Next"
        Me.cmdNext.UseVisualStyleBackColor = True
        '
        'cmdAllStores
        '
        Me.cmdAllStores.Location = New System.Drawing.Point(7, 16)
        Me.cmdAllStores.Name = "cmdAllStores"
        Me.cmdAllStores.Size = New System.Drawing.Size(65, 35)
        Me.cmdAllStores.TabIndex = 0
        Me.cmdAllStores.Text = "&All Stores"
        Me.cmdAllStores.UseVisualStyleBackColor = True
        '
        'fraControls2
        '
        Me.fraControls2.Controls.Add(Me.cmdAdd)
        Me.fraControls2.Controls.Add(Me.cmdPrint2)
        Me.fraControls2.Controls.Add(Me.cmdMenu2)
        Me.fraControls2.Controls.Add(Me.cmdNext2)
        Me.fraControls2.Controls.Add(Me.cmdApply)
        Me.fraControls2.Controls.Add(Me.cmdReturn)
        Me.fraControls2.Location = New System.Drawing.Point(330, 377)
        Me.fraControls2.Name = "fraControls2"
        Me.fraControls2.Size = New System.Drawing.Size(302, 58)
        Me.fraControls2.TabIndex = 9
        Me.fraControls2.TabStop = False
        '
        'cmdPrint2
        '
        Me.cmdPrint2.Location = New System.Drawing.Point(246, 15)
        Me.cmdPrint2.Name = "cmdPrint2"
        Me.cmdPrint2.Size = New System.Drawing.Size(47, 33)
        Me.cmdPrint2.TabIndex = 5
        Me.cmdPrint2.Text = "&Print"
        Me.cmdPrint2.UseVisualStyleBackColor = True
        '
        'cmdMenu2
        '
        Me.cmdMenu2.Location = New System.Drawing.Point(198, 15)
        Me.cmdMenu2.Name = "cmdMenu2"
        Me.cmdMenu2.Size = New System.Drawing.Size(47, 33)
        Me.cmdMenu2.TabIndex = 4
        Me.cmdMenu2.Text = "Men&u"
        Me.cmdMenu2.UseVisualStyleBackColor = True
        '
        'cmdNext2
        '
        Me.cmdNext2.Location = New System.Drawing.Point(150, 15)
        Me.cmdNext2.Name = "cmdNext2"
        Me.cmdNext2.Size = New System.Drawing.Size(47, 33)
        Me.cmdNext2.TabIndex = 3
        Me.cmdNext2.Text = "Ne&xt"
        Me.cmdNext2.UseVisualStyleBackColor = True
        '
        'cmdApply
        '
        Me.cmdApply.Location = New System.Drawing.Point(102, 15)
        Me.cmdApply.Name = "cmdApply"
        Me.cmdApply.Size = New System.Drawing.Size(47, 33)
        Me.cmdApply.TabIndex = 2
        Me.cmdApply.Text = "Appl&y"
        Me.cmdApply.UseVisualStyleBackColor = True
        '
        'cmdReturn
        '
        Me.cmdReturn.Location = New System.Drawing.Point(6, 15)
        Me.cmdReturn.Name = "cmdReturn"
        Me.cmdReturn.Size = New System.Drawing.Size(47, 33)
        Me.cmdReturn.TabIndex = 0
        Me.cmdReturn.Text = "&Return"
        Me.cmdReturn.UseVisualStyleBackColor = True
        '
        'cmbGrid2
        '
        Me.cmbGrid2.FormattingEnabled = True
        Me.cmbGrid2.Location = New System.Drawing.Point(149, 460)
        Me.cmbGrid2.Name = "cmbGrid2"
        Me.cmbGrid2.Size = New System.Drawing.Size(121, 21)
        Me.cmbGrid2.TabIndex = 10
        Me.cmbGrid2.Visible = False
        '
        'cmdAdjustTax
        '
        Me.cmdAdjustTax.Location = New System.Drawing.Point(509, 339)
        Me.cmdAdjustTax.Name = "cmdAdjustTax"
        Me.cmdAdjustTax.Size = New System.Drawing.Size(35, 23)
        Me.cmdAdjustTax.TabIndex = 11
        Me.cmdAdjustTax.Text = "&*"
        Me.cmdAdjustTax.UseVisualStyleBackColor = True
        Me.cmdAdjustTax.Visible = False
        '
        'lblDiffTax
        '
        Me.lblDiffTax.AutoSize = True
        Me.lblDiffTax.Location = New System.Drawing.Point(552, 339)
        Me.lblDiffTax.Name = "lblDiffTax"
        Me.lblDiffTax.Size = New System.Drawing.Size(80, 13)
        Me.lblDiffTax.TabIndex = 12
        Me.lblDiffTax.Text = "Difference Tax:"
        '
        'lblBalDue
        '
        Me.lblBalDue.AutoSize = True
        Me.lblBalDue.Location = New System.Drawing.Point(560, 361)
        Me.lblBalDue.Name = "lblBalDue"
        Me.lblBalDue.Size = New System.Drawing.Size(72, 13)
        Me.lblBalDue.TabIndex = 13
        Me.lblBalDue.Text = "Balance Due:"
        '
        'txtDiffTax0
        '
        Me.txtDiffTax0.Location = New System.Drawing.Point(638, 336)
        Me.txtDiffTax0.Name = "txtDiffTax0"
        Me.txtDiffTax0.Size = New System.Drawing.Size(100, 20)
        Me.txtDiffTax0.TabIndex = 14
        '
        'txtBalDue
        '
        Me.txtBalDue.Location = New System.Drawing.Point(638, 362)
        Me.txtBalDue.Name = "txtBalDue"
        Me.txtBalDue.Size = New System.Drawing.Size(100, 20)
        Me.txtBalDue.TabIndex = 15
        '
        'lblRate0
        '
        Me.lblRate0.AutoSize = True
        Me.lblRate0.Location = New System.Drawing.Point(744, 339)
        Me.lblRate0.Name = "lblRate0"
        Me.lblRate0.Size = New System.Drawing.Size(40, 13)
        Me.lblRate0.TabIndex = 16
        Me.lblRate0.Text = "lblRate"
        '
        'cmdAdd
        '
        Me.cmdAdd.Appearance = System.Windows.Forms.Appearance.Button
        Me.cmdAdd.AutoSize = True
        Me.cmdAdd.Location = New System.Drawing.Point(59, 20)
        Me.cmdAdd.Name = "cmdAdd"
        Me.cmdAdd.Size = New System.Drawing.Size(36, 23)
        Me.cmdAdd.TabIndex = 6
        Me.cmdAdd.Text = "&Add"
        Me.cmdAdd.UseVisualStyleBackColor = True
        '
        'OnScreenReport
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 552)
        Me.Controls.Add(Me.lblRate0)
        Me.Controls.Add(Me.txtBalDue)
        Me.Controls.Add(Me.txtDiffTax0)
        Me.Controls.Add(Me.lblBalDue)
        Me.Controls.Add(Me.lblDiffTax)
        Me.Controls.Add(Me.fraControls2)
        Me.Controls.Add(Me.cmdAdjustTax)
        Me.Controls.Add(Me.cmbGrid2)
        Me.Controls.Add(Me.fraControls1)
        Me.Controls.Add(Me.UGridIO2)
        Me.Controls.Add(Me.lblBalDue2)
        Me.Controls.Add(Me.lblPrevBal)
        Me.Controls.Add(Me.lblPrevBal2)
        Me.Controls.Add(Me.lblCaption)
        Me.Controls.Add(Me.cmbManuf)
        Me.Controls.Add(Me.UGridIO1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "OnScreenReport"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.fraControls1.ResumeLayout(False)
        Me.fraControls1.PerformLayout()
        Me.fraControls2.ResumeLayout(False)
        Me.fraControls2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents UGridIO1 As UGridIO
    Friend WithEvents cmbManuf As ComboBox
    Friend WithEvents lblCaption As Label
    Friend WithEvents lblPrevBal2 As Label
    Friend WithEvents lblPrevBal As Label
    Friend WithEvents lblBalDue2 As Label
    Friend WithEvents UGridIO2 As UGridIO
    Friend WithEvents fraControls1 As GroupBox
    Friend WithEvents cmdMenu As Button
    Friend WithEvents cmdPrint As Button
    Friend WithEvents cmdNext As Button
    Friend WithEvents cmdAllStores As Button
    Friend WithEvents txtLocation As TextBox
    Friend WithEvents fraControls2 As GroupBox
    Friend WithEvents cmdMenu2 As Button
    Friend WithEvents cmdNext2 As Button
    Friend WithEvents cmdApply As Button
    Friend WithEvents cmdReturn As Button
    Friend WithEvents cmbGrid2 As ComboBox
    Friend WithEvents cmdAdjustTax As Button
    Friend WithEvents lblDiffTax As Label
    Friend WithEvents lblBalDue As Label
    Friend WithEvents txtDiffTax0 As TextBox
    Friend WithEvents txtBalDue As TextBox
    Friend WithEvents cmdPrint2 As Button
    Friend WithEvents lblRate0 As Label
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents cmdAdd As CheckBox
End Class
