﻿CREATE PROCEDURE [dbo].[APIgetCarsByEsrId]
	@EsrId int 
AS
	SELECT [cars].[carid]
		,[cars].[employeeid]
		,[cars].[startdate]
		,[cars].[enddate]
		,[cars].[make]
		,[cars].[model]
		,[cars].[registration]
		,[cars].[mileageid]
		,[cars].[cartypeid]
		,[cars].[active]
		,[cars].[odometer]
		,[cars].[fuelcard]
		,[cars].[endodometer]
		,[cars].[taxexpiry]
		,[cars].[taxlastchecked]
		,[cars].[taxcheckedby]
		,[cars].[mottestnumber]
		,[cars].[motlastchecked]
		,[cars].[motcheckedby]
		,[cars].[motexpiry]
		,[cars].[insurancenumber]
		,[cars].[insuranceexpiry]
		,[cars].[insurancelastchecked]
		,[cars].[insurancecheckedby]
		,[cars].[serviceexpiry]
		,[cars].[servicelastchecked]
		,[cars].[servicecheckedby]
		,[cars].[CreatedOn]
		,[cars].[CreatedBy]
		,[cars].[ModifiedOn]
		,[cars].[ModifiedBy]
		,[cars].[default_unit]
		,[cars].[enginesize]
		,[cars].[approved]
		,[cars].[exemptFromHomeToOffice]
		,[cars].[taxAttachID]
		,[cars].[MOTAttachID]
		,[cars].[insuranceAttachID]
		,[cars].[serviceAttachID]
	FROM
		[dbo].[cars]
			join [dbo].[employees] on [employees].[employeeid] = [cars].[employeeid]
	WHERE
		[employees].[ESRPersonId] = @EsrId
