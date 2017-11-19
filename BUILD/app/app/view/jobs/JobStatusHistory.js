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
                    xtype:'hidden',
                    name: 'JobKey'
                }, {
                    xtype: 'datetimefield',
                    margin: '0 0 0 0',
                    fieldLabel: 'Date',
                    name: 'StatusDate',
                    allowBlank: false,
                    editable: false,
                    readOnly: true
                }, {
                    xtype: 'combo',
                    margin: '0 0 0 0',
                    fieldLabel: 'Status',
                    name: 'StatusStatusKey',
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
                    enforceMaxLength: true,
                    maxLength: 2000,
                    maxLengthText: "The text cannot exceed 2.000 characters",
                    name: 'StatusMemo',
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

        me.down('field[name=StatusStatusKey]').focus(true, 200);
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

        record.set("JobModifiedBy", CBH.GlobalSettings.getCurrentUserName());

        var statusKey = me.down("field[name=StatusStatusKey]").getValue(),
            store = me.down("field[name=StatusStatusKey]").getStore();

        var statusIndex = store.find('StatusKey', statusKey),
            status = store.getAt(statusIndex);

        //*** Update closed/completed
        if (status.get("StatusClosed") && record.get("JobClosed") === null) 
            record.set("JobClosed", new Date());

        if (status.get("StatusCompleted") && record.get("JobComplete") === null) 
            record.set("JobComplete", new Date());
        
        if (status.get("StatusStatusKey") > 0)
            record.set("JobStatusKey", statusKey);
        
        if (status.get("StatusStatusKey") === 71) {
            record.set("JobStatusKey", 0);
            record.set("JobClosed", null);
        }

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
                CBH.AppEvents.fireEvent("jobclosed");
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
            me.down('field[name=StatusStatusKey]').getRawValue() + "\n" +
            me.down('field[name=StatusMemo]').getValue();


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
