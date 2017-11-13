Ext.define('CBH.model.jobs.qryJobSearch', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'JobKey', type:'int', defaultValue: null },
        { name:'JobNum', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrKey', type:'int', useNull: true, defaultValue: null },
        { name:'QHdrNum', type:'string', useNull: true, defaultValue: null },
        { name:'JobProdDescription', type:'string', useNull: true, defaultValue: null },
        { name:'JobReference', type:'string', useNull: true, defaultValue: null },
        { name:'JobOrderEmployee', type:'string', useNull: true, defaultValue: null },
        { name:'CustName', type:'string', useNull: true, defaultValue: null },
        { name:'CustContact', type:'string', useNull: true, defaultValue: null },
        { name:'CustShipName', type:'string', useNull: true, defaultValue: null },
        { name:'JobCustRefNum', type:'string', useNull: true, defaultValue: null },
        { name:'JobCustCurrencyCode', type:'string' },
        { name:'ShipType', type:'string', useNull: true, defaultValue: null },
        { name:'InspectionNum', type:'string', useNull: true, defaultValue: null },
        { name:'JobDUINum', type:'string', useNull: true, defaultValue: null },
        { name:'JobShipDate', type:'date', defaultValue: null },
        { name:'JobClosed', type:'date', useNull: true, defaultValue: null }
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/qryJobSearch',
		headers: {
			'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
		},
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
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