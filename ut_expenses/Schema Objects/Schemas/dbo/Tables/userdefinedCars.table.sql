CREATE TABLE [dbo].[userdefinedCars] (
    [carid]     INT             NOT NULL,
    [udf493]    DECIMAL (18, 2) NULL,
    [udf509old] NVARCHAR (50)   NULL,
    [udf525]    NVARCHAR (10)   NULL,
    [udf509]    INT             NULL,
    [udf532]    NVARCHAR (50)   NULL,
    CONSTRAINT [PK_userdefinedCars] PRIMARY KEY CLUSTERED ([carid] ASC),
    CONSTRAINT [FK_userdefinedCars_cars] FOREIGN KEY ([carid]) REFERENCES [dbo].[cars] ([carid]) ON DELETE CASCADE
);



