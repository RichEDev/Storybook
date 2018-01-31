CREATE PROCEDURE [dbo].[GetAccountIdsBySingleSignOnIssuerUri]
	@Hostname nvarchar(300),
	@IssuerUri nvarchar(1000)
AS

DECLARE @accounts IntPK;
DECLARE @accountID int;
DECLARE @dbname NVARCHAR(MAX);
DECLARE @UriMatchQuery NVARCHAR(MAX);
DECLARE @UriMatch int;

DECLARE AccountCursor CURSOR FOR SELECT DISTINCT registeredusers.accountid, dbname FROM registeredusers 
JOIN accountsLicencedElements on accountsLicencedElements.accountID = registeredusers.accountid
JOIN registeredUsersHostnames on registeredUsersHostnames.accountid = registeredusers.accountid
JOIN hostnames on hostnames.hostnameID = registeredUsersHostnames.hostnameID
WHERE archived = 0 and accountsLicencedElements.elementID = 185 and hostnames.hostname = @hostname

OPEN AccountCursor;
FETCH NEXT FROM AccountCursor INTO
@accountID, @dbname

WHILE @@FETCH_STATUS = 0
BEGIN
set @UriMatchQuery = 'select @UriMatch = count(issueruri) from ' + @dbname + '.dbo.SingleSignOn where IssuerUri = ''' + @issueruri + ''''
exec sp_executesql @UriMatchQuery, N'@UriMatch int out', @UriMatch out

IF (@UriMatch > 0)
	INSERT INTO @accounts VALUES (@accountId)
FETCH NEXT FROM AccountCursor INTO @accountID, @dbname
END
CLOSE AccountCursor
DEALLOCATE AccountCursor

select * from @accounts