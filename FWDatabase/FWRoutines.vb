Imports System.IO
Imports System.Diagnostics

Public Module FWRoutines
    Private Texts(9999) As String
    Private count As Integer
    Public Const MAXFIELDS As Integer = 230 ' one more than there are fields selectable
    Public Const MAXIDFIELDS As Integer = 10
    Public Const TOTAL_4COLSPX As String = "600"

    Public Enum ReportCnvDir
        TO_DB = 0
        FROM_DB = 1
    End Enum

    Structure LicencesItem
        Public Table As Integer
        Public Parent As String
        Public SortId As String
        Public FieldType As String
        Public DispName As String
        Public FieldName As String
        Public Chosen As Integer
        Public Sorted As Integer
        Public Subtotal As Integer
        Public Unique As Integer
        Public ColWidth As Integer
    End Structure

    Structure ViewSelection
        Public Table As String
        Public Field As String
        Public IsIDField As Boolean
        Public JoinTable As String
        Public JoinField As String
        Public JoinType As String
        Public HeaderText As String
        Public Display As Integer
        Public FieldID As Integer
        Public FieldType As String
        Public canTotal As Boolean
        Public ColumnWidth As Integer
        Public Sort As String
        Public SubTotal As Boolean
        Public Seed As Integer
        Public IsHidden As Boolean
        Public Isindirect As Boolean
        Public FieldGroup As String
    End Structure

    ' Data columns selected are stored in this array
    Public Selection(100) As LicencesItem
    Public LicencesItems As Integer

    ' Member function and subs
	'Public Sub RoutinesSetup(ByVal app As cFWSettings, ByVal db As cFWDBConnection)
	'	FWDb = db
	'	AppSettings = app
	'End Sub

    Public Function ReplaceString(ByVal s0, ByVal s1, ByVal s2) As String
        ' Replace all occurrences of s1 by s2 in the string s0

        Dim x
        x = InStr(s0, s1)
        Do While x <> 0
            s0 = Left(s0, x - 1) & s2 & Right(s0, Len(s0) - x - Len(s1) + 1)
            x = InStr(s0, s1)
        Loop
        ReplaceString = s0

    End Function

	Public Function checkDate(ByVal dateString As String, ByRef errString As String) As Boolean
		Dim theDay, theMonth, theYear As Integer
		Dim leapYear As Double
		Dim chDiv1 As Char
		Dim chDiv2 As Char

		theDay = Left(dateString, 2)
		theMonth = Mid(dateString, 4, 2)
		theYear = Mid(dateString, 7)
		chDiv1 = Mid(dateString, 3, 1)
		chDiv2 = Mid(dateString, 6, 1)

		leapYear = Val(theYear) / 4

		If chDiv1 <> "/" Or chDiv2 <> "/" Then
			errString = "Invalid separators. Use dd/mm/yyyy format"
			checkDate = False
			Exit Function
		End If

		If Val(theMonth) < 1 Or Val(theMonth) > 12 Then
			errString = "Invalid month entered"
			checkDate = False
			Exit Function
		End If

		If Val(theDay) < 1 Then
			errString = "Invalid day entered"
			checkDate = False
			Exit Function
		End If

		Select Case Val(theMonth)
			Case 1, 3, 5, 7, 8, 9, 10, 12
				If Val(theDay) > 31 Then
					errString = "Invalid day entered"
					checkDate = False
					Exit Function
				End If
			Case 2
				If CInt(leapYear) = leapYear Then
					If Val(theDay) > 29 Then
						errString = "Invalid day entered"
						checkDate = False
						Exit Function
					End If
				Else
					If Val(theDay) > 28 Then
						errString = "Invalid day entered"
						checkDate = False
						Exit Function
					End If
				End If
			Case 4, 6, 9, 11
				If Val(theDay) > 30 Then
					errString = "Invalid day entered"
					checkDate = False
					Exit Function
				End If
			Case Else
				errString = "Unknown month to check!"
				checkDate = False
		End Select
		checkDate = True
	End Function

	Public Function FormatDate(ByVal DateStr As String) As String
		Dim tmpstr As String

		tmpstr = ""

		If DateStr <> "" Then
			If IsDate(DateStr) = True Then
				tmpstr = Format(CDate(DateStr), "dd/MM/yyyy")
			End If
		End If

		FormatDate = tmpstr
	End Function

	Public Sub FWLog(ByVal app As HttpApplication, ByVal logfilename As String, ByVal Msg As String)
		' This routine time-stamps the message and writes it to the log file
		Dim logfile As StreamWriter
		Dim mappedFileName As String

		Try
			Debug.WriteLine("Entered Log() function")

			Debug.WriteLine("logfilename = " & logfilename)
			Debug.WriteLine(app.Server.MapPath(logfilename))

			mappedFileName = app.Server.MapPath(logfilename)

			Debug.WriteLine("mappedFileName = " & mappedFileName)

			logfile = New StreamWriter(mappedFileName, True)

			logfile.WriteLine(Now.ToShortDateString & " " & Now.ToShortTimeString & " : " & Msg)
			Debug.WriteLine(Now.ToShortDateString & " " & Now.ToShortTimeString & " : " & Msg)

			logfile.Flush()
			logfile.Close()
			logfile = Nothing

			Debug.WriteLine("Log() executed successfully")

		Catch ex As Exception

			Debug.WriteLine("Log() of message failed " & Now.ToShortDateString & " : " & Now.ToShortTimeString)
			Debug.WriteLine("Message = " & Trim(Msg))
			Debug.WriteLine("Error : " & ex.Message)
		End Try
	End Sub


End Module
