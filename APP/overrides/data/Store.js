Ext.define('Overrides.data.Store', {
	override: 'Ext.data.Store',
    // overriden to provide striping of the indexes as sorting occurs.
    // this cannot be done inside of sort because datachanged has already
    // fired and will trigger a repaint of the bound view.
    doSort: function(sorterFn) {
        var me = this,
            range,
            ln,
            i;

        if (me.remoteSort) {

            // For a buffered Store, we have to clear the prefetch cache since it is keyed by the index within the dataset.
            // Then we must prefetch the new page 1, and when that arrives, reload the visible part of the Store
            // via the guaranteedrange event
            var lastParams = me.lastOptions.params,
                lastFilters = me.lastOptions.filters;

            if (me.buffered) {
                me.data.clear();
                me.loadPage(1, {params: lastParams, filters: lastFilters});
            } else {
                //the load function will pick up the new sorters and request the sorted data from the proxy
                me.load({params: lastParams, filters: lastFilters});
            }
        } else {
            //<debug>
            if (me.buffered) {
                Ext.Error.raise({
                    msg: 'Local sorting may not be used on a buffered store'
                });
            }
            //</debug>
            me.data.sortBy(sorterFn);
            if (!me.buffered) {
                range = me.getRange();
                ln = range.length;
                for (i = 0; i < ln; i++) {
                    range[i].index = i;
                }
            }
            me.fireEvent('datachanged', me);
            me.fireEvent('refresh', me);
        }
    }
});