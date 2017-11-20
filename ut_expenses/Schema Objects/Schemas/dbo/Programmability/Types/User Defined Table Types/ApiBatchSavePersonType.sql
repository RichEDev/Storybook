﻿CREATE TYPE [dbo].[ApiBatchSavePersonType] AS TABLE (
    [ESRPersonId]           BIGINT         NULL,
    [EffectiveStartDate]    DATETIME       NULL,
    [EffectiveEndDate]      DATETIME       NULL,
    [EmployeeNumber]        NVARCHAR (500) NULL,
    [Title]                 NVARCHAR (500) NULL,
    [LastName]              NVARCHAR (500) NULL,
    [FirstName]             NVARCHAR (500) NULL,
    [MiddleNames]           NVARCHAR (500) NULL,
    [MaidenName]            NVARCHAR (500) NULL,
    [PreferredName]         NVARCHAR (500) NULL,
    [PreviousLastName]      NVARCHAR (500) NULL,
    [Gender]                NVARCHAR (500) NULL,
    [DateOfBirth]           DATETIME       NULL,
    [NINumber]              NVARCHAR (500) NULL,
    [NHSUniqueId]           NVARCHAR (500) NULL,
    [HireDate]              DATETIME       NULL,
    [ActualTerminationDate] DATETIME       NULL,
    [TerminationReason]     NVARCHAR (500) NULL,
    [EmployeeStatusFlag]    NVARCHAR (500) NULL,
    [WTROptOut]             NVARCHAR (500) NULL,
    [WTROptOutDate]         DATETIME       NULL,
    [EthnicOrigin]          NVARCHAR (500) NULL,
    [MaritalStatus]         NVARCHAR (500) NULL,
    [CountryOfBirth]        NVARCHAR (500) NULL,
    [PreviousEmployer]      NVARCHAR (500) NULL,
    [PreviousEmployerType]  NVARCHAR (500) NULL,
    [CSD3Months]            DATETIME       NULL,
    [CSD12Months]           DATETIME       NULL,
    [NHSCRSUUID]            NVARCHAR (500) NULL,
    [SystemPersonType]      NVARCHAR (500) NULL,
    [UserPersonType]        NVARCHAR (500) NULL,
    [OfficeEmailAddress]    NVARCHAR (500) NULL,
    [NHSStartDate]          DATETIME       NULL,
    [ESRLastUpdateDate]     DATETIME       NULL,
    [DisabilityFlag]        NVARCHAR (500) NULL,
    [LegacyPayrollNumber]   NVARCHAR (500) NULL,
    [Nationality]           NVARCHAR (500) NULL);

