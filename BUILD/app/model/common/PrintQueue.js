Ext.define('CBH.model.common.PrintQueue', {
    extend: 'Ext.data.Model',
    alias: 'model.PrintQueue',

    fields: [
        { name: 'PrintKey', type: 'int', defaultValue: null },
        { name: 'PrintEmployeeKey', type: 'int', defaultValue: null },
        { name: 'PrintRptName', type: 'string' },
        { name: 'PrintStrWhere', type: 'string', useNull: true, defaultValue: null }
    ],

    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/PrintQueue',
        headers: {
            'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message'
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
    }
});
