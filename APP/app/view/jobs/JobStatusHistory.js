Ext.define('CBH.view.jobs.JobStatusHistory', {
    extend: 'Ext.form.Panel',
    alias: 'widget.jobstatushistory',
    modal: true,
    width: 600,
    layout: 'column',
    title: 'Job Status History',
    bodyPadding: 10,
    closable: true,
    floating: true,
    callerForm: "",

    
    initComponent: function() {

        var me = this;

        var storeStatus = new CBH.store.common.Status().load({
            params: {
                category: 1,
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
                    name: 'JobStatusDate',
                    allowBlank: false,
                    editable: false,
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
                    name: 'JobStatusStatusKey',
                    valueField: 'StatusKey',
                    displayField: 'StatusText',
                    allowBlank: false,
                    queryMode: 'local',
                    store: storeStatus,
                    anyMatch: true
                }, {
                    xtype: 'textareafield',
                    height: 150,
                    margin: '0 0 20 0',
                    fieldLabel: 'Notes',
                    labelWidth: 50,
                    name: 'JobStatusMemo',
                    allowBlank: false
                }]
            }, {
                xtype: 'fieldcontainer',
                columnWidth: 1,
                layout: 'column',
                defaultType: 'checkboxfield',
                items: [{
                    boxLabel: 'EmailCBH',
                    columnWidth: 1,
                    name: 'EmailCBH',
                    checked: true
                }, {
                    boxLabel: 'EmailForwarder',
                    columnWidth: 1,
                    name: 'EmailForwarder'
                }, {
                    boxLabel: 'EmailCustomer',
                    columnWidth: 1,
                    name: 'EmailCustomer'
                }, {
                    boxLabel: 'Public (ShareWithCustomer)',
                    columnWidth: 1,
                    name: 'StatusPublic'
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
                    itemId: 'acceptbutton',
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
        var storeJobHistory = new CBH.store.jobs.qfrmJobStatusHistory().load({
            params: {
                id: me.JobKey
            },
            callback: function(records, operation, success) {
                var mailEmployee = records[0].data.EmployeeEmail,
                    mailForwarder = records[0].data.ForwarderEmail,
                    mailCustomer = records[0].data.CustEmail;

                mailForwarder = (mailForwarder) ? mailForwarder.toLowerCase().replaceAll(';','<br/>') : null;
                mailCustomer = (mailCustomer) ? mailCustomer.toLowerCase().replaceAll(';','<br/>') : null;

                me.down('field[name=EmailCBH]').setBoxLabel(mailEmployee);
                me.down('field[name=EmailForwarder]').setBoxLabel(mailForwarder);
                me.down('field[name=EmailCustomer]').setBoxLabel(mailCustomer);
                me.currentJob = records[0].data;
            }
        });

        me.down('field[name=JobStatusStatusKey]').focus(true, 200);
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
            job = me.currentJob,
            strMessage = "",
            url = "",
            recipient = "",
            cbh = me.down('field[name=EmailCBH]').boxLabel,
            cust = me.down('field[name=EmailCustomer]').boxLabel,
            fwd = me.down('field[name=EmailForwarder]').boxLabel,
            subject = "CBH Update (Job " + me.JobNum + ")",
            emailCBH = me.down('field[name=EmailCBH]').checked,
            emailCust = me.down('field[name=EmailCustomer]').checked,
            emailForwarder = me.down('field[name=EmailForwarder]').checked;

        if (!emailCBH && !emailCust && !emailForwarder) return;

        var contactTitle = (job.ContactTitle === null) ? "" : job.ContactTitle;

        strMessage = "Customer: " + job.CustName + "\n" +
            "Contact: " + contactTitle + " " + job.ContactFirstName + " " + job.ContactLastName + "\n" +
            "CBH Job Number: " + me.JobNum + "\n" +
            ((!String.isNullOrEmpty(job.QuoteNum)) ? "CBH Quote Number: " + job.QuoteNum + "\n": "") +
            ((!String.isNullOrEmpty(job.JobProdDescription)) ? "Product Description: " + job.JobProdDescription + "\n": "") +
            me.down('field[name=JobStatusStatusKey]').getRawValue() + "\n" +
            me.down('field[name=JobStatusMemo]').getValue();


        if(!String.isNullOrEmpty(job.JobCustRefNum) && emailCust) {
            subject = "CBH Update (" + job.JobCustRefNum + ")";
        } else if(String.isNullOrEmpty(job.JobCustRefNum) && emailCust && !String.isNullOrEmpty(job.QuoteNum)) {
            subject = "CBH Update (Quote " + job.QuoteNum + ")";
        }

        recipient += (emailCBH) ? ";" + encodeURIComponent(cbh) : "";
        recipient += (emailCust) ? ";" + encodeURIComponent(cust) : "";
        recipient += (emailForwarder) ? ";" + encodeURIComponent(fwd) : "";

        recipient = recipient.substr(1);

        url = "mailto:" + recipient + "?subject=" + encodeURIComponent(subject);

        url = url + "&body=" + encodeURIComponent(strMessage);

        window.open(url, 'Mail', false);
    }
});
