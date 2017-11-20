CREATE procedure [dbo].[deleteDocumentMergeProject]
@projectid int,
@userid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
begin
               IF NOT EXISTS (SELECT TOP 1 documentid FROM document_templates WHERE mergeprojectid = @projectid)
                              BEGIN
                                             declare @recordTitle nvarchar(2000);
                                             select @recordTitle = project_name from document_mergeprojects where mergeprojectid = @projectid;

                                             delete from document_mergesources where mergeprojectid = @projectid
                                             delete from document_mergeprojects where mergeprojectid = @projectid

                                             exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @projectid, @recordTitle, null;
                                             SELECT 0
                              END
               ELSE
                              BEGIN
                                             SELECT -1
                              END
end
