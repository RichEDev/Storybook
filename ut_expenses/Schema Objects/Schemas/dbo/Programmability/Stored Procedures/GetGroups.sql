CREATE PROCEDURE [dbo].[GetGroups]
AS
	select groupid, [description], groupname, oneClickAuthorisation, createdon, createdby, modifiedon, modifiedby, MailClaimantWhenEnvelopeReceived, MailClaimantWhenEnvelopeNotReceived from dbo.groups
RETURN 0
