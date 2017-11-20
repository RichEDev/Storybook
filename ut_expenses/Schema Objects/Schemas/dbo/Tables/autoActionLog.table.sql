CREATE TABLE [dbo].[autoActionLog] (
    [actionid]            INT      IDENTITY (1, 1) NOT NULL,
    [datestamp]           DATETIME NOT NULL,
    [activationCount]     SMALLINT NOT NULL,
    [archiveCount]        SMALLINT NOT NULL,
    [processed]           BIT      NOT NULL,
    [archiveFailCount]    SMALLINT NOT NULL,
    [assignmentCount]     SMALLINT NOT NULL,
    [assignmentFailCount] SMALLINT NOT NULL
);

