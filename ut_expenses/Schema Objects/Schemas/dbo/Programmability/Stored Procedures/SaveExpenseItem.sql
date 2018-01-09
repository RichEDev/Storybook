CREATE PROCEDURE [dbo].[SaveExpenseItem] (
	@claimid INT
	,@itemtype TINYINT
	,@bmiles DECIMAL(18, 2)
	,@pmiles DECIMAL(18, 2)
	,@reason NVARCHAR(2500)
	,@receipt BIT
	,@net MONEY
	,@vat MONEY
	,@total MONEY
	,@subcatid INT
	,@date DATETIME
	,@staff TINYINT
	,@others TINYINT
	,@companyid INT
	,@home BIT
	,@refnum NVARCHAR(50)
	,@plitres INT
	,@blitres INT
	,@allowanceamount MONEY
	,@currencyid INT
	,@attendees NVARCHAR(1000)
	,@tip MONEY
	,@countryid INT
	,@foreignvat MONEY
	,@convertedtotal MONEY
	,@exchangerate FLOAT
	,@normalreceipt BIT
	,@reasonid INT
	,@allowancestartdate DATETIME
	,@allowanceenddate DATETIME
	,@carid INT
	,@allowancededuct MONEY
	,@allowanceid INT
	,@nonights TINYINT
	,@quantity FLOAT
	,@directors TINYINT
	,@amountpayable MONEY
	,@hotelid INT
	,@primaryitem BIT
	,@norooms TINYINT
	,@vatnumber NVARCHAR(50)
	,@personalguests TINYINT
	,@remoteworkers TINYINT
	,@accountcode NVARCHAR(50)
	,@basecurrency INT
	,@globalexchangerate FLOAT
	,@globalbasecurrency INT
	,@globaltotal MONEY
	,@savedDate DATETIME
	,@userid INT
	,@mileageid INT
	,@transactionid INT
	,@journey_unit TINYINT
	,@assignmentnum INT
	,@hometooffice_deduction_method TINYINT
	,@addedAsMobileItem BIT
	,@addedByDeviceTypeId INT
	,@validationProgress INT
	,@validationCount INT
	,@BankAccountId INT
	,@WorkAddressId INT
	,@MobileMetricDeviceId INT
	,@expenseId INT
	)
AS
BEGIN
	IF (
			SELECT DISTINCT accessRoles.EmployeesMustHaveBankAccount
			FROM accessRoles
			INNER JOIN employeeAccessRoles ON accessRoles.roleID = employeeAccessRoles.accessRoleID
			WHERE employeeAccessRoles.employeeID = @userid
				AND accessRoles.EmployeesMustHaveBankAccount = 1
			) = 1
		AND @bankAccountId IS NULL
	BEGIN
		RETURN - 7;
	END
	ELSE
	BEGIN
		IF (@expenseId = 0)
		BEGIN
			INSERT INTO savedexpenses (
				claimid
				,itemtype
				,bmiles
				,pmiles
				,reason
				,receipt
				,net
				,vat
				,total
				,subcatid
				,[date]
				,staff
				,others
				,organisationIdentifier
				,home
				,refnum
				,plitres
				,blitres
				,allowanceamount
				,currencyid
				,attendees
				,tip
				,countryid
				,foreignvat
				,convertedtotal
				,exchangerate
				,normalreceipt
				,reasonid
				,allowancestartdate
				,allowanceenddate
				,carid
				,allowancededuct
				,allowanceid
				,nonights
				,quantity
				,directors
				,amountpayable
				,hotelid
				,primaryitem
				,norooms
				,vatnumber
				,personalguests
				,remoteworkers
				,accountcode
				,basecurrency
				,globalexchangerate
				,globalbasecurrency
				,globaltotal
				,createdon
				,createdby
				,mileageid
				,transactionid
				,journey_unit
				,esrAssignID
				,hometooffice_deduction_method
				,addedAsMobileItem
				,addedByDeviceTypeId
				,ValidationProgress
				,BankAccountId
				,WorkAddressId
				,MobileMetricDeviceId
				)
			VALUES (
				@claimid
				,@itemtype
				,@bmiles
				,@pmiles
				,@reason
				,@receipt
				,@net
				,@vat
				,@total
				,@subcatid
				,@date
				,@staff
				,@others
				,@companyid
				,@home
				,@refnum
				,@plitres
				,@blitres
				,@allowanceamount
				,@currencyid
				,@attendees
				,@tip
				,@countryid
				,@foreignvat
				,@convertedtotal
				,@exchangerate
				,@normalreceipt
				,@reasonid
				,@allowancestartdate
				,@allowanceenddate
				,@carid
				,@allowancededuct
				,@allowanceid
				,@nonights
				,@quantity
				,@directors
				,@amountpayable
				,@hotelid
				,@primaryitem
				,@norooms
				,@vatnumber
				,@personalguests
				,@remoteworkers
				,@accountcode
				,@basecurrency
				,@globalexchangerate
				,@globalbasecurrency
				,@globaltotal
				,@savedDate
				,@userid
				,@mileageid
				,@transactionid
				,@journey_unit
				,@assignmentnum
				,@hometooffice_deduction_method
				,@addedAsMobileItem
				,@addedByDeviceTypeId
				,@validationProgress
				,@BankAccountId
				,@WorkAddressId
				,@MobileMetricDeviceId
				);

			RETURN @@identity;
		END
		ELSE
		BEGIN
			UPDATE savedexpenses
			SET claimid = @claimid
				,bmiles = @bmiles
				,pmiles = @pmiles
				,reason = @reason
				,receipt = @receipt
				,net = @net
				,vat = @vat
				,total = @total
				,subcatid = @subcatid
				,[date] = @date
				,staff = @staff
				,others = @others
				,organisationIdentifier = @companyid
				,home = @home
				,plitres = @plitres
				,blitres = @blitres
				,currencyid = @currencyid
				,attendees = @attendees
				,tip = @tip
				,countryid = @countryid
				,foreignvat = @foreignvat
				,convertedtotal = @convertedtotal
				,exchangerate = @exchangerate
				,reasonid = @reasonid
				,normalreceipt = @normalreceipt
				,allowancestartdate = @allowancestartdate
				,allowanceenddate = @allowanceenddate
				,carid = @carid
				,allowancededuct = @allowancededuct
				,allowanceid = @allowanceid
				,nonights = @nonights
				,quantity = @quantity
				,directors = @directors
				,amountpayable = @amountpayable
				,hotelid = @hotelid
				,primaryitem = @primaryitem
				,norooms = @norooms
				,vatnumber = @vatnumber
				,personalguests = @personalguests
				,remoteworkers = @remoteworkers
				,accountcode = @accountcode
				,basecurrency = @basecurrency
				,globalexchangerate = @globalexchangerate
				,globalbasecurrency = @globalbasecurrency
				,globaltotal = @globaltotal
				,ModifiedOn = @savedDate
				,ModifiedBy = @userid
				,mileageid = @mileageid
				,transactionid = @transactionid
				,journey_unit = @journey_unit
				,itemtype = @itemtype
				,esrAssignID = @assignmentnum
				,hometooffice_deduction_method = @hometooffice_deduction_method
				,ValidationProgress = @validationProgress
				,ValidationCount = @validationCount
				,BankAccountId = @BankAccountId
				,WorkAddressId = @WorkAddressId
			WHERE expenseid = @expenseId

			RETURN @expenseId;
		END
	END
END