ALTER TABLE [dbo].[information_messages]
    ADD CONSTRAINT [DF_information_messages_deleted] DEFAULT ((0)) FOR [deleted];

