Ext.define('CBH.view.customers.Customers', {
    extend: 'Ext.form.Panel',
    alias: 'widget.customers',
    bodyPadding: 10,
    frameHeader: false,
    header: false,
    enableKeyEvents: true,

    storeNavigator: null,

    title: 'Customer Maintenance',

    requires: [
        'CBH.view.customers.CustomerContacts',
        'CBH.view.customers.CustomerShipAddress'
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
        storeSalesEmployees = new CBH.store.common.Employees().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        storeOrderEmployees = new CBH.store.common.Employees().load({
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
        storeCustStatus = new CBH.store.customers.CustomerStatus().load({
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

        storeContacts = null;
        storeShipAddress = null;


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
                        name: 'CustKey',
                        hideTrigger: true,
                        fieldLabel: 'Customer Code',
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
                    height: me.height,
                    items: [
                        // Customer Information Panel
                        {
                            xtype: 'panel',
                            title: 'Customer Information',
                            layout: {
                                type: 'column'
                            },
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
                                    name: 'CustName',
                                    itemId: 'custname',
                                    allowBlank: false,
                                    listeners: {
                                        blur: function(field) {
                                            if (field.value !== null) {
                                                var form = field.up('form'),
                                                    peachtree = form.down('#custpeachtreeid');

                                                if (String.isNullOrEmpty(peachtree.getValue())) {
                                                    peachtree.setValue(field.value.toUpperCase());
                                                }
                                            }
                                        }
                                    }
                                }, {
                                    xtype: 'textfield',
                                    columnWidth: 1,
                                    fieldLabel: 'Address 1',
                                    name: 'CustAddress1',
                                    itemId: 'custaddress1'
                                }, {
                                    xtype: 'textfield',
                                    columnWidth: 1,
                                    fieldLabel: 'Address 2',
                                    name: 'CustAddress2',
                                    itemId: 'custaddress2'
                                }, {
                                    xtype: 'textfield',
                                    margin: '0 0 0 0',
                                    columnWidth: 0.3,
                                    fieldLabel: 'City',
                                    name: 'CustCity',
                                    itemId: 'custcity'
                                }, {
                                    xtype: 'combo',
                                    margin: '0 0 0 5',
                                    columnWidth: 0.4,
                                    fieldLabel: 'State',
                                    name: 'CustState',
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
                                    name: 'CustZip',
                                    //vtype: 'alphanum',
                                    itemId: 'custzip'
                                }, {
                                    xtype: 'combo',
                                    columnWidth: 0.25,
                                    margin: '0 0 0 0',
                                    fieldLabel: 'Country',
                                    name: 'CustCountryKey',
                                    queryMode: 'local',
                                    typeAhead: true,
                                    minChars: 2,
                                    forceSelection: true,
                                    emptyText: 'Choose Country',
                                    displayField: 'CountryName',
                                    store: storeCountries,
                                    valueField: 'CountryKey',
                                    itemId: 'custcountrykey',
                                    listeners: {
                                        beforequery: function(record) {
                                            record.query = new RegExp(record.query, 'i');
                                            record.forceAll = true;
                                        }
                                    }
                                }, {
                                    columnWidth: 0.25,
                                    margin: '0 0 0 5',
                                    xtype: 'combo',
                                    fieldLabel: 'Default Language',
                                    name: 'CustLanguageCode',
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
                                    listeners: {
                                        beforequery: function(record) {
                                            record.query = new RegExp(record.query, 'i');
                                            record.forceAll = true;
                                        }
                                    }
                                }, {
                                    xtype: 'textfield',
                                    columnWidth: 0.25,
                                    margin: '0 0 0 5',
                                    fieldLabel: 'Phone',
                                    name: 'CustPhone',
                                    itemId: 'custphone'
                                }, {
                                    xtype: 'textfield',
                                    columnWidth: 0.25,
                                    margin: '0 0 0 5',
                                    fieldLabel: 'Fax',
                                    name: 'CustFax',
                                    itemId: 'custfax'
                                }, {
                                    xtype: 'textfield',
                                    columnWidth: 0.5,
                                    margin: '0 0 0 0',
                                    fieldLabel: 'Email',
                                    name: 'CustEmail',
                                    itemId: 'email'
                                }, {
                                    xtype: 'textfield',
                                    columnWidth: 0.5,
                                    margin: '0 0 0 5',
                                    fieldLabel: 'Website',
                                    name: 'CustWebsite',
                                    itemId: 'website'
                                }

                                ]
                            }, {
                                xtype: 'fieldcontainer',
                                columnWidth: 0.4,
                                margin: '0 0 10 10',
                                //anchor: '100%',
                                //height: 120,
                                layout: {
                                    type: 'column'
                                },
                                items: [{
                                    xtype: 'textfield',
                                    columnWidth: 1,
                                    fieldLabel: 'Peachtree ID',
                                    name: 'CustPeachtreeID',
                                    itemId: 'custpeachtreeid',
                                    allowBlank: false
                                }, {
                                    xtype: 'combo',
                                    columnWidth: 1,
                                    fieldLabel: 'Sales Rep',
                                    name: 'CustSalesRepKey',
                                    itemId: 'custsalesrepkey',
                                    displayField: 'x_EmployeeFullName',
                                    valueField: 'EmployeeKey',
                                    queryMode: 'local',
                                    minChars: 2,
                                    //allowBlank: false,
                                    selectOnFocus: true,
                                    emptyText: 'Choose Employee',
                                    //enableKeyEvents: true,
                                    store: storeSalesEmployees,
                                    listeners: {
                                        beforequery: function(record) {
                                            record.query = new RegExp(record.query, 'i');
                                            record.forceAll = true;
                                        }
                                    }
                                }, {
                                    xtype: 'combo',
                                    columnWidth: 1,
                                    fieldLabel: 'Orders Rep',
                                    name: 'CustOrdersRepKey',
                                    itemId: 'custordersrepkey',
                                    displayField: 'x_EmployeeFullName',
                                    valueField: 'EmployeeKey',
                                    queryMode: 'local',
                                    minChars: 2,
                                    selectOnFocus: true,
                                    emptyText: 'Choose Employee',
                                    store: storeOrderEmployees,
                                    //enableKeyEvents: true,
                                    listeners: {
                                        beforequery: function(record) {
                                            record.query = new RegExp(record.query, 'i');
                                            record.forceAll = true;
                                        }
                                    }
                                }, {
                                    xtype: 'combo',
                                    columnWidth: 1,
                                    name: 'CustStatus',
                                    fieldLabel: 'Status',
                                    itemId: 'custstatus',
                                    displayField: 'StatusDescription',
                                    valueField: 'StatusKey',
                                    queryMode: 'local',
                                    minChars: 2,
                                    //triggerAction: '',
                                    //allowBlank: false,
                                    selectOnFocus: true,
                                    emptyText: 'Choose Status',
                                    store: storeCustStatus,
                                    listeners: {
                                        beforequery: function(record) {
                                            record.query = new RegExp(record.query, 'i');
                                            record.forceAll = true;
                                        }
                                    }
                                }, {
                                    xtype: 'combo',
                                    name: 'CustCurrencyCode',
                                    itemId: 'custcurrencycode',
                                    fieldLabel: 'Currency',
                                    store: storeCurrencyRates,
                                    columnWidth: 0.65,
                                    valueField: 'CurrencyCode',
                                    displayField: 'CurrencyCode',
                                    queryMode: 'local',
                                    typeAhead: true,
                                    minChars: 2,
                                    allowBlank: false,
                                    forceSelection: true,
                                    //triggerAction: '',
                                    listeners: {
                                        /*blur: function(field, The, eOpts) {
                                            if (field.value !== null) {
                                                //me.onSaveClick();
                                            }
                                        },*/
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
                                    itemId: 'custcreditlimit',
                                    margin: '0 0 0 5',
                                    columnWidth: 0.35,
                                    name: 'CustCreditLimit',
                                    fieldLabel: 'Credit Limit',
                                    fieldStyle: 'text-align: right;',
                                    minValue: 0,
                                    hideTrigger: true,
                                    useThousandSeparator: true,
                                    decimalPrecision: 2,
                                    alwaysDisplayDecimals: true,
                                    allowNegative: false,
                                    alwaysDecimals: true,
                                    thousandSeparator: ','
                                }]
                            }]
                        },
                        // Contacts Panel
                        {
                            xtype: 'panel',
                            title: 'Contacts',
                            items: [{
                                xtype: 'gridpanel',
                                itemId: 'gridcontacts',
                                store: storeContacts,
                                pageSize: 11,
                                minHeight: 200,
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

                                            var form = new CBH.view.customers.CustomerContacts();
                                            form.loadRecord(record);
                                            form.center();
                                            form.show();
                                        },
                                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                                        tootip: 'view details'
                                    }]
                                }],
                                tbar: [{
                                        xtype: 'searchfield',
                                        width: 400,
                                        itemId: 'contactsearchfield',
                                        //name:'searchField',
                                        listeners: {
                                            'triggerclick': function(field) {
                                                me.onContactSearchFieldChange();
                                            }
                                        }
                                    },
                                    // 'Search',
                                    // {
                                    //     xtype: 'textfield',
                                    //     name: 'searchField',
                                    //     itemId: 'contactsearchfield',
                                    //     hideLabel: true,
                                    //     width: 400,
                                    //     listeners: {
                                    //         change: {
                                    //             fn: me.onContactSearchFieldChange,
                                    //             scope: this,
                                    //             buffer: 100
                                    //         }
                                    //     }
                                    // }, 
                                    // {
                                    //     xtype: 'button',
                                    //     cls:'x-btn-toolbar-small-cus',
                                    //     iconCls: 'app-search',
                                    //     tooltip: 'Search',
                                    //     handler: me.onContactSearchFieldChange,
                                    //     scope: me
                                    // },
                                    {
                                        xtype: 'component',
                                        flex: 1
                                    }, {
                                        text: 'Add',
                                        itemId: 'addcontact',
                                        handler: function() {
                                            currentRecord = this.up('form').getRecord();
                                            var custkey = currentRecord.data.CustKey;
                                            record = new CBH.model.customers.CustomerContacts({
                                                ContactCustKey: custkey
                                            });

                                            var form = new CBH.view.customers.CustomerContacts();
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
                                    itemId: 'contactpagingtoolbar',
                                    store: storeContacts,
                                    displayInfo: true,
                                    displayMsg: 'Displaying records {0} - {1} of {2}',
                                    emptyMsg: "No records to display"
                                }),
                                selType: 'rowmodel',
                                listeners: {
                                    selectionchange: function(view, records) {
                                        this.down('#deletecontact').setDisabled(!records.length);
                                    },
                                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                                        var form = new CBH.view.customers.CustomerContacts();
                                        form.loadRecord(record);
                                        form.callerForm = this.up('form');
                                        form.show();
                                    }
                                }
                            }]
                        },
                        // Ship Address Panel
                        {
                            xtype: 'panel',
                            title: 'Shipping Address',
                            items: [{
                                xtype: 'gridpanel',
                                itemId: 'gridshipaddress',
                                minHeight: 200,
                                columns: [{
                                    xtype: 'rownumberer',
                                    format: '00,000'
                                }, {
                                    xtype: 'checkcolumn',
                                    text: 'Default',
                                    dataIndex: 'ShipDefault'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Name',
                                    dataIndex: 'ShipName'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Address',
                                    dataIndex: 'x_ShipAddress',
                                    flex: 1
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'City',
                                    dataIndex: 'ShipCity'
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
                                            var form = Ext.widget('customershipaddress');
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
                                    itemId: 'addshipaddress',
                                    handler: function() {
                                        currentRecord = this.up('form').getRecord();
                                        var custkey = currentRecord.data.CustKey;
                                        var form = Ext.widget('customershipaddress');
                                        record = Ext.create('CBH.model.customers.CustomerShipAddress', {
                                            ShipCustKey: custkey
                                        });
                                        form.loadRecord(record);
                                        form.center();
                                        form.callerForm = this.up('form');
                                        form.show();
                                    },
                                    disabled: true
                                }, {
                                    itemId: 'deleteshipaddress',
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
                                        this.down('#deleteshipaddress').setDisabled(!records.length);
                                    },
                                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                                        var form = new CBH.view.customers.CustomerShipAddress();
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
                // Toolbar Header
                {
                    xtype: 'formtoolbar',
                    itemId: 'FormToolbar',
                    dock: 'top',
                    store: me.storeNavigator,
                    //navigationEnabled: true,
                    listeners: {
                        beginedit: {
                            fn: me.onBeginEdit,
                            scope: me
                        },
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
                },
                // Bottom Toolbar
                {
                    xtype: 'toolbar',
                    dock: 'bottom',
                    ui: 'footer',
                    items: [{
                        xtype: 'textfield',
                        name: 'CustCreatedBy',
                        readOnly: true,
                        fieldLabel: 'Created By',
                        editable: false
                    }, {
                        xtype: 'datetimefield',
                        name: 'CustCreatedDate',
                        readOnly: true,
                        fieldLabel: 'Created Date',
                        hideTrigger: true,
                        editable: false
                    }, {
                        xtype: 'textfield',
                        name: 'CustModifiedBy',
                        readOnly: true,
                        fieldLabel: 'Modified By',
                        editable: false
                    }, {
                        xtype: 'datetimefield',
                        name: 'CustModifiedDate',
                        readOnly: true,
                        fieldLabel: 'Modified Date',
                        hideTrigger: true,
                        editable: false
                    }, {
                        xtype: 'component',
                        flex: 1
                    }, {
                        xtype:'button',
                        text:'View/Print Web Extranet Usage',
                        disabled: true
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
        var me = this,
            toolbar = me.down('#FormToolbar');
    },

    onAfterLoadRecord: function(toolbar, record) {
        var me = this;

        if (record.phantom) {
            me.down('#addcontact').setDisabled(true);
            me.down('#addshipaddress').setDisabled(true);
            return;
        }

        me.down('#addcontact').setDisabled(false);
        me.down('#addshipaddress').setDisabled(false);

        var currentCustKey = record.data.CustKey;

        curRec = record;

        var gridcontacts = me.down('#gridcontacts');
        Ext.Msg.wait('Loading Data', 'Wait');

        var filterCust = new Ext.util.Filter({
            property: 'ContactCustKey',
            value: record.data.CustKey
        });

        var storeContacts = new CBH.store.customers.CustomerContacts().load({
            filters: [filterCust],
            callback: function() {
                gridcontacts.reconfigure(storeContacts);
                me.down('#contactpagingtoolbar').bindStore(storeContacts);

                var storeShipAddress = new CBH.store.customers.CustomerShipAddress().load({
                    params: {
                        page: 0,
                        start: 0,
                        limit: 0,
                        custkey: currentCustKey
                    },
                    callback: function() {
                        var grid = me.down('#gridshipaddress');
                        grid.reconfigure(storeShipAddress);
                        Ext.Msg.hide();
                        storeShipAddress.lastOptions.callback = null;
                    }
                });
            }
        });
    },

    onAddClick: function(toolbar, record) {
        var me = toolbar.up('form'),
            grid = me.down('#gridcontacts');

        me.down('#gridcontacts').store.removeAll();
        me.down('#gridshipaddress').store.removeAll();

        record.data.CustLanguageCode = 'en';

        me.down('field[name=CustName]').focus(true, 200);
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

        var isdirty = record.phantom;

        record.data.CustModifiedBy = (!isdirty) ? CBH.GlobalSettings.getCurrentUserName() : null;

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
                                    callback: function() {}
                                });
                                if (toolbar.store.getCount() > 0) {
                                    toolbar.gotoAt(prevRec);
                                } else {
                                    toolbar.up('form').up('panel').destroy();
                                }
                                Ext.Msg.hide();
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

    onBeginEdit: function(toolbar, record) {
        var me = this;
        me.down('field[name=CustName]').focus(true, 200);
    },

    onContactSearchFieldChange: function() {
        var form = this,
            field = form.down('#contactsearchfield'),
            grid = form.down('#gridcontacts'),
            record = form.down('#FormToolbar').getCurrentRecord();

        grid.store.removeAll();

        if (!String.isNullOrEmpty(field.value)) {
            grid.store.loadPage(1, {
                params: {
                    query: field.value,
                    custkey: record.data.CustKey
                },
                callback: function() {
                    form.down('#contactpagingtoolbar').bindStore(this);
                }
            });
        } else {
            grid.store.loadPage(1, {
                params: {
                    custkey: record.data.CustKey
                },
                callback: function() {
                    form.down('#contactpagingtoolbar').bindStore(this);
                }
            });
        }
    },

    onCloseForm: function() {
        var me = this;
        var custField = this.callerForm.down('field[name=FileCustKey]');
        var record = me.getForm().getRecord();
        if (custField && record) {
            var custname = record.data.CustName;
            custField.setRawValue(custname);
            custField.doRawQuery();
            custField.focus(true, 400);
        }
    }
});
