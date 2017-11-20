CREATE TABLE [dbo].[SupportQuestionHeadings](
	[SupportQuestionHeadingId] [int] IDENTITY(1,1) NOT NULL,
	[Heading] [nvarchar](250) NOT NULL,
 [Order] INT NOT NULL, 
    CONSTRAINT [PK_SupportQuestionHeadings] PRIMARY KEY CLUSTERED 
(
	[SupportQuestionHeadingId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
) ON [PRIMARY]