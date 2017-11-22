CREATE TABLE [dbo].[EmployeeMobileAppReviewPreferences]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[NeverPromptForReview] [bit] NOT NULL, 
    CONSTRAINT [PK_EmployeeMobileAppReviewPreferences] PRIMARY KEY ([Id]),
)

