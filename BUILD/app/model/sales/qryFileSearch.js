Ext.define('CBH.model.sales.qryFileSearch', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'FileKey', type:'int', defaultValue: null },
        { name:'FileNum', type:'string', useNull: true, defaultValue: null },
        { name:'SalesEmployee', type:'string', useNull: true, defaultValue: null },
        { name:'OrderEmployee', type:'string', useNull: true, defaultValue: null },
        { name:'CustName', type:'string', useNull: true, defaultValue: null },
        { name:'CustContact', type:'string', useNull: true, defaultValue: null },
        { name:'CustShipName', type:'string', useNull: true, defaultValue: null },
        { name:'FileReference', type:'string', useNull: true, defaultValue: null },
        { name:'FileCurrencyCode', type:'string' },
        { name:'QHdrKey', type:'int', useNull: true, defaultValue: null },
        { name:'QHdrNum', type:'string', useNull: true, defaultValue: null },
        { name:'VendorName', type:'string', useNull: true, defaultValue: null },
        { name:'VendorContact', type:'string', useNull: true, defaultValue: null },
        { name:'ShipType', type:'string', useNull: true, defaultValue: null },
        { name:'WarehouseName', type:'string', useNull: true, defaultValue: null },
        { name:'VendorCurrencyCode', type:'string', useNull: true, defaultValue: null }
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/qryFileSearch',
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