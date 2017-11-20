CREATE TABLE [dbo].[link_variations] (
    [linkId]              INT      IDENTITY (1, 1) NOT NULL,
    [primaryContractId]   INT      NOT NULL,
    [variationContractId] INT      NOT NULL,
    [Closed]              SMALLINT NOT NULL
);

