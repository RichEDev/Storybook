
CREATE TABLE [dbo].[CustomEntityAttributeLookupSearchResultFields](
	[LookupSearchResultFieldId] [int] IDENTITY(1,1) NOT NULL,
	[attributeId] [int] NOT NULL,
	[fieldId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CustomEntityAttributeLookupSearchResultFields] PRIMARY KEY CLUSTERED 
(
	[LookupSearchResultFieldId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[CustomEntityAttributeLookupSearchResultFields]  WITH CHECK ADD  CONSTRAINT [FK_CustomEntityAttributeLookupSearchResultFields_customEntityAttributes] FOREIGN KEY([attributeId])
REFERENCES [dbo].[customEntityAttributes] ([attributeid])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CustomEntityAttributeLookupSearchResultFields] CHECK CONSTRAINT [FK_CustomEntityAttributeLookupSearchResultFields_customEntityAttributes]


