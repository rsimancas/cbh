Ext.define('CBH.model.common.Employees', {
    extend: 'Ext.data.Model',

    fields: [
        { name:'EmployeeKey', type:'int', defaultValue: null },
        { name:'EmployeeFirstName', type:'string' },
        { name:'EmployeeMiddleInitial', type:'string', useNull: true, defaultValue: null },
        { name:'EmployeeLastName', type:'string' },
        { name:'EmployeeTitleCode', type:'int', useNull: true, defaultValue: null },
        { name:'EmployeeEmail', type:'string', useNull: true, defaultValue: null },
        { name:'EmployeeEmailCC', type:'string', useNull: true, defaultValue: null },
        { name:'EmployeeAddress1', type:'string', useNull: true, defaultValue: null },
        { name:'EmployeeCity', type:'string', useNull: true, defaultValue: null },
        { name:'EmployeeState', type:'string', useNull: true, defaultValue: null },
        { name:'EmployeeZip', type:'string', useNull: true, defaultValue: null },
        { name:'EmployeePhone', type:'string', useNull: true, defaultValue: null },
        { name:'EmployeeLogin', type:'string', useNull: true, defaultValue: null },
        { name:'EmployeeStatusCode', type:'int', defaultValue: null },
        { name:'EmployeePeachtreeID', type:'string', useNull: true, defaultValue: null },
        { name:'EmployeeLocationKey', type:'int', defaultValue: 1 },
        { name:'EmployeePassword', type:'string' },
        { name:'EmployeeModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'EmployeeModifiedDate', type:'date', useNull: true, defaultValue: null },
        { name:'EmployeeCreatedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
        { name:'EmployeeCreatedDate', type:'date', defaultValue: null },
        { name:'EmployeeSecurityLevel', type:'boolean', defaultValue: null },
        { name:'EmployeeAccessLevel', type:'int', defaultValue: null },
        { name: 'x_EmployeeFullName', type:'string'}
    ],

    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/Employees',
        headers: {
            'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'EmployeeKey'
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