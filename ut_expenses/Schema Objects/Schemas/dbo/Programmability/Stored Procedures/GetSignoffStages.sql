CREATE PROCEDURE [dbo].[GetSignoffStages]
AS
	SELECT
		amount, holidayid, holidaytype, [include], notify, onholiday, relid, extraApprovalLevels, signoffid, signofftype, stage, claimantmail,
		includeid, singlesignoff, sendmail, displaydeclaration, createdon, createdby, modifiedon, modifiedby, groupid, approveHigherLevelsOnly,approverJustificationsRequired,
		nhsassignmentsupervisorapproveswhenmissingcostcodeowner, AllocateForPayment, IsPostValidationCleanupStage, ValidationCorrectionThreshold, ClaimPercentageToValidate
	FROM
		signoffs
	ORDER BY
		groupid, stage;

RETURN 0;