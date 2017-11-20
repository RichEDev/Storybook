
CREATE VIEW [dbo].[card_statements]
AS
SELECT     dbo.card_statements_base.statementid, dbo.card_statements_base.name, dbo.card_statements_base.statementdate, dbo.card_statements_base.CreatedOn, 
                      dbo.card_statements_base.cardproviderid, metabaseExpenses.dbo.card_providers.cardprovider, dbo.card_statements_base.CreatedBy, dbo.card_statements_base.ModifiedOn, 
                      dbo.card_statements_base.ModifiedBy,
                          (SELECT     COUNT(*) AS Expr1
                            FROM          dbo.card_transactions
                            WHERE      (statementid = dbo.card_statements_base.statementid)) AS item_count,
                          (SELECT     COUNT(DISTINCT card_number) AS Expr1
                            FROM          dbo.card_transactions AS card_transactions_1
                            WHERE      (corporatecardid IS NULL) AND (statementid = dbo.card_statements_base.statementid)) AS unallocated_cards
FROM         dbo.card_statements_base INNER JOIN
                      [$(metabaseExpenses)].dbo.card_providers ON metabaseExpenses.dbo.card_providers.cardproviderid = dbo.card_statements_base.cardproviderid


