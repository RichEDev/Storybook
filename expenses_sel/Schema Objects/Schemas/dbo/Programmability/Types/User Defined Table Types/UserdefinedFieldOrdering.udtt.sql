CREATE TYPE [dbo].[UserdefinedFieldOrdering] AS  TABLE (
    [userdefinedFieldID] INT NOT NULL,
    [displayOrder]       INT NOT NULL,
    PRIMARY KEY CLUSTERED ([userdefinedFieldID] ASC));

