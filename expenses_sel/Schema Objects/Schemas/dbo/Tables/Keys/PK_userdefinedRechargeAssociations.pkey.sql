ALTER TABLE [dbo].[userdefinedRechargeAssociations]
    ADD CONSTRAINT [PK_userdefinedRechargeAssociations] PRIMARY KEY CLUSTERED ([rechargeid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

