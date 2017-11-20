CREATE TABLE [dbo].[moduleLicencesBase] (
    [moduleID]   INT      NOT NULL,
    [accountID]  INT      NOT NULL,
    [expiryDate] DATETIME NULL,
    [maxUsers]   INT      CONSTRAINT [DF_module_licences_maxUers] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_moduleLicencesBase] PRIMARY KEY CLUSTERED ([moduleID] ASC, [accountID] ASC),
    CONSTRAINT [FK_module_licences_base_module_base] FOREIGN KEY ([moduleID]) REFERENCES [dbo].[moduleBase] ([moduleID]) ON DELETE CASCADE ON UPDATE CASCADE NOT FOR REPLICATION,
    CONSTRAINT [FK_module_licences_base_registeredusers] FOREIGN KEY ([accountID]) REFERENCES [dbo].[registeredusers] ([accountid]) ON DELETE CASCADE ON UPDATE CASCADE
);



