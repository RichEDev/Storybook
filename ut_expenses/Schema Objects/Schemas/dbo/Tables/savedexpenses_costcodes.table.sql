CREATE TABLE [dbo].[savedexpenses_costcodes] (
    [savedcostcodeid] INT IDENTITY (1, 1)  NOT NULL,
    [expenseid]       INT NOT NULL,
    [departmentid]    INT NULL,
    [costcodeid]      INT NULL,
    [percentused]     INT NOT NULL,
    [projectcodeid]   INT NULL
);

