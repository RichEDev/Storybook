CREATE TABLE [dbo].[userdefined_employees] (
    [employeeid] INT             NOT NULL,
    [udf529]     NVARCHAR (50)   NULL,
    [udf537]     NVARCHAR (4000) NULL,
    [udf550]     DATETIME        NULL,
    [udf551]     NVARCHAR (MAX)  NULL,
    [udf553]     INT             NULL,
    [udf554]     INT             NULL,
    [udf556]     INT             NULL,
    [udf557]     NVARCHAR (20)   NULL,
    [udf558]     INT             NULL,
    [udf559]     INT             NULL,
    [udf561]     NVARCHAR (50)   NULL,
    CONSTRAINT [PK_userdefined_employees] PRIMARY KEY CLUSTERED ([employeeid] ASC),
    CONSTRAINT [FK_userdefined_employees_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE
);



