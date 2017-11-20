CREATE TABLE [dbo].[moduleBase] (
    [moduleID]      INT             IDENTITY (8, 1) NOT FOR REPLICATION NOT NULL,
    [moduleName]    NVARCHAR (100)  COLLATE Latin1_General_CI_AS NOT NULL,
    [description]   NVARCHAR (4000) COLLATE Latin1_General_CI_AS NULL,
    [brandName]     NVARCHAR (250)  CONSTRAINT [DF_moduleBase_brandName] DEFAULT (N'Brand Name') NOT NULL,
    [brandNameHTML] NVARCHAR (MAX)  CONSTRAINT [DF_moduleBase_brandNameHTML] DEFAULT (N'<strong>Brand Name</strong>') NOT NULL,
    CONSTRAINT [PK_module_base] PRIMARY KEY CLUSTERED ([moduleID] ASC)
);



