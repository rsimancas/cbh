Ext.define('CBH.model.common.States', {
    extend: 'Ext.data.Model',
    alias: 'model.states',

    fields: [
    { name:'StateCode', type:'string' },
    { name:'StateName', type:'string', useNull: true, defaultValue: null },
    { name:'StateCountryKey', type:'int', useNull: true },
    { name:'StateCountry', type:'string', useNull: true, defaultValue: null },
    { name:'StateCountryCode', type:'string' }
    ]
});