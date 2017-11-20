CREATE PROCEDURE [dbo].[EsrGetDefaultCostCentresForVPDNumber] 
@trustId int
AS
BEGIN
    DECLARE @vpdNumber nvarchar(3);
    
    SET @vpdNumber = (SELECT top 1 trustVPD from esrTrusts where trustID = @trustId);
    select distinct o.DefaultCostCentre from ESROrganisations o where o.OrganisationName like @vpdNumber + '%' and o.DefaultCostCentre != '' and o.DefaultCostCentre is not null
END