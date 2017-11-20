CREATE PROCEDURE [dbo].[saveImportTemplateMapping]
@templateMappingID int,
@templateID int,
@fieldID uniqueidentifier,
@destinationField nvarchar(250),
@columnRef int,
@importElementType tinyint,
@mandatory bit,
@dataType tinyint,
@lookupTable uniqueidentifier,
@matchField uniqueidentifier,
@overridePrimaryKey bit,
@importField bit,
@CUemployeeID INT,
@CUdelegateID INT

AS

BEGIN
 declare @title1 nvarchar(500);
 select @title1 = templateName FROM importTemplates WHERE templateID = @templateID;

 declare @recordTitle nvarchar(2000);
 set @recordTitle = (select 'Import Template Mapping - ' + @title1);

 INSERT INTO importTemplateMappings (templateID, fieldID, destinationField, columnRef, importElementType, mandatory, dataType, lookupTable, matchField, overridePrimaryKey, importField) 
 VALUES (@templateID, @fieldID, @destinationField, @columnRef, @importElementType, @mandatory, @dataType, @lookupTable, @matchField, @overridePrimaryKey, @importField)
 SET @templateMappingID = scope_identity();

 exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 27, @templateMappingID, @recordTitle, null;
END

return @templateMappingID
