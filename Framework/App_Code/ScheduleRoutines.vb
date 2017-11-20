Imports Microsoft.VisualBasic
Imports FWBase
Imports Spend_Management
Imports SpendManagementLibrary
Imports System
Imports System.IO
Imports System.Data
Imports System.Web

Namespace Framework2006
    Public Module ScheduleRoutines
		Public Function AddVariation(ByVal activeServer As HttpServerUtility, ByVal curUser As CurrentUser, ByVal cId As Integer, ByVal accProperties As cAccountProperties) As Integer
			Try
				Dim newContractId As Integer
				Dim sql, fieldName, fieldType As String
				Dim dcol As DataColumn
				Dim tmpKey As String
				Dim firstpass As Boolean
				Dim dset, dset2 As DataSet
				Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection(), curUser.CurrentSubAccountId)
				Dim db As New cFWDBConnection()
				db.DBOpen(fws, False)

				dset = New DataSet
				dset2 = New DataSet

                sql = "SELECT *, dbo.GetNextVariationSequence(@conId) AS [NextSequenceNum] FROM [contract_details] WHERE [contractId] = @conId"
				db.AddDBParam("conId", cId, True)
				db.RunSQL(sql, dset, False, "", False)

				firstpass = True
				tmpKey = ""

				For Each dcol In dset.Tables(0).Columns
					fieldName = dcol.ColumnName

					Select Case dcol.DataType.ToString
						Case "System.String"
							fieldType = "S"
						Case "System.Int32", "System.Int16", "System.Double"
							fieldType = "N"
						Case "System.DateTime"
							fieldType = "D"
						Case Else
							fieldType = "S"
					End Select

					Select Case fieldName
                        Case "contractId"
                            ' skip contract id because it's the identity field

                        Case "contractKey"
                            If IsDBNull(dset.Tables(0).Rows(0).Item(fieldName)) = True Then
                                ' shouldn't be null, so update actual contract
                                tmpKey = accProperties.ContractKey & "/" & cId.ToString.Trim & "/" & Trim(Str(dset.Tables(0).Rows(0).Item("supplierId")))
                                db.AddDBParam("key", tmpKey.Trim, True)
                                db.AddDBParam("cId", cId, False)
                                db.RunSQL("UPDATE [contract_details] SET [ContractKey] = @key WHERE [ContractId] = @cId", dset2, False, "", False)
                            Else
                                tmpKey = Trim(dset.Tables(0).Rows(0).Item(fieldName))
                            End If

                        Case "scheduleNumber"
                            Dim dupSN As Boolean = True

                            If accProperties.EnableVariationAutoSeq Then
                                db.SetFieldValue(fieldName, dset.Tables(0).Rows(0).Item("NextSequenceNum"), "S", firstpass)
                                dupSN = False
                                firstpass = False
                            End If

                            If dupSN Then
                                If IsDBNull(dset.Tables(0).Rows(0).Item(fieldName)) = False Then
                                    If fieldType = "S" Then
                                        db.SetFieldValue(fieldName, Trim(dset.Tables(0).Rows(0).Item(fieldName)), fieldType, firstpass)
                                    Else
                                        db.SetFieldValue(fieldName, dset.Tables(0).Rows(0).Item(fieldName), fieldType, firstpass)
                                    End If

                                    firstpass = False
                                End If
                            End If


                        Case "contractValue"
                            db.SetFieldValue(fieldName, 0, "N", firstpass)

						Case "NextSequenceNum"

                        Case "createdOn"
                            db.SetFieldValue(fieldName, DateTime.Now, "D", firstpass)
                        Case "lastChangeDate"
                            db.SetFieldValue(fieldName, DateTime.Now, "D", firstpass)
                        Case "lastChangedBy"
                            db.SetFieldValue(fieldName, curUser.Employee.firstname & " " & curUser.Employee.surname, "S", firstpass)
						Case Else
							If IsDBNull(dset.Tables(0).Rows(0).Item(fieldName)) = False Then
								If fieldType = "S" Then
									db.SetFieldValue(fieldName, Trim(dset.Tables(0).Rows(0).Item(fieldName)), fieldType, firstpass)
								Else
									db.SetFieldValue(fieldName, dset.Tables(0).Rows(0).Item(fieldName), fieldType, firstpass)
								End If

								firstpass = False
							End If
					End Select
				Next

				db.FWDb("W", "contract_details", "", "", "", "", "", "", "", "", "", "", "", "")
				newContractId = db.glIdentity
                db.FWDb("R", "contract_details", "contractId", newContractId, "", "", "", "", "", "", "", "", "", "")
				If db.FWDbFlag = True Then
                    tmpKey = accProperties.ContractKey & "/" & newContractId.ToString.Trim & "/" & db.FWDbFindVal("supplierId", 1).Trim
                    db.SetFieldValue("contractKey", tmpKey, "S", True)
                    db.FWDb("A", "contract_details", "contractId", newContractId, "", "", "", "", "", "", "", "", "", "")

					' create duplication of it's contract products
					DuplicateNotify(curUser, cId, newContractId)
					DuplicateAudience(curUser, "contract_audience", cId, newContractId)
				End If

				' create link entry in link_variations
                db.SetFieldValue("primaryContractId", cId, "N", True)
                db.SetFieldValue("variationContractId", newContractId, "N", False)
				db.FWDb("W", "link_variations", "", "", "", "", "", "", "", "", "", "", "", "")

				dset.Dispose()
				dset2.Dispose()
				dset = Nothing
				dset2 = Nothing

				db.DBClose()

				Return newContractId

			Catch ex As Exception
				Return -1
			End Try
		End Function

		Public Function AddSchedule(ByRef activeServer As HttpServerUtility, ByVal cId As Integer, ByVal curUser As CurrentUser) As Integer
			' create a duplicate of the contract and add a suffix to the schedule number
			Dim newContractId As Integer
			Dim sql, fieldName, fieldType As String
			Dim dcol As DataColumn
			Dim tmpStr, tmpKey As String
			Dim firstpass As Boolean
			Dim dset, dset2 As DataSet
			Dim subaccs As New cAccountSubAccounts(curuser.Account.accountid)
            Dim params As cAccountProperties
            If curUser.CurrentSubAccountId >= 0 Then
                params = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Else
                params = subaccs.getFirstSubAccount().SubAccountProperties
            End If

            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim db As New cFWDBConnection

            'Try
            'db.ExecuteSQL("BEGIN TRANSACTION 'schedule_" & Trim(userinfo.UserId) & "'")
            db.DBOpen(fws, False)

            dset = New DataSet
            dset2 = New DataSet

            sql = "SELECT * FROM [contract_details] WHERE [contractId] = @conId"
            db.AddDBParam("conId", cId, True)
            db.RunSQL(sql, dset, False, "", False)

            firstpass = True
            tmpKey = ""

            For Each dcol In dset.Tables(0).Columns
                fieldName = dcol.ColumnName

                Select Case dcol.DataType.ToString
                    Case "System.String"
                        fieldType = "S"
                    Case "System.Int32", "System.Int16", "System.Double"
                        fieldType = "N"
                    Case "System.DateTime"
                        fieldType = "D"
                    Case Else
                        fieldType = "S"
                End Select

                Select Case fieldName.ToLower
                    Case "contractid"
                        ' skip contract id because it's the identity field

                    Case "contractkey"
                        If IsDBNull(dset.Tables(0).Rows(0).Item(fieldName)) = True Then
                            ' shouldn't be null, so update actual contract
                            tmpKey = params.ContractKey.Trim & "/" & Trim(Str(cId)) & "/" & Trim(Str(dset.Tables(0).Rows(0).Item("supplierId")))
                            db.AddDBParam("key", tmpKey.Trim, True)
                            db.AddDBParam("cId", cId, False)
                            db.RunSQL("UPDATE [contract_details] SET [contractKey] = @key WHERE [contractId] = @cId", dset2, False, "", False)
                        Else
                            tmpKey = Trim(dset.Tables(0).Rows(0).Item(fieldName))
                        End If

                    Case "schedulenumber"
                        ' Add suffix to the schedule to indicate the new one
                        If IsDBNull(dset.Tables(0).Rows(0).Item(fieldName)) = False Then
                            tmpStr = Trim(dset.Tables(0).Rows(0).Item(fieldName))
                            db.SetFieldValue(fieldName, tmpStr & "/new", "S", firstpass)
                            firstpass = False
                        End If

                    Case Else
                        If IsDBNull(dset.Tables(0).Rows(0).Item(fieldName)) = False Then
                            If fieldType = "S" Then
                                db.SetFieldValue(fieldName, Trim(dset.Tables(0).Rows(0).Item(fieldName)), fieldType, firstpass)
                            Else
                                db.SetFieldValue(fieldName, dset.Tables(0).Rows(0).Item(fieldName), fieldType, firstpass)
                            End If

                            firstpass = False
                        End If
                End Select
            Next

            Dim ALog As New cAuditLog(curUser.Account.accountid, curUser.Employee.employeeid)

            db.FWDb("W", "contract_details", "", "", "", "", "", "", "", "", "", "", "", "")
            newContractId = db.glIdentity
            db.FWDb("R", "contract_details", "contractId", newContractId, "", "", "", "", "", "", "", "", "", "")
            If db.FWDbFlag = True Then
                tmpKey = params.ContractKey.Trim & "/" & Trim(Str(newContractId)) & "/" & Trim(db.FWDbFindVal("supplierId", 1))
                db.SetFieldValue("contractKey", tmpKey, "S", True)
                db.FWDb("A", "contract_details", "contractId", newContractId, "", "", "", "", "", "", "", "", "", "")

                ' create duplication of it's contract products
                DuplicateProducts(curUser, cId, newContractId, Val(db.FWDbFindVal("supplierId", 1)))
                DuplicateNotify(curUser, cId, newContractId)
                DuplicateAudience(curUser, "contract_audience", cId, newContractId)
                DuplicateAttachments(activeServer, curUser, AttachmentArea.CONTRACT, cId, newContractId)
            End If

            ALog.addRecord(SpendManagementElement.ContractDetails, "Contract Schedule", 0)

            ' create a schedule link in the [Link Matrix]
            sql = "SELECT [link_definitions].*,[isArchived] FROM [link_matrix] " & vbNewLine
            sql += "INNER JOIN [link_definitions] ON [link_definitions].[linkId] = [link_matrix].[linkId] " & vbNewLine
            sql += "WHERE [isScheduleLink] = 1 AND [contractId] = " & cId.ToString
            dset.Clear()
            db.RunSQL(sql, dset, False, "", False)

            If db.glNumRowsReturned > 0 Then
                ' must be a new schedule in a chain
                db.SetFieldValue("linkId", db.GetFieldValue(dset, "linkId", 0, 0), "N", True)
                db.SetFieldValue("contractId", newContractId, "N", False)
                db.SetFieldValue("isArchived", db.GetFieldValue(dset, "isArchived", 0, 0), "N", False)
                db.FWDb("WX", "link_matrix", "", "", "", "", "", "", "", "", "", "", "", "")
            Else
                ' must be first new schedule created
                db.FWDb("R2", "contract_details", "contractId", cId, "", "", "", "", "", "", "", "", "", "")
                If db.FWDb2Flag = True Then
                    Dim linkId As Integer

                    If db.FWDbFindVal("contractKey", 2) <> "" Then
                        db.SetFieldValue("linkDefinition", db.FWDbFindVal("contractKey", 2), "S", True)
                    Else
                        db.SetFieldValue("linkDefinition", db.FWDbFindVal("contractDescription", 2), "S", True)
                    End If
                    db.SetFieldValue("isScheduleLink", 1, "N", False)
                    db.FWDb("W", "link_definitions", "", "", "", "", "", "", "", "", "", "", "", "")

                    linkId = db.glIdentity

                    ' original contract
                    db.SetFieldValue("linkId", linkId, "N", True)
                    db.SetFieldValue("contractId", cId, "N", False)
                    db.SetFieldValue("isArchived", IIf(db.FWDbFindVal("isArchived", 2) = "Y", 1, 0), "N", False)
                    db.FWDb("WX", "link_matrix", "", "", "", "", "", "", "", "", "", "", "", "")

                    ' new contract in the schedule chain
                    db.SetFieldValue("linkId", linkId, "N", True)
                    db.SetFieldValue("contractId", newContractId, "N", False)
                    db.SetFieldValue("isArchived", IIf(db.FWDbFindVal("isArchived", 2) = "Y", 1, 0), "N", False)
                    db.FWDb("WX", "link_matrix", "", "", "", "", "", "", "", "", "", "", "", "")
                End If
            End If

            Return newContractId

            dset.Dispose()
            dset2.Dispose()
            dset = Nothing
            dset2 = Nothing
            'db.ExecuteSQL("COMMIT TRANSACTION 'schedule_" & Trim(userinfo.UserId) & "'")
            db.DBClose()

            'Catch ex As Exception
            '             Return -1
            '             'db.ExecuteSQL("ROLLBACK TRANSACTION 'schedule_" & Trim(userinfo.UserId) & "'")
            '         End Try

		End Function

		Public Sub DuplicateProducts(ByVal curUser As CurrentUser, ByVal origContractId As Integer, ByVal newContractId As Integer, ByVal VendorId As Integer)
			Dim sql As String
			Dim drow As DataRow
			Dim dcol As DataColumn
			Dim fieldName, fieldType As String
			Dim firstpass As Boolean
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection(), curUser.CurrentSubAccountId)
			Dim db As New cFWDBConnection()
			db.DBOpen(fws, False)

            sql = "SELECT * FROM [contract_productdetails] WHERE [contractId] = @conId"
			db.AddDBParam("conId", origContractId, True)
			db.glDBWorkC.Clear()
			db.RunSQL(sql, db.glDBWorkC, False, "", False)

			For Each drow In db.glDBWorkC.Tables(0).Rows
				firstpass = True

				For Each dcol In db.glDBWorkC.Tables(0).Columns
					fieldName = dcol.ColumnName
					Select Case dcol.DataType.ToString
						Case "System.String"
							fieldType = "S"
						Case "System.Int32", "System.Int16", "System.Double"
							fieldType = "N"
						Case "System.DateTime"
							fieldType = "D"
						Case Else
							fieldType = "S"
					End Select

                    Select Case fieldName.ToLower
                        Case "contractproductid"
                            ' skip contract id because it's the identity field

                        Case "contractid"
                            db.SetFieldValue("ContractId", newContractId, "N", firstpass)
                            firstpass = False

                        Case "supplierid"
                            db.SetFieldValue("supplierId", VendorId, "N", firstpass)
                            firstpass = False

                        Case Else
                            If IsDBNull(drow.Item(fieldName)) = False Then
                                If fieldType = "S" Then
                                    db.SetFieldValue(fieldName, Trim(drow.Item(fieldName)), fieldType, firstpass)
                                Else
                                    db.SetFieldValue(fieldName, drow.Item(fieldName), fieldType, firstpass)
                                End If

                                firstpass = False
                            End If
                    End Select
				Next

				If firstpass = False Then
					' must have some fields to add
					db.FWDb("W", "contract_productdetails", "", "", "", "", "", "", "", "", "", "", "", "")
					If db.glIdentity > 0 Then
						Dim tmpId As Integer

						tmpId = db.glIdentity

						' check for any call-off data to be replicated.
                        DuplicateCallOff(curUser, drow.Item("ContractProductId"), tmpId)

						' check for any product-platform information
                        DuplicateProductPlatforms(curUser, drow.Item("ContractProductId"), tmpId)

						' by default, vendor & product notes get carried across - not xferring contract notes presently
					End If
				End If

			Next

			db.DBClose()
		End Sub

		Private Sub DuplicateCallOff(ByVal curUser As CurrentUser, ByVal origConProdId As Integer, ByVal newConProdId As Integer)
			Dim sql As String
			Dim fieldName, fieldType As String
			Dim drow As DataRow
			Dim dcol As DataColumn
			Dim firstpass As Boolean
			Dim dsetA As DataSet
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection(), curUser.CurrentSubAccountId)
			Dim db As New cFWDBConnection()
			db.DBOpen(fws, False)

			dsetA = New DataSet

            sql = "SELECT * FROM [contract_productdetails_calloff] WHERE [ContractProductId] = @cpId"
			db.AddDBParam("cpId", origConProdId, True)
			db.RunSQL(sql, dsetA, False, "", False)

			For Each drow In dsetA.Tables(0).Rows
				firstpass = True

				For Each dcol In dsetA.Tables(0).Columns
					fieldName = dcol.ColumnName
					Select Case dcol.DataType.ToString
						Case "System.String"
							fieldType = "S"
						Case "System.Int32", "System.Int16", "System.Double"
							fieldType = "N"
						Case "System.DateTime"
							fieldType = "D"
						Case Else
							fieldType = "S"
					End Select

                    Select Case fieldName.ToLower
                        Case "calloffid"
                            ' skip contract id because it's the identity field

                        Case "contractproductid"
                            db.SetFieldValue("ContractProductId", newConProdId, "N", firstpass)
                            firstpass = False

                        Case Else
                            If IsDBNull(drow.Item(fieldName)) = False Then
                                If fieldType = "S" Then
                                    db.SetFieldValue(fieldName, Trim(drow.Item(fieldName)), fieldType, firstpass)
                                Else
                                    db.SetFieldValue(fieldName, drow.Item(fieldName), fieldType, firstpass)
                                End If

                                firstpass = False
                            End If
                    End Select
				Next

				If firstpass = False Then
					' must have a call off to save
					db.FWDb("W", "contract_productdetails_calloff", "", "", "", "", "", "", "", "", "", "", "", "")
				End If
			Next

			db.DBClose()
			dsetA.Dispose()
			dsetA = Nothing
		End Sub

		Private Sub DuplicateProductPlatforms(ByVal curuser As CurrentUser, ByVal origConProdId As Integer, ByVal newConProdId As Integer)
			Dim sql As String
			Dim fieldName, fieldType As String
			Dim drow As DataRow
			Dim dcol As DataColumn
			Dim firstpass As Boolean
			Dim dsetA As DataSet
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curuser.Account, New cAccountSubAccounts(curuser.Account.accountid).getSubAccountsCollection(), curuser.CurrentSubAccountId)
			Dim db As New cFWDBConnection()
			db.DBOpen(fws, False)

			dsetA = New DataSet

            sql = "SELECT * FROM [contract_productplatforms] WHERE [ContractProductId] = @cpId"
			db.AddDBParam("cpId", origConProdId, True)
			db.RunSQL(sql, dsetA, False, "", False)

			For Each drow In dsetA.Tables(0).Rows
				firstpass = True

				For Each dcol In dsetA.Tables(0).Columns
					fieldName = dcol.ColumnName
					Select Case dcol.DataType.ToString
						Case "System.String"
							fieldType = "S"
						Case "System.Int32", "System.Int16", "System.Double"
							fieldType = "N"
						Case "System.DateTime"
							fieldType = "D"
						Case Else
							fieldType = "S"
					End Select

                    Select Case fieldName.ToLower
                        Case "productplatformid"
                            ' skip contract id because it's the identity field

                        Case "contractproductid"
                            db.SetFieldValue("ContractProductId", newConProdId, "N", firstpass)
                            firstpass = False

                        Case Else
                            If IsDBNull(drow.Item(fieldName)) = False Then
                                If fieldType = "S" Then
                                    db.SetFieldValue(fieldName, Trim(drow.Item(fieldName)), fieldType, firstpass)
                                Else
                                    db.SetFieldValue(fieldName, drow.Item(fieldName), fieldType, firstpass)
                                End If

                                firstpass = False
                            End If
                    End Select
				Next

				If firstpass = False Then
					' must have a call off to save
					db.FWDb("W", "contract_productplatforms", "", "", "", "", "", "", "", "", "", "", "", "")
				End If
			Next

			db.DBClose()
			dsetA.Dispose()
			dsetA = Nothing
		End Sub

		Public Function UpdateSchedule(ByRef activeServer As HttpServerUtility, ByVal cId As Integer, ByVal curUser As CurrentUser) As Integer
			Try
				' perform duplication of contract, products, call-off, platforms as per AddSchedule
				Dim newContractId As Integer

				newContractId = AddSchedule(activeServer, cId, curUser)

				If newContractId > 0 Then
					' now must also duplicate contract notes,attachments,invoices+notes+breakdown,forecast+breakdown,
					DuplicateNotes(activeServer, curUser, AttachmentArea.CONTRACT_NOTES, cId, newContractId)

					DuplicateInvoices(activeServer, curUser, cId, newContractId)
					DuplicateForecasts(curUser, cId, newContractId)
					'DuplicateAttachments(activeServer, db, AttachmentArea.CONTRACT, cId, newContractId)
				End If

				UpdateSchedule = newContractId

			Catch ex As Exception
				UpdateSchedule = -1
			End Try

		End Function

		Public Sub DuplicateNotes(ByRef activeServer As HttpServerUtility, ByVal curUser As CurrentUser, ByVal noteType As AttachmentArea, ByVal origId As Integer, ByVal newId As Integer)
			Dim noteTable, noteIdField As String

			Select Case noteType
				Case AttachmentArea.CONTRACT_NOTES
                    noteTable = "contractNotes"
                    noteIdField = "ContractId"

				Case AttachmentArea.INVOICE_NOTES
                    noteTable = "invoiceNotes"
                    noteIdField = "InvoiceId"

				Case AttachmentArea.PRODUCT_NOTES
                    noteTable = "productNotes"
                    noteIdField = "ProductId"

				Case AttachmentArea.VENDOR_NOTES
                    noteTable = "supplierNotes"
                    noteIdField = "supplierId"

				Case AttachmentArea.CONTACT_NOTES
                    noteTable = "supplierContactNotes"
                    noteIdField = "contactId"

				Case Else
					Exit Sub
			End Select

			Dim dcol As DataColumn
			Dim drow As DataRow
			Dim fieldName, fieldType, sql As String
			Dim firstpass As Boolean
			Dim dsetA As DataSet
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection(), curUser.CurrentSubAccountId)
			Dim db As New cFWDBConnection()
			db.DBOpen(fws, False)

			dsetA = New DataSet

			sql = "SELECT * FROM [" & noteTable & "] WHERE [" & noteIdField & "] = @origId"
			db.AddDBParam("origId", origId, True)
			db.RunSQL(sql, dsetA, False, "", False)

			For Each drow In dsetA.Tables(0).Rows
				firstpass = True

				For Each dcol In dsetA.Tables(0).Columns
					fieldName = dcol.ColumnName
					Select Case dcol.DataType.ToString
						Case "System.String"
							fieldType = "S"
						Case "System.Int32", "System.Int16", "System.Double"
							fieldType = "N"
						Case "System.DateTime"
							fieldType = "D"
						Case Else
							fieldType = "S"
					End Select

                    Select Case fieldName.ToLower
                        Case "noteid"
                            ' skip contract id because it's the identity field

                        Case noteIdField
                            db.SetFieldValue(noteIdField, newId, "N", firstpass)
                            firstpass = False

                        Case Else
                            If IsDBNull(drow.Item(fieldName)) = False Then
                                If fieldType = "S" Then
                                    db.SetFieldValue(fieldName, Trim(drow.Item(fieldName)), fieldType, firstpass)
                                Else
                                    db.SetFieldValue(fieldName, drow.Item(fieldName), fieldType, firstpass)
                                End If

                                firstpass = False
                            End If
                    End Select
				Next

				If firstpass = False Then
					' must have a note to save
					db.FWDb("W", noteTable, "", "", "", "", "", "", "", "", "", "", "", "")
                    DuplicateAttachments(activeServer, curUser, noteType, drow.Item("noteId"), db.glIdentity)
				End If
			Next

			db.DBClose()
			dsetA.Dispose()
			dsetA = Nothing
		End Sub

		Public Sub DuplicateInvoices(ByRef activeServer As HttpServerUtility, ByVal curUser As CurrentUser, ByVal origId As Integer, ByVal newId As Integer)
			Dim sql As String
			Dim fieldName, fieldType As String
			Dim drow As DataRow
			Dim dcol As DataColumn
			Dim firstpass As Boolean
			Dim dsetA As DataSet
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection(), curUser.CurrentSubAccountId)
			Dim db As New cFWDBConnection()
			db.DBOpen(fws, False)

			dsetA = New DataSet

            sql = "SELECT * FROM [invoices] WHERE [ContractId] = @origId"
			db.AddDBParam("origId", origId, True)
			db.RunSQL(sql, dsetA, False, "", False)

			For Each drow In dsetA.Tables(0).Rows
				firstpass = True

				For Each dcol In dsetA.Tables(0).Columns
					fieldName = dcol.ColumnName
					Select Case dcol.DataType.ToString
						Case "System.String"
							fieldType = "S"
						Case "System.Int32", "System.Int16", "System.Double"
							fieldType = "N"
						Case "System.DateTime"
							fieldType = "D"
						Case Else
							fieldType = "S"
					End Select

                    Select Case fieldName.ToLower
                        Case "invoiceid"
                            ' skip contract id because it's the identity field

                        Case "contractid"
                            db.SetFieldValue("ContractId", newId, "N", firstpass)
                            firstpass = False

                        Case Else
                            If IsDBNull(drow.Item(fieldName)) = False Then
                                If fieldType = "S" Then
                                    db.SetFieldValue(fieldName, Trim(drow.Item(fieldName)), fieldType, firstpass)
                                Else
                                    db.SetFieldValue(fieldName, drow.Item(fieldName), fieldType, firstpass)
                                End If

                                firstpass = False
                            End If
                    End Select
				Next

				If firstpass = False Then
					' must have an invoice to save
					Dim newInvId As Integer
                    db.FWDb("W", "invoices", "", "", "", "", "", "", "", "", "", "", "", "")
					newInvId = db.glIdentity

					If newInvId > 0 Then
                        DuplicateInvoiceProducts(curUser, drow.Item("InvoiceId"), newInvId, newId)
                        DuplicateInvoiceLog(curUser, drow.Item("InvoiceId"), newInvId)
                        DuplicateNotes(activeServer, curUser, AttachmentArea.INVOICE_NOTES, drow.Item("InvoiceId"), newInvId)
					End If
				End If
			Next

			db.DBClose()
			dsetA.Dispose()
			dsetA = Nothing
		End Sub

		Public Sub DuplicateForecasts(ByVal curUser As CurrentUser, ByVal origId As Integer, ByVal newId As Integer)
			Dim sql As String
			Dim fieldName, fieldType As String
			Dim drow As DataRow
			Dim dcol As DataColumn
			Dim firstpass As Boolean
			Dim dsetA As DataSet
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection(), curUser.CurrentSubAccountId)
			Dim db As New cFWDBConnection()
			db.DBOpen(fws, False)

			dsetA = New DataSet

            sql = "SELECT * FROM [contract_forecastdetails] WHERE [ContractId] = @origId"
			db.AddDBParam("origId", origId, True)
			db.RunSQL(sql, dsetA, False, "", False)

			For Each drow In dsetA.Tables(0).Rows
				firstpass = True

				For Each dcol In dsetA.Tables(0).Columns
					fieldName = dcol.ColumnName
					Select Case dcol.DataType.ToString
						Case "System.String"
							fieldType = "S"
						Case "System.Int32", "System.Int16", "System.Double"
							fieldType = "N"
						Case "System.DateTime"
							fieldType = "D"
						Case Else
							fieldType = "S"
					End Select

                    Select Case fieldName.ToLower
                        Case "contractforecastid"
                            ' skip contract id because it's the identity field

                        Case "contractid"
                            db.SetFieldValue("ContractId", newId, "N", firstpass)
                            firstpass = False

                        Case Else
                            If IsDBNull(drow.Item(fieldName)) = False Then
                                If fieldType = "S" Then
                                    db.SetFieldValue(fieldName, Trim(drow.Item(fieldName)), fieldType, firstpass)
                                Else
                                    db.SetFieldValue(fieldName, drow.Item(fieldName), fieldType, firstpass)
                                End If

                                firstpass = False
                            End If
                    End Select
				Next

				If firstpass = False Then
					' must have a call off to save
					db.FWDb("W", "contract_forecastdetails", "", "", "", "", "", "", "", "", "", "", "", "")
                    DuplicateForecastProducts(curUser, drow.Item("ContractForecastId"), db.glIdentity)
				End If
			Next

			db.DBClose()
			dsetA.Dispose()
			dsetA = Nothing
		End Sub

		Private Sub DuplicateForecastProducts(ByVal curUser As CurrentUser, ByVal origId As Integer, ByVal newId As Integer)
			Dim sql As String
			Dim fieldName, fieldType As String
			Dim drow As DataRow
			Dim dcol As DataColumn
			Dim firstpass As Boolean
			Dim dsetA As DataSet
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection(), curUser.CurrentSubAccountId)
			Dim db As New cFWDBConnection()
			db.DBOpen(fws, False)

			dsetA = New DataSet

            sql = "SELECT * FROM [contract_forecastproducts] WHERE [ForecastId] = @origId"
			db.AddDBParam("origId", origId, True)
			db.RunSQL(sql, dsetA, False, "", False)

			For Each drow In dsetA.Tables(0).Rows
				firstpass = True

				For Each dcol In dsetA.Tables(0).Columns
					fieldName = dcol.ColumnName
					Select Case dcol.DataType.ToString
						Case "System.String"
							fieldType = "S"
						Case "System.Int32", "System.Int16", "System.Double"
							fieldType = "N"
						Case "System.DateTime"
							fieldType = "D"
						Case Else
							fieldType = "S"
					End Select

                    Select Case fieldName.ToLower
                        Case "forecastproductid"
                            ' skip contract id because it's the identity field

                        Case "forecastid"
                            db.SetFieldValue("forecastId", newId, "N", firstpass)
                            firstpass = False

                        Case Else
                            If IsDBNull(drow.Item(fieldName)) = False Then
                                If fieldType = "S" Then
                                    db.SetFieldValue(fieldName, Trim(drow.Item(fieldName)), fieldType, firstpass)
                                Else
                                    db.SetFieldValue(fieldName, drow.Item(fieldName), fieldType, firstpass)
                                End If

                                firstpass = False
                            End If
                    End Select
				Next

				If firstpass = False Then
					' must have an invoice to save
					db.FWDb("W", "contract_forecastproducts", "", "", "", "", "", "", "", "", "", "", "", "")
				End If
			Next

			db.DBClose()
			dsetA.Dispose()
			dsetA = Nothing
		End Sub

		Private Sub DuplicateInvoiceProducts(ByVal curUser As CurrentUser, ByVal origId As Integer, ByVal newId As Integer, ByVal newContractId As Integer)
			Dim sql As String
			Dim fieldName, fieldType As String
			Dim drow As DataRow
			Dim dcol As DataColumn
			Dim firstpass As Boolean
			Dim dsetA As DataSet
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection(), curUser.CurrentSubAccountId)
			Dim db As New cFWDBConnection()
			db.DBOpen(fws, False)

			dsetA = New DataSet

            sql = "SELECT * FROM [invoice_productdetails] WHERE [invoiceId] = " & origId.ToString
			db.RunSQL(sql, dsetA, False, "", False)

			For Each drow In dsetA.Tables(0).Rows
				firstpass = True

				For Each dcol In dsetA.Tables(0).Columns
					fieldName = dcol.ColumnName
					Select Case dcol.DataType.ToString
						Case "System.String"
							fieldType = "S"
						Case "System.Int32", "System.Int16", "System.Double"
							fieldType = "N"
						Case "System.DateTime"
							fieldType = "D"
						Case Else
							fieldType = "S"
					End Select

                    Select Case fieldName.ToLower
                        Case "invoiceproductid"
                            ' skip contract id because it's the identity field

                        Case "invoiceid"
                            db.SetFieldValue("invoiceId", newId, "N", firstpass)
                            firstpass = False

                        Case "contractid"
                            db.SetFieldValue("contractId", newContractId, "N", firstpass)
                            firstpass = False

                        Case Else
                            If IsDBNull(drow.Item(fieldName)) = False Then
                                If fieldType = "S" Then
                                    db.SetFieldValue(fieldName, Trim(drow.Item(fieldName)), fieldType, firstpass)
                                Else
                                    db.SetFieldValue(fieldName, drow.Item(fieldName), fieldType, firstpass)
                                End If
                                firstpass = False
                            End If
                    End Select
				Next

				If firstpass = False Then
					' must have an invoice to save
					db.FWDb("W", "invoice_productdetails", "", "", "", "", "", "", "", "", "", "", "", "")
				End If
			Next

			db.DBClose()
			dsetA.Dispose()
			dsetA = Nothing
		End Sub

		Private Sub DuplicateInvoiceLog(ByVal curUser As CurrentUser, ByVal origId As Integer, ByVal newId As Integer)
			Dim sql As String
			Dim fieldName, fieldType As String
			Dim drow As DataRow
			Dim dcol As DataColumn
			Dim firstpass As Boolean
			Dim dsetA As DataSet
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection(), curUser.CurrentSubAccountId)
			Dim db As New cFWDBConnection()
			db.DBOpen(fws, False)

			dsetA = New DataSet

            sql = "SELECT * FROM [invoiceLog] WHERE [invoiceId] = @invId"
            db.AddDBParam("invId", origId, True)
			db.RunSQL(sql, dsetA, False, "", False)

			For Each drow In dsetA.Tables(0).Rows
				firstpass = True

				For Each dcol In dsetA.Tables(0).Columns
					fieldName = dcol.ColumnName
					Select Case dcol.DataType.ToString
						Case "System.String"
							fieldType = "S"
						Case "System.Int32", "System.Int16", "System.Double"
							fieldType = "N"
						Case "System.DateTime"
							fieldType = "D"
						Case Else
							fieldType = "S"
					End Select

                    Select Case fieldName.ToLower
                        Case "invoicelogid"
                            ' skip id because it's the identity field

                        Case "invoiceid"
                            db.SetFieldValue("invoiceId", newId, "N", firstpass)
                            firstpass = False

                        Case Else
                            If IsDBNull(drow.Item(fieldName)) = False Then
                                If fieldType = "S" Then
                                    db.SetFieldValue(fieldName, Trim(drow.Item(fieldName)), fieldType, firstpass)
                                Else
                                    db.SetFieldValue(fieldName, drow.Item(fieldName), fieldType, firstpass)
                                End If

                                firstpass = False
                            End If
                    End Select
				Next

				If firstpass = False Then
					' must have an invoice to save
                    db.FWDb("W", "invoiceLog", "", "", "", "", "", "", "", "", "", "", "", "")
				End If
			Next

			db.DBClose()
			dsetA.Dispose()
			dsetA = Nothing
		End Sub

		Public Sub DuplicateAttachments(ByRef activeServer As HttpServerUtility, ByVal curUser As CurrentUser, ByVal attArea As AttachmentArea, ByVal origId As Integer, ByVal newId As Integer)
			Dim sql As String
			Dim fieldName, fieldType As String
			Dim drow As DataRow
			Dim dcol As DataColumn
			Dim firstpass As Boolean
			Dim dsetA As DataSet
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection(), curUser.CurrentSubAccountId)
			Dim db As New cFWDBConnection()
			db.DBOpen(fws, False)

			dsetA = New DataSet

            sql = "SELECT * FROM [attachments] WHERE [ReferenceNumber] = @refNum AND [AttachmentArea] = @attArea"
            db.AddDBParam("refNum", origId, True)
            db.AddDBParam("attArea", attArea, False)
			db.RunSQL(sql, dsetA, False, "", False)

			For Each drow In dsetA.Tables(0).Rows
				firstpass = True

				For Each dcol In dsetA.Tables(0).Columns
					fieldName = dcol.ColumnName
					Select Case dcol.DataType.ToString
						Case "System.String"
							fieldType = "S"
						Case "System.Int32", "System.Int16", "System.Double"
							fieldType = "N"
						Case "System.DateTime"
							fieldType = "D"
						Case Else
							fieldType = "S"
					End Select

                    Select Case fieldName.ToLower
                        Case "attachmentid"
                            ' skip id because it's the identity field

                        Case "referencenumber"
                            db.SetFieldValue("ReferenceNumber", newId, "N", firstpass)
                            firstpass = False

                        Case "filename"
                            ' cannot link the object to exactly the same file (in case it gets deleted!)
                            Try
                                Dim curFile, filename As String

                                filename = Trim(drow.Item("Filename"))
                                curFile = Trim(drow.Item("Directory")) & filename

                                If File.Exists(activeServer.MapPath(curFile)) = True Then
                                    Dim bIsUnique As Boolean
                                    Dim seqNum As Integer
                                    Dim extension, preExtension As String
                                    seqNum = 1
                                    bIsUnique = False

                                    Do While Not bIsUnique
                                        ' ensure that file of that name doesn't already exist. if so, append seq no on the end
                                        If File.Exists(activeServer.MapPath(Trim(drow.Item("Directory")) & filename)) = True Then
                                            ' file already exists, try next filename sequence
                                            Dim xref, dotPos As Integer

                                            xref = InStr(filename, "_")
                                            dotPos = InStr(filename, ".")
                                            extension = Mid(filename, dotPos)
                                            preExtension = Mid(filename, 1, Len(filename) - ((Len(filename) - dotPos) + 1))

                                            If xref > 0 Then
                                                preExtension = Left(preExtension, xref - 1)
                                            End If

                                            filename = Trim(preExtension) & "_" & Trim(Str(seqNum)) & Trim(extension)
                                            seqNum = seqNum + 1
                                        Else
                                            ' store the file as the filename is now unique
                                            bIsUnique = True
                                        End If
                                    Loop

                                    ' now have a unique filename for the duplicate file
                                    File.Copy(activeServer.MapPath(curFile), activeServer.MapPath(Trim(drow.Item("Directory")) & filename))

                                    ' save the new filename
                                    db.SetFieldValue("Filename", filename, "S", firstpass)
                                    firstpass = False
                                Else
                                    ' file isn't there, so cannot duplicate it!
                                    firstpass = True
                                    Exit For
                                End If

                            Catch ex As Exception
                                ' cannot duplicate this attachments, so move on to the next
                                firstpass = True
                                Exit For
                            End Try

                        Case Else
                            If IsDBNull(drow.Item(fieldName)) = False Then
                                If fieldType = "S" Then
                                    db.SetFieldValue(fieldName, Trim(drow.Item(fieldName)), fieldType, firstpass)
                                Else
                                    db.SetFieldValue(fieldName, drow.Item(fieldName), fieldType, firstpass)
                                End If

                                firstpass = False
                            End If
                    End Select
				Next

				If firstpass = False Then
					' must have an Attachment to save
					db.FWDb("W", "attachments", "", "", "", "", "", "", "", "", "", "", "", "")
					Dim newAttId As Integer

					newAttId = db.glIdentity

					' need to duplicate any audience
                    If CType(drow.Item("AttachmentType"), AttachmentType) = AttachmentType.Audience Then
                        DuplicateAudience(curUser, "attachment_audience", origId, newAttId)
                    End If
				End If
			Next

			db.DBClose()
			dsetA.Dispose()
			dsetA = Nothing
		End Sub

		Private Sub DuplicateNotify(ByVal curUser As CurrentUser, ByVal origId As Integer, ByVal newId As Integer)
			Dim sql As String
			Dim fieldName, fieldType As String
			Dim drow As DataRow
			Dim dcol As DataColumn
			Dim firstpass As Boolean
			Dim dsetA As DataSet
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection(), curUser.CurrentSubAccountId)
			Dim db As New cFWDBConnection()
			db.DBOpen(fws, False)

			dsetA = New DataSet

            sql = "SELECT * FROM [contract_notification] WHERE [contractId] = @conId"
            db.AddDBParam("conId", origId, True)
			db.RunSQL(sql, dsetA, False, "", False)

			For Each drow In dsetA.Tables(0).Rows
				firstpass = True

				For Each dcol In dsetA.Tables(0).Columns
					fieldName = dcol.ColumnName
					Select Case dcol.DataType.ToString
						Case "System.String"
							fieldType = "S"
						Case "System.Int32", "System.Int16", "System.Double"
							fieldType = "N"
						Case "System.DateTime"
							fieldType = "D"
						Case Else
							fieldType = "S"
					End Select

                    Select Case fieldName.ToLower
                        Case "notificationid"
                            ' skip id because it's the identity field

                        Case "contractid"
                            db.SetFieldValue("contractId", newId, "N", firstpass)
                            firstpass = False

                        Case Else
                            If IsDBNull(drow.Item(fieldName)) = False Then
                                If fieldType = "S" Then
                                    db.SetFieldValue(fieldName, Trim(drow.Item(fieldName)), fieldType, firstpass)
                                Else
                                    db.SetFieldValue(fieldName, drow.Item(fieldName), fieldType, firstpass)
                                End If

                                firstpass = False
                            End If
                    End Select
				Next

				If firstpass = False Then
					' must have an invoice to save
					db.FWDb("W", "contract_notification", "", "", "", "", "", "", "", "", "", "", "", "")
				End If
			Next

			db.DBClose()
			dsetA.Dispose()
			dsetA = Nothing
		End Sub

		Private Sub DuplicateAudience(ByVal curUser As CurrentUser, ByVal AudienceTable As String, ByVal origId As Integer, ByVal newId As Integer)
			Dim sql As String
			Dim fieldName, fieldType As String
			Dim drow As DataRow
			Dim dcol As DataColumn
			Dim firstpass As Boolean
			Dim dsetA As DataSet
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection(), curUser.CurrentSubAccountId)
			Dim db As New cFWDBConnection()
			db.DBOpen(fws, False)

			dsetA = New DataSet

			Select Case AudienceTable
				Case "contract_audience"
                    sql = "SELECT * FROM [contract_audience] WHERE [contractId] = @conId"
                    db.AddDBParam("conId", origId, True)
				Case "attachment_audience"
                    sql = "SELECT * FROM [attachment_audience] WHERE [attachmentId] = @attId"
                    db.AddDBParam("attId", origId, True)
				Case Else
					Exit Sub
			End Select

			db.RunSQL(sql, dsetA, False, "", False)

			For Each drow In dsetA.Tables(0).Rows
				firstpass = True

				For Each dcol In dsetA.Tables(0).Columns
					fieldName = dcol.ColumnName
					Select Case dcol.DataType.ToString
						Case "System.String"
							fieldType = "S"
						Case "System.Int32", "System.Int16", "System.Double"
							fieldType = "N"
						Case "System.DateTime"
							fieldType = "D"
						Case Else
							fieldType = "S"
					End Select

                    Select Case fieldName.ToLower
                        Case "audienceid"
                            ' skip id because it's the identity field

                        Case "contractid", "attachmentid"
                            db.SetFieldValue(fieldName, newId, "N", firstpass)
                            firstpass = False

                        Case Else
                            If IsDBNull(drow.Item(fieldName)) = False Then
                                If fieldType = "S" Then
                                    db.SetFieldValue(fieldName, Trim(drow.Item(fieldName)), fieldType, firstpass)
                                Else
                                    db.SetFieldValue(fieldName, drow.Item(fieldName), fieldType, firstpass)
                                End If

                                firstpass = False
                            End If
                    End Select
				Next

				If firstpass = False Then
					' must have an invoice to save
					db.FWDb("W", AudienceTable, "", "", "", "", "", "", "", "", "", "", "", "")
				End If
			Next

			db.DBClose()
			dsetA.Dispose()
			dsetA = Nothing
		End Sub
    End Module
End Namespace
