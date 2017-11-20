CREATE TABLE [dbo].[sysdiagrams] (
    [name]         [sysname]       COLLATE Latin1_General_CI_AS NOT NULL,
    [principal_id] INT             NOT NULL,
    [diagram_id]   INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [version]      INT             NULL,
    [definition]   VARBINARY (MAX) NULL,
    CONSTRAINT [PK__sysdiagrams__17C81630] PRIMARY KEY CLUSTERED ([diagram_id] ASC),
    CONSTRAINT [UK_principal_name] UNIQUE NONCLUSTERED ([principal_id] ASC, [name] ASC)
);



