CREATE TABLE [dbo].[custom_entities] (
    [entityid]                   INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [entity_name]                NVARCHAR (250)   NOT NULL,
    [plural_name]                NVARCHAR (250)   NOT NULL,
    [description]                NVARCHAR (4000)  NULL,
    [createdon]                  DATETIME         NOT NULL,
    [createdby]                  INT              NOT NULL,
    [modifiedon]                 DATETIME         NULL,
    [modifiedby]                 INT              NULL,
    [tableid]                    UNIQUEIDENTIFIER NOT NULL,
    [enableAttachments]          BIT              NOT NULL,
    [enableAudiences]            BIT              NOT NULL,
    [allowdocmergeaccess]        BIT              NOT NULL,
    [systemview_derivedentityid] INT              NULL,
    [systemview]                 BIT              NOT NULL,
    [attachmentTableID]          UNIQUEIDENTIFIER NOT NULL,
    [audienceTableID]            UNIQUEIDENTIFIER NOT NULL,
    [systemview_entityid]        INT              NULL,
    [enableCurrencies]           BIT              NOT NULL,
    [defaultCurrencyID]          INT              NULL
);

