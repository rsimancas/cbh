Ext.define('CBH.view.jobs.JobPurchaseOrderStatusHistory', {
    extend: 'Ext.form.Panel',
    alias: 'widget.JobPurchaseOrderStatusHistory',
    modal: true,
    width: 400,
    layout: {
        type: 'absolute'
    },
    title: 'Purchase Order Status History',
    bodyPadding: 10,
    closable: true,
    floating: true,
    callerForm: "",
    currentRecord: null,
    EmployeeEmail: null,
    ForwarderEmail: null,
    CustEmail: null,

    initComponent: function() {

        var me = this;

        var storeStatus = Ext.create('CBH.store.common.Status').load({params:{category:0,page:0,start:0,limit:0}});

        Ext.applyIf(me, 
        {
            fieldDefaults: {
                labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items:[
            {
                xtype:'fieldcontainer',
                layout: {
                    type:'fit'
                },
                items:[
                {
                    xtype: 'datetimefield',
                    margin: '0 0 0 0',
                    fieldLabel: 'Date',
                    name: 'POStatusDate',
                    allowBlank: false,
                    readOnly: true,
                    editable: false,
                    listeners: {
                        blur: function () {
                            //me.onSaveChangesClick();
                        }
                    }
                },
                {
                    xtype: 'combo',
                    margin: '0 0 0 0',
                    fieldLabel: 'Status',
                    name: 'POStatusStatusKey',
                    valueField: 'StatusKey',
                    displayField: 'StatusText',
                    allowBlank: false,
                    queryMode: 'local',
                    store: storeStatus,
                    listeners: {
                        beforequery: function(record){  
                            record.query = new RegExp(record.query, 'i');
                            record.forceAll = true;
                        }
                    }
                },
                {
                    xtype: 'textareafield',
                    height: 150,
                    margin: '0 0 20 0',
                    fieldLabel: 'Notes',
                    labelWidth: 50,
                    name: 'POStatusMemo',
                    allowBlank: true
                },
                {
                    xtype: 'checkboxfield',
                    margin: '5 0 5 0',
                    boxLabel: 'Public (Share with Customer)',
                    name: 'POStatusPublic',
                    inputValue: '0'
                }, 
                {
                    xtype: 'fieldcontainer',
                    margin: '5 0 15 0',
                    layout: 'fit',
                    defaultType: 'checkboxfield',
                    items: [{
                        boxLabel: (me.EmployeeEmail) ? me.EmployeeEmail.toLowerCase() : "",
                        name: 'EmployeeEmail',
                        checked: true,
                    }, {
                        boxLabel: (me.ForwarderEmail) ? me.ForwarderEmail.toLowerCase().replaceAll(';','<br/>') : "",
                        name: 'ForwarderEmail',
                    }, {
                        boxLabel: me.CustEmail ? me.CustEmail.toLowerCase().replaceAll(';','<br/>') : "",
                        name: 'CustEmail'
                    }]
                }          
                ]
            }
            ],
            dockedItems: [
            {
                xtype: 'toolbar',
                dock: 'bottom',
                ui: 'footer',
                items: [
                {
                    xtype: 'component',
                    flex: 1
                },
                {
                    xtype: 'button',
                    text: 'Save Changes',
                    formBind: true,
                    listeners: {
                        click: {
                            fn: me.onSaveChanges,
                            scope: this
                        }
                    }
                }
                ]
            }
            ],
            listeners:{
                show: {
                    fn: me.onShowWindow,
                    scope: me
                }
            }
        });

        me.callParent(arguments);
    },

    onShowWindow: function() {
        if(!this.getForm().getRecord().phantom) {
            this.down('field[name=POStatusDate]').readOnly = true;
            this.down('field[name=POStatusStatusKey]').readOnly = true;    
            this.down('field[name=POStatusMemo]').focus(true, 200);    
        } else {
            this.down('field[name=POStatusStatusKey]').focus(true, 200);    
        }
    },

    onSaveChanges: function(button, e, eOpts) {
        var me = this,
            form = me.getForm();

        if(!form.isValid())  { 
            Ext.Msg.alert("Validation","Check data for valid input!!!");
            return false;
        }

        form.updateRecord();

        record = form.getRecord();
       
        Ext.Msg.wait('Saving Record...', 'Wait');

        record.save({ 
            success: function(e) {
                Ext.Msg.hide();
                var form = me.callerForm;
                
                grid = form.down('#gridstatus');
                lastOpt = grid.store.lastOptions;
                grid.store.reload({params: lastOpt.params});
                me.sendMail();
                me.destroy();
            },
            failure: function() {
                Ext.Msg.hide();
            }
        });
    },

    sendMail: function() {
        var me = this,
            employeeEmail = me.down('field[name=EmployeeEmail]').checked,
            custEmail = me.down('field[name=CustEmail]').checked,
            fwdEmail = me.down('field[name=ForwarderEmail]').checked;

        if (!employeeEmail && !custEmail && !fwdEmail) return;

        var curPO = me.currentRecord,
            strEmailBody = "",
            url = "",
            recipient = me.down('field[name=EmployeeEmail]').boxLabel,
            cc = me.down('field[name=CustEmail]').boxLabel,
            subject = "CBH Update (PO " + me.PONum + ") " + me.down('field[name=POStatusStatusKey]').getRawValue();

        if(curPO.data.JobCustRefNum && custEmail) {
                subject = "CBH Update (" + curPO.data.JobCustRefNum + ")";
        } if (curPO.data.JobCustRefNum && custEmail && curPO.data.QuoteNum) {
            subject = "CBH Update (Quote " + curPO.data.QuoteNum + ")";
        } else {
            subject = "CBH Update (Job " + me.JobNum + ")";
        }

        /*strEmailBody = "Customer: " + curPO.CustName + "\n" +
            "Contact: " + curPO.ContactFirstName + " " + curPO.ContactLastName + "\n" +
            "Reference: " + curPO.FileReference + "\n" +
            me.down('field[name=POStatusMemo]').getValue();*/

        strEmailBody = "CBH Job Number: " + me.JobNum + "\n";
            if (!custEmail) strEmailBody = strEmailBody + "PO Number: " + me.callerForm.PONum + "\n";
            if (curPO.data.QuoteNum) strEmailBody = strEmailBody + "CBH Quote Number: " + curPO.data.QuoteNum + "\n";
            if (curPO.data.JobProdDescription) strEmailBody = strEmailBody + "Product Description: " + curPO.data.JobProdDescription + "\n";
            if (custEmail) {
                strEmailBody = strEmailBody + curPO.data.VendorDisplayToCust + "\n";
            } else {
                strEmailBody = strEmailBody + curPO.data.VendorName + "\n";
            }
            strEmailBody = strEmailBody + "\n";
            strEmailBody = strEmailBody + me.down("field[name=POStatusStatusKey]").getValue();
            if (me.down("field[name=POStatusMemo]").getValue())  strEmailBody = strEmailBody + " - " + me.down("field[name=POStatusMemo]").getValue();


        if (!employeeEmail && custEmail) {
            recipient = cc;
            cc = "";
        }

        if (employeeEmail && !custEmail) {
            cc = "";
        }

        url = "mailto:" + recipient + "?subject=" + encodeURIComponent(subject);

        if (!String.isNullOrEmpty(cc)) url += "&cc=" + encodeURIComponent(cc);

        url = url + "&body=" + encodeURIComponent(strEmailBody);

        window.open(url, 'Mail', false);
    }
});