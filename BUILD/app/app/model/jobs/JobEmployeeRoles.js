Ext.define('CBH.model.jobs.JobEmployeeRoles', {
	extend: 'Ext.data.Model',
	idProperty: 'JobEmployeeKey',

	fields: [
	{ name:'JobEmployeeKey', type:'int' },
	{ name:'JobEmployeeJobKey', type:'int' },
	{ name:'JobEmployeeRoleKey', type:'int' },
	{ name:'JobEmployeeEmployeeKey', type:'int' },
	{ name:'JobEmployeeModifiedBy', type:'string', useNull: true, defaultValue: null },
	{ name:'JobEmployeeModifiedDate', type:'date', useNull: true },
	{ name:'JobEmployeeCreatedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
	{ name:'JobEmployeeCreatedDate', type:'date', defaultValue: new Date() },
	{ name:'x_EmployeeName', type:'string'},
	{ name:'x_RoleName', type:'string'}
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/JobEmployeeRoles',
		headers: {
			'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
		},
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'JobEmployeeKey'
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
	},

	belongsTo: [
	'CBH.model.jobs.JobHeader'
	]

});