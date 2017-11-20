CREATE TABLE [dbo].[broadcast_messages] (
    [broadcastid]    INT            IDENTITY (1, 1) NOT NULL,
    [title]          NVARCHAR (250) NOT NULL,
    [message]        NTEXT          NOT NULL,
    [startdate]      DATETIME       NULL,
    [enddate]        DATETIME       NULL,
    [expirewhenread] BIT            NOT NULL,
    [location]       TINYINT        NOT NULL,
    [datestamp]      DATETIME       NOT NULL,
    [oncepersession] BIT            NOT NULL,
    [CreatedOn]      DATETIME       NULL,
    [CreatedBy]      INT            NULL,
    [ModifiedOn]     DATETIME       NULL,
    [ModifiedBy]     INT            NULL
);

