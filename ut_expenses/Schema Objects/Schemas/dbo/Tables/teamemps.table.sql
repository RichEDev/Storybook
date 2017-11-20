CREATE TABLE [dbo].[teamemps] (
    [teamempid]  INT IDENTITY (1, 1) NOT NULL,
    [teamid]     INT CONSTRAINT [DF_teamemps_teamid] DEFAULT (0) NOT NULL,
    [employeeid] INT CONSTRAINT [DF_teamemps_employeeid] DEFAULT (0) NOT NULL,
    PRIMARY KEY CLUSTERED ([teamempid] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_teamemps_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_teamemps_teams] FOREIGN KEY ([teamid]) REFERENCES [dbo].[teams] ([teamid]) ON DELETE CASCADE ON UPDATE CASCADE
);



