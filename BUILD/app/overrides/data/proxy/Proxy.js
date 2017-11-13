Ext.define('Overrides.data.proxy.Proxy', {
	override: 'Ext.data.proxy.Proxy',
    _silentMode: false,
    setSilentMode: function(silentMode) {
        this._silentMode = silentMode;
    },
    getSilentMode: function() {
        return this._silentMode;
    }
});