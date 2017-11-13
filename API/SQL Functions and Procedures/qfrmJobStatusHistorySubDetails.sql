/****** Object:  View [dbo].[qfrmJobStatusHistorySubDetails]    Script Date: 26-11-2015 06:21:34 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[qfrmJobStatusHistorySubDetails]
AS
SELECT        TOP (100) PERCENT StatusKey, StatusJobKey, StatusPONum, StatusDate, StatusStatusKey, StatusMemo, StatusPublic, StatusModifiedBy, StatusModifiedDate
FROM            (SELECT        JobStatusKey AS StatusKey, JobStatusJobKey AS StatusJobKey, '*' AS StatusPONum, JobStatusDate AS StatusDate, JobStatusStatusKey AS StatusStatusKey, JobStatusMemo AS StatusMemo, 
                                                    JobStatusPublic AS StatusPublic, JobStatusModifiedBy AS StatusModifiedBy, JobStatusModifiedDate AS StatusModifiedDate
                          FROM            dbo.tblJobStatusHistory
                          UNION
                          SELECT        POStatusKey AS StatusKey, POStatusJobKey AS StatusJobKey, 'PO ' + dbo.fnGetPONum(POStatusPOKey) AS StatusPONum, POStatusDate AS StatusDate, POStatusStatusKey AS StatusStatusKey, 
                                                   POStatusMemo AS StatusMemo, POStatusPublic AS StatusPublic, POStatusModifiedBy AS StatusModifiedBy, POStatusModifiedDate AS StatusModifiedDate
                          FROM            dbo.tblJobPurchaseOrderStatusHistory
                          UNION
                          SELECT        dbo.tblFileQuoteStatusHistory.QStatusKey AS StatusKey, dbo.tblJobHeader.JobKey, dbo.fnGetQuoteNum(dbo.tblFileQuoteStatusHistory.QStatusQHdrKey) AS QuoteNum, 
                                                   dbo.tblFileQuoteStatusHistory.QStatusDate, dbo.tblFileQuoteStatusHistory.QStatusStatusKey, dbo.tblFileQuoteStatusHistory.QStatusMemo, 0 AS StatusPublic, 
                                                   dbo.tblFileQuoteStatusHistory.QStatusModifiedBy, dbo.tblFileQuoteStatusHistory.QStatusModifiedDate
                          FROM            dbo.tblJobHeader INNER JOIN
                                                   dbo.tblFileQuoteStatusHistory ON dbo.tblJobHeader.JobQHdrKey = dbo.tblFileQuoteStatusHistory.QStatusQHdrKey) AS StatusList
ORDER BY StatusDate DESC

GO


