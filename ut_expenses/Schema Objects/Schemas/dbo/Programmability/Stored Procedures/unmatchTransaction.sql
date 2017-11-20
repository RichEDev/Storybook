CREATE PROCEDURE unmatchTransaction 
 @ids intpk readonly
AS
BEGIN
 
 SET NOCOUNT ON;

    update savedexpenses set transactionid = null where expenseid in (select c1 from ids)
END