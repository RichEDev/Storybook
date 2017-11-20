CREATE TABLE [dbo].[mileage_dateranges] (
    [mileagedateid] INT      IDENTITY (1, 1)  NOT NULL,
    [mileageid]     INT      NOT NULL,
    [startdate]     DATETIME NULL,
    [enddate]       DATETIME NULL,
    [CreatedOn]     DATETIME NULL,
    [CreatedBy]     INT      NULL,
    [ModifiedOn]    DATETIME NULL,
    [ModifiedBy]    INT      NULL,
    [datevalue1]    DATETIME NULL,
    [datevalue2]    DATETIME NULL,
    [daterangetype] TINYINT  NULL
);

