CREATE VIEW [dbo].[ContractAudience]
	AS SELECT audienceid, contractid, CASE WHEN audienceType = 0 THEN accessId END AS EmployeeId, CASE WHEN audienceType = 1 THEN accessId END AS TeamId FROM contract_audience
