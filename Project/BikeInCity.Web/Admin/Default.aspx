<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BikeInCity.Web.Pages.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <table>
        <tr>
            <td>
                <asp:Button ID="Button1" OnClick="RegenerateDB_Click" Text="Generate DB" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="Button2" OnClick="Reinsert_Click" Text="Reinsert Cities" runat="server" />
            </td>
        </tr>
        <tr>
            <td><asp:Button OnClick="RemoveAllStations_Click" Text="Remove All Stations" runat="server" /></td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="Button3" OnClick="DownloadAllCities_Click" Text="Download All cities"
                    runat="server" />
            <tr>
                <td>
                    Scheduler state:
                    <asp:Label ID="lblSchedulerState" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnSchedulerStart" OnClick="SchedulerStart_Click" Text="Start Scheduler"
                        runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnSchedulerStop" OnClick="SchedulerStop_Click" Text="Stop Scheduler"
                        runat="server" />
                </td>
                
            </tr>
            <tr>
                <td><asp:Label ID="lblCityStatuses" runat="server" />
                </td>
            </tr>
    </table>
    <asp:TextBox ID="secretTextBox" Text="secret" Width="200" runat="server" />
    <asp:Button ID="btnJsonBackup" OnClick="BackupToJson_Click" Text="Backup to Json"
        runat="server" />
    <asp:Label ID="lblOutput" runat="server" />
    </form>
</body>
</html>
