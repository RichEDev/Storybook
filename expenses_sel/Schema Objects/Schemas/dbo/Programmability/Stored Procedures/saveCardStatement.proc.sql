
CREATE PROCEDURE [dbo].[saveCardStatement]
@statementid int,
@name nvarchar(250),
@statementdate DATETIME,
@cardproviderid INT,
@date DateTime,
@userid int,
@delegateID INT

AS

if (@statementid = 0)
begin
	insert into card_statements ([name], statementdate, cardproviderid, createdby, createdon) values (@name, @statementdate, @cardproviderid, @userid, @date)
	set @statementid = scope_identity()
	
	if @userid > 0
	BEGIN
		exec addInsertEntryToAuditLog @userid, @delegateID, 17, @statementid, @name, null;
	END
end
else
begin
	declare @oldname nvarchar(250);
	declare @oldstatementdate DATETIME;
	declare @oldcardproviderid INT;

	update card_statements set [name] = @name, statementdate = @statementdate, modifiedby = @userid, modifiedon = @date where statementid = @statementid
	
	if @userid > 0
	BEGIN
		if @oldname <> @name
			exec addUpdateEntryToAuditLog @userid, @delegateID, 17, @statementid, 'cf224fec-6b1f-46d2-b3a5-6d0554ccae37', @oldname, @name, @name, null;
		if @oldstatementdate <> @statementdate
			exec addUpdateEntryToAuditLog @userid, @delegateID, 17, @statementid, '7fb17fed-9243-40b5-bdf1-a4aac1dee6af', @oldstatementdate, @statementdate, @name, null;
		if @oldname <> @name
			exec addUpdateEntryToAuditLog @userid, @delegateID, 17, @statementid, '0694931b-4335-4e9b-a7d4-1371932cf3ae', @oldcardproviderid, @cardproviderid, @name, null;
	END
end

return @statementid
