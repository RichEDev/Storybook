CREATE PROCEDURE [dbo].[saveBankAccount] (
	@BankAccountId INT
	,@EmployeeId INT
	,@AccountName VARCHAR(max)
	,@AccountNumber VARCHAR(max)
	,@AccountType INT
	,@SortCode VARCHAR(max)
	,@Reference VARCHAR(max)
	,@CurrencyId INT
	,@requestorEmployeeId INT
	,@CountryId INT
	,@SwiftCode nvarchar(50)
    ,@Iban nvarchar(50)
	)
AS
BEGIN
	DECLARE @newBankAccountID INT;
	DECLARE @accountNameExist INT;
	DECLARE @BankAccountElement INT

	SELECT @BankAccountElement = elementID
	FROM Elements
	WHERE elementFriendlyName = 'Bank Accounts'

	SET @accountNameExist = (
			SELECT count(*)
			FROM BankAccounts
			WHERE dbo.getDecryptedValue(AccountName) = @AccountName
				AND EmployeeId = @EmployeeId
			)

	IF @BankAccountId = 0
		AND @accountNameExist = 0
	BEGIN
		INSERT INTO BankAccounts (
			EmployeeId
			,AccountName
			,AccountNumber
			,AccountType
			,SortCode
			,Reference
			,CurrencyId
			,CreatedOn
			,CreatedBy
			,CountryId
			,SwiftCode
			,Iban
			)
		VALUES (
			@EmployeeId
			,dbo.getEncryptedValue(@AccountName)
			,dbo.getEncryptedValue(@AccountNumber)
			,@AccountType
			,dbo.getEncryptedValue(@SortCode)
			,dbo.getEncryptedValue(@Reference)
			,@CurrencyId
			,GETUTCDATE()
			,@requestorEmployeeId
			,@CountryId
			,dbo.getEncryptedValue(@SwiftCode)
			,dbo.getEncryptedValue(@Iban)
			);

		SET @newBankAccountID = @@IDENTITY;

		IF @newBankAccountID > 0
		BEGIN
			EXEC addInsertEntryToAuditLog @EmployeeId
				,@requestorEmployeeId
				,@BankAccountElement
				,@newBankAccountID
				,@AccountName
				,NULL;
		END
	END
	ELSE IF @BankAccountId = 0
		AND @accountNameExist > 0
	BEGIN
		SET @newBankAccountID = - 1;
	END
	ELSE
	BEGIN
		DECLARE @oldAccountName NVARCHAR(100);
		DECLARE @RecordName NVARCHAR(100);

		SELECT @oldAccountName = dbo.getDecryptedValue(AccountName)
		FROM BankAccounts
		WHERE BankAccountId = @BankAccountId;

		IF @oldAccountName <> @AccountName
			AND @accountNameExist > 0
		BEGIN
			SET @newBankAccountID = - 1;
		END
		ELSE
		BEGIN
			DECLARE @oldAccountNumber NVARCHAR(100);
			DECLARE @oldAccountType NVARCHAR(100);
			DECLARE @oldSortCode NVARCHAR(100);
			DECLARE @oldAccountCurrency NVARCHAR(100);
			DECLARE @oldCountry NVARCHAR(100);
			DECLARE @oldReference NVARCHAR(100);
			DECLARE @FieldId NVARCHAR(2000);
			DECLARE @OldSwiftCode NVARCHAR(100);
			DECLARE @OldIban NVARCHAR(100);

			SELECT @FieldId = fieldid
			FROM fields
			WHERE tableid = '4AA56947-597A-4C91-99C8-5645561C6D01'
				AND description = 'Country'

			SELECT @oldAccountName = dbo.getDecryptedValue(AccountName)
				,@oldAccountType = AccountType
				,@oldSortCode = dbo.getDecryptedValue(SortCode)
				,@oldAccountNumber = dbo.getDecryptedValue(AccountNumber)
				,@oldAccountCurrency = CurrencyId
				,@oldCountry = CountryId
				,@oldReference = dbo.getDecryptedValue(Reference)
				,@OldIban = dbo.getDecryptedValue(Iban),@OldSwiftCode = dbo.getDecryptedValue(SwiftCode)
			FROM BankAccounts
			WHERE BankAccountId = @BankAccountId;

			UPDATE BankAccounts
			SET EmployeeId = @EmployeeId
				,CountryId = @CountryId
				,AccountName = dbo.getEncryptedValue(@AccountName)
				,AccountNumber = dbo.getEncryptedValue(@AccountNumber)
				,AccountType = @AccountType
				,SortCode = dbo.getEncryptedValue(@SortCode)
				,Reference = dbo.getEncryptedValue(@Reference)
				,CurrencyId = @CurrencyId
				,modifiedOn = GETUTCDATE()
				,modifiedBy = @requestorEmployeeId
				,SwiftCode = dbo.getEncryptedValue(@SwiftCode)
				,Iban = dbo.getEncryptedValue(@Iban)
			WHERE BankAccountId = @BankAccountId;

			SET @newBankAccountID = @BankAccountId;

			SELECT @RecordName = dbo.getDecryptedValue(AccountName)
			FROM BankAccounts
			WHERE BankAccountId = @BankAccountId;

			IF @oldAccountName <> @AccountName
			BEGIN
				EXEC addUpdateEntryToAuditLog @requestorEmployeeId
					,NULL
					,@BankAccountElement
					,@BankAccountId
					,'AD3452FF-C8F1-4353-8A6B-A7AC82554994'
					,@oldAccountName
					,@AccountName
					,@RecordName
					,NULL;
			END

			IF @oldAccountNumber <> @AccountNumber
			BEGIN
				EXEC addUpdateEntryToAuditLog @requestorEmployeeId
					,NULL
					,@BankAccountElement
					,@BankAccountId
					,'DDA2C25B-D573-4D89-979D-5D1509917D4B'
					,@oldAccountNumber
					,@AccountNumber
					,@RecordName
					,NULL;
			END

			IF @oldAccountType <> @AccountType
			BEGIN
				EXEC addUpdateEntryToAuditLog @requestorEmployeeId
					,NULL
					,@BankAccountElement
					,@BankAccountId
					,'CD9C8630-0417-4B4D-967B-AC9D033FC754'
					,@oldAccountType
					,@AccountType
					,@RecordName
					,NULL;
			END

			IF @oldAccountCurrency <> @CurrencyId
			BEGIN
				EXEC addUpdateEntryToAuditLog @requestorEmployeeId
					,NULL
					,@BankAccountElement
					,@BankAccountId
					,'393A5146-2893-41B8-BBC1-097E26C7E271'
					,@oldAccountCurrency
					,@CurrencyId
					,@RecordName
					,NULL;
			END

			IF @oldSortCode <> @SortCode
			BEGIN
				EXEC addUpdateEntryToAuditLog @requestorEmployeeId
					,NULL
					,@BankAccountElement
					,@BankAccountId
					,'AC1624EA-2F5F-4427-9555-12F2DB7D0E52'
					,@oldSortCode
					,@SortCode
					,@RecordName
					,NULL;
			END

			IF @oldCountry <> @CountryId
			BEGIN
				EXEC addUpdateEntryToAuditLog @requestorEmployeeId
					,NULL
					,@BankAccountElement
					,@BankAccountId
					,@FieldId
					,@oldCountry
					,@CountryId
					,@RecordName
					,NULL;
			END

			IF @oldReference <> @Reference
			BEGIN
				EXEC addUpdateEntryToAuditLog @requestorEmployeeId
					,NULL
					,@BankAccountElement
					,@BankAccountId
					,'4C6FF468-A5B5-41E6-94F5-FB2CC7770889'
					,@oldReference
					,@Reference
					,@RecordName
					,NULL;
			END
			 IF @OldIban <> @Iban
   BEGIN
    EXEC addUpdateEntryToAuditLog @requestorEmployeeId, null, @BankAccountElement, @BankAccountId, '4C6FF468-A5B5-41E6-94F5-FB2CC7770889', @OldIban, @Iban, @RecordName, null;
   END
   IF @OldSwiftCode <> @SwiftCode
   BEGIN
    EXEC addUpdateEntryToAuditLog @requestorEmployeeId, null, @BankAccountElement, @BankAccountId, '4C6FF468-A5B5-41E6-94F5-FB2CC7770889', @OldSwiftCode, @SwiftCode, @RecordName, null;
   END
		END
	END

	RETURN @newBankAccountID;
END


