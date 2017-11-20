CREATE PROCEDURE [dbo].[saveOutboundFileData] 

@FileData varbinary(MAX),
@NHSTrustID int,
@FileName nvarchar(250),
@FileCreatedOn datetime,
@FileModifiedOn datetime,
@Status tinyint
      
AS

BEGIN
      INSERT INTO OutboundFileData (FileData, NHSTrustID, [FileName], FileCreatedOn, FileModifiedOn, Status) VALUES (@FileData, @NHSTrustID, @FileName, @FileCreatedOn, @FileModifiedOn, @Status)
END
