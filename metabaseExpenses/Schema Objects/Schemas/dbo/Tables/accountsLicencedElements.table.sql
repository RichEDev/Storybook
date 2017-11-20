CREATE TABLE [dbo].[accountsLicencedElements] (
    [accountID] INT NOT NULL,
    [elementID] INT NOT NULL,
    CONSTRAINT [PK_accountsLicencedElements] PRIMARY KEY CLUSTERED ([accountID] ASC, [elementID] ASC),
    CONSTRAINT [FK_registeredusers_licenced_elements_elements_base] FOREIGN KEY ([elementID]) REFERENCES [dbo].[elementsBase] ([elementID]) NOT FOR REPLICATION,
    CONSTRAINT [FK_registeredusers_licenced_elements_registeredusers] FOREIGN KEY ([accountID]) REFERENCES [dbo].[registeredusers] ([accountid]) ON DELETE CASCADE
);



