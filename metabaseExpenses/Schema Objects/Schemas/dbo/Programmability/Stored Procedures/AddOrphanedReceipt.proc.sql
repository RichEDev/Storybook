CREATE PROCEDURE [dbo].[AddOrphanedReceipt]
 @fileExtension nvarchar(6),
 @creationMethod tinyint
AS
BEGIN
 SET NOCOUNT ON;
 INSERT INTO OrphanedReceipts (FileExtension, CreationMethod) 
 VALUES (@fileExtension, @creationMethod); 
 RETURN SCOPE_IDENTITY();
END