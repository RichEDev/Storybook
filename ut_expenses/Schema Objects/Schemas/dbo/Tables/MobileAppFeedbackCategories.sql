CREATE TABLE [dbo].[MobileAppFeedbackCategories]
(
    [CategoryId] [int] IDENTITY(1,1) NOT NULL,
    [Description] NVARCHAR(50) NOT NULL, 
    [Active] BIT NOT NULL, 
    CONSTRAINT [PK_MobileAppFeedbackCategories] PRIMARY KEY ([CategoryId])
)
