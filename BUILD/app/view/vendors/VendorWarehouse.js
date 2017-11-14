Ext.define('CBH.view.vendors.VendorWarehouse', {
    extend: 'Ext.form.Panel',
    modal: true,
    width: 500,
    layout: {
        type: 'absolute'
    },
    title: 'Carrier Warehouse Address',
    bodyPadding: 10,
    closable: true,
    stateful: false,
    floating: true,
    callerForm: "",
    forceFit: true,

    initComponent: function() {

        var me = this;

        var storeStates = new CBH.store.common.States().load({
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
                    xtype: 'textfield',
                    columnWidth: 1,
                    fieldLabel: 'Name',
                    name: 'WarehouseName',
                    allowBlank: false,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'textfield',
                    columnWidth: 1,
                    fieldLabel: 'Address',
                    name: 'WarehouseAddress1',
                    allowBlank: false,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'textfield',
                    columnWidth: 1,
                    name: 'WarehouseAddress2',
                    //allowBlank: false,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'textfield',
                    columnWidth: 0.3,
                    fieldLabel: 'City',
                    name: 'WarehouseCity'
                }, {
                    xtype: 'combo',
                    margin: '0 0 0 5',
                    columnWidth: 0.4,
                    fieldLabel: 'State',
                    name: 'WarehouseState',
                    displayField: 'StateName',
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 2,
                    forceSelection: true,
                    store: storeStates,
                    valueField: 'StateCode',
                    emptyText: 'Choose State',
                    listeners: {
                        beforequery: function(record) {
                            record.query = new RegExp(record.query, 'i');
                            record.forceAll = true;
                        }
                    }
                }, {
                    xtype: 'textfield',
                    margin: '0 0 0 5',
                    columnWidth: 0.3,
                    fieldLabel: 'Zip',
                    name: 'WarehouseZip',
                }, {
                    xtype: 'combo',
                    columnWidth: 1,
                    margin: '0 0 0 0',
                    fieldLabel: 'Country',
                    name: 'WarehouseCountryKey',
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 2,
                    forceSelection: true,
                    emptyText: 'Choose Country',
                    displayField: 'CountryName',
                    store: storeCountries,
                    valueField: 'CountryKey',
                    listeners: {
                        beforequery: function(record) {
                            record.query = new RegExp(record.query, 'i');
                            record.forceAll = true;
                        }
                    }
                }, {
                    xtype: 'textfield',
                    margin: '0 0 0 0',
                    columnWidth: 1,
                    fieldLabel: 'Phone',
                    name: 'WarehousePhone',
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
        this.down("field[name=WarehouseName]").focus(true, 200);
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
                var form = me.callerForm;
                me.destroy();
                var grid = form.down('gridwarehouse');
                customer = form.getRecord();

                var storeWarehouse = new CBH.store.vendors.VendorWarehouse().load({
                    params: {
                        page: 0,
                        start: 0,
                        limit: 0,
                        custkey: customer.data.CustKey
                    },
                    callback: function() {
                        grid.reconfigure(storeWarehouse);
                        Ext.Msg.hide();
                    },
                    synchronous: true
                });
            },
            failure: function() {
                Ext.Msg.hide();
            }
        });
    }
});
