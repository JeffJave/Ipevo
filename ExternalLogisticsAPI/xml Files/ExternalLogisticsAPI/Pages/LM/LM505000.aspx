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
            <px:PXTextEdit runat="server" ID="edFinPeriod" DataField="FinPeriod" Width="200px"></px:PXTextEdit>
            <px:PXSelector runat="server" ID="edItemClassID" DataField="ItemClassID" Width="200px"></px:PXSelector>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="PrimaryInquire" AllowAutoHide="false">
        <Levels>
            <px:PXGridLevel DataMember="ImportPACList">
                <Columns>
                    <px:PXGridColumn DataField="Selected" Width="40" Type="CheckBox" TextAlign="Center" CommitChanges="True" AllowCheckAll="True"></px:PXGridColumn>
                    <px:PXGridColumn DataField="FinPeriodID"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Siteid"></px:PXGridColumn>
                    <px:PXGridColumn DataField="InventoryID"></px:PXGridColumn>
                    <px:PXGridColumn DataField="ItemClassID"></px:PXGridColumn>
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

    <script type="text/javascript">
        window.onload = function () {
            window.setTimeout(function () {
                document.getElementById("ctl00_phF_form_edFinPeriod").placeholder = "yyyyMM";
            }, 1000);
        };
    </script>
</asp:Content>
