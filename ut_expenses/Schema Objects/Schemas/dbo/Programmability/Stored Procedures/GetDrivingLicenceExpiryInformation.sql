CREATE PROCEDURE [dbo].[GetDrivingLicenceExpiryInformation]
@employeeId int,
@expenseItemDate datetime,
@hasDvlaLookupKeyAndDvlaConnectLicenceElement BIT

AS
BEGIN

DECLARE @sqlQuery NVARCHAR(MAX); 
DECLARE @sqlQuery1 NVARCHAR(MAX);
DECLARE @sqlQuery2 NVARCHAR(MAX);
DECLARE @sqlQuery3 NVARCHAR(MAX);
DECLARE @sqlQuery4 NVARCHAR(MAX);
DECLARE @sqlQuery5 NVARCHAR(MAX);
Declare @drivingLicenceTableId varchar(20) =[dbo].[GetEntityId] ('223018FE-EDAE-408E-8851-C09ABA09DF81',1);
Declare @attEmployee varchar(20) =[dbo].[GetAttributeId] ('91596ACE-E16C-4DB3-9E02-776785EB11CF',1);
Declare @attLicenceType varchar(20) =[dbo].[GetAttributeId] ('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE',1);
Declare @attLicenceTypeId varchar(20) =[dbo].[GetAttributeId] ('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE',0);
Declare @attDateOfIssue varchar(20) =[dbo].[GetAttributeId] ('AF7127A6-5F90-4811-9A78-99F4E48A60B1',1);
Declare @attDrivingLicenceId varchar(20) =[dbo].[GetAttributeId] ('C11ADAEB-27E0-4046-843F-7FD39513DACF',1);
Declare @attExpireDate varchar(20) =[dbo].[GetAttributeId] ('292D65BC-75E5-4214-9AF3-CC37661A566B',1);
Declare @attStatus varchar(20) =[dbo].[GetAttributeId] ('EB913CB2-C56B-4461-924E-833C43DD6F65',1); 
Declare @attStatusValue varchar(20) =[dbo].[GetAttributeId] ('EB913CB2-C56B-4461-924E-833C43DD6F65',0); 
declare @reviewstatus varchar(50) =dbo.GetAttributeId('C790EC93-A920-4CFC-8846-605F8B4B50B5',1);
declare @reviewstatusvalue varchar(50) =dbo.GetAttributeId('C790EC93-A920-4CFC-8846-605F8B4B50B5',0);
DECLARE @reviewDateForManualLicence varchar(50) =dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1);

DECLARE @attLicencePhotoCard varchar(10);
DECLARE @attLicencePaper varchar(10);
DECLARE @attLicenceNonGb varchar(10);
DECLARE @attReviewedOK varchar(10);

SELECT @attLicencePhotoCard = cast(valueid as nvarchar(10)) from customEntityAttributeListItems where attributeid = @attLicenceTypeId  and item = 'DVLA Photocard Licence';
SELECT @attLicencePaper = cast(valueid as nvarchar(10)) from customEntityAttributeListItems where attributeid = +@attLicenceTypeId  and item = 'DVLA Pre-1998 Paper Licence';
SELECT @attReviewedOK = cast(valueid as nvarchar(10))FROM customEntityAttributeListItems where attributeid = @attStatusValue and item = 'Reviewed - OK';

if not exists(select 1 from customEntityAttributeListItems where item='Non-GB Licence')
BEGIN
DECLARE @licencetypeid int
set @licencetypeid =dbo.GetAttributeId('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE',0)
insert into customEntityAttributeListItems(attributeid,item,[order],archived) values(@licencetypeid,'Non-GB Licence',(select Max([order])+1 from customEntityAttributeListItems where attributeid =@licencetypeid),0)
END

SELECT @attLicenceNonGb = cast(valueid as nvarchar(10)) from customEntityAttributeListItems where attributeid = +@attLicenceTypeId and item = 'Non-GB Licence';

