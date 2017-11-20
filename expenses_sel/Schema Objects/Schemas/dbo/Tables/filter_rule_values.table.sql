CREATE TABLE [dbo].[filter_rule_values] (
    [filterruleid] INT      IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [parentid]     INT      NOT NULL,
    [childid]      INT      NOT NULL,
    [filterid]     INT      NOT NULL,
    [createdon]    DATETIME NULL,
    [createdby]    INT      NULL
);

