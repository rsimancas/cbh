/****** Object:  View [dbo].[qlstJobs]    Script Date: 08-04-2016 07:48:44 a.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER VIEW [dbo].[qlstJobs]
AS
SELECT  TOP (100) PERCENT a.JobKey, CONVERT(nvarchar, a.JobCreatedDate, 6) AS Date, CAST(a.JobYear AS nvarchar) 
    + N'-' + RIGHT('0000' + CAST(a.JobNum AS nvarchar), 4) AS [Job Num], b.CustName AS Customer, a.JobReference AS Reference, ISNULL
        ((SELECT        TOP (1) dbo.tlkpStatus.StatusText
            FROM            dbo.tblJobStatusHistory INNER JOIN
                                    dbo.tlkpStatus ON dbo.tblJobStatusHistory.JobStatusStatusKey = dbo.tlkpStatus.StatusKey
            WHERE        (dbo.tblJobStatusHistory.JobStatusJobKey = a.JobKey)
            ORDER BY dbo.tblJobStatusHistory.JobStatusDate DESC), '*No Status*') AS Status, a.JobCreatedBy, a.JobClosed, a.JobCreatedDate, 
    a.JobCustCurrencyCode AS CustCurrencyCode, a.JobCustCurrencyRate AS CustCurrencyRate, a.JobModifiedBy, a.JobModifiedDate, 
    a.JobCustShipKey, a.JobWarehouseKey, a.JobShipType, a.JobCustKey, a.JobQHdrKey, c.Quote, c.QHdrFileKey,
	'F' + RIGHT(CAST(d.FileYear AS nvarchar), 2) + N'-' + RIGHT('0000' + CONVERT(nvarchar, d.FileNum), 4) AS [FileNum] 
FROM            dbo.tblJobHeader AS a INNER JOIN
                dbo.tblCustomers AS b ON a.JobCustKey = b.CustKey LEFT OUTER JOIN
				dbo.qsumFileQuoteSummary AS c ON a.JobQHdrKey = c.QHdrKey LEFT OUTER JOIN
				dbo.tblFileHeader AS d ON c.QHdrFileKey = d.FileKey
ORDER BY a.JobCreatedDate DESC


GO


