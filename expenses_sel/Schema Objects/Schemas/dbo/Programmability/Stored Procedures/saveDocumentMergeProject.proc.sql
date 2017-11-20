CREATE procedure [dbo].[saveDocumentMergeProject]
@projectid int,
@projectName nvarchar(150),
@projectDescription nvarchar(max),
@userId int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
               declare @returnProjectId int
               declare @count int

               if @projectid = 0
               begin
                              set @count = (select count([mergeprojectid]) from document_mergeprojects where project_name = @projectName);
                              if @count > 0
                                             return -1;

                              insert into document_mergeprojects (project_name, project_description, complete, createddate, createdby, modifieddate, modifiedby)
                              values (@projectName, @projectDescription, 0, getdate(), @userId, getdate(), @userId)

                              set @returnProjectId = scope_identity();

                              exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @returnProjectId, @projectName, null;
               end
               else
               begin
                              set @count = (select count([mergeprojectid]) from document_mergeprojects where project_name = @projectName and mergeprojectid <> @projectId)
                              if @count > 0
                                             return -1;

                              declare @oldprojectName nvarchar(150);
                              declare @oldprojectDescription nvarchar(max);
                              declare @oldprojectStatus tinyint;
                              declare @olddocumentId int;

                              select @oldprojectName = project_name, @oldprojectDescription = project_description from document_mergeprojects where mergeprojectid = @projectId;

                              update document_mergeprojects set project_name = @projectName, project_description = @projectDescription, modifieddate = getdate(), modifiedby = @userId
                              where mergeprojectid = @projectId;

                              if @oldprojectName <> @projectName
                                             exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @projectId, '9a589843-5fab-44fd-81e6-dd9575220f76', @oldprojectName, @projectName, @projectName, null;
                              if @oldprojectDescription <> @projectDescription
                                             exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @projectId, 'e415c68b-2d40-4012-be7c-7a576ca956c8', @oldprojectDescription, @projectDescription, @projectName, null;

                              set @returnProjectId = @projectid;
               end

               return @returnProjectId
end
