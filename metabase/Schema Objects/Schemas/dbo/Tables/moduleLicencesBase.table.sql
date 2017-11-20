CREATE TABLE [dbo].[moduleLicencesBase] (
    [moduleID]   INT      NOT NULL,
    [accountID]  INT      NOT NULL,
    [expiryDate] DATETIME NULL,
    [maxUsers]   INT      NOT NULL
);

