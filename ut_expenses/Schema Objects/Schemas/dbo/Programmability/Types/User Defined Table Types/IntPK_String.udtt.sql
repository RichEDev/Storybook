CREATE TYPE [dbo].[IntPK_String] AS  TABLE (
    [c1] INT            NOT NULL,
    [c2] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([c1] ASC));

