Ext.define('CBH.view.vendors.VendorContacts', {
    extend: 'Ext.form.Panel',
    widget: 'widget.vendorcontacts',
    modal: true,
    width: 334,
    layout: {
        type: 'absolute'
    },
    title: 'Vendor Contact',
    bodyPadding: 10,
    closable: true,
    floating: true,
    callerForm: "",

    initComponent: function() {

        var me = this;

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
                layout: {
                    type: 'fit'
                },
                items: [{
                    xtype: 'textfield',
                    fieldLabel: 'First Name',
                    name: 'ContactFirstName',
                    allowBlank: false
                }, {
                    xtype: 'textfield',
                    fieldLabel: 'Last Name',
                    name: 'ContactLastName',
                    allowBlank: false
                }, {
                    xtype: 'textfield',
                    fieldLabel: 'Phone',
                    name: 'ContactPhone'
                }, {
                    xtype: 'textfield',
                    margin: '0 0 0 0',
                    fieldLabel: 'Email',
                    name: 'ContactEmail'
                }, {
                    xtype: 'textfield',
                    fieldLabel: 'Fax',
                    name: 'ContactFax',
                    allowBlank: true
                }, {
                    xtype: 'textareafield',
                    fieldLabel: 'Notes',
                    name: 'ContactMemo',
                    allowBlank: true
                }, {
                    margin: '0 0 10 0',
                    xtype: 'checkbox',
                    name: 'ContactAllowedWebAccess',
                    labelSeparator: '',
                    hideLabel: true,
                    boxLabel: 'Contact is Allowed Web Extranet Access',
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
                    value: me.valueProfit,
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
        this.down('field[name=ContactFirstName]').focus(true, 200);
    },

    onSaveChanges: function(button, e, eOpts) {
        var me = this,
            form = me.getForm();

        if (!form.isValid()) {
            Ext.Msg.alert("Validation", "Check data for valid input!!!")
            return false;
        };

        form.updateRecord();

        record = form.getRecord();

        Ext.Msg.wait('Saving Record...', 'Wait');

        record.save({
            success: function(rec, eOpts) {
                Ext.Msg.hide();
                var form = me.callerForm,
                    grid = form.down('#gridcontacts'),
                    fieldContact = form.down('field[name=POVendorContactKey]');

                if (grid) {
                    grid.store.reload();
                }

                if (fieldContact) {
                    var contacts = new CBH.store.vendors.VendorContacts().load({
                        params: {
                            page: 0,
                            start: 0,
                            limit: 0,
                            vendorkey: rec.get("ContactVendorKey")
                        },
                        callback: function() {
                            fieldContact.getStore().removeAll();
                            fieldContact.bindStore(contacts);

                            if (this.totalCount > 0) {
                                fieldContact.setValue(record.get("ContactKey"));
                                fieldContact.focus(true, 200);
                            }
                        }
                    });
                }

                me.destroy();
            },
            failure: function() {
                Ext.Msg.hide();
            }
        });
    }
});