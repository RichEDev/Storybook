CREATE TABLE [dbo].[esrTrusts] (
    [trustID]            INT            IDENTITY (1, 1) NOT NULL,
    [trustVPD]           NVARCHAR (3)   NOT NULL,
    [periodType]         NVARCHAR (1)   NOT NULL,
    [periodRun]          NVARCHAR (1)   NOT NULL,
    [runSequenceNumber]  INT            NOT NULL,
    [ftpAddress]         NVARCHAR (100) NULL,
    [ftpUsername]        NVARCHAR (100) NULL,
    [ftpPassword]        NVARCHAR (100) NULL,
    [archived]           BIT            NOT NULL,
    [createdOn]          DATETIME       NULL,
    [modifiedOn]         DATETIME       NULL,
    [trustName]          NVARCHAR (150) NOT NULL,
    [delimiterCharacter] NVARCHAR (5)   NOT NULL
);

