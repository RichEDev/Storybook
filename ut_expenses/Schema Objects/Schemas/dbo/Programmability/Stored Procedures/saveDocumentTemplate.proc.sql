CREATE procedure [dbo].[saveDocumentTemplate]
@documentid int,
@documentName nvarchar(150),
@documentPath nvarchar(250),
@documentFilename nvarchar(150),
@documentDescription nvarchar(max),
@documentContentType nvarchar(100),
@userid int,
@documentImage varbinary(max),
@CUemployeeID INT,
@CUdelegateID INT,
@mergeProjectId int
as
begin
               declare @returnDocId int
               declare @count int

               if @documentid = 0
               begin
                              SET @count = (SELECT COUNT([documentid]) FROM document_templates WHERE doc_name = @documentName);
                              IF @count > 0
                                             RETURN -1;

                              insert into document_templates (doc_name, doc_path, doc_filename, doc_description, doc_contenttype, createddate, createdby, modifieddate, modifiedby, mergeprojectid)
                              values (@documentName, @documentPath, @documentFilename, @documentDescription, @documentContentType, getdate(), @userid, getdate(), @userid, @mergeProjectId)

                              set @returnDocId = scope_identity()

                              exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 103, @returnDocId, @documentName, null;

                              if not @documentImage is null
                              begin
                                             insert into document_template_data (documentid, document_data)
                                             values (@returnDocId, @documentImage)
                              end
               end
               else
               begin
                              SET @count = (SELECT COUNT([documentid]) FROM document_templates WHERE doc_name = @documentName and documentid <> @documentid);
                              IF @count > 0
                                             RETURN -1;

                              declare @olddocumentName nvarchar(150);
                              declare @olddocumentPath nvarchar(250);
                              declare @olddocumentFilename nvarchar(150);
                              declare @olddocumentDescription nvarchar(max);
                              declare @olddocumentContentType nvarchar(100);
                              declare @oldMergeProjectId int;
                              
                              select @olddocumentName = doc_name, @olddocumentPath = doc_path, @olddocumentFilename = doc_filename, @olddocumentDescription = doc_description, @olddocumentContentType = doc_contenttype, @oldMergeProjectId = mergeprojectid from document_templates where documentid = @documentid;

                              update document_templates set
                              doc_name = @documentName,
                              doc_path = @documentPath,
                              doc_filename = @documentFilename,
                              doc_description = @documentDescription,
                              doc_contenttype = @documentContentType,
                              modifieddate = getdate(),
                              modifiedby = @userid,
                              mergeprojectid = @mergeProjectId
                              where documentid = @documentid;

                              set @returnDocId = @documentid;


                              if @olddocumentName <> @documentName
                                             exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 103, @documentid, null, @olddocumentName, @documentName, @documentName, null;
                              if @olddocumentPath <> @documentPath
                                             exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 103, @documentid, null, @olddocumentPath, @documentPath, @documentName, null;
                              if @olddocumentFilename <> @documentFilename
                                             exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 103, @documentid, null, @olddocumentFilename, @documentFilename, @documentName, null;
                              if @olddocumentDescription <> @documentDescription
                                             exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 103, @documentid, null, @olddocumentDescription, @documentDescription, @documentName, null;
                              if @olddocumentContentType <> @documentContentType
                                             exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 103, @documentid, null, @olddocumentContentType, @documentContentType, @documentName, null;
                              if @oldMergeProjectId <> @mergeProjectId
                                             exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 103, @documentid, null, @oldMergeProjectId, @mergeProjectId, @documentName, null;
                              
                              if not @documentImage is null
                              begin
                                             if exists (select [documentid] from document_template_data where documentid = @documentid)
                                             begin
                                                            update document_template_data set document_data = @documentImage where documentid = @documentid
                                             end
                                             else
                                             begin
                                                            insert into document_template_data (documentid, document_data)
                                                            values (@returnDocId, @documentImage)
                                             end
                              end
               end

			exec TorchRefreshPredefinedSections  
			
               return @returnDocId
end