DECLARE @attReviewedNone varchar(10);
DECLARE @attLicenceDvla varchar(50);
SELECT @attLicenceDvla = cast(valueid as nvarchar(10)) from customEntityAttributeListItems where attributeid = +@attLicenceTypeId  and item = 'Driving Licence (Automatic DVLA Check)';
SELECT @attReviewedNone = cast(valueid as nvarchar(10))FROM customEntityAttributeListItems where attributeid = @attStatusValue and item = '[None]';
declare @drivinglicencereviewentity nvarchar(50) = dbo.GetEntityId('5137C32E-E500-4297-BFF5-69CFED26C9B6',1)
SET @sqlQuery =
'CREATE PROCEDURE [dbo].[GetDrivingLicenceExpiryInformation]
@employeeId int,
@expenseItemDate datetime,
@hasDvlaLookupKeyAndDvlaConnectLicenceElement BIT
AS
BEGIN
	DECLARE @paperlicenceValidFrom DATETIME
	DECLARE @nongblicenceValidFrom DATETIME
	DECLARE @photocardlicenceexpiry DATETIME
	DECLARE @nongblicenceexpiry DATETIME
	DECLARE @blockDrivingLicence BIT
	DECLARE @photocardcheck BIT
	DECLARE @nongblicencecheck BIT
	DECLARE @ReviewStatus varchar(100)
 	
    DECLARE @dvlalicenceexpiry DATETIME
    DECLARE @dvlacheck BIT  
    DECLARE @dvlaLicenceValid BIT = 0  
	Declare @isvalidlicence Bit=1
	DECLARE @IsValidManualLicence Bit = 1
	DECLARE @LastLookUpDate DATETIME
	DECLARE @LatestReviewForManualLicence DATETIME
	DECLARE @StartDateOfPhotoCardLicence DATETIME
	
	DECLARE @DrivingLicenceReviewPeriodically BIT=0
	DECLARE @DrivingLicenceReviewFrequency INT
	DECLARE @DateOfExpense BIT=0
	SELECT @DrivingLicenceReviewPeriodically = stringValue FROM accountProperties WHERE stringKey=''DrivingLicenceReviewPeriodically''
	IF @DrivingLicenceReviewPeriodically =1
	BEGIN
	SELECT @DrivingLicenceReviewFrequency =stringValue FROM accountProperties WHERE stringKey=''DrivingLicenceReviewFrequency''
	END

	SELECT @DateOfExpense =stringValue FROM accountProperties WHERE stringKey=''useDateOfExpenseForDutyOfCareChecks''

	print @hasDvlaLookupKeyAndDvlaConnectLicenceElement
   
    SELECT @dvlacheck = cast(stringValue as varchar) 
	FROM accountProperties 
	WHERE stringKey = ''enableAutomaticDrivingLicenceLookup''
	'

