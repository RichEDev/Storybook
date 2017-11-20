CREATE PROCEDURE [dbo].[UpdateStageNumbers]
	@groupId int
AS

	WITH StageNos AS
	(
		SELECT
			signoffid,
			ROW_NUMBER() OVER (ORDER BY IsPostValidationCleanupStage, stage) AS stage
		FROM
			signoffs
		WHERE
			groupid = @groupId
	)
	UPDATE
		signoffs
	SET
		signoffs.stage = StageNos.stage
	FROM
		signoffs
			JOIN StageNos ON StageNos.signoffid = signoffs.signoffid;

RETURN 0;
