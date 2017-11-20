/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

/* delete duplicate entry of CreationMethod field in fields_base for the employees table */
/*delete from fields_base where fieldid = 'E440C02D-7B68-4986-8225-1EDA887E9EA1';

if not exists (select * from fields_base where tableid = 'F07E080B-F2AE-452B-90B3-7D2530BAAE17' and field = 'maxRows')
begin
	insert into fields_base (fieldid, field, fieldtype, description, normalview, idfield, genlist, width, cantotal, printout,valuelist,allowimport,mandatory,amendedon,useforlookup,workflowUpdate,workflowSearch,length,tableid,IsForeignKey)
	values
	('D5E74321-DC00-47C5-8DDB-6BA1EAEB4D78','maxRows', 'N','Number of match rows returned', 0, 0, 0, 0, 0, 0, 0, 0, 0, GETDATE(), 0,0,0,0,'F07E080B-F2AE-452B-90B3-7D2530BAAE17',0)
end
go

if not exists (select * from fields_base where tableid = '0304DF31-45B5-41F4-9F08-3CCA2887E271' and field = 'maxRows')
begin
	insert into fields_base (fieldid, field, fieldtype, description, normalview, idfield, genlist, width, cantotal, printout,valuelist,allowimport,mandatory,amendedon,useforlookup,workflowUpdate,workflowSearch,length,tableid,IsForeignKey)
	values
	('B8ED3D1A-1CE5-4C6A-AA16-094A003E3BB2','maxRows', 'N','Number of match rows returned', 0, 0, 0, 0, 0, 0, 0, 0, 0, GETDATE(), 0,0,0,0,'0304DF31-45B5-41F4-9F08-3CCA2887E271',0)
end
go

if not exists (select * from fields_base where tableid = 'F07E080B-F2AE-452B-90B3-7D2530BAAE17' and field = 'displayField')
begin
	insert into fields_base
	(fieldid, tableid, field, fieldtype, description, normalview, idfield, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon)
	values
	('69166EFE-BA0C-4750-8883-6D4C750D64B3', 'F07E080B-F2AE-452B-90B3-7D2530BAAE17', 'displayField', 'U', 'Field displayed in auto complete list for relationship fields', 0, 0, 0, 0, 0, 0, 0, 0, 0, getdate())
end
go

if not exists (select * from fields_base where tableid = '0304DF31-45B5-41F4-9F08-3CCA2887E271' and field = 'relationshipDisplayField')
begin
	insert into fields_base
	(fieldid, tableid, field, fieldtype, description, normalview, idfield, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon)
	values
	('7E16B657-5EDD-4B69-B440-99E9CBD22A08', '0304DF31-45B5-41F4-9F08-3CCA2887E271', 'relationshipDisplayField', 'U', 'Field displayed in auto complete list for relationship fields', 0, 0, 0, 0, 0, 0, 0, 0, 0, getdate())
end
go

update tables_base set description = 'Expense Categories' where tableid = '75C247C2-457E-4B14-BBEC-1391CD77FB9E';
go

if not exists (select * from fields_base where fieldid='EE862456-B9C8-4B8B-8A71-174CEE18F1C3')
	insert into fields_base ([fieldid]
           ,[field]
           ,[fieldtype]
           ,[description]
           ,[comment]
           ,[normalview]
           ,[idfield]
           ,[viewgroupid_old]
           ,[genlist]
           ,[width]
           ,[cantotal]
           ,[printout]
           ,[valuelist]
           ,[allowimport]
           ,[mandatory]
           ,[amendedon]
           ,[lookuptable]
           ,[lookupfield]
           ,[useforlookup]
           ,[workflowUpdate]
           ,[workflowSearch]
           ,[length]
           ,[tableid]
           ,[viewgroupid]
           ,[relabel]
           ,[relabel_param]
           ,[fieldid_old]
           ,[allowDuplicateChecking]
           ,[classPropertyName]
           ,[IsForeignKey]) 
     values 
			('EE862456-B9C8-4B8B-8A71-174CEE18F1C3', 
			'is_audit_identity', 
			'X', 
			'Used for Audit', 
			null, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 
			'2010-04-20 14:24:53.207', 
			null, null, 0, 1, 1, 0, 
			'0304DF31-45B5-41F4-9F08-3CCA2887E271', 
			null, 0, null, null, 0, null, 0)
else
	update fields_base set description = 'Used for Audit' where fieldid = 'EE862456-B9C8-4B8B-8A71-174CEE18F1C3'
go

if not exists (select * from fields_base where fieldid='EE862456-B9C8-4B8B-8A71-174CEE18F1C3')
	insert into fields_base ([fieldid]
           ,[field]
           ,[fieldtype]
           ,[description]
           ,[comment]
           ,[normalview]
           ,[idfield]
           ,[viewgroupid_old]
           ,[genlist]
           ,[width]
           ,[cantotal]
           ,[printout]
           ,[valuelist]
           ,[allowimport]
           ,[mandatory]
           ,[amendedon]
           ,[lookuptable]
           ,[lookupfield]
           ,[useforlookup]
           ,[workflowUpdate]
           ,[workflowSearch]
           ,[length]
           ,[tableid]
           ,[viewgroupid]
           ,[relabel]
           ,[relabel_param]
           ,[fieldid_old]
           ,[allowDuplicateChecking]
           ,[classPropertyName]
           ,[IsForeignKey]) 
     values 
			('654706A1-05E9-475A-9A0C-0197B162BF5C', 
			'fieldtype', 
			'X', 
			'Type', 
			null, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 
			'2010-04-20 14:24:53.207', 
			null, null, 0, 1, 1, 0, 
			'0304DF31-45B5-41F4-9F08-3CCA2887E271', 
			null, 0, null, null, 0, null, 0)
else
	update fields_base set description = 'Type' where fieldid = '654706A1-05E9-475A-9A0C-0197B162BF5C'
GO

update fields_base set useforlookup = 1 where fieldid in
(
'B5DD3E91-DA63-4E9B-B933-FEC64831D920',
'5F4A4551-1C05-4C85-B6D9-06D036BC327E',
'9C9F07DD-A9D0-4CCF-9231-DD3C10D491B8'
)
GO
*/