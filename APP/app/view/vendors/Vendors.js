Ext.define('CBH.view.vendors.Vendors', {
    extend: 'Ext.form.Panel',
    alias: 'widget.vendors',
    layout: {
        type: 'column'
    },
    bodyPadding: 5,
    frameHeader: false,
    header: false,
    enableKeyEvents: true,

    title: 'Vendor Maintenance',

    storeNavigator: null,

    requires: [
        'CBH.view.vendors.VendorContacts',
        'CBH.view.vendors.VendorOriginAddress',
        'CBH.view.vendors.VendorWarehouse',
        'CBH.view.vendors.Items'
    ],

    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;

        var me = this;

        storeStates = new CBH.store.common.States().load({
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
        storeItems = new CBH.store.vendors.Items();

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
                {
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
                        name: 'VendorKey',
                        hideTrigger: true,
                        fieldLabel: 'Vendor Code',
                        editable: false,
                        labelAlign:'left',
                        labelWidth: 80,
                        //labelStyle: 'text-align: right',
                        fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: left'
                    }]
                }, 
                // Tab Panels
                {
                    xtype: 'tabpanel',
                    columnWidth: 1,
                    margin: '0 0 0 0',
                    activeTab: 0,
                    items: [
                        //General Information
                        {
                            xtype: 'panel',
                            title: 'Vendor Information',
                            minHeight: 350,
                            items: [{
                                xtype: 'fieldset',
                                columnWidth: 1,
                                layout: {
                                    type: 'column'
                                },
                                padding: '0 5 5 5',
                                items: [{
                                    xtype: 'fieldcontainer',
                                    columnWidth: 0.6,
                                    layout: {
                                        type: 'column'
                                    },
                                    items: [{
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        fieldLabel: 'Name',
                                        labelAlign: 'top',
                                        labelWidth: 50,
                                        name: 'VendorName',
                                        itemId: 'vendorname',
                                        allowBlank: false,
                                        listeners: {
                                            blur: function(field) {
                                                if (field.value !== null) {
                                                    var form = field.up('form');
                                                    form.down('#vendorpeachtreeid').setValue(field.value.toUpperCase());
                                                }
                                            }
                                        }
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        fieldLabel: 'Address',
                                        name: 'VendorAddress1',
                                        itemId: 'vendoraddress1'
                                    }, {
                                        margin: '5 0 0 0',
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        name: 'VendorAddress2',
                                        itemId: 'vendoraddress2'
                                    }, {
                                        xtype: 'textfield',
                                        margin: '0 0 0 0',
                                        columnWidth: 0.3,
                                        fieldLabel: 'City',
                                        name: 'VendorCity',
                                        itemId: 'vendorcity'
                                    }, {
                                        xtype: 'combo',
                                        margin: '0 0 0 5',
                                        columnWidth: 0.4,
                                        fieldLabel: 'State',
                                        name: 'VendorState',
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
                                        margin: '0 0 0 5',
                                        columnWidth: 0.3,
                                        fieldLabel: 'Zip',
                                        name: 'VendorZip',
                                        itemId: 'vendorzip'
                                    }, {
                                        columnWidth: 1,
                                        xtype: 'combo',
                                        fieldLabel: 'Default Language',
                                        name: 'VendorLanguageCode',
                                        displayField: 'LanguageName',
                                        valueField: 'LanguageCode',
                                        enableKeyEvents: true,
                                        forceSelection: true,
                                        queryMode: 'local',
                                        selectOnFocus: true,
                                        emptyText: 'Choose Language',
                                        allowBlank: false,
                                        defaultValue: 'en',
                                        store: storeLangs,
                                        anyMatch: true
                                    }, {
                                        xtype: 'combo',
                                        columnWidth: 0.4,
                                        fieldLabel: 'Country',
                                        name: 'VendorCountryKey',
                                        queryMode: 'local',
                                        typeAhead: true,
                                        minChars: 2,
                                        forceSelection: true,
                                        emptyText: 'Choose Country',
                                        displayField: 'CountryName',
                                        store: storeCountries,
                                        valueField: 'CountryKey',
                                        itemId: 'vendorcountrykey'
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 0.3,
                                        margin: '0 0 0 5',
                                        fieldLabel: 'Phone',
                                        name: 'VendorPhone',
                                        itemId: 'vendorphone'
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 0.3,
                                        margin: '0 0 0 5',
                                        fieldLabel: 'Fax',
                                        name: 'VendorFax',
                                        itemId: 'vendorfax'
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 0.5,
                                        margin: '0 0 0 0',
                                        fieldLabel: 'Email',
                                        name: 'VendorEmail',
                                        itemId: 'vendoremail'
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 0.5,
                                        margin: '0 0 0 5',
                                        fieldLabel: 'Website',
                                        name: 'VendorWebsite',
                                        itemId: 'vendorwebsite'
                                    }
                                    ]
                                }, {
                                    xtype: 'fieldcontainer',
                                    columnWidth: 0.4,
                                    margin: '0 0 0 5',
                                    layout: {
                                        type: 'column'
                                    },
                                    items: [{
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        fieldLabel: 'Peachtree ID',
                                        name: 'VendorPeachtreeID',
                                        itemId: 'vendorpeachtreeid',
                                        //allowBlank: false
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        fieldLabel: 'Peachtree Item ID',
                                        name: 'VendorPeachtreeItemID',
                                        itemId: 'vendorpeachtreeitemid',
                                        //allowBlank: false
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        fieldLabel: 'Peachtree Job ID',
                                        name: 'VendorPeachtreeJobID',
                                        itemId: 'vendorpeachtreejobid',
                                        //allowBlank: false
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        fieldLabel: 'Display to Cust',
                                        name: 'VendorDisplayToCust',
                                        itemId: 'vendordisplaytocust',
                                        //allowBlank: false
                                    }, {
                                        xtype: 'textfield',
                                        columnWidth: 1,
                                        fieldLabel: 'Account Num',
                                        name: 'VendorAcctNum',
                                        itemId: 'vendoracctnum',
                                        //allowBlank: false
                                    }, {
                                        columnWidth: 0.5,
                                        xtype: 'checkbox',
                                        name: 'VendorCarrier',
                                        labelSeparator: '',
                                        hideLabel: true,
                                        boxLabel: 'Vendor is Carrier'
                                    }]
                                }]
                            }]
                        },
                        // Panel Items
                        {
                            xtype: 'panel',
                            title: 'Items',
                            items: [{
                                xtype: 'gridpanel',
                                itemId: 'griditems',
                                minHeight: 350,
                                store: storeItems,
                                maxHeight: 400,
                                columns: [{
                                    width: 60,
                                    xtype: 'rownumberer',
                                    format: '00,000'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Item Number',
                                    dataIndex: 'ItemNum'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Description',
                                    dataIndex: 'x_ItemName',
                                    flex: 1
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Weight',
                                    dataIndex: 'ItemWeight'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Volume',
                                    dataIndex: 'ItemVolume'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'State',
                                    dataIndex: 'x_StateName'
                                }, {
                                    xtype: 'numbercolumn',
                                    text: 'Cost',
                                    dataIndex: 'ItemCost',
                                    format: '00,000.00'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'CUR',
                                    dataIndex: 'ItemCurrencyCode'
                                }, {
                                    xtype: 'actioncolumn',
                                    draggable: false,
                                    width: 35,
                                    resizable: false,
                                    hideable: false,
                                    stopSelection: false,
                                    items: [{
                                        handler: function(view, rowIndex, colIndex, item, e, record, row) {
                                            var me = this.up('form');

                                            storeToNavigate = new CBH.store.vendors.Items({
                                                autoLoad: false
                                            });
                                            model = record;
                                            storeToNavigate.add(model);
                                            var form = Ext.widget('items', {
                                                storeNavigator: storeToNavigate,
                                                modal: true,
                                                width: 700,
                                                frameHeader: true,
                                                header: true,
                                                layout: {
                                                    type: 'column'
                                                },
                                                title: 'Item',
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
                                        },
                                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                                        tootip: 'view details'
                                    }]
                                }],
                                tbar: [
                                    // Search Field
                                    {
                                        xtype: 'searchfield',
                                        width: 400,
                                        itemId: 'itemsearchfield',
                                        listeners: {
                                            'triggerclick': function(field) {
                                                me.onItemSearchFieldChange();
                                            }
                                        }
                                    }, {
                                        xtype: 'component',
                                        flex: 1
                                    }, {
                                        text: 'Add',
                                        itemId: 'additem',
                                        handler: function() {
                                            var me = this.up('form'),
                                                toolbar = me.down('#FormToolbar'),
                                                curVendor = toolbar.getCurrentRecord().data;

                                            storeToNavigate = new CBH.store.vendors.Items({
                                                autoLoad: false
                                            });

                                            model = Ext.create('CBH.model.vendors.Items', {
                                                ItemVendorKey: curVendor.VendorKey
                                            });

                                            var form = Ext.widget('items', {
                                                storeNavigator: storeToNavigate,
                                                modal: true,
                                                width: 700,
                                                frameHeader: true,
                                                header: true,
                                                layout: {
                                                    type: 'column'
                                                },
                                                title: 'New Item',
                                                bodyPadding: 10,
                                                closable: true,
                                                stateful: false,
                                                floating: true,
                                                callerForm: me,
                                                forceFit: true
                                            });

                                            form.show();

                                            var btn = form.down('#FormToolbar').down('#add');
                                            btn.fireEvent('click', btn, null, null, model); // aditional param model for send model data to click event handler
                                        },
                                        disabled: true
                                    }, {
                                        itemId: 'deleteitem',
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
                                                                    grid.store.remove(sm.getSelection());
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
                                    }
                                ],
                                bbar: new Ext.PagingToolbar({
                                    itemId: 'itemspagingtoolbar',
                                    store: storeItems,
                                    displayInfo: true,
                                    displayMsg: 'Displaying records {0} - {1} of {2}',
                                    emptyMsg: "No records to display"
                                }),
                                selType: 'rowmodel',
                                listeners: {
                                    selectionchange: function(view, records) {
                                        this.down('#deleteitem').setDisabled(!records.length);
                                    },
                                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                                        var me = this.up('form');

                                        storeToNavigate = new CBH.store.vendors.Items({
                                            autoLoad: false
                                        });
                                        model = record;
                                        storeToNavigate.add(model);
                                        var form = Ext.widget('items', {
                                            storeNavigator: storeToNavigate,
                                            modal: true,
                                            width: 700,
                                            frameHeader: true,
                                            header: true,
                                            layout: {
                                                type: 'column'
                                            },
                                            title: 'New Item',
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
                                        // var field = form.down('field[name=ItemNum]');
                                        // field.setValue(value);
                                        // field.focus(true, 200);
                                    }
                                }
                            }]
                        },
                        // Panel Contacts
                        {
                            xtype: 'panel',
                            title: 'Contacts',
                            items: [{
                                xtype: 'gridpanel',
                                itemId: 'gridcontacts',
                                minHeight: 350,
                                columns: [{
                                    xtype: 'rownumberer'
                                }, {
                                    xtype: 'gridcolumn',
                                    flex: 3,
                                    dataIndex: 'x_ContactFullName',
                                    text: 'Name'
                                }, {
                                    xtype: 'gridcolumn',
                                    flex: 3,
                                    dataIndex: 'ContactEmail',
                                    text: 'Email'

                                }, {
                                    xtype: 'gridcolumn',
                                    flex: 3,
                                    dataIndex: 'ContactPhone',
                                    text: 'Phone'

                                }, {
                                    xtype: 'gridcolumn',
                                    flex: 3,
                                    dataIndex: 'ContactFax',
                                    text: 'Fax'
                                }, {
                                    xtype: 'actioncolumn',
                                    draggable: false,
                                    width: 35,
                                    resizable: false,
                                    hideable: false,
                                    stopSelection: false,
                                    items: [{
                                        handler: function(view, rowIndex, colIndex, item, e, record, row) {

                                            var form = new CBH.view.vendors.VendorContacts();
                                            form.loadRecord(record);
                                            form.center();
                                            form.show();
                                        },
                                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                                        tootip: 'view details'
                                    }]
                                }],
                                tbar: [{
                                    xtype: 'component',
                                    flex: 1
                                }, {
                                    text: 'Add',
                                    itemId: 'addcontact',
                                    //formBind: true,
                                    handler: function() {
                                        currentRecord = this.up('form').getRecord();
                                        var vendorkey = currentRecord.data.VendorKey;
                                        record = new CBH.model.vendors.VendorContacts({
                                            ContactVendorKey: vendorkey
                                        });

                                        var form = new CBH.view.vendors.VendorContacts();
                                        form.loadRecord(record);
                                        form.center();
                                        form.callerForm = this.up('form');
                                        form.show();
                                    },
                                    disabled: true
                                }, {
                                    itemId: 'deletecontact',
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
                                }, ],
                                selType: 'rowmodel',
                                listeners: {
                                    selectionchange: function(view, records) {
                                        this.down('#deletecontact').setDisabled(!records.length);
                                    },
                                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                                        var form = new CBH.view.vendors.VendorContacts();
                                        form.loadRecord(record);
                                        form.callerForm = this.up('form');
                                        form.show();
                                    }
                                }
                            }]
                        },
                        // Panel Origin Address
                        {
                            xtype: 'panel',
                            title: 'Origin Address',
                            items: [{
                                xtype: 'gridpanel',
                                itemId: 'gridoriginaddress',
                                minHeight: 350,
                                columns: [{
                                    xtype: 'rownumberer',
                                    format: '00,000'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Name',
                                    dataIndex: 'OriginName'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Address',
                                    dataIndex: 'x_OriginAddress',
                                    flex: 1
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'City',
                                    dataIndex: 'OriginCity'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'State',
                                    dataIndex: 'x_StateName'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Country',
                                    dataIndex: 'x_CountryName'
                                }, {
                                    xtype: 'actioncolumn',
                                    draggable: false,
                                    width: 35,
                                    resizable: false,
                                    hideable: false,
                                    stopSelection: false,
                                    items: [{
                                        handler: function(view, rowIndex, colIndex, item, e, record, row) {
                                            var form = new CBH.view.vendors.VendorOriginAddress();
                                            form.modal = true;
                                            form.loadRecord(record);
                                            form.callerForm = this.up('form');
                                            form.show();
                                        },
                                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                                        tootip: 'view details'
                                    }]
                                }],
                                tbar: [{
                                    xtype: 'component',
                                    flex: 1
                                }, {
                                    text: 'Add',
                                    itemId: 'addoriginaddress',
                                    handler: function() {
                                        currentRecord = this.up('form').getRecord();
                                        var vendorkey = currentRecord.data.VendorKey;
                                        var form = new CBH.view.vendors.VendorOriginAddress();
                                        record = new CBH.model.vendors.VendorOriginAddress({
                                            OriginVendorKey: vendorkey
                                        });
                                        form.loadRecord(record);
                                        form.center();
                                        form.callerForm = this.up('form');
                                        form.show();
                                    },
                                    disabled: true
                                }, {
                                    itemId: 'deleteoriginaddress',
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
                                                                grid.store.remove(sm.getSelection());
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
                                }, ],
                                selType: 'rowmodel',
                                listeners: {
                                    selectionchange: function(view, records) {
                                        this.down('#deleteoriginaddress').setDisabled(!records.length);
                                    },
                                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                                        var form = new CBH.view.vendors.VendorOriginAddress();
                                        form.modal = true;
                                        form.loadRecord(record);
                                        form.callerForm = this.up('form');
                                        form.show();
                                    }
                                }
                            }]
                        },
                        // Panel Carrier Warehouse address
                        {
                            xtype: 'panel',
                            title: 'Carrier Warehouse Address',
                            items: [{
                                xtype: 'gridpanel',
                                itemId: 'gridwarehouse',
                                minHeight: 200,
                                columns: [{
                                    xtype: 'rownumberer',
                                    format: '00,000'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Name',
                                    dataIndex: 'WarehouseName'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Address',
                                    dataIndex: 'WarehouseAddress1',
                                    flex: 1
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'City',
                                    dataIndex: 'WarehouseCity'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'State',
                                    dataIndex: 'x_StateName'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Country',
                                    dataIndex: 'x_CountryName'
                                }, {
                                    xtype: 'actioncolumn',
                                    draggable: false,
                                    width: 35,
                                    resizable: false,
                                    hideable: false,
                                    stopSelection: false,
                                    items: [{
                                        handler: function(view, rowIndex, colIndex, item, e, record, row) {
                                            var form = new CBH.view.vendors.VendorOriginAddress();
                                            form.modal = true;
                                            form.loadRecord(record);
                                            form.callerForm = this.up('form');
                                            form.show();
                                        },
                                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                                        tootip: 'view details'
                                    }]
                                }],
                                tbar: [{
                                    xtype: 'component',
                                    flex: 1
                                }, {
                                    text: 'Add',
                                    itemId: 'addwarehouse',
                                    handler: function() {
                                        currentRecord = this.up('form').getRecord();
                                        var vendorkey = currentRecord.data.VendorKey;
                                        var form = new CBH.view.vendors.VendorWarehouse();
                                        record = new CBH.model.vendors.VendorWarehouse({
                                            WarehouseVendorKey: vendorkey
                                        });
                                        form.loadRecord(record);
                                        form.center();
                                        form.callerForm = this.up('form');
                                        form.show();
                                    },
                                    disabled: true
                                }, {
                                    itemId: 'deletewarehouse',
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
                                                                grid.store.remove(sm.getSelection());
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
                                }, ],
                                selType: 'rowmodel',
                                listeners: {
                                    selectionchange: function(view, records) {
                                        this.down('#deletewarehouse').setDisabled(!records.length);
                                    },
                                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                                        var form = new CBH.view.vendors.VendorWarehouse();
                                        form.modal = true;
                                        form.loadRecord(record);
                                        form.callerForm = this.up('form');
                                        form.show();
                                    }
                                }
                            }]
                        }
                    ]
                }
            ],
            dockedItems: [
                // navigation toolbar
                {
                    xtype: 'formtoolbar',
                    itemId: 'FormToolbar',
                    dock: 'top',
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
                },
                // toolbar bottom
                {
                    xtype: 'toolbar',
                    dock: 'bottom',
                    ui: 'footer',
                    items: [{
                        xtype: 'textfield',
                        name: 'VendorCreatedBy',
                        readOnly: true,
                        fieldLabel: 'Created By',
                        editable: false
                    }, {
                        xtype: 'datefield',
                        name: 'VendorCreatedDate',
                        readOnly: true,
                        fieldLabel: 'Created Date',
                        hideTrigger: true,
                        editable: false
                    }, {
                        xtype: 'textfield',
                        name: 'VendorModifiedBy',
                        readOnly: true,
                        fieldLabel: 'Modified By',
                        editable: false
                    }, {
                        xtype: 'datefield',
                        name: 'VendorModifiedDate',
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
    },

    onAfterLoadRecord: function(tool, record) {
        var me = this;

        if (record.phantom) {
            me.down('#additem').setDisabled(true);
            me.down('#addcontact').setDisabled(true);
            me.down('#addoriginaddress').setDisabled(true);
            me.down('#addwarehouse').setDisabled(true);
            return;
        }

        me.down('#additem').setDisabled(false);
        me.down('#addcontact').setDisabled(false);
        me.down('#addoriginaddress').setDisabled(false);
        me.down('#addwarehouse').setDisabled(false);

        var currentVendorKey = record.data.VendorKey;

        curRec = record;
        Ext.Msg.wait('Loading Contacts', 'Wait');
        storeContacts = new CBH.store.vendors.VendorContacts().load({
            params: {
                page: 0,
                start: 0,
                limit: 0,
                vendorkey: currentVendorKey
            },
            callback: function() {

                var grid = me.down('#gridcontacts');
                grid.reconfigure(this);

                Ext.Msg.wait('Loading Origin Address', 'Wait');
                storeOriginAddress = new CBH.store.vendors.VendorOriginAddress().load({
                    params: {
                        page: 0,
                        start: 0,
                        limit: 0,
                        vendorkey: currentVendorKey
                    },
                    callback: function(records, operations, success) {
                        var grid = me.down('#gridoriginaddress');
                        grid.reconfigure(storeOriginAddress);

                        Ext.Msg.wait('Loading Warehouse', 'Wait');
                        storeWarehouse = new CBH.store.vendors.VendorWarehouse().load({

                            params: {
                                page: 0,
                                start: 0,
                                limit: 0,
                                vendorkey: currentVendorKey
                            },
                            callback: function() {
                                var grid = me.down('#gridwarehouse');
                                grid.reconfigure(storeWarehouse);
                                var filterVendor = new Ext.util.Filter({
                                    property: 'ItemVendorKey',
                                    value: currentVendorKey
                                });

                                Ext.Msg.wait('Loading Items', 'Wait');
                                storeItems = new CBH.store.vendors.Items().load({
                                    filters: [filterVendor],
                                    callback: function() {
                                        me.down('#griditems').reconfigure(storeItems);
                                        me.down('#itemspagingtoolbar').bindStore(storeItems);
                                        Ext.Msg.hide();
                                    }
                                });
                            }
                        });
                    }
                });
            }
        });

        if (record.data.VendorCarrier) {
            me.down('#gridwarehouse').setDisabled(false);
        } else {
            me.down('#gridwarehouse').setDisabled(true);
        }
    },

    onAddClick: function(toolbar, record) {
        var me = this;

        me.down('#griditems').store.removeAll();
        me.down('#gridcontacts').store.removeAll();
        me.down('#gridoriginaddress').store.removeAll();
    },

    onBeginEdit: function(tool, record) {
        var me = this;
        me.down('#vendorname').focus(true, 200);
    },

    onSaveClick: function(button, e, eOpts) {
        var me = this,
            toolbar = me.down('#FormToolbar'),
            form = me.getForm();

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

    onItemSearchFieldChange: function() {
        var form = this,
            field = form.down('#itemsearchfield'),
            grid = form.down('#griditems'),
            record = form.down('#FormToolbar').getCurrentRecord();

        grid.store.removeAll();

        if (!String.isNullOrEmpty(field.value)) {
            grid.store.loadPage(1, {
                params: {
                    query: field.value,
                    vendorkey: record.data.VendorKey
                },
                callback: function() {
                    form.down('#itemspagingtoolbar').bindStore(this);
                }
            });
        } else {
            grid.store.loadPage(1, {
                params: {
                    vendorkey: record.data.VendorKey
                },
                callback: function() {
                    form.down('#itemspagingtoolbar').bindStore(this);
                }
            });
        }
    },

    onCloseForm: function() {
        var me = this;
        var vendorField = me.callerForm.down('field[name=QuoteVendorKey]');
        var record = me.getForm().getRecord();
        if (vendorField && record) {
            var vendorname = record.data.VendorName;
            vendorField.setRawValue(vendorname);
            vendorField.doRawQuery();
            vendorField.focus(true, 400);
        }
    }
});
