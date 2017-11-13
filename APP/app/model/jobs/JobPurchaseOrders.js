Ext.define('CBH.model.jobs.JobPurchaseOrders', {
    extend: 'Ext.data.Model',
    alias: 'model.JobPurchaseOrders',

    fields: [
        { name:'POKey', type:'int',useNull: true, defaultValue: null },
        { name:'PONum', type:'int',useNull: true, defaultValue: null },
        { name:'PORevisionNum', type:'int', defaultValue: null },
        { name:'PONumShipment', type:'int', defaultValue: null },
        { name:'POJobKey', type:'int', defaultValue: null },
        { name:'POInvoiceKey', type:'int', useNull: true, defaultValue: null },
        { name:'POVendorKey', type:'int', useNull: true, defaultValue: null },
        { name:'POVendorContactKey', type:'int', useNull: true, defaultValue: null },
        { name:'POVendorReference', type:'string', useNull: true, defaultValue: null },
        { name:'POVendorOriginAddress', type:'int', useNull: true, defaultValue: null },
        { name:'PODate', type:'date', defaultValue: null },
        { name:'POGoodThruDate', type:'date', defaultValue: null },
        { name:'POLeadTime', type:'string', useNull: true, defaultValue: null },
        { name:'PODefaultProfitMargin', type:'float', useNull: true, defaultValue: null },
        { name:'POVendorPaymentTerms', type:'int', useNull: true, defaultValue: null },
        { name:'POCurrencyCode', type:'string', defaultValue: 'USD' },
        { name:'POCurrencyRate', type:'float', useNull: true, defaultValue: 1 },
        { name:'POShipmentType', type:'int', defaultValue: null },
        { name:'POFreightDestination', type:'int', defaultValue: null },
        { name:'POFreightDestinationZip', type:'string', useNull: true, defaultValue: null },
        { name:'POCustShipKey', type:'int', useNull: true, defaultValue: null },
        { name:'POWarehouseKey', type:'int', useNull: true, defaultValue: null },
        { name:'POShipETA', type:'date', useNull: true, defaultValue: null },
        { name:'POSubmittedDate', type:'date', useNull: true, defaultValue: null },
        { name:'POStatusReport', type:'string', useNull: true, defaultValue: null },
        { name:'POClosed', type:'boolean', defaultValue: null },
        { name:'POModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'POModifiedDate', type:'date', useNull: true, defaultValue: null },
        { name:'POCreatedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
        { name:'POCreatedDate', type:'date', defaultValue: new Date() },
        { name:'POPeachtreeNRecord', type:'int', useNull: true, defaultValue: null },
        { name:'POUseOriginAddress', type:'boolean', defaultValue: null },
        { name:'x_JobNumFormatted', type:'string' },
        { name:'x_PONumFormatted', type:'string' },
        { name:'x_POCurrencyCode', type:'string',
            convert: function(val,row) {
                return row.data.POCurrencyCode;
            }
        }
    ],
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/JobPurchaseOrders',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'POKey'
        },
        writer: {
            type: 'json',
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
        });
    }
});