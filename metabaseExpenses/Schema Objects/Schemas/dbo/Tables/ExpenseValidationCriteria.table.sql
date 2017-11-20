CREATE TABLE [dbo].[ExpenseValidationCriteria]
(
   [CriterionId]			INT					NOT NULL	constraint PK_ExpenseValidationCriteria Primary Key IDENTITY(1,1),
   [AccountId]				INT					NULL		constraint FK_ExpenseValidationCriteria_RegisteredUsers  FOREIGN KEY REFERENCES registeredUsers(accountId),
   [FieldId]				UNIQUEIDENTIFIER	NULL		constraint FK_ExpenseValidationCriteria_FieldsBase FOREIGN KEY REFERENCES fields_base(fieldid),
   [SubcatId]				INT					NULL,
   [Requirements]			nvarchar(400)		NOT NULL,
   [Enabled]				bit					NOT NULL default(1),
   [FraudulentIfFailsVAT]	bit					NOT NULL default(0),
   [FriendlyMessageFoundAndMatched] nvarchar(200) NOT NULL,
   [FriendlyMessageFoundNotMatched] nvarchar(200) NOT NULL, 
   [FriendlyMessageFoundNotReadable] nvarchar(200) NOT NULL, 
   [FriendlyMessageNotFound] nvarchar(200) NOT NULL
)
GO
create index IX_ExpenseValidationCriterion_Requirements_FreeText on [dbo].[ExpenseValidationCriteria] (Requirements)
GO