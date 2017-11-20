CREATE PROCEDURE [dbo].[saveCustomEntityForm] 
(
@formid INT, 
@entityid INT, 
@formname NVARCHAR(100), 
@description NVARCHAR(4000),
@showSave bit,
@saveButtonText nvarchar(100),
@showSaveAndNew bit,
@saveAndNewButtonText nvarchar(100),
@showSaveAndStay bit,
@saveAndStayButtonText nvarchar(100),
@showCancel bit,
@cancelButtonText nvarchar(100),
@showSubMenus bit,
@showBreadcrumbs bit,
@date DATETIME,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @count INT;
    DECLARE @recordTitle nvarchar(250);
    
    IF @formid = 0
		BEGIN
			SET @count = (SELECT COUNT(*) FROM [custom_entity_forms] WHERE entityid = @entityid AND form_name = @formname);
			IF @count > 0
				RETURN -1;
			
			INSERT INTO [custom_entity_forms] (
				[entityid],
				[form_name],
				[description],
				[showSave],
				[SaveButtonText],
				[showSaveAndNew],
				[SaveAndNewButtonText],
				[showSaveAndStay],
				[saveAndStayButtonText],
				[showCancel],
                [cancelButtonText],
				[showBreadCrumbs],
				[showSubMenus],
				[createdon],
				[createdby]
			) VALUES 
			(	@entityid, 
				@formname, 
				@description, 
				@showSave,
				@saveButtonText,
				@showSaveAndNew,
				@saveAndNewButtonText,
				@showSaveAndStay,
				@saveAndStayButtonText,
				@showCancel,
				@cancelButtonText,
				@showBreadcrumbs,
				@showSubMenus,
				@date, 
				@CUemployeeID
			);
			SET @formid = SCOPE_IDENTITY();
			
			set @recordTitle = 'Custom Entity Form ID: ' + CAST(@formid AS nvarchar(20)) + ' (' + @description + ')';
			exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 133, @formid, @recordTitle, null;
		END
	ELSE
		BEGIN
			SET @count = (SELECT COUNT(*) FROM [custom_entity_forms] WHERE entityid = @entityid AND form_name = @formname AND formid <> @formid);
			IF @count > 0
				RETURN -1;
			
			declare @oldformname NVARCHAR(100);
			declare @olddescription NVARCHAR(4000);
			declare @oldshowSave bit;
			declare @oldsaveButtonText nvarchar(100);
			declare @oldshowSaveAndNew bit;
			declare @oldsaveAndNewButtonText nvarchar(100);
			declare @oldshowSaveAndStay bit;
			declare @oldsaveAndStayButtonText nvarchar(100);
			declare @oldshowCancel bit;
			declare @oldcancelButtonText nvarchar(100);
			declare @oldshowSubMenus bit;
			declare @oldshowBreadcrumbs bit;

			select @oldformname = form_name, @olddescription=description, @oldshowSave=showSave, @oldsaveButtonText=saveButtonText, @oldshowSaveAndNew=showSaveAndNew, @oldsaveAndNewButtonText=saveAndNewButtonText,
			@oldsaveAndStayButtonText=saveAndStayButtonText, @oldshowCancel=showCancel, @oldcancelButtonText=cancelButtonText, @oldshowBreadcrumbs=showBreadcrumbs, @oldshowSubMenus=showSubMenus
			from custom_entity_forms where formid = @formid;

			set @recordTitle = 'Custom Entity Form ID: ' + CAST(@formid AS nvarchar(20)) + ' (' + @description + ')';

			if @olddescription <> @description
				exec addUpdateEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, '8D763165-545C-4EF5-99F1-D07D22021426', @olddescription, @description, @recordtitle, null;

			if @oldformname <> @formname
				exec addUpdateEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, '619E9023-4252-401F-A1AD-0A16A89DAE6B', @oldformname, @formname, @recordtitle, null;

			if @oldshowSave <> @showSave
				exec addUpdateEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, 'B8F89B53-DFB2-4472-8D0F-7E593B863287', @oldshowSave, @showSave, @recordtitle, null;
			
			if @oldsaveButtonText <> @saveButtonText
				exec addUpdateEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, 'ED5BECD9-7335-416B-A30A-828BB101C43F', @oldsaveButtonText, @saveButtonText, @recordtitle, null;
			
			if @oldshowSaveAndNew <> @showSaveAndNew
				exec addUpdateEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, '3F3E0CDD-986B-44A4-A87A-CA15221F7E52', @oldshowSaveAndNew, @showSaveAndNew, @recordtitle, null;
			
			if @oldsaveAndNewButtonText <> @saveAndNewButtonText
				exec addUpdateEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, 'DAAE2384-0C39-4891-A110-AC5935D47C53', @oldsaveAndNewButtonText, @saveAndNewButtonText, @recordtitle, null;
				
			if @oldshowSaveAndStay <> @showSaveAndStay
				exec addUpdateEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, '163F97B0-6721-4E75-B3B5-FE490CCE3DFD', @oldshowSaveAndStay, @showSaveAndStay, @recordtitle, null;
				
			if @oldsaveAndStayButtonText <> @saveAndStayButtonText
				exec addUpdateEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, 'AE24B773-6717-4B1C-B65F-E3992EA1B56A', @oldsaveAndStayButtonText, @saveAndStayButtonText, @recordtitle, null;
				
			if @oldshowCancel <> @showCancel
				exec addUpdateEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, 'B0EA1C62-E03A-45CE-A16D-66A0AE62C4A9', @oldshowCancel, @showCancel, @recordtitle, null;
				
			if @oldcancelButtonText <> @cancelButtonText
				exec addUpdateEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, 'A93485AD-4414-497C-BDEA-274D333788FD', @oldcancelButtonText, @cancelButtonText, @recordtitle, null;
			
			if @oldshowBreadcrumbs <> @showBreadcrumbs
				exec addUpdateEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, '4C758581-D659-4F71-816F-F7A7EEA394BC', @oldshowBreadcrumbs, @showBreadcrumbs, @recordtitle, null;
				
			if @oldshowSubMenus <> @showSubMenus
				exec addUpdateEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, '6421999A-C83F-48EA-896B-06925FFAAB7E', @oldshowSubMenus, @showSubMenus, @recordtitle, null;
											
			UPDATE [custom_entity_forms] SET 
			[form_name] = @formname, 
			[description] = @description, 
			showSave = @showSave,
			saveButtonText = @saveButtonText,
			showSaveAndNew = @showSaveAndNew,
			saveAndNewButtonText = @saveAndNewButtonText,
			showSaveAndStay = @showSaveAndStay,
			saveAndStayButtonText = @saveAndStayButtonText,
			showCancel = @showCancel,
			cancelButtonText = @cancelButtonText,
			showBreadcrumbs = @showBreadcrumbs,
			showSubMenus = @showSubMenus,
			modifiedon = @date, 
			modifiedby = @CUemployeeID 
			WHERE formid = @formid;
		END

	RETURN @formid;	
END
