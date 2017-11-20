
CREATE PROCEDURE [dbo].[APIgetEsrTrusts]
	@trustID int 
	
AS
BEGIN
IF	@trustID = 0
	BEGIN
		SELECT     esrTrusts.trustID, esrTrusts.trustVPD, esrTrusts.periodType, esrTrusts.periodRun, esrTrusts.runSequenceNumber, esrTrusts.ftpAddress, esrTrusts.ftpUsername, 
                      esrTrusts.ftpPassword, esrTrusts.archived, esrTrusts.createdOn, esrTrusts.modifiedOn, esrTrusts.trustName, esrTrusts.delimiterCharacter, 
                      esrTrusts.ESRVersionNumber, esrTrusts.currentOutboundSequence
		FROM         esrTrusts INNER JOIN
                      importTemplates ON esrTrusts.trustID = importTemplates.NHSTrustID
		WHERE     (esrTrusts.ESRVersionNumber = 2) AND (esrTrusts.archived = 0)
	END
ELSE
	BEGIN
		SELECT     esrTrusts.trustID, esrTrusts.trustVPD, esrTrusts.periodType, esrTrusts.periodRun, esrTrusts.runSequenceNumber, esrTrusts.ftpAddress, esrTrusts.ftpUsername, 
                      esrTrusts.ftpPassword, esrTrusts.archived, esrTrusts.createdOn, esrTrusts.modifiedOn, esrTrusts.trustName, esrTrusts.delimiterCharacter, 
                      esrTrusts.ESRVersionNumber, esrTrusts.currentOutboundSequence
		FROM         esrTrusts INNER JOIN
                      importTemplates ON esrTrusts.trustID = importTemplates.NHSTrustID
		WHERE     (esrTrusts.ESRVersionNumber = 2) AND (esrTrusts.archived = 0) AND (trustID = @trustID) 
	END
END