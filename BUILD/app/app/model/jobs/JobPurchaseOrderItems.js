Ext.define('CBH.model.jobs.JobPurchaseOrderItems', {
    extend: 'Ext.data.Model',
    alias: 'model.JobPurchaseOrderItems',

    fields: [
        { name:'POItemsKey', type:'int', defaultValue: null },
        { name:'POItemsJobKey', type:'int', defaultValue: null },
        { name:'POItemsPOKey', type:'int', defaultValue: null },
        { name:'POItemsSort', type:'int', useNull: true, defaultValue: null },
        { name:'POItemsQty', type:'float', defaultValue: null },
        { name:'POItemsItemKey', type:'int', defaultValue: null },
        { name:'POItemsCost', type:'float', defaultValue: null },
        { name:'POItemsPrice', type:'float', defaultValue: null },
        { name:'POItemsLineCost', type:'float', defaultValue: null },
        { name:'POItemsLinePrice', type:'float', defaultValue: null },
        { name:'POItemsCurrencyCode', type:'string' },
        { name:'POItemsCurrencyRate', type:'float', defaultValue: null },
        { name:'POItemsWeight', type:'float', defaultValue: null },
        { name:'POItemsVolume', type:'float', defaultValue: null },
        { name:'POItemsLineWeight', type:'float', defaultValue: null },
        { name:'POItemsLineVolume', type:'float', defaultValue: null },
        { name:'POItemsBackorderQty', type:'int', useNull: true, defaultValue: null },
        { name:'POItemsMemoCustomer', type:'string', useNull: true, defaultValue: null },
        { name:'POItemsMemoCustomerMoveBottom', type:'boolean', defaultValue: null },
        { name:'POItemsMemoVendor', type:'string', useNull: true, defaultValue: null },
        { name:'POItemsMemoVendorMoveBottom', type:'boolean', defaultValue: null },
        { name:'POItemsQuoteItemKey', type:'int', useNull: true, defaultValue: null },
        { name:'x_ItemName', type:'string', useNull: true, defaultValue: null },
        { name:'x_LineCost', type:'float', defaultValue: null },
        { name:'x_LinePrice', type:'float', defaultValue: null },
        { name:'x_LineWeight', type:'float', defaultValue: null },
        { name:'x_LineVolume', type:'float', defaultValue: null },
        { name:'x_ItemNum', type:'string', useNull: true, defaultValue: null },
        { name:'x_ProfitMargin', type:'float',
            convert: function(val,row) {
                return GetProfitMargin(row.data.POItemsCost, row.data.POItemsPrice);
            }   
        }
    ],
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/JobPurchaseOrderItems',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'POItemsKey'
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
        })
    }
});