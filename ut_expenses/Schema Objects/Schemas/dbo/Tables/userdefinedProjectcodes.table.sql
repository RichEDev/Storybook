CREATE TABLE [dbo].[userdefinedProjectcodes] (
    [projectcodeid] INT           NOT NULL,
    [udf526]        NVARCHAR (2)  NULL,
    [udf533]        NVARCHAR (50) NULL,
    CONSTRAINT [PK_userdefinedProjectcodes] PRIMARY KEY CLUSTERED ([projectcodeid] ASC),
    CONSTRAINT [FK_userdefinedProjectcodes_project_codes] FOREIGN KEY ([projectcodeid]) REFERENCES [dbo].[project_codes] ([projectcodeid]) ON DELETE CASCADE
);



