CREATE procedure [dbo].[ApiBatchSaveImportLog]
 @list ApiBatchSaveImportLogType READONLY
as
begin
 declare @logID [int]
 declare @logType [tinyint]
 declare @logName [nvarchar] (500)
 declare @successfulLines [int]
 declare @failedLines [int]
 declare @warningLines [int]
 declare @expectedLines [int]
 declare @processedLines [int]
 declare @createdOn [datetime]
 declare @modifiedOn [datetime]
 declare @retIds table(logID int);

 declare TheCursor cursor fast_forward
 for
 select [logID],
  [logType],
  [logName],
  [successfulLines],
  [failedLines],
  [warningLines],
  [expectedLines],
  [processedLines],
  [createdOn],
  [modifiedOn]
 from @list

 open TheCursor;

 fetch next
 from TheCursor
 into @logID,
  @logType,
  @logName,
  @successfulLines,
  @failedLines,
  @warningLines,
  @expectedLines,
  @processedLines,
  @createdOn,
  @modifiedOn

 while @@FETCH_STATUS = 0
 begin
  if @logID = 0
  begin
   insert into logNames
   (
    logType,
    logName,
    successfulLines,
    failedLines,
    warningLines,
    expectedLines,
    processedLines,
    createdOn
   )
   values
   (
    @logType,
    @logName,
    @successfulLines,
    @failedLines,
    @warningLines,
    @expectedLines,
    @processedLines,
    GETUTCDATE()
   )
   
   set @logID = Scope_identity();
  end
  else
  begin
   update logNames
   set logType = @logType,
    logName = @logName,
    successfulLines = @successfulLines,
    failedLines = @failedLines,
    warningLines = @warningLines,
    expectedLines = @expectedLines,
    processedLines = @processedLines,
    modifiedOn = GETUTCDATE()
   where logID = @logID
  end

  insert into @retIds
  values (@logID);

  fetch next
  from TheCursor
  into @logID,
   @logType,
   @logName,
   @successfulLines,
   @failedLines,
   @warningLines,
   @expectedLines,
   @processedLines,
   @createdOn,
   @modifiedOn;
 end;

 close TheCursor;
 deallocate TheCursor;

 select *
 from @retIds;

 return 0;
end