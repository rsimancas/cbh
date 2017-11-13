Ext.define('CBH.model.jobs.JobPurchaseOrderCharges', {
    extend: 'Ext.data.Model',
    alias: 'model.JobPurchaseOrderCharges',
    idProperty: 'POChargesKey',

    fields: [
    { name:'POChargesKey', type:'int', defaultValue: null },
    { name:'POChargesJobKey', type:'int', defaultValue: null },
    { name:'POChargesPOKey', type:'int', defaultValue: null },
    { name:'POChargesSort', type:'int', defaultValue: null },
    { name:'POChargesChargeKey', type:'int', defaultValue: null },
    { name:'POChargesMemo', type:'string', useNull: true, defaultValue: null },
    { name:'POChargesQty', type:'float', defaultValue: null },
    { name:'POChargesCost', type:'float', defaultValue: null },
    { name:'POChargesPrice', type:'float', defaultValue: null },
    { name:'POChargesCurrencyCode', type:'string' },
    { name:'POChargesCurrencyRate', type:'float', defaultValue: null },
    { name:'POChargesFreightCompany', type:'int', useNull: true, defaultValue: null },
    { name:'x_UnitCost', type:'float', defaultValue: null },
    { name:'x_UnitPrice', type:'float', defaultValue: null },
    { name:'x_DescriptionText', type:'string', useNull: true, defaultValue: null }
    ],
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/JobPurchaseOrderCharges',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'POChargesKey'
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