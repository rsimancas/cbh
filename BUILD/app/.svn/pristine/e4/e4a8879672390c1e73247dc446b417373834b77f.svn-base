Ext.define('CBH.model.sales.FileStatus', {
	extend: 'Ext.data.Model',
	idProperty: 'StatusKey',

	fields: [
	{ name:'StatusKey', type:'int' },
	{ name:'StatusCategory', type:'int' },
	{ name:'StatusSort', type:'int' },
	{ name:'StatusText', type:'string' },
	{ name:'StatusPublicDefault', type:'boolean' },
	{ name:'StatusCustEntry', type:'boolean' },
	{ name:'StatusClosed', type:'boolean' },
	{ name:'StatusCompleted', type:'boolean' }
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/FileStatus',
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