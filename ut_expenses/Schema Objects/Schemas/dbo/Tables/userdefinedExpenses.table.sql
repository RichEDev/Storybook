CREATE TABLE [dbo].[userdefinedExpenses] (
    [expenseid] INT             NOT NULL,
    [udf508old] NVARCHAR (50)   NULL,
    [udf400]    NVARCHAR (4000) NULL,
    [udf492]    DECIMAL (18, 2) NULL,
    [udf520]    DECIMAL (18, 2) NULL,
    [udf508]    INT             NULL,
    [udf528]    NVARCHAR (50)   NULL,
    [udf534]    NVARCHAR (4000) NULL,
    [udf535]    NVARCHAR (4000) NULL,
    [udf536]    NVARCHAR (50)   NULL,
    [udf539]    NVARCHAR (4000) NULL,
    [udf541]    DECIMAL (18)    NULL,
    [udf543]    NVARCHAR (4000) NULL,
    CONSTRAINT [PK_userdefinedExpenses] PRIMARY KEY CLUSTERED ([expenseid] ASC)
);



