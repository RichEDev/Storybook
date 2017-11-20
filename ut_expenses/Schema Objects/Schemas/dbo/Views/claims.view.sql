-- View

CREATE VIEW [dbo].[claims]
AS
SELECT     claims_base.claimid, claims_base.claimno, claims_base.employeeid, claims_base.approved, claims_base.paid, claims_base.datesubmitted, claims_base.datepaid, claims_base.description, claims_base.status, claims_base.teamid, claims_base.checkerid, claims_base.stage, claims_base.submitted,claims_base.name, claims_base.currencyid, claims_base.referencenumber, claims_base.splitapprovalstage,claims_base.createdon,claims_base.createdby,claims_base.modifiedon,claims_base.modifiedby,
sum(case when primaryitem = 1 then 1 else 0 end) as numberofitems,
max(savedexpenses.date) as enddate,
min (savedexpenses.date) as startdate,
    sum(round(savedexpenses.total,2)) as total,
    SUM(round(savedexpenses.amountpayable,2)) as amountpayable,
    SUM(round(savedexpenses.vat,2)) as vat,
    SUM(round(savedexpenses.net,2)) as net,
    case (select count(claimid) from claimhistory where claimid = claims_base.claimid) when 0 then cast(0 as bit) else cast(1 as bit) end as hasHistory,
    (select count(signoffid) from signoffs inner join groups on groups.groupid = signoffs.groupid inner join employees on employees.groupid = groups.groupid where employeeid = claims_base.employeeid) as totalStageCount,
    case (select count(savedexpenses.expenseid) from savedexpenses inner join returnedexpenses on returnedexpenses.expenseid = savedexpenses.expenseid where returned = 1 and corrected = 0 and claimid = claims_base.claimid) when 0 then cast(0 as bit) else cast(1 as bit) end as hasReturnedItems,
    case sum (case when itemtype = 1 then 1 else 0 end) when 0 then cast(0 as bit) else cast(1 as bit) end as hasCashItems,
    case sum (case when itemtype = 2 then 1 else 0 end) when 0 then cast(0 as bit) else cast(1 as bit) end as hasCreditCardItems,
    case sum (case when itemtype = 3 then 1 else 0 end) when 0 then cast(0 as bit) else cast(1 as bit) end as hasPurchaseCardItems,
    case (select count(savedexpenses.expenseid) from savedexpensesFlags inner join savedexpenses on savedexpenses.expenseid = savedexpensesFlags.expenseid where savedexpenses.claimid = claims_base.claimid) when 0 then cast(0 as bit) else cast(1 as bit) end as hasFlaggedItems,
    sum(case when normalreceipt=1 and primaryitem = 1 THEN 1 ELSE 0 END) as numberOfReceipts,
    sum(case when tempallow=0 and primaryitem = 1  AND ISNULL(Edited, 0) <> 1 THEN 1 ELSE 0 END) as numberOfUnapprovedItems,
    sum(case when itemtype = 2 then total else 0 END) as creditCardTotal,
    sum(case when itemtype = 3 then total else 0 END) as purchaseCardTotal,	
	sum(case when subcats.reimbursable = 0 then 0 else round(savedexpenses.vat,2) end) as totalReimbursableVAT,
	sum(case when subcats.reimbursable = 0 then 0 else round(savedexpenses.net,2) end) as totalReimbursableNET,
    case (select signofftype from signoffs inner join groups on groups.groupid = signoffs.groupid inner join employees on employees.groupid = groups.groupid where employeeid = claims_base.employeeid and stage = claims_base.stage)
  when 1 then (select surname + ', ' + title + ' ' + firstname from employees where employeeid = claims_base.checkerid)
  when 2 then (select surname + ', ' + title + ' ' + firstname from employees where employeeid = claims_base.checkerid)
  when 6 then
      case when checkerid is null then (select '')
            else
                  case when teamid is null then (select surname + ', ' + title + ' ' + firstname from employees where employeeid = claims_base.checkerid ) else (select teamname from teams where teamid = teamid) end
      end
  when 7 then (select surname + ', ' + title + ' ' + firstname from employees where employeeid = claims_base.checkerid)
  when 5 then (select surname + ', ' + title + ' ' + firstname from employees where employeeid = claims_base.checkerid)
  when 9 then 'Assignment Supervisor'
  when 8 then 'Cost Code Owner(s)'
  when 4 then (select surname + ', ' + title + ' ' + firstname from employees where employeeid = claims_base.checkerid)
end as currentApprover,
  max(savedexpenses.date) as maxdate,
min (savedexpenses.date) as mindate,
PayBeforeValidate
FROM         dbo.claims_base
left join savedexpenses on savedexpenses.claimid = claims_base.claimid
left join subcats on savedexpenses.subcatid = subcats.subcatid
group by claims_base.claimid, claims_base.claimno, claims_base.employeeid, claims_base.approved, claims_base.paid, claims_base.datesubmitted, claims_base.datepaid, claims_base.description, claims_base.status, claims_base.teamid, claims_base.checkerid, claims_base.stage, claims_base.submitted,claims_base.name, claims_base.currencyid, claims_base.referencenumber, claims_base.splitapprovalstage,claims_base.createdon,claims_base.createdby,claims_base.modifiedon,claims_base.modifiedby, PayBeforeValidate