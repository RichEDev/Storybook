Imports FWBase
Imports SpendManagementLibrary
Imports Spend_Management

Module ImportRoutines
    Public Enum ColumnMappingCodes
        FixedSelection = -1
        UnmappedDefault = -2
        DualReference = -3
    End Enum

    Public Function GetUserFields(ByVal db As DBConnection, ByVal coreTable As String) As DataSet
        Dim dset As New DataSet
        Dim sql As String

        Dim udfTableId As Guid = Guid.Empty

        sql = "select userdefined_table from tables where tablename = @tableName"
        db.sqlexecute.Parameters.AddWithValue("@tableName", coreTable)
        Using reader As SqlClient.SqlDataReader = db.GetReader(sql)
            While reader.Read
                If reader.IsDBNull(0) = False Then
                    udfTableId = reader.GetGuid(0)
                End If
            End While
            reader.Close()
        End Using

        db.sqlexecute.Parameters.Clear()

        sql = "SELECT userdefined.*,userdefinedGroupings.[order], tables.tablename as relatedTableName, t2.tablename as udfTableName FROM userdefined "
        sql += "LEFT JOIN userdefinedGroupings ON userdefinedGroupings.[userdefinedGroupId] = userdefined.[groupId] "
        sql += "LEFT JOIN tables ON tables.tableid = userdefined.relatedTable "
        sql += "INNER JOIN tables AS t2 ON userdefined.tableid = t2.tableid "
        sql += "WHERE userdefined.tableid = @tableId ORDER BY userdefinedGroupings.[order],userdefined.[order]"
        db.sqlexecute.Parameters.AddWithValue("@tableId", udfTableId)
        dset = db.GetDataSet(sql)

        Return dset
    End Function
End Module
