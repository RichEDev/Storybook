CREATE procedure [dbo].[APIBatchSaveImportHistory]
 @list APIBatchSaveImportHistoryType READONLY
as
begin
 declare @historyId [INT]
 declare @importId [INT]
 declare @logId [INT]
 declare @importedDate [DATETIME]
 declare @importStatus [INT]
 declare @applicationType [INT]
 declare @dataId [INT]
 declare @count int
 declare @retIds table(historyid int);

 declare thecursor cursor fast_forward
 for
 select historyid,
  importid,
  logid,
  importeddate,
  importstatus,
  applicationtype,
  dataid
 from @list

 open thecursor;

 fetch next
 from thecursor
 into @historyId,
  @importId,
  @logId,
  @importedDate,
  @importStatus,
  @applicationType,
  @dataId

 while @@FETCH_STATUS = 0
 begin
  if @historyId = 0
  begin
   insert into importhistory
   (
    importid,
    logid,
    importeddate,
    importstatus,
    applicationtype,
    dataid,
    createdon
   )
   values
   (
    @importId,
    @logId,
    @importedDate,
    @importStatus,
    @applicationType,
    @dataId,
    Getdate()
   );

   set @historyId = Scope_identity();
  end
  else
  begin
   update importhistory
   set importid = @importId,
    logid = @logId,
    importeddate = @importedDate,
    importstatus = @importStatus,
    applicationtype = @applicationType,
    dataid = @dataId,
    modifiedon = Getdate()
   where historyid = @historyId;
  end

  insert into @retIds
  values (@historyId);

  fetch next
  from thecursor
  into @historyId,
   @importId,
   @logId,
   @importedDate,
   @importStatus,
   @applicationType,
   @dataId

 end;

 close thecursor;
 deallocate thecursor;

 select *
 from @retIds;

 return 0;
end