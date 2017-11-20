CREATE procedure [dbo].[saveDocumentTableConfig]
@userid int,
@mappingid int,
@reportcolumnid uniqueidentifier,
@hiddencolumnid uniqueidentifier,
@columnidx int,
@width decimal(18,2),
@alternateHeader nvarchar(100),
@columnbold bit,
@subAccountID int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
      delete from document_mappings_table where mappingid = @mappingid and columnidx = @columnidx;

      insert into document_mappings_table (mappingid, reportcolumnid, hiddencolumnid, columnidx, width, headertext, columntextbold) values (@mappingid, @reportcolumnid, @hiddencolumnid, @columnidx, @width, @alternateHeader, @columnbold);

      declare @title1 nvarchar(500);
      declare @recordTitle nvarchar(2000);
      select @title1 = project_name from document_mergeprojects where mergeprojectid = (select mergeprojectid from document_mappings where mappingid = @mappingid);
      set @recordTitle = (select 'Table Config for ' + @title1);

      exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @mappingid, @recordTitle, @subAccountID;

      return;
end
