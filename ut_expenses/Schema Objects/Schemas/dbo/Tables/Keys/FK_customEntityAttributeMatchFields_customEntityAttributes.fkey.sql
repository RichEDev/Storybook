ALTER TABLE dbo.customEntityAttributeMatchFields
ADD CONSTRAINT [FK_customEntityAttributeMatchFields_customEntityAttributes] FOREIGN KEY (attributeId) REFERENCES customEntityAttributes (attributeId) ON DELETE CASCADE
