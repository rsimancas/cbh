Ext.define('CBH.model.jobs.qLstJobInvoices', {
    extend: 'Ext.data.Model',
    idProperty: 'InvoiceKey',

    fields: [
    { name:'InvoiceKey', type:'int', defaultValue: null },
    { name:'JobKey', type:'int', useNull: true, defaultValue: null },
    { name:'Date', type:'date', useNull: true, defaultValue: null },
    { name:'Invoice', type:'string', useNull: true, defaultValue: null },
    { name:'BillTo', type:'string', useNull: true, defaultValue: null },
    { name:'Price', type:'float', useNull: true, defaultValue: null },
    { name:'CurrencyCode', type:'string', useNull: true, defaultValue: null },
    { name:'CurrencyRate', type:'float', useNull: true, defaultValue: null }
    ]
});