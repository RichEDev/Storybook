CREATE PROCEDURE [dbo].[SaveSignoffStage]
	@signoffid						INT			= null,
	@groupid						INT			= null,
	@signofftype					TINYINT,
	@relid							INT,
	@extraApprovalLevels			INT,
	@include						INT,
	@amount							MONEY,
	@notify							INT,
	@onholiday						TINYINT,
	@holidaytype					INT,
	@holidayid						INT,
	@stage							TINYINT		= null,
	@includeid						INT,
	@claimantmail					BIT,
	@singlesignoff					BIT,
	@sendmail						BIT,
	@displaydeclaration				BIT,
	@createdon						DATETIME	= null,
	@createdby						INT			= null,
	@modifiedon						DATETIME	= null,
	@modifiedby						INT			= null,
	@approveHigherLevelsOnly		INT,
	@approverJustificationsRequired TINYINT,
	@nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner BIT,
	@AllocateForPayment				BIT,
	@IsPostValidationCleanupStage	BIT			= null,
	@ValidationCorrectionThreshold	INT
AS

	if @signoffid is null
	begin
		insert signoffs
		(
			groupid, signofftype, relid, extraApprovalLevels, [include], amount, notify, onholiday, holidaytype, holidayid, stage,
			includeid, claimantmail, singlesignoff, sendmail, displaydeclaration, createdon, createdby, approveHigherLevelsOnly,
			nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner, approverJustificationsRequired, AllocateForPayment, IsPostValidationCleanupStage, ValidationCorrectionThreshold
		)
		values
		(
			@groupid, @signofftype, @relid, @extraApprovalLevels, @include, @amount, @notify, @onholiday, @holidaytype, @holidayid, @stage,
			@includeid, @claimantmail, @singlesignoff, @sendmail, @displaydeclaration, @createdon, @createdby, @approveHigherLevelsOnly,
			@nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner, @approverJustificationsRequired, @AllocateForPayment, @IsPostValidationCleanupStage, @ValidationCorrectionThreshold
		);

		exec dbo.UpdateStageNumbers @groupid;

	end
	else
		update
			signoffs
		set
			signofftype = @signofftype,
			relid = @relid,
			extraApprovalLevels = @extraApprovalLevels,
			[include] = @include,
			amount = @amount,
			notify = @notify,
			onholiday = @onholiday,
			holidaytype = @holidaytype,
			holidayid = @holidayid,
			includeid = @includeid,
			claimantmail = @claimantmail,
			singlesignoff = @singlesignoff,
			sendmail = @sendmail,
			displaydeclaration = @displaydeclaration,
			modifiedon = @modifiedon,
			modifiedby = @modifiedby,
			approveHigherLevelsOnly = @approveHigherLevelsOnly,
			approverJustificationsRequired = @approverJustificationsRequired,
			nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner = @nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner,
			AllocateForPayment = @AllocateForPayment,
			ValidationCorrectionThreshold = @ValidationCorrectionThreshold
		where
			signoffid = @signoffid;

RETURN 0;
