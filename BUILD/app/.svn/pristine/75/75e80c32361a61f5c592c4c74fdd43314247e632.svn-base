Ext.define('CBH.model.common.ScheduleBLanguage', {
    extend: 'Ext.data.Model',
    //alias: 'model.paymenttermsdescriptions',
    idProperty: 'SBLanguageKey',

    fields: [
        { name:'SBLanguageKey', type:'int', defaultValue: null },
        { name:'SBLanguageSchBNum', type:'string' },
        { name:'SBLanguageSchBSubNum', type:'string', useNull: true, defaultValue: null },
        { name:'SBLanguageCode', type:'string' },
        { name:'SBLanguageText', type:'string' },
        { name:'x_Language', type:'string' },
        { name: 'x_SchBNumFormatted', 
            type: 'string',
            convert: function(val,row) {
                var val = row.data.SBLanguageSchBSubNum;
                var p = row.data.SBLanguageSchBNum;

                val = (val === null) ? '' : val;
                p = (p === null) ? '' : p;

                var m = p.match(/^(\d{4})?[\.]?(\d{2})?[\.]?(\d{4})$/);

                if (/^(\d{4})?[\.]?(\d{2})?[\.]?(\d{4})$/.test(p) && m[1] && m[2]) {
                    return m[1] + '.' + m[2] + '.' + m[3] + val;
                } else {
                    return val;
                }
            }   
        }
    ],

    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/ScheduleBLanguage',
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