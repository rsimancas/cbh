Ext.define('CBH.model.sales.FileQuoteItemsSummary', {
	extend: 'Ext.data.Model',

	fields: [
	{ name:'QSummaryKey', type:'int', defaultValue: null },
    { name:'QSummaryQHdrKey', type:'int', defaultValue: null },
    { name:'QSummarySort', type:'int', defaultValue: null },
    { name:'QSummaryQty', type:'float', defaultValue: null },
    { name:'QSummaryVendorKey', type:'int', useNull: true, defaultValue: null },
    { name:'QSummaryItemNum', type:'string', useNull: true, defaultValue: null },
    { name:'QSummaryDescription', type:'string', useNull: true, defaultValue: null },
    { name:'QSummaryPrice', type:'float', defaultValue: null },
    { name:'QSummaryLinePrice', type:'float', defaultValue: null },
    { name:'QSummaryCurrencyCode', type:'string' },
    { name:'QSummaryCurrencyRate', type:'float', defaultValue: null },
    { name:'QSummaryDescriptionFontColor', type:'int', defaultValue: null },
    { name:'x_VendorName', type:'string'}
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/FileQuoteItemsSummary',
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