CREATE TABLE [dbo].[customEntityFormFields] (
    [formid]      INT     NOT NULL,
    [attributeid] INT     NOT NULL,
    [readonly]    BIT     NOT NULL,
    [sectionid]   INT     NULL,
    [row]         TINYINT NOT NULL,
    [column]      TINYINT NOT NULL,
	[labelText]	NVARCHAR(250)	NULL, 
    [DefaultValue] NVARCHAR(MAX) NULL,
	[FormMandatory] BIT NULL
);

