CREATE TABLE [dbo].[elementsBase] (
    [elementID]             INT             IDENTITY (168, 1) NOT FOR REPLICATION NOT NULL,
    [categoryID]            INT             NOT NULL,
    [elementName]           NVARCHAR (100)  COLLATE Latin1_General_CI_AS NOT NULL,
    [elementFriendlyName]   NVARCHAR (100)  COLLATE Latin1_General_CI_AS NULL,
    [description]           NVARCHAR (4000) COLLATE Latin1_General_CI_AS NULL,
    [accessRolesCanEdit]    BIT             CONSTRAINT [DF_elements_base_accessRolesCanEdit] DEFAULT ((1)) NULL,
    [accessRolesCanAdd]     BIT             CONSTRAINT [DF_elements_base_accessRolesCanAdd] DEFAULT ((1)) NULL,
    [accessRolesCanDelete]  BIT             CONSTRAINT [DF_elements_base_accessRolesCanDelete] DEFAULT ((1)) NULL,
    [accessRolesApplicable] BIT             CONSTRAINT [DF_elementsBase_accessRolesApplicable] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_elements_base] PRIMARY KEY CLUSTERED ([elementID] ASC),
    CONSTRAINT [FK_elements_base_elements_base] FOREIGN KEY ([categoryID]) REFERENCES [dbo].[elementCategoryBase] ([categoryID]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [IX_elementsBase] UNIQUE NONCLUSTERED ([elementName] ASC),
    CONSTRAINT [IX_elementsBase_1] UNIQUE NONCLUSTERED ([elementFriendlyName] ASC)
);



