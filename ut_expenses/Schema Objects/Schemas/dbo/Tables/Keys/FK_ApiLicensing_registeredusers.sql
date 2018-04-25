ALTER TABLE [dbo].[ApiLicensing]  WITH CHECK ADD  CONSTRAINT [FK__ApiLicensing__registeredusers] FOREIGN KEY([AccountId])
REFERENCES [dbo].[registeredusers] ([accountid])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[ApiLicensing] CHECK CONSTRAINT [FK__ApiLicensing__registeredusers]