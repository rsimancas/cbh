Ext.define('CBH.model.jobs.InvoiceItemsSummary', {
    extend: 'Ext.data.Model',
    alias: 'model.invoiceitemssummary',
    idProperty: 'ISummaryKey',

    fields: [
        { name:'ISummaryKey', type:'int', defaultValue: null },
        { name:'ISummaryInvoiceKey', type:'int', defaultValue: null },
        { name:'ISummarySort', type:'int', defaultValue: null },
        { name:'ISummaryQty', type:'float', defaultValue: null },
        { name:'ISummaryVendorKey', type:'int', useNull: true, defaultValue: null },
        { name:'ISummaryItemNum', type:'string', useNull: true, defaultValue: null },
        { name:'ISummaryDescription', type:'string', useNull: true, defaultValue: null },
        { name:'ISummaryPrice', type:'float', defaultValue: null },
        { name:'ISummaryLinePrice', type:'float', defaultValue: null },
        { name:'ISummaryCurrencyCode', type:'string' },
        { name:'ISummaryCurrencyRate', type:'float', defaultValue: null },
        { name:'x_ItemName', type:'string'},
        { name:'x_ItemVendorName', type:'string' }
    ],
    proxy: {
            type: 'rest',
            url: CBH.GlobalSettings.webApiPath + '/api/InvoiceItemsSummary',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            reader: {
                type: 'json',
                root: 'data',
                totalProperty: 'total',
                successProperty: 'success',
                messageProperty: 'message',
                idProperty: 'ISummaryKey'
            },
            writer: {
                type: 'json',
                writeAllFields: true
            },
            afterRequest: function(request, success) {
                if (request.action == 'read') {
                    //this.readCallback(request);
                } else if (request.action == 'create') {
                    if (!request.operation.success) {
                        Ext.popupMsg.msg("Warning", "Record was not created");
                        Ext.global.console.warn(request.proxy.reader.jsonData.message);
                    } else {
                        Ext.popupMsg.msg("Success", "Created Successfully");
                    }
                } else if (request.action == 'update') {
                    if (!request.operation.success) {
                        Ext.popupMsg.msg("Warning", "Record was not saved");
                        Ext.global.console.warn(request.proxy.reader.jsonData.message);
                    } else {
                        Ext.popupMsg.msg("Success", "Updated Successfully");
                    }
                } else if (request.action == 'destroy') {
                    if (!request.operation.success) {
                        Ext.popupMsg.msg("Warning", "Record was not deleted");
                        //Ext.global.console.warn(request.proxy.reader.jsonData.message);
                    } else {
                        Ext.popupMsg.msg("Success", "Deleted Successfully");
                    }
                }
            }
        } //,
    // load: function(id, config) {
    //     config = Ext.apply({}, config);
    //     config = Ext.applyIf(config, {
    //         model: this,   //this line is necessary
    //         action: 'read',
    //         params: {
    //             id: id
    //         }
    //     });
    // }
});