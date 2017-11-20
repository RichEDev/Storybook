
CREATE PROCEDURE [dbo].[saveAccessRole]
	
	@accessRoleID int,
	@accessRoleName nvarchar(250),
	@description nvarchar(4000),
	@roleAccessLevel tinyint,
	@expenseClaimMaximumAmount decimal,  
	@expenseClaimMinimumAmount decimal, 
	@employeesCanAmendDesignatedCostCode bit, 
	@employeesCanAmendDesignatedDepartment bit, 
	@employeesCanAmendDesignatedProjectCode bit,
	@employeesMustHaveBankAccount bit,
	@allowWebsiteAccess bit,
	@allowMobileAccess bit,
	@allowApiAccess bit,
	@employeeID int,
	@CUdelegateID INT,
	@exclusionType INT = 1
AS 
	DECLARE @count INT;
	DECLARE @date DateTime;
	SET @date = getdate();
	IF (@accessRoleID = 0)
		BEGIN
			SET @count = (SELECT COUNT(*) FROM accessRoles WHERE roleName = @accessRoleName);
		
			IF @count > 0
				RETURN -1;

			INSERT INTO accessRoles (roleName, description, createdOn, createdBy, roleAccessLevel, expenseClaimMaximumAmount, expenseClaimMinimumAmount, employeesCanAmendDesignatedCostCode, employeesCanAmendDesignatedDepartment, employeesCanAmendDesignatedProjectCode, employeesMustHaveBankAccount, allowWebsiteAccess, allowMobileAccess, allowApiAccess, ExclusionType) VALUES (@accessRoleName, @description, @date, @employeeID, @roleAccessLevel, @expenseClaimMaximumAmount, @expenseClaimMinimumAmount, @employeesCanAmendDesignatedCostCode, @employeesCanAmendDesignatedDepartment, @employeesCanAmendDesignatedProjectCode, @employeesMustHaveBankAccount, @allowWebsiteAccess, @allowMobileAccess, @allowApiAccess, @exclusionType);
			SET @accessRoleID = scope_identity();

			if @employeeID > 0
			BEGIN
				exec addInsertEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, @accessRoleName, null;
			END
		END
	ELSE
		BEGIN

			SET @count = (SELECT COUNT(*) FROM accessRoles WHERE roleName = @accessRoleName AND roleID <> @accessRoleID);
			
			IF @count > 0
				RETURN -1;

			declare @oldaccessRoleName nvarchar(250);
			declare @olddescription nvarchar(4000);
			declare @oldroleAccessLevel tinyint;
			declare @oldexpenseClaimMaximumAmount decimal;
			declare @oldexpenseClaimMinimumAmount decimal;
			declare @oldemployeesCanAmendDesignatedCostCode bit;
			declare @oldemployeesCanAmendDesignatedDepartment bit;
			declare @oldemployeesCanAmendDesignatedProjectCode bit;
			declare @oldemployeesMustHaveBankAccount bit;
			declare @oldAllowWebsiteAccess bit;
			declare @oldAllowMobileAccess bit;
			declare @oldAllowApiAccess bit;
			declare @oldExclusionType INT;
			declare @newExclusionTypeForAuditLog nvarchar(20);
			declare @oldExclusionTypeForAuditLog nvarchar(20);
			select @oldaccessRoleName = roleName, @olddescription = description, @oldroleAccessLevel = roleAccessLevel, @oldexpenseClaimMaximumAmount = expenseClaimMaximumAmount, @oldexpenseClaimMinimumAmount = expenseClaimMinimumAmount, @oldemployeesCanAmendDesignatedCostCode = employeesCanAmendDesignatedCostCode, @oldemployeesCanAmendDesignatedDepartment = employeesCanAmendDesignatedDepartment, @oldemployeesCanAmendDesignatedProjectCode = employeesCanAmendDesignatedProjectCode, @oldemployeesMustHaveBankAccount = employeesMustHaveBankAccount, @oldAllowWebsiteAccess = allowWebsiteAccess, @oldAllowMobileAccess = allowMobileAccess, @oldAllowApiAccess = allowApiAccess, @oldExclusionType = exclusiontype    from accessRoles WHERE roleID=@accessRoleID;

			UPDATE accessRoles SET roleName=@accessRoleName, description=@description, modifiedOn=@date, modifiedBy=@employeeID, roleAccessLevel=@roleAccessLevel,expenseClaimMaximumAmount=@expenseClaimMaximumAmount, expenseClaimMinimumAmount=@expenseClaimMinimumAmount, employeesCanAmendDesignatedCostCode=@employeesCanAmendDesignatedCostCode, employeesCanAmendDesignatedDepartment=@employeesCanAmendDesignatedDepartment, employeesCanAmendDesignatedProjectCode=@employeesCanAmendDesignatedProjectCode, employeesMustHaveBankAccount = @employeesMustHaveBankAccount, allowWebsiteAccess = @allowWebsiteAccess, allowMobileAccess = @allowMobileAccess, allowApiAccess=@allowApiAccess, exclusiontype= @exclusionType   WHERE roleID=@accessRoleID;

			SET @oldExclusionTypeForAuditLog = CASE @oldExclusionType
 WHEN 2 THEN 'Include All' 
 WHEN 1 THEN 'Include Selected' 
 WHEN 0 THEN 'Exclude Selected' 
 ELSE NULL
 END

 			SET @newExclusionTypeForAuditLog = CASE @exclusionType
 WHEN 2 THEN 'Include All' 
 WHEN 1 THEN 'Include Selected' 
 WHEN 0 THEN 'Exclude Selected' 
 ELSE NULL
 END

			if @employeeID > 0
			BEGIN
				if @oldaccessRoleName <> @accessRoleName
					exec addUpdateEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, '735a4159-090b-420d-80c4-57987422380c', @oldaccessRoleName, @accessRoleName, @accessRoleName, null;
				if @olddescription <> @description
					exec addUpdateEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, '0ed6481d-e3ff-49a1-b95f-f8a0ca0951f4', @olddescription, @description, @accessRoleName, null;
				if @oldroleAccessLevel <> @roleAccessLevel
					exec addUpdateEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, '632dbb9d-0b15-430a-83eb-aa7396cecdb8', @oldroleAccessLevel, @roleAccessLevel, @accessRoleName, null;
				if ISNULL(@oldexpenseClaimMaximumAmount, 0) <> @expenseClaimMaximumAmount
					exec addUpdateEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, '13014990-baf3-4757-99c5-5bc0e0c7648b', @oldexpenseClaimMaximumAmount, @expenseClaimMaximumAmount, @accessRoleName, null;
				if ISNULL(@oldexpenseClaimMinimumAmount, 0) <> @expenseClaimMinimumAmount
					exec addUpdateEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, '6dd81def-9d69-4832-8f88-0f2500eb4475', @oldexpenseClaimMinimumAmount, @expenseClaimMinimumAmount, @accessRoleName, null;
				if @oldemployeesCanAmendDesignatedCostCode <> @employeesCanAmendDesignatedCostCode
					exec addUpdateEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, 'c7535335-8c4c-49e4-8d5c-703b3a9bbcd2', @oldemployeesCanAmendDesignatedCostCode, @employeesCanAmendDesignatedCostCode, @accessRoleName, null;
				if @oldemployeesCanAmendDesignatedDepartment <> @employeesCanAmendDesignatedDepartment
					exec addUpdateEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, 'd4632a1a-97d0-4233-93d7-1f19a101c87a', @oldemployeesCanAmendDesignatedDepartment, @employeesCanAmendDesignatedDepartment, @accessRoleName, null;
				if @oldemployeesCanAmendDesignatedProjectCode <> @employeesCanAmendDesignatedProjectCode
					exec addUpdateEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, '891d99b6-902a-41ac-aa84-675a058a4528', @oldemployeesCanAmendDesignatedProjectCode, @employeesCanAmendDesignatedProjectCode, @accessRoleName, null;
	            if @oldemployeesMustHaveBankAccount <> @employeesMustHaveBankAccount
					exec addUpdateEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, 'AAFFDAAB-BB13-4783-BFA6-6CC2CB53A425', @oldemployeesMustHaveBankAccount, @employeesMustHaveBankAccount, @accessRoleName, null;				
				if @oldAllowWebsiteAccess <> @allowWebsiteAccess
					exec addUpdateEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, 'B4A9ED4B-8DAB-4597-BC5A-F3BCE8088584', @oldAllowWebsiteAccess, @AllowWebsiteAccess, @accessRoleName, null;
				if @oldAllowMobileAccess <> @allowMobileAccess
					exec addUpdateEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, '589EE013-459F-49C4-A38B-87397A780164', @oldAllowMobileAccess, @AllowMobileAccess, @accessRoleName, null;
				if @oldAllowApiAccess <> @allowApiAccess
					exec addUpdateEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, '7EBD53C6-9177-49B7-B6AD-96E4FEDC566D', @oldAllowApiAccess, @AllowApiAccess, @accessRoleName, null;	
					if @oldExclusionType <> @exclusionType
					exec addUpdateEntryToAuditLog @employeeID, @CUdelegateID, 6, @accessRoleID, '9B637872-5777-4658-A600-554B06698002', @oldExclusionTypeForAuditLog, @newExclusionTypeForAuditLog, @accessRoleName, null;				
			END
		END
RETURN @accessRoleID;

