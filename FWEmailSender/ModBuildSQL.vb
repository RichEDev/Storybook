Imports FWBase
Imports FWClasses

Module ModBuildSQL
    Public Enum csvJOIN_Fields
        RowStatus = 0
        JoinID = 1
        JoinSQL = 2
        JoinTable = 3
    End Enum

    Public Function CReviewSQL(ByVal emParam As Integer) As String
        Dim sql As New System.Text.StringBuilder

        sql.Append("SELECT [contractNumber]," & vbNewLine)
        sql.Append("[contractId]," & vbNewLine)
        sql.Append("[contractDescription]," & vbNewLine)
        sql.Append("[noticePeriod]," & vbNewLine)
        sql.Append("[reviewPeriod]," & vbNewLine)
        sql.Append("[reviewDate]," & vbNewLine)
        sql.Append("[reviewComplete]," & vbNewLine)
        sql.Append("[contractKey]," & vbNewLine)
        sql.Append("[maintenanceType]," & vbNewLine)
        sql.Append("[maintenancePct]," & vbNewLine)
        sql.Append("[endDate]," & vbNewLine)
        sql.Append("[supplier_details].[supplierName]" & vbNewLine)
        sql.Append("FROM [contract_details]" & vbNewLine)
        sql.Append("INNER JOIN [supplier_details] ON [supplier_details].[supplierId] = [contract_details].[supplierId]" & vbNewLine)
        sql.Append("WHERE ([reviewComplete] = 0 OR [reviewComplete] IS NULL)" & vbNewLine)
        sql.Append("AND [reviewDate] <= CONVERT(datetime,DATEADD(day," & emParam & ",getdate()))" & vbNewLine)
        sql.Append("AND [Archived] = 'N' AND [contract_details].[subAccountId] = @locId")
        Return sql.ToString
    End Function

    Public Function GetRecpListSQL(ByVal activeLocation As Integer, ByVal ContractID As Integer) As String
        Dim sql As New System.Text.StringBuilder

        sql.Append("SELECT * FROM dbo.GetNotifyRecipients(" & ContractID.ToString & ", " & activeLocation.ToString & ")")
        Return sql.ToString
    End Function

    Public Function ConstructQuery(ByVal Fields() As String, ByVal BaseTable As String, ByVal KeyField As String, ByVal KeyValue As String, Optional ByVal IncludeFields As String = "") As String
        AddToLog("Generating query to fetch fields from database...")
        Dim sHeader As New System.Text.StringBuilder
        Dim sFields As New System.Text.StringBuilder
        Dim sJoins As New System.Text.StringBuilder
        Dim sQuery As New System.Text.StringBuilder
        Dim usedFields As New System.Collections.Specialized.NameValueCollection
        Dim usedJoins As New System.Collections.Specialized.NameValueCollection
        Dim isnull As Boolean = False

        Dim ds As DataSet
        Dim csv As New csvParser.cCSV
        ds = csv.CSVToDataset("database.csv")

        sFields.Append("[" & BaseTable & "].[" & KeyField & "]")

        For i As Integer = 0 To Fields.Length - 1
            For Each dRow As DataRow In ds.Tables(0).Rows
                isnull = False
                If Fields(i) = "[*" & dRow.Item(csvDB_Fields.FriendlyName) & "*]" Then
                    If usedFields.Item(dRow.Item(csvDB_Fields.FriendlyName)) <> "y" Then
                        If sFields.Length > 0 Then
                            sFields.Append(", ")
                        End If

                        sFields.Append(dRow.Item(csvDB_Fields.TableName) & "." & dRow.Item(csvDB_Fields.FieldName))

                        If dRow.Item(csvDB_Fields.TableName) = "dbo" Then
                            sFields.Append(" AS " & dRow.Item(csvDB_Fields.FieldAlias))
                        End If

                        usedFields.Add(dRow.Item(csvDB_Fields.FriendlyName), "y")
                    End If

                    If IsDBNull(dRow.Item(csvDB_Fields.JoinsReq)) = True Then
                        isnull = True
                    End If
                    If isnull = False Then
                        If dRow.Item(csvDB_Fields.JoinsReq) = "" Then
                            isnull = True
                        End If
                    End If

                    If isnull = False Then
                        Dim joinField As String = dRow.Item(csvDB_Fields.JoinsReq)
                        Dim joinFields() As String
                        If joinField.IndexOf(",") > -1 Then
                            joinFields = joinField.Split(",")
                        Else
                            ReDim joinFields(0)
                            joinFields(0) = joinField
                        End If

                        For x As Integer = 0 To UBound(joinFields)
                            If usedJoins.Item(joinFields(x)) <> "y" Then
                                usedJoins.Add(joinFields(x), "y")
                            End If
                        Next
                    End If

                End If
            Next
        Next

        sQuery.Append("SELECT " & IncludeFields & sFields.ToString() & " FROM [" & BaseTable & "] " & getJoins(usedJoins, BaseTable) & " WHERE [" & BaseTable & "].[" & KeyField & "] = " & KeyValue & "")
        AddToLog("Query built.")
        Return sQuery.ToString
    End Function

    Private Function getJoins(ByVal selectedJoins As System.Collections.Specialized.NameValueCollection, ByVal BaseTable As String) As String
        AddToLog("Generating joins to relate tables...")
        Dim csvIN As New csvParser.cCSV
        Dim builtJoins As New System.Collections.Specialized.NameValueCollection
        Dim joins As New System.Text.StringBuilder

        Dim ds As New DataSet
        Dim sjEnum As System.Collections.IEnumerator
        ds = csvIN.CSVToDataset("joins.csv")
        sjEnum = selectedJoins.GetEnumerator
        'selectedJoins.Set("", "")

        For Each dRow As DataRow In ds.Tables(0).Rows
            sjEnum.Reset()
            Do Until sjEnum.MoveNext() = False
                Dim sI As String = sjEnum.Current
                If builtJoins.Item(sI) <> "y" Then
                    If dRow.Item(csvJOIN_Fields.JoinID) = sI Then
                        If dRow.Item(csvJOIN_Fields.JoinTable) <> BaseTable Then
                            joins.Append(dRow.Item(csvJOIN_Fields.JoinSQL) & " ")
                            builtJoins.Add(sI, "y")
                        End If
                    End If
                End If
            Loop
        Next

        AddToLog("Joins done.")

        Return joins.ToString()
    End Function
End Module
