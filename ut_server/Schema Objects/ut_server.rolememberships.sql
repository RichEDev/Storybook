EXECUTE sp_addrolemember @rolename = N'db_owner', @membername = N'spenduser';


GO
EXECUTE sp_addrolemember @rolename = N'db_owner', @membername = N'NIBLEY\simon';


GO
EXECUTE sp_addrolemember @rolename = N'db_owner', @membername = N'ESRUser';

