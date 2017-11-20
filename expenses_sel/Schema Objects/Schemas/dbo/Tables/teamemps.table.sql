CREATE TABLE [dbo].[teamemps] (
    [teamempid]  INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [teamid]     INT NOT NULL,
    [employeeid] INT NOT NULL
);

