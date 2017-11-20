﻿CREATE TABLE [dbo].[employees] (
    [employeeid]                INT             IDENTITY (1, 1) NOT NULL,
    [username]                  NVARCHAR (50)   NOT NULL,
    [password]                  NVARCHAR (250)  COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [title]                     NVARCHAR (30)   NOT NULL,
    [firstname]                 NVARCHAR (150)  NOT NULL,
    [surname]                   NVARCHAR (150)  NOT NULL,
    [mileagetotal]              INT             NULL,
    [email]                     NVARCHAR (250)  COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [currefnum]                 INT             NOT NULL,
    [curclaimno]                INT             NOT NULL,
    [speedo]                    INT             NULL,
    [payroll]                   NVARCHAR (50)   COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [position]                  NVARCHAR (250)  NULL,
    [telno]                     NVARCHAR (50)   COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [creditor]                  NVARCHAR (50)   COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [archived]                  BIT             NOT NULL,
    [groupid]                   INT             NULL,
    [roleid]                    INT             NULL,
    [hint]                      NVARCHAR (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [lastchange]                SMALLDATETIME   NULL,
    [additems]                  INT             NOT NULL,
    [cardnum]                   NVARCHAR (50)   COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [userole]                   BIT             NOT NULL,
    [costcodeid]                INT             NULL,
    [departmentid]              INT             NULL,
    [extension]                 NVARCHAR (50)   COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [pagerno]                   NVARCHAR (50)   COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [mobileno]                  NVARCHAR (50)   COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [faxno]                     NVARCHAR (50)   COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [homeemail]                 NVARCHAR (250)  NULL,
    [linemanager]               INT             NULL,
    [advancegroupid]            INT             NULL,
    [mileage]                   INT             NOT NULL,
    [mileageprev]               INT             NOT NULL,
    [customiseditems]           BIT             NOT NULL,
    [active]                    BIT             NOT NULL,
    [primarycountry]            INT             NULL,
    [primarycurrency]           INT             NULL,
    [verified]                  BIT             NOT NULL,
    [licenceexpiry]             DATETIME        NULL,
    [licencelastchecked]        DATETIME        NULL,
    [licencecheckedby]          INT             NULL,
    [licencenumber]             NVARCHAR (50)   COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [groupidcc]                 INT             NULL,
    [groupidpc]                 INT             NULL,
    [CreatedOn]                 DATETIME        NULL,
    [CreatedBy]                 INT             NULL,
    [ModifiedOn]                DATETIME        NULL,
    [ModifiedBy]                INT             NULL,
    [country]                   NVARCHAR (100)  COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [ninumber]                  NVARCHAR (50)   COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [maidenname]                NVARCHAR (150)  NULL,
    [middlenames]               NVARCHAR (150)  NULL,
    [gender]                    NVARCHAR (6)    COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [dateofbirth]               DATETIME        NULL,
    [hiredate]                  DATETIME        NULL,
    [terminationdate]           DATETIME        NULL,
    [homelocationid]            INT             NULL,
    [officelocationid]          INT             NULL,
    [applicantnumber]           NVARCHAR (50)   NULL,
    [applicantactivestatusflag] BIT             NOT NULL,
    [passwordMethod]            TINYINT         NULL,
    [name]                      NVARCHAR (50)   NULL,
    [accountnumber]             NVARCHAR (50)   NULL,
    [accounttype]               NVARCHAR (50)   NULL,
    [sortcode]                  NVARCHAR (50)   NULL,
    [reference]                 NVARCHAR (50)   NULL,
    [localeID]                  INT             NULL,
    [NHSTrustID]                INT             NULL,
    [logonCount]                INT             NOT NULL,
    [retryCount]                INT             NOT NULL,
    [firstLogon]                BIT             NOT NULL,
    [licenceAttachID]           INT             NULL,
    [defaultSubAccountId]       INT             NULL,
    [CacheExpiry]               DATETIME        NULL,
    [supportPortalAccountID]    INT             NULL,
    [supportPortalPassword]     NVARCHAR (250)  NULL,
    [CreationMethod]            TINYINT         NOT NULL,
    [mileagetotaldate]          DATETIME        NULL,
	[adminonly]					[bit]			NOT NULL,
	[locked]					[bit]			NOT NULL,
	[ESRPersonId]				BIGINT				NULL,
	[ESREffectiveStartDate]		DATETIME		NULL,
	[ESREffectiveEndDate]		DATETIME		NULL,
	[PreferredName]				NVARCHAR(150)	NULL,
	[EmployeeNumber]			NVARCHAR(30)	NULL,
	[NHSUniqueId]				NVARCHAR(15)	NULL,
	[ESRPersonType]				NVARCHAR(80)	NULL,  
    [AuthoriserLevelDetailId] INT NULL, 
    [ApproverLastRemindedDate] DATETIME NULL, 
    [NotifyClaimUnsubmission] BIT NOT NULL DEFAULT 0, 
    [DriverId] INT NULL, 
    [DVLAConsentDate] DATETIME NULL, 
    [SecurityCode] UNIQUEIDENTIFIER NULL, 
    [drivinglicence_firstname] VARBINARY(MAX) NULL, 
    [drivinglicence_surname] VARBINARY(MAX) NULL, 
    [drivinglicence_dateofbirth] VARBINARY(MAX) NULL, 
    [drivinglicence_sex] VARBINARY(MAX) NULL,
	[drivinglicence_licenceNumber] VARBINARY(MAX) NULL, 
    [drivinglicence_Email] VARBINARY(MAX) NULL,
	[Drivinglicence_Middlename] VARBINARY(MAX) NULL, 
    [DvlaLookUpDate] DATETIME NULL, 
    [DvlaResponseCode] NVARCHAR(50) NULL, 
    [AgreeToProvideConsent] BIT NULL,
	[ExcessMileage] FLOAT NULL
);

