CREATE TABLE [dbo].[car_documents] (
    [documentid]   INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [employeeid]   INT            NOT NULL,
    [carid]        INT            NULL,
    [documenttype] TINYINT        NOT NULL,
    [filename]     NVARCHAR (500) NOT NULL
);

