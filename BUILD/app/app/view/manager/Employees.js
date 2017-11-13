Ext.define('CBH.view.manager.Employees', {
    extend: 'Ext.form.Panel',
    alias: 'widget.Employees',

    layout: {
        type: 'column'
    },
    bodyPadding: 10,
    frameHeader: false,
    header: false,
    enableKeyEvents: true,

    storeNavigator: null,

    initComponent: function() {

        var me = this;

        var storeStates = new CBH.store.common.States().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        var storeTitles = new CBH.store.common.tsysEmployeeCodes().load({
            params: {
                page: 0,
                start: 0,
                limit: 0,
                fieldFilters: JSON.stringify({
                    fields: [
                        { name: 'TextLanguageCode', type: 'string', value: 'en' },
                        { name: 'TextCategory', type: 'int', value: 0 }
                    ]
                })
            }
        });

        var storeStatus = new CBH.store.common.tsysEmployeeCodes().load({
            params: {
                page: 0,
                start: 0,
                limit: 0,
                fieldFilters: JSON.stringify({
                    fields: [
                        { name: 'TextLanguageCode', type: 'string', value: 'en' },
                        { name: 'TextCategory', type: 'int', value: 1 }
                    ]
                })
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
                xtype: 'textfield',
                columnWidth: 0.5,
                fieldLabel: 'First Name',
                name: 'EmployeeFirstName',
                allowBlank: false
            }, {
                xtype: 'textfield',
                columnWidth: 0.5,
                margin: '0 0 0 5',
                fieldLabel: 'Last Name',
                name: 'EmployeeLastName',
                allowBlank: false
            }, {
                columnWidth: 1,
                xtype: 'textfield',
                fieldLabel: 'Address',
                name: 'EmployeeAddress1',
            }, {
                xtype: 'textfield',
                margin: '0 0 0 0',
                columnWidth: 0.3,
                fieldLabel: 'City',
                name: 'EmployeeCity'
            }, {
                xtype: 'combo',
                margin: '0 0 0 5',
                columnWidth: 0.4,
                fieldLabel: 'State',
                name: 'EmployeeState',
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
                name: 'EmployeeZip'
            }, {
                xtype: 'textfield',
                columnWidth: 1,
                fieldLabel: 'Phone',
                name: 'EmployeePhone',
            }, {
                xtype: 'textfield',
                columnWidth: 1,
                fieldLabel: 'Email',
                name: 'EmployeeEmail'
            }, {
                xtype: 'combo',
                columnWidth: 1,
                fieldLabel: 'Title',
                name: 'EmployeeTitleCode',
                displayField: 'Text',
                queryMode: 'local',
                typeAhead: true,
                minChars: 2,
                allowBlank: false,
                forceSelection: true,
                store: storeTitles,
                valueField: 'TextExpression',
                emptyText: 'Choose Title',
                listeners: {
                    beforequery: function(record) {
                        record.query = new RegExp(record.query, 'i');
                        record.forceAll = true;
                    }
                }
            }, {
                xtype: 'combo',
                columnWidth: 1,
                fieldLabel: 'Status',
                name: 'EmployeeStatusCode',
                displayField: 'Text',
                queryMode: 'local',
                typeAhead: true,
                minChars: 2,
                forceSelection: true,
                allowBlank: false,
                store: storeStatus,
                valueField: 'TextExpression',
                emptyText: 'Choose Status',
                listeners: {
                    beforequery: function(record) {
                        record.query = new RegExp(record.query, 'i');
                        record.forceAll = true;
                    }
                }
            }, {
                xtype: 'textfield',
                columnWidth: 1,
                fieldLabel: 'Peachtree ID',
                name: 'EmployeePeachtreeID'
            }, {
                xtype: 'fieldset',
                margin: '5 0 0 0',
                columnWidth: 1,
                layout: 'column',
                title: 'Login Access',
                padding: '0 10 10 10',
                items: [{
                    xtype: 'textfield',
                    columnWidth: 1,
                    fieldLabel: 'Login',
                    name: 'EmployeeLogin'
                }, {
                    xtype: 'textfield',
                    columnWidth: 1,
                    fieldLabel: 'Password',
                    emptyText: 'Enter Password',
                    name: 'EmployeePassword',
                    inputType: 'password'
                }, {
                    columnWidth: 0.25,
                    xtype: 'combo',
                    fieldLabel: 'Level',
                    displayField: 'name',
                    valueField: 'id',
                    name: 'EmployeeAccessLevel',
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 1,
                    forceSelection: true,
                    emptyText: 'choose level',
                    enableKeyEvents: true,
                    autoSelect: true,
                    selectOnFocus: true,
                    store: {
                        fields: ['name','id'],
                        data: [{
                            "name": "User",
                            "id": 3
                        }, {
                            "name": "Administrator",
                            "id": 2
                        }, {
                            "name": "Developer",
                            "id": 1
                        }]
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
                    itemId: 'EmployeeCreatedBy',
                    name: 'EmployeeCreatedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created By'
                }, {
                    xtype: 'datefield',
                    name: 'EmployeeCreatedDate',
                    itemId: 'EmployeeCreatedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created Date',
                    hideTrigger: true
                }, {
                    xtype: 'textfield',
                    name: 'EmployeeModifiedBy',
                    itemId: 'EmployeeModifiedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified By'
                }, {
                    xtype: 'datefield',
                    name: 'EmployeeModifiedDate',
                    itemId: 'EmployeeModifiedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified Date',
                    hideTrigger: true
                }, {
                    xtype: 'component',
                    flex: 1
                }]
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
        // var toolbar = me.down('#FormToolbar');

        // if(toolbar.store.getCount()===1 && toolbar.store.getAt(0).phantom) {
        //     toolbar.items.items.forEach(function(btn){btn.setVisible(false);});
        //     toolbar.down('#save').setVisible(true);
        // }

        var field = me.down('field[name=EmployeeFirstName]');
        field.focus(true, 200);
    },

    onAfterLoadRecord: function(tool, record) {
        var form = this;
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

    onAddClick: function(toolbar, record) {
        //var me = this
    },

    onBeginEdit: function(toolbar, record) {
        var me = this;
        me.down('field[name=EmployeeFirstName]').focus(true, 200);
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

        if (!record.phantom) {
            record.data.CurrencyModifiedDate = new Date();
        }

        Ext.Msg.wait('Saving Record...', 'Wait');

        record.save({
            success: function(e) {
                me.loadRecord(record);
                Ext.Msg.hide();
                toolbar.doRefresh();
                CBH.AppEvents.broadcast.fireEvent('EmployeesChanged');
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
    }
});
