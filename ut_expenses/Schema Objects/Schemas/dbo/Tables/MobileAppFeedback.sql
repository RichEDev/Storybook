CREATE TABLE [dbo].[MobileAppFeedback](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FeedbackCategoryId] [int] NOT NULL,
	[Feedback] [nvarchar](500) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[MobileMetricId] [int] NOT NULL,
	[AppVersion] [nvarchar](100) NOT NULL,
        [DateSubmitted] [DateTime] NOT NULL
 CONSTRAINT [PK_MobileAppFeedback] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]