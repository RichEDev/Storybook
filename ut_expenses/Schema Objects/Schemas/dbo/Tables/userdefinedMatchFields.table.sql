create table dbo.userdefinedMatchFields
(
	matchFieldId int IDENTITY(1,1) not null,
	userdefineid int not null,
	fieldId uniqueidentifier not null
)
