Ext.define('CBH.model.common.Users', {
    extend: 'Ext.data.Model',

    fields: [
        {
            name: 'UserName'
        },
        {
            name: 'UserPassword'
        },
        {
            name: 'UserFullName'
        },
        {
            name: 'EmployeeKey',
            type: 'int'
        },
        {
            name: 'EmployeeAccessLevel',
            type: 'int'
        },
        {
            name: 'RememberMe',
            type: 'bool'
        }
    ]
});