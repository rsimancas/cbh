Ext.define('CBH.model.vendors.Items', {
    extend: 'Ext.data.Model',
    alias: 'model.items',
    idProperty: 'ItemKey',

    fields: [
        { name:'ItemKey', type:'int' },
        { name:'ItemVendorKey', type:'int', useNull: true },
        { name:'ItemNum', type:'string' },
        { name:'ItemCost', type:'float' },
        { name:'ItemPrice', type:'float' },
        { name:'ItemCurrencyCode', type:'string', defaultValue: 'USD' },
        { name:'ItemWeight', type:'float' },
        { name:'ItemVolume', type:'float' },
        { name:'ItemSchBImportKey', type:'int', useNull: true },
        { name:'ItemSchBNum', type:'float', useNull: true },
        { name:'ItemSchBExportDescription', type:'string', useNull: true, defaultValue: null },
        { name:'ItemModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'ItemModifiedDate', type:'date', useNull: true },
        { name:'ItemCreatedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
        { name:'ItemCreatedDate', type:'date', defaultValue: new Date() },
        { name:'ItemActive', type:'boolean', defaultValue: true },
        { name:'x_ItemName', type:'string'},
        { name:'x_VendorName', type:'string'},
        { name:'x_ItemNumName', type:'string'},
        { name:'ItemCurrencyRate', type:'float', useNull: true, defaultValue: true },
        { name:'ItemCurrencySymbol', type:'string'}
    ],

    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/Items',
        headers: {
            'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'ItemKey'
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
    },

    belongsTo: 'CBH.model.vendors.Vendors'
});