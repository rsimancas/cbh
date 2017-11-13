Ext.define('CBH.model.jobs.InvoiceCharges', {
    extend: 'Ext.data.Model',
    alias: 'model.invoicecharges',
    idProperty: 'IChargeKey',

    fields: [
    { name:'IChargeKey', type:'int', defaultValue: null },
    { name:'IChargeInvoiceKey', type:'int', defaultValue: null },
    { name:'IChargeSort', type:'int', defaultValue: null },
    { name:'IChargeChargeKey', type:'int', useNull: true, defaultValue: null },
    { name:'IChargeMemo', type:'string', useNull: true, defaultValue: null },
    { name:'IChargeQty', type:'float', defaultValue: null },
    { name:'IChargeCost', type:'float', defaultValue: null },
    { name:'IChargePrice', type:'float', defaultValue: null },
    { name:'IChargeCurrencyCode', type:'string' },
    { name:'IChargeCurrencyRate', type:'float', defaultValue: null },
    { name:'LineCost', type:'float', defaultValue: null },
    { name:'LinePrice', type:'float', defaultValue: null },
    { name:'x_UnitCost', type:'float', defaultValue: null },
    { name:'x_UnitPrice', type:'float', defaultValue: null },
    { name:'x_ChargeCurrency', type:'string'},
    { name:'x_ChargeDescription', type:'string'}
    ],
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/InvoiceCharges',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'IChargeKey'
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