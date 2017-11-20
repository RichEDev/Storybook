CREATE VIEW dbo.card_providers
AS
SELECT     cp.cardproviderid, cardprovider, card_type, creditcard, purchasecard, cp.createdon, cp.createdby, cp.modifiedon, cp.modifiedby, AutoImport, claimants_settle_bill
FROM         [$(targetMetabase)].dbo.card_providers AS cp
LEFT JOIN corporate_cards as cc on cc.cardproviderid = cp.cardproviderid
