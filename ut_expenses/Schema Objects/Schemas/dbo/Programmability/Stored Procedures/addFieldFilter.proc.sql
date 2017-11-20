
CREATE PROCEDURE [dbo].[addFieldFilter]
	-- Add the parameters for the stored procedure here
	@viewID INT,
	@attributeID INT,
	@userdefineID INT,
	@fieldID UNIQUEIDENTIFIER,
	@order TINYINT,
	@operator TINYINT,
	@valueOne NVARCHAR(150),
	@valueTwo NVARCHAR(150),
	@joinViaID INT,
	@CUemployeeID INT,
	@CUdelegateID INT,
	@formID INT,
	@isParentFilter bit
AS
BEGIN
	DECLARE @ExistingResults INT;
	
	IF @viewID IS NOT NULL
	BEGIN
		-- Custom entity view filter
		SET @ExistingResults = (SELECT COUNT (fieldid) 
				FROM fieldFilters 
				WHERE viewid = @viewID AND [order] = @order);
	END
	ELSE IF @attributeID IS NOT NULL
	BEGIN
		-- n:1 match field filter
		SET @ExistingResults = (SELECT COUNT (fieldid) 
				FROM fieldFilters 
				WHERE attributeid = @attributeID AND formid = @formID AND [order] = @order And isParentFilter=@isParentFilter);
	END
	ELSE IF @userdefineID IS NOT NULL
	BEGIN
		-- UDF relationship match filter
		SET @ExistingResults = (SELECT COUNT (fieldid) 
				FROM fieldFilters 
				WHERE userdefineid = @userdefineID AND [order] = @order);
	END				
	
	IF (@ExistingResults > 0)
		RETURN -1
	ELSE
	BEGIN
		INSERT INTO [fieldFilters] (
			[viewid],
			[attributeid],
			[userdefineid],
			[fieldid],
			[order],
			[condition],
			[value],
			[valueTwo],
			[joinViaID],
			[formid],
			isParentFilter
		) VALUES ( @viewID, @attributeID, @userdefineID, @fieldID, @order, @operator, @valueOne, @valueTwo, @joinViaID, @formID,@isParentFilter);
	END
	RETURN SCOPE_IDENTITY();
END