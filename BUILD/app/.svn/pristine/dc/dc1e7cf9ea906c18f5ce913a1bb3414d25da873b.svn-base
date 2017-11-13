Ext.define('CBH.model.sales.FileQuoteCharges', {
	extend: 'Ext.data.Model',
	idProperty: 'QChargeKey',

	fields: [
	{ name:'QChargeKey', type:'int', defaultValue: null },
    { name:'QChargeFileKey', type:'int', defaultValue: null },
    { name:'QChargeHdrKey', type:'int', defaultValue: null },
    { name:'QChargeSort', type:'int', defaultValue: null },
    { name:'QChargeQCDKey', type:'int', useNull: true, defaultValue: null },
    { name:'QChargeChargeKey', type:'int', useNull: true, defaultValue: null },
    { name:'QChargeMemo', type:'string', useNull: true, defaultValue: null },
    { name:'QChargeQty', type:'float', defaultValue: null },
    { name:'QChargeCost', type:'float', defaultValue: null },
    { name:'QChargePrice', type:'float', defaultValue: null },
    { name:'QChargeCostEstimated', type:'boolean', defaultValue: null },
    { name:'QChargeCurrencyCode', type:'string' },
    { name:'QChargeCurrencyRate', type:'float', defaultValue: null },
    { name:'QChargeFreightCompany', type:'int', useNull: true, defaultValue: null },
    { name:'QChargePrint', type:'boolean', defaultValue: null },
    { name:'x_ChargeCurrency', type:'string'},
    { name:'x_ChargeDescription', type:'string'},
    { name:'x_FreightCompany', type:'string'}
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/FileQuoteCharges',
		headers: {
			'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
		},
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'QChargeKey'
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