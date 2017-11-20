
 

CREATE PROCEDURE [dbo].[GetAddressLocationsByPrefixText] 

 

@company nvarchar(250),

@companyType tinyint

 

AS

BEGIN

      -- SET NOCOUNT ON added to prevent extra result sets from

      -- interfering with SELECT statements.

      SET NOCOUNT ON;

      

      IF @companyType = 0 -- Company

            BEGIN

                  SELECT TOP 15 companyid, company FROM companies WHERE archived = 0 AND isCompany = 1 AND (company LIKE @company OR postcode LIKE @company) ORDER BY company

            END

      ELSE IF @companyType = 1 -- From

            BEGIN

                  SELECT TOP 15 companyid, company from companies where archived = 0 AND showfrom = 1 AND (company LIKE @company OR postcode LIKE @company) ORDER BY company

            END

      ELSE IF @companyType = 2 -- To

            BEGIN

                  SELECT TOP 15 companyid, company from companies where archived = 0 AND showto = 1 AND (company LIKE @company OR postcode LIKE @company) ORDER BY company

            END

      ELSE IF @companyType = 3 -- None

            BEGIN

                  SELECT TOP 15 companyid, company FROM companies WHERE archived = 0 AND (company LIKE @company OR postcode LIKE @company) ORDER BY company 

            END   

            

END

