CREATE PROCEDURE [dbo].[saveUserDefinedField]
@userdefineid INT,
@attributename nvarchar(250),
@displayname nvarchar(250),
@description nvarchar(4000),
@tooltip NVARCHAR(4000),
@mandatory BIT,
@fieldtype TINYINT,
@date DateTime,
@userid INT,
@maxlength INT,
@format TINYINT,
@defaultvalue NVARCHAR(50),
@precision TINYINT,
@order INT,
@tableid UNIQUEIDENTIFIER,
@groupID int,
@specificItem BIT,
@hyperlinkText nvarchar(MAX),
@hyperlinkPath nvarchar(MAX),
@allowSearch bit,
@relatedTable UNIQUEIDENTIFIER,
@acDisplayField UNIQUEIDENTIFIER,
@maxRows INT,
@allowEmployeeToPopulate bit,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @count INT;
	if (@userdefineid = 0)
		BEGIN
			SET @count = (SELECT COUNT(*) FROM [userdefined] WHERE userdefineid = @userdefineid AND attribute_name = @attributename);
					IF @count > 0
						RETURN -1;

						INSERT INTO [userdefined] (attribute_name, display_name, fieldtype, mandatory, [description], [order], createdon, createdby, tooltip, maxlength, format, defaultvalue, tableid, groupID, specific, [precision], hyperlinkText, hyperlinkPath, allowSearch, relatedTable, displayField, maxRows, allowEmployeeToPopulate) VALUES (@attributename, @displayname, @fieldtype, @mandatory, @description, @order, @date, @userid, @tooltip, @maxlength, @format, @defaultvalue, @tableid, @groupID, @specificItem, @precision, @hyperlinkText, @hyperlinkPath, @allowSearch, @relatedTable, @acDisplayField, @maxRows, @allowEmployeeToPopulate);
						SET @userdefineid = SCOPE_IDENTITY();
						DECLARE @sql NVARCHAR(4000);
						DECLARE @tablename NVARCHAR(500);
						SELECT @tablename = tablename FROM tables WHERE tableid = @tableid;
						SET @sql = 'alter TABLE [' + @tablename + '] add [udf' + cast(@userdefineid as nvarchar(10)) + ']';

						IF @fieldtype = 1
							begin
								SET @sql = @sql + ' nvarchar'
								IF @maxlength IS NULL
									BEGIN
										SET @sql = @sql + '(4000)';
									END
								else
									BEGIN
										SET @sql = @sql + '(' + CAST(@maxlength AS NVARCHAR(10)) + ')';
									END
							END
						ELSE IF @fieldtype = 2
							BEGIN
								SET @sql = @sql + ' int'
							end	
						ELSE IF @fieldtype = 7
							BEGIN
								SET @sql = @sql + ' decimal(18,' + CAST(@precision AS NVARCHAR(4)) + ')';
							END
						ELSE IF @fieldtype = 6
							BEGIN
								SET @sql = @sql + ' money'
							end	
						ELSE IF @fieldtype = 4
							BEGIN
								SET @sql = @sql + ' nvarchar(50)'
							end	
						ELSE IF @fieldtype = 5
							BEGIN
								SET @sql = @sql + ' bit'
							end	
						ELSE IF @fieldtype = 3
							BEGIN
								SET @sql = @sql + ' DateTime'
							end	
						ELSE IF @fieldtype = 10
							BEGIN
								SET @sql = @Sql + ' nvarchar(max)'
							end
						ELSE IF @fieldtype = 8
							BEGIN
								SET @sql = @sql + ' nvarchar(max)'
							END
						ELSE IF @fieldtype = 9
							BEGIN
								SET @sql = @sql + ' int'
							END
						ELSE IF @fieldtype = 16
							BEGIN
								SET @sql = @sql + ' nvarchar(2000)'
							END
						
						EXECUTE sp_executesql @sql;

				exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, @attributename, null;
			END
		ELSE IF (@userdefineid > 0)
		BEGIN
			declare @oldattributename nvarchar(250);
			declare @olddisplayname nvarchar(250);
			declare @olddescription nvarchar(4000);
			declare @oldtooltip NVARCHAR(4000);
			declare @oldmandatory BIT;
			declare @oldmaxlength INT;
			declare @oldformat TINYINT;
			declare @oldprecision TINYINT;
			declare @oldorder INT;
			declare @oldspecificItem BIT;
			declare @oldhyperlinkText nvarchar(MAX);
			declare @oldhyperlinkPath nvarchar(MAX);
			declare @oldallowsearch bit;
			declare @oldRelatedTable uniqueidentifier;
			declare @oldDisplayField uniqueidentifier;
			declare @oldMaxRows int;
			declare @oldallowEmployeeToPopulate bit;

			select @oldattributename = attribute_name, @olddisplayname = display_name, @olddescription = [description], @oldtooltip = tooltip, @oldmandatory = mandatory, @oldmaxlength = maxlength, @oldformat = format, @oldprecision = [precision], @oldorder = [order], @oldspecificItem = specific, @oldhyperlinkText = hyperlinkText, @oldhyperlinkPath = hyperlinkPath, @oldallowsearch = allowSearch, @oldRelatedTable = relatedTable, @oldDisplayField = displayField, @oldMaxRows = maxRows, @oldallowEmployeeToPopulate = allowEmployeeToPopulate  from [userdefined] WHERE userdefineid=@userdefineid;

			UPDATE [userdefined] SET attribute_name=@attributename, display_name=@displayname, mandatory=@mandatory, [description]=@description, [order]=@order, modifiedOn=getutcdate(), modifiedBy=@userid, tooltip=@tooltip, maxlength=@maxlength, format=@format, specific=@specificItem, defaultvalue = @defaultvalue, [precision] = @precision, hyperlinkText = @hyperlinkText, hyperlinkPath = @hyperlinkPath, allowSearch = @allowSearch, relatedTable = @relatedTable, groupID = @groupID, displayField = @acDisplayField, maxRows = @maxRows, allowEmployeeToPopulate = @allowEmployeeToPopulate WHERE userdefineid=@userdefineid;

			if @oldattributename <> @attributename
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, 'ae62f2ab-0e4f-4902-864d-3cd4cf05ebca', @oldattributename, @attributename, @attributename, null;
			if @olddisplayname <> @displayname
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, 'b384f241-1cf5-4c9e-8bbe-07880ab2a0a4', @olddisplayname, @displayname, @attributename, null;
			if @olddescription <> @description
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, 'f48c4391-2360-4e01-9d06-2cebdeea701f', @olddescription, @description, @attributename, null;
			if @oldtooltip <> @tooltip
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, '00eb22f0-6720-4ba5-996d-82dccb4fd959', @oldtooltip, @tooltip, @attributename, null;
			if @oldmandatory <> @mandatory
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, '2cf396de-8db6-4c79-a30d-a44584609351', @oldmandatory, @mandatory, @attributename, null;
			if @oldmaxlength <> @maxlength
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, '919dd370-4291-4328-a4b1-283441ef310f', @oldmaxlength, @maxlength, @attributename, null;
			if @oldformat <> @format
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, '9f1d8603-25b5-4ee5-9797-4fa158027d44', @oldformat, @format, @attributename, null;
			if @oldprecision <> @precision
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, '6137bf59-9870-46ff-992a-3675dd88a090', @oldprecision, @precision, @attributename, null;
			if @oldorder <> @order
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, 'd0c2cd70-5de1-4bf7-81fb-b2d821dad4d1', @oldorder, @order, @attributename, null;
			if @oldspecificItem <> @specificItem
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, 'e0db7f7b-1ce8-4912-aaca-c4f3eeff7379', @oldspecificItem, @specificItem, @attributename, null;
			if @oldhyperlinkText <> @hyperlinkText
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, '56455dda-c52d-4c9f-a88e-126718a549ff', @oldhyperlinkText, @hyperlinkText, @attributename, null;
			if @oldhyperlinkPath <> @hyperlinkPath
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, '41a85548-5e50-45b1-b671-43354b978256', @oldhyperlinkPath, @hyperlinkPath, @attributename, null;
			if @oldallowsearch <> @allowSearch
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, '26D0033C-66D0-4223-A8A3-FA1570C4735F', @oldallowsearch, @allowSearch, @attributename, null;
			if @oldRelatedTable <> @relatedTable
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, '4b8a4cb0-af40-4da7-b0d6-468076806c26', @oldRelatedTable, @relatedTable, @attributename, null;
			if @oldDisplayField <> @acDisplayField
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, '69166EFE-BA0C-4750-8883-6D4C750D64B3', @oldDisplayField, @acDisplayField, @attributename, null;
			if @oldMaxRows <> @maxRows
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, 'D5E74321-DC00-47C5-8DDB-6BA1EAEB4D78', @oldMaxRows, @maxRows, @attributename, null;
			if @oldallowEmployeeToPopulate <> @allowEmployeeToPopulate
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, '29DFEC86-8BBE-476E-96D5-BEDB3DEB33FA', @oldallowEmployeeToPopulate, @allowEmployeeToPopulate, @attributename, null;
		END

		RETURN @userdefineid;
END

