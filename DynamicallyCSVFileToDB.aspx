<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DynamicallyCSVFileToDB.aspx.cs" Inherits="CSVFileConvertDataBase.DynamicallyCSVFileToDB" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
       <div>
            <asp:FileUpload ID="FileUpload1" runat="server" />
            <asp:Button Text="Upload" OnClick="Upload" runat="server" />
        </div>
    </form>
</body>
</html>
