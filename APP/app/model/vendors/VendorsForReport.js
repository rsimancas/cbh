Ext.define('CBH.model.vendors.VendorsForReport', {
    extend: 'Ext.data.Model',
    idProperty: 'VendorKey',

    fields: [
        { name:'VendorKey', type:'int' },
        { name:'VendorName', type:'string' }
    ]
});