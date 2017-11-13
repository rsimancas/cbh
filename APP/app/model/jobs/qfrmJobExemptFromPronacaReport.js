Ext.define('CBH.model.jobs.qfrmJobExemptFromPronacaReport', {
    extend: 'Ext.data.Model',

    fields: [
        { name:'JobKey', type:'int', defaultValue: null },
        { name:'JobNum', type:'string', useNull: true, defaultValue: null },
        { name:'JobProdDescription', type:'string', useNull: true, defaultValue: null },
        { name:'JobReference', type:'string', useNull: true, defaultValue: null },
        { name:'CustName', type:'string', useNull: true, defaultValue: null },
        { name:'CustContact', type:'string' },
        { name:'JobExemptFromPronacaReport', type:'boolean', defaultValue: null }
    ],

    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/qfrmJobExemptFromPronacaReport',
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
            var silentMode = this.getSilentMode();

            if (request.action == 'read') {
                //this.readCallback(request);
            } else if (request.action == 'create') {
                if (!request.operation.success) {
                    Ext.popupMsg.msg("Warning", "Record was not saved!!!");
                    Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    if (!silentMode)
                        Ext.popupMsg.msg("Success", "Saved Successfully!!!");
                }
            } else if (request.action == 'update') {
                if (!request.operation.success) {
                    Ext.popupMsg.msg("Warning", "Record was not saved!!!");
                    Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    if (!silentMode)
                        Ext.popupMsg.msg("Success", "Saved Successfully!!!");
                }
            } else if (request.action == 'destroy') {
                if (!request.operation.success) {
                    Ext.popupMsg.msg("Warning", "Record was not Deleted");
                    //Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    if (!silentMode)
                        Ext.popupMsg.msg("Success", "Deleted Successfully");
                }
            }

            this.setSilentMode(false);
        }
    }
});
