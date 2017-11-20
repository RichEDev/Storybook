Module ModDates
    Public Function GetNextRunDate(ByVal curDate As DateTime, ByVal lastRunDate As DateTime, ByVal FreqType As emailFreq, ByVal FreqParam As Integer) As DateTime
        Dim retDate, nextDate As DateTime
        Dim tmpDateTime As New DateTime(curDate.Year, curDate.Month, curDate.Day, lastRunDate.Hour, lastRunDate.Minute, lastRunDate.Second)
        Dim tmpDateStr As String

        retDate = curDate
        Try
            Select Case FreqType
                Case emailFreq.Daily
                    AddToLog("Frequency type set to 'Daily'")
                    nextDate = tmpDateTime.AddDays(1)

                Case emailFreq.Every_n_Days
                    AddToLog("Frequency type set to 'Every " & FreqParam.ToString & " days'")
                    nextDate = tmpDateTime.AddDays(FreqParam)

                Case emailFreq.MonthlyOnDay
                    AddToLog("Frequency type set to 'Monthly'")
                    nextDate = tmpDateTime.AddMonths(1)

                Case emailFreq.MonthlyOnFirstXDay
                    AddToLog("Frequency type set to 'Monthly on 1st X day' = [" & CType(FreqParam, DayOfWeek) & "]")
                    Dim x As Integer

                    For x = 1 To 7
                        If curDate.Month + 1 > 12 Then
                            tmpDateStr = tmpDateTime.Year + 1 & "-01-" & x.ToString("00") & " " & tmpDateTime.Hour.ToString & ":" & tmpDateTime.Minute.ToString & ":00"
                        Else
                            tmpDateStr = tmpDateTime.Year & "-" & (tmpDateTime.Month + 1).ToString("00") & "-" & x.ToString("00") & " " & tmpDateTime.Hour.ToString & ":" & lastRunDate.Minute.ToString & ":00"
                        End If

                        nextDate = DateTime.Parse(tmpDateStr)
                        If nextDate.DayOfWeek = CType(FreqParam, DayOfWeek) Then
                            ' found the 1st occurrence of the required day
                            Exit For
                        End If
                    Next

                Case emailFreq.Once
                    ' this shouldn't be called for a one off execution

                Case emailFreq.Weekly
                    AddToLog("Frequency type set to 'Weekly'")
                    nextDate = tmpDateTime.AddDays(7)

                Case Else

                    Exit Try
            End Select
            retDate = nextDate

        Catch ex As Exception
            AddToLog("Error occurred obtaining the next scheduled run date")
            AddToLog(ex.Message)
        End Try
        Return retDate
    End Function


End Module
