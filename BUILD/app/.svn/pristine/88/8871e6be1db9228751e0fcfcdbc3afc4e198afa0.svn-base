 Ext.define('CBH.view.common.MainHeader', {
 	extend: 'Ext.container.Container',

 	xtype: 'app_header',
 	autorRender: true,
 	autoShow: true,
 	frame: true,
 	split: false,
 	//height: 70,
 	layout: {
 		type: 'hbox'
 	},

 	requires: [
 		'CBH.view.common.ToolBar'
 	],

 	initComponent: function() {
 		var me = this;

 		Ext.applyIf(me, {
 			
 			items: [
 			/*{
 				xtype: 'container',
 				html: '<div><img src="images/logo_header.png"/>',
 				flex: 1
 			},*/
 			{
 				xtype:'component',
 				flex: 1
 			},
			{ 
 				xtype: 'app_toolbar',
 				border: 0,
 				margin: '0 0 0 0',
 			}
 			]
 		});

 		me.callParent(arguments);
 	}

 });