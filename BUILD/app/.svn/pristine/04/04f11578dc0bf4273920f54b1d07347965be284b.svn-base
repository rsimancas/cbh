Ext.define('CBH.view.manager.EmployeeList', {
    extend: 'Ext.form.Panel',
    alias: 'widget.EmployeeList',

    bodyPadding: 10,
    layout: {
        type: 'anchor'
    },


    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;

        var me = this;

        var store = new CBH.store.common.Employees({pageSize: 11, autoLoad:false}).load();

        Ext.applyIf(me, {
            items: [{
                xtype: 'gridpanel',
                title: 'Employees',
                minHeight: 400,
                store: store,
                columns: [{
                    xtype: 'rownumberer',
                    format: '00,000'
                }, {
                    xtype: 'gridcolumn',
                    text: 'Full Name',
                    dataIndex: 'x_EmployeeFullName',
                    flex: 1
                }, {
                    xtype: 'gridcolumn',
                    text: 'Email',
                    dataIndex: 'EmployeeEmail',
                    flex: 1
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

                            storeToNavigate = new CBH.store.common.Employees().load({
                                params: {
                                    id: record.data.EmployeeKey
                                },
                                callback: function() {
                                    var form = Ext.widget('Employees', {
                                        storeNavigator: storeToNavigate
                                    });

                                    var tab = tabs.add({
                                        closable: true,
                                        iconCls: 'tabs',
                                        autoScroll: true,
                                        title: 'Employee Maintenance',
                                        items: [form]
                                    });


                                    form.down('#FormToolbar').gotoAt(1);

                                    tab.show();
                                }
                            });

                        },
                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                        tootip: 'view details'
                    }]
                }],
                tbar: [
                    // Search Field
                    {
                        xtype: 'searchfield',
                        width: '50%',
                        itemId: 'searchfield',
                        name: 'searchField',
                        listeners: {
                            'triggerclick': function(field) {
                                me.onSearchFieldChange();
                            }
                        }
                    }, {
                        xtype: 'component',
                        flex: 1
                    }, {
                        text: 'Add',
                        itemId: 'additem',
                        handler: function() {
                            var tabs = this.up('app_pageframe');

                            var storeToNavigate = new CBH.store.common.Employees();

                            var form = Ext.widget('Employees', {
                                storeNavigator: storeToNavigate
                            });

                            var tab = tabs.add({
                                closable: true,
                                iconCls: 'tabs',
                                autoScroll: true,
                                title: 'New Employee',
                                items: [form]
                            });

                            tab.show();

                            var btn = form.down('#FormToolbar').down('#add');
                            btn.fireEvent('click', btn);
                        }
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
                                                success: function() {}
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
                    itemId: 'pagingtoolbar',
                    store: store,
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
                        var tabs = this.up('app_pageframe');

                        var storeToNavigate = new CBH.store.common.Employees().load({
                            params: {
                                id: record.data.EmployeeKey
                            },
                            callback: function() {
                                var form = Ext.widget('Employees', {
                                    storeNavigator: storeToNavigate
                                });

                                var tab = tabs.add({
                                    closable: true,
                                    iconCls: 'tabs',
                                    autoScroll: true,
                                    title: 'Employee Maintenance',
                                    items: [form]
                                });


                                form.down('#FormToolbar').gotoAt(1);

                                tab.show();
                            }
                        });
                    }
                }
            }],
            listeners: {
                render: {
                    fn: me.onRenderForm,
                    scope: me
                }
            }
        });
        me.callParent(arguments);
    },

    onRenderForm: function() {
        var me = this;

        var grid = me.down('gridpanel');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        var field = me.down('#searchfield').focus(true, 200);
    },

    onSearchFieldChange: function() {
        var form = this,
            field = form.down('#searchfield'),
            fieldValue = field.getRawValue(),
            grid = form.down('gridpanel');

        grid.store.removeAll();

        if (!String.isNullOrEmpty(fieldValue)) {
            grid.store.loadPage(1, {
                params: {
                    query: fieldValue
                },
                callback: function() {
                    form.down('#pagingtoolbar').bindStore(this);
                }
            });
        } else {
            grid.store.loadPage(1, {
                callback: function() {
                    form.down('#pagingtoolbar').bindStore(this);
                }
            });
        }
    }
});