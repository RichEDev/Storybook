CREATE TABLE dbo.customEntityAttributeMatchFields
(
	matchFieldId int identity(1,1) not null,
	attributeId int not null,
	fieldId uniqueidentifier not null
)