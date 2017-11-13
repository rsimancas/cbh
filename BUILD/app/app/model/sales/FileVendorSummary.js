Ext.define('CBH.model.sales.FileVendorSummary', {
	extend: 'Ext.data.Model',
	//idProperty: 'QHdrFileKey',

	fields: [
	{ name:'QuoteFileKey', type:'int' },
	{ name:'VendorKey', type:'int' },
	{ name:'Vendor', type:'string' },
	{ name:'Qty', type:'int', useNull: true },
	{ name:'Cost', type:'float', useNull: true },
	{ name:'Price', type:'float', useNull: true },
	{ name:'Currency', type:'string' },
	{ name:'x_Selected', type:'bool', defaultValue: false}
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/FileVendorSummary',
		headers: {
			'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
		},
		reader:{
			type:'json',
			root:'data'
		}
	},

	belongsTo: 'CBH.model.sales.FileOverview'

});