Ext.define('CBH.model.sales.qfrmFileQuoteStatusHistory', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'QHdrKey', type:'int', defaultValue: null },
        { name:'QHdrFileKey', type:'int', defaultValue: null },
        { name:'QuoteNum', type:'string', useNull: true, defaultValue: null },
        { name:'EmployeeKey', type:'int', useNull: true, defaultValue: null },
        { name:'EmployeeEmail', type:'string', useNull: true, defaultValue: null },
        { name:'CustEmail', type:'string', useNull: true, defaultValue: null },
        { name:'ForwarderEmail', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrClosed', type:'date', useNull: true, defaultValue: null },
        { name:'QHdrModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrModifiedDate', type:'date', useNull: true, defaultValue: null },
        { name:'CustName', type:'string', useNull: true, defaultValue: null },
        { name:'ContactTitle', type:'string', useNull: true, defaultValue: null },
        { name:'ContactFirstName', type:'string', useNull: true, defaultValue: null },
        { name:'ContactLastName', type:'string', useNull: true, defaultValue: null },
        { name:'FileReference', type:'string', useNull: true, defaultValue: null }
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/qfrmFileQuoteStatusHistory',
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