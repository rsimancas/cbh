/****** Object:  UserDefinedFunction [dbo].[fnGetLocationFooter]    Script Date: 11/24/2014 13:02:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rony Simancas
-- Create date: 13/11/2014
-- Description:	Get Location For User Logged
-- =============================================
CREATE FUNCTION [dbo].[fnGetLocationFooter]
(
	@LocationKey int
)
RETURNS NVARCHAR(4000)
AS
BEGIN
	DECLARE @ResultVar NVARCHAR(4000), @CRLF NVARCHAR(4000)
	SET @CRLF = CHAR(13) + CHAR(10)

	IF(@LocationKey = 0) 
		SET @LocationKey = 1
	
	DECLARE @Address1 NVARCHAR(4000), @Address2 NVARCHAR(4000), @City NVARCHAR(4000),
	  @State NVARCHAR(4000), @StateName NVARCHAR(4000), 
      @Zip NVARCHAR(4000), @Country NVARCHAR(4000), @Phone NVARCHAR(4000),
      @Fax NVARCHAR(4000), @WebSite NVARCHAR(4000)
		
    SELECT TOP 1 @Address1 = ISNULL(LocationAddress1,''), @Address2 = ISNULL(LocationAddress2,''), @City = ISNULL(LocationCity,''),
      @State = ISNULL(LocationState,''), @StateName = StateName, 
      @Zip = ISNULL(LocationZip,''), @Country = dbo.fnGetCountry(dbo.tsysLocations.LocationCountry),
      @Phone = ISNULL(LocationPhone,''), @Fax = ISNULL(LocationFax,''), @WebSite = ISNULL(LocationWebSite,'')
    FROM tsysLocations LEFT OUTER JOIN tblStates ON LocationState = StateCode 
    WHERE LocationKey = @LocationKey
    
    SET @ResultVar = @Address1 + Space(2) + CHAR(149) + Space(2)
        
    IF (LEN(@Address2) > 0)
        SET @ResultVar = @ResultVar+ @Address2 + Space(2) + CHAR(149) + Space(2)
    
    SET @ResultVar = @ResultVar + @City
    
    IF (LEN(@State) > 0)
        SET @ResultVar = @ResultVar +', ' + @StateName
    
    IF (LEN(@Zip) > 0)
        SET @ResultVar = @ResultVar +Space(2)+ @Zip
    
    IF (LEN(@Country) > 0)
    BEGIN
        If (@Country = 'United States')
            SET @ResultVar = @ResultVar + ', U.S.A.'
        Else
            SET @ResultVar = @ResultVar +', ' + @Country
    END
    
    SET @ResultVar = @ResultVar +Space(2) + CHAR(149) + Space(2)
    
    IF (LEN(@Phone) > 0)
        SET @ResultVar = @ResultVar +'Tel.: '+ @Phone + Space(2) + CHAR(149) + Space(2)
    
    IF (LEN(@Fax) > 0)
        SET @ResultVar = @ResultVar +'Fax: '+ @Fax
            
    IF (LEN(@Website) > 0)
        SET @ResultVar = @ResultVar + @CRLF + @Website
    

	-- Return the result of the function
	RETURN @ResultVar
END
