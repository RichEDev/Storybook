-- <Migration ID="3f02ead3-87ea-46de-8f71-b2ab32a59d97" />
GO
SET IDENTITY_INSERT [dbo].[SupportQuestionHeadings] ON 
GO
INSERT [dbo].[SupportQuestionHeadings] ([SupportQuestionHeadingId], [Heading], [ModuleId], [Order]) VALUES (1, N'I am unsure how to use Expenses', 2, 0)
GO
INSERT [dbo].[SupportQuestionHeadings] ([SupportQuestionHeadingId], [Heading], [ModuleId], [Order]) VALUES (2, N'I have a question about my expense policy', 2, 10)
GO
INSERT [dbo].[SupportQuestionHeadings] ([SupportQuestionHeadingId], [Heading], [ModuleId], [Order]) VALUES (3, N'My details are incorrect', 2, 20)
GO
INSERT [dbo].[SupportQuestionHeadings] ([SupportQuestionHeadingId], [Heading], [ModuleId], [Order]) VALUES (5, N'I am unsure how to use Framework', 3, 1)
GO
INSERT [dbo].[SupportQuestionHeadings] ([SupportQuestionHeadingId], [Heading], [ModuleId], [Order]) VALUES (6, N'My details are incorrect', 3, 10)
GO
SET IDENTITY_INSERT [dbo].[SupportQuestionHeadings] OFF
GO
