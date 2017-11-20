CREATE FUNCTION [dbo].[getAccountType] (@accounttypeid int)  
RETURNS nvarchar (50) AS  
BEGIN 
Declare @accounttype varchar(20)
if @accounttypeid = 1 
 set @accounttype ='Savings'
 else if @accounttypeid =2 
 set @accounttype = 'Current'
 else if @accounttypeid =3 
 set @accounttype = 'Credit Card'
 else
 set @accounttype = 'None'

 return @accounttype
END