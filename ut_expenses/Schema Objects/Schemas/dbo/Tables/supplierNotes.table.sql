CREATE TABLE [dbo].[supplierNotes] (
    [noteId]     INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [supplierid] INT            NULL,
    [Code]       INT            NULL,
    [Note]       NVARCHAR (MAX) NULL,
    [Date]       DATETIME       NULL,
    [noteType]   INT            NULL,
    [noteCatId]  INT            NULL,
    [createdBy]  INT            NULL,
    CONSTRAINT [PK_SupplierNotes] PRIMARY KEY CLUSTERED ([noteId] ASC),
    CONSTRAINT [FK_suppliernotes_employees] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[employees] ([employeeid]),
    CONSTRAINT [FK_suppliernotes_supplier_details] FOREIGN KEY ([supplierid]) REFERENCES [dbo].[supplier_details] ([supplierid]) ON DELETE CASCADE
);



