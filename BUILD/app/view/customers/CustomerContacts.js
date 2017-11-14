Ext.define('CBH.view.customers.CustomerContacts', {
    extend: 'Ext.form.Panel',
    modal: true,
    width: 334,
    layout: {
        type: 'absolute'
    },
    title: 'Customer Contact',
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
                    margin: '0 0 0 10',
                    fieldLabel: 'First Name',
                    labelAlign: 'top',
                    labelWidth: 50,
                    name: 'ContactFirstName',
                    allowBlank: false,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'textfield',
                    //columnWidth: 0.7,
                    margin: '0 0 0 10',
                    fieldLabel: 'Last Name',
                    labelAlign: 'top',
                    labelWidth: 50,
                    name: 'ContactLastName',
                    allowBlank: false,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'textfield',
                    //columnWidth: 0.7,
                    margin: '0 0 0 10',
                    fieldLabel: 'Phone',
                    labelAlign: 'top',
                    labelWidth: 50,
                    name: 'ContactPhone',
                    //allowBlank: false,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'textfield',
                    //columnWidth: 0.7,
                    margin: '0 0 0 10',
                    fieldLabel: 'Email',
                    labelAlign: 'top',
                    labelWidth: 50,
                    name: 'ContactEmail',
                    //vtype: 'email',
                    //allowBlank: false,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'textfield',
                    //columnWidth: 0.7,
                    margin: '0 0 0 10',
                    fieldLabel: 'Fax',
                    labelAlign: 'top',
                    labelWidth: 50,
                    name: 'ContactFax',
                    allowBlank: true,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'textareafield',
                    //columnWidth: 0.7,
                    margin: '0 0 0 10',
                    fieldLabel: 'Notes',
                    labelAlign: 'top',
                    labelWidth: 50,
                    name: 'ContactMemo',
                    allowBlank: true,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'checkbox',
                    name: 'ContactAllowedWebAccess',
                    margin: '0 0 10 10',
                    itemId: 'contactallowedwebaccess',
                    labelSeparator: '',
                    hideLabel: true,
                    boxLabel: 'Contact is Allowed Web Extranet Access',
                    fieldLabel: 'Contact is Allowed Web Extranet Access',
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
            Ext.Msg.alert("Validation", "Check data for valid input!!!");
            return false;
        }

        form.updateRecord();

        record = form.getRecord();

        Ext.Msg.wait('Saving Record...', 'Wait');

        record.save({
            callback: function(records, operation, success) {
                if (success) {
                    var form = me.callerForm,
                        grid = form.down('#gridcontacts'),
                        customer = records.data.ContactCustKey;

                    me.destroy();

                    if (grid) {
                        grid.store.reload();
                    } else {
                        var filterCust = new Ext.util.Filter({
                            property: 'ContactCustKey',
                            value: customer
                        });

                        storeContacts = new CBH.store.customers.CustomerContacts().load({
                            filters: [filterCust],
                            callback: function() {
                                var field = form.down('field[name=FileContactKey]').bindStore(storeContacts);
                                field.setValue(records.data.ContactKey);
                                field.focus(true, 400);
                            }
                        });
                    }
                    Ext.Msg.hide();
                } else {
                    Ext.Msg.hide();
                }
            }
        });
    }
});
