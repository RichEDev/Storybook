CREATE TYPE [dbo].[ApiBatchSaveEsrTrustType] AS TABLE (
    [trustID]                 INT            NULL,
    [trustVPD]                NVARCHAR (500) NULL,
    [periodType]              NVARCHAR (500) NULL,
    [periodRun]               NVARCHAR (500) NULL,
    [runSequenceNumber]       INT            NULL,
    [ftpAddress]              NVARCHAR (500) NULL,
    [ftpUsername]             NVARCHAR (500) NULL,
    [ftpPassword]             NVARCHAR (500) NULL,
    [archived]                BIT            NULL,
    [createdOn]               DATETIME       NULL,
    [modifiedOn]              DATETIME       NULL,
    [trustName]               NVARCHAR (500) NULL,
    [delimiterCharacter]      NVARCHAR (500) NULL,
    [ESRVersionNumber]        TINYINT        NULL,
    [currentOutboundSequence] INT            NULL);

