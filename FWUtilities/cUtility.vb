Public Class cUtility
    Public Structure ImportMap
        Public CoreTable As String
        Public Field As String
        Public AltDescription As String
        Public Type As String
        Public MaxSize As Integer
        Public isUnique As Boolean
        Public isMandatory As Boolean
        Public RefField As String
        Public LinkRef As String
        Public LinkMatch As String
        Public DefaultValueIfBlank As Object
        Public IsUserdefined As Boolean
        Public UDF_IDFieldName As String
        Public UDF_TableName As String
    End Structure

    <Serializable()> _
    Public Enum ImportType
        ContractDetails = 1
        VendorDetails = 2
        VendorContacts = 3
        ProductDetails = 4
        StaffDetails = 5
        ContractProducts = 6
        CodesUnits = 7
        CodesInflator = 8
        CodesSalesTax = 9
        CodesInvFrequency = 10
        CodesContractStatus = 11
        CodesRechargeEntity = 12
        CodesSites = 13
        CodesVendorCategory = 14
        ContractProductRecharge = 15
        ProductVendorAssoc = 16
        ContractSavings = 17
        CodesAccountCodes = 18
        RechargeAssociations = 19
        RechargeAssociationsUnq = 20
        CodesProductCategory = 21
        RechargeAssociationsBespoke = 22
        InvoiceDetails = 23
        UFListItems = 24
        CodesContractCategory = 25
    End Enum

    <Serializable()> _
    Public Enum srcField
        ColumnId = 0
        FieldName = 1
    End Enum

    <Serializable()> _
        Public Enum dstField
        xmapField = 0
        FieldName = 1
        Idx = 2
        srcIdx = 3
        defaultvalue = 4
    End Enum

    <Serializable()> _
        Public Structure UserMappings
        Public Source_ColumnNo As Integer
        Public ImpMap_idx As Integer
        Public DefaultValue As String
    End Structure

    Public Enum emailType
        ContractReview = 1
        OverdueInvoice = 2
        AuditCleardown = 3
        LicenceExpiry = 4
    End Enum

    Public Enum emailFreq
        Once = 0
        Daily = 1
        Weekly = 2
        MonthlyOnFirstXDay = 3
        MonthlyOnDay = 4
        Every_n_Days = 5
    End Enum

    Public Shared Function Crypt(ByVal pw As Object, ByVal p As Object)
        Dim x As Integer

        ' Encrypt or decrypt passwords
        ' Errors returned prefixed with '$'

        Try
            Dim Q%, CH$, op, EC, n
            n = Len(pw)
            'If n < 6 Or n > 10 Then
            '    Crypt = "$" & "Password must be 6 to 10 characters"
            '    Exit Function
            'End If

            For x = 1 To n
                If Asc(Mid(pw, n, 1)) > 122 Or Asc(Mid(pw, n, 1)) < 48 Then
                    Crypt = "$" & "Password contains an invalid character"
                    Exit Function
                End If
            Next x
            Select Case p
                Case 1  ' Encrypt
                    op = ""
                    For Q% = 1 To Len(pw)
                        CH$ = Mid(pw, Q%, 1)
                        EC = Asc(CH$) + n
                        If EC > 122 Then EC = EC - 75
                        op = op & Chr(EC)
                        n = n + 1
                    Next Q%
                    Crypt = op
                Case 2  ' Decrypt
                    op = ""
                    For Q% = 1 To Len(pw)
                        CH$ = Mid(pw, Q%, 1)
                        EC = Asc(CH$) - n
                        If EC < 48 Then EC = EC + 75
                        op = op & Chr(EC)
                        n = n + 1
                    Next Q%
                    Crypt = op
                Case Else
                    Crypt = "$Unknown param issued to Crypt() function"
            End Select

        Catch ex As Exception
            Crypt = "$Err: Crypt() failed"
        End Try
    End Function
End Class
