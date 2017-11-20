CREATE TRIGGER EnvelopeInsert ON [dbo].[Envelopes] FOR INSERT
AS
SET CONCAT_NULL_YIELDS_NULL OFF;
	insert into EnvelopeHistory (EnvelopeId, EnvelopeStatus, Data, ModifiedBy, ModifiedOn)
	select	i.EnvelopeId, 1, ('Generated: ' + i.EnvelopeNumber + ', Type: ' + convert(varchar, i.EnvelopeType) + ', AccountId: ' + convert(varchar, i.AccountId)), i.LastModifiedBy, GETUTCDATE()
	from inserted i;
SET CONCAT_NULL_YIELDS_NULL ON;	