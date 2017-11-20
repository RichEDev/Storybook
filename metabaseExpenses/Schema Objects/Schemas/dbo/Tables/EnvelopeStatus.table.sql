CREATE TABLE [EnvelopeStatus] (
   EnvelopeStatusId int NOT NULL constraint PK_EnvelopeStatus Primary Key IDENTITY(1,1),
   Message nvarchar(200) NOT NULL)