Ext.define('CBH.model.jobs.qfrmJobStatusHistory', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'JobKey', type:'int', defaultValue: null },
        { name:'JobModifiedBy', type:'string', useNull: true, defaultValue: CBH.GlobalSettings.getCurrentUserName() },
        { name:'JobModifiedDate', type:'date', useNull: true, defaultValue: null },
        { name:'EmployeeKey', type:'int', useNull: true, defaultValue: null },
        { name:'EmployeeEmail', type:'string', useNull: true, defaultValue: null },
        { name:'CustEmail', type:'string' },
        { name:'ForwarderEmail', type:'string', useNull: true, defaultValue: null },
        { name:'JobClosed', type:'date', useNull: true, defaultValue: null },
        { name:'JobComplete', type:'date', useNull: true, defaultValue: null },
        { name:'QuoteNum', type:'string', useNull: true, defaultValue: null },
        { name:'JobProdDescription', type:'string', useNull: true, defaultValue: null },
        { name:'JobCustRefNum', type:'string', useNull: true, defaultValue: null },
        { name:'CustName', type:'string', useNull: true, defaultValue: null },
        { name:'ContactName', type:'string' },
        { name:'JobStatusKey', type:'int', defaultValue: null },
        { name:'StatusDate', type:'date', useNull: true, defaultValue: null },
        { name:'StatusMemo', type:'string', useNull: true, defaultValue: null },
        { name:'StatusPublic', type:'boolean', defaultValue: null }
	],

	proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/qfrmJobStatusHistory',
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