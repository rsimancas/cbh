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
-- Create date: 02-23-2016
-- Description:	Get Job Status
-- =============================================
CREATE FUNCTION fnGetJobStatus
(
	-- Add the parameters for the function here
	@id int
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar VARCHAR(MAX)

	-- Add the T-SQL statements to compute the return value here
	SELECT TOP 1 @ResultVar = StatusMemo 
	FROM (
		SELECT JobStatusDate AS StatusDate, JobStatusMemo as StatusMemo
		FROM            dbo.tblJobStatusHistory
		WHERE JobStatusJobKey=@id AND (JobStatusMemo IS NOT NULL)
		UNION
		SELECT POStatusDate AS StatusDate, POStatusMemo as StatusMemo
		FROM dbo.tblJobPurchaseOrderStatusHistory
		WHERE POStatusJobKey=@id AND (POStatusMemo IS NOT NULL)
	) a
	ORDER BY StatusDate DESC


	-- Return the result of the function
	RETURN @ResultVar
END
GO

