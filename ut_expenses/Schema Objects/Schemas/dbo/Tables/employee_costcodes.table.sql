CREATE TABLE [dbo].[employee_costcodes] (
    [employeecostcodeid] INT IDENTITY (1, 1)  NOT NULL,
    [employeeid]         INT NOT NULL,
    [departmentid]       INT NULL,
    [costcodeid]         INT NULL,
    [projectcodeid]      INT NULL,
    [percentused]        INT NOT NULL
);

