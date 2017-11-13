/****** Object:  UserDefinedFunction [dbo].[fnGetPONum]    Script Date: 03/07/2016 06:14:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fnGetPONumStyle]
	(@POKey int, @Style int)
RETURNS nvarchar(10)
AS
	BEGIN
	DECLARE @PONum int, @PORevisionNum tinyint, @PONumShipment int, @ReturnVal NVARCHAR(15)
	
	SELECT @PONum = PONum, @PORevisionNum = PORevisionNum, @PONumShipment = PONumShipment FROM tblJobPurchaseOrders WHERE POKey = @POKey
	
	SET @ReturnVal = @PONum
	
	If @PORevisionNum > 0 SET @ReturnVal = @ReturnVal + CHAR(@PORevisionNum + 64)

    IF @Style = 0  --00000@-00
        SET @ReturnVal = @ReturnVal + '-' + RIGHT('00' + CAST(@PONumShipment as nvarchar), 2) 
    IF @Style = 1  -- 00000@-##
    BEGIN
        If @PONumShipment > 1 SET @ReturnVal = @ReturnVal + '-' + RIGHT('00' + CAST(@PONumShipment as nvarchar), 2) 
    END
    --Case 2  '00000@
	
	RETURN @ReturnVal
	--RETURN Convert(nvarchar, @PONum) + LTRIM(CASE @PORevisionNum WHEN  0 THEN '' ELSE Char(@PORevisionNum+64) END) + CASE @PONumShipment WHEN 1 THEN '' ELSE ('-' + RIGHT('00' + CAST(@PONumShipment as nvarchar), 2)) END
END
GO


