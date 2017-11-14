// fix some bugs at combo box picker
Ext.define('Overrides.form.ComboBox', {
	override: 'Ext.form.ComboBox',
    typeAheadDelay: 1500,
    filters: null,
    queryBy: null,

    onLoad: function(store, records, success) {
        var me = this;

        if (me.ignoreSelection > 0) {
            --me.ignoreSelection;
        }

        // If not querying using the raw field value, we can set the value now we have data
        if (success && !store.lastOptions.rawQuery) {
            // Set the value on load

            // There's no value.
            if (me.value === null) {
                // Highlight the first item in the list if autoSelect: true
                if (me.store.getCount()) {
                    me.doAutoSelect();
                } else {
                    // assign whatever empty value we have to prevent change from firing
                    //me.setValue(me.value);
                }
            } else {
                me.setValue(me.value);
            }
        }
    },

    doQuery: function(queryString, forceAll, rawQuery) {
        var me = this;

            // Decide if, and how we are going to query the store
        var queryPlan = me.beforeQuery({
            query: queryString || '',
            rawQuery: rawQuery,
            forceAll: forceAll,
            combo: me,
            cancel: false
        });

        // Allow veto.
        if (me.queryMode==='local' && (queryPlan === false || queryPlan.cancel)) {
            return false;
        }

        // If they're using the same value as last time, just show the dropdown
        if (me.queryCaching && queryPlan.query === me.lastQuery) {
            me.expand();
        }
       // Otherwise filter or load the store
        else {
            me.lastQuery = queryPlan.query;

            if (me.queryMode === 'local') {
                me.doLocalQuery(queryPlan);

            } else {
                me.doRemoteQuery(queryPlan);
            }
        }

        return true;
    },

    doRemoteQuery: function(queryPlan) {
        var me = this,
            loadCallback = function() {
                me.afterQuery(queryPlan);
            };

        // expand before loading so LoadMask can position itself correctly
        me.expand();

        // In queryMode: 'remote', we assume Store filters are added by the developer as remote filters,
        // and these are automatically passed as params with every load call, so we do *not* call clearFilter.
        var queryParams;
        queryParams = me.getParams(queryPlan.query);

        if(!String.isNullOrEmpty(me.queryBy)) {
            queryParams.queryBy = me.queryBy;
        } else if(!String.isNullOrEmpty(me.displayField)) {
            queryParams.queryBy = me.displayField;
        }

        var lastOptions = me.store.lastOptions;
        if(lastOptions && lastOptions.params && lastOptions.params.hasOwnProperty("fieldFilters"))
            queryParams.fieldFilters = lastOptions.params.fieldFilters;

        if (me.pageSize) {
             me.store.loadPage(1, {
                params: queryParams,
                filters: (me.filters !== null) ? [me.filters] : null,
                //rawQuery: queryPlan.rawQuery,
                callback: loadCallback
            });
        } else {
            me.store.load({
                params: queryParams,
                filters: (me.filters !== null) ? [me.filters] : null,
                //rawQuery: queryPlan.rawQuery,
                callback: loadCallback
            });
        }
    },

    loadPage: function(pageNum, options) {
        var me = this;
        
        var queryParams = this.store.lastOptions.params;

        this.store.loadPage(pageNum, Ext.apply({
            params: (queryParams) ? queryParams : null,
            filters: (me.filters !== null) ? [me.filters] : null
        }, options));
    },

    onBlur: function() {
        Ext.form.ComboBox.superclass.onBlur.call(this);
    },
   
    onFocus: function() {
        Ext.form.ComboBox.superclass.onFocus.call(this);
    },

    /**
     * @private
     * Enables the key nav for the BoundList when it is expanded.
     */
    onExpand: function() {
        var me = this,
            keyNav = me.listKeyNav,
            selectOnTab = me.selectOnTab,
            picker = me.getPicker();

        // Handle BoundList navigation from the input field. Insert a tab listener specially to enable selectOnTab.
        if (keyNav) {
            keyNav.enable();
        } else {
            keyNav = me.listKeyNav = new Ext.view.BoundListKeyNav(this.inputEl, {
                boundList: picker,
                forceKeyDown: true,
                tab: function(e) {
                    if (selectOnTab) {
                        this.selectHighlighted(e);
                        //me.triggerBlur();
                        me.collapse();
                    }
                    // Tab key event is allowed to propagate to field
                    return false;
                },
                enter: function(e){
                    var selModel = picker.getSelectionModel(),
                        count = selModel.getCount();
                        
                    this.selectHighlighted(e);
                    
                    // Handle the case where the highlighted item is already selected
                    // In this case, the change event won't fire, so just collapse
                    if (!me.multiSelect && count === selModel.getCount()) {
                        me.collapse();
                    }
                }
            });
        }

        // While list is expanded, stop tab monitoring from Ext.form.field.Trigger so it doesn't short-circuit selectOnTab
        if (selectOnTab) {
            me.ignoreMonitorTab = true;
        }

        Ext.defer(keyNav.enable, 1, keyNav); //wait a bit so it doesn't react to the down arrow opening the picker
        me.inputEl.focus();
    },

    getSelectedIndex: function() {
        var v = this.getValue();
        var r = this.findRecord(this.valueField || this.displayField, v);
        return(this.store.indexOf(r));
    },

    getSelectedRecord: function() {
        var v = this.getValue();
        var r = this.findRecord(this.valueField || this.displayField, v);
        return r;
    }
});