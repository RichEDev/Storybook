CREATE procedure [dbo].[savePendingEmail]
@emailType int,
@subject nvarchar(150),
@body nvarchar(max),
@recipientId int,
@recipientType int
as
begin
	declare @Identity int

	insert into pending_email (emailType, datestamp, subject, body, recipientId, recipientType)
	values (@emailType, getdate(), @subject, @body, @recipientId, @recipientType)

	set @Identity = scope_identity();

	return @Identity;
end
