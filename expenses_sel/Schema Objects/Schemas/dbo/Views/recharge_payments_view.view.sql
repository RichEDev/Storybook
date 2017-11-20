CREATE VIEW [dbo].[recharge_payments_view]
AS
SELECT 
[RechargeItemId] AS [RecordId],
[RechargeId] AS [RechargeId],
[contract_productdetails].[ContractId] AS [ContractId],
[RechargePeriod] AS [ChargeDate],
[RechargeAmount] AS [ChargeAmount],
[RechargeEntityId],
contract_productdetails.[CurrencyId] AS [CurrencyId],
0 AS [IsOneOffCharge]
FROM contract_productdetails_recharge
INNER JOIN contract_productdetails ON contract_productdetails.[ContractProductId] = contract_productdetails_recharge.[ContractProductId]
UNION
SELECT 
[ChargeId] AS [RecordId],
0 AS [RechargeId],
contract_productdetails_oneoffcharge.[ContractId] AS [ContractId],
[ChargeDate] AS [ChargeDate],
[ChargeAmount] AS [ChargeAmount],
[RechargeEntityId],
contract_details.[ContractCurrency] AS [Currency Id],
1 AS [Is One Off Charge]
FROM contract_productdetails_oneoffcharge
INNER JOIN contract_details ON contract_details.[ContractId] = contract_productdetails_oneoffcharge.[ContractId]

