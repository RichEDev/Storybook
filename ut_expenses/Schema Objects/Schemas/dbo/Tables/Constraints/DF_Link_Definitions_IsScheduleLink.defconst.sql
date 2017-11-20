ALTER TABLE [dbo].[link_definitions]
    ADD CONSTRAINT [DF_Link_Definitions_IsScheduleLink] DEFAULT ((0)) FOR [isScheduleLink];

