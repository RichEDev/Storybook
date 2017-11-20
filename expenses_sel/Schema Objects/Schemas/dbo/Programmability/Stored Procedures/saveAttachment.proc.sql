﻿

CREATE PROCEDURE [dbo].[saveAttachment] 

@recordID int,
@idfieldname nvarchar(100),
@tablename nvarchar(300),
@title nvarchar(200),
@description nvarchar(max),
@fileName nvarchar(150),
@mimeID int,
@userID int,
@fileData varbinary(MAX),
@CUemployeeID INT,
@CUdelegateID INT

AS

BEGIN

      DECLARE @sql nvarchar(2500)

      DECLARE @attachmentid int

      DECLARE @parmDefinition nvarchar(50)

 

      SET @sql = 'INSERT INTO [' + replace(@tablename,'_attachments','_attachmentData') + '] (fileData) VALUES (@inData); set @newIdentity = scope_identity();'

      SET @parmDefinition = '@inData varbinary(max), @newIdentity int OUTPUT'

      EXECUTE sp_executesql @sql, @parmDefinition, @inData = @fileData, @newIdentity = @attachmentid OUTPUT

 

      if @attachmentid is null or @attachmentid = 0

      begin

            return 0

      end

      else

      begin

            SET @sql = 'INSERT INTO [' + @tablename + '] (attachmentID, ' + @idfieldname + ', title, description, fileName, mimeID, createdon, createdby) ' +

            'VALUES (' + CAST(@attachmentid as nvarchar) + ',' + CAST(@recordID as nvarchar) + ',' + char(39) + @title + char(39) + ',' + char(39) + @description + char(39) + ',' + char(39) + @fileName + char(39) + ',' + CAST(@mimeID as nvarchar) + ',getutcdate(),' + CAST(@userID as nvarchar) + ')'

 

            execute sp_executesql @sql

 
			exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @attachmentid, @tablename, null;

            return @attachmentid

      end

END
