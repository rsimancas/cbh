Ext.define('CBH.model.sales.FileQuoteSummary', {
	extend: 'Ext.data.Model',
	idProperty: 'QHdrKey',

	fields: [
	{ name:'QHdrFileKey', type:'int' },
	{ name:'QHdrKey', type:'int' },
	{ name:'Date', type:'string', useNull: true, defaultValue: null },
	{ name:'Quote', type:'string', useNull: true, defaultValue: null },
	{ name:'Vendors', type:'int', useNull: true },
	{ name:'Status', type:'string' }
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/FileQuoteSummary',
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
	},

	belongsTo: 'CBH.model.sales.FileOverview'

});