CREATE TABLE [dbo].[userdefined_costcodes] (
    [costcodeid] INT           NOT NULL,
    [udf530]     NVARCHAR (50) NULL,
    CONSTRAINT [PK_userdefined_costcodes] PRIMARY KEY CLUSTERED ([costcodeid] ASC),
    CONSTRAINT [FK_userdefined_costcodes_costcodes] FOREIGN KEY ([costcodeid]) REFERENCES [dbo].[costcodes] ([costcodeid]) ON DELETE CASCADE
);



