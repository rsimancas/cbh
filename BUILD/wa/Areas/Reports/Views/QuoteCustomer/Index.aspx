<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>Quote Customer</title>
</head>
<body style="height: 279px">
    <form id="form1" runat="server" style="background-color: white;">
        <%--<asp:UpdateProgress ID="updateProgress" runat="server">
            <ProgressTemplate>
                <div style="position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
                    <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/wa/Areas/Reports/images/Loading32x32.gif" AlternateText="Loading ..." ToolTip="Loading ..." Style="padding: 10px; position: fixed; top: 45%; left: 50%;" />
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>--%>
        <%--<object data="<%= Url.Action("GetPDF") %>" type="application/pdf" style="width: 100%; height: 100%; top: 0px; right: 0px; bottom: 0px; left: 0px; position: fixed; background-color: white;" contenteditable="false">
    </object>--%>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1">
            <ProgressTemplate>
                <div style="position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
                    <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/wa/Areas/Reports/images/Loading32x32.gif" AlternateText="Loading ..." ToolTip="Loading ..." Style="padding: 10px; position: fixed; top: 45%; left: 50%;" />
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <embed src="<%= Url.Action("GetPDF") %>" type="application/pdf" style="width: 100%; height: 100%; top: 0px; right: 0px; bottom: 0px; left: 0px; position: fixed;" />
    </form>
</body>
</html>
