Ext.define('CBH.view.vendors.VendorOriginAddress', {
    extend: 'Ext.form.Panel',
    widget: 'widget.vendororiginaddress',
    modal: true,
    width: 500,
    layout: {
        type: 'absolute'
    },
    title: 'Vendors Origin Address',
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
                    xtype: 'checkbox',
                    columnWidth: 1,
                    name: 'OriginDefault',
                    labelSeparator: '',
                    hideLabel: true,
                    boxLabel: 'Default Address',
                }, {
                    xtype: 'textfield',
                    columnWidth: 1,
                    margin: '0 0 0 0',
                    fieldLabel: 'Name',
                    name: 'OriginName',
                    allowBlank: false
                }, {
                    xtype: 'textfield',
                    columnWidth: 1,
                    margin: '0 0 0 0',
                    fieldLabel: 'Address',
                    name: 'OriginAddress1',
                    allowBlank: false
                }, {
                    xtype: 'textfield',
                    columnWidth: 1,
                    margin: '0 0 0 0',
                    name: 'OriginAddress2'
                }, {
                    xtype: 'textfield',
                    margin: '0 0 0 0',
                    columnWidth: 0.3,
                    fieldLabel: 'City',
                    name: 'OriginCity'
                }, {
                    xtype: 'combo',
                    margin: '0 0 0 10',
                    columnWidth: 0.4,
                    fieldLabel: 'State',
                    name: 'OriginState',
                    displayField: 'StateName',
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 2,
                    forceSelection: true,
                    store: storeStates,
                    valueField: 'StateCode',
                    emptyText: 'Choose State',
                    anyMatch: true
                }, {
                    xtype: 'textfield',
                    margin: '0 0 0 10',
                    columnWidth: 0.3,
                    fieldLabel: 'Zip',
                    name: 'OriginZip'
                }, {
                    xtype: 'combo',
                    columnWidth: 1,
                    margin: '0 0 0 0',
                    fieldLabel: 'Country',
                    name: 'OriginCountryKey',
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 2,
                    forceSelection: true,
                    emptyText: 'Choose Country',
                    displayField: 'CountryName',
                    store: storeCountries,
                    valueField: 'CountryKey',
                    anyMatch: true
                }, {
                    xtype: 'textfield',
                    margin: '0 0 0 0',
                    columnWidth: 1,
                    fieldLabel: 'Comments',
                    name: 'OriginComments',
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
        this.down("field[name=OriginName]").focus(true, 200);
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

        record.data.OriginModifiedBy = CBH.GlobalSettings.getCurrentUserName();

        record.save({
            success: function(e) {
                var form = me.callerForm;
                me.destroy();
                var grid = form.down('#gridoriginaddress');
                vendor = form.getRecord();

                var storeContacts = new CBH.store.vendors.VendorOriginAddress().load({
                    params: {
                        page: 0,
                        start: 0,
                        limit: 0,
                        vendorkey: vendor.data.VendorKey
                    },
                    callback: function() {
                        grid.reconfigure(storeContacts);
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
