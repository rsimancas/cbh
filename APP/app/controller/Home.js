Ext.define('CBH.controller.Home', {
    extend: 'Ext.app.Controller',

    models: [
        'common.Users',
        'jobs.JobStatusHistory'
    ],

    stores: [
        'common.Countries',
        'common.CurrencyRates',
        'common.Employees',
        'common.FreightDestinations',
        'common.InspectionCompanies',
        'common.Languages',
        'common.NavTree',
        'common.PaymentTerms',
        'common.PaymentTermsDescriptions',
        'common.ShipmentTypes',
        'common.States',
        'common.Status',
        'common.Users',
        'common.tsysEmployeeCodes',
        'common.ScheduleB',
        'common.ScheduleBLanguage',
        'common.CountriesForReport',
        'common.EmployeesForReport',
        'common.ReportCriteria',
        'common.PrintQueue',
        'customers.CustomerContacts',
        'customers.Customers',
        'customers.CustomerShipAddress',
        'customers.CustomerStatus',
        'customers.CustomersForReport',
        'customers.CustomerContactsForReport',
        'jobs.InvoiceCharges',
        'jobs.InvoiceChargesSubTotal',
        'jobs.InvoiceHeader',
        'jobs.InvoiceItemsSummary',
        'jobs.JobEmployeeRoles',
        'jobs.JobHeader',
        'jobs.JobList',
        'jobs.JobPurchaseOrderCharges',
        'jobs.JobPurchaseOrderInstructions',
        'jobs.JobPurchaseOrderItems',
        'jobs.JobPurchaseOrders',
        'jobs.JobPurchaseOrderStatusHistory',
        'jobs.JobRoles',
        'jobs.JobStatusHistory',
        'jobs.JobCurrencyMaster',
        'jobs.qJobOverview',
        'jobs.qLstJobInvoices',
        'jobs.qLstJobPurchaseOrders',
        'jobs.tlkpChargeCategories',
        'jobs.tlkpJobPurchaseOrderInstructions',
        'jobs.JobStatusHistorySubDetails',
        'jobs.qfrmJobStatusHistory',
        'jobs.qfrmJobPurchaseOrderStatusHistory',
        'jobs.qryJobSearch',
        'jobs.qryPOSearch',
        'jobs.qryJobInvoiceSearch',
        'jobs.InvoiceDDL',
        'jobs.qfrmJobOverviewPopupUpdateCurrency',
        'jobs.JobStatus',
        'jobs.qfrmJobExemptFromProfitReport',
        'jobs.qfrmJobExemptFromPronacaReport',
        'sales.ChargeCategories',
        'sales.FileEmployeeRoles',
        'sales.FileHeader',
        'sales.FileList',
        'sales.FileOverview',
        'sales.FileQuoteCharges',
        'sales.FileQuoteChargesSubTotals',
        'sales.FileQuoteDetail',
        'sales.FileQuoteHeader',
        'sales.FileQuoteItemsSummary',
        'sales.FileQuoteStatusHistory',
        'sales.FileQuoteVendorInfo',
        'sales.FileStatusHistory',
        'sales.FileStatusHistorySubDetails',
        'sales.FileQuoteStatusHistorySubDetails',
        'sales.LeadTime',
        'sales.qfrmFileStatusHistory',
        'sales.qfrmFileQuoteStatusHistory',
        'sales.qfrmFileQuoteConfirmation',
        'sales.qfrmFileQuoteConfirmationSVInfo',
        'sales.qfrmItemPriceHistoryPurchaseOrders',
        'sales.qfrmFileQuoteDetailsSub',
        'sales.qryFileQuoteVendorSummaryWithDiscount',        
        'sales.qryFileQuoteSearch',
        'sales.qryFileSearch',
        'sales.qsumFileQuoteVendorSummary',
        'sales.tlkpGenericLists',
        'sales.tlkpInvoiceSubTotalCategories',
        'sales.qsumFileQuoteChargesByGLAccount',
        'vendors.ItemDescriptions',
        'vendors.Items',
        'vendors.LastQuoteMargin',
        'vendors.VendorContacts',
        'vendors.VendorOriginAddress',
        'vendors.Vendors',
        'vendors.VendorsForReport',
        'vendors.VendorWarehouse',
        'vendors.WarehouseTypes',
        'vendors.qlstScheduleBImports'
    ],

    views: [
        'sales.FileQuoteConfirmation',
        'sales.FileQuoteConfirmationSubVendorInfo',
        'sales.FileForm',
        'sales.FileLineEntry',
        'sales.FileOrderEntry',
        'sales.FileOverview',
        'sales.FileStatusHistory',
        'sales.FileStatusHistoryList',
        'sales.FileQuoteEntry',
        'sales.FileQuoteMaintenance',
        'sales.FileQuoteStatusHistory',
        'sales.FileQuoteStatusHistoryList',
        'sales.FileFindFile',
        'sales.FileFindQuote',
        'sales.FileQuoteVendorSelection',
        'sales.FileQuoteEditItemSummary',
        'sales.FileQuoteOrderEntryQuickAdd',
        'sales.InputProfitMargin',
        'jobs.JobNewInvoice',
        'jobs.JobPurchaseOrderMaintenance',
        'jobs.InvoiceMaintenance',
        'jobs.InvoiceEditItems',
        'jobs.InvoiceEditCharges',
        'jobs.JobStatusHistory',
        'jobs.JobStatusHistoryList',
        'jobs.JobPurchaseOrderStatusHistory',
        'jobs.JobPurchaseOrderStatusHistoryList',
        'jobs.JobFindJob',
        'jobs.JobInformation',
        'jobs.JobOverview',
        'jobs.JobMenu',
        'jobs.JobFindPO',
        'jobs.JobFindJobInvoice',
        'jobs.JobOverviewPopupUpdateCurrency',
        'jobs.qfrmJobExemptFromProfitReportList',
        'jobs.qfrmJobExemptFromPronacaReport',
        'vendors.ItemsList',
        'vendors.Items',
        'customers.CustomersList',
        'vendors.VendorsList',
        'vendors.ItemsList',
        'sales.SalesMenu',
        'common.CurrencyList',
        'common.PaymentTermsList',
        'common.MainMenu',
        'common.ScheduleBList',
        'common.ScheduleB',
        'common.InputConvert',
        'common.FormReportCriteria',
        'manager.ManagerMenu',
        'manager.EmployeeList',
        'manager.Employees',
        'manager.JobRolesList'
    ],

    init: function(application) {
        var auth = Ext.util.Cookies.get('CBH.UserAuth');

        if (!auth || auth === null) {
            window.removeEventListener('beforeunload', CBH.GlobalSettings.unloadListener, false);
            location.href = '#logon';
            return;
        }

        var myEvent = window.attachEvent || window.addEventListener;
        var chkevent = window.attachEvent ? 'onbeforeunload' : 'beforeunload'; /// make IE7, IE8 compitable

        myEvent(chkevent, CBH.GlobalSettings.unloadListener);
    }

});