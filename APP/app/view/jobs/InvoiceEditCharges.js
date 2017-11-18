Ext.define('CBH.view.jobs.InvoiceEditCharges', {
    extend: 'Ext.form.Panel',
    alias: 'widget.InvoiceEditCharges',
    //height: 185,
    modal: true,
    width: 400,
    layout: {
        type: 'absolute'
    },
    title: 'Charges',
    bodyPadding: 10,
    closable: true,
    //constrain: true,
    floating: true,
    callerForm: "",

    initComponent: function() {

        var me = this;

        Ext.Msg.wait('Wait', 'Loading...');
        var storeCategories = new CBH.store.jobs.tlkpChargeCategories({
            remoteSort: false
        }).load({
            callback: function() {
                storeCategories.sort("x_DescriptionText", "ASC");
                Ext.Msg.hide();
            }
        });

        var storeCurrencyRates = new CBH.store.common.CurrencyRates().load({
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
                layout: {
                    type: 'column'
                },
                items: [{
                    xtype: 'numericfield',
                    columnWidth: 0.5,
                    name: 'IChargeSort',
                    fieldLabel: 'Sort',
                    fieldStyle: 'text-align: right;',
                    minValue: 100,
                    hideTrigger: false,
                    useThousandSeparator: true,
                    decimalPrecision: 0,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: false,
                    thousandSeparator: ',',
                    step: 100
                }, {
                    margin: '0 0 0 5',
                    xtype: 'numericfield',
                    columnWidth: 0.5,
                    name: 'IChargeQty',
                    fieldLabel: 'Qty',
                    fieldStyle: 'text-align: right;',
                    minValue: 1,
                    hideTrigger: false,
                    useThousandSeparator: true,
                    decimalPrecision: 2,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: false,
                    thousandSeparator: ','
                }, {
                    xtype: 'combo',
                    columnWidth: 1,
                    fieldLabel: 'Description',
                    name: 'IChargeChargeKey',
                    displayField: 'x_DescriptionText',
                    valueField: 'ChargeKey',
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 2,
                    forceSelection: true,
                    store: storeCategories,
                    emptyText: 'choose category',
                    anyMatch: true
                }, {
                    xtype: 'textareafield',
                    columnWidth: 1,
                    fieldLabel: 'Notes & Comments',
                    labelAlign: 'top',
                    labelWidth: 50,
                    name: 'IChargeMemo',
                    allowBlank: true,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    xtype: 'numericfield',
                    columnWidth: 0.5,
                    name: 'x_UnitCost',
                    hideTrigger: true,
                    useThousandSeparator: true,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    // currencySymbol:'$',
                    alwaysDecimals: true,
                    thousandSeparator: ',',
                    fieldLabel: 'Cost Unit',
                    labelAlign: 'top',
                    fieldStyle: 'text-align: right;',
                    allowBlank: false,
                    listeners: {
                        change: function(field, newValue, oldValue, eOpts) {
                            if (document.activeElement.name !== field.name) return;

                            var val = newValue ? newValue : 0;

                            var me = field.up('panel');
                            
                            var qtyField = me.down("field[name=IChargeQty]"),
                                lineCostField = me.down("field[name=IChargeCost]");

                            lineCostField.setValue(qtyField.getValue() * val);

                        }
                    }
                }, {
                    margin: '0 0 0 5',
                    xtype: 'numericfield',
                    columnWidth: 0.5,
                    name: 'x_UnitPrice',
                    hideTrigger: true,
                    useThousandSeparator: true,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    //currencySymbol:'$',
                    alwaysDecimals: true,
                    thousandSeparator: ',',
                    fieldLabel: 'Price Unit',
                    labelAlign: 'top',
                    fieldStyle: 'text-align: right;',
                    allowBlank: false,
                    listeners: {
                        change: function(field, newValue, oldValue, eOpts) {
                            if (document.activeElement.name !== field.name) return;

                            var val = newValue ? newValue : 0;

                            var me = field.up('panel');
                            
                            var qtyField = me.down("field[name=IChargeQty]"),
                                linePriceField = me.down("field[name=IChargePrice]");

                            linePriceField.setValue(qtyField.getValue() * val);

                        }
                    }
                }, {
                    xtype: 'numericfield',
                    columnWidth: 0.5,
                    name: 'IChargeCost',
                    hideTrigger: true,
                    useThousandSeparator: true,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    // currencySymbol:'$',
                    alwaysDecimals: true,
                    thousandSeparator: ',',
                    fieldLabel: 'Total Cost',
                    labelAlign: 'top',
                    fieldStyle: 'text-align: right;',
                    allowBlank: false,
                    listeners: {
                        change: function(field, newValue, oldValue, eOpts) {
                            if (document.activeElement.name !== field.name) return;

                            var val = newValue ? newValue : 0;

                            var me = field.up('panel');
                            
                            var qtyField = me.down("field[name=IChargeQty]"),
                                costField = me.down("field[name=x_UnitCost]");

                            if(qtyField.getValue() > 0) 
                                costField.setValue(val / qtyField.getValue());

                        }
                    }
                }, {
                    margin: '0 0 0 5',
                    xtype: 'numericfield',
                    columnWidth: 0.5,
                    name: 'IChargePrice',
                    hideTrigger: true,
                    useThousandSeparator: true,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    //currencySymbol:'$',
                    alwaysDecimals: true,
                    thousandSeparator: ',',
                    fieldLabel: 'Total Price',
                    labelAlign: 'top',
                    fieldStyle: 'text-align: right;',
                    allowBlank: false,
                    listeners: {
                        change: function(field, newValue, oldValue, eOpts) {
                            if (document.activeElement.name !== field.name) return;

                            var val = newValue ? newValue : 0;

                            var me = field.up('panel');
                            
                            var qtyField = me.down("field[name=IChargeQty]"),
                                priceField = me.down("field[name=x_UnitPrice]");

                            if(qtyField.getValue() > 0) 
                                priceField.setValue(val / qtyField.getValue());

                        }
                    }
                }, {
                    margin: '0 0 15 0',
                    xtype: 'combo',
                    name: 'IChargeCurrencyCode',
                    fieldLabel: 'Currency',
                    store: storeCurrencyRates,
                    labelWidth: 50,
                    listConfig: {
                        minWidth: null
                    },
                    columnWidth: 0.5,
                    valueField: 'CurrencyCode',
                    displayField: 'CurrencyCodeDesc',
                    queryMode: 'local',
                    autoSelect: false,
                    //typeAhead: true,
                    minChars: 2,
                    allowBlank: false,
                    forceSelection: true,
                    anyMatch: true,
                    listeners: {
                        select: function(field, records, eOpts) {
                            if (records.length > 0) {
                                var me = this.up('panel');
                                field.next().setValue(records[0].data.CurrencyRate);
                            }
                        },
                        change: function(field, newValue, oldValue) {
                            if (newValue === null) {
                                field.next().setValue(0);
                            }
                        },
                        blur: function(field, The, eOpts) {
                            if (field.value !== null) {
                                var me = this.up('panel');
                                copyToField = field.valueModels[0].data.CurrencyRate;
                                copyField = me.down('field[name=IChargeCurrencyRate]');
                                copyField.setValue(copyToField);
                            }
                        }
                    },
                    tpl: Ext.create('Ext.XTemplate',
                        '<tpl for=".">',
                        '<div class="x-boundlist-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                        '</tpl>')
                }, {
                    xtype: 'numericfield',
                    hidden: true,
                    name: 'IChargeCurrencyRate'
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
        this.down('field[name=IChargeSort]').focus(true, 200);
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
                    var parentForm = me.callerForm;
                    parentForm.down("#gridcharges").store.reload();
                    me.destroy();
                    Ext.Msg.hide();
                } else {
                    Ext.Msg.hide();
                }
            }
        });
    }
});
