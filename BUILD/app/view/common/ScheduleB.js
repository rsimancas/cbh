Ext.define('CBH.view.common.ScheduleB', {
    extend: 'Ext.form.Panel',
    alias: 'widget.ScheduleB',

    layout: {
        type: 'column'
    },
    bodyPadding: 10,
    frameHeader: false,
    header: false,
    enableKeyEvents: true,

    storeNavigator: null,

    SchBNum: 0,

    initComponent: function() {
        var me = this;

        storeLangs = new CBH.store.common.Languages().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        storeDesc = null;

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
                        var grid = rowEditing.editor.up('gridpanel');
                        // Canceling editing of a locally added, unsaved record: remove it
                        if (context.record.phantom) {
                            grid.store.remove(context.record);
                        }
                    }
                },
                edit: {
                    fn: function(rowEditing, context) {
                        var grid = rowEditing.editor.up('gridpanel'),
                            record = context.record;
                        record.save({
                            callback: function(records, operation, success) {
                                if (success) {
                                    var lastOpt = grid.store.lastOptions;
                                    lastOpt.params.SchBNum = record.data.SBLanguageSchBNum;
                                    grid.store.reload({
                                        params: lastOpt.params,
                                        callback: function() {}
                                    });
                                }
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
            items: [{
                xtype: 'fieldset',
                columnWidth: 1,
                layout: {
                    type: 'column'
                },
                padding: '0 10 10 10',
                collapsible: true,
                title: 'Schedule B Information',
                items: [{
                    xtype: 'textfield',
                    name: 'SchBNum',
                    fieldLabel: 'Schedule B/HTS#',
                    columnWidth: 0.5,
                    allowBlank: false,
                    //vtype:'arancel',
                    //maskRe: /^\d{4}?[\.]?\d{2}[\.]?\d{4}$/,
                    plugins: [new Ext.ux.InputTextMask('9999.99.9999')]
                }, {
                    margin: '0 0 0 5',
                    xtype: 'textfield',
                    name: 'SchBUnitOfMeasure',
                    fieldLabel: 'Unit Of Measure 1',
                    columnWidth: 0.25
                }, {
                    margin: '0 0 0 5',
                    xtype: 'textfield',
                    name: 'SchBUnitOfMeasure2',
                    fieldLabel: 'Unit Of Measure 2',
                    columnWidth: 0.25
                }, {
                    xtype: 'textfield',
                    name: 'SchBShortDescription',
                    fieldLabel: 'Short Description',
                    columnWidth: 1,
                    allowBlank: false
                }, {
                    xtype: 'textareafield',
                    name: 'SchBLongDescription',
                    fieldLabel: 'Long Description',
                    columnWidth: 1
                }, {
                    xtype: 'textfield',
                    name: 'SchBSITC',
                    fieldLabel: 'BSITC',
                    columnWidth: 0.25
                }, {
                    margin: '0 0 0 5',
                    xtype: 'textfield',
                    name: 'SchBNAICS',
                    fieldLabel: 'NAICS',
                    columnWidth: 0.25
                }, {
                    margin: '0 0 0 5',
                    xtype: 'textfield',
                    name: 'SchBEndUseClassification',
                    fieldLabel: 'End Use Class',
                    columnWidth: 0.25
                }, {
                    margin: '0 0 0 5',
                    xtype: 'textfield',
                    name: 'SchBHiTechClassification',
                    fieldLabel: 'Hi Tech Class',
                    columnWidth: 0.25
                }, {
                    margin: '25 0 0 0',
                    xtype: 'checkbox',
                    columnWidth: 0.25,
                    name: 'SchBUSDA',
                    labelSeparator: '',
                    hideLabel: true,
                    boxLabel: 'USDA'
                }, {
                    xtype: 'fieldcontainer',
                    columnWidth: 0.75,
                    layout: 'hbox',
                    items: [{
                        xtype: 'component',
                        flex: 1
                    }, {
                        margin: '25 0 0 0',
                        xtype: 'checkbox',
                        //columnWidth: 0.25,
                        name: 'SchBImport',
                        labelSeparator: '',
                        hideLabel: true,
                        boxLabel: 'Import'
                    }, {
                        margin: '25 0 0 10',
                        xtype: 'checkbox',
                        //columnWidth: 0.25,
                        name: 'SchBExport',
                        labelSeparator: '',
                        hideLabel: true,
                        boxLabel: 'Export'
                    }, {
                        margin: '25 0 0 10',
                        xtype: 'checkbox',
                        //columnWidth: 0.25,
                        name: 'SchBRetired',
                        labelSeparator: '',
                        hideLabel: true,
                        boxLabel: 'Retired'
                    }]
                }]
            }, {
                columnWidth: 1,
                xtype: 'panel',
                title: 'Descriptions',
                margin: '0 0 0 10',
                items: [{
                    xtype: 'gridpanel',
                    itemId: 'griddesc',
                    minHeight: 200,
                    store: storeDesc,
                    maxHeight: 600,
                    columns: [{
                        xtype: 'rownumberer',
                        format: '00,000',
                        width: 35,
                    }, {
                        xtype: 'gridcolumn',
                        text: 'Partida Arancelaria #',
                        dataIndex: 'x_SchBNumFormatted',
                        width: 130,
                        editor: {
                            xtype: 'textfield',
                            name: 'SBLanguageSchBSubNum',
                            displayField: 'SBLanguageSchBSubNum',
                            valueField: 'SBLanguageSchBSubNum'
                        }
                    }, {
                        xtype: 'gridcolumn',
                        text: 'Language',
                        dataIndex: 'x_Language',
                        width: 120,
                        editor: {
                            xtype: 'combo',
                            displayField: 'LanguageName',
                            valueField: 'LanguageCode',
                            name: 'SBLanguageCode',
                            enableKeyEvents: true,
                            forceSelection: true,
                            queryMode: 'local',
                            selectOnFocus: true,
                            emptyText: 'Choose Language',
                            allowBlank: false,
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
                        dataIndex: 'SBLanguageText',
                        flex: 1,
                        editor: {
                            xtype: 'textfield',
                            name: 'SBLanguageText',
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
                            var parentkey = me.down('#FormToolbar').getCurrentRecord().data.SchBNum;

                            // Create a model instance
                            var r = Ext.create('CBH.model.common.ScheduleBLanguage', {
                                SBLanguageSchBNum: me.SchBNum
                            });

                            var count = grid.getStore().count();
                            grid.store.insert(count, r);
                            rowEditing.startEdit(count, 1);
                        },
                        disabled: true
                    }, {
                        itemId: 'deletedesc',
                        text: 'Delete',
                        handler: function() {
                            var grid = this.up('gridpanel'),
                                sm = grid.getSelectionModel();

                            rowEditing.cancelEdit();

                            selection = sm.getSelection();

                            if (selection) {
                                selection[0].destroy({
                                    success: function() {
                                        grid.store.reload();
                                    }
                                });
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
            }],
            dockedItems: [{
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
            }, {
                xtype: 'toolbar',
                dock: 'bottom',
                ui: 'footer',
                items: [{
                    xtype: 'textfield',
                    name: 'SchBCreatedBy',
                    readOnly: true,
                    fieldLabel: 'Created By',
                    editable: false
                }, {
                    xtype: 'datetimefield',
                    name: 'SchBCreatedDate',
                    readOnly: true,
                    fieldLabel: 'Created Date',
                    hideTrigger: true,
                    editable: false
                }, {
                    xtype: 'textfield',
                    name: 'SchBModifiedBy',
                    readOnly: true,
                    fieldLabel: 'Modified By',
                    editable: false
                }, {
                    xtype: 'datetimefield',
                    name: 'SchBModifiedDate',
                    readOnly: true,
                    fieldLabel: 'Modified Date',
                    hideTrigger: true,
                    editable: false
                }, {
                    xtype: 'component',
                    flex: 1
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
                },
                close: function() {

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
        var toolbar = me.down('#FormToolbar');

        if (toolbar.store.getCount() === 1 && toolbar.store.getAt(0).phantom) {
            toolbar.items.items.forEach(function(btn) {
                btn.setVisible(false);
            });
            toolbar.down('#save').setVisible(true);
        }

        var field = me.down('field[name=SchBNum]');
        field.focus(true, 200);
    },

    onAfterLoadRecord: function(tool, record) {
        var me = this;
        var parentkey = record.data.SchBNum || me.SchBNum;

        if (record.phantom) {
            me.down('#adddesc').setDisabled(true);
        } else {
            me.down('field[name=SchBNum]').setDisabled(false);
            me.down('field[name=SchBNum]').setValue(record.data.x_SchBNumFormatted);
        }

        curRec = record;

        Ext.Msg.wait('Loading Descriptions', 'Wait');
        var storeDesc = new CBH.store.common.ScheduleBLanguage().load({
            params: {
                page: 0,
                start: 0,
                limit: 0,
                SchBNum: parentkey
            },
            callback: function() {

                var grid = me.down('#griddesc');
                grid.reconfigure(this);
                Ext.Msg.hide();
            }
        });
    },

    onAddClick: function(toolbar, record) {
        var me = this,
            grid = me.down('#griddesc');

        grid.store.removeAll();

        me.down('#adddesc').setDisabled(true);
        me.down('#FormToolbar').down('#add').setDisabled(true);
        me.down('field[name=SchBNum]').focus(true, 200);
    },

    onBeginEdit: function(toolbar, record) {
        var me = this;

        if (record.phantom) {
            setTimeout(function() {
                me.down('field[name=SchBNum]').focus(true, 200);
                me.down('field[name=SchBNum]').setReadOnly(false);
            }, 200);
        } else {
            setTimeout(function() {
                me.down('field[name=SchBUnitOfMeasure]').focus(true, 200);
                me.down('field[name=SchBNum]').setReadOnly(true);
            }, 200);
        }
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

        me.getEl().mask('Saving Record...');

        if (!record.phantom) {
            record.data.SchBModifiedBy = CBH.GlobalSettings.getCurrentUserName();
        }

        var SchBNum = record.data.SchBNum;

        while (SchBNum.indexOf('.') != -1) {
            SchBNum = SchBNum.replace('.', '');
        }

        record.set('SchBNum', SchBNum);

        record.save({
            callback: function(records, operation, success) {
                me.down('#FormToolbar').down('#add').setDisabled(false);
                me.down('#adddesc').setDisabled(false);

                if (success) {
                    me.getEl().unmask();
                    me.SchBNum = (records.length) ? records[0].data.SchBNum : records.data.SchBNum;
                    //me.loadRecord((records.length) ? records[0] : records);
                    var grid = me.down('gridpanel');
                    var lastOpt = grid.store.lastOptions;
                    lastOpt.params.SchBNum = me.SchBNum;
                    grid.store.lastOptions = lastOpt;
                    var tbLastOpt = toolbar.store.lastOptions;
                    tbLastOpt.params.id = me.SchBNum;
                    tbLastOpt.params.page = 0;
                    tbLastOpt.params.start = 0;
                    tbLastOpt.params.limit = 1;
                    toolbar.store.lastOptions = tbLastOpt;
                    var savedRecord = (records.length) ? records[0] : records;
                    savedRecord.idProperty = "";
                    toolbar.doRefresh(null, null, savedRecord);


                    /*var storeDesc = new CBH.store.common.ScheduleBLanguage().load({
                        params: {
                            page: 0,
                            start: 0,
                            limit: 0,
                            SchBNum: me.SchBNum
                        },
                        callback: function() {
                            var grid = me.down('#griddesc');
                            grid.reconfigure(storeDesc);
                            me.down('#adddesc').setDisabled(false);
                            me.getEl().unmask();
                            toolbar.doRefresh();
                        }
                    });*/
                } else {
                    me.getEl().ummask();
                }
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
                            callback: function(records, operation, success) {
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
    }
});
