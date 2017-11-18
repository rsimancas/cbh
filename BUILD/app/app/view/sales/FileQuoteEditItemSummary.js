Ext.define('CBH.view.sales.FileQuoteEditItemSummary', {
    extend: 'Ext.form.Panel',
    alias: 'widget.FileQuoteEditItemSummary',
    modal: true,
    width: 1024,
    layout: {
        type: 'column'
    },
    title: 'Item Summary',
    bodyPadding: 10,
    closable: true,
    floating: true,
    callerForm: "",
    VendorKey: null,
    ItemKey: null,

    storesLoaded: null,
    storeToLoad: 0,

    initComponent: function() {

        var me = this;

        var storeVendors = null;
        var storeItems = null;
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
                xtype: 'container',
                columnWidth: 1,
                layout: {
                    type: 'anchor'
                },
                items: [{
                    xtype: 'fieldset',
                    anchor: '100%',
                    defaults: {
                        anchor: '100%',
                    },
                    collapsible: true,
                    title: 'Item Detail',
                    items: [{
                        xtype: 'fieldcontainer',
                        layout: {
                            type: 'hbox'
                        },
                        items: [{
                            flex: 2,
                            xtype: 'numericfield',
                            name: 'QSummaryQty',
                            fieldLabel: 'Quantity',
                            fieldStyle: 'text-align: right;',
                            minValue: 1,
                            hideTrigger: false,
                            useThousandSeparator: true,
                            decimalPrecision: 0,
                            alwaysDisplayDecimals: true,
                            allowNegative: false,
                            alwaysDecimals: false,
                            thousandSeparator: ',',
                            selectOnFocus: true,
                            listeners: {
                                blur: function(field, The, eOpts) {
                                    if (field.value !== null) {
                                        if (field.value <= 0) {
                                            field.focus(true, 200);
                                            return;
                                        }

                                        var me = this.up('panel');
                                        //cost = me.down('field[name=QSummaryCost]').getValue();
                                        price = me.down('field[name=QSummaryPrice]').getValue();
                                        qty = field.value;

                                        //me.down('field[name=x_LineCost]').setValue(cost * qty);
                                        me.down('field[name=QSummaryLinePrice]').setValue(price * qty);
                                    }
                                },
                                change: function(field, newValue, oldValue, eOpts) {
                                    if (document.activeElement.name !== field.name) return;

                                    var val = newValue ? newValue : 0;

                                    var me = field.up('panel');

                                    var priceField = me.down("field[name=QSummaryPrice]"),
                                        linePriceField = me.down("field[name=QSummaryLinePrice]");

                                    linePriceField.setValue(priceField.getValue() * val);

                                }
                            }
                        }, {
                            xtype: 'component',
                            flex: 2
                        }, {
                            flex: 2,
                            xtype: 'numericfield',
                            name: 'QSummarySort',
                            fieldLabel: 'Sort',
                            fieldStyle: 'text-align: right;',
                            labelStyle: 'text-align: right;',
                            minValue: 100,
                            hideTrigger: false,
                            useThousandSeparator: true,
                            decimalPrecision: 0,
                            alwaysDisplayDecimals: true,
                            allowNegative: false,
                            alwaysDecimals: false,
                            thousandSeparator: ',',
                            selectOnFocus: true,
                            step: 100
                        }]
                    }, {
                        xtype: 'fieldcontainer',
                        layout: {
                            type: 'column'
                        },
                        items: [{
                            xtype: 'combo',
                            name: 'QSummaryVendorKey',
                            fieldLabel: 'Vendor',
                            columnWidth: 1,
                            valueField: 'VendorKey',
                            displayField: 'VendorName',
                            store: storeVendors,
                            queryMode: 'remote',
                            pageSize: 11,
                            minChars: 2,
                            allowBlank: false,
                            triggerAction: '',
                            forceSelection: false,
                            queryCaching: false,
                            emptyText: 'Choose Vendor',
                            autoSelect: false,
                            selectOnFocus: true,
                            listeners: {
                                buffer: 100,
                                change: function(field, newValue, oldValue) {
                                    if (document.activeElement.name !== field.name) return;

                                    if (field.readOnly) return;

                                    var me = field.up('panel');

                                    field.next().clearValue();
                                    me.down('field[name=QSummaryDescription]').setValue(null);
                                },
                                blur: {
                                    fn: me.onVendorBlur,
                                    scope: me
                                }
                            }
                        }, {
                            xtype: 'combo',
                            name: 'QSummaryItemNum',
                            fieldLabel: 'Item Num',
                            columnWidth: 1,
                            valueField: 'ItemNum',
                            displayField: 'ItemNum',
                            store: storeItems,
                            pageSize: 11,
                            queryMode: 'remote',
                            minChars: 2,
                            allowBlank: false,
                            forceSelection: false,
                            selectOnFocus: true,
                            triggerAction: '',
                            queryBy: 'ItemNum,x_ItemName',
                            queryCaching: false, // set false to let show new item created
                            enableKeyEvents: true,
                            emptyText: 'Choose Item',
                            tpl: Ext.create('Ext.XTemplate',
                                '<tpl for="."><div class="x-boundlist-item">{x_ItemNumName}</div></tpl>'
                            ),
                            listeners: {
                                buffer: 100,
                                select: function(field, records, eOpts) {
                                    if (records.length > 0) {
                                        field.next().setValue(records[0].data.x_ItemName);
                                    }
                                },
                                change: function(field, newValue, oldValue) {
                                    if (document.activeElement.name !== field.name) return;

                                    if(String.isNullOrEmpty(newValue)) {
                                        var me = field.up('form');
                                        me.onVendorBlur(me.down('field[name=QSummaryVendorKey]'));
                                    }
                                },
                                blur: {
                                    fn: me.onItemVendorBlur,
                                    scope: me
                                }
                            }
                        }, {
                            xtype: 'textfield',
                            fieldLabel: 'Description',
                            name: 'QSummaryDescription',
                            columnWidth: 1,
                            selectOnFocus: true,
                            allowBlank: false
                        }]
                    }]
                }, {
                    xtype: 'fieldset',
                    anchor: '100%',
                    margin: '0 0 0 5',
                    defaults: {
                        anchor: '100%',
                    },
                    collapsible: true,
                    title: 'Cost and Pricing Information',
                    items: [{
                        xtype: 'fieldcontainer',
                        layout: {
                            type: 'column'
                        },
                        items: [{
                            xtype: 'combo',
                            name: 'QSummaryCurrencyCode',
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
                            minChars: 2,
                            allowBlank: false,
                            forceSelection: true,
                            anyMatch: true,
                            listeners: {
                                select: function(field, records, eOpts) {
                                    if (records.length > 0) {
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
                                        copyField = me.down('field[name=QSummaryCurrencyRate]');
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
                            name: 'QSummaryCurrencyRate',
                            columnWidth: 0.5,
                            margin: '0 0 0 10',
                            hideTrigger: true,
                            useThousandSeparator: true,
                            decimalPrecision: 2,
                            alwaysDisplayDecimals: true,
                            allowNegative: false,
                            currencySymbol: '$',
                            alwaysDecimals: true,
                            thousandSeparator: ',',
                            fieldLabel: 'Rate',
                            labelAlign: 'top',
                            fieldStyle: 'text-align: right;',
                            allowBlank: false,
                            selectOnFocus: true
                        }]
                    }, {
                        xtype: 'fieldcontainer',
                        layout: {
                            type: 'hbox'
                        },
                        items: [{
                            xtype: 'numericfield',
                            flex: 3,
                            name: 'QSummaryPrice',
                            hideTrigger: true,
                            useThousandSeparator: true,
                            alwaysDisplayDecimals: true,
                            allowNegative: false,
                            alwaysDecimals: true,
                            thousandSeparator: ',',
                            fieldLabel: 'Per Unit Price',
                            labelAlign: 'top',
                            fieldStyle: 'text-align: right;',
                            currencySymbol: '$',
                            selectOnFocus: true,
                            allowBlank: false,
                            listeners: {
                                change: function(field, newValue, oldValue, eOpts) {
                                    if (document.activeElement.name !== field.name) return;

                                    var val = newValue ? newValue : 0;

                                    var me = field.up('panel');

                                    var qtyField = me.down("field[name=QSummaryQty]"),
                                        linePriceField = me.down("field[name=QSummaryLinePrice]");

                                    linePriceField.setValue(qtyField.getValue() * val);

                                }
                            }
                        }, {
                            xtype: 'numericfield',
                            flex: 3,
                            name: 'QSummaryLinePrice',
                            margin: '0 0 0 10',
                            hideTrigger: true,
                            useThousandSeparator: true,
                            //decimalPrecision: 5,
                            alwaysDisplayDecimals: true,
                            allowNegative: false,
                            currencySymbol: '$',
                            selectOnFocus: true,
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

                                    var qtyField = me.down("field[name=QSummaryQty]"),
                                        priceField = me.down("field[name=QSummaryPrice]");

                                    priceField.setValue((qtyField.getValue() / val).toFixed(2));

                                }
                            }
                        }]
                    }]
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
        var me = this;
        me.onLoadRecord();
    },

    onLoadRecord: function() {
        var me = this,
            vendorKey = me.currentRecord.data.QSummaryVendorKey;

        me.storeToLoad = (me.currentRecord.data.QSummaryItemKey) ? 2 : 1;
        me.storesLoaded = [];

        Ext.Msg.wait('Loading data...', 'Wait');

        var storeVendors = new CBH.store.vendors.Vendors().load({
            params: {
                id: vendorKey ? vendorKey : 0,
                page: 1,
                start: 0,
                limit: 8
            },
            callback: function(records, operation, success) {
                if (records[0]) {
                    var cboVendor = me.down('field[name=QSummaryVendorKey]');
                    cboVendor.bindStore(storeVendors);
                    cboVendor.setValue(vendorKey);
                }

                me.storesLoaded.push("Vendors");
                me.checkLoaded();
            }
        });

        if (me.currentRecord.data.QSummaryItemKey) {
            var filterVendor = new Ext.util.Filter({
                property: 'ItemVendorKey',
                value: vendorKey
            });

            var storeItems = new CBH.store.vendors.Items().load({
                params: {
                    ItemNum: me.currentRecord.data.QSummaryItemNum,
                    page: 1,
                    limit: 8,
                    start: 0
                },
                filters: [filterVendor],
                callback: function(records, operation, success) {
                    me.down('field[name=QSummaryItemKey]').filters = filterVendor;
                    me.down('field[name=QSummaryItemKey]').bindStore(this);
                    me.down('field[name=QSummaryItemKey]').setValue(records[0].data.ItemKey);

                    me.storesLoaded.push("Items");
                    me.checkLoaded();
                }
            });
        }

        this.down('field[name=QSummaryQty]').focus(true, 200);
    },

    checkLoaded: function() {
        var me = this,
            stores = me.storesLoaded;

        if (stores.length < me.storeToLoad) {
            return;
        }

        Ext.Msg.hide();
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
                    parentForm.down("#griditemssummary").store.reload();
                    me.destroy();
                    Ext.Msg.hide();
                } else {
                    Ext.Msg.hide();
                }
            }
        });
    },

    onVendorBlur: function(field, The, eOpts) {
        if (field.readOnly) return;

        var me = field.up('panel'),
            rawvalue = field.getRawValue();

        if (field && field.valueModels !== null) {

            me.getEl().mask('Loading item\'s vendors Please wait...');
            vendormodel = new CBH.model.vendors.Vendors({
                VendorKey: field.value
            });
            storeLastQuoteMargin = vendormodel.LastQuoteMargin().load({
                params: {
                    page: 0,
                    start: 0,
                    limit: 0
                },
                callback: function() {
                    me.storeLastQuoteMargin = storeLastQuoteMargin;
                    var filterVendor = new Ext.util.Filter({
                        property: 'ItemVendorKey',
                        value: field.value
                    });
                    var storeItems = new CBH.store.vendors.Items().load({
                        params: {
                            page: 1,
                            start: 0,
                            limit: 8
                        },
                        filters: [filterVendor],
                        callback: function() {
                            field.next().bindStore(storeItems);
                            field.next().filters = filterVendor;
                            me.getEl().unmask();
                        }
                    });
                }
            });
        } else if (rawvalue !== null && rawvalue !== '') {
            Ext.Msg.show({
                title: 'Question',
                msg: 'The vendor doesn\'t exists, Do you want to add to database?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(e) {
                    if (e === "yes") {
                        me.addVendor(rawvalue);
                    } else {
                        field.setValue(null);
                        field.focus(true, 200);
                    }
                }
            });
        }
    },

    addVendor: function(value) {
        var me = this;

        var storeToNavigate = new CBH.store.vendors.Vendors({
            autoLoad: false
        });
        model = Ext.create('CBH.model.vendors.Vendors', {
            VendorName: value,
            VendorLanguageCode: 'en'
        });
        var form = Ext.widget('vendors', {
            storeNavigator: storeToNavigate,
            modal: true,
            width: 700,
            frameHeader: true,
            header: true,
            layout: {
                type: 'column'
            },
            title: 'New Vendor',
            bodyPadding: 10,
            closable: true,
            stateful: false,
            floating: true,
            callerForm: me,
            forceFit: true
        });

        form.show();

        var btn = form.down('#FormToolbar').down('#add');
        btn.fireEvent('click', btn, null, null, model);
    },

    onItemVendorBlur: function(field, The, eOpts) {
        var me = this;

        if (field && field.valueModels !== null && field.valueModels.length) {

            var selectedItem = field.valueModels[0].data;

            field.next().setValue(selectedItem.x_ItemName);

            var cost = selectedItem.ItemCost,
                price = selectedItem.ItemPrice,
                qty = me.down('field[name=QSummaryQty]').getValue();

            me.down('field[name=QSummaryPrice]').setValue(price);
            me.down('field[name=QSummaryLinePrice]').setValue(price * qty);

            me.down('field[name=QSummaryCurrencyCode]').setValue('USD');
            me.down('field[name=QSummaryCurrencyRate]').setValue(1);
        }
    },

    setProfitMargin: function(valueMargin) {
        var me = this;

        var profitMargin = valueMargin;

        //var cost = me.down('field[name=QSummaryCost]').getValue();
        var price = me.down('field[name=QSummaryPrice]').getValue();
        var qty = me.down('field[name=QSummaryQty]').getValue();

        //var linecost = cost * qty;
        var lineprice = price * qty;

        var itemWeight = me.down('field[name=QSummaryWeight]').getValue();
        var itemVolume = me.down('field[name=QSummaryVolume]').getValue();

        var lineWeight = itemWeight * qty;
        var lineVolume = itemVolume * qty;

        //me.down('#x_LineWeight').setValue(lineWeight);
        //me.down('#x_LineVolume').setValue(lineVolume);

        //me.down('#x_ProfitMargin').setValue(profitMargin);
        me.down('field[name=QSummaryPrice]').setValue(price);
        //me.down('field[name=x_LineCost]').setValue(linecost);
        me.down('field[name=QSummaryLinePrice]').setValue(lineprice);
    }
});
