﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class AddOnAcc
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.fraControls = New System.Windows.Forms.GroupBox()
        Me.cmdRevolving = New System.Windows.Forms.CheckBox()
        Me.cmdNew = New System.Windows.Forms.Button()
        Me.cmdAddToNew = New System.Windows.Forms.Button()
        Me.cmdAdd = New System.Windows.Forms.Button()
        Me.lblHeadings = New System.Windows.Forms.Label()
        Me.lstAccounts = New System.Windows.Forms.ListBox()
        Me.fraControls.SuspendLayout()
        Me.SuspendLayout()
        '
        'fraControls
        '
        Me.fraControls.Controls.Add(Me.cmdRevolving)
        Me.fraControls.Controls.Add(Me.cmdNew)
        Me.fraControls.Controls.Add(Me.cmdAddToNew)
        Me.fraControls.Controls.Add(Me.cmdAdd)
        Me.fraControls.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.fraControls.Location = New System.Drawing.Point(8, 6)
        Me.fraControls.Name = "fraControls"
        Me.fraControls.Size = New System.Drawing.Size(131, 165)
        Me.fraControls.TabIndex = 0
        Me.fraControls.TabStop = False
        Me.fraControls.Text = "Existing Account:"
        '
        'cmdRevolving
        '
        Me.cmdRevolving.Appearance = System.Windows.Forms.Appearance.Button
        Me.cmdRevolving.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdRevolving.Location = New System.Drawing.Point(6, 125)
        Me.cmdRevolving.Name = "cmdRevolving"
        Me.cmdRevolving.Size = New System.Drawing.Size(119, 34)
        Me.cmdRevolving.TabIndex = 3
        Me.cmdRevolving.Text = "&Revolving"
        Me.cmdRevolving.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.cmdRevolving.UseVisualStyleBackColor = True
        '
        'cmdNew
        '
        Me.cmdNew.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdNew.Location = New System.Drawing.Point(6, 91)
        Me.cmdNew.Name = "cmdNew"
        Me.cmdNew.Size = New System.Drawing.Size(119, 34)
        Me.cmdNew.TabIndex = 2
        Me.cmdNew.Text = "&New Account"
        Me.cmdNew.UseVisualStyleBackColor = True
        '
        'cmdAddToNew
        '
        Me.cmdAddToNew.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdAddToNew.Location = New System.Drawing.Point(6, 57)
        Me.cmdAddToNew.Name = "cmdAddToNew"
        Me.cmdAddToNew.Size = New System.Drawing.Size(119, 34)
        Me.cmdAddToNew.TabIndex = 1
        Me.cmdAddToNew.Text = "Add On &To New"
        Me.cmdAddToNew.UseVisualStyleBackColor = True
        '
        'cmdAdd
        '
        Me.cmdAdd.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdAdd.Location = New System.Drawing.Point(6, 23)
        Me.cmdAdd.Name = "cmdAdd"
        Me.cmdAdd.Size = New System.Drawing.Size(119, 34)
        Me.cmdAdd.TabIndex = 0
        Me.cmdAdd.Text = "&Add On Account"
        Me.cmdAdd.UseVisualStyleBackColor = True
        '
        'lblHeadings
        '
        Me.lblHeadings.Location = New System.Drawing.Point(156, 6)
        Me.lblHeadings.Name = "lblHeadings"
        Me.lblHeadings.Size = New System.Drawing.Size(324, 13)
        Me.lblHeadings.TabIndex = 1
        Me.lblHeadings.Text = "Account No           Telephone                          Balance"
        '
        'lstAccounts
        '
        Me.lstAccounts.FormattingEnabled = True
        Me.lstAccounts.Location = New System.Drawing.Point(152, 22)
        Me.lstAccounts.Name = "lstAccounts"
        Me.lstAccounts.Size = New System.Drawing.Size(328, 147)
        Me.lstAccounts.TabIndex = 2
        '
        'AddOnAcc
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(485, 173)
        Me.Controls.Add(Me.lstAccounts)
        Me.Controls.Add(Me.lblHeadings)
        Me.Controls.Add(Me.fraControls)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AddOnAcc"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Make Selection"
        Me.fraControls.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents fraControls As GroupBox
    Friend WithEvents cmdNew As Button
    Friend WithEvents cmdAddToNew As Button
    Friend WithEvents cmdAdd As Button
    Friend WithEvents lblHeadings As Label
    Friend WithEvents lstAccounts As ListBox
    Friend WithEvents cmdRevolving As CheckBox
End Class
