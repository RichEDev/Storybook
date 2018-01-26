Imports SpendManagementLibrary
Imports FWClasses
Imports Spend_Management

Namespace Framework2006
    Partial Class ViewSingleNote
        Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub


        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim NoteType As String
            Dim NoteIdx, OwnerIdx As Integer
			Dim db As New cFWDBConnection
            Dim IDField, NoteTable, srcTable, srcField, OwnerIdField As String
            Dim strHTML As New System.Text.StringBuilder
            Dim sql As New System.Text.StringBuilder
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim att_area As AttachmentArea
            Dim currentElement As SpendManagementElement

			db.DBOpen(fws, False)

            If Me.IsPostBack = False Then
                Title = "View Note"
                Master.title = Title
                Master.enablenavigation = False
                Master.useCloseNavigationMsg = True

                NoteType = Request.QueryString("notetype")
                NoteIdx = Request.QueryString("id")
                OwnerIdx = Request.QueryString("ownerid")

                Select Case LCase(NoteType)
                    Case "suppliers"
                        IDField = "supplierId"
                        OwnerIdField = IDField
                        NoteTable = "supplierNotes"
                        srcTable = "supplier_details"
                        srcField = "supplierName"
                        att_area = AttachmentArea.VENDOR_NOTES
                        currentElement = SpendManagementElement.SupplierNotes

                    Case "contract"
                        IDField = "contractID"
                        OwnerIdField = IDField
                        NoteTable = "contractNotes"
                        srcTable = "contract_details"
                        srcField = "Contract Description"
                        att_area = AttachmentArea.CONTRACT_NOTES
                        currentElement = SpendManagementElement.ContractNotes
                        ViewState("ActiveContract") = ownerIdx

                    Case "invoice"
                        IDField = "invoiceID"
                        OwnerIdField = "contractId"
                        NoteTable = "invoiceNotes"
                        srcTable = "invoices"
                        srcField = "InvoiceNumber"
                        att_area = AttachmentArea.INVOICE_NOTES
                        currentElement = SpendManagementElement.InvoiceNotes

                    Case "product"
                        IDField = "productID"
                        OwnerIdField = "contractId"
                        NoteTable = "productNotes"
                        srcTable = "productDetails"
                        srcField = "ProductName"
                        att_area = AttachmentArea.PRODUCT_NOTES
                        currentElement = SpendManagementElement.ProductNotes

                    Case "suppliercontact"
                        IDField = "contactId"
                        OwnerIdField = "supplierId"
                        NoteTable = "suppliercontactnotes"
                        srcTable = "supplier_contacts"
                        srcField = "Name"
                        att_area = AttachmentArea.CONTACT_NOTES
                        currentElement = SpendManagementElement.SupplierContactNotes

                    Case Else
                        currentElement = SpendManagementElement.None
                        db.DBClose()
                        db = Nothing
                        Exit Sub
                End Select

                curUser.CheckAccessRole(AccessRoleType.View, currentElement, False, True)

                db.FWDb("R2", srcTable, IDField, OwnerIdx, "", "", "", "", "", "", "", "", "", "")

                If db.FWDb2Flag = True Then
                    lblTitle.Text = " for " & Trim(db.FWDbFindVal(srcField, 2))
                End If

                sql.Append("SELECT [" & NoteTable & "].*,[primarynotes].[fullDescription] AS [primarydesc],[secondarynotes].[fullDescription] AS [secondarydesc],")
                sql.Append("(SELECT COUNT(*) FROM [attachments] WHERE [ReferenceNumber] = [" & NoteTable & "].[noteId] AND [AttachmentArea] = @attArea AND dbo.CheckAttachmentAccess([AttachmentId],@userId) > 0) AS [Num Attachments] ")
                sql.Append("FROM [" & NoteTable & "] ")
                sql.Append("INNER JOIN [codes_notecategory] AS [primarynotes] ON [primarynotes].[noteCatId] = [" & NoteTable & "].[noteType] ")
                sql.Append("INNER JOIN [codes_notecategory] AS [secondarynotes] ON [secondarynotes].[noteCatId] = [" & NoteTable & "].[noteCatId] ")
                sql.Append("WHERE [noteId] = @noteId ")
                sql.Append("ORDER BY [Date] DESC")
                db.AddDBParam("noteId", NoteIdx, True)
                db.AddDBParam("attArea", att_area, False)
                db.AddDBParam("userId", curUser.EmployeeID, False)
                'sql = "SELECT [" & Trim(NoteTable) & "].*,[primarynotes].[Full Description] AS [primarydesc],[secondarynotes].[Full Description] AS [secondarydesc] " & vbNewLine & _
                '    "FROM [" & Trim(NoteTable) & "]" & vbNewLine & _
                '    "INNER JOIN [codes_notecategory] AS [primarynotes] ON [primarynotes].[Note Cat Id] = [" & Trim(NoteTable) & "].[Note Type]" & vbNewLine & _
                '    "INNER JOIN [codes_notecategory] AS [secondarynotes] ON [secondarynotes].[Note Cat Id] = [" & Trim(NoteTable) & "].[Note Cat Id]" & vbNewLine & _
                '    "WHERE [Note Id] = " & Trim(Str(NoteIdx))
                db.RunSQL(sql.ToString, db.glDBWorkD, False, "", False)

                strHTML.Append("<table>" & vbNewLine)
                strHTML.Append("<tr>" & vbNewLine)

                If db.GetRowCount(db.glDBWorkD, 0) > 0 Then
                    If db.GetFieldValue(db.glDBWorkD, "Num Attachments", 0, 0) > 0 Then
                        ' must have attachments, so display ddlist with attachments
                        Dim attSQL As String
                        attSQL = "SELECT * FROM [attachments] WHERE [AttachmentArea] = @att_area AND [ReferenceNumber] = @refId AND dbo.CheckAttachmentAccess([AttachmentId],@userId) > 0 ORDER BY [Description]"
                        db.AddDBParam("att_area", att_area, True)
                        db.AddDBParam("refId", db.GetFieldValue(db.glDBWorkD, "NoteId", 0, 0), False)
                        db.AddDBParam("userId", curUser.EmployeeID, False)
                        db.RunSQL(attSQL, db.glDBWorkI, False, "", False)

                        If db.glNumRowsReturned > 0 Then
                            Dim ddlistHTML As New System.Text.StringBuilder
                            Dim drow As DataRow
                            Dim crypt As New cSecureData

                            ddlistHTML.Append("<select name=""attachments"" onchange=""OpenAttachment();"">" & vbNewLine)
                            ddlistHTML.Append("<option value=""0"" selected>[select attachment]</option>" & vbNewLine)

                            Dim tmpStr As String
                            For Each drow In db.glDBWorkI.Tables(0).Rows
                                tmpStr = ""

                                If IsDBNull(drow.Item("Description")) = True Then
                                    If CType(drow.Item("AttachmentType"), AttachmentType) = AttachmentType.Hyperlink Then
                                        tmpStr = drow.Item("Directory")
                                    Else
                                        tmpStr = Trim(drow.Item("Directory")) & Trim(drow.Item("Filename"))
                                    End If
                                Else
                                    If Trim(drow.Item("Description")) = "" Then
                                        If CType(drow.Item("AttachmentType"), AttachmentType) = AttachmentType.Hyperlink Then
                                            tmpStr = Trim(drow.Item("Directory"))
                                        Else
                                            tmpStr = Trim(drow.Item("Directory")) & Trim(drow.Item("Filename"))
                                        End If
                                    Else
                                        tmpStr = Trim(drow.Item("Description"))
                                    End If
                                End If

                                Select Case CType(drow.Item("AttachmentType"), AttachmentType)
                                    Case AttachmentType.Hyperlink
                                        ddlistHTML.Append("<option value=""" & Trim(drow.Item("AttachmentType")) & drow.Item("Directory") & """>" & tmpStr & "</option>" & vbNewLine)
                                    Case Else
                                        ddlistHTML.Append("<option value=""" & Trim(drow.Item("AttachmentType")) & Server.UrlEncode(crypt.Encrypt(Trim(drow.Item("AttachmentId")))) & """>" & tmpStr & "</option>" & vbNewLine)
                                End Select
                            Next
                            ddlistHTML.Append("</select>" & vbNewLine)

                            strHTML.Append("<td class=""labeltd"">Note Attachments</td>" & vbNewLine)
                            strHTML.Append("<td class=""inputtd"">")
                            strHTML.Append(ddlistHTML.ToString)
                            strHTML.Append("</td>" & vbNewLine & "</tr>" & vbNewLine & "<tr>" & vbNewLine)
                        End If
                    End If
                    strHTML.Append("<td class=""inputtd formpanel_padding"" colspan=""2"">" & vbNewLine)
                    strHTML.Append("<textarea readonly rows=""20"" style=""width: 800px; height:600px;"">" & Trim(db.GetFieldValue(db.glDBWorkD, "Note", 0, 0)) & "</textarea>" & vbNewLine)
                    strHTML.Append("</td>" & vbNewLine)
                Else
                    strHTML.Append("<td class=""row1"" align=""center"" width=""250"">Note data was unavailable</td>" & vbNewLine)
                End If

                strHTML.Append("</tr>" & vbNewLine)
                strHTML.Append("</table>" & vbNewLine)

                litNoteData.Text = strHTML.ToString
            End If

            db.DBClose()
            db = Nothing
        End Sub

        Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
            Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.NotesSummary & "&id=" & ViewState("ActiveContract"), True)
        End Sub
    End Class

End Namespace
