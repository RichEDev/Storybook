CREATE TABLE [dbo].[elementsBase] (
    [elementID]             INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [categoryID]            INT             NOT NULL,
    [elementName]           NVARCHAR (100)  COLLATE Latin1_General_CI_AS NOT NULL,
    [elementFriendlyName]   NVARCHAR (100)  COLLATE Latin1_General_CI_AS NULL,
    [description]           NVARCHAR (4000) COLLATE Latin1_General_CI_AS NULL,
    [accessRolesCanEdit]    BIT             NULL,
    [accessRolesCanAdd]     BIT             NULL,
    [accessRolesCanDelete]  BIT             NULL,
    [accessRolesApplicable] BIT             NOT NULL
);

