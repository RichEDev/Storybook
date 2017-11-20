CREATE PROCEDURE [dbo].[addStaticExchangeRate]
	@currencyid int,
	@tocurrencyid int,
	@exchangerate float,
	@date DateTime,
	@userid int
AS

BEGIN
	insert into static_exchangerates (currencyid, tocurrencyid, exchangerate, createdon, createdby)
                values (@currencyid, @tocurrencyid, @exchangerate, @date, @userid)
END
