ALTER TABLE [dbo].[userdefined_list_items]
    ADD CONSTRAINT [PK_userdefined_list_items] PRIMARY KEY CLUSTERED ([userdefineid] ASC, [item] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

