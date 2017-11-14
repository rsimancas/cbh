Ext.define('CBH.view.sales.FileStatusHistory', {
    extend: 'Ext.form.Panel',
    alias: 'widget.filestatushistory',
    modal: true,
    width: 400,
    layout: 'column',
    title: 'Status History',
    bodyPadding: 10,
    closable: true,
    floating: true,
    callerForm: "",
    FileKey: 0,
    currentFile: null,

    initComponent: function() {

        var me = this;

        var storeStatus = new CBH.store.common.Status().load({
            params: {
                category: 2,
                page: 0,
                start: 0,
                limit: 0
            }
        });

        Ext.applyIf(me, {
            fieldDefaults: {
                labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items: [{
                xtype: 'fieldcontainer',
                columnWidth: 1,
                layout: 'fit',
                items: [{
                    xtype: 'datetimefield',
                    margin: '0 0 0 0',
                    fieldLabel: 'Date',
                    name: 'FileStatusDate',
                    allowBlank: false,
                    readOnly: true,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'combo',
                    margin: '0 0 0 0',
                    fieldLabel: 'Status',
                    name: 'FileStatusStatusKey',
                    valueField: 'StatusKey',
                    displayField: 'StatusText',
                    allowBlank: false,
                    queryMode: 'local',
                    store: storeStatus,
                    listeners: {
                        beforequery: function(record) {
                            record.query = new RegExp(record.query, 'i');
                            record.forceAll = true;
                        }
                    }
                }, {
                    xtype: 'textareafield',
                    height: 150,
                    margin: '0 0 20 0',
                    fieldLabel: 'Notes',
                    labelWidth: 50,
                    name: 'FileStatusMemo',
                    allowBlank: false
                }]
            }, {
                xtype: 'fieldcontainer',
                columnWidth: 1,
                layout: 'fit',
                defaultType: 'checkboxfield',
                items: [{
                    boxLabel: 'EmailCBH',
                    name: 'EmailCBH',
                    checked: true
                }, {
                    boxLabel: '',
                    name: 'EmailForwarder'
                }, {
                    boxLabel: 'EmailCustomer',
                    name: 'EmailCustomer'
                }]
            }],
            dockedItems: [{
                xtype: 'toolbar',
                dock: 'bottom',
                ui: 'footer',
                items: [{
                    xtype: 'component',
                    flex: 1
                }, {
                    xtype: 'button',
                    id: 'acceptbutton',
                    text: 'Save Changes',
                    formBind: true,
                    listeners: {
                        click: {
                            fn: me.onSaveChanges,
                            scope: this
                        }
                    }
                }]
            }],
            listeners: {
                show: {
                    fn: me.onShowWindow,
                    scope: me
                }
            }
        });

        me.callParent(arguments);
    },

    onShowWindow: function() {
        var me = this;
        var storeFileHistory = new CBH.store.sales.qfrmFileStatusHistory().load({
            params: {
                id: me.FileKey
            },
            callback: function(records, operation, success) {
                me.down('field[name=EmailCBH]').setBoxLabel(records[0].data.EmployeeEmail);
                me.down('field[name=EmailCustomer]').setBoxLabel(records[0].data.CustEmail);
                me.currentFile = records[0].data;
            }
        });

        me.down('field[name=FileStatusStatusKey]').focus(true, 200);

    },

    onSaveChanges: function(button, e, eOpts) {
        var me = this,
            form = me.getForm();

        if (!form.isValid()) {
            Ext.Msg.alert("Validation", "Check data for valid input!!!");
            return false;
        }

        form.updateRecord();

        record = form.getRecord();

        Ext.Msg.wait('Saving Record...', 'Wait');

        record.save({
            success: function(e) {
                Ext.Msg.hide();
                var form = me.callerForm;
                //me.destroy();
                grid = form.down('#gridstatus');
                lastOpt = grid.store.lastOptions;
                grid.store.reload({
                    params: lastOpt.params
                });
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
            file = me.currentFile,
            strMessage = "",
            url = "",
            recipient = me.down('field[name=EmailCBH]').boxLabel,
            cc = me.down('field[name=EmailCustomer]').boxLabel,
            subject = "CBH Update (File " + me.FileNum + ") " + me.down('field[name=FileStatusStatusKey]').getRawValue(),
            emailCBH = me.down('field[name=EmailCBH]').checked,
            emailCust = me.down('field[name=EmailCustomer]').checked,
            emailForwarder = me.down('field[name=EmailForwarder]').checked;

        if (!emailCBH && !emailCust && !emailForwarder) return;

        var contactTitle = (file.ContactTitle === null) ? "" : file.ContactTitle;

        strMessage = "Customer: " + file.CustName + "\n" +
            "Contact: " + contactTitle + " " + file.ContactFirstName + " " + file.ContactLastName + "\n" +
            "Reference: " + file.FileReference + "\n" +
            me.down('field[name=FileStatusMemo]').getValue();


        if (!emailCBH && emailCust) {
            recipient = cc;
            cc = "";
        }

        if (emailCBH && !emailCust) {
            cc = "";
        }

        url = "mailto:" + recipient + "?subject=" + encodeURIComponent(subject);

        if (!String.isNullOrEmpty(cc)) url += "&cc=" + encodeURIComponent(cc);

        url = url + "&body=" + encodeURIComponent(strMessage);

        window.open(url, 'Mail', false);
    }
});
