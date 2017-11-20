CREATE TYPE [dbo].[UserdefinedGroupOrdering] AS  TABLE (
    [userdefinedGroupID] INT NOT NULL,
    [displayOrder]       INT NOT NULL,
    PRIMARY KEY CLUSTERED ([userdefinedGroupID] ASC));

