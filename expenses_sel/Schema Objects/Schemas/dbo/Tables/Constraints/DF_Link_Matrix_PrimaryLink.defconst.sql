ALTER TABLE [dbo].[link_matrix]
    ADD CONSTRAINT [DF_Link_Matrix_PrimaryLink] DEFAULT ((0)) FOR [primaryLink];

