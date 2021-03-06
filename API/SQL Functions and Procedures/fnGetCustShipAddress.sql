SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rony Simancas
-- Create date: 03-18-2016
-- Description:	Get cust ship address
-- =============================================
CREATE FUNCTION [dbo].[fnGetCustShipAddress] 
(
	-- Add the parameters for the function here
	@id int
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar VARCHAR(MAX), @ShipName VARCHAR(MAX), @ShipAddress1 VARCHAR(MAX),
		@ShipAddress2 VARCHAR(MAX), @ShipCity VARCHAR(MAX), @ShipZip VARCHAR(MAX),
		@ShipState VARCHAR(MAX), @CountryName VARCHAR(MAX), @CrLf VARCHAR(MAX)

	-- Add the T-SQL statements to compute the return value here
	SELECT @ShipName = ShipName, @ShipAddress1 = ShipAddress1,
		@ShipAddress2 = ShipAddress2, @ShipCity = ShipCity, @ShipZip = ShipZip,
		@ShipState = ShipState, @CountryName = CountryName
	FROM tblCustomerShipAddress LEFT JOIN tblCountries ON ShipCountryKey = CountryKey
	WHERE ShipKey = @id

	SET @ResultVar = ''
	SET @CrLf = CHAR(13) + CHAR(10)


	If @ShipName IS NOT NULL SET @ResultVar = @ResultVar + @ShipName + @CrLf
    If @ShipAddress1 IS NOT NULL SET @ResultVar = @ResultVar + @ShipAddress1 + @CrLf
    If @ShipAddress2 IS NOT NULL SET @ResultVar = @ResultVar + @ShipAddress2 + @CrLf
    If @ShipCity IS NOT NULL SET @ResultVar = @ResultVar + @ShipCity
    If @ShipCity IS NOT NULL AND (@ShipState + @ShipZip) IS NOT NULL SET @ResultVar = @ResultVar + ', '+ RTRIM(@ShipState + '  ' + @ShipZip)
    If @CountryName IS NOT NULL SET @ResultVar = @ResultVar + @CrLf + @CountryName

	-- Return the result of the function
	RETURN @ResultVar
END