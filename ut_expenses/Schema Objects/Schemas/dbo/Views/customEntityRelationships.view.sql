create view [dbo].[customEntityRelationships]
as
select 
[customEntityAttributes].attributeid, 
[customEntityAttributes].entityid,
(select top 1 mastertablename from [customEntities] where [customEntities].entityid = [customEntityAttributes].entityid) as parent_mastertablename,
(select top 1 tableid from [customEntities] where [customEntities].entityid = [customEntityAttributes].entityid) as parent_tableid,
(select top 1 fieldid from CustomerFields where tableid = (select top 1 tableid from [customEntities] where [customEntities].entityid = [customEntityAttributes].entityid) and idfield = 1) as parent_fieldid, 
(select top 1 field from CustomerFields where tableid = (select top 1 tableid from [customEntities] where [customEntities].entityid = [customEntityAttributes].entityid) and idfield = 1) as parent_fieldname, 
[customEntityAttributes].display_name, 
[customEntityAttributes].relatedtable, 
[customEntityAttributes].fieldid as related_field, 
(select top 1 field from CustomerFields where fieldid = [customEntityAttributes].fieldid) as related_fieldname,
[customEntityAttributes].related_entity as related_entity
from [customEntityAttributes]
where [customEntityAttributes].fieldtype = 9 and [customEntityAttributes].relationshiptype=2