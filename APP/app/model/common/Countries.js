Ext.define('CBH.model.common.Countries', {
    extend: 'Ext.data.Model',

    fields: [
    { name:'CountryKey', type:'int' },
    { name:'CountryCode', type:'string' },
    { name:'CountryName', type:'string' },
    { name:'CountryFOBValueForInspection', type:'float' },
    { name:'CountryFOBValueForInspectionCurrencyCode', type:'string' },
    { name:'CountryModifiedBy', type:'string', useNull: true, defaultValue: null },
    { name:'CountryModifiedDate', type:'date', useNull: true }
    ]
});