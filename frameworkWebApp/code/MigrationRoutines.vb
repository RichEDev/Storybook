Imports SpendManagementLibrary
Imports System

Namespace Framework2006
    Public Module Migration_Routines
        Public Function Migrate_UserFields(ByVal db As cFWDBConnection) As Boolean
            Try
                db.ExecuteSQL("EXECUTE dbo.RefreshUFReportFields")

                Return Not db.glError

            Catch ex As Exception
                Return False
            End Try

        End Function

        Public Function Migrate_Reports(ByVal db As cFWDBConnection) As Boolean
            Try
                db.ExecuteSQL("EXECUTE dbo.migrate_reports")

                Return Not db.glError
            Catch ex As Exception
                Return False
            End Try
        End Function
    End Module
End Namespace