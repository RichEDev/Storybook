Create FUNCTION [dbo].[getDecryptedValue] (@param varchar(max))  
RETURNS varchar (max) AS  
BEGIN 
Declare @salt varchar(max)='TeamCosgroveHall_Key';

return convert(varchar(max), DecryptByPassPhrase(@salt, @param),0)
END