set @sqlQuery2 = 'SELECT @LastLookUpDate = DvlaLookUpDate from employees where employeeid = @employeeId


	SELECT @LatestReviewForManualLicence='+ @reviewDateForManualLicence +' 
				FROM '+@drivingLicenceTableId+' dl
						inner join '+@drivinglicencereviewentity+' review on (dl.'+dbo.GetAttributeId('C11ADAEB-27E0-4046-843F-7FD39513DACF',1)+' =review.'+dbo.GetAttributeId('D602AF08-2471-4035-B92C-50C3A9F492FD',1)+')
				WHERE '+dbo.GetAttributeId('C790EC93-A920-4CFC-8846-605F8B4B50B5',1)+' = '+Convert(nvarchar,(select valueid from customEntityAttributeListItems where attributeid =dbo.GetAttributeId('C790EC93-A920-4CFC-8846-605F8B4B50B5',0) and item ='Reviewed - OK'))+' 
					and '+@attEmployee +' = @employeeid 
					and '+@attLicenceType +' <> '+@attLicenceDvla+ ' 
					and @expenseItemDate between isnull('+@attDateOfIssue +',''2001-jan-01 00:00:00'') and '+ @attExpireDate +'
					and (@DrivingLicenceReviewPeriodically =0 OR (@DateOfExpense=0 and  CAST(GETUTCDATE() AS DATE) between CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE) and DATEADD(M,+@DrivingLicenceReviewFrequency,'+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+')) OR (@DateOfExpense=1 and  CAST(@expenseItemDate AS DATE) between CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE) and DATEADD(M,+@DrivingLicenceReviewFrequency,'+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+'))) 
				order by dl.CreatedOn asc

	SELECT @blockDrivingLicence = cast(stringValue as varchar) 
	FROM accountProperties 
	WHERE stringKey = ''blockDrivingLicence''

		if exists (select * from '+@drivingLicenceTableId+' inner join '+dbo.GetEntityId('9E876ECE-0F84-4387-837D-310DFBD05C34',1)+' on ('+dbo.GetAttributeId('C11ADAEB-27E0-4046-843F-7FD39513DACF',1)+' = '+dbo.GetAttributeId('1AED36BE-FCEC-401B-8DF6-AEAF8FF19F9E',1)+') where '+@attEmployee +' = @employeeid  and '+dbo.GetAttributeId('3DEF57CE-8734-44D5-9AC1-F46D4A60ECF9',1)+' =1 and  ((@expenseItemDate between '+dbo.GetAttributeId('CBCC1986-D25C-4187-893D-6059F76E08F0',1)+' and '+dbo.GetAttributeId('951AE74D-0EC3-44BC-8578-D7F5FC3B86B6',1)+') or (@expenseItemDate >= '+dbo.GetAttributeId('CBCC1986-D25C-4187-893D-6059F76E08F0',1)+' and '+dbo.GetAttributeId('951AE74D-0EC3-44BC-8578-D7F5FC3B86B6',1)+' is null) or (@expenseItemDate <= '+dbo.GetAttributeId('951AE74D-0EC3-44BC-8578-D7F5FC3B86B6',1)+' and '+dbo.GetAttributeId('CBCC1986-D25C-4187-893D-6059F76E08F0',1)+' is null )) and (((@dvlacheck=1 AND @hasDvlaLookupKeyAndDvlaConnectLicenceElement=1)   and '+dbo.GetAttributeId('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE',1)+' = (select valueid from customEntityAttributeListItems where attributeid= '+ Convert(varchar, dbo.GetAttributeId('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE',0)) +' and item=''Driving Licence (Automatic DVLA Check)'')) or (@dvlacheck=0 and '+dbo.GetAttributeId('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE',1)+' in ((select valueid from customEntityAttributeListItems where attributeid= '+Convert(varchar,dbo.GetAttributeId('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE',0))+'))))and @blockDrivingLicence=1)
	begin
	set @isvalidlicence=0
	end

			if exists (select * from '+@drivingLicenceTableId+' inner join '+dbo.GetEntityId('9E876ECE-0F84-4387-837D-310DFBD05C34',1)+' on ('+dbo.GetAttributeId('C11ADAEB-27E0-4046-843F-7FD39513DACF',1)+' = '+dbo.GetAttributeId('1AED36BE-FCEC-401B-8DF6-AEAF8FF19F9E',1)+') where '+@attEmployee +' = @employeeid  and '+dbo.GetAttributeId('3DEF57CE-8734-44D5-9AC1-F46D4A60ECF9',1)+' =1 and ((@expenseItemDate between '+dbo.GetAttributeId('CBCC1986-D25C-4187-893D-6059F76E08F0',1)+' and '+dbo.GetAttributeId('951AE74D-0EC3-44BC-8578-D7F5FC3B86B6',1)+') or (@expenseItemDate >= '+dbo.GetAttributeId('CBCC1986-D25C-4187-893D-6059F76E08F0',1)+' and '+dbo.GetAttributeId('951AE74D-0EC3-44BC-8578-D7F5FC3B86B6',1)+' is null) or (@expenseItemDate <= '+dbo.GetAttributeId('951AE74D-0EC3-44BC-8578-D7F5FC3B86B6',1)+' and '+dbo.GetAttributeId('CBCC1986-D25C-4187-893D-6059F76E08F0',1)+' is null )) and (('+dbo.GetAttributeId('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE',1)+' <> (select valueid from customEntityAttributeListItems where attributeid= '+ Convert(varchar, dbo.GetAttributeId('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE',0)) +' and item=''Driving Licence (Automatic DVLA Check)'')))and @blockDrivingLicence=1)
	begin
	set @isvalidlicence=0
	end

   
   if(@isvalidlicence = 1)
   begin
   	-- Check valid dvla exists regardless of general option enableAutomaticDrivingLicenceLookup

	SELECT TOP 1 @dvlalicenceexpiry='+ @attExpireDate +' , @IsValidManualLicence = 0
	FROM '+@drivingLicenceTableId+' 
	WHERE '+ @attStatus +' = ' +@attReviewedNone+ ' 
		and '+@attEmployee +' = @employeeid 
		and '+@attLicenceType +' = '+@attLicenceDvla+ ' 
		and @expenseItemDate between isnull('+@attDateOfIssue +',''2001-jan-01 00:00:00'') and '+ @attExpireDate +' and @hasDvlaLookupKeyAndDvlaConnectLicenceElement=1 and
		((@DateOfExpense=0 and  CAST(GETUTCDATE() AS DATE) between CAST('+@attDateOfIssue +' AS DATE) and CAST('+ @attExpireDate +' AS DATE)) OR (@DateOfExpense=1 and  CAST(@expenseItemDate AS DATE) between CAST('+@attDateOfIssue +' AS DATE) and  CAST('+@attExpireDate +' AS DATE)))
	order by '+@attExpireDate+' Desc
	'	

	set @sqlQuery3='

        IF(@hasDvlaLookupKeyAndDvlaConnectLicenceElement=1)
		BEGIN	
			IF @dvlalicenceexpiry IS NULL
				BEGIN
					SELECT TOP 1 @dvlalicenceexpiry= '+ @attExpireDate +' 
					FROM '+@drivingLicenceTableId+' 
					WHERE '+ @attStatus +' = ' +@attReviewedNone+ ' 
						and '+@attEmployee +' = @employeeid 
						and '+@attLicenceType +' = ' + @attLicenceDvla + ' 
						and @expenseItemDate > '+ @attExpireDate +' and @hasDvlaLookupKeyAndDvlaConnectLicenceElement=1 
					order by '+@attExpireDate+' Desc
				END
		END	
	    if((@IsValidManualLicence = 1) OR @LatestReviewForManualLicence > = @LastLookUpDate)
		BEGIN
		    --remove invalid dvla document data
		    -- If the DVLA is not enabled, still the driving licence updated from dvla auto lookup should be considered if they are valid			

				SELECT @photocardcheck =1 FROM '+@drivingLicenceTableId+' 
				WHERE '+@attEmployee +' = @employeeid 
					AND '+@attLicenceType +' = ' +@attLicencePhotoCard+ ' 
					
				SELECT @nongblicencecheck =1 FROM '+@drivingLicenceTableId+'
				WHERE '+@attEmployee +' = @employeeid
					AND '+@attLicenceType +' = ' +@attLicenceNonGb+ '
   
				SELECT @paperlicenceValidFrom=isnull('+@attDateOfIssue +',''2001-jan-01 00:00:00'') 
				FROM '+@drivingLicenceTableId+' dl
				inner join '+@drivinglicencereviewentity+' review on (dl.'+dbo.GetAttributeId('C11ADAEB-27E0-4046-843F-7FD39513DACF',1)+' =review.'+dbo.GetAttributeId('D602AF08-2471-4035-B92C-50C3A9F492FD',1)+')
				WHERE '+dbo.GetAttributeId('C790EC93-A920-4CFC-8846-605F8B4B50B5',1)+' = '+Convert(nvarchar,(select valueid from customEntityAttributeListItems where attributeid =dbo.GetAttributeId('C790EC93-A920-4CFC-8846-605F8B4B50B5',0) and item ='Reviewed - OK'))+' 
					and '+@attEmployee +' = @employeeid 
					and '+@attLicenceType +' = ' +@attLicencePaper+ '
					and @expenseItemDate > =isnull('+@attDateOfIssue +',''2001-jan-01 00:00:00'') 
					and (@DrivingLicenceReviewPeriodically =0 OR (@DateOfExpense=0 and  CAST(GETUTCDATE() AS DATE) between CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE) and DATEADD(M,+@DrivingLicenceReviewFrequency,'+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+')) OR (@DateOfExpense=1 and CAST(@expenseItemDate AS DATE) between CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE) and DATEADD(M,+@DrivingLicenceReviewFrequency,'+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+')))
				order by dl.CreatedOn asc

				SELECT @nongblicenceValidFrom=isnull('+@attDateOfIssue +',''2001-jan-01 00:00:00'')
				FROM '+@drivingLicenceTableId+' dl
				inner join '+@drivinglicencereviewentity+' review on (dl.'+dbo.GetAttributeId('C11ADAEB-27E0-4046-843F-7FD39513DACF',1)+' =review.'+dbo.GetAttributeId('D602AF08-2471-4035-B92C-50C3A9F492FD',1)+')
				WHERE '+dbo.GetAttributeId('C790EC93-A920-4CFC-8846-605F8B4B50B5',1)+' = '+Convert(nvarchar,(select valueid from customEntityAttributeListItems where attributeid =dbo.GetAttributeId('C790EC93-A920-4CFC-8846-605F8B4B50B5',0) and item ='Reviewed - OK'))+' 
					and '+@attEmployee +' = @employeeid 
					and '+@attLicenceType +' = ' +@attLicenceNonGb+ '
					and @expenseItemDate > =isnull('+@attDateOfIssue +',''2001-jan-01 00:00:00'') 
					and (@DrivingLicenceReviewPeriodically =0 OR (@DateOfExpense=0 and  CAST(GETUTCDATE() AS DATE) between CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE) and DATEADD(M,+@DrivingLicenceReviewFrequency,'+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+')) OR (@DateOfExpense=1 and CAST(@expenseItemDate AS DATE) between CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE) and DATEADD(M,+@DrivingLicenceReviewFrequency,'+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+')))
				order by dl.CreatedOn asc

				SELECT @nongblicenceexpiry='+ @attExpireDate +' 
				FROM '+@drivingLicenceTableId+' dl
						inner join '+@drivinglicencereviewentity+' review on (dl.'+dbo.GetAttributeId('C11ADAEB-27E0-4046-843F-7FD39513DACF',1)+' =review.'+dbo.GetAttributeId('D602AF08-2471-4035-B92C-50C3A9F492FD',1)+')
				WHERE '+dbo.GetAttributeId('C790EC93-A920-4CFC-8846-605F8B4B50B5',1)+' = '+Convert(nvarchar,(select valueid from customEntityAttributeListItems where attributeid =dbo.GetAttributeId('C790EC93-A920-4CFC-8846-605F8B4B50B5',0) and item ='Reviewed - OK'))+' 
					and '+@attEmployee +' = @employeeid 
					and '+@attLicenceType +' = '+@attLicenceNonGb+ ' 
					and @expenseItemDate between isnull('+@attDateOfIssue +',''2001-jan-01 00:00:00'') and '+ @attExpireDate +'
					and (@DrivingLicenceReviewPeriodically =0 OR (@DateOfExpense=0 and  CAST(GETUTCDATE() AS DATE) between CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE) and DATEADD(M,+@DrivingLicenceReviewFrequency,'+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+')) OR (@DateOfExpense=1 and  CAST(@expenseItemDate AS DATE) between CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE) and DATEADD(M,+@DrivingLicenceReviewFrequency,'+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+'))) 
				order by dl.CreatedOn asc
				 '
  	  
	set @sqlQuery1='
				SELECT @photocardlicenceexpiry='+ @attExpireDate +' 
				FROM '+@drivingLicenceTableId+' dl
						inner join '+@drivinglicencereviewentity+' review on (dl.'+dbo.GetAttributeId('C11ADAEB-27E0-4046-843F-7FD39513DACF',1)+' =review.'+dbo.GetAttributeId('D602AF08-2471-4035-B92C-50C3A9F492FD',1)+')
				WHERE '+dbo.GetAttributeId('C790EC93-A920-4CFC-8846-605F8B4B50B5',1)+' = '+Convert(nvarchar,(select valueid from customEntityAttributeListItems where attributeid =dbo.GetAttributeId('C790EC93-A920-4CFC-8846-605F8B4B50B5',0) and item ='Reviewed - OK'))+' 
					and '+@attEmployee +' = @employeeid 
					and '+@attLicenceType +' = '+@attLicencePhotoCard+ ' 
					and @expenseItemDate between isnull('+@attDateOfIssue +',''2001-jan-01 00:00:00'') and '+ @attExpireDate +'
					and (@DrivingLicenceReviewPeriodically =0 OR (@DateOfExpense=0 and  CAST(GETUTCDATE() AS DATE) between CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE) and DATEADD(M,+@DrivingLicenceReviewFrequency,'+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+')) OR (@DateOfExpense=1 and  CAST(@expenseItemDate AS DATE) between CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE) and DATEADD(M,+@DrivingLicenceReviewFrequency,'+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+'))) 
				order by dl.CreatedOn asc
  
			-- Expiry dates will be null if there is no document that has been approved. Hence, we then get the latest document that has been uploaded to get the expiry date  (which in this case will be the expiry date of the document that has been uploaded but hasn''t been approved.) Non-GB licence dates can be null regardless.

			    IF @paperlicenceValidFrom IS NULL
				BEGIN
					SELECT @paperlicenceValidFrom=isnull('+@attDateOfIssue +',''2001-jan-01 00:00:00'') ,

						@ReviewStatus =
			            CASE  
						when '+@reviewstatus+' Is NULL then ''requires a review''
						when '+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Awaiting Review'))+' or ('+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Reviewed - OK'))+' and CAST(@expenseItemDate AS DATE) < CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE))  then ''is awaiting review''	
						when '+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Reviewed - Failed'))+' then ''has failed review''		
						when '+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Invalidated'))+' then ''details have been changed and your review invalidated''
						ELSE ''review has expired''
						end
				

					FROM '+@drivingLicenceTableId+' dl
					left join '+@drivinglicencereviewentity+' review on (dl.'+dbo.GetAttributeId('C11ADAEB-27E0-4046-843F-7FD39513DACF',1)+' =review.'+dbo.GetAttributeId('D602AF08-2471-4035-B92C-50C3A9F492FD',1)+')
					WHERE '+@attEmployee +' = @employeeid 
						AND '+@attLicenceType +' = ' +@attLicencePaper+ ' 
						and @expenseItemDate >= isnull('+@attDateOfIssue +',''2001-jan-01 00:00:00'')  
					ORDER BY  dl.CreatedOn,review.CreatedOn ASC
				END
	
			    IF @paperlicenceValidFrom IS NULL
				BEGIN
					SELECT @paperlicenceValidFrom=isnull('+@attDateOfIssue +',''2001-jan-01 00:00:00'')
					FROM '+@drivingLicenceTableId+' 
					WHERE '+@attEmployee +' = @employeeid 
						AND '+@attLicenceType +' = ' +@attLicencePaper+ ' 
						and @expenseItemDate < isnull('+@attDateOfIssue +',''2001-jan-01 00:00:00'')  
					ORDER BY  '+@drivingLicenceTableId+'.CreatedOn ASC
				END
				
				IF @nongblicenceValidFrom IS NULL
				BEGIN
					SELECT @nongblicenceValidFrom='+@attDateOfIssue+' ,
					
						@ReviewStatus =
						CASE
						when '+@reviewstatus+' Is NULL then ''requires a review''
						when '+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Awaiting Review'))+' or ('+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Reviewed - OK'))+' and CAST(@expenseItemDate AS DATE) < CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE))  then ''is awaiting review''	
						when '+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Reviewed - Failed'))+' then ''has failed review''		
						when '+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Invalidated'))+' then ''details have been changed and your review invalidated''
						when '+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Reviewed - OK'))+' and CAST(@expenseItemDate AS DATE) between CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE) and DATEADD(M,+@DrivingLicenceReviewFrequency,'+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+') then ''''
						ELSE ''review has expired''
						end
						
					FROM '+@drivingLicenceTableId+' dl
					left join '+@drivinglicencereviewentity+' review on (dl.'+dbo.GetAttributeId('C11ADAEB-27E0-4046-843F-7FD39513DACF',1)+' =review.'+dbo.GetAttributeId('D602AF08-2471-4035-B92C-50C3A9F492FD',1)+')
					WHERE '+@attEmployee +' = @employeeid
						AND '+@attLicenceType +' = ' +@attLicenceNonGb+ '
						and @expenseItemDate < '+@attDateOfIssue +'
					OR '+@attEmployee +' = @employeeid
						AND '+@attLicenceType +' = ' +@attLicenceNonGb+ '
						AND '+@attDateOfIssue+' IS NULL
					ORDER BY dl.CreatedOn,review.CreatedOn ASC
				END
				
				IF @nongblicenceValidFrom IS NULL
				BEGIN
					SELECT @nongblicenceValidFrom='+@attDateOfIssue+'
					FROM '+@drivingLicenceTableId+'
					WHERE '+@attEmployee +' = @employeeid
						AND '+@attLicenceType +' = ' +@attLicenceNonGb+ '
						and @expenseItemDate < '+@attDateOfIssue +'
					OR '+@attEmployee +' = @employeeid
						AND '+@attLicenceType +' = ' +@attLicenceNonGb+ '
						AND '+@attDateOfIssue+' IS NULL
					ORDER BY '+@drivingLicenceTableId+'.CreatedOn ASC
				END'								   

				SET @sqlQuery4 = '
				
				IF @photocardlicenceexpiry IS NULL
				BEGIN
					SELECT @photocardlicenceexpiry='+ @attExpireDate +' ,	@StartDateOfPhotoCardLicence = '+@attDateOfIssue+',		
					@ReviewStatus =
				 CASE  
						when '+@reviewstatus+' Is NULL then ''requires a review''
						when '+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Awaiting Review'))+' or ('+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Reviewed - OK'))+' and CAST(@expenseItemDate AS DATE) < CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE))  then ''is awaiting review''	
						when '+@reviewstatus+'='+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Reviewed - Failed'))+' then ''has failed review''		
						when '+@reviewstatus+'='+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Invalidated'))+' then ''details have been changed and your review invalidated''
						ELSE ''review has expired''
						end

					FROM '+@drivingLicenceTableId+' dl
						left join '+@drivinglicencereviewentity+' review on (dl.'+dbo.GetAttributeId('C11ADAEB-27E0-4046-843F-7FD39513DACF',1)+' =review.'+dbo.GetAttributeId('D602AF08-2471-4035-B92C-50C3A9F492FD',1)+')
					WHERE '+@attEmployee +' = @employeeid 
						AND '+@attLicenceType +' = ' + @attLicencePhotoCard + ' 
						and @expenseItemDate between isnull('+@attDateOfIssue +',''2001-jan-01 00:00:00'') and '+ @attExpireDate +' 
					ORDER BY  dl.CreatedOn,review.CreatedOn ASC
				END
	
			    IF @photocardlicenceexpiry IS NULL
				BEGIN
					SELECT @photocardlicenceexpiry= '+ @attExpireDate +' 
					FROM '+@drivingLicenceTableId+' 
					WHERE 
					'+@attEmployee +' = @employeeid 
						and '+@attLicenceType +' = ' + @attLicencePhotoCard + ' 
						and @expenseItemDate > '+ @attExpireDate +' 
					order by '+@drivingLicenceTableId+'.CreatedOn asc
				END
				
				IF @nongblicenceexpiry IS NULL
				BEGIN
					SELECT @nongblicenceexpiry='+ @attExpireDate +' , @nongblicenceValidFrom = '+@attDateOfIssue+',
						
						@ReviewStatus =
						CASE
						when '+@reviewstatus+' Is NULL then ''requires a review''
						when '+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Awaiting Review'))+' or ('+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Reviewed - OK'))+' and CAST(@expenseItemDate AS DATE) < CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE))  then ''is awaiting review''	
						when '+@reviewstatus+'='+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Reviewed - Failed'))+' then ''has failed review''		
						when '+@reviewstatus+'='+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Invalidated'))+' then ''details have been changed and your review invalidated''
						when '+@reviewstatus+'= '+Convert(varchar,(select valueid from customEntityAttributeListItems where attributeid =@reviewstatusvalue and item='Reviewed - OK'))+' and CAST(@expenseItemDate AS DATE) between CAST('+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+' AS DATE) and DATEADD(M,+@DrivingLicenceReviewFrequency,'+dbo.GetAttributeId('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)+') then ''''
						ELSE ''review has expired''
						end

					FROM '+@drivingLicenceTableId+' dl
						left join '+@drivinglicencereviewentity+' review on (dl.'+dbo.GetAttributeId('C11ADAEB-27E0-4046-843F-7FD39513DACF',1)+' =review.'+dbo.GetAttributeId('D602AF08-2471-4035-B92C-50C3A9F492FD',1)+')
					WHERE '+@attEmployee +' = @employeeid 
						AND '+@attLicenceType +' = ' +@attLicenceNonGb+ '
					OR '+@attEmployee +' = @employeeid 
						AND '+@attLicenceType +' = ' +@attLicenceNonGb+ '
						AND '+@attExpireDate+' IS NULL
					ORDER BY dl.CreatedOn,review.CreatedOn ASC					
				END
				
				IF @nongblicenceexpiry IS NULL
				BEGIN
					SELECT @photocardlicenceexpiry= '+ @attExpireDate +' 
					FROM '+@drivingLicenceTableId+'
					WHERE
					'+@attEmployee +' = @employeeid
						and '+@attLicenceType +' = ' + @attLicenceNonGb + '
						and @expenseItemDate > '+ @attExpireDate +'
					OR '+@attEmployee +' = @employeeid
						and '+@attLicenceType +' = ' + @attLicenceNonGb + '
						AND '+@attExpireDate+' IS NULL
					order by '+@drivingLicenceTableId+'.CreatedOn asc
				END
				--force the checks to happen for manual when DVLA is not valid
		  SET @dvlacheck = 0
		  SET @dvlaLicenceValid = 0'
		  
		  SET @sqlQuery5 = '
		  
		  --check if the manual licence expiry is valid
		  -- Non-GB licences can have none, either, or both of Start and End date, so checks must be made for each possibility, for both todays date and expenseItemDate
		  IF NOT((@DateOfExpense=0 and  CAST(GETUTCDATE() AS DATE) between CAST(@StartDateOfPhotoCardLicence AS DATE) and CAST(@photocardlicenceexpiry AS DATE)) 
		  OR (@DateOfExpense=1 and  CAST(@expenseItemDate AS DATE) between CAST(@StartDateOfPhotoCardLicence AS DATE) and  CAST(@photocardlicenceexpiry AS DATE))
		  OR (@DateOfExpense=0 and CAST(GETUTCDATE() AS DATE) between CAST(@nongblicenceValidFrom AS DATE) and CAST(@nongblicenceexpiry AS DATE))
		  OR (@DateOfExpense=0 and @nongblicenceValidFrom IS NULL and CAST(GETUTCDATE() AS DATE) <= CAST(@nongblicenceexpiry AS DATE))
		  OR (@DateOfExpense=0 and @nongblicenceexpiry IS NULL and CAST(GETUTCDATE() AS DATE) >= CAST(@nongblicenceValidFrom AS DATE))
		  OR (@DateOfExpense=1 and CAST(@expenseItemDate AS DATE) between CAST(@nongblicenceValidFrom AS DATE) and CAST(@nongblicenceexpiry AS DATE))		  
		  OR (@DateOfExpense=1 and @nongblicenceValidFrom IS NULL and CAST(@expenseItemDate AS DATE) <= CAST(@nongblicenceexpiry AS DATE))
		  OR (@DateOfExpense=1 and @nongblicenceexpiry IS NULL and CAST(@expenseItemDate AS DATE) >= CAST(@nongblicenceValidFrom AS DATE))
		  OR (@nongblicencecheck=1 and @nongblicenceValidFrom IS NULL and @nongblicenceexpiry IS NULL))		
		  SET @IsValidManualLicence = 0

		  IF (@ReviewStatus <> '''' OR (@photocardlicenceexpiry is NULL AND @paperlicenceValidFrom is null AND @nongblicencecheck=0))
		  SET @IsValidManualLicence = 0

		   END
		END
			--IF there is no manual licence then consider DVLA as default
	IF(@photocardlicenceexpiry is NULL AND @paperlicenceValidFrom is null AND @ReviewStatus IS NULL AND @dvlalicenceexpiry IS NOT NULL AND @isvalidlicence <>0)
	SET @dvlaLicenceValid = 1

	SELECT @paperlicenceValidFrom AS PaperDrivingLicenceDocumentValidFromDate, @nongblicenceValidFrom AS NonGbDrivingLicenceStartDate, @photocardlicenceexpiry AS PhotocardDrivingLicenceExpiryDate, @nongblicenceexpiry AS NonGbDrivingLicenceExpiryDate,@blockDrivingLicence AS BlockDrivingLicence, @photocardcheck AS PhotocardExists, @nongblicencecheck AS NonGbDrivingLicenceExists,@dvlalicenceexpiry AS DvlaLicenceExpiry, @dvlaLicenceValid AS ValidDvlaLicence ,@dvlacheck AS DvlaLookupEnabled ,@ReviewStatus as ReviewStatus,@isvalidlicence as IsValidLicence, @IsValidManualLicence as IsValidManualLicence
END'

exec ( @sqlQuery+@sqlQuery2+@sqlQuery3+@sqlQuery1+@sqlQuery4+@sqlQuery5)

END