﻿create table dbo.ESRPersons
	(
		ESRPersonId bigint not null,
		EffectiveStartDate datetime null,
		EffectiveEndDate datetime null,
		EmployeeNumber nvarchar(30) null,
		Title nvarchar(30) null,
		LastName nvarchar(150) not null,
		FirstName nvarchar(150) null,
		MiddleNames nvarchar(60) null,
		MaidenName nvarchar(150) null,
		PreferredName nvarchar(80) null,
		PreviousLastName nvarchar(150) null,
		Gender nvarchar(30) null,
		DateOfBirth datetime null,
		NINumber nvarchar(30) null,
		NHSUniqueId nvarchar(15) null,
		HireDate datetime null,
		ActualTerminationDate datetime null,
		TerminationReason nvarchar(30) null,
		EmployeeStatusFlag nvarchar(3) null,
		WTROptOut nvarchar(3) null,
		WTROptOutDate datetime null,
		EthnicOrigin nvarchar(30) null,
		MaritalStatus nvarchar(30) null,
		CountryOfBirth nvarchar(30) null,
		PreviousEmployer nvarchar(240) null,
		PreviousEmployerType nvarchar(30) null,
		CSD3Months datetime null,
		CSD12Months datetime null,
		NHSCRSUUID nvarchar(12) null,
		SystemPersonType nvarchar(30) null,
		UserPersonType nvarchar(80) null,
		OfficeEmailAddress nvarchar(240) null,
		NHSStartDate datetime null,
		ESRLastUpdateDate datetime null,
		DisabilityFlag nvarchar(1) null,
		LegacyPayrollNumber nvarchar(150) null,
		Nationality nvarchar(30) null
	)