Create FUNCTION [dbo].[getEncryptedValue] (@param varchar(200))  
RETURNS varbinary (200) AS  
BEGIN 
Declare @salt varchar(max)='TeamCosgroveHall_Key';
Declare @encryptedValue varbinary(200);
if @param <>''
begin
set @encryptedValue= EncryptByPassPhrase(@salt, @param)
end


return @encryptedValue
END