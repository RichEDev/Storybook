Imports Microsoft.VisualBasic
Imports SpendManagementLibrary
Imports FWBase
Imports System.Diagnostics
Imports System.Threading
Imports System
Imports System.Data

Namespace Framework2006
    Public Class cEmail
        Inherits System.Web.HttpApplication
        Public fws As New cFWSettings
        Protected WithEvents aTimer As System.Timers.Timer
        Public appinfo As System.Web.HttpApplication
        Const EMAIL_LOGFILE As String = "reports\EmailNotify.log"

        Public Sub InitSchedule(ByVal stateInfo As Object)
            ' delay for a 60 seconds to enable the application properties to be loaded etc.
            Debug.WriteLine("cEmail:InitSchedule:Opening InitSchedule()")

            Debug.WriteLine("cEmail:InitSchedule:fws.glMailServer = " & fws.glMailServer.Trim)
            aTimer = New System.Timers.Timer

            If fws.glWebEmailerCheckInterval > 0 Then
                Debug.WriteLine("cEmail:InitSchedule:Activating the Timer")
                aTimer.AutoReset = True
                aTimer.Interval = fws.glWebEmailerCheckInterval * (60 * 1000)
                aTimer.Enabled = True
                aTimer.Start()
            Else
                Debug.WriteLine("cEmail:InitSchedule:NOT activating the timer")
                aTimer.Enabled = False
                aTimer.AutoReset = False
                aTimer.Stop()
            End If

            Debug.WriteLine("cEmail:InitSchedule:fws.glWebEmailerCheckInterval = " & fws.glWebEmailerCheckInterval.ToString.Trim)
            Debug.WriteLine("cEmail:InitSchedule:InitSchedule completed.")
        End Sub

        Private Sub aTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles aTimer.Elapsed
            Try
                Debug.WriteLine("cEmail:aTimer_Elapsed:Timer elapsed")

                ELog(EMAIL_LOGFILE, "Email thread woke up.")

                'Log(System.Web.HttpContext.Current.ApplicationInstance, fws.glEmailLog & EMAIL_LOGFILE, "Woke up for check at " & Trim(Now.ToShortDateString) & " " & Trim(Now.ToShortTimeString))

                'If CheckSchedule() = False Then
                '    Debug.WriteLine("cEmail:aTimer_Elapsed:CheckSchedule() returned false")
                'End If

                ' update the interval in case it has changed
                'aTimer.Interval = fws.glWebEmailerCheckInterval * (60 * 1000)

            Catch ex As Exception
                Debug.WriteLine("cEmail:aTimer_Elapsed:" & ex.Message)
            End Try

        End Sub

        Private Function CheckSchedule(ByVal properties As cAccountProperties) As Boolean
            Dim db As New cFWDBConnection
            Dim drow As DataRow
            Dim sql As String
            Dim updateDate As Boolean

            Debug.WriteLine("cEmail:CheckSchedule:entered function")

            db.DBOpen(fws, False)

            sql = "SELECT * FROM [email_schedule]"
            db.RunSQL(sql, db.glDBWorkA, False, "", False)

            Debug.WriteLine("successful sql run for : " & sql.Trim)

            For Each drow In db.glDBWorkA.Tables(0).Rows
                Debug.WriteLine("drow.item(""Schedule Id"") = " & drow.Item("Schedule Id").ToString.Trim)

                If drow.Item("Next Run Date") <= Now() Then
                    updateDate = False

                    Select Case CType(drow.Item("Email Type"), emailType)
                        Case emailType.ContractReview
                            Debug.WriteLine("cEmail:CheckSchedule:calling Contract Review")

                            Dim daysahead As Integer
                            If IsDBNull(drow.Item("Email Param")) = False Then
                                daysahead = drow.Item("Email Param")
                            Else
                                daysahead = 0
                            End If

                            Dim svr As New FWEmailServer
                            If svr.DoReviewEmail(Today(), daysahead) = True Then
                                ELog(EMAIL_LOGFILE, "CheckSchedule():Review Email executed successfully")
                                updateDate = True
                            Else
                                ELog(EMAIL_LOGFILE, "CheckSchedule():Review Email execution failed. Cannot set 'Next Run Date'.")
                                Exit Select
                            End If
                            svr.Dispose()

                        Case emailType.OverdueInvoice
                            Debug.WriteLine("cEmail:CheckSchedule:calling Overdue Invoice")

                            Dim svr As New FWEmailServer

                            svr.Dispose()

                        Case emailType.AuditCleardown
                            Debug.WriteLine("cEmail:CheckSchedule:calling AuditCleardown")
                            Debug.WriteLine("cEmail:CheckSchedule:fws.glApplicationPath = " & Trim(fws.glApplicationPath))

                            Dim tmpStr As String
                            tmpStr = SMRoutines.ClearDownAudit(fws.MetabaseCustomerId, fws.getConnectionString, Nothing, Nothing, Nothing, properties)

                            Debug.WriteLine("cEmail:CheckSchedule:ClearDownAudit returned " & tmpStr)

                            If tmpStr = "OK" Then
                                updateDate = True
                            End If

                        Case Else

                    End Select

                    If updateDate = True Then
                        Debug.WriteLine("cEmail:CheckSchedule:updating 'Next Run Date'")

                        If CType(drow.Item("Email Type"), emailFreq) = emailFreq.Once Then
                            ' execution successful, so delete the schedule
                            db.FWDb("D", "email_schedule", "Schedule Id", drow.Item("Schedule Id"), "", "", "", "", "", "", "", "", "", "")
                        Else
                            'update the next run date and time
                            Dim nextDate, curDate As Date
                            Dim loopCount As Integer

                            curDate = drow.Item("Next Run Date")
                            nextDate = curDate
                            loopCount = 0
                            While nextDate <= curDate
                                loopCount += 1

                                If IsDBNull(drow.Item("Frequency Param")) = False Then
                                    nextDate = SMRoutines.GetNextRunDate(curDate, CType(drow.Item("Email Frequency"), emailFreq), drow.Item("Frequency Param"))
                                Else
                                    nextDate = SMRoutines.GetNextRunDate(curDate, CType(drow.Item("Email Frequency"), emailFreq), 0)
                                End If
                                Debug.WriteLine("cEmail:CheckSchedule:Next Run Date = " & nextDate.ToShortDateString)

                                If loopCount > 10 Then
                                    ' avoid any terminal looping
                                    Exit While
                                End If
                            End While

                            ' should have a new date
                            db.SetFieldValue("Next Run Date", nextDate, "D", True)
                            db.FWDb("A", "email_schedule", "Schedule Id", drow.Item("Schedule Id"), "", "", "", "", "", "", "", "", "", "")
                        End If
                    End If
                End If
            Next

            db.DBClose()
            db = Nothing

            ELog(EMAIL_LOGFILE, "CheckSchedule() completed successfully.")
        End Function

        Public Sub ELog(ByVal logfilename As String, ByVal Msg As String)
            ' This routine time-stamps the message and writes it to the log file
            Dim logfile As System.IO.StreamWriter
            Dim mappedFileName, msgStr As String

            Try
                'Debug.WriteLine("cEmail:ELog:Entered ELog() function")

                'Debug.WriteLine("cEmail:ELog:logfilename = " & logfilename)
                'Debug.WriteLine("cEmail:ELog:Email log path = " & fws.glEmailLog)

                mappedFileName = System.IO.Path.Combine(fws.glEmailLog, logfilename)

                'Debug.WriteLine("cEmail:ELog:mappedFileName = " & mappedFileName)

                logfile = New System.IO.StreamWriter(mappedFileName, True)

                msgStr = Date.Now.ToShortDateString & " " & Date.Now.ToShortTimeString & " : " & Msg
                logfile.WriteLine(msgStr)
                'Debug.WriteLine("cEmail:ELog:" & msgStr)

                logfile.Flush()
                logfile.Close()
                logfile = Nothing

            Catch ex As Exception
                Debug.WriteLine("cEmail:ELog:ELog() of message failed " & Date.Now.ToShortDateString & " : " & Date.Now.ToShortTimeString)
                Debug.WriteLine("cEmail:ELog:Error : " & ex.Message)
            End Try
        End Sub

        Public Overrides Sub Dispose()
            Debug.WriteLine("cEmail:Dispose:called Dispose():stopping timer")
            aTimer.Stop()
            aTimer.Dispose()
        End Sub

        Public Sub StopTimer()
            Debug.WriteLine("cEmail:StopTimer:called StopTimer()")
            aTimer.Stop()
        End Sub

        Public Sub StartTimer()
            Debug.WriteLine("cEmail:StartTimer:called StartTimer()")
            aTimer.Interval = fws.glWebEmailerCheckInterval * (60 * 1000)
            aTimer.Start()
        End Sub

        Public Sub SetInterval(ByVal secs As Double)
            fws.glWebEmailerCheckInterval = secs * (60 * 1000)

            Dim tmpFWS As New cFWSettings
            tmpFWS = fws
            tmpFWS.glWebEmailerCheckInterval = secs * (60 * 1000)
            Application.Lock()
            fws = tmpFWS
            Application.UnLock()
        End Sub

        Public Function KillThread() As String
            KillThread = "Email Thread being closed"
            Dispose()
        End Function
    End Class

End Namespace

