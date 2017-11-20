ALTER TABLE [dbo].[link_matrix]
    ADD CONSTRAINT [DF_Link_Matrix_LinkId] DEFAULT ((0)) FOR [linkId];

