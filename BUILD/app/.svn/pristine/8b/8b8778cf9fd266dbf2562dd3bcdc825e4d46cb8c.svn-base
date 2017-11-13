Ext.define('CBH.model.sales.FileStatusOverview', {
	extend: 'Ext.data.Model',
	idProperty: 'StatusStatusKey',

	fields: [
	{ name: 'StatusStatusKey', type: 'int' },
	{ name: 'StatusFileKey', type: 'int' },
	{ name: 'StatusDate'},
	{ name: 'StatusMemo'}
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/FileStatusOverview',
		headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
		reader:{
			type:'json',
			root:'data'
		}
	},

	belongsTo: 'CBH.model.sales.FileList'

});