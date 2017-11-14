Ext.define('CBH.model.sales.FileStatusHistorySubDetails', {
	extend: 'Ext.data.Model',

	fields: [
	{ name:'StatusFileKey', type:'int' },
	{ name:'StatusQuoteNum', type:'string', useNull: true, defaultValue: null },
	{ name:'StatusDate', type:'date' },
	{ name:'StatusStatusKey', type:'int' },
	{ name:'StatusMemo', type:'string', useNull: true, defaultValue: null },
	{ name:'StatusModifiedBy', type:'string' },
	{ name:'StatusModifiedDate', type:'date' },
	{ name:'StatusKey', type:'int' },
	{ name:'x_Status', type:'string' },
	{ name:'x_FileClosed', type:'date'}	
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/FSHSubDetails',
		headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
		reader:{
			type:'json',
			root:'data'
		}
	}
});