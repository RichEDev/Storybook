ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_applicantactivestatusflag] DEFAULT 0 FOR [applicantactivestatusflag];

