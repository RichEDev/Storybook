CREATE TABLE [EnvelopeHistory] (
   EnvelopeHistoryId int NOT NULL constraint PK_EnvelopeHistory Primary Key IDENTITY(1,1),
   EnvelopeId int constraint FK_EnvelopeHistory_Envelopes Foreign Key REFERENCES dbo.Envelopes NULL,
   EnvelopeStatus int NOT NULL,
   Data nvarchar(500) NULL,
   ModifiedOn DateTime NOT NULL,
   ModifiedBy int NOT NULL )
