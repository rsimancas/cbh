Ext.define('CBH.view.sales.FileLineEntry', {
    extend: 'Ext.form.Panel',
    alias: 'widget.filelineentry',

    layout: 'column',
    bodyPadding: 10,
    frameHeader: false,
    header: false,
    title: 'Line Entry',
    enableKeyEvents: true,

    modeNew: false,
    FileKey: 0,
    FileNum: "",
    VendorKey: 0,
    ItemKey: 0,
    storeNavigator: null,
    storeLastQuoteMargin: null,

    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;
        
        var me = this;

        var storeVendors = new CBH.store.vendors.Vendors().load({
            params: {
                id: me.VendorKey,
                page: 1,
                start: 0,
                limit: 8,
                sort: JSON.stringify([{"property": "VendorName", "direction": "ASC"}])
            }
        });

        var storeCurrencyRates = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        vendormodel = new CBH.model.vendors.Vendors({
            VendorKey: me.VendorKey
        });

        var filterVendor = new Ext.util.Filter({
            property: 'ItemVendorKey',
            value: me.VendorKey
        });

        var storeItems = new CBH.store.vendors.Items().loadPage(1, {
            params: {
                id: me.ItemKey
            },
            filters: [filterVendor]
        });

        var storeLastQuoteMargin = new CBH.store.vendors.LastQuoteMargin().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            },
            filters: [filterVendor]
        });

        Ext.applyIf(me, {
            fieldDefaults: {
                labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items: [
                // Panel Left
                {
                    xtype: 'container',
                    columnWidth: 0.5,
                    layout: {
                        type: 'anchor'
                    },
                    items: [
                        // Item Detail
                        {
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
                                    name: 'QuoteQty',
                                    itemId: 'QuoteQty',
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

                                                weight = me.down('field[name=QuoteItemWeight]').getValue();
                                                volume = me.down('field[name=QuoteItemVolume]').getValue();
                                                qty = newValue;

                                                if(weight !== null)
                                                    me.down('field[name=x_LineWeight]').setValue(weight * qty);

                                                if(volume !== null)
                                                    me.down('field[name=x_LineVolume]').setValue(volume * qty);
                                            }
                                        },
                                        blur: function(field, The, eOpts) {
                                            if (field.value !== null) {
                                                if (field.value <= 0) {
                                                    field.focus(true, 200);
                                                    return;
                                                }

                                                var me = this.up('panel');
                                                cost = me.down('#QuoteItemCost').getValue();
                                                price = me.down('#QuoteItemPrice').getValue();
                                                weight = me.down('field[name=QuoteItemWeight]').getValue();
                                                volume = me.down('field[name=QuoteItemVolume]').getValue();
                                                qty = field.value;

                                                me.down('#QuoteItemLineCost').setValue(cost * qty);
                                                me.down('#QuoteItemLinePrice').setValue(price * qty);

                                                if(weight !== null)
                                                    me.down('field[name=x_LineWeight]').setValue(weight * qty);

                                                if(volume !== null)
                                                    me.down('field[name=x_LineVolume]').setValue(volume * qty);
                                            }
                                        }
                                    }
                                }, {
                                    xtype: 'component',
                                    flex: 2
                                }, {
                                    flex: 2,
                                    xtype: 'numericfield',
                                    name: 'QuoteSort',
                                    itemId: 'QuoteSort',
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
                                }, {
                                    xtype: 'component',
                                    flex: 1,
                                }, {
                                    xtype: 'displayfield',
                                    itemId: 'x_FileNum',
                                    name: 'x_FileNum',
                                    fieldLabel: 'Reference Num',
                                    fieldStyle: 'color: #157fcc;font-weight:bold;text-align: right;',
                                    labelStyle: 'text-align: right;',
                                    flex: 2
                                }]
                            }, {
                                xtype: 'fieldcontainer',
                                layout: 'column',
                                items: [
                                {   
                                    xtype: 'fieldcontainer',
                                    layout: 'hbox',
                                    columnWidth: 1,
                                    items:[
                                    {
                                        xtype: 'combo',
                                        flex: 1,
                                        name: 'QuoteVendorKey',
                                        fieldLabel: 'Vendor',
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
                                                if (field.readOnly) return;

                                                var me = field.up('panel');

                                                me.down("field[name=QuoteItemKey]").clearValue();
                                                me.down('#x_ItemName').setValue(null);
                                            },
                                            blur: {
                                                fn: me.onVendorBlur,
                                                scope: me
                                            }
                                        }
                                    }, 
                                    {
                                        xtype: 'button',
                                        margin: '25 0 0 0',
                                        glyph: 0xf1e5, //0xf067,
                                        itemId: 'btnViewVendor',
                                        scale: 'medium',
                                        border: false,
                                        width: 35,
                                        /*cls:'myButton',*/
                                        ui: 'plain',
                                        style: 'background-color:white!important; color:#004A9C !important; font-size: 16px !important;',
                                        iconAlign: 'left',
                                        tooltip: 'Vendor Maintenance',
                                        handler: function(btn) {
                                            var me = btn.up('form');
                                            var VendorKey = me.down('field[name=QuoteVendorKey]').getValue();

                                            if(VendorKey)
                                                me.onViewVendorClick(VendorKey);
                                        }
                                    }]
                                },
                                {
                                    xtype: 'fieldcontainer',
                                    layout: 'hbox',
                                    columnWidth: 1,
                                    items:[
                                    {
                                        xtype: 'combo',
                                        flex: 1,
                                        itemId: 'QuoteItemKey',
                                        name: 'QuoteItemKey',
                                        fieldLabel: 'Item Num',
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
                                                    var me = field.up("form");
                                                    me.down("field[name=x_ItemName]").setValue(records[0].data.x_ItemName);
                                                }
                                            },
                                            blur: {
                                                fn: me.onItemVendorBlur,
                                                scope: me
                                            }
                                        }
                                    }, 
                                    {
                                        xtype: 'button',
                                        margin: '25 0 0 0',
                                        glyph: 0xf1e5, //0xf067,
                                        itemId: 'btnViewItem',
                                        scale: 'medium',
                                        border: false,
                                        width: 35,
                                        ui: 'plain',
                                        style: 'background-color:white!important; color:#004A9C !important; font-size: 16px !important;',
                                        iconAlign: 'left',
                                        tooltip: 'Item Maintenance',
                                        handler: function(btn) {
                                            var me = btn.up('form');
                                            var ItemKey = me.down('field[name=QuoteItemKey]').getValue();

                                            if(ItemKey)
                                                me.onViewItemClick(ItemKey);
                                        }
                                    }]
                                }, {
                                    xtype: 'textfield',
                                    itemId: 'x_ItemName',
                                    fieldLabel: 'Item Name',
                                    name: 'x_ItemName',
                                    columnWidth: 1,
                                    readOnly: true,
                                    editable: false
                                }]
                            }]
                        },
                        // Cost and Pricing Information
                        {
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
                                    name: 'QuoteItemCurrencyCode',
                                    itemId: 'QuoteItemCurrencyCode',
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
                                                var me = field.up("form");
                                                field.next().setValue(records[0].data.CurrencyRate);

                                                me.down("field[name=QuoteItemCost]").currencySymbol = records[0].data.CurrencySymbol;
                                                me.down("field[name=QuoteItemPrice]").currencySymbol = records[0].data.CurrencySymbol;
                                                me.down("field[name=QuoteItemLineCost]").currencySymbol = records[0].data.CurrencySymbol;
                                                me.down("field[name=QuoteItemLinePrice]").currencySymbol = records[0].data.CurrencySymbol;
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
                                                copyField = me.down('#QuoteItemCurrencyRate');
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
                                    name: 'QuoteItemCurrencyRate',
                                    itemId: 'QuoteItemCurrencyRate',
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
                                    name: 'QuoteItemCost',
                                    itemId: 'QuoteItemCost',
                                    hideTrigger: true,
                                    useThousandSeparator: true,
                                    alwaysDisplayDecimals: true,
                                    allowNegative: false,
                                    currencySymbol: '$',
                                    alwaysDecimals: true,
                                    decimalPrecision: 4,
                                    thousandSeparator: ',',
                                    fieldLabel: 'Cost Per Unit',
                                    labelAlign: 'top',
                                    fieldStyle: 'text-align: right;',
                                    allowBlank: false,
                                    listeners: {
                                        change: function(field, newValue, oldValue, eOpts) {
                                            if (document.activeElement.name !== field.name) return;

                                            if (field.value !== null) {
                                                var me = this.up('panel');

                                                profitMargin = parseFloat(me.down('#x_ProfitMargin').getValue());

                                                me.down('field[name=QuoteItemPrice]').setValue(field.value / (1 - (profitMargin / 100)));

                                                var qty = me.down('#QuoteQty').getValue(),
                                                    rate = me.down('#QuoteItemCurrencyRate').getValue();

                                                me.down('#QuoteItemLineCost').setValue(field.value * qty);
                                                me.down('field[name=QuoteItemLineCostInUSD]').setValue(newValue * qty * rate);
                                                me.down('#QuoteItemLinePrice').setValue(me.down('#QuoteItemPrice').getValue() * qty);
                                                me.down('field[name=QuoteItemLinePriceInUSD]').setValue(newValue * qty * rate);
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
                                    decimalPrecision: 1,
                                    alwaysDisplayDecimals: true,
                                    allowNegative: false,
                                    currencySymbol:'%',
                                    alwaysDecimals: true,
                                    thousandSeparator: ',',
                                    labelAlign: 'top',
                                    fieldStyle: 'text-align: right;',
                                    allowBlank: false,
                                    maxValue: 99.99,
                                    labelStyle: 'text-align: center;',
                                    listeners: {
                                        change: function(field, newValue, oldValue, eOpts) {
                                            if (document.activeElement.name !== field.name) return;

                                            if (field.value !== null) {
                                                var me = this.up('panel');
                                                var cost = me.down('#QuoteItemCost').getValue();
                                                var profitMargin = (newValue > 0) ? newValue : 0;
                                                var price = cost;

                                                if (profitMargin > 1) profitMargin = profitMargin / 100;

                                                price = GetMarginPrice(cost, profitMargin);

                                                me.down('#QuoteItemPrice').setValue(price);

                                                var qty = me.down('#QuoteQty').getValue(),
                                                    rate = me.down('#QuoteItemCurrencyRate').getValue();

                                                me.down('#QuoteItemLinePrice').setValue(price * qty);
                                                me.down('field[name=QuoteItemLinePriceInUSD]').setValue(newValue * qty * rate);
                                            }
                                        },
                                        blur: function(field) {
                                            if (field.value !== null && field.value < 1) {
                                                field.value = field.value * 100;
                                            }
                                        }
                                    }
                                }, {
                                    xtype: 'numericfield',
                                    flex: 3,
                                    name: 'QuoteItemPrice',
                                    itemId: 'QuoteItemPrice',
                                    margin: '0 0 0 10',
                                    hideTrigger: true,
                                    useThousandSeparator: true,
                                    decimalPrecision: 4,
                                    alwaysDisplayDecimals: true,
                                    allowNegative: false,
                                    currencySymbol: '$',
                                    alwaysDecimals: true,
                                    thousandSeparator: ',',
                                    fieldLabel: 'Per Unit Price',
                                    labelAlign: 'top',
                                    fieldStyle: 'text-align: right;',
                                    allowBlank: false,
                                    listeners: {
                                        change: function(field, newValue, oldValue, eOpts) {
                                            if (document.activeElement.name !== field.name) return;

                                            if (field.value !== null) {
                                                var me = this.up('panel');
                                                cost = me.down('#QuoteItemCost').getValue();
                                                profitMargin = GetProfitMargin(cost, newValue);
                                                //profitMargin = (1 - (cost / field.value)).toFixed(2) * 100;
                                                me.down('#x_ProfitMargin').setValue(profitMargin);

                                                var qty = me.down('#QuoteQty').getValue(),
                                                    rate = me.down('#QuoteItemCurrencyRate').getValue();


                                                me.down('#QuoteItemLinePrice').setValue(newValue * qty);
                                                me.down('field[name=QuoteItemLinePriceInUSD]').setValue(newValue * qty * rate);
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
                                    name: 'QuoteItemLineCost',
                                    itemId: 'QuoteItemLineCost',
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
                                    name: 'QuoteItemLinePrice',
                                    itemId: 'QuoteItemLinePrice',
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
                            }, {
                                xtype: 'fieldcontainer',
                                layout: {
                                    type: 'hbox'
                                },
                                items: [{
                                    xtype: 'numericfield',
                                    flex: 3,
                                    name: 'QuoteItemLineCostInUSD',
                                    hideTrigger: true,
                                    useThousandSeparator: true,
                                    alwaysDisplayDecimals: true,
                                    allowNegative: false,
                                    currencySymbol: '$',
                                    alwaysDecimals: true,
                                    thousandSeparator: ',',
                                    fieldLabel: 'Total Cost (in $ USD)',
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
                                    name: 'QuoteItemLinePriceInUSD',
                                    margin: '0 0 0 10',
                                    hideTrigger: true,
                                    useThousandSeparator: true,
                                    //decimalPrecision: 5,
                                    alwaysDisplayDecimals: true,
                                    allowNegative: false,
                                    currencySymbol: '$',
                                    alwaysDecimals: true,
                                    thousandSeparator: ',',
                                    fieldLabel: 'Total Price (in $ USD)',
                                    labelAlign: 'top',
                                    fieldStyle: 'text-align: right;',
                                    allowBlank: false,
                                    readOnly: true
                                }]
                            }]
                        },
                        // Grid Totals
                        {
                            xtype: 'fieldset',
                            anchor: '100%',
                            margin: '0 0 0 5',
                            padding: '5 5 5 5',
                            layout: 'column',
                            title: 'Total File Cost and Price',
                            items: [
                                // Grid Totals
                                {
                                    margin: '5 0 5 0',
                                    columnWidth: 1,
                                    title: 'Vendor Total',
                                    xtype: 'gridpanel',
                                    itemId: 'gridtotal',
                                    store: null,
                                    minHeight: 140,
                                    height: 140,
                                    features: [{
                                        ftype: 'summary'
                                    }],
                                    //hideHeaders: true,
                                    columns: [{
                                        xtype:'gridcolumn',
                                        dataIndex:'Vendor',
                                        text: 'Vendor',
                                        flex: 1,
                                        summaryType: 'count',
                                        summaryRenderer: function(value, summaryData, dataIndex) {
                                            return 'Grand Total';
                                        }
                                    },
                                    {
                                        xtype: 'numbercolumn',
                                        flex: 1,
                                        renderer: function(value, metaData, record) {
                                            var returnValue = Ext.util.Format.usMoney(value),
                                                currencySymbol = record.data.CurrencyFormat.substring(0,1);
                                            return returnValue.replace('$', currencySymbol);
                                        },
                                        dataIndex: 'Cost',
                                        text: 'Cost',
                                        align: 'right',
                                        summaryType: 'count',
                                        summaryRenderer: function(value, summaryData, dataIndex) {
                                            var me = this.up('form'),
                                                toolbar = me.down('#FormToolbar'),
                                                parentRecord = toolbar.getCurrentRecord(),
                                                lineCost = 0,
                                                returnValue = Ext.util.Format.usMoney(0);

                                            if(me.down('#GrandLineCost')) 
                                                lineCost = parseFloat(me.down('#GrandLineCost').getValue());

                                            if(!isNaN(lineCost)) {
                                                lineCost = (lineCost / parentRecord.data.FileDefaultCurrencyRate).toFixed(2);
                                                returnValue = Ext.util.Format.usMoney(lineCost);
                                            }
                                            
                                            var currencySymbol = parentRecord.data.FileDefaultCurrencySymbol;

                                            return returnValue.replace('$', currencySymbol);
                                        }
                                    }, {
                                        xtype: 'numbercolumn',
                                        flex: 1,
                                        renderer: function(value, metaData, record) {
                                            var returnValue = Ext.util.Format.usMoney(value),
                                                currencySymbol = record.data.CurrencyFormat.substring(0,1);
                                            return returnValue.replace('$', currencySymbol);
                                        },
                                        dataIndex: 'DiscountAmount',
                                        text: 'Discount',
                                        align: 'right'
                                    }, {
                                        xtype: 'numbercolumn',
                                        flex: 1,
                                        renderer: function(value, metaData, record) {
                                            var returnValue = Ext.util.Format.usMoney(value),
                                                currencySymbol = record.data.CurrencyFormat.substring(0,1);
                                            return returnValue.replace('$', currencySymbol);
                                        },
                                        dataIndex: 'CostAfterDiscount',
                                        text: 'Cost Discount',
                                        align: 'right'
                                    }, {
                                        xtype: 'numbercolumn',
                                        flex: 1,
                                        renderer: function(value, metaData, record) {
                                            var returnValue = Ext.util.Format.usMoney(value),
                                                currencySymbol = record.data.CurrencyFormat.substring(0,1);
                                            return returnValue.replace('$', currencySymbol);
                                        },
                                        dataIndex: 'Price',
                                        text: 'Price',
                                        align: 'right',
                                        summaryType: 'count',
                                        summaryRenderer: function(value, summaryData, dataIndex) {
                                            var me = this.up('form'),
                                                toolbar = me.down('#FormToolbar'),
                                                parentRecord = toolbar.getCurrentRecord(),
                                                linePrice = 0,
                                                returnValue = Ext.util.Format.usMoney(0);

                                            if(me.down("#GrandLinePrice"))
                                                linePrice = parseFloat(me.down('#GrandLinePrice').getValue());

                                            if(!isNaN(linePrice)) {
                                                linePrice = (linePrice / parentRecord.data.FileDefaultCurrencyRate).toFixed(2);
                                                returnValue = Ext.util.Format.usMoney(linePrice);
                                            }
                                            
                                            var currencySymbol = parentRecord.data.FileDefaultCurrencySymbol;

                                            return returnValue.replace('$', currencySymbol);
                                        }
                                    }],
                                    selType: 'rowmodel'
                                },
                                // Grand Total
                                {
                                    xtype:'fieldcontainer',
                                    columnWidth: 1,
                                    layout:'hbox',
                                    hidden: true,
                                    items:[
                                        {
                                            xtype:'component',
                                            flex:1
                                        },
                                        {
                                            margin: '0 0 10 0', 
                                            xtype:'numericfield',
                                            labelAlign:'left',
                                            labelWidth: 80,
                                            fieldLabel:'Grand Total',
                                            itemId:'GrandLinePrice',
                                            fieldStyle: 'text-align: right;',
                                            minValue: 1,
                                            hideTrigger: false,
                                            useThousandSeparator: true,
                                            decimalPrecision: 2,
                                            alwaysDisplayDecimals: true,
                                            allowNegative: false,
                                            currencySymbol: '$',
                                            alwaysDecimals: true,
                                            thousandSeparator: ',',
                                            editable: false,
                                            readOnly: true
                                        },
                                        {
                                            margin: '0 0 10 0', 
                                            xtype:'numericfield',
                                            labelAlign:'left',
                                            labelWidth: 80,
                                            fieldLabel:'Grand Total',
                                            itemId:'GrandLineCost',
                                            fieldStyle: 'text-align: right;',
                                            minValue: 1,
                                            hideTrigger: false,
                                            useThousandSeparator: true,
                                            decimalPrecision: 2,
                                            alwaysDisplayDecimals: true,
                                            allowNegative: false,
                                            currencySymbol: '$',
                                            alwaysDecimals: true,
                                            thousandSeparator: ',',
                                            editable: false,
                                            readOnly: true
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                },
                // Panel Right
                {
                    xtype: 'container',
                    columnWidth: 0.5,
                    margin: '0 0 0 10',
                    layout: {
                        type: 'anchor'
                    },
                    items: [
                        // Weight and Volume
                        {
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
                                    name: 'QuoteItemWeight',
                                    itemId: 'QuoteItemWeight',
                                    fieldLabel: 'Per Unit. Kg',
                                    fieldStyle: 'text-align: right;',
                                    hideTrigger: true,
                                    flex: 3,
                                    listeners: {
                                        change: function(field, newValue, oldValue, eOpts) {
                                            if (document.activeElement.name !== field.name) return;

                                            if (field.value !== null) {
                                                var me = this.up('panel');

                                                var qty = me.down('#QuoteQty').getValue();

                                                me.down('field[name=x_LineWeight]').setValue(field.value * qty);
                                                
                                            }
                                        }
                                    }
                                }, 
                                {
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
                                            value = me.down('field[name=QuoteItemWeight]').getValue();

                                        me.onConvertPoundsClick(value);
                                    }
                                }, {
                                    xtype: 'component',
                                    flex: 1
                                }, {
                                    xtype: 'numberfield',
                                    name: 'QuoteItemVolume',
                                    itemId: 'QuoteItemVolume',
                                    fieldLabel: 'Per Unit m³',
                                    fieldStyle: 'text-align: right;',
                                    hideTrigger: true,
                                    flex: 3,
                                    listeners: {
                                        change: function(field, newValue, oldValue, eOpts) {
                                            if (document.activeElement.name !== field.name) return;

                                            if (field.value !== null) {
                                                var me = this.up('panel');

                                                var qty = me.down('#QuoteQty').getValue();

                                                me.down('field[name=x_LineVolume]').setValue(field.value * qty);
                                                
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
                                            value = me.down('field[name=QuoteItemVolume]').getValue();

                                        me.onConvertCubicFeetsClick(value);
                                    }
                                }]
                            }, {
                                xtype: 'fieldcontainer',
                                layout: {
                                    type: 'hbox'
                                },
                                items: [{
                                    xtype: 'numberfield',
                                    name: 'x_LineWeight',
                                    itemId: 'x_LineWeight',
                                    fieldLabel: 'Total Kg',
                                    fieldStyle: 'text-align: right;',
                                    hideTrigger: true,
                                    flex: 3,
                                    listeners: {
                                        change: function(field, newValue, oldValue, eOpts) {
                                            if (document.activeElement.name !== field.name) return;

                                            if (field.value !== null) {
                                                var me = this.up('panel');

                                                var qty = me.down('#QuoteQty').getValue();

                                                me.down('field[name=QuoteItemWeight]').setValue(field.value / qty);
                                                
                                            }
                                        }
                                    }
                                }, {
                                    xtype: 'component',
                                    width: 35
                                }, {
                                    xtype: 'component',
                                    flex: 1
                                }, {
                                    xtype: 'numberfield',
                                    name: 'x_LineVolume',
                                    itemId: 'x_LineVolume',
                                    fieldLabel: 'Total m³',
                                    fieldStyle: 'text-align: right;',
                                    hideTrigger: true,
                                    flex: 3,
                                    listeners: {
                                        change: function(field, newValue, oldValue, eOpts) {
                                            if (document.activeElement.name !== field.name) return;

                                            if (field.value !== null) {
                                                var me = this.up('panel');

                                                var qty = me.down('#QuoteQty').getValue();

                                                me.down('field[name=QuoteItemVolume]').setValue(field.value / qty);
                                                
                                            }
                                        }
                                    }
                                }, {
                                    xtype: 'component',
                                    width: 35
                                }]
                            }]
                        },
                        // Notes To Customer & Notes To Supplier
                        {
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
                                    xtype: 'checkbox',
                                    name: 'QuoteItemMemoCustomerMoveBottom',
                                    itemId: 'QuoteItemMemoCustomerMoveBottom',
                                    labelSeparator: '',
                                    hideLabel: true,
                                    boxLabel: 'Print note below item',
                                    fieldLabel: 'Print note below item'
                                }, {
                                    xtype: 'textareafield',
                                    name: 'QuoteItemMemoCustomer',
                                    itemId: 'QuoteItemMemoCustomer'
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
                                    xtype: 'checkbox',
                                    name: 'QuoteItemMemoSupplierMoveBottom',
                                    itemId: 'QuoteItemMemoSupplierMoveBottom',
                                    labelSeparator: '',
                                    hideLabel: true,
                                    boxLabel: 'Print note below item',
                                    fieldLabel: 'Print note below item',
                                }, {
                                    xtype: 'textareafield',
                                    anchor: '100%',
                                    name: 'QuoteItemMemoSupplier',
                                    itemId: 'QuoteItemMemoSupplier'
                                }]
                            }]
                        }
                    ]
                }
            ],
            dockedItems: [{
                xtype: 'formtoolbar',
                itemId: 'FormToolbar',
                dock: 'top',
                store: me.storeNavigator,
                navigationEnabled: true,
                addEnabled: false,
                listeners: {
                    addrecord: {
                        fn: me.onAddClick,
                        scope: me
                    },
                    savechanges: {
                        fn: me.onSaveClick,
                        scope: me
                    },
                    deleterecord: {
                        fn: me.onDeleteClick,
                        scope: me
                    },
                    afterloadrecord: {
                        fn: me.onAfterLoadRecord,
                        scope: me
                    }
                }
            }],
            listeners: {
                render: {
                    fn: me.onRenderForm,
                    scope: me
                },
                afterrender: {
                    fn: me.registerKeyBindings,
                    scope: me
                }
            }
        });
        me.callParent(arguments);
    },

    registerKeyBindings: function(view, options) {
        var me = this,
            btnViewVendorId = me.down('#btnViewVendor').getEl().id,
            btnViewItemId = me.down('#btnViewItem').getEl().id;

        Ext.EventManager.on(view.getEl(), 'keyup', function(evt, t, o) {

                if (evt.ctrlKey && evt.keyCode === Ext.EventObject.F8) { 
                    evt.stopEvent();
                    var toolbar = me.down('#FormToolbar');
                    if(toolbar.isEditing) {
                        var btn = toolbar.down('#save');
                        btn.fireEvent('click');
                    }
                }

                if (evt.keyCode === Ext.EventObject.TAB && evt.target.id === btnViewVendorId) {
                    me.down("field[name=QuoteItemKey]").focus(true, 500);
                }

                if (evt.keyCode === Ext.EventObject.TAB && evt.target.id === btnViewItemId) {
                    me.down("field[name=QuoteItemCurrencyCode]").focus(true, 500);
                }
            },
            this);
    },

    onAfterLoadRecord: function(tool, record) {
        var me = this;
        var fieldVendor = me.down('field[name=QuoteVendorKey]');

        if (record.phantom) {
            me.down('#gridtotal').store.removeAll();
            fieldVendor.setReadOnly(false);
            return;
        }

        fieldVendor.setReadOnly(true);

        var curRec = record;

        Ext.Msg.wait('Loading Data', 'Wait');

        var storeVendors = new CBH.store.vendors.Vendors().load({
            params: {
                id: record.data.QuoteVendorKey,
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function() {
                var me = fieldVendor.up('panel');
                var curVendor = this.getAt(0);
                me.down('field[name=QuoteVendorKey]').bindStore(storeVendors);
                me.down('field[name=QuoteVendorKey]').setValue(curVendor.data.VendorKey);

                var filterVendor = new Ext.util.Filter({
                    property: 'FVVendorKey',
                    value: curVendor.data.VendorKey
                });

                var storeLastQuoteMargin = new CBH.store.vendors.LastQuoteMargin().load({
                    filters: [filterVendor],
                    callback: function() {
                        me.storeLastQuoteMargin = this;
                        var profit = 0,
                            cost = curRec.data.QuoteItemCost,
                            price = curRec.data.QuoteItemPrice;

                        /*profit = (price && price > 0) ?
                            GetProfitMargin(cost, price) :
                            this.getAt(0).data.FVProfitMargin;*/

                        //profit = GetProfitMargin(cost, price).to

                        //me.down('field[name=x_ProfitMargin]').setValue(profit);

                        filterVendor.property = 'ItemVendorKey';

                        var storeItems = new CBH.store.vendors.Items().load({
                            params: {
                                id: curRec.data.QuoteItemKey,
                                page: 0,
                                start: 0,
                                limit: 0
                            },
                            filters: [filterVendor],
                            callback: function() {
                                var itemField = me.down('#QuoteItemKey');
                                itemField.bindStore(storeItems);
                                itemField.filters = filterVendor;

                                if (!curRec.dirty) {
                                    itemField.setValue(storeItems.getAt(0).data.ItemKey);
                                    me.down("field[name=x_ItemName]").setValue(storeItems.getAt(0).data.x_ItemName);
                                    me.down('field[name=QuoteVendorKey]').setReadOnly(true);
                                    me.down('#QuoteItemCurrencyCode').setReadOnly(true);
                                } else {
                                    itemField.setReadOnly(false);
                                    me.down('field[name=QuoteVendorKey]').setReadOnly(false);
                                    me.down('#QuoteItemCurrencyCode').setReadOnly(false);
                                }
                                storeItems.lastOptions.callback = null;
                                Ext.Msg.hide();
                            }
                        });

                        storeLastQuoteMargin.lastOptions.callback = null;
                    }
                });
            }
        });

        var storeGrandT = new CBH.store.sales.qfrmFileQuoteDetailsSub().load({
            params: {
                FileKey: curRec.data.QuoteFileKey,
                page: 0,
                limit: 0,
                start: 0
            },
            callback: function(records, operation, success) {
                var lineCost = records[0] ? parseFloat(this.sum('LineCost')) : 0,
                    linePrice = records[0] ? parseFloat(this.sum('LinePrice')) : 0;

                me.down('#GrandLineCost').setValue(lineCost);
                me.down('#GrandLinePrice').setValue(linePrice);

                var storeTotals = new CBH.store.sales.qryFileQuoteVendorSummaryWithDiscount().load({
                    params: {
                        FileKey: curRec.data.QuoteFileKey,
                        VendorKey: curRec.data.QuoteVendorKey,
                        page: 0,
                        limit: 0,
                        start: 0
                    },
                    callback: function(records, operation, success) {
                        var grid = me.down('#gridtotal');

                        grid.reconfigure(this);

                        var storeCR = me.down("#QuoteItemCurrencyCode").store,
                            index = storeCR.find('CurrencyCode', record.data.QuoteItemCurrencyCode),
                            currencySymbol = storeCR.getAt(index).data.CurrencySymbol;

                        me.down('#QuoteItemCost').currencySymbol = currencySymbol;
                        me.down('#QuoteItemPrice').currencySymbol = currencySymbol;
                        me.down('#QuoteItemLineCost').currencySymbol = currencySymbol;
                        me.down('#QuoteItemLinePrice').currencySymbol = currencySymbol;

                        me.down('#QuoteItemCost').setValue(record.data.QuoteItemCost);
                        me.down('#QuoteItemPrice').setValue(record.data.QuoteItemPrice);
                        me.down('#QuoteItemLineCost').setValue(record.data.QuoteItemLineCost);
                        me.down('#QuoteItemLinePrice').setValue(record.data.QuoteItemLinePrice);
                    }
                });
            }
        });
    },

    onRenderForm: function() {
        //this.down('#QuoteQty').focus(true, 200);
    },

    onAddClick: function(toolbar, record) {

        var me = this;

        record.data.QuoteFileKey = me.FileKey;
        record.data.x_FileNum = me.FileNum;
        //record.data.QuoteSort = (toolbar.store.getCount() + 1) * 100;
        record.data.QuoteQty = 1;

        return true;
    },

    onSaveClick: function(toolbar, record) {

        var me = this;
        var editform = me.getForm();

        if (!editform.isValid()) {
            Ext.Msg.alert("Validation", "Check data for valid input!!!");
            return false;
        }

        editform.updateRecord();

        var savedRecord = editform.getRecord();

        Ext.Msg.wait('Saving Record...', 'Wait');

        savedRecord.save({
            success: function(e) {
                Ext.Msg.hide();
                toolbar.doRefresh();
            },
            failure: function() {
                Ext.Msg.hide();
            }
        });
    },

    onDeleteClick: function(toolbar, record) {
        if (record) {
            var curRec = record.index - 1;
            curPage = toolbar.store.currentPage;
            prevRec = (curRec <= 0) ? 1 : curRec;

            Ext.Msg.show({
                title: 'Delete',
                msg: 'Do you want to delete?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(btn) {
                    if (btn === "yes") {
                        Ext.Msg.wait('Deleting Record...', 'Wait');
                        record.destroy({
                            success: function() {
                                var lastOpt = toolbar.store.lastOptions,
                                    params = (lastOpt) ? lastOpt.params : null;

                                toolbar.store.reload({
                                    params: params,
                                    callback: function() {

                                        if (toolbar.store.getCount() > 0) {
                                            toolbar.gotoAt(prevRec);
                                        } else {
                                            toolbar.up('form').up('panel').destroy();
                                        }

                                        Ext.Msg.hide();

                                        this.lastOptions.callback = null;
                                    }
                                });
                            },
                            failure: function() {
                                Ext.Msg.hide();
                            }
                        });
                    }
                }
            }).defaultButton = 2;
        }
    },

    onDestroy: function(e, eOpts) {
        //---
    },

    setProfitMargin: function(valueMargin) {
        var me = this;

        var profitMargin = valueMargin;

        var cost = me.down('#QuoteItemCost').getValue();
        var price = me.down('#QuoteItemPrice').getValue();
        var qty = me.down('#QuoteQty').getValue();
        //var price = GetMarginPrice(cost, profitMargin);
        var linecost = cost * qty;
        var lineprice = price * qty;

        var itemWeight = me.down('#QuoteItemWeight').getValue();
        var itemVolume = me.down('#QuoteItemVolume').getValue();

        var lineWeight = itemWeight * qty;
        var lineVolume = itemVolume * qty;

        me.down('#x_LineWeight').setValue(lineWeight);
        me.down('#x_LineVolume').setValue(lineVolume);

        me.down('#x_ProfitMargin').setValue(profitMargin);
        me.down('#QuoteItemPrice').setValue(price);
        me.down('#QuoteItemLineCost').setValue(linecost);
        me.down('#QuoteItemLinePrice').setValue(lineprice);
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
                            me.down("field[name=QuoteItemKey]").bindStore(storeItems);
                            me.down("field[name=QuoteItemKey]").filters = filterVendor;
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
            width: 680,
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
            forceFit: true,
            constrain: true
        });

        form.center().show();

        var btn = form.down('#FormToolbar').down('#add');
        btn.fireEvent('click', btn, null, null, model);
    },


    onItemVendorBlur: function(field, The, eOpts) {
        var me = this;

        if (field && field.valueModels !== null && field.valueModels.length) {

            var selectedItem = field.valueModels[0].data;

            me.down("field[name=x_ItemName]").setValue(selectedItem.x_ItemName);

            var cost = selectedItem.ItemCost,
                price = selectedItem.ItemPrice;

            me.down('#QuoteItemCost').currencySymbol = selectedItem.ItemCurrencySymbol;
            me.down('#QuoteItemPrice').currencySymbol = selectedItem.ItemCurrencySymbol;
            me.down('#QuoteItemLineCost').currencySymbol = selectedItem.ItemCurrencySymbol;
            me.down('#QuoteItemLinePrice').currencySymbol = selectedItem.ItemCurrencySymbol;

            me.down('#QuoteItemCost').setValue(cost);
            me.down('#QuoteItemPrice').setValue(price);

            me.down('#QuoteItemWeight').setValue(selectedItem.ItemWeight);
            me.down('#QuoteItemVolume').setValue(selectedItem.ItemVolume);

            me.down('#QuoteItemCurrencyCode').setValue(selectedItem.ItemCurrencyCode); //'USD'
            me.down('#QuoteItemCurrencyRate').setValue(selectedItem.ItemCurrencyRate);

            var rate = selectedItem.ItemCurrencyRate,
                qty = me.down('#QuoteQty').getValue();

            me.down('field[name=QuoteItemLineCostInUSD]').setValue(cost * qty * rate);
            me.down('field[name=QuoteItemLinePriceInUSD]').setValue(price * qty * rate);


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
        } else {
            var rawvalue = field.getRawValue(),
                vendorKey = me.down('field[name=QuoteVendorKey]').value;

            if (!String.isNullOrEmpty(rawvalue) && vendorKey !== null) {
                Ext.Msg.show({
                    title: 'Question',
                    msg: 'The item doesn\'t exists, Do you want to add to database?',
                    buttons: Ext.Msg.YESNO,
                    icon: Ext.Msg.QUESTION,
                    fn: function(e) {
                        if (e === "yes") {
                            me.addVendorItem(rawvalue);
                        } else {
                            field.setValue(null);
                            field.focus(true, 200);
                        }
                    }
                });
            }
        }
    },

    addVendorItem: function(value) {
        var me = this,
            tabs = me.up('app_pageframe');

        var vendorkey = me.down('field[name=QuoteVendorKey]').getValue();
        var storeToNavigate = new CBH.store.vendors.Items({
            autoLoad: false
        });
        model = Ext.create('CBH.model.vendors.Items', {
            ItemNum: value,
            ItemVendorKey: vendorkey
        });

        var form = Ext.widget('items', {
            storeNavigator: storeToNavigate,
            modal: true,
            width: 680,
            frameHeader: true,
            header: true,
            title: 'New Item',
            bodyPadding: 10,
            closable: true,
            stateful: false,
            floating: true,
            callerForm: me,
            constrain: true
        });

        form.center().show();

        var btn = form.down('#FormToolbar').down('#add');
        btn.fireEvent('click', btn, null, null, model);
    },

    onViewVendorClick: function(VendorKey) {
        var me = this,
            tabs = me.up('app_pageframe');

        me.getEl().mask("Loading Vendor...");
        storeToNavigate = new CBH.store.vendors.Vendors().load({
            params: {
                id: VendorKey
            },
            callback: function(records) {
                var form = Ext.widget('vendors', {
                    storeNavigator: storeToNavigate,
                    callerForm: me
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Vendor Maintenance',
                    items: [form],
                });

                //This javascript replaces all 3 types of line breaks with a space
                if(records[0].data.VendorAddress1) 
                    records[0].data.VendorAddress1 = records[0].data.VendorAddress1.replace(/(\r\n|\n|\r)/gm," ");
                if(records[0].data.VendorAddress2)
                    records[0].data.VendorAddress2 = records[0].data.VendorAddress2.replace(/(\r\n|\n|\r)/gm," ");
                
                form.down('#FormToolbar').gotoAt(1);
                tab.show();
                me.getEl().unmask();
            }
        });
    },

    onViewItemClick: function(ItemKey) {
        var me = this;
        var tabs = this.up('app_pageframe');

        me.getEl().mask("Loading Item...");
        storeToNavigate = new CBH.store.vendors.Items().load({
            params: {
                id: ItemKey
            },
            callback: function() {
                var form = Ext.widget('items', {
                    storeNavigator: storeToNavigate,
                    ItemKey: ItemKey,
                    fromLineEntry: true
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Item Maintenance',
                    items: [form]
                });

                form.down('#FormToolbar').gotoAt(1);
                tab.show();
                me.getEl().unmask();
            }
        });
    },

    onConvertPoundsClick: function(value) {
        var me = this,
            toolbar = me.down('#FormToolbar');

        if(!toolbar.isEditing)
            toolbar.down("#edit").fireEvent('click');

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
        me.down("field[name=QuoteItemWeight]").setValue(data.value);
        me.down("field[name=QuoteItemWeight]").focus(true, 200);
        var qty = me.down('#QuoteQty').getValue();
        me.down('field[name=x_LineWeight]').setValue(data.value * qty);
    },

    onConvertCubicFeetsClick: function(value) {
        var me = this,
            toolbar = me.down('#FormToolbar');

        if(!toolbar.isEditing)
            toolbar.down("#edit").fireEvent('click');

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
        me.down("field[name=QuoteItemVolume]").setValue(data.value);
        me.down("field[name=QuoteItemVolume]").focus(true, 200);
        var qty = me.down('#QuoteQty').getValue();
        me.down('field[name=x_LineVolume]').setValue(data.value * qty);
    }
});