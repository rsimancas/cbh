Ext.define('CBH.model.common.ReportCriteria', {
    extend: 'Ext.data.Model',
    idProperty: 'CriteriaKey',

    fields: [
        { name:'CriteriaKey', type:'int', defaultValue: null },
        { name:'CriteriaEmployeeKey', type:'int', defaultValue: null },
        { name:'CriteriaRptName', type:'string' },
        { name:'CriteriaFieldName', type:'string' },
        { name:'CriteriaValue', type:'int', defaultValue: null }
    ],

    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/ReportCriteria',
        headers: {
            'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'CriteriaKey'
        },
        afterRequest: function(request, success) {
            if (request.action == 'read') {
                //this.readCallback(request);
            } else if (request.action == 'create') {
                if (!request.operation.success) {
                    Ext.popupMsg.msg("Warning", "Record was not created");
                    Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    //Ext.popupMsg.msg("Success", "Created Successfully");
                }
            } else if (request.action == 'update') {
                if (!request.operation.success) {
                    Ext.popupMsg.msg("Warning", "Record was not saved");
                    Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    //Ext.popupMsg.msg("Success", "Updated Successfully");
                }
            } else if (request.action == 'destroy') {
                if (!request.operation.success) {
                    Ext.popupMsg.msg("Warning", "Record was not deleted");
                    //Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    //Ext.popupMsg.msg("Success", "Deleted Successfully");
                }
            }
        }
    }
});