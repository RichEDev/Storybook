
CREATE TABLE [dbo].[LogonMessages](
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryTitle] [nvarchar](80) NULL,
	[CategoryTitleColourCode] [nvarchar](50) NOT NULL,
	[HeaderText] [nvarchar](100) NULL,
	[HeaderTextColourCode] [nvarchar](50) NULL,
	[BodyText] [nvarchar](400) NULL,
	[BodyTextColourCode] [nvarchar](50) NULL,
	[BackgroundImage] [nvarchar](max) NULL,
	[Icon] [nvarchar](max) NULL,
	[ButtonText] [nvarchar](50) NULL,
	[ButtonLink] [nvarchar](200) NULL,
	[ButtonForeColour] [nvarchar](50) NULL,
	[ButtonBackGroundColour] [nvarchar](50) NULL,
	[Archived] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_LogonMessages] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
