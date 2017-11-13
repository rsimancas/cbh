Ext.define('CBH.model.sales.qfrmFileStatusHistory', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'FileKey', type:'int', defaultValue: null },
        { name:'FileModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'FileModifiedDate', type:'date', useNull: true, defaultValue: null },
        { name:'EmployeeKey', type:'int', useNull: true, defaultValue: null },
        { name:'EmployeeEmail', type:'string', useNull: true, defaultValue: null },
        { name:'CustEmail', type:'string', useNull: true, defaultValue: null },
        { name:'FileReference', type:'string', useNull: true, defaultValue: null },
        { name:'CustName', type:'string', useNull: true, defaultValue: null },
        { name:'ContactTitle', type:'string', useNull: true, defaultValue: null },
        { name:'ContactFirstName', type:'string', useNull: true, defaultValue: null },
        { name:'ContactLastName', type:'string', useNull: true, defaultValue: null }
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/qfrmFileStatusHistory',
		headers: {
			'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
		},
		reader:{
			type:'json',
			root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message'
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
	}
});