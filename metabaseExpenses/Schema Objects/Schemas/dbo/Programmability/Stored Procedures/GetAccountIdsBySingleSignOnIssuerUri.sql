CREATE PROCEDURE [dbo].[GetAccountIdsBySingleSignOnIssuerUri]
	@Hostname nvarchar(300),
	@IssuerUri nvarchar(1000)
AS
    declare @sql nvarchar(4000) = '';
	
    select
        @sql = @sql + 'select ''' + registeredusers.dbname + ''' as dbname, IssuerUri COLLATE DATABASE_DEFAULT as IssuerUri from ' + registeredusers.dbname + '.dbo.SingleSignOn union '
    from
        registeredusers
			join accountsLicencedElements on accountsLicencedElements.accountID = registeredusers.accountid
			join registeredUsersHostnames on registeredUsersHostnames.accountid = registeredusers.accountid
				join hostnames on hostnames.hostnameID = registeredUsersHostnames.hostnameID
    where
        registeredusers.archived = 0
        and accountsLicencedElements.elementID = 185 -- SingleSignOn
        and hostnames.hostname = @Hostname

	if LEN(@sql) > 0
	begin
		set @sql = 'select registeredusers.accountid from registeredusers join (' + SUBSTRING(@sql, 1, LEN(@sql) - 6) + ') sso on sso.dbname = registeredusers.dbname where sso.IssuerUri COLLATE DATABASE_DEFAULT = @IssuerUri'
	    exec sp_executesql @sql, N'@IssuerUri nvarchar(1000)', @IssuerUri = @IssuerUri
	end

RETURN 0
