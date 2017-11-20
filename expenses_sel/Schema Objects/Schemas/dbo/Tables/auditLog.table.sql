CREATE TABLE [dbo].[auditLog] (
    [logid]            INT              IDENTITY (1, 1) NOT NULL,
    [elementID]        INT              NOT NULL,
    [employeeID]       INT              NULL,
    [employeeUsername] NVARCHAR (50)    NOT NULL,
    [delegateID]       INT              NULL,
    [delegateUsername] NVARCHAR (50)    NULL,
    [entityid]         INT              NULL,
    [recordTitle]      NVARCHAR (2000)  NULL,
    [datestamp]        DATETIME         NOT NULL,
    [action]           TINYINT          NOT NULL,
    [field]            UNIQUEIDENTIFIER NULL,
    [oldvalue]         NVARCHAR (4000)  NULL,
    [newvalue]         NVARCHAR (4000)  NULL,
    [subAccountId]     INT              NULL
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1=add,2=edit,3=delete,4=logon,5=logoff', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'auditLog', @level2type = N'COLUMN', @level2name = N'action';

