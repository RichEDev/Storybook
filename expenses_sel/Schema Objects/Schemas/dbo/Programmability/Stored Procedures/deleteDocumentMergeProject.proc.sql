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

                                             declare @mappingid int
                                             declare loop_cursor cursor for
                                             select mappingid from document_mappings where mergeprojectid = @projectid
                                             open loop_cursor
                                             fetch next from loop_cursor into @mappingid
                                             
                                             while @@fetch_status = 0            
                                             begin
                                                            delete from document_mappings_field where mappingid = @mappingid
                                                            delete from document_mappings_static where mappingid = @mappingid
                                                            delete from document_mappings_table where mappingid = @mappingid
                                                            
                                                            fetch next from loop_cursor into @mappingid
                                             end
                                             close loop_cursor
                                             deallocate loop_cursor
                                             
                                             delete from document_mappings where mergeprojectid = @projectid
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
