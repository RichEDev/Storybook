CREATE TABLE [EnvelopesPhysicalStates] 
(
	Id int NOT NULL constraint PK_EnvelopesPhysicalStates Primary Key IDENTITY(1,1),
	EnvelopeId INT NOT NULL CONSTRAINT FK_EnvelopePhysicalStates_Envelopes FOREIGN KEY REFERENCES Envelopes (EnvelopeId),
	EnvelopePhysicalStateId INT NOT NULL CONSTRAINT FK_EnvelopePhysicalStates_EnvelopePhysicalState FOREIGN KEY REFERENCES EnvelopePhysicalState (EnvelopePhysicalStateId)
)
GO