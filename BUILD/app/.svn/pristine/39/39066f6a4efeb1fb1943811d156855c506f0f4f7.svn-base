Ext.define('CBH.model.common.ScheduleB', {
    extend: 'Ext.data.Model',

    fields: [
        { name:'SchBNum', type:'string' },
        { name:'SchBShortDescription', type:'string', useNull: true, defaultValue: null },
        { name:'SchBLongDescription', type:'string', useNull: true, defaultValue: null },
        { name:'SchBUnitOfMeasure', type:'string', useNull: true, defaultValue: null },
        { name:'SchBUnitOfMeasure2', type:'string', useNull: true, defaultValue: null },
        { name:'SchBSITC', type:'string', useNull: true, defaultValue: null },
        { name:'SchBEndUseClassification', type:'string', useNull: true, defaultValue: null },
        { name:'SchBUSDA', type:'boolean', defaultValue: null },
        { name:'SchBNAICS', type:'string', useNull: true, defaultValue: null },
        { name:'SchBHiTechClassicification', type:'string', useNull: true, defaultValue: null },
        { name:'SchBImport', type:'boolean', defaultValue: null },
        { name:'SchBExport', type:'boolean', defaultValue: null },
        { name:'SchBRetired', type:'boolean', defaultValue: null },
        { name:'SchBModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'SchBModifiedDate', type:'date', useNull: true, defaultValue: null },
        { name:'SchBCreatedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
        { name:'SchBCreatedDate', type:'date', defaultValue: new Date() },
        { name: 'x_SchBNumFormatted', 
            type: 'string',
            convert: function(val,row) {
                val = row.data.SchBNum;

                var m = ('' + val).match(/^(\d{4})?[\.]?(\d{2})?[\.]?(\d{4})$/);

                if (/^(\d{4})?[\.]?(\d{2})?[\.]?(\d{4})$/.test('' + val) && m[1] && m[2]) {
                    return m[1] + '.' + m[2] + '.' + m[3];
                } else {
                    return val;
                }
            }   
        }
    ],

    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/ScheduleB',
        headers: {
           'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message'
        },
        writer: {
            type: 'json',
            writeAllFields: true
        },
        afterRequest: function (request, success) {
            if (request.action == 'read') {
                //this.readCallback(request);
            }
            else if (request.action == 'create') {
                if (!request.operation.success)
                {
                    Ext.popupMsg.msg("Warning", "Record was not created");
                    Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    Ext.popupMsg.msg("Success","Created Successfully");
                }
            }
            else if (request.action == 'update') {
                if (!request.operation.success)
                {
                    Ext.popupMsg.msg("Warning", "Record was not saved");
                    Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    Ext.popupMsg.msg("Success","Updated Successfully");
                }
            }
            else if (request.action == 'destroy') {
                if (!request.operation.success)
                {
                    Ext.popupMsg.msg("Warning", "Record was not deleted");
                    //Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    Ext.popupMsg.msg("Success","Deleted Successfully");
                }
            }
        }
    }
});