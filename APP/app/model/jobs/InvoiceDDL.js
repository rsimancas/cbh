Ext.define('CBH.model.jobs.InvoiceDDL', {
    extend: 'Ext.data.Model',
    alias: 'model.invoiceddl',

    fields: [
        { name:'InvoiceKey', type:'int', defaultValue: null },
        { name:'x_InvoiceNum', type:'string', useNull: true, defaultValue: null }
    ],
    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/InvoiceDDL',
        headers: {
            'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'InvoiceKey'
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
    load: function(id, config) {
        config = Ext.apply({}, config);
        config = Ext.applyIf(config, {
            model: this, //this line is necessary
            action: 'read',
            params: {
                id: id
            }
        });
    }
});