<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM505003.aspx.cs" Inherits="Pages_LM505003" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="ExternalLogisticsAPI.Graph.LUMPACUnitCostHistoryProc"
        PrimaryView="Filter">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="100px" AllowAutoHide="false">
        <Template>
            <px:PXTextEdit runat="server" ID="edFinPeriod" DataField="FinPeriod" Width="200px" CommitChanges="true"></px:PXTextEdit>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="PrimaryInquire" AllowAutoHide="false">
        <Levels>
            <px:PXGridLevel DataMember="ImportHistoryList">
                <Columns>
                    <px:PXGridColumn DataField="FinPeriodID"></px:PXGridColumn>
                    <px:PXGridColumn DataField="InventoryID"></px:PXGridColumn>
                    <px:PXGridColumn DataField="ItemClassID"></px:PXGridColumn>
                    <px:PXGridColumn DataField="PACUnitCost"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinBegCost"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinBegQty"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Finptdcogs"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdCOGSCredits"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdCostAdjusted"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdCostAssemblyIn"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdCostAssemblyOut"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdCostIssued"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdCostReceived"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdCostTransferIn"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdCostTransferOut"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdQtyAdjusted"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdQtyAssemblyIn"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdQtyAssemblyOut"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdQtyCreditMemos"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdQtyDropShipSales"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdQtyIssued"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdQtyReceived"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdQtySales"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdQtyTransferIn"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPtdQtyTransferOut"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinYtdCost"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinYtdQty"></px:PXGridColumn>
                    <px:PXGridColumn DataField="TotalCostIN"></px:PXGridColumn>
                    <px:PXGridColumn DataField="TotalQtyIn"></px:PXGridColumn>
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
                document.getElementById("ctl00_phF_form_edFinPeriod").placeholder = "yyyyMM";
            }, 1000);
        };
    </script>
</asp:Content>
