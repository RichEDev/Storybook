ALTER TABLE [dbo].[mobileAPITypes]
   ADD CONSTRAINT [DF_mobileAPITypes_API_TypeId] 
   DEFAULT (newid()) FOR [API_TypeId]


