create view [dbo].[custom_entity_relationships]
as
select 
custom_entity_attributes.attributeid, 
custom_entity_attributes.entityid,
(select top 1 plural_name from custom_entities where custom_entities.entityid = custom_entity_attributes.entityid) as parent_pluralname,
(select top 1 tableid from custom_entities where custom_entities.entityid = custom_entity_attributes.entityid) as parent_tableid,
(select top 1 fieldid from fields where tableid = (select top 1 tableid from custom_entities where custom_entities.entityid = custom_entity_attributes.entityid) and idfield = 1) as parent_fieldid, 
(select top 1 field from fields where tableid = (select top 1 tableid from custom_entities where custom_entities.entityid = custom_entity_attributes.entityid) and idfield = 1) as parent_fieldname, 
custom_entity_attributes.attribute_name, 
custom_entity_attributes.relatedtable, 
custom_entity_attributes.fieldid as related_field, 
(select top 1 field from fields where fieldid = custom_entity_attributes.fieldid) as related_fieldname,
custom_entity_attributes.related_entity as related_entity
from custom_entity_attributes
where custom_entity_attributes.fieldtype = 9 and custom_entity_attributes.relationshiptype=2
