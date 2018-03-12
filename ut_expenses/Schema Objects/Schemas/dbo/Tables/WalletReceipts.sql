﻿CREATE TABLE WalletReceipts 
(
	WalletReceiptId INT NOT NULL IDENTITY(1,1) 
	,FileExtension NVARCHAR(6) NOT NULL
	,Status TINYINT NOT NULL
	,CreatedOn DATETIME NOT NULL DEFAULT GetDate()
	,CreatedBy INT NOT NULL
)