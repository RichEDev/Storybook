CREATE VIEW [dbo].[employeeTasks]
AS
SELECT     dbo.tasks.taskId, dbo.tasks.regardingId, tasks.regardingArea, 
                      CASE regardingarea WHEN 1 THEN contract_details.contractdescription WHEN 5 THEN contract_details.contractdescription WHEN 7 THEN contractproductdetails.productname
                       WHEN 2 THEN contractproductdetails.productname WHEN 3 THEN productdetails.productname WHEN 8 THEN employees.firstname + ' ' + employees.surname WHEN
                       13 THEN employees.firstname + ' ' + employees.surname WHEN 4 THEN supplier_details.suppliername WHEN 6 THEN codes_rechargeentity.name WHEN 9 THEN invoices.invoicenumber
                       WHEN 10 THEN CAST(contract_forecastdetails.forecastamount AS nvarchar(25)) WHEN 17 THEN cars.make + ' ' + cars.model END AS regarding, dbo.tasks.subject, 
                      dbo.tasks.description, dbo.tasks.startDate, dbo.tasks.dueDate, dbo.tasks.endDate, 
                      case taskownertype when 3 then taskowners.firstname + ' ' + taskowners.surname
						when 1 then teams.teamname end as taskOwner,
                      dbo.tasks.statusId, dbo.tasks.taskOwnerType, dbo.tasks.taskOwnerId, 
                      dbo.tasks.taskCreatorId, dbo.teamemps.employeeid as teamemployeeid,
					  tasks.subaccountid
FROM         dbo.tasks LEFT OUTER JOIN
                      dbo.contract_details ON dbo.contract_details.contractId = dbo.tasks.regardingId LEFT OUTER JOIN
                      dbo.contract_productdetails ON dbo.contract_productdetails.contractProductId = dbo.tasks.regardingId LEFT OUTER JOIN
                      dbo.productDetails AS contractproductdetails ON dbo.contract_productdetails.productId = contractproductdetails.productId LEFT OUTER JOIN
                      dbo.productDetails ON contractproductdetails.productId = dbo.tasks.regardingId LEFT OUTER JOIN
                      dbo.employees ON dbo.employees.employeeid = dbo.tasks.regardingId LEFT OUTER JOIN
                      employees as taskOwners on taskowners.employeeid = tasks.taskownerid left join 
                      dbo.supplier_details ON dbo.supplier_details.supplierid = dbo.tasks.regardingId LEFT OUTER JOIN
                      dbo.recharge_associations ON dbo.recharge_associations.rechargeId = dbo.tasks.regardingId LEFT OUTER JOIN
                      dbo.codes_rechargeentity ON dbo.codes_rechargeentity.entityId = dbo.recharge_associations.rechargeEntityId LEFT OUTER JOIN
                      dbo.invoices ON dbo.invoices.invoiceID = dbo.tasks.regardingId LEFT OUTER JOIN
                      dbo.contract_forecastdetails ON dbo.contract_forecastdetails.contractForecastId = dbo.tasks.regardingId LEFT OUTER JOIN
                      dbo.cars ON dbo.cars.carid = dbo.tasks.regardingId LEFT OUTER JOIN
                      dbo.teams ON dbo.teams.teamid = dbo.tasks.taskOwnerId LEFT OUTER JOIN
                      dbo.teamemps ON dbo.teams.teamid = dbo.teamemps.teamid