//overriden to fix bug when pulse different page and don't take last options
Ext.define('Overrides.toolbar.Paging', {
	override: 'Ext.toolbar.Paging',

    // @private
    onPagingKeyDown : function(field, e){
        var me = this,
            k = e.getKey(),
            pageData = me.getPageData(),
            increment = e.shiftKey ? 10 : 1,
            pageNum;

        if (k == e.RETURN) {
            e.stopEvent();
            pageNum = me.readPageFromInput(pageData);
            if (pageNum !== false) {
                pageNum = Math.min(Math.max(1, pageNum), pageData.pageCount);
                if(me.fireEvent('beforechange', me, pageNum) !== false){
                    var lastOpt = me.store.lastOptions;
                    me.store.loadPage(pageNum, {params:lastOpt.params, filters: lastOpt.filters});
                }
            }
        } else if (k == e.HOME || k == e.END) {
            e.stopEvent();
            pageNum = k == e.HOME ? 1 : pageData.pageCount;
            field.setValue(pageNum);
        } else if (k == e.UP || k == e.PAGE_UP || k == e.DOWN || k == e.PAGE_DOWN) {
            e.stopEvent();
            pageNum = me.readPageFromInput(pageData);
            if (pageNum) {
                if (k == e.DOWN || k == e.PAGE_DOWN) {
                    increment *= -1;
                }
                pageNum += increment;
                if (pageNum >= 1 && pageNum <= pageData.pageCount) {
                    field.setValue(pageNum);
                }
            }
        }
    },

    /**
     * Move to the first page, has the same effect as clicking the 'first' button.
     */
    moveFirst : function(){
        var me = this;
        if (this.fireEvent('beforechange', this, 1) !== false){
            var lastOpt = me.store.lastOptions;
            this.store.loadPage(1,{params:lastOpt.params, filters: lastOpt.filters});
        }
    },

    /**
     * Move to the previous page, has the same effect as clicking the 'previous' button.
     */
    movePrevious : function(){
        var me = this,
            prev = me.store.currentPage - 1;

        if (prev > 0) {
            if (me.fireEvent('beforechange', me, prev) !== false) {
                var lastOpt = me.store.lastOptions;
                me.store.previousPage({params: lastOpt.params, filters: lastOpt.filters});
            }
        }
    },

    /**
     * Move to the next page, has the same effect as clicking the 'next' button.
     */
    moveNext : function(){
        var me = this,
            total = me.getPageData().pageCount,
            next = me.store.currentPage + 1;
            
        if (next <= total) {
            if (me.fireEvent('beforechange', me, next) !== false) {
                var lastOpt = me.store.lastOptions;
                me.store.nextPage({params:lastOpt.params, filters: lastOpt.filters});
            }
        }
    },

    /**
     * Move to the last page, has the same effect as clicking the 'last' button.
     */
    moveLast : function(){
        var me = this,
            last = me.getPageData().pageCount;

        if (me.fireEvent('beforechange', me, last) !== false) {
            lastOpt = me.store.lastOptions;
            me.store.loadPage(last, {params:lastOpt.params, filters: lastOpt.filters});
        }
    },

    /**
     * Refresh the current page, has the same effect as clicking the 'refresh' button.
     */
    doRefresh : function(callback){
        var me = this,
            current = me.store.currentPage;

        if (me.fireEvent('beforechange', me, current) !== false) {
            lastOpt = me.store.lastOptions;
            me.store.loadPage(current, {params: lastOpt.params, filters: lastOpt.filters, callback: callback});
        }
    }
});