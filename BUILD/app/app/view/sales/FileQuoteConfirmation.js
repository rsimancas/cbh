Ext.define("CBH.view.sales.FileQuoteConfirmation", {
  extend: "Ext.form.Panel",
  alias: "widget.filequoteconfirmation",
  layout: {
    type: "column"
  },
  bodyPadding: 5,
  FileKey: 0,
  frameHeader: false,
  header: false,
  enableKeyEvents: true,
  storeNavigator: null,
  Customer: null,
  FileNum: null,
  QuoteNum: null,
  storesLoaded: null,
  storeToLoad: 0,
  currentRecord: null,

  initComponent: function() {
    var me = this;

    var storePaymentTerms = new CBH.store.common.PaymentTerms().load({
      params: {
        page: 0,
        start: 0,
        limit: 0
      }
    });

    var storeInspectionCompanies = new CBH.store.common.InspectionCompanies().load(
      {
        params: {
          page: 0,
          start: 0,
          limit: 0
        }
      }
    );

    var storeSalesEmployee = new CBH.store.common.Employees().load({
      params: {
        page: 0,
        start: 0,
        limit: 0
      }
    });

    var storeOrderEmployee = new CBH.store.common.Employees().load({
      params: {
        page: 0,
        start: 0,
        limit: 0
      }
    });

    Ext.applyIf(me, {
      fieldDefaults: {
        labelAlign: "top",
        labelWidth: 60,
        msgTarget: "side",
        fieldStyle: "font-size:11px",
        labelStyle: "font-size:11px"
      },
      items: [
        // Header Container
        {
          xtype: "fieldcontainer",
          columnWidth: 1,
          layout: {
            type: "hbox"
          },
          items: [
            {
              xtype: "textfield",
              fieldLabel: "Customer",
              readOnly: true,
              value: me.Customer,
              flex: 8,
              fieldStyle: "font-size: 12px; color: #157fcc;font-weight:bold;"
            },
            {
              xtype: "textfield",
              margin: "0 0 0 5",
              fieldLabel: "File Num",
              readOnly: true,
              value: me.FileNum,
              flex: 1,
              fieldStyle: "font-size: 12px; color: #157fcc;font-weight:bold;"
            },
            {
              xtype: "textfield",
              margin: "0 0 0 5",
              fieldLabel: "Quote Number",
              readOnly: true,
              flex: 1,
              value: me.QuoteNum,
              fieldStyle: "font-size: 12px; color: #157fcc;font-weight:bold;"
            },
            {
              xtype: "hidden",
              name: "QHdrKey"
            }
          ]
        },
        // Customer
        {
          xtype: "fieldcontainer",
          columnWidth: 0.4,
          layout: "column",
          items: [
            // General Information fieldset
            {
              xtype: "fieldset",
              title: "General Information",
              padding: "0 10 10 10",
              columnWidth: 1,
              layout: "column",
              items: [
                {
                  xtype: "textfield",
                  name: "QHdrProdDescription",
                  fieldLabel: "Product Description",
                  columnWidth: 1
                },
                {
                  xtype: "textfield",
                  name: "QHdrShippingDescription",
                  fieldLabel: "BL/AWB Desc",
                  columnWidth: 1
                },
                {
                  xtype: "datefield",
                  name: "QHdrQuoteConfirmationDate",
                  fieldLabel: "Date",
                  columnWidth: 0.5
                },
                {
                  margin: "0 0 0 5",
                  xtype: "textfield",
                  name: "FileReference",
                  fieldLabel: "Reference",
                  columnWidth: 0.5
                },
                {
                  columnWidth: 0.5,
                  xtype: "combo",
                  name: "FileQuoteEmployeeKey",
                  fieldLabel: "Sales Employee",
                  valueField: "EmployeeKey",
                  displayField: "x_EmployeeFullName",
                  store: storeSalesEmployee,
                  queryMode: "local",
                  typeAhead: false,
                  minChars: 2,
                  forceSelection: false,
                  anyMatch: true
                },
                {
                  margin: "0 0 0 5",
                  columnWidth: 0.5,
                  xtype: "combo",
                  name: "FileOrderEmployeeKey",
                  fieldLabel: "Order Employee",
                  valueField: "EmployeeKey",
                  displayField: "x_EmployeeFullName",
                  store: storeOrderEmployee,
                  queryMode: "local",
                  typeAhead: false,
                  minChars: 2,
                  forceSelection: false,
                  anyMatch: true
                },
                {
                  columnWidth: 0.5,
                  xtype: "combo",
                  name: "QHdrInspectorKey",
                  fieldLabel: "Inspection",
                  labelWidth: 50,
                  valueField: "InspectorKey",
                  displayField: "InspectorName",
                  store: storeInspectionCompanies,
                  queryMode: "local",
                  minChars: 2,
                  forceSelection: false,
                  queryCaching: false,
                  anyMatch: true
                },
                {
                  margin: "0 0 0 5",
                  xtype: "textfield",
                  name: "QHdrInspectionNum",
                  fieldLabel: "Num",
                  columnWidth: 0.5
                },
                {
                  xtype: "textfield",
                  name: "QHdrDUINum",
                  fieldLabel: "DUI Number",
                  columnWidth: 1
                },
                {
                  xtype: "combo",
                  name: "QHdrCarrierKey",
                  fieldLabel: "Carrier/Logistics",
                  columnWidth: 1,
                  valueField: "VendorKey",
                  displayField: "VendorName",
                  queryMode: "remote",
                  pageSize: 11,
                  minChars: 2,
                  allowBlank: false,
                  triggerAction: "",
                  forceSelection: false,
                  queryCaching: false,
                  emptyText: "Choose Carrier",
                  autoSelect: false,
                  selectOnFocus: true
                }
              ]
            },
            // Customer Information fieldset
            {
              xtype: "fieldset",
              title: "Customer Information",
              padding: "0 10 10 10",
              columnWidth: 1,
              layout: "column",
              items: [
                {
                  xtype: "textfield",
                  name: "QHdrCustRefNum",
                  fieldLabel: "Client Order Num",
                  columnWidth: 1
                },
                {
                  columnWidth: 1,
                  xtype: "combo",
                  name: "FileContactKey",
                  fieldLabel: "- Contact",
                  valueField: "ContactKey",
                  displayField: "x_ContactFullName",
                  queryMode: "local",
                  typeAhead: false,
                  minChars: 2,
                  forceSelection: false,
                  anyMatch: true
                },
                {
                  xtype: "textfield",
                  name: "ContactPhone",
                  fieldLabel: "  Phone",
                  columnWidth: 0.5,
                  editable: false
                },
                {
                  margin: "0 0 0 5",
                  xtype: "textfield",
                  name: "CustFax",
                  fieldLabel: "  Fax",
                  columnWidth: 0.5,
                  editable: false
                },
                {
                  xtype: "textfield",
                  name: "ContactEmail",
                  fieldLabel: "  Email",
                  columnWidth: 1,
                  editable: false
                },
                {
                  xtype: "fieldcontainer",
                  columnWidth: 1,
                  layout: "column",
                  items: [
                    {
                      xtype: "datefield",
                      columnWidth: 0.3,
                      fieldLabel: "Date Rqd. by Customer",
                      labelAlign: "top",
                      labelWidth: 50,
                      name: "FileDateCustRequired"
                    },
                    {
                      xtype: "textfield",
                      columnWidth: 0.7,
                      margin: "0 0 0 10",
                      fieldLabel: "Date Rqd. by Cust. Note",
                      labelAlign: "top",
                      labelWidth: 50,
                      name: "FileDateCustRequiredNote"
                    }
                  ]
                },
                {
                  xtype: "combo",
                  columnWidth: 1,
                  name: "QHdrCustPaymentTerms",
                  fieldLabel: "Payment Terms",
                  valueField: "TermKey",
                  displayField: "x_Description",
                  store: storePaymentTerms,
                  queryMode: "local",
                  typeAhead: false,
                  minChars: 2,
                  forceSelection: true,
                  anyMatch: true
                }
              ]
            }
          ]
        },
        // Vendor
        {
          xtype: "fieldset",
          margin: "0 0 0 10",
          padding: "10 10 20 10",
          title: "Vendor Information",
          columnWidth: 0.6,
          layout: "column",
          items: [
            {
              columnWidth: 1,
              xtype: "combo",
              name: "FileCurrentVendor",
              fieldLabel: "Current Vendor",
              valueField: "VendorKey",
              displayField: "VendorName",
              store: null,
              queryMode: "local",
              typeAhead: false,
              minChars: 2,
              forceSelection: false,
              anyMatch: true,
              listeners: {
                change: function(field, newValue, oldValue, eOpts) {
                  var me = field.up("form");
                  me.onCurrentVendorChange(newValue);
                }
              }
            },
            Ext.widget("filequoteconfirmationsvinfo")
          ]
        },
        // Estimate
        {
          xtype: "fieldcontainer",
          columnWidth: 1,
          layout: "column",
          items: [
            // 1st row
            {
              columnWidth: 0.15,
              margin: "20 0 0 0",
              xtype: "displayfield",
              value: "Freight Cost:",
              readOnly: true,
              fieldStyle: "text-align: right;  font-size: 11px;"
            },
            {
              margin: "0 0 0 5",
              name: "txt4990Cost",
              columnWidth: 0.15,
              xtype: "textfield",
              fieldLabel: "Ocean",
              value: "$0.00",
              readOnly: true,
              fieldStyle: "text-align: right;"
            },
            {
              margin: "0 0 0 5",
              columnWidth: 0.15,
              name: "txt49901Cost",
              xtype: "textfield",
              fieldLabel: "Land",
              value: "$2,140.00",
              readOnly: true,
              fieldStyle: "text-align: right;"
            },
            {
              margin: "0 0 0 5",
              columnWidth: 0.15,
              name: "txt49903Cost",
              xtype: "textfield",
              fieldLabel: "Air",
              value: "$0.00",
              readOnly: true,
              fieldStyle: "text-align: right;"
            },
            {
              margin: "0 0 0 5",
              columnWidth: 0.15,
              name: "txt49910Cost",
              xtype: "textfield",
              fieldLabel: "Insurance",
              value: "$0.00",
              readOnly: true,
              fieldStyle: "text-align: right;"
            },
            {
              margin: "0 0 0 5",
              columnWidth: 0.15,
              name: "txtOtherCost",
              xtype: "textfield",
              fieldLabel: "Ocean",
              value: "$0.00",
              readOnly: true,
              fieldStyle: "text-align: right;"
            },
            {
              margin: "20 0 0 5",
              columnWidth: 0.1,
              xtype: "displayfield",
              value: "~ = Estimated",
              fieldStyle: "text-align: left; color: blue; font-size: 11px;"
            },
            // 2nd row
            {
              columnWidth: 0.15,
              margin: "5 0 0 0",
              xtype: "displayfield",
              value: "Price:",
              fieldStyle: "text-align: right;  font-size: 11px;"
            },
            {
              margin: "0 0 0 5",
              columnWidth: 0.15,
              name: "txt49902Price",
              xtype: "textfield",
              value: "$0.00",
              readOnly: true,
              fieldStyle: "text-align: right;"
            },
            {
              margin: "0 0 0 5",
              columnWidth: 0.15,
              name: "txt49901Price",
              xtype: "textfield",
              value: "$2,375.00",
              readOnly: true,
              fieldStyle: "text-align: right;"
            },
            {
              margin: "0 0 0 5",
              columnWidth: 0.15,
              name: "txt49903Price",
              xtype: "textfield",
              readOnly: true,
              value: "$0.00",
              fieldStyle: "text-align: right;"
            },
            {
              margin: "0 0 0 5",
              columnWidth: 0.15,
              name: "txt49910Price",
              xtype: "textfield",
              value: "$0.00",
              readOnly: true,
              fieldStyle: "text-align: right;"
            },
            {
              margin: "0 0 0 5",
              columnWidth: 0.15,
              name: "txtOtherPrice",
              xtype: "textfield",
              value: "$0.00",
              readOnly: true,
              fieldStyle: "text-align: right;"
            }
          ]
        }
      ],
      dockedItems: [
        {
          xtype: "toolbar",
          dock: "bottom",
          ui: "footer",
          items: [
            {
              xtype: "component",
              flex: 1
            },
            {
              xtype: "button",
              text: "Save Changes",
              handler: function() {
                var me = this.up("form");
                me.onClickSaveChanges();
              }
            },
            {
              xtype: "button",
              text: "Convert Quote to Order",
              handler: function() {
                var me = this.up("form");
                me.onClickConvertQuoteToOrder();
              }
            }
          ]
        }
      ],
      listeners: {
        render: {
          fn: me.onRenderForm,
          scope: me
        },
        afterrender: {
          fn: me.registerKeyBindings,
          scope: me
        }
      }
    });

    me.callParent(arguments);
  },

  registerKeyBindings: function(view, options) {
    var me = this;
    Ext.EventManager.on(
      view.getEl(),
      "keyup",
      function(evt, t, o) {
        if (evt.ctrlKey && evt.keyCode === Ext.EventObject.F8) {
          evt.stopEvent();
          var toolbar = me.down("#FormToolbar");
          if (toolbar.isEditing) {
            var btn = toolbar.down("#save");
            btn.fireEvent("click");
          }
        }
      },
      this
    );
  },

  onRenderForm: function() {
    var me = this,
      formsvinfo = me.down("#formsvinfo");

    formsvinfo.down("field[name=FVPaymentTerms]").bindStore(
      new CBH.store.common.PaymentTerms().load({
        params: {
          page: 0,
          start: 0,
          limit: 0
        }
      })
    );

    //var toolbar = me.down('#FormToolbar');
    //toolbar.down('#add').setVisible(false);
    // if(toolbar.store.getCount()===1 && toolbar.store.getAt(0).phantom) {
    //     toolbar.items.items.forEach(function(btn){btn.setVisible(false);});
    //     toolbar.down('#save').setVisible(true);
    // }
  },

  onAfterLoadRecord: function(record) {
    var me = this,
      curRec = record.data;

    me.storeToLoad = 4;
    me.storesLoaded = [];

    me
      .up("app_viewport")
      .getEl()
      .mask("Please wait...");

    var storeVendorCarrier = new CBH.store.vendors.Vendors().load({
      params: {
        id: curRec.QHdrCarrierKey,
        page: 0,
        start: 0,
        limit: 0
      },
      callback: function() {
        me.down("field[name=QHdrCarrierKey]").bindStore(this);
        me.down("field[name=QHdrCarrierKey]").setValue(curRec.QHdrCarrierKey);
        me.storesLoaded.push(this);
        me.checkLoaded();
      }
    });

    var storeCurrentVendor = new CBH.store.vendors.Vendors().load({
      params: {
        QHdrKey: curRec.QHdrKey,
        page: 0,
        start: 0,
        limit: 0
      },
      callback: function() {
        me.down("field[name=FileCurrentVendor]").bindStore(this);
        me
          .down("field[name=FileCurrentVendor]")
          .setValue(curRec.FileCurrentVendor);
        me.storesLoaded.push(this);
        me.checkLoaded();
      }
    });

    var storeCustomerContacts = new CBH.store.customers.CustomerContacts().load(
      {
        params: {
          custkey: curRec.FileCustKey,
          page: 0,
          start: 0,
          limit: 0
        },
        callback: function() {
          me.down("field[name=FileContactKey]").bindStore(this);
          me.down("field[name=FileContactKey]").setValue(curRec.FileContactKey);
          me.storesLoaded.push(this);
          me.checkLoaded();
        }
      }
    );

    var storeqsumFileQuoteChargesByGLAccount = new CBH.store.sales.qsumFileQuoteChargesByGLAccount().load(
      {
        params: {
          QHdrKey: curRec.QHdrKey
        },
        callback: function(records, success, eOpts) {
          if (records[0]) {
            var curOtherCostSum = 0,
              curOtherPriceSum = 0;
            this.each(function(record) {
              curOtherCostSum = curOtherCostSum + record.data.Cost;
              curOtherPriceSum = curOtherPriceSum + record.data.Price;
            });

            me
              .down("field[name=txtOtherCost]")
              .setValue(Ext.util.Format.usMoney(curOtherCostSum));
            me
              .down("field[name=txtOtherPrice]")
              .setValue(Ext.util.Format.usMoney(curOtherPriceSum));
          }
          me.storesLoaded.push(this);
          me.checkLoaded();
        }
      }
    );
  },

  checkLoaded: function() {
    var me = this,
      stores = me.storesLoaded;

    if (stores.length < me.storeToLoad) {
      return;
    }

    me
      .up("app_viewport")
      .getEl()
      .unmask();
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

    Ext.Msg.wait("Saving Record...", "Wait");

    var isdirty = record.dirty;

    record.save({
      callback: function(records, operation, success) {
        if (success) {
          Ext.Msg.hide();
          //toolbar.doRefresh();
        } else {
          Ext.Msg.hide();
        }
      }
    });
  },

  onCurrentVendorChange: function(VendorKey) {
    if (!VendorKey) return;

    var me = this,
      formsvinfo = me.down("#formsvinfo"),
      curRec = me.currentRecord;

    Ext.Msg.wait("Loading...", "Wait");

    var storeSVInfo = new CBH.store.sales.qfrmFileQuoteConfirmationSVInfo().load(
      {
        params: {
          QHdrKey: curRec.data.QHdrKey,
          VendorKey: VendorKey
        },
        callback: function(records, operation, success) {
          if (success && records.length > 0) {
            formsvinfo.onAfterLoadRecord(records[0]);
          }
          Ext.Msg.hide();
        }
      }
    );
  },

  onClickConvertQuoteToOrder: function() {
    var me = this,
      record = me.currentRecord,
      QHdrKey = record.data.QHdrKey,
      JobKey = record.data.QHdrJobKey,
      FileKey = record.data.QHdrFileKey;

    // If has Jobkey Open JobOverview and return
    if (JobKey) {
      me.openJobOverview(JobKey);
      return;
    }

    //'*** Verify that all vendors have been confirmed
    //'*** (First run cleanup to delete any vendors that got stuck in the FileVendorInfo table)
    me.CleanupFileQuoteVendorInfo(FileKey);
  },

  openJobOverview: function(JobKey) {
    var me = this;

    var tabs = me.up("app_pageframe"),
      currentTab = tabs.getActiveTab();

    Ext.Msg.wait("Loading....", "Wait");
    var storeJobs = new CBH.store.jobs.JobList().load({
      params: {
        id: JobKey
      },
      callback: function(records, operation, success) {
        var curJob = records[0];

        var storeJobOverview = new CBH.store.jobs.qJobOverview().load({
          params: {
            id: JobKey
          },
          callback: function(records, operation, success) {
            var jobOverview = records[0];

            var form = Ext.widget("joboverview", {
              currentRecord: jobOverview,
              currentJob: curJob,
              JobKey: JobKey,
              JobNum: jobOverview.data.JobNum
            });

            form.loadRecord(jobOverview);

            var tab = tabs.add({
              closable: true,
              iconCls: "tabs",
              autoScroll: true,
              title: "Job Overview: " + jobOverview.data.JobNum,
              items: [form],
              listeners: {
                activate: function() {
                  var form = this.down("form");
                  form.refreshData();
                }
              }
            });

            tab.show();
            me.destroy();
            currentTab.destroy();
            Ext.Msg.hide();
          }
        });
      }
    });
  },

  CleanupFileQuoteVendorInfo: function(FileKey) {
    var me = this;

    Ext.Msg.wait("Please wait....", "Wait");

    Ext.Ajax.request({
      method: "GET",
      type: "json",
      url: CBH.GlobalSettings.webApiPath + "/api/CleanupFileQuoteVendorInfo",
      headers: {
        "Authorization-Token": Ext.util.Cookies.get("CBH.UserAuth")
      },
      params: {
        FileKey: FileKey
      },
      success: function(response) {
        var data = Ext.JSON.decode(response.responseText);
        if (data.success) me.checkUnconfirmedVendor();
        Ext.Msg.hide();
      }
    });
  },

  checkUnconfirmedVendor: function() {
    var me = this,
      curRec = me.currentRecord;

    Ext.Msg.wait("Please wait....", "Wait");
    Ext.Ajax.request({
      method: "GET",
      type: "json",
      url: CBH.GlobalSettings.webApiPath + "/api/CheckUnconfirmedVendor",
      headers: {
        "Authorization-Token": Ext.util.Cookies.get("CBH.UserAuth")
      },
      params: {
        QHdrKey: curRec.data.QHdrKey
      },
      success: function(response) {
        var data = Ext.JSON.decode(response.responseText);
        if (data.success) {
          me.afterCleanupFileQuoteVendorInfo();
          Ext.Msg.hide();
        } else {
          Ext.Msg.show({
            title: "Conversion Aborted!",
            msg: "Not all vendors have been confirmed yet",
            buttons: Ext.Msg.OK,
            icon: Ext.MessageBox.WARNING
          });
        }
      }
    });
  },

  afterCleanupFileQuoteVendorInfo: function() {
    var me = this,
      record = me.currentRecord;

    //'*** Verify inspection information
    if (String.isNullOrEmpty(record.data.QHdrInspectionNum)) {
      //'*** Check to see whether inspection is required
      me.isQuoteOverFOBLimit(record.data.QHdrKey);
    } else {
      //'*** Check to see if inspector is selected
      if (record.data.QHdrInspectorKey === null) {
        Ext.Msg.show({
          title: "Conversion Aborted!",
          msg:
            "You must select an inspection company if there is an inspection number",
          buttons: Ext.Msg.OK,
          fn: function() {
            me.down("field[name=QHdrInspectorKey]").focus(true, 200);
          },
          icon: Ext.MessageBox.WARNING
        });
      } else {
        me.verifyDUINumber();
      }
    }
  },

  verifyDUINumber: function() {
    var me = this,
      record = me.currentRecord,
      QHdrDUINum = me.down("field[name=QHdrDUINum]").getValue();

    //'*** Verify DUI Number
    if (record.data.QHdrDUINum === null) {
      Ext.Msg.show({
        title: "Continue Conversion?",
        msg: "Are you sure you want to continue without a DUI Number?",
        buttons: Ext.Msg.YESNO,
        icon: Ext.Msg.QUESTION,
        fn: function(btn) {
          if (btn === "yes") {
            me.convertQuoteToOrder();
          } else {
            me.down("field[name=QHdrDUINum]").focus(true, 200);
          }
        }
      });
    } else {
      me.convertQuoteToOrder();
    }
  },

  convertQuoteToOrder: function() {
    var me = this,
      curRec = me.currentRecord;
    JobNum = curRec.data.JobNum;

    Ext.Msg.wait("Please wait....", "Wait");
    Ext.Ajax.request({
      method: "GET",
      type: "json",
      url: CBH.GlobalSettings.webApiPath + "/api/ConvertQuoteToOrder",
      headers: {
        "Authorization-Token": Ext.util.Cookies.get("CBH.UserAuth")
      },
      params: {
        QHdrKey: curRec.data.QHdrKey,
        CurrentUser: CBH.GlobalSettings.getCurrentUser().UserName,
        JobNum: JobNum
      },
      success: function(response) {
        var data = Ext.JSON.decode(response.responseText);
        if (data.success) {
          me.openJobOverview(parseFloat(data.JobKey));
          me.newJobNotification(parseFloat(data.JobKey));
        }
        Ext.Msg.hide();
      },
      failure: function(response) {
        Ext.Msg.hide();
      }
    });
  },

  isQuoteOverFOBLimit: function(QHdrKey) {
    var me = this;

    Ext.Msg.wait("Please wait....", "Wait");
    Ext.Ajax.request({
      method: "GET",
      type: "json",
      url: CBH.GlobalSettings.webApiPath + "/api/IsQuoteOverFOBLimit",
      headers: {
        "Authorization-Token": Ext.util.Cookies.get("CBH.UserAuth")
      },
      params: {
        QHdrKey: QHdrKey
      },
      success: function(response) {
        var data = Ext.JSON.decode(response.responseText);
        if (data.success) {
          Ext.Msg.show({
            title: "Continue without Inspection?",
            msg:
              "The FOB Value for this quote requires an inspection for the country the product is being shipped to.  (Or no country has been specified)  Are you sure you want to continue with the conversion?",
            buttons: Ext.Msg.YESNO,
            icon: Ext.Msg.QUESTION,
            fn: function(btn) {
              if (btn === "yes") {
                me.verifyDUINumber();
              } else {
                me.down("field[name=QHdrInspectorKey]").focus(true, 200);
              }
            }
          });
        } else {
          Ext.Msg.hide();
          me.verifyDUINumber();
        }
      }
    });
  },

  newJobNotification: function(JobKey) {
    var me = this;

    Ext.Msg.wait("Please wait....", "Wait");
    Ext.Ajax.request({
      method: "GET",
      type: "json",
      url: CBH.GlobalSettings.webApiPath + "/api/NewJobNotification",
      headers: {
        "Authorization-Token": Ext.util.Cookies.get("CBH.UserAuth")
      },
      params: {
        CurrentUser: CBH.GlobalSettings.getCurrentUser().UserName,
        JobKey: JobKey
      },
      success: function(response) {
        var data = Ext.JSON.decode(response.responseText);
        if (data.success) {
          me.sendMail(data.message);
        }
        Ext.Msg.hide();
      }
    });
  },

  sendMail: function(msg) {
    var me = this,
      strMessage = msg.Body,
      url = "",
      recipient = msg.RecipientsTO.join().replace('"', ""),
      cc =
        msg.RecipientsCC.length > 0
          ? msg.RecipientsCC.join().replace('"', "")
          : "",
      subject = msg.Subject;

    url = "mailto:" + recipient + "?subject=" + encodeURIComponent(subject);

    if (!String.isNullOrEmpty(cc)) url += "&cc=" + encodeURIComponent(cc);

    url = url + "&body=" + encodeURIComponent(strMessage);

    window.open(url, "Mail", false);
  },

  onClickSaveChanges: function() {
    var me = this,
      form = me.getForm();

    form.updateRecord();

    var savedRecord = form.getRecord();

    if (savedRecord.data.FileKey !== 0) {
      savedRecord.set(
        "FileModifiedBy",
        CBH.GlobalSettings.getCurrentUserName()
      );
      savedRecord.set(
        "QHdrModifiedBy",
        CBH.GlobalSettings.getCurrentUserName()
      );
    }

    Ext.Msg.wait("Saving Vendor Info...", "Wait");

    me.saveVendorInfo().then({
      success: function() {
        Ext.Msg.wait("Saving General Info...", "Wait");
        savedRecord.save({
          callback: function(records, operation, success) {
            Ext.Msg.hide();
          }
        });
      }
    });
  },

  saveVendorInfo: function() {
    var deferred = Ext.create("Deft.Deferred");
    var me = this.down("form"),
      form = me.getForm();

    form.updateRecord();

    var vendorRecord = form.getRecord();

    vendorRecord.getProxy().setSilentMode(true);
    vendorRecord.save({
      callback: function(records, operation, success) {
        if (success) {
          deferred.resolve(records);
        } else {
          deferred.reject("Error saving.");
        }
      }
    });

    // new CBH.store.jobs.JobPurchaseOrders().load({
    //   params: {
    //     id: POKey
    //   },
    //   callback: function(records, operation, success) {
    //     if (success) {
    //       deferred.resolve(records);
    //     } else {
    //       deferred.reject("Error loading Companies.");
    //     }
    //   }
    // });

    return deferred.promise;
  }
});
