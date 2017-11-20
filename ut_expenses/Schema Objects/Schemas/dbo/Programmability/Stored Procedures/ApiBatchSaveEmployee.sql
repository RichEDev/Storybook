
CREATE PROCEDURE [dbo].[ApiBatchSaveEmployee] @list ApiBatchSaveEmployeeType READONLY
AS
BEGIN
	DECLARE @index INT
	DECLARE @count INT
	DECLARE @employeeid INT
	DECLARE @username NVARCHAR(50)
	DECLARE @password NVARCHAR(250)
	DECLARE @title NVARCHAR(30)
	DECLARE @firstname NVARCHAR(150)
	DECLARE @surname NVARCHAR(150)
	DECLARE @mileagetotal INT
	DECLARE @email NVARCHAR(250)
	DECLARE @currefnum INT
	DECLARE @curclaimno INT
	DECLARE @speedo INT
	DECLARE @payroll NVARCHAR(50)
	DECLARE @position NVARCHAR(250)
	DECLARE @telno NVARCHAR(50)
	DECLARE @creditor NVARCHAR(50)
	DECLARE @archived BIT
	DECLARE @groupid INT
	DECLARE @roleid INT
	DECLARE @hint NVARCHAR(1000)
	DECLARE @lastchange SMALLDATETIME
	DECLARE @additems INT
	DECLARE @cardnum NVARCHAR(50)
	DECLARE @userole BIT
	DECLARE @costcodeid INT
	DECLARE @departmentid INT
	DECLARE @extension NVARCHAR(50)
	DECLARE @pagerno NVARCHAR(50)
	DECLARE @mobileno NVARCHAR(50)
	DECLARE @faxno NVARCHAR(50)
	DECLARE @homeemail NVARCHAR(250)
	DECLARE @linemanager INT
	DECLARE @advancegroupid INT
	DECLARE @mileage INT
	DECLARE @mileageprev INT
	DECLARE @customiseditems BIT
	DECLARE @active BIT
	DECLARE @primarycountry INT
	DECLARE @primarycurrency INT
	DECLARE @verified BIT
	DECLARE @licenceexpiry DATETIME
	DECLARE @licencelastchecked DATETIME
	DECLARE @licencecheckedby INT
	DECLARE @licencenumber NVARCHAR(50)
	DECLARE @groupidcc INT
	DECLARE @groupidpc INT
	DECLARE @CreatedOn DATETIME
	DECLARE @CreatedBy INT
	DECLARE @ModifiedOn DATETIME
	DECLARE @ModifiedBy INT
	DECLARE @country NVARCHAR(100)
	DECLARE @ninumber NVARCHAR(50)
	DECLARE @maidenname NVARCHAR(150)
	DECLARE @middlenames NVARCHAR(150)
	DECLARE @gender NVARCHAR(6)
	DECLARE @dateofbirth DATETIME
	DECLARE @hiredate DATETIME
	DECLARE @terminationdate DATETIME
	DECLARE @homelocationid INT
	DECLARE @officelocationid INT
	DECLARE @applicantnumber NVARCHAR(50)
	DECLARE @applicantactivestatusflag BIT
	DECLARE @passwordMethod TINYINT
	DECLARE @name NVARCHAR(50)
	DECLARE @accountnumber NVARCHAR(50)
	DECLARE @accounttype NVARCHAR(50)
	DECLARE @sortcode NVARCHAR(50)
	DECLARE @reference NVARCHAR(50)
	DECLARE @localeID INT
	DECLARE @NHSTrustID INT
	DECLARE @logonCount INT
	DECLARE @retryCount INT
	DECLARE @firstLogon BIT
	DECLARE @licenceAttachID INT
	DECLARE @defaultSubAccountId INT
	DECLARE @CacheExpiry DATETIME
	DECLARE @supportPortalAccountID INT
	DECLARE @supportPortalPassword NVARCHAR(250)
	DECLARE @CreationMethod TINYINT
	DECLARE @mileagetotaldate DATETIME
	DECLARE @adminonly BIT
	DECLARE @locked BIT
	DECLARE @ESRPersonId BIGINT
	DECLARE @preferredname NVARCHAR(150)
	DECLARE @employeenumber NVARCHAR(30)
	DECLARE @esrpersontype NVARCHAR(80)
	DECLARE @nhsuniqueid NVARCHAR(15)
	DECLARE @esreffectivestartdate DATETIME
	DECLARE @esreffectiveenddate DATETIME
	DECLARE @tmp TABLE (
		tmpID INT
		,OriginalEmployeeId INT
		,employeeId INT
		,ReturnCode INT
		,ESRPersonId BIGINT
		)

	INSERT @tmp
	SELECT ROW_NUMBER() OVER (
			ORDER BY employeeId
			)
		,employeeid
		,employeeid
		,0
		,ESRPersonId
	FROM @list

	SELECT @count = count(*)
	FROM @tmp

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SET @ESRPersonId = (
				SELECT TOP 1 EsrPersonId
				FROM @tmp
				WHERE tmpID = @index
				)

		SELECT TOP 1 @employeeid = employeeid
			,@username = username
			,@password = [password]
			,@title = title
			,@firstname = firstname
			,@surname = surname
			,@mileagetotal = mileagetotal
			,@email = email
			,@currefnum = currefnum
			,@curclaimno = curclaimno
			,@speedo = speedo
			,@payroll = payroll
			,@position = position
			,@telno = telno
			,@creditor = creditor
			,@archived = archived
			,@groupid = groupid
			,@roleid = roleid
			,@hint = hint
			,@lastchange = lastchange
			,@additems = additems
			,@cardnum = cardnum
			,@userole = userole
			,@costcodeid = costcodeid
			,@departmentid = departmentid
			,@extension = extension
			,@pagerno = pagerno
			,@mobileno = mobileno
			,@faxno = faxno
			,@homeemail = homeemail
			,@linemanager = linemanager
			,@advancegroupid = advancegroupid
			,@mileage = mileage
			,@mileageprev = mileageprev
			,@customiseditems = customiseditems
			,@active = active
			,@primarycountry = primarycountry
			,@primarycurrency = primarycurrency
			,@verified = verified
			,@licenceexpiry = licenceexpiry
			,@licencelastchecked = licencelastchecked
			,@licencecheckedby = licencecheckedby
			,@licencenumber = licencenumber
			,@groupidcc = groupidcc
			,@groupidpc = groupidpc
			,@CreatedOn = CreatedOn
			,@CreatedBy = CreatedBy
			,@ModifiedOn = ModifiedOn
			,@ModifiedBy = ModifiedBy
			,@country = country
			,@ninumber = ninumber
			,@maidenname = maidenname
			,@middlenames = middlenames
			,@gender = gender
			,@dateofbirth = dateofbirth
			,@hiredate = hiredate
			,@terminationdate = terminationdate
			,@homelocationid = homelocationid
			,@officelocationid = officelocationid
			,@applicantnumber = applicantnumber
			,@applicantactivestatusflag = applicantactivestatusflag
			,@passwordMethod = passwordMethod
			,@name = NAME
			,@accountnumber = accountnumber
			,@accounttype = accounttype
			,@sortcode = sortcode
			,@reference = reference
			,@localeID = localeID
			,@NHSTrustID = NHSTrustID
			,@logonCount = logonCount
			,@retryCount = retryCount
			,@firstLogon = firstLogon
			,@licenceAttachID = licenceAttachID
			,@defaultSubAccountId = defaultSubAccountId
			,@CacheExpiry = CacheExpiry
			,@supportPortalAccountID = supportPortalAccountID
			,@supportPortalPassword = supportPortalPassword
			,@CreationMethod = CreationMethod
			,@mileagetotaldate = mileagetotaldate
			,@adminonly = adminonly
			,@locked = locked
			,@preferredname = preferredname
			,@employeenumber = employeenumber
			,@esrpersontype = esrpersontype
			,@nhsuniqueid = nhsuniqueid
			,@esreffectivestartdate = esreffectivestartdate
			,@esreffectiveenddate = esreffectiveenddate
		FROM @list
		WHERE ESRPersonId = @ESRPersonId

		DECLARE @counter INT;

		IF @employeeid = 0
		BEGIN
			SET @counter = (
					SELECT count(*)
					FROM employees
					WHERE username = @username
					)

			IF @counter > 0
				UPDATE @tmp
				SET ReturnCode = - 1
				WHERE tmpID = @index;
			ELSE
			BEGIN
				SELECT TOP 1 @defaultSubAccountId = subAccountID
				FROM accountsSubAccounts
				IF @password = '' SET @password = '8whQ2Q';
				INSERT INTO [dbo].[employees] (
					[username]
					,[password]
					,[title]
					,[firstname]
					,[surname]
					,[mileagetotal]
					,[email]
					,[currefnum]
					,[curclaimno]
					,[speedo]
					,[payroll]
					,[position]
					,[telno]
					,[creditor]
					,[archived]
					,[groupid]
					,[roleid]
					,[hint]
					,[lastchange]
					,[additems]
					,[cardnum]
					,[userole]
					,[costcodeid]
					,[departmentid]
					,[extension]
					,[pagerno]
					,[mobileno]
					,[faxno]
					,[homeemail]
					,[linemanager]
					,[advancegroupid]
					,[mileage]
					,[mileageprev]
					,[customiseditems]
					,[active]
					,[primarycountry]
					,[primarycurrency]
					,[verified]
					,[licenceexpiry]
					,[licencelastchecked]
					,[licencecheckedby]
					,[licencenumber]
					,[groupidcc]
					,[groupidpc]
					,[CreatedOn]
					,[CreatedBy]
					,[ModifiedOn]
					,[ModifiedBy]
					,[country]
					,[ninumber]
					,[maidenname]
					,[middlenames]
					,[gender]
					,[dateofbirth]
					,[hiredate]
					,[terminationdate]
					,[homelocationid]
					,[officelocationid]
					,[applicantnumber]
					,[applicantactivestatusflag]
					,[passwordMethod]
					,[name]
					,[accountnumber]
					,[accounttype]
					,[sortcode]
					,[reference]
					,[localeID]
					,[NHSTrustID]
					,[logonCount]
					,[retryCount]
					,[firstLogon]
					,[licenceAttachID]
					,[defaultSubAccountId]
					,[CacheExpiry]
					,[supportPortalAccountID]
					,[supportPortalPassword]
					,[CreationMethod]
					,[mileagetotaldate]
					,[adminonly]
					,[locked]
					,[ESRPersonId]
					,[PreferredName]
					,[EmployeeNumber]
					,[NHSUniqueId]
					,[ESRPersonType]
					,[ESREffectiveStartDate]
					,[ESREffectiveEndDate]
					)
				VALUES (
					@username
					,@password
					,@title
					,@firstname
					,@surname
					,@mileagetotal
					,@email
					,@currefnum
					,@curclaimno
					,@speedo
					,@payroll
					,@position
					,@telno
					,@creditor
					,@archived
					,@groupid
					,@roleid
					,@hint
					,@lastchange
					,@additems
					,@cardnum
					,@userole
					,@costcodeid
					,@departmentid
					,@extension
					,@pagerno
					,@mobileno
					,@faxno
					,@homeemail
					,@linemanager
					,@advancegroupid
					,@mileage
					,@mileageprev
					,@customiseditems
					,@active
					,@primarycountry
					,@primarycurrency
					,@verified
					,@licenceexpiry
					,@licencelastchecked
					,@licencecheckedby
					,@licencenumber
					,@groupidcc
					,@groupidpc
					,@CreatedOn
					,@CreatedBy
					,@ModifiedOn
					,@ModifiedBy
					,@country
					,@ninumber
					,@maidenname
					,@middlenames
					,@gender
					,@dateofbirth
					,@hiredate
					,@terminationdate
					,@homelocationid
					,@officelocationid
					,@applicantnumber
					,@applicantactivestatusflag
					,@passwordMethod
					,@name
					,@accountnumber
					,@accounttype
					,@sortcode
					,@reference
					,@localeID
					,@NHSTrustID
					,@logonCount
					,@retryCount
					,@firstLogon
					,@licenceAttachID
					,@defaultSubAccountId
					,@CacheExpiry
					,@supportPortalAccountID
					,@supportPortalPassword
					,@CreationMethod
					,@mileagetotaldate
					,@adminonly
					,@locked
					,@ESRPersonId
					,@preferredname
					,@employeenumber
					,@nhsuniqueid
					,@esrpersontype
					,@esreffectivestartdate
					,@esreffectiveenddate
					)

				SET @employeeid = scope_identity();

				UPDATE @tmp
				SET employeeId = @employeeid
				WHERE tmpID = @index;
			END
		END
		ELSE
		BEGIN
			SET @counter = (
					SELECT count(*)
					FROM employees
					WHERE username = @username
						AND employeeid <> @employeeid
					)

			IF @counter > 0
				UPDATE @tmp
				SET ReturnCode = - 1
				WHERE tmpID = @index;

			DECLARE @oldgroupid INT = null;

			SELECT @oldgroupid = groupid
			FROM employees
			WHERE employeeid = @employeeid;

			IF @oldgroupid <> @groupid
			BEGIN
				SET @counter = (
						SELECT count(*)
						FROM claims
						WHERE employeeid = @employeeid
							AND submitted = 1
							AND paid = 0
						)

				IF @counter > 0
					UPDATE @tmp
					SET ReturnCode = - 2
					WHERE tmpID = @index;
			END

			UPDATE [dbo].[employees]
			SET [username] = @username
				,[password] = @password
				,[title] = @title
				,[firstname] = @firstname
				,[surname] = @surname
				,[mileagetotal] = @mileagetotal
				,[email] = @email
				,[currefnum] = @currefnum
				,[curclaimno] = @curclaimno
				,[speedo] = @speedo
				,[payroll] = @payroll
				,[position] = @position
				,[telno] = @telno
				,[creditor] = @creditor
				,[archived] = @archived
				,[groupid] = @groupid
				,[roleid] = @roleid
				,[hint] = @hint
				,[lastchange] = @lastchange
				,[additems] = @additems
				,[cardnum] = @cardnum
				,[userole] = @userole
				,[costcodeid] = @costcodeid
				,[departmentid] = @departmentid
				,[extension] = @extension
				,[pagerno] = @pagerno
				,[mobileno] = @mobileno
				,[faxno] = @faxno
				,[homeemail] = @homeemail
				,[linemanager] = @linemanager
				,[advancegroupid] = @advancegroupid
				,[mileage] = @mileage
				,[mileageprev] = @mileageprev
				,[customiseditems] = @customiseditems
				,[active] = @active
				,[primarycountry] = @primarycountry
				,[primarycurrency] = @primarycurrency
				,[verified] = @verified
				,[licenceexpiry] = @licenceexpiry
				,[licencelastchecked] = @licencelastchecked
				,[licencecheckedby] = @licencecheckedby
				,[licencenumber] = @licencenumber
				,[groupidcc] = @groupidcc
				,[groupidpc] = @groupidpc
				,[CreatedOn] = @CreatedOn
				,[CreatedBy] = @CreatedBy
				,[ModifiedOn] = @ModifiedOn
				,[ModifiedBy] = @ModifiedBy
				,[country] = @country
				,[ninumber] = @ninumber
				,[maidenname] = @maidenname
				,[middlenames] = @middlenames
				,[gender] = @gender
				,[dateofbirth] = @dateofbirth
				,[hiredate] = @hiredate
				,[terminationdate] = @terminationdate
				,[homelocationid] = @homelocationid
				,[officelocationid] = @officelocationid
				,[applicantnumber] = @applicantnumber
				,[applicantactivestatusflag] = @applicantactivestatusflag
				,[passwordMethod] = @passwordMethod
				,[name] = @name
				,[accountnumber] = @accountnumber
				,[accounttype] = @accounttype
				,[sortcode] = @sortcode
				,[reference] = @reference
				,[localeID] = @localeID
				,[NHSTrustID] = @NHSTrustID
				,[logonCount] = @logonCount
				,[retryCount] = @retryCount
				,[firstLogon] = @firstLogon
				,[licenceAttachID] = @licenceAttachID
				,[defaultSubAccountId] = @defaultSubAccountId
				,[CacheExpiry] = @CacheExpiry
				,[supportPortalAccountID] = @supportPortalAccountID
				,[supportPortalPassword] = @supportPortalPassword
				,[CreationMethod] = @CreationMethod
				,[mileagetotaldate] = @mileagetotaldate
				,[adminonly] = @adminonly
				,[locked] = @locked
				,[ESRPersonId] = @ESRPersonId
				,[PreferredName] = @preferredname
				,[EmployeeNumber] = @employeenumber
				,[NHSUniqueId] = @nhsuniqueid
				,[ESRPersonType] = @esrpersontype
				,[ESREffectiveStartDate] = @esreffectivestartdate
				,[ESREffectiveEndDate] = @esreffectiveenddate
			WHERE [employeeid] = @employeeid
		END

		SET @index = @index + 1
	END

	-- reduce the returning record set for performance increase
	DELETE
	FROM @tmp
	WHERE OriginalEmployeeId = employeeId
		AND ReturnCode = 0;

	SELECT *
	FROM @tmp;

	RETURN 0;
END