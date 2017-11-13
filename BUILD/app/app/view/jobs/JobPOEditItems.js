Ext.define('CBH.view.jobs.JobPOEditItems', {
    extend: 'Ext.form.Panel',
    alias: 'widget.jobpoedititems',
    modal: true,
    width: 1024,
    layout: {
        type: 'column'
    },
    title: 'Item',
    bodyPadding: 10,
    closable: true,
    floating: true,
    callerForm: "",
    VendorKey: null,

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
                columnWidth: 0.5,
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
                            name: 'POItemsQty',
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
                            listeners: {
                                change: function(field, newValue, oldValue) {
                                    if (document.activeElement.name !== field.name) return;

                                    if (field.value !== null) {
                                        var me = this.up('panel');

                                        weight = me.down('field[name=POItemsWeight]').getValue();
                                        volume = me.down('field[name=POItemsVolume]').getValue();
                                        qty = newValue;

                                        if(weight !== null)
                                            me.down('field[name=POItemsLineWeight]').setValue(weight * qty);

                                        if(volume !== null)
                                            me.down('field[name=POItemsLineVolume]').setValue(volume * qty);
                                    }
                                },
                                blur: function(field, The, eOpts) {
                                    if (field.value !== null) {
                                        if (field.value <= 0) {
                                            field.focus(true, 200);
                                            return;
                                        }

                                        var me = this.up('panel');
                                        cost = me.down('field[name=POItemsCost]').getValue();
                                        price = me.down('field[name=POItemsPrice]').getValue();
                                        qty = field.value;

                                        me.down('field[name=POItemsLineCost]').setValue(cost * qty);
                                        me.down('field[name=POItemsLinePrice]').setValue(price * qty);

                                        weight = me.down('field[name=POItemsWeight]').getValue();
                                        volume = me.down('field[name=POItemsVolume]').getValue();

                                        if(weight !== null)
                                            me.down('field[name=POItemsLineWeight]').setValue(weight * qty);

                                        if(volume !== null)
                                            me.down('field[name=POItemsLineVolume]').setValue(volume * qty);
                                    }
                                }
                            }
                        }, {
                            xtype: 'component',
                            flex: 2
                        }, {
                            flex: 2,
                            xtype: 'numericfield',
                            name: 'POItemsSort',
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
                            step: 100
                        }]
                    }, {
                        xtype: 'fieldcontainer',
                        layout: {
                            type: 'column'
                        },
                        items: [{
                            xtype: 'textfield',
                            name: 'x_VendorName',
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
                            /*listeners: {
                                buffer: 100,
                                change: function(field, newValue, oldValue) {
                                    if (field.readOnly) return;

                                    var me = field.up('panel');

                                    field.next().clearValue();
                                    me.down('field[name=x_ItemName]').setValue(null);
                                },
                                blur: {
                                    fn: me.onVendorBlur,
                                    scope: me
                                }
                            },*/
                            readOnly: true
                        }, {
                            xtype: 'combo',
                            name: 'POItemsItemKey',
                            fieldLabel: 'Item Num',
                            columnWidth: 0.95,
                            valueField: 'ItemKey',
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
                                blur: {
                                    fn: me.onItemVendorBlur,
                                    scope: me
                                }//,
                                /*afterrender: function (combo) {
                                    var me = combo.up("form");
                                    combo.getEl().on('dblclick', function() {
                                        me.onPOItemsKeyDoubleClick();
                                    });

                                    new Ext.ToolTip({
                                        target: combo.getEl(),
                                        html: 'Double-Click to load Item Maintenance'
                                    });
                                }*/
                            }
                        }, {
                            xtype: 'button',
                            margin: '26 0 0 5',
                            columnWidth: 0.05,
                            glyph: 0xf1e5, //0xf067,
                            itemId: 'btnAddItem',
                            scale: 'medium',
                            border: false,
                            /*cls:'myButton',*/
                            ui: 'plain',
                            style: 'background-color:white!important; color:#004A9C !important; font-size: 16px !important;',
                            iconAlign: 'left',
                            tooltip: 'Item Maintenance',
                            listeners: {
                                click: {
                                    fn: me.onPOItemsKeyDoubleClick,
                                    scope: me
                                }
                            }
                        }, {
                            xtype: 'textfield',
                            fieldLabel: 'Item Name',
                            name: 'x_ItemName',
                            columnWidth: 1,
                            readOnly: true
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
                            name: 'POItemsCurrencyCode',
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
                                        copyField = me.down('field[name=POItemsCurrencyRate]');
                                        copyField.setValue(copyToField);
                                    }
                                },
                                beforequery: function(record) {
                                    record.query = new RegExp(record.query, 'i');
                                    record.forceAll = true;
                                }
                            },
                            tpl: Ext.create('Ext.XTemplate',
                                '<tpl for=".">',
                                '<div class="x-boundlist-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                                '</tpl>')
                        }, {
                            xtype: 'numericfield',
                            name: 'POItemsCurrencyRate',
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
                            allowBlank: false
                        }]
                    }, {
                        xtype: 'fieldcontainer',
                        layout: {
                            type: 'hbox'
                        },
                        items: [{
                            xtype: 'numericfield',
                            flex: 3,
                            name: 'POItemsCost',
                            hideTrigger: true,
                            useThousandSeparator: true,
                            alwaysDisplayDecimals: true,
                            allowNegative: false,
                            alwaysDecimals: true,
                            thousandSeparator: ',',
                            fieldLabel: 'Cost Per Unit',
                            labelAlign: 'top',
                            fieldStyle: 'text-align: right;',
                            allowBlank: false,
                            listeners: {
                                blur: function(field, The, eOpts) {
                                    if (field.value !== null) {
                                        var me = this.up('panel');

                                        profitMargin = parseFloat(me.down('#x_ProfitMargin').getValue());

                                        me.down('field[name=POItemsPrice]').setValue(field.value / (1 - (profitMargin / 100)));

                                        qty = me.down('field[name=POItemsQty]').getValue();

                                        me.down('field[name=POItemsLineCost]').setValue(field.value * qty);
                                        me.down('field[name=POItemsLinePrice]').setValue(me.down('#POItemsPrice').getValue() * qty);
                                    }
                                }
                            }
                        }, {
                            xtype: 'numericfield',
                            flex: 3,
                            name: 'x_ProfitMargin',
                            itemId: 'x_ProfitMargin',
                            margin: '0 0 0 10',
                            fieldLabel: 'Profit',
                            hideTrigger: true,
                            useThousandSeparator: true,
                            decimalPrecision: 2,
                            alwaysDisplayDecimals: true,
                            allowNegative: false,
                            alwaysDecimals: true,
                            thousandSeparator: ',',
                            labelAlign: 'top',
                            fieldStyle: 'text-align: right;',
                            allowBlank: false,
                            maxValue: 99.99,
                            labelStyle: 'text-align: center;',
                            listeners: {
                                blur: function(field, The, eOpts) {
                                    if (field.value !== null) {
                                        var me = this.up('panel');
                                        var cost = me.down('field[name=POItemsCost]').getValue();
                                        var profitMargin = (field.value > 0) ? field.value : 0;
                                        var price = cost;

                                        if (profitMargin < 100) price = cost / (1 - (profitMargin / 100));

                                        me.down('field[name=POItemsPrice]').setValue(price);

                                        qty = me.down('field[name=POItemsQty]').getValue();

                                        me.down('field[name=POItemsLinePrice]').setValue(price * qty);
                                    }
                                }
                            }
                        }, {
                            xtype: 'numericfield',
                            flex: 3,
                            name: 'POItemsPrice',
                            margin: '0 0 0 10',
                            hideTrigger: true,
                            useThousandSeparator: true,
                            alwaysDisplayDecimals: true,
                            allowNegative: false,
                            alwaysDecimals: true,
                            thousandSeparator: ',',
                            fieldLabel: 'Per Unit Price',
                            labelAlign: 'top',
                            fieldStyle: 'text-align: right;',
                            allowBlank: false,
                            listeners: {
                                blur: function(field, The, eOpts) {
                                    if (field.value !== null) {
                                        var me = this.up('panel');
                                        cost = me.down('field[name=POItemsCost]').getValue();
                                        profitMargin = (1 - (cost / field.value)).toFixed(2) * 100;
                                        me.down('#x_ProfitMargin').setValue(profitMargin);

                                        qty = me.down('field[name=POItemsQty]').getValue();

                                        me.down('field[name=POItemsLinePrice]').setValue(field.value * qty);
                                    }
                                }
                            }
                        }]
                    }, {
                        xtype: 'fieldcontainer',
                        layout: {
                            type: 'hbox'
                        },
                        items: [{
                            xtype: 'numericfield',
                            flex: 3,
                            name: 'POItemsLineCost',
                            hideTrigger: true,
                            useThousandSeparator: true,
                            alwaysDisplayDecimals: true,
                            allowNegative: false,
                            currencySymbol: '$',
                            alwaysDecimals: true,
                            thousandSeparator: ',',
                            fieldLabel: 'Total Cost',
                            labelAlign: 'top',
                            fieldStyle: 'text-align: right;',
                            allowBlank: false,
                            readOnly: true,
                        }, {
                            xtype: 'component',
                            flex: 3
                        }, {
                            xtype: 'numericfield',
                            flex: 3,
                            name: 'POItemsLinePrice',
                            margin: '0 0 0 10',
                            hideTrigger: true,
                            useThousandSeparator: true,
                            //decimalPrecision: 5,
                            alwaysDisplayDecimals: true,
                            allowNegative: false,
                            currencySymbol: '$',
                            alwaysDecimals: true,
                            thousandSeparator: ',',
                            fieldLabel: 'Total Price',
                            labelAlign: 'top',
                            fieldStyle: 'text-align: right;',
                            allowBlank: false,
                            readOnly: true
                        }]
                    }]
                }]
            }, {
                xtype: 'container',
                columnWidth: 0.5,
                margin: '0 0 0 10',
                layout: {
                    type: 'anchor'
                },
                items: [{
                    xtype: 'fieldset',
                    anchor: '100%',
                    margin: '0 5 5 0',
                    defaults: {
                        anchor: '100%',
                    },
                    collapsible: true,
                    title: 'Weight and Volume',
                    items: [{
                        xtype: 'fieldcontainer',
                        layout: 'hbox',
                        items: [{
                            xtype: 'numberfield',
                            name: 'POItemsWeight',
                            fieldLabel: 'Per Unit. Kg',
                            fieldStyle: 'text-align: right;',
                            hideTrigger: true,
                            flex: 4,
                            listeners: {
                                change: function(field, newValue, oldValue, eOpts) {
                                    if (document.activeElement.name !== field.name) return;

                                    if (field.value !== null) {
                                        var me = this.up('panel');

                                        var qty = me.down('field[name=POItemsQty]').getValue();

                                        me.down('field[name=POItemsLineWeight]').setValue(field.value * qty);
                                    }
                                }
                            }
                        }, {
                            xtype: 'button',
                            margin: '25 0 0 0',
                            glyph: 0xf0d0,
                            itemId: 'btnConvertPounds',
                            scale: 'medium',
                            border: false,
                            width: 35,
                            ui: 'plain',
                            style: 'background-color:white!important; color:#004A9C !important; font-size: 16px !important;',
                            iconAlign: 'left',
                            tooltip: 'convert pounds',
                            handler: function(btn) {
                                var me = btn.up('form'),
                                    value = me.down('field[name=POItemsWeight]').getValue();

                                me.onConvertPoundsClick(value);
                            }
                        }, {
                            xtype: 'component',
                            flex: 1
                        }, {
                            xtype: 'numberfield',
                            name: 'POItemsVolume',
                            fieldLabel: 'Per Unit m³',
                            fieldStyle: 'text-align: right;',
                            hideTrigger: true,
                            flex: 4,
                            listeners: {
                                change: function(field, newValue, oldValue, eOpts) {
                                    if (document.activeElement.name !== field.name) return;

                                    if (field.value !== null) {
                                        var me = this.up('panel');

                                        var qty = me.down('field[name=POItemsQty]').getValue();

                                        me.down('field[name=POItemsLineVolume]').setValue(field.value * qty);
                                        
                                    }
                                }
                            }
                        }, 
                        {
                            xtype: 'button',
                            margin: '25 0 0 0',
                            glyph: 0xf0d0,
                            itemId: 'btnConvertCubicFeets',
                            scale: 'medium',
                            border: false,
                            width: 35,
                            ui: 'plain',
                            style: 'background-color:white!important; color:#004A9C !important; font-size: 16px !important;',
                            iconAlign: 'left',
                            tooltip: 'convert pounds',
                            handler: function(btn) {
                                var me = btn.up('form'),
                                    value = me.down('field[name=POItemsVolume]').getValue();

                                me.onConvertCubicFeetsClick(value);
                            }
                        }]
                    }, {
                        xtype: 'fieldcontainer',
                        layout: 'hbox',
                        items: [{
                            xtype: 'numberfield',
                            name: 'POItemsLineWeight',
                            fieldLabel: 'Total Kg',
                            fieldStyle: 'text-align: right;',
                            hideTrigger: true,
                            flex: 3
                        }, {
                            xtype: 'component',
                            flex: 1
                        }, {
                            xtype: 'numberfield',
                            name: 'POItemsLineVolume',
                            fieldLabel: 'Total m³',
                            fieldStyle: 'text-align: right;',
                            hideTrigger: true,
                            flex: 3
                        }]
                    }]
                }, {
                    xtype: 'fieldcontainer',
                    flex: 5,
                    layout: {
                        type: 'anchor'
                    },
                    items: [{
                        xtype: 'fieldset',
                        flex: 4,
                        margin: '0 5 5 0',
                        defaults: {
                            anchor: '100%',
                        },
                        collapsible: true,
                        title: 'Notes To Customer',
                        items: [{
                            xtype: 'textareafield',
                            name: 'POItemsMemoCustomer',
                        }]
                    }, {
                        xtype: 'fieldset',
                        flex: 4,
                        margin: '0 5 5 0',
                        defaults: {
                            anchor: '100%',
                        },
                        collapsible: true,
                        title: 'Notes To Supplier',
                        items: [{
                            xtype: 'textareafield',
                            anchor: '100%',
                            name: 'POItemsMemoVendor',
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
        var me = this;

        me.storeToLoad = 2;
        me.storesLoaded = [];

        Ext.Msg.wait('Loading data...', 'Wait');

        var storeVendors = new CBH.store.vendors.Vendors().load({
            params: {
                id: me.VendorKey,
                page: 1,
                start: 0,
                limit: 8
            },
            callback: function(records, operation, success) {
                if (records[0]) {
                    var cboVendor = me.down('field[name=x_VendorName]');

                    /*if (!cboVendor.readOnly) {
                        cboVendor.bindStore(this);
                        cboVendor.setValue(records[0].data.VendorKey);
                    } else {
                        cboVendor.setRawValue(records[0].data.VendorName);
                    }*/

                    cboVendor.setRawValue(records[0].data.VendorName);
                }

                me.storesLoaded.push("Vendors");
                me.checkLoaded();
            }
        });

        // vendormodel = new CBH.model.vendors.Vendors({VendorKey:me.VendorKey}); 

        var filterVendor = new Ext.util.Filter({
            property: 'ItemVendorKey',
            value: me.VendorKey
        });

        var storeItems = new CBH.store.vendors.Items().loadPage(1, {
            params: {
                id: me.currentRecord.data.POItemsItemKey
            },
            filters: [filterVendor],
            callback: function(records, operation, success) {
                me.down('field[name=POItemsItemKey]').filters = filterVendor;
                me.down('field[name=POItemsItemKey]').bindStore(this);
                me.down('field[name=POItemsItemKey]').setValue(records[0].data.ItemKey);

                me.storesLoaded.push("Items");
                me.checkLoaded();
            }
        });


        this.down('field[name=POItemsQty]').focus(true, 200);
    },

    checkLoaded: function() {
        var me = this,
            stores = me.storesLoaded;

        if (stores.length < me.storeToLoad) {
            return;
        }

        me.checkChange();
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
                    parentForm.down("#gridPOItems").store.reload();
                    me.destroy();
                    Ext.Msg.hide();
                } else {
                    Ext.Msg.hide();
                }
            }
        });
    },

    onItemVendorBlur: function(field, The, eOpts) {
        var me = this;

        if (field && field.valueModels !== null && field.valueModels.length) {

            var selectedItem = field.valueModels[0].data;

            //field.next().setValue(selectedItem.x_ItemName);
            me.down('field[name=x_ItemName]').setValue(selectedItem.x_ItemName);

            var cost = selectedItem.ItemCost,
                price = selectedItem.ItemPrice;

            me.down('field[name=POItemsCost]').setValue(cost);
            me.down('field[name=POItemsPrice]').setValue(price);

            if(selectedItem.ItemWeight)
                me.down('field[name=POItemsWeight]').setValue(selectedItem.ItemWeight);
            if(selectedItem.ItemVolume)
                me.down('field[name=POItemsVolume]').setValue(selectedItem.ItemVolume);

            me.down('field[name=POItemsCurrencyCode]').setValue('USD');
            me.down('field[name=POItemsCurrencyRate]').setValue(1);


            var valueMargin = 15,
                profit = GetProfitMargin(cost, price);
            if (me.storeLastQuoteMargin && me.storeLastQuoteMargin.getCount() > 0) {
                // valueMargin = me.storeLastQuoteMargin.getAt(0).data.FVProfitMargin;
                // if (Math.max(profit, valueMargin) === valueMargin) {
                me.setProfitMargin(profit);
                //}
            } else {
                var wind = new CBH.view.sales.InputProfitMargin();
                wind.modal = true;
                wind.callerForm = me.id;
                wind.down("numberfield").setValue(valueMargin);
                wind.show();
            }
        }
    },

    onPOItemsKeyDoubleClick: function() {
        var me = this,
            ItemKey = me.down("field[name=POItemsItemKey]").getValue();

        //var vendorkey = me.down('field[name=QuoteVendorKey]').getValue();
        var storeToNavigate = new CBH.store.vendors.Items().load({
            params: {
                id: ItemKey
            },
            callback: function() {
                var form = Ext.widget('items', {
                    storeNavigator: storeToNavigate,
                    modal: true,
                    width: 790,
                    frameHeader: true,
                    header: true,
                    layout: {
                        type: 'column'
                    },
                    title: 'Item Maintenance',
                    bodyPadding: 10,
                    closable: true,
                    //constrain: true,
                    stateful: false,
                    floating: true,
                    callerForm: me,
                    forceFit: true
                });

                form.down('#FormToolbar').gotoAt(1);

                form.show();
            }
        });
    },

    onConvertPoundsClick: function(value) {
        var me = this;

        var wind = new CBH.view.common.InputConvert({
            callerForm: me,
            convertOptions: {
                typeConvertion: "kilograms",
                callback: me.setKilograms
            },
            title: 'Enter Pounds',
            fieldLabel: 'Enter the pounds for this item',
            currentValue: value
        });
        wind.modal = true;
        wind.show();
    },

    setKilograms: function(data) {
        var me = data.callerForm;
        me.down("field[name=POItemsWeight]").setValue(data.value);
        me.down("field[name=POItemsWeight]").focus(true, 200);
        var qty = me.down('field[name=POItemsQty]').getValue();
        me.down('field[name=POItemsLineWeight]').setValue(data.value * qty);
    },

    onConvertCubicFeetsClick: function(value) {
        var me = this;

        var wind = new CBH.view.common.InputConvert({
            callerForm: me,
            convertOptions: {
                typeConvertion: "cubicmeters",
                callback: me.setVolume
            },
            title: 'Enter Cubic Feet',
            fieldLabel: 'Enter the cubic feet for this item',
            currentValue: value
        });
        wind.modal = true;
        wind.show();
    },

    setVolume: function(data) {
        var me = data.callerForm;
        me.down("field[name=POItemsVolume]").setValue(data.value);
        me.down("field[name=POItemsVolume]").focus(true, 200);
        var qty = me.down('field[name=POItemsQty]').getValue();
        me.down('field[name=POItemsLineVolume]').setValue(data.value * qty);
    }
});