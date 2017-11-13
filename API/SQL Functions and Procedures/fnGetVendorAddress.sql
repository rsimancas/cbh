-- ================================================
-- Template generated from Template Explorer using:
-- Create Scalar Function (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rony Simancas
-- Create date: 02-22-2016
-- Description:	Get address vendor
-- =============================================
CREATE FUNCTION dbo.fnGetVendorAddress 
(
	-- Add the parameters for the function here
	@id int
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar VARCHAR(MAX), @VendorName VARCHAR(MAX), @VendorAddress1 VARCHAR(MAX),
		@VendorAddress2 VARCHAR(MAX), @VendorCity VARCHAR(MAX), @VendorZip VARCHAR(MAX),
		@VendorState VARCHAR(MAX), @CountryName VARCHAR(MAX), @CrLf VARCHAR(MAX)

	-- Add the T-SQL statements to compute the return value here
	SELECT @VendorName = VendorName, @VendorAddress1 = VendorAddress1,
		@VendorAddress2 = VendorAddress2, @VendorCity = VendorCity, @VendorZip = VendorZip,
		@VendorState = VendorState, @CountryName = CountryName
	FROM tblVendors LEFT JOIN tblCountries ON VendorCountryKey = CountryKey
	WHERE VendorKey = @id

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