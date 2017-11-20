CREATE TABLE [dbo].[esrTrusts] (
    [trustID]                 INT            IDENTITY (1, 1) NOT NULL,
    [trustVPD]                NVARCHAR (3)   NOT NULL,
    [periodType]              NVARCHAR (1)   NOT NULL,
    [periodRun]               NVARCHAR (1)   NOT NULL,
    [runSequenceNumber]       INT            CONSTRAINT [DF_esrTrusts_runSequenceNumber] DEFAULT ((0)) NOT NULL,
    [ftpAddress]              NVARCHAR (100) NULL,
    [ftpUsername]             NVARCHAR (100) NULL,
    [ftpPassword]             NVARCHAR (100) NULL,
    [archived]                BIT            CONSTRAINT [DF_esrTrusts_archived] DEFAULT ((0)) NOT NULL,
    [createdOn]               DATETIME       NULL,
    [modifiedOn]              DATETIME       NULL,
    [trustName]               NVARCHAR (150) NOT NULL,
    [delimiterCharacter]      NVARCHAR (5)   CONSTRAINT [DF__esrTrusts__delim__6A5D774A] DEFAULT (',') NOT NULL,
    [ESRVersionNumber]        TINYINT        CONSTRAINT [DF_esrTrusts_ESRVersionNumber] DEFAULT ((1)) NOT NULL,
    [currentOutboundSequence] INT            NULL,
    CONSTRAINT [PK_esrTrusts] PRIMARY KEY CLUSTERED ([trustID] ASC)
);



