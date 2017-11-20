CREATE TYPE [dbo].[ApiBatchSaveEsrOrganisationType] AS TABLE (
    [ESROrganisationId]     BIGINT         NULL,
    [OrganisationName]      NVARCHAR (500) NULL,
    [OrganisationType]      NVARCHAR (500) NULL,
    [EffectiveFrom]         DATETIME       NULL,
    [EffectiveTo]           DATETIME       NULL,
    [HierarchyVersionId]    BIGINT         NULL,
    [HierarchyVersionFrom]  DATETIME       NULL,
    [HierarchyVersionTo]    DATETIME       NULL,
    [DefaultCostCentre]     NVARCHAR (500) NULL,
    [ParentOrganisationId]  BIGINT         NULL,
    [NACSCode]              NVARCHAR (500) NULL,
    [ESRLocationId]         BIGINT         NULL,
    [ESRLastUpdateDate]     DATETIME       NULL,
    [CostCentreDescription] NVARCHAR (500) NULL);

