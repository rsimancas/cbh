Ext.define('Ext.ux.form.ToolbarOld', {
    extend: 'Ext.toolbar.Toolbar',
    height: 30,
    padding: '2 1 1 5',
    alias: 'widget.formtoolbar',
    //alternateClassName: 'Ext.PagingToolbar',
    requires: ['Ext.toolbar.TextItem', 'Ext.form.field.Number'],
    mixins: {
        bindable: 'Ext.util.Bindable'    
    },
    /**
     * @cfg {Ext.data.Store} store (required)
     * The {@link Ext.data.Store} the paging toolbar should use as its data source.
     */

    /**
     * @cfg {Boolean} displayInfo
     * true to display the displayMsg
     */
    displayInfo: false,

    /**
     * @cfg {Boolean} prependButtons
     * true to insert any configured items _before_ the paging buttons.
     */
    prependButtons: false,

    //<locale>
    /**
     * @cfg {String} displayMsg
     * The paging status message to display. Note that this string is
     * formatted using the braced numbers {0}-{2} as tokens that are replaced by the values for start, end and total
     * respectively. These tokens should be preserved when overriding this string if showing those values is desired.
     */
    //displayMsg : 'Displaying {0} - {1} of {2}',
    //</locale>

    //<locale>
    /**
     * @cfg {String} emptyMsg
     * The message to display when no records are found.
     */
    emptyMsg : 'No data to display',
    //</locale>

    //<locale>
    /**
     * @cfg {String} beforePageText
     * The text displayed before the input item.
     */
    beforeRecordText : 'Record',
    //</locale>

    //<locale>
    /**
     * @cfg {String} afterPageText
     * Customizable piece of the default paging text. Note that this string is formatted using
     * {0} as a token that is replaced by the number of total pages. This token should be preserved when overriding this
     * string if showing the total page count is desired.
     */
    afterRecordText : 'of {0}',
    //</locale>

    //<locale>
    /**
     * @cfg {String} firstText
     * The quicktip text displayed for the first page button.
     * **Note**: quick tips must be initialized for the quicktip to show.
     */
    firstText : 'First Record',
    //</locale>

    //<locale>
    /**
     * @cfg {String} prevText
     * The quicktip text displayed for the previous page button.
     * **Note**: quick tips must be initialized for the quicktip to show.
     */
    prevText : 'Previous Record',
    //</locale>

    //<locale>
    /**
     * @cfg {String} nextText
     * The quicktip text displayed for the next page button.
     * **Note**: quick tips must be initialized for the quicktip to show.
     */
    nextText : 'Next Record',
    //</locale>

    //<locale>
    /**
     * @cfg {String} lastText
     * The quicktip text displayed for the last page button.
     * **Note**: quick tips must be initialized for the quicktip to show.
     */
    lastText : 'Last Record',
    //</locale>

    //<locale>
    /**
     * @cfg {String} refreshText
     * The quicktip text displayed for the Refresh button.
     * **Note**: quick tips must be initialized for the quicktip to show.
     */
    refreshText : 'Refresh Record',
    //</locale>

    /**
     * @cfg {Number} inputItemWidth
     * The width in pixels of the input field used to display and change the current page number.
     */
    inputItemWidth : 40,


    store: null,


    currentRecord: 0,
    lastPage: 1,

    /* 
    MODES Toolbar
        Empty
        Normal
        Editing
        New
        Unique
    */
    navigationEnabled: false,
    isEditing: false,
    addEnabled: true,

    /**
     * Gets the standard paging items in the toolbar
     * @private
     */
    getPagingItems: function() {
        var me = this,
            addItems = null;

        if(me.addEnabled) {
            addItems = [
            {
                itemId: 'add',
                iconCls: 'app-page-add',
                cls:'x-btn-toolbar-small-cus',
                tooltip: 'add record',
                //formBind: true,
                listeners: {
                    click: function (btn, e, eOpts, model) {
                        var me = this.up('formtoolbar'),
                            modelName = me.getStore().getProxy().getModel().modelName,
                            newModel = null;

                        newModel = (!model) ? Ext.create(modelName) : model;

                        if(me.fireEvent('addrecord', me, newModel) !== false) {
                            me.store.insert(0,newModel);
                            me.gotoAt(1);
                            var btn = me.down('#edit');
                            btn.fireEvent('click', btn);
                        }
                    }
                }
            }
            ];
        }

        returnedItems = [
        {
            itemId: 'edit',
            iconCls: 'app-page-edit',
            cls:'x-btn-toolbar-small-cus',
            tooltip: 'edit record',
            //formBind: true,
            listeners: {
                click: function () {
                    var me = this.up('formtoolbar');
                    if(me.fireEvent('beforeedit', me, me.getCurrentRecord()) !== false) {
                        me.setToolMode('Editing');
                        me.setReadOnlyFormFields(false);
                        me.fireEvent('beginedit', me, me.getCurrentRecord());
                    }
                }
            }
        },
        {
            itemId: 'undo',
            iconCls: 'app-page-undo',
            cls:'x-btn-toolbar-small-cus',
            tooltip: 'undo changes',
            //formBind: true,
            listeners: {
                click: function () {
                    var me = this.up('formtoolbar');

                    me.fireEvent('undochanges', me, me.getCurrentRecord());
                    me.setReadOnlyFormFields(true);
                    record = me.getCurrentRecord();
                    if(record.phantom) {
                        me.store.remove(record);
                        if(me.store.getCount() > 0) {
                            me.gotoAt(1);
                        } else {
                            parent = me.up('form').up('panel');
                            if(parent && parent.iconCls === 'tabs') {
                                parent.close();
                            } else {
                                me.up('form').close();
                            }
                        }
                    } else {
                        me.doRefresh();
                    }
                }
            }
        },
        {
            itemId: 'save',
            iconCls: 'app-page-save',
            cls:'x-btn-toolbar-small-cus',
            tooltip: 'save (F8)',
            formBind: true,
            listeners: {
                click: function () {
                    var me = this.up('formtoolbar');
                    if(me.fireEvent('beforesavechanges', me, me.getCurrentRecord()) !== false) {
                        me.fireEvent('savechanges', me, me.getCurrentRecord());
                        me.setReadOnlyFormFields(true);
                    }
                }
            }
        },
        {
            itemId: 'delete',
            iconCls: 'app-page-delete',
            cls:'x-btn-toolbar-small-cus',
            tooltip: 'delete record',
            //formBind: true,
            listeners: {
                click: function () {
                    Ext.Msg.show({
                        title:'Question',
                        msg: 'Do you want to delete this record?',
                        buttons: Ext.Msg.YESNO,
                        icon: Ext.Msg.WARNING,
                        fn: function(e) {
                            if(e === "yes") {
                                //var me = this.up('formtoolbar');
                                if(me.fireEvent('beforedeleterecord', me, me.getCurrentRecord()) !== false) {
                                    me.fireEvent('deleterecord', me, me.getCurrentRecord());
                                }
                            }
                        }
                    });
                }
            }
        },
        {
            itemId: 'refresh',
            tooltip: me.refreshText,
            overflowText: me.refreshText,
            iconCls: 'app-page-refresh',
            cls:'x-btn-toolbar-small-cus',
            //formBind: true,
            handler: me.doRefresh,
            scope: me
        }];

        if(me.navigationEnabled) {
            returnedItems = returnedItems.concat([
            {
                xtype: 'tbseparator',
                itemId: 'SepBeforeNavigationBar'
            },
            {
                itemId: 'first',
                tooltip: me.firstText,
                overflowText: me.firstText,
                iconCls: 'app-page-first',
                cls:'x-btn-toolbar-small-cus',
                disabled: true,
                formBind: true,
                handler: me.moveFirst,
                scope: me
            },{
                itemId: 'prev',
                tooltip: me.prevText,
                overflowText: me.prevText,
                //iconCls: Ext.baseCSSPrefix + 'tbar-page-prev',
                iconCls: 'app-page-prev',
                cls:'x-btn-toolbar-small-cus',
                disabled: true,
                formBind: true,
                handler: me.movePrevious,
                scope: me
            },
            {
                xtype: 'numberfield',
                itemId: 'inputItem',
                cls: Ext.baseCSSPrefix + 'tbar-page-number',
                allowDecimals: false,
                minValue: 1,
                fieldStyle: 'text-align: right;',
                hideTrigger: true,
                enableKeyEvents: true,
                keyNavEnabled: false,
                selectOnFocus: true,
                submitValue: false,
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px',
                isFormField: false,
                width: me.inputItemWidth,
                formBind: true,
                margins: '-1 2 3 2',
                listeners: {
                    scope: me,
                    keydown: me.onKeyDown,
                    blur: me.onBlur
                }
            },
            {
                xtype: 'tbtext',
                itemId: 'formAfterTextItem',
                text: Ext.String.format(me.afterRecordText, 1),
                style: 'font-size:11px;'
            },
            {
                itemId: 'next',
                tooltip: me.nextText,
                overflowText: me.nextText,
                iconCls: 'app-page-next',
                cls:'x-btn-toolbar-small-cus',
                disabled: true,
                formBind: true,
                handler: me.moveNext,
                scope: me
            },
            {
                itemId: 'last',
                tooltip: me.lastText,
                overflowText: me.lastText,
                iconCls: 'app-page-last',
                cls:'x-btn-toolbar-small-cus',
                disabled: true,
                formBind: true,
                handler: me.moveLast,
                scope: me
            }
            ]);
        }

        if(me.addEnabled) {
            returnedItems = addItems.concat(returnedItems);
        }

        return returnedItems
    },

    initComponent : function(){
        var me = this,
            pagingItems = me.getPagingItems(),
            userItems   = me.items || me.buttons || [];

        if (me.prependButtons) {
            me.items = userItems.concat(pagingItems);
        } else {
            me.items = pagingItems.concat(userItems);
        }
        delete me.buttons;

        // if (me.displayInfo) {
        //     me.items.push('->');
        //     me.items.push({xtype: 'tbtext', itemId: 'displayItem'});
        // }

        me.callParent();

        me.addEvents(
            'beforechange',
            'change',
            'afterloadrecord',
            'beforeedit',
            'beginedit',
            'beforedeleterecord',
            'deleterecord',
            'beforesavechanges',
            'undochanges',
            'savechanges'
        );

        me.on('beforerender', me.onLoad, me, {single: true});

        me.bindStore(me.store || 'ext-empty-store', true);
    },

    // @private
    updateInfo : function(){
        var me = this,
            displayItem = me.child('#displayItem'),
            store = me.store,
            count, msg;

        if (displayItem) {
            count = store.totalCount;
            if (count === 0) {
                msg = me.emptyMsg;
            } else {
                //msg = Ext.String.format
                msg = me.emptyMsg;
            }
            displayItem.setText(msg);
        }
    },

    // @private
    onLoad : function(){
        var me = this,
            afterText,
            recordCount,
            count,
            isEmpty,
            item;

        if(this.currentRecord > 1 ) return;

        count = me.store.totalCount;
        isEmpty = count === 0;
        if (!isEmpty) {
            this.currentRecord = 1;
            recordCount = count;
            afterText = Ext.String.format(me.afterRecordText, isNaN(recordCount) ? 1 : recordCount);
        } else {
            this.currentRecord = 0;
            recordCount = 0;
            afterText = Ext.String.format(me.afterRecordText, 0);
        }

        Ext.suspendLayouts();
        item = me.child('#formAfterTextItem');
        if (item) {    
            item.setText(afterText);
        }
        item = me.getInputItem();
        if (item) {
            item.setDisabled(isEmpty).setValue(this.currentRecord);
        }

        me.refreshButtons();
        me.updateInfo();
        Ext.resumeLayouts(true);

        if (me.rendered) {
            me.fireEvent('change', me, me.getCurrentRecord());
        }
    },
    
    setChildDisabled: function(selector, disabled){
        var item = this.child(selector);
        if (item) {
            item.setDisabled(disabled);
        }
    },

    // @private
    onLoadError : function(){
        if (!this.rendered) {
            return;
        }
        this.setChildDisabled('#refresh', false);
    },
    
    getInputItem: function(){
        var me = this;
        return me.child('#inputItem');
    },

    // @private
    readRecordFromInput : function(recordData){
        var inputItem = this.getInputItem(),
            recordNum = false,
            v;

        if (inputItem) {
            v = inputItem.getValue();
            recordNum = parseInt(v, 10);
            if (!v || isNaN(recordNum)) {
                inputItem.setValue(this.currentRecord);
                return false;
            }
        }
        return recordNum;
    },

    onRecordFocus : function(){
        var inputItem = this.getInputItem();
        if (inputItem) {
            inputItem.select();
        }
    },

    // @private
    onRecordBlur : function(e){
        var inputItem = this.getInputItem(),
            curRecord;
            
        if (inputItem) {
            curRecord = this.currentRecord;
            inputItem.setValue(curRecord);
        }
    },

    // @private
    onKeyDown : function(field, e){
        var me = this,
            k = e.getKey(),
            increment = e.shiftKey ? 10 : 1,
            recordNum;

        if (k == e.RETURN || k == e.TAB) {
            e.stopEvent();
            recordNum = field.value;
            recordNum = Math.min(Math.max(1, recordNum), me.store.totalCount);
            if(me.fireEvent('beforechange', me, recordNum) !== false){
                me.gotoAt(recordNum);
            }
        } else if (k == e.HOME || k == e.END) {
            e.stopEvent();
            recordNum = k == e.HOME ? 1 : me.store.totalCount;
            field.setValue(recordNum);
            me.gotoAt(recordNum);
        } else if (k == e.UP || k == e.PAGE_UP || k == e.DOWN || k == e.PAGE_DOWN) {
            e.stopEvent();
            recordNum = field.value;
            if (k == e.UP || k == e.PAGE_UP) {
                increment *= -1;
            }
            recordNum += increment;
            if (recordNum >= 1 && recordNum <= me.store.totalCount) {
                field.setValue(recordNum);
                me.gotoAt(recordNum);
            }
        }
    },

    // @private
    beforeLoad : function(){
        if (this.rendered) {
            this.setChildDisabled('#refresh', true);
        }
    },

    /**
     * Move to the first page, has the same effect as clicking the 'first' button.
     */
    moveFirst : function(){
        var me = this;

        if (me.fireEvent('beforechange', me, 1) !== false){
            //this.store.loadRecord(1);
            // me.currentRecord = 1 ;
            // this.getInputItem().setValue(me.currentRecord);
            // me.refreshButtons();
            // me.up('form').loadRecord(me.store.getAt(me.currentRecord - 1));
            // me.fireEvent('afterloadrecord', me, 1);
            me.gotoAt(1);
        }
    },

    /**
     * Move to the previous page, has the same effect as clicking the 'previous' button.
     */
    movePrevious : function(){
        var me = this,
            prev = me.currentRecord - 1;


        if (prev > 0) {
            me.gotoAt(prev);
        }
    },

    /**
     * Move to the next page, has the same effect as clicking the 'next' button.
     */
    moveNext : function(){
        var me = this,
            total = me.store.totalCount,
            next = me.currentRecord + 1;

        if(next <= total) {
            me.gotoAt(next);
        }
    },

    /**
     * Move to the last page, has the same effect as clicking the 'last' button.
     */
    moveLast : function(){
        var me = this,
            last = me.store.totalCount;

        me.gotoAt(last);
    },

    /**
     * Refresh the current record, has the same effect as clicking the 'refresh' button.
     */
    doRefresh : function(){
        var me = this;
        if(me.currentRecord > 0) {
            me.gotoAt(me.currentRecord);
        }
    },
    
    getStoreListeners: function() {
        return {
            beforeload: this.beforeLoad,
            load: this.onLoad,
            exception: this.onLoadError
        };
    },

    /**
     * Unbinds the paging toolbar from the specified {@link Ext.data.Store} **(deprecated)**
     * @param {Ext.data.Store} store The data store to unbind
     */
    unbind : function(store){
        this.bindStore(null);
    },

    /**
     * Binds the paging toolbar to the specified {@link Ext.data.Store} **(deprecated)**
     * @param {Ext.data.Store} store The data store to bind
     */
    bind : function(store){
        this.bindStore(store);
    },

    // @private
    onDestroy : function(){
        this.unbind();
        this.callParent();
    },

    refreshButtons: function() {
        var me = this,
            count = me.store.totalCount;

        isEmpty = count === 0;
        isUnique = count === 1;

        if(isEmpty) {
            me.setToolMode('Empty');
        } else if(isUnique) {
            me.setToolMode('Unique');
        } else {
            me.setToolMode('Normal');
        }
    },

    setToolMode: function(mode) {
        var me = this;

        me.isEditing = false;
        me.items.items.forEach(function(btn){btn.setVisible(true);});
        me.setReadOnlyFormFields(true);
        if(me.navigationEnabled)
        me.down('#inputItem').setVisible(true);

        switch (mode) {
            case 'Normal':
                me.down('#save').setVisible(false);
                me.down('#undo').setVisible(false);
                me.down('#refresh').setVisible(true);

                if(me.navigationEnabled) {
                    me.down('#SepBeforeNavigationBar').setVisible(true);
                    me.down('#first').setVisible(true);
                    me.down('#prev').setVisible(true);
                    me.down('#next').setVisible(true);
                    me.down('#last').setVisible(true);
                    me.down('#inputItem').setVisible(true);
                    me.down('#formAfterTextItem').setVisible(true);
                }
                break;

            case 'Editing':
                me.isEditing = true;
                if(me.addEnabled) me.down('#add').setVisible(false);
                me.down('#edit').setVisible(false);
                me.down('#delete').setVisible(false);
                if(me.navigationEnabled) {
                    me.down('#SepBeforeNavigationBar').setVisible(false);
                    me.down('#first').setVisible(false);
                    me.down('#prev').setVisible(false);
                    me.down('#next').setVisible(false);
                    me.down('#last').setVisible(false);
                    me.down('#inputItem').setVisible(false);
                    me.down('#formAfterTextItem').setVisible(false);
                }
                me.down('#refresh').setVisible(false);
                break;

            case 'Unique':
                me.down('#save').setVisible(false);
                me.down('#undo').setVisible(false);
                if(me.navigationEnabled) {
                    me.down('#SepBeforeNavigationBar').setVisible(false);
                    me.down('#first').setVisible(false);
                    me.down('#prev').setVisible(false);
                    me.down('#next').setVisible(false);
                    me.down('#last').setVisible(false);
                    me.down('#inputItem').setVisible(false);
                    me.down('#formAfterTextItem').setVisible(false);
                }
                break;

            case 'Empty':
                me.down('#edit').setVisible(false);
                me.down('#save').setVisible(false);
                me.down('#delete').setVisible(false);
                me.down('#undo').setVisible(false);
                me.down('#refresh').setVisible(false);
                if(me.navigationEnabled) {
                    me.down('#SepBeforeNavigationBar').setVisible(false);
                    me.down('#first').setVisible(false);
                    me.down('#prev').setVisible(false);
                    me.down('#next').setVisible(false);
                    me.down('#last').setVisible(false);
                    me.down('#inputItem').setVisible(false);
                    me.down('#formAfterTextItem').setVisible(false);
                }
                me.down('#refresh').setVisible(false);
                break;
        }

    },

    gotoAt: function(gotoRecord) {
         var me = this,
            curPage = me.store.currentPage,
            nextPage = me.store.getPageFromRecordIndex(gotoRecord - 1),
            pageSize = me.store.pageSize;

        if (curPage != nextPage) {
            //Ext.suspendLayouts();
            var lastOpt = me.store.lastOptions,
                lastParams = (lastOpt) ? lastOpt.params : null,
                filters = (lastOpt) ? lastOpt.filters : null;

            me.store.loadPage(nextPage, {params:lastParams, filters:[filters], callback: function(options) {
                me.gotoRecord(gotoRecord);
                //Ext.resumeLayouts(true);
            }});
            return;
        }

        me.gotoRecord(gotoRecord);
        //me.down("#inputItem").focus(true,200);
    },

    gotoRecord: function(gotoRecord) {

        var me = this,
            curPage = (me.store.currentPage) ? me.store.currentPage : 1,
            pageSize = me.store.pageSize;

        var gotoRecordAt = gotoRecord <= pageSize ? gotoRecord : pageSize - ((curPage * pageSize) - gotoRecord);


        if (me.fireEvent('beforechange', me, gotoRecord) !== false) {
            me.currentRecord = gotoRecord;

            recordCount = me.store.totalCount;

            afterText = Ext.String.format(me.afterRecordText, isNaN(recordCount) ? 1 : recordCount);

            Ext.suspendLayouts();
            if(me.navigationEnabled) {
                me.getInputItem().setValue(me.currentRecord);
                var item = me.down('#formAfterTextItem');
                if (item) {    
                    item.setText(afterText);
                }
                item = me.getInputItem();
                if (item) {
                    item.setValue(me.currentRecord);
                }
            }
            me.refreshButtons();
            Ext.resumeLayouts(true);

            me.up('form').loadRecord(me.getCurrentRecord());
            me.fireEvent('afterloadrecord', me, me.getCurrentRecord());
        };
    },

    getCurrentRecord: function() {
        var me = this,
            curPage = me.store.currentPage,
            pageSize = me.store.pageSize,
            gotoRecord = me.currentRecord,
            gotoRecordAt = gotoRecord <= pageSize ? gotoRecord : pageSize - ((curPage * pageSize) - gotoRecord);

        if(me.store.totalCount == 0 ) return null;
        
        return me.store.getAt(gotoRecordAt - 1);
    },

    setReadOnlyFormFields: function (bReadOnly) {
        var me = this,
            formpanel = me.up('form');

        formpanel.getForm().getFields().each (function (field) {
            if(field.editable)
                field.setReadOnly(bReadOnly);
        });
    },
});
