ALTER TABLE [dbo].[link_matrix]
    ADD CONSTRAINT [DF_Link_Matrix_IsArchived] DEFAULT ((0)) FOR [isArchived];

