

CREATE PROCEDURE [dbo].[DeleteFieldFilters]
	@filterType INT,
	@recordID INT,
	@CUemployeeID INT,
	@CUdelegateID INT,
	@formID INT,
	@isParentFilter BIT
AS
BEGIN
	IF @filterType = 0
	BEGIN
		-- Deleting custom entity view filter
		DELETE FROM [fieldFilters] WHERE viewid = @recordID;
	END

	IF @filterType = 1
	BEGIN
		-- Deleting n:1 match field filter
		IF @formID IS NULL
		DELETE FROM [fieldFilters] WHERE attributeid = @recordID AND formid IS NULL
		ELSE
		DELETE FROM [fieldFilters] WHERE attributeid = @recordID AND formid = @formID AND isParentFilter=@isParentFilter;
	END

	IF @filterType = 2
	BEGIN
		-- Deleting UDF relationship match filter
		DELETE FROM [fieldFilters] WHERE userdefineid = @recordID;
	END
	RETURN 0;
END