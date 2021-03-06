/*
Deployment script for CBH

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/


SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;

GO

PRINT N'Altering [dbo].[fnGetJobPONumbers]...';
GO
-- =============================================
-- Author:		Rony Simancas
-- Create date: 11/17/2016
-- Description:	Get Job PO Numbers
-- =============================================
ALTER FUNCTION fnGetJobPONumbers
(
	-- Add the parameters for the function here
	@JobKey INT
	,@ExcludePOKey INT
)
RETURNS VARCHAR(MAX)
AS
BEGIN
    --DECLARE @JobKey INT = 5542
	--,@ExcludePOKey INT = 9933;
	
	-- Declare the return variable here
	DECLARE @ResultVar VARCHAR(MAX)

	-- Add the T-SQL statements to compute the return value here
	SELECT DISTINCT @ResultVar = COALESCE(@ResultVar + ',','') + PONum 
	FROM tblJobPurchaseOrders 
	WHERE POJobKey = @JobKey AND POKey <> @ExcludePOKey

	-- Return the result of the function
	RETURN ISNULL(@ResultVar,'')
END
GO
PRINT N'Altering [dbo].[fnGetPaymentTermDetails]...';


GO



-- =============================================
-- Author:  Rony Simancas
-- Create date: 25/11/2016
-- Modified date: 27/04/2017
-- Description: Get PaymentTerms details
-- =============================================
ALTER FUNCTION [dbo].[fnGetPaymentTermDetails]
(
  @TransactionDate datetime
 ,@TermKey int
 ,@Amount DECIMAL(18,6)
)
RETURNS NVARCHAR(4000)
AS
BEGIN
	DECLARE @ResultVar NVARCHAR(4000) = ''
	,@TermPercentPrepaid DECIMAL(18,6) = 0
	,@CurBalance DECIMAL(18,6) = @Amount
	,@CrLf VARCHAR(MAX) = CHAR(10) + CHAR(13)
	,@TermPercentDays INT = 0

	SELECT TOP 1 @TermPercentPrepaid = TermPercentPrepaid, @TermPercentDays = TermPercentDays
	FROM tlkpPaymentTerms 
	WHERE TermKey = @TermKey
    
    --- Check for prepaid amount
    IF(@TermPercentPrepaid = 1)
    BEGIN
		SET @ResultVar = 'The invoice must be prepaid and is due upon receipt!'
		RETURN @ResultVar
    END
    ELSE
    BEGIN 
		SET @ResultVar = CONVERT(varchar, CAST(@TermPercentPrepaid * 100 AS money)) + '% (' + CONVERT(varchar, CAST(@Amount * @TermPercentPrepaid as money)) + ') of this invoice must be prepaid and is due upon receipt!'
        SET @CurBalance = @CurBalance - (@Amount * @TermPercentPrepaid)
    END
    
	-- Check for net terms
    IF(@curBalance <> @Amount)
    BEGIN
        SET @ResultVar = @ResultVar + @CrLf + 'The balance (' + CONVERT(VARCHAR,CAST(@CurBalance AS money),1) + ') is due by ' + CONVERT(varchar, DATEADD(dd, @TermPercentDays, @TransactionDate), 106) + '.'
    END
    ELSE
    BEGIN
        SET @ResultVar = 'This invoice is due by ' + CONVERT(VARCHAR, DATEADD(dd, @TermPercentDays, @TransactionDate), 106) + '.'
    END

	-- Return the result of the function
	RETURN @ResultVar
END
GO
PRINT N'Altering [dbo].[fnGetPOInstructionText]...';


GO
-- =============================================
-- Author:		Rony Simancas
-- Create date: 11/17/2016
-- Description:	Get PO InstructionText
-- =============================================
ALTER FUNCTION fnGetPOInstructionText
(
	-- Add the parameters for the function here
	@InstructionKey INT
	,@LanguageCode CHAR(2)
	,@POKey INT
	,@Table TableTuple READONLY
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	DECLARE @IText NVARCHAR(MAX)
	,@PONumbers NVARCHAR(MAX) = ''

	IF(@InstructionKey = 0)
		RETURN ''

	SELECT @IText=a.ITextMemo FROM tlkpJobPurchaseOrderInstructionsText a WHERE ITextInstructionKey = @InstructionKey AND a.ITextLanguageCode = @LanguageCode
	IF(@@ROWCOUNT = 0)
		SELECT @IText=a.ITextMemo FROM tlkpJobPurchaseOrderInstructionsText a WHERE ITextInstructionKey = @InstructionKey AND a.ITextLanguageCode = 'en'

	DECLARE @JobKey INT
	,@ColName NVARCHAR(MAX)
	,@ColValue NVARCHAR(MAX)
	
	SELECT @JobKey = CAST(ColValue AS INT) FROM @Table WHERE ColName = 'JobKey'
	SELECT @PONumbers = dbo.fnGetJobPONumbers(@JobKey, @POKey)

	SET @IText = REPLACE(ISNULL(@IText,''), '[PO Numbers]', @PONumbers)


	DECLARE QDATA CURSOR FOR
	select ColName,ColValue from @Table
	OPEN QDATA
	FETCH NEXT FROM QDATA INTO @ColName, @ColValue
	WHILE @@FETCH_STATUS = 0
	BEGIN
	   SET @IText =REPLACE(@IText, '[' + @ColName + ']', @ColValue)
	   FETCH NEXT FROM QDATA INTO @ColName, @ColValue
	END
	CLOSE QDATA;
	DEALLOCATE QDATA;

	-- Return the result of the function
	RETURN @IText
END
GO
PRINT N'Altering [dbo].[fnGetReportText]...';


GO

-- =============================================
-- Author:		Rony Simancas
-- Create date: 13/11/2014
-- Description:	Get Report Text
-- =============================================
ALTER FUNCTION [dbo].[fnGetReportText]
(
	@TextKey int,
	@LanguageCode char(2)
)
RETURNS NVARCHAR(4000)
AS
BEGIN
	DECLARE @ResultVar NVARCHAR(4000)

	IF(@LanguageCode = '') 
		SET @LanguageCode = 'en'
		
	SELECT TOP 1 @ResultVar = Text
    FROM tsysReportText
    WHERE (TextExpression = @TextKey) AND (TextLanguageCode = @LanguageCode)

    IF(@ResultVar IS NULL)
    BEGIN
		SET @LanguageCode = 'en'
		SELECT TOP 1 @ResultVar = Text
		FROM tsysReportText
		WHERE (TextExpression = @TextKey) AND (TextLanguageCode = @LanguageCode)
    END
	-- Return the result of the function
	RETURN @ResultVar

END
GO
PRINT N'Altering [dbo].[fnGetShipmentType]...';


GO


-- =============================================
-- Author:		Rony Simancas
-- Create date: 11/14/2016
-- Description:	Get ShipmentType
-- =============================================
ALTER FUNCTION [dbo].[fnGetShipmentType]
(
	@ShipType int,
	@LanguageCode char(2)
)
RETURNS NVARCHAR(4000)
AS
BEGIN
	DECLARE @ResultVar NVARCHAR(4000)
	

	IF(@LanguageCode = '') 
		SET @LanguageCode = 'en'
		
	SELECT TOP 1 @ResultVar = ShipTypeText
    FROM tsysShipmentTypes
    WHERE (ShipTypeExpression =  @ShipType) AND (ShipTypeLanguageCode = @LanguageCode)
    
    IF(@ResultVar IS NULL)
    BEGIN
        SET @ResultVar = '* * * Invalid Ship Type * * *'
		SET @LanguageCode = 'en'
		
		SELECT TOP 1 @ResultVar = ShipTypeText
		FROM tsysShipmentTypes
		WHERE (ShipTypeExpression =  @ShipType) AND (ShipTypeLanguageCode = @LanguageCode)
    END
    
	-- Return the result of the function
	RETURN @ResultVar

END
GO
PRINT N'Altering [dbo].[fnGetVendorOriginAddress]...';


GO

-- =============================================
-- Author:		Rony Simancas
-- Create date: 11-14-2016
-- Description:	Get vendor origin address
-- =============================================
ALTER FUNCTION [dbo].[fnGetVendorOriginAddress] 
(
	-- Add the parameters for the function here
	@OriginKey int
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar VARCHAR(MAX), @VendorName VARCHAR(MAX), @VendorAddress1 VARCHAR(MAX),
		@VendorAddress2 VARCHAR(MAX), @VendorCity VARCHAR(MAX), @VendorZip VARCHAR(MAX),
		@VendorState VARCHAR(MAX), @CountryName VARCHAR(MAX), @CrLf VARCHAR(MAX)

	-- Add the T-SQL statements to compute the return value here
	SELECT @VendorName = OriginName, @VendorAddress1 = OriginAddress1,
		@VendorAddress2 = OriginAddress2, @VendorCity = OriginCity, @VendorZip = OriginZip,
		@VendorState = OriginState, @CountryName = CountryName
	FROM tblVendorOriginAddress LEFT JOIN tblCountries ON OriginCountryKey = CountryKey
	WHERE OriginKey = @OriginKey

	SET @ResultVar = ''
	SET @CrLf = CHAR(13) + CHAR(10)


	If @VendorName IS NOT NULL SET @ResultVar = @ResultVar + @VendorName + @CrLf
    If @VendorAddress1 IS NOT NULL SET @ResultVar = @ResultVar + @VendorAddress1 + @CrLf
    If @VendorAddress2 IS NOT NULL SET @ResultVar = @ResultVar + @VendorAddress2 + @CrLf
    If @VendorCity IS NOT NULL SET @ResultVar = @ResultVar + @VendorCity
    If (@VendorCity IS NOT NULL AND (@VendorState + @VendorZip) IS NOT NULL) --SET @ResultVar = @ResultVar + ', '
    SET @ResultVar = @ResultVar + ', ' + RTRIM(@VendorState + '  ' + @VendorZip)
    If @CountryName IS NOT NULL SET @ResultVar = @ResultVar + @CrLf + @CountryName

	-- Return the result of the function
	RETURN @ResultVar
END
GO
PRINT N'Creating [dbo].[qrptFileSummaryV2]...';


GO

CREATE VIEW [dbo].[qrptFileSummaryV2]
AS
SELECT TOP 100 PERCENT FH.FileKey, dbo.fnGetFileNum(FH.FileKey) AS FileNum, FH.FileCustKey AS CustKey, 
                        C.CustName, FH.FileContactKey AS ContactKey, ISNULL(CC.ContactFirstName + N' ', N'') 
                        + ISNULL(CC.ContactLastName, N'') AS ContactName, FH.FileReference, 
                        FH.FileDateCustRequired, FH.FileDateCustRequiredNote, 
                        Sta.StatusText + ISNULL(N'-' + FSH.FileStatusMemo, N'') AS FileStatusMemo, FSH.FileStatusDate, 
                        FSH.FileStatusModifiedBy, FH.FileCreatedDate, ISNULL(FH.FileModifiedDate, 
                        FH.FileCreatedDate) AS rptDate, CSA.ShipCountryKey AS CountryKey, 
                        CASE WHEN FH.FileClosed IS NULL THEN Cast(0 AS bit) ELSE Cast(1 AS bit) END AS IsClosed, E.EmployeeKey, 
                        FH.FileModifiedDate, CC.ContactLastName
FROM          dbo.tblCustomers C RIGHT OUTER JOIN
                        dbo.tblFileHeader FH LEFT OUTER JOIN
                        dbo.tblEmployees E ON FH.FileQuoteEmployeeKey = E.EmployeeKey ON 
                        C.CustKey = FH.FileCustKey LEFT OUTER JOIN
                        dbo.tblCustomerShipAddress CSA ON FH.FileCustShipKey = CSA.ShipKey LEFT OUTER JOIN
                        dbo.tblCustomerContacts CC ON FH.FileContactKey = CC.ContactKey LEFT OUTER JOIN
                        dbo.tlkpStatus Sta INNER JOIN
                        dbo.tblFileStatusHistory FSH ON Sta.StatusKey = FSH.FileStatusStatusKey ON 
                        FH.FileKey = FSH.FileStatusFileKey
WHERE      (FSH.FileStatusModifiedDate =
                            (SELECT      MAX(FileStatusModifiedDate)
                              FROM           tblFileStatusHistory
                              WHERE       FileStatusFileKey = FileKey))
ORDER BY IsClosed, FileYear DESC, FileNum DESC
GO
PRINT N'Update complete.';


GO
