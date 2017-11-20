CREATE TABLE [dbo].[task_history] (
    [historyId]     INT            IDENTITY (1, 1)  NOT NULL,
    [taskId]        INT            NOT NULL,
    [datestamp]     DATETIME       NOT NULL,
    [changeDetails] NVARCHAR (150) NULL,
    [preVal]        NVARCHAR (MAX) NULL,
    [postVal]       NVARCHAR (MAX) NULL,
    [changedBy]     INT            NOT NULL
);

