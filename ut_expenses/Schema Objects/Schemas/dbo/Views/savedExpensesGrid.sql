CREATE VIEW [dbo].[savedExpensesGrid]
AS

SELECT dbo.savedexpenses.expenseid, dbo.savedexpenses.receiptattached, dbo.float_allocations.floatid, dbo.savedexpenses.transactionid, dbo.savedexpenses.currencyid, dbo.global_currencies.label, dbo.savedexpenses.returned, dbo.returnedexpenses.corrected, dbo.savedexpenses.countryid, dbo.global_countries.country, CASE 
  WHEN savedexpensesSplitHierarchy.primaryitem IS NOT NULL
   THEN CAST(1 AS BIT)
  ELSE CAST(0 AS BIT)
  END AS hasSplitItems, CASE 
  WHEN (
    (
     SELECT COUNT(savedexpensesFlags.expenseid)
     FROM savedexpensesFlags
     WHERE savedexpensesFlags.expenseid in (select splititem from dbo.getExpenseItemIdsFromPrimary(savedexpenses.expenseid))
     )
    ) = 0
   THEN CAST(0 AS BIT)
  ELSE CAST(1 AS BIT)
  END AS hasFlags,
  (select max(flagcolour) from savedexpensesFlags where savedexpensesflags.expenseid in (select splititem from dbo.getExpenseItemIdsFromPrimary(savedexpenses.expenseid))) as flagColour,
   CASE 
  WHEN (
    SELECT COUNT(savedexpenses_journey_steps.expenseid)
    FROM savedexpenses_journey_steps
    WHERE savedexpenses_journey_steps.expenseid = savedexpenses.expenseid
     AND StartAddressID IS NOT NULL
     AND EndAddressID IS NOT NULL
    ) = 0
   THEN CAST(0 AS BIT)
  ELSE CAST(1 AS BIT)
  END AS hasJourneySteps, dbo.savedexpenses.total + ISNULL(SUM(splits.total), 0) AS grandTotal, dbo.savedexpenses.net + ISNULL(SUM(splits.net), 0) AS NETgrandTotal, dbo.savedexpenses.vat + ISNULL(SUM(splits.vat), 0) AS VATgrandTotal, dbo.savedexpenses.amountpayable + ISNULL(SUM(splits.amountpayable), 0) AS amountPayableGrandTotal, dbo.savedexpenses.convertedtotal + ISNULL(SUM(splits.convertedtotal), 0) AS convertedGrandTotal, dbo.savedexpenses.globaltotal + ISNULL(SUM(splits.globaltotal), 0) AS globalGrandTotal, dbo.savedexpenses.others + ISNULL(SUM(splits.others), 0) AS othersGrandTotal, dbo.savedexpenses.remoteworkers + ISNULL(SUM(splits.remoteworkers), 0) AS remoteWorkerGrandTotal, dbo.savedexpenses.personalguests + ISNULL(SUM(splits.personalguests), 0) AS personalGuestsGrandTotal
FROM dbo.savedexpenses
LEFT OUTER JOIN dbo.savedexpensesSplitHierarchy ON dbo.savedexpensesSplitHierarchy.primaryitem = dbo.savedexpenses.expenseid
LEFT OUTER JOIN dbo.savedexpenses AS splits ON dbo.savedexpensesSplitHierarchy.splititem = splits.expenseid
LEFT OUTER JOIN dbo.currencies ON dbo.currencies.currencyid = dbo.savedexpenses.currencyid
LEFT OUTER JOIN dbo.global_currencies ON dbo.global_currencies.globalcurrencyid = dbo.currencies.globalcurrencyid
LEFT OUTER JOIN dbo.float_allocations ON dbo.float_allocations.expenseid = dbo.savedexpenses.expenseid
LEFT OUTER JOIN dbo.countries ON dbo.countries.countryid = dbo.savedexpenses.countryid
LEFT OUTER JOIN dbo.global_countries ON dbo.global_countries.globalcountryid = dbo.countries.globalcountryid
LEFT OUTER JOIN dbo.returnedexpenses ON dbo.returnedexpenses.expenseid = dbo.savedexpenses.expenseid
WHERE (dbo.savedexpenses.primaryitem = 1)
GROUP BY dbo.savedexpenses.expenseid, dbo.savedexpenses.receiptattached, dbo.float_allocations.floatid, dbo.savedexpenses.transactionid, dbo.savedexpenses.currencyid, dbo.global_currencies.label, dbo.savedexpenses.returned, dbo.returnedexpenses.corrected, dbo.savedexpenses.countryid, dbo.global_countries.country, dbo.savedexpensesSplitHierarchy.primaryitem, dbo.savedexpenses.total, dbo.savedexpenses.net, dbo.savedexpenses.vat, dbo.savedexpenses.amountpayable, dbo.savedexpenses.convertedtotal, dbo.savedexpenses.globaltotal, dbo.savedexpenses.others, dbo.savedexpenses.remoteworkers, dbo.savedexpenses.personalguests
GO
