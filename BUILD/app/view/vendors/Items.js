Ext.define('CBH.view.vendors.Items', {
    extend: 'Ext.form.Panel',
    alias: 'widget.items',

    layout: {
        type: 'column'
    },
    bodyPadding: 10,
    frameHeader: false,
    header: false,
    enableKeyEvents: true,

    storeNavigator: null,
    VendorKey: 0,
    ItemKey: 0,

    requires: [
        'Ext.ux.form.NumericField'
    ],

    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;

        var me = this;

        storeVendors = null;
        storeCurrencyRates = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        storeLangs = new CBH.store.common.Languages().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        storeDesc = null;
        storeHistory = null;
        storeSchedules = null;

        rowEditing = new Ext.grid.plugin.RowEditing({
            clicksToMoveEditor: 2,
            autoCancel: false,
            errorSummary: false,
            listeners: {
                beforeedit: {
                    delay: 500,
                    fn: function(item, e) {
                        this.getEditor().onFieldChange();
                    }
                },
                cancelEdit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('gridpanel');
                        // Canceling editing of a locally added, unsaved record: remove it
                        if (context.record.phantom) {
                            grid.store.remove(context.record);
                        }
                    }
                },
                edit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('#griddesc'),
                            record = context.record;

                        record.save({
                            callback: function() {
                                grid.store.reload();
                            }
                        });
                    }
                }
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
            items: [
                // header form
                /*{
                    xtype: 'fieldset',
                    padding: '5',
                    margin: '0 0 5 0',
                    columnWidth: 1,
                    layout: {
                        type: 'hbox'
                    },
                    items: [{
                        xtype:'component',
                        flex:1
                    },
                    {
                        xtype: 'displayfield',
                        name: 'ItemKey',
                        hideTrigger: true,
                        fieldLabel: 'Reference Num',
                        editable: false,
                        labelAlign:'left',
                        labelWidth: 120,
                        //labelStyle: 'text-align: right',
                        fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: left'
                    }]
                },*/
                {
                    xtype: 'tabpanel',
                    columnWidth: 1,
                    margin: '0 0 0 0',
                    activeTab: 0,
                    height: me.height,
                    items: [
                        // General Panel
                        {
                            xtype: 'panel',
                            title: 'General Information',
                            layout: {
                                type: 'column'
                            },
                            items: [
                                // Item Information
                                {
                                    xtype: 'fieldset',
                                    columnWidth: 0.5,
                                    layout: {
                                        type: 'column'
                                    },
                                    padding: '0 10 10 10',
                                    collapsible: true,
                                    title: 'Item Information',
                                    items: [{
                                        xtype: 'combo',
                                        name: 'ItemVendorKey',
                                        fieldLabel: 'Vendor',
                                        columnWidth: 1,
                                        listConfig: {
                                            minWidth: null
                                        },
                                        valueField: 'VendorKey',
                                        displayField: 'VendorName',
                                        store: storeVendors,
                                        pageSize: 11,
                                        queryMode: 'remote',
                                        triggerAction: '',
                                        queryCaching: false,
                                        typeAhead: true,
                                        typeAheadDelay: 200,
                                        minChars: 3,
                                        allowBlank: false,
                                        forceSelection: true,
                                        emptyText: 'Choose Vendor'
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        fieldLabel: 'Item Number',
                                        itemId: 'ItemNum',
                                        name: 'ItemNum',
                                        allowBlank: false,
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        fieldLabel: 'Description (English)',
                                        name: 'x_ItemName',
                                        allowBlank: false,
                                    }, {
                                        xtype: 'numericfield',
                                        columnWidth: 0.5,
                                        name: 'ItemCost',
                                        fieldLabel: 'Latest Item Cost',
                                        fieldStyle: 'text-align: right;',
                                        minValue: 0,
                                        hideTrigger: true,
                                        useThousandSeparator: true,
                                        decimalPrecision: 2,
                                        alwaysDisplayDecimals: true,
                                        allowNegative: false,
                                        alwaysDecimals: true,
                                        thousandSeparator: ',',
                                        disabled: me.fromLineEntry,
                                        editable: !me.fromLineEntry
                                    }, {
                                        margin: '0 0 0 5',
                                        xtype: 'numericfield',
                                        columnWidth: 0.5,
                                        name: 'ItemPrice',
                                        fieldLabel: 'Latest Item Price',
                                        fieldStyle: 'text-align: right;',
                                        minValue: 0,
                                        hideTrigger: true,
                                        useThousandSeparator: true,
                                        decimalPrecision: 2,
                                        alwaysDisplayDecimals: true,
                                        allowNegative: false,
                                        alwaysDecimals: true,
                                        thousandSeparator: ',',
                                        disabled: me.fromLineEntry,
                                        editable: !me.fromLineEntry
                                    }, {
                                        columnWidth: 1,
                                        xtype: 'combo',
                                        name: 'ItemCurrencyCode',
                                        fieldLabel: 'Currency',
                                        store: storeCurrencyRates,
                                        labelWidth: 50,
                                        listConfig: {
                                            minWidth: null
                                        },
                                        valueField: 'CurrencyCode',
                                        displayField: 'CurrencyCodeDesc',
                                        queryMode: 'local',
                                        typeAhead: !me.fromLineEntry,
                                        minChars: 2,
                                        allowBlank: false,
                                        forceSelection: true,
                                        listeners: {
                                            beforequery: function(record) {
                                                record.query = new RegExp(record.query, 'i');
                                                record.forceAll = true;
                                            }
                                        },
                                        disabled: me.fromLineEntry,
                                        editable: !me.fromLineEntry
                                    }, {
                                        xtype: 'fieldcontainer',
                                        columnWidth: 0.5,
                                        layout: 'hbox',
                                        items: [{
                                            flex: 1,
                                            xtype: 'numericfield',
                                            name: 'ItemWeight',
                                            fieldLabel: 'Weight (kg)',
                                            fieldStyle: 'text-align: right;',
                                            minValue: 0,
                                            hideTrigger: true,
                                            useThousandSeparator: true,
                                            decimalPrecision: 2,
                                            alwaysDisplayDecimals: true,
                                            allowNegative: false,
                                            alwaysDecimals: true,
                                            thousandSeparator: ',',
                                            disabled: me.fromLineEntry,
                                            editable: !me.fromLineEntry
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
                                                    value = me.down('field[name=ItemWeight]').getValue();

                                                me.onConvertPoundsClick(value);
                                            }
                                        }]
                                    }, {
                                        margin: '0 0 0 5',
                                        xtype: 'fieldcontainer',
                                        columnWidth: 0.5,
                                        layout: 'hbox',
                                        items: [{
                                            flex: 1,
                                            xtype: 'numericfield',
                                            name: 'ItemVolume',
                                            fieldLabel: 'Volume (m³)',
                                            fieldStyle: 'text-align: right;',
                                            minValue: 0,
                                            hideTrigger: true,
                                            useThousandSeparator: true,
                                            decimalPrecision: 2,
                                            alwaysDisplayDecimals: true,
                                            allowNegative: false,
                                            alwaysDecimals: true,
                                            thousandSeparator: ',',
                                            disabled: me.fromLineEntry,
                                            editable: !me.fromLineEntry
                                        }, {
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
                                                    value = me.down('field[name=ItemVolume]').getValue();

                                                me.onConvertCubicFeetsClick(value);
                                            }
                                        }]
                                    }, {
                                        xtype: 'checkbox',
                                        columnWidth: 1,
                                        name: 'ItemActive',
                                        labelSeparator: '',
                                        hideLabel: true,
                                        boxLabel: 'Item is active'
                                    }]
                                },
                                // P A #
                                {
                                    xtype: 'fieldset',
                                    title: 'PA #',
                                    margin: '0 0 0 10',
                                    columnWidth: 0.5,
                                    layout: 'column',
                                    padding: '0 10 10 10',
                                    collapsible: true,
                                    items: [
                                        // fields
                                        {
                                            xtype: 'fieldcontainer',
                                            layout: 'hbox',
                                            columnWidth: 1,
                                            items: [{
                                                xtype: 'combo',
                                                flex: 1,
                                                name: 'ItemSchBImportKey',
                                                //fieldLabel: 'PA #',
                                                valueField: 'SBLanguageKey',
                                                displayField: 'x_Description',
                                                store: storeSchedules,
                                                pageSize: 11,
                                                queryMode: 'remote',
                                                triggerAction: '',
                                                queryCaching: false,
                                                typeAhead: true,
                                                typeAheadDelay: 200,
                                                minChars: 3,
                                                allowBlank: true,
                                                forceSelection: true,
                                                //selectOnFocus: true,
                                                emptyText: 'Choose',
                                                listeners: {
                                                    buffer: 100,
                                                    select: function(field, records, eOpts) {
                                                        var me = field.up('form');
                                                        if (records.length > 0) {
                                                            me.down("field[name=SBLanguageText]").setValue(records[0].data.SBLanguageText);
                                                        }
                                                    }
                                                }
                                            }, {
                                                xtype: 'button',
                                                margin: '5 0 0 5',
                                                width: 35,
                                                glyph: 0xf1e5, //0xf067,
                                                itemId: 'btnAddItem',
                                                scale: 'medium',
                                                border: false,
                                                /*cls:'myButton',*/
                                                ui: 'plain',
                                                style: 'background-color:white!important; color:#004A9C !important; font-size: 16px !important;',
                                                iconAlign: 'left',
                                                tooltip: 'Schedule B Maintenance',
                                                listeners: {
                                                    click: {
                                                        fn: me.onItemSchBImportKeyDoubleClick,
                                                        scope: me
                                                    }
                                                }
                                            }]
                                        }, {
                                            xtype: 'textarea',
                                            name: 'SBLanguageText',
                                            columnWidth: 1,
                                            margin: '10 0 0 0'
                                        }
                                    ]
                                }
                            ]
                        },
                        // Descriptions Panel
                        {
                            xtype: 'panel',
                            title: 'Descriptions',
                            layout: 'column',
                            items: [{
                                columnWidth: 1,
                                xtype: 'gridpanel',
                                itemId: 'griddesc',
                                minHeight: 250,
                                store: storeDesc,
                                maxHeight: 250,
                                columns: [{
                                    xtype: 'rownumberer',
                                    format: '00,000'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Language',
                                    dataIndex: 'x_Language',
                                    flex: 2,
                                    editor: {
                                        xtype: 'combo',
                                        displayField: 'LanguageName',
                                        valueField: 'LanguageCode',
                                        name: 'DescriptionLanguageCode',
                                        enableKeyEvents: true,
                                        forceSelection: true,
                                        queryMode: 'local',
                                        selectOnFocus: true,
                                        emptyText: 'Choose Language',
                                        listeners: {
                                            select: function(field, records, eOpts) {
                                                var form = field.up('panel'),
                                                    record = form.context.record;
                                                if (records.length > 0) {
                                                    record.set('x_Language', records[0].data.LanguageName);
                                                }
                                            },
                                            change: function(field) {
                                                var form = field.up('panel');
                                                form.onFieldChange();
                                            },
                                            beforequery: function(record) {
                                                record.query = new RegExp(record.query, 'i');
                                                record.forceAll = true;
                                            }
                                        },
                                        store: storeLangs
                                    }
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Description',
                                    dataIndex: 'DescriptionText',
                                    flex: 8,
                                    editor: {
                                        xtype: 'textfield',
                                        name: 'DescriptionText',
                                        allowBlank: false,
                                        listeners: {
                                            change: function(field) {
                                                var form = field.up('panel');
                                                form.onFieldChange();
                                            }
                                        }
                                    }
                                }],
                                tbar: [{
                                    xtype: 'component',
                                    flex: 1
                                }, {
                                    text: 'Add',
                                    itemId: 'adddesc',
                                    handler: function() {
                                        rowEditing.cancelEdit();

                                        var grid = this.up('gridpanel');
                                        var itemkey = me.down('#FormToolbar').getCurrentRecord().data.ItemKey;

                                        // Create a model instance
                                        var r = Ext.create('CBH.model.vendors.ItemDescriptions', {
                                            DescriptionItemKey: itemkey
                                        });

                                        var count = grid.getStore().count();
                                        grid.store.insert(count, r);
                                        rowEditing.startEdit(count, 1);
                                    },
                                    disabled: true
                                }, {
                                    itemId: 'deletedesc',
                                    text: 'Delete',
                                    hidden: accLevel === 3,
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
                                                        selection[0].destroy({
                                                            success: function() {
                                                                grid.store.remove(selection[0]);
                                                                if (grid.store.getCount() > 0) {
                                                                    sm.select(0);
                                                                }
                                                            }
                                                        });
                                                    }
                                                }
                                            }).defaultButton = 2;
                                        }
                                    },
                                    disabled: true
                                }],
                                selType: 'rowmodel',
                                plugins: [rowEditing],
                                listeners: {
                                    selectionchange: function(view, records) {
                                        this.down('#deletedesc').setDisabled(!records.length);
                                    }
                                }
                            }]
                        },
                        // Price history 
                        {
                            xtype: 'panel',
                            title: 'Price History',
                            layout: 'column',
                            items: [{
                                xtype: 'gridpanel',
                                itemId: 'gridhistory',
                                minHeight: 350,
                                store: storeHistory,
                                maxHeight: 600,
                                columnWidth: 1,
                                title: 'Double click to open Purchase Order',
                                columns: [{
                                    xtype: 'rownumberer',
                                    format: '00,000'
                                }, {
                                    xtype: 'numbercolumn',
                                    width: 65,
                                    dataIndex: 'PONum',
                                    text: 'PO Num',
                                    format: '0'
                                }, {
                                    xtype: 'numbercolumn',
                                    width: 125,
                                    text: 'Cost From Supplier',
                                    dataIndex: 'CostFromSupplier',
                                    format: '00,000.00',
                                    align: 'right'
                                }, {
                                    xtype: 'numbercolumn',
                                    width: 150,
                                    text: 'Price paid by Customer',
                                    dataIndex: 'PricePaidByCustomer',
                                    format: '00,000.00',
                                    align: 'right'
                                }, {
                                    xtype: 'gridcolumn',
                                    width: 100,
                                    flex: 3,
                                    dataIndex: 'Customer',
                                    text: 'Name'
                                }, {
                                    xtype: 'gridcolumn',
                                    width: 100,
                                    flex: 1,
                                    dataIndex: 'Date',
                                    text: 'Date',
                                    renderer: Ext.util.Format.dateRenderer('m/d/Y')
                                }],
                                listeners: {
                                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                                        var me = this.up('form');
                                        me.onViewDetailPriceHistory(record);
                                    }
                                }
                            }]
                        }
                    ]
                }
            ],
            dockedItems: [
                // Toolbar Header
                {
                    xtype: 'container',
                    layout: 'hbox',
                    dock: 'top',
                    items: [{
                        xtype: 'formtoolbar',
                        itemId: 'FormToolbar',
                        flex: 1,
                        store: me.storeNavigator,
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
                            },
                            beginedit: {
                                fn: me.onBeginEdit,
                                scope: me
                            }
                        }
                    }, {
                        xtype: 'component',
                        flex: 1
                    }, {
                        margin: '5 10 0 0',
                        xtype: 'displayfield',
                        name: 'ItemKey',
                        hideTrigger: true,
                        fieldLabel: 'Reference Num',
                        editable: false,
                        labelAlign: 'left',
                        labelWidth: 120,
                        //labelStyle: 'text-align: right',
                        fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: left'
                    }]
                },
                // Toolbar Bottom
                {
                    xtype: 'toolbar',
                    dock: 'bottom',
                    ui: 'footer',
                    items: [{
                        xtype: 'textfield',
                        name: 'ItemCreatedBy',
                        readOnly: true,
                        fieldLabel: 'Created By',
                        editable: false
                    }, {
                        xtype: 'datetimefield',
                        name: 'ItemCreatedDate',
                        readOnly: true,
                        fieldLabel: 'Created Date',
                        hideTrigger: true,
                        editable: false
                    }, {
                        xtype: 'textfield',
                        name: 'ItemModifiedBy',
                        readOnly: true,
                        fieldLabel: 'Modified By',
                        editable: false
                    }, {
                        xtype: 'datetimefield',
                        name: 'ItemModifiedDate',
                        readOnly: true,
                        fieldLabel: 'Modified Date',
                        hideTrigger: true,
                        editable: false
                    }, {
                        xtype: 'component',
                        flex: 1
                    }]
                }
            ],
            listeners: {
                render: {
                    fn: me.onRenderForm,
                    scope: me
                },
                afterrender: {
                    fn: me.registerKeyBindings,
                    scope: me
                },
                close: {
                    fn: me.onCloseForm,
                    scope: me
                }
            }
        });

        me.callParent(arguments);
    },

    onClickItemPriceHistory: function(record) {
        var me = this,
            curitemkey = me.down("#FormToolbar").getCurrentRecord().data.ItemKey;

        var tabs = this.up('app_pageframe');
        Ext.Msg.wait('Loading Item Price History...', 'Wait');
        var storeItems = new CBH.store.vendors.Items().load({
            params: {
                id: curitemkey,
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function(records, operation, success) {
                var currecord = records[0];
                var form = Ext.widget('itemspricehistory', {
                    CurRecord: currecord
                });
                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    title: 'Items Price History',
                    items: [form]
                });

                form.loadRecord(currecord);
                Ext.Msg.hide();
                tab.show();
            }
        });
    },

    registerKeyBindings: function(view, options) {
        var me = this;
        Ext.EventManager.on(view.getEl(), 'keyup', function(evt, t, o) {
                if (evt.ctrlKey && evt.keyCode === Ext.EventObject.F8) {
                    evt.stopEvent();
                    var toolbar = me.down('#FormToolbar');
                    if (toolbar.isEditing) {
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

        if (toolbar.store.getCount() === 1 && toolbar.store.getAt(0).phantom) {
            toolbar.items.items.forEach(function(btn) {
                btn.setVisible(false);
            });
            toolbar.down('#save').setVisible(true);
        }*/

        var field = me.down('field[name=ItemNum]');
        field.focus(true, 100);
    },

    onAfterLoadRecord: function(tool, record) {
        var me = this;

        if (record.phantom) {
            Ext.Msg.wait('Loading Vendors', 'Wait');
            storeVendors = new CBH.store.vendors.Vendors().load({
                params: {
                    id: record.data.ItemVendorKey || 0,
                    page: 1,
                    start: 0,
                    limit: 8
                },
                callback: function() {
                    me.down('field[name=ItemVendorKey]').bindStore(storeVendors);
                    if (record.data.ItemVendorKey > 0) {
                        me.down('field[name=ItemVendorKey]').select(storeVendors.getAt(0));
                        vendorkey = storeVendors.getAt(0).data.VendorKey;
                    }
                    Ext.Msg.hide();
                }
            });
            return;
        }

        var currentItemKey = record.data.ItemKey;

        curRec = record;
        Ext.Msg.wait('Loading Vendors...', 'Wait');
        storeVendors = new CBH.store.vendors.Vendors().load({
            params: {
                id: record.data.ItemVendorKey,
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function() {
                var curVendor = this.getAt(0);
                var fieldVendor = me.down('field[name=ItemVendorKey]');
                fieldVendor.bindStore(storeVendors);
                fieldVendor.setValue(curVendor.data.VendorKey);
                if (curVendor.data.VendorKey) {
                    fieldVendor.setReadOnly(true);
                }

                Ext.Msg.wait('Loading Descriptions...', 'Wait');
                var filterItem = {
                    property: ''
                };
                storeDesc = new CBH.store.vendors.ItemDescriptions().load({
                    params: {
                        page: 0,
                        start: 0,
                        limit: 0,
                        itemkey: currentItemKey
                    },
                    callback: function() {

                        var grid = me.down('#griddesc');
                        grid.reconfigure(this);
                        me.down('#adddesc').setDisabled(false);

                        Ext.Msg.wait('Loading Price History...', 'Wait');
                        storeHistory = new CBH.store.sales.qfrmItemPriceHistoryPurchaseOrders().load({
                            params: {
                                page: 0,
                                ItemKey: currentItemKey
                            },
                            callback: function(records, operation, success) {
                                me.down('#gridhistory').reconfigure(this);

                                Ext.Msg.wait('Loading Schedule..', 'Wait');
                                storeSchedules = new CBH.store.vendors.qlstScheduleBImports().load({
                                    scope: storeSchedules,
                                    params: {
                                        id: curRec.data.ItemSchBImportKey,
                                        start: 0,
                                        page: 0,
                                        limit: 8
                                    },
                                    callback: function(records) {
                                        if (records && records[0]) {
                                            me.down("field[name=ItemSchBImportKey]").bindStore(this);
                                            me.down("field[name=ItemSchBImportKey]").setValue(curRec.data.ItemSchBImportKey);
                                            me.down("field[name=SBLanguageText]").setValue(records[0].data.SBLanguageText);
                                        }
                                        storeSchedules.lastOptions.callback = null;
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

    onAddClick: function(toolbar, record) {

    },

    onBeginEdit: function(toolbar, record) {
        var me = this;
        me.down('field[name=ItemVendorKey]').focus(true, 200);
        me.checkChange();
    },

    onSaveClick: function(toolbar, record) {
        var me = this,
            form = me.getForm();

        if (!form.isValid()) {
            Ext.Msg.alert("Validation", "Check data for valid input!!!");
            return false;
        }

        form.updateRecord();

        record = form.getRecord();

        Ext.Msg.wait('Saving Record...', 'Wait');

        var isPhantom = record.phantom;

        record.save({
            success: function(e) {
                me.ItemKey = record.data.ItemKey;
                Ext.Msg.hide();
                toolbar.doRefresh();
            },
            failure: function() {
                Ext.Msg.hide();
            }
        });
    },

    onDeleteClick: function(pageTool, record) {

        if (record) {
            var curRec = record.index - 1;
            curPage = pageTool.store.currentPage;
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
                                Ext.Msg.hide();
                                var lastOpt = pageTool.store.lastOptions;
                                pageTool.store.reload({
                                    params: lastOpt.params,
                                    callback: function() {}
                                });
                                if (pageTool.store.getCount() > 0) {
                                    pageTool.gotoAt(prevRec);
                                } else {
                                    pageTool.up('form').up('panel').destroy();
                                }
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

    onCloseForm: function() {
        var me = this;

        var callerForm = me.callerForm,
            grid = callerForm.down('#griditems');

        if (grid) {
            Ext.Msg.hide();
            me.destroy();
            grid.store.reload();
            return;
        } else {
            var itemField = callerForm.down('field[name=QuoteItemKey]');

            if (itemField) {
                var record = me.getForm().getRecord();
                if (itemField && record) {
                    var itemnum = record.data.ItemNum;
                    itemField.setRawValue(itemnum);
                    itemField.doRawQuery();
                    itemField.focus(true, 400);
                }
            }
        }
    },

    onItemSchBImportKeyDoubleClick: function() {
        var me = this,
            record = me.down('field[name=ItemSchBImportKey]').getSelectedRecord();

        //var tabs = this.up('app_pageframe');

        me.getEl().mask('Please wait...');
        storeToNavigate = new CBH.store.common.ScheduleB().load({
            params: {
                id: (record) ? record.data.SchBNum : '000000000000'
            },
            callback: function(records, operation, success) {
                var form = Ext.widget('ScheduleB', {
                    storeNavigator: storeToNavigate,
                    modal: true,
                    width: 700,
                    frameHeader: true,
                    header: true,
                    layout: {
                        type: 'column'
                    },
                    title: 'Schedule B Maintenance',
                    bodyPadding: 10,
                    closable: true,
                    //constrain: true,
                    stateful: false,
                    floating: true,
                    callerForm: me,
                    forceFit: true
                });

                me.getEl().unmask();
                if (records && records.length) {
                    form.down('#FormToolbar').gotoAt(1);
                    form.show();
                } else {
                    form.show();
                    var model = new CBH.model.common.ScheduleB();
                    var btn = form.down('#FormToolbar').down('#add');
                    btn.fireEvent('click', btn, null, null, model);
                }
            }
        });
    },

    onConvertPoundsClick: function(value) {
        var me = this,
            toolbar = me.down('#FormToolbar');

        if (!toolbar.isEditing)
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
        me.down("field[name=ItemWeight]").setValue(data.value);
        me.down("field[name=ItemWeight]").focus(true, 200);
        /*var qty = me.down('#QuoteQty').getValue();
        me.down('field[name=x_LineWeight]').setValue(data.value * qty);*/
    },

    onConvertCubicFeetsClick: function(value) {
        var me = this,
            toolbar = me.down('#FormToolbar');

        if (!toolbar.isEditing)
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
        me.down("field[name=ItemVolume]").setValue(data.value);
        me.down("field[name=ItemVolume]").focus(true, 200);
        /*var qty = me.down('#QuoteQty').getValue();
        me.down('field[name=x_LineVolume]').setValue(data.value * qty);*/
    },

    onViewDetailPriceHistory: function(record) {
        var me = this;

        var tabs = me.up('app_pageframe');

        me.loadPurchaseOrder(record.data.POKey).then({
            success: function(records) {
                var purchaseOrder = records[0];
                Ext.Msg.wait('Loading....', 'Wait');
                var storeJobs = new CBH.store.jobs.JobList().load({
                    params: {
                        id: purchaseOrder.data.POJobKey
                    },
                    callback: function(records, operation, success) {
                        var curJob = records[0];

                        var storeJobOverview = new CBH.store.jobs.qJobOverview().load({
                            params: {
                                id: purchaseOrder.data.POJobKey
                            },
                            callback: function(records, operation, success) {
                                var jobOverview = records[0];

                                var form = Ext.widget('joboverview', {
                                    currentRecord: jobOverview,
                                    currentJob: curJob,
                                    JobKey: purchaseOrder.data.POJobKey,
                                    JobNum: jobOverview.data.JobNum
                                });

                                form.loadRecord(jobOverview);

                                var tab = tabs.add({
                                    closable: true,
                                    iconCls: 'tabs',
                                    autoScroll: true,
                                    title: 'Job Overview: ' + purchaseOrder.data.x_JobNumFormatted,
                                    items: [form],
                                    listeners: {
                                        activate: function() {
                                            var form = this.down('form');
                                            form.refreshData();
                                        }
                                    }
                                });

                                tab.show();
                                Ext.Msg.hide();
                            }
                        });
                    }
                });
            }
        });
    },

    loadPurchaseOrder: function(POKey) {
        var deferred = Ext.create('Deft.Deferred');

        new CBH.store.jobs.JobPurchaseOrders().load({
            params: {
                id: POKey
            },
            callback: function(records, operation, success) {
                if (success) {
                    deferred.resolve(records);
                } else {
                    deferred.reject("Error loading Companies.");
                }
            }
        });

        return deferred.promise;
    }

});
