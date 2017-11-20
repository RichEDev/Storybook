CREATE TABLE [EnvelopeTypes] (
   EnvelopeTypeId int NOT NULL constraint PK_EnvelopeTypes Primary Key IDENTITY(1,1),
   Label nvarchar(50) NOT NULL)