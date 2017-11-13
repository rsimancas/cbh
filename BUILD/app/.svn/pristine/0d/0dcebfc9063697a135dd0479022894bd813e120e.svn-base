Ext.define('CBH.model.sales.FileQuoteChargesSubTotals', {
	extend: 'Ext.data.Model',

	fields: [
	{ name:'QSTQHdrKey', type:'int', defaultValue: null },
    { name:'QSTSubTotalKey', type:'int', defaultValue: null },
    { name:'QSTLocation', type:'int', useNull: true, defaultValue: null },
    { name:'x_SubTotalSort', type:'int', useNull: true, defaultValue: null},
    { name:'x_Location', type:'string', useNull: true, defaultValue: null},
    { name:'x_Category', type:'string', useNull: true, defaultValue: null}
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/FileQuoteChargesSubTotals',
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
	},

	belongsTo: [
	'CBH.model.sales.FileQuoteHeader'
	]

});