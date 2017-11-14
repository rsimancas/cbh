Ext.define('CBH.model.common.tsysEmployeeCodes', {
    extend: 'Ext.data.Model',

    fields: [
        { name:'TextKey', type:'int', defaultValue: null },
        { name:'TextExpression', type:'int', defaultValue: null },
        { name:'TextLanguageCode', type:'string' },
        { name:'Text', type:'string' },
        { name:'TextCategory', type:'int', defaultValue: null }
    ],
    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/tsysEmployeeCodes',
        headers: {
            'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'TextKey'
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
    }
});