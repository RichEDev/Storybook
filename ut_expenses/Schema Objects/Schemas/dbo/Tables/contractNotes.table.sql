CREATE TABLE [dbo].[contractNotes] (
    [noteId]     INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [contractID] INT            NULL,
    [Code]       INT            NULL,
    [Note]       NVARCHAR (MAX) NULL,
    [Date]       DATETIME       NULL,
    [noteType]   INT            NULL,
    [noteCatId]  INT            NULL,
    [createdBy]  INT            NULL,
    [modifiedBy] INT            NULL,
    [modifiedOn] DATETIME       NULL,
    CONSTRAINT [PK_Contract_Notes] PRIMARY KEY CLUSTERED ([noteId] ASC),
    CONSTRAINT [FK_contract_notes_contract_details] FOREIGN KEY ([contractID]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE CASCADE,
    CONSTRAINT [FK_contractnotes_employees] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[employees] ([employeeid])
);



