ALTER TABLE [dbo].customEntityAttributes
	ADD CONSTRAINT [FK_customEntityAttributes_customEntityAttributes_triggerAttributeId] 
	FOREIGN KEY (triggerAttributeId)
	REFERENCES customEntityAttributes (attributeId)

