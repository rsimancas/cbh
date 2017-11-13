Ext.define('CBH.model.sales.FileOverview', {
	extend: 'Ext.data.Model',
	idProperty: 'FileKey',

	requires:[
	'CBH.model.sales.FileQuoteSummary',
	'CBH.model.sales.FileVendorSummary'          /* rule 2 */
	],

	fields: [
		{ name:'FileKey', type:'int' },
		{ name:'FileYear', type:'int' },
		{ name:'FileNum', type:'int' },
		{ name:'FileStatusKey', type:'int' },
		{ name:'FileQuoteEmployeeKey', type:'int' },
		{ name:'FileOrderEmployeeKey', type:'int', useNull: true },
		{ name:'FileCustKey', type:'int', useNull: true },
		{ name:'FileContactKey', type:'int', useNull: true },
		{ name:'FileCustShipKey', type:'int', useNull: true },
		{ name:'FileReference', type:'string', useNull: true, defaultValue: null },
		{ name:'FileProfitMargin', type:'float' },
		{ name:'FileCurrentVendor', type:'int', useNull: true },
		{ name:'FileModifiedBy', type:'string', useNull: true, defaultValue: null },
		{ name:'FileModifiedDate', type:'date', useNull: true },
		{ name:'FileCreatedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
		{ name:'FileCreatedDate', type:'date' },
		{ name:'FileDateCustRequired', type:'date', useNull: true },
		{ name:'FileDateCustRequiredNote', type:'string', useNull: true, defaultValue: null },
		{ name:'FileDefaultCurrencyCode', type:'string' },
		{ name:'FileDefaultCurrencyRate', type:'float' },
		{ name:'FileClosed', type:'date', useNull: true },
		{ name:'CustName', type:'string', useNull: true, defaultValue: null },
		{ name:'CustPeachtreeID', type:'string', useNull: true, defaultValue: null },
		{ name:'CustPhone', type:'string', useNull: true, defaultValue: null },
		{ name:'CustFax', type:'string', useNull: true, defaultValue: null },
		{ name:'CustEmail', type:'string', useNull: true, defaultValue: null },
		{ name:'ContactFirstName', type:'string', useNull: true, defaultValue: null },
		{ name:'ContactLastName', type:'string', useNull: true, defaultValue: null },
		{ name:'ContactPhone', type:'string', useNull: true, defaultValue: null },
		{ name:'ContactFax', type:'string', useNull: true, defaultValue: null },
		{ name:'ContactEmail', type:'string', useNull: true, defaultValue: null },
		{ 
			name: 'ContactFullName', 
			type: 'string',
			convert: function(val,row) {
				return (row.data.ContactFirstName !== null) ? row.data.ContactLastName +', '+ row.data.ContactFirstName : '';
			}   
		},
		{ 
			name: 'Phone', 
			type: 'string',
			convert: function(val,row) {
				var phone = "";
				if (row.data.CustPhone !== null) phone = row.data.CustPhone;
				if (row.data.ContactPhone !== null) phone = row.data.ContactPhone;
				
				return phone;
			}   
		},
		{ 
			name: 'Fax', 
			type: 'string',
			convert: function(val,row) {
				var phone = "";
				if (row.data.CustFax !== null) phone = row.data.CustFax;
				if (row.data.ContactFax !== null) phone = row.data.ContactFax;
				
				return phone;
			}   
		},
		{ 
			name: 'Email', 
			type: 'string',
			convert: function(val,row) {
				var phone = "";
				if (row.data.CustEmail !== null) phone = row.data.CustEmail;
				if (row.data.ContactEmail !== null) phone = row.data.ContactEmail;
				
				return phone;
			}   
		},
		{ name:'x_FileNum', type:'string', useNull: true, defaultValue: null }
	],
	hasMany: [{
		name: "Quotes",
		model: 'CBH.model.sales.FileQuoteSummary',
		primaryKey: 'FileKey',
		foreignKey: 'QHdrFileKey',
        associationKey: 'quotes' // read child data from nested.child_groups
    },
    {
    	name: "Vendors",
    	model: 'CBH.model.sales.FileVendorSummary',
    	primaryKey: 'FileKey',
    	foreignKey: 'QuoteFileKey',
        associationKey: 'vendors' // read child data from nested.child_groups
    }
    ]
});