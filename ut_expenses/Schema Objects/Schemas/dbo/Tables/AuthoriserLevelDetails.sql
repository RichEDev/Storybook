CREATE TABLE [dbo].[AuthoriserLevelDetails](
	[AuthoriserLevelDetailId] [int] IDENTITY(1,1) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_AuthoriserLevelDetail] PRIMARY KEY CLUSTERED 
(
	[AuthoriserLevelDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
