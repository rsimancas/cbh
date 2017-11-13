Ext.define('CBH.model.sales.FileQuoteHeader', {
	extend: 'Ext.data.Model',
	idProperty: 'QHdrKey',

	fields: [
    	{ name:'QHdrKey', type:'int' },
        { name:'QHdrFileKey', type:'int' },
        { name:'QHdrPrefix', type:'string' },
        { name:'QHdrNum', type:'int' },
        { name:'QHdrRevision', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrDate', type:'date' },
        { name:'QHdrGoodThruDate', type:'date', useNull: true, defaultValue: null },
        { name:'QHdrCustPaymentTerms', type:'int' },
        { name:'QHdrMemo', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrExFactoryOption', type:'boolean' },
        { name:'QHdrExFactoryLocation', type:'int', useNull: true },
        { name:'QHdrFOBOption', type:'boolean' },
        { name:'QHdrFOBLocation', type:'int', useNull: true },
        { name:'QHdrCIFOption', type:'int' },
        { name:'QHdrCIFLocation', type:'int', useNull: true },
        { name:'QHdrFreightDirect', type:'boolean' },
        { name:'QHdrLeadTime', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrCarrierKey', type:'int', useNull: true },
        { name:'QHdrWarehouseKey', type:'int', useNull: true },
        { name:'QHdrShipType', type:'int' },
        { name:'QHdrSentDate', type:'date', useNull: true },
        { name:'QHdrCurrencyCode', type:'string' },
        { name:'QHdrCurrencyRate', type:'float' },
        { name:'QHdrProdDescription', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrShippingDescription', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrCustRefNum', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrModifiedDate', type:'date', useNull: true },
        { name:'QHdrCreatedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
        { name:'QHdrCreatedDate', type:'date', defaultValue: new Date() },
        { name:'QHdrQuoteConfirmationDate', type:'date', useNull: true },
        { name:'QHdrInsurance', type:'boolean', useNull: true },
        { name:'QHdrInspectorKey', type:'int', useNull: true },
        { name:'QHdrInspectionNum', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrDUINum', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrJobKey', type:'int', useNull: true },
        { name:'QHdrClosed', type:'date', useNull: true },
        { name:'x_FileCurrencyCode', type:'string', useNull: true, defaultValue: null},
        { name:'x_FileCurrencyRate', type:'float', useNull: true, defaultValue: null},
        { name:'x_FileReference', type:'string', useNull: true, defaultValue: null},
        { name:'x_CustName', type:'string', useNull: true, defaultValue: null},
        { name:'x_CustLanguageCode', type:'string', useNull: true, defaultValue: null},
        { name:'x_CustContactName', type:'string', useNull: true, defaultValue: null},
        { name:'x_Status', type:'string', useNull: true, defaultValue: null},
        { name:'JobNum', type:'string', useNull: true, defaultValue: null},
        { name:'QuoteNum', type:'string', useNull: true, defaultValue: null},
        { name:'x_QHdrCurrencyCode', type:'string',
            convert: function(val,row) {
                return row.data.QHdrCurrencyCode;
            }
        }
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/FileQuoteHeader',
		headers: {
			'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
		},
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'QHdrKey'
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