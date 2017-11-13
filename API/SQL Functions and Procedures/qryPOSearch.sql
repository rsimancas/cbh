/****** Object:  View [dbo].[qryJobSearch]    Script Date: 29-04-2016 04:57:28 a.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[qryPOSearch]
AS
SELECT a.*,dbo.fnGetJobNum(a.POJobKey) as JobNum, 
  dbo.fnGetPONumStyle(a.POKey, 0) as PONumFormatted, b.VendorName
from tblJobPurchaseOrders a
	LEFT JOIN tblVendors b ON a.POVendorKey = b.VendorKey
GO