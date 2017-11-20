CREATE PROCEDURE [dbo].[APIgetCompanys]
	@companyid int
	
AS
BEGIN
	IF @companyid = 0
		BEGIN
			SELECT [companies].[companyid]
			  ,[company]
			  ,[archived]
			  ,[comment]
			  ,[companycode]
			  ,[showfrom]
			  ,[showto]
			  ,[CreatedOn]
			  ,[CreatedBy]
			  ,[ModifiedOn]
			  ,[ModifiedBy]
			  ,[address1]
			  ,[address2]
			  ,[city]
			  ,[county]
			  ,[postcode]
			  ,[country]
			  ,[parentcompanyid]
			  ,[iscompany]
			  ,[CacheExpiry]
			  ,[addressCreationMethod]
			  ,[isPrivateAddress]
			  ,[addressLookupDate]
			  ,[subAccountID]
			  ,[CompanyEsrAllocation].[ESRLocationId]
			  ,[address3]
			  ,[CompanyEsrAllocation].[ESRAddressId]
		  FROM [dbo].[companies]
		  LEFT JOIN CompanyEsrAllocation ON CompanyEsrAllocation.companyid = [companies].companyid
		END
	ELSE
		BEGIN
			SELECT [companies].[companyid]
			  ,[company]
			  ,[archived]
			  ,[comment]
			  ,[companycode]
			  ,[showfrom]
			  ,[showto]
			  ,[CreatedOn]
			  ,[CreatedBy]
			  ,[ModifiedOn]
			  ,[ModifiedBy]
			  ,[address1]
			  ,[address2]
			  ,[city]
			  ,[county]
			  ,[postcode]
			  ,[country]
			  ,[parentcompanyid]
			  ,[iscompany]
			  ,[CacheExpiry]
			  ,[addressCreationMethod]
			  ,[isPrivateAddress]
			  ,[addressLookupDate]
			  ,[subAccountID]
			  ,[CompanyEsrAllocation].[ESRLocationId]
			  ,[address3]
			  ,[CompanyEsrAllocation].[ESRAddressId]
		  FROM [dbo].[companies]
		  LEFT JOIN CompanyEsrAllocation ON CompanyEsrAllocation.companyid = [companies].companyid
		  WHERE [companies].[companyid] = @companyid
		 END
END	
RETURN 0