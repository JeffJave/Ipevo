<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM505000.aspx.cs" Inherits="Pages_LM505000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="ExternalLogisticsAPI.Graph.LUMPACImportProc"
        PrimaryView="Filter">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="100px" AllowAutoHide="false">
        <Template>
            <px:PXTextEdit runat="server" ID="edFinPeriod" DataField="FinPeriod"></px:PXTextEdit>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="PrimaryInquire" AllowAutoHide="false">
        <Levels>
            <px:PXGridLevel DataMember="ImportPACList">
                <Columns>
                    <px:PXGridColumn DataField="FinPeriodID"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Siteid"></px:PXGridColumn>
                    <px:PXGridColumn DataField="InventoryID"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdQtySales"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Finptdcogs"></px:PXGridColumn>
                    <px:PXGridColumn DataField="PACUnitCost"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Paccogs"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Cogsadj"></px:PXGridColumn>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <ActionBar>
        </ActionBar>
    </px:PXGrid>

    <%--    <script type="text/javascript">
        window.onload = function () {
            window.setTimeout(function () {
                var Prepare = document.querySelectorAll('div[data-cmd="PrepareImport"]')[0].parentNode;
                var Import = document.querySelectorAll('div[data-cmd="Process"]')[0].parentNode;
                Import.parentNode.insertBefore(Prepare, Import);
            }, 100);
        };
    </script>--%>
</asp:Content>
