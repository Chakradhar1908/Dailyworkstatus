﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPrintPreviewDocument
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
        Me.picPicture = New System.Windows.Forms.PictureBox()
        Me.cmdNavigate7 = New System.Windows.Forms.Button()
        Me.fraNavigate = New System.Windows.Forms.GroupBox()
        Me.lblHelp = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        CType(Me.picPicture, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.fraNavigate.SuspendLayout()
        Me.SuspendLayout()
        '
        'picPicture
        '
        Me.picPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picPicture.Location = New System.Drawing.Point(3, 0)
        Me.picPicture.Name = "picPicture"
        Me.picPicture.Size = New System.Drawing.Size(282, 125)
        Me.picPicture.TabIndex = 0
        Me.picPicture.TabStop = False
        '
        'cmdNavigate7
        '
        Me.cmdNavigate7.Location = New System.Drawing.Point(6, 14)
        Me.cmdNavigate7.Name = "cmdNavigate7"
        Me.cmdNavigate7.Size = New System.Drawing.Size(75, 23)
        Me.cmdNavigate7.TabIndex = 1
        Me.cmdNavigate7.Text = "Goto"
        Me.cmdNavigate7.UseVisualStyleBackColor = True
        '
        'fraNavigate
        '
        Me.fraNavigate.Controls.Add(Me.cmdNavigate7)
        Me.fraNavigate.Location = New System.Drawing.Point(69, 164)
        Me.fraNavigate.Name = "fraNavigate"
        Me.fraNavigate.Size = New System.Drawing.Size(200, 53)
        Me.fraNavigate.TabIndex = 2
        Me.fraNavigate.TabStop = False
        '
        'lblHelp
        '
        Me.lblHelp.AutoSize = True
        Me.lblHelp.Location = New System.Drawing.Point(24, 188)
        Me.lblHelp.Name = "lblHelp"
        Me.lblHelp.Size = New System.Drawing.Size(39, 13)
        Me.lblHelp.TabIndex = 3
        Me.lblHelp.Text = "Label1"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(188, 54)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'frmPrintPreviewDocument
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(322, 236)
        Me.ControlBox = False
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.lblHelp)
        Me.Controls.Add(Me.fraNavigate)
        Me.Controls.Add(Me.picPicture)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmPrintPreviewDocument"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Up"
        CType(Me.picPicture, System.ComponentModel.ISupportInitialize).EndInit()
        Me.fraNavigate.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents picPicture As PictureBox
    Friend WithEvents cmdNavigate7 As Button
    Friend WithEvents fraNavigate As GroupBox
    Friend WithEvents lblHelp As Label
    Friend WithEvents Button1 As Button
End Class
