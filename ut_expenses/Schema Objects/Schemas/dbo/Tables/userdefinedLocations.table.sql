CREATE TABLE [dbo].[userdefinedLocations] (
    [locationid] INT             NOT NULL,
    [udf548]     NVARCHAR (60)   NULL,
    [udf549]     NVARCHAR (4000) NULL,
    [udf552]     MONEY           NULL,
    [udf555]     INT             NULL,
    CONSTRAINT [PK_userdefinedLocations] PRIMARY KEY CLUSTERED ([locationid] ASC),
    CONSTRAINT [FK_userdefinedLocations_companies] FOREIGN KEY ([locationid]) REFERENCES [dbo].[companies] ([companyid]) ON DELETE CASCADE
);



