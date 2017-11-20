
CREATE TABLE [dbo].[KnowledgeCustomArticles](
 [KnowledgeCustomArticleId] [int] IDENTITY(1,1) NOT NULL,
 [ProductCategory] [nvarchar](255) NOT NULL,
 [Title] [nvarchar](255) NOT NULL,
 [Summary] [nvarchar](255) NOT NULL,
 [Body] [nvarchar](max) NOT NULL,
 [Published] [bit] NOT NULL,
 [PublishedOn] [datetime] NULL,
 [CreatedBy] [int] NULL,
 [CreatedOn] [datetime] NULL,
 [ModifiedBy] [int] NULL,
 [ModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_KnowledgeCustomArticles] PRIMARY KEY CLUSTERED ([KnowledgeCustomArticleId] ASC)
)