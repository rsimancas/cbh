CREATE FUNCTION [dbo].[fn_FormatUsingMask] 
(
    -- Add the parameters for the function here
    @input nvarchar(1000),
    @mask nvarchar(1000)
)
RETURNS nvarchar(1000)
AS
BEGIN
    -- Declare the return variable here
    DECLARE @result nvarchar(1000) = ''
    DECLARE @inputPos int = 1
    DECLARE @maskPos int = 1
    DECLARE @maskSign char(1) = ''

    WHILE @maskPos <= Len(@mask)
    BEGIN
        set @maskSign = substring(@mask, @maskPos, 1)

        IF @maskSign = '#'
        BEGIN
            set @result = @result + substring(@input, @inputPos, 1)
            set @inputPos += 1
            set @maskPos += 1
        END
        ELSE
        BEGIN
            set @result = @result + @maskSign
            set @maskPos += 1
        END
    END
    -- Return the result of the function
    RETURN @result

END