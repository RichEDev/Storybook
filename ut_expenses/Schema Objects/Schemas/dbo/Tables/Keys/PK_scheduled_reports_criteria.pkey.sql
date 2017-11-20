ALTER TABLE [dbo].[scheduled_reports_criteria]
    ADD CONSTRAINT [PK_scheduled_reports_criteria] PRIMARY KEY CLUSTERED ([scheduleid] ASC, [criteriaid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

