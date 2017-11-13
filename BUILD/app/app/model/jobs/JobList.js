Ext.define('CBH.model.jobs.JobList', {
    extend: 'Ext.data.Model',
    idProperty: 'JobKey',

    fields: [
        { name: 'JobKey', type: 'int' },
        { name: 'Date', type: 'date' },
        { name: 'JobNum', type: 'string' },
        { name: 'Customer', type: 'string' },
        { name: 'Reference', type: 'string' },
        { name: 'Status', type: 'string' },
        { name: 'JobCreatedBy', type: 'string' },
        { name: 'JobClosed', type: 'date' },
        { name: 'JobStatusDesc', type: 'string' },
        { name: 'StatusModifiedBy', type: 'string' },
        { name: 'StatusModifiedDate', type: 'date' },
        { name: 'row', type: 'int' },
        { name: 'CustCurrencyCode', type:'string', useNull: true, defaultValue: null },
        { name: 'CustCurrencyRate', type:'float', useNull: true, defaultValue: null },
        { name: 'JobCustKey', type: 'int', useNull: true, defaultValue: null },
        { name: 'JobCustShipKey', type: 'int', useNull: true, defaultValue: null },
        { name: 'JobWarehouseKey', type: 'int', useNull: true, defaultValue: null },
        { name: 'JobShipType', type: 'int', useNull: true, defaultValue: null },
        { name: 'JobQHdrKey', type: 'int', useNull: true, defaultValue: null },
        { name: 'Quote', type:'string', useNull: true, defaultValue: null },
        { name: 'QHdrFileKey', type: 'int', useNull: true, defaultValue: null },
        { name: 'FileNum', type:'string', useNull: true, defaultValue: null }
    ]
});