CREATE PROCEDURE [dbo].[ApiSetEsrOrganisationLevel]
	
AS
	WITH Organisations ([ESROrganisationId]
      ,[ParentOrganisationId]
	  , Level)
AS
(
-- Anchor member definition
SELECT [ESROrganisationId]
      ,[ParentOrganisationId]
	  ,0 AS Level
  FROM [dbo].[ESROrganisations]
  where ParentOrganisationId is null
    UNION ALL
-- Recursive member definition
SELECT ESROrganisations.[ESROrganisationId]
      ,ESROrganisations.[ParentOrganisationId]
	  ,Organisations.[Level] + 1
  FROM [dbo].ESROrganisations
  inner join Organisations on organisations.ESROrganisationId = ESROrganisations.ParentOrganisationId
)

UPDATE ESROrganisations
SET ESRORGANISATIONS.Level = ORGANISATIONS.Level
FROM ESROrganisations
INNER JOIN organisations
ON ORGANISATIONS.ESROrganisationId = ESROrganisations.ESROrganisationId



