CREATE TABLE [dbo].[userdefinedClaims] (
    [claimid] INT             NOT NULL,
    [udf523]  NVARCHAR (1123) NULL,
    [udf524]  NVARCHAR (222)  NULL,
    [udf527]  NVARCHAR (50)   NULL,
    [udf542]  NVARCHAR (4000) NULL,
    CONSTRAINT [PK_userdefinedClaims] PRIMARY KEY CLUSTERED ([claimid] ASC),
    CONSTRAINT [FK_userdefinedClaims_claims_base] FOREIGN KEY ([claimid]) REFERENCES [dbo].[claims_base] ([claimid]) ON DELETE CASCADE
);



