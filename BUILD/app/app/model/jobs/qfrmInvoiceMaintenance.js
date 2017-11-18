Ext.define('CBH.model.jobs.qfrmInvoiceMaintenance', {
    extend: 'Ext.data.Model',
    alias: 'model.qfrmInvoiceMaintenance',

    fields: [
    { name:'InvoiceKey', type:'int', defaultValue: null },
        { name:'InvoiceJobKey', type:'int', useNull: true, defaultValue: null },
        { name:'InvoicePrefix', type:'int', defaultValue: null },
        { name:'InvoiceNum', type:'int', defaultValue: null },
        { name:'InvoiceRevisionNum', type:'int', defaultValue: null },
        { name:'InvoiceDate', type:'date', defaultValue: null },
        { name:'InvoiceRecipient', type:'int', defaultValue: null },
        { name:'InvoiceVendorKey', type:'int', useNull: true, defaultValue: null },
        { name:'InvoiceVendorContactKey', type:'int', useNull: true, defaultValue: null },
        { name:'InvoiceCustKey', type:'int', useNull: true, defaultValue: null },
        { name:'InvoiceCustContactKey', type:'int', useNull: true, defaultValue: null },
        { name:'InvoiceBillingName', type:'string', useNull: true, defaultValue: null },
        { name:'InvoiceBillingAddress1', type:'string', useNull: true, defaultValue: null },
        { name:'InvoiceBillingAddress2', type:'string', useNull: true, defaultValue: null },
        { name:'InvoiceBillingCity', type:'string', useNull: true, defaultValue: null },
        { name:'InvoiceBillingState', type:'string', useNull: true, defaultValue: null },
        { name:'InvoiceBillingZip', type:'string', useNull: true, defaultValue: null },
        { name:'InvoiceBillingCountryKey', type:'int', useNull: true, defaultValue: null },
        { name:'InvoiceCustShipKey', type:'int', useNull: true, defaultValue: null },
        { name:'InvoiceCustReference', type:'string', useNull: true, defaultValue: null },
        { name:'InvoiceEmployeeKey', type:'int', defaultValue: null },
        { name:'InvoiceCurrencyCode', type:'string' },
        { name:'InvoiceCurrencyRate', type:'float', defaultValue: null },
        { name:'InvoicePaymentTerms', type:'int', defaultValue: null },
        { name:'InvoiceMemo', type:'string', useNull: true, defaultValue: null },
        { name:'InvoiceModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'InvoiceModifiedDate', type:'date', useNull: true, defaultValue: null },
        { name:'InvoiceCreatedBy', type:'string', useNull: true, defaultValue: null },
        { name:'InvoiceCreatedDate', type:'date', useNull: true, defaultValue: null },
        { name:'InvoiceMemoFont', type:'int', useNull: true, defaultValue: null },
        { name:'CustKey', type:'int', useNull: true, defaultValue: null },
        { name:'CustPeachtreeID', type:'string', useNull: true, defaultValue: null },
        { name:'CustPeachtreeIndex', type:'int', useNull: true, defaultValue: null },
        { name:'CustName', type:'string', useNull: true, defaultValue: null },
        { name:'CustAddress1', type:'string', useNull: true, defaultValue: null },
        { name:'CustAddress2', type:'string', useNull: true, defaultValue: null },
        { name:'CustCity', type:'string', useNull: true, defaultValue: null },
        { name:'CustState', type:'string', useNull: true, defaultValue: null },
        { name:'CustZip', type:'string', useNull: true, defaultValue: null },
        { name:'CustCountryKey', type:'int', useNull: true, defaultValue: null },
        { name:'CustPhone', type:'string', useNull: true, defaultValue: null },
        { name:'CustFax', type:'string', useNull: true, defaultValue: null },
        { name:'CustEmail', type:'string', useNull: true, defaultValue: null },
        { name:'CustWebsite', type:'string', useNull: true, defaultValue: null },
        { name:'CustSalesRepKey', type:'int', useNull: true, defaultValue: null },
        { name:'CustOrdersRepKey', type:'int', useNull: true, defaultValue: null },
        { name:'CustLanguageCode', type:'string', useNull: true, defaultValue: null },
        { name:'CustStatus', type:'int', useNull: true, defaultValue: null },
        { name:'CustModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'CustModifiedDate', type:'date', useNull: true, defaultValue: null },
        { name:'CustCreatedBy', type:'string', useNull: true, defaultValue: null },
        { name:'CustCreatedDate', type:'date', useNull: true, defaultValue: null },
        { name:'CustCreditLimit', type:'float', useNull: true, defaultValue: null },
        { name:'CustCurrencyCode', type:'string', useNull: true, defaultValue: null },
        { name:'CustMemo', type:'string', useNull: true, defaultValue: null },
        { name:'VendorKey', type:'int', useNull: true, defaultValue: null },
        { name:'VendorName', type:'string', useNull: true, defaultValue: null },
        { name:'VendorPeachtreeID', type:'string', useNull: true, defaultValue: null },
        { name:'VendorPeachtreeItemID', type:'string', useNull: true, defaultValue: null },
        { name:'VendorPeachtreeJobID', type:'string', useNull: true, defaultValue: null },
        { name:'VendorDisplayToCust', type:'string', useNull: true, defaultValue: null },
        { name:'VendorContact', type:'string', useNull: true, defaultValue: null },
        { name:'VendorAddress1', type:'string', useNull: true, defaultValue: null },
        { name:'VendorAddress2', type:'string', useNull: true, defaultValue: null },
        { name:'VendorCity', type:'string', useNull: true, defaultValue: null },
        { name:'VendorState', type:'string', useNull: true, defaultValue: null },
        { name:'VendorZip', type:'string', useNull: true, defaultValue: null },
        { name:'VendorCountryKey', type:'int', useNull: true, defaultValue: null },
        { name:'VendorPhone', type:'string', useNull: true, defaultValue: null },
        { name:'VendorFax', type:'string', useNull: true, defaultValue: null },
        { name:'VendorEmail', type:'string', useNull: true, defaultValue: null },
        { name:'VendorWebsite', type:'string', useNull: true, defaultValue: null },
        { name:'VendorLanguageCode', type:'string', useNull: true, defaultValue: null },
        { name:'VendorAcctNum', type:'string', useNull: true, defaultValue: null },
        { name:'VendorCarrier', type:'boolean', useNull: true, defaultValue: null },
        { name:'VendorModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'VendorModifiedDate', type:'date', useNull: true, defaultValue: null },
        { name:'VendorCreatedBy', type:'string', useNull: true, defaultValue: null },
        { name:'VendorCreatedDate', type:'date', useNull: true, defaultValue: null },
        { name:'VendorDefaultCommissionPercent', type:'float', useNull: true, defaultValue: null },
        { name:'JobShipType', type:'int', useNull: true, defaultValue: null },
        { name:'JobShipDate', type:'date', useNull: true, defaultValue: null },
        { name:'FullInvoiceNum', type:'string', useNull: true, defaultValue: null }
    ],
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/qfrmInvoiceMaintenance',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'InvoiceKey'
        },
        writer: {
            type:'json',
            writeAllFields: true
        },
        afterRequest: function (request, success) {
            if (request.action == 'read') {
                //this.readCallback(request);
            }
            else if (request.action == 'create') {
                if (!request.operation.success)
                {
                    Ext.popupMsg.msg("Warning", "Record was not created");
                    Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    Ext.popupMsg.msg("Success","Created Successfully");
                }
            }
            else if (request.action == 'update') {
                if (!request.operation.success)
                {
                    Ext.popupMsg.msg("Warning", "Record was not saved");
                    Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    Ext.popupMsg.msg("Success","Updated Successfully");
                }
            }
            else if (request.action == 'destroy') {
                if (!request.operation.success)
                {
                    Ext.popupMsg.msg("Warning", "Record was not deleted");
                    //Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    Ext.popupMsg.msg("Success","Deleted Successfully");
                }
            }
        }
    },
    load: function(id, config) {
        config = Ext.apply({}, config);
        config = Ext.applyIf(config, {
            model: this,   //this line is necessary
            action: 'read',
            params: {
                id: id
            }
        })
    }
});