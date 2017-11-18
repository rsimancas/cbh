Ext.define('CBH.view.sales.FileQuoteEntry', {
    extend: 'Ext.form.Panel',
    alias: 'widget.filequoteentry',
    layout: {
        type: 'column'
    },
    bodyPadding: 5,
    FileKey: 0,
    frameHeader: false,
    header: false,
    enableKeyEvents: true,
    storeNavigator: null,

    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;

        var me = this;

        var storeCurrencyRates = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        var storeCurrencyRatesDiscount = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        var storeShipmentTypes = new CBH.store.common.ShipmentTypes().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        storePaymentTerms = new CBH.store.common.PaymentTerms().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        storeFreightDestinations = new CBH.store.common.FreightDestinations().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        storeQuoteNum = new CBH.store.sales.FileQuoteHeader().load({
            params: {
                page: 0,
                start: 0,
                limit: 0,
                FileKey: me.FileKey
            }
        });

        storeVendor = null;
        storeQuotes = null;

        storeVendorContacts = null;
        storeWarehouseType = null;


        Ext.applyIf(me, {
            fieldDefaults: {
                labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items: [{
                xtype: 'combo',
                fieldLabel: 'Current Vendor',
                name: 'FVVendorKey',
                displayField: 'VendorName',
                valueField: 'VendorKey',
                enableKeyEvents: true,
                forceSelection: true,
                queryMode: 'local',
                selectOnFocus: true,
                emptyText: 'Choose Vendor',
                allowBlank: false,
                store: storeVendor,
                readOnly: true,
                columnWidth: 0.4,
                anyMatch: true
            }, {
                margin: '0 0 0 5',
                xtype: 'textfield',
                fieldLabel: 'Customer',
                readOnly: true,
                value: me.Customer,
                columnWidth: 0.4
            }, {
                margin: '0 0 0 5',
                xtype: 'textfield',
                fieldLabel: 'Reference Num',
                readOnly: true,
                value: me.FileNum,
                columnWidth: 0.2
            }, {
                margin: '10 0 10 0',
                columnWidth: 0.5,
                xtype: 'fieldset',
                layout: 'column',
                bodyPadding: 5,
                items: [{
                    columnWidth: 0.3,
                    xtype: 'combo',
                    name: 'FVQHdrKey',
                    fieldLabel: 'Quote Number',
                    valueField: 'QHdrKey',
                    displayField: 'QuoteNum',
                    store: storeQuoteNum,
                    queryMode: 'local',
                    typeAhead: false,
                    minChars: 2,
                    forceSelection: false,
                    anyMatch: true,
                }, {
                    margin: '0 0 0 5',
                    xtype: 'numericfield',
                    fieldLabel: 'Profit Margin (%)',
                    name: 'FVProfitMargin',
                    columnWidth: 0.3,
                    format: '00.00',
                    hideTrigger: true,
                    fieldStyle: 'text-align: right;'
                }, {
                    margin: '0 0 0 5',
                    columnWidth: 0.4,
                    xtype: 'combo',
                    fieldLabel: 'PO Currency',
                    name: 'FVPOCurrencyCode',
                    store: storeCurrencyRates,
                    labelWidth: 50,
                    listConfig: {
                        minWidth: null
                    },
                    valueField: 'CurrencyCode',
                    displayField: 'CurrencyCodeDesc',
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 2,
                    //allowBlank: false,
                    forceSelection: true,
                    tpl: Ext.create('Ext.XTemplate',
                        '<tpl for=".">',
                        '<div class="x-boundlist-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                        '</tpl>'),
                    anyMatch: true
                }, {
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    columnWidth: 0.5,
                    items:[{
                        flex: 1,
                        xtype: 'combo',
                        name: 'FVVendorContactKey',
                        fieldLabel: 'Vendor Contact',
                        labelWidth: 50,
                        listConfig: {
                            minWidth: null
                        },
                        anchor: '100%',
                        valueField: 'ContactKey',
                        displayField: 'x_ContactFullName',
                        store: storeVendorContacts,
                        queryMode: 'local',
                        typeAhead: false,
                        minChars: 2,
                        //allowBlank: false,
                        forceSelection: true,
                        anyMatch: true
                    }, {
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
                        var VendorKey = me.down('field[name=FVVendorKey]').getValue();

                        if(VendorKey)
                            me.onViewVendorClick(VendorKey);
                    }
                }]
            }, {
                    margin: '0 0 0 5',
                    columnWidth: 0.5,
                    xtype: 'combo',
                    name: 'FVPaymentTerms',
                    fieldLabel: 'Payment Terms',
                    labelWidth: 50,
                    listConfig: {
                        minWidth: null
                    },
                    anchor: '100%',
                    valueField: 'TermKey',
                    displayField: 'x_Description',
                    store: storePaymentTerms,
                    queryMode: 'local',
                    typeAhead: false,
                    minChars: 2,
                    forceSelection: true,
                    anyMatch: true
                }, {
                    xtype: 'fieldcontainer',
                    columnWidth: 0.2,
                    layout: 'hbox',
                    items: [{
                        flex: 1,
                        xtype: 'numericfield',
                        fieldLabel: 'Total Weight (kg)',
                        name: 'FVTotalWeight',
                        minValue: 0,
                        hideTrigger: true,
                        useThousandSeparator: true,
                        decimalPrecision: 2,
                        alwaysDisplayDecimals: true,
                        allowNegative: false,
                        alwaysDecimals: true,
                        thousandSeparator: ',',
                        fieldStyle: 'text-align: right;'
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
                                value = me.down('field[name=FVTotalWeight]').getValue();

                            me.onConvertPoundsClick(value);
                        }
                    }] 
                }, {
                    xtype: 'fieldcontainer',
                    columnWidth: 0.2,
                    layout: 'hbox',
                    items: [{
                        flex: 1,
                        margin: '0 0 0 5',
                        xtype: 'numericfield',
                        fieldLabel: 'Total Volume (m³)',
                        name: 'FVTotalVolume',
                        minValue: 0,
                        hideTrigger: true,
                        useThousandSeparator: true,
                        decimalPrecision: 2,
                        alwaysDisplayDecimals: true,
                        allowNegative: false,
                        alwaysDecimals: true,
                        thousandSeparator: ',',
                        fieldStyle: 'text-align: right;'
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
                                value = me.down('field[name=FVTotalVolume]').getValue();

                            me.onConvertCubicFeetsClick(value);
                        }
                    }]
                }, {
                    margin: '0 0 0 5',
                    columnWidth: 0.3,
                    xtype: 'numericfield',
                    name: 'FVFreightCost',
                    fieldLabel: 'Freight Cost',
                    minValue: 0,
                    hideTrigger: true,
                    currencySymbol: '$',
                    useThousandSeparator: true,
                    decimalPrecision: 2,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: true,
                    thousandSeparator: ',',
                    fieldStyle: 'text-align: right;'
                }, {
                    margin: '0 0 0 5',
                    columnWidth: 0.3,
                    xtype: 'numericfield',
                    name: 'FVFreightPrice',
                    fieldLabel: 'Freight Price',
                    minValue: 0,
                    hideTrigger: true,
                    currencySymbol: '$',
                    useThousandSeparator: true,
                    decimalPrecision: 2,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: true,
                    thousandSeparator: ',',
                    fieldStyle: 'text-align: right;'
                }, {
                    margin: '0 0 10 0',
                    columnWidth: 1,
                    xtype: 'textfield',
                    fieldLabel: 'Lead Time',
                    name: 'FVLeadTime'
                }]
            }, {
                columnWidth: 0.5,
                margin: '10 0 0 5',
                xtype: 'fieldset',
                layout: 'column',
                items: [{
                    columnWidth: 0.5,
                    xtype: 'combo',
                    name: 'FVFreightShipmentType',
                    fieldLabel: 'Shipment Type',
                    listConfig: {
                        minWidth: null
                    },
                    valueField: 'ShipTypeExpression',
                    displayField: 'ShipTypeText',
                    store: storeShipmentTypes,
                    queryMode: 'local',
                    typeAhead: false,
                    minChars: 2,
                    //allowBlank: false,
                    forceSelection: true,
                    anyMatch: true
                }, {
                    margin: '0 0 0 5',
                    columnWidth: 0.5,
                    xtype: 'combo',
                    name: 'FVFreightDestination',
                    fieldLabel: 'Freight Destination',
                    listConfig: {
                        minWidth: null
                    },
                    valueField: 'DestinationKey',
                    displayField: 'Destination',
                    store: storeFreightDestinations,
                    queryMode: 'local',
                    typeAhead: false,
                    minChars: 2,
                    //allowBlank: false,
                    forceSelection: true,
                    anyMatch: true
                }, {
                    columnWidth: 0.5,
                    xtype: 'combo',
                    name: 'FVWarehouseKey',
                    fieldLabel: 'Warehouse Key',
                    listConfig: {
                        minWidth: null
                    },
                    valueField: 'WarehouseKey',
                    displayField: 'CarrierWarehouse',
                    store: storeWarehouseType,
                    queryMode: 'local',
                    typeAhead: false,
                    minChars: 2,
                    //allowBlank: false,
                    forceSelection: false,
                    anyMatch: true
                }, {
                    margin: '0 0 0 5',
                    columnWidth: 0.5,
                    xtype: 'textfield',
                    name: 'FVFreightDestinationZip',
                    fieldLabel: 'Destination Zip'
                }, {
                    columnWidth: 0.3,
                    xtype: 'numericfield',
                    fieldLabel: 'Discount Percent (%)',
                    name: 'FVDiscountPercent',
                    minValue: 0,
                    hideTrigger: true,
                    useThousandSeparator: true,
                    decimalPrecision: 2,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: true,
                    thousandSeparator: ',',
                    fieldStyle: 'text-align: right;'
                }, {
                    margin: '0 0 0 5',
                    columnWidth: 0.3,
                    xtype: 'numericfield',
                    fieldLabel: 'and / or Discount',
                    name: 'FVDiscount',
                    minValue: 0,
                    hideTrigger: true,
                    currencySymbol: '$',
                    useThousandSeparator: true,
                    decimalPrecision: 2,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: true,
                    thousandSeparator: ',',
                    fieldStyle: 'text-align: right;'
                }, {
                    margin: '0 0 10 5',
                    columnWidth: 0.4,
                    xtype: 'combo',
                    fieldLabel: 'Discount Currency',
                    name: 'FVDiscountCurrencyCode',
                    store: storeCurrencyRatesDiscount,
                    labelWidth: 50,
                    listConfig: {
                        minWidth: null
                    },
                    valueField: 'CurrencyCode',
                    displayField: 'CurrencyCodeDesc',
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 2,
                    //allowBlank: false,
                    forceSelection: true,
                    tpl: Ext.create('Ext.XTemplate',
                        '<tpl for=".">',
                        '<div class="x-boundlist-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                        '</tpl>'),
                    anyMatch: true
                }]
            }, {
                columnWidth: 1,
                xtype: 'gridpanel',
                title: 'Details',
                itemId: 'gridorderentry',
                store: storeQuotes,
                minHeight: 350,
                features: [{
                    ftype: 'summary'
                }],
                columns: [{
                    xtype: 'numbercolumn',
                    width: 60,
                    dataIndex: 'QuoteSort',
                    text: 'Sort',
                    format: '00,000'
                }, {
                    xtype: 'numbercolumn',
                    width: 70,
                    dataIndex: 'QuoteQty',
                    text: 'Qty.',
                    align: 'right',
                    format: '00,000'
                }, {
                    xtype: 'gridcolumn',
                    width: 100,
                    dataIndex: 'x_VendorName',
                    text: 'Vendor'
                }, {
                    xtype: 'gridcolumn',
                    width: 75,
                    dataIndex: 'x_ItemNum',
                    text: 'Item Num.'
                }, {
                    xtype: 'gridcolumn',
                    flex: 1,
                    dataIndex: 'x_ItemName',
                    text: 'Description',
                    totalsText: 'First column is sum, second max',
                    summaryType: 'count',
                    summaryRenderer: function(value, summaryData, dataIndex) {
                        return Ext.String.format('Total for {0} items', value, value !== 1 ? value : 0);
                    }
                }, {
                    xtype: 'numbercolumn',
                    width: 68,
                    text: 'Profit (%)',
                    align: 'right',
                    dataIndex: 'x_ProfitMargin'
                }, {
                    xtype: 'gridcolumn',
                    text: 'Total Cost',
                    align: 'right',
                    dataIndex: 'QuoteItemLineCost',
                    renderer: Ext.util.Format.usMoney,
                    summaryType: 'sum'
                }, {
                    xtype: 'gridcolumn',
                    text: 'Weight (kg)',
                    dataIndex: 'QuoteItemWeight',
                    format: '0,000.00',
                    align: 'right',
                    summaryType: 'sum'
                }, {
                    xtype: 'gridcolumn',
                    text: 'Volume (m³)',
                    dataIndex: 'QuoteItemVolume',
                    format: '0,000.00',
                    align: 'right',
                    summaryType: 'sum'
                }, {
                    xtype: 'gridcolumn',
                    text: 'Total Price',
                    align: 'right',
                    dataIndex: 'QuoteItemLinePrice',
                    renderer: Ext.util.Format.usMoney,
                    summaryType: 'sum'
                }, {
                    width: 75,
                    xtype: 'gridcolumn',
                    text: 'CUR',
                    dataIndex: 'QuoteItemCurrencyCode'
                }, {
                    xtype: 'actioncolumn',
                    draggable: false,
                    width: 35,
                    resizable: false,
                    hideable: false,
                    stopSelection: false,
                    items: [{
                        handler: function(view, rowIndex, colIndex, item, e, record, row) {
                            var tabs = this.up('app_pageframe');

                            var storeToNavigate = new CBH.store.sales.FileQuoteDetail().load({
                                params: {
                                    filekey: me.FileKey,
                                    vendorkey: record.data.QuoteVendorKey
                                },
                                callback: function() {
                                    var form = Ext.widget('filelineentry', {
                                        storeNavigator: this,
                                        VendorKey: record.data.QuoteVendorKey,
                                        ItemKey: record.data.QuoteItemKey
                                    });


                                    var tab = tabs.add({
                                        closable: true,
                                        iconCls: 'tabs',
                                        autoScroll: true,
                                        title: 'Line Entry',
                                        items: [form]
                                    });

                                    form.down('#FormToolbar').gotoAt(record.index + 1);

                                    tab.show();
                                }
                            });
                        },
                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                        tootip: 'view line detail'
                    }]
                }],
                tbar: [{
                    text: 'Add',
                    hidden: true,
                    handler: function() {
                        var tabs = Ext.getCmp('cbh_pageframe'),
                            grid = me.down('gridpanel');

                        var model = Ext.create('CBH.model.sales.FileQuoteDetail', {
                            QuoteFileKey: me.FileKey,
                            QuoteVendorKey: 0,
                            x_FileNum: me.FileNum
                        });

                        var storeToNavigate = new CBH.store.sales.FileQuoteDetail();

                        var form = Ext.widget('filelineentry', {
                            storeNavigator: storeToNavigate,
                            FileKey: me.FileKey,
                            FileNum: me.FileNum
                        });

                        var tab = tabs.add({
                            closable: true,
                            iconCls: 'tabs',
                            autoScroll: true,
                            title: 'New Line Entry',
                            items: [form]
                        });

                        //form.down('#FormToolbar').gotoAt(1);
                        tab.show();

                        var btn = form.down('#FormToolbar').down('#add');
                        btn.fireEvent('click', btn, null, null, model);
                    }
                }, {
                    itemId: 'deleteline',
                    text: 'Delete',
                    hidden: true,
                    handler: function() {
                        var grid = this.up('gridpanel');
                        var sm = grid.getSelectionModel();

                        selection = sm.getSelection();

                        if (selection) {
                            Ext.Msg.show({
                                title: 'Delete',
                                msg: 'Do you want to delete?',
                                buttons: Ext.Msg.YESNO,
                                icon: Ext.Msg.QUESTION,
                                fn: function(btn) {
                                    if (btn === "yes") {
                                        selection[0].destroy();
                                    }
                                }
                            }).defaultButton = 2;
                        }
                    },
                    disabled: true
                }],
                selType: 'rowmodel',
                listeners: {
                    selectionchange: function(view, records) {
                        this.down('#deleteline').setDisabled(!records.length);
                    },
                    validateedit: {
                        fn: me.onValidateEdit,
                        scope: me
                    },
                    /*viewready: {
                        fn: me.onGridViewReady,
                        scope: me
                    },*/
                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                        var tabs = this.up('app_pageframe');

                        var storeToNavigate = new CBH.store.sales.FileQuoteDetail().load({
                            params: {
                                filekey: record.data.QuoteFileKey
                            },
                            callback: function() {
                                var form = Ext.widget('filelineentry', {
                                    storeNavigator: this,
                                    VendorKey: record.data.QuoteVendorKey,
                                    ItemKey: record.data.QuoteItemKey
                                });


                                var tab = tabs.add({
                                    closable: true,
                                    iconCls: 'tabs',
                                    autoScroll: true,
                                    title: 'Line Entry',
                                    items: [form]
                                });

                                form.down('#FormToolbar').gotoAt(record.index + 1);

                                tab.show();
                            }
                        });

                    },
                    viewready: function(grid) {
                        var view = grid.view;

                        // record the current cellIndex
                        grid.mon(view, {
                            uievent: function(type, view, cell, recordIndex, cellIndex, e) {
                                grid.cellIndex = cellIndex;
                                grid.recordIndex = recordIndex;
                            }
                        });

                        grid.tip = Ext.create('Ext.tip.ToolTip', {
                            target: view.el,
                            delegate: '.x-grid-cell',
                            trackMouse: true,
                            renderTo: Ext.getBody(),
                            listeners: {
                                beforeshow: function updateTipBody(tip) {
                                    if (!Ext.isEmpty(grid.cellIndex) && grid.cellIndex !== -1) {
                                        header = grid.headerCt.getGridColumns()[grid.cellIndex];
                                        tip.update(grid.getStore().getAt(grid.recordIndex).get(header.dataIndex));
                                    }
                                }
                            }
                        });

                    }
                }
            }],
            dockedItems: [{
                xtype: 'formtoolbar',
                itemId: 'FormToolbar',
                dock: 'top',
                store: me.storeNavigator,
                addEnabled: false,
                navigationEnabled: true,
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
            }, {
                xtype: 'toolbar',
                dock: 'bottom',
                ui: 'footer',
                items: [{
                    xtype: 'textfield',
                    name: 'FileCreatedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created By'
                }, {
                    xtype: 'datefield',
                    name: 'FileCreatedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created Date',
                    hideTrigger: true
                }, {
                    xtype: 'textfield',
                    name: 'FileModifiedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified By'
                }, {
                    xtype: 'datefield',
                    name: 'FileModifiedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified Date',
                    hideTrigger: true
                }, {
                    xtype: 'component',
                    flex: 1
                }, {
                    xtype: 'fieldcontainer',
                    layout: 'column',
                    items: [{
                        xtype: 'button',
                        columnWidth: 0.5,
                        text: 'RFQ -> Excel',
                        disabled: true
                    }, {
                        margin: '0 10 0 5',
                        xtype: 'button',
                        columnWidth: 0.5,
                        text: 'RFQ -> Web Url',
                        disabled: true
                    }, {
                        xtype: 'button',
                        columnWidth: 0.5,
                        text: 'Import Costs From Excel',
                        disabled: true
                    }]
                }]
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
        var me = this;
        Ext.EventManager.on(view.getEl(), 'keyup', function(evt, t, o) {
                if (evt.ctrlKey && evt.keyCode === Ext.EventObject.F8) { 
                    evt.stopEvent();
                    var toolbar = me.down('#FormToolbar');
                    if(toolbar.isEditing) {
                        var btn = toolbar.down('#save');
                        btn.fireEvent('click');
                    }
                }
            },
            this);
    },

    onRenderForm: function() {
        var me = this;
        /*var toolbar = me.down('#FormToolbar');

        toolbar.down('#add').setVisible(false);
        //toolbar.down('#delete').setVisible(false);

        if (toolbar.store.getCount() === 1 && toolbar.store.getAt(0).phantom) {
            toolbar.items.items.forEach(function(btn) {
                btn.setVisible(false);
            });
            toolbar.down('#save').setVisible(true);
        }*/

        //var field = me.down('field[name=VendorName]');
        //field.focus(true, 100);
    },

    onAfterLoadRecord: function(tool, record) {
        var me = this;

        vendorkey = 0;
        vendorcontactkey = 0;
        warehousekey = 0;
        if (!record.phantom) {
            vendorkey = record.data.FVVendorKey;
            vendorcontactkey = record.data.FVVendorContactKey;
            warehousekey = record.data.FVWarehouseKey;
        }

        var toolbar = me.down('#FormToolbar');
        var btn = toolbar.down('#add');
        if(btn)
            btn.setVisible(false);

        Ext.Msg.wait('Loading data...', 'Wait');

        var storeFile = new CBH.store.sales.FileHeader().load({
            params: {
                id: record.data.FVFileKey
            },
            callback: function(records, operation, success) {
                if (success && records[0]) {
                    me.down('field[name=FileCreatedBy]').setValue(records[0].data.FileCreatedBy);
                    me.down('field[name=FileCreatedDate]').setValue(records[0].data.FileCreatedDate);
                    me.down('field[name=FileModifiedBy]').setValue(records[0].data.FileModifiedBy);
                    me.down('field[name=FileModifiedDate]').setValue(records[0].data.FileModifiedDate);
                }
            }
        });

        var storeVendor = new CBH.store.vendors.Vendors().load({
            params: {
                id: vendorkey
            },
            callback: function() {
                field = me.down('field[name=FVVendorKey]').bindStore(storeVendor);
                field.setValue(vendorkey);

                var storeVendorContacts = new CBH.store.vendors.VendorContacts().load({
                    params: {
                        vendorkey: vendorkey
                    },
                    callback: function() {

                        field = me.down('field[name=FVVendorContactKey]').bindStore(storeVendorContacts);
                        field.setValue(vendorcontactkey);

                        var storeWarehouseType = new CBH.store.vendors.WarehouseTypes().load({
                            callback: function() {
                                field = me.down('field[name=FVWarehouseKey]').bindStore(storeWarehouseType);
                                field.setValue(warehousekey);

                                var storeQuotes = new CBH.store.sales.FileQuoteDetail().load({
                                    params: {
                                        filekey: me.FileKey,
                                        vendorkey: vendorkey
                                    },
                                    callback: function() {
                                        me.down('#gridorderentry').bindStore(storeQuotes);
                                        Ext.Msg.hide();
                                    }
                                });
                            }
                        });
                    }
                });
            }
        });
    },

    onSaveClick: function(toolbar, record) {
        var me = this;
        var form = me.getForm();

        if (!form.isValid()) {
            Ext.Msg.alert("Validation", "Check data for valid input!!!");
            return false;
        }

        form.updateRecord();

        record = form.getRecord();

        Ext.Msg.wait('Saving Record...', 'Wait');

        var isdirty = record.dirty;

        record.save({
            success: function(e) {
                Ext.Msg.hide();
                toolbar.doRefresh();
            },
            failure: function() {
                Ext.Msg.hide();
            }
        });
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
        me.down("field[name=FVTotalWeight]").setValue(data.value);
        me.down("field[name=FVTotalWeight]").focus(true, 200);
        /*var qty = me.down('#QuoteQty').getValue();
        me.down('field[name=x_LineWeight]').setValue(data.value * qty);*/
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
        me.down("field[name=FVTotalVolume]").setValue(data.value);
        me.down("field[name=FVTotalVolume]").focus(true, 200);
        /*var qty = me.down('#QuoteQty').getValue();
        me.down('field[name=x_LineVolume]').setValue(data.value * qty);*/
    },

    refreshData: function() {
        var me = this,
            grids = me.query("gridpanel"),
            combos = me.query("combobox");

        Ext.each(grids, function(grid, index) {
           var store = grid.getStore(); 
           if(store)
                store.reload();
        });

        Ext.each(combos, function(combo, index) {
            var store = combo.getStore(); 
            if(store)
                store.reload();
        });
        
        /*me.down("#gridvendors").getStore().reload();
        me.down("#gridquotes").getStore().reload();
        me.down("#gridroles").getStore().reload();
        me.down("#gridstatus").getStore().reload();*/
    }
});
