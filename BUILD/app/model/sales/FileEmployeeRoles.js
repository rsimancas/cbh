Ext.define('CBH.model.sales.FileEmployeeRoles', {
	extend: 'Ext.data.Model',
	idProperty: 'FileEmployeeKey',

	fields: [
	{ name:'FileEmployeeKey', type:'int' },
	{ name:'FileEmployeeFileKey', type:'int' },
	{ name:'FileEmployeeRoleKey', type:'int' },
	{ name:'FileEmployeeEmployeeKey', type:'int' },
	{ name:'FileEmployeeModifiedBy', type:'string', useNull: true, defaultValue: null },
	{ name:'FileEmployeeModifiedDate', type:'date', useNull: true },
	{ name:'FileEmployeeCreatedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
	{ name:'FileEmployeeCreatedDate', type:'date', defaultValue: new Date() },
	{ name:'x_EmployeeName', type:'string'},
	{ name:'x_RoleName', type:'string'}
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/FileEmployeeRoles',
		headers: {
			'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
		},
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'FileEmployeeKey'
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
	'CBH.model.sales.FileOverview',
	'CBH.model.sales.FileHeader'
	]

});