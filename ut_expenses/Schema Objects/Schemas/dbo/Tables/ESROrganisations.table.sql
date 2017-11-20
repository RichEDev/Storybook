CREATE TABLE [dbo].[ESROrganisations] (
    [ESROrganisationId]     BIGINT         NOT NULL,
    [OrganisationName]      NVARCHAR (240) NULL,
    [OrganisationType]      NVARCHAR (80)  NULL,
    [EffectiveFrom]         DATETIME       NOT NULL,
    [EffectiveTo]           DATETIME       NULL,
    [HierarchyVersionId]    BIGINT         NOT NULL,
    [HierarchyVersionFrom]  DATETIME       NULL,
    [HierarchyVersionTo]    DATETIME       NULL,
    [DefaultCostCentre]     NVARCHAR (15)  NULL,
    [ParentOrganisationId]  BIGINT         NULL,
    [NACSCode]              NVARCHAR (30)  NULL,
    [ESRLocationId]         BIGINT         NULL,
    [ESRLastUpdateDate]     DATETIME       NULL,
    [CostCentreDescription] NVARCHAR (240) NULL,
    CONSTRAINT [PK_ESROrganisations] PRIMARY KEY CLUSTERED ([ESROrganisationId] ASC)
);

