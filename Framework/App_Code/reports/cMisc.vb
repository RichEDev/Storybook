Public Enum Action
    Add = 1
    Edit
    Delete
End Enum

Public Class cReportMisc
	Public Shared ReadOnly Property path() As String
		Get
			If System.Web.HttpRuntime.AppDomainAppVirtualPath = "/" Then
				Return ""
			Else
				Return System.Web.HttpRuntime.AppDomainAppVirtualPath
			End If
		End Get
	End Property
End Class

Public Class wizardStep
    Private nActualStep As Integer
    Private nLogicalStep As Integer
    Private sLabel As String

    Public Sub New(ByVal actualstep As Integer, ByVal logicalstep As Integer, ByVal label As String)
        nActualStep = actualstep
        nLogicalStep = logicalstep
        sLabel = label
    End Sub

#Region "properties"
    Public ReadOnly Property actualstep() As Integer
        Get
            Return nActualStep
        End Get
    End Property

    Public ReadOnly Property logicalstep() As Integer
        Get
            Return nLogicalStep
        End Get
    End Property

    Public ReadOnly Property label() As String
        Get
            Return sLabel
        End Get
    End Property
#End Region
End Class