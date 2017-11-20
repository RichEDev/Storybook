ALTER TABLE [dbo].[employee_costcodes]
    ADD CONSTRAINT [DF_employee_costcodes_percentused] DEFAULT (0) FOR [percentused];

