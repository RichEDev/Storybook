CREATE TABLE [dbo].[subcats_userdefined] (
    [subuserdefineid] INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [subcatid]        INT NOT NULL,
    [userdefineid]    INT NOT NULL
);

