CREATE PROCEDURE [dbo].[APIgetUserDefinedMatchFieldsSpecial]
	@reference nvarchar(100)
	
AS
	SELECT userdefineid, fieldid 
		FROM userdefinedMatchFields 
		WHERE userdefineid = @reference
RETURN 0