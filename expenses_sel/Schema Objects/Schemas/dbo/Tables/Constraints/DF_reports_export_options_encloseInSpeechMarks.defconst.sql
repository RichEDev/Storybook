ALTER TABLE [dbo].[reports_export_options]
    ADD CONSTRAINT [DF_reports_export_options_encloseInSpeechMarks] DEFAULT ((1)) FOR [encloseInSpeechMarks];

