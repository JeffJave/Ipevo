<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM504000.aspx.cs" Inherits="Pages_LM504000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="ExternalLogisticsAPI.Graph.LUMP3PLImportProc"
        PrimaryView="ImportDataList">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="PrimaryInquire" AllowAutoHide="false">
        <Levels>
            <px:PXGridLevel DataMember="ImportDataList">
                <Columns>
                    <px:PXGridColumn DataField="Selected" Width="40" Type="CheckBox" TextAlign="Center" CommitChanges="True" AllowCheckAll="True"></px:PXGridColumn>
                    <px:PXGridColumn DataField="WarehouseOrder"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CustomerOrderRef"></px:PXGridColumn>
                    <px:PXGridColumn DataField="OrderStatus"></px:PXGridColumn>
                    <px:PXGridColumn DataField="UnitsSent"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Carrier"></px:PXGridColumn>
                    <px:PXGridColumn DataField="TrackingNumber"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FreightCost"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FreightCurrency"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FtpFileName"></px:PXGridColumn>
                    <px:PXGridColumn DataField="LUMP3PLImportProcessLog__IsProcess" />
                    <px:PXGridColumn DataField="LUMP3PLImportProcessLog__ProcessMessage" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <ActionBar>
        </ActionBar>
    </px:PXGrid>

    <script type="text/javascript">
        window.onload = function () {
            window.setTimeout(function () {
                var Prepare = document.querySelectorAll('div[data-cmd="PrepareImport"]')[0].parentNode;
                var Import = document.querySelectorAll('div[data-cmd="Process"]')[0].parentNode;
                Import.parentNode.insertBefore(Prepare, Import);
            }, 100);
        };
    </script>

</asp:Content>
