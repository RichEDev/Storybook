ALTER TABLE [dbo].[customEntityAttributeSummary]
    ADD CONSTRAINT [PK_customEntityAttributeSummary_summaryid] PRIMARY KEY CLUSTERED ([summary_attributeid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

