CREATE TABLE [dbo].[custom_entity_forms] (
    [formid]                INT             IDENTITY (1, 1) NOT NULL,
    [entityid]              INT             NOT NULL,
    [form_name]             NVARCHAR (100)  NOT NULL,
    [description]           NVARCHAR (4000) NULL,
    [createdon]             DATETIME        NOT NULL,
    [createdby]             INT             NOT NULL,
    [modifiedon]            DATETIME        NULL,
    [modifiedby]            INT             NULL,
    [showBreadCrumbs]       BIT             NOT NULL,
    [showSaveAndNew]        BIT             NOT NULL,
    [showSubMenus]          BIT             NOT NULL,
    [SaveAndNewButtonText]  NVARCHAR (100)  NOT NULL,
    [SaveButtonText]        NVARCHAR (100)  NOT NULL,
    [CancelButtonText]      NVARCHAR (100)  NOT NULL,
    [showSave]              BIT             NOT NULL,
    [showCancel]            BIT             NOT NULL,
    [SaveAndStayButtonText] NVARCHAR (100)  NOT NULL,
    [showSaveAndStay]       BIT             NOT NULL
);

