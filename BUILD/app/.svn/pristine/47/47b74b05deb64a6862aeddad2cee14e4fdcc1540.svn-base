Ext.define('CBH.model.jobs.InvoiceChargesSubTotal', {
    extend: 'Ext.data.Model',
    alias: 'model.invoicechargessubtotal',
    idProperty: 'id',

    fields: [
        { name:'ISTInvoiceKey', type:'int', defaultValue: null },
        { name:'ISTSubTotalKey', type:'int', defaultValue: null },
        { name:'ISTLocation', type:'int', useNull: true, defaultValue: null },
        { name:'x_Location', type:'string', useNull: true, defaultValue: null},
        { name:'x_Category', type:'string', useNull: true, defaultValue: null},
        { name:'x_SubTotalSort', type:'string', useNull: true, defaultValue: null},
        { name:'id', type:'string', 
            convert: function(value,record) {
                return record.get('ISTInvoiceKey') + ',' + record.get('ISTSubTotalKey');
            }
        }
    ],
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/InvoiceChargesSubTotal',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'id'
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