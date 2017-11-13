Ext.define('CBH.model.common.Languages', {
    extend: 'Ext.data.Model',

    fields: [
        {
            name: 'LanguageCode',
            type: 'string'
        },
        {
            name: 'LanguageName',
            type: 'string'
        },
        {
            name: 'LanguageSort',
            type: 'int'
        }
    ],

    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/Languages',
        headers: {
           'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data'
        }
    }
});