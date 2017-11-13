Ext.define('CBH.view.manager.JobRolesList', {
    extend: 'Ext.form.Panel',
    alias: 'widget.JobRolesList',
    title: 'Job Roles Maintenance',

    initComponent: function() {
        var me = this;

        var storeJobRoles = new CBH.store.jobs.JobRoles({remoteFilter: false}).load({
            params: { page: 0, start: 0, limit: 0},
            callback: function() {
                me.down("#gridmain").reconfigure(storeJobRoles);
            }
        });

        rowEditing = new Ext.grid.plugin.RowEditing({
            clicksToMoveEditor: 2,
            autoCancel: false,
            errorSummary: false,
            listeners: {
                beforeedit: {
                    delay: 100,
                    fn: function(item, e) {
                        this.getEditor().onFieldChange();
                    }
                },
                cancelEdit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up("gridpanel");
                        // Canceling editing of a locally added, unsaved record: remove it
                        if (context.record.phantom) {
                            grid.store.remove(context.record);
                            grid.up('panel').down('#searchfield').focus(true, 200);
                        }
                    }
                },
                edit: {
                    fn: function(editor, context) {
                        var grid = this.editor.up('gridpanel'),
                            record = context.record,
                            fromEdit = true,
                            isPhantom = record.phantom;

                        record.data.JobRoleModifiedBy = CBH.GlobalSettings.getCurrentUserName();

                        record.save({
                            callback: function() {
                                grid.store.reload({
                                    callback: function() {
                                        if (fromEdit && isPhantom)
                                            grid.up('panel').down("#addline").fireHandler();
                                    }
                                });
                            }
                        });
                    }
                }
            }
        });

        Ext.applyIf(me, {
            items: [{
                xtype: 'gridpanel',
                itemId: 'gridmain',
                autoScroll: true,
                columnWidth: 1,
                viewConfig: {
                    stripeRows: true
                },
                minHeight: 450,
                forceFit: true,
                store: null,
                columns: [{
                    xtype: 'numbercolumn',
                    sortable: true,
                    width: 60,
                    dataIndex: 'JobRoleSort',
                    text: 'Sort',
                    format: '000'
                }, {
                    xtype: 'gridcolumn',
                    flex: 1,
                    dataIndex: 'JobRoleDescription',
                    text: 'Name',
                    editor: {
                        xtype: 'textfield',
                        name: 'JobRoleDescription',
                        fieldStyle: 'text-transform:uppercase',
                        allowBlank: false,
                        listeners: {
                            change: function(field, newValue, oldValue) {
                                var form = field.up('panel');
                                form.onFieldChange();

                                if (!String.isNullOrEmpty(newValue)) field.setValue(newValue.toUpperCase());
                            }
                        }
                    }
                }, {
                    xtype: 'gridcolumn',
                    flex: 1,
                    dataIndex: 'JobRoleCreatedBy',
                    text: 'Created By'
                }, {
                    xtype: 'gridcolumn',
                    width: 120,
                    dataIndex: 'JobRoleCreatedDate',
                    text: 'Created Date',
                    renderer: Ext.util.Format.dateRenderer('m/d/Y H:i')
                }, {
                    xtype: 'gridcolumn',
                    flex: 1,
                    dataIndex: 'JobRoleModifiedBy',
                    text: 'Modified By'
                }, {
                    xtype: 'gridcolumn',
                    width: 120,
                    dataIndex: 'JobRoleModifiedDate',
                    text: 'Modified Date',
                    renderer: Ext.util.Format.dateRenderer('m/d/Y H:i')
                }, {
                    xtype: 'actioncolumn',
                    draggable: false,
                    width: 35,
                    resizable: false,
                    hideable: false,
                    stopSelection: false,
                    items: [{
                        handler: me.onClickActionColumn,
                        tooltip: 'Edit',
                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf040@FontAwesome';}
                    }]
                }],
                tbar: [{
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
                    itemId: 'addline',
                    xtype: 'button',
                    text: 'Add',
                    tooltip: 'Add (Ins)',
                    handler: function() {
                        rowEditing.cancelEdit();

                        var grid = this.up("gridpanel"),
                            count = grid.store.getCount(),
                            sort =  parseFloat(grid.store.max("JobRoleSort")) + 100;

                        // Create a model instance
                        var r = Ext.create('CBH.model.jobs.JobRoles', {
                            JobRoleSort: sort
                        });

                        grid.store.insert(count, r);
                        rowEditing.startEdit(r, 1);
                        rowEditing.editor.down('field[name=JobRoleDescription]').focus(true, 200);
                    }
                }, {
                    itemId: 'deleteline',
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
                }],
                selType: 'rowmodel',
                plugins: [rowEditing],
                /*bbar: new Ext.PagingToolbar({
                    itemId: 'pagingtoolbar',
                    store: storeJobRoles,
                    displayInfo: true,
                    displayMsg: 'Show {0} - {1} of {2}',
                    emptyMsg: "No records to show"
                }),*/
                listeners: {
                    selectionchange: function(view, records) {
                        this.down('#deleteline').setDisabled(!records.length);
                    },
                    validateedit: function(e) {
                        var myTargetRow = 6;

                        if (e.rowIdx == myTargetRow) {
                            e.cancel = true;
                            e.record.data[e.field] = e.value;
                        }
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
                },
            }

        });

        me.callParent(arguments);
    },

    onRenderForm: function() {
        var me = this;

        var grid = me.down('#gridmain');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }
    },

    registerKeyBindings: function(view, options) {
        /*var me = this;
        Ext.EventManager.on(view.getEl(), 'keyup', function(evt, t, o) {
                if (evt.keyCode === Ext.EventObject.INSERT) {
                    evt.stopEvent();
                    var btn = me.down('#addline');
                    btn.fireHandler();
                }
            },
            this);

        me.down('#searchfield').focus(true, 300);*/
    },

    onSearchFieldChange: function() {
        var form = this,
            field = form.down('#searchfield'),
            fieldValue = field.getRawValue(),
            grid = form.down('#gridmain'),
            store = grid.getStore();

        if(!String.isNullOrEmpty(fieldValue)) {
            store.filter([
                Ext.create('Ext.util.Filter', {
                    filterFn: function(item) {
                        var pattern = new RegExp(fieldValue, 'i'); 
                        return pattern.test(item.get("JobRoleDescription")) || 
                            pattern.test(item.get("JobRoleSort")) || 
                            pattern.test(item.get("JobRoleCreatedBy")) || 
                            pattern.test(item.get("JobRoleCreatedDate")) || 
                            pattern.test(item.get("JobRoleModifiedBy")) || 
                            pattern.test(item.get("JobRoleModifiedDate")); 
                    }, 
                    root: 'data'
                })
            ]);
        } else {
            // Clear the filter collection without updating the UI
            store.clearFilter(false);
        }
    },

    onClickActionColumn: function(view, rowIndex, colIndex, item, e, record) {
        var me = this.up('panel').up('panel');
        rowEditing.startEdit(record, 1);
        this.up('panel').editingPlugin.editor.down('field[name=JobRoleDescription]').focus(true, 200);
    }

});