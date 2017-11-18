Ext.define('CBH.view.customers.CustomerShipAddress', {
    extend: 'Ext.form.Panel',
    alias: 'widget.customershipaddress',
    modal: true,
    width: 500,
    layout: {
        type: 'absolute'
    },
    title: 'Customer Ship Address',
    bodyPadding: 10,
    closable: true,
    stateful: false,
    floating: true,
    callerForm: "",
    forceFit: true,

    initComponent: function() {

        var me = this;

        storeShipStates = new CBH.store.common.States().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        storeCountries = new CBH.store.common.Countries().load({
            params: {
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
                margin: '0 0 20 0',
                layout: {
                    type: 'column'
                },
                items: [{
                    xtype: 'checkbox',
                    columnWidth: 1,
                    name: 'ShipDefault',
                    labelSeparator: '',
                    hideLabel: true,
                    boxLabel: 'Default Ship Address',
                    //fieldLabel: 'Print note below item',
                }, {
                    xtype: 'textfield',
                    columnWidth: 1,
                    margin: '0 0 0 0',
                    fieldLabel: 'Name',
                    name: 'ShipName',
                    allowBlank: false,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'textfield',
                    columnWidth: 1,
                    margin: '0 0 0 0',
                    fieldLabel: 'Address 1',
                    name: 'ShipAddress1',
                    allowBlank: false,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'textfield',
                    columnWidth: 1,
                    margin: '0 0 0 0',
                    fieldLabel: 'Address 2',
                    name: 'ShipAddress2',
                    //allowBlank: false,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'textfield',
                    margin: '0 0 0 0',
                    columnWidth: 0.3,
                    fieldLabel: 'City',
                    name: 'ShipCity'
                }, {
                    xtype: 'combo',
                    margin: '0 0 0 10',
                    columnWidth: 0.4,
                    fieldLabel: 'State',
                    name: 'ShipState',
                    displayField: 'StateName',
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 2,
                    forceSelection: true,
                    store: storeShipStates,
                    valueField: 'StateCode',
                    emptyText: 'Choose State',
                    anyMatch: true,
                }, {
                    xtype: 'textfield',
                    margin: '0 0 0 10',
                    columnWidth: 0.3,
                    fieldLabel: 'Zip',
                    name: 'ShipZip'
                }, {
                    xtype: 'combo',
                    columnWidth: 1,
                    margin: '0 0 0 0',
                    fieldLabel: 'Country',
                    name: 'ShipCountryKey',
                    //vtype: 'alphanum',
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 2,
                    forceSelection: true,
                    emptyText: 'Choose Country',
                    displayField: 'CountryName',
                    store: storeCountries,
                    valueField: 'CountryKey',
                    anyMatch: true,
                }, {
                    xtype: 'textfield',
                    margin: '0 0 0 0',
                    columnWidth: 1,
                    fieldLabel: 'Comments',
                    name: 'ShipComments',
                }],
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
        this.down("field[name=ShipName]").focus(true, 200);
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
                    var form = me.callerForm;
                    me.destroy();
                    var grid = form.down('#gridshipaddress');
                    customer = records.data.ShipCustKey;

                    storeAddress = new CBH.store.customers.CustomerShipAddress().load({
                        params: {
                            page: 0,
                            start: 0,
                            limit: 0,
                            custkey: customer
                        },
                        callback: function() {
                            if (form.title === "Customer Maintenance") {
                                grid.reconfigure(storeAddress);
                            } else {
                                var field = form.down('field[name=FileCustShipKey]').bindStore(storeAddress);
                                field.setValue(records.data.ShipKey);
                                field.focus(true, 400);
                            }
                            storeAddress.lastOptions.callback = null;
                            Ext.Msg.hide();
                        }
                    });
                } else {
                    Ext.Msg.hide();
                }
            }
        });
    }
});
