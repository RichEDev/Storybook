CREATE TABLE [EnvelopePhysicalState] 
(
   EnvelopePhysicalStateId int NOT NULL constraint PK_EnvelopePhysicalState Primary Key IDENTITY(1,1),
   Details nvarchar(100) NOT NULL
)
GO