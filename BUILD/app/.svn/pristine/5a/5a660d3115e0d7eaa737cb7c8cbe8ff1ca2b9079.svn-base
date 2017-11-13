Ext.define('Overrides.form.field.Date', {
	override: 'Ext.form.field.Date',
    
    initComponent: function() {
        var me = this;
        me.callParent(arguments);

        me.on('focus',function(field) {
            var me = this;

            if(!me.readOnly)
            	me.expand();
        });
    }
});