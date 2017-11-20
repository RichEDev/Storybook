CREATE TABLE [dbo].[audienceTeams] (
    [audienceTeamID] INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [audienceID]     INT NOT NULL,
    [teamID]         INT NOT NULL
);

