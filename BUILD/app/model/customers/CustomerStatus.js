Ext.define('CBH.model.customers.CustomerStatus', {
    extend: 'Ext.data.Model',
    alias: 'model.customerstatus',
    idProperty: 'StatusKey',

    fields: [
    { name:'StatusKey', type:'int', convert: null },
    { name:'StatusDescription', type:'string' }
    ],

    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/CustomerStatus',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message'
        }
    },

    //belongsTo: 'CBH.model.customers.Customers'
});