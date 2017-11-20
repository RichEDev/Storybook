CREATE PROCEDURE [dbo].[allowExpenseItems] 
 @ids IntPK readonly
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

    -- Insert statements for procedure here
 update savedexpenses set returned = 0, tempallow = 1, IsItemEscalated=0 where expenseid in (select c1 from @ids)
 delete from returnedexpenses where expenseid in (select c1 from @ids)
END