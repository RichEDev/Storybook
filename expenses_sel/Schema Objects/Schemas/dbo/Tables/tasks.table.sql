CREATE TABLE [dbo].[tasks] (
    [taskId]           INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [subAccountId]     INT            NULL,
    [regardingId]      INT            NOT NULL,
    [regardingArea]    SMALLINT       NOT NULL,
    [taskCreatorId]    INT            NULL,
    [taskCreationDate] DATETIME       NOT NULL,
    [taskTypeId]       INT            NULL,
    [taskOwnerId]      INT            NOT NULL,
    [taskOwnerType]    SMALLINT       NOT NULL,
    [subject]          NVARCHAR (150) NULL,
    [description]      NVARCHAR (MAX) NULL,
    [startDate]        DATETIME       NULL,
    [dueDate]          DATETIME       NULL,
    [endDate]          DATETIME       NULL,
    [statusId]         SMALLINT       NOT NULL,
    [escalated]        BIT            NOT NULL,
    [escalationDate]   DATETIME       NULL,
    [taskCmdType]      SMALLINT       NULL
);

