Ext.define('CBH.model.sales.FileList', {
	extend: 'Ext.data.Model',
    idProperty: 'FileKey',

    requires: [
        'CBH.model.sales.QuotesById',
        'CBH.model.sales.FileStatusOverview',
        'CBH.model.sales.FileQuoteDetail' /* rule 2 */
    ],


	fields: [
		{ name: 'FileKey', type: 'int' },
		{ name: 'Date'},
		{ name: 'FileNum'},
		{ name: 'Customer'},
		{ name: 'Reference'},
		{ name: 'Status'},
		{ name: 'CreatedBy', type: 'string', defaultValue: CBH.GlobalSettings.getCurrentUserName()},
		{ name: 'ModifiedBy'},
		{ name: 'CreatedDate', type: 'date'},
		{ name: 'ModifiedDate', type: 'date'},
		{ name: 'FileClosed', type: 'date'},
		{ name: 'FileCustKey', type: 'int' }
	],

    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/File',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'FileKey'
        },
        writer: {
            type:'json',
            writeAllFields: true
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

	hasMany: [{
	    name: "Quotes",
	    model: 'CBH.model.sales.QuotesById',
	    primaryKey: 'FileKey',
	    foreignKey: 'QHdrFileKey',
	    //autoLoad: true,
	    associationKey: 'quotes' // read child data from nested.child_groups
	}, {
	    name: "Status",
	    model: 'CBH.model.sales.FileStatusOverview',
	    primaryKey: 'FileKey',
	    foreignKey: 'StatusFileKey',
	    //autoLoad: true,
	    associationKey: 'status' // read child data from nested.child_groups
	}, {
	    name: "FileQuoteDetail",
	    model: 'CBH.model.sales.FileQuoteDetail',
	    primaryKey: 'FileKey',
	    foreignKey: 'QuoteFileKey',
	    //autoLoad: true,
	    associationKey: 'detail' // read child data from nested.child_groups
	}]
});