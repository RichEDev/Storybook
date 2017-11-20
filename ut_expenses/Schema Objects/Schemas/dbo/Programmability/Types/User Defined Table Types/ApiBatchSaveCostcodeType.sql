CREATE TYPE [dbo].[ApiBatchSaveCostcodeType] AS TABLE (
    [costcodeid]  INT            NULL,
    [costcode]    NVARCHAR (500) NULL,
    [description] NVARCHAR (500) NULL,
    [archived]    BIT            NULL,
    [CreatedOn]   DATETIME       NULL,
    [CreatedBy]   DATETIME       NULL,
    [ModifiedOn]  DATETIME       NULL,
    [ModifiedBy]  DATETIME       NULL);

