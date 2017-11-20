CREATE TABLE [dbo].[SupportQuestions](
	[SupportQuestionId] [int] IDENTITY(1,1) NOT NULL,
	[Question] [nvarchar](250) NOT NULL,
	[KnowledgeArticleUrl] [nvarchar](1000) NULL,
	[SupportTicketSel] [bit] NOT NULL,
	[SupportTicketInternal] [bit] NOT NULL,
	[SupportQuestionHeadingId] [int] NOT NULL,
 CONSTRAINT [PK_SupportQuestions] PRIMARY KEY CLUSTERED 
(
	[SupportQuestionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SupportQuestions]  WITH CHECK ADD  CONSTRAINT [FK_SupportQuestions_SupportQuestionHeadings] FOREIGN KEY([SupportQuestionHeadingId])
REFERENCES [dbo].[SupportQuestionHeadings] ([SupportQuestionHeadingId])
GO

ALTER TABLE [dbo].[SupportQuestions] CHECK CONSTRAINT [FK_SupportQuestions_SupportQuestionHeadings]
GO

ALTER TABLE [dbo].[SupportQuestions]  WITH CHECK ADD  CONSTRAINT [CK_SupportQuestion] CHECK (
([SupportTicketSel]=(1) AND [SupportTicketInternal]=(0) AND [KnowledgeArticleUrl] IS NULL 
OR [SupportTicketSel]=(0) AND [SupportTicketInternal]=(1) AND [KnowledgeArticleUrl] IS NULL 
OR [SupportTicketSel]=(0) AND [SupportTicketInternal]=(0) AND [KnowledgeArticleUrl] IS NOT NULL)
)
GO

ALTER TABLE [dbo].[SupportQuestions] CHECK CONSTRAINT [CK_SupportQuestion]
GO

ALTER TABLE [dbo].[SupportQuestions] ADD  CONSTRAINT [DF_SupportQuestions_SupportTicketSel]  DEFAULT ((0)) FOR [SupportTicketSel]
GO

ALTER TABLE [dbo].[SupportQuestions] ADD  CONSTRAINT [DF_SupportQuestions_SupportTicketInternal]  DEFAULT ((0)) FOR [SupportTicketInternal]
GO