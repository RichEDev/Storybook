Create FUNCTION [dbo].[DeriveCostcodeForESRReport]
(
	@employeeid int,
	@costcodeid int,
	@costcode nvarchar(50),
	@trustid int
)
RETURNS nvarchar(50)
AS
BEGIN

	
	-- Declare the return variable here

	if (exists(select employeecostcodeid from employee_costcodes where employeeid = @employeeid and costcodeid = @costcodeid))
		return ''

	DECLARE @vpdNumber nvarchar(3);
    
    SET @vpdNumber = (SELECT top 1 trustVPD from esrTrusts where trustID = @trustId);
    
	if (select count(defaultcostcentre) from ESROrganisations o where o.OrganisationName like @vpdNumber + '%' and o.DefaultCostCentre = @costcode and o.DefaultCostCentre != '' and o.DefaultCostCentre is not null) > 0
		return @costcode
	else
		return ''
	
	

	return @costcode
END
