ALTER TABLE [dbo].[scheduled_reports_columns]
    ADD CONSTRAINT [PK_scheduled_reports_columns_1] PRIMARY KEY CLUSTERED ([scheduleid] ASC, [reportcolumnid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

