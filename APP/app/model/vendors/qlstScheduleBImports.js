Ext.define('CBH.model.vendors.qlstScheduleBImports', {
    extend: 'Ext.data.Model',
    alias: 'model.qlstschedulebimports',
    idProperty: 'SBLanguageKey',

    fields: [
        { name:'SBLanguageKey', type:'int', defaultValue: null },
        { name:'SchBNumC', type:'string', useNull: true, defaultValue: null },
        { name:'SBLanguageSchBSubNum', type:'string', useNull: true, defaultValue: null },
        { name:'SBLanguageText', type:'string' },
        { name:'SchBNum', type:'string' },
        { name:'x_Description', type:'string'},
        { name:'x_SchBNumFormatted', type:'string'}
    ],

    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/qlstScheduleBImports',
        headers: {
            'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'SBLanguageKey'
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